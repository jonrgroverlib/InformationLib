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
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- FileNameSpace -->
    /// <summary>
    ///      The FileNameSpace class has 2 little strings containing:
    ///      1. the file name and 
    ///      2. the name space delimited by a colon excepting where there are neither
    /// </summary>
    /// <remarks>
    ///      deprecated - garbage
    ///      This class needs to be eliminated
    /// </remarks>
    public class FileNameSpace
    {
        private string _fileName;
        public  string FileName  { get { return _fileName;  } set { _fileName  = value; } }
        private string _nameSpace;
        public  string NameSpace { get { return _nameSpace; } set { _nameSpace = value; } }


        public FileNameSpace()
        {
            _fileName = "";
            _nameSpace = "";
        }


        public FileNameSpace(string fileNameSpace)
        {
            _fileName = Regex.Replace(fileNameSpace, "^([^:]+):.*$", "$1");
            _nameSpace = Regex.Replace(fileNameSpace, "^[^:]+:", "");
        }


        public FileNameSpace(string fileName, string nameSpace)
        {
            _fileName = fileName;
            _nameSpace = nameSpace;
        }


        public override string ToString()
        {
            if (_fileName == "" && _nameSpace == "") return "";
            else return _fileName + ":" + _nameSpace;
        }

    }
}
