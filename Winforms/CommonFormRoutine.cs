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
using System;                         // 
using System.Collections.Generic;     // 
using System.Data;                    // for DataTable
using System.Text;                    // 
using System.Text.RegularExpressions; // for Regex
using System.Windows.Forms;           // for ComboBox

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- CommonFormRoutine -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>stub</remarks>
    public static class CommonFormRoutine
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Fill -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="table"></param>
        /// <param name="valueField"></param>
        /// <param name="textField"></param>
        public static void Fill(ComboBox drop, DataTable table, string valueField, string textField)
        {
            drop.DataSource    = table;
            drop.DisplayMember = textField;
            drop.ValueMember   = valueField;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Prepend -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="value"></param>
        public static void Prepend(ComboBox drop, string text)
        {
            //object obj = dropInput2.Items[0];
            //Type typ = obj.GetType();
            //dropInput2.Items.Insert(0, "-SelectClient-");
        }
    }
}
