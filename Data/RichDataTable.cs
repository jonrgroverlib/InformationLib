// ------------------------------------------------------------------------------------------------
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
// ------------------------------------------------------------------------------------------------
using InformationLib.Endemes ;        // for Endeme
using InformationLib.HardData;        // for FileIO
using InformationLib.SoftData;        // for Tally
using InformationLib.Strings ;        // for IResults, Date
using InformationLib.Testing ;        // for RandomSource
using System;                         // for many
using System.Collections;             // for ArrayList
using System.Collections.Generic;     // for List
using System.Data;                    // for many
using System.Data.OleDb;              // for OleDbConnection, OleDbDataAdapter
using System.Data.SqlClient;          // for SqlDataReader
using System.Data.SqlTypes;           // for SqlDateTime, SqlXml
using System.Globalization;           // for CultureInfo
using System.IO;                      // for StreamWriter
using System.Linq;                    // for 
using System.Reflection;              // for GetProperties
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex, MatchCollection
using System.Xml;                     // for XmlNode, XmlNodeList

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RichDataTable -->
    /// <summary>
    ///      The RichDataTable class is a factory and a wrapper for creating and working with DataTables
    /// </summary>
    /// <remarks>
    ///      RichDataTable is sort of like a DataTable with a bunch of extension methods,
    ///      and various construction methods, It is sort of like a faactory combined with the 
    ///      class it produces coimbined with a bunch of extension methods, Endeme
    ///      
    /// 
    ///      production ready
    /// </remarks>
    public sealed class RichDataTable : IDisposable
    {
        public enum StringFormat { CSV, XML, JSON }

        // ----------------------------------------------------------------------------------------
        //  Direct DataTable access methods and properties (all of the methods and properties that directly access _table are here, well except for 'Table' which actually exposes _table)
        /* ------------------------------------------------------------------------------------- */                                                                      /// <summary>The DataTable can either be constructed by the RichDataTable 'factory' or placed here after construction</summary>
        public DataTable            Table                        { get {                                      return                   _table; } } private DataTable _table;
                                                                                                       
        public void                 Clear    ()                        {                                                               _table.Clear()                   ; }   /// <summary>Returns a new blank table with the same structure as this one</summary>
        public RichDataTable        Clone    ()                        { if (_table == null) return null    ; return new RichDataTable(_table.Clone(), "")              ; }
        public DataColumnCollection Columns                      { get { if (_table == null) return null    ; return                   _table.Columns                   ; } } /// <summary>Runs dataTable.Compute(expression, filter)</summary><param name="expr">expression</param><param name="fltr">filter</param>
        public object               Compute (string expr, string fltr) { if (_table == null) return null    ; return                   _table.Compute(expr, fltr)       ; }   /// <summary>Counts the number of rows in the table</summary><returns>the number of rows in the table</returns>
        public int                  Count                        { get { if (_table == null) return 0       ; return                   _table.Rows.Count                ; } }
        public DataView             DefaultView                  { get { if (_table == null) return null    ; return                   _table.DefaultView               ; } }
        public DataView             View                         { get { if (_table == null) return null    ; return new DataView(     _table)                          ; } }
        public void                 Dispose  ()                        { if (_table == null) return         ;                          _table.Dispose()                 ; }
        public bool                 Exists   ()                        {                                      return                   _table != null                   ; }
        public void                 ImportRow(DataRow       row      ) { if (!Exists()     ) return         ;                          _table.ImportRow(row)            ; }
        public void                 Init     ()                        {                                                               _table = new DataTable()         ; }
        public void                 Init     (string        tableName) {                                                               _table = new DataTable(tableName); }
        public void                 Merge    (DataTable     table    ) { if (_table == null) return         ;                          _table.Merge(table)              ; }
        public void                 Merge    (RichDataTable table    ) { if (_table == null) return         ;                          _table.Merge(table._table);      ; }
        public DataRow              NewRow   ()                        { if (_table == null) return null    ; return                   _table.NewRow()                  ; }
        public DataColumn[]         PrimaryKey                   { get { if (_table == null) return null    ; return                   _table.PrimaryKey                ; } set { _table.PrimaryKey = value; } }
        public void                 RemoveAt (int           row      ) { if (_table == null) return         ;                          _table.Rows.RemoveAt(row)        ; }
        public DataRowCollection    Rows                         { get { if (_table == null) return null    ; return                   _table.Rows                      ; } }
        public DataRow[]            Select   (string filter          ) { if (_table == null) return null    ; return                   _table.Select(BodyOf(filter))    ; }
        public string               TableName                    { get { if (_table == null) return "[null]"; return                   _table.TableName                 ; } set { _table.TableName = value; } }  /// <summary>Copies the entire RichDataTable including the current errors and xml if they exist</summary>
        public RichDataTable        Copy     ()                        { if (_table == null) return null; RichDataTable copy = new RichDataTable(_table.Copy(), ""); copy._errors = _errors; copy._results = _results; copy._lastRow = _lastRow; copy._lastColumn = _lastColumn; copy._lastXml = _lastXml; if (_lastXdoc != null) copy._lastXdoc = _lastXdoc._Copy(); else copy._lastXdoc = null; return copy; }
        private static string       BodyOf   (string clause          ) { clause = Regex.Replace(clause, "^ *(by|order +by|where) +", "", RegexOptions.IgnoreCase); return clause; }


        // ----------------------------------------------------------------------------------------
        //  Members for handling information
        // ----------------------------------------------------------------------------------------
        public string         Errors   { get { return _errors  ; } set { if (value.Trim() != _errors.Trim()) _errors += value; } } private string _errors  ;  /// <summary>Records non-error results</summary>
        public string         Results  { get { return _results ; } set { _results  = value;                                    } } private string _results ;
        public string         IdColumn { get { return _idColumn; } set { _idColumn = value;                                    } } private string _idColumn;
        public RichSqlCommand Cmd      { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Members for handling XML columns
        // ----------------------------------------------------------------------------------------
        private int             LastRow    { get { return _lastRow;    } set { _lastRow    = value; } } private int             _lastRow;
        private string          LastColumn { get { return _lastColumn; } set { _lastColumn = value; } } private string          _lastColumn;
        public  RichXmlDocument LastXdoc   { get { return _lastXdoc;   } set { _lastXdoc   = value; } }
        private RichXmlDocument _lastXdoc;
        private string          LastXml    { get { return _lastXml;    } set { _lastXml    = value; } } private string          _lastXml;


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        public DataRow this[int row] { get { if (0 <= row && row < Count) return Rows[row]; else return null; } }


        // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //  Constructors    (Data Source                             Table Name                                         Primary Key   Preparation                                              Initialize Table             Fill Table                                                                                   Administration
        // ---------------- ---------------------------------        -------------------------------------------------  -----------   -------------------------------------------------------  ---------------------------  -------------------------------------------------------------------------------------------  -------------------------------
        public RichDataTable(                                                                                                     ) {                                                          Init();                                                                                                                   _errors = "";                 }
        public RichDataTable(                                        string tableName                                  , string PK) {                                                          Init(tableName);                                                                                                          _errors = ""; _idColumn = PK; }
        public RichDataTable(DataTable              dt                                                                 , string PK) { if (dt == null)                                          Init();                else  _table = dt;                                                                                 _errors = ""; _idColumn = PK; }
        public RichDataTable(DataView               view                                                               , string PK) { if (view == null)                                        Init();                else  _table = view.ToTable();                                                                     _errors = ""; _idColumn = PK; }
        public RichDataTable(SqlDataReader          dr             , string tableName, bool disposeSrcRdr              , string PK) {                                                          Init(tableName);             dr = _FillFrom(dr, disposeSrcRdr);                                                                         _idColumn = PK; }  /// <summary>Constructs a RichDataTable from a "SELECT" stored procedure or text command</summary><param name="cmd">A command that returns a table as per a SELECT command</param><param name="tableName">What you want to name the table that is returned</param>
        public RichDataTable(RichSqlCommand cmd, SqlTransaction trx, string tableName                                  , string PK) { Cmd = cmd;                                           Init(tableName);             Create(cmd, trx);                                                                                          _idColumn = PK; }  /// <summary>Reads from a file into a data table, creating all string columns</summary>
        public RichDataTable(string                 path           , string tableName, List<string> columns            , string PK) {                                                          Init(tableName, columns);    List<string> str = FilesIO.GetListFromFile(path); for (int row = 0; row < str.Count; ++row) Add(str[row]); _idColumn = PK; }  /// <summary>Constructs a Data Table/schema from an object's properties using reflection</summary><remarks>intended to be used with Add(object obj) adding objects of the same class</remarks>
        public RichDataTable(object                 obj            , string tableName, bool deepBuild                  , string PK) { List<DataColumn> column = ListMembersOf(obj, deepBuild); Init(tableName);             Add(column);                                                                                 _errors = ""; _idColumn = PK; }
        public RichDataTable(SqlCommand             cmd            , string tableName, EndemeSet actions, Endeme action, string PK) { Cmd = new RichSqlCommand(cmd, actions, action);      Init(tableName);             Create(Cmd, null);                                                                                     _idColumn = PK; }  /// <summary>Constructs a table from a hash</summary>
        public RichDataTable(Dictionary<string,int> hash           , string tableName, string valCol                   , string PK) {                                                          Init(tableName, PK, valCol); foreach (string key in hash.Keys) { int row = Add(); this[row][PK] = key; this[row][valCol] = hash[key]; } _idColumn = PK; }
        public RichDataTable(EndemeSet              enSet          , string tableName, bool IncludeEmptyCharacteristics = false   ) {                                                          Init(tableName);             FillWithEndemeSet(enSet, "EndemeSetID", IncludeEmptyCharacteristics);                        _errors = "";                 }


        // ----------------------------------------------------------------------------------------
        //  Operators
        /* ------------------------------------------------------------------------------------- */        /// <remarks>TODO: prepends rows and columns, or appends rows and columns, depending on which table is bigger (in rows)</remarks>
        public static RichDataTable operator +(RichDataTable table1, RichDataTable table2) { return table1.Copy().Add(table2); } // add columns too?


        // ----------------------------------------------------------------------------------------
        //  Cell operations     Method        Row      Column                         Default         Check Existence          Default Out            Get Cell Value                                                                           Record Access                 Return Result
        /* ---------------------------------  -------  -------------  --------------  --------------  ------------------------------------  --------  ---------------------------------------------------------------------------------------  ----------------------------  ------------ */ /// <summary>Does its best to provide an exception-free boolean value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the method can't figure it out</param><returns>boolean representation of the cell value or default value</returns>
        public  bool            BoolValue    (int row, string column                , bool    dflt) { if (!Exists(row,column)) return dflt;           bool        bit  = TreatAs.BoolValue     (    this[row][column]         , dflt)        ; RecordLast(row,column      ); return bit ; } /// <summary>Does its best to provide an exception-free char    value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  byte[]          ByteValue    (int row, string column               /* null */     ) { if (!Exists(row,column)) return null;           byte[]      data = TreatAs.ByteValue     (    this[row][column]               )        ; RecordLast(row,column      ); return data; }
        public  char            CharValue    (int row, string column                , char    dflt) { if (!Exists(row,column)) return dflt;           char        cha  = TreatAs.CharValue     (ObjValue(row, column,null)    , dflt)        ; RecordLast(row,column      ); return cha ; } /// <summary>Does its best to provide an exception-free decimal value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  decimal         DecValue     (int row, string column                , decimal dflt) { if (!Exists(row,column)) return dflt;           decimal     dnum = TreatAs.DecValue      (    this[row][column]         , dflt)        ; RecordLast(row,column      ); return dnum; } /// <summary>Does its best to provide an exception-free GUID    value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column name  </param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  Guid            GuidValue    (int row, string column                , Guid    dflt) { if (!Exists(row,column)) return dflt;           Guid        guid = TreatAs.GuidValue     (    this[row][column]         , dflt)        ; RecordLast(row,column      ); return guid; } /// <summary>Does its best to provide an exception-free integer value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the method can not figure it out</param>
        public  int             IntValue     (int row, string column                , int     dflt) { if (!Exists(row,column)) return dflt;           int         inum = TreatAs.IntValue      (    this[row][column]         , dflt)        ; RecordLast(row,column      ); return inum; } /// <summary>Does its best to provide an exception-free float   value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  float           FloatValue   (int row, string column                , float   dflt) { if (!Exists(row,column)) return dflt;           float       fnum = TreatAs.FloatValue    (    this[row][column]         , dflt)        ; RecordLast(row,column      ); return fnum; } /// <summary>Does its best to provide an exception-free long    value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  long            LongValue    (int row, string column                , long    dflt) { if (!Exists(row,column)) return dflt;           long        lnum = TreatAs.LongValue     (    this[row][column]         , dflt)        ; RecordLast(row,column      ); return lnum; } /// <summary>Does its best to provide an exception-free object  value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  object          ObjValue     (int row, string column                , object  dflt) { if (!Exists(row,column)) return dflt;           object      obj  =                            this[row][column]                        ; RecordLast(row,column      ); return obj ; } /// <summary>Converts a cell value to a string, using the default value when null or nonexistent, This method never blows up so exceptions need to be thrown above this level in the code</summary>
        public  double          RealValue    (int row, string column                , double  dflt) { if (!Exists(row,column)) return dflt;           double      rnum = TreatAs.RealValue     (    this[row][column]         , dflt)        ; RecordLast(row,column      ); return rnum; } /// <summary>Does its best to provide an exception-free double  value from a cell using culture info, defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  double          RealValue    (int row, string column, CultureInfo ci, double  dflt) { if (!Exists(row,column)) return dflt;           double      n    = TreatAs.RealValue     (    this[row][column], ci     , dflt)        ; RecordLast(row,column      ); return n   ; } /// <summary>Does its best to provide an exception-free SqlDate value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="defaultValue">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  SqlDateTime     SqlDateValue (int row, string column                              ) {                                                 SqlDateTime date = InData .GetSqlDateTime(ObjValue(row, column,null)          )        ; RecordLast(row,column      ); return date; } /// <summary>Does its best to provide an exception-free SQLInt  value from a cell which defaults to null if there would have been an exception</summary>
        public  SqlInt32        SqlIntValue  (int row, string column          /* SqlInt32.Null */ ) { if (!Exists(row,column)) return SqlInt32.Null;  SqlInt32    inum = InData .GetSqlInt32   (    this[row][column]               )        ; RecordLast(row,column      ); return inum; } /// <summary>Converts a cell value to a string, using the default value when null or nonexistent, This method never blows up so exceptions need to be thrown above this level in the code</summary>/// <param name="dflt">value returned if the cell is nonexistent or its value is null</param>
        public  string          RandomValue  (         string column, Random r      , string  dflt) { if (!Exists(0  ,column)) return dflt;           int row = r.Next(Count); string s =       StrValue(row, column          , dflt)        ; RecordLast(row,column      ); return s   ; }
        public  object          ObjValue     (int row, int    col                   , object  dflt) { if (!Exists(row,col   )) return dflt;           object      obj  =         ObjValue (row, _table.Columns[col].ColumnName, dflt)        ; RecordLast(row,col         ); return obj ; } /// <summary>Does its best to provide an exception-free GUID    value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="col"   >cell column number</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  Guid            GuidValue    (int row, int    col                   , Guid    dflt) { if (!Exists(row,col   )) return dflt;           Guid        guid =         GuidValue(row, _table.Columns[col].ColumnName, dflt)        ; RecordLast(row,col         ); return guid; }
        public  int             IntValue     (int row, int    col                   , int     dflt) { if (!Exists(row,col   )) return dflt;           int         num  =         IntValue (row, _table.Columns[col].ColumnName, dflt)        ; RecordLast(row,col         ); return num ; } /// <summary>Returns the number of a random row in string format</summary>
        public  RichXmlDocument XmlDocValue  (int row, string column               /* null */     ) { if (!Exists(row,column)) return null; string xml    = StrValue(row,column,""); RichXmlDocument x = new RichXmlDocument(xml)            ; RecordLast(row,column,x,xml); return x   ; } /// <summary>Sets the value of a row/column to a string</summary>
        public  int?            IntValue     (int row, string column               /* null */     ) { object obj = this.ObjValue(row, column, null); if (obj == null || obj is DBNull) return null; else return IntValue(row, column, -1); }
        public  void            Set          (int row, string column, string str                  ) { if (!Exists(row,column)) return     ;                                                         this[row][column] = str                  ; RecordLast(row,column      );              } /// <summary>Sets the value of a row/column to an xml</summary>
        public  void            Set          (int row, string column, SqlXml xml                  ) { if (!Exists(row,column)) return     ;                                                         this[row][column] = xml                  ; RecordLast(row,column      );              } /// <summary>Counts the number of matches in a string representation of the cell to the pattern</summary>
        public  int             _Count       (int row, string column, string pattern              ) { Regex regex = new Regex(pattern)    ;           string      str  =         StrValue      (         row, column, ""); MatchCollection match = regex.Matches(str); return match.Count;} /// <summary>Determines whether the specified (row X column) exists</summary><param name="row">row number</param><param name="column">column name</param>
        public  bool            Exists       (int row, string column                              ) { return (_table != null && 0 <= row && row < Count && Contains(column));                                                                                                             } /// <param name="row">row number</param><param name="col">column number</param>
        public  bool            Exists       (int row, int    col                                 ) { return (_table != null && 0 <= row && row < Count && 0 <= col && col < _table.Columns.Count);                                                                                       } /// <summary>Records the most recent extraction from the datatable</summary>
        private void            RecordLast   (int row, string column, RichXmlDocument xdoc, string xml) { LastXml = xml; LastXdoc = xdoc                                                                                                     ; RecordLast(row,column      );              }
        private void            RecordLast   (int row, string column                              ) { if (row >= 0) LastRow = row; LastColumn = column;                                                                                                                                   } /// <summary>Records the most recent xml extraction from the datatable</summary>
        private void            RecordLast   (int row, int    col                                 ) { if (row >= 0) LastRow = row; LastColumn = _table.Columns[col].ColumnName;                                                                                                           }
        public  string          StrValue     (int row, int    col                   , string  dflt) { if (!Exists(row,col   )) return dflt; string column = _table.Columns[col].ColumnName; string str = StrValue(row, column, dflt)         ; RecordLast(row,column      ); return str ; }
        public  string          StrValue     (int row, string column               /* null */     ) { if (!Exists(row,column)) return null; string s = TreatAs.StrValue(ObjValue(row,column,null))                                           ; RecordLast(row,column      ); return s   ; }
        public  string          StrValue     (int row, string column                , string  dflt) { if (!Exists(row,column)) return dflt;           string      str  = TreatAs.StrValue     (    this[row][column]     , dflt)             ; RecordLast(row,column      ); return str ; } /// <summary>Does its best to provide an exception-free double  value from a cell which defaults as specified if there would have been an exception</summary><param name="row">cell row</param><param name="column">cell column</param><param name="dflt">value returned if the cell is null or does not exist or is a nonconvertable format or type</param>
        public  string          StrValue     (int row, string a, string b           , string  dflt) {                          return StrValue(row, a, StrValue(row, b                                                   , dflt   )); }
        public  string          StrValue     (int row, string a, string b, string c , string  dflt) {                          return StrValue(row, a, StrValue(row, b, StrValue(row, c                                  , dflt  ))); }
        public  string          StrValue     (int row, string a, string b, string c, string d, string dflt) {                  return StrValue(row, a, StrValue(row, b, StrValue(row, c, StrValue(row, d                 , dflt )))); }
        public  string          StrValue     (int row, string a, string b, string c, string d, string e, string dflt) {        return StrValue(row, a, StrValue(row, b, StrValue(row, c, StrValue(row, d, StrValue(row, e, dflt))))); }

        /// <summary>
        /// Don't use this, this is for ntext only, ntext is deprecated
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public String TextValue(int row, string column)
        {
            if (!Exists(row,column)) return "";
            object obj = this[row][column];
            if (obj == null) return "";
            Type ctype = _table.Columns[column].DataType;
            Type dtyp = obj.GetType();
            if (dtyp == typeof(String)) return (String)obj;
            if (dtyp == typeof(DBNull)) return "";
            throw new FormatException("Use TextValue for ntext columns only");
        }
 
        // ----------------------------------------------------------------------------------------
        //  Cell accessors to be rewritten (because TimeDate_old is deprecated)
        /* ------------------------------------------------------------------------------------- */                                                                                                                                                                                                         /// <summary>Returns a SqlDateTime which allows null dates</summary><param name="dflt">may use SqlDateTime.Null</param>
        public  SqlDateTime     SqlDateValue (int row, string column                   , SqlDateTime  dflt) { SqlDateTime  date; if (Exists(row, column)) date = TimeDate_old.SqlDate(this[row][column], dflt); else date = dflt ; RecordLast(row, column); return date; } /// <summary>Returns a regular System.DateTime (which does not allow null dates so this can be a problem)</summary>
        public  DateTime        DateValue    (int row, string column                   , DateTime     dflt) { DateTime     date; if (Exists(row, column)) date = TimeDate_old.ClrDate(this[row][column], dflt); else date = dflt ; RecordLast(row, column); return date; } /// <summary>Returns a TimeDate_old from a cell</summary>
        public  TimeDate_old    TimeValue    (int row, string column, DateTimeKind kind, TimeDate_old dflt) { TimeDate_old date; if (Exists(row, column)) { object obj = this[row][column]; if (obj is SqlDateTime) date = (new TimeDate_old((SqlDateTime)obj, kind)); else date = (new TimeDate_old(TreatAs.StrValue(obj, dflt.ToString()), kind)); } else date = dflt; RecordLast(row, column); return date; }
        public  TimeDate_old    XmlDate      (int row, string column, string nameSpace , string      xpath) { string text = _XmlStr(row, column, nameSpace, xpath); TimeDate_old date = new TimeDate_old(text); RecordLast(row, column); return date; }
        public  DateTime        MaxValue     (         string column                   , DateTime     dflt)
        {
            DateTime max = TimeDate_old.MinSqlValue.CLRFormat;
            if (Exists(0, column)) { for (int row = 0; row < Count; ++row) if (DateValue(row, column, dflt) > max) max = DateValue(row, column, dflt); } else max = dflt;
            return max;
        }


        // ----------------------------------------------------------------------------------------
        //  Column operations using row traversal           Column    Values        Defaults         Check Existence                      Initialize Results                                                    Row Traversal                      Row by row operations                    (row, column , dflt )                                                 Administration      Return Results
        /* -------------------------------------------------------  --------------  ---------------  -----------------------------------  --------------------------------------------------------------------  --------------------------------   -------------------------------------------------------------------------------------------------------------  ------------------  ----------- */ /// <summary>Adds sort of an identity id column to the table starting with 1, and fills it</summary>
        public void                       Add        (string column, int    startId               ) { if (!Exists(        )) return      ;                           Add(column, typeof(int))                  ; for (int row = 0;row<Count;++row) { this[row][column] = startId +             row;                                                              } RecordLast(column);              }
        public bool                       AllTheSame (string column                               ) { if (!Exists(  column)) return false; string                    first = StrValue(0, column, "")           ; for (int row = 0;row<Count;++row) {                     if (     StrValue    (row, column , ""   ) != first) return false;                      } RecordLast(column); return true; } /// <summary>Changes the type of a column and migrates the data</summary>
        public bool                       ChangeType (string column, Type   type   , double   dflt) { if (!Exists(  column)) return false;                           Rename(column, "XQVZ"); Add(column, type) ; for (int row = 0;row<Count;++row) { this[row][column] =          RealValue   (row, "XQVZ" , dflt );             } Remove("XQVZ")                ; RecordLast(column); return true; } /// <summary>Changes the type of a column and migrates the data</summary>
        public bool                       ChangeType (string column, Type   type   , int      dflt) { if (!Exists(  column)) return false;                           Rename(column, "XQVZ"); Add(column, type) ; for (int row = 0;row<Count;++row) { this[row][column] =          IntValue    (row, "XQVZ" , dflt );             } Remove("XQVZ")                ; RecordLast(column); return true; } /// <summary>Changes the type of a column and migrates the data</summary>
        public bool                       ChangeType (string column, Type   type   , string   dflt) { if (!Exists(  column)) return false;                           Rename(column, "XQVZ"); Add(column, type) ; for (int row = 0;row<Count;++row) { this[row][column] =          StrValue    (row, "XQVZ" , dflt );             } Remove("XQVZ")                ; RecordLast(column); return true; }
        public int                        _MatchCount(string column, string pattern               ) { if (!Exists(  column)) return 0    ; int                       tot  = 0                                  ; for (int row = 0;row<Count;++row) {            if (Regex.IsMatch(StrValue    (row, column , ""   ), pattern)) tot++;                            } RecordLast(column); return tot ; } /// <summary>Returns the maxium character length of the value in a column</summary>
        public int                        MaxLength  (string column, int    maxRowCount           ) { if (!Exists(  column)) return 0    ; int max = 0; int          cnt  = Math.Min(Count, maxRowCount)       ; for (int row = 0;row<cnt  ;++row) { max      = Math.Max(max,     StrValue    (row, column , ""   ).Length);                                     } RecordLast(column); return max ; } /// <summary>Returns the minimum value of a column</summary>
        public int                        MinValue   (string column                               ) {                                      int                       min  = IntValue(0, column, int.MinValue)  ; for (int row = 0;row<Count;++row) { min      = Math.Min(min,     (int)Rows   [row][column]       );                                             } RecordLast(column); return min ; } /// <summary>Returns the minimum date of a column</summary>
        public DateTime                   MinValue   (string column                , DateTime dflt) { if (!Exists(  column)) return dflt ; DateTime                  min  = System.DateTime.MaxValue           ; for (int row = 0;row<Count;++row) {            if (DateValue(row,column,dflt) < min) min = DateValue(row,column,dflt);                          } RecordLast(column); return min ; } /// <summary>Returns the number of the first where the column equals the value</summary><param name="candy">candy does not do anything</param>
      //public int                        RowWhere   (string column, Is candy, int value          ) { if (!Exists(0,column)) return -1   ; int                       fnd  = -1; int notEq = value + 100        ; for (int row = 0;row<Count;++row) {                     if (     IntValue    (row, column, notEq) == value) found = row;                        } RecordLast(column); return foun; } /// <summary>Sets all of the values in a column to a double value</summary>
        public bool                       Set        (string column, double value                 ) { if (!Exists(0,column)) return false;                                                                       for (int row = 0;row<Count;++row) {                                                                                  this[row][column] = value; } RecordLast(column); return true; } /// <summary>Sets all of the values in a column to a string value</summary>
        public bool                       Set        (string column, string value                 ) { if (!Exists(0,column)) return false;                                                                       for (int row = 0;row<Count;++row) {                                                                                  this[row][column] = value; } RecordLast(column); return true; } /// <summary>Sets all of the values in a column to an int value</summary><remarks>test me</remarks>
        public bool                       Set        (string column, int    value                 ) { if (!Exists(0,column)) return false; if (!IsInteger(column))       return false                          ; for (int row = 0;row<Count;++row) {                                                                                  this[row][column] = value; } RecordLast(column); return true; } /// <summary>Sets all of the values in a column to a float value</summary>
        public bool                       Set        (string column, float  value                 ) { if (!Exists(0,column)) return false;                                                                       for (int row = 0;row<Count;++row) {                                                                                  this[row][column] = value; } RecordLast(column); return true; } /// <summary>Sets one column to another (if the 'to column' is a string type)</summary>
        public bool                       Set        (string column, string colFrom, string   dflt) { if (!Exists(  column)) return false; if (!Exists(colFrom))         return false                          ; for (int row = 0;row<Count;++row) { string      str = StrValue    (row, colFrom, dflt           );                   this[row][column] = str;   } RecordLast(column); return true; } /// <summary>Sets one column to another (if the 'to column' is a string type)</summary>
        public bool                       Set        (string column, string colFrom, double   dflt) { if (!Exists(  column)) return false; if (!Exists(colFrom))         return false                          ; for (int row = 0;row<Count;++row) { double      num = RealValue   (row, colFrom, dflt           );                   this[row][column] = num;   } RecordLast(column); return true; } /// <summary>Concatenates two columns into a third</summary><param name="column">the destination column</param><param name="A">the first source column</param><param name="join"></param><param name="B">the second source column</param>
        public bool                       Set        (string column, string A,string join,string B) { if (!Exists(  column)) return false; if (!Exists(A) || !Exists(B)) return false                          ; for (int row = 0;row<Count;++row) { string      str = StrValue    (row, A, "") + join + StrValue(row, B, "");        this[row][column] = str;   } RecordLast(column); return true; } /// <summary>Sets all the values of a column to a particular literal</summary>
        public bool                       SetColumn  (string column, object obj    , string   dflt) { if (!Exists(  column)) return false; string                    str  = TreatAs.StrValue(obj, dflt)        ; for (int row = 0;row<Count;++row) {                                                                                  this[row][column] = str;   } RecordLast(column); return true; } /// <summary>Returns the values of the column, AND'ed together</summary>
        public bool                       ColumnAnd  (string column                , bool     dflt) { if (!Exists(0,column)) return dflt ; bool                      sum  = true                               ; for (int row = 0;row<Count;++row) { sum            &= BoolValue   (row, column , false          );                                              } RecordLast(column); return sum ; } /// <summary>Returns the values of the column, OR'ed together</summary>
        public bool                       ColumnOr   (string column                , bool     dflt) { if (!Exists(0,column)) return dflt ; bool                      sum  = false                              ; for (int row = 0;row<Count;++row) { sum            |= BoolValue   (row, column , false          );                                              } RecordLast(column); return sum ; } /// <summary>Returns the sum of the values in a column</summary>
        public float                      Sum        (string column                , float    dflt) { if (!Exists(  column)) return dflt ; float                     sum  = 0.0F                               ; for (int row = 0;row<Count;++row) { sum            += FloatValue  (row, column , 0.0F           );                                              } RecordLast(column); return sum ; } /// <summary>Returns the sum of the values in a column</summary>
        public int                        Sum        (string column                , int      dflt) { if (!Exists(  column)) return dflt ; int                       sum  = 0                                  ; for (int row = 0;row<Count;++row) { sum            += IntValue    (row, column , 0              );                                              } RecordLast(column); return sum ; } /// <summary>Returns the sum of the values in a column</summary>
        public double                     Sum        (string column                , double   dflt) { if (!Exists(  column)) return dflt ; double                    sum  = 0.0                                ; for (int row = 0;row<Count;++row) { sum            += RealValue   (row, column , 0.0            );                                              } RecordLast(column); return sum ; } /// <summary>Prodcues a tally of the values in a column as strings</summary>
        public Tally                      Tally      (string column, bool includeBlanks           ) { if (!Exists(  column)) return null ; Tally                     tly  = new Tally()                        ; for (int row = 0;row<Count;++row) { string      str = StrValue    (row, column , ""             );                 tly.Incr(str,includeBlanks); } RecordLast(column); return tly ; } /// <summary>Produces a tally of the integer values in a column</summary>
        public bool                       Trim       (string column                , string   dflt) { if (!Exists(0,column)) return false;                                                                       for (int row = 0;row<Count;++row) { string      str = StrValue    (row, column , dflt           ).Trim();             this[row][column] = str;  } RecordLast(column); return true; }
        public bool                       TrimStart  (string column, char trimMe   , string   dflt) { if (!Exists(0,column)) return false;                                                                       for (int row = 0;row<Count;++row) { string      str = StrValue    (row, column , dflt ).TrimStart("0".ToCharArray()); this[row][column] = str;  } RecordLast(column); return true; }
        public List      <int           > ToList     (string column               , int       dflt) { List      <int           > list = new List      <int           >(Count); if (!Exists(0,column)) return null; for (int row=0;row<Count;++row) { int         num = IntValue    (row, column , dflt           );                    list.Add(num);            } RecordLast(column); return list; } /// <summary>Returns a table column as a List&lt;SqlDateTime></summary>
        public List      <object        > ToList     (string column               , object    dflt) { List      <object        > list = new List      <object        >(Count); if (!Exists(0,column)) return null; for (int row=0;row<Count;++row) { object      obj = Rows        [row][column]                  ;                    list.Add(obj);            } RecordLast(column); return list; } /// <summary>Returns a table column as a List&lt;Guid></summary>
        public List      <Guid          > ToList     (string column               , Guid      dflt) { List      <Guid          > list = new List      <Guid          >(Count); if (!Exists(0,column)) return list; for (int row=0;row<Count;++row) { Guid        id  = GuidValue   (row, column , dflt           );                    list.Add(id );            } RecordLast(column); return list; }
        public List      <SqlDateTime   > ToList     (string column, SqlDateTime dateTime         ) { List      <SqlDateTime   > list = new List      <SqlDateTime   >(Count); if (!Exists(0,column)) return null; for (int row=0;row<Count;++row) { SqlDateTime sdt = SqlDateValue(row, column                  );                    list.Add(sdt);            } RecordLast(column); return list; } /// <summary>Returns a list of the values in a column as strings</summary>
        public List      <string        > ToList     (string column               , string    dflt) { List      <string        > list = new List      <string        >(Count); if (!Exists(0,column)) return list; for (int row=0;row<Count;++row) { string      str = StrValue    (row, column , dflt           );                    list.Add(str);            } RecordLast(column); return list; }
        public Dictionary<int   ,DataRow> Index      (string column               , int       dflt) { Dictionary<int   ,DataRow> indx = new Dictionary<int   ,DataRow>(Count); if (!Exists(  column)) return null; for (int row=0;row<Count;++row) { int         num = IntValue    (row, column , -1-row /* not safe */ );             indx.Add(num, this[row]); } RecordLast(column); return indx; } /// <summary>Trims leading and trailing spaces from every value in a string column</summary>
        public Dictionary<string,DataRow> Index      (string column               , string    dflt) { Dictionary<string,DataRow> indx = new Dictionary<string,DataRow>(Count); if (!Exists(  column)) return null; for (int row=0;row<Count;++row) { string      str = StrValue    (row, column , Guid.NewGuid().ToString());          indx.Add(str, this[row]); } RecordLast(column); return indx; }
        public Dictionary<int   ,int    > Tally      (string column                               ) { Dictionary<int   ,int    > t    = new Dictionary<int   ,int    >()     ; if (!Exists(  column)) return null; for (int row=0;row<Count;++row) { int         num = IntValue    (row, column , -1             ); if(!t.ContainsKey(num))t.Add(num,0);t[num]++;} RecordLast(column); return t   ; } /// <summary>Returns an array of string of the values in a column</summary>
        public List      <string        > ToList     (string column                               ) { List      <string        > list = new List      <string        >(Count); if (!Exists(0,column)) return list; for (int row=0;row<Count;++row) { object      obj = this        [row][column]                  ;   if (obj != null) list.Add(obj.ToString()); } RecordLast(column); return list; } /// <summary>Returns a table column as a List&lt;int></summary>
        public ArrayList                  ToArrayList(string column                               ) { ArrayList                  arry = new ArrayList                 (Count); if (!Exists(0,column)) return null; for (int row=0;row<Count;++row) { object      obj = this        [row][column]                  ;   if (obj != null) arry.Add(obj.ToString()); } RecordLast(column); return arry; } /// <summary>Returns a table column as a List&lt;string></summary>
        public            Guid  []        _Guids     (string column                               ) {            Guid  []        arry = new            Guid           [Count]; if (!Exists(  column)) return arry; for (int row=0;row<Count;++row) { Guid        id  = GuidValue   (row, column , Guid.Empty     );                    arry[row] = id;           } RecordLast(column); return arry; } /// <summary>Counts the number rows containing items matching the input pattern</summary>
        public            string[]        ToArray    (string column                               ) {            string[]        arry = new            string         [Count]; if (!Exists(  column)) return null; for (int row=0;row<Count;++row) { string      str = StrValue    (row, column , ""             );                    arry[row] = str;          } RecordLast(column); return arry; } /// <summary>Returns an array of string of the values in two columns</summary>
        public            string[]        ToArray    (string column, string join, string colB     ) {            string[]        arry = new            string         [Count]; if (!Exists(  column)) return null; for (int row=0;row<Count;++row) { string      str = StrValue    (row, column , ""             ); arry[row] = str+join+StrValue(row,colB,"");  } RecordLast(column); return arry; } /// <summary>Copies a table column into an ArrayList of strings, Add column values from DataTable to ArrayList</summary>


        // ----------------------------------------------------------------------------------------
        //  Other column operations        Column
        /* ------------------------------------------------------------------------------------- */                                                                                                                                                                               /// <summary>Adds the columns of one table to another given a 'joining' column, returning whether it added or not</summary>/// <remarks>It is usually better to let SQL Server do joins - if you can, use SQL, T-SQL, a view, SP or Fn</remarks>
        public  bool          Add         (DataColumn column, List<string> list) { bool added = false; if (!Contains(column.ColumnName)) { list.Add(column.ColumnName); Add(column.ColumnName, column.DataType); added = true; }                           return added; }        /// <summary>Adds columns from the collection that are missing in the current table</summary>
        public  void          Add         (DataColumnCollection columns        ) {                                         for (int col = 0; col <       columns.Count; ++col) { if (!Contains(columns[col].ColumnName)) Columns.Add(Copy(columns[col])); }              }
        private void          Add         (List<DataColumn> columns            ) {                                         for (int col = 0; col <       columns.Count; ++col) {                                         Columns.Add(     columns[col]) ; }              }        /// <summary>Adds the columns of one table to another given a 'joining' column from each table, only adds if 'join' joins to only one row in the input table</summary><returns>a list of the new columns created</returns><remarks>It is usually better to let SQL Server do joins - if you can, use SQL, T-SQL, a view, SP or Fn</remarks>
        private List<string>  Add         (RichDataTable input, string inputPK ) { List<string> list = new List<string>(); for (int col = 0; col < input.Columns.Count; ++col) { bool added = Add(input.Columns[col], list)                             ; } return list; }
        private string        AsciiLine   (List<int> width, char line          ) { string delim = ""; string str = "";     for (int col = 0; col <       Columns.Count; ++col) { str += delim + line.ToString().PadRight(width[col], line); delim = "+" ; } return str ; }
        public  string        ColumnList  (                                    ) { string str = ""; string delim = ""; foreach (DataColumn column in Columns) { str += delim + column.ColumnName; delim = ", "; } return str;   }        /// <summary>Adds a string row to a table as delimited</summary><param name="separatorChars">recommend "\t".ToCharArray()</param>
        public  bool          Contains    (string column                       ) { if (_table == null) return false;                                                                    return _table.Columns.Contains(column); }        /// <summary>Destructively retrieves column value from a random row in a table, removing one or more rows in the process</summary>
        public  string        DrawRandom  (string column                       ) { string str = ""; Random r = RandomSource.New().Random; for (int i = 0; Count > 0 && i < 10 && __.StringIsNullOrWhiteSpace(str); ++i) { int row = r.Next(Count); str = StrValue(row, column, ""); RemoveAt(row); } return str; }
        private string        Error       (int    col   , string type          ) {                                         return "Error - column number '" + col + "' out of range in RichDataTable."+type+"Value(row, col)."; }
        private bool          Exists      (string column                       ) {                                         return (Exists() && Contains(column));                                                               }
        private bool          Exists      (int    col                          ) {                                         return (Exists() && 0 <= col && col < Columns.Count);                                                }
        private bool          IsInteger   (string column                       ) { Type type = Columns[column].DataType; return (type == typeof(long) || type == typeof(Int64) || type == typeof(int) || type == typeof(Int32) || type == typeof(short) || type == typeof(Int16)); }
        public  string        PrettyColumnList(                                ) { string str = ""; foreach (DataColumn col in Columns  ) { string name = col.ColumnName; Type type = col.DataType; str += "\r\n" + name + "  ---  " + type.Name; } return str; }
        private void          RecordLast  (string column                       ) { RecordLast(-1,column);                                                                                                                       }
        public  bool          Remove      (string column                       ) { if (!Contains(column)) return false; _table.Columns.Remove(column);                                                 return (!Contains(column)); }        /// <summary>Renames a column if the column exists</summary>
        public  bool          Rename      (int    col   , string newName       ) { bool renamed = true; if (Exists(col   ) && !Exists(newName)) _table.Columns[col   ].ColumnName = newName; else renamed = false; return renamed; }
        public  bool          Rename      (string column, string newName       ) { bool renamed = true; if (Exists(column) && !Exists(newName)) _table.Columns[column].ColumnName = newName; else renamed = false; return renamed; }        /// <summary>Sets the PrimaryKey array to the specified column</summary>
        public  void          SetPK       (int    col                          ) { DataColumn[]  key = new DataColumn[1]; key[0] = Columns[col]; PrimaryKey = key;                                                              }        /// <summary>Returns the lowest and highest dates in a column</summary>
        public  SqlDateTime[] SqlDateRange(string column                       ) { RichDataTable tbl = SortCopy("BY " + column); SqlDateTime[] range = new SqlDateTime[2]; range[0] = tbl.SqlDateValue(0 , column); range[1] = tbl.SqlDateValue(tbl.Count-1, column); return range; }


        // ----------------------------------------------------------------------------------------
        //  Row operations       Row                                                           Get Row Number                                           Check Existence                                                Traverse Columns                     Perform Operations                                                              Return Row Number
        /* --------------------  ------------------------------------------------------------  -------------------------------------------------------  -------------------------------------------------------------  -----------------------------------  -----------------------------------------------------------------------------   ----------- */ /// <summary>Adds a blank row to the table, returning the index of the row added</summary><returns>the index of the added row</returns>
        public  int     Add     (                                                          ) { int     row  = Count; Rows.Add(NewRow());                                                                                                                                                                                                    return row ; } /// <summary>Adds a row to the table</summary>
        public  int     Add     (Guid     id     , string PK  , string column, string value) { int     row  = -1   ;                                    if (Exists(PK)                  && Exists(column )) {                                               row = Add(); this[row][PK]      = id  ; this[row][column ] = value          ; } return row ; } 
        public  int     Add     (int      id     , string PK  , string column, string text ) { int     row  = -1   ;                                    if (Exists(PK) && IsInteger(PK) && Exists(column )) {                                               row = Add(); this[row][PK]      = id  ; this[row][column ] = text.ToString(); } return row ; } 
        public  int     Add     (string   columnA, string valA, string columnB, string valB) { int     row  = -1   ;                                    if (Exists(columnA)             && Exists(columnB)) {                                               row = Add(); this[row][columnA] = valA; this[row][columnB] = valB           ; } return row ; } 
        public  int     Add     (DataRow  dataRow                                          ) { int     row  = Count; try {                              if (dataRow.Table == Table                        )                                                 Rows.Add(dataRow); else ImportRow(dataRow); } catch { row--; }                  return row ; } /// <summary>Adds an object as a row in a table, intended to be used with new RichDataTable(object obj, string tableName);</summary>
        public  int     Add     (int      fromRow, RichDataTable fromTable                 ) { int     row  = Count;                                    if (0 <= fromRow && fromRow < fromTable.Count     ) {                                               DataRow copy = Copy(fromTable.Rows[fromRow]); Rows.Add(copy);                 } return row ; } /// <summary>Add rows from a split csv file (except line 0)</summary>
        private int     Add     (string[] csvLine, string[] columnList                     ) { int     row  = Count;                                                                                                   for (int col = 1; col < csvLine.Length; ++col) { Add(csvLine[col], ',', columnList);                               } return row ; }
        private int     Add     (string   csvLine, char[]   separatorChars                 ) { int     row  = Add(); string[] cell = csvLine.Split(separatorChars);  int count = Math.Min(cell.Length, Columns.Count); for (int col = 0; col < count         ; ++col) { this[row][col]      = cell[col] ;                                 } return row ; } /// <summary>Adds a tab delimited line as a row, assuming that the columns are in the same order as the content of the line</summary>
        public  int     Add     (string   line   , char  delim, string[] column            ) { int     row  = Add(); string[] cell = line.Split(delim); if (cell.Length   == column.Length                )            for (int col = 0; col < column.Length ; ++col) { Set(row, column[col], cell[col]);                                 } return row ; } /// <summary>Adds a row from a csv file to the table</summary><param name="column">the columns in the csv file</param>
        public  int     Add     (string   line                                             ) { int     row  = Add(); string[] cell = line.Split("\t".ToCharArray()); int count = Math.Min(cell.Length, Columns.Count); for (int col = 0; col < count         ; ++col) { this[row][col]      = cell[col] ;                                 } return row ; } 
        public  int     Add     (object   obj                                              ) { int     row  = Add();    DataColumnCollection member = Columns; Type type = obj.GetType();                              for (int col = 0; col < member.Count  ; ++col) { Add(row, member[col].ColumnName, obj, type);                      } return row ; }
        private string  AsciiRow(int      row    , string colDelim, List<int> width        ) { string  str  = ""   ; string delim = "";                                                                                for (int col = 0; col < Columns.Count ; ++col) { str += delim + __.SetLength(width[col], StrValue(row, col, "")); delim = colDelim; } return str; } /// <summary>Adds a row to the table (a real copy not a reference) from the passed table, tables must have the same number of columns with the same types in the same order</summary><param name="table">table with the same type signature</param><param name="fromRow">row number to add</param>
        public  DataRow Copy    (DataRow  from                                             ) { DataRow copy = NewRow(); DataColumnCollection column = Columns;                                                         for (int col = 0; col < column.Count  ; ++col) { copy[column[col].ColumnName] = from[column[col].ColumnName];      } return copy; } 


        // ----------------------------------------------------------------------------------------
        //  Other short methods and properties
        /* ------------------------------------------------------------------------------------- */        /// <summary>Adds all the rows of one table to another</summary><remarks>add missing columns to this and to copy</remarks>
        public           RichDataTable Add          (RichDataTable table                              ) { RichDataTable copy = table.Copy(); Add(copy.Columns); copy.Add(Columns); for (int row = 0; row < copy.Count; ++row) Add(row, copy);                                                         return this   ; }        /// <summary>Adds the columns of one table to another and fills them given a 'joining' column from each table, only adds if 'join' joins to only one row in the input table</summary><remarks>It is usually better to let SQL Server do the work, if you can, use SQL, T-SQL, a view, SP or Fn</remarks>
        public           void          Add          (RichDataTable table    , string PK, string thisFK) { /* Add columns as necessary */ List<string> newColumn = Add(table, PK); /* Now fill the cells */ Set(newColumn, thisFK, table, PK);                                                                         }        /// <summary>Adds the values and columns of a csv file to the table, defaults column types to string</summary>
        public           RichDataTable Add          (string        csv      , StringFormat format     ) { string[] line   = csv.Split("\r\n".ToCharArray(),StringSplitOptions.RemoveEmptyEntries); if (line.Length > 0) { string[] columnList = Add(line[0], ','); Add(line, columnList); }           return this   ; }        /// <summary>Adds any missing columns (csv add)</summary>
        public           string[]      Add          (string        csvLine1 , char delim              ) { string[] column = csvLine1.Split(delim); for (int name = 0; name < column.Length; ++name) { string columnName = column[name]; if (!Contains(columnName)) Add(columnName, typeof(string)); } return column ; }        /// <summary>Returns a table grid as an ASCII string, is a property for debugging</summary>
        public           string        AsciiTable                                                 { get { string   ascii  = ""; if (Columns.Count > 0) { ascii = AsciiGrid(120, 40, '-', "|"); } else { ascii = "no columns found"; }                                                                 return ascii  ; } }
        public  static   RichDataTable Empty                                                      { get { RichDataTable empty = new RichDataTable();                                                                                                                                                  return empty  ; } }
        public           SqlDataReader _FillFrom    (SqlDataReader reader   , bool disposeSourceReader) { DataReaderAdapter dra = new DataReaderAdapter(); dra.FillFromReader(Table, reader); if (disposeSourceReader) { reader.Dispose(); reader = null; }                                           return reader ; }        /// <summary>Initializes the specified table</summary>
        private          int           Init         (string        tableName, List<string> column     ) { Init(tableName); int count = 0; foreach (string col in column) if (!Contains(col)) { Add(col, typeof(string)); count++; }                                                                   return count  ; }
        private          int           Init         (string        tableName, string PK, string valCol) { Init(tableName); int count = 1; Columns.Add(PK, typeof(string)); if (!Contains(valCol)) { Columns.Add(valCol, typeof(int)); count++; }                                                      return count  ; }        /// <summary>Sort of like string.IsNullOrWhitespace</summary>
        public  static   bool          IsNullOrEmpty(RichDataTable table                              ) { bool          isEmpty = (table == null || table.Count == 0);                                                                                                                                return isEmpty; }        /// <summary>Sorts the actual RichDataTable by passing it through a DataView</summary>
        public           RichDataTable Sort         (                         string orderByClause    ) { RichDataTable table   = SortCopy(orderByClause); Clear(); Merge(table);                                                                                                                     return this   ; }        /// <summary>returns a copy of the sorted table, does not sort the actual table</summary>
        private          RichDataTable SortCopy     (                         string orderByClause    ) { DataView      view    = View; view.Sort = BodyOf(orderByClause); view.ApplyDefaultSort = true; RichDataTable sorted = new RichDataTable(view, "");                                          return sorted ; }
        public  override string        ToString     (                                                 ) { string        output  = TreatAs.StrValue(TableName, "") + "(" + Count.ToString() + ")";                                                                                                     return output ; }
        private          BindingFlags  Bound                                                      { get { return (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a member of an object as a cell in a row, intended to be used with RichDataTable(object obj, string tableName);
        /// </summary>
        /// <param name="row"></param>
        /// <param name="member"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        private void Add(int row, string member, object obj, Type type)
        {
            PropertyInfo prop = type.GetProperty(member, Bound);
            ParameterInfo[] parameters = prop.GetIndexParameters();
            if (prop != null && parameters.Length == 0)
            {
                object val = prop.GetValue(obj, null);
                if (val == null) this[row][member] = DBNull.Value;
                else this[row][member] = val;
            }
            else this[row][member] = DBNull.Value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds the contents of the file to the table
        /// </summary>
        /// <param name="fi"></param>
        /// <remarks>
        ///      Usage Example:
        ///
        ///      table.Add("TRANSACTIONLOG_QTY"            , typeof(int)        );  // If a column is not a string you have to define it here - use the same name as the xml node child
        ///      table.Add("TRANSACTIONLOG_TRANENDDATETIME", typeof(SqlDateTime));
        ///
        ///      FileInfo[] fileList = (new DirectoryInfo(folderPath)).GetFiles();
        ///      foreach (FileInfo fi in fileList)
        ///      {
        ///          table.Add(fi);
        ///      }
        /// </remarks>
        public void Add(FileInfo fi)
        {
            string str = FilesIO.GetStringFromFile(fi.FullName, "");
            if      (Regex.IsMatch(fi.FullName, "csv$" , RegexOptions.IgnoreCase)) { Add(str); } // assume CSV
          //else if (Regex.IsMatch(fi.FullName, "json$", RegexOptions.IgnoreCase)) { Add(str, StringFormat.Json); }
            else { RichXmlDocument xdoc = new RichXmlDocument(str); Add(xdoc, "/NewDataSet/transaction"); xdoc.Dispose(); } // assume XML
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds rows for each parallel xml stem, adds columns as needed for leaves, default new columns to string
        ///      you must predefine non-string columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="xdoc"></param>
        /// <param name="stemNodeXPath">a stem is the direct parent of the child leaves which are the columns to be added to the table</param>
        /// <remarks>test me</remarks>
        public int Add(RichXmlDocument xdoc, string stemNodeXPath)
        {
            int n = 0;
            RichXmlNode stemNode = new RichXmlNode(xdoc.SelectSingleNode(stemNodeXPath + "[1]"));
            for (n = 1; stemNode.HasChildNodes && n < 1000; ++n)
            {
                if (stemNode.HasChildNodes)
                {
                    XmlNodeList nodeList = stemNode.XmlNodeChildNodes;
                    int numAdded = Add(nodeList, typeof(string)); // without an xml schema, everything's a string
                    Add(stemNode);
                }
                stemNode = new RichXmlNode(xdoc.SelectSingleNode(stemNodeXPath + "["+(n+1)+"]"));
            }
            return n;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a table row with the inner strings of each of the child nodes of a 'stem' node,
        ///      assumes that column names are identical to node children names
        /// </summary>
        /// <param name="table">type of each child node is determined by table column type</param>
        /// <param name="stemNode">
        ///      a node with leaf nodes with the same names as the columns,
        ///      each containing data in .InnerText
        /// </param>
        /// <remarks>test me</remarks>
        public void Add(RichXmlNode stemNode)
        {
            // --------------------------------------------------------------------------
            //  Go through the columns/child nodes and grab the data from the xml node children
            // --------------------------------------------------------------------------
            int row = Add();
            for (int c = 0; c < Columns.Count; ++c)
            {
                string column = Columns[c].ColumnName;
                Type typ = Columns[c].DataType;
                switch (typ.Name)
                {
                    case "string"      : this[row][column] = stemNode.StrValue             (column, ""  ); break;
                    case "String"      : this[row][column] = stemNode.StrValue             (column, ""  ); break;
                    case "Int32"       : this[row][column] = stemNode.IntValue             (column, 0   ); break;
                    case "DateTime"    : this[row][column] = stemNode.NullableDateTimeValue(column, null); break;
                    case "SqlDateTime" : this[row][column] = stemNode.NullableDateTimeValue(column, null); break;
                    case "int"         : this[row][column] = stemNode.IntValue             (column, 0   ); break;
                    default            : this[row][column] = stemNode.StrValue             (column, ""  );
                        Throws.A(new NotImplementedException("add this data type"), Throws.Actions, "P");
                        break;
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a string column reflecting the values in a bool column
        /// </summary>
        /// <param name="boolColumnFrom"></param>
        /// <param name="strColumnTo"></param>
        /// <param name="defaultValue"></param>
        /// <param name="trueText"></param>
        /// <param name="falseText"></param>
        public void Add(string boolColumnFrom, string strColumnTo, bool defaultValue, string trueText, string falseText)
        {
            if (Exists(boolColumnFrom))
            {
                if (Contains(strColumnTo) && Columns[strColumnTo].DataType == typeof(bool))
                    return;
                Add(strColumnTo, typeof(string));
                for (int row = 0; row < Count; ++row)
                    Rows[row][strColumnTo] = StrValue(row, boolColumnFrom, defaultValue, trueText, falseText);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a column to the table if it does not already exist
        /// </summary>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <returns>true if the column was created, false if it was already there</returns>
        /// <remarks>test me</remarks>
        public bool Add(string column, Type type)
        {
            if (Contains(column))
            {
                Type currentType = _table.Columns[column].DataType;
                if (type != currentType)
                {
                    Throws.A(new NotSupportedException("Error -"
                        + " attempt to add a column that already exists"
                        + " as a different type"
                        + " to the " + TableName + " table"), Throws.Actions, "P");
                }
                return false;
            }
            else
            {
                Columns.Add(column, type);
                return true;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a column to the table if it is missing in the node list (for generating tables from XML documents)
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>test me</remarks>
        public int Add(XmlNodeList nodeList, Type type)
        {
            int count = nodeList.Count;
            int numAdded = 0;
            for (int i = 0; i < count; ++i)
            {
                if (!Contains(nodeList[i].Name))
                {
                    Add(nodeList[i].Name, type);
                    numAdded++;
                }
            }
            return numAdded;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add --> 
        /// <summary>
        ///      Adds a column that is a string representation of a set of bit columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="boolColumnFrom"></param>
        /// <param name="value"></param>
        /// <param name="stringColumnTo"></param>
        /// <returns></returns>
        public RichDataTable Add(List<string> boolColumnFrom, List<string> value, string stringColumnTo)
        {
            Add(stringColumnTo, typeof(string));
            for (int row = 0; row < Count; ++row)
            {
                List<string> list = new List<string>(4);
                for (int i = 0; i < boolColumnFrom.Count; ++i)
                    list.Add(TreatAs.StrValue(BoolValue(row, boolColumnFrom[i], false), false, value[i], ""));
                Rows[row][stringColumnTo] = __.Glue(list, ", ");
            }
            return this;
        }
        public RichDataTable Add(string column1, string column2, string column3, string  value1, string  value2, string  value3, string columnTo)
        {
            List<string> columnFrom = new List<string>(3);  columnFrom.Add(column1);  columnFrom.Add(column2);  columnFrom.Add(column3);
            List<string> value      = new List<string>(3);       value.Add( value1);       value.Add( value2);       value.Add( value3);
            return Add(columnFrom, value, columnTo);
        }
        public RichDataTable Add(string column1, string column2, string column3, string column4, string value1 , string value2 , string value3 , string value4, string columnTo)
        {
            List<string> columnFrom = new List<string>(3);  columnFrom.Add(column1);  columnFrom.Add(column2);  columnFrom.Add(column3);  columnFrom.Add(column4);
            List<string> value      = new List<string>(3);       value.Add( value1);       value.Add( value2);       value.Add( value3);       value.Add( value4);
            return Add(columnFrom, value, columnTo);
        }
        public RichDataTable Add(string column1, string column2, string column3, string column4, string column5, string  value1, string  value2, string  value3, string  value4, string  value5, string columnTo)
        {
            List<string> columnFrom = new List<string>(3);  columnFrom.Add(column1);  columnFrom.Add(column2);  columnFrom.Add(column3);  columnFrom.Add(column4);  columnFrom.Add(column5);
            List<string> value      = new List<string>(3);       value.Add( value1);       value.Add( value2);       value.Add( value3);       value.Add( value4);       value.Add( value5);
            return Add(columnFrom, value, columnTo);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AllAvgs -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public RichDataTable AllAvgs()
        {
            RichDataTable avg = Clone();
            avg.Rows.Add(avg.NewRow());
            double count = Count;
            foreach (DataColumn column in avg.Columns)
            {
                Type type = column.DataType;
                switch (type.Name)
                {
                    case "Int16"   : avg.Rows[0][column] = (Int16)(Sum(column.ColumnName, 0) / count);               break;
                    case "String"  : avg.Rows[0][column] = "1";                                                            break;
                    case "DateTime": avg.Rows[0][column] = _Average(column.ColumnName, TimeDate_old.MinSqlValue.CLRFormat);   break;
                    case "Int32"   : avg.Rows[0][column] = (int)((0.5 + 1.0 * Sum(column.ColumnName, 0.0)) / count); break;
                    case "Double"  : avg.Rows[0][column] = Sum(column.ColumnName, 0.0) / count;                      break;
                    case "Single"  : avg.Rows[0][column] = Sum(column.ColumnName, 0.0F) / (float)count;              break;
                    default: break;
                }
            }

            return avg;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AllSums -->
        /// <summary>
        ///      Returns the sums of all the columns
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public RichDataTable AllSums()
        {
            RichDataTable sum = Clone();
            sum.Rows.Add(sum.NewRow());
            foreach (DataColumn column in sum.Columns)
            {
                Type type = column.DataType;
                switch (type.Name)
                {
                    case "Int16"   : sum.Rows[0][column] = (Int16)Sum(column.ColumnName, 0);   break;
                    case "String"  : sum.Rows[0][column] = "" + Count;                          break;
                    case "DateTime": sum.Rows[0][column] = MaxValue(column.ColumnName, TimeDate_old.MinSqlValue.CLRFormat); break;
                    case "Int32"   : sum.Rows[0][column] = Sum(column.ColumnName, 0);          break;
                    case "Double"  : sum.Rows[0][column] = Sum(column.ColumnName, 0.0);        break;
                    case "Single"  : sum.Rows[0][column] = Sum(column.ColumnName, 0.0F);       break;
                    default: break;
                }
            }

            return sum;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiBody -->
        /// <summary>
        ///      Construct ASCII art for body
        /// </summary>
        /// <param name="width"></param>
        /// <param name="colDelim"></param>
        /// <returns></returns>
        private string AsciiBody(List<int> width, string colDelim, char line, int maxRowCount)
        {
            StringBuilder body = new StringBuilder();
            string rowDelim = "";
            for (int row = 0; row < Count && row < maxRowCount; ++row)
            {
                try
                {
                    body.Append(rowDelim + AsciiRow(row, colDelim, width));
                    rowDelim = "\r\n";
                }
                catch (Exception ex)
                {
                    Throws.A(new InformationException(ex), Throws.Actions, "P");
                }


                if ((row+1) % 10 == 0)
                {
                    body.Append(rowDelim + AsciiLine(width,line));
                }
            }

            return body.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiBuildCode -->
        /// <summary>
        ///      Returns the chunk of coded needed to build this RichDataTable using literals
        /// </summary>
        public string AsciiBuildCode { get
        {
            StringBuilder code = new StringBuilder(Math.Max(10000, Count * 100));

            code.Append("RichDataTable table = new RichDataTable();");

            string delim = "\r\n";
            code.Append(delim);

            for (int col = 0; col < Columns.Count; ++col)
            {
                code.Append(delim + "table.Add(\"" + Columns[col] + "\", typeof("+Columns[col].DataType+"));");
            }

            code.Append(delim + "int gRow = 0;");
            for (int row = 0; row < Count; ++row)
            {
                code.Append(delim);
                code.Append(delim + "gRow = table.Add();");
                for (int col = 0; col < Columns.Count; ++col)
                {
                    string value = this[row][col].ToString();
                    string literal = "";
                    string dataType = Columns[col].DataType.ToString();
                    switch (dataType)
                    {
                        case "System.String"   : literal =                "\"" + value + "\"" ; break;
                        case "System.DateTime" : literal = "DateTime.Parse(\"" + value + "\")"; break;
                        default                : literal =                       value        ; break;
                    }
                    code.Append(delim + "table[gRow]["+col+"] = " + literal + ";");
                }
            }

            return code.ToString();
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiEstimateColumnWidths -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxColumnWidth"></param>
        /// <returns></returns>
        /// <remarks>debug code</remarks>
        private List<int> AsciiEstimateColumnWidths(int maxColumnWidth, int maxRowCount)
        {
          //int defaultWidth = Math.Max(Math.Min(240 / Columns.Count, maxColumnWidth), 22);
            List<int> width = new List<int>();

            for (int col = 0; col < Columns.Count; ++col)
            {
                width.Add(maxColumnWidth);
                string column = Columns[col].ColumnName;
                int    rowLen = MaxLength(column, maxRowCount);
                int    colLen = (Columns[col].ColumnName.Length +2)/3;
                int    len    = Math.Max(rowLen,colLen);
                if (len < maxColumnWidth)
                    width[col] = Math.Max(2, len);
            }

            return width;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiGrid -->
        /// <summary>
        ///      Returns a table grid as an ASCII string, used by AsciiTable for debugging
        /// </summary>
        /// <param name="maxRowCount"></param>
        /// <param name="maxColumnWidth"></param>
        /// <param name="rowDelim"></param>
        /// <param name="colDelim"></param>
        /// <returns></returns>
        /// <remarks>debug code</remarks>
        private string AsciiGrid(int maxRowCount, int maxColumnWidth, char rowDelim, string colDelim)
        {
            List<int> width = AsciiEstimateColumnWidths(maxColumnWidth, maxRowCount);
            string    str   = TableName + " (" + Count + "):"
                + "\r\n" + AsciiHeader(width, colDelim, rowDelim)
                + "\r\n" + AsciiBody  (width, colDelim, rowDelim, maxRowCount);
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiHeader -->
        /// <summary>
        ///      Construct ASCII art for header
        /// </summary>
        /// <param name="width"></param>
        /// <param name="colDelim"></param>
        /// <returns></returns>
        /// <remarks>debug code</remarks>
        private string AsciiHeader(List<int> width, string colDelim, char line)
        {
            string header = "";
            try
            {
                // ----------------------------------------------------------------------
                //  Add column name rows
                // ----------------------------------------------------------------------
                string delim = "";
                bool done = false;
                for (int i = 0; i < 6 && !done; ++i)
                {
                    // ------------------------------------------------------------------
                    //  Add another row for column names if needed
                    // ------------------------------------------------------------------
                    string nextRow = "";
                    delim = "";
                    for (int col = 0; col < Columns.Count; ++col)
                    {
                        string remainder = "";
                        string column = Columns[col].ColumnName;
                        if (column.Length > width[col]*i)
                        {
                            remainder = column.Substring(width[col]*i);
                        }
                        nextRow += delim + __.SetLength(width[col], remainder);
                        delim = colDelim;
                    }
                    if (Regex.IsMatch(nextRow, "^[ "+colDelim+"]+$"))
                    {
                        done = true;
                    }
                    else
                    {
                        header += "\r\n" + nextRow;
                    }
                }


                // ----------------------------------------------------------------------
                //  Add line to divide header from rows
                // ----------------------------------------------------------------------
                header += "\r\n" + AsciiLine(width, line);
            }
            catch (Exception ex)
            {
                Throws.A(new InformationException(ex), Throws.Actions, "P");
            }
            return header;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- AttemptToDelete -->
		/// <summary>
		///      Checks to make sure the number of rows to be deleted is in the specified range
		///      before deleting
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="fromAndWhereClause">FROM (table) WHERE (condition)</param>
		/// <param name="maxToDelete">
		///      do not delete if the the where clause covers more than this number
		/// </param>
		/// <returns>
		///      The number of items 'covered' by the where clause and therefore assumed deleted,
		///      returns zero if more than maxToDelete
		/// </returns>
		/// <remarks>, old</remarks>
		public static int AttemptToDelete(string conn, string fromAndWhereClause, int maxToDelete)
		{
			// -------------------------------------------------------------------------------
			//  Count how many will be deleted
			// -------------------------------------------------------------------------------
			RichSqlCommand ctcmd = new RichSqlCommand(CommandType.Text, conn, "SELECT COUNT(*) " + fromAndWhereClause, Throws.Actions, "P");
			int count = ctcmd.ExecuteScalar(-2);


			// ---------------------------------------------------------------------------
			//  Delete stuff if count is in range
			// ---------------------------------------------------------------------------
			if (0 < count && count <= maxToDelete)
            {
				RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn, "DELETE " + fromAndWhereClause, Throws.Actions, "P");
				cmd.ExecuteNonQuery();
				return count;
			}
            else
            {
				return 0;
			}
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- _Average -->
        /// <summary>
        ///      This is wrong but I'll use it for now as a stub
        /// </summary>
        /// <param name="column"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public DateTime _Average(string column, DateTime defaultValue)
        {
            //  This is wrong but I'll use it for now as a stub
            DateTime min = MinValue(column, defaultValue);
            DateTime max = MaxValue(column, defaultValue);
            TimeSpan t = max - min;
            long half = t.Ticks / 2;
            DateTime mid = min.AddTicks(half);
            return mid;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CalculateSimilarity -->
        /// <summary>
        ///      Calculates the similarity of one column of two tables
        /// </summary>
        /// <param name="leftLoc"></param>
        /// <param name="rightLoc"></param>
        /// <param name="columnToCompare"></param>
        /// <returns>a number from 0.0 to 1.0 indicating matches, or -1.0 if one or both tables are empty</returns>
        /// <remarks>alpha code</remarks>
        public static double CalculateSimilarity(RichDataTable table1, RichDataTable table2, string columnToCompare)
        {
            if (table1 == null || table2 == null || table1.Count == 0 || table2.Count == 0)
            {
                return -1.0;
            }
            else
            {
                Dictionary<string, int> collate = new Dictionary<string, int>(table1.Count + table2.Count);


                // ----------------------------------------------------------------------
                //  Add table 1 values to the collation as 1's
                // ----------------------------------------------------------------------
                for (int row = 0; row < table1.Count; ++row)
                {
                    string key = table1.StrValue(row, columnToCompare, "");
                    if (__.StringHasContent(key))
                        if (!collate.ContainsKey(key))
                            collate.Add(key, 1);
                }


                // ----------------------------------------------------------------------
                //  Add table 2 values to the collation as -1's (or 2's for a match)
                // ----------------------------------------------------------------------
                for (int row = 0; row < table2.Count; ++row)
                {
                    string key = table2.StrValue(row, columnToCompare, "");
                    if (!string.IsNullOrEmpty(key))
                    {
                        if (collate.ContainsKey(key))
                        {
                            if (collate[key] > 0)
                                collate[key] = 2;
                        }
                        else
                            collate.Add(key, -1);
                    }
                }


                // ----------------------------------------------------------------------
                //  Reduce unsimilarity by 50% for each match found
                // ----------------------------------------------------------------------
                double unsimilarity = 1.0;
                bool   onesFound    = false;
                foreach (int value in collate.Values)
                {
                    if (value == 2) unsimilarity *= 0.5; else onesFound = true;
                }


                // ----------------------------------------------------------------------
                //  Convert unsimilarity to similarity
                // ----------------------------------------------------------------------
                if (onesFound)
                    return (1.0 - unsimilarity);
                else return 1.0;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _CheckKeyUniqueness -->
        /// <summary>
        ///      Returns a message saying which single column keys are duplicated
        /// </summary>
        /// <param name="table"></param>
        /// <param name="keyColumn"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string _CheckKeyUniqueness(string keyColumn)
        {
            List<string> array = new List<string>();
            int count = Count;
            string table = TableName;
            string delim = "";
            string str = "";


            for (int row = 0; row < count; ++row)
            {
                // ----------------------------------------------------------------------
                //  Look to see if row key as already in array
                // ----------------------------------------------------------------------
                string key = StrValue(row, keyColumn, "");
                if (array.Contains(key))
                {
                    str += delim + table + ": " + key + " not unique";
                    delim = "\r\n";
                }

                array.Add(key); //  Put row key in array
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Collate -->
        /// <summary>
        ///      Collates a column's values into a big long string
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string Collate(string column, string join)
        {
            StringBuilder str = new StringBuilder();
            string delim = "";
            int count = Rows.Count;
            for (int row = 0; row < count; ++row)
            {
                str.Append(delim + StrValue(row, column, ""));
                delim = join;
            }
            return str.ToString();
        }
        public string Collate(string column1, string column2, string colJoin, string rowJoin)
        {
            StringBuilder str = new StringBuilder();
            string colDelim = colJoin;
            string rowDelim = "";
            int count = Rows.Count;
            for (int row = 0; row < count; ++row)
            {
                str.Append(rowDelim + StrValue(row, column1, "")
                    + colDelim + StrValue(row, column2, ""));
                rowDelim = rowJoin;
            }
            return str.ToString();
        }
        public string Collate(string column1, string column2, string column3, string colJoin, string rowJoin)
        {
            StringBuilder str = new StringBuilder();
            string   colDelim = colJoin;
            string   rowDelim = "";
            int      count    = Rows.Count;

            for (int row = 0; row < count; ++row)
            {
                str.Append(rowDelim + StrValue(row, column1, "")
                    + colDelim + StrValue(row, column2, "")
                    + colDelim + StrValue(row, column3, "")
                    );
                rowDelim = rowJoin;
            }

            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Copy -->
        /// <summary>
        ///      Returns a copy of the DataColumn sent to it
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private static DataColumn Copy(DataColumn column)
        {
            DataColumn copy = new DataColumn(column.ColumnName, column.DataType);

            copy.ColumnMapping = column.ColumnMapping;
            copy.DateTimeMode  = column.DateTimeMode ;
            copy.DefaultValue  = column.DefaultValue ;
            copy.Expression    = column.Expression   ;
            copy.MaxLength     = column.MaxLength    ;
            copy.Unique        = column.Unique       ;
            copy.AllowDBNull   = column.AllowDBNull  ;
            copy.Caption       = column.Caption      ;

            return copy;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CopyColumnsExcept -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptOneColumn"></param>
        /// <returns></returns>
        private DataColumn[] CopyColumnsExcept(string exceptOneColumn)
        {
            // ----------------------------------------------------------------------
            //  Set up the initial array structures
            // ----------------------------------------------------------------------
            DataColumn[] oldColumns = new DataColumn[Columns.Count];
            DataColumn[] newColumns = new DataColumn[Columns.Count - 1];
            Columns.CopyTo(oldColumns, 0);


            // ----------------------------------------------------------------------
            //  Add all the columns except the specified one
            // ----------------------------------------------------------------------
            int x = 0;
            for (int col = 0; col < Columns.Count; ++col, ++x)
            {
                if (oldColumns[col].ColumnName == exceptOneColumn) --x;
                else newColumns[x] = oldColumns[col];
            }

            return newColumns;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Create -->
        /// <summary>
        ///      Creates a RichDataTable from a "SELECT" stored procedure or text command
        /// </summary>
        /// <param name="cmd">A command that returns a table as per a SELECT command</param>
        /// <param name="tableName">What you want to name the table that is returned</param>
        private void Create(RichSqlCommand cmd, SqlTransaction trx)
        {
            bool touchConnection = (trx == null && cmd.Connection.State != ConnectionState.Open);

            // --------------------------------------------------------------------------
            //  Preprocessing
            // --------------------------------------------------------------------------
            _errors = "";
            if (touchConnection) cmd.Open();
            if (__.StringHasContent(cmd.Errors)) { Errors = cmd.Errors; return; }
            cmd.StatementCompleted = false;


            // --------------------------------------------------------------------------
            //  Run the reader
            // --------------------------------------------------------------------------
            //MessageBox.Show("Before execute reader");
            SqlDataReader dr = null;
            try { dr = cmd.ExecuteReader(); } catch (Exception ex) { Is.Trash(ex); }
            //MessageBox.Show("After execute reader");
            Errors   += __.ConcatIf("\r\n", cmd.Errors);
            _results += __.ConcatIf("\r\n", cmd.Results);


            // --------------------------------------------------------------------------
            //  Decant the reader
            // --------------------------------------------------------------------------
            if (dr != null)
                try
                {
                    dr = _FillFrom(dr, false);
                    cmd.StatementCompleted = true;
                    _results += __.ConcatIf("\r\n", cmd.Results);
                }
                // ----------------------------------------------------------------------
                //  Error handling
                // ----------------------------------------------------------------------
                catch (Exception ex) {  Errors += "\r\n" + "RichDataTable constructor error - \r\n" + ex.Message; }
                finally
                {
                    Errors   = cmd.Errors;
                    _results += cmd.Results;
                    if (touchConnection) InData.Close(cmd.Connection);
                    dr.Dispose();
                    dr = null;
                }
            else
            {
                if (touchConnection) InData.Close(cmd.Connection);
                Errors += "\r\n" + "RichDataTable constructor error - \r\n" + cmd.Errors;
            }


            // --------------------------------------------------------------------------
            //  Post error processing
            // --------------------------------------------------------------------------
            if (!cmd.StatementCompleted)
                Errors += "\r\n" + "Error - statement did not complete:" + "\r\n" + cmd.CommandText;
            if (!string.IsNullOrEmpty(Errors))
                Throws.A(new DataException(Errors), Throws.Actions, "P");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateCsCode -->
        /// <summary>
        ///      Creates C# code to create a DataTable from this RichDataTable
        /// </summary>
        /// <returns></returns>
        public string CreateCsCode()
        {
            // --------------------------------------------------------------------------
            //  Append code to instantiate DataTable
            // --------------------------------------------------------------------------
            StringBuilder str = new StringBuilder();
            string delim = "            ";
            str.Append(delim + "DataTable testTable = new DataTable("+_table.TableName+");");
            delim = "\r\n" + "            ";


            // --------------------------------------------------------------------------
            //  Append code to add columns
            // --------------------------------------------------------------------------
            foreach (DataColumn column in _table.Columns)
            {
                str.Append(delim + "testTable.Columns.Add("
                    + "new DataColumn(" + "\""+column.ColumnName+"\""
                    + ", " +  "typeof("+column.DataType     +")" + ")"
                    + ");");
            }


            // --------------------------------------------------------------------------
            //  Append code to add all the rows including data
            // --------------------------------------------------------------------------
            str.Append(delim + "");
            str.Append(delim + "int tgtRow = 0;"          );
            for (int srcRow = 0; srcRow < _table.Rows.Count; srcRow++)
            {
                // ----------------------------------------------------------------------
                //  Append code to add a row
                // ----------------------------------------------------------------------
                str.Append(delim + "tgtRow = testTable.Rows.Count;");
                str.Append(delim + "testTable.Rows.Add(testTable.NewRow());");
                str.Append(delim + "{");
                foreach (DataColumn column in _table.Columns)
                {
                    str.Append(delim + "    testTable.Rows[tgtRow][\""+column.ColumnName+"\"]");


                    // ------------------------------------------------------------------
                    //  Append code to add a row value
                    // ------------------------------------------------------------------
                    string columnType = column.DataType.Name;
                    object obj = _table.Rows[srcRow][column.ColumnName];
                    Type valueType = obj.GetType();
                    if (valueType == typeof(System.DBNull))
                    {
                        str.Append(" = System.DBNull.Value;"); 
                    }
                    else
                    {
                        switch (columnType)
                        {
                            case "Boolean"  : if ((bool)obj) str.Append(" = true;"); else str.Append(" = false;");  break;
                            case "DateTime" : str.Append(" = \"" + _table.Rows[srcRow][column.ColumnName] + "\";"); break;
                            case "Decimal"  : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +  "M;"); break;
                            case "Double"   : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +   ";"); break;
                            case "Int32"    : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +   ";"); break;
                            case "Long"     : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +  "L;"); break;
                            case "String"   : str.Append(" = \"" + _table.Rows[srcRow][column.ColumnName] + "\";"); break;
                            default         : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +   ";"); break;
                        }
                    }
                }
                str.Append(delim + "}");
            }

            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateVbCode -->
        /// <summary>
        ///      Creates VB.NET code to create a DataTable from this RichDataTable
        /// </summary>
        /// <returns></returns>
        public string CreateVbCode()
        {
	        // --------------------------------------------------------------------------
	        //  Append code to instantiate DataTable
	        // --------------------------------------------------------------------------
	        StringBuilder str = new StringBuilder();
	        string delim = "            ";
	        str.Append(delim + "Dim testTable As DataTable");
	        delim = "\r\n" + "            ";
	        str.Append(delim + "testTable = new DataTable(\"" + _table.TableName + "\")");


	        // --------------------------------------------------------------------------
	        //  Append code to add columns
	        // --------------------------------------------------------------------------
	        foreach (DataColumn column in _table.Columns)
            {
		        str.Append(delim + "testTable.Columns.Add("
                    + "new DataColumn(" + "\""+column.ColumnName+"\""
                    + ", " + "GetType("+column.DataType.Name+")" + ")"
                    + ")" );
	        }


	        // --------------------------------------------------------------------------
	        //  Append code to add all the rows including data
	        // --------------------------------------------------------------------------
	        str.Append(delim + "");
	        str.Append(delim + "Dim tgtRow As Integer = 0");
	        for (int srcRow = 0; srcRow < _table.Rows.Count; srcRow++)
            {
		        // ----------------------------------------------------------------------
		        //  Append code to add a row
		        // ----------------------------------------------------------------------
		        str.Append(delim + "tgtRow = testTable.Rows.Count" );
		        str.Append(delim + "testTable.Rows.Add(testTable.NewRow())" );

		        foreach (DataColumn column in _table.Columns)
                {
			        str.Append(delim + "    testTable.Rows(tgtRow)(\""+column.ColumnName+"\")");


			        // ------------------------------------------------------------------
			        //  Append code to add a row value
			        // ------------------------------------------------------------------
			        string columnType = column.DataType.Name;
			        object obj = _table.Rows[srcRow][column.ColumnName];
                    Type valueType = obj.GetType();
                    if (valueType == typeof(System.DBNull))
                    {
				        str.Append(" = System.DBNull.Value" );
			        }
                    else
                    {
				        switch (columnType)
                        {
					        case "Boolean"  : if ((bool)obj) str.Append(" = True" ); else str.Append(" = False" );  break;
					        case "DateTime" : str.Append(" = \"" + _table.Rows[srcRow][column.ColumnName] + "\"" ); break;
					        case "Decimal"  : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +  "D" ); break;
					        case "Double"   : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +   "" ); break;
					        case "Int32"    : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +   "" ); break;
					        case "Long"     : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +  "L" ); break;
					        case "String"   : str.Append(" = \"" + _table.Rows[srcRow][column.ColumnName] + "\"" ); break;
					        default         : str.Append(" = "   + _table.Rows[srcRow][column.ColumnName] +   "" ); break;
				        }
			        }
		        }

	        }

	        return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DateValue -->
        /// <summary>
        ///      Returns a nullable DateTime
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public DateTime? DateValue(int row, string column)
        {
            DateTime  date;
            DateTime? output;
            object    input = ObjValue(row, column, null);


            if (Is.Null(input))                                              { output = null;                       }
            else { if (input.GetType() == typeof(SqlDateTime))               { output = ((SqlDateTime)input).Value; }
                   else { if (DateTime.TryParse(input.ToString(), out date)) { output = date;                       }
                          else                                               { output = null;                       }
                        }                                                                           
                 }          
            return output;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- DeleteOneRow -->
        /// <summary>
        ///      Deletes one row from a table given a from-where clause, Warning, you are responsible for handling SQL injection
        /// </summary>
        /// <param name="fromWhereClause"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns>the number of rows deleted, or -1 if something goes wrong</returns>
        /// <remarks>Warning, you are responsible for handling SQL injection</remarks>
        public static int DeleteOneRow(string fromWhereClause, SqlConnection connection, SqlTransaction trx)
        {
            try
            {
                // -----------------------------------------------------------------------
                //  Check the where clause then perform the deletion if everything is OK
                // -----------------------------------------------------------------------
                RichSqlCommand read = new RichSqlCommand(CommandType.Text, connection, trx, "SELECT * " + fromWhereClause, Throws.Actions, "PR");
                RichDataTable  test = new RichDataTable (read, null, "FromWhereTest", "");
                if (test.Count == 1)
                {
                    RichSqlCommand delete = new RichSqlCommand(CommandType.Text, connection, trx, "DELETE " + fromWhereClause, Throws.Actions, "PR");
                    delete.ExecuteNonQuery();
                    return  1;
                }
                else               { return  0; }
            } catch (Exception ex) { Is.Trash(ex); return -1; }
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- DeleteSomeRows -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromWhereClause"></param>
        /// <param name="maxDeleteCount"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static int DeleteSomeRows(string fromWhereClause, int maxDeleteCount, SqlConnection connection, SqlTransaction trx)
        {
            try
            {
                // -----------------------------------------------------------------------
                //  Check the where clause then perform the deletion if everything is OK
                // -----------------------------------------------------------------------
                RichSqlCommand read = new RichSqlCommand(CommandType.Text, connection, trx, "SELECT * " + fromWhereClause, Throws.Actions, "PR");
                RichDataTable  test = new RichDataTable (read, null, "FromWhereTest", "");
                if (0 < test.Count && test.Count <= maxDeleteCount)
                {
                    RichSqlCommand delete = new RichSqlCommand(CommandType.Text, connection, trx, "DELETE " + fromWhereClause, Throws.Actions, "PR");
                    int result = delete.ExecuteNonQuery();
                    return test.Count;
                }
                else               { return  0; }
            } catch (Exception ex) { Is.Trash(ex); return -1; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Distinct -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<object> Distinct(string column)
        {
            Dictionary<string, int> hash = new Dictionary<string,int>();
            List<object> list = new List<object>();
            bool nullInList = false;
            for (int row = 0; row < Count; ++row)
            {
                object obj = ObjValue(row, column, null);
                if (obj == null && !nullInList)
                {
                    list.Add(null);
                    nullInList = true;
                }
                else
                {
                    string key = obj.ToString();
                    if (!hash.ContainsKey(key))
                    {
                        hash.Add(key, 1);
                        list.Add(obj);
                    }
                }
            }

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Distinct -->
        /// <summary>
        ///      Returns a distinct list of strings in the order in which they first appear in a column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public List<string> Distinct(string column, string defaultValue)
        {
            Dictionary<string,int> hash = new Dictionary<string, int>();
            List<string> list = new List<string>();


            for (int row = 0; row < Count; ++row)
            {
                string str = StrValue(row, column, defaultValue);
                if (hash.ContainsKey(str)) { hash[str]++;     }
                else        { list.Add(str); hash.Add(str,1); }
            }

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExcelToTable -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelPath">path to the excel spreadsheet</param>
        /// <param name="worksheet">the worksheet to extract</param>
        /// <returns></returns>
        /// <remarks>beta code - this is generally production ready but has experienced glitches in the past based on odd 'ACE' versioning</remarks>
        public static RichDataTable ExcelToTable(string excelPath, string worksheet)
        {
            RichDataTable data = null;

            try
            {
                // --------------------------------------------------------------------------
                //  Prepare command
                // --------------------------------------------------------------------------
                string sourceConstr = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                    + "Data Source='" + excelPath + "';"
                    + "Extended Properties='Excel 12.0;HDR=No;IMEX=1'";
                OleDbConnection con = new OleDbConnection(sourceConstr);
                OleDbDataAdapter myCommand = new OleDbDataAdapter(" SELECT * FROM [" + worksheet + "$]", con);


                // --------------------------------------------------------------------------
                //  Get the data
                // --------------------------------------------------------------------------
                DataSet ds = new DataSet();
                myCommand.TableMappings.Add(worksheet, "dtExcel");
                myCommand.Fill(ds);
                data = new RichDataTable(ds.Tables[0], "");
            }
            catch (Exception ex)
            {
                data = new RichDataTable();
                data.Errors = ex.Message;
            }

            return data;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FillWithEndemeSet -->
        /// <summary>
        ///      Builds a RichDataTable out of an EndemeSet
        /// </summary>
        /// <param name="enSet"></param>
        /// <param name="enSetFK"></param>
        /// <param name="IncludeEmptyCharacteristics"></param>
        private void FillWithEndemeSet(EndemeSet enSet, string enSetFK, bool IncludeEmptyCharacteristics)
        {
            // --------------------------------------------------------------------------
            //  Initialize output table
            // --------------------------------------------------------------------------
            Add("EndemeSetLabel"      , typeof(string));
            Add(enSetFK               , typeof(Guid));
            Add("CharacteristicLetter", typeof(char));
            Add("CharacteristicCode"  , typeof(string));
            Add("CharacteristicLabel" , typeof(string));
            Add("CharacteristicDescr" , typeof(string));
            
            
            for (int c = 0; c <= enSet.Count - 1; c++)
            {
                // -----------------------------------------------------------------------
                //  Add row to table
                // -----------------------------------------------------------------------
                EndemeCharacteristic enChar = enSet.Characteristics()[c];
                
                if (IncludeEmptyCharacteristics | !string.IsNullOrEmpty(enChar.Label))
                {
                    int row = Add();
                    _table.Rows[row]["EndemeSetLabel"      ] = enSet.Label;
                    _table.Rows[row][enSetFK               ] = enSet.SetId;
                    _table.Rows[row]["CharacteristicLetter"] = enChar.Letter;
                    _table.Rows[row]["CharacteristicCode"  ] = enChar.Code;
                    _table.Rows[row]["CharacteristicLabel" ] = enChar.Label;
                    _table.Rows[row]["CharacteristicDescr" ] = enChar.Descr;
                }
            }

            _idColumn = "CharacteristicLetter";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetPascalSpacedLabel -->
		/// <summary>
		///      Get the Pascal spaced version of a label  
		/// </summary>
		/// <param name="label">Name to be changed</param>
		/// <returns>PascalSpaced version of the name</returns>
        /// <remarks>This is a utility method is modified from NetTiers</remarks>
		public string GetPascalSpacedLabel(string label, string defaultValue)
		{
			if (string.IsNullOrEmpty(label)) return defaultValue;
	        Regex regex = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z])");
		    return regex.Replace(label, " ${x}");
	    }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetRelevantRows -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableFK"></param>
        /// <param name="userTableKey"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public RichDataTable GetRelevantRows(string tableFK, string userTableName, string userTableKey, string conn)
        {
            string muanList1 = "'" + Regex.Replace(Collate(tableFK, "', '"), "[^0-9, ']", "") + "'";
            TrimStart(tableFK, '0', "");
            string muanList2 = "'" + Regex.Replace(Collate(tableFK, "', '"), "[^0-9, ']", "") + "'";


            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "SELECT * FROM "+userTableName+" WHERE "+userTableKey+" IN ("+muanList1+","+muanList2+")"
                , Throws.Actions, "P");
            RichDataTable user = new RichDataTable(cmd, null, "User", userTableKey);


            string report = InData.AsciiNewQuery(cmd.Command);
            Cmd = cmd;

            return user;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Index -->
        /// <summary>
        ///      Returns an index for a table column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="skipDuplicates">if there are any duplicates found in the column, skip them</param>
        /// <returns></returns>
        public Dictionary<int, int> Index(string column, bool skipDuplicates)
        {
            Dictionary<int,int> index = new Dictionary<int,int>();

            for (int row = 0; row < Count; ++row)
            {
                int rid = IntValue(row, column, -1);
                if (!index.ContainsKey(rid))
                    index.Add(rid, row);
                else
                {
                    if (!skipDuplicates)
                        Throws.A(new AmbiguousResultException("there are more than one "+column+" with the same value in the table."), Throws.Actions, "P");;
                }
            }

            return index;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InitializeCountColumn -->
        /// <summary>
        ///      Initializations - add quantity column, initialize to zero
        /// </summary>
        /// <param name="column">name of new column</param>
        /// <param name="pageIdx">if less than zero initialize the entire table, otherwise just the page specified</param>
        public RichDataTable InitializeCountColumn(string column, int pageIdx)
        {
            if (!Contains(column)) Add(column, typeof(int));

            if (pageIdx >= 0)
            {
                int pageRow = 10 * pageIdx;
                for (int row = pageRow; row <= (pageRow + 10) && row < Count; ++row)
                    this[row][column] = 0;
            }
            else for (int row = 0; row < Count; ++row) this[row][column] = 0;

            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _IntStrDictionary -->
        /// <summary>
        ///      Returns an &lt;int,string> dictionary treating the first column as a set of integers
        ///      and the second column as a set of strings, collates strings on duplicated keys
        /// </summary>
        /// <param name="intColumn"></param>
        /// <param name="strColumn"></param>
        /// <returns></returns>
        public Dictionary<int,string> _IntStrDictionary(string intColumn, string strColumn)
        {
            Dictionary<int,string> output = new Dictionary<int,string>();

            for (int row = 0; row < Count; ++row)
            {
                int key = IntValue(row, intColumn, 0);
                if (output.ContainsKey(key))
                {
                    output[key] += ", " + StrValue(row,strColumn,"");
                }
                else
                {
                    output.Add(key,StrValue(row,strColumn,""));
                }
            }

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LinqToDataTable -->
        /// <summary>
        ///      This is very fragile - this linq has to be exactly right and any changes to the
        ///      datatable will break it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varlist"></param>
        /// <returns></returns>
        /// <remarks>alpha code - very fragile</remarks>
        private DataTable LinqToDataTable<T>(IEnumerable<T> varlist)
        {
            // --------------------------------------------------------------------------
            //  Prep
            // --------------------------------------------------------------------------
            DataTable output = new DataTable();
            PropertyInfo[] property = null;  // column names 
            if (varlist == null) return output;


            foreach (T rec in varlist)
            {
                // ----------------------------------------------------------------------
                //  Use reflection to get property names, to create table, Only first time, others will follow 
                if (property == null)
                {
                    property = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in property)
                    {
                        Type colType = pi.PropertyType;
                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                            colType = colType.GetGenericArguments()[0];
                        output.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = output.NewRow();

                foreach (PropertyInfo pi in property)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                output.Rows.Add(dr);
            }

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ListMembersOf -->
        /// <summary>
        ///      Lists the properties of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<DataColumn> ListMembersOf(object obj)
        {
            List<string> list = new List<string>();
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties(Members);
            List<DataColumn> column = new List<DataColumn>();


            for (int i = 0; i < propertyInfos.Length; ++i)
            {
                PropertyInfo member = propertyInfos[i];
                if (member.MemberType == MemberTypes.Property)
                {
                    Type type = member.PropertyType;
                    string fullTypeName = type.FullName;
                    string name = member.Name;
                    if (Regex.IsMatch(fullTypeName, "^System")
                        && !Regex.IsMatch(fullTypeName, @"\[\]"))
                    {
                        list.Add(name);
                        Type unType = Nullable.GetUnderlyingType(type);
                        column.Add(new DataColumn(name, unType ?? type));
                    }
                }

            }
            
            return column;
        }

        BindingFlags Members { get { return BindingFlags.Public | BindingFlags.NonPublic // Get public and non-public
                | BindingFlags.Static | BindingFlags.Instance  // Get instance + static
                | BindingFlags.FlattenHierarchy; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- ListMembersOf -->
        /// <summary>
        ///      Lists the members of an object using reflection
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="deepBuild"></param>
        /// <returns></returns>
        /// <remarks>works pretty well but still needs a little work</remarks>
        public List<DataColumn> ListMembersOf(object obj, bool deepBuild)
        {
            if (!deepBuild)
            {
                return ListMembersOf(obj);
            }
            else
            {
                List<string> list = new List<string>();


                // --------------------------------------------------------------------------
                //  Get list of members to include in list
                // --------------------------------------------------------------------------
                PropertyInfo[] propertyInfos = obj.GetType().GetProperties
                    ( BindingFlags.Public | BindingFlags.NonPublic // Get public and non-public
                    | BindingFlags.Static | BindingFlags.Instance  // Get instance + static
                    | BindingFlags.FlattenHierarchy);


                List<DataColumn> column = new List<DataColumn>();


                for (int i = 0; i < propertyInfos.Length; ++i)
                {
                    PropertyInfo member = propertyInfos[i];
                    if (member.MemberType == MemberTypes.Property)
                    {
                        Type type = member.PropertyType;
                        string fullTypeName = type.FullName;
                        string name = member.Name;
                        //if (Regex.IsMatch(fullTypeName, "^System")
                        //    && !Regex.IsMatch(fullTypeName, @"\[\]"))
                        //{

                            list.Add(name);
                            Type unType = Nullable.GetUnderlyingType(type);
                            column.Add(new DataColumn(name, unType ?? type));
                        //}
                    }

                }

                return column;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LongestActiveColumnLabelInRow -->
        /// <summary>
        ///      Returns the length of the pretty printed label of the longest active column
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Goes through the columns, inserts spaces in the headers, finds the longest column
        ///      name in the row, null and blank values are not active
        /// 
        ///      production ready - mostly for debuggin
        /// </remarks>
        public int LongestActiveColumnLabelInRow(int row)
        {
            DataColumnCollection columnList = Columns;
            int len = 0;
            foreach(DataColumn dc in columnList)
            {
                string column = dc.ColumnName;
                string value = StrValue(row, column, "");
                if (!string.IsNullOrEmpty(value))
                {
                    column = GetPascalSpacedLabel(column, "");
                    len = Math.Max(len, column.Length);
                }
            }
            return len;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MergeTables -->
        /// <summary>
        ///      Merges two tables with identical schemas, copies the other table if one is null
        /// </summary>
        /// <param name="tableA"></param>
        /// <param name="tableB"></param>
        /// <returns></returns>
        /// <remarks>
        ///      The schema comparison could be improved significantly
        ///      
        ///      alpha code - I've used it in production but do not trust it and test to make sure it works
        /// </remarks>
        public static RichDataTable MergeTables(RichDataTable tableA, RichDataTable tableB)
        {
            RichDataTable output = null;

            if (tableA != null && tableB == null) output = tableA.Copy();
            if (tableA == null && tableB != null) output = tableB.Copy();
            if (tableA != null && tableB != null)
            {
                if (tableA.Columns.Count == tableB.Columns.Count)
                    output = tableA.Copy().Add(tableB);
                else
                    output = null;
            }

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _MinimizeXml -->
        /// <summary>
        ///      Removes all empty tags
        /// </summary>
        /// <remarks>
        ///      Move this to an XML namespace!
        /// </remarks>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string _MinimizeXml(int row, string column)
        {
            string small = Minimize(StrValue(row, column, ""));
            RecordLast(row, column);
            return small;
        }
        private string Minimize(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            Minimize(doc.FirstChild, 0);
            return doc.InnerXml;
        }
        private XmlNode Minimize(XmlNode node, int level)
        {
            for (int i =  node.ChildNodes.Count - 1; i>= 0; --i)
            {
                XmlNode child =  node.ChildNodes[i];
                if (child.InnerText == "")
                    node.RemoveChild(child);
                else if (child.InnerXml != child.InnerText)
                    Minimize(child, level + 1);
            }

            return node;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _NewColumnList -->
        /// <summary>
        ///      Creates a new column list from the object minus the specified column
        /// </summary>
        /// <param name="exceptOneColumn">columnToLeaveOut - columnToMakeIntoTables</param>
        /// <returns></returns>
        private DataColumn[] _NewColumnList(string exceptOneColumn)
        {
            int count = Columns.Count;
            DataColumn[] newColumns;


            if (Contains(exceptOneColumn))
            {
                newColumns = CopyColumnsExcept(exceptOneColumn);
            }
            else
            {
                // ----------------------------------------------------------------------
                //  If the column to leave out was not in the list, just copy the list
                // ----------------------------------------------------------------------
                newColumns = new DataColumn[count];
                Columns.CopyTo(newColumns, 0);
            }

            return newColumns;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NewRow -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataRow NewRow(Random r)
        {
            if (!Exists()) { return null; }

            DataRow dr = _table.NewRow();

            if (_table.PrimaryKey != null && _table.PrimaryKey.Length > 0)
            {
                DataColumn[] pk   = _table.PrimaryKey;
                DataColumn   pk0  = pk[0];
                string       name = pk0.DataType.Name;
                int          num  = r.Next(int.MaxValue);
                BigNumber    bnum = new BigNumber(num, 10, BigNumber.Base64StyleEncoding);
                string       str  = __.Truncate(bnum.ToString(), 7);


                switch (name)
                {
                    case "Integer" : dr[pk0] = num;            break;
                    case "String"  : dr[pk0] = str;            break;
                    case "Guid"    : dr[pk0] = Guid.NewGuid(); break;
                    default: throw new Exception("Data type unhandled in RichDataTable pk");
                }
            }

            return dr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PrettyFormatRow -->
        /// <summary>
        ///      Formats a single row as a string in an ASCII art table vertically
        ///      with column names to the left and values to the right
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string PrettyFormatRow(int row, int width, string columnDelim)
        {
            int len = LongestActiveColumnLabelInRow(row);


            // --------------------------------------------------------------------------
            //  Show the value and the header for those with values, print vertically
            // --------------------------------------------------------------------------
            DataColumnCollection columnList = Columns;
            StringBuilder str = new StringBuilder();
            string delim = "";
            foreach (DataColumn dc in columnList)
            {
                string column = dc.ColumnName;
                string value = Regex.Replace(StrValue(row, column, ""), @"\r\n?|\n", " ");
                if (value.Length > width-len-2)
                    value = value.Substring(0,width-len-6) + " ...";
                if (!string.IsNullOrEmpty(value))
                {
                    column = GetPascalSpacedLabel(column, "");
                    str.Append(delim + (column + ":").PadRight(len+1) + " " + value.PadRight(width-len-2));
                    delim = columnDelim;
                }
            }

            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PropagationPredecessor -->
        /// <summary>
        ///      Wander backward the the first non-blank value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private int PropagationPredecessor(int row, string column)
        {
            int predecessor = -1;
            for (int cursor = row-1; cursor >= 0 && predecessor < 0; --cursor)
                if (__.StringHasContent(StrValue(cursor, column, "")))
                    predecessor = cursor;
            return predecessor;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SequentialSubtable -->
        /// <summary>
        ///      Returns a subtable of this table building out from a specified row,
        ///      sort of like a 'SELECT WHERE column = value' but only as long as the check is valid
        /// </summary>
        /// <param name="keyColumn"></param>
        /// <param name="key"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks>major table manipulation</remarks>
        public RichDataTable SequentialSubtable(int middleRow, string column)
        {
            string target = StrValue(middleRow, column, "");


            // --------------------------------------------------------------------------
            //  How far before in the table is the column value equal
            // --------------------------------------------------------------------------
            int firstRow = middleRow;
            bool keepGoing = true;
            for (int cursor1 = middleRow-1; keepGoing && cursor1 >= 0; --cursor1)
                if (StrValue(cursor1, column, "") == target) firstRow = cursor1;
                else keepGoing = false;


            // --------------------------------------------------------------------------
            //  How far after in the table is the column value equal
            // --------------------------------------------------------------------------
            int lastRow = middleRow;
            keepGoing = true;
            for (int cursor2 = middleRow+1; cursor2 < Count; ++cursor2)
                if (StrValue(cursor2, column, "") == target) lastRow = cursor2;
                else keepGoing = false;


            // --------------------------------------------------------------------------
            //  Build the block table from the row range found
            // --------------------------------------------------------------------------
            RichDataTable block = Clone();
            for (int row = firstRow; row <= lastRow; ++row)
            {
                block.Add(this[row]);
            }
            return block;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PropagationSuccessor -->
        /// <summary>
        ///      Wander forward the the first non-blank value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private int PropagationSuccessor(int row, string column)
        {
            int successor = -1;

            for (int cursor = row+1; cursor < Count && successor < 0; ++cursor)
            {
                string value = StrValue(cursor, column, "");
                if (__.StringHasContent(value))
                {
                    successor = cursor;
                }
            }

            return successor;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RowCountIn -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int RowCountIn(string tableName, string whereClause, string conn)
        {
            RichSqlCommand cmd   = null;
            RichDataTable  table = null;
            int count = -1;
            tableName   = InData.ResistSqlInjection(tableName);
            whereClause = InData.ResistSqlInjection(whereClause);


            try
            {
                // ----------------------------------------------------------------------
                //  Get rows
                // ----------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, conn
                    , "SELECT * FROM " + tableName + " " + whereClause
                    , Throws.Actions, "P");
                table = new RichDataTable(cmd, null, "CountTable", "hiworld");


                count = table.Count;
            }
            catch { throw; }
            finally
            {
                if (cmd   != null) cmd  .Dispose();
                if (table != null) table.Dispose();
            }

            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SelectionPreferences -->
        /// <summary>
        ///      Propagation preferences also etc.
        ///      C)losest, N)earest, P)revious, S)uccessor
        /// </summary>
        /// <remarks>I should probably put this in a constants class in Aspects</remarks>
        public static EndemeSet SelectionPreferences { get
        {
            if (_selectionPreferences == null)
            {
                try { _selectionPreferences = new EndemeSet("Selection Preferences"); } catch { }
                try { _selectionPreferences.Add('C', "Closest"    , "Select the closest in value"   ); } catch { }
                try { _selectionPreferences.Add('N', "Nearest"    , "Select the closest in position"); } catch { }
                try { _selectionPreferences.Add('P', "Predecessor", "Prefer the previous one"       ); } catch { }
                try { _selectionPreferences.Add('S', "Successor"  , "Prefer the next one"           ); } catch { }
            }

            return _selectionPreferences;
        } }
        private static volatile EndemeSet _selectionPreferences;

        //// ----------------------------------------------------------------------------------------
        ///// <!-- PropagateByDate -->
        ///// <summary>
        /////      Major table manipulation
        ///// </summary>
        ///// <param name="column1"></param>
        ///// <param name="candy">candy does not do anything</param>
        ///// <param name="column2"></param>
        ///// <param name="preferPredecessor"></param>
        ///// <returns></returns>
        //private int PropagateByDate(string column1, Cells candy, string column2, Endeme prefer)
        //{
        //    // --------------------------------------------------------------------------
        //    //  Prepare table
        //    // --------------------------------------------------------------------------
        //    Dictionary<int,int> fromRow = new Dictionary<int,int>();
        //    Add("TempColumn", typeof(int));
        //    for (int row = 0; row < Count; ++row) { this[row]["TempColumn"] = row; }
        //    Sort("ORDER BY " + column2);  // order by date


        //    for (int row = 0; row < Count; ++row)
        //    {
        //        string value = StrValue(row, column1, "");
        //        if (string.IsNullOrEmpty(value))
        //        {
        //            DateTime mvalue = DateTimeValue(row, column2, DateTime.Now);

        //            // ------------------------------------------------------------------
        //            //  Go in both directions (until the closest is found)?
        //            // ------------------------------------------------------------------
        //            int successor   = PropagationSuccessor  (row, column1);
        //            int predecessor = PropagationPredecessor(row, column1);


        //            if (successor >= 0 || predecessor >= 0)
        //            {
        //                if      (predecessor == -1) fromRow.Add(row, successor  );
        //                else if (successor   == -1) fromRow.Add(row, predecessor);
        //                else
        //                {
        //                    DateTime bvalue = DateTimeValue(row, column2, DateTime.Now);
        //                    DateTime tvalue = DateTimeValue(row, column2, DateTime.Now);
        //                    TimeSpan diff1 = mvalue - bvalue;
        //                    TimeSpan diff2 = tvalue - mvalue;


        //                    if (prefer.Contains('P')) // endemize to add 'Closest'
        //                         { if (diff1 <= diff2) fromRow.Add(row, predecessor); else fromRow.Add(row, successor); }
        //                    else { if (diff1 <  diff2) fromRow.Add(row, predecessor); else fromRow.Add(row, successor); }
        //                }
        //            }
        //        }
        //    }


        //    foreach (int toRow in fromRow.Keys)
        //    {
        //        this[toRow][column1] = ObjValue(fromRow[toRow], column1, null);
        //    }

        //    //  Put back in original order
        //    Sort("ORDER BY TempColumn");

        //    return fromRow.Count;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- _Select -->
        /// <summary>
        ///      Returns a subtable from a table using a filter
        /// </summary>
        /// <param name="from"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public RichDataTable _Select(string filter)
        {
            RichDataTable subTable = Clone();
            try { DataRow[] rows = Select(filter); foreach (DataRow row in rows) subTable.Table.ImportRow(row); }
            catch (Exception ex) { subTable.Errors = Errors + "\r\n\r\n" + ex.Message; Throws.A(ex, Throws.Actions, "P"); }
            return subTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Set -->
        /// <summary>
        ///      Fills the columns from the added joined table,
        ///      Use with Add(RichDataTable table, string inputTablePK, string thisTableFK)
        /// </summary>
        /// <param name="newColumn"></param>
        /// <param name="thisTableFK"></param>
        /// <param name="table"></param>
        /// <param name="inputTablePK"></param>
        /// <remarks>
        ///      It is usually better to let SQL Server do the work, 
        ///      if you can, use SQL, T-SQL, a view, SP or Fn
        /// </remarks>
        private void Set(List<string> newColumn, string thisTableFK, RichDataTable table, string inputTablePK)
        {
            if (newColumn.Count > 0 && table.Count > 0)
            {
                for (int row = 0; row < Count; ++row)
                {
                    int value = IntValue(row, thisTableFK, 0);
                    RichDataTable dr = table._Select("WHERE " + inputTablePK + " = " + value);
                    if (dr.Count == 1)
                    {
                        for (int i = 0; i < newColumn.Count; ++i)
                        {
                            string columnName = newColumn[i];
                            object obj = dr.ObjValue(0, columnName, null);
                            this[row][columnName] = obj;
                        }
                    }
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Set -->
        /// <summary>
        ///      Sets the value of a cell
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void Set(int row, string column, object value)
        {
            if      (Columns[column].DataType == typeof(string     )) Rows[row][column] = TreatAs.StrValue             (value, ""  );
            else if (Columns[column].DataType == typeof(DateTime   )) Rows[row][column] = TreatAs.NullableDateTimeValue(value, null);
            else if (Columns[column].DataType == typeof(SqlDateTime)) Rows[row][column] = InData .GetSqlDateTime       (value      );
            else if (Columns[column].DataType == typeof(int        )) Rows[row][column] = TreatAs.IntValue             (value, 0   );
            else
            {
                Type type = Columns[column].DataType;
                this[row][column] = value;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetColumn -->
        /// <summary>
        ///      Sets the value of a row/column based on another column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="columnFrom"></param>
        /// <param name="pattern"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        public void SetColumn(int row, string columnTo, string columnFrom, string pattern, string trueValue, string falseValue)
        {
            string str = StrValue(row, columnFrom, "");
            if (Regex.IsMatch(str, pattern))
                this[row][columnTo] = trueValue;
            else
                this[row][columnTo] = falseValue;
            LastRow = row;
        }

        //public RichDataTable Sort(string orderByClause)
        //{
        //    DataView dv = View;  //  Load the DataTable into a view
        //    try
        //    {
        //        dv.Sort = BodyOf(orderByClause);
        //        dv.ApplyDefaultSort = true;
        //        RichDataTable rt = new RichDataTable(dv);
        //        Clear();
        //        Merge(rt);
        //    }
        //    catch (Exception ex) { Errors = ex.Message; }

        //    return this;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- _Split -->
        /// <summary>
        ///      Splits a table into multiple tables based on the values in a column,
        ///      This is part an effort to deal with the famous SQL/C# impedence mismatch
        /// </summary>
        /// <param name="column"></param>
        public List<RichDataTable> _Split(string column)
        {
            List<object> keys = Distinct(column);
            Dictionary<object,RichDataTable> tables = new Dictionary<object,RichDataTable>(keys.Count);
            foreach (object obj in keys)
                tables[obj] = Clone();


            for (int row = 0; row < Count; ++row)
            {
                object obj = ObjValue(row, column, null);
                tables[obj].Add(row, this);
            }


            List<RichDataTable> table = new List<RichDataTable>(keys.Count);
            for (int i = 0; i < keys.Count; ++i)
            {
                object obj = keys[i];
                table.Add(tables[obj]);
            }

            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SqlDateValue -->
        /// <summary>
        ///      Returns a SqlDateTime given a number of possibilities for column name
        /// </summary>
        /// <param name="row"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e">5th item</param>
        /// <param name="f"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SqlDateTime SqlDateValue(int row, string[] possibleColumn, SqlDateTime defaultValue)
        {
            SqlDateTime date = defaultValue;


            foreach (string column in possibleColumn)
            {
                date = SqlDateValue(row, column, defaultValue);
                if (date.ToString() != defaultValue.ToString())
                    break;
            }
            LastRow = row;
            return date;
        }
        public SqlDateTime SqlDateValue(int row, string a, string b,                                                   SqlDateTime defaultValue) { string[] cols = { a, b };                 return SqlDateValue(row, cols, defaultValue); }
        public SqlDateTime SqlDateValue(int row, string a, string b, string c,                                         SqlDateTime defaultValue) { string[] cols = { a, b, c };              return SqlDateValue(row, cols, defaultValue); }
        public SqlDateTime SqlDateValue(int row, string a, string b, string c, string d,                               SqlDateTime defaultValue) { string[] cols = { a, b, c, d };           return SqlDateValue(row, cols, defaultValue); }
        public SqlDateTime SqlDateValue(int row, string a, string b, string c, string d, string e,                     SqlDateTime defaultValue) { string[] cols = { a, b, c, d, e };        return SqlDateValue(row, cols, defaultValue); }
        public SqlDateTime SqlDateValue(int row, string a, string b, string c, string d, string e, string f,           SqlDateTime defaultValue) { string[] cols = { a, b, c, d, e, f };     return SqlDateValue(row, cols, defaultValue); }
        public SqlDateTime SqlDateValue(int row, string a, string b, string c, string d, string e, string f, string g, SqlDateTime defaultValue) { string[] cols = { a, b, c, d, e, f, g };  return SqlDateValue(row, cols, defaultValue); }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrValue -->
        /// <summary>
        ///      Returns a string appropriate for the boolean value of the input column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="defaultValue"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue">false value is also the default value</param>
        /// <returns></returns>
        public string StrValue(int row, string column, bool defaultValue, string trueValue, string falseValue)
        {
            if (Exists(row, column))
            {
                bool value = BoolValue(row, column, false);
                return TreatAs.StrValue(value, false, trueValue, falseValue);
            }
            else return falseValue;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- StrValue -->
        ///// <summary>
        /////      Returns the first column's value that is not blank
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="possibleColumn">array of possible column names</param>
        ///// <param name="defaultValue">default value to look for something else than</param>
        ///// <returns></returns>
        //private string StrValue(int row, string[] possibleColumn)
        //{
        //    string str = "";
        //    int count = possibleColumn.Length;
        //    for (int i = 0; i < count && str == ""; ++i)
        //        str = StrValue(row, possibleColumn[i], "");
        //    return str;
        //}
        //public string StrValue(int row, string a, string b, string c)                               { string[] cols = { a, b, c };           return StrValue(row, cols); }
        //public string StrValue(int row, string a, string b, string c, string d)                     { string[] cols = { a, b, c, d };        return StrValue(row, cols); }
        //public string StrValue(int row, string a, string b, string c, string d, string e)           { string[] cols = { a, b, c, d, e };     return StrValue(row, cols); }
        //public string StrValue(int row, string a, string b, string c, string d, string e, string f) { string[] cols = { a, b, c, d, e, f };  return StrValue(row, cols); }

        // ----------------------------------------------------------------------------------------
        /// <!-- _ToDataSet -->
        /// <summary>
        ///      Converts a data table into a dataset using a column for the DataSet table names
        /// </summary>
        /// <param name="columnToMakeIntoTables">The column containing the DataSet table names</param>
        /// <returns></returns>
        public DataSet _ToDataSet(string columnToMakeIntoTables)
        {
            DataSet ds = new DataSet();

            if (Contains(columnToMakeIntoTables))
            {
                int count = Count;
                string tableName;
                DataTable dt;
                int newRow;


                // ----------------------------------------------------------------------
                //  Construct the new table columns list
                // ----------------------------------------------------------------------
                DataColumn[] newColumns = this._NewColumnList(columnToMakeIntoTables);


                for (int row = 0; row < count; ++row)
                {
                    // ------------------------------------------------------------------
                    //  Educe a table reference
                    // ------------------------------------------------------------------
                    tableName = StrValue(row, columnToMakeIntoTables, "defaulttable");
                    if (!ds.Tables.Contains(tableName))
                        ds.Tables.Add(InData.NewTable(tableName, newColumns));
                    dt = ds.Tables[tableName];


                    // ------------------------------------------------------------------
                    //  Create a new row
                    // ------------------------------------------------------------------
                    newRow = dt.Rows.Count;
                    dt.Rows.Add(dt.NewRow());
                    foreach (DataColumn col in newColumns)
                        dt.Rows[newRow][col.ColumnName]
                            = this[row][col.ColumnName];
                }
            }

            return ds;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- TableExists -->
		/// <summary>
		///      
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		/// <remarks>, old</remarks>
		public static bool TableExists(string conn, string tableName)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES" + " WHERE TABLE_NAME = '" + tableName + "'"
                , Throws.Actions, "PD");
			int count = cmd.ExecuteScalar(-1);
			return (count > 0);
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- ToAtom -->
        /// <summary>
        ///      Returns a list of dictionaries, where each dictionary contains a row of tuples of data
        ///      as an Atom which sublcasses from List&lt;Dictionary&lt;string, object>>
        /// </summary>
        /// <returns></returns>
        public Atom ToAtom()
        {
            Atom list = new Atom();

            for (int row = 0; row < Count; ++row)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                for (int col = 0; col < Columns.Count; ++col)
                    dictionary[Columns[col].ColumnName] = ObjValue(row, col, null);
                list.Add(dictionary);
            }

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToDictionary -->
        /// <summary>
        ///      Creates a multikey dictionary from two key columns and a value column,
        ///      saving the first value for each key combination
        /// </summary>
        /// <param name="keyColumn1"></param>
        /// <param name="delim">delimits the two keys</param>
        /// <param name="keyColumn2"></param>
        /// <param name="valueColumn"></param>
        /// <param name="takeTheFirst">takes the first value for a key, otherwise takes the last value for a key</param>
        /// <returns></returns>
        /// <remarks>
        ///      Usage Example:
        ///      productInSlot = product.ToDictionary("MachineName", ":", "Selection", "ProductName", true);
        /// </remarks>
        public Dictionary<string, string> ToDictionary(string keyColumn1, string delim, string keyColumn2, string valueColumn, bool takeTheFirst)
        {
            Dictionary<string,string> output = new Dictionary<string,string>(Count);

            for (int row = 0; row < Count; ++row)
            {
                // ----------------------------------------------------------------------
                //  Resolve the key
                // ----------------------------------------------------------------------
                string part1 = StrValue(row, keyColumn1 , "");
                string part2 = StrValue(row, keyColumn2 , "");
                string key   = part1+delim+part2;


                // ----------------------------------------------------------------------
                //  Fill the value
                // ----------------------------------------------------------------------
                string value = StrValue(row, valueColumn, "");
                if (output.ContainsKey(key))
                {
                    if (!takeTheFirst) output[key] = value;
                }
                else
                    output.Add(key,value);
            }

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToDictionaryList -->
        /// <summary>
        ///      Returns a list of dictionaries, where each dictionary contains a row of tuples of data
        /// </summary>
        /// <returns></returns>
        /// <remarks>this returns sort of a natural conversion of a table into objects</remarks>
        public List<Dictionary<string, object>> ToDictionaryList()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            for (int row = 0; row < Count; ++row)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                for (int col = 0; col < Columns.Count; ++col)
                    dictionary[Columns[col].ColumnName] = ObjValue(row, col, null);
                list.Add(dictionary);
            }

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToParticle -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public Proton ToParticle(int row)
        {
            Proton particle = new Proton();
            for (int col = 0; col < Columns.Count; ++col)
                particle[Columns[col].ColumnName] = ObjValue(row, col, null);
            return particle;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToCarbon -->
        /// <summary>
        ///      Returns a String-String-Dictionary-List in the form of the Carbon class
        /// </summary>
        /// <returns></returns>
        /// <remarks>DATATABLE TO LIST</remarks>
        public Carbon ToCarbon()
        {
            Carbon list = new Carbon();

            for (int row = 0; row < Count; ++row)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                for (int col = 0; col < Columns.Count; ++col)
                    dictionary[Columns[col].ColumnName] = StrValue(row, col, "");
                list.Add(dictionary);
            }

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndeme -->
        /// <summary>
        ///      Returns an endeme contained as single characters in a series of columns
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public Endeme ToEndeme(int row, List<string> columns)
        {
            string str = "";
            Dictionary<char,int> positions = new Dictionary<char,int>(columns.Count);
            for (int i = 0; i < columns.Count; ++i)
                str += CharValue(row, columns[i], 'X');
            return new Endeme(null, str);
        }
        public Endeme ToEndeme(int row, string column, EndemeSet enSet)
        {
            return new Endeme(enSet, StrValue(row, column, ""));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeSet -->
        /// <summary>
        ///      Returns an endeme set
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="guidColumn"></param>
        /// <param name="letterColumn"></param>
        /// <param name="labelColumn"></param>
        /// <param name="meaningColumn"></param>
        /// <returns></returns>
        public EndemeSet ToEndemeSet(string setName, string guidColumn, string letterColumn, string labelColumn, string meaningColumn)
        {
            EndemeSet output = null;

            if (Count > 0)
            {
                output = new EndemeSet(GuidValue(0, guidColumn, Guid.Empty), setName);
                for (int row = 0; row < Count; ++row)
                {
                    char   letter = CharValue(row, letterColumn , ' ');
                    string label  = StrValue (row, labelColumn  , "");
                    string descr  = StrValue (row, meaningColumn, "");
                    EndemeCharacteristic cha = new EndemeCharacteristic(letter, label, descr);
                    output.Add(cha);
                }
            }
            else
            {
                output = new EndemeSet(setName);
            }

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeSet -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="charColumn"></param>
        /// <param name="labelColumn"></param>
        /// <param name="descrColumn"></param>
        /// <returns></returns>
        public EndemeSet ToEndemeSet(string setName, string charColumn, string labelColumn, string descrColumn)
        {
            EndemeSet set = new EndemeSet(setName);

            for (int row = 0; row < Count; ++row)
            {
                char   cha   = CharValue(row, charColumn , ' '); // TODO: get this from row and 'A'
                string label = StrValue (row, labelColumn, cha.ToString());
                string descr = StrValue (row, descrColumn, "");
                EndemeCharacteristic characteristic = new EndemeCharacteristic(cha, label, descr);
                set.Add(characteristic);
            }

            return set;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- UpdateRowsIn -->
        /// <summary>
        ///      Updates some column(s) in a table given a whereClause, Warning, you are responsible for handling SQL injection
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="setClause"></param>
        /// <param name="whereClause"></param>
        /// <param name="minRows"></param>
        /// <param name="maxRows"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks>Warning, you are responsible for handling SQL injection when using this method</remarks>
        public static int UpdateRowsIn(string tableName, string setClause, string whereClause, int minRows, int maxRows, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand read = new RichSqlCommand(CommandType.Text, connection, trx, "SELECT * FROM " + tableName + " " + whereClause, Throws.Actions, "PR");
            RichDataTable  test = new RichDataTable (read, null, "UpdateReadTest", "");
            if (minRows <= test.Count & test.Count <= maxRows)
            {
                RichSqlCommand update = new RichSqlCommand(CommandType.Text, connection, trx, "UPDATE " + tableName + " " + setClause + " " + whereClause, Throws.Actions, "PR");
                return test.Count;
            }
            else { return 0; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _XmlNodes -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="nameSpace"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNodeList _XmlNodes(int row, string column, string nameSpace, string xpath)
        {
            RichXmlDocument xdoc = null;
            XmlNodeList node = null;
            //string text = "";
            string xml = "";


            if (Exists(row, column))
            {
                //  Get xml
                xml = StrValue(row, column, "");
                if (xml != LastXml || string.IsNullOrEmpty(_lastXml))
                    xdoc = new RichXmlDocument(xml); // WARNING: inefficient
                else xdoc = LastXdoc;


                if (xdoc != null) node = xdoc._GetNodes(nameSpace, xpath);
            }


            RecordLast(row, column, xdoc, xml);
            return node;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _XmlStr -->
        /// <summary>
        ///      Returns a field in an xml in a table row specified by an xpath
        /// </summary>
        /// <remarks>
        ///      This WAS inefficient, now it caches the xml document with RecordLast
        /// </remarks>
        /// <param name="row">row number:  0 to n-1</param>
        /// <param name="column">The column the xml document is in</param>
        /// <param name="nameSpace">The general namespace</param>
        /// <param name="xpath">An xpath to the field</param>
        /// <returns>returns an xml field, defaults to ""</returns>
        public string _XmlStr(int row, string column, string nameSpace, string xpath)
        {
            RichXmlDocument xdoc = null;
            XmlNode node = null;
            string  text = "";
            string  xml  = "";


            if (Exists(row, column))
            {
                // ----------------------------------------------------------------------
                //  Get xml
                // ----------------------------------------------------------------------
                xml = StrValue(row, column, "");
                if (xml != LastXml || string.IsNullOrEmpty(_lastXml))
                    xdoc = new RichXmlDocument(xml);
                else xdoc = LastXdoc;


                // ----------------------------------------------------------------------
                //  Get string
                // ----------------------------------------------------------------------
                if (xdoc != null) node = xdoc._GetNode(nameSpace, xpath);
                if (node != null) text = node.InnerText;
            }


            RecordLast(row, column, xdoc, xml);
            return text;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RebuildByFirstRow -->
        /// <summary>
        ///      Rebuilds a DataTable to use the first row as the column names
        /// </summary>
        /// <param name="tableName">this is actually a new table name defined here, and does not have to be the same as the original source</param>
        /// <param name="PK">thousl should usually match the original source's primary key</param>
        /// <returns></returns>
        public RichDataTable RebuildByFirstRow(string tableName, string PK)
        {
            // --------------------------------------------------------------------------
            //  Get old columns
            // --------------------------------------------------------------------------
            string columnList = ColumnList();
            string[] origColumn = columnList.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            // --------------------------------------------------------------------------
            //  Get new columns
            // --------------------------------------------------------------------------
            List<string> newColumn = new List<string>();
            for (int col = 0; col < origColumn.Length; ++col)
            {
                newColumn.Add(__.PascalCase(StrValue(0,col,"")));
            }


            // --------------------------------------------------------------------------
            //  Copy data from old columns to new columns
            // --------------------------------------------------------------------------
            RichDataTable output = new RichDataTable(tableName, PK);
            output = Copy();
            for (int col = 0; col < origColumn.Length; ++col)
            {
                output.Add(newColumn[col], typeof(string));
                output.Set(newColumn[col], origColumn[col].Trim(), "");
            }


            // --------------------------------------------------------------------------
            //  Remove old columns and first row
            // --------------------------------------------------------------------------
            for (int col = 0; col < origColumn.Length; ++col)
            {
                output.Columns.Remove(origColumn[col].Trim());
            }
            output.Rows.RemoveAt(0);


            return output;
        }

    }
}
