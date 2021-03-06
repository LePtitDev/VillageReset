﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
//using System.Runtime.Serialization.Configuration;
using System.Text.RegularExpressions;
using UnityEngine;

public class Memory : MonoBehaviour {

	/// <summary>
	/// Request answer
	/// </summary>
	public class Answer
	{
		
		// Columns names
		private string[][] _names;

		// Columns type
		private Type[] _types;

		// Columns contents
		private object[][] _columns;
		
		/// <summary>
		/// Columns names
		/// </summary>
		public string[][] Names { get { return _names; } }

		/// <summary>
		/// Columns types
		/// </summary>
		public Type[] Types { get { return _types; } }
		
		/// <summary>
		/// Columns contents
		/// </summary>
		public object[][] Columns { get { return _columns; } }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="names">Columns names</param>
		/// <param name="types">Columns types</param>
		/// <param name="columns">Columns contents</param>
		public Answer(string[][] names, Type[] types, object[][] columns)
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

            // Regex evaluators
            public static readonly Regex RgxBoolean = new Regex(@"^\s*(true|false|TRUE|FALSE)");
            public static readonly Regex RgxInteger = new Regex(@"^\s*(\-?[0-9]+)");
            public static readonly Regex RgxString = new Regex("^\\s*\"(.*)\"");
            public static readonly Regex RgxVariable = new Regex(@"^\s*([a-zA-Z][a-zA-Z0-9_$]*(\.[a-zA-Z][a-zA-Z0-9_$]*)?)");
            public static readonly Regex RgxExpression = new Regex(@"^\s*\((\.+)\)");

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

            // Indicate if operand is valid
            private bool _valid;

			/// <summary>
			/// Operand type
			/// </summary>
			public OperandType Type { get { return _type; } }

			/// <summary>
			/// Operand value
			/// </summary>
			public object Value { get { return _value; } }

            /// <summary>
            /// Indicate if operand is valid
            /// </summary>
            public bool Valid { get { return _valid; } }

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="s">Code to eval</param>
            /// <param name="count"></param>
			public Operand(string s, out int count)
			{
                count = 0;
                Match match;
                if ((match = RgxBoolean.Match(s)).Success)
                {
                    _type = OperandType.BOOLEAN;
                    _value = bool.Parse(match.Value.Replace(" ", "").ToLower());
                }
                else if ((match = RgxInteger.Match(s)).Success)
                {
                    _type = OperandType.INTEGER;
                    _value = int.Parse(match.Value.Replace(" ", ""));
                }
                else if ((match = RgxString.Match(s)).Success)
                {
                    _type = OperandType.STRING;
                    _value = match.Value.Substring(match.Groups[1].Index, match.Groups[1].Length);
                }
                else if ((match = RgxVariable.Match(s)).Success)
                {
                    _type = OperandType.VARIABLE;
                    _value = match.Value.Substring(match.Groups[0].Index, match.Groups[0].Length).Split('.');
                    if (((string[])_value).Length == 1)
                        _value = new string[2] { "", ((string[])_value)[0] };
                }
                else if ((match = RgxExpression.Match(s)).Success)
                {
                    _type = OperandType.EXPRESSION;
                    _value = new Expression(match.Value.Substring(match.Groups[0].Index, match.Groups[0].Length), out count);
                    if (!((Expression)_value).Valid)
                    {
                        _valid = false;
                        return;
                    }
                }
                else
                {
                    _valid = false;
                    return;
                }
                count = match.Length;
                _valid = true;
            }

            /// <summary>
            /// Contructor for pre-created expression
            /// </summary>
            /// <param name="expr">Expression</param>
            public Operand(Expression expr)
            {
                _valid = true;
                _type = OperandType.EXPRESSION;
                _value = expr;
            }

            /// <summary>
            /// Constructor for object
            /// </summary>
            /// <param name="obj">Object</param>
            public Operand(object obj)
            {
                _valid = true;
                Type type = obj.GetType();
                if (type == typeof(bool))
                    _type = OperandType.BOOLEAN;
                else if (type == typeof(int))
                    _type = OperandType.INTEGER;
                else if (type == typeof(string))
                    _type = OperandType.STRING;
                else
                    _valid = false;
                _value = obj;
            }

            /// <summary>
            /// Evaluate integer parsable value
            /// </summary>
            /// <param name="names">Names</param>
            /// <param name="types">Types</param>
            /// <param name="tuple">Current tuple</param>
            /// <returns>The integer</returns>
            public int Evaluate(string[][] names, Type[] types, object[] tuple)
            {
                if (!_valid) return 0;
                switch (_type)
                {
                    case OperandType.BOOLEAN: return ((bool)_value) ? 1 : 0;
                    case OperandType.INTEGER: return (int)_value;
                    case OperandType.STRING: return 0;
                    case OperandType.VARIABLE:
                        for (int i = 0; i < names.Length; i++)
                        {
                            string[] n = (string[])_value;
                            if ((n[0] == "" && names[i][1] == n[1]) || (names[i][0] == n[0] && names[i][1] == n[1]))
                                return new Operand(tuple[i]).Evaluate(names, types, tuple);
                        }
                        return 0;
                    case OperandType.EXPRESSION: return ((Expression)_value).Evaluate(names, types, tuple);
                }
                return 0;
            }

			/// <summary>
			/// Evaluate string parsable value
			/// </summary>
			/// <param name="names">Names</param>
			/// <param name="types">Types</param>
			/// <param name="tuple">Current tuple</param>
			/// <returns>The string if success and &lt;null&gt; otherwise</returns>
			public string EvalString(string[][] names, Type[] types, object[] tuple)
			{
				if (_type == OperandType.STRING)
					return (string) _value;
				if (_type == OperandType.VARIABLE)
				{
					for (int i = 0; i < names.Length; i++)
					{
						string[] n = (string[])_value;
						if ((n[0] == "" && names[i][1] == n[1]) || (names[i][0] == n[0] && names[i][1] == n[1]))
							return new Operand(tuple[i]).EvalString(names, types, tuple);
					}
				}
				return "<null>";
			}

		}

		/// <summary>
		/// Operator type enumerator
		/// </summary>
		public enum OperatorType
		{
			ADDITION,
			SUBTRACT,
			MULTIPLY,
			DIVIDE,
			MODULO,
			EQUAL,
			NOTEQUAL,
			LESSER,
			GREATER,
			LESS_EQU,
			GREAT_EQU,
            AND,
            OR
		}

        // Operands
        private Operand _op1, _op2;

        // Operator
        private OperatorType _op;

        // Indicate if operator is valid
        private bool _valid;

        /// <summary>
        /// Indicate if expression is valid
        /// </summary>
        public bool Valid { get { return _valid; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">String to evaluate</param>
        /// <param name="count">Characters count readed</param>
        public Expression(string s, out int count)
        {
            _valid = true;
            count = 0;
            _op1 = new Operand(s, out count);
            if (!_op1.Valid)
            {
                _valid = false;
                count = 0;
                return;
            }
            if (count >= s.Length)
            {
                _valid = false;
                count = 0;
                return;
            }
            while (s[count] == ' ') count++;
            if (count >= s.Length)
            {
                _valid = false;
                count = 0;
                return;
            }
            switch (s[count])
            {
                case '+': _op = OperatorType.ADDITION; break;
                case '-': _op = OperatorType.SUBTRACT; break;
                case '*': _op = OperatorType.MULTIPLY; break;
                case '/': _op = OperatorType.DIVIDE; break;
                case '%': _op = OperatorType.MODULO; break;
                case '=': _op = OperatorType.EQUAL; break;
                case '!':
                    if (s[count+1] == '=')
                        _op = OperatorType.NOTEQUAL;
                    else
                        _valid = false;
                    count++;
                    break;
                case '<':
                    if (s[count + 1] == '=')
                    {
                        _op = OperatorType.LESS_EQU;
                        count++;
                    }
                    else
                        _op = OperatorType.LESSER;
                    break;
                case '>':
                    if (s[count + 1] == '=')
                    {
                        _op = OperatorType.GREAT_EQU;
                        count++;
                    }
                    else
                        _op = OperatorType.GREATER;
                    break;
                case 'a':
                case 'A':
                    if ((s[count + 1] == 'n' || s[count + 1] == 'N') && (s[count + 2] == 'd' || s[count + 2] == 'D'))
                        _op = OperatorType.AND;
                    else
                        _valid = false;
                    count += 2;
                    break;
                case 'o':
				case 'O':
                    if (s[count + 1] == 'r' || s[count + 1] == 'R')
                        _op = OperatorType.OR;
                    else
                        _valid = false;
                    count++;
                    break;
                default:
                    _valid = false;
                    break;
            }
            if (!_valid)
            {
                count = 0;
                return;
            }
            count++;
            int nextCount = 0;
            Expression nextExpr = new Expression(s.Substring(count), out nextCount);
            if (nextExpr.Valid)
            {
                count += nextCount;
                if (nextExpr.Priority() > Priority())
                {
                    _op2 = new Operand(nextExpr);
                }
                else
                {
                    OperatorType ot = nextExpr._op;
                    nextExpr._op = _op;
                    _op = ot;
                    _op2 = nextExpr._op2;
                    nextExpr._op2 = nextExpr._op1;
                    nextExpr._op1 = _op1;
                    _op1 = new Operand(nextExpr);
                }
            }
            else
            {
                nextCount = 0;
                _op2 = new Operand(s.Substring(count), out nextCount);
                if (!_op2.Valid)
                {
                    _valid = false;
                    count = 0;
                }
            }
        }

		public int Evaluate(string[][] names, Type[] types, object[] tuple)
		{
            if (!_valid)
                return 0;
			string so1 = _op1.EvalString(names, types, tuple);
			string so2 = _op2.EvalString(names, types, tuple);
            if (so1 != "<null>" || so2 != "<null>")
            {
                if (so1 == "<null>" || so2 == "<null>")
                    return 0;
                switch (_op)
                {
                    case OperatorType.EQUAL: return so1 == so2 ? 1 : 0;
                    case OperatorType.NOTEQUAL: return so1 != so2 ? 1 : 0;
                    case OperatorType.LESSER: return so1.CompareTo(so2) < 0 ? 1 : 0;
                    case OperatorType.LESS_EQU: return so1.CompareTo(so2) <= 0 ? 1 : 0;
                    case OperatorType.GREATER: return -so1.CompareTo(so2) > 0 ? 1 : 0;
                    case OperatorType.GREAT_EQU: return -so1.CompareTo(so2) >= 0 ? 1 : 0;
                    default: return 0;
                }
            }
            int o1 = _op1.Evaluate(names, types, tuple);
            int o2 = _op2.Evaluate(names, types, tuple);
            switch (_op)
            {
                case OperatorType.ADDITION: return o1 + o2;
                case OperatorType.SUBTRACT: return o1 - o2;
                case OperatorType.MULTIPLY: return o1 * o2;
                case OperatorType.DIVIDE: return o1 / o2;
                case OperatorType.MODULO: return o1 % o2;
                case OperatorType.EQUAL: return o1 == o2 ? 1 : 0;
                case OperatorType.NOTEQUAL: return o1 != o2 ? 1 : 0;
                case OperatorType.LESSER: return o1 < o2 ? 1 : 0;
                case OperatorType.LESS_EQU: return o1 <= o2 ? 1 : 0;
                case OperatorType.GREATER: return o1 > o2 ? 1 : 0;
                case OperatorType.GREAT_EQU: return o1 >= o2 ? 1 : 0;
                case OperatorType.AND: return o1 != 0 && o2 != 0 ? 1 : 0;
                case OperatorType.OR: return o1 != 0 || o2 != 0 ? 1 : 0;
            }
            return 0;
        }

        /// <summary>
        /// Get operator priority
        /// </summary>
        /// <returns>The operator priority</returns>
        public int Priority()
        {
            if (!_valid) return 0;
            switch (_op)
            {
                case OperatorType.ADDITION:
                case OperatorType.SUBTRACT:
                    return 5;
                case OperatorType.MULTIPLY:
                case OperatorType.DIVIDE:
                case OperatorType.MODULO:
                    return 6;
                case OperatorType.EQUAL:
                case OperatorType.NOTEQUAL:
                    return 3;
                case OperatorType.LESSER:
                case OperatorType.LESS_EQU:
                case OperatorType.GREATER:
                case OperatorType.GREAT_EQU:
                    return 4;
                case OperatorType.AND:
                    return 2;
                case OperatorType.OR:
                    return 1;
                default:
                    return 0;
            }
        }

        public override string ToString()
        {
            if (!_valid)
                return "NOT VALID";
            string str = "( ";
	        if (_op1.Type != Operand.OperandType.VARIABLE)
		        str += (_op1.Type == Operand.OperandType.STRING ? "\"" : "") + _op1.Value.ToString() + (_op1.Type == Operand.OperandType.STRING ? "\"" : "");
	        else
		        str += (((string[]) _op1.Value)[0] != "" ? ((string[]) _op1.Value)[0] + "." : "") + ((string[]) _op1.Value)[1];
            switch (_op)
            {
                case OperatorType.ADDITION: str += " + "; break;
                case OperatorType.SUBTRACT: str += " - "; break;
                case OperatorType.MULTIPLY: str += " * "; break;
                case OperatorType.DIVIDE: str += " / "; break;
                case OperatorType.MODULO: str += " % "; break;
                case OperatorType.EQUAL: str += " = "; break;
                case OperatorType.NOTEQUAL: str += " != "; break;
                case OperatorType.LESSER: str += " < "; break;
                case OperatorType.LESS_EQU: str += " >= "; break;
                case OperatorType.GREATER: str += " > "; break;
                case OperatorType.GREAT_EQU: str += " >= "; break;
                case OperatorType.AND: str += " AND "; break;
                case OperatorType.OR: str += " OR "; break;
            }
	        if (_op2.Type != Operand.OperandType.VARIABLE)
		        str += (_op2.Type == Operand.OperandType.STRING ? "\"" : "") + _op2.Value.ToString() + (_op2.Type == Operand.OperandType.STRING ? "\"" : "");
	        else
		        str += (((string[]) _op2.Value)[0] != "" ? ((string[]) _op2.Value)[0] + "." : "") + ((string[]) _op2.Value)[1];
            return str + " )";
        }

    }

	/// <summary>
	/// Request regex patern									 SELECT   COLUMN 1            .ELEMENT 1               ,   COLUMN K            .ELEMENT K                FROM   TABLE 1               ,   TABLE N
	/// </summary>
	public static readonly Regex RequestPatern = new Regex(@"^\s*select\s+[a-z_$][a-z0-9_$]*(\.[a-z_$][a-z0-9_$]*)?(\s*,\s*[a-z_$][a-z0-9_$]*(\.[a-z_$][a-z0-9_$]*)?)*\s+from\s+[a-z_$][a-z0-9_$]*(\s*,\s*[a-z_$][a-z0-9_$])*(where\s+)?\s*");

	// Memory database
	private Database _db = null;

    // Agent controller
    private AgentController _agent;

	/// Database accessor
	public Database DB { get { return _db; } }

	// Use this for initialization
	private void Start () {
        _agent = GetComponent<AgentController>();
		_db = new Database ();
		_db.Add("Patch", new Database.Table(new string[]
		{
			"Type", "Above", "PosX", "PosZ", "LastUpdate"
		}, new Type[]
		{
			typeof(bool), typeof(string), typeof(int), typeof(int), typeof(float)
		}));
        int mapWidth = Manager.Instance.GetComponent<Manager>().Width;
        int mapHeight = Manager.Instance.GetComponent<Manager>().Height;
        Database.Table t = _db.Tables["Patch"];
        foreach (Patch patch in GameObject.Find("Ground").GetComponentsInChildren<Patch>())
        {
            t.Insert(new object[] {
                patch.name == "Grass(Clone)", "None", (int)patch.transform.position.x, (int)patch.transform.position.z, 0f
            });
        }
    }

    // Update is called once per frame
    private void Update()
    {
        int perc = (int)_agent.PerceptionRadius;
        for (int i = (int)transform.position.x - perc, _i = (int)transform.position.x + perc; i <= _i; i++)
        {
            for (int j = (int)transform.position.z - perc, _j = (int)transform.position.z + perc; j <= _j; j++)
            {
                if (i < 0 || i >= Manager.Instance.Width || j < 0 || j >= Manager.Instance.Height)
                    continue;
                object[] tuple = GetPatch(i, j);
                Patch patch = Patch.GetPatch(i, j).GetComponent<Patch>();
                Entity en = patch.InnerObjects.Length == 0 ? null : patch.InnerObjects[0] == null ? null : patch.InnerObjects[0].GetComponent<Entity>();
                tuple[1] =  en == null ? "None" : en.Name;
                tuple[4] = Time.time;
            }
        }
	    foreach (Entity en in _agent.Percepts)
	    {
		    if (en != null && en.Type == Entity.EntityType.VILLAGER)
			    Syncronize(en.GetComponent<Memory>());
	    }

    }

    // Get a patch tuple
    private object[] GetPatch(int x, int z)
    {
        Database.Table t = _db.Tables["Patch"];
        foreach (object[] tuple in t.Entries)
        {
            if ((int)tuple[2] == x && (int)tuple[3] == z)
                return tuple;
        }
        return null;
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
			fromPos = req.IndexOf("from"),
            wherePos = req.IndexOf("where");
		string[] selectNames = request.Substring(selectPos + 7, fromPos - selectPos - 8).Replace(" ", "").Split(',');
        string[] fromNames;
        if (wherePos == -1)
            fromNames = request.Substring(fromPos + 5).Replace(" ", "").Split(',');
        else
            fromNames = request.Substring(fromPos + 5, wherePos - fromPos - 6).Replace(" ", "").Split(',');
        Expression expr = null;
        if (wherePos != -1)
        {
            int c;
            expr = new Expression(request.Substring(wherePos + 6), out c);
            if (!expr.Valid)
                expr = null;
        }
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
				for (int j = 0, _j = t.Names.Length; j < _j; j++)
				{
					if (selections[i, 1] != t.Names[j])
						continue;
					names.Add(new string[2] { table, selections[i, 1] });
					types.Add(t.Types[j]);
					List<object> col = new List<object>();
					for (int k = 0; k < t.Count; k++)
						col.Add(t.Entries[k][j]);
					objects[objects.Count - 1].Add(col.ToArray());
				}
			}
		}

		for (int i = objects.Count - 1; i >= 0; i--)
		{
			if (objects[i].Count == 0 || objects[i][0].Length == 0)
				objects.RemoveAt(i);
		}
		if (objects.Count == 0)
			return new Answer(new string[][] {}, new Type[0], new object[][] {});
		string[][] anames = names.ToArray();
		Type[] atypes = types.ToArray();
		object[] tuple = new object[anames.Length];
		for (int i = 0, k = 0; i < objects.Count; i++)
		{
			for (int j = 0; j < objects[i].Count; j++, k++)
				tuple[k] = objects[i][j][0];
		}
		int[] its = new int[objects.Count];
		for (int i = 0; i < its.Length; i++)
			its[i] = 0;
		List<object[]> tuples = new List<object[]>();
		while (its[0] < objects[0][0].Length)
		{
			if (expr == null || expr.Evaluate(anames, atypes, tuple) != 0)
				tuples.Add(tuple);
			for (int i = objects.Count - 1; i >= 0; i--)
			{
				if (++its[i] < objects[i][0].Length)
					break;
				if (i != 0) its[i] = 0;
			}
			if (its[0] >= objects[0][0].Length)
				break;
			tuple = new object[anames.Length];
			for (int i = 0, k = 0; i < objects.Count; i++)
			{
				for (int j = 0; j < objects[i].Count; j++, k++)
					tuple[k] = objects[i][j][its[i]];
			}
		}
		return new Answer(anames, atypes, tuples.ToArray());
	}

	/// <summary>
	/// Syncronize memories
	/// </summary>
	/// <param name="other">Other memory</param>
	public void Syncronize(Memory other)
	{
		foreach (KeyValuePair<string, Database.Table> pair in _db.Tables)
		{
			Database.Table t = other._db.Tables[pair.Key];
			if (t == null) continue;
			int updt = new List<string>(t.Names).IndexOf("LastUpdate");
			if (updt == -1) continue;
			object[][] en1 = pair.Value.Entries;
			object[][] en2 = t.Entries;
			for (int i = 0; i < en1.Length && i < en2.Length; i++)
			{
				object[] tuple1 = en1[i];
				object[] tuple2 = en2[i];
				if ((float) tuple1[updt] < (float) tuple2[updt])
				{
					for (int j = 0; j < tuple1.Length; j++)
						tuple1[j] = tuple2[j];
				}
				else
				{
					for (int j = 0; j < tuple1.Length; j++)
						tuple2[j] = tuple1[j];
				}
			}
		}
	}

}
