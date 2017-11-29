using System;
using System.Collections;
using System.Collections.Generic;

public class Database {

	/////////////////
	/// ACCESSORS ///
	/////////////////

	// Table names list
	private Dictionary<string, Table> _tables = null;

	/////////////////
	/// ACCESSORS ///
	/////////////////

	/// <summary>
	/// Tables accessor
	/// </summary>
	public Dictionary<string, Table> Tables { get { return _tables; } }

	///////////////
	/// METHODS ///
	///////////////

	/// <summary>
	/// Database constructor
	/// </summary>
	public Database() {
		_tables = new Dictionary<string, Table> ();
	}

	/// <summary>
	/// Add a table in the database
	/// </summary>
	/// <param name="name">name</param>
	/// <param name="t">content</param>
	public void Add(string name, Table t) {
		_tables [name] = t;
	}

	/// <summary>
	/// Remove a table in the database
	/// </summary>
	/// <param name="name">name</param>
	public void Remove(string name) {
		_tables.Remove (name);
	}

	///////////////
	/// CLASSES ///
	///////////////

	public class Table {

		/////////////////
		/// DELEGATES ///
		/////////////////

		// Search delegate
		public delegate bool BoolDelegate(Dictionary<string, object> entry);

		// Search/modify delegate
		public delegate Dictionary<string, object> EntryDelegate(Dictionary<string, object> entry);

		//////////////////
		/// ATTRIBUTES ///
		//////////////////

		// Field names
		string[] fieldNames;

		// Field data types
		Type[] fieldTypes;

		// List of entries
		List<object[]> entries;

		/////////////////
		/// ACCESSORS ///
		/////////////////

		/// <summary>
		/// Field names accessor
		/// </summary>
		public string[] Names { get { return (string[])fieldNames.Clone(); } }
		
		/// <summary>
		/// Field data types
		/// </summary>
		public Type[] Types { get { return fieldTypes; } }

		/// <summary>
		/// List of entries
		/// </summary>
		public object[][] Entries { get { return entries.ToArray (); } }

		/// <summary>
		/// Entries count
		/// </summary>
		public int Count { get { return entries.Count; } }

		///////////////
		/// METHODS ///
		///////////////

		// Table constructor
		Table(string[] names, Type[] types) {
			fieldNames = (string[])names.Clone();
			fieldTypes = (Type[])types.Clone();
		}

		/// <summary>
		/// Create a table with specified fields
		/// </summary>
		/// <param name="names">Field names</param>
		/// <param name="types">Field data types</param>
		/// <remarks>Lists need same lengths</remarks>
		public static Table Create(string[] names, Type[] types) {
			if (names.Length != types.Length)
				return null;
			return new Table (names, types);
		}

		/// <summary>
		/// Insert an entry in the table
		/// </summary>
		/// <param name="entry">entry</param>
		/// <returns>True if success</returns>
		/// <remarks>Objects need same types and lengths than fields</remarks>
		public bool Insert(object[] entry) {
			if (entry.Length != fieldTypes.Length)
				return false;
			for (int i = 0; i < entry.Length; i++) {
				if (entry [i].GetType () != fieldTypes [i])
					return false;
			}
			entries.Add (entry);
			return true;
		}

		/// <summary>
		/// Remove entries that correspond to the search
		/// </summary>
		/// <param name="search">search delegate</param>
		public void Remove(BoolDelegate search) {
			Dictionary<string, object> entry = new Dictionary<string, object> ();
			for (int i = entries.Count - 1; i >= 0; i--) {
				for (int j = 0; j < fieldNames.Length; j++)
					entry [fieldNames [j]] = entries [i] [j];
				if (search (entry))
					entries.RemoveAt (i);
			}
		}

		/// <summary>
		/// Modify entries that correspond to search
		/// </summary>
		/// <param name="search">search delegate</param>
		/// <param name="alter">alter delegate</param>
		public void Modify(BoolDelegate search, EntryDelegate alter) {
			Dictionary<string, object> entry = new Dictionary<string, object> ();
			for (int i = 0; i < entries.Count; i++) {
				for (int j = 0; j < fieldNames.Length; j++)
					entry [fieldNames [j]] = entries [i] [j];
				if (search (entry)) {
					entry = alter (entry);
					for (int j = 0; j < fieldNames.Length; j++) {
						if (entry [fieldNames [j]].GetType() == fieldTypes [j])
							entries [i] [j] = entry [fieldNames [j]];
					}
				}
			}
		}

		/// <summary>
		/// Return entries that correspond to search
		/// </summary>
		/// <param name="search">search delegate</param>
		public object[][] Search(BoolDelegate search) {
			List<object[]> res = new List<object[]> ();
			Dictionary<string, object> entry = new Dictionary<string, object> ();
			for (int i = 0; i < entries.Count; i++) {
				for (int j = 0; j < fieldNames.Length; j++)
					entry [fieldNames [j]] = entries [i] [j];
				if (search (entry))
					res.Add ((object[])entries [i].Clone ());
			}
			return res.ToArray ();
		}

	}

}
