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
using System;                         // for Exception
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- MDIFormInstanceLimitException -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>old beta code</remarks>
    class MDIFormInstanceLimitException : ApplicationException
    {
        private const string _defaultMessage = "Too many MDI forms open."
            + " These instances are limited in order not to run out of system resources";


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public MDIFormInstanceLimitException()           : base(_defaultMessage) { }
        public MDIFormInstanceLimitException(string msg) : base(msg)             { }
        public MDIFormInstanceLimitException(string msg, Exception inner) : base(msg, inner) { }
    }
}
