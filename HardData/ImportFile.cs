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
using System.Data;                    // for 
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex
//using System.Threading.Tasks;         // for 

namespace InformationLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ImportFile -->
    /// <summary>
    ///      not presently used
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class ImportFile
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public string    FilePath       { get; set; }
        public DataTable ImportData     { get; set; } // one row per input row
        public DataTable StatusErrorLog { get; set; } // one row per error or successful load
        public DataTable ImportedData   { get; set; } // one row per successful load
        public DataTable InvalidData    { get; set; } // one row per input row with error columns appended
        public string    ContentType    { get; set; } // location, person, account etc.


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public ImportFile()
        {
            ImportedData   = new DataTable();
            InvalidData    = new DataTable();
            StatusErrorLog = new DataTable();


        }
        public ImportFile(string importFilePath)
        {

        }
    }
}
