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
using System.Data;                    // for many
using System.Data.Common;             // for DbDataAdapter
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- DataReaderAdapter -->
    /// <summary>
    ///      Implementation of DbDataAdapter
    /// </summary>
    public class DataReaderAdapter : DbDataAdapter
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- FillFromReader -->
        /// <summary>
        ///      Fills a DataTable from a data reader, implements DbDataAdapter.Fill, throws an exception if dataReader is null
        /// </summary>
        /// <param name="dataTable">the table to be filled</param>
        /// <param name="dataReader">an open reader</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public int FillFromReader(DataTable dataTable, IDataReader dataReader)
        {
            if (dataReader == null)
                throw new NoNullAllowedException("FillFromReader error - dataReader is null");
            return this.Fill(dataTable, dataReader);
        }


        protected override RowUpdatedEventArgs  CreateRowUpdatedEvent (DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) { return null; }
        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) { return null; }
        protected override void OnRowUpdated (RowUpdatedEventArgs  value) { }
        protected override void OnRowUpdating(RowUpdatingEventArgs value) { }
    }
}