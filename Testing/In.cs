//-------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation, either version 3
// of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//-------------------------------------------------------------------------------------------------
using System.Collections.Generic;     // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Testing  // add to this namespace very sparingly and use only System references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- In -->
    /// <summary>
    ///      The In class manipulates values residing in dictionaries
    /// </summary>
    /// <remarks>alpha code - experiment</remarks>
    public static class In
    {

        public static void crement(Dictionary<double, int> list, double key) { if (!list.ContainsKey(key)) try { list.Add(key, 0); } catch { } list[key]++; }
        public static void crement(Dictionary<int   , int> list, int    key) { if (!list.ContainsKey(key)) try { list.Add(key, 0); } catch { } list[key]++; }
        public static void crement(Dictionary<string, int> list, string key) { if (!list.ContainsKey(key)) try { list.Add(key, 0); } catch { } list[key]++; }

    }
}
