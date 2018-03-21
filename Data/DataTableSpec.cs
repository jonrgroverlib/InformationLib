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

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- DataTableSpec -->
    /// <summary>
    ///      The DataTableSpec class specifies a data table using pre-existing classes
    /// </summary>
    /// <remarks>
    ///      unfinished, unused
    ///      the problem with this approach is that it may have a development ceiling
    ///      
    ///      alpha code
    /// </remarks>
    public class DataTableSpec
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private DataSet _spec;
        private List<string> _primaryKey;
        private List<string> _defaultSort;


        // ----------------------------------------------------------------------------------------
        //  Properties
        // ----------------------------------------------------------------------------------------
        public string Name { get { return _spec.Tables[0].TableName; } }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public DataTableSpec(string tableName, string primaryKey, Type type)
        {
            // --------------------------------------------------------------------------
            //  Define the main table
            // --------------------------------------------------------------------------
            DataTable table0 = new DataTable(tableName);
            table0.Columns.Add(primaryKey, type);
            table0.Columns[0].AllowDBNull = false;
            table0.Columns[0].Unique      = true;


            // --------------------------------------------------------------------------
            //  Define its specification value container
            // --------------------------------------------------------------------------
            _primaryKey  = new List<string>(); _primaryKey.Add (primaryKey);
            _defaultSort = new List<string>(); _defaultSort.Add(primaryKey);
            _spec = new DataSet(tableName+"Spec");
            _spec.Tables.Add(table0);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a column to the main table
        /// </summary>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Add(string column, Type type)
        {
            bool added = true;
            if (_spec.Tables[0].Columns.Contains(column)) added = false;
            else _spec.Tables[0].Columns.Add(column, type);
            return added;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds one spec to another
        /// </summary>
        /// <param name="table"></param>
        /// <param name="foreignKey"></param>
        public void Add(DataTableSpec table, string foreignKey)
        {
            int n = _spec.Tables.Count;
            DataTable tableN = new DataTable(table.Name);
            // add input spec items to spec
            // add input spec list to spec
        }
    }
}
