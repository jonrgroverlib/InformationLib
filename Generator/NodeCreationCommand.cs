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
using InformationLib.Data;            // for 
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for 

namespace InformationLib.Generator
{
	// --------------------------------------------------------------------------------------------
	/// <!-- NodeCreationCommand -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public class NodeCreationCommand
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Enumerations -->
        private List<string> _enumerations;
        /// <summary>
        /// 
        /// </summary>
        public List<string> Enumerations
        {
            get { return _enumerations; }
            set { _enumerations = value; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- MaxLength -->
        private int _maxLength;
        /// <summary>
        ///      Maximum length for the string to be created
        /// </summary>
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- MinLength -->
        private int _minLength;
        /// <summary>
        ///      Minimum length for the string to be created
        /// </summary>
        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- RegexPattern -->
        private string _regexPattern;
        /// <summary>
        ///      This is the regex pattern if one is specified
        /// </summary>
        public string RegexPattern
        {
            get { return _regexPattern; }
            set { _regexPattern = value; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- WhiteSpace -->
        private string _whiteSpace;
        /// <summary>
        /// 
        /// </summary>
        public string WhiteSpace
        {
            get { return _whiteSpace; }
            set { _whiteSpace = value; }
        }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public NodeCreationCommand() { Init(); }
        public NodeCreationCommand(XmlNode restrictionNode) { Init();  Fill(restrictionNode); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Sets the command to default values
        /// </summary>
        private void Init()
        {
            _enumerations = null;
            _maxLength    = -1;
            _minLength    = -1;
            _regexPattern = "";
            _whiteSpace   = "collapse";
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Fill -->
        /// <summary>
        ///      Fill a command from a node's restricitons
        /// </summary>
        /// <param name="restrictions"></param>
        /// <param name="cmd"></param>
        public void Fill(XmlNode node)
        {
            Dictionary<string, List<string>> restrictions = new Dictionary<string,List<string>>();
            RichXmlNode rxNode = new RichXmlNode(node);
            restrictions = rxNode.CollectRestrictions();


            // --------------------------------------------------------------------------
            //  Unpack the restrictions
            // --------------------------------------------------------------------------
            foreach (string item in restrictions.Keys)
            {
                string str = Regex.Replace(item, "^.*:", "");
                switch (item)
                {
                    case "xs:enumeration": Enumerations = restrictions[item];                    break;
                    case "xs:maxLength"  : MaxLength    = _gen_.Integer(restrictions[item][0], 840); break;
                    case "xs:minLength"  : MinLength    = _gen_.Integer(restrictions[item][0], 0);   break;
                    case "xs:pattern"    : RegexPattern = restrictions[item][0];                 break;
                    case "xs:whiteSpace" : WhiteSpace   = restrictions[item][0];                 break;
                    case "xs:length"     :
                    default:
                        throw new NotSupportedException("Code not yet written"
                            + " to support the string restriction"
                            + " " + item + ".");
                }
            }
        }


    }
}
