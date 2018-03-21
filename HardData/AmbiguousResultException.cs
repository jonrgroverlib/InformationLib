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

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
	// --------------------------------------------------------------------------------------------
	/// <!-- AmbiguousResultException -->
    /// <summary>
    ///      Use this exception when a query which is supposed to return only one result, returns multiple results
    /// </summary>
    /// <remarks>production ready</remarks>
    public class AmbiguousResultException : Exception
    {
        private const string _defaultMessage = "Multiple data items retrieved where one item expected";


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public AmbiguousResultException()                            : base(_defaultMessage) { }
        public AmbiguousResultException(string msg)                  : base(msg)             { }
        public AmbiguousResultException(string msg, Exception inner) : base(msg, inner)      { }
    }
}
