using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Configuration;
using System.Text.RegularExpressions;
using UnityEngine;

public class Memory : MonoBehaviour {

	/// <summary>
	/// Request answer
	/// </summary>
	public class Answer
	{
		
		// Columns names
		private string[,] _names;

		// Columns type
		private Type[] _types;

		// Columns contents
		private object[,] _columns;
		
		/// <summary>
		/// Columns names
		/// </summary>
		public string[,] Names { get { return _names; } }

		/// <summary>
		/// Columns types
		/// </summary>
		public Type[] Types { get { return _types; } }
		
		/// <summary>
		/// Columns contents
		/// </summary>
		public object[,] Columns { get { return _columns; } }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="names">Columns names</param>
		/// <param name="types">Columns types</param>
		/// <param name="columns">Columns contents</param>
		public Answer(string[,] names, Type[] types, object[,] columns)
		{
			_names = names;
			_types = types;
			_columns = columns;
		}
		
	}

	/// <summary>
	/// Evaluable expression
	/// </summary>
	public class Expression
	{

		/// <summary>
		/// Expression operand
		/// </summary>
		public class Operand
		{

			/// <summary>
			/// Operand type enumerator
			/// </summary>
			public enum OperandType
			{
				BOOLEAN,
				INTEGER,
				STRING,
				VARIABLE,
				EXPRESSION
			}

			// Operand type
			private OperandType _type;

			// Operand value
			private object _value;

			/// <summary>
			/// Operand type
			/// </summary>
			public Type Type;

			/// <summary>
			/// Operand value
			/// </summary>
			public object Value;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="s">Code to eval</param>
			public Operand(string s)
			{
				
			}

		}

		/// <summary>
		/// Operator type enumerator
		/// </summary>
		public enum OperatorType
		{
			ADDITION,
			SUBSTRACT,
			MULTIPLY,
			DIVIDE,
			MODULO,
			EQUAL,
			NOTEQUAL,
			LESSER,
			UPPER,
			LESS_EQU,
			UPP_EQU
		}

		public int Evaluate(string[][] names, Type types, object[] tuple)
		{
			return 0;
		}
		
	}

	/// <summary>
	/// Request regex patern									 SELECT   COLUMN 1            .ELEMENT 1               ,   COLUMN K            .ELEMENT K                FROM   TABLE 1               ,   TABLE N
	/// </summary>
	public static readonly Regex RequestPatern = new Regex(@"^\s*select\s+[a-z_$][a-z0-9_$]*(\.[a-z_$][a-z0-9_$]*)?(\s*,\s*[a-z_$][a-z0-9_$]*(\.[a-z_$][a-z0-9_$]*)?)*\s+from\s+[a-z_$][a-z0-9_$]*(\s*,\s*[a-z_$][a-z0-9_$])*\s*$");

	// Memory database
	private Database _db = null;

	/// Database accessor
	public Database DB { get { return _db; } }

	// Use this for initialization
	private void Start () {
		_db = new Database ();
	}

	/// <summary>
	/// Request informations in memory
	/// </summary>
	/// <param name="request">The formated request</param>
	/// <returns>The request answer if success and null otherwise</returns>
	public Answer Request(string request)
	{
		string req = request.ToLower();
		if (!RequestPatern.Match(req).Success)
			return null;
		int selectPos = req.IndexOf("select"),
			fromPos = req.IndexOf("from");
		string[] selectNames = req.Substring(selectPos + 7, fromPos - selectPos - 8).Replace(" ", "").Split(',');
		string[] fromNames = req.Substring(fromPos + 5).Replace(" ", "").Split(',');
		string[,] selections = new string[selectNames.Length,2];
		for (int i = 0; i < selectNames.Length; i++)
		{
			string[] s = selectNames[i].Split('.');
			if (s.Length == 1)
			{
				selections[i, 0] = "";
				selections[i, 1] = selectNames[i];
			}
			else
			{
				selections[i, 0] = s[0];
				selections[i, 1] = s[1];
			}
		}
		List<string[]> names = new List<string[]>();
		List<Type> types = new List<Type>();
		List<List<object[]>> objects = new List<List<object[]>>();
		foreach (string table in fromNames)
		{
			if (!_db.Tables.ContainsKey(table))
				continue;
			Database.Table t = _db.Tables[table];
			objects.Add(new List<object[]>());
			for (int i = 0, sz = selections.GetLength(0); i < sz; i++)
			{
				if (selections[i, 0] != "" && selections[i, 0] != table)
					continue;
				for (int j = 0, _j = t.Count; j < _j; j++)
				{
					if (selections[i, 0] != t.Names[j])
						continue;
					names.Add(new string[2] { table, selections[i, 1] });
					types.Add(t.Types[j]);
					objects[objects.Count - 1].Add(t.Entries[j]);
				}
			}
		}
		return null;
	}

}
