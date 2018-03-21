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
using InformationLib.Data    ;        // for 
using InformationLib.SoftData;        // for 
using InformationLib.Strings ;        // for 
using InformationLib.Testing ;        // for RandomSource
using System;                         // for Random
using System.Collections.Generic;     // for Dictionary, List
using System.Data;                    // for 
using System.IO;                      // for File, StreamWriter
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for XmlDocument, XmlNode, XmlTextReader, XmlValidatingReader
using System.Xml.Schema;              // for XmlSchemaCollection, XmlSchemaSet

namespace InformationLib.Generator
{
    // --------------------------------------------------------------------------------------------
    /// <!-- XmlGenerator -->
    /// <summary>
    ///      The XmlGenerator class creates a new XML to match a group of schemas or a single schema
    /// </summary>
    /// <remarks>
    ///      Lessons Learned:
    ///       1. use XmlSchemaSet - or maybe not
    ///       2. use the abstract factory pattern - (use the factory method pattern first?)
    ///       3. try to use more strongly typed nodes - (working on it)
    ///       
    ///       beta code - used once in production
    /// </remarks>
    public abstract class AbstractXmlGenerator : IGeneratorResults
    {

        #region constructors and members


        // ----------------------------------------------------------------------------------------
        /// <!-- NewXml -->
        private RichXmlDocument _newXml;
        /// <summary>
        ///      The Xml thus created
        /// </summary>
        public RichXmlDocument NewXml
        {
            get { return _newXml; }
            set { _results = value.ToString(); throw new NotSupportedException("run Create first"); }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SaveMemento -->
        private RichXmlDocument _memento;
        /// <summary>
        ///      Saves the xml state
        /// </summary>
        public void SaveMemento()            { _memento.LoadXml(XmlToString); }
        public AbstractXmlGenerator RestoreMemento() { _newXml.LoadXml(_memento.OuterXml); return this; }
        public bool IsChanged { get { return (XmlToString != PrettyTraversal_manager(_memento)); } }


        // ----------------------------------------------------------------------------------------
        //  Various XML creation objects
        // ----------------------------------------------------------------------------------------
        private string _schemaResult;


        // ----------------------------------------------------------------------------------------
        /// <!-- Cursor -->
        private XmlNode _cursor;
        /// <summary>
        ///      the cursor is used for traversing and creating the XML
        /// </summary>
        private XmlNode Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Schemas -->
        private RichXmlSchemaSet _schemas;
        /// <summary>
        ///      The schema set being used
        /// </summary>
        public XmlSchemaSet Schemas
        {
            get { return (XmlSchemaSet)_schemas; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- IsValid -->
        /// <summary>
        ///      Tells whether a document is valid
        /// </summary>
        public bool IsValid
        {
            get { return (new Validator()).IsValid(XmlToString, Schemas); }
        }


        // ----------------------------------------------------------------------------------------
        //  Various other attributes
        // ----------------------------------------------------------------------------------------
        private static bool _debugMode = true;
        private bool _started;


        // ----------------------------------------------------------------------------------------
        /// <!-- Errors -->
        private string _errors;
        /// <summary>
        /// 
        /// </summary>
        public string Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Results -->
        private string _results;
        /// <summary>
        /// 
        /// </summary>
        public string Results
        {
            get { return _results; }
            set { _results = value; }
        }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public AbstractXmlGenerator() { Init(); }
        public AbstractXmlGenerator(string filename)
        {
            string pathWhereSchemaFound = filename;
            Init();


            try
            {
                pathWhereSchemaFound = Load_manager(filename);
            }
            catch (Exception ex)
            {
                _errors = ex.Message;
                throw new FileNotFoundException("XmlGenerator constructor error"
                    + " - the xml file '" + filename + "' could not be found.");
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SchemaLoadEventHandler -->
        /// <summary>
        ///        Handles schema validation failure events
        /// </summary>
        private void SchemaLoadEventHandler(object o, ValidationEventArgs args)
        {
            _schemaResult += "\r\nSchema not valid" + __.ConcatIf(":\r\n", args.Message);
        }


        #endregion constructors and members

        #region recursive method managers


        // ----------------------------------------------------------------------------------------
        /// <!-- Create -->
        /// <summary>
        ///      Creates an XML instance from the schema mentioned in the path
        /// </summary>
        /// <param name="schemaFilePath"></param>
        /// <returns></returns>
        public string Create(string schemaFilePath)
        {
            Init();
            string whereFoundPath = Load_manager(schemaFilePath);
            string xml = CreateFrom(__.FileName(schemaFilePath));
            return whereFoundPath;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateFirstNode -->
        /// <summary>
        ///      Creates the first node including namespace stuff
        /// </summary>
        /// <remarks>
        ///      for now this is just going to be a bunch of hacks
        /// </remarks>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private RichXmlNode CreateFirstNode(SimpleSchema schema, string name)
        {
            // --------------------------------------------------------------------------
            //  HL7 schemas
            // --------------------------------------------------------------------------
            if (!_started
                && (Regex.IsMatch(schema._FileName, "_GLO_DEF.xsd$")
                 || Regex.IsMatch(schema._FileName, "PatientProfile")
                   )
               )  // encapsulate
            {
                RichXmlNode node = AddNewElement(name, "", "ns0");
                NewAttribute(node, "xmlns:ns0", "http://microsoft.com/HealthCare/HL7/2X");  // encapsulate
                _started = true;
                return node;
            }


            // --------------------------------------------------------------------------
            //  Multi-file schemas
            // --------------------------------------------------------------------------
            if (!_started && Regex.IsMatch(schema._FileName, "endeme4.xsd$"))
            {
                RichXmlNode node = AddNewElement(name, "");
                NewAttribute(node, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                NewAttribute(node, "xsi:noNamespaceSchemaLocation", schema._FileName);
                _started = true;
                return node;
            }


            // --------------------------------------------------------------------------
            //  Simple (single file) schemas
            // --------------------------------------------------------------------------
            if (!_started)
            {
                RichXmlNode node = AddNewElement(name, "");
                NewAttribute(node, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                NewAttribute(node, "xsi:noNamespaceSchemaLocation", schema._FileName);
                _started = true;
                return node;
            }


            return new RichXmlNode(_newXml.CreateElement("hi", null));
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateFrom -->
        /// <summary>
        ///      Creates an XML from the presently loaded schema
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string CreateFrom(string fileName)
        {
            SimpleSchema schema = null;


            // --------------------------------------------------------------------------
            //  Prepare for creation traversal
            // --------------------------------------------------------------------------
            try { schema = _schemas._ByFileName(fileName); }
            catch (KeyNotFoundException e)
                { Throws.A(new FileNotFoundException("Schema '"+fileName+"' not found -- " + e.Message), Throws.Actions, "P"); }
            if (schema == null)
                { Throws.A(new NoNullAllowedException("schema found at "+fileName+" is null"), Throws.Actions, "P");  return ""; }


            // --------------------------------------------------------------------------
            //  Convert it to a pretty format
            // --------------------------------------------------------------------------
            string pretty = PrettyTraversal_manager(new RichXmlDocument(schema.XmlDocument));


            // --------------------------------------------------------------------------
            //  Debugging stuff
            // --------------------------------------------------------------------------
            if (_debugMode)
            {
                string str;
                str = schema._RefTargetList(", ");
                str = schema._TypeList(", ");
            }


            // --------------------------------------------------------------------------
            //  Perform creation traversal
            // --------------------------------------------------------------------------
            _newXml = new RichXmlDocument();
            Cursor = _newXml.XmlDocument;
            CreationTraversal(schema, new RichXmlNode(schema), 0);
            string strXml = PrettyTraversal_manager(_newXml);


            return strXml;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FoundInSchema -->
        /// <summary>
        ///      Returns the schema where an XML element is referenced within a schema
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private SimpleSchema FoundInSchema(string elementName)
        {
            SimpleSchema schema = new SimpleSchema();
            RichXmlNode found = null;
            foreach (string key in _schemas._FileNames)
            {
                found = FindInSchema_recursive(new RichXmlNode(_schemas._ByFileName(key)), elementName);
                schema = _schemas._ByFileName(key);
                if (found != null) break;
            }
            return schema;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FindInSchema -->
        /// <summary>
        ///      Returns the node where an XML element is referenced within a schema
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private RichXmlNode FindInSchema(string elementName)
        {
            RichXmlNode found = null;
            foreach (string key in _schemas._FileNames)
            {
                found = FindInSchema_recursive(new RichXmlNode(_schemas._ByFileName(key)), elementName);
                if (found != null) break;
            }
            return found;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FlatTraversal_manager -->
        /// <summary>
        ///      Sets off a recursive Traversal of the XML document
        /// </summary>
        /// <returns></returns>
        private string FlatTraversal_manager()
        {
            RichXmlNode node = new RichXmlNode(_schemas._ByNamespace(""));
            return node.FlatTraversal(0);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Load_manager -->
        /// <summary>
        ///      Does stuff
        /// </summary>
        /// <remarks>
        ///      A simple loading method and other methods that could be inherited if XmlGenerator was
        ///      derived from XmlDocument, but that's probably not a good idea because this
        ///      class contains three XmlDocument's
        /// </remarks>
        /// <param name="filePame">path where schema is supposed to be</param>
        public string Load_manager(string filePath)
        {
            string pathWhereSchemaFound = "";


            // --------------------------------------------------------------------------
            //  Bring in and register all the schemas
            // --------------------------------------------------------------------------
            Clear();
            PathSlicer path = new PathSlicer(filePath, "\\");
            pathWhereSchemaFound = ImportTraversal(path, "", "");
            if (string.IsNullOrEmpty(pathWhereSchemaFound))
            {
                Throws.A(new IOException("Can not find schema at " + filePath), Throws.Actions, "P");
                return "";
            }


            foreach (string fileNameSpace in _schemas.Namespaces)
            {
                // ----------------------------------------------------------------------
                //  Fill the schema's and its parent's node lists
                // ----------------------------------------------------------------------
                SimpleSchema schema = _schemas._ByNamespace(fileNameSpace);
                SimpleSchema parentSchema = _schemas._ParentSchema(schema);
                RefTargetTraversal(schema, new RichXmlNode(schema), "", 0);
                if (parentSchema != null)
                    RefTargetTraversal(parentSchema
                        , new RichXmlNode(schema), ParentPrefix(fileNameSpace), 0);
            }


            // --------------------------------------------------------------------------
            //  Bring in and register all the schemas using XmlSchemaSet
            // --------------------------------------------------------------------------
            XmlSchema mySchema;
            mySchema = XmlSchema.Read(new XmlTextReader(pathWhereSchemaFound), SchemaLoadEventHandler);
            _schemas.Add(mySchema);
            if (_schemas.Count < _schemas.Namespaces.Count)
                Throws.A(new XmlSchemaException("too few schemas in the schema set ("+_schemas.Count+")"
                    + " for the found schemas from the file path '"+filePath+"' ("+_schemas.Namespaces.Count+")."
                    + " There may be a problem with a schemaLocation property in the "+filePath+" file."), Throws.Actions, "P");


            return pathWhereSchemaFound;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- PrettyTraversal_manager -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string PrettyTraversal_manager(RichXmlDocument xml)
        {
            RichXmlNode node = new RichXmlNode(xml.XmlNode);
            string pretty = node.PrettyTraversal_recursive(0);


            string target = Regex.Replace(xml.OuterXml, "[ \n\r\t]", "");
            string output = Regex.Replace(pretty, "[ \n\r\t]", "");
            if (output != target)
                return pretty;
//                return Regex.Replace(xml.OuterXml, "><", ">\r\n<");
//                throw new Exception("pretty print broke XML or the XML was broken");
            else
                return pretty;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ModifyXml -->
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///      How can we update Xml?
        ///      Six ways in order of increasing difficulty (pick one or more)
        ///      1. change some content
        ///      2. change attribute
        ///      3. remove nodes
        ///      4. add nodes
        ///      5. replace node
        ///      6. new entire xml
        ///      Should we work with the memento? (file)
        ///      Should we pull out of the database? (guid)
        /// 
        ///      Need to test this.
        ///</remarks>
        public string RandomlyChange()
        {
            // --------------------------------------------------------------------------
            //  Pick random node from the list
            // --------------------------------------------------------------------------
            RichXmlNode node = PickRandomNode();
            int num = CreateNewNumNodes(node);


            // --------------------------------------------------------------------------
            //  Erase and/or replace the node
            //  TODO: add in possibilties of removing siblings too
            // --------------------------------------------------------------------------
            switch (num)
            {
                case -1: break;
                case 0 :  node.RemoveSelf();       break;
                case 1 :  RecreateNode(node);  break;
                default : // recreate node, create new nodes
                    node.RemoveAll();
                    break;
            }


            return node.Name;
        }


        public void RipOutMostSegments(int numLeft)
        {
            _results += numLeft.ToString();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateNumNodes -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int CreateNewNumNodes(RichXmlNode node)
        {
            // --------------------------------------------------------------------------
            //  Find the node in the schema
            //  Determine the number of nodes the node will be replaced by
            // --------------------------------------------------------------------------
            int num = CreateNumElements(FindInSchema(node.Name));
            int currentNum = node.ParentNode.CountChildrenLike(node.Tag);
            num = Math.Max(num - currentNum + 1, 0);


            return num;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- RecreateNode -->
        /// <summary>
        ///      Recreates a node from the schema
        /// </summary>
        /// <param name="node"></param>
        private void RecreateNode(RichXmlNode node)
        {
            if (node != null && node.ParentNode != null)
            {
                RichXmlNode found = FindInSchema(node.Name);
                SimpleSchema schema = FoundInSchema(node.Name);
                RichXmlNode parent = node.ParentNode;
                node.RemoveAll();


                Cursor = parent.Node;
                CreationTraversal(schema, found, 0);
                _results += "replaced " + node.OuterXml
                    + __.ConcatIf("\r\nwith ", parent.LastChild.OuterXml);
                parent.ReplaceChild(parent.LastChild, node);
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- PickRandomNode -->
        /// <summary>
        ///      You've got some schemas - pick a node, erase it, and rebuild it, Register the schemas if necessary
        /// </summary>
        /// <returns></returns>
        private RichXmlNode PickRandomNode()
        {
            // --------------------------------------------------------------------------
            //  List the nodes in the XML, pick random node from the list
            // --------------------------------------------------------------------------
            List<RichXmlNode> list = (new RichXmlNode(_newXml.XmlNode)).ElementList_recursive();
            Random r = RandomSource.New().Random;
            int n = 1 + r.Next(list.Count - 1);
            RichXmlNode node = list[n];


            return node;
        }


        #endregion recursive method managers

        #region recursive methods


        // ----------------------------------------------------------------------------------------
        /// <!-- ImportTraversal -->
        /// <summary>
        ///      Recursively imports an xml schema mentioned in a parent schema
        /// </summary>
        /// <param name="importNode"></param>
        /// <param name="fileName"></param>
        private string ImportTraversal(PathSlicer p, string nameSpace, string parentFileName)
        {
            SimpleSchema newSchema = new SimpleSchema();
            string fileNameSpace;


            // --------------------------------------------------------------------------
            //  Retrieve the imported schema - I really only need it in the refs
            // --------------------------------------------------------------------------
            string whereFound = newSchema._Load_simple(p.Name, "CW.Testgen", "xml", "schemas");
            if (string.IsNullOrEmpty(whereFound))
                whereFound = newSchema._Load_simple(p.Name + ".xsd", "CHNw.CWTestHarness", "xml", "schemas");
            newSchema._FileName = p.Name;


            if (!string.IsNullOrEmpty(whereFound))
            {
                // ----------------------------------------------------------------------
                //  Save and link imported schema
                // ----------------------------------------------------------------------
                fileNameSpace = __.Openface(p.Name, ":", nameSpace);


                _schemas._AddCallerByFilename(p.Name, parentFileName);
                _schemas._RegisterSchema(p.Name, fileNameSpace, newSchema);


                RichXmlNode xSchema = new RichXmlNode(newSchema);
                XmlNode schemaNode = xSchema.FindChild("xs:schema");
                RichXmlNode xNode = new RichXmlNode(schemaNode);


                // ----------------------------------------------------------------------
                //  Do an import traversal, identify and load import schemas and prefixes
                // ----------------------------------------------------------------------
                newSchema._RecordPrefixes();


                List<XmlNode> nodeList = xNode.FindChildren("xs:import");
                foreach (XmlNode node in nodeList)
                {
                    RichXmlNode xNode2 = new RichXmlNode(node);
                    string newNameSpace = xNode2.Attribute("namespace");
                    string partialFilePath = xNode2.Attribute("schemaLocation");


                    partialFilePath = RichXmlSchemaSet.FixName(partialFilePath);


                    //string newNameSpace = AttributeValue(node, "namespace");
                    //string newFileName = AttributeValue(node, "schemaLocation");
                    newSchema._RecordImport(newNameSpace, partialFilePath);
                    ImportTraversal(new PathSlicer(partialFilePath, "."), newNameSpace, p.Name);
                }
            }


            return whereFound;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreationTraversal -->
        /// <summary>
        ///      Creates an xml to match the schema, assumes the schema is in 'russian doll' format,
        ///      Those portions that are not in 'russian doll' format (like imported schemas) are
        ///      handled ad hoc elsewhere
        /// </summary>
        /// <param name="node"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private void CreationTraversal(SimpleSchema schema, RichXmlNode schemaNode, int level)
        {
            RichXmlNode node = schemaNode;
            string nodeTag = node.Tag;
            string name = node.Attribute("name");
            if (string.IsNullOrEmpty(name))
                name = node.Attribute("ref");


            // --------------------------------------------------------------------------
            //  Ignore documentation and referenced types
            // --------------------------------------------------------------------------
            if (nodeTag == "xs:annotation"
                || level > 2 && !_started
                || level == 2 && (nodeTag == "xs:simpleType" || nodeTag == "xs:complexType"))
                return;
            else
            {
                // ----------------------------------------------------------------------
                //  This prepares to create multiple instances of the same element
                // ----------------------------------------------------------------------
                int num = CreateNumberOfElements(node);  // FIX


                for (int i = 0; i < num; ++i)
                {
                    string type = schemaNode.AttributeValue("type");
                    if (string.IsNullOrEmpty(type))  type = name;


                    bool done = false;


                    if (!done && schema._IsSimpleType(type))
                    {
                        AddNewElement(name, CreateSimpleType(schema, schema._SimpleNode(type), name));
                        done = true;
                    }
                    else
                    {
                        switch (nodeTag)
                        {
                            case "xs:attribute"   : CreateAttribute(schema, name, type); break;
                            case "xs:extension"   : CreateExtension(schema, node, name); break;
                            case "xs:field"       : break;
                            case "xs:import"      : break;
                            case "xs:selector"    : break;
                            case "xs:complexType" : CreateComplexType(schema, node); break;
                            case "xs:element" :
                                if (node.HasChildNodes) // or node ref has child nodes
                                {
                                    TryCreateFirstElement(schema, name);
                                    foreach (RichXmlNode childNode in node.RichChildNodes)
                                        CreationTraversal(schema, childNode, level + 1);
                                    if (!string.IsNullOrEmpty(name) && Cursor != null && Cursor.ParentNode != null)
                                        Cursor = Cursor.ParentNode;
                                }
                                else
                                {
                                    RichXmlNode node2 = schema._GetElement(type);
                                    if (node2.Node == null)
                                        CreateElement(schema, node);
                                    else
                                        CreateElement(schema, node2);
                                }
                                break;
                            default:
                                if (node.HasChildNodes) // or node ref has child nodes
                                {
                                    foreach (RichXmlNode childNode in node.RichChildNodes)
                                        CreationTraversal(schema, childNode, level + 1);
                                }
                                else
                                {
                                    if (!done && nodeTag == "?xml") done = true;
                                    if (!done && nodeTag == "xml") done = true;
                                    if (!done)
                                        Pause();
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void Pause()
        {
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateAttribute -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateAttribute(SimpleSchema schema, string name, string type)
        {
            string data = "CreateAttribute";
            // --------------------------------------------------------------------------
            //  Appends a new attribute to the current node
            // --------------------------------------------------------------------------
            if (!string.IsNullOrEmpty(name))
            {
                if (schema._IsSimpleType(type))
                    data = CreateSimpleType(schema, schema._SimpleNode(type), name);
                else data = CreateBaseType_nonrecursive(type, name);  //  Assume that it is a base type


                AddNewAttribute(name, data);
            }
            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateComplexType -->
        /// <summary>
        ///      Creates complex type data from a complex type node
        /// </summary>
        /// <remarks>
        ///      CreateComplexType_recursive is part of a two method recursion set
        /// </remarks>
        /// <param name="node"></param>
        /// <returns></returns>
        private string CreateComplexType(SimpleSchema schema, RichXmlNode node)
        {
            if (node == null || schema == null)                return "";
            string data = "CreateComplexType";

            if (node.Attribute("name") == "PID")
                Pause();


            int num = node.RichChildNodes.Count;
            foreach (RichXmlNode childNode in node.RichChildNodes)
            {
                switch (childNode.Tag)
                {
                    case "xs:sequence"     :  data = CreateSequence(schema, data, childNode);  break;
                    case "xs:simpleContent":  data = CreateSimpleContent(schema, childNode);   break;
                    case "xs:annotation"   : /* do nothing */
                        break;
                    default :
                        Throws.A(new NotImplementedException("CreateComplexType - bam!"), Throws.Actions, "P");
                        break;
                }
            }


            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateElement -->
        /// <summary>
        ///      Appends a new element to the current without moving into it
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="xNode"></param>
        /// <returns></returns>
        private string CreateElement(SimpleSchema schema, RichXmlNode node)
        {


            string data = "hi there";
            string name = node.Attribute("name");
            string refr = node.Attribute("ref");
            if (string.IsNullOrEmpty(name))
                name = refr;
            string type = node.Attribute("type");


            // --------------------------------------------------------------------------
            //  determine number of these elements
            // --------------------------------------------------------------------------
            int num = CreateNumberOfElements(node);
            if (num == 0
                && (name == "PID_1_SetIdPatientId"
                || name == "PID_2_PatientIdExternalId"
                || name == "PID_3_PatientIdInternalId"))
                Pause();


            for (int n = 0; n < num; ++n)
            {
                // ----------------------------------------------------------------------
                //  Look it up in the simpletypes or the complextypes list
                // ----------------------------------------------------------------------
                string lookupType = schema._LookupType(node.Node);
                switch (lookupType)
                {
                    case "complexNode":
                        //  go get that complex type node in the element
                        int count = node.RichChildNodes.Count;
                        if (!string.IsNullOrEmpty(refr))
                        {
                            node = schema._GetElement(refr);
                        }
                        foreach (RichXmlNode childNode in node.RichChildNodes)
                        {
                            string childType = childNode.Name;
                            switch (childNode.Name)
                            {
                                case "xs:complexType" :
                                    Cursor = AddNewElement(name, "").Node;
                                    CreateComplexType(schema, childNode);
                                    Cursor = Cursor.ParentNode;
                                    break;
                                default :
                                    Throws.A(new NotImplementedException("childNode.Name"), Throws.Actions, "P");
                                    break;
                            }
                        }
                        break;
                    case "SimpleType" :
                        //data = CreateSimpleType(schema, type, name);
                        string childSchemaFileName = schema._ChildSchemaName(type);
                        if (string.IsNullOrEmpty(childSchemaFileName))
                            data = CreateSimpleType(schema, schema._SimpleNode(type), name);
                        else
                        {
                            SimpleSchema childSchema = _schemas._ByFileName(childSchemaFileName);
                //if (Regex.IsMatch(name, "EVN_1_EventTypeCode"))
                //    _.BreakPoint();
                            data = CreateSimpleType(childSchema, schema._SimpleNode(type), name);
                        }
                        AddNewElement(name, data);
                        break;
                    case "ComplexType":
                        // --------------------------------------------------------------
                        //  Move to child schema if necessary
                        //  Get rid of prefixes on type
                        //  Add a new element and enter it
                        // --------------------------------------------------------------
                        Cursor = AddNewElement(name, "").Node;
                        SimpleSchema schema2 = MoveToChildSchemaIfNecessary(schema, type);
                        CreateComplexType(schema2, schema2._ComplexNode(type));
                        Cursor = Cursor.ParentNode;
                        break;
                    default:
                        data = CreateBaseType_nonrecursive(type, name);
                        AddNewElement(name, data);
                        //     Cursor.InnerText = data; // this does not work
                        break;
                }
            }


            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateExtension -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private string CreateExtension(SimpleSchema schema, RichXmlNode node)
        {
            string data = "CreateExtension";
            foreach (RichXmlNode childNode in node.RichChildNodes)
            {
                switch (childNode.Tag)
                {
                    case "xs:attribute" :
                        //data = CreateAttribute(schema, childNode);
                        data = CreateAttribute(schema, childNode.Attribute("name"), childNode.Attribute("type"));
                        break;
                    default :
                        Throws.A(new NotImplementedException("CreateExtension - " + childNode.Tag), Throws.Actions, "P");
                        break;
                }
            }
            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateExtension -->
        /// <summary>
        ///      ??Creates data based on the input schema data type???
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <param name="xNode"></param>
        /// <returns></returns>
        private string CreateExtension(SimpleSchema schema, RichXmlNode xNode, string name)
        {
            string data = "CreateExtension";


            if (Cursor.FirstChild != null)
            {
                // ----------------------------------------------------------------------
                //  Add some data as inner text to the current element
                // ----------------------------------------------------------------------
                string type = xNode.Attribute("base");
                if (schema._IsSimpleType(type))
                    data = CreateSimpleType(schema, schema._SimpleNode(type), name);
                else data = CreateBaseType_nonrecursive(type, name);  //  Assume that it is a base type
                Cursor.InnerText = data;
            }


            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateSequence -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="data"></param>
        /// <param name="xNode"></param>
        /// <returns></returns>
        private string CreateSequence(SimpleSchema schema, string data, RichXmlNode node)
        {
            if (node.HasChildNodes)
            {
                //string name2 = xNode.Attribute("name");
                foreach (RichXmlNode childNode in node.RichChildNodes)
                {
                    switch (childNode.Tag)
                    {
                        case "xs:element": data = CreateElement(schema, childNode); break;
                        case "xs:sequence": CreateSequence(schema, "", childNode); break;
                        default: throw new NotImplementedException("whack");
                    }
                            //if (childNode.Tag == "xs:element")
                            //    data = CreateElement(schema, childNode);
                            //else
                            //    throw new NotImplementedException("whack");
                }
            }
            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateSimpleContent -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private string CreateSimpleContent(SimpleSchema schema, RichXmlNode node)
        {
            string data = "CreateSimpleContent";
            foreach (RichXmlNode childNode in node.RichChildNodes)
            {
                switch (childNode.Tag)
                {
                    case "xs:extension" :
                        string type = childNode.Attribute("base");
                        data = CreateExtension(schema, childNode);
                        break;
                    default :
                        break;
                }
            }
            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateSimpleType -->
        /// <summary>
        ///      Creates simple type data
        /// </summary>
        /// <param name="simpleTypeNode"></param>
        /// <returns></returns>
        private string CreateSimpleType(SimpleSchema schema, RichXmlNode simpleTypeNode, string name)
        {
            if (simpleTypeNode == null)
                return "";
            Dictionary<string, List<string>> restrictions = new Dictionary<string,List<string>>();
            RichXmlNode xNode;
            RichXmlNode node;
            string type;
            string data = "";


            if (simpleTypeNode.HasChildNodes)
            {
                node = simpleTypeNode.RichChildNodes[0];
                xNode = node;
                type = xNode.Attribute("base");


                switch (type)
                {
                    case "xs:decimal"      : data = CreateDecimal(node); break;
                    case "xs:string"       :
                        if (Regex.IsMatch(node.OuterXml, "&lt;"))
                            Pause();
                        data = CreateString(node.Node, name);
                        break;
                    case "xs:anyType"      :
                    case "xs:anyURI"       :
                    case "xs:base64Binary" :
                    case "xs:boolean"      :
                    case "xs:byte"         :
                    case "xs:date"         : throw new NotImplementedException("You must implement Create"+type+"(node)");
                    case "xs:dateTime"     : data = CreateDateTime(name);  break;
                    case "xs:gday"         :
                    case "xs:double"       :
                    case "xs:duration"     :
                    case "xs:float"        :
                    case "xs:hexBinary"    :
                    case "xs:int"          : throw new NotImplementedException("You must implement Create"+type+"(node)");
                    case "xs:integer"      : data = CreateInteger(node);
                        throw new NotImplementedException("You must implement CreateInteger(node)");
                    case "xs:long"         :
                    case "xs:gMonth"       :
                    case "xs:gMonthDay"    : throw new NotImplementedException("You must implement Create"+type+"(node)");
                    case "xs:negativeInteger": throw new NotImplementedException("You must implement CreateNegativeInteger(node)");
                    case "xs:nonNegativeInteger":
                    case "xs:nonPositiveInteger":
                    case "xs:positiveInteger":
                    case "xs:short"        : throw new NotImplementedException("You must implement Create"+type+"(node)");
                    case "xs:time"         : CreateTime(node); break;
                    case "xs:unsignedByte" :
                    case "xs:unsignedInt"  :
                    case "xs:unsignedLong" :
                    case "xs:unsignedShort":
                    case "xs:gYear"        : throw new NotImplementedException("You must implement Create"+type+"(node)");
                    case "xs:gYearMonth"   : throw new NotImplementedException("You must implement CreateYearMonth(node)");
                    default:
                        // --------------------------------------------------------------
                        //  This means the restriction is a defined base type
                        //  1. get the namespace for that base type prefix
                        //  23. get the new schema
                        //  4. get the xmlnode from it
                        //  5. call recursively
                        // --------------------------------------------------------------
                        string childSchemaFileName = schema._ChildSchemaName(type);
                        SimpleSchema childSchema = _schemas._ByFileName(childSchemaFileName);
                        string childType = Regex.Replace(type, "^.*:", "");
                        data = CreateSimpleType(childSchema, childSchema._SimpleNode(childType), name);
                        break;
                }
            }
            else
                Throws.A(new NotSupportedException("CreateSimpleType - simpleTypeNode has no child nodes"), Throws.Actions, "P");


            if (data == "<")
                Throws.A(new FormatException("CreateSimpleType error -"
                    + " data: " + data + " is invalid data for xml text"), Throws.Actions, "P");


            return data;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- MoveToChildSchemaIfNecessary -->
        /// <summary>
        ///      Returns a child schema if the type is not in the current schema
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private SimpleSchema MoveToChildSchemaIfNecessary(SimpleSchema schema, string type)
        {
            string childSchemaName;
            childSchemaName = schema._ChildSchemaName(type);
            if (!string.IsNullOrEmpty(childSchemaName))
                schema = _schemas._ByFileName(childSchemaName);
            return schema;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- RefTargetTraversal -->
        /// <summary>
        ///      Goes does stuff
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="node"></param>
        /// <param name="prefix"></param>
        /// <param name="level"></param>
        private void RefTargetTraversal(SimpleSchema schema, RichXmlNode node, string prefix, int level)
        {
            RichXmlNode xNode = node;
            if (node.HasChildNodes) // elements
            {
                // ----------------------------------------------------------------------
                //  Recurse down one level
                // ----------------------------------------------------------------------
                if (level == 2)
                    schema._StoreRef(node.Node, prefix, xNode.Tag, xNode.Attribute("name"));
                foreach (RichXmlNode childNode in node.RichChildNodes)
                    RefTargetTraversal(schema, childNode, prefix, level + 1);
            }
            else
            {
                // ----------------------------------------------------------------------
                //  Tally any reference used as used
                // ----------------------------------------------------------------------
                schema._SetUsed(xNode.Attribute("ref"));
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FindInSchema_recursive -->
        /// <summary>
        ///      Finds the schema fragment which specifyies a particular element
        /// </summary>
        /// <param name="schemaFragment">the schema fragment in which we are looking</param>
        /// <param name="elementName">the name of the element we are looking for</param>
        /// <returns>returns a schema fragment node if found, otherwise null</returns>
        private RichXmlNode FindInSchema_recursive(RichXmlNode schemaFragment, string elementName)
        {
            RichXmlNode found = null;


            string name = schemaFragment.Attribute("name");
            if (elementName == name)
                found = schemaFragment;
            else
                // ----------------------------------------------------------------------
                //  Keep looking
                // ----------------------------------------------------------------------
                if (schemaFragment.HasChildNodes)
                    foreach (RichXmlNode childSchemaFragment in schemaFragment.RichChildNodes)
                    {
                        found = FindInSchema_recursive(childSchemaFragment, elementName);
                        if (found != null)  break;
                    }


            return found;
       }


        // ----------------------------------------------------------------------------------------
        /// <!-- TryCreateFirstElement -->
        /// <summary>
        ///      Add a new element to the xml and move into that element
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        private void TryCreateFirstElement(SimpleSchema schema, string name)
        {
            if (String.IsNullOrEmpty(name))
                Throws.A(new NotImplementedException("Can't create an element which has no name"), Throws.Actions, "P");
            else
            {
                // ----------------------------------------------------------------------
                //  Check to see if this is the first created node
                // ----------------------------------------------------------------------
                if (Cursor == null || Cursor.FirstChild == null)
                {
                    if (!schema._IsUsed(name))
                        Cursor = CreateFirstNode(schema, name).Node;
                }
                else
                {
                    Pause();
                    Cursor = AddNewElement(name, "").Node;
                }
            }
        }


        #endregion recursive methods

        #region utility methods

        // ----------------------------------------------------------------------------------------
        /// <!-- AddNewAttribute -->
        /// <summary>
        ///      Creates an attribute, Centralizes the creation of all attribute nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private XmlAttribute AddNewAttribute(string name, string data)
        { return NewAttribute(new RichXmlNode(Cursor), name, data); }
        private XmlAttribute NewAttribute(RichXmlNode node, string name, string data)
        {
            XmlAttribute attr = _newXml.CreateAttribute(name);
            attr.Value = data;
            node.Attributes.Append(attr);
            return attr;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- AddNewElement -->
        /// <summary>
        ///      Creates a new element node, Centralizes the creation of all element nodes
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="text">inner XML</param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        private RichXmlNode AddNewElement(string tag, string text)
            { return AddNewElement(tag, text, ""); }
        private RichXmlNode AddNewElement(string tag, string text, string prefix)
        {
            RichXmlNode node;
            if (!string.IsNullOrEmpty(tag) && text != "<")
            {
                //if (Regex.IsMatch(tag, "EVN_1_EventTypeCode"))
                //    _.BreakPoint();
                node = new RichXmlNode(_newXml.CreateNode(XmlNodeType.Element, prefix, tag, null));
                node.InnerXml = text;
                if (Cursor != null)
                    Cursor.AppendChild(node.Node);
                if (tag == "endeme")
                    Pause();
            }
            else
            {
                Throws.A(new NoNullAllowedException("AddNewElement - " + text), Throws.Actions, "P");
                node = null;
            }
            return node;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AttributeValue -->
        /// <summary>
        ///      Gets the value of a specified attribute if there is one
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        //private static string AttributeValue(LightXmlNode node, string attributeName)
        //{
        //    XmlAttributeCollection attributes = node.Attributes;
        //    string attr = "";


        //    // --------------------------------------------------------------------------
        //    //  Returns the value of the specified attribute if it exists in the list
        //    // --------------------------------------------------------------------------
        //    if (attributes != null)
        //        foreach (XmlAttribute attribute in attributes)
        //            if (attribute.Name == attributeName)
        //                attr = attribute.Value;


        //    return attr;
        //}


        // ----------------------------------------------------------------------------------------
        /// <!-- Clear -->
        /// <summary>
        ///      
        /// </summary>
        private void Clear()
        {
            _schemas._ClearCallers();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateBaseType -->
        /// <summary>
        ///      Creates data for the specified basic xml type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateBaseType_nonrecursive(string type, string name)
        {
            string data;
            Random r = RandomSource.New().Random;

            switch (type)
            {
                case "string"  : case "xs:string"  : data = CreatePrettyLabel(name);                                break;   // encapsulate
                case "dateTime": case "xs:dateTime": data = CreateDateTime(name);                                   break;
                case "date"    : case "xs:date"    : data = CreateDate(name);                                       break;
                case "xs:int"  : case "xs:integer" : data = (r.Next(int.MaxValue)-r.Next(int.MaxValue)).ToString(); break;
                case "xs:byte" : case "xs:short"   : data = (r.Next(128)).ToString();                               break;
                case "xs:anyType"           :
                case "xs:anyURI"            :
                case "xs:base64Binary"      :
                case "xs:boolean"           :
                case "xs:gday"              :
                case "xs:decimal"           :
                case "xs:double"            :
                case "xs:duration"          :
                case "xs:float"             :
                case "xs:hexBinary"         :
                case "xs:long"              :
                case "xs:gMonth"            :
                case "xs:gMonthDay"         :
                case "xs:negativeInteger"   :
                case "xs:nonNegativeInteger":
                case "xs:nonPositiveInteger":
                case "xs:positiveInteger"   :
                case "xs:time"              :
                case "xs:unsignedByte"      :
                case "xs:unsignedInt"       :
                case "xs:unsignedLong"      :
                case "xs:unsignedShort"     :
                case "xs:gYear"             :
                case "xs:gYearMonth"        :
                default:
                    data = "hi";
                    Throws.A(new NotImplementedException("Please implement "+type+" in AbstactXmGenerator.CreateBaseType_nonrecursive."), Throws.Actions, "P");
                    break;
                    //throw new NotSupportedException("Code not yet written"
                    //    + " to support base type"
                    //    + " " + type + ".");
            }

            return data;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateDecimal -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        private static string CreateDecimal(RichXmlNode node)
        {
            Dictionary<string, List<string>> restrictions = new Dictionary<string,List<string>>();
            restrictions = node.CollectRestrictions();


            Range range = new Range(0.0, 100.0, true, true);


            // --------------------------------------------------------------------------
            //  Unpack the restrictions
            // --------------------------------------------------------------------------
            foreach (string restriction in restrictions.Keys)
            {
                switch (restriction)
                {
                    case "xs:minExclusive": range.SetLow(restrictions[restriction][0],  false); break;
                    case "xs:minInclusive": range.SetLow(restrictions[restriction][0],  true);  break;
                    case "xs:maxExclusive": range.SetHigh(restrictions[restriction][0], false); break;
                    case "xs:maxInclusive": range.SetHigh(restrictions[restriction][0], true);  break;
                    case "xs:totalDigits":
                    case "xs:fractionDigits":
                    default:
                        throw new NotSupportedException("Code not yet written"
                            + " to support the decimal restriction"
                            + " " + restriction + ".");
                }
            }


            // --------------------------------------------------------------------------
            //  Use the restrictions
            // --------------------------------------------------------------------------
            string data = range.CreateNum(RandomSource.New().Random).ToString();


            return data;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateInteger -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string CreateInteger(RichXmlNode node)
        {
            _results += node.Name;
            Throws.A(new NotImplementedException("CreateInteger - is not implemented."), Throws.Actions, "P");
            return "1234";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateTime -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string CreateTime(RichXmlNode node)
        {
            _results += node.Name;
            Throws.A(new NotImplementedException("CreateTime - is not implemented."), Throws.Actions, "P");
            return "1753-01-01T12:00:00+00:00";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateString -->
        /// <summary>
        ///      Creates a string to meet a particular set of restrictions
        /// </summary>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        private string CreateString(XmlNode node, string name)
        {
            // --------------------------------------------------------------------------
            //  Get restrictions
            // --------------------------------------------------------------------------
            NodeCreationCommand cmd = new NodeCreationCommand(node);
            string str = CreateString(cmd, name);
            if (str == "<")
                Throws.A(new FormatException("ack data is " + str), Throws.Actions, "P");
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DrawGenerationalSuffix -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string DrawGenerationalSuffix()
        {
            RandomSource rs = RandomSource.New();
            if (rs.Random.Next(10) == 0)
            {
                List<string> suffix = new List<string>();
                suffix.Add("Sr");
                suffix.Add("Jr");
                suffix.Add("I");
                suffix.Add("II");
                suffix.Add("III");
                suffix.Add("IV");
                return rs.Draw(suffix);
            }
            else return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Initializes all of those members
        /// </summary>
        private void Init()
        {
            _newXml = new RichXmlDocument();
            _memento = new RichXmlDocument();
            _schemas = new RichXmlSchemaSet();
            _schemaResult = "";
            _started = false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ParentPrefix -->
        /// <summary>
        ///      Identifies the calling schema prefix
        /// </summary>
        /// <param name="fileNameSpace"></param>
        /// <returns></returns>
        private string ParentPrefix(string fileNameSpace)
        {
            string prefix = "";
            FileNameSpace ns = new FileNameSpace(fileNameSpace);


            if (_schemas._CallerListContainsFilename(ns.FileName))
            {
                string parentFileName = _schemas._CallerByFileName(ns.FileName);
                if (_schemas._ContainsFile(parentFileName))
                {
                    SimpleSchema parentSchema = _schemas._ByFileName(parentFileName);
                    prefix = parentSchema._Prefix(ns.NameSpace);
                }
                else
                    prefix = "";
            }
            else
                throw new KeyNotFoundException("file name '" + ns.FileName + "' not found in _callerByFileName dictionary");


            return prefix;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- XmlToString -->
        /// <summary>
        /// 
        /// </summary>
        public string XmlToString
        {
            get { return PrettyTraversal_manager(_newXml); }
        }

        #endregion utility methods

        #region abstract methods


        // ----------------------------------------------------------------------------------------
        //  Abstract methods
        // ----------------------------------------------------------------------------------------
        protected abstract string CreateDate(string name);
        protected abstract string CreateDateTime(string name);
        protected abstract int    CreateNumberOfElements(RichXmlNode node);
        protected abstract int    CreateNumElements(RichXmlNode node);
        protected abstract string CreatePrettyLabel(string name);
        protected abstract string CreateString(NodeCreationCommand cmd, string name);
        protected abstract string CreateTimeZone(Random r);
        protected abstract bool   IsDateTime(string name);
        protected abstract bool   IsDigitField(string name);
        protected abstract bool   IsIdField(string name);
        protected abstract bool   IsNumericField(string name);
        protected abstract string LblType(string name);


        #endregion abstract methods

    }
}
