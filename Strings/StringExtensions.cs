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
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- StringExtension -->
    /// <summary>
    ///      The StringExtension class contains string extension methods
    /// </summary>
    /// <remarks>alpha code</remarks>
    public static class StringExtension
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- ReverseString -->
        /// <summary>
        ///      Returns the reverse of the input string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(this string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetLength -->
        /// <summary>
        ///      Sets a string to a particular length, pads or truncates it as necessary
        /// </summary>
        /// <param name="str"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string SetLength(this string str, int n)
        {
            if (string.IsNullOrEmpty(str))
                return "".PadRight(n);
            if (str.Length > n)
                return str.Substring(0,n);
            else return str.PadRight(n);
        }
    }
}
