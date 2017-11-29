using System;
using System.Collections;
using System.Collections.Generic;
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

            // Regex evaluators
            public static readonly Regex RgxBoolean = new Regex(@"^\s*(true|false)");
            public static readonly Regex RgxInteger = new Regex(@"^\s*(\-?[0-9]+)");
            public static readonly Regex RgxString = new Regex(@"^\s*'(\.*)'");
            public static readonly Regex RgxVariable = new Regex(@"^\s*([a-z][a-z0-9_$]*(\.[a-z][a-z0-9_$]*)?)");
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
                    _value = bool.Parse(match.Value.Replace(" ", ""));
                }
                else if ((match = RgxInteger.Match(s)).Success)
                {
                    _type = OperandType.INTEGER;
                    _value = int.Parse(match.Value.Replace(" ", ""));
                }
                else if ((match = RgxString.Match(s)).Success)
                {
                    _type = OperandType.STRING;
                    _value = match.Value.Substring(match.Groups[0].Index, match.Groups[0].Length);
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
            /// <returns></returns>
            public int Evaluate(string[][] names, Type[] types, object[] tuple)
            {
                if (!_valid) return 0;
                switch (_type)
                {
                    case OperandType.BOOLEAN: return ((bool)_value) ? 1 : 0;
                    case OperandType.INTEGER: return ((bool)_value) ? 1 : 0;
                    case OperandType.STRING: return 0;
                    case OperandType.VARIABLE:
                        for (int i = 0; i < names.Length; i++)
                        {
                            string[] n = (string[])_value;
                            if ((names[i][0] == "" && names[i][1] == n[1]) || (names[i][0] == n[0] && names[i][1] == n[1]))
                                return new Operand(tuple[i]).Evaluate(names, types, tuple);
                        }
                        return 0;
                    case OperandType.EXPRESSION: return ((Expression)_value).Evaluate(names, types, tuple);
                }
                return 0;
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
                    if (s[count + 1] == 'n' && s[count + 2] == 'd')
                        _op = OperatorType.AND;
                    else
                        _valid = false;
                    count += 2;
                    break;
                case 'o':
                    if (s[count + 1] == 'r')
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
            if (_op1.Type == Operand.OperandType.STRING || _op2.Type == Operand.OperandType.STRING)
            {
                if (_op1.Type != Operand.OperandType.STRING || _op2.Type != Operand.OperandType.STRING)
                    return 0;
                switch (_op)
                {
                    case OperatorType.EQUAL: return (string)_op1.Value == (string)_op2.Value ? 1 : 0;
                    case OperatorType.NOTEQUAL: return (string)_op1.Value != (string)_op2.Value ? 1 : 0;
                    case OperatorType.LESSER: return ((string)_op1.Value).CompareTo((string)_op2.Value);
                    case OperatorType.LESS_EQU: return ((string)_op1.Value).CompareTo((string)_op2.Value) + 1;
                    case OperatorType.GREATER: return -((string)_op1.Value).CompareTo((string)_op2.Value);
                    case OperatorType.GREAT_EQU: return -((string)_op1.Value).CompareTo((string)_op2.Value) - 1;
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
            str += _op1.Value.ToString();
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
            str += _op2.Value.ToString();
            return str + " )";
        }

    }

	/// <summary>
	/// Request regex patern									 SELECT   COLUMN 1            .ELEMENT 1               ,   COLUMN K            .ELEMENT K                FROM   TABLE 1               ,   TABLE N
	/// </summary>
	public static readonly Regex RequestPatern = new Regex(@"^\s*select\s+[a-z_$][a-z0-9_$]*(\.[a-z_$][a-z0-9_$]*)?(\s*,\s*[a-z_$][a-z0-9_$]*(\.[a-z_$][a-z0-9_$]*)?)*\s+from\s+[a-z_$][a-z0-9_$]*(\s*,\s*[a-z_$][a-z0-9_$])*(where\s+)?\s*$");

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
			fromPos = req.IndexOf("from"),
            wherePos = req.IndexOf("where");
		string[] selectNames = req.Substring(selectPos + 7, fromPos - selectPos - 8).Replace(" ", "").Split(',');
        string[] fromNames;
        if (wherePos != -1)
            fromNames = req.Substring(fromPos + 5).Replace(" ", "").Split(',');
        else
            fromNames = req.Substring(fromPos + 5, wherePos - fromPos - 1).Replace(" ", "").Split(',');
        Expression expr = null;
        if (wherePos != -1)
        {
            int c;
            expr = new Expression(req.Substring(wherePos + 6), out c);
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
        // A CONTINUER
	}

}
