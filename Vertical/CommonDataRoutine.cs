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
using InformationLib.Data    ;        // for RichDataTable
using InformationLib.Endemes ;        // for Throws
using InformationLib.SoftData;        // for TreatAs,TimeDate
using InformationLib.Strings ;        // for __.Pluralize, __.A
using System;                         // for Type, DateTime, Exception
using System.Collections.Generic;     // for Dictionary, List
using System.Data;                    // for DataTableCollection
using System.Linq;                    // for join
using System.Text.RegularExpressions; // for Regex, RegexOptions
using System.Web.SessionState;        // for HttpSessionState

namespace InformationLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- CommonDataRoutine -->
    /// <summary>
    ///      The CommonDataRoutine class is a repository for data access methods that use both the DAL and the InfoLib
    /// </summary>
    /// <remarks>deprecated</remarks>
    public class CommonDataRoutine
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- AddSsnFinal4 -->
        /// <summary>
        ///      Adds a column to the table contining the final N digits of the SSN
        /// </summary>
        /// <param name="table">the table to change</param>
        /// <param name="columnFrom">the current ssn column</param>
        /// <param name="columnTo">the (new) column to put the ssn final 4 in</param>
        /// <returns></returns>
        public static RichDataTable AddSsnFinalN(RichDataTable table, int num, string columnFrom, string columnTo)
        {
            table.Add(columnTo, typeof(string));
            for (int row = 0; row < table.Count; ++row)
            {
                if (num > 0)
                {
                    string str = table.StrValue(row, columnFrom, "");
                    //string lastX = Regex.Replace(str, "^.*(.{" + num + "})$", "$1");
                    string lastX = __.LastNChars(num, str);
                    table.Rows[row][columnTo] = lastX;
                }
            }
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FindDsmCodesLike -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        private static RichDataTable FindDsmCodesLike(string keyword, RichDataTable all)
        {
            int id = TreatAs.IntValue(keyword, 0);

            string whereClause = "    ConditionName    LIKE '%" + keyword + "%'"
                               + " OR ConditionCode4TR LIKE  '" + keyword + "%'"
                               + " OR ConditionCode5   LIKE  '" + keyword + "%'"
                               ;

            return all._Select(whereClause);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NameSourceList -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static RichDataTable NameSourceList()
        {
            RichDataTable source = new RichDataTable("DsmCodeSource", "Id");
            source.Add("Id"    , typeof(string));
            source.Add("Source", typeof(string));


            source.Add("Id", "DSM4"            , "Source", "Official DSM4"   );
            source.Add("Id", "DSMV"            , "Source", "Official DSMV"   );
            source.Add("Id", "Both DSM4 & DSMV", "Source", "Both DSM4 & DSMV");
            source.Add("Id", "Alternate"       , "Source", "Alternate"       );
            source.Add("Id", "BHAS App Input"  , "Source", "BHAS App Input"  );
            source.Add("Id", "Unknown"         , "Source", "Unknown"         );

            return source;
        }
    }
}
