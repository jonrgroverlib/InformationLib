//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using InformationLib.Endemes;         // for Throw
using InformationLib.Strings;         // for PathSlicer(1)
using System;                         // for NotImplementedException
using System.Collections.Generic;     // for Dictionary, KeyNotFoundException
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for XmlNode, XmlDocument

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- SimpleSchema -->
    /// <summary>
    ///      The SimpleSchema class contains information about a schema specifically lists of
    ///      elements of various types, lists of namespaces, lists of imports
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public class SimpleSchema : XmlDocument
    {
        private Dictionary<string,string> _xmlns;  // namespace--prefix
        private Dictionary<string,PathSlicer> _imports; // namespace--filename
        private static string _baseLocation;
        private string SetBaseLocation { set { _baseLocation = value; } }
        public  string BaseLocation    { get { return _baseLocation;  } }


        public XmlDocument XmlDocument { get { return this; } }
        

        // ----------------------------------------------------------------------------------------
        /// <!-- _FileName -->
        private string _fileName;
        /// <summary>
        ///     The name of the file in which the schema is found
        /// </summary>
        public string _FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
	

        // ----------------------------------------------------------------------------------------
        //  Various dictionaries of nodes
        // ----------------------------------------------------------------------------------------
        private Dictionary<string, XmlNode> _complexTypes;
        private Dictionary<string, XmlNode> _simpleTypes;
        private Dictionary<string, XmlNode> _otherTypes;
        private Dictionary<string, XmlNode> _elements;
        private Dictionary<string, XmlNode> _abstract;
        private Dictionary<string, bool> _usedRef;


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public SimpleSchema()
        {
            Init();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            _complexTypes = new Dictionary<string,XmlNode>();
            _simpleTypes = new Dictionary<string,XmlNode>();
            _otherTypes = new Dictionary<string,XmlNode>();
            _elements = new Dictionary<string, XmlNode>();
            _abstract = new Dictionary<string,XmlNode>();
            _imports = new Dictionary<string,PathSlicer>();
            _usedRef = new Dictionary<string,bool>();
            _xmlns = new Dictionary<string, string>();


            _Clear();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _AttributeValue -->
        /// <summary>
        ///      Gets the value of a specified attribute if there is one
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string _AttributeValue(XmlNode node, string attributeName)
        {
            if (node == null) return "";


            XmlAttributeCollection attributes = node.Attributes;
            string attr = "";


            // --------------------------------------------------------------------------
            //  Returns the value of the specified attribute if it exists in the list
            // --------------------------------------------------------------------------
            if (attributes != null)
                foreach (XmlAttribute attribute in attributes)
                    if (attribute.Name == attributeName)
                        attr = attribute.Value;
            return attr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _ChildSchemaFileName -->
        /// <summary>
        ///      Returns the file name of the child schema identified by the type if it finds it
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string _ChildSchemaName(string type)
        {
            string fileName;
            string nameSpace;


            // --------------------------------------------------------------------------
            //  Do stuff
            // --------------------------------------------------------------------------
            nameSpace = _NameSpace(type);
            if (_imports.ContainsKey(nameSpace))
                fileName = _imports[nameSpace].Name;
            else
                fileName = _fileName;


            return fileName;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Clear -->
        /// <summary>
        /// 
        /// </summary>
        private void _Clear()
        {
            _xmlns.Clear();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _ComplexNode -->
        /// <summary>
        ///      Returns the node of this type if it is complex
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RichXmlNode _ComplexNode(string type)
        {
            if (_IsComplexType(type))
                return (new RichXmlNode(_complexTypes[type]));
            type = Regex.Replace(type, "^.*:", "");
            if (_IsComplexType(type))
                return (new RichXmlNode(_complexTypes[type]));
            else return null;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _FindChild -->
        /// <summary>
        ///      Gets the child node with a particular tag
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="nodeTag"></param>
        /// <returns></returns>
        private XmlNode _FindChild(XmlNode node, string nodeTag)
        {
            XmlNode foundNode = null;
            foreach (XmlNode childNode in node.ChildNodes)
                if (_Tag(childNode) == nodeTag)
             // if (childNode.Name == nodeTag)
                    foundNode = childNode;
            return foundNode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _GetElement -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public RichXmlNode _GetElement(string reference)
        {
            XmlNode node = null;
            if (_elements.ContainsKey(reference))
                node = _elements[reference];
            reference = Regex.Replace(reference, "^[a-z]+:", "");
            if (_elements.ContainsKey(reference))
                node = _elements[reference];
            if (node == null)
                __.BreakPoint();
            return (new RichXmlNode(node));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _IsElement -->
        /// <summary>
        ///      Determines whether the type indicated is in the schema's element dictionary
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool _IsElement(string type)
        {
            return _elements.ContainsKey(type);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _IsComplexType -->
        /// <summary>
        ///      Determines whether the type indicated is in the schema's complex types dictionary
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool _IsComplexType(string type)
        {
            return _complexTypes.ContainsKey(type);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _IsSimpleType -->
        /// <summary>
        ///      Determines whether the type indicated is in the schema's simple types dictionary
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool _IsSimpleType(string type)
        {
            return _simpleTypes.ContainsKey(type);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _IsUsed -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool _IsUsed(string reference)
        {
            return _usedRef.ContainsKey(reference) && _usedRef[reference];
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Load_simple -->
        /// <summary>
        ///      A load that tries a few places to load an xml schema from
        /// </summary>
        /// <param name="newXml"></param>
        /// <param name="schemaLocation"></param>
        /// <returns></returns>
        public string _Load_simple(string schemaLocation, string hint1, string hint2, string hint3)
        {
            if (Regex.IsMatch(schemaLocation, ":"))
            {
                throw new FormatException("There should not be a : in the path " + schemaLocation);
            }


            string whereFound = "";


            PathSet paths = new PathSet();


            string tryHere = PathSlicer.FindPath(schemaLocation, hint1, hint2);
            try { whereFound = _Load(_baseLocation, schemaLocation); SetBaseLocation = whereFound; } catch {
            try { whereFound = _Load(tryHere);                       SetBaseLocation = whereFound; } catch {
            try { whereFound = _Load("",            schemaLocation); SetBaseLocation = whereFound; } catch {
            } } }


            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, "");
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint1);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint1 + "\\" + hint2);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint2);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint2 + "\\" + hint1);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint1 + "\\" + hint2 + "\\" + hint3);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint2 + "\\" + hint3);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint3);


            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint1 + "\\" + hint3);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint3 + "\\" + hint1);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint3 + "\\" + hint2);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint1 + "\\" + hint3 + "\\" + hint2);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint2 + "\\" + hint1 + "\\" + hint3);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint2 + "\\" + hint3 + "\\" + hint1);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint3 + "\\" + hint1 + "\\" + hint2);
            if (string.IsNullOrEmpty(whereFound))  whereFound = TryLoad(schemaLocation, hint3 + "\\" + hint2 + "\\" + hint1);


            return whereFound;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TryLoad -->
        /// <summary>
        ///      If a 'try' blows up _baseLocation is not set
        ///      If it does not blow up the next schema is likely to be found in the same place
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hint"></param>
        /// <returns></returns>
        private string TryLoad(string fileName, string hint)
        {
            string path = "";
            if (hint == "")
            {
                try { path = _Load("..\\..\\..\\..\\..", fileName); SetBaseLocation = path; } catch {
                try { path = _Load("..\\..\\..\\..",     fileName); SetBaseLocation = path; } catch {
                try { path = _Load("..\\..\\..",         fileName); SetBaseLocation = path; } catch {
                try { path = _Load("..\\..",             fileName); SetBaseLocation = path; } catch {
                try { path = _Load("..",                 fileName); SetBaseLocation = path; } catch {
                } } } } }
            }
            else
            {
                string sub1 = hint.Substring(0,1);
                string sub2 = hint.Substring(hint.Length-1,1);
                if (sub1 != "\\" && sub2 != "\\")
                {
                    try { path = _Load("..\\..\\..\\..\\" + hint, fileName); SetBaseLocation = path; } catch {
                    try { path = _Load("..\\..\\..\\" + hint,     fileName); SetBaseLocation = path; } catch {
                    try { path = _Load("..\\..\\" + hint,         fileName); SetBaseLocation = path; } catch {
                    try { path = _Load("..\\" + hint,             fileName); SetBaseLocation = path; } catch {
                    try { path = _Load(hint,                      fileName); SetBaseLocation = path; } catch {
                    } } } } }
                }
            }
            return path;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Load -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string _Load(string path)
        {
            Load(path);
            return path;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Load -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string _Load(string dirPath, string fileName)
        {
            string path = dirPath + "\\" + fileName;
            Load(path);
            return path;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _LookupType -->
        /// <summary>
        ///      Returns what dictionary a type is in
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string _LookupType(string type)
        {
            string lookupType = "basic";
            if (_IsSimpleType(type))  lookupType = "SimpleType";
            if (_IsComplexType(type)) lookupType = "ComplexType";
            if (_IsElement(type))     lookupType = "ElementNode";
            return lookupType;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _LookupType -->
        /// <summary>
        ///      Returns what dictionary a type is in
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string _LookupType(XmlNode node)
        {
            if (node == null)  return "";
            string type = _AttributeValue(node, "type");
            if (type == "")
                type = _AttributeValue(node, "ref");


            // --------------------------------------------------------------------------
            //  Stuff
            // --------------------------------------------------------------------------
            string lookupType = "basic";
            if (node.HasChildNodes)               lookupType = "complexNode";
            if (_simpleTypes.ContainsKey(type))   lookupType = "SimpleType";
            if (_complexTypes.ContainsKey(type))  lookupType = "ComplexType";
            if (_elements.ContainsKey(type))      lookupType = "complexNode";
            if (type == "xs:string")              lookupType = "basicType";
            if (type == "xs:dateTime")            lookupType = "basicType";
            if (type == "xs:date")                lookupType = "basicType";


            return lookupType;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _NameSpace -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix_etc">accepts prefix or type with prefix</param>
        /// <returns></returns>
        public string _NameSpace(string prefix_etc)
        {
            string prefix = prefix_etc;


            // --------------------------------------------------------------------------
            //  Search the entire xmlns dictionary for the prefix and return its key
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(prefix, ":"))
                prefix = Regex.Replace(prefix_etc, ":.*$", "");
            else
                prefix = "targetNamespace";


            if (_xmlns.ContainsValue(prefix))
                foreach (string key in _xmlns.Keys)
                    if (_xmlns[key] == prefix)
                       return key;


            return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Prefix -->
        /// <summary>
        ///      Does a lookup on the xmlns dictionary with error checking
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public string _Prefix(string nameSpace)
        {
            string prefix;
            if (_xmlns.ContainsKey(nameSpace))
                prefix = _xmlns[nameSpace];
            else
                throw new KeyNotFoundException("nameSpace " + nameSpace + " not found in _xmlns dictionary");
            return prefix;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _RecordImport -->
        /// <summary>
        ///      Adds an import file to the _imports dictionary
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="fileName"></param>
        public void _RecordImport(string nameSpace, string fileName)
        {
            _imports[nameSpace] = new PathSlicer(fileName, ".");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _RecordPrefixes -->
        /// <summary>
        ///      Records name space prefixes in the _xmlns dictionary
        /// </summary>
        public void _RecordPrefixes()
        {
            XmlNode schemaNode = _FindChild(this, "xs:schema");
            string prefix;


            // --------------------------------------------------------------------------
            //  Record name space prefixes in the _xmlns dictionary
            // --------------------------------------------------------------------------
            foreach (XmlAttribute attr in schemaNode.Attributes)
            {
                prefix = Regex.Replace(attr.Name, "^xmlns:", "");
                if (prefix == "xmlns")
                    prefix = "";
                _xmlns[attr.Value] = prefix;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _RefNode -->
        /// <summary>
        ///      Gets a referenced node from one of the dictionaries
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="nodeTag"></param>
        /// <returns></returns>
        public XmlNode _RefNode(string reference, string nodeTag)
        {
            XmlNode node;
            switch (nodeTag)
            {
                case "xs:element":
                    node = _GetElement(reference).Node;
                    break;
                case "xs:attribute": // TODO: write this case
                default:
                    throw new NotImplementedException(nodeTag + " ref not written yet");
            }
            return node;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _RefTargetList -->
        /// <summary>
        ///      Returns a string containing a list of the referencable data types
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public string _RefTargetList(string delimiter)
        {
            string strRefs = "";
            string delim = "";
            foreach (string key in _elements.Keys)
            {
                strRefs += delim + key + "(" + _usedRef[key].ToString() + ")";
                delim = delimiter;
            }
            return strRefs;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _RussianDollTraversal -->
        /// <summary>
        ///      Creates a string like what the 'russian doll' version would be
        /// </summary>
        /// <remarks>
        ///      A 'Russian Doll' schema is one that uses no 'ref' atttributes, so everything is
        ///      in the form that the XML it validates or creates will be in
        /// </remarks>
        /// <param name="node"></param>
        /// <param name="level"></param>
        public string _RussianDollTraversal(XmlNode node, int level, string attrs)
        {
            RichXmlNode rxNode = new RichXmlNode(node);
            string margin = rxNode.Margin(level);
            string nodeTag = rxNode.Tag;
            string str;
            rxNode.AttributeList();


            if (rxNode.HasChildNodes) // elements
            {
                // ----------------------------------------------------------------------
                //  Process the node
                // ----------------------------------------------------------------------
                string reference = rxNode.Attribute("name");
                string minBound = rxNode.Attribute("minOccurs");
                string maxBound = rxNode.Attribute("maxOccurs");


                //  Things to skip:
                if (level == 2 && _IsUsed(reference) || nodeTag == "xs:annotation")
                    str = "";
                else
                {
                    switch (level)
                    {
                        case 0 : str = "";
                            break;
                        case 1 : str = "<" + nodeTag + rxNode.AttributeString() + attrs + ">";
                            break;
                        default: str = margin + "<" + nodeTag + rxNode.AttributeString() + attrs + ">";
                            break;
                    }


                    // ------------------------------------------------------------------
                    //  Add element tags and recurse down one level
                    // ------------------------------------------------------------------
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        str += _RussianDollTraversal(childNode, level + 1, "");
                    }
                    if (level > 0)  str += margin + "</" + nodeTag + ">";
                }
            }
            else
            {
                // ----------------------------------------------------------------------
                //  Process the leaf: show it or continue into the ref - refs are leaves
                // ----------------------------------------------------------------------
                string reference = rxNode.Attribute("ref");
                if (reference == "")
                    if (rxNode.NodeType == XmlNodeType.XmlDeclaration)
                        str = margin + "<?" + nodeTag + rxNode.AttributeString() + rxNode.LeafEnd();
                    else
                        str = margin + "<" + nodeTag + rxNode.AttributeString() + rxNode.LeafEnd();
                else
                {
                    str = _RussianDollTraversal(_RefNode(reference, nodeTag), level
                        , rxNode.MoreAttributes());
                }
            }


            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _SetUsed -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        public void _SetUsed(string reference)
        {
            if (!string.IsNullOrEmpty(reference))
            {
                if (_usedRef.ContainsKey(reference))
                    _usedRef[reference] = true;
                else _usedRef.Add(reference, true);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _SimpleNode -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RichXmlNode _SimpleNode(string type)
        {
            if (_IsSimpleType(type))
                return (new RichXmlNode(_simpleTypes[type]));
            else
                return null;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _StoreRef -->
        /// <summary>
        ///      Stores a named node in one of the dictionaries
        /// </summary>
        /// <param name="node"></param>
        /// <param name="prefix"></param>
        /// <param name="nodeTag"></param>
        /// <param name="name">stores a named node, ignores an unnamed node</param>
        public void _StoreRef(XmlNode node, string prefix, string nodeTag, string name)
        {
            // --------------------------------------------------------------------------
            //  Add a new reference to both dictionaries
            // --------------------------------------------------------------------------
            string abstr = _AttributeValue(node, "abstract");
            if (!string.IsNullOrEmpty(name))
            {
                if (abstr == "true")
                {
                    _abstract.Add(name, node);
                }
                else
                {
                    string fullName = __.ConcatIf(prefix, ":", name);
                    //if (name == "all")
                    //    nodeTag = nodeTag;
                    switch (nodeTag)
                    {
                        case "xs:element":
                            _elements.Add(fullName, node);
                            if (!_usedRef.ContainsKey(fullName))
                                _usedRef.Add(fullName, false);
                            break;
                        case "xs:simpleType" :  _simpleTypes.Add(fullName, node);   break;
                        case "xs:complexType":  _complexTypes.Add(fullName, node);  break;
                        default              :  _otherTypes.Add(fullName, node);    break;
                    }
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Tag -->
        /// <summary>
        ///      Returns the tag of the node (there's got to be a better way)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string _Tag(XmlNode node)
        {
            string tag;
            tag = Regex.Replace(node.OuterXml, " .+$", "", RegexOptions.Singleline);
            tag = Regex.Replace(tag, "^<", "");
            return tag;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _TypeList -->
        /// <summary>
        ///      Returns a string containing a list of types
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public string _TypeList(string delimiter)
        {
            string strTypes = "";
            string delim = "";
            foreach (string key in _simpleTypes.Keys)
            {
                strTypes += delim + key + "(" + _simpleTypes[key].ToString() + ")";
                delim = delimiter;
            }
            return strTypes;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Schema '" + _fileName + "'";
        }
    }
}
