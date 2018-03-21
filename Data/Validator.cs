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
using InformationLib.HardData;        // for FileIO
using InformationLib.Strings;         // for __.many
using System;                         // for Exception, NotImplementedException
using System.Text.RegularExpressions; // for Regex, MatchCollection
using System.Xml;                     // for many
using System.Xml.Schema;              // for XmlSchemaSet, ValidationEventArgs

namespace InformationLib.Data
{
	// --------------------------------------------------------------------------------------------
	/// <!-- Validator -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public class Validator : XmlReader
    {
        private static bool _isValid = true;      // If a validation error occurs, set this flag
        //private string _schemaResult = "";        // to false in the validation event handler.


        // ----------------------------------------------------------------------------------------
        /// <!-- Errors, Results -->
        public string Errors  { get { return _errors;  } set { _errors  = value; } }  private string _errors;
        public string Results { get { return _results; } set { _results = value; } }  private string _results;


        XmlReader _reader;


        // ----------------------------------------------------------------------------------------
        /// <!-- Validator constructors -->
        public Validator() { /* default constructor */ }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="schemas"></param>
        public Validator(string filePath, XmlSchemaSet schemas)
        {
            XmlReaderSettings settings = new XmlReaderSettings();


            // -------------------------------------------------------------------------90
            //  Prepare validator settings
            // -------------------------------------------------------------------------90
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(XmlValidationEventHandler);
            settings.Schemas = schemas;


            // -------------------------------------------------------------------------90
            //  Create the validator which is acutally a reader in disguise
            // -------------------------------------------------------------------------90
            try
            {
                _reader = Create(new XmlTextReader(filePath), settings);
            }
            catch (Exception ex)
            {
                throw new Exception("bad schema set or xml - " + ex.Message);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Validate -->
        /// <summary>
        ///      Validates the XML File, Display whether the document is valid or invalid
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            if (Validates())  _results += "\r\nDocument is valid";
            else              _results += "\r\nDocument is invalid";
            return _results;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Validates -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool Validates()
        {
            _isValid = true;
            while (Read()) { /* Can add code here to process the content */ }
            Close();
            return _isValid;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- XmlFile -->
        /// <summary>
        ///      Validates an XML file with the specified schema
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="schemaPath"></param>
        /// <returns></returns>
        public static string XmlFile(string filePath, string schemaPath)
        {
            return (new Validator()).Validate(filePath, schemaPath);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- XmlString -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="schemaPath"></param>
        /// <returns></returns>
        public static string XmlString(string xml, string schemaPath)
        {
            if (xml == null || xml == "")
                return "xml string is null";
            FilesIO.WriteStringToFile("efghiopqr.xml", xml, false);
            return (new Validator()).Validate("efghiopqr.xml", schemaPath);
        }
        
        // ----------------------------------------------------------------------------------------
        /// <!-- Validate -->
        /// <summary>
        ///      Validates an XML file with the specified schema
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="schemaPath"></param>
        /// <returns></returns>
        private string Validate(string filePath, string schemaPath)
        {
            XmlSchemaSet schemas = RichXmlSchemaSet.SchemaSet(schemaPath);
            Validator reader = new Validator(filePath, schemas);
            return __.ConcatIf(_results, "\r\n", reader.Validate());
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- XmlValidationEventHandler -->
        /// <summary>
        ///      Handles XML validation failure events
        /// </summary>
        private void XmlValidationEventHandler(object o, ValidationEventArgs args) 
        {
            _isValid = false;
            _results += __.ConcatIf("\r\nValidation event\r\n", args.Message);
            __.BreakPoint();
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- SchemaValidationEventHandler -->
        ///// <summary>
        /////        Handles schema validation failure events
        ///// </summary>
        //public void SchemaValidationEventHandler(object o, ValidationEventArgs args)
        //{
        //    _schemaResult += _.Affix("\r\nSchema not valid:\r\n", args.Message);
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- Close -->
        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            if (_reader != null)
                _reader.Close();
            else
                throw new Exception("_reader is null in Validator.Close");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Validate -->
        /// <summary>
        ///      Validates an xml using a schema file
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="schemaFile"></param>
        /// <param name="hint1"></param>
        /// <param name="hint2"></param>
        /// <returns></returns>
        public string Validate(string xml, XmlSchemaSet schemas)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            _results = "";


            // -------------------------------------------------------------------------90
            //  Prepare validator settings
            // -------------------------------------------------------------------------90
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(XmlValidationEventHandler);
            settings.Schemas = schemas;


            // -------------------------------------------------------------------------90
            //  Run validator
            // -------------------------------------------------------------------------90
            FilesIO.WriteStringToFile(@"C:\temp.xml", xml, false);
            try   { _reader = Create(new XmlTextReader(@"C:\temp.xml"), settings); }
            catch { return __.ConcatIf(_results, "\r\n", "Document or schema set is invalid"); }
            return __.ConcatIf(_results, "\r\n", Validate());
        }
        //public string Validate(string xml, string schemaFile, string hint1, string hint2) { return Validate(xml, RichXmlSchemaSet.SchemaSet(schemaFile, hint1, hint2)); }
        public bool IsValid (XmlDocument xdoc)                 { return IsValid(xdoc.OuterXml, xdoc.Schemas); }
        public bool IsValid (string xml, XmlSchemaSet schemas) { string result = Validate(xml, schemas);  return (result == "\r\nDocument is valid"); }

        // ----------------------------------------------------------------------------------------
        /// <!-- Read -->
        /// <summary>
        ///      Reads the next node from the stream
        /// </summary>
        /// <returns></returns>
        public override bool Read()
        {
            if (_reader != null)
            {
                bool ok = false;
                try { ok = _reader.Read(); }
                catch (XmlException e) { ok = false; _errors = e.Message; _isValid = false; }
                catch (Exception    e) { ok = false; _errors = e.Message; _isValid = false; Type type = e.GetType(); }
                return ok;
            }
            else
            {
                _isValid = false;
                throw new XmlException("Validation reader is null");
            }
        }


        // ----------------------------------------------------------------------------------------
        //  Abstract Properties
        // ----------------------------------------------------------------------------------------
        public override int AttributeCount { get { throw new NotImplementedException("The property AttributeCount is not implemented."); } }
        public override int Depth          { get { throw new NotImplementedException("The property Depth is not implemented."); } }


        public override bool EOF            { get { throw new NotImplementedException("The property EOF is not implemented."); } }
        public override bool HasValue       { get { throw new NotImplementedException("The property HasValue is not implemented."); } }
        public override bool IsEmptyElement { get { throw new NotImplementedException("The property IsEmptyElement is not implemented."); } }


        public override string BaseURI      { get { throw new NotImplementedException("The property BaseURI is not implemented."); } }
        public override string LocalName    { get { throw new NotImplementedException("The property LocalName is not implemented."); } }
        public override string NamespaceURI { get { throw new NotImplementedException("The property NamespaceURI is not implemented."); } }
        public override string Prefix       { get { throw new NotImplementedException("The property Prefix is not implemented."); } }
        public override string Value        { get { throw new NotImplementedException("The property Value is not implemented."); } }


        public override XmlNameTable NameTable { get { throw new NotImplementedException("The property NameTable is not implemented."); } }
        public override XmlNodeType  NodeType  { get { throw new NotImplementedException("The property NodeType is not implemented."); } }
        public override ReadState    ReadState { get { throw new NotImplementedException("The property ReadState is not implemented."); } }


        // ----------------------------------------------------------------------------------------
        //  Abstract Methods
        // ----------------------------------------------------------------------------------------
        public override string GetAttribute   (string name, string namespaceURI){ throw new NotImplementedException("The method or operation is not implemented."); }
        public override string GetAttribute   (string name)  { throw new NotImplementedException("The method or operation is not implemented."); }
        public override string GetAttribute   (int i)        { throw new NotImplementedException("The method or operation is not implemented."); }
        public override string LookupNamespace(string prefix){ throw new NotImplementedException("The method or operation is not implemented."); }


        public override bool MoveToAttribute(string name, string ns){ throw new NotImplementedException("The method or operation is not implemented."); }
        public override bool MoveToAttribute(string name)  { throw new NotImplementedException("The method or operation is not implemented."); }
        public override bool MoveToElement  ()             { throw new NotImplementedException("The method or operation is not implemented."); }
        public override bool MoveToFirstAttribute()        { throw new NotImplementedException("The method or operation is not implemented."); }
        public override bool MoveToNextAttribute()         { throw new NotImplementedException("The method or operation is not implemented."); }
        public override bool ReadAttributeValue()          { throw new NotImplementedException("The method or operation is not implemented."); }
        public override void ResolveEntity  ()             { throw new NotImplementedException("The method or operation is not implemented."); }
    }
}
