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
using InformationLib.DataModels;      // for EndemeTable
using InformationLib.Endemes   ;      // for many
using InformationLib.HardData  ;      // for ConnectSource
using InformationLib.SoftData  ;      // for TreatAs.StrValue
using InformationLib.Strings   ;      // for __.CommonPrefixLength, __.LevenshteinDistance
using InformationLib.Testing   ;      // for Is.Trash
using System;                         // for Guid
using System.Collections.Generic;     // for List
using System.Data;                    // for CommandType, ConnectionState, DataTable
using System.Data.SqlClient;          // for SqlConnection, SqlTransaction
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
	// ---------------------------------------------------------------------------------------------
	/// <!-- EndemeAccess -->
    /// <summary>
    ///      The EndemeAccess class is a more recent, stripped down and better tested version of EndemeDataAccess1
    /// </summary>
	public class EndemeAccess
	{
        // -----------------------------------------------------------------------------------------
        //  Members
        // -----------------------------------------------------------------------------------------
        public static string SetIdColumn    { get { return m_setIdColumn   ; } } private static string m_setIdColumn    = "EndemeSetID"   ; // I'm going to want to change this at some point and make it consistent across all endeme tables:: EndemeSetID
        public static string SetLabelColumn { get { return m_setLabelColumn; } } private static string m_setLabelColumn = "EndemeSetLabel";
        public static string EnSchema       { get { return m_enSchemaName  ; } } private static string m_enSchemaName   = "dbo"           ;


        public          String ConnectionString { get; set; }
        public          string Errors           { get { return _errors; }
                                                  set { _errors = value; } } private string _errors;
        internal        InfoAspect _aspect          { get; set; }
        internal        Result _result;

        public   const  string ENDEME_MAIN_TABLE    = "dbo.Endeme"              ;
        public   const  string ENDEME_CHAR_TABLE    = "dbo.EndemeCharacteristic";
        public   const  string ENDEME_LARGE_TABLE   = "dbo.EndemeLarge"         ;
        public   const  string ENDEME_SET_TABLE     = "dbo.EndemeSet"           ;
        public   const  string ENDEME_PROFILE_TABLE = "dbo.EndemeProfile"       ;
        public   const  string ENDEME_INDEX_TABLE   = "dbo.EndemeIndex"         ;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        //public EndemeAccess()
        //{
        //}
        public EndemeAccess(string conn)
        {
            ConnectionString = conn;
            _errors = "";
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        /* ------------------------------------------------------------------------------------- */ /// <summary>Deletes all the characteristics for an endeme set, leaving the header intact</summary>
		private static int ClearSet(Guid setId, string conn) { return RichDataTable.AttemptToDelete(conn, "FROM "+EnSchema+".EndemeCharacteristic WHERE "+SetIdColumn+" = " + "'" + setId.ToString() + "'", 32); }


        #region Endeme Characteristic table

        // ----------------------------------------------------------------------------------------
        /// <!-- AttachEndemeSubSet -->
        /// <summary>
        ///      Makes an endeme set a subset by attaching it to a characteristic of another endeme set
        /// </summary>
        /// <param name="enCharId"></param>
        /// <param name="subEnSetId"></param>
        /// <param name="conn"></param>
        public static void AttachEndemeSubSet(Guid enCharId, Guid subEnSetId, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " UPDATE "+EnSchema+".EndemeCharacteristic SET CharacteristicIsASet = '"+subEnSetId+"'"
                + "\r\n" + " WHERE  EndemeCharacteristicID = '"+enCharId+"'"
                , Throws.Actions, "P");


            cmd.ExecuteNonQuery();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AttachEndemeSubSet -->
        /// <summary>
        ///      Makes an endeme set a subset by attaching it to a characteristic of another endeme set
        /// </summary>
        /// <param name="enCharId"></param>
        /// <param name="subEnSetId"></param>
        /// <param name="conn"></param>
        public static void AttachEndemeSubSet_old(Guid enCharId, Guid subEnSetId, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " UPDATE "+EnSchema+".EndemeCharacteristic SET CharacteristicIsASet = '"+subEnSetId+"'"
                + "\r\n" + " WHERE  charId = '"+enCharId+"'"
                , Throws.Actions, "P");


            cmd.ExecuteNonQuery();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DeleteCharacteristic -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enChar"></param>
        /// <param name="setID"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static int DeleteCharacteristic(EndemeCharacteristic enChar, Guid setID, SqlConnection connection, SqlTransaction trx)
        {
            return RichDataTable.DeleteOneRow("FROM "+EnSchema+".EndemeCharacteristic WHERE "+SetIdColumn+" = '" + setID.ToString() + "' AND CharacteristicLetter = '" + enChar.Letter + "'"
                , connection, trx);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetCharacteristicId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageSetId"></param>
        /// <param name="characteristicLabel"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Guid GetCharacteristicId(Guid ageSetId, string characteristicLabel, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " SELECT EndemeCharacteristicID"
                + "\r\n" + " FROM "+EnSchema+".EndemeCharacteristic WITH(NOLOCK)"
                + "\r\n" + " WHERE   CharacteristicLabel = '"+characteristicLabel+"'"
                + "\r\n" + "     AND "+SetIdColumn+" = '"+ageSetId.ToString()+"'"
                , Throws.Actions, "P");


            RichDataTable table = new RichDataTable(cmd, null, "OneChar", SetIdColumn);
            Guid charId = table.GuidValue(0, "CharId", Guid.Empty);
            return charId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetCharacteristicId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageSetId"></param>
        /// <param name="characteristicLabel"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Guid GetCharacteristicId_old(Guid ageSetId, string characteristicLabel, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , " SELECT CharId"
                + " FROM "+EnSchema+".EndemeCharacteristic WITH(NOLOCK)"
                + " WHERE   EndemeCharacteristicLabel = '"+characteristicLabel+"'"
                + "     AND EndemeSetId = '"+ageSetId.ToString()+"'"
                , Throws.Actions, "P");


            RichDataTable table = new RichDataTable(cmd, null, "OneChar", SetIdColumn);
            Guid charId = table.GuidValue(0, "CharId", Guid.Empty);
            return charId;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertCharacteristic -->
        /// <summary>
        ///      Inserts an endeme characteristic into a database, used by InsertEndemeSet
        /// </summary>
        /// <param name="setID"></param>
        /// <param name="letter"></param>
        /// <param name="code"></param>
        /// <param name="label"></param>
        /// <param name="descr"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks>beta quality code, tested as part of InsertEndemeSet</remarks>
        public static Guid InsertCharacteristic(Guid setID, char letter, string code, string label, string descr, SqlConnection connection, SqlTransaction trx)
        {
            Guid charID = Guid.NewGuid();


            // --------------------------------------------------------------------------
            //  Build the command
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " INSERT INTO "+EnSchema+".EndemeCharacteristic"
                + "\r\n" + "     ( EndemeCharacteristicID, EndemeSetID        , CharacteristicLetter"
                + "\r\n" + "     , CharacteristicCode    , CharacteristicLabel, CharacteristicDescr)"
                + "\r\n" + " VALUES"
                + "\r\n" + "     (@Key, @SetID, @Letter, @Code, @Label, @Descr)"
                , Throws.Actions, "PR")
                .AddParameter     ("@Key"   , charID                          )
                .AddParameter     ("@SetID" , setID                           )
                .AddParameter     ("@Letter", letter                          )
                .AddParameter_safe("@Code"  , TreatAs.StrValue(code , ""),   8)
                .AddParameter_safe("@Label" , label                      , 128)
                .AddParameter_safe("@Descr" , TreatAs.StrValue(descr, "")     );


            cmd.ExecuteNonQuery();
            return charID;
        }
        public static Guid InsertCharacteristic(EndemeCharacteristic enChar, Guid setID, SqlConnection connection, SqlTransaction trx)
            { if (enChar != null) { return InsertCharacteristic(setID, enChar.Letter, enChar.Code, enChar.Label, enChar.Descr, connection, trx); } else { return Guid.Empty; } }

        // -----------------------------------------------------------------------------------------
        /// <!-- UpdateCharacteristic -->
        /// <summary>
        ///      Updates an endeme characteristic in the database
        /// </summary>
        /// <param name="enChar"></param>
        /// <param name="setID"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int UpdateCharacteristic(EndemeCharacteristic enChar, Guid setID, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " UPDATE "+EnSchema+".EndemeCharacteristic"
                + "\r\n" + " SET     CharacteristicCode   = @Code"
                + "\r\n" + "     ,   CharacteristicLabel  = @Label"
                + "\r\n" + "     ,   CharacteristicDescr  = @Descr"
                + "\r\n" + " WHERE   EndemeSetID          = @SetID"
                + "\r\n" + "     AND CharacteristicLetter = @Letter"
                , Throws.Actions, "PR")
                .AddParameter_safe("@Code"  , enChar.Code  )
                .AddParameter_safe("@Label" , enChar.Label )
                .AddParameter_safe("@Descr" , enChar.Descr )
                .AddParameter     ("@SetID" , setID        )
                .AddParameter_char("@Letter", enChar.Letter);


            return cmd.ExecuteNonQuery();
        }

        #endregion

        #region Endeme Set table methods

        // ----------------------------------------------------------------------------------------
        /// <!-- ConvertSetToTable -->
        /// <summary>
        ///      Converts the sets produced by ConvertTableToSet into RichDataTables
        /// </summary>
        /// <param name="enSet"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <remarks>Not really data access, beta quality code</remarks>
        public static RichDataTable ConvertSetToTable(EndemeSet enSet, string tableName, bool IncludeEmptyCharacteristics = false)
        {
	        // --------------------------------------------------------------------------
	        //  Initialize output table
	        // --------------------------------------------------------------------------
	        RichDataTable output = new RichDataTable(tableName, SetIdColumn);
	        output.Add(SetIdColumn           , typeof(Guid)  );
	        output.Add("EndemeSetLabel"      , typeof(string));
	        output.Add("CharacteristicLetter", typeof(char)  );
	        output.Add("CharacteristicCode"  , typeof(string));
	        output.Add("CharacteristicLabel" , typeof(string));
	        output.Add("CharacteristicDescr" , typeof(string));


	        for (int c = 0; c < enSet.Count; c++)
            {
                // ----------------------------------------------------------------------
                //  Add row to table
                // ----------------------------------------------------------------------
                EndemeCharacteristic enChar = enSet.Characteristics()[c];

                if (IncludeEmptyCharacteristics | __.StringHasContent(enChar.Label)) {
        	        int row = output.Add();
        	        output[row][SetIdColumn           ] = enSet.SetId     ;
        	        output[row]["EndemeSetLabel"      ] = enSet.Label  ;
        	        output[row]["CharacteristicLetter"] = enChar.Letter;
        	        output[row]["CharacteristicCode"  ] = enChar.Code  ;
        	        output[row]["CharacteristicLabel" ] = enChar.Label ;
        	        output[row]["CharacteristicDescr" ] = enChar.Descr ;
                }
	        }

	        return output;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- ConvertTableToSet -->
        /// <summary>
        ///      Converts a table produced by ConvertSetToTable into an endeme set
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        /// <remarks>Not really data access, beta quality code</remarks>
        public static EndemeSet ConvertTableToSet(DataTable dt)
        {
            RichDataTable rt = new RichDataTable                      (dt, "CharacteristicLetter"            ) ;
            EndemeSet     es = new EndemeSet            ( rt.GuidValue(0 , SetIdColumn           , Guid.Empty)
                                                        , rt.StrValue (0 , "EndemeSetLabel"      , ""        ));
            for (int rw = 0; rw < rt.Count; rw++) es.Add( rt.CharValue(rw, "CharacteristicLetter", ' '       )
                                                        , rt.StrValue (rw, "CharacteristicCode"  , ""        )
                                                        , rt.StrValue (rw, "CharacteristicLabel" , ""        )
                                                        , rt.StrValue (rw, "CharacteristicDescr" , ""        ));
            return es;
        }

        public static int DeleteEndemeSet(EndemeSet enSet, SqlConnection connection, SqlTransaction trx)
        {
            int result = -1;

            //  get the set ID
            Guid enSetID = Guid.Empty;
            bool setFound = false;
            if (enSet.SetId == Guid.Empty) { enSetID = GetEndemeSetID(enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // Look for set by name
            else { enSetID = GetEndemeSet(enSet.SetId, connection, trx).SetId; }


            // --------------------------------------------------------------------------
            //  Delete the characeristics and the header
            // --------------------------------------------------------------------------
            if (enSetID != Guid.Empty)
            {
                result = RichDataTable.DeleteSomeRows("FROM "+EnSchema+".EndemeCharacteristic WHERE "+SetIdColumn+" = '" + enSetID.ToString() + "'", 24, connection, trx);
                result = RichDataTable.DeleteOneRow  ("FROM "+EnSchema+".EndemeSet            WHERE "+SetIdColumn+" = '" + enSetID.ToString() + "'"    , connection, trx);
            }

            return result;
        }
        public static int DeleteEndemeSet(EndemeSet enSet, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); int result = DeleteEndemeSet(enSet, connection, null); connection.Dispose(); return result; }

        // ----------------------------------------------------------------------------------------
        /// <!-- EduceSetHeader -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setLabel"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>alpha quality code</remarks>
        public static Guid EduceSetHeader(string setLabel, SqlConnection connection, SqlTransaction trx)
        {

            // --------------------------------------------------------------------------
            //  Initialize Variables
            // --------------------------------------------------------------------------
            Guid setID = Guid.Empty;
            RichSqlCommand cmdRead = null;
            RichSqlCommand cmdInsert = null;

            try
            {
                // -----------------------------------------------------------------------
                //  try to get a table containing the endeme set ID
                // -----------------------------------------------------------------------
                cmdRead = new RichSqlCommand(CommandType.Text, connection, trx
                    , "SELECT * FROM "+EnSchema+".EndemeSet WITH(NOLOCK) WHERE "+SetLabelColumn+" = @SetLabel"
                    , Throws.Actions, "P")
                    .AddParameter_safe("@SetLabel", setLabel);


                RichDataTable table = new RichDataTable(cmdRead, trx, "EndemeSet", SetIdColumn);


                if ((table.Count < 1))
                {
                    // -------------------------------------------------------------------
                    //  Build command to create the set if it does not yet exist
                    // -------------------------------------------------------------------
                    cmdInsert = new RichSqlCommand(CommandType.Text, connection, null
                        , " INSERT INTO "+EnSchema+".EndemeSet (" + SetIdColumn + ", "+SetLabelColumn+") VALUES (@EnSetId ,@SetLabel)"
                        , Throws.Actions, "P")
                        .AddParameter     ("@EnSetId", Guid.NewGuid())
                        .AddParameter_safe("@SetLabel", setLabel);
                    cmdInsert.ExecuteNonQuery();

                    table = new RichDataTable(cmdRead, trx, "EndemeSet", SetIdColumn);
                }

                // -----------------------------------------------------------------------
                //  Extract Endeme Set ID
                // -----------------------------------------------------------------------
                setID = table.GuidValue(0, SetIdColumn, Guid.Empty);

            }
            catch (Exception ex) { Is.Trash(ex); }
            finally
            {
                if (cmdRead   != null) { cmdRead  .Dispose(); }
                if (cmdInsert != null) { cmdInsert.Dispose(); }
            }

            return setID;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSet -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code, unit tested</remarks>
        public static EndemeSet GetEndemeSet(Guid setID, SqlConnection connection, SqlTransaction trx)
        {
            EndemeSet enSet = EndemeSet.Empty;
            RichSqlCommand cmd = null;

            try
            {
                string setNameColumn = "EndemeSetLabel";

                // --------------------------------------------------------------------------
                //  Retrieve the endeme set data
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, connection, trx
                    , "\r\n" + " SELECT s." + SetIdColumn + ", s."+SetLabelColumn+" AS " + setNameColumn + ", c.*"
                    + "\r\n" + " FROM                "+ENDEME_SET_TABLE +" AS s WITH(NOLOCK)"
                    + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_CHAR_TABLE+" AS c WITH(NOLOCK) ON c."+SetIdColumn+" = s." + SetIdColumn
                    + "\r\n" + " WHERE s." + SetIdColumn + " = @SetID"
                    , Throws.Actions, "PR")
                    .AddParameter("@SetID", setID);
                RichDataTable source = new RichDataTable(cmd, trx, "EndemeSet", SetIdColumn);


                // --------------------------------------------------------------------------
                //  Construct the endeme set from the retrieved data
                // --------------------------------------------------------------------------
                enSet = new EndemeSet(setID, source.StrValue(0, setNameColumn, ""));

                for (int row = 0; row < source.Count; row++)
                {
                    enSet.Add
                        ( source.CharValue(row, "CharacteristicLetter", ' ')
                        , source.StrValue (row, "CharacteristicCode"  , "" )
                        , source.StrValue (row, "CharacteristicLabel" , "" )
                        , source.StrValue (row, "CharacteristicDescr" , "" )
                        );
                }
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { cmd.Dispose(); cmd = null; }
            return enSet;
        }
        public static EndemeSet GetEndemeSet(Guid setID, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); EndemeSet enSet = GetEndemeSet(setID, connection, null); connection.Dispose(); return enSet; }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSet -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static EndemeSet GetEndemeSet(string setName, string conn)
        {
            EndemeSet enSet = EndemeSet.Empty;
            RichSqlCommand cmd = null;

            try
            {
                string setNameColumn = "EndemeSetLabel";

                // --------------------------------------------------------------------------
                //  Retrieve the endeme set data
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, conn
                    , "\r\n" + " SELECT s." + SetIdColumn + ", s.EndemeSetLabel AS " + setNameColumn + ", c.*"
                    + "\r\n" + " FROM                "+ENDEME_SET_TABLE +" AS s WITH(NOLOCK)"
                    + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_CHAR_TABLE+" AS c WITH(NOLOCK) ON c."+SetIdColumn+" = s." + SetIdColumn
                    + "\r\n" + " WHERE s.EndemeSetLabel = @SetName"
                    , Throws.Actions, "PR")
                    .AddParameter_safe("@SetName", setName);


                RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", SetIdColumn);


                // --------------------------------------------------------------------------
                //  Construct the endeme set from the retrieved data
                // --------------------------------------------------------------------------
                enSet = new EndemeSet(table.GuidValue(0, SetIdColumn, Guid.Empty), table.StrValue(0, setNameColumn, ""));

                for (int row = 0; row < table.Count; row++)
                {
                    enSet.Add
                        ( table.CharValue(row, "EndemeCharLetter", ' ')
                        , table.StrValue (row, "EndemeCharCode"  , "" )
                        , table.StrValue (row, "EndemeCharLabel" , "" )
                        , table.StrValue (row, "EndemeCharDescr" , "" )
                        );
                }
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { cmd.Dispose(); cmd = null; }
            return enSet;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSetHeader -->
        /// <summary>
        ///      Returns a table contining the header row only for the endeme set
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>alpha quality code</remarks>
        public static RichDataTable GetEndemeSetHeader(Guid setId, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "SELECT * FROM "+EnSchema+".EndemeSet WITH(NOLOCK) WHERE " + SetIdColumn + " = @SetID"
                , Throws.Actions, "PR")
                .AddParameter("@SetID", setId);
            RichDataTable header = new RichDataTable(cmd, trx, "EndemeSetHeader", SetIdColumn);


            header.IdColumn = SetIdColumn;
            return header;
        }
        public static RichDataTable GetEndemeSetHeader(Guid setId, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); RichDataTable table = GetEndemeSetHeader(setId, connection, null); connection.Dispose(); return table; }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSetID -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code, unit tested</remarks>
        public static Guid GetEndemeSetID(string label, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "SELECT " + SetIdColumn + " FROM "+EnSchema+".EndemeSet WITH(NOLOCK) WHERE "+SetLabelColumn+" = @SetName"
                , Throws.Actions, "PR")
                .AddParameter_safe("@SetName", label);
            RichDataTable table = new RichDataTable(cmd, trx, "EndemeSet", SetIdColumn);


            Guid setID = table.GuidValue(0, SetIdColumn, Guid.Empty);
            return setID;
        }
        public static Guid GetEndemeSetID(string label, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); Guid id = GetEndemeSetID(label, connection, null); connection.Dispose(); return id; }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertEndemeSet -->
        /// <summary>
        ///      Stores an endeme set in the database using a transaction to make sure it is complete
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>
        ///      You should either save the whole endeme set, or if something goes wrong, none of it
        /// 
        ///      beta quality code, unit tested
        /// </remarks>
        public static Guid InsertEndemeSet(EndemeSet enSet, SqlConnection connection, SqlTransaction inputTrx)
        {
            // --------------------------------------------------------------------------
            //  Handle the connection
            // --------------------------------------------------------------------------
            bool closeConn = false;
            bool commitTrx = false;
            if (connection.State == ConnectionState.Closed)
                { InData.Open(connection); closeConn = true; commitTrx = true; }


            // --------------------------------------------------------------------------
            //  This should be in a transaction
            // --------------------------------------------------------------------------
            SqlTransaction trx = default(SqlTransaction);
            if (inputTrx == null) { trx = InData.Begin(connection); commitTrx = true ; }
            else                  { trx = inputTrx;                                    }

            Guid setID = Guid.Empty;


            try
            {
                // -----------------------------------------------------------------------
                //  Create the endeme set
                // -----------------------------------------------------------------------
                setID = InsertEndemeSetHeader(enSet.Label, connection, trx);
                char[] alphabet = enSet.UnsortedAlphabet;
                EndemeCharacteristic cha;
                foreach (char c in alphabet)
                    { cha = enSet[c]; if (cha != null) { InsertCharacteristic(setID, cha.Letter, cha.Code, cha.Label, cha.Descr, connection, trx); } }


                // -----------------------------------------------------------------------
                //  Complete the process
                // -----------------------------------------------------------------------
                if (commitTrx)                    { InData.Commit(trx)  ; }
            }                                     
            catch (Exception ex) { if (commitTrx) { InData.Rollback(trx); } setID = Guid.Empty; Is.Trash(ex); }
            finally              { if (commitTrx) { trx = null          ; }
                                   if (commitTrx && closeConn && !(connection.State == ConnectionState.Closed)) { InData.Close(connection); } }
            return setID;
        }
        public static Guid InsertEndemeSet(EndemeSet enSet, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); Guid id = InsertEndemeSet(enSet, connection, null); connection.Dispose(); return id; }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertEndemeSetHeader -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setLabel"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code</remarks>
        public static Guid InsertEndemeSetHeader(string setLabel, SqlConnection connection, SqlTransaction trx)
        {
            Guid setID = Guid.NewGuid();

            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "INSERT INTO "+EnSchema+".EndemeSet (" + SetIdColumn + ", "+SetLabelColumn+") VALUES (@SetID, @Label)"
                , Throws.Actions, "PR")
                .AddParameter     ("@SetID", setID   )
                .AddParameter_safe("@Label", setLabel);


            cmd.ExecuteNonQuery();
            return setID;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- RetrieveAllSets -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>alpha quality code</remarks>
        public static RichDataTable RetrieveAllSets(SqlConnection connection, SqlTransaction trx)
        {
            // --------------------------------------------------------------------------
            //  Build the query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " SELECT s."+SetLabelColumn+" AS SetLabel         , c.CharacteristicLetter AS Letter"
                + "\r\n" + "     , c.CharacteristicCode AS Code , c.CharacteristicLabel"
                + "\r\n" + "     , c.CharacteristicDescr        , s." + SetIdColumn + ", c.EndemeCharacteristicID"
                + "\r\n" + " FROM                "+ENDEME_SET_TABLE +" AS s WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_CHAR_TABLE+" AS c WITH(NOLOCK) ON c.EndemeSetID = s." + SetIdColumn
                + "\r\n" + " ORDER BY s."+SetLabelColumn+", c.CharacteristicLetter"
                , Throws.Actions, "PR");
            RichDataTable all = new RichDataTable(cmd, trx, "EndemeSet", SetIdColumn);


            all.IdColumn = "EndemeCharacteristicID";
            return all;
        }
        public static RichDataTable RetrieveAllSets(string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); RichDataTable table = RetrieveAllSets(connection, null); connection.Dispose(); return table; }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpdateEndemeSet -->
        /// <summary>
        ///      Updates an endeme set in the database
        /// </summary>
        /// <param name="enSetNew"></param>
        /// <param name="setID"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks>endeme set labels are not expected to change</remarks>
        public static int UpdateEndemeSet(EndemeSet enSetNew, Guid setID, SqlConnection connection, SqlTransaction trx)
        {
            // --------------------------------------------------------------------------
            //  Resolve context
            // --------------------------------------------------------------------------
            EndemeSet enSetCurrent = GetEndemeSet(setID, connection, trx);                  List<char> currAlphabet = new List<char>(enSetCurrent.UnsortedAlphabet);
            Endeme    enCombined   = enSetNew.RandomEndeme() + enSetCurrent.RandomEndeme(); List<char> newAlphabet  = new List<char>(enSetNew.UnsortedAlphabet    );
            char[]    allAlphabet  = enCombined.ToString().ToCharArray();
            int       result = 0;


            //  endeme set labels are not expected to change


            // --------------------------------------------------------------------------
            //  Go through each candidate charcteristic and update, delete or insert it
            // --------------------------------------------------------------------------
            for (int i = 0; i <= allAlphabet.Length; i++)
            {
                char letter = allAlphabet[i];
                if ( currAlphabet.Contains(letter) &  newAlphabet.Contains(letter)) { UpdateCharacteristic(enSetNew[letter]    , setID, connection, trx);               }
                if ( currAlphabet.Contains(letter) & !newAlphabet.Contains(letter)) { DeleteCharacteristic(enSetCurrent[letter], setID, connection, trx); result += -1; }
                if (!currAlphabet.Contains(letter) &  newAlphabet.Contains(letter)) { InsertCharacteristic(enSetNew[letter]    , setID, connection, trx); result += +1; }
            }

            return result;
        }

        #endregion

        #region Endeme table methods

        // -----------------------------------------------------------------------------------------
        /// <!-- ClosestEndemeTo -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enSet"></param>
        /// <returns></returns>
        /// <remarks>Not really data access, more of a complex parsing method, alpha quality code</remarks>
        public static Endeme ClosestEndemeTo(string str, EndemeSet enSet)
        {
	        // --------------------------------------------------------------------------
	        //  Initialize results
	        // --------------------------------------------------------------------------
	        int maxLen = 0;
	        EndemeCharacteristic maxChar = new EndemeCharacteristic();
	        char[] alphabet = enSet.UnsortedAlphabet;
            EndemeCharacteristic enChar;
            int len;


	        if (maxLen < 2)
            {
                // ----------------------------------------------------------------------
                //  Look for best label prefix match
                // ----------------------------------------------------------------------
                maxLen = 0;
                maxChar = new EndemeCharacteristic();
                for (int i = 0; i <= alphabet.Length - 1; i++)
                {
        	        enChar = enSet[alphabet[i]]; len = __.CommonPrefixLength(str, enChar.Label, false);
        	        if (len > maxLen) { maxLen = len; maxChar = enChar; }
                }
	        }


	        if (maxLen < 2)
            {
                // ----------------------------------------------------------------------
                //  Look for best code prefix match
                // ----------------------------------------------------------------------
                maxLen = 0;
                maxChar = new EndemeCharacteristic();
                for (int i = 0; i <= alphabet.Length - 1; i++)
                {
        	        enChar = enSet[alphabet[i]]; len = __.CommonPrefixLength(str, enChar.Code, false);
        	        if (len > maxLen) { maxLen = len; maxChar = enChar; }
                }
	        }


	        if (maxLen < 2)
            {
                // ----------------------------------------------------------------------
                //  Look for closest Levenschtein distance match
                // ----------------------------------------------------------------------
                maxLen = 0;
                maxChar = new EndemeCharacteristic();
                for (int i = 0; i <= alphabet.Length - 1; i++)
                {
        	        enChar = enSet[alphabet[i]]; len = Math.Max(str.Length, enChar.Label.Length) - __.LevenshteinDistance(str, enChar.Label);
        	        if (len > maxLen) { maxLen = len; maxChar = enChar; }
                }
	        }

	        return new Endeme(enSet, maxChar.Letter.ToString());
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndeme -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endemeID"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks>unit tested</remarks>
        public static Endeme GetEndeme(Guid endemeID, SqlConnection connection, SqlTransaction trx)
        {

            Endeme en = Endeme.Empty;
            RichSqlCommand cmd = null;

            try
            {
                // --------------------------------------------------------------------------
                //  Build Query
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, connection, trx
                    , "\r\n" + " SELECT e.*"
                    + "\r\n" + " FROM  "+EnSchema+".Endeme AS e WITH(NOLOCK)"
                    + "\r\n" + " WHERE EndemeID = @EndemeID"
                    , Throws.Actions, "PR")
                    .AddParameter("@EndemeID", endemeID);
                RichDataTable table = new RichDataTable(cmd, trx, "Endeme", "EndemeID");


                Guid setID = table.GuidValue(0, SetIdColumn, Guid.Empty);
                en = new Endeme(GetEndemeSet(setID, connection, trx), table.StrValue(0, "CharString", ""));
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { if (cmd != null) { cmd.Dispose(); } }

            return en;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeID -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listLabel"></param>
        /// <param name="setID"></param>
        /// <param name="endemeLabel"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static List<Guid> GetEndemeID(string listLabel, Guid setID, string endemeLabel, SqlConnection connection, SqlTransaction trx)
        {

            List<Guid> endemeID = new List<Guid>();
            RichSqlCommand cmd = null;

            try
            {
                // --------------------------------------------------------------------------
                //  Build Query
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, connection, trx
                    , "\r\n" + " SELECT e.*"
                    + "\r\n" + " FROM  "+EnSchema+".Endeme AS e WITH(NOLOCK)"
                    + "\r\n" + " WHERE EndemeSetID = @SetID AND EndemeListLabel = @ListLabel AND EndemeLabel = @EndemeLabel"
                    , Throws.Actions, "PR")
                    .AddParameter     ("@SetID"      , setID      )
                    .AddParameter_safe("@ListLabel"  , listLabel  )
                    .AddParameter_safe("@EndemeLabel", endemeLabel);
                RichDataTable table = new RichDataTable(cmd, trx, "Endeme", "EndemeID");


                endemeID = table.ToList("EndemeID", Guid.Empty);
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { if (cmd != null) { cmd.Dispose(); } }

            return endemeID;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertEndeme -->
        /// <summary>
        ///      This non-simple version does a check to make sure the endeme set is in the database,
        ///      finds the endeme set id if it is and stores the endeme set in the database if it is not
        /// </summary>
        /// <param name="en"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code, unit tested</remarks>
        public static Guid InsertEndeme(Endeme en, SqlConnection connection, SqlTransaction trx)
        {
            EndemeSet enSet = en.EnSet;
            RichDataTable header;


            // check for a real endeme set
            if (enSet == null || enSet.Count == 0) 
            {
                return Guid.Empty;
            }
            else
            {
                // -----------------------------------------------------------------------
                //  Educe endeme set id
                // -----------------------------------------------------------------------
                bool setFound = false;
                if (enSet.SetId == Guid.Empty)                      { enSet.SetId = GetEndemeSetID    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // Look for set by name
                else {                                             header   = GetEndemeSetHeader(enSet.SetId   , connection, trx);                                         // If there is a set id, Look for the set by guid.
                        if (header == null || header.Count == 0) { enSet.SetId = GetEndemeSetID    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // If the set id is not good, Look for the set by name
                        else                                     {                                                              setFound = true;                      } // the normal case
                     }
                if (!setFound) { enSet.SetId = InsertEndemeSet(enSet, connection, trx); }


                // -----------------------------------------------------------------------
                //  Insert endeme
                // -----------------------------------------------------------------------
                return InsertEndeme_simple(enSet.SetId, en, connection, trx);
            }
        }
        public static Guid InsertEndeme(Endeme en, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); Guid id = InsertEndeme(en, connection, null); connection.Dispose(); return id; }

        // -----------------------------------------------------------------------------------------
        /// <!--  InsertEndeme_simple -->
        /// <summary>
        ///       Stores the endeme in the database (method assumes that the endeme set id is good)
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="en"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>
        ///       The non-simple version should also do a check to make sure the endeme set is in
        ///       the database too and store the endeme set as well if not
        /// 
        ///       beta quality code
        /// </remarks>
        private static Guid InsertEndeme_simple(Guid setId, Endeme en, SqlConnection connection, SqlTransaction trx)
        {
            RichDataTable header = GetEndemeSetHeader(setId, connection, trx);
            Guid endemeID = Guid.NewGuid();


            // --------------------------------------------------------------------------
            //  Construct command
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " INSERT INTO "+EnSchema+".Endeme"
                + "\r\n" + "        (  EndemeId,  CharString,  " + SetIdColumn + ", DerivedSetLabel,  EndemeLabel)"
                + "\r\n" + " VALUES ( @EndemeId, @CharString,     @EndemeSetId    ,   @DerivedLabel, @EndemeLabel)"
                , Throws.Actions, "PR")
                .AddParameter     ("@EndemeId"    , endemeID                              )
                .AddParameter_safe("@CharString"  , en.ToString()                         )
                .AddParameter     ("@EndemeSetId" , setId                                 )
                .AddParameter_safe("@DerivedLabel", header.StrValue(0, SetLabelColumn, ""))
                .AddParameter_safe("@EndemeLabel" , ""                                    );


            cmd.ExecuteNonQuery();
            return endemeID;
        }
        private static Guid InsertEndeme_simple(Guid setId, Endeme en, string conn)
        { SqlConnection connection = ConnectSource.Connection(conn); Guid id = InsertEndeme_simple(setId, en, connection, null); connection.Dispose(); return id; }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertEndemeItem -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listLabel"></param>
        /// <param name="enItem"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static List<Guid> InsertEndemeItem(string listLabel, EndemeItem enItem, SqlConnection connection, SqlTransaction trx)
        {
            List<Guid> endemeID = new List<Guid>();


            if (enItem == null || enItem.ItemEndeme == null || enItem.ItemEndeme.EnSet == null || enItem.ItemEndeme.EnSet.Count == 0)
            {
                return endemeID;
            }
            else
            {
                // -----------------------------------------------------------------------
                //  Educe endeme set id
                // -----------------------------------------------------------------------
                EndemeSet enSet = enItem.ItemEndeme.EnSet;
                RichDataTable header;
                bool setFound = false;
                if (enSet.SetId == Guid.Empty)                      { enSet.SetId = GetEndemeSetID    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // Look for set by name
                else {                                             header   = GetEndemeSetHeader(enSet.SetId   , connection, trx);                                         // If there is a set id, Look for the set by guid.
                        if (header == null || header.Count == 0) { enSet.SetId = GetEndemeSetID    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // If the set id is not good, Look for the set by name
                        else                                     {                                                              setFound = true;                      } // the normal case
                     }
                if (!setFound) { enSet.SetId = InsertEndemeSet(enSet, connection, trx); }


                // -----------------------------------------------------------------------
                //  Educe the endeme item
                // -----------------------------------------------------------------------
                endemeID = GetEndemeID(listLabel, enSet.SetId, enItem.ItemLabel, connection, trx);
                if (endemeID.Count < 1)
                    endemeID.Add(InsertEndemeItem_simple(listLabel, enSet.SetId, enItem, connection, trx));
            }

            return endemeID;
        }
        public static List<Guid> InsertEndemeItem(string listLabel, EndemeItem enItem, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); List<Guid> endemeID = InsertEndemeItem(listLabel, enItem, connection, null); connection.Dispose(); return endemeID; }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertEndemeItem_simple -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="enItem"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        private static Guid InsertEndemeItem_simple(string listLabel, Guid setId, EndemeItem enItem, SqlConnection connection, SqlTransaction trx)
        {
            RichDataTable header = GetEndemeSetHeader(setId, connection, trx);
            Guid endemeID = Guid.NewGuid();
            if (enItem.ItemKey != Guid.Empty) endemeID = enItem.ItemKey;


            // --------------------------------------------------------------------------
            //  Construct command
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " INSERT INTO "+EnSchema+".Endeme"
                + "\r\n" + "        (  EndemeId,  CharString,  " + SetIdColumn + ", DerivedSetLabel,  EndemeLabel, EndemeListLabel)"
                + "\r\n" + " VALUES ( @EndemeId, @CharString,     @EndemeSetId    ,   @DerivedLabel, @EndemeLabel,      @ListLabel)"
                , Throws.Actions, "PR")
                .AddParameter     ("@EndemeId"    , endemeID                              )
                .AddParameter_safe("@CharString"  , enItem.ItemEndeme.ToString()         )
                .AddParameter     ("@EndemeSetId" , setId                                 )
                .AddParameter_safe("@DerivedLabel", header.StrValue(0, SetLabelColumn, ""))
                .AddParameter_safe("@EndemeLabel" , enItem.ItemLabel                      )
                .AddParameter_safe("@ListLabel"   , listLabel                        , 128);


            cmd.ExecuteNonQuery();
            return endemeID;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- RescueEndemes -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldSetID"></param>
        /// <param name="newSetLabel"></param>
        /// <param name="minRows"></param>
        /// <param name="maxRows"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static int RescueEndemes(Guid oldSetID, string newSetLabel, int minRows, int maxRows, SqlConnection connection, SqlTransaction trx)
        {
            Guid newSetID = GetEndemeSetID(newSetLabel, connection, trx);
            return RichDataTable.UpdateRowsIn("Endeme", "Set SetID = '" + newSetID.ToString() + "'", "WHERE SetID = '" + oldSetID.ToString() + "'", minRows, maxRows, connection, trx);
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- UpdateEndeme -->
        /// <summary>
        ///      Updates an endeme in the database
        /// </summary>
        /// <param name="en"></param>
        /// <param name="endemeID"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns>standard ExecuteNonQuery result</returns>
        /// <remarks>alpha quality code, unit tested</remarks>
        public static int UpdateEndeme(Endeme en, Guid endemeID, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , " UPDATE Endeme SET CharString = @Value WHERE EndemeID = @EndemeID"
                , Throws.Actions, "PR")
                .AddParameter_safe("@Value", en.ToString())
                .AddParameter     ("@EndemeID", endemeID);


            return cmd.ExecuteNonQuery();
        }

        #endregion



        //  new tables

        #region EndemeTable table methods
        // ----------------------------------------------------------------------------------------
        //  Endeme table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeTable> AtMareEndemeTableOf         (int         endemesetId    , InfoAspect aspect) { return AtOnMareEndemeTable(MareEndemeTableOf           (endemesetId    , aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndeme       (int         endemeId       , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndeme       (endemeId       , aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeSet    (Guid        endemeSetId    , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeSet    (endemeSetId    , aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeString (string      endemeString   , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeString (endemeString   , aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeCode   (string      endemeCode     , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeCode   (endemeCode     , aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeLabel  (string      endemeLabel    , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeLabel  (endemeLabel    , aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeDescr  (string      endemeDescr    , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeDescr  (endemeDescr    , aspect)); }
        public List<EndemeTable> AtEndemeTableOfRawValue     (bool        rawValue       , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfRawValue     (rawValue       , aspect)); }
        public List<EndemeTable> AtEndemeTableOfMultipleUse  (bool        multipleUse    , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfMultipleUse  (multipleUse    , aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueBinary  (byte[]      valueBinary    , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueBinary  (valueBinary    , aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueDateTime(DateTime    valueDateTime  , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueDateTime(valueDateTime  , aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueFloat   (double      valueFloat     , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueFloat   (valueFloat     , aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueNumber  (decimal     valueNumber    , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueNumber  (valueNumber    , aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueText    (string      valueText      , InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueText    (valueText      , aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeProfile(int         endemeProfileId, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeProfile(endemeProfileId, aspect)); }
        public Atom              AtomMaEndemeTableOf         (int         endemesetId    , InfoAspect aspect) { return MainEndemeTableOf  (endemesetId, aspect).ToAtom(); }
        public void              ExEndemeTable               (int         endemeId       , InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM "+ENDEME_MAIN_TABLE+" WHERE EndemeId = " + endemeId, aspect.MainConn); }
        public RichDataTable     IntoEndemeTable             (EndemeTable endemeTable    , InfoAspect aspect) { if (   IsEndemeTable(endemeTable.EndemeId, aspect)) return InEndemeTable(endemeTable.EndemeId, aspect); else { return InEndemeTable(ToEndemeTable(endemeTable, aspect), aspect); } }
        public EndemeTable       OnInEndemeTable             (int         endemeId       , InfoAspect aspect) { return OnEndemeTable(InEndemeTable  (endemeId, aspect), 0, true); }
        public EndemeTable       OnIntoEndemeTable           (EndemeTable endemeTable    , InfoAspect aspect) { return OnEndemeTable(IntoEndemeTable(endemeTable  , aspect), 0, true); }
        public int               UpToEndemeTable             (EndemeTable endemeTable    , InfoAspect aspect) { if (   IsEndemeTable(endemeTable.EndemeId, aspect)) return UpEndemeTable(endemeTable        , aspect); else { return ToEndemeTable(endemeTable, aspect); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeTable_test -->
        /// <summary>
        ///      Tests the ToEndemeTable method
        /// </summary>
        public void ToEndemeTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeTableAccess";
            string adoMethod = "ToEndemeTable";
            Random r = RandomSource.New().Random;
            int    endemeId = -1;
            EndemeAccess ende = new EndemeAccess("");


            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeTable endemeTableTo = ende.AnneEndemeTable(r);
            try
            {
                endemeId = ende.ToEndemeTable(endemeTableTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeTable endemeTableFrom = ende.OnInEndemeTable(endemeId, _aspect);
            ende.AssertEqualContent(endemeTableFrom, endemeTableTo, adoClass, adoMethod);


            ende.ExEndemeTable(endemeId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeTable_test -->
        /// <summary>
        ///      Tests the UpEndemeTable method
        /// </summary>
        public void UpEndemeTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass          = "EndemeTableAccess";
            string adoMethod         = "UpEndemeTable";
            Random r                 = RandomSource.New().Random;
            EndemeTable endemeTable1 = null;

            EndemeAccess ende = new EndemeAccess("");
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeId with a newly created endemeTable
                // ----------------------------------------------------------------------
                int    endemeId = ende.AnIdOfEndemeTable(_aspect);
                endemeTable1               = ende.OnInEndemeTable(endemeId, _aspect);
                EndemeTable  endemeTable2  = ende.AnneEndemeTable(r);
                endemeTable2.EndemeId = endemeTable1.EndemeId;
                ende.UpEndemeTable(endemeTable2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeTable endemeTable3 = ende.OnInEndemeTable(endemeId, _aspect);
                Assert.That(endemeTable3.EndemeId, Is.equal_to, endemeTable2.EndemeId, adoClass, adoMethod + " test update existing endemeTable");
                ende.AssertEqualContent  (endemeTable3, endemeTable2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeTable3, endemeTable1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeTable, did the update fail?
                // ----------------------------------------------------------------------
                EndemeTable  endemeTable4  = ende.AnneEndemeTable(r);
                endemeTable4.EndemeId = ende.HiIdOfEndemeTable(_aspect) + 1;
                int          count         = ende.UpEndemeTable(endemeTable4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeTable");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeTable(endemeTable1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeTable -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeTable table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public int AnIdOfEndemeTable(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT EndemeId FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeTableId = new RichDataTable(cmd, null, "EndemeTableId", "EndemeId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeTableId.Count > 0) return (int)endemeTableId.ToList("EndemeId", -1)[r.Next(endemeTableId.Count)];
                else return -1;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeTable -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeTable object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeTable</returns>
        private EndemeTable AnneEndemeTable(Random r)
        {
            EndemeTable endemeTable = new EndemeTable
              //{ EndemeId        = go.HiIdOfEndemeId() + 1
                { EndemeSetId     = Guid.NewGuid()
                , EndemeString    = r.Next(10000).ToString()
                , EndemeCode      = r.Next(10000).ToString()
                , EndemeLabel     = r.Next(10000).ToString()
                , EndemeDescr     = r.Next(10000).ToString()
                , RawValue        = true
                , MultipleUse     = true
                , ValueBinary     = null
                , ValueDateTime   = DateTime.Now
                , ValueFloat      = r.Next(10000)
                , ValueNumber     = r.Next(10000)
                , ValueText       = r.Next(10000).ToString()
                , EndemeProfileId = r.Next(10000)
                };
            return endemeTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeTable">endemeTable being tested</param>
        /// <param name="tgt"      >endemeTable being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeTable endemeTable, EndemeTable tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeTable.EndemeSetId    , Is.equal_to, tgt.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(endemeTable.EndemeString   , Is.equal_to, tgt.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(endemeTable.EndemeCode     , Is.equal_to, tgt.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(endemeTable.EndemeLabel    , Is.equal_to, tgt.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(endemeTable.EndemeDescr    , Is.equal_to, tgt.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
            Assert.That(endemeTable.RawValue       , Is.equal_to, tgt.RawValue       , adoClass, adoMethod + " RawValue"       );
            Assert.That(endemeTable.MultipleUse    , Is.equal_to, tgt.MultipleUse    , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(endemeTable.ValueBinary    , Is.equal_to, tgt.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(endemeTable.ValueDateTime  , Is.equal_to, tgt.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(endemeTable.ValueFloat     , Is.equal_to, tgt.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(endemeTable.ValueNumber    , Is.equal_to, tgt.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(endemeTable.ValueText      , Is.equal_to, tgt.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(endemeTable.EndemeProfileId, Is.equal_to, tgt.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeTable">endemeTable being tested</param>
        /// <param name="tgt"      >endemeTable being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeTable endemeTable, EndemeTable tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeTable.EndemeSetId    , Is.not_equal_to, tgt.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(endemeTable.EndemeString   , Is.not_equal_to, tgt.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(endemeTable.EndemeCode     , Is.not_equal_to, tgt.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(endemeTable.EndemeLabel    , Is.not_equal_to, tgt.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(endemeTable.EndemeDescr    , Is.not_equal_to, tgt.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
          //Assert.That(endemeTable.RawValue       , Is.not_equal_to, tgt.RawValue       , adoClass, adoMethod + " RawValue"       );
          //Assert.That(endemeTable.MultipleUse    , Is.not_equal_to, tgt.MultipleUse    , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(endemeTable.ValueBinary    , Is.not_equal_to, tgt.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(endemeTable.ValueDateTime  , Is.not_equal_to, tgt.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(endemeTable.ValueFloat     , Is.not_equal_to, tgt.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(endemeTable.ValueNumber    , Is.not_equal_to, tgt.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(endemeTable.ValueText      , Is.not_equal_to, tgt.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(endemeTable.EndemeProfileId, Is.not_equal_to, tgt.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeTable -->
        /// <summary>
        ///     Returns a list of EndemeTable objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeTable objects</returns>
        public List<EndemeTable> AtEndemeTable(RichDataTable table)
        {
            List<EndemeTable> list = new List<EndemeTable>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeTable(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeTable -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeTable objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeTable objects"></param>
        public List<EndemeTable> AtOnMareEndemeTable(RichDataTable table)
        {
            List<EndemeTable> endemeTableList = new List<EndemeTable>(table.Count);
            Dictionary<int,EndemeTable> found = new Dictionary<int,EndemeTable>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeId = table.IntValue(row, "EndemeId", -1);
                EndemeTable endemeTable = null;

                if (!found.ContainsKey(endemeId))
                {
                    endemeTable = OnEndemeTable(table, row);

                  //endemeTable.EndemeIndexList = new List<EndemeIndex>();
                  //endemeTable.EndemeSet   = (new EndemeSetAccess(ConnectionString)).OnEndemeSet(table, row);
                    endemeTableList.Add(endemeTable);
                    found.Add(endemeId, endemeTable);
                }
                else
                {
                    endemeTable = found[endemeId];
                }

              //endemeTable.EndemeIndexList.Add((new EndemeIndexAccess(ConnectionString)).OnEndemeIndex(table, row));
            }

            return endemeTableList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeTable -->
        /// <summary>
        ///      Copies an endemeTable
        /// </summary>
        /// <param name="endemeTable">endemeTable to copy</param>
        public static EndemeTable CpEndemeTable(EndemeTable endemeTable)
        {
            EndemeTable output = new EndemeTable();

            output.EndemeId        = endemeTable.EndemeId;
            output.EndemeSetId     = endemeTable.EndemeSetId;
            output.EndemeString    = endemeTable.EndemeString;
            output.EndemeCode      = endemeTable.EndemeCode;
            output.EndemeLabel     = endemeTable.EndemeLabel;
            output.EndemeDescr     = endemeTable.EndemeDescr;
            output.RawValue        = endemeTable.RawValue;
            output.MultipleUse     = endemeTable.MultipleUse;
            output.ValueBinary     = endemeTable.ValueBinary;
            output.ValueDateTime   = endemeTable.ValueDateTime;
            output.ValueFloat      = endemeTable.ValueFloat;
            output.ValueNumber     = endemeTable.ValueNumber;
            output.ValueText       = endemeTable.ValueText;
            output.EndemeProfileId = endemeTable.EndemeProfileId;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeTableCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the Endeme table
        /// </summary>
        /// <returns>a count of rows in the Endeme table</returns>
        public int EndemeTableCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT COUNT(*) FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = TreatAs.IntValue(cmd.ExecuteScalar(-1), -1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeTable: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EnEndemeTable -->
        /// <summary>
        ///      (En)ables an EndemeTable
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        public void EnEndemeTable(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_MAIN_TABLE+" SET EntryState = 1 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeTable -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endeme table
        /// </summary>
        /// <returns>Maximum Endeme.EndemeId</returns>
        public int HiIdOfEndemeTable(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT MAX(EndemeId) AS EndemeId WITH(NOLOCK) FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int endemeId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeTable: " + cmd.Errors);
            return endemeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTable -->
        /// <summary>
        ///      Returns the indicated row (In) the Endeme table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeTable(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId"  , endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndeme -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeId column
        /// </summary>
        /// <param name="endemeId">value in EndemeId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeId</returns>
        public RichDataTable InEndemeTableOfEndeme(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId" , endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueDateTime -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueDateTime column
        /// </summary>
        /// <param name="valueDateTime">value in ValueDateTime column</param>
        /// <returns>a table of rows related to the specifed value of ValueDateTime</returns>
        public RichDataTable InEndemeTableOfValueDateTime(DateTime valueDateTime, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE ValueDateTime = @ValueDateTime"
                , Throws.Actions, "PR")
                .AddParameter_date("@ValueDateTime" , valueDateTime, SqlDbType.DateTime);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf10EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueFloat -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueFloat column
        /// </summary>
        /// <param name="valueFloat">value in ValueFloat column</param>
        /// <returns>a table of rows related to the specifed value of ValueFloat</returns>
        public RichDataTable InEndemeTableOfValueFloat(double valueFloat, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE ValueFloat = @ValueFloat"
                , Throws.Actions, "PR")
                .AddParameter_null("@ValueFloat" , valueFloat);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf11EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueNumber -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueNumber column
        /// </summary>
        /// <param name="valueNumber">value in ValueNumber column</param>
        /// <returns>a table of rows related to the specifed value of ValueNumber</returns>
        public RichDataTable InEndemeTableOfValueNumber(decimal valueNumber, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE ValueNumber = @ValueNumber"
                , Throws.Actions, "PR")
                .AddParameter_null("@ValueNumber" , valueNumber);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf12EndemeTable: " + cmd.Errors);
            return table;







        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueText -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueText column
        /// </summary>
        /// <param name="valueText">value in ValueText column</param>
        /// <returns>a table of rows related to the specifed value of ValueText</returns>
        public RichDataTable InEndemeTableOfValueText(string valueText, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE ValueText = @ValueText"
                , Throws.Actions, "PR")
                .AddParameter_safe("@ValueText" , valueText);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf13EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeProfile -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeProfileId column
        /// </summary>
        /// <param name="endemeProfileId">value in EndemeProfileId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeProfileId</returns>
        public RichDataTable InEndemeTableOfEndemeProfile(int endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter_null("@EndemeProfileId" , endemeProfileId);
            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf14EndemeTable: " + cmd.Errors);
            return table;







        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeTableOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter_null("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeString -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeString column
        /// </summary>
        /// <param name="endemeString">value in EndemeString column</param>
        /// <returns>a table of rows related to the specifed value of EndemeString</returns>
        public RichDataTable InEndemeTableOfEndemeString(string endemeString, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeString = @EndemeString"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeString" , endemeString);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeCode -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeCode column
        /// </summary>
        /// <param name="endemeCode">value in EndemeCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCode</returns>
        public RichDataTable InEndemeTableOfEndemeCode(string endemeCode, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeCode = @EndemeCode"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeCode" , endemeCode);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeLabel -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeLabel column
        /// </summary>
        /// <param name="endemeLabel">value in EndemeLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeLabel</returns>
        public RichDataTable InEndemeTableOfEndemeLabel(string endemeLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeLabel = @EndemeLabel"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeLabel" , endemeLabel);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeTable: " + cmd.Errors);
            return table;







        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeDescr -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeDescr column
        /// </summary>
        /// <param name="endemeDescr">value in EndemeDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeDescr</returns>
        public RichDataTable InEndemeTableOfEndemeDescr(string endemeDescr, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeDescr = @EndemeDescr"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeDescr" , endemeDescr);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeTable: " + cmd.Errors);
            return table;







        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfRawValue -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the RawValue column
        /// </summary>
        /// <param name="rawValue">value in RawValue column</param>
        /// <returns>a table of rows related to the specifed value of RawValue</returns>
        public RichDataTable InEndemeTableOfRawValue(bool rawValue, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE RawValue = @RawValue"
                , Throws.Actions, "PR")
                .AddParameter_null("@RawValue" , rawValue);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeTable: " + cmd.Errors);
            return table;







        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfMultipleUse -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the MultipleUse column
        /// </summary>
        /// <param name="multipleUse">value in MultipleUse column</param>
        /// <returns>a table of rows related to the specifed value of MultipleUse</returns>
        public RichDataTable InEndemeTableOfMultipleUse(bool multipleUse, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE MultipleUse = @MultipleUse"
                , Throws.Actions, "PR")
                .AddParameter("@MultipleUse" , multipleUse);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf8EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueBinary -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueBinary column
        /// </summary>
        /// <param name="valueBinary">value in ValueBinary column</param>
        /// <returns>a table of rows related to the specifed value of ValueBinary</returns>
        public RichDataTable InEndemeTableOfValueBinary(byte[] valueBinary, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE ValueBinary = @ValueBinary"
                , Throws.Actions, "PR")
                .AddParameter_byte("@ValueBinary" , valueBinary);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf9EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeTable -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the Endeme table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeTable(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                    , "SELECT * FROM "+ENDEME_MAIN_TABLE+" WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                    , Throws.Actions, "P")
                    .AddParameter("@EndemeId", endemeId);


                using (RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeTable with EndemeId " + endemeId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }
            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeTable -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeTable
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of endemeTable rows with their joined parent data</returns>
        public RichDataTable MainEndemeTable(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, e.*"
                + "\r\n" + " FROM                "+ENDEME_MAIN_TABLE+" AS e  WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE +" AS es WITH(NOLOCK) ON es.EndemeSetId = e.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeTableOf -->
        /// <summary>
        ///      Returns a endemeTable list (Of) a p(a)rent endemeset with its endemeset data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeTable rows with their joined parent data</returns>
        public RichDataTable MainEndemeTableOf(int endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, e.*"
                + "\r\n" + " FROM                "+ENDEME_MAIN_TABLE+" AS e  WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE +" AS es WITH(NOLOCK) ON es.EndemeSetId = e.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeTableOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeTableOf -->
        /// <summary>
        ///      Returns a endemeTable list (Of) a p(a)rent endemeset with (re)trieved endemeTable and endemeindex data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeTable rows with their joined parent data</returns>
        public RichDataTable MareEndemeTableOf(int endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, e.*, ei.*"
                + "\r\n" + " FROM                "+ENDEME_MAIN_TABLE + " AS e WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE  + " AS es WITH(NOLOCK) ON es.EndemeSetId = e.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_INDEX_TABLE+ " AS ei WITH(NOLOCK) ON ei.EndemeId    = e.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeTableOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeTable -->
        /// <summary>
        ///      Creates a (Ne)w EndemeTable (o)bject from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawValue"       ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new EndemeTable object</returns>
        public EndemeTable NeonEndemeTable
            ( int      endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawValue
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            )
        {
            EndemeTable endemeTable = new EndemeTable
                { EndemeId        = endemeId
                , EndemeSetId     = endemeSetId
                , EndemeString    = endemeString
                , EndemeCode      = endemeCode
                , EndemeLabel     = endemeLabel
                , EndemeDescr     = endemeDescr
                , RawValue        = rawValue
                , MultipleUse     = multipleUse
                , ValueBinary     = valueBinary
                , ValueDateTime   = valueDateTime
                , ValueFloat      = valueFloat
                , ValueNumber     = valueNumber
                , ValueText       = valueText
                , EndemeProfileId = endemeProfileId
                };
            return endemeTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeTable -->
        /// <summary>
        ///      Converts a row in the Endeme data table into a EndemeTable (O)bject
        /// </summary>
        /// <param name="endemeTableTable">a table containing columns to build a batch object</param>
        /// <param name="row"             >the row to convert</param>
        /// <param name="dispose"         >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeTable OnEndemeTable(RichDataTable endemeTableTable, int row, bool dispose = false)
        {
            EndemeTable endemeTable = new EndemeTable();
            endemeTable.EndemeId        = endemeTableTable.IntValue (row, "EndemeId"       , -1          );
            endemeTable.EndemeSetId     = endemeTableTable.GuidValue(row, "EndemeSetId"    , Guid.Empty  );
            endemeTable.EndemeString    = endemeTableTable.StrValue (row, "EndemeString"   , ""          );
            endemeTable.EndemeCode      = endemeTableTable.StrValue (row, "EndemeCode"     , ""          );
            endemeTable.EndemeLabel     = endemeTableTable.StrValue (row, "EndemeLabel"    , ""          );
            endemeTable.EndemeDescr     = endemeTableTable.StrValue (row, "EndemeDescr"    , ""          );
            endemeTable.RawValue        = endemeTableTable.BoolValue(row, "RawValue"       , false       );
            endemeTable.MultipleUse     = endemeTableTable.BoolValue(row, "MultipleUse"    , false       );
            endemeTable.ValueBinary     = endemeTableTable.ByteValue(row, "ValueBinary"                  );
            endemeTable.ValueDateTime   = endemeTableTable.DateValue(row, "ValueDateTime"  , DateTime.Now);
            endemeTable.ValueFloat      = endemeTableTable.RealValue(row, "ValueFloat"     , -1.0        );
            endemeTable.ValueNumber     = endemeTableTable.DecValue (row, "ValueNumber"    , -1.0M       );
            endemeTable.ValueText       = endemeTableTable.StrValue (row, "ValueText"      , ""          );
            endemeTable.EndemeProfileId = endemeTableTable.IntValue (row, "EndemeProfileId", -1          );

            if (dispose) endemeTableTable.Dispose();
            return endemeTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeTable -->
        /// <summary>
        ///      Returns a table of a EndemeTable (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of EndemeTable rows with their joined details</returns>
        public RichDataTable ReinEndemeTable(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT e.*, ei.*"
                + "\r\n" + " FROM                "+ENDEME_MAIN_TABLE +" AS e WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_INDEX_TABLE+" AS ei WITH(NOLOCK) ON ei.EndemeId = e.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeTable -->
        /// <summary>
        ///      Inserts an endeme object in(To) the database
        /// </summary>
        /// <param name="endemeTable">endemeTable to insert into database</param>
        /// <returns>the Id of the inserted Endeme</returns>
        public int ToEndemeTable(EndemeTable endeme, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " INSERT INTO "+ENDEME_MAIN_TABLE
              //+ "\r\n" + "        ( EndemeId"
                + "\r\n" + "        ( EndemeSetId"
                + "\r\n" + "        , EndemeString"
                + "\r\n" + "        , EndemeCode"
                + "\r\n" + "        , EndemeLabel"
                + "\r\n" + "        , EndemeDescr"
                + "\r\n" + "        , RawValue"
                + "\r\n" + "        , MultipleUse"
                + "\r\n" + "        , ValueBinary"
                + "\r\n" + "        , ValueDateTime"
                + "\r\n" + "        , ValueFloat"
                + "\r\n" + "        , ValueNumber"
                + "\r\n" + "        , ValueText"
                + "\r\n" + "        , EndemeProfileId"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeId"
              //+ "\r\n" + " VALUES ( @EndemeId"
                + "\r\n" + " VALUES ( @EndemeSetId"
                + "\r\n" + "        , @EndemeString"
                + "\r\n" + "        , @EndemeCode"
                + "\r\n" + "        , @EndemeLabel"
                + "\r\n" + "        , @EndemeDescr"
                + "\r\n" + "        , @RawValue"
                + "\r\n" + "        , @MultipleUse"
                + "\r\n" + "        , @ValueBinary"
                + "\r\n" + "        , @ValueDateTime"
                + "\r\n" + "        , @ValueFloat"
                + "\r\n" + "        , @ValueNumber"
                + "\r\n" + "        , @ValueText"
                + "\r\n" + "        , @EndemeProfileId"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
              //.AddParameter     ("@EndemeId"       , endeme.EndemeId                           )
                .AddParameter_null("@EndemeSetId"    , endeme.EndemeSetId                        )
                .AddParameter_safe("@EndemeString"   , endeme.EndemeString   ,   24)
                .AddParameter_safe("@EndemeCode"     , endeme.EndemeCode     ,    8)
                .AddParameter_safe("@EndemeLabel"    , endeme.EndemeLabel    ,   64)
                .AddParameter_safe("@EndemeDescr"    , endeme.EndemeDescr    ,   -1)
                .AddParameter_null("@RawValue"       , endeme.RawValue                           )
                .AddParameter     ("@MultipleUse"    , endeme.MultipleUse                        )
                .AddParameter_byte("@ValueBinary"    , endeme.ValueBinary                        )
                .AddParameter_date("@ValueDateTime"  , endeme.ValueDateTime  , SqlDbType.DateTime)
                .AddParameter_null("@ValueFloat"     , endeme.ValueFloat                         )
                .AddParameter_null("@ValueNumber"    , endeme.ValueNumber                        )
                .AddParameter_safe("@ValueText"      , endeme.ValueText      ,   -1)
                .AddParameter_null("@EndemeProfileId", endeme.EndemeProfileId                    );


            int endemeId = TreatAs.IntValue(cmd.ExecuteScalar(-1),-2);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeTable: " + cmd.Errors);
            return endemeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeTable -->
        /// <summary>
        ///     Inserts in(To) the EndemeTable table a (ne)w endemeTable built from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawValue"       ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new EndemeTable object</returns>
        public EndemeTable ToneEndemeTable
            ( int      endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawValue
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            , InfoAspect aspect)
        {
            EndemeTable endemeTable = NeonEndemeTable
                ( endemeId
                , endemeSetId
                , endemeString
                , endemeCode
                , endemeLabel
                , endemeDescr
                , rawValue
                , multipleUse
                , valueBinary
                , valueDateTime
                , valueFloat
                , valueNumber
                , valueText
                , endemeProfileId
                );
            endemeTable.EndemeId = ToEndemeTable(endemeTable, aspect);
            return endemeTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeTable -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeTable
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <param name="disableValue">the value meaning diable</param>
        public void UnEndemeTable(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_MAIN_TABLE+" SET EntryState = 4 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeTable -->
        /// <summary>
        ///      (Up)dates a row in the Endeme table from a EndemeTable object
        /// </summary>
        /// <param name="endemeTable">endemeTable to update</param>
        /// <returns>the count of the updated endemeTable rows"></param>
        public int UpEndemeTable(EndemeTable endemeTable, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeId = " + endemeTable.EndemeId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(ENDEME_MAIN_TABLE, whereClause, aspect.MainConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                            , "\r\n" + " UPDATE "+ENDEME_MAIN_TABLE
                            + "\r\n" + " SET   EndemeSetId     = @EndemeSetId"
                            + "\r\n" + "     , EndemeString    = @EndemeString"
                            + "\r\n" + "     , EndemeCode      = @EndemeCode"
                            + "\r\n" + "     , EndemeLabel     = @EndemeLabel"
                            + "\r\n" + "     , EndemeDescr     = @EndemeDescr"
                            + "\r\n" + "     , RawValue        = @RawValue"
                            + "\r\n" + "     , MultipleUse     = @MultipleUse"
                            + "\r\n" + "     , ValueBinary     = @ValueBinary"
                            + "\r\n" + "     , ValueDateTime   = @ValueDateTime"
                            + "\r\n" + "     , ValueFloat      = @ValueFloat"
                            + "\r\n" + "     , ValueNumber     = @ValueNumber"
                            + "\r\n" + "     , ValueText       = @ValueText"
                            + "\r\n" + "     , EndemeProfileId = @EndemeProfileId"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            .AddParameter     ("@EndemeId"       , endemeTable.EndemeId                           )
                            .AddParameter_null("@EndemeSetId"    , endemeTable.EndemeSetId                        )
                            .AddParameter_safe("@EndemeString"   , endemeTable.EndemeString   ,   24)
                            .AddParameter_safe("@EndemeCode"     , endemeTable.EndemeCode     ,    8)
                            .AddParameter_safe("@EndemeLabel"    , endemeTable.EndemeLabel    ,   64)
                            .AddParameter_safe("@EndemeDescr"    , endemeTable.EndemeDescr    ,   -1)
                            .AddParameter_null("@RawValue"       , endemeTable.RawValue                           )
                            .AddParameter     ("@MultipleUse"    , endemeTable.MultipleUse                        )
                            .AddParameter_byte("@ValueBinary"    , endemeTable.ValueBinary                        )
                            .AddParameter_date("@ValueDateTime"  , endemeTable.ValueDateTime  , SqlDbType.DateTime)
                            .AddParameter_null("@ValueFloat"     , endemeTable.ValueFloat                         )
                            .AddParameter_null("@ValueNumber"    , endemeTable.ValueNumber                        )
                            .AddParameter_safe("@ValueText"      , endemeTable.ValueText      ,   -1)
                            .AddParameter_null("@EndemeProfileId", endemeTable.EndemeProfileId                    )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeTable: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeId " + endemeTable.EndemeId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeTable table

        #region EndemeCharacteristic table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeCharacteristic table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeCharacteristic> AtMareEndemeCharacteristicOf            (Guid                 endemeSetId         , InfoAspect aspect) { return AtOnMareEndemeCharacteristic(MareEndemeCharacteristicOf(endemeSetId, aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharLetter(char                 endemeCharLetter    , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharLetter(endemeCharLetter, aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeSet       (Guid                 endemeSetId         , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeSet       (endemeSetId     , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharCode  (string               endemeCharCode      , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharCode  (endemeCharCode  , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharLabel (string               endemeCharLabel     , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharLabel (endemeCharLabel , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharDescr (string               endemeCharDescr     , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharDescr (endemeCharDescr , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharIsASet(Guid                 endemeCharIsASet    , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharIsASet(endemeCharIsASet, aspect)); }
        public Atom                       AtomMaEndemeCharacteristicOf            (Guid                 endemesetId         , InfoAspect aspect) { return MainEndemeCharacteristicOf(endemesetId, aspect).ToAtom(); }
        public void                       ExEndemeCharacteristic                  (Guid   endemeSetId, char endemeCharLetter, InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM "+ENDEME_CHAR_TABLE+" WHERE EndemeSetId = '"+endemeSetId.ToString()+"' AND EndemeCharLetter = " + endemeCharLetter, aspect.MainConn); }
        public RichDataTable              IntoEndemeCharacteristic                (EndemeCharacteristic endemeCharacteristic, InfoAspect aspect) { if (   IsEndemeCharacteristic(endemeCharacteristic.EndemeCharLetter, aspect)) return InEndemeCharacteristic(endemeCharacteristic.EndemeSetId, endemeCharacteristic.EndemeCharLetter, aspect); else { ToEndemeCharacteristic(endemeCharacteristic, aspect); return InEndemeCharacteristic(endemeCharacteristic.EndemeSetId, endemeCharacteristic.EndemeCharLetter, aspect); } }
        public EndemeCharacteristic       OnInEndemeCharacteristic                (Guid   endemeSetId, char endemeCharLetter, InfoAspect aspect) { return OnEndemeCharacteristic(InEndemeCharacteristic  (endemeSetId, endemeCharLetter, aspect), 0, true); }
        public EndemeCharacteristic       OnIntoEndemeCharacteristic              (EndemeCharacteristic endemeCharacteristic, InfoAspect aspect) { return OnEndemeCharacteristic(IntoEndemeCharacteristic(endemeCharacteristic  , aspect), 0, true); }
        public int                        UpToEndemeCharacteristic                (EndemeCharacteristic endemeCharacteristic, InfoAspect aspect) { if (   IsEndemeCharacteristic(endemeCharacteristic.EndemeCharLetter, aspect)) return UpEndemeCharacteristic(endemeCharacteristic, aspect); else { ToEndemeCharacteristic(endemeCharacteristic, aspect); return 1; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeCharacteristic_test -->
        /// <summary>
        ///      Tests the ToEndemeCharacteristic method
        /// </summary>
        public void ToEndemeCharacteristic_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeCharacteristicAccess";
            string adoMethod = "ToEndemeCharacteristic";
            Random r = RandomSource.New().Random;
            char   endemeCharLetter = ' ';
            EndemeAccess ende = new EndemeAccess("");
            Guid   endemeSetId = Guid.Empty;


            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeCharacteristic endemeCharacteristicTo = ende.AnneEndemeCharacteristic(r);
            endemeSetId      = endemeCharacteristicTo.EndemeSetId;
            endemeCharLetter = endemeCharacteristicTo.EndemeCharLetter;
            try
            {
                ende.ToEndemeCharacteristic(endemeCharacteristicTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeCharLetter, Is.greater_than, -1, adoClass, adoMethod);
            EndemeCharacteristic endemeCharacteristicFrom = ende.OnInEndemeCharacteristic(endemeSetId, endemeCharLetter, _aspect);
            ende.AssertEqualContent(endemeCharacteristicFrom, endemeCharacteristicTo, adoClass, adoMethod);


            ende.ExEndemeCharacteristic(endemeSetId, endemeCharLetter, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeCharacteristic_test -->
        /// <summary>
        ///      Tests the UpEndemeCharacteristic method
        /// </summary>
        public void UpEndemeCharacteristic_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass                            = "EndemeCharacteristicAccess";
            string adoMethod                           = "UpEndemeCharacteristic";
            Random r                          = RandomSource.New().Random;
            EndemeCharacteristic endemeCharacteristic1 = null;

            EndemeAccess ende = new EndemeAccess("");
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeCharLetter with a newly created endemeCharacteristic
                // ----------------------------------------------------------------------
                List<object> id = ende.AnIdOfEndemeCharacteristic(_aspect);
                Guid    endemeSetId      = TreatAs.GuidValue(id[0], Guid.Empty);
                char    endemeCharLetter = TreatAs.CharValue(id[1], ' '       );
                endemeCharacteristic1                        = ende.OnInEndemeCharacteristic(endemeSetId, endemeCharLetter, _aspect);
                EndemeCharacteristic  endemeCharacteristic2  = ende.AnneEndemeCharacteristic(r);
                endemeCharacteristic2.EndemeCharLetter = endemeCharacteristic1.EndemeCharLetter;
                ende.UpEndemeCharacteristic(endemeCharacteristic2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeCharacteristic endemeCharacteristic3 = ende.OnInEndemeCharacteristic(endemeSetId, endemeCharLetter, _aspect);
                Assert.That(endemeCharacteristic3.EndemeCharLetter, Is.equal_to, endemeCharacteristic2.EndemeCharLetter, adoClass, adoMethod + " test update existing endemeCharacteristic");
                ende.AssertEqualContent  (endemeCharacteristic3, endemeCharacteristic2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeCharacteristic3, endemeCharacteristic1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeCharacteristic, did the update fail?
                // ----------------------------------------------------------------------
                EndemeCharacteristic  endemeCharacteristic4  = ende.AnneEndemeCharacteristic(r);
                endemeCharacteristic4.EndemeCharLetter       = ende.HiIdOfEndemeCharacteristic(_aspect);
                int                   count                  = ende.UpEndemeCharacteristic(endemeCharacteristic4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeCharacteristic");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeCharacteristic(endemeCharacteristic1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeCharacteristic -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeCharacteristic table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public List<object> AnIdOfEndemeCharacteristic(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT EndemeSetId, EndemeCharLetter FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeCharacteristicId = new RichDataTable(cmd, null, "EndemeCharacteristicId", "EndemeCharLetter"))
            {
                List<object> output = new List<object>(1);
                Random r = RandomSource.New().Random;
                if (endemeCharacteristicId.Count > 0)
                {
                    int row = r.Next(endemeCharacteristicId.Count);
                    output.Add(endemeCharacteristicId.GuidValue(row, "EndemeSetId"     , Guid.Empty));
                    output.Add(endemeCharacteristicId.CharValue(row, "EndemeCharLetter", ' '       ));
                    return output;
                }
                else return output;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeCharacteristic -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeCharacteristic object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeCharacteristic</returns>
        private EndemeCharacteristic AnneEndemeCharacteristic(Random r)
        {
            EndemeCharacteristic endemeCharacteristic = new EndemeCharacteristic
                { EndemeCharLetter     = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray()[r.Next(22)]
                , EndemeSetId          = Guid.NewGuid()
                , EndemeCharCode       = r.Next(10000).ToString()
                , EndemeCharLabel      = r.Next(10000).ToString()
                , EndemeCharDescr      = r.Next(10000).ToString()
                , EndemeCharIsASet     = Guid.NewGuid()
                };
            return endemeCharacteristic;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic being tested</param>
        /// <param name="tgt"      >endemeCharacteristic being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeCharacteristic endemeCharacteristic, EndemeCharacteristic tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeCharacteristic.EndemeCharCode      , Is.equal_to, tgt.EndemeCharCode      , adoClass, adoMethod + " EndemeCharCode"      );
            Assert.That(endemeCharacteristic.EndemeCharLabel     , Is.equal_to, tgt.EndemeCharLabel     , adoClass, adoMethod + " EndemeCharLabel"     );
            Assert.That(endemeCharacteristic.EndemeCharDescr     , Is.equal_to, tgt.EndemeCharDescr     , adoClass, adoMethod + " EndemeCharDescr"     );
            Assert.That(endemeCharacteristic.EndemeCharIsASet    , Is.equal_to, tgt.EndemeCharIsASet    , adoClass, adoMethod + " EndemeCharIsASet"    );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic being tested</param>
        /// <param name="tgt"      >endemeCharacteristic being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeCharacteristic endemeCharacteristic, EndemeCharacteristic tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeCharacteristic.EndemeCharCode      , Is.not_equal_to, tgt.EndemeCharCode      , adoClass, adoMethod + " EndemeCharCode"      );
            Assert.That(endemeCharacteristic.EndemeCharLabel     , Is.not_equal_to, tgt.EndemeCharLabel     , adoClass, adoMethod + " EndemeCharLabel"     );
            Assert.That(endemeCharacteristic.EndemeCharDescr     , Is.not_equal_to, tgt.EndemeCharDescr     , adoClass, adoMethod + " EndemeCharDescr"     );
            Assert.That(endemeCharacteristic.EndemeCharIsASet    , Is.not_equal_to, tgt.EndemeCharIsASet    , adoClass, adoMethod + " EndemeCharIsASet"    );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeCharacteristic -->
        /// <summary>
        ///     Returns a list of EndemeCharacteristic objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeCharacteristic objects</returns>
        public List<EndemeCharacteristic> AtEndemeCharacteristic(RichDataTable table)
        {
            List<EndemeCharacteristic> list = new List<EndemeCharacteristic>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeCharacteristic(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeCharacteristic -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeCharacteristic objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeCharacteristic objects"></param>
        public List<EndemeCharacteristic> AtOnMareEndemeCharacteristic(RichDataTable table)
        {
            List<EndemeCharacteristic> endemeCharacteristicList = new List<EndemeCharacteristic>(table.Count);
            Dictionary<int,EndemeCharacteristic> found = new Dictionary<int,EndemeCharacteristic>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeCharLetter = table.IntValue(row, "EndemeCharLetter", -1);
                EndemeCharacteristic endemeCharacteristic = null;

                if (!found.ContainsKey(endemeCharLetter))
                {
                    endemeCharacteristic = OnEndemeCharacteristic(table, row);

                    endemeCharacteristic.EndemeMeaningList = new List<EndemeMeaning>();
                  //endemeCharacteristic.EndemeSet     = (new EndemeAccess(ConnectionString)).OnEndemeSet(table, row);
                    endemeCharacteristicList.Add(endemeCharacteristic);
                    found.Add(endemeCharLetter, endemeCharacteristic);
                }
                else
                {
                    endemeCharacteristic = found[endemeCharLetter];
                }

              //endemeCharacteristic.EndemeMeaningList.Add((new EndemeAccess(ConnectionString)).OnEndemeMeaning(table, row));
            }

            return endemeCharacteristicList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeCharacteristic -->
        /// <summary>
        ///      Copies an endemeCharacteristic
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic to copy</param>
        public static EndemeCharacteristic CpEndemeCharacteristic(EndemeCharacteristic endemeCharacteristic)
        {
            EndemeCharacteristic output = new EndemeCharacteristic();

            output.EndemeCharLetter     = endemeCharacteristic.EndemeCharLetter;
            output.EndemeSetId          = endemeCharacteristic.EndemeSetId;
            output.EndemeCharCode       = endemeCharacteristic.EndemeCharCode;
            output.EndemeCharLabel      = endemeCharacteristic.EndemeCharLabel;
            output.EndemeCharDescr      = endemeCharacteristic.EndemeCharDescr;
            output.EndemeCharIsASet     = endemeCharacteristic.EndemeCharIsASet;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeCharacteristicCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeCharacteristic table
        /// </summary>
        /// <returns>a count of rows in the EndemeCharacteristic table</returns>
        public int EndemeCharacteristicCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT COUNT(*) FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = TreatAs.IntValue(cmd.ExecuteScalar(-1), -1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeCharacteristic: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EnEndemeCharacteristic -->
        /// <summary>
        ///      (En)ables an EndemeCharacteristic
        /// </summary>
        /// <param name="endemeCharLetter">the primary key</param>
        public void EnEndemeCharacteristic(Guid endemeSetId, char endemeCharLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , " UPDATE "+ENDEME_CHAR_TABLE+" SET MeaningActive = 1"
                + " WHERE EndemeSetId = @EndemeSetId AND EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId"     , endemeSetId     )
                .AddParameter("@EndemeCharLetter", endemeCharLetter);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeCharacteristic: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeCharacteristic -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemecharacteristic table
        /// </summary>
        /// <returns>Maximum EndemeCharacteristic.EndemeCharLetter</returns>
        public char HiIdOfEndemeCharacteristic(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT EndemeCharLetter WITH(NOLOCK) FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            char endemeCharLetter = TreatAs.CharValue(cmd.ExecuteScalar(""),' ');
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeCharacteristic: " + cmd.Errors);
            return endemeCharLetter;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristic -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeCharacteristic table
        /// </summary>
        /// <param name="endemeCharLetter">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeCharacteristic(Guid endemeSetId, char endemeCharLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , " SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK)"
                + " WHERE EndemeSetId = @EndemeSetId AND EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetIdr"    , endemeSetId)
                .AddParameter("@EndemeCharLetter", endemeCharLetter);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharLetter -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharLetter column
        /// </summary>
        /// <param name="endemeCharLetter">value in EndemeCharLetter column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharLetter</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharLetter(char endemeCharLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK) WHERE EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                .AddParameter_char("@EndemeCharLetter" , endemeCharLetter);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf1EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf2EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharCode -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharCode column
        /// </summary>
        /// <param name="endemeCharCode">value in EndemeCharCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharCode</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharCode(string endemeCharCode, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK) WHERE EndemeCharCode = @EndemeCharCode"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeCharCode" , endemeCharCode);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf3EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharLabel column
        /// </summary>
        /// <param name="endemeCharLabel">value in EndemeCharLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharLabel</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharLabel(string endemeCharLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK) WHERE EndemeCharLabel = @EndemeCharLabel"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeCharLabel" , endemeCharLabel);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf4EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharDescr -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharDescr column
        /// </summary>
        /// <param name="endemeCharDescr">value in EndemeCharDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharDescr</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharDescr(string endemeCharDescr, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK) WHERE EndemeCharDescr = @EndemeCharDescr"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeCharDescr" , endemeCharDescr);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf5EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharIsASet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharIsASet column
        /// </summary>
        /// <param name="endemeCharIsASet">value in EndemeCharIsASet column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharIsASet</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharIsASet(Guid endemeCharIsASet, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK) WHERE EndemeCharIsASet = @EndemeCharIsASet"
                , Throws.Actions, "PR")
                .AddParameter_null("@EndemeCharIsASet" , endemeCharIsASet);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf6EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeCharacteristic -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeCharacteristic table
        /// </summary>
        /// <param name="endemeCharLetter">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeCharacteristic(char endemeCharLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                    , "SELECT * FROM "+ENDEME_CHAR_TABLE+" WITH(NOLOCK) WHERE EndemeCharLetter = @EndemeCharLetter"
                    , Throws.Actions, "P")
                    .AddParameter("@EndemeCharLetter", endemeCharLetter);


                using (RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeCharLetter"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeCharacteristic with EndemeCharLetter " + endemeCharLetter.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }
            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeCharacteristic -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeCharacteristic
        /// </summary>
        /// <param name="endemeCharLetter">the primary key</param>
        /// <returns>a table of endemeCharacteristic rows with their joined parent data</returns>
        public RichDataTable MainEndemeCharacteristic(char endemeCharLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, ec.*"
                + "\r\n" + " FROM                "+ENDEME_CHAR_TABLE+" AS ec WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE +" AS es WITH(NOLOCK) ON es.EndemeSetId = ec.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ec.EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeCharLetter", endemeCharLetter);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeCharacteristicOf -->
        /// <summary>
        ///      Returns a endemeCharacteristic list (Of) a p(a)rent endemeset with its endemeset data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeCharacteristic rows with their joined parent data</returns>
        public RichDataTable MainEndemeCharacteristicOf(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, ec.*"
                + "\r\n" + " FROM                "+ENDEME_CHAR_TABLE+" AS ec WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE +" AS es WITH(NOLOCK) ON es.EndemeSetId = ec.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ec.EndemeSetId      = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter     ("@EndemeSetId"     , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeCharacteristicOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeCharacteristicOf -->
        /// <summary>
        ///      Returns a endemeCharacteristic list (Of) a p(a)rent endemeset with (re)trieved endemeCharacteristic and endememeaning data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeCharacteristic rows with their joined parent data</returns>
        public RichDataTable MareEndemeCharacteristicOf(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, ec.*, em.*"
                + "\r\n" + " FROM                "+ENDEME_CHAR_TABLE+" AS ec WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE +" AS es WITH(NOLOCK) ON es.EndemeSetId      = ec.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN dbo.EndemeMeaning        AS em WITH(NOLOCK) ON em.EndemeCharLetter = ec.EndemeCharLetter"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ec.EndemeSetId      = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter     ("@EndemeSetId"     , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeCharacteristicOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeCharacteristic -->
        /// <summary>
        ///      Creates a (Ne)w EndemeCharacteristic (o)bject from member values
        /// </summary>
        /// <param name="endemeCharLetter"    ></param>
        /// <param name="endemeSetId"         ></param>
        /// <param name="endemeCharCode"      ></param>
        /// <param name="endemeCharLabel"     ></param>
        /// <param name="endemeCharDescr"     ></param>
        /// <param name="endemeCharIsASet"    ></param>
        /// <returns>the new EndemeCharacteristic object</returns>
        public EndemeCharacteristic NeonEndemeCharacteristic
            ( char     endemeCharLetter
            , Guid     endemeSetId
            , string   endemeCharCode
            , string   endemeCharLabel
            , string   endemeCharDescr
            , Guid     endemeCharIsASet
            )
        {
            EndemeCharacteristic endemeCharacteristic = new EndemeCharacteristic
                { EndemeCharLetter     = endemeCharLetter
                , EndemeSetId          = endemeSetId
                , EndemeCharCode       = endemeCharCode
                , EndemeCharLabel      = endemeCharLabel
                , EndemeCharDescr      = endemeCharDescr
                , EndemeCharIsASet     = endemeCharIsASet
                };
            return endemeCharacteristic;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeCharacteristic -->
        /// <summary>
        ///      Converts a row in the EndemeCharacteristic data table into a EndemeCharacteristic (O)bject
        /// </summary>
        /// <param name="endemeCharacteristicTable">a table containing columns to build a batch object</param>
        /// <param name="row"                      >the row to convert</param>
        /// <param name="dispose"                  >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeCharacteristic OnEndemeCharacteristic(RichDataTable endemeCharacteristicTable, int row, bool dispose = false)
        {
            EndemeCharacteristic endemeCharacteristic = new EndemeCharacteristic();
            endemeCharacteristic.EndemeCharLetter     = endemeCharacteristicTable.CharValue(row, "EndemeCharLetter"    , ' '         );
            endemeCharacteristic.EndemeSetId          = endemeCharacteristicTable.GuidValue(row, "EndemeSetId"         , Guid.Empty  );
            endemeCharacteristic.EndemeCharCode       = endemeCharacteristicTable.StrValue (row, "EndemeCharCode"      , ""          );
            endemeCharacteristic.EndemeCharLabel      = endemeCharacteristicTable.StrValue (row, "EndemeCharLabel"     , ""          );
            endemeCharacteristic.EndemeCharDescr      = endemeCharacteristicTable.StrValue (row, "EndemeCharDescr"     , ""          );
            endemeCharacteristic.EndemeCharIsASet     = endemeCharacteristicTable.GuidValue(row, "EndemeCharIsASet"    , Guid.Empty  );

            if (dispose) endemeCharacteristicTable.Dispose();
            return endemeCharacteristic;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeCharacteristic -->
        /// <summary>
        ///      Returns a table of a EndemeCharacteristic (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeCharLetter">the primary key</param>
        /// <returns>a table of EndemeCharacteristic rows with their joined details</returns>
        public RichDataTable ReinEndemeCharacteristic(char endemeCharLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT ec.*, em.*"
                + "\r\n" + " FROM                "+ENDEME_CHAR_TABLE+" AS ec WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN dbo.EndemeMeaning        AS em WITH(NOLOCK) ON em.EndemeCharLetter = ec.EndemeCharLetter"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ec.EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                .AddParameter_char("@EndemeCharLetter", endemeCharLetter);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeCharacteristic -->
        /// <summary>
        ///      Inserts an endemeCharacteristic object in(To) the database
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic to insert into database</param>
        /// <returns>the Id of the inserted EndemeCharacteristic</returns>
        public void ToEndemeCharacteristic(EndemeCharacteristic endemeCharacteristic, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " INSERT INTO "+ENDEME_CHAR_TABLE
                + "\r\n" + "        ( EndemeCharLetter"
                + "\r\n" + "        , EndemeSetId"
                + "\r\n" + "        , EndemeCharCode"
                + "\r\n" + "        , EndemeCharLabel"
                + "\r\n" + "        , EndemeCharDescr"
                + "\r\n" + "        , EndemeCharIsASet"
                + "\r\n" + "        )"
                + "\r\n" + " VALUES ( @EndemeCharLetter"
                + "\r\n" + "        , @EndemeSetId"
                + "\r\n" + "        , @EndemeCharCode"
                + "\r\n" + "        , @EndemeCharLabel"
                + "\r\n" + "        , @EndemeCharDescr"
                + "\r\n" + "        , @EndemeCharIsASet"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                .AddParameter_char("@EndemeCharLetter"    , endemeCharacteristic.EndemeCharLetter                        )
                .AddParameter     ("@EndemeSetId"         , endemeCharacteristic.EndemeSetId                             )
                .AddParameter_safe("@EndemeCharCode"      , endemeCharacteristic.EndemeCharCode      ,    8)
                .AddParameter_safe("@EndemeCharLabel"     , endemeCharacteristic.EndemeCharLabel     ,   64)
                .AddParameter_safe("@EndemeCharDescr"     , endemeCharacteristic.EndemeCharDescr     , 4096)
                .AddParameter_null("@EndemeCharIsASet"    , endemeCharacteristic.EndemeCharIsASet                        );


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeCharacteristic: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeCharacteristic -->
        /// <summary>
        ///     Inserts in(To) the EndemeCharacteristic table a (ne)w endemeCharacteristic built from member values
        /// </summary>
        /// <param name="endemeCharLetter"    ></param>
        /// <param name="endemeSetId"         ></param>
        /// <param name="endemeCharCode"      ></param>
        /// <param name="endemeCharLabel"     ></param>
        /// <param name="endemeCharDescr"     ></param>
        /// <param name="endemeCharIsASet"    ></param>
        /// <returns>the new EndemeCharacteristic object</returns>
        public EndemeCharacteristic ToneEndemeCharacteristic
            ( char     endemeCharLetter
            , Guid     endemeSetId
            , string   endemeCharCode
            , string   endemeCharLabel
            , string   endemeCharDescr
            , Guid     endemeCharIsASet
            , InfoAspect aspect)
        {
            EndemeCharacteristic endemeCharacteristic = NeonEndemeCharacteristic
                ( endemeCharLetter
                , endemeSetId
                , endemeCharCode
                , endemeCharLabel
                , endemeCharDescr
                , endemeCharIsASet
                );
            ToEndemeCharacteristic(endemeCharacteristic, aspect);
            return endemeCharacteristic;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeCharacteristic -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeCharacteristic
        /// </summary>
        /// <param name="endemeCharLetter">the primary key</param>
        /// <param name="disableValue">the value meaning diable</param>
        public void UnEndemeCharacteristic(char endemeCharLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_CHAR_TABLE+" SET MeaningActive = 0 WHERE EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeCharLetter", endemeCharLetter);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeCharacteristic: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeCharacteristic -->
        /// <summary>
        ///      (Up)dates a row in the EndemeCharacteristic table from a EndemeCharacteristic object
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic to update</param>
        /// <returns>the count of the updated endemeCharacteristic rows"></param>
        public int UpEndemeCharacteristic(EndemeCharacteristic endemeCharacteristic, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeCharLetter = " + endemeCharacteristic.EndemeCharLetter.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(ENDEME_CHAR_TABLE, whereClause, aspect.MainConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                            , "\r\n" + " UPDATE "+ENDEME_CHAR_TABLE
                            + "\r\n" + " SET   EndemeSetId          = @EndemeSetId"
                            + "\r\n" + "     , EndemeCharCode       = @EndemeCharCode"
                            + "\r\n" + "     , EndemeCharLabel      = @EndemeCharLabel"
                            + "\r\n" + "     , EndemeCharDescr      = @EndemeCharDescr"
                            + "\r\n" + "     , EndemeCharIsASet     = @EndemeCharIsASet"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            .AddParameter_char("@EndemeCharLetter", endemeCharacteristic.EndemeCharLetter      )
                            .AddParameter     ("@EndemeSetId"     , endemeCharacteristic.EndemeSetId           )
                            .AddParameter_safe("@EndemeCharCode"  , endemeCharacteristic.EndemeCharCode  ,    8)
                            .AddParameter_safe("@EndemeCharLabel" , endemeCharacteristic.EndemeCharLabel ,   64)
                            .AddParameter_safe("@EndemeCharDescr" , endemeCharacteristic.EndemeCharDescr , 4096)
                            .AddParameter_null("@EndemeCharIsASet", endemeCharacteristic.EndemeCharIsASet      )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException(cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeCharacteristicId " + endemeCharacteristic.EndemeCharLetter.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeCharacteristic table

        #region EndemeIndex table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeIndex table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeIndex> AtMareEndemeIndexOf             (int                  endemetableId      , InfoAspect aspect) { return AtOnMareEndemeIndex(MareEndemeIndexOf(endemetableId, aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfEndemeKey        (int                  endemeKeyId        , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfEndemeKey          (endemeKeyId        , aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfEndemeLarge      (int                  endemeLargeId      , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfEndemeLarge        (endemeLargeId      , aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfMatchStrength    (int                  matchStrength      , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfMatchStrength      (matchStrength      , aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfDocumentTransform(int                  documentTransformId, InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfDocumentTransform  (documentTransformId, aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfEndemeSet        (Guid                 endemeSetId        , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfEndemeSet          (endemeSetId        , aspect)); }
        public Atom              AtomMaEndemeIndexOf             (int                  endemetableId      , InfoAspect aspect) { return MainEndemeIndexOf(endemetableId, aspect).ToAtom(); }
        public void              ExEndemeIndex                   (int endemeKeyId, int endemeLargeId      , InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM "+ENDEME_INDEX_TABLE+" WHERE EndemeKeyId = " + endemeKeyId + " AND EndemeLargeId = " + endemeLargeId, aspect.MainConn); }
        public RichDataTable     IntoEndemeIndex                 (EndemeIndex           endemeIndex       , InfoAspect aspect) { if (   IsEndemeIndex(endemeIndex.EndemeKeyId, endemeIndex.EndemeLargeId, aspect)) return InEndemeIndex(endemeIndex.EndemeKeyId, endemeIndex.EndemeLargeId, aspect); else { return InEndemeIndex(ToEndemeIndex(endemeIndex, aspect), aspect); } }
        public EndemeIndex       OnInEndemeIndex                 (int endemeKeyId, int endemeLargeId      , InfoAspect aspect) { return OnEndemeIndex(InEndemeIndex  (endemeKeyId, endemeLargeId, aspect), 0, true); }
        public EndemeIndex       OnIntoEndemeIndex               (EndemeIndex           endemeIndex       , InfoAspect aspect) { return OnEndemeIndex(IntoEndemeIndex(endemeIndex  , aspect), 0, true); }
        public int               UpToEndemeIndex                 (EndemeIndex           endemeIndex       , InfoAspect aspect) { if (   IsEndemeIndex(endemeIndex.EndemeKeyId, endemeIndex.EndemeLargeId, aspect)) return UpEndemeIndex(endemeIndex, aspect); else { ToEndemeIndex(endemeIndex, aspect); return 1; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeIndex_test -->
        /// <summary>
        ///      Tests the ToEndemeIndex method
        /// </summary>
        public void ToEndemeIndex_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeIndexAccess";
            string adoMethod = "ToEndemeIndex";
            Random r = RandomSource.New().Random;
            List<int> endemeIndexId = new List<int>();
            EndemeAccess ende = new EndemeAccess("");


            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeIndex endemeIndexTo = ende.AnneEndemeIndex(r);
            try
            {
                endemeIndexId = ende.ToEndemeIndex(endemeIndexTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeIndexId[0], Is.greater_than, -1, adoClass, adoMethod);
            EndemeIndex endemeIndexFrom = ende.OnInEndemeIndex(endemeIndexId[0], endemeIndexId[1], _aspect);
            ende.AssertEqualContent(endemeIndexFrom, endemeIndexTo, adoClass, adoMethod);


            ende.ExEndemeIndex(endemeIndexId[0], endemeIndexId[1], _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeIndex_test -->
        /// <summary>
        ///      Tests the UpEndemeIndex method
        /// </summary>
        public void UpEndemeIndex_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass          = "EndemeIndexAccess";
            string adoMethod         = "UpEndemeIndex";
            Random r                 = RandomSource.New().Random;
            EndemeIndex endemeIndex1 = null;

            EndemeAccess ende = new EndemeAccess("");
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeKeyId with a newly created endemeIndex
                // ----------------------------------------------------------------------
                List<int>    endemeKeyId  = ende.AnIdOfEndemeIndex(_aspect);
                endemeIndex1              = ende.OnInEndemeIndex(endemeKeyId[0], endemeKeyId[1], _aspect);
                EndemeIndex  endemeIndex2 = ende.AnneEndemeIndex(r);
                endemeIndex2.EndemeKeyId  = endemeIndex1.EndemeKeyId;
                ende.UpEndemeIndex(endemeIndex2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeIndex endemeIndex3 = ende.OnInEndemeIndex(endemeKeyId[0], endemeKeyId[1], _aspect);
                Assert.That(endemeIndex3.EndemeKeyId, Is.equal_to, endemeIndex2.EndemeKeyId, adoClass, adoMethod + " test update existing endemeIndex");
                ende.AssertEqualContent  (endemeIndex3, endemeIndex2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeIndex3, endemeIndex1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeIndex, did the update fail?
                // ----------------------------------------------------------------------
                EndemeIndex  endemeIndex4  = ende.AnneEndemeIndex(r);
                endemeIndex4.EndemeKeyId   = ende.HiIdOfEndemeKey  (_aspect) + 1;
                endemeIndex4.EndemeLargeId = ende.HiIdOfEndemeIndexLarge(_aspect) + 1;
                int          count         = ende.UpEndemeIndex(endemeIndex4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeIndex");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeIndex(endemeIndex1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeIndex -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeIndex table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public List<int> AnIdOfEndemeIndex(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT TOP 1000000 EndemeKeyId, EndemeLargeId FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) ORDER BY EndemeLargeId DESC"
                , Throws.Actions, "P");


            using (RichDataTable endemeIndex = new RichDataTable(cmd, null, "EndemeIndexId", "EndemeKeyId, EndemeLargeId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeIndex.Count > 0)
                {
                    int row = r.Next(endemeIndex.Count);
                    List<int> endemeIndexId = new List<int>(2);
                    endemeIndexId.Add(endemeIndex.IntValue(row, "EndemeKeyId"  , -1));
                    endemeIndexId.Add(endemeIndex.IntValue(row, "EndemeLargeId", -1));
                    return endemeIndexId;
                }
                else return new List<int>();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeIndex -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeIndex object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeIndex</returns>
        private EndemeIndex AnneEndemeIndex(Random r)
        {
            EndemeIndex endemeIndex = new EndemeIndex
                { EndemeKeyId         = HiIdOfEndemeKey  (_aspect) + 1
                , EndemeLargeId       = HiIdOfEndemeIndexLarge(_aspect) + 1
                , MatchStrength       = r.Next(10000)
                , DocumentTransformId = r.Next(10000)
                , EndemeSetId         = Guid.NewGuid()
                };
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeIndex">endemeIndex being tested</param>
        /// <param name="tgt"      >endemeIndex being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeIndex endemeIndex, EndemeIndex tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeIndex.MatchStrength      , Is.equal_to, tgt.MatchStrength      , adoClass, adoMethod + " MatchStrength"      );
            Assert.That(endemeIndex.DocumentTransformId, Is.equal_to, tgt.DocumentTransformId, adoClass, adoMethod + " DocumentTransformId");
            Assert.That(endemeIndex.EndemeSetId        , Is.equal_to, tgt.EndemeSetId        , adoClass, adoMethod + " EndemeSetId"        );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeIndex">endemeIndex being tested</param>
        /// <param name="tgt"      >endemeIndex being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeIndex endemeIndex, EndemeIndex tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeIndex.MatchStrength      , Is.not_equal_to, tgt.MatchStrength      , adoClass, adoMethod + " MatchStrength"      );
            Assert.That(endemeIndex.DocumentTransformId, Is.not_equal_to, tgt.DocumentTransformId, adoClass, adoMethod + " DocumentTransformId");
            Assert.That(endemeIndex.EndemeSetId        , Is.not_equal_to, tgt.EndemeSetId        , adoClass, adoMethod + " EndemeSetId"        );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeIndex -->
        /// <summary>
        ///     Returns a list of EndemeIndex objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeIndex objects</returns>
        public List<EndemeIndex> AtEndemeIndex(RichDataTable table)
        {
            List<EndemeIndex> list = new List<EndemeIndex>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeIndex(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeIndex -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeIndex objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeIndex objects"></param>
        public List<EndemeIndex> AtOnMareEndemeIndex(RichDataTable table)
        {
            List<EndemeIndex> endemeIndexList = new List<EndemeIndex>(table.Count);
            Dictionary<string,EndemeIndex> found = new Dictionary<string,EndemeIndex>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeKeyId   = table.IntValue(row, "EndemeKeyId"  , -1);
                int endemeLargeId = table.IntValue(row, "EndemeLargeId", -1);
                EndemeIndex endemeIndex = null;

                if (!found.ContainsKey(endemeKeyId + "-" + endemeLargeId))
                {
                    endemeIndex = OnEndemeIndex(table, row);

                    endemeIndex.EndemeLargeList = new List<EndemeLarge>();
                    endemeIndex.EndemeTable = (new EndemeAccess(ConnectionString)).OnEndemeTable(table, row);
                    endemeIndexList.Add(endemeIndex);
                    found.Add(endemeKeyId + "-" + endemeLargeId, endemeIndex);
                }
                else
                {
                    endemeIndex = found[endemeKeyId + "-" + endemeLargeId];
                }

                endemeIndex.EndemeLargeList.Add((new EndemeAccess(ConnectionString)).OnEndemeLarge(table, row));
            }

            return endemeIndexList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeIndex -->
        /// <summary>
        ///      Copies an endemeIndex
        /// </summary>
        /// <param name="endemeIndex">endemeIndex to copy</param>
        public static EndemeIndex CpEndemeIndex(EndemeIndex endemeIndex)
        {
            EndemeIndex output = new EndemeIndex();

            output.EndemeKeyId         = endemeIndex.EndemeKeyId;
            output.EndemeLargeId       = endemeIndex.EndemeLargeId;
            output.MatchStrength       = endemeIndex.MatchStrength;
            output.DocumentTransformId = endemeIndex.DocumentTransformId;
            output.EndemeSetId         = endemeIndex.EndemeSetId;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeIndexCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeIndex table
        /// </summary>
        /// <returns>a count of rows in the EndemeIndex table</returns>
        public int EndemeIndexCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT COUNT(*) FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = TreatAs.IntValue(cmd.ExecuteScalar(-1), -1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeIndex: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeKey -->
        /// <summary>
        ///      Returns the (Hi)ghest Key (Id) (Of) the endemeindex table
        /// </summary>
        /// <returns>Maximum EndemeIndex.EndemeKeyId</returns>
        public int HiIdOfEndemeKey(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT MAX(EndemeKeyId) AS EndemeKeyId WITH(NOLOCK) FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int endemeKeyId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeIndexKey: " + cmd.Errors);
            return endemeKeyId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeLarge -->
        /// <summary>
        ///      Returns the (Hi)ghest Large (Id) (Of) the endemeindex table
        /// </summary>
        /// <returns>Maximum EndemeIndex.EndemeKeyId</returns>
        public int HiIdOfEndemeIndexLarge(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT MAX(EndemeLargeId) AS EndemeLargeId WITH(NOLOCK) FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int endemeLargeId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeLarge: " + cmd.Errors);
            return endemeLargeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndex -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeIndex table
        /// </summary>
        /// <param name="endemeKeyId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeIndex(int endemeKeyId, int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) WHERE EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeKeyId"  , endemeKeyId  )
                .AddParameter("@EndemeLargeId", endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeIndex: " + cmd.Errors);
            return table;
        }
        public RichDataTable InEndemeIndex(List<int> endemeIndexId, InfoAspect aspect) { return InEndemeIndex(endemeIndexId[0], endemeIndexId[1], aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfEndemeKey -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the EndemeKeyId column
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>a table of rows related to the specifed value of EndemeKeyId</returns>
        public RichDataTable InEndemeIndexOfEndemeKey(int endemeKeyId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) WHERE EndemeKeyId = @EndemeKeyId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeKeyId" , endemeKeyId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfEndemeLarge -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the EndemeLargeId column
        /// </summary>
        /// <param name="endemeLargeId">value in EndemeLargeId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeLargeId</returns>
        public RichDataTable InEndemeIndexOfEndemeLarge(int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) WHERE EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeLargeId" , endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfMatchStrength -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the MatchStrength column
        /// </summary>
        /// <param name="matchStrength">value in MatchStrength column</param>
        /// <returns>a table of rows related to the specifed value of MatchStrength</returns>
        public RichDataTable InEndemeIndexOfMatchStrength(int matchStrength, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) WHERE MatchStrength = @MatchStrength"
                , Throws.Actions, "PR")
                .AddParameter_null("@MatchStrength" , matchStrength);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfDocumentTransform -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the DocumentTransformId column
        /// </summary>
        /// <param name="documentTransformId">value in DocumentTransformId column</param>
        /// <returns>a table of rows related to the specifed value of DocumentTransformId</returns>
        public RichDataTable InEndemeIndexOfDocumentTransform(int documentTransformId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) WHERE DocumentTransformId = @DocumentTransformId"
                , Throws.Actions, "PR")
                .AddParameter_null("@DocumentTransformId" , documentTransformId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeIndexOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter_null("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeIndex -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeIndex table
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeIndex(int endemeKeyId, int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                    , "SELECT * FROM "+ENDEME_INDEX_TABLE+" WITH(NOLOCK) WHERE EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                    , Throws.Actions, "P")
                    .AddParameter("@EndemeKeyId"  , endemeKeyId)
                    .AddParameter("@EndemeLargeId", endemeLargeId);


                using (RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeKeyId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeIndex with EndemeKeyId " + endemeKeyId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }
            return present;
        }
        public bool IsEndemeIndex(List<int> endemeIndexId, InfoAspect aspect) { return IsEndemeIndex(endemeIndexId[0], endemeIndexId[1], aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeIndex -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeIndex
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>a table of endemeIndex rows with their joined parent data</returns>
        public RichDataTable MainEndemeIndex(int endemeKeyId, int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT e.*, ei.*"
                + "\r\n" + " FROM                "+ENDEME_INDEX_TABLE+" AS ei WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_MAIN_TABLE +" AS e  WITH(NOLOCK) ON e.EndemeId = ei.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ei.EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeKeyId"  , endemeKeyId)
                .AddParameter("@EndemeLargeId", endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeIndex: " + cmd.Errors);
            return table;
        }
        public RichDataTable MainEndemeIndex(List<int> endemeIndexId, InfoAspect aspect) { return MainEndemeIndex(endemeIndexId[0], endemeIndexId[1], aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeIndexOf -->
        /// <summary>
        ///      Returns a endemeIndex list (Of) a p(a)rent endemetable with its endemetable data
        /// </summary>
        /// <param name="endemetableId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeIndex rows with their joined parent data</returns>
        public RichDataTable MainEndemeIndexOf(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT e.*, ei.*"
                + "\r\n" + " FROM                "+ENDEME_INDEX_TABLE+" AS ei WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_MAIN_TABLE +" AS e  WITH(NOLOCK) ON e.EndemeId = ei.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ei.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeKeyId, EndemeLargeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeIndexOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeIndexOf -->
        /// <summary>
        ///      Returns a endemeIndex list (Of) a p(a)rent endemetable with (re)trieved endemeIndex and endemelarge data
        /// </summary>
        /// <param name="endemetableId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeIndex rows with their joined parent data</returns>
        public RichDataTable MareEndemeIndexOf(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT e.*, ei.*, el.*"
                + "\r\n" + " FROM                "+ENDEME_INDEX_TABLE+" AS ei WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_MAIN_TABLE +" AS e  WITH(NOLOCK) ON e .EndemeId    = ei.EndemeId"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_LARGE_TABLE+" AS el WITH(NOLOCK) ON el.EndemeKeyId = ei.EndemeKeyId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ei.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeIndexOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeIndex -->
        /// <summary>
        ///      Creates a (Ne)w EndemeIndex (o)bject from member values
        /// </summary>
        /// <param name="endemeKeyId"        ></param>
        /// <param name="endemeLargeId"      ></param>
        /// <param name="matchStrength"      ></param>
        /// <param name="documentTransformId"></param>
        /// <param name="endemeSetId"        ></param>
        /// <returns>the new EndemeIndex object</returns>
        public EndemeIndex NeonEndemeIndex
            ( int      endemeKeyId
            , int      endemeLargeId
            , int      matchStrength
            , int      documentTransformId
            , Guid     endemeSetId
            )
        {
            EndemeIndex endemeIndex = new EndemeIndex
                { EndemeKeyId         = endemeKeyId
                , EndemeLargeId       = endemeLargeId
                , MatchStrength       = matchStrength
                , DocumentTransformId = documentTransformId
                , EndemeSetId         = endemeSetId
                };
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeIndex -->
        /// <summary>
        ///      Converts a row in the EndemeIndex data table into a EndemeIndex (O)bject
        /// </summary>
        /// <param name="endemeIndexTable">a table containing columns to build a batch object</param>
        /// <param name="row"             >the row to convert</param>
        /// <param name="dispose"         >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeIndex OnEndemeIndex(RichDataTable endemeIndexTable, int row, bool dispose = false)
        {
            EndemeIndex endemeIndex = new EndemeIndex();
            endemeIndex.EndemeKeyId         = endemeIndexTable.IntValue (row, "EndemeKeyId"        , -1        );
            endemeIndex.EndemeLargeId       = endemeIndexTable.IntValue (row, "EndemeLargeId"      , -1        );
            endemeIndex.MatchStrength       = endemeIndexTable.IntValue (row, "MatchStrength"      , -1        );
            endemeIndex.DocumentTransformId = endemeIndexTable.IntValue (row, "DocumentTransformId", -1        );
            endemeIndex.EndemeSetId         = endemeIndexTable.GuidValue(row, "EndemeSetId"        , Guid.Empty);

            if (dispose) endemeIndexTable.Dispose();
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeIndex -->
        /// <summary>
        ///      Returns a table of a EndemeIndex (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>a table of EndemeIndex rows with their joined details</returns>
        public RichDataTable ReinEndemeIndex(int endemeKeyId, int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT ei.*, el.*"
                + "\r\n" + " FROM                "+ENDEME_INDEX_TABLE+" AS ei WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_LARGE_TABLE+" AS el WITH(NOLOCK) ON el.EndemeKeyId = ei.EndemeKeyId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ei.EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeKeyId"  , endemeKeyId  )
                .AddParameter("@EndemeLargeId", endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeIndex: " + cmd.Errors);
            return table;
        }
        public RichDataTable ReinEndemeIndex(List<int> endemeIndexId, InfoAspect aspect) { return ReinEndemeIndex(endemeIndexId[0], endemeIndexId[1], aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeIndex -->
        /// <summary>
        ///      Inserts an endemeIndex object in(To) the database
        /// </summary>
        /// <param name="endemeIndex">endemeIndex to insert into database</param>
        /// <returns>the Id of the inserted EndemeIndex</returns>
        public List<int> ToEndemeIndex(EndemeIndex endemeIndex, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " INSERT INTO "+ENDEME_INDEX_TABLE
                + "\r\n" + "        ( EndemeKeyId"
                + "\r\n" + "        , EndemeLargeId"
                + "\r\n" + "        , MatchStrength"
                + "\r\n" + "        , DocumentTransformId"
                + "\r\n" + "        , EndemeSetId"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeKeyId"
                + "\r\n" + " VALUES ( @EndemeKeyId"
                + "\r\n" + "        , @EndemeLargeId"
                + "\r\n" + "        , @MatchStrength"
                + "\r\n" + "        , @DocumentTransformId"
                + "\r\n" + "        , @EndemeSetId"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                .AddParameter     ("@EndemeKeyId"        , endemeIndex.EndemeKeyId        )
                .AddParameter     ("@EndemeLargeId"      , endemeIndex.EndemeLargeId      )
                .AddParameter_null("@MatchStrength"      , endemeIndex.MatchStrength      )
                .AddParameter_null("@DocumentTransformId", endemeIndex.DocumentTransformId)
                .AddParameter_null("@EndemeSetId"        , endemeIndex.EndemeSetId        );


            int endemeIndexId = TreatAs.IntValue(cmd.ExecuteScalar(-1),-2);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeIndex: " + cmd.Errors);
            List<int> id = new List<int>();
            id.Add(endemeIndex.EndemeKeyId);
            id.Add(endemeIndex.EndemeLargeId);
            return id;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeIndex -->
        /// <summary>
        ///     Inserts in(To) the EndemeIndex table a (ne)w endemeIndex built from member values
        /// </summary>
        /// <param name="endemeKeyId"        ></param>
        /// <param name="endemeLargeId"      ></param>
        /// <param name="matchStrength"      ></param>
        /// <param name="documentTransformId"></param>
        /// <param name="endemeSetId"        ></param>
        /// <returns>the new EndemeIndex object</returns>
        public EndemeIndex ToneEndemeIndex
            ( int      endemeKeyId
            , int      endemeLargeId
            , int      matchStrength
            , int      documentTransformId
            , Guid     endemeSetId
            , InfoAspect aspect)
        {
            EndemeIndex endemeIndex = NeonEndemeIndex
                ( endemeKeyId
                , endemeLargeId
                , matchStrength
                , documentTransformId
                , endemeSetId
                );
            ToEndemeIndex(endemeIndex, aspect);
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeIndex -->
        /// <summary>
        ///      (Up)dates a row in the EndemeIndex table from a EndemeIndex object
        /// </summary>
        /// <param name="endemeIndex">endemeIndex to update</param>
        /// <returns>the count of the updated endemeIndex rows"></param>
        public int UpEndemeIndex(EndemeIndex endemeIndex, InfoAspect aspect)
        {
            string whereClause
                = " WHERE   EndemeKeyId   = " + endemeIndex.EndemeKeyId.ToString()
                + "     AND EndemeLargeId = " + endemeIndex.EndemeLargeId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(ENDEME_INDEX_TABLE, whereClause, aspect.MainConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                            , "\r\n" + " UPDATE "+ENDEME_INDEX_TABLE
                            + "\r\n" + " SET   EndemeLargeId       = @EndemeLargeId"
                            + "\r\n" + "     , MatchStrength       = @MatchStrength"
                            + "\r\n" + "     , DocumentTransformId = @DocumentTransformId"
                            + "\r\n" + "     , EndemeSetId         = @EndemeSetId"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            .AddParameter     ("@EndemeKeyId"        , endemeIndex.EndemeKeyId        )
                            .AddParameter     ("@EndemeLargeId"      , endemeIndex.EndemeLargeId      )
                            .AddParameter_null("@MatchStrength"      , endemeIndex.MatchStrength      )
                            .AddParameter_null("@DocumentTransformId", endemeIndex.DocumentTransformId)
                            .AddParameter_null("@EndemeSetId"        , endemeIndex.EndemeSetId        )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeIndex: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeIndexId " + endemeIndex.EndemeKeyId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeIndex table

        #region EndemeLarge table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeLarge table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeLarge> AtMareEndemeLargeOf         (int         endemesetId    , InfoAspect aspect) { return AtOnMareEndemeLarge(MareEndemeLargeOf(endemesetId, aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndeme       (int         endemeId       , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndeme         (endemeId       , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeSet    (Guid        endemeSetId    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeSet      (endemeSetId    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeString (string      endemeString   , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeString   (endemeString   , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeCode   (string      endemeCode     , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeCode     (endemeCode     , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeLabel  (string      endemeLabel    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeLabel    (endemeLabel    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeDescr  (string      endemeDescr    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeDescr    (endemeDescr    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfRawValue     (bool        rawValue       , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfRawValue       (rawValue       , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfMultipleUse  (bool        multipleUse    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfMultipleUse    (multipleUse    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueBinary  (byte[]      valueBinary    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueBinary    (valueBinary    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueDateTime(DateTime    valueDateTime  , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueDateTime  (valueDateTime  , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueFloat   (double      valueFloat     , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueFloat     (valueFloat     , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueNumber  (decimal     valueNumber    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueNumber    (valueNumber    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueText    (string      valueText      , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueText      (valueText      , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeProfile(int         endemeProfileId, InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeProfile  (endemeProfileId, aspect)); }
        public Atom              AtomMaEndemeLargeOf         (int         endemesetId    , InfoAspect aspect) { return MainEndemeLargeOf(endemesetId, aspect).ToAtom(); }
        public void              ExEndemeLarge               (int         endemeId       , InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM "+ENDEME_LARGE_TABLE+" WHERE EndemeId = " + endemeId, aspect.MainConn); }
        public RichDataTable     IntoEndemeLarge             (EndemeLarge endemeLarge    , InfoAspect aspect) { if (   IsEndemeLarge(endemeLarge.EndemeId, aspect)) return InEndemeLarge(endemeLarge.EndemeId, aspect); else { return InEndemeLarge(ToEndemeLarge(endemeLarge, aspect), aspect); } }
        public EndemeLarge       OnInEndemeLarge             (int         endemeId       , InfoAspect aspect) { return OnEndemeLarge(InEndemeLarge  (endemeId, aspect), 0, true); }
        public EndemeLarge       OnIntoEndemeLarge           (EndemeLarge endemeLarge    , InfoAspect aspect) { return OnEndemeLarge(IntoEndemeLarge(endemeLarge  , aspect), 0, true); }
        public int               UpToEndemeLarge             (EndemeLarge endemeLarge    , InfoAspect aspect) { if (   IsEndemeLarge(endemeLarge.EndemeId, aspect)) return UpEndemeLarge(endemeLarge        , aspect); else { return ToEndemeLarge(endemeLarge, aspect); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeLarge_test -->
        /// <summary>
        ///      Tests the ToEndemeLarge method
        /// </summary>
        public void ToEndemeLarge_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeLargeAccess";
            string adoMethod = "ToEndemeLarge";
            Random r = RandomSource.New().Random;
            int    endemeId = -1;
            EndemeAccess ende = new EndemeAccess("");

            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeLarge endemeLargeTo = ende.AnneEndemeLarge(r);
            try
            {
                endemeId = ende.ToEndemeLarge(endemeLargeTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeLarge endemeLargeFrom = ende.OnInEndemeLarge(endemeId, _aspect);
            ende.AssertEqualContent(endemeLargeFrom, endemeLargeTo, adoClass, adoMethod);


            ende.ExEndemeLarge(endemeId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeLarge_test -->
        /// <summary>
        ///      Tests the UpEndemeLarge method
        /// </summary>
        public void UpEndemeLarge_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass          = "EndemeLargeAccess";
            string adoMethod         = "UpEndemeLarge";
            Random r                 = RandomSource.New().Random;
            EndemeLarge endemeLarge1 = null;

            EndemeAccess ende = new EndemeAccess("");
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeId with a newly created endemeLarge
                // ----------------------------------------------------------------------
                int    endemeId = ende.AnIdOfEndemeLarge(_aspect);
                endemeLarge1               = ende.OnInEndemeLarge(endemeId, _aspect);
                EndemeLarge  endemeLarge2  = ende.AnneEndemeLarge(r);
                endemeLarge2.EndemeId = endemeLarge1.EndemeId;
                ende.UpEndemeLarge(endemeLarge2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeLarge endemeLarge3 = ende.OnInEndemeLarge(endemeId, _aspect);
                Assert.That(endemeLarge3.EndemeId, Is.equal_to, endemeLarge2.EndemeId, adoClass, adoMethod + " test update existing endemeLarge");
                ende.AssertEqualContent  (endemeLarge3, endemeLarge2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeLarge3, endemeLarge1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeLarge, did the update fail?
                // ----------------------------------------------------------------------
                EndemeLarge  endemeLarge4  = ende.AnneEndemeLarge(r);
                endemeLarge4.EndemeId      = ende.HiIdOfEndemeLarge(_aspect) + 1;
                int          count         = ende.UpEndemeLarge(endemeLarge4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeLarge");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeLarge(endemeLarge1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeLarge -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeLarge table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public int AnIdOfEndemeLarge(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT EndemeId FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeLargeId = new RichDataTable(cmd, null, "EndemeLargeId", "EndemeId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeLargeId.Count > 0) return (int)endemeLargeId.ToList("EndemeId", -1)[r.Next(endemeLargeId.Count)];
                else return -1;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeLarge -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeLarge object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeLarge</returns>
        private EndemeLarge AnneEndemeLarge(Random r)
        {
            EndemeLarge endemeLarge = new EndemeLarge
              //{ EndemeId        = go.HiIdOfEndemeId() + 1
                { EndemeSetId     = Guid.NewGuid()
                , EndemeString    = r.Next(10000).ToString()
                , EndemeCode      = r.Next(10000).ToString()
                , EndemeLabel     = r.Next(10000).ToString()
                , EndemeDescr     = r.Next(10000).ToString()
                , RawValue        = true
                , MultipleUse     = true
                , ValueBinary     = null
                , ValueDateTime   = DateTime.Now
                , ValueFloat      = r.Next(10000)
                , ValueNumber     = r.Next(10000)
                , ValueText       = r.Next(10000).ToString()
                , EndemeProfileId = r.Next(10000)
                };
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeLarge">endemeLarge being tested</param>
        /// <param name="tgt"      >endemeLarge being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeLarge endemeLarge, EndemeLarge tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeLarge.EndemeSetId    , Is.equal_to, tgt.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(endemeLarge.EndemeString   , Is.equal_to, tgt.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(endemeLarge.EndemeCode     , Is.equal_to, tgt.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(endemeLarge.EndemeLabel    , Is.equal_to, tgt.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(endemeLarge.EndemeDescr    , Is.equal_to, tgt.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
            Assert.That(endemeLarge.RawValue       , Is.equal_to, tgt.RawValue       , adoClass, adoMethod + " RawValue"       );
            Assert.That(endemeLarge.MultipleUse    , Is.equal_to, tgt.MultipleUse    , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(endemeLarge.ValueBinary    , Is.equal_to, tgt.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(endemeLarge.ValueDateTime  , Is.equal_to, tgt.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(endemeLarge.ValueFloat     , Is.equal_to, tgt.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(endemeLarge.ValueNumber    , Is.equal_to, tgt.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(endemeLarge.ValueText      , Is.equal_to, tgt.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(endemeLarge.EndemeProfileId, Is.equal_to, tgt.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeLarge">endemeLarge being tested</param>
        /// <param name="tgt"      >endemeLarge being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeLarge endemeLarge, EndemeLarge tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeLarge.EndemeSetId    , Is.not_equal_to, tgt.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(endemeLarge.EndemeString   , Is.not_equal_to, tgt.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(endemeLarge.EndemeCode     , Is.not_equal_to, tgt.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(endemeLarge.EndemeLabel    , Is.not_equal_to, tgt.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(endemeLarge.EndemeDescr    , Is.not_equal_to, tgt.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
          //Assert.That(endemeLarge.RawValue       , Is.not_equal_to, tgt.RawValue       , adoClass, adoMethod + " RawValue"       );
          //Assert.That(endemeLarge.MultipleUse    , Is.not_equal_to, tgt.MultipleUse    , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(endemeLarge.ValueBinary    , Is.not_equal_to, tgt.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(endemeLarge.ValueDateTime  , Is.not_equal_to, tgt.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(endemeLarge.ValueFloat     , Is.not_equal_to, tgt.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(endemeLarge.ValueNumber    , Is.not_equal_to, tgt.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(endemeLarge.ValueText      , Is.not_equal_to, tgt.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(endemeLarge.EndemeProfileId, Is.not_equal_to, tgt.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeLarge -->
        /// <summary>
        ///     Returns a list of EndemeLarge objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeLarge objects</returns>
        public List<EndemeLarge> AtEndemeLarge(RichDataTable table)
        {
            List<EndemeLarge> list = new List<EndemeLarge>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeLarge(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeLarge -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeLarge objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeLarge objects"></param>
        public List<EndemeLarge> AtOnMareEndemeLarge(RichDataTable table)
        {
            List<EndemeLarge> endemeLargeList = new List<EndemeLarge>(table.Count);
            Dictionary<int,EndemeLarge> found = new Dictionary<int,EndemeLarge>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeId = table.IntValue(row, "EndemeId", -1);
                EndemeLarge endemeLarge = null;

                if (!found.ContainsKey(endemeId))
                {
                    endemeLarge = OnEndemeLarge(table, row);

                    endemeLarge.EndemeIndexList = new List<EndemeIndex>();
                  //endemeLarge.EndemeSet   = (new EndemeAccess(ConnectionString)).OnEndemeSet(table, row);
                    endemeLargeList.Add(endemeLarge);
                    found.Add(endemeId, endemeLarge);
                }
                else
                {
                    endemeLarge = found[endemeId];
                }

                endemeLarge.EndemeIndexList.Add((new EndemeAccess(ConnectionString)).OnEndemeIndex(table, row));
            }

            return endemeLargeList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeLarge -->
        /// <summary>
        ///      Copies an endemeLarge
        /// </summary>
        /// <param name="endemeLarge">endemeLarge to copy</param>
        public static EndemeLarge CpEndemeLarge(EndemeLarge endemeLarge)
        {
            EndemeLarge output = new EndemeLarge();

            output.EndemeId        = endemeLarge.EndemeId;
            output.EndemeSetId     = endemeLarge.EndemeSetId;
            output.EndemeString    = endemeLarge.EndemeString;
            output.EndemeCode      = endemeLarge.EndemeCode;
            output.EndemeLabel     = endemeLarge.EndemeLabel;
            output.EndemeDescr     = endemeLarge.EndemeDescr;
            output.RawValue        = endemeLarge.RawValue;
            output.MultipleUse     = endemeLarge.MultipleUse;
            output.ValueBinary     = endemeLarge.ValueBinary;
            output.ValueDateTime   = endemeLarge.ValueDateTime;
            output.ValueFloat      = endemeLarge.ValueFloat;
            output.ValueNumber     = endemeLarge.ValueNumber;
            output.ValueText       = endemeLarge.ValueText;
            output.EndemeProfileId = endemeLarge.EndemeProfileId;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeLargeCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeLarge table
        /// </summary>
        /// <returns>a count of rows in the EndemeLarge table</returns>
        public int EndemeLargeCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT COUNT(*) FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = TreatAs.IntValue(cmd.ExecuteScalar(-1), -1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeLarge: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EnEndemeLarge -->
        /// <summary>
        ///      (En)ables an EndemeLarge
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        public void EnEndemeLarge(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_LARGE_TABLE+" SET N/A = 1 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeLarge: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeLarge -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemelarge table
        /// </summary>
        /// <returns>Maximum EndemeLarge.EndemeId</returns>
        public int HiIdOfEndemeLarge(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT MAX(EndemeId) AS EndemeId WITH(NOLOCK) FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int endemeId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeLarge: " + cmd.Errors);
            return endemeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLarge -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeLarge table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeLarge(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId"  , endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndeme -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeId column
        /// </summary>
        /// <param name="endemeId">value in EndemeId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeId</returns>
        public RichDataTable InEndemeLargeOfEndeme(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId" , endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueDateTime -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueDateTime column
        /// </summary>
        /// <param name="valueDateTime">value in ValueDateTime column</param>
        /// <returns>a table of rows related to the specifed value of ValueDateTime</returns>
        public RichDataTable InEndemeLargeOfValueDateTime(DateTime valueDateTime, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE ValueDateTime = @ValueDateTime"
                , Throws.Actions, "PR")
                .AddParameter_date("@ValueDateTime" , valueDateTime, SqlDbType.DateTime);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf10EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueFloat -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueFloat column
        /// </summary>
        /// <param name="valueFloat">value in ValueFloat column</param>
        /// <returns>a table of rows related to the specifed value of ValueFloat</returns>
        public RichDataTable InEndemeLargeOfValueFloat(double valueFloat, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE ValueFloat = @ValueFloat"
                , Throws.Actions, "PR")
                .AddParameter_null("@ValueFloat" , valueFloat);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf11EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueNumber -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueNumber column
        /// </summary>
        /// <param name="valueNumber">value in ValueNumber column</param>
        /// <returns>a table of rows related to the specifed value of ValueNumber</returns>
        public RichDataTable InEndemeLargeOfValueNumber(decimal valueNumber, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE ValueNumber = @ValueNumber"
                , Throws.Actions, "PR")
                .AddParameter_null("@ValueNumber" , valueNumber);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf12EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueText -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueText column
        /// </summary>
        /// <param name="valueText">value in ValueText column</param>
        /// <returns>a table of rows related to the specifed value of ValueText</returns>
        public RichDataTable InEndemeLargeOfValueText(string valueText, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE ValueText = @ValueText"
                , Throws.Actions, "PR")
                .AddParameter_safe("@ValueText" , valueText);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf13EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeProfile -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeProfileId column
        /// </summary>
        /// <param name="endemeProfileId">value in EndemeProfileId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeProfileId</returns>
        public RichDataTable InEndemeLargeOfEndemeProfile(int endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter_null("@EndemeProfileId" , endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf14EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeLargeOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter_null("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeString -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeString column
        /// </summary>
        /// <param name="endemeString">value in EndemeString column</param>
        /// <returns>a table of rows related to the specifed value of EndemeString</returns>
        public RichDataTable InEndemeLargeOfEndemeString(string endemeString, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeString = @EndemeString"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeString" , endemeString);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeCode -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeCode column
        /// </summary>
        /// <param name="endemeCode">value in EndemeCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCode</returns>
        public RichDataTable InEndemeLargeOfEndemeCode(string endemeCode, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeCode = @EndemeCode"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeCode" , endemeCode);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeLabel column
        /// </summary>
        /// <param name="endemeLabel">value in EndemeLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeLabel</returns>
        public RichDataTable InEndemeLargeOfEndemeLabel(string endemeLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeLabel = @EndemeLabel"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeLabel" , endemeLabel);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeDescr -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeDescr column
        /// </summary>
        /// <param name="endemeDescr">value in EndemeDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeDescr</returns>
        public RichDataTable InEndemeLargeOfEndemeDescr(string endemeDescr, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeDescr = @EndemeDescr"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeDescr" , endemeDescr);
            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfRawValue -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the RawValue column
        /// </summary>
        /// <param name="rawValue">value in RawValue column</param>
        /// <returns>a table of rows related to the specifed value of RawValue</returns>
        public RichDataTable InEndemeLargeOfRawValue(bool rawValue, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE RawValue = @RawValue"
                , Throws.Actions, "PR")
                .AddParameter_null("@RawValue" , rawValue);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfMultipleUse -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the MultipleUse column
        /// </summary>
        /// <param name="multipleUse">value in MultipleUse column</param>
        /// <returns>a table of rows related to the specifed value of MultipleUse</returns>
        public RichDataTable InEndemeLargeOfMultipleUse(bool multipleUse, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE MultipleUse = @MultipleUse"
                , Throws.Actions, "PR")
                .AddParameter("@MultipleUse" , multipleUse);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf8EndemeLarge: " + cmd.Errors);
            return table;







        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueBinary -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueBinary column
        /// </summary>
        /// <param name="valueBinary">value in ValueBinary column</param>
        /// <returns>a table of rows related to the specifed value of ValueBinary</returns>
        public RichDataTable InEndemeLargeOfValueBinary(byte[] valueBinary, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE ValueBinary = @ValueBinary"
                , Throws.Actions, "PR")
                .AddParameter_byte("@ValueBinary" , valueBinary);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf9EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeLarge -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeLarge table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeLarge(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                    , "SELECT * FROM "+ENDEME_LARGE_TABLE+" WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                    , Throws.Actions, "P")
                    .AddParameter("@EndemeId", endemeId);


                using (RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeLarge with EndemeId " + endemeId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }
            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeLarge -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeLarge
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of endemeLarge rows with their joined parent data</returns>
        public RichDataTable MainEndemeLarge(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, el.*"
                + "\r\n" + " FROM "               +ENDEME_LARGE_TABLE+" AS el WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE+" AS es WITH(NOLOCK) ON es.EndemeSetId = el.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND el.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeLargeOf -->
        /// <summary>
        ///      Returns a endemeLarge list (Of) a p(a)rent endemeset with its endemeset data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeLarge rows with their joined parent data</returns>
        public RichDataTable MainEndemeLargeOf(int endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, el.*"
                + "\r\n" + " FROM "               +ENDEME_LARGE_TABLE+" AS el WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE  +" AS es WITH(NOLOCK) ON es.EndemeSetId = el.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND el.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeLargeOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeLargeOf -->
        /// <summary>
        ///      Returns a endemeLarge list (Of) a p(a)rent endemeset with (re)trieved endemeLarge and endemeindex data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeLarge rows with their joined parent data</returns>
        public RichDataTable MareEndemeLargeOf(int endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, el.*, ei.*"
                + "\r\n" + " FROM "               +ENDEME_LARGE_TABLE+" AS el WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_SET_TABLE  +" AS es WITH(NOLOCK) ON es.EndemeSetId = el.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_INDEX_TABLE+" AS ei WITH(NOLOCK) ON ei.EndemeId    = el.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND el.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeLargeOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeLarge -->
        /// <summary>
        ///      Creates a (Ne)w EndemeLarge (o)bject from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawValue"       ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new EndemeLarge object</returns>
        public EndemeLarge NeonEndemeLarge
            ( int      endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawValue
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            )
        {
            EndemeLarge endemeLarge = new EndemeLarge
                { EndemeId        = endemeId
                , EndemeSetId     = endemeSetId
                , EndemeString    = endemeString
                , EndemeCode      = endemeCode
                , EndemeLabel     = endemeLabel
                , EndemeDescr     = endemeDescr
                , RawValue        = rawValue
                , MultipleUse     = multipleUse
                , ValueBinary     = valueBinary
                , ValueDateTime   = valueDateTime
                , ValueFloat      = valueFloat
                , ValueNumber     = valueNumber
                , ValueText       = valueText
                , EndemeProfileId = endemeProfileId
                };
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeLarge -->
        /// <summary>
        ///      Converts a row in the EndemeLarge data table into a EndemeLarge (O)bject
        /// </summary>
        /// <param name="endemeLargeTable">a table containing columns to build a batch object</param>
        /// <param name="row"             >the row to convert</param>
        /// <param name="dispose"         >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeLarge OnEndemeLarge(RichDataTable endemeLargeTable, int row, bool dispose = false)
        {
            EndemeLarge endemeLarge = new EndemeLarge();
            endemeLarge.EndemeId        = endemeLargeTable.IntValue (row, "EndemeId"       , -1          );
            endemeLarge.EndemeSetId     = endemeLargeTable.GuidValue(row, "EndemeSetId"    , Guid.Empty  );
            endemeLarge.EndemeString    = endemeLargeTable.StrValue (row, "EndemeString"   , ""          );
            endemeLarge.EndemeCode      = endemeLargeTable.StrValue (row, "EndemeCode"     , ""          );
            endemeLarge.EndemeLabel     = endemeLargeTable.StrValue (row, "EndemeLabel"    , ""          );
            endemeLarge.EndemeDescr     = endemeLargeTable.StrValue (row, "EndemeDescr"    , ""          );
            endemeLarge.RawValue        = endemeLargeTable.BoolValue(row, "RawValue"       , false       );
            endemeLarge.MultipleUse     = endemeLargeTable.BoolValue(row, "MultipleUse"    , false       );
            endemeLarge.ValueBinary     = endemeLargeTable.ByteValue(row, "ValueBinary"                  );
            endemeLarge.ValueDateTime   = endemeLargeTable.DateValue(row, "ValueDateTime"  , DateTime.Now);
            endemeLarge.ValueFloat      = endemeLargeTable.RealValue(row, "ValueFloat"     , -1.0        );
            endemeLarge.ValueNumber     = endemeLargeTable.DecValue (row, "ValueNumber"    , -1.0M       );
            endemeLarge.ValueText       = endemeLargeTable.StrValue (row, "ValueText"      , ""          );
            endemeLarge.EndemeProfileId = endemeLargeTable.IntValue (row, "EndemeProfileId", -1          );

            if (dispose) endemeLargeTable.Dispose();
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeLarge -->
        /// <summary>
        ///      Returns a table of a EndemeLarge (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of EndemeLarge rows with their joined details</returns>
        public RichDataTable ReinEndemeLarge(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT el.*, ei.*"
                + "\r\n" + " FROM "               +ENDEME_LARGE_TABLE+" AS el WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_INDEX_TABLE+" AS ei WITH(NOLOCK) ON ei.EndemeId = el.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND el.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeLarge -->
        /// <summary>
        ///      Inserts an endemeLarge object in(To) the database
        /// </summary>
        /// <param name="endemeLarge">endemeLarge to insert into database</param>
        /// <returns>the Id of the inserted EndemeLarge</returns>
        public int ToEndemeLarge(EndemeLarge endemeLarge, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " INSERT INTO "+ENDEME_LARGE_TABLE
              //+ "\r\n" + "        ( EndemeId"
                + "\r\n" + "        ( EndemeSetId"
                + "\r\n" + "        , EndemeString"
                + "\r\n" + "        , EndemeCode"
                + "\r\n" + "        , EndemeLabel"
                + "\r\n" + "        , EndemeDescr"
                + "\r\n" + "        , RawValue"
                + "\r\n" + "        , MultipleUse"
                + "\r\n" + "        , ValueBinary"
                + "\r\n" + "        , ValueDateTime"
                + "\r\n" + "        , ValueFloat"
                + "\r\n" + "        , ValueNumber"
                + "\r\n" + "        , ValueText"
                + "\r\n" + "        , EndemeProfileId"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeId"
              //+ "\r\n" + " VALUES ( @EndemeId"
                + "\r\n" + " VALUES ( @EndemeSetId"
                + "\r\n" + "        , @EndemeString"
                + "\r\n" + "        , @EndemeCode"
                + "\r\n" + "        , @EndemeLabel"
                + "\r\n" + "        , @EndemeDescr"
                + "\r\n" + "        , @RawValue"
                + "\r\n" + "        , @MultipleUse"
                + "\r\n" + "        , @ValueBinary"
                + "\r\n" + "        , @ValueDateTime"
                + "\r\n" + "        , @ValueFloat"
                + "\r\n" + "        , @ValueNumber"
                + "\r\n" + "        , @ValueText"
                + "\r\n" + "        , @EndemeProfileId"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
              //.AddParameter     ("@EndemeId"       , endemeLarge.EndemeId                           )
                .AddParameter_null("@EndemeSetId"    , endemeLarge.EndemeSetId                        )
                .AddParameter_safe("@EndemeString"   , endemeLarge.EndemeString   ,   24)
                .AddParameter_safe("@EndemeCode"     , endemeLarge.EndemeCode     ,    8)
                .AddParameter_safe("@EndemeLabel"    , endemeLarge.EndemeLabel    ,   64)
                .AddParameter_safe("@EndemeDescr"    , endemeLarge.EndemeDescr    ,   -1)
                .AddParameter_null("@RawValue"       , endemeLarge.RawValue                           )
                .AddParameter     ("@MultipleUse"    , endemeLarge.MultipleUse                        )
                .AddParameter_byte("@ValueBinary"    , endemeLarge.ValueBinary                        )
                .AddParameter_date("@ValueDateTime"  , endemeLarge.ValueDateTime  , SqlDbType.DateTime)
                .AddParameter_null("@ValueFloat"     , endemeLarge.ValueFloat                         )
                .AddParameter_null("@ValueNumber"    , endemeLarge.ValueNumber                        )
                .AddParameter_safe("@ValueText"      , endemeLarge.ValueText      ,   -1)
                .AddParameter_null("@EndemeProfileId", endemeLarge.EndemeProfileId                    );


            int endemeLargeId = TreatAs.IntValue(cmd.ExecuteScalar(-1),-2);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeLarge: " + cmd.Errors);
            return endemeLargeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeLarge -->
        /// <summary>
        ///     Inserts in(To) the EndemeLarge table a (ne)w endemeLarge built from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawValue"       ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new EndemeLarge object</returns>
        public EndemeLarge ToneEndemeLarge
            ( int      endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawValue
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            , InfoAspect aspect)
        {
            EndemeLarge endemeLarge = NeonEndemeLarge
                ( endemeId
                , endemeSetId
                , endemeString
                , endemeCode
                , endemeLabel
                , endemeDescr
                , rawValue
                , multipleUse
                , valueBinary
                , valueDateTime
                , valueFloat
                , valueNumber
                , valueText
                , endemeProfileId
                );
            endemeLarge.EndemeId = ToEndemeLarge(endemeLarge, aspect);
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeLarge -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeLarge
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <param name="disableValue">the value meaning diable</param>
        public void UnEndemeLarge(int endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_LARGE_TABLE+" SET N/A = 0 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeLarge: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeLarge -->
        /// <summary>
        ///      (Up)dates a row in the EndemeLarge table from a EndemeLarge object
        /// </summary>
        /// <param name="endemeLarge">endemeLarge to update</param>
        /// <returns>the count of the updated endemeLarge rows"></param>
        public int UpEndemeLarge(EndemeLarge endemeLarge, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeId = " + endemeLarge.EndemeId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(ENDEME_LARGE_TABLE, whereClause, aspect.MainConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                            , "\r\n" + " UPDATE "+ENDEME_LARGE_TABLE
                            + "\r\n" + " SET   EndemeSetId     = @EndemeSetId"
                            + "\r\n" + "     , EndemeString    = @EndemeString"
                            + "\r\n" + "     , EndemeCode      = @EndemeCode"
                            + "\r\n" + "     , EndemeLabel     = @EndemeLabel"
                            + "\r\n" + "     , EndemeDescr     = @EndemeDescr"
                            + "\r\n" + "     , RawValue        = @RawValue"
                            + "\r\n" + "     , MultipleUse     = @MultipleUse"
                            + "\r\n" + "     , ValueBinary     = @ValueBinary"
                            + "\r\n" + "     , ValueDateTime   = @ValueDateTime"
                            + "\r\n" + "     , ValueFloat      = @ValueFloat"
                            + "\r\n" + "     , ValueNumber     = @ValueNumber"
                            + "\r\n" + "     , ValueText       = @ValueText"
                            + "\r\n" + "     , EndemeProfileId = @EndemeProfileId"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            .AddParameter     ("@EndemeId"       , endemeLarge.EndemeId                           )
                            .AddParameter_null("@EndemeSetId"    , endemeLarge.EndemeSetId                        )
                            .AddParameter_safe("@EndemeString"   , endemeLarge.EndemeString   ,   24)
                            .AddParameter_safe("@EndemeCode"     , endemeLarge.EndemeCode     ,    8)
                            .AddParameter_safe("@EndemeLabel"    , endemeLarge.EndemeLabel    ,   64)
                            .AddParameter_safe("@EndemeDescr"    , endemeLarge.EndemeDescr    ,   -1)
                            .AddParameter_null("@RawValue"       , endemeLarge.RawValue                           )
                            .AddParameter     ("@MultipleUse"    , endemeLarge.MultipleUse                        )
                            .AddParameter_byte("@ValueBinary"    , endemeLarge.ValueBinary                        )
                            .AddParameter_date("@ValueDateTime"  , endemeLarge.ValueDateTime  , SqlDbType.DateTime)
                            .AddParameter_null("@ValueFloat"     , endemeLarge.ValueFloat                         )
                            .AddParameter_null("@ValueNumber"    , endemeLarge.ValueNumber                        )
                            .AddParameter_safe("@ValueText"      , endemeLarge.ValueText      ,   -1)
                            .AddParameter_null("@EndemeProfileId", endemeLarge.EndemeProfileId                    )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeLarge: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeLargeId " + endemeLarge.EndemeId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeLarge table

        #region EndemeSet table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeSet table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeSet> AtEndemeSetOfEndemeSet        (Guid   endemeSetId      , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSet        (endemeSetId      , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetCode    (string endemeSetCode    , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetCode    (endemeSetCode    , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetLabel   (string endemeSetLabel   , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetLabel   (endemeSetLabel   , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetDescr   (string endemeSetDescr   , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetDescr   (endemeSetDescr   , aspect)); }
        public List<EndemeSet> AtEndemeSetOfDefaultEndeme    (string defaultEndeme    , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfDefaultEndeme    (defaultEndeme    , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetResource(string endemeSetResource, InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetResource(endemeSetResource, aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetVersion (string endemeSetVersion , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetVersion (endemeSetVersion , aspect)); }
        public void            ExEndemeSet    (Guid      endemeSetId, InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM "+ENDEME_SET_TABLE+" WHERE EndemeSetId = '" + endemeSetId + "'", aspect.MainConn); }
        public RichDataTable   IntoEndemeSet  (EndemeSet endemeSet  , InfoAspect aspect) { if   (!IsEndemeSet(endemeSet.EndemeSetId, aspect)) ToEndemeSet(endemeSet, aspect); return InEndemeSet(endemeSet.SetId, aspect); }
        public EndemeSet       OnInEndemeSet  (Guid      endemeSetId, InfoAspect aspect) { return OnEndemeSet(InEndemeSet  (endemeSetId, aspect), 0, true); }
        public EndemeSet       OnIntoEndemeSet(EndemeSet endemeSet  , InfoAspect aspect) { return OnEndemeSet(IntoEndemeSet(endemeSet  , aspect), 0, true); }
        public Guid            UpToEndemeSet  (EndemeSet endemeSet  , InfoAspect aspect) { if   ( IsEndemeSet(endemeSet.EndemeSetId, aspect)) UpEndemeSet(endemeSet, aspect); else ToEndemeSet(endemeSet, aspect); return endemeSet.SetId; }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeSet_test -->
        /// <summary>
        ///      Tests the ToEndemeSet method
        /// </summary>
        public void ToEndemeSet_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeSetAccess";
            string adoMethod = "ToEndemeSet";
            Random r = RandomSource.New().Random;
            Guid   endemeSetId = Guid.Empty;
            EndemeAccess ende = new EndemeAccess("");

            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeSet endemeSetTo = ende.AnneEndemeSet(r);
            endemeSetId = endemeSetTo.SetId;
            try
            {
                ende.ToEndemeSet(endemeSetTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeSetId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeSet endemeSetFrom = ende.OnInEndemeSet(endemeSetId, _aspect);
            ende.AssertEqualContent(endemeSetFrom, endemeSetTo, adoClass, adoMethod);


            ende.ExEndemeSet(endemeSetId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeSet_test -->
        /// <summary>
        ///      Tests the UpEndemeSet method
        /// </summary>
        public void UpEndemeSet_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass      = "EndemeSetAccess";
            string adoMethod     = "UpEndemeSet";
            Random r               = RandomSource.New().Random;
            EndemeSet endemeSet1 = null;

            EndemeAccess ende = new EndemeAccess("");
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeSetId with a newly created endemeSet
                // ----------------------------------------------------------------------
                Guid    endemeSetId = ende.AnIdOfEndemeSet(_aspect);
                endemeSet1             = ende.OnInEndemeSet(endemeSetId, _aspect);
                EndemeSet  endemeSet2  = ende.AnneEndemeSet(r);
                endemeSet2.EndemeSetId = endemeSet1.EndemeSetId;
                ende.UpEndemeSet(endemeSet2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeSet endemeSet3 = ende.OnInEndemeSet(endemeSetId, _aspect);
                Assert.That(endemeSet3.EndemeSetId, Is.equal_to, endemeSet2.EndemeSetId, adoClass, adoMethod + " test update existing endemeSet");
                ende.AssertEqualContent  (endemeSet3, endemeSet2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeSet3, endemeSet1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeSet, did the update fail?
                // ----------------------------------------------------------------------
                EndemeSet  endemeSet4  = ende.AnneEndemeSet(r);
                endemeSet4.EndemeSetId = Guid.NewGuid();
                int        count       = ende.UpEndemeSet(endemeSet4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeSet");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeSet(endemeSet1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeSet -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeSet table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public Guid AnIdOfEndemeSet(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT EndemeSetId FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeSetId = new RichDataTable(cmd, null, "EndemeSetId", "EndemeSetId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeSetId.Count > 0) return (Guid)endemeSetId.ToList("EndemeSetId", Guid.NewGuid())[r.Next(endemeSetId.Count)];
                else return Guid.NewGuid();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeSet -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeSet object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeSet</returns>
        private EndemeSet AnneEndemeSet(Random r)
        {
            EndemeSet endemeSet = new EndemeSet
                { EndemeSetId       = Guid.NewGuid()
                , EndemeSetCode     = r.Next(10000).ToString()
                , EndemeSetLabel    = r.Next(10000).ToString()
                , EndemeSetDescr    = r.Next(10000).ToString()
                , DefaultEndeme     = r.Next(10000).ToString()
                , EndemeSetResource = r.Next(10000).ToString()
                , EndemeSetVersion  = r.Next(10000).ToString()
                };
            return endemeSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeSet">endemeSet being tested</param>
        /// <param name="tgt"      >endemeSet being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeSet endemeSet, EndemeSet tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeSet.EndemeSetCode    , Is.equal_to, tgt.EndemeSetCode    , adoClass, adoMethod + " EndemeSetCode"    );
            Assert.That(endemeSet.EndemeSetLabel   , Is.equal_to, tgt.EndemeSetLabel   , adoClass, adoMethod + " EndemeSetLabel"   );
            Assert.That(endemeSet.EndemeSetDescr   , Is.equal_to, tgt.EndemeSetDescr   , adoClass, adoMethod + " EndemeSetDescr"   );
            Assert.That(endemeSet.DefaultEndeme    , Is.equal_to, tgt.DefaultEndeme    , adoClass, adoMethod + " DefaultEndeme"    );
            Assert.That(endemeSet.EndemeSetResource, Is.equal_to, tgt.EndemeSetResource, adoClass, adoMethod + " EndemeSetResource");
            Assert.That(endemeSet.EndemeSetVersion , Is.equal_to, tgt.EndemeSetVersion , adoClass, adoMethod + " EndemeSetVersion" );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeSet">endemeSet being tested</param>
        /// <param name="tgt"      >endemeSet being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeSet endemeSet, EndemeSet tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeSet.EndemeSetCode    , Is.not_equal_to, tgt.EndemeSetCode    , adoClass, adoMethod + " EndemeSetCode"    );
            Assert.That(endemeSet.EndemeSetLabel   , Is.not_equal_to, tgt.EndemeSetLabel   , adoClass, adoMethod + " EndemeSetLabel"   );
            Assert.That(endemeSet.EndemeSetDescr   , Is.not_equal_to, tgt.EndemeSetDescr   , adoClass, adoMethod + " EndemeSetDescr"   );
            Assert.That(endemeSet.DefaultEndeme    , Is.not_equal_to, tgt.DefaultEndeme    , adoClass, adoMethod + " DefaultEndeme"    );
            Assert.That(endemeSet.EndemeSetResource, Is.not_equal_to, tgt.EndemeSetResource, adoClass, adoMethod + " EndemeSetResource");
            Assert.That(endemeSet.EndemeSetVersion , Is.not_equal_to, tgt.EndemeSetVersion , adoClass, adoMethod + " EndemeSetVersion" );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeSet -->
        /// <summary>
        ///     Returns a list of EndemeSet objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeSet objects</returns>
        public List<EndemeSet> AtEndemeSet(RichDataTable table)
        {
            List<EndemeSet> list = new List<EndemeSet>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeSet(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeSet -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeSet objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeSet objects"></param>
        public List<EndemeSet> AtOnMareEndemeSet(RichDataTable table)
        {
            List<EndemeSet> endemeSetList = new List<EndemeSet>(table.Count);
            Dictionary<int,EndemeSet> found = new Dictionary<int,EndemeSet>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeSetId = table.IntValue(row, "EndemeSetId", -1);
                EndemeSet endemeSet = null;

                if (!found.ContainsKey(endemeSetId))
                {
                    endemeSet = OnEndemeSet(table, row);

                    endemeSet.EndemeCharacteristicList = new List<EndemeCharacteristic>();
                    endemeSetList.Add(endemeSet);
                    found.Add(endemeSetId, endemeSet);
                }
                else
                {
                    endemeSet = found[endemeSetId];
                }

                endemeSet.EndemeCharacteristicList.Add((new EndemeAccess(ConnectionString)).OnEndemeCharacteristic(table, row));
            }

            return endemeSetList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeSet -->
        /// <summary>
        ///      Copies an endemeSet
        /// </summary>
        /// <param name="endemeSet">endemeSet to copy</param>
        public static EndemeSet CpEndemeSet(EndemeSet endemeSet)
        {
            EndemeSet output = new EndemeSet();

            output.EndemeSetId       = endemeSet.EndemeSetId;
            output.EndemeSetCode     = endemeSet.EndemeSetCode;
            output.EndemeSetLabel    = endemeSet.EndemeSetLabel;
            output.EndemeSetDescr    = endemeSet.EndemeSetDescr;
            output.DefaultEndeme     = endemeSet.DefaultEndeme;
            output.EndemeSetResource = endemeSet.EndemeSetResource;
            output.EndemeSetVersion  = endemeSet.EndemeSetVersion;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeSetCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeSet table
        /// </summary>
        /// <returns>a count of rows in the EndemeSet table</returns>
        public int EndemeSetCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT COUNT(*) FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = TreatAs.IntValue(cmd.ExecuteScalar(-1), -1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeSet: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EnEndemeSet -->
        /// <summary>
        ///      (En)ables an EndemeSet
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        public void EnEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_SET_TABLE+" SET N/A = 1 WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeSet: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeSet -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemeset table
        /// </summary>
        /// <returns>Maximum EndemeSet.EndemeSetId</returns>
//      public Guid HiIdOfEndemeSet(Aspect aspect)
//      {
//          RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
//              , "SELECT MAX(EndemeSetId) AS EndemeSetId WITH(NOLOCK) FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK)"
//              , Throws.Actions, "PR");


//          Guid endemeSetId = cmd.ExecuteScalar(Guid.NewGuid());
//          if (__.StringContainsStuff(cmd.Errors))
//              throw new ApplicationException("HiIdOfEndemeSet: " + cmd.Errors);
//          return endemeSetId;
//      }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSet -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeSet table
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId"  , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeSetOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetCode -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetCode column
        /// </summary>
        /// <param name="endemeSetCode">value in EndemeSetCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetCode</returns>
        public RichDataTable InEndemeSetOfEndemeSetCode(string endemeSetCode, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetCode = @EndemeSetCode"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeSetCode" , endemeSetCode);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetLabel column
        /// </summary>
        /// <param name="endemeSetLabel">value in EndemeSetLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetLabel</returns>
        public RichDataTable InEndemeSetOfEndemeSetLabel(string endemeSetLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetLabel = @EndemeSetLabel"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeSetLabel" , endemeSetLabel);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetDescr -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetDescr column
        /// </summary>
        /// <param name="endemeSetDescr">value in EndemeSetDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetDescr</returns>
        public RichDataTable InEndemeSetOfEndemeSetDescr(string endemeSetDescr, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetDescr = @EndemeSetDescr"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeSetDescr" , endemeSetDescr);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfDefaultEndeme -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the DefaultEndeme column
        /// </summary>
        /// <param name="defaultEndeme">value in DefaultEndeme column</param>
        /// <returns>a table of rows related to the specifed value of DefaultEndeme</returns>
        public RichDataTable InEndemeSetOfDefaultEndeme(string defaultEndeme, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE DefaultEndeme = @DefaultEndeme"
                , Throws.Actions, "PR")
                .AddParameter_safe("@DefaultEndeme" , defaultEndeme);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetResource -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetResource column
        /// </summary>
        /// <param name="endemeSetResource">value in EndemeSetResource column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetResource</returns>
        public RichDataTable InEndemeSetOfEndemeSetResource(string endemeSetResource, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetResource = @EndemeSetResource"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeSetResource" , endemeSetResource);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetVersion -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetVersion column
        /// </summary>
        /// <param name="endemeSetVersion">value in EndemeSetVersion column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetVersion</returns>
        public RichDataTable InEndemeSetOfEndemeSetVersion(string endemeSetVersion, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetVersion = @EndemeSetVersion"
                , Throws.Actions, "PR")
                .AddParameter_safe("@EndemeSetVersion" , endemeSetVersion);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeSet -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeSet table
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                    , "SELECT * FROM "+ENDEME_SET_TABLE+" WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                    , Throws.Actions, "P")
                    .AddParameter("@EndemeSetId", endemeSetId);


                using (RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeSet with EndemeSetId " + endemeSetId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }
            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeSet -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeSet
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>a table of endemeSet rows with their joined parent data</returns>
        public RichDataTable MainEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT n/a.*, es.*"
                + "\r\n" + " FROM                "+ENDEME_SET_TABLE+" AS es  WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN dbo.N/A       AS n/a WITH(NOLOCK) ON n/a.n/a = es.n/a"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND es.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeSet -->
        /// <summary>
        ///      Creates a (Ne)w EndemeSet (o)bject from member values
        /// </summary>
        /// <param name="endemeSetId"      ></param>
        /// <param name="endemeSetCode"    ></param>
        /// <param name="endemeSetLabel"   ></param>
        /// <param name="endemeSetDescr"   ></param>
        /// <param name="defaultEndeme"    ></param>
        /// <param name="endemeSetResource"></param>
        /// <param name="endemeSetVersion" ></param>
        /// <returns>the new EndemeSet object</returns>
        public EndemeSet NeonEndemeSet
            ( Guid     endemeSetId
            , string   endemeSetCode
            , string   endemeSetLabel
            , string   endemeSetDescr
            , string   defaultEndeme
            , string   endemeSetResource
            , string   endemeSetVersion
            )
        {
            EndemeSet endemeSet = new EndemeSet
                { EndemeSetId       = endemeSetId
                , EndemeSetCode     = endemeSetCode
                , EndemeSetLabel    = endemeSetLabel
                , EndemeSetDescr    = endemeSetDescr
                , DefaultEndeme     = defaultEndeme
                , EndemeSetResource = endemeSetResource
                , EndemeSetVersion  = endemeSetVersion
                };
            return endemeSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeSet -->
        /// <summary>
        ///      Converts a row in the EndemeSet data table into a EndemeSet (O)bject
        /// </summary>
        /// <param name="endemeSetTable">a table containing columns to build a batch object</param>
        /// <param name="row"           >the row to convert</param>
        /// <param name="dispose"       >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeSet OnEndemeSet(RichDataTable endemeSetTable, int row, bool dispose = false)
        {
            EndemeSet endemeSet = new EndemeSet();
            endemeSet.EndemeSetId       = endemeSetTable.GuidValue(row, "EndemeSetId"      , Guid.Empty  );
            endemeSet.EndemeSetCode     = endemeSetTable.StrValue (row, "EndemeSetCode"    , ""          );
            endemeSet.EndemeSetLabel    = endemeSetTable.StrValue (row, "EndemeSetLabel"   , ""          );
            endemeSet.EndemeSetDescr    = endemeSetTable.StrValue (row, "EndemeSetDescr"   , ""          );
            endemeSet.DefaultEndeme     = endemeSetTable.StrValue (row, "DefaultEndeme"    , ""          );
            endemeSet.EndemeSetResource = endemeSetTable.StrValue (row, "EndemeSetResource", ""          );
            endemeSet.EndemeSetVersion  = endemeSetTable.StrValue (row, "EndemeSetVersion" , ""          );

            if (dispose) endemeSetTable.Dispose();
            return endemeSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeSet -->
        /// <summary>
        ///      Returns a table of a EndemeSet (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>a table of EndemeSet rows with their joined details</returns>
        public RichDataTable ReinEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT es.*, ec.*"
                + "\r\n" + " FROM                "+ENDEME_SET_TABLE +" AS es WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_CHAR_TABLE+" AS ec WITH(NOLOCK) ON ec.EndemeSetId = es.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND es.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeSet -->
        /// <summary>
        ///      Inserts an endemeSet object in(To) the database
        /// </summary>
        /// <param name="endemeSet">endemeSet to insert into database</param>
        /// <returns>the Id of the inserted EndemeSet</returns>
        public int ToEndemeSet(EndemeSet endemeSet, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " INSERT INTO "+ENDEME_SET_TABLE
                + "\r\n" + "        ( EndemeSetId"
                + "\r\n" + "        , EndemeSetCode"
                + "\r\n" + "        , EndemeSetLabel"
                + "\r\n" + "        , EndemeSetDescr"
                + "\r\n" + "        , DefaultEndeme"
                + "\r\n" + "        , EndemeSetResource"
                + "\r\n" + "        , EndemeSetVersion"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeSetId"
                + "\r\n" + " VALUES ( @EndemeSetId"
                + "\r\n" + "        , @EndemeSetCode"
                + "\r\n" + "        , @EndemeSetLabel"
                + "\r\n" + "        , @EndemeSetDescr"
                + "\r\n" + "        , @DefaultEndeme"
                + "\r\n" + "        , @EndemeSetResource"
                + "\r\n" + "        , @EndemeSetVersion"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                .AddParameter     ("@EndemeSetId"      , endemeSet.EndemeSetId                          )
                .AddParameter_safe("@EndemeSetCode"    , endemeSet.EndemeSetCode    ,    8)
                .AddParameter_safe("@EndemeSetLabel"   , endemeSet.EndemeSetLabel   ,  128)
                .AddParameter_safe("@EndemeSetDescr"   , endemeSet.EndemeSetDescr   ,   -1)
                .AddParameter_safe("@DefaultEndeme"    , endemeSet.DefaultEndeme    ,   24)
                .AddParameter_safe("@EndemeSetResource", endemeSet.EndemeSetResource,  128)
                .AddParameter_safe("@EndemeSetVersion" , endemeSet.EndemeSetVersion ,   32);


            int endemeSetId = TreatAs.IntValue(cmd.ExecuteScalar(-1),-2);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeSet: " + cmd.Errors);
            return endemeSetId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeSet -->
        /// <summary>
        ///     Inserts in(To) the EndemeSet table a (ne)w endemeSet built from member values
        /// </summary>
        /// <param name="endemeSetId"      ></param>
        /// <param name="endemeSetCode"    ></param>
        /// <param name="endemeSetLabel"   ></param>
        /// <param name="endemeSetDescr"   ></param>
        /// <param name="defaultEndeme"    ></param>
        /// <param name="endemeSetResource"></param>
        /// <param name="endemeSetVersion" ></param>
        /// <returns>the new EndemeSet object</returns>
        public EndemeSet ToneEndemeSet
            ( Guid     endemeSetId
            , string   endemeSetCode
            , string   endemeSetLabel
            , string   endemeSetDescr
            , string   defaultEndeme
            , string   endemeSetResource
            , string   endemeSetVersion
            , InfoAspect aspect)
        {
            EndemeSet endemeSet = NeonEndemeSet
                ( endemeSetId
                , endemeSetCode
                , endemeSetLabel
                , endemeSetDescr
                , defaultEndeme
                , endemeSetResource
                , endemeSetVersion
                );
            ToEndemeSet(endemeSet, aspect);
            return endemeSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeSet -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeSet
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <param name="disableValue">the value meaning diable</param>
        public void UnEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_SET_TABLE+" SET N/A = 0 WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeSetId", endemeSetId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeSet: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeSet -->
        /// <summary>
        ///      (Up)dates a row in the EndemeSet table from a EndemeSet object
        /// </summary>
        /// <param name="endemeSet">endemeSet to update</param>
        /// <returns>the count of the updated endemeSet rows"></param>
        public int UpEndemeSet(EndemeSet endemeSet, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeSetId = " + endemeSet.EndemeSetId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(ENDEME_SET_TABLE, whereClause, aspect.MainConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                            , "\r\n" + " UPDATE "+ENDEME_SET_TABLE
                            + "\r\n" + " SET   EndemeSetCode     = @EndemeSetCode"
                            + "\r\n" + "     , EndemeSetLabel    = @EndemeSetLabel"
                            + "\r\n" + "     , EndemeSetDescr    = @EndemeSetDescr"
                            + "\r\n" + "     , DefaultEndeme     = @DefaultEndeme"
                            + "\r\n" + "     , EndemeSetResource = @EndemeSetResource"
                            + "\r\n" + "     , EndemeSetVersion  = @EndemeSetVersion"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            .AddParameter     ("@EndemeSetId"      , endemeSet.EndemeSetId                          )
                            .AddParameter_safe("@EndemeSetCode"    , endemeSet.EndemeSetCode    ,    8)
                            .AddParameter_safe("@EndemeSetLabel"   , endemeSet.EndemeSetLabel   ,  128)
                            .AddParameter_safe("@EndemeSetDescr"   , endemeSet.EndemeSetDescr   ,   -1)
                            .AddParameter_safe("@DefaultEndeme"    , endemeSet.DefaultEndeme    ,   24)
                            .AddParameter_safe("@EndemeSetResource", endemeSet.EndemeSetResource,  128)
                            .AddParameter_safe("@EndemeSetVersion" , endemeSet.EndemeSetVersion ,   32)
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeSet: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeSetId " + endemeSet.EndemeSetId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeSet table

        #region EndemeProfile table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeProfile table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeProfileTable> AtEndemeProfileTableOfEndemeProfile   (Guid     endemeProfileId , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfEndemeProfile   (endemeProfileId , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataLabel       (string   dataLabel       , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataLabel       (dataLabel       , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataValueString (string   dataValueString , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataValueString (dataValueString , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataTableName   (string   dataTableName   , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataTableName   (dataTableName   , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataRow         (int      dataRowId       , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataRow         (dataRowId       , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataColumnName  (string   dataColumnName  , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataColumnName  (dataColumnName  , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfProfileMatch    (double   profileMatch    , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfProfileMatch    (profileMatch    , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfProfileTbdColumn(string   profileTbdColumn, InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfProfileTbdColumn(profileTbdColumn, aspect)); }
        public void               ExEndemeProfileTable    (Guid               endemeProfileId, InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM "+ENDEME_PROFILE_TABLE+" WHERE EndemeProfileId = '" + endemeProfileId + "'", aspect.MainConn); }
        public RichDataTable      IntoEndemeProfileTable  (EndemeProfileTable endemeProfileTable  , InfoAspect aspect) { if (!IsEndemeProfileTable(endemeProfileTable.EndemeProfileId, aspect)) ToEndemeProfileTable(endemeProfileTable, aspect); return InEndemeProfileTable(endemeProfileTable.EndemeProfileId, aspect); }
        public EndemeProfileTable OnInEndemeProfileTable  (Guid               endemeProfileId, InfoAspect aspect) { return OnEndemeProfileTable(InEndemeProfileTable  (endemeProfileId, aspect), 0, true); }
        public EndemeProfileTable OnIntoEndemeProfileTable(EndemeProfileTable endemeProfileTable  , InfoAspect aspect) { return OnEndemeProfileTable(IntoEndemeProfileTable(endemeProfileTable  , aspect), 0, true); }
        public Guid               UpToEndemeProfileTable  (EndemeProfileTable endemeProfileTable  , InfoAspect aspect) { if (IsEndemeProfileTable(endemeProfileTable.EndemeProfileId, aspect)) UpEndemeProfileTable(endemeProfileTable, aspect); else ToEndemeProfileTable(endemeProfileTable, aspect); return endemeProfileTable.EndemeProfileId; }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeProfileTable_test -->
        /// <summary>
        ///      Tests the ToEndemeProfileTable method
        /// </summary>
        public void ToEndemeProfileTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeProfileTableAccess";
            string adoMethod = "ToEndemeProfileTable";
            Random r = RandomSource.New().Random;
            Guid    endemeProfileId = Guid.Empty;
            EndemeAccess ende = new EndemeAccess("");

            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeProfileTable endemeProfileTableTo = ende.AnneEndemeProfileTable(r);
            endemeProfileId = endemeProfileTableTo.EndemeProfileId;
            try
            {
                ende.ToEndemeProfileTable(endemeProfileTableTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeProfileId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeProfileTable endemeProfileTableFrom = ende.OnInEndemeProfileTable(endemeProfileId, _aspect);
            ende.AssertEqualContent(endemeProfileTableFrom, endemeProfileTableTo, adoClass, adoMethod);


            ende.ExEndemeProfileTable(endemeProfileId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeProfileTable_test -->
        /// <summary>
        ///      Tests the UpEndemeProfileTable method
        /// </summary>
        public void UpEndemeProfileTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass                        = "EndemeProfileTableAccess";
            string adoMethod                       = "UpEndemeProfileTable";
            Random r                        = RandomSource.New().Random;
            EndemeProfileTable endemeProfileTable1 = null;

            EndemeAccess ende = new EndemeAccess("");
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeProfileId with a newly created endemeProfileTable
                // ----------------------------------------------------------------------
                Guid    endemeProfileId = ende.AnIdOfEndemeProfileTable(_aspect);
                endemeProfileTable1                      = ende.OnInEndemeProfileTable(endemeProfileId, _aspect);
                EndemeProfileTable  endemeProfileTable2  = ende.AnneEndemeProfileTable(r);
                endemeProfileTable2.EndemeProfileId = endemeProfileTable1.EndemeProfileId;
                ende.UpEndemeProfileTable(endemeProfileTable2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeProfileTable endemeProfileTable3 = ende.OnInEndemeProfileTable(endemeProfileId, _aspect);
                Assert.That(endemeProfileTable3.EndemeProfileId, Is.equal_to, endemeProfileTable2.EndemeProfileId, adoClass, adoMethod + " test update existing endemeProfileTable");
                ende.AssertEqualContent  (endemeProfileTable3, endemeProfileTable2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeProfileTable3, endemeProfileTable1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeProfileTable, did the update fail?
                // ----------------------------------------------------------------------
                EndemeProfileTable  endemeProfileTable4 = ende.AnneEndemeProfileTable(r);
                endemeProfileTable4.EndemeProfileId     = Guid.NewGuid();
                int                 count               = ende.UpEndemeProfileTable(endemeProfileTable4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeProfileTable");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeProfileTable(endemeProfileTable1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeProfileTable -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeProfileTable table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public Guid AnIdOfEndemeProfileTable(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT EndemeProfileId FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeProfileTableId = new RichDataTable(cmd, null, "EndemeProfileTableId", "EndemeProfileId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeProfileTableId.Count > 0) return (Guid)endemeProfileTableId.ToList("EndemeProfileId", Guid.NewGuid())[r.Next(endemeProfileTableId.Count)];
                else return Guid.NewGuid();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeProfileTable -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeProfileTable object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeProfileTable</returns>
        private EndemeProfileTable AnneEndemeProfileTable(Random r)
        {
            EndemeProfileTable endemeProfileTable = new EndemeProfileTable
                { EndemeProfileId  = Guid.NewGuid()
                , DataLabel        = r.Next(10000).ToString()
                , DataValueString  = r.Next(10000).ToString()
                , DataTableName    = r.Next(10000).ToString()
                , DataRowId        = r.Next(10000)
                , DataColumnName   = r.Next(10000).ToString()
                , ProfileMatch     = r.Next(10000)
                , ProfileTbdColumn = r.Next(10000).ToString()
                };
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable being tested</param>
        /// <param name="tgt"      >endemeProfileTable being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeProfileTable endemeProfileTable, EndemeProfileTable tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeProfileTable.DataLabel       , Is.equal_to, tgt.DataLabel       , adoClass, adoMethod + " DataLabel"       );
            Assert.That(endemeProfileTable.DataValueString , Is.equal_to, tgt.DataValueString , adoClass, adoMethod + " DataValueString" );
            Assert.That(endemeProfileTable.DataTableName   , Is.equal_to, tgt.DataTableName   , adoClass, adoMethod + " DataTableName"   );
            Assert.That(endemeProfileTable.DataRowId       , Is.equal_to, tgt.DataRowId       , adoClass, adoMethod + " DataRowId"       );
            Assert.That(endemeProfileTable.DataColumnName  , Is.equal_to, tgt.DataColumnName  , adoClass, adoMethod + " DataColumnName"  );
            Assert.That(endemeProfileTable.ProfileMatch    , Is.equal_to, tgt.ProfileMatch    , adoClass, adoMethod + " ProfileMatch"    );
            Assert.That(endemeProfileTable.ProfileTbdColumn, Is.equal_to, tgt.ProfileTbdColumn, adoClass, adoMethod + " ProfileTbdColumn");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable being tested</param>
        /// <param name="tgt"      >endemeProfileTable being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeProfileTable endemeProfileTable, EndemeProfileTable tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeProfileTable.DataLabel       , Is.not_equal_to, tgt.DataLabel       , adoClass, adoMethod + " DataLabel"       );
            Assert.That(endemeProfileTable.DataValueString , Is.not_equal_to, tgt.DataValueString , adoClass, adoMethod + " DataValueString" );
            Assert.That(endemeProfileTable.DataTableName   , Is.not_equal_to, tgt.DataTableName   , adoClass, adoMethod + " DataTableName"   );
            Assert.That(endemeProfileTable.DataRowId       , Is.not_equal_to, tgt.DataRowId       , adoClass, adoMethod + " DataRowId"       );
            Assert.That(endemeProfileTable.DataColumnName  , Is.not_equal_to, tgt.DataColumnName  , adoClass, adoMethod + " DataColumnName"  );
            Assert.That(endemeProfileTable.ProfileMatch    , Is.not_equal_to, tgt.ProfileMatch    , adoClass, adoMethod + " ProfileMatch"    );
            Assert.That(endemeProfileTable.ProfileTbdColumn, Is.not_equal_to, tgt.ProfileTbdColumn, adoClass, adoMethod + " ProfileTbdColumn");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeProfileTable -->
        /// <summary>
        ///     Returns a list of EndemeProfileTable objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeProfileTable objects</returns>
        public List<EndemeProfileTable> AtEndemeProfileTable(RichDataTable table)
        {
            List<EndemeProfileTable> list = new List<EndemeProfileTable>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeProfileTable(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeProfileTable -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeProfileTable objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeProfileTable objects"></param>
        public List<EndemeProfileTable> AtOnMareEndemeProfileTable(RichDataTable table)
        {
            List<EndemeProfileTable> endemeProfileTableList = new List<EndemeProfileTable>(table.Count);
            Dictionary<int,EndemeProfileTable> found = new Dictionary<int,EndemeProfileTable>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeProfileId = table.IntValue(row, "EndemeProfileId", -1);
                EndemeProfileTable endemeProfileTable = null;

                if (!found.ContainsKey(endemeProfileId))
                {
                    endemeProfileTable = OnEndemeProfileTable(table, row);

                    endemeProfileTable.EndemeCharacteristicList = new List<EndemeCharacteristic>();
                    endemeProfileTableList.Add(endemeProfileTable);
                    found.Add(endemeProfileId, endemeProfileTable);
                }
                else
                {
                    endemeProfileTable = found[endemeProfileId];
                }

                endemeProfileTable.EndemeCharacteristicList.Add((new EndemeAccess(ConnectionString)).OnEndemeCharacteristic(table, row));
            }

            return endemeProfileTableList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeProfileTable -->
        /// <summary>
        ///      Copies an endemeProfileTable
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable to copy</param>
        public static EndemeProfileTable CpEndemeProfileTable(EndemeProfileTable endemeProfileTable)
        {
            EndemeProfileTable output = new EndemeProfileTable();

            output.EndemeProfileId  = endemeProfileTable.EndemeProfileId;
            output.DataLabel        = endemeProfileTable.DataLabel;
            output.DataValueString  = endemeProfileTable.DataValueString;
            output.DataTableName    = endemeProfileTable.DataTableName;
            output.DataRowId        = endemeProfileTable.DataRowId;
            output.DataColumnName   = endemeProfileTable.DataColumnName;
            output.ProfileMatch     = endemeProfileTable.ProfileMatch;
            output.ProfileTbdColumn = endemeProfileTable.ProfileTbdColumn;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeProfileTableCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeProfile table
        /// </summary>
        /// <returns>a count of rows in the EndemeProfile table</returns>
        public int EndemeProfileTableCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT COUNT(*) FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = TreatAs.IntValue(cmd.ExecuteScalar(-1), -1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeProfileTable: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EnEndemeProfileTable -->
        /// <summary>
        ///      (En)ables an EndemeProfileTable
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        public void EnEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_PROFILE_TABLE+" SET N/A = 1 WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeProfileId", endemeProfileId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeProfileTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeProfileTable -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemeprofile table
        /// </summary>
        /// <returns>Maximum EndemeProfile.EndemeProfileId</returns>
//      public Guid HiIdOfEndemeProfileTable(InfoAspect aspect)
//      {
//          RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
//              , "SELECT MAX(EndemeProfileId) AS EndemeProfileId WITH(NOLOCK) FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK)"
//              , Throws.Actions, "PR");


//          Guid endemeProfileId = cmd.ExecuteScalar(Guid.NewGuid());
//          if (__.StringContainsStuff(cmd.Errors))
//              throw new ApplicationException("HiIdOfEndemeProfileTable: " + cmd.Errors);
//          return endemeProfileId;
//      }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTable -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeProfile table
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeProfileId"  , endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfEndemeProfile -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the EndemeProfileId column
        /// </summary>
        /// <param name="endemeProfileId">value in EndemeProfileId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeProfileId</returns>
        public RichDataTable InEndemeProfileTableOfEndemeProfile(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeProfileId" , endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataLabel column
        /// </summary>
        /// <param name="dataLabel">value in DataLabel column</param>
        /// <returns>a table of rows related to the specifed value of DataLabel</returns>
        public RichDataTable InEndemeProfileTableOfDataLabel(string dataLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE DataLabel = @DataLabel"
                , Throws.Actions, "PR")
                .AddParameter_safe("@DataLabel" , dataLabel);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataValueString -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataValueString column
        /// </summary>
        /// <param name="dataValueString">value in DataValueString column</param>
        /// <returns>a table of rows related to the specifed value of DataValueString</returns>
        public RichDataTable InEndemeProfileTableOfDataValueString(string dataValueString, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE DataValueString = @DataValueString"
                , Throws.Actions, "PR")
                .AddParameter_safe("@DataValueString" , dataValueString);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataTableName -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataTableName column
        /// </summary>
        /// <param name="dataTableName">value in DataTableName column</param>
        /// <returns>a table of rows related to the specifed value of DataTableName</returns>
        public RichDataTable InEndemeProfileTableOfDataTableName(string dataTableName, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE DataTableName = @DataTableName"
                , Throws.Actions, "PR")
                .AddParameter_safe("@DataTableName" , dataTableName);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataRow -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataRowId column
        /// </summary>
        /// <param name="dataRowId">value in DataRowId column</param>
        /// <returns>a table of rows related to the specifed value of DataRowId</returns>
        public RichDataTable InEndemeProfileTableOfDataRow(int dataRowId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE DataRowId = @DataRowId"
                , Throws.Actions, "PR")
                .AddParameter_null("@DataRowId" , dataRowId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataColumnName -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataColumnName column
        /// </summary>
        /// <param name="dataColumnName">value in DataColumnName column</param>
        /// <returns>a table of rows related to the specifed value of DataColumnName</returns>
        public RichDataTable InEndemeProfileTableOfDataColumnName(string dataColumnName, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE DataColumnName = @DataColumnName"
                , Throws.Actions, "PR")
                .AddParameter_safe("@DataColumnName" , dataColumnName);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfProfileMatch -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the ProfileMatch column
        /// </summary>
        /// <param name="profileMatch">value in ProfileMatch column</param>
        /// <returns>a table of rows related to the specifed value of ProfileMatch</returns>
        public RichDataTable InEndemeProfileTableOfProfileMatch(double profileMatch, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE ProfileMatch = @ProfileMatch"
                , Throws.Actions, "PR")
                .AddParameter_null("@ProfileMatch" , profileMatch);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfProfileTbdColumn -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the ProfileTbdColumn column
        /// </summary>
        /// <param name="profileTbdColumn">value in ProfileTbdColumn column</param>
        /// <returns>a table of rows related to the specifed value of ProfileTbdColumn</returns>
        public RichDataTable InEndemeProfileTableOfProfileTbdColumn(string profileTbdColumn, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE ProfileTbdColumn = @ProfileTbdColumn"
                , Throws.Actions, "PR")
                .AddParameter_safe("@ProfileTbdColumn" , profileTbdColumn);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf8EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeProfileTable -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeProfile table
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                    , "SELECT * FROM "+ENDEME_PROFILE_TABLE+" WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                    , Throws.Actions, "P")
                    .AddParameter("@EndemeProfileId", endemeProfileId);


                using (RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeProfileTable with EndemeProfileId " + endemeProfileId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }
            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeProfileTable -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeProfileTable
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <returns>a table of endemeProfileTable rows with their joined parent data</returns>
        public RichDataTable MainEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT n/a.*, ep.*"
                + "\r\n" + " FROM                dbo.EndemeProfile AS ep  WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN dbo.N/A           AS n/a WITH(NOLOCK) ON n/a.n/a = ep.n/a"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ep.EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeProfileId", endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeProfileTable -->
        /// <summary>
        ///      Creates a (Ne)w EndemeProfileTable (o)bject from member values
        /// </summary>
        /// <param name="endemeProfileId" ></param>
        /// <param name="dataLabel"       ></param>
        /// <param name="dataValueString" ></param>
        /// <param name="dataTableName"   ></param>
        /// <param name="dataRowId"       ></param>
        /// <param name="dataColumnName"  ></param>
        /// <param name="profileMatch"    ></param>
        /// <param name="profileTbdColumn"></param>
        /// <returns>the new EndemeProfileTable object</returns>
        public EndemeProfileTable NeonEndemeProfileTable
            ( Guid     endemeProfileId
            , string   dataLabel
            , string   dataValueString
            , string   dataTableName
            , int      dataRowId
            , string   dataColumnName
            , double   profileMatch
            , string   profileTbdColumn
            )
        {
            EndemeProfileTable endemeProfileTable = new EndemeProfileTable
                { EndemeProfileId  = endemeProfileId
                , DataLabel        = dataLabel
                , DataValueString  = dataValueString
                , DataTableName    = dataTableName
                , DataRowId        = dataRowId
                , DataColumnName   = dataColumnName
                , ProfileMatch     = profileMatch
                , ProfileTbdColumn = profileTbdColumn
                };
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeProfileTable -->
        /// <summary>
        ///      Converts a row in the EndemeProfile data table into a EndemeProfileTable (O)bject
        /// </summary>
        /// <param name="endemeProfileTableTable">a table containing columns to build a batch object</param>
        /// <param name="row"                    >the row to convert</param>
        /// <param name="dispose"                >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeProfileTable OnEndemeProfileTable(RichDataTable endemeProfileTableTable, int row, bool dispose = false)
        {
            EndemeProfileTable endemeProfileTable = new EndemeProfileTable();
            endemeProfileTable.EndemeProfileId  = endemeProfileTableTable.GuidValue(row, "EndemeProfileId" , Guid.Empty  );
            endemeProfileTable.DataLabel        = endemeProfileTableTable.StrValue (row, "DataLabel"       , ""          );
            endemeProfileTable.DataValueString  = endemeProfileTableTable.StrValue (row, "DataValueString" , ""          );
            endemeProfileTable.DataTableName    = endemeProfileTableTable.StrValue (row, "DataTableName"   , ""          );
            endemeProfileTable.DataRowId        = endemeProfileTableTable.IntValue (row, "DataRowId"       , -1          );
            endemeProfileTable.DataColumnName   = endemeProfileTableTable.StrValue (row, "DataColumnName"  , ""          );
            endemeProfileTable.ProfileMatch     = endemeProfileTableTable.RealValue(row, "ProfileMatch"    , -1.0        );
            endemeProfileTable.ProfileTbdColumn = endemeProfileTableTable.StrValue (row, "ProfileTbdColumn", ""          );

            if (dispose) endemeProfileTableTable.Dispose();
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeProfileTable -->
        /// <summary>
        ///      Returns a table of a EndemeProfileTable (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <returns>a table of EndemeProfileTable rows with their joined details</returns>
        public RichDataTable ReinEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " SELECT ep.*, ec.*"
                + "\r\n" + " FROM                dbo.EndemeProfile        AS ep WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "+ENDEME_CHAR_TABLE+" AS ec WITH(NOLOCK) ON ec.EndemeProfileId = ep.EndemeProfileId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND ep.EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeProfileId", endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, null, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeProfileTable -->
        /// <summary>
        ///      Inserts an endemeProfile object in(To) the database
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable to insert into database</param>
        /// <returns>the Id of the inserted EndemeProfile</returns>
        public int ToEndemeProfileTable(EndemeProfileTable endemeProfile, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "\r\n" + " INSERT INTO "+ENDEME_PROFILE_TABLE
                + "\r\n" + "        ( EndemeProfileId"
                + "\r\n" + "        , DataLabel"
                + "\r\n" + "        , DataValueString"
                + "\r\n" + "        , DataTableName"
                + "\r\n" + "        , DataRowId"
                + "\r\n" + "        , DataColumnName"
                + "\r\n" + "        , ProfileMatch"
                + "\r\n" + "        , ProfileTbdColumn"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeProfileId"
                + "\r\n" + " VALUES ( @EndemeProfileId"
                + "\r\n" + "        , @DataLabel"
                + "\r\n" + "        , @DataValueString"
                + "\r\n" + "        , @DataTableName"
                + "\r\n" + "        , @DataRowId"
                + "\r\n" + "        , @DataColumnName"
                + "\r\n" + "        , @ProfileMatch"
                + "\r\n" + "        , @ProfileTbdColumn"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                .AddParameter     ("@EndemeProfileId" , endemeProfile.EndemeProfileId                     )
                .AddParameter_safe("@DataLabel"       , endemeProfile.DataLabel       ,  128)
                .AddParameter_safe("@DataValueString" , endemeProfile.DataValueString ,   -1)
                .AddParameter_safe("@DataTableName"   , endemeProfile.DataTableName   ,  128)
                .AddParameter_null("@DataRowId"       , endemeProfile.DataRowId                           )
                .AddParameter_safe("@DataColumnName"  , endemeProfile.DataColumnName  ,  128)
                .AddParameter_null("@ProfileMatch"    , endemeProfile.ProfileMatch                        )
                .AddParameter_safe("@ProfileTbdColumn", endemeProfile.ProfileTbdColumn,   50);


            int endemeProfileId = TreatAs.IntValue(cmd.ExecuteScalar(-1),-2);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeProfileTable: " + cmd.Errors);
            return endemeProfileId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeProfileTable -->
        /// <summary>
        ///     Inserts in(To) the EndemeProfileTable table a (ne)w endemeProfileTable built from member values
        /// </summary>
        /// <param name="endemeProfileId" ></param>
        /// <param name="dataLabel"       ></param>
        /// <param name="dataValueString" ></param>
        /// <param name="dataTableName"   ></param>
        /// <param name="dataRowId"       ></param>
        /// <param name="dataColumnName"  ></param>
        /// <param name="profileMatch"    ></param>
        /// <param name="profileTbdColumn"></param>
        /// <returns>the new EndemeProfileTable object</returns>
        public EndemeProfileTable ToneEndemeProfileTable
            ( Guid     endemeProfileId
            , string   dataLabel
            , string   dataValueString
            , string   dataTableName
            , int      dataRowId
            , string   dataColumnName
            , double   profileMatch
            , string   profileTbdColumn
            , InfoAspect aspect)
        {
            EndemeProfileTable endemeProfileTable = NeonEndemeProfileTable
                ( endemeProfileId
                , dataLabel
                , dataValueString
                , dataTableName
                , dataRowId
                , dataColumnName
                , profileMatch
                , profileTbdColumn
                );
            ToEndemeProfileTable(endemeProfileTable, aspect);
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeProfileTable -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeProfileTable
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <param name="disableValue">the value meaning diable</param>
        public void UnEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                , "UPDATE "+ENDEME_PROFILE_TABLE+" SET N/A = 0 WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                .AddParameter("@EndemeProfileId", endemeProfileId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeProfileTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeProfileTable -->
        /// <summary>
        ///      (Up)dates a row in the EndemeProfile table from a EndemeProfileTable object
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable to update</param>
        /// <returns>the count of the updated endemeProfileTable rows"></param>
        public int UpEndemeProfileTable(EndemeProfileTable endemeProfileTable, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeProfileId = " + endemeProfileTable.EndemeProfileId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(ENDEME_PROFILE_TABLE, whereClause, aspect.MainConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.MainConn
                            , "\r\n" + " UPDATE "+ENDEME_PROFILE_TABLE
                            + "\r\n" + " SET   DataLabel        = @DataLabel"
                            + "\r\n" + "     , DataValueString  = @DataValueString"
                            + "\r\n" + "     , DataTableName    = @DataTableName"
                            + "\r\n" + "     , DataRowId        = @DataRowId"
                            + "\r\n" + "     , DataColumnName   = @DataColumnName"
                            + "\r\n" + "     , ProfileMatch     = @ProfileMatch"
                            + "\r\n" + "     , ProfileTbdColumn = @ProfileTbdColumn"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            .AddParameter     ("@EndemeProfileId" , endemeProfileTable.EndemeProfileId                     )
                            .AddParameter_safe("@DataLabel"       , endemeProfileTable.DataLabel       ,  128)
                            .AddParameter_safe("@DataValueString" , endemeProfileTable.DataValueString ,   -1)
                            .AddParameter_safe("@DataTableName"   , endemeProfileTable.DataTableName   ,  128)
                            .AddParameter_null("@DataRowId"       , endemeProfileTable.DataRowId                           )
                            .AddParameter_safe("@DataColumnName"  , endemeProfileTable.DataColumnName  ,  128)
                            .AddParameter_null("@ProfileMatch"    , endemeProfileTable.ProfileMatch                        )
                            .AddParameter_safe("@ProfileTbdColumn", endemeProfileTable.ProfileTbdColumn,   50)
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeProfileTable: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeProfileId " + endemeProfileTable.EndemeProfileId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeProfileTable table

    }
}