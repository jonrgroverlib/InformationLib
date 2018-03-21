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
using InformationLib.HardData;        // for FileIO
using InformationLib.Strings;         // for PathSlicer
using System;                         // for Exception
using System.Collections.Generic;     // for Dictionary, List, IEnumerable
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for XmlDocument
using System.Xml.Schema;              // for XmlSchemaSet

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RichXmlSchemaSet -->
    /// <summary>
    ///      The RichXmlSchemaSet class extends the XmlSchemaSet class
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public class RichXmlSchemaSet : XmlSchemaSet
    {
        private static string _staticSchemaResult = "";        // validation event handler. 
        private Dictionary<string, SimpleSchema> _schemasByFileName;


        // ----------------------------------------------------------------------------------------
        /// <!-- _ByFileName -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public SimpleSchema _ByFileName(string fileName)
        {
            SimpleSchema schema = null;


            if (schema == null) schema = TryFileName(fileName);
            if (schema == null) schema = TryFileName(fileName + ".xsd");
            if (schema == null) schema = TryFileName(Regex.Replace(fileName, "\\.xsd$", ""));
            if (schema == null)
                throw new Exception("schema not found in _schemasByFileName");


            return schema;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TryFileName -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tryMe"></param>
        /// <returns></returns>
        private SimpleSchema TryFileName(string tryMe)
        {
            SimpleSchema schema = null;
            if (_schemasByFileName.ContainsKey(tryMe))
                schema = _schemasByFileName[tryMe];
            return schema;
        }


        public bool _ContainsFile(string fileName) { return (_schemasByFileName.ContainsKey(fileName)); }


        private Dictionary<string, SimpleSchema> _schemasByNameSpace;
        public SimpleSchema _ByNamespace(string nSpace) { return _schemasByNameSpace[nSpace]; }
        public Dictionary<string,SimpleSchema>.KeyCollection Namespaces
        { get { return (Dictionary<string,SimpleSchema>.KeyCollection)_schemasByNameSpace.Keys; } }


        private Dictionary<string,string> _hashCallerByFileName;
        public void _AddCallerByFilename(string fileName, string parentFileName) { _hashCallerByFileName[fileName] = parentFileName; }
        public string _CallerByFileName(string fileName) { return _hashCallerByFileName[fileName]; }
        public bool _CallerListContainsFilename(string fileName) { return _hashCallerByFileName.ContainsKey(fileName); }


        public void _RegisterSchema(string fileName, string fileNameSpace, SimpleSchema schema)
        {
            _schemasByFileName[fileName] = schema;
            _schemasByNameSpace[fileNameSpace] = schema;
        }
        public void _ClearCallers() { _hashCallerByFileName.Clear(); }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public RichXmlSchemaSet()
        {
            _schemasByNameSpace = new Dictionary<string,SimpleSchema>();
            _schemasByFileName  = new Dictionary<string,SimpleSchema>();
            _hashCallerByFileName   = new Dictionary<string,string>();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CountCheck -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expected"></param>
        public static void CountCheck(XmlSchemaSet schemas, int expected)
        {
            if (schemas.Count != expected)
                throw new Exception("Schema set"
                    + " only has " + schemas.Count + " schema");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _FileNames -->
        /// <summary>
        ///      Lists the schema file names
        /// </summary>
        public List<string> _FileNames
        {
            get
            {
                List<string> list = new List<string>();
                foreach (XmlSchema schema in Schemas())
                    list.Add(Regex.Replace(schema.SourceUri, @"^.*[/\\]", ""));
                return list;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FixImport -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        private static void FixImport(string oldPath, string newPath)
        {
            string oldSchema = FilesIO.GetStringFromFile(oldPath, "");
            string newSchema = FixImport(oldSchema);
            newSchema = Regex.Replace(newSchema, "encoding=\"utf-16\"", "encoding=\"utf-8\"");
            FilesIO.WriteStringToFile(newPath, newSchema, false);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FixImport -->
        /// <summary>
        ///      Fixes the import line in a schema
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static string FixImport(string schema)
        {
            schema = Regex.Replace(schema, " schemaLocation=\"([^\"]+).xsd\"", " schemaLocation=\"$1\"");
            schema = Regex.Replace(schema, " schemaLocation=\"[^\"]+[.]([^\".]+)\"", " schemaLocation=\"$1\"");
            schema = Regex.Replace(schema, " schemaLocation=\"([^\"]+)\"", " schemaLocation=\"$1.xsd\"");
            return schema;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _ParentSchema -->
        /// <summary>
        ///      Identifies the calling schema
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public SimpleSchema _ParentSchema(SimpleSchema schema)
        {
            SimpleSchema parent;


            if (_CallerListContainsFilename(schema._FileName))
            {
                string fileName = _CallerByFileName(schema._FileName);
                if (_ContainsFile(fileName))
                    parent = _ByFileName(fileName);
                else
                    parent = null;
            }
            else
                throw new KeyNotFoundException("missing file '" + schema._FileName + "' in caller list _callerByFileName");


            return parent;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SchemaSet -->
        /// <summary>
        ///      Converts a schema into a schema set
        /// </summary>
        /// <param name="schemaFile"></param>
        /// <param name="hint1"></param>
        /// <param name="hint2"></param>
        /// <returns></returns>
        public static XmlSchemaSet SchemaSet(string schemaFile, string hint1, string hint2)
        {   return SchemaSet(PathSlicer.FindPath(schemaFile, hint1, hint2)); }
        public static XmlSchemaSet SchemaSet(string tempFilePath, string simpleSchema)
        {   FilesIO.WriteStringToFile(tempFilePath, simpleSchema, false);  return SchemaSet(tempFilePath); }
        public static XmlSchemaSet SchemaSet(string schemaPath)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            XmlSchema schema = new XmlSchema();
            _staticSchemaResult = "";
            bool found = false;

            // proprocess schema, put in temp, use that one

            string tempPath = __.NearPath(schemaPath, ".xsd", "_temp.xsd");
            FixImport(schemaPath, tempPath);
            if (!found) found = TryRead(tempPath, out schema);
            if (!found) found = TryRead(schemaPath, out schema);
            if (!found) found = TryRead(FixName(schemaPath), out schema);


            schemas.Add(schema);  // fails right here
            return schemas;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TryRead -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaPath"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static bool TryRead(string schemaPath, out XmlSchema schema)
        {
            XmlTextReader rdr = new XmlTextReader(schemaPath);
            bool found = false;


            schema = new XmlSchema();
            try { schema = XmlSchema.Read(rdr, StaticSchemaValidationEventHandler); found = true; }
            catch (Exception ex)
            {
                string error = ex.Message;
                __.BreakPoint();
            }
            finally { rdr.Close(); rdr = null; }


            return found;
        }
        
        // ----------------------------------------------------------------------------------------
        /// <!-- StaticSchemaValidationEventHandler -->
        /// <summary>
        /// 
        /// </summary>
        public static void StaticSchemaValidationEventHandler(object o, ValidationEventArgs args)
        {
            _staticSchemaResult += "\r\nSchema not valid" + __.ConcatIf(":\r\n", args.Message);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FixName -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partialFilePath"></param>
        /// <returns></returns>
        public static string FixName(string schemaPath)
        {
            schemaPath = Regex.Replace(schemaPath, "\\.xsd$", "");
            schemaPath = Regex.Replace(schemaPath, "^.*\\.([^.]+)$", "$1");
            schemaPath = Regex.Replace(schemaPath, "$", ".xsd");
            return schemaPath;
        }
    }
}
