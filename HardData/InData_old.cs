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
using System;                         // for many
using System.Collections.Generic;     // for Dictionary
using System.Data;                    // for DataTable, DataRow
using System.Data.SqlClient;          // for SqlConnection, SqlCommand, SqlDataReader
using System.Data.SqlTypes;           // for SqlDateTime, SqlInt32, SqlString
using System.IO;                      // for File, IOException, StramReader, StreamWriter, TextReader
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- InData -->
    /// <summary>
    ///      The beginning of a data oriented software development framework
    /// </summary>
    public class InData
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- SqlInjectionIdentified -->
        /// <summary>
        ///      Checks each cell in a row in a datatable for SQL injection
        /// </summary>
        /// <param name="field">the row</param>
        public static bool SqlInjectionIdentified(DataRow field)
        {
            bool injectionFound = false;
            for (int cell = 0; cell < field.Table.Columns.Count; ++cell)
                if (field[cell] != null && InData.DetectSqlInjection(field[cell].ToString()))
                    injectionFound = true;
            return injectionFound;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiNewQuery -->
        /// <summary>
        ///      Produces a parameterized sql query that can be dropped into SSMS query analyzer 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string AsciiNewQuery(SqlCommand command)
        {
            string db = command.Connection.Database;
            string query = " USE "+db+"\r\n"+"\r\n";
            string delim = "";


            foreach (SqlParameter param in command.Parameters)
            {
                // ----------------------------------------------------------------------
                //  Build the DECLARE statment
                // ----------------------------------------------------------------------
                string typeName  = param.SqlDbType.ToString()      ;
                string paramName = param.ParameterName.PadRight(22);
                switch (typeName)
                {
                    case "Date"     : query += delim + " DECLARE " + paramName + " AS "  + " Date"         .PadRight(17) + ";"; break;
                    case "DateTime" : query += delim + " DECLARE " + paramName + " AS "  + " DateTime"     .PadRight(17) + ";"; break;
                    case "NVarChar" : query += delim + " DECLARE " + paramName + " AS "  + " NVarChar(MAX)".PadRight(17) + ";"; break;
                    case "VarChar"  : query += delim + " DECLARE " + paramName + " AS "  + " VarChar(MAX)" .PadRight(17) + ";"; break;
                    default         : query += delim + " DECLARE " + paramName + " AS  " +   typeName      .PadRight(16) + ";"; break;
                }


                // ----------------------------------------------------------------------
                //  Resolve teh set values
                // ----------------------------------------------------------------------
                string valueStr  = param.SqlValue.ToString()       ;
                if (typeName == "NVarChar")
                {
                    valueStr = Regex.Replace(valueStr, "'", "''");
                }
                if (typeName == "Bit")
                {
                    if (valueStr == "True" ) valueStr = "1";
                    if (valueStr == "False") valueStr = "0";
                }


                // ----------------------------------------------------------------------
                //  Build the SET statment
                // ----------------------------------------------------------------------
                if (valueStr == "Null") { query += " SET " + paramName + " =  " + "Null"        ; }
                else switch (typeName)  {
                        case "Bit"      : query += " SET " + paramName + " =  " + valueStr      ; break;
                        case "Int"      : query += " SET " + paramName + " =  " + valueStr      ; break;
                        case "DateTime" : query += " SET " + paramName + " = '" + valueStr + "'"; break;
                        case "NVarChar" : query += " SET " + paramName + " = '" + valueStr + "'"; break;
                        case "VarChar"  : query += " SET " + paramName + " = '" + valueStr + "'"; break;
                        default: Pause(); string str = typeName;
                            break;
                    }

                delim = "\r\n";
            }

            query += delim + command.CommandText;

            return query;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsValue -->
        /// <summary>
        ///      Checks whether a DataTable column contains a particular value
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsValue(DataTable table, string column, object value)
        {
            if (table.Columns.Contains(column))
            {
                Type type = value.GetType();
                if (type == typeof(bool)    || type == typeof(bool?)    || type == typeof(Boolean) || type == typeof(SqlBoolean) ||
                    type == typeof(byte)    || type == typeof(byte?)    || type == typeof(Byte)    || type == typeof(SqlByte)    ||
                    type == typeof(short)   || type == typeof(short?)   || type == typeof(Int16)   || type == typeof(SqlInt16)   ||
                    type == typeof(int)     || type == typeof(int?)     || type == typeof(Int32)   || type == typeof(SqlInt32)   ||
                    type == typeof(long)    || type == typeof(long?)    || type == typeof(Int64)   || type == typeof(SqlInt64)   ||
                    type == typeof(sbyte)   || type == typeof(sbyte?)   || type == typeof(SByte)   ||
                    type == typeof(ushort)  || type == typeof(ushort?)  || type == typeof(UInt16)  ||
                    type == typeof(uint)    || type == typeof(uint?)    || type == typeof(UInt32)  ||
                    type == typeof(ulong)   || type == typeof(ulong?)   || type == typeof(UInt64)  ||
                    type == typeof(float)   || type == typeof(float?)   || type == typeof(Single)  || type == typeof(SqlSingle)  ||
                    type == typeof(double)  || type == typeof(double?)  || type == typeof(Double)  || type == typeof(SqlDouble)  ||
                    type == typeof(decimal) || type == typeof(decimal?) || type == typeof(Decimal) || type == typeof(SqlDecimal) || type == typeof(SqlMoney)
                   )
                {
                    DataRow[] field = table.Select(column + " = " + value.ToString());
                    return (field.Length > 0);
                }
                else
                {
                    if (type == typeof(DateTime) || type == typeof(SqlDateTime))
                    {
                        DataRow[] field = new DataRow[0];
                        try
                        {
                            field = table.Select(column + " = '" + value.ToString() + "'");
                        }
                        catch { }
                        return (field.Length > 0);
                    }
                    else
                    {
                        DataRow[] field = table.Select(column + " = '" + value.ToString() + "'");
                        if (field.Length == 0)
                            field = table.Select(column + " = '''" + value.ToString() + "'");
                        return (field.Length > 0);
                    }
                }
            }
            else
            {
                return false;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DatabaseIdentity -->
        /// <summary>
        ///      Extract a user-relevant idenity for the database from the connection string
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string DatabaseIdentity(string conn)
        {
            string database = ";" + conn + ";";

            // --------------------------------------------------------------------------
            //  Strip out irrelevant and sensistive information from the connectin string
            // --------------------------------------------------------------------------
            database = Regex.Replace(database, "; *password[^;]*;", ";", RegexOptions.IgnoreCase);
            database = Regex.Replace(database, "; *user id[^;]*;", ";", RegexOptions.IgnoreCase);
            database = Regex.Replace(database, "; *persist security info[^;]*;", ";", RegexOptions.IgnoreCase);
            database = Regex.Replace(database, "^;+", "", RegexOptions.IgnoreCase);
            database = Regex.Replace(database, ";+$", "", RegexOptions.IgnoreCase);


            // --------------------------------------------------------------------------
            //  Make double sure the password is not included
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(database, "password", RegexOptions.IgnoreCase))
                database = "";
            return database;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DeleteUpTo -->
        /// <summary>
        ///      Deletes up to a certain number of items from the table and filter included in the
        ///      fromWhereClause, deletes nothing if it would delete more than the specified number
        /// </summary>
        /// <param name="maxDeleteCount"></param>
        /// <param name="fromWhereClause"></param>
        /// <param name="connection"></param>
        public static void DeleteUpTo(int maxDeleteCount, string fromWhereClause, SqlConnection connection)
        {
            // --------------------------------------------------------------------------
            //  Count the number of rows that would be be deleted
            // --------------------------------------------------------------------------
            string countQuery = "SELECT COUNT(*) " + fromWhereClause;
            int count = 0;
            using (SqlCommand cmdCount = new SqlCommand(countQuery, connection))
            {
                object obj = cmdCount.ExecuteScalar();
                if (obj != null && !int.TryParse(obj.ToString(), out count))
                    count = 0;
            }


            // --------------------------------------------------------------------------
            //  Delete the rows if the number is reasonable
            // --------------------------------------------------------------------------
            if (0 < count && count <= maxDeleteCount)
            {
                string deleteQuery = "DELETE " + fromWhereClause;
                using (SqlCommand cmdDelete = new SqlCommand(deleteQuery, connection))
                {
                    cmdDelete.ExecuteNonQuery();
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DetectSqlInjection -->
        /// <summary>
        ///      A fairly simple data oriented attempt to resist sql injection in query parameters
        /// </summary>
        /// <param name="output"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static bool DetectSqlInjection(string str)
        {
            string output = str;

            output = Regex.Replace(output, "(/[*]|[*]/)"                                                 , "");
            output = Regex.Replace(output, "xp_(cmdshell|enumgroups|grantlogin|logevent|loginconfig|logininfo|msver|revokelogin|sprintf|sqlmaint|sscanf)", "", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, "(ASCII|CHAR)[ (]+[^()]+[)]"                                  , ""        , RegexOptions.IgnoreCase);
            output = Regex.Replace(output, "(sys|sysx)(column|message|object|process|server|login)(s|es)", "$1 $2 $3", RegexOptions.IgnoreCase);

            output = Regex.Replace(output, "(union)(.*)(select)"                                         , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(alter|create|drop|truncate)(.*)(role|table|view)"           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(grant|revoke)(.*)(insert|select|update|delete| all)"        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(delete|select)(.*)(from|top|[(][)])"                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(insert|update)(.*)(into|set|values)"                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(order) +(by)"                                               , "'$1$2'"  , RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(cmd|exec|execute)(.*)(cmd|dbo|execute|master|sys|xp_|sp_)"  , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(;)(.*)(shutdown)"                                           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            output = Regex.Replace(output, "(where)(.*)( like )"                                         , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            output = Regex.Replace(output, "[-]+" , "-"       );
            output = Regex.Replace(output, "@+"   , "@"       );
            output = Regex.Replace(output, @"[\\]", ""        );

            if (output != str)
                Pause();
            return (output != str);
        }

        private static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractIntValue_defaultNull -->
        /// <summary>
        ///      Extracts integer value from extract or returns null on nonnumeric string
        /// </summary>
        /// <param name="field"></param>
        /// <param name="extractColumn"></param>
        /// <param name="loadLength"></param>
        /// <returns></returns>
        public static SqlInt32 ExtractIntValue_defaultNull(DataRow field, string extractColumn, int loadLength)
        {
            SqlString strValue = InData.Truncate(InData.GetSqlString(field, extractColumn), loadLength);
            SqlInt32  intValue = SqlInt32.Null;
            if (!strValue.IsNull)
            {
                if (Regex.IsMatch(strValue.ToString().Trim(), "^[-+0-9]+$"))
                {
                    intValue = SqlInt32.Parse(strValue.ToString().Trim());
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(strValue.ToString()))
                    {
                        intValue = SqlInt32.Null;
                    }
                }
            }
            return intValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractIntValue_defaultMinusOne -->
        /// <summary>
        ///      Extracts integer value from extract or returns -1 on nonnumeric string
        /// </summary>
        /// <param name="field"></param>
        /// <param name="extractColumn"></param>
        /// <param name="loadLength"></param>
        /// <returns></returns>
        public static SqlInt32 ExtractIntValue_defaultMinusOne(DataRow field, string extractColumn, int loadLength)
        {
            SqlString str = InData.Truncate(InData.GetSqlString(field, extractColumn), loadLength);
            SqlInt32 intValue = SqlInt32.Null;
            if (!str.IsNull)
            {
                if (Regex.IsMatch(str.ToString().Trim(), "^[-+0-9]+$"))
                {
                    intValue = SqlInt32.Parse(str.ToString().Trim());
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(str.ToString()))
                    {
                        intValue = -1;
                    }
                }
            }
            return intValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetDictionaryFrom -->
        /// <summary>
        ///      Converts two columns of a database table into a Dictionary<int,int> structure
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyColumn"></param>
        /// <param name="valueColumn"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static Dictionary<int, int> GetDictionaryFrom(string tableName, string keyColumn
            , string valueColumn, SqlConnection connection)
        {
            Dictionary<int,int> list = new Dictionary<int,int>(10);
            string query
                = " SELECT " + keyColumn + ", " + valueColumn
                + " FROM "   + tableName
                + " WHERE "  + valueColumn + " IS NOT NULL";


            try
            {
                var command = new SqlCommand(query, connection);
                DataTable table = new DataTable();
                using (command)
                {
                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);
                }
                for (int row = 0; row < table.Rows.Count; ++row)
                {
                    int key = 0;
                    int value = 0;
                    if (int.TryParse(table.Rows[row][keyColumn  ].ToString(), out key  ) &&
                        int.TryParse(table.Rows[row][valueColumn].ToString(), out value))
                    list.Add(key, value);
                }
            }
            catch (SqlException ex)
            {

                Pause(); 
                if (ex.InnerException != null)
                    throw new Exception("Sql connection having trouble? - " + ex.Message + " - " + ex.InnerException.Message, ex);
                else
                    throw new Exception("Sql connection having trouble? - " + ex.Message, ex);
            }
            catch (Exception ex) { Pause(); throw new Exception(ex.Message, ex); }

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetOldestSqlDateTime -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="column"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static SqlDateTime GetOldestSqlDateTime(string query, string column, SqlConnection connection)
        {
            SqlDateTime date = SqlDateTime.Null;


            // --------------------------------------------------------------------------
            //  Code should be beautiful
            // --------------------------------------------------------------------------
            using             (SqlCommand        command = new SqlCommand(query, connection))
            {   using         (SqlDataReader     reader  = command.ExecuteReader          ())
                {   using     (DataReaderAdapter adapter = new DataReaderAdapter          ())
                    {   using (DataTable         table   = new DataTable                  ())
                        {
                            adapter.FillFromReader(table, reader);
                            if (table.Rows.Count > 0)
                            {
                                date = InData.GetSqlDateTime(table.Rows[0], column);
            }   }   }   }   }


            return date;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetRandomRowId -->
        /// <summary>
        ///      Returns the id of a barely 'random' row in the table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tablePk"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static SqlInt32 GetRandomRowId(string tableName, string tablePk, SqlConnection connection) // TAG
        {
            SqlInt32 id = SqlInt32.Null;

            // ----------------------------------------------------------------------
            //  Grab an existing Id
            // ----------------------------------------------------------------------
            string query = " SELECT TOP 1 "+tablePk+" FROM "+tableName;
            SqlCommand command = new SqlCommand(query, connection);
            object obj = command.ExecuteScalar();


            // ----------------------------------------------------------------------
            //  Complain if there are no ids (and therefore an empty table)
            // ----------------------------------------------------------------------
            if (obj != null)
                id = (int)obj;
            else
                throw new RowNotInTableException("GetRandomRowId - The "+tableName+" table is empty.");

            return id;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlBoolean -->
        /// <summary>
        ///      Returns false for "0" or "false", true for "1" or "true" and SqlBoolean.Null otherwise
        /// </summary>
        /// <param name="field"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SqlBoolean GetSqlBoolean(DataRow field, string column)
        {
            SqlBoolean value = SqlBoolean.Null;

            if (field.Table.Columns.Contains(column))
            {
                if (field[column] != null)
                {
                    string str = field[column].ToString();
                    bool bit = false;

                    if      (str == "0")                  value = false;
                    else if (str == "1")                  value = true;
                    else if (bool.TryParse(str, out bit)) value = bit;
                }
            }
            else
            {
                throw new System.Data.EvaluateException("Column '"+column+"' not found in DataTable '"+field.Table.TableName+"'.");
            }

            return value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlBoolean -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetSqlBoolean(object value)
        {
            SqlBoolean output = SqlBoolean.Null;

            if (value != null)
            {
                string str = value.ToString();

                bool bit = false;
                if      (str == "0")                  output = false;
                else if (str == "1")                  output = true;
                else if (bool.TryParse(str, out bit)) output = bit;
            }

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlDateTime -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SqlDateTime GetSqlDateTime(DataRow field, string column)
        {
            SqlDateTime outDate = SqlDateTime.Null;

            if (field.Table.Columns.Contains(column))
            {
                if (field[column] != null)
                {
                    DateTime date = new DateTime();
                    string strDate = field[column].ToString();
                    strDate = Regex.Replace(strDate, "^'", "");
                    if (!string.IsNullOrWhiteSpace(strDate))
                    {
                        if (DateTime.TryParse(strDate, out date))
                        {
                            outDate = date;
                        }
                        else
                            Pause();
                    }
                }
            }
            else
            {
                throw new System.Data.EvaluateException("Column '"+column+"' not found in DataTable '"+field.Table.TableName+"'.");
            }

            return outDate;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlDateTime -->
        /// <summary>
        ///      Convert whatever to a SqlDateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>TODO: use this method in GetSqlDateTime(DataRow field, string column) above</remarks>
        public static SqlDateTime GetSqlDateTime(object value)
        {
            SqlDateTime outDate = SqlDateTime.Null;

            DateTime date = new DateTime();
            string strDate = value.ToString();
            strDate = Regex.Replace(strDate, "^'", "");
            if (!string.IsNullOrWhiteSpace(strDate))
            {
                if (DateTime.TryParse(strDate, out date))
                {
                    outDate = date;
                }
                else
                    Pause();
            }

            return outDate;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlInt32 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SqlInt32 GetSqlInt32(DataRow field, string column)
        {
            SqlInt32 num = SqlInt32.Null;
            int foundNum = 0;

            if (field.Table.Columns.Contains(column))
            {
                object obj = field[column];
                if (obj != null && Int32.TryParse(obj.ToString(), out foundNum))
                    num = foundNum;
            }
            else
            {
                throw new System.Data.EvaluateException("Column '"+column+"' not found in DataTable '"+field.Table.TableName+"'.");
            }

            return num;
        }
        public static SqlInt32 GetSqlInt32(DataTable table, int row, string column)
        { SqlInt32 num = SqlInt32.Null; if (table != null && table.Rows.Count > row) num = GetSqlInt32(table.Rows[row], column); return num; }

        public static SqlInt32 GetSqlInt32(object obj)
        {
            SqlInt32 num = SqlInt32.Null;
            int foundNum = 0;
            if (obj != null && Int32.TryParse(obj.ToString(), out foundNum))
                num = foundNum;
            return num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlInt32 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static SqlInt32 GetSqlInt32(string query, SqlConnection connection)
        {
            SqlInt32 output = SqlInt32.Null;
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                object obj = command.ExecuteScalar();
                int num;
                if (obj != null && int.TryParse(obj.ToString(), out num))
                {
                    output = num;
                }
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlInt32 -->
        /// <summary>
        ///      Runs a scalar query returning an integer that has parameters
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameterList"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static SqlInt32 GetSqlInt32(string query, List<SqlParameter> parameterList, SqlConnection connection)
        {
            SqlInt32 output = SqlInt32.Null;

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.CommandType = CommandType.Text;
                foreach (SqlParameter parameter in parameterList) cmd.Parameters.Add(parameter);
                object obj = cmd.ExecuteScalar();
                int num; if (obj != null && int.TryParse(obj.ToString(), out num)) output = num;
            }

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlInt32 -->
        /// <summary>
        ///      Gets a value from a row given a selection table, column and value
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rowSelectionColumn"></param>
        /// <param name="rowSelectionValue"></param>
        /// <param name="valueColumn"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static SqlInt32 GetSqlInt32(string tableName, string rowSelectionColumn
            , SqlInt32 rowSelectionValue, string valueColumn, SqlConnection connection)
        {
            string query
                = " SELECT TOP 1 "+valueColumn
                + " FROM "+tableName
                + " WHERE "+rowSelectionColumn+" = "+rowSelectionValue+"";
            DataTable table = InData.GetTable(tableName, query, connection);


            SqlInt32 output = SqlInt32.Null;
            if (table.Rows.Count > 0)
            {
                valueColumn = Regex.Replace(valueColumn, @"[\[\]]", "");
                output = InData.GetSqlInt32(table.Rows[0], valueColumn);
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlInt64 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SqlInt64 GetSqlInt64(DataRow field, string column)
        {
            SqlInt64 num = SqlInt64.Null;
            Int64 foundNum = 0;

            if (field.Table.Columns.Contains(column))
            {
                object obj = field[column];
                if (obj != null && Int64.TryParse(obj.ToString(), out foundNum))
                    num = foundNum;
            }
            else
            {
                throw new System.Data.EvaluateException("Column '"+column+"' not found in DataTable '"+field.Table.TableName+"'.");
            }

            return num;
        }
        public static SqlInt64 GetSqlInt64(DataTable table, int row, string column)
        { SqlInt64 num = SqlInt64.Null; if (table != null && table.Rows.Count > row) num = GetSqlInt64(table.Rows[row], column); return num; }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlString -->
        /// <summary>
        ///      Extracts the specified column from the row
        /// </summary>
        /// <param name="field"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static SqlString GetSqlString(DataRow field, string column)
        {
            SqlString str = SqlString.Null;

            if (field.Table.Columns.Contains(column))
            {
                object obj = field[column];
                if (obj != null)
                    str = obj.ToString();
            }
            else
            {
                throw new System.Data.EvaluateException("Column '"+column+"' not found in DataTable '"+field.Table.TableName+"'.");
            }

            return str;
        }
        public static SqlString GetSqlString(DataTable table, int row, string column)
        { SqlString str = SqlString.Null; if (table != null && table.Rows.Count > row) str = GetSqlString(table.Rows[row], column); return str; }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSqlString -->
        /// <summary>
        ///      Conversts whatever to a sql string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlString GetSqlString(object value)
        {
            SqlString str = SqlString.Null;
            if (value != null)
                str = value.ToString();
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetStringFromFile -->
        /// <summary>
        ///      Gets a non-humongous string from a file
        /// </summary>
        /// <param name="filePath"></param>
        public static string GetStringFromFile(string filePath)
        {
            string content = "";

            try
            {
                using (TextReader reader = new StreamReader(filePath))
                {
                    try
                    {
                        //System.Runtime.Remoting.ObjRef refer = reader.CreateObjRef(typeof(String));
                        content = reader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("GetStringFromFile:ReadToEnd - " + ex.Message, ex);
                    }
                    finally
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetStringFromFile:StreamReader - " + ex.Message, ex);
            }

            return content;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetTable -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="query"></param>
        /// <param name="parameterList"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static DataTable GetTable(string tableName, string query, List<SqlParameter> parameterList, SqlConnection connection)
        {
            DataTable table = new DataTable(tableName);

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.CommandType = CommandType.Text;
                foreach (SqlParameter parameter in parameterList) cmd.Parameters.Add(parameter);
                using (SqlDataReader reader = cmd.ExecuteReader())
                    { if (reader.HasRows) (new DataReaderAdapter()).FillFromReader(table, reader); }
            }

            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetTable -->
        /// <summary>
        ///      Runs a select query, does not provide sql injection resistance
        /// </summary>
        /// <param name="newTableName">what do you want to call the table returned</param>
        /// <param name="query"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static DataTable GetTable(string newTableName, string query, SqlConnection connection)
        {
            DataTable table = new DataTable(newTableName);

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        (new DataReaderAdapter()).FillFromReader(table, reader);
                    }
                }
            }

            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsNull -->
        /// <summary>
        ///      Determines whether an object passed to it is some form of null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNull(object value)
        {
            Type type = value.GetType();

            switch (type.Name)
            {
                case "SqlInt32"  : return (                 ((SqlInt32 )value).IsNull); break;
                case "SqlString" : return (value == null || ((SqlString)value).IsNull); break;
                default          : return (value == null                             ); break;
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- KeyWithHighestValue -->
        /// <summary>
        ///      Returns the most frequent key in a tally dictionary
        /// </summary>
        /// <param name="tally">a tally dictionary</param>
        /// <returns></returns>
        /// <remarks>see TallyIntColumn</remarks>
        public static int KeyWithHighestValue(Dictionary<int,int> tally)
        {
            if (tally.Count > 0)
            {
                int mostCommon = 0;
                int frequency = 0;
                foreach (int key in tally.Keys)
                    if (tally[key] > frequency)
                    {
                        mostCommon = key;
                        frequency = tally[key];
                    }
                return mostCommon;
            }
            else return 0;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MostCommon -->
        /// <summary>
        ///      Returns the most common value of an integer column
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int MostCommon(string column, DataTable table)
        {
            return InData.KeyWithHighestValue(InData.TallyIntColumn(table, column));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PathLeaf -->
        /// <summary>
        ///      Returns the leaf segment of a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PathLeaf(string path)
        {
            return Regex.Replace(path, @"^.*[\\/]([^\\/]+)$", "$1");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PathWhack -->
        /// <summary>
        ///      Removes the leaf segment of a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PathWhack(string path)
        {
            return Regex.Replace(path, @"[\\/][^\\/]+$", "");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PrependToFileName -->
        /// <summary>
        ///      Prepends a string to a file name in a  path, assumes a three character extension
        /// </summary>
        /// <param name="prepend"></param>
        /// <param name="oldPath"></param>
        /// <returns></returns>
        public static string PrependToFileName(string prepend, string oldPath)
        {
            string newPath = Regex.Replace(oldPath, @"([\\/])([^\\/]+[.]...)$", "$1"+prepend+"$2");
            if (newPath == oldPath)
                throw new ArgumentException("Use a file with an extension having three characters.");
            return newPath;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ResistSqlInjection -->
        /// <summary>
        ///      A fairly simple attempt to resist sql injection in dynamic queries
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static string ResistSqlInjection(string str, int maxLen)
        {
            if (maxLen > 0 && str.Length > maxLen)
                str = str.Substring(0,maxLen);
            str = Regex.Replace(str, "(/[*]|[*]/)", "");

            str = Regex.Replace(str, "(xp_cmdshell|xp_enumgroups|xp_grantlogin|xp_logevent|xp_loginconfig|xp_logininfo|xp_msver|xp_revokelogin|xp_sprintf|xp_sqlmaint|xp_sscanf)", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "(ASCII|CHAR)[ (]+[^()]+[)]", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "(sys|sysx)(column|message|object|process|server|login)(s|es)", "$1 $2 $3", RegexOptions.IgnoreCase);

            str = Regex.Replace(str, "(union)(.*)(select)"                                         , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(alter|create|drop|truncate)(.*)(role|table|view)"           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(grant|revoke)(.*)(insert|select|update|delete|all)"         , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(delete|select)(.*)(from|top|[(][)])"                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(insert|update)(.*)(into|set|values)"                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //str = Regex.Replace(str, "(and|if|or)( .* )(=|<>|or|and|else|exists|like|select)"    , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(order)( .*|.* )(by)"                                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(cmd|exec|execute)(.*)(cmd|dbo|execute|master|sys|xp_|sp_)"  , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(;)(.*)(shutdown)"                                           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(where)(.*)(like)"                                           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            str = Regex.Replace(str, "[-]+" , "-" );
            str = Regex.Replace(str, "@+"   , "@" );
            //str = Regex.Replace(str, " = "  , " equals ");
            str = Regex.Replace(str, @"[\\]", ""  );
            str = Regex.Replace(str, "'+"   , "''");

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SimpleRetreive -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static DataTable SimpleRetreive(SqlCommand cmd, SqlConnection connection)
        {
            DataTable table = new DataTable();

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        DataReaderAdapter adapter = new DataReaderAdapter();
                        table = new DataTable();
                        adapter.FillFromReader(table, reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SimpleRetreive - " + ex.Message, ex);
            }

            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Sort -->
        /// <summary>
        ///      Returns the DataTable after being sorted by passing it through a DataView.
        /// </summary>
        /// <param name="orderByClause">may include 'ORDER BY'</param>
        public static DataTable Sort(DataTable table, string orderByClause)
        {
            try
            {
                // ----------------------------------------------------------------------
                //  Load the DataTable into a view
                // ----------------------------------------------------------------------
                DataView dv = new DataView(table);


                // ----------------------------------------------------------------------
                //  Sort the view
                // ----------------------------------------------------------------------
                string orderBy = Regex.Replace(orderByClause, "^ *order +by +", "", RegexOptions.IgnoreCase);
                orderBy        = Regex.Replace(orderBy      , "^ *by +"       , "", RegexOptions.IgnoreCase);
                dv.Sort = orderByClause;
                dv.ApplyDefaultSort = true;


                // ----------------------------------------------------------------------
                //  Load the view back into the DataTable
                // ----------------------------------------------------------------------
                DataTable temp = dv.ToTable();
                table.Clear();
                table.Merge(temp);
            }
            catch (Exception ex) { Pause(); throw ex; }

            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TallyIntColumn -->
        /// <summary>
        ///      Returns a tally dictionary of a table column
        /// </summary>
        /// <param name="data"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static Dictionary<int,int> TallyIntColumn(DataTable data, string column)
        {
            Dictionary<int,int> tally = new Dictionary<int,int>();
            if (data.Columns.Contains(column))
            {
                for (int row = 0; row < data.Rows.Count; ++row)
                {
                    int value = 0;
                    if (data.Rows[row][column] != null && int.TryParse(data.Rows[row][column].ToString(), out value))
                    {
                        if (value > 0)
                        {
                            if (!tally.ContainsKey(value))
                                tally.Add(value, 0);
                            tally[value]++;
                        }
                    }
                }
            }

            return tally;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Trim -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static SqlString Trim(SqlString str)
        {
            if (!str.IsNull)
                str = new SqlString(str.ToString().Trim());

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Truncate -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static SqlString Truncate(SqlString str, int maxLength)
        {
            if (str.ToString().Length > maxLength)
            {
                str = new SqlString(str.ToString().Substring(0,maxLength));
            }
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteStringToFile -->
        /// <summary>
        ///      Writes a non-humongous string to a file
        /// </summary>
        /// <remarks>Shouldn't this be done in another class?</remarks>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteStringToFile(string filePath, string content, bool append)
        {
            if (content == null)
            {
                throw new ArgumentNullException("WriteStringToFile error - content is null");
            }
            else
            {
                string[]     lines      = new string[1];
                StreamWriter fileWriter = null;
                lines[0] = content;


                if (filePath == null || filePath.Length == 0) return;
                if (lines == null || lines.Length == 0) return;


                try
                {
                    if (append)
                        fileWriter = File.AppendText(filePath);
                    else
                        fileWriter = File.CreateText(filePath);
                    foreach (string line in lines) fileWriter.Write(line);
                }
                catch (Exception ex)
                {
                    throw new IOException("WriteStringToFile: Unable to write string to file " + filePath + "..." + ex.Message);
                }
                finally { if (fileWriter != null)  fileWriter.Close(); }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpdateField -->
        /// <summary>
        ///      Updates the a particular field in a particular table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="idColumn"></param>
        /// <param name="idValue"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool UpdateField(string tableName, string column, object value, string idColumn
            , string idValue, SqlConnection connection)
        {
            bool status = true;
            string query = " UPDATE " + tableName + " SET " + column + " = @valueParam WHERE " + idColumn + " = @idParam";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@valueParam", value);
                    command.Parameters.AddWithValue("@idParam", idValue);
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                status = false;
            }
            return status;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpdateField -->
        /// <summary>
        ///      Updates a value in a column in the database
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="idColumn"></param>
        /// <param name="idValue"></param>
        /// <param name="maxFields">maximum number of rows to be updated</param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>TODO: limit number of fields to be updated</remarks>
        public static bool UpdateField(string tableName, string column, object value, string idColumn
            , int idValue, int maxFields, SqlConnection connection)
        {
            bool status = true;
            string query = " UPDATE " + tableName + " SET " + column + " = @valueParam " + "WHERE " + idColumn + " = @idParam";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@valueParam", value);
                    command.Parameters.AddWithValue("@idParam", idValue);
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                status = false;
            }
            return status;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ValueExists -->
        /// <summary>
        ///      Determines whether a value from a datarow exists in a table column
        /// </summary>
        /// <param name="field"></param>
        /// <param name="csvColumn">column to search from</param>
        /// <param name="tableName">table to search in</param>
        /// <param name="dbColumn">column to search in</param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool ValueExists(DataRow field, string csvColumn, string tableName, string dbColumn, SqlConnection connection)
        {
            SqlString value = InData.GetSqlString(field, csvColumn);
            if (value.IsNull || string.IsNullOrWhiteSpace(value.ToString())) { return false; }
            else
            {
                DataTable table = InData.GetTable(tableName, "SELECT * FROM "+tableName+" WHERE "+dbColumn+" = " + value.ToString(), connection);
                return (table.Rows.Count > 0);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- YYYYMMDD -->
        /// <summary>
        ///      Returns the current date in YYYYMMDD format
        /// </summary>
        /// <returns></returns>
        public static string YYYYMMDD { get
        {
            string ymd
                = DateTime.Now.Year .ToString().PadLeft(4, '0')
                + DateTime.Now.Month.ToString().PadLeft(2, '0')
                + DateTime.Now.Day  .ToString().PadLeft(2, '0');
            return ymd;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- DatabaseIsAvailableFor -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DatabaseIsAvailableFor(string conn)
        {
            SqlConnection connection = ConnectSource.Connection(conn);

            if (connection.State == ConnectionState.Open       ||
                connection.State == ConnectionState.Connecting ||
                connection.State == ConnectionState.Executing  ||
                connection.State == ConnectionState.Fetching    ) return true;


            if (connection.State == ConnectionState.Broken) return false;


            int timeout = connection.ConnectionTimeout;
            try
            {
                connection.Open();
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
            if (connection.State == ConnectionState.Broken) return false;
            else return true;
        }
    }
}
