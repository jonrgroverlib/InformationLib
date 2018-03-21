//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms of
// the GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with
// InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using InformationLib.HardData;        // for ResistSqlInjection, SqlQueryWithDeclaredParameters
using InformationLib.Endemes ;        // for Endeme, EndemeSet
using InformationLib.SoftData;        // for TreatAs
using InformationLib.Strings ;        // for __.ConcatIf
using System;                         // for TimeSpan, Guid, EventArgs
using System.Collections.Generic;     // for List
using System.ComponentModel;          // for IContainer
using System.Configuration;           // for ConfigurationManager
using System.Data;                    // for IDataParameterCollection
using System.Data.Sql;                // for SqlNotificationRequest
using System.Data.SqlClient;          // for SqlCommand, SqlConnection
using System.Data.SqlTypes;           // for SqlTypeException
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for XmlReader

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RichSqlCommand -->
    /// <summary>
    ///      The RichSqlCommand class is a pseudo-derived wrapper class of the SqlCommand class 
    ///      with a few extra generic methods added
    /// </summary>
    public sealed class RichSqlCommand
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Command -->
        /// <summary>
        ///      The SqlCommand object being wrapped, try to avoid modifying this directly
        /// </summary>
        /// <remarks>
        ///      This object is from the class that I am wrapping, the SqlCommand class,
        ///      I am using composition rather than inheritance or extension methods
        ///      which means I have to write a method for each SqlCommand method and so on
        ///      
        /// 
        ///      generally production ready - the AddParameter methods probably need some testing though
        /// </remarks>
        public SqlCommand Command { get { return _command; } } private SqlCommand _command;


        // ----------------------------------------------------------------------------------------
        //  'Get' members
        /* --------------------------------------------------------------------------------------*/                      /// <summary>Did it succeed or not (unreliable)</summary>
        public string       Results          { get { return _results        ; } } private string       _results        ; /// <summary>Number of rows affected</summary>
        public int          RowsAffected     { get { return _rowsAffected   ; } } private int          _rowsAffected   ; /// <summary>What the sp returned</summary>
        public int          Returns          { get { return _spReturns      ; } } private int          _spReturns      ; /// <summary>A parameter used by the sp to return stuff</summary>
        public SqlParameter ReturnParameter  { get { return _returnValue    ; } } private SqlParameter _returnValue    ; /// <summary>How long it took                          </summary>
        public TimeSpan     OperationTime    { get { return _operationTime  ; } } private TimeSpan     _operationTime  ; /// <summary>What to do when things go worng           </summary>
        public Endeme       ExceptionActions { get { return _exceptionAction; } } private Endeme       _exceptionAction;


        // ----------------------------------------------------------------------------------------
        //  Other members
        // ----------------------------------------------------------------------------------------
        public  string Errors             { get; set; }
        public  bool   StatementCompleted { get { return _completed; } set { _completed = value; } } private bool _completed;
        private List<string> _inputParam;   // just for functions


        // ----------------------------------------------------------------------------------------
        //  Constructors which initialize variables, message handling, connections, commands
        /* ------------------------------------------------------------------------------------- */                                                                                                  ///<summary>Instantiates a RichSqlCommand object to run a text command or a stored procedure</summary><param name="commandType">generally either .Text or .StoredProcedure, this hasn't been tested for .TableDirect</param><param name="conn">the connection in the form of either a connnection string, connnection string configuraiton file label, or existing connection</param><param name="query">the query in the form of either a string or a StringBuilder</param><param name="actions">a set of options for when things go wrong</param><param name="action">actual actions to take if things go wrong</param>
        public RichSqlCommand(CommandType type, string        conn                      , string        query, EndemeSet actions, Endeme action) { Init(type, ConnectSource.Connection(conn), action, query           ); } ///<summary>Instantiates a RichSqlCommand object to run a text command or a stored procedure</summary><param name="commandType">generally either .Text or .StoredProcedure, this hasn't been tested for .TableDirect</param><param name="conn">the connection in the form of either a connnection string, connnection string configuraiton file label, or existing connection</param><param name="query">the query in the form of either a string or a StringBuilder</param><param name="actions">a set of options for when things go wrong</param><param name="action">actual actions to take if things go wrong</param>
        public RichSqlCommand(CommandType type, string        conn                      , StringBuilder query, EndemeSet actions, Endeme action) { Init(type, ConnectSource.Connection(conn), action, query.ToString()); } ///<summary>Instantiates a RichSqlCommand object to run a text command or a stored procedure</summary><param name="commandType">generally either .Text or .StoredProcedure, this hasn't been tested for .TableDirect</param><param name="conn">the connection in the form of either a connnection string, connnection string configuraiton file label, or existing connection</param><param name="query">the query in the form of either a string or a StringBuilder</param><param name="actions">a set of options for when things go wrong</param><param name="action">actual actions to take if things go wrong</param>
        public RichSqlCommand(CommandType type, SqlConnection conn                      , string        query, EndemeSet actions, Endeme action) { Init(type,                          conn , action, query           ); } ///<summary>Instantiates a RichSqlCommand object to run a text command or a stored procedure</summary><param name="commandType">generally either .Text or .StoredProcedure, this hasn't been tested for .TableDirect</param><param name="conn">the connection in the form of either a connnection string, connnection string configuraiton file label, or existing connection</param><param name="query">the query in the form of either a string or a StringBuilder</param><param name="actions">a set of options for when things go wrong</param><param name="action">actual actions to take if things go wrong</param>
        public RichSqlCommand(CommandType type, SqlConnection conn, SqlTransaction trans, string        query, EndemeSet actions, Endeme action) { Init(type,       conn , action, query); _command.Transaction = trans; } ///<summary>Instantiates a RichSqlCommand object to run a text command or a stored procedure</summary><param name="commandType">generally either .Text or .StoredProcedure, this hasn't been tested for .TableDirect</param><param name="conn">the connection in the form of either a connnection string, connnection string configuraiton file label, or existing connection</param><param name="query">the query in the form of either a string or a StringBuilder</param><param name="actions">a set of options for when things go wrong</param><param name="action">actual actions to take if things go wrong</param>
        public RichSqlCommand(CommandType type, SqlConnection conn                      , StringBuilder query, EndemeSet actions, Endeme action) { Init(type,                          conn , action, query.ToString()); }
        public RichSqlCommand(                                                                                                                 ) { _InitVariables();                 _InitMessages(); _exceptionAction = "NPL" ; }
        public RichSqlCommand(SqlCommand  cmd                                                                , EndemeSet actions, Endeme action) { _InitVariables(); _command = cmd; _InitMessages(); _exceptionAction = action; }
        public RichSqlCommand(string conn, string sqlFunction, List<string> inputParam                       , EndemeSet actions, Endeme action) { Init("SELECT "+sqlFunction+"("+__.Collate(_inputParam,", ")+")", inputParam, ConnectSource.Connection(conn), action); }


        // ----------------------------------------------------------------------------------------
        //  Pseudo-inherited properties
        // ----------------------------------------------------------------------------------------
        public     IDataParameterCollection Parameters    { get { return _command.Parameters;             }                                                  }
        public     SqlNotificationRequest Notification    { get { return _command.Notification;           } set { _command.Notification           = value; } }
        public     UpdateRowSource UpdatedRowSource       { get { return _command.UpdatedRowSource;       } set { _command.UpdatedRowSource       = value; } }
        public     bool            NotificationAutoEnlist { get { return _command.NotificationAutoEnlist; } set { _command.NotificationAutoEnlist = value; } }
        public     string          CommandText            { get { return _command.CommandText;            } set { _command.CommandText            = value; } }
        public     int             CommandTimeout         { get { return _command.CommandTimeout;         } set { _command.CommandTimeout         = value; } }
        public     CommandType     CommandType            { get { return _command.CommandType;            } set { _command.CommandType            = value; } }
        public     SqlConnection   Connection             { get { return _command.Connection;             } set { _command.Connection             = value; } }
        public     IContainer      Container              { get { return _command.Container;              }                                                  }
        public     ISite           Site                   { get { return _command.Site;                   } set { _command.Site                   = value; } }
        public     SqlTransaction  Transaction            { get { return _command.Transaction;            } set { _command.Transaction            = value; } }


        // ----------------------------------------------------------------------------------------
        //  Pass-through methods
        // ----------------------------------------------------------------------------------------
        public          void       Cancel                   ()  {        _command.Cancel                   (); }
        public          void       Dispose                  ()  {        _command.Dispose                  (); }
        public          void       Prepare                  ()  {        _command.Prepare                  (); }
        public          SqlCommand _Clone                   ()  { return _command.Clone                    (); } // TODO: rewrite to return RichSqlCommand
        public          XmlReader  _ExecuteXmlReader        ()  { return _command.ExecuteXmlReader         (); } // TODO: rewrite like _ExecuteNonQuery
        public          object     GetLifetimeService       ()  { return _command.GetLifetimeService       (); }
        public          object     InitializeLifetimeService()  { return _command.InitializeLifetimeService(); }
        public          void       ResetCommandTimeout      ()  {        _command.ResetCommandTimeout      (); }


        /* ------------------------------------------------------------------------------------- */
        //  Other short methods and properties
        /* ------------------------------------------------------------------------------------- */                                                                                          /// <summary>Close the connection if it was opened by this method - if it was closed</summary><param name="closeConnection">closes connection if true</param>
        public  void   _Close      (bool   closeConnection             ) { if (closeConnection) InData.Close(Connection);                                                                  } /// <summary>Creates connection using connection name</summary>
        public  void   _ConnectWith(string connectionName              ) { _command.Connection.ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString; } /// <summary>Sets the value of a parameter whose type is already set</summary>
        public  void   _SetParam   (string paramName, object paramValue) { _command.Parameters[paramName].Value = paramValue;                                                              } /// <summary>Produces a parameterized sql query that can be dropped into SSMS query analyzer</summary>
        public  string AsciiQuery                                  { get { return InData.AsciiNewQuery(_command);                                                                          } }
        private void   ConsiderMakingThisAStoredProcedure()              { }


        /* ------------------------------------------------------------------------------------- */
        //  Safe and Unsafe AddParameter versions for string values - ADD STRING TYPE VALUES HERE!
        /* ------------------------------------------------------------------------------------- */                                                                                                                                                               /// <summary>Adds a parameter of the specified name, type and value</summary>
        public  RichSqlCommand AddParameter_safe  (string name, string      value                        ) { if (value == null) AddParameter(name, SqlString.Null, SqlDbType.VarChar); else { AddParameter(name, InData.ResistSqlInjection(value, 0     ), SqlDbType.VarChar        ); } return this; } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>
        public  RichSqlCommand AddParameter_safe  (string name, string      value, int maxLen            ) { if (value == null) AddParameter(name, SqlString.Null, SqlDbType.VarChar); else { AddParameter(name, InData.ResistSqlInjection(value, maxLen), SqlDbType.VarChar, maxLen); } return this; } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>
        public  RichSqlCommand AddParameter_safe  (string name, string      value, ParameterDirection dir) { if (value == null) AddParameter(name, SqlString.Null, SqlDbType.VarChar); else { AddParameter(name, InData.ResistSqlInjection(value, 0     ), SqlDbType.VarChar, dir   ); } return this; } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>
        public  RichSqlCommand AddParameter_safe  (string name, SqlString   value                        ) { SqlString safer = SqlString.Null; if (!value.IsNull) safer = InData.ResistSqlInjection(value.ToString(), 0     );   AddParameter(name, safer, SqlDbType.VarChar        );   return this; } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>
        public  RichSqlCommand AddParameter_safe  (string name, SqlString   value, int maxLen            ) { SqlString safer = SqlString.Null; if (!value.IsNull) safer = InData.ResistSqlInjection(value.ToString(), maxLen);   AddParameter(name, safer, SqlDbType.VarChar        );   return this; } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>
        public  RichSqlCommand AddParameter_safe  (string name, SqlString   value, ParameterDirection dir) { SqlString safer = SqlString.Null; if (!value.IsNull) safer = InData.ResistSqlInjection(value.ToString(), 0     );   AddParameter(name, safer, SqlDbType.VarChar, dir   );   return this; } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>
        public  RichSqlCommand AddParameter_unsafe(string name, string      value                        ) {                                                                                                                     AddParameter(name, value, SqlDbType.VarChar        );   return this; } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary><remarks>test me</remarks>
        public  RichSqlCommand AddParameter_unsafe(string name, string      value, ParameterDirection dir) {                                                                                                                     AddParameter(name, value, SqlDbType.VarChar, dir   );   return this; } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>
        private RichSqlCommand AddParameter_sized (string name, SqlDbType   type , object value, int size) { SqlParameter sp = new SqlParameter(name, type); sp.Value = value; sp.Direction = ParameterDirection.Input; sp.Size = size; _command.Parameters.Add(sp); return this; }


        /* ------------------------------------------------------------------------------------- */
        //  Safe AddParameter versions, types specified by input value type - DO NOT ADD STRING TYPE VALUES HERE!
        /* ------------------------------------------------------------------------------------- */                                                                                                                                                                                                                 /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public RichSqlCommand AddParameter       (string name, bool        value, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, SqlDbType.Bit             );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public RichSqlCommand AddParameter       (string name, bool        value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Bit             );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_null  (string name, bool?       value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Bit             ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_byte  (string name, byte[]      value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.VarBinary       ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
      //public RichSqlCommand AddParameter_safe  (string name, char        value   /* needed? */        ) { char      safer = ' '           ; if (value != null) { safer = value; } AddParameter_sized(name, SqlDbType.VarChar, safer, 1); return this; } 
        public RichSqlCommand AddParameter_char  (string name, char        value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Char            );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_char  (string name, char        value,    DbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_char  (string name, char        value, SqlDbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_char  (string name, char?       value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Char            ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, DateTime    value,    DbType type        ) { SqlParameter sp = new SqlParameter(name, type                      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
        public RichSqlCommand AddParameter       (string name, DateTime    value, SqlDbType type        ) { SqlParameter sp = new SqlParameter(name, type                      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
        public RichSqlCommand AddParameter_date  (string name, DateTime?   value,    DbType type        ) { SqlParameter sp = new SqlParameter(name, type                      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_date  (string name, DateTime?   value, SqlDbType type        ) { SqlParameter sp = new SqlParameter(name, type                      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, Guid        value, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, SqlDbType.UniqueIdentifier); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, Guid        value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.UniqueIdentifier); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_null  (string name, Guid?       value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.UniqueIdentifier); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, SqlDateTime value,    DbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
        public RichSqlCommand AddParameter       (string name, SqlDateTime value, SqlDbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
                                                                                                                                                                                                                                                                                                                    /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, decimal     value,    DbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
        public RichSqlCommand AddParameter       (string name, decimal     value, SqlDbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
        public RichSqlCommand AddParameter_null  (string name, decimal?    value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Decimal         ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, double      value,    DbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, double      value, SqlDbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_null  (string name, double?     value                        ) { SqlParameter sp = new SqlParameter(name,    DbType.Double          ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, float       value,    DbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, float       value, SqlDbType type        ) { SqlParameter sp = new SqlParameter(name, type                      );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_null  (string name, float?      value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Float           ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, int         value, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, SqlDbType.Int             );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public RichSqlCommand AddParameter       (string name, int         value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Int             );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_null  (string name, int?        value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.Int             ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, long        value, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, SqlDbType.BigInt          );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public RichSqlCommand AddParameter       (string name, long        value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.BigInt          );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_null  (string name, long?       value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.BigInt          ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter       (string name, short       value, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, SqlDbType.SmallInt        );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public RichSqlCommand AddParameter       (string name, short       value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.SmallInt        );                                                  sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
        public RichSqlCommand AddParameter_null  (string name, short?      value                        ) { SqlParameter sp = new SqlParameter(name, SqlDbType.SmallInt        ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); return this; } /// <summary>Adds a parameter of the specified name and value</summary>
                                                                                                                                                                     
        /* ------------------------------------------------------------------------------------- */                                                                  
        //  Safe AddParameter versions, types not specified by input value type - DO NOT ADD STRING TYPE VALUES HERE!                                                
        /* ------------------------------------------------------------------------------------- */                                                                                                                                                                                                                 /// <summary>Adds a parameter of the specified name, SqlDbType, value and direction</summary>
        public  void AddParameter       (string name, DateTime    value,    DbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, SqlDbType, value and direction</summary>
        public  void AddParameter       (string name, SqlDateTime value,    DbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, SqlDbType, value and direction</summary>
        public  void AddParameter       (string name, char        value,    DbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name and value</summary>
        public  void AddParameter       (string name, DateTime    value, SqlDbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, SqlDbType, value and direction</summary>
        public  void AddParameter       (string name, SqlDateTime value, SqlDbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, SqlDbType, value and direction</summary>
        public  void AddParameter       (string name, char        value, SqlDbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      );                                                  sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name and value</summary>
                                                                                                                                                                                                                                                                                                                    /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
        public  void AddParameter       (string name, decimal     value,    DbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type           );                                             sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, SqlDbType, value and direction</summary>
        public  void AddParameter       (string name, double      value,    DbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, DbType.Double  );                                             sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public  void AddParameter       (string name, float       value,    DbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, SqlDbType.Float);                                             sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public  void AddParameter       (string name, decimal     value, SqlDbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type           );                                             sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, SqlDbType, value and direction</summary>
        public  void AddParameter       (string name, double      value, SqlDbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, DbType.Double  );                                             sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, value and direction</summary>
        public  void AddParameter       (string name, float       value, SqlDbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, SqlDbType.Float);                                             sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, value and direction</summary>

        /* ------------------------------------------------------------------------------------- */
        //  Unsafe AddParameter versions - do not make these public, instead add a new AddParameter for that type, do this to resist SQL injection
        /* ------------------------------------------------------------------------------------- */                                                                                                                                                                                                                  /// <summary>Adds a parameter of the specified name and value</summary>
        private void AddParameter       (string name, object      value,    DbType type                        ) { SqlParameter sp = new SqlParameter(name, type      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name and value</summary>
        private void AddParameter       (string name, string      value, SqlDbType type, int size              ) { SqlParameter sp = new SqlParameter(name, type, size); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name and value</summary>
        private void AddParameter       (string name, object      value, SqlDbType type                        ) { SqlParameter sp = new SqlParameter(name, type      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = ParameterDirection.Input; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, type, value and direction</summary>
        private void AddParameter       (string name, object      value,    DbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Adds a parameter of the specified name, SqlDbType and value</summary>
        private void AddParameter       (string name, object      value, SqlDbType type, ParameterDirection dir) { SqlParameter sp = new SqlParameter(name, type      ); if (value == null) sp.Value = DBNull.Value; else sp.Value = value; sp.Direction = dir                     ; _command.Parameters.Add(sp); } /// <summary>Safer string Parameter adding (resists Sql Injection)</summary>


		// -----------------------------------------------------------------------------------------
		/// <!-- BuildSchema -->
		/// <summary>
		///      Sets up a schema in the database for tables to go into
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>, old</remarks>
		public static void BuildSchema(string schemaName, string ownerID, string conn, EndemeSet exceptionActions, Endeme action)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn, "CREATE SCHEMA "+schemaName+" AUTHORIZATION " + ownerID, exceptionActions, action);
			cmd.ExecuteNonQuery();
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- DropSchema -->
		/// <summary>
		///      Removes a schema from the database, note: all the table have to be deleted first
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>, old</remarks>
		public static void DropSchema(string schemaName, string conn, EndemeSet exceptionActions, Endeme action)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn, "DROP SCHEMA " + schemaName, exceptionActions, action);
			cmd.ExecuteNonQuery();
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteFunction -->
        /// <summary>
        ///      Executes a single valued prepared function
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code</remarks>
        public object ExecuteFunction(SqlDbType inputType, object inputValue, object defaultValue)
        {
            // --------------------------------------------------------------------------
            //  Set the parameter value
            // --------------------------------------------------------------------------
            _RemoveParametersExcept("@Return_Value");
            AddParameter(_inputParam[0], inputValue, inputType);


            // --------------------------------------------------------------------------
            //  Run the function
            // --------------------------------------------------------------------------
            bool ifOpenedhere = Open();
            StatementCompleted = false;
            object value;
            SqlDataReader rdr = null;
            try
            {
                // ----------------------------------------------------------------------
                //  Run the function
                // ----------------------------------------------------------------------
                rdr = this.ExecuteReader();
                if (rdr.HasRows)
                    { rdr.Read();  value = rdr[0]; }
                else value = defaultValue;
            }
            catch
            {
                Throws.A(new DataException("data reader failed on function "+_command.CommandText), Throws.Actions, _exceptionAction);
                value = defaultValue;
            }
            finally { if (rdr != null)  rdr.Dispose(); }
            _Close(ifOpenedhere);


            // --------------------------------------------------------------------------
            //  Process errors detected by events
            // --------------------------------------------------------------------------
            if (!string.IsNullOrEmpty(Errors))
                Throws.A(new DataException(Errors), Throws.Actions, _exceptionAction);
            if (!_completed)
            {
                string error = "statement '" + _command.CommandText + "' did not complete";
                Errors += "\r\n" + error;
                Throws.A(new SqlTypeException(error), Throws.Actions, _exceptionAction);
            }

            return value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteFunction -->
        /// <summary>
        ///      Executes a function, returning the value it produces
        /// </summary>
        /// <param name="typeA"></param>
        /// <param name="valueA"></param>
        /// <param name="typeB"></param>
        /// <param name="valueB"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>beta code</remarks>
        public object ExecuteFunction(SqlDbType typeA, string valueA, SqlDbType typeB, string valueB
            , object defaultValue)
        {
            // --------------------------------------------------------------------------
            //  Set the parameter value
            // --------------------------------------------------------------------------
            _RemoveParametersExcept("@Return_Value");
            AddParameter(_inputParam[0], valueA, typeA);
            AddParameter(_inputParam[1], valueB, typeB);


            // --------------------------------------------------------------------------
            //  Run the function
            // --------------------------------------------------------------------------
            bool ifOpenedhere = Open();
            StatementCompleted = false;
            object value;
            SqlDataReader rdr = null;
            try
            {
                // ----------------------------------------------------------------------
                //  Run the function
                // ----------------------------------------------------------------------
                rdr = this.ExecuteReader();
                if (rdr.HasRows)
                    { rdr.Read();  value = rdr[0]; }
                else value = defaultValue;
            }
            catch
            {
                Throws.A(new DataException("data reader failed on function "+_command.CommandText), Throws.Actions, _exceptionAction);
                value = defaultValue;
            }
            finally { if (rdr != null)  rdr.Dispose(); }
            _Close(ifOpenedhere);


            // --------------------------------------------------------------------------
            //  Process errors detected by events
            // --------------------------------------------------------------------------
            if (!string.IsNullOrEmpty(Errors))
                Throws.A(new DataException(Errors), Throws.Actions, _exceptionAction);
            if (!_completed)
            {
                string error = "statement '" + _command.CommandText + "' did not complete";
                Errors += "\r\n" + error;
                Throws.A(new SqlTypeException(error), Throws.Actions, _exceptionAction);
            }

            return value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteNonQuery -->
        /// <summary>
        ///      Runs the prepared ExecuteNonQuery command
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public int ExecuteNonQuery()
        {
            _results = "";


            // --------------------------------------------------------------------------
            //  Open the connection if it isn't already
            // --------------------------------------------------------------------------
            bool openConnection = Open();


            // --------------------------------------------------------------------------
            //  Run stored procedure
            // --------------------------------------------------------------------------
            int affected = 0;
            try { affected = this.ExecuteNonQuery_simple(); }
            catch (Exception ex)
            {
                Errors += __.ConcatIf("\r\n\r\nExecuteNonQuery caught error - ", ex.Message);
            }


            // --------------------------------------------------------------------------
            //  Look at results
            // --------------------------------------------------------------------------
            //if (_spReturns > 100000)
            //    _results += _.Affix("the stored procedure '" + this.CommandText + "'"
            //        + " returns: ", BigNumber.Base16(_spReturns));
            //else
                _results += __.ConcatIf("the stored procedure '" + this.CommandText + "'"
                    + " returns: ", _spReturns);


            // --------------------------------------------------------------------------
            //  Normal post processing, close the connection if this method opened it
            // --------------------------------------------------------------------------
            _results += __.ConcatIf("\r\n# rows involved: ", affected);
            _results += __.ConcatIf("\r\nthe execute-non-query returns: ", _rowsAffected);
            if (openConnection)
                InData.Close(Connection);


            // --------------------------------------------------------------------------
            //  Error post processing
            // --------------------------------------------------------------------------
            if (!string.IsNullOrEmpty(Errors))
            {
                string xml = _Str("@newXML", ""); // for debugging
                Throws.A(new SqlTypeException(Errors), Throws.Actions, _exceptionAction);
            }

            return affected;
        }
        public int ExecuteNonQuery(Endeme exceptionAction) { _exceptionAction = exceptionAction; return ExecuteNonQuery(); }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteNonQuery_simple -->
        /// <summary>
        ///      Executes a Transact-SQL statement against the connection and
        ///      returns the number of rows affected
        /// </summary>
        /// <returns>the number of rows affected</returns>
        /// <remarks>production ready</remarks>
        private int ExecuteNonQuery_simple()
        {
            DateTime stamp1 = DateTime.Now;
            Errors          = "";
            _completed      = false;
            _rowsAffected   = _command.ExecuteNonQuery();


            _spReturns      = TreatAs.IntValue(_returnValue.Value, -2);
            DateTime stamp2 = DateTime.Now;
            _operationTime  = stamp2 - stamp1;
            return _rowsAffected;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteReader -->
        /// <summary>
        ///      Executes a Transact-SQL statement against the connection, returning the results in
        ///      a SqlDataReader
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///      Todo:
        ///      rewrite like ExecuteNonQuery
        /// 
        ///      production ready - please test me</remarks>
        /// </remarks>
        public SqlDataReader ExecuteReader()
        {
            // --------------------------------------------------------------------------
            //  Pre processing and validation
            // --------------------------------------------------------------------------
            SqlDataReader rdr = null;
           _results = "";
            if (Connection.State == ConnectionState.Closed)
                Throws.A(new DataException("Change your program"
                    + " - connection needs to be opened externally for a reader"), Throws.Actions, _exceptionAction);
            if (_completed)
            {
                Throws.A(new ArgumentException("Please handle _StatementCompleted in the code surrounding the use of the reader."
                    + "\r\n" + "Set this property false when opening the connection"), Throws.Actions, _exceptionAction);
                return null;
            }


            // --------------------------------------------------------------------------
            //  Execute the reader and catch the results
            // --------------------------------------------------------------------------
            try { rdr      = ExecuteReader_simple(); }
            catch (Exception ex)
            {
                Errors += __.ConcatIf("\r\n_ExecuteReader Error - ", ex.Message);
                if (!string.IsNullOrEmpty(ex.Message))
                    Errors += "\r\nException type:  " + ex.GetType().ToString();
            }
            finally
            {
                if (rdr == null)
                    Throws.A(new DataException("SqlCommand reader is null"), Throws.Actions, _exceptionAction);
                else _results += "reader does " + rdr.HasRows.ToString() + " have rows";
            }


            // --------------------------------------------------------------------------
            //  Post error processing
            // --------------------------------------------------------------------------
            if (Errors != "")
                Throws.A(new DataException(Errors), Throws.Actions, _exceptionAction);

            return rdr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteReader_simple -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        private SqlDataReader ExecuteReader_simple()
        {
            DateTime stamp1 = DateTime.Now;
            Errors          = "";
            _completed      = false;
            SqlDataReader rdr = _command.ExecuteReader();


            _spReturns      = TreatAs.IntValue(_returnValue.Value, -2);
            DateTime stamp2 = DateTime.Now;
            _operationTime  = stamp2 - stamp1;
            return rdr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteScalar -->
        /// <summary>
        ///      Runs the prepared ExecuteScalar command
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="startTime">recommend DateTime.Now</param>
        /// <returns>an object</returns>
        /// <remarks>production ready</remarks>
        public object ExecuteScalar(object defaultValue, DateTime startTime)
        {
            bool ifOpenedHere = Open();  // Open the connection if it isn't


            // --------------------------------------------------------------------------
            //  Run stored procedure
            // --------------------------------------------------------------------------
            object value = defaultValue;
            _results = "";
            try { value = ExecuteScalar_simple(startTime); }
            catch (Exception ex)
            {
                Errors += __.ConcatIf("\r\nCaught Error - ", ex.Message); Throws.A(ex, Throws.Actions, "P");
            }
            if (!string.IsNullOrEmpty(Errors))
                Throws.A(new Exception(Errors), Throws.Actions, _exceptionAction);


            // --------------------------------------------------------------------------
            //  Look at results
            // --------------------------------------------------------------------------
            //if (_spReturns > 100000)
            //    _results += _.Affix("\r\nmy stored procedure returns: ", BigNumber.Base16(_spReturns));
            //else
            _results += __.ConcatIf("\r\nmy stored procedure returns: " , _spReturns   );
            _results += __.ConcatIf("\r\nmy execute-non-query returns: ", _rowsAffected);
            _results += Errors;
            _Close(ifOpenedHere);  // Close the connection if this method opened it


            return value;
        }

        public bool      ExecuteScalar(bool      dflt) { return TreatAs.BoolValue (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public byte[]    ExecuteScalar(byte[]    dflt) { return TreatAs.ByteValue (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
      //public Byte[]    ExecuteScalar(Byte[]    dflt) { return TreatAs.ByteValue (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public char      ExecuteScalar(char      dflt) { return TreatAs.CharValue (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public decimal   ExecuteScalar(decimal   dflt) { return TreatAs.DecValue  (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public double    ExecuteScalar(double    dflt) { return TreatAs.RealValue (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public float     ExecuteScalar(float     dflt) { return TreatAs.FloatValue(ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public Guid      ExecuteScalar(Guid      dflt) { return TreatAs.GuidValue (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public int       ExecuteScalar(int       dflt) { return TreatAs.IntValue  (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public long      ExecuteScalar(long      dflt) { return TreatAs.LongValue (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
      //public object    ExecuteScalar(object    dflt) { return TreatAs.ObjValue  (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public string    ExecuteScalar(string    dflt) { return TreatAs.StrValue  (ExecuteScalar(dflt, DateTime.Now), dflt); } /// <summary>Runs the prepared ExecuteScalar command, returning an object of the type of the default value</summary>
        public DateTime? ExecuteScalar(DateTime? dflt) { object obj = ExecuteScalar(dflt, DateTime.Now); if (obj == null) return dflt; else return TreatAs.NullableDateTimeValue(obj, dflt); }


        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteScalar_simple -->
        /// <summary>
        ///      Executes a Transact-SQL statement against the connection, returning the result
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        private object ExecuteScalar_simple(DateTime stamp1)
        {
            Errors          = "";
            _completed      = false;
            object value    = _command.ExecuteScalar();


            _spReturns      = TreatAs.IntValue(_returnValue.Value, -2);
            DateTime stamp2 = DateTime.Now;
            _operationTime  = stamp2 - stamp1;
            return value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Guid -->
        /// <summary>
        ///      Returns the guid value of the named parameter or an empty guid as default
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public Guid _Guid(string parameter)
        {
            SqlParameter param = (SqlParameter)Parameters[parameter];
            Guid guid = TreatAs.GuidValue(param.Value, Guid.Empty);
            if (guid == Guid.Empty)
            {
                object obj = _command.Parameters[2].Value;
                guid = TreatAs.GuidValue(obj, Guid.Empty);
            }
            return guid;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Implements the constructor to run a stored procedure or a text query
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <param name="query"></param>
        /// <remarks>production ready - please test me</remarks>
        private void Init(CommandType type, SqlConnection connection, Endeme action, string query)
        {
            if (type == CommandType.Text)
                ConsiderMakingThisAStoredProcedure();


            _InitVariables();
            _command.CommandText = query;
            _command.Connection  = connection;
            _InitMessages();
            _command.CommandType = type;
            _exceptionAction     = action;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Initialize command to run a SQL function
        /// </summary>
        /// <param name="query"></param>
        /// <param name="inputParam"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <remarks>production ready</remarks>
        private void Init(string query, List<string> inputParam, SqlConnection connection, Endeme action)
        {
            _InitVariables();
            _inputParam          = inputParam;
            _command.CommandText = query;
            _command.Connection  = connection;
            _InitMessages();
            _command.CommandType = CommandType.Text;
            _exceptionAction     = action;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _InitMessages -->
        /// <summary>
        ///      Dependency: needs to a have the command's connection object instantiated first
        /// </summary>
        /// <remarks>production ready - please test me</remarks>
        private void _InitMessages()
        {
            if (_command == null)
                Throws.A(new NoNullAllowedException("RichSqlCommand._InitMessages error - command is null"), Throws.Actions, _exceptionAction);
            if (_command.Connection == null)
                Throws.A(new NoNullAllowedException("RichSqlCommand._InitMessages error - connection is null"), Throws.Actions, _exceptionAction);


            _returnValue = new SqlParameter("@Return_Value", SqlDbType.Int);
            _returnValue.Direction = ParameterDirection.ReturnValue;
            _command.Parameters.Add(_returnValue);
            _command.Connection.FireInfoMessageEventOnUserErrors = true;


            _command.Connection.InfoMessage += new SqlInfoMessageEventHandler    (_OnInfoMessage);
            _command.StatementCompleted     += new StatementCompletedEventHandler(_OnStatementCompleted);
            _command.Disposed               += new EventHandler                  (_OnDisposed);
            _command.Connection.StateChange += new StateChangeEventHandler       (_OnStateChange);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _InitVariables -->
        /// <summary>
        ///      Initializes private member variables
        /// </summary>
        /// <remarks>production ready - please test me</remarks>
        private void _InitVariables()
        {
            _completed = true; // initialize to true so that readers can tell if it is being handled properly
            _rowsAffected = -1;
            _spReturns = -1;
            Errors = "";


            _command = new SqlCommand();
            _command.Connection = new SqlConnection();
            _inputParam = new List<string>();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string state = "";
            int    count = -1;
            string text  = "";
            string type  = "";

            if (_command != null)
            {
                if (_command.Connection  != null) state = _command.Connection.State.ToString();
                if (_command.Parameters  != null) count = _command.Parameters.Count;
                if (_command.CommandText != null) text  = _command.CommandText;
                type  = _command.CommandType.ToString();
            }

            return state + " " + type + ":" + __.Truncate(text, 12) + "... ("+count+")";
        }


        // ----------------------------------------------------------------------------------------
        #region Event implementations
        // ----------------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
        /// <!-- _OnDisposed -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        /// <remarks>production ready</remarks>
        private void _OnDisposed(object o, EventArgs args)
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _OnInfoMessage -->
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>production ready</remarks>
        private void _OnInfoMessage(object o, SqlInfoMessageEventArgs args)
        {
            foreach (SqlError err in args.Errors)
            {
                string subMessage = "\r\nInfoMessage:"
                    + "\r\n"
                    + "a.  The "                  + err.Source
                    + " has received a severity " + err.Class
                    + ", state "                  + err.State + " error\r\n"
                    + "b.  Error number "         + err.Number
                    + " on line "                 + err.LineNumber
                    + " of procedure "            + err.Procedure
                    + " on server "               + err.Server + ":\r\n"
                    + "c.  "                      + err.Message;
                subMessage = Regex.Replace(subMessage, "of procedure  on server", "of (command text?) on server");
                Errors += "\r\n" + subMessage;
            }
            Errors += "\r\n\r\nSummary:  "        + args.Message
                    + "\r\nFrom Source:  "        + args.Source;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _OnStateChange -->
        /// <summary>
        ///      Does stuff
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        /// <remarks>production ready</remarks>
        private void _OnStateChange(object o, StateChangeEventArgs args)
        {
            ConnectionState state = args.CurrentState;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _OnStatementCompleted -->
        /// <summary>
        ///      If the statement does not complete, something probably went wrong
        /// </summary>
        /// <remarks>production ready</remarks>
        private void _OnStatementCompleted(object o, StatementCompletedEventArgs args)
        {
            _completed = true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Open -->
        /// <summary>
        ///      Opens the connection if it is not already open
        /// </summary>
        /// <returns>Side effect: returns whether it opened the connection or not</returns>
        /// <remarks>production ready - test me</remarks>
        public bool Open()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                //MessageBox.Show("Before open connection");
                try
                {
                    InData.Open(Connection);
                }
                catch (Exception ex)
                {
                    Type type = ex.GetType();
                    Errors = type.ToString() + " - " + ex.Message;
                    Throws.A(ex, Throws.Actions, "P");
                }
                //MessageBox.Show("After open connection");
                return true;
            }
            else return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _RemoveInputParameters -->
        /// <summary>
        ///      Removes all the parameters except '@Return_Value'
        /// </summary>
        /// <remarks>alpha code</remarks>
        private void _RemoveParametersExcept(string paramName)
        {
            for (int i = Parameters.Count - 1; i >= 0; --i)
            {
                SqlParameter param = (SqlParameter)Parameters[i];
                if (param.ParameterName != paramName) Parameters.RemoveAt(1);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Str -->
        /// <summary>
        ///      Returns the value of a stored procedure parameter as a string
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>beta code - for debugging</remarks>
        public string _Str(string parameterName, string defaultValue)
        {
            string str;
            if (Parameters.Contains(parameterName))
                str = TreatAs.StrValue(((SqlParameter)Parameters[parameterName]).Value, defaultValue);
            else
                str = defaultValue;
            return str;
        }

        // ----------------------------------------------------------------------------------------
        #endregion Event implementations
        // ----------------------------------------------------------------------------------------
    }
}
