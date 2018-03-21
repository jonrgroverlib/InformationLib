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
using InformationLib.SoftData;        // for TreatAs
using System;                         // for DateTime
using System.Collections;             // for IEnumerator
using System.Data.SqlTypes;           // for SqlDateTime
using System.IO;                      // for TextWriter
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for many...
using System.Xml.Schema;              // for ValidationEventhandler etc.
using System.Xml.XPath;               // for XPathNavigator

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RichXmlDocument -->
    /// <summary>
    ///      The X class is ...
    /// </summary>
    /// <remarks>mostly beta code - used twice in production, needs testing and fixing</remarks>
    public class RichXmlDocument
    {
        private XmlDocument _doc;
        private XmlNamespaceManager _xmlns;
        public XmlNamespaceManager Xmlns { get { return _xmlns; } set { _xmlns = value; } }
        private const string _defaultRootTag = "richxml";


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public RichXmlDocument()                 { _doc = new XmlDocument();                     }
        public RichXmlDocument(XmlDocument xdoc) { _doc = xdoc;                                  }
        public RichXmlDocument(string xdoc)
        {
            LoadXml(xdoc);
        }


        // ----------------------------------------------------------------------------------------
        //  Base Properties
        // ----------------------------------------------------------------------------------------
        public string BaseURI      { get { return _doc.BaseURI;      } }
        public string InnerText    { get { return _doc.InnerText;    } set { _doc.InnerText = value; } }
        public string InnerXml     { get { return _doc.InnerXml;     } set { _doc.InnerXml = value; } }
        public string LocalName    { get { return _doc.LocalName;    } }
        public string Name         { get { return _doc.Name;         } }
        public string NamespaceURI { get { return _doc.NamespaceURI; } }
        public string OuterXml     { get { return _doc.OuterXml;     } }
        public string Prefix       { get { return _doc.Prefix;       } }
        public string Value        { get { return _doc.Value;        } }


        public bool HasChildNodes      { get { return _doc.HasChildNodes;      } }
        public bool IsReadOnly         { get { return _doc.IsReadOnly;         } }
        public bool PreserveWhitespace { get { return _doc.PreserveWhitespace; } set { _doc.PreserveWhitespace = value; } }


        public XmlNode FirstChild      { get { return _doc.FirstChild;      } }
        public XmlNode LastChild       { get { return _doc.LastChild;       } }
        public XmlNode NextSibling     { get { return _doc.NextSibling;     } }
        public XmlNode ParentNode      { get { return _doc.ParentNode;      } }
        public XmlNode PreviousSibling { get { return _doc.PreviousSibling; } }


        public XmlNodeList     ChildNodes      { get { return _doc.ChildNodes;      } }
        public XmlElement      DocumentElement { get { return _doc.DocumentElement; } }
        public XmlDocumentType DocumentType    { get { return _doc.DocumentType;    } }
        public XmlNameTable    NameTable       { get { return _doc.NameTable;       } }
        public XmlNodeType     NodeType        { get { return _doc.NodeType;        } }
        public XmlDocument     OwnerDocument   { get { return _doc.OwnerDocument;   } }
        public IXmlSchemaInfo  SchemaInfo      { get { return _doc.SchemaInfo;      } }
        public XmlSchemaSet    Schemas         { get { return _doc.Schemas;         } set { _doc.Schemas = value; } }
        public XmlResolver     XmlResolver     { set { _doc.XmlResolver = value;    } }


        public XmlImplementation Implementation  { get { return _doc.Implementation; } }
        public XmlAttributeCollection Attributes { get { return _doc.Attributes; } }


        // ----------------------------------------------------------------------------------------
        //  Custom Properties
        // ----------------------------------------------------------------------------------------
        public XmlNode     XmlNode     { get { return (XmlNode)_doc; } }
        public XmlDocument XmlDocument { get { return _doc; } set { _doc = value; } }


        // ----------------------------------------------------------------------------------------
        //  Base Methods
        // ----------------------------------------------------------------------------------------
        public void Load(string filename)       { _doc.Load(filename);    }
        public void Normalize()                 { _doc.Normalize();       }
        public void RemoveAll()                 { _doc.RemoveAll();       }
        public void Save(XmlWriter w)           { _doc.Save(w);           }
        public void Save(string filename)       { _doc.Save(filename);    }
        public void Save(Stream outStream)      { _doc.Save(outStream);   }
        public void Save(TextWriter writer)     { _doc.Save(writer);      }
        public void WriteContentTo(XmlWriter w) { _doc.WriteContentTo(w); }
        public void WriteTo(XmlWriter w)        { _doc.WriteTo(w);        }


        public string GetNamespaceOfPrefix(string prefix)       { return _doc.GetNamespaceOfPrefix(prefix); }
        public string GetPrefixOfNamespace(string namespaceURI) { return _doc.GetPrefixOfNamespace(namespaceURI); }
        public bool   Supports(string feature, string version)  { return _doc.Supports(feature, version); }


        public XmlNode AppendChild     (XmlNode newChild)                      { return _doc.AppendChild(newChild); }
        public XmlNode CreateNode      (string      nodeTypeString,      string name, string namespaceURI) { return _doc.CreateNode(nodeTypeString, name, namespaceURI); }
        public XmlNode CreateNode      (XmlNodeType type,                string name, string namespaceURI) { return _doc.CreateNode(type,           name, namespaceURI); }
        public XmlNode CreateNode      (XmlNodeType type, string prefix, string name, string namespaceURI) { return _doc.CreateNode(type, prefix,   name, namespaceURI); }
        public XmlNode ImportNode      (XmlNode node, bool deep)               { return _doc.ImportNode(node, deep); }
        public XmlNode InsertAfter     (XmlNode newChild, XmlNode refChild)    { return _doc.InsertAfter(newChild, refChild); }
        public XmlNode InsertBefore    (XmlNode newChild, XmlNode refChild)    { return _doc.InsertBefore(newChild, refChild); }
        public XmlNode PrependChild    (XmlNode newChild)                      { return _doc.PrependChild(newChild); }
        public XmlNode ReadNode        (XmlReader reader)                      { return _doc.ReadNode(reader); }
        public XmlNode RemoveChild     (XmlNode oldChild)                      { return _doc.RemoveChild(oldChild); }
        public XmlNode ReplaceChild    (XmlNode newChild, XmlNode oldChild)    { return _doc.ReplaceChild(newChild, oldChild); }
        public XmlNode SelectSingleNode(string xpath)                          { return _doc.SelectSingleNode(xpath); }
        public XmlNode SelectSingleNode(string xpath, XmlNamespaceManager mgr) { return _doc.SelectSingleNode(xpath, mgr); }


        public XmlAttribute  CreateAttribute   (string name)                                          { return _doc.CreateAttribute(name);                            }
        public XmlAttribute  CreateAttribute   (string qualifiedName,            string NamespaceURI) { return _doc.CreateAttribute(qualifiedName,     NamespaceURI); }
        public XmlAttribute  CreateAttribute   (string prefix, string localName, string NamespaceURI) { return _doc.CreateAttribute(prefix, localName, NamespaceURI); }
        public XmlComment    CreateComment     (string data)                                          { return _doc.CreateComment(data); }
        public XmlElement    CreateElement     (string name)                                          { return _doc.CreateElement(name); }
        public XmlElement    CreateElement     (string qualifiedName,            string NamespaceURI) { return _doc.CreateElement(qualifiedName,     NamespaceURI); }
        public XmlElement    CreateElement     (string prefix, string localName, string NamespaceURI) { return _doc.CreateElement(prefix, localName, NamespaceURI); }
        public XmlText       CreateTextNode    (string text)                                          { return _doc.CreateTextNode(text); }
        public XmlWhitespace CreateWhitespace  (string text)                                          { return _doc.CreateWhitespace(text); }
        public XmlElement    GetElementById    (string elementId)                                     { return _doc.GetElementById(elementId); }
        public XmlNodeList   GetElementsByTagName(string name)                                       { return _doc.GetElementsByTagName(name); }
        public XmlNodeList   SelectNodes       (string xpath)                                         { return _doc.SelectNodes(xpath); }
        public XmlNodeList   SelectNodes       (string xpath, XmlNamespaceManager mgr)                { return _doc.SelectNodes(xpath, mgr); }


        public void Validate(ValidationEventHandler validationEventHandler) { _doc.Validate(validationEventHandler); }
        public void Validate(ValidationEventHandler validationEventHandler, XmlNode nodeToValidate) { _doc.Validate(validationEventHandler, nodeToValidate); }
        public IEnumerator              GetEnumerator()                    { return _doc.GetEnumerator(); }
        public XPathNavigator           CreateNavigator()                  { return _doc.CreateNavigator(); }
        public XmlDeclaration           CreateXmlDeclaration(string version, string encoding, string standAlone) { return _doc.CreateXmlDeclaration(version, encoding, standAlone); }
        public XmlDocumentType          CreateDocumentType(string name, string publicId, string systemId, string internalSubset) { return _doc.CreateDocumentType(name, publicId, systemId, internalSubset); }
        public XmlEntityReference       CreateEntityReference(string name) { return _doc.CreateEntityReference(name); }
        public XmlDocumentFragment      CreateDocumentFragment()           { return _doc.CreateDocumentFragment(); }
        public XmlSignificantWhitespace CreateSignificantWhitespace(string text) { return _doc.CreateSignificantWhitespace(text); }
        public XmlProcessingInstruction CreateProcessingInstruction(string target, string data) { return _doc.CreateProcessingInstruction(target, data); }


        // ----------------------------------------------------------------------------------------
        //  Custom Methods
        // ----------------------------------------------------------------------------------------


        // ----------------------------------------------------------------------------------------
        /// <!-- _Bool -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="namespacePrefix"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool _Bool(PathSlicer xpath, string namespacePrefix, bool defaultValue)
        {
            return TreatAs.BoolValue(_Str(xpath, namespacePrefix, ""), defaultValue);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- _BreakLevel -->
        /// <summary>
        ///      Finds where an xpath 'peters out' within an xml document
        /// </summary>
        /// <param name="xpaths"></param>
        /// <param name="xdoc"></param>
        /// <param name="namespacePrefix"></param>
        /// <returns></returns>
        private int _BreakLevel(PathSlicer xpath, string namespacePrefix)
        {
            int level = 0;
            XmlNode node = _GetNode(namespacePrefix, "/");
            for (level = 0; level < xpath.Count && node != null; ++level)
                node = _GetNode(namespacePrefix, xpath.SubPaths[level]);
            return (--level);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Copy -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RichXmlDocument _Copy()
        {
            RichXmlDocument rx = new RichXmlDocument(_doc.OuterXml);
            XmlNameTable nt = _doc.NameTable;
            rx._xmlns = new XmlNamespaceManager(nt);
            rx._xmlns.AddNamespace(string.Empty, "default.schema");
            rx._xmlns.AddNamespace(_defaultRootTag, "default.schema");
            return rx;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _CreateNodes -->
        /// <summary>
        ///      Inserts a fragment to fullfill nodes specified by an xpath
        /// </summary>
        /// <remarks>
        ///      To insert a node based on an xpath I could...
        ///       a.   use the XML generator to build 'complete templates'               (somewhat attractive option (see 'e' below))
        ///       b.   create this as an implementation of the XML generator             (much much too closely coupled)
        ///       c. * iterate through the insert node locations until the xml validates (terribly ineficient but fairly simple)
        ///       d.   dig into the schema and do analysis                               (very very complicated)
        ///       e.   use the xpath as an input to the generator to make sure the required node is there (attractive option)
        ///       f.   just do it wrong                                                  (won't validate)
        ///       g.   make sure that xmls always have the needed nodes                  (don't control the xmls)
        ///       h.   suck out all the data and build a new xml with it                 (huge pretty big project, unreliable, very slow) - use approach h as the general approach to what I am doing
        ///       i.   keep track of positions in some array somewhere                   (too compolicated - like 'd')
        ///       j.   convert to a more explicit xpath                                  (is there a more explicit xpath?)
        ///       k.   use LINQ                                                          (need to upgrade to vs 2008 - and learn LINQ)
        ///       * I picked c
        /// 
        /// Warning:
        ///      Validity may require data
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        public XmlNode _CreateNodes(PathSlicer xpath, string value)
        {
            string original = _doc.OuterXml;
            Validator val = new Validator();
            XmlNode fragment;
            XmlNode sibling;
            XmlNode parent;
            int level;


            // ----------------------------------------------------------------------
            //  This won't work properly without a schema
            // ----------------------------------------------------------------------
            if (_doc.Schemas == null || _doc.Schemas.Count == 0)
            {
                throw new XmlSchemaException("CreateNodes error -"
                    + " document needs an included schema set"
                    + " for PathBuilder.CreateNodes to work properly");
            }


            // ----------------------------------------------------------------------
            //  It should blow up if it's not valid to start with
            // ----------------------------------------------------------------------
            if (!val.IsValid(_doc))
            {
                throw new XmlException("CreateNodes error -"
                    + " xml document is not valid to start with"
                    + " according to schema " + _SchemaSetName(_doc));
            }


            // ----------------------------------------------------------------------
            //  It should blow up if the xpath root does nto match the xml root
            // ----------------------------------------------------------------------
            level    = this._BreakLevel(xpath, "ns0");
            if (level == 0)
            {
                throw new Exception("CreateNodes error -"
                    + " may not use an xpath to change an xml document"
                    + " if the root nodes are not the same");
            }


            // ----------------------------------------------------------------------
            //  Insert a fragment to complete the xpath
            //  1. Get level in xpaths telescope at which node does not exist
            //  2. Create a fragment to match the missing xpath
            //  3. Identify where to insert it (parent)
            //  4. Insert it as the first child
            // ----------------------------------------------------------------------
            fragment = _CreateNodes(xpath, level, value);
            if (level > 0)
            {
                parent = _GetNode("ns0", xpath.SubPaths[level - 1]);
                parent.InsertBefore(fragment, parent.FirstChild);


                // ------------------------------------------------------------------
                //  Move the new fragment along until the document validates
                // ------------------------------------------------------------------
                sibling = fragment.NextSibling;
                while (sibling != null && !val.IsValid(_doc))
                {
                    sibling = fragment.NextSibling;
                    parent.RemoveChild(fragment);
                    parent.InsertAfter(fragment, sibling);
                }
            }
            else
                parent = null;


            // ----------------------------------------------------------------------
            //  Restore original document if it didn't work
            // ----------------------------------------------------------------------
            if (!val.IsValid(_doc))  _doc.LoadXml(original);


            return _GetNode("ns0", xpath.Path);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- _CreateNodes -->
        /// <summary>
        ///      Test me
        /// </summary>
        /// <remarks>
        ///      Example usage:
        /// 
        ///       XmlDocument doc = new XmlDocument();
        ///       doc.LoadXml("<ns0:PatientProfile>"
        ///           + "   <EVN_EventType>"
        ///           + "      <EVN_1_EventTypeCode>R03</EVN_1_EventTypeCode>"
        ///           + "      <EVN_2_DateTimeOfEvent>2008-04-30T02:05:51-05:00</EVN_2_DateTimeOfEvent>"
        ///           + "      <EVN_4_EventReasonCode>03</EVN_4_EventReasonCode>"
        ///           + "      <EVN_5_OperatorId>914390304393256806</EVN_5_OperatorId>"
        ///           + "   </EVN_EventType>"
        ///           + "</ns0:PatientProfile>");
        ///       doc.Schemas  = _.SchemaSet("PatientProfile.xsd", "xml", "Jon.CWTestHarness");
        ///       string xpath = "//ns0:PatientProfile/EVN_EventType/EVN_3_DateTimePlannedEvent";
        ///       XmlNode node = _.GetNode(doc, "ns0", xpath);
        ///       if (node == null)  node = (new PathBuilder(xpath)).CreateNodes(doc);
        /// </remarks>
        /// <param name="xpath"></param>
        /// <param name="start">At what segment in the xpath to start creating</param>
        /// <param name="value"></param>
        /// <returns></returns>
        private XmlNode _CreateNodes(PathSlicer xpath, int start, string value)
        {
            if (start < 0)  return null;
            XmlNode root = _doc.CreateNode(XmlNodeType.Element, "", "hi", null);;
            XmlNode cursor = root;
            XmlNode node = null;


            // ----------------------------------------------------------------------
            //  Create a string of nodes
            // ----------------------------------------------------------------------
            for (int i = start; i < xpath.Count; ++i)
            {
                char[] c = { ':' };
                string[] substr = xpath.Segments[i].Split(c);
                switch (substr.Length)
                {
                    case 0  : throw new Exception("PathSlicer segment does not exist");
                    case 1  : node = _doc.CreateNode(XmlNodeType.Element, "", substr[0], null);  break;
                    case 2  : node = _doc.CreateNode(XmlNodeType.Element, substr[0], substr[1], null); break;
                    default : throw new Exception("PathSlicer segment - too many ':'s");
                }


                if (i == start) root = node;  else  cursor.AppendChild(node);
                cursor = node;
            }


            // ----------------------------------------------------------------------
            //  Set the value of the last one
            // ----------------------------------------------------------------------
            cursor.InnerText = value;


            return root;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- _Date -->
        /// <summary>
        ///      Retrieves an xml datetime given the path
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="namespacePrefix"></param>
        /// <returns></returns>
        public DateTime _Date(PathSlicer xpath, string namespacePrefix)
        {
            return TimeDate_old.ClrDate(_Str(xpath, namespacePrefix, ""), (DateTime)SqlDateTime.MinValue);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Date constructor -->
        /// <summary>
        ///      Gets the text from the XML document in the place specified by the xpath
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="namespacePrefix"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        //public Date Dates(string namespacePrefix, string xpath)
        //{
        //    Date date = new Date();
        //    XmlNode node = _GetNode(namespacePrefix, xpath);
        //    if (node == null) date.SetDateNull();
        //    else date.ISOformat = node.InnerText;
        //    return date;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- Dispose -->
        /// <summary>
        ///      I'm not sure this in necessary, but something was eating up all my memory
        /// </summary>
        public void Dispose()
        {
            _doc   = null;
            _xmlns = null;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _GetNode -->
        /// <summary>
        ///      Gets a node from a document using an xpath when there is a namespace prefix
        /// </summary>
        /// <param name="xdoc">this xmldocument must have a schemaset defined internally</param>
        /// <param name="namespacePrefix">this is the namespace prefix</param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNode _GetNode(string namespacePrefix, string xpath)
        {
            XmlDocument dummy = new XmlDocument();
            dummy.LoadXml("<null />");
            if (_doc == null || _doc.DocumentElement == null || string.IsNullOrEmpty(_doc.InnerXml))
            {
                return dummy;
            }


            XmlNode node;


            if (namespacePrefix == "")
            {
                namespacePrefix = SetNamespacePrefix(namespacePrefix);
                xpath = Regex.Replace(xpath, "^//", "//"+_defaultRootTag+":");
            }


            // --------------------------------------------------------------------------
            //  Prepare a namespace manager for the selection
            // --------------------------------------------------------------------------
            XmlNamespaceManager ns = new XmlNamespaceManager(_doc.NameTable);
            XPathNavigator nav = _doc.CreateNavigator();
            string namespaceURI = _doc.DocumentElement.GetNamespaceOfPrefix(namespacePrefix);
            ns.AddNamespace(namespacePrefix, namespaceURI);


            // --------------------------------------------------------------------------
            //  Do the selection
            // --------------------------------------------------------------------------
            node = _doc.SelectSingleNode(xpath, ns);
            string newXpath = Regex.Replace(xpath, namespacePrefix + ":", "");


            if (node == null) node = _doc.SelectSingleNode(newXpath, ns);
            if (node == null) node = _doc.SelectSingleNode(newXpath);
            if (node == null) try { node = _doc.SelectSingleNode("//" + newXpath); } catch { }
            if (node == null) try { node = _doc.SelectSingleNode("//" + xpath);    } catch { }
            if (node == null) node = dummy;


            return node;
        }
        public XmlNode _GetNode(string xpath) { return _GetNode("", xpath); }
        public XmlNode _GetNode(string namespacePrefix, PathSlicer xpath)
        {
            XmlNode node = _GetNode(namespacePrefix, xpath.Path);
            if (node == null)  // if SelectSingleNode doesn't work, try finding the node 'manually'
            {
                RichXmlNode n2 = new RichXmlNode(_doc);
                foreach (string segment in xpath.Segments)
                {
                    n2 = new RichXmlNode(n2.FindChild(segment));
                    if (n2.Node == null)
                        break;
                }
                node = n2.Node;
            }
            return node;
        }


        public XmlNodeList _GetNodes(string namespacePrefix, string xpath)
        {
            XmlNodeList node;
            if (_doc == null || _doc.DocumentElement == null)  return null;


            if (namespacePrefix == "")
            {
                namespacePrefix = this.SetNamespacePrefix(namespacePrefix);
                xpath = Regex.Replace(xpath, "^//", "//"+_defaultRootTag+":");
            }


            // --------------------------------------------------------------------------
            //  Prepare a namespace manager for the selection
            // --------------------------------------------------------------------------
            XmlNamespaceManager ns = new XmlNamespaceManager(_doc.NameTable);
            XPathNavigator nav = _doc.CreateNavigator();
            string namespaceURI = _doc.DocumentElement.GetNamespaceOfPrefix(namespacePrefix);
            ns.AddNamespace(namespacePrefix, namespaceURI);


            // --------------------------------------------------------------------------
            //  Do the selection
            // --------------------------------------------------------------------------
            node = _doc.SelectNodes(xpath, ns);
            string newXpath = Regex.Replace(xpath, namespacePrefix + ":", "");


            if (node == null) node = _doc.SelectNodes(newXpath, ns);
            if (node == null) node = _doc.SelectNodes(newXpath);
            if (node == null) try { node = _doc.SelectNodes("//" + newXpath); } catch { }
            if (node == null) try { node = _doc.SelectNodes("//" + xpath);    } catch { }


            return node;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IntValue -->
        /// <summary>
        ///      returns an integer or the default if it can not parse one
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int IntValue(string xPath, int defaultValue)
        {
            string str = StrValue(xPath, "");
            int    num = TreatAs.IntValue(str, defaultValue);
            return num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetNamespacePrefix -->
        /// <summary>
        ///      Changes the namespace to _defaultRootTag
        /// </summary>
        /// <param name="namespacePrefix"></param>
        /// <returns>namespacePrefix</returns>
        private string SetNamespacePrefix(string namespacePrefix)
        {
            string xmlNamespace = NameSpace;
            if (xmlNamespace != _defaultRootTag)
                Throws.A(new XmlException("Code did not properly pre-process the xml document!"
                    + "\r\nxml namespace problem"), Throws.Actions, "P");
            namespacePrefix = _defaultRootTag;
            return namespacePrefix;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NameSpace -->
        /// <summary>
        /// 
        /// </summary>
        public string NameSpace
        {
            get { string xmlNamespace = Regex.Replace(_doc.InnerXml, "^<([^/ ]+):.*$", "$1"); return xmlNamespace; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LoadXml -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        public void LoadXml(string xml)
        {
            _doc = new XmlDocument();
            if (string.IsNullOrEmpty(xml))  return;
            xml = Regex.Replace(xml, "^[^<]+<", "<");  // get rid of any crap before the root
            if (Regex.IsMatch(xml, "^<[^<:> ]+ "))
            {
                string root = _RootTag(xml);
                xml = Regex.Replace(xml, "^<"+root+" ", "<"+_defaultRootTag+":"+root+" ");
                xml = Regex.Replace(xml, "</"+root+">", "</"+_defaultRootTag+":"+root+">");
                xml = Regex.Replace(xml, "xmlns=\"h", "xmlns:"+_defaultRootTag+"=\"h");


                _doc.LoadXml(xml);
                XmlNameTable nt = _doc.NameTable;
                _xmlns = new XmlNamespaceManager(nt);
                _xmlns.AddNamespace(string.Empty, "default.schema");
                _xmlns.AddNamespace(_defaultRootTag, "default.schema");
            }
            else
                _doc.LoadXml(xml);
        }


        private static string _RootTag(string xml)
        {
            return Regex.Replace(xml, "^<([^< >]+) .+$", "$1");
        }

        //public string _RootTag { get { return Regex.Replace(_doc.InnerXml, "^<([^< >]+) .+$", "$1"); } }

        // ----------------------------------------------------------------------------------------
        /// <!-- NullableDateTimeValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public DateTime? NullableDateTimeValue(string xPath, DateTime? defaultValue)
        {
            string    str  = StrValue(xPath, "");
            DateTime? time = TreatAs.NullableDateTimeValue(str, defaultValue);
            return    time;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _SchemaSetName -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private static string _SchemaSetName(XmlDocument xdoc)
        {
            string name = "";
            foreach (XmlSchema schema in xdoc.Schemas.Schemas())
            {
                name = schema.SourceUri;
                break;
            }
            name = Regex.Replace(name, "^.*/", "");
            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _StoreValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="namespacePrefix"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public XmlNode _StoreValue(PathSlicer xpath, string namespacePrefix, object value)
        {
            string strValue = TreatAs.StrValue(value, "");
            XmlNode node = _GetNode(namespacePrefix, xpath.Path);
            if (node == null)
                node = _CreateNodes(xpath, strValue);
            else node.InnerText = strValue;
            return node;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Str -->
        /// <summary>
        ///      Retrieves xml data given the path
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="namespacePrefix"></param>
        /// <returns></returns>
        public string _Str(PathSlicer xpath, string namespacePrefix, string defaultValue)
        {
            string str = defaultValue;
            XmlNode node = _GetNode(namespacePrefix, xpath);
            if (node != null)
                str = node.InnerText;
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string StrValue(string xPath, string defaultValue)
        {
            string str = "";
            XmlNode node = this.SelectSingleNode(xPath);
            if (node == null || node.InnerText == null)
                str = defaultValue;
            else
                str = node.InnerText;
            return str;
        }
    }
}
