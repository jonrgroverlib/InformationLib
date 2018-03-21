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
using System;                         // for 
using System.Collections.Generic;     // for 
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.InfoAccess
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ClassInfo -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    /// <remarks>alpha code</remarks>
    public class ClassInfo
    {
        // ----------------------------------------------------------------------------------------
        //  Properties
        // ----------------------------------------------------------------------------------------
        public string       Name     { get {                 return _name;     } set { _name     = value; } } private string       _name;
        public List<string> Method   { get { InsureMethod(); return _method;   } set { _method   = value; } } private List<string> _method;
        public string       FileName { get {                 return _fileName; } set { _fileName = value; } } private string       _fileName;

        private void InsureMethod() { if ((_method == null)) { _method = new List<string>(); } } 


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public ClassInfo(string className, string filePath)
        {
            Name = className;
            FileName = filePath;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        public void Add(string methodName)
        {
            Method.Add(methodName);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddTo -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toThis"></param>
        /// <param name="addFromThis"></param>
        /// <remarks></remarks>
        public static void AddTo(List<ClassInfo> toThis, List<ClassInfo> addFromThis)
        {
            if (addFromThis.Count > 0) {
                foreach (ClassInfo listItem in addFromThis) {
                    toThis.Add(listItem);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Display -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Display()
        {
            StringBuilder str = new StringBuilder(Name + "  (" + Regex.Replace(FileName, "^.*[\\\\/]([^\\\\/]+)$", "$1") + ")");
            foreach (string item in Method) {
                str = str.Append("\r\n - " + item);
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + " (" + Method.Count + ")";
        }
    }
}
