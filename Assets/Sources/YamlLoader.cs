using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class YamlLoader
{

    /// <summary>
    /// Class statique contenant toutes les fonctions de regex pour la lecture de fichier YAML
    /// </summary>
    private static class PropertyRegex
    {

        /// <summary>
        /// Regex constants pour le YAML
        /// </summary>

        static readonly Regex rgxSpace = new Regex(@"^(\s)*");
        static readonly Regex rgxUseful = new Regex(@"^\s*[\-a-zA-Z_$0-9]");
        static readonly Regex rgxVariable = new Regex(@"^[a-zA-Z_$][\-a-zA-Z_$0-9]*");
        static readonly Regex rgxUInt = new Regex(@"^[0-9]+");
        static readonly Regex rgxFloat = new Regex(@"^[+-]?(([0-9]+(([eE][+-]?[0-9]+)|(\.([0-9]+([eE][+-]?[0-9]+)?)?))?)|(\.[0-9]+([eE][+-]?[0-9]+)?))");
        static readonly Regex rgxNotValue = new Regex(@"^(\s)*$");

        /// <summary>
        /// Permet de lire les flottants au format US (avec des "." et non des ",")
        /// </summary>
        private static readonly CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Détermine si la chaine de caractère contient un élément utile
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne true si c'est le cas et false sinon</returns>
        public static bool IsUseful(string s)
        {
            return rgxUseful.IsMatch(s);
        }

        /// <summary>
        /// Détermine si la chaine de caractère contient une variable
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne true si c'est le cas et false sinon</returns>
        public static bool IsVariable(string s)
        {
            return rgxVariable.IsMatch(s);
        }

        /// <summary>
        /// Détermine si la chaine de caractère contient un flottant
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne true si c'est le cas et false sinon</returns>
        private static bool IsFloat(string s)
        {
            return rgxFloat.IsMatch(s);
        }

        /// <summary>
        /// Détermine si la chaine de caractère contient une valeur
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne true si c'est le cas et false sinon</returns>
        public static bool IsValue(string s)
        {
            return !rgxNotValue.IsMatch(s);
        }

        /// <summary>
        /// Convertie une chaine de caractère en flottant
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne le flottant si réussi et NaN sinon</returns>
        private static float ConvertDouble(string s)
        {
            Thread.CurrentThread.CurrentCulture = usCulture;
            try
            {
                return float.Parse(s);
            }
            catch (FormatException)
            {
                return float.NaN;
            }
        }

        /// <summary>
        /// Indique le nombre d'espaces avant l'entrée à lire
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne le nombre d'espaces</returns>
        public static int GetSpaceLevel(string s)
        {
            return rgxSpace.Match(s).Length;
        }

        /// <summary>
        /// Détermine le nom de variable contenu dans la chaine
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne le nom si réussi et "" sinon</returns>
        public static string GetVariable(string s)
        {
            Match match = rgxVariable.Match(s);
            if (match.Success)
                return match.Value;
            else
                return "";
        }

        /// <summary>
        /// Détermine la valeur contenue dans la chaine (flottant ou chaine de caractères)
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne la valeur si réussi et null sinon</returns>
        public static object GetValue(string s)
        {
            if (IsValue(s))
            {
                s = s.Substring(1);
                s = s.Substring(GetSpaceLevel(s));
                if (IsFloat(s))
                    return ConvertDouble(s);
                else
                    return s;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Détermine la valeur de l'entier contenu dans la chaine
        /// </summary>
        /// <param name="s">chaine à lire</param>
        /// <returns>Retourne le nombre si réussi et 0 sinon</returns>
        public static int GetUint(string s)
        {
            Match match = rgxUInt.Match(s);
            if (match.Success)
                return int.Parse(match.Value);
            else
                return 0;
        }

    }

    /// <summary>
    /// Class permettant de lire et de stocker un élément
    /// </summary>
    public class PropertyElement
    {

        /// <summary>
        /// Nom de l'entrée
        /// </summary>
        private string name;
        
        /// <summary>
        /// Valeur de l'entrée
        /// </summary>
        private object value;

        /// <summary>
        /// Indique si l'entrée a été correctement lue
        /// </summary>
        private bool valid;

        /// <summary>
        /// Nom de l'entrée
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// Type de la valeur de l'entrée
        /// </summary>
        public string Type { get { return value.GetType().ToString(); } }
        /// <summary>
        /// Valeur de l'entrée
        /// </summary>
        public object Value { get { return value; } }

        /// <summary>
        /// Indique si l'entrée a été correctement lue (sert uniquement pour le constructeur de "PropertiesManager")
        /// </summary>
        public bool Valid { get { return valid; } }

        /// <summary>
        /// Indique si la valeur est un nombre (float)
        /// </summary>
        public bool IsNumber { get { return typeof(float).Equals(value.GetType()); } }
        /// <summary>
        /// Indique si la valeur est une chaine de caractère (string)
        /// </summary>
        public bool IsString { get { return typeof(string).Equals(value.GetType()); } }
        /// <summary>
        /// Indique si la valeur est une liste (List<PropertyElement>)
        /// </summary>
        public bool IsList { get { return typeof(List<PropertyElement>).Equals(value.GetType()); } }

        /// <summary>
        /// Floatting property constructor
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Value</param>
        public PropertyElement(string name, float value)
        {
            this.name = name;
            this.value = value;
            valid = true;
        }

        /// <summary>
        /// String property constructor
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Value</param>
        public PropertyElement(string name, string value)
        {
            this.name = name;
            this.value = value;
            valid = true;
        }

        /// <summary>
        /// List property constructor
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Value</param>
        public PropertyElement(string name, IEnumerable<PropertyElement> value)
        {
            this.name = name;
            this.value = new List<PropertyElement>(value);
            valid = true;
        }

        /// <summary>
        /// Constructeur de la class PropertyElement
        /// </summary>
        /// <param name="syntax">une structure FIFO contenant les lignes du fichier à lire</param>
        public PropertyElement(ref Queue<string> syntax)
        {
            // Initialisation
            valid = false;
            while (syntax.Count > 0 && !PropertyRegex.IsUseful(syntax.Peek()))
                syntax.Dequeue();
            if (syntax.Count == 0) return;

            // Assignation du niveau d'espacement et du nom
            string peek = syntax.Dequeue();
            var spaceLevel = PropertyRegex.GetSpaceLevel(peek);
            peek = peek.Substring(spaceLevel);
            if (!PropertyRegex.IsVariable(peek))
            {
                // C'est un élément de liste
                name = "";
            }
            else
                name = PropertyRegex.GetVariable(peek);

            // Assignation si présente de la valeur sur la ligne
            peek = peek.Substring(name.Length);
            peek = peek.Substring(PropertyRegex.GetSpaceLevel(peek) + 1);
            if (PropertyRegex.IsValue(peek))
            {
                value = PropertyRegex.GetValue(peek);
                valid = true;
                return;
            }

            // Vérification d'une valeur valide après
            while (syntax.Count > 0 && !PropertyRegex.IsUseful(syntax.Peek()))
                syntax.Dequeue();
            if (syntax.Count == 0) return;


            // On ajoute les éléments
            List<PropertyElement> elems = new List<PropertyElement>();
            do
            {
                peek = syntax.Peek();
                if (PropertyRegex.GetSpaceLevel(peek) > spaceLevel)
                {
                    var tmp = new PropertyElement(ref syntax);
                    if (!tmp.Valid) break;
                    elems.Add(tmp);
                    valid = true;
                }
                else
                {
                    break;
                }

                while (syntax.Count > 0 && !PropertyRegex.IsUseful(syntax.Peek()))
                    syntax.Dequeue();

            } while (syntax.Count > 0);

            // On met les éléments dans la valeur
            if (valid)
                value = elems;
        }

        /// <summary>
        /// Retourne l'élément identifié par la chaine
        /// </summary>
        /// <param name="ident">chaine identifiant</param>
        /// <returns>Retourne l'élément identifié si réussi et null sinon</returns>
        public PropertyElement GetElement(string ident)
        {
            if (ident == name) return this;
            if (ident.Length < name.Length) return null;

            string tmp = ident.Substring(0, name.Length);
            if (tmp != name) return null;

            ident = ident.Substring(name.Length);
            if (IsList)
            {
                if (ident[0] == '.')
                {
                    ident = ident.Substring(1);
                    string varName = PropertyRegex.GetVariable(ident);
                    foreach (PropertyElement e in (List<PropertyElement>)value)
                    {
                        if (e.Name == varName)
                            return e.GetElement(ident);
                    }
                    return null;
                }
                else if (ident[0] == '[')
                {
                    ident = ident.Substring(1);
                    int idx = PropertyRegex.GetUint(ident);
                    ident = ident.Substring((idx / 10) + 2);
                    int it = 0;
                    foreach (PropertyElement e in (List<PropertyElement>)value)
                    {
                        if (e.Name == "" && (it++) == idx)
                            return e.GetElement(ident);
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retourne les chaines identifiant tous les éléments
        /// </summary>
        /// <returns></returns>
        public string[] GetElements()
        {
            List<string> names = new List<string>();
            names.Add(name);
            if (IsList)
            {
                int it = 0;
                string[] tmp;
                foreach (PropertyElement e in (List<PropertyElement>)value)
                {
                    tmp = e.GetElements();
                    foreach (string s in tmp)
                    {
                        if (e.Name == "")
                            names.Add(name + "[" + it + "]" + s);
                        else
                            names.Add(name + "." + s);
                    }
                    if (e.Name == "")
                        it++;
                }
            }
            return names.ToArray();
        }

    }

    /// <summary>
    /// Liste des éléments du fichier de propriétés
    /// </summary>
    private List<PropertyElement> _e;
    
    /// <summary>
    /// Liste des éléments du fichier de propriétés
    /// </summary>
    public PropertyElement[] Elements { get { return _e.ToArray(); } }

    /// <summary>
    /// Constructeur de la class YamlLoader
    /// </summary>
    public YamlLoader()
    {
        _e = new List<PropertyElement>();
    }

    /// <summary>
    /// Constructeur de la class YamlLoader
    /// </summary>
    /// <param name="filename">Nom du fichier à charger</param>
    public YamlLoader(string filename)
    {
        _e = new List<PropertyElement>();
        Load(filename);
    }

    /// <summary>
    /// Charge un fichier YAML
    /// </summary>
    /// <param name="filename">Nom du fichier à charger</param>
    /// <returns>True si réussi et false sinon</returns>
    public bool Load(string filename)
    {
        string fileContent;
        this._e = new List<PropertyElement>();

        if (!System.IO.File.Exists(filename))
            return false;
        else
        {
            System.IO.StreamReader fd = System.IO.File.OpenText(filename);
            fileContent = fd.ReadToEnd();
            fd.Close();
        }

        Queue<string> q = new Queue<string>(fileContent.Split('\n'));
        while (q.Count > 0)
        {
            var tmp = new PropertyElement(ref q);
            if (tmp.Valid)
                this._e.Add(tmp);
        }
        return true;
    }

    /// <summary>
    /// Sauvegarde dans un fichier YAML
    /// </summary>
    /// <param name="filename">Nom du fichier à sauvegarder</param>
    /// <returns>True si réussi et false sinon</returns>
    public void Save(string filename)
    {
        using (System.IO.StreamWriter fd = new System.IO.StreamWriter(filename))
        {
            fd.WriteLine("### YamlLoader C# Class ###");
            fd.WriteLine();
            Stack<string> elements = new Stack<string>();
            List<string> swap = new List<string>();
            foreach (PropertyElement element in _e)
                swap.AddRange(element.GetElements());
            for (int i = swap.Count - 1; i >= 0; i--)
                elements.Push(swap[i]);
            while (elements.Count > 0)
            {
                string e = elements.Pop();
                string tmp = e;
                string identifier = "";
                if (e[e.Length - 1] == ']')
                {
                    string num = "";
                    for (int i = e.Length - 2; i >= 0 && e[i] != '['; i--)
                        num = e[i] + num;
                    e = e.Remove(e.Length - num.Length - 3);
                    identifier = "- ";
                }
                else
                {
                    for (int i = e.Length - 1; i >= 0 && e[i] != '.' && e[i] != ']'; i--)
                        identifier = e[i] + identifier;
                    e = e.Remove(e.Length - identifier.Length);
                    identifier += ": ";
                }
                string line = "";
                foreach (char c in e)
                {
                    if (c == '.' || c == '[')
                        line += "    ";
                }
                PropertyElement element = GetElement(tmp);
                fd.WriteLine(line + identifier + ((element.IsNumber || element.IsString) ? element.Value.ToString() : ""));
            }
        }
    }

    /// <summary>
    /// Détermine si l'élément identifié existe
    /// </summary>
    /// <param name="name">nom de l'élément à identifier</param>
    /// <returns>Retourne true s'il existe et false sinon</returns>
    public bool ElementExist(string name)
    {
        return GetElement(name) != null;
    }

    /// <summary>
    /// Ajoute un nouvel élément
    /// </summary>
    /// <param name="element">Elément à ajouter</param>
    public void AddElement(PropertyElement element)
    {
        _e.Add(element);
    }

    /// <summary>
    /// Permet de récupérer un élément par son nom
    /// </summary>
    /// <param name="name">nom de l'élément à identifier</param>
    /// <returns>Retourne l'élément s'il existe et null sinon</returns>
    public PropertyElement GetElement(string name)
    {
        string varName = PropertyRegex.GetVariable(name);
        foreach (PropertyElement e in this._e)
        {
            if (varName == e.Name)
                return (PropertyElement)e.GetElement(name);
        }
        return null;
    }

    /// <summary>
    /// Permet de récupérer la liste des noms de tous les éléments du fichier de propriétés
    /// </summary>
    /// <returns>Retourne la liste des noms</returns>
    public string[] GetElementNames()
    {
        List<string> names = new List<string>();
        foreach (PropertyElement e in this._e)
        {
            names.AddRange(e.GetElements());
        }
        return names.ToArray();
    }
    
}