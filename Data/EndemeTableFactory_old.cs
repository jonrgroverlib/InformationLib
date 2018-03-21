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
using InformationLib.Data    ;        // for many
using InformationLib.Endemes ;        // for many
using InformationLib.Micro   ;        // for Throws
using InformationLib.SoftData;        // for TreatAs.IntValue
using InformationLib.Strings ;        // for __.DoubleQuotes, __.Truncate
using System;                         // for 
using System.Collections;             // for 
using System.Collections.Generic;     // for 
using System.Data;                    // for 
using System.Data.SqlClient;
using System.Diagnostics;             // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
	// ---------------------------------------------------------------------------------------------
	/// <!-- EndemeTableFactory -->
	/// <summary>
	///      The EndemeTableFactory class performs direct interactions with the Knowledge database or excel file (see Standard)
	/// </summary>
	/// <remarks>
	///      Includes:
	///       - schema and table builders
	///       - endeme set managers
	///       - endeme managers
	///       - endeme member managers
	/// 
	/// 
	///      Namespace Dependency Hierarchy:
	///      
	/// 
	///           Data Apps        Info Apps                  (application types)
	///               |                |
	///               |                |
	///               |            InfoAccess
	///               |                |
	///          +----+----+  +--------+----------+           (advanced namespaces)
	///          |         |  |        |          |
	///          |      DataAccess     |          |
	///          |          |          |          |
	///          +----------+----------+          |
	///          |          |          |          |
	///      SoftData    Strings    Endemes    FuzzyLogic     (primitive namespaces)
	///          |          |          |          |
	///          +----------+-----+----+----------+
	///                           |
	///                        System.*
	/// 
    /// 
	///      alpha code - nearly beta
    /// </remarks>
	public class EndemeTableFactory_old
	{
        // -----------------------------------------------------------------------------------------
        //  Members
        // -----------------------------------------------------------------------------------------
        public static string SetIdColumn { get { return m_setIdColumn; } } private static string m_setIdColumn = "EndemeSetID"; // I'm going to want to change this at some point and make it consistent across all endeme tables:: EndemeSetID


		// -----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeCharacteristicTable -->
		/// <summary>
		/// EndemeCharacteristic
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeCharacteristicTable(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + ""
                + "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeCharacteristic]("
                + "\r\n" + "       CharId         uniqueidentifier NOT NULL"
                + "\r\n" + "     , SetId          uniqueidentifier     NULL"
                + "\r\n" + "     , Letter         char(1)              NULL"
                + "\r\n" + "     , Code           varchar(8)           NULL"
                + "\r\n" + "     , Label          varchar(64)          NULL"
                + "\r\n" + "     , Descr          nvarchar(255)        NULL"
                + "\r\n" + "     , OppositeLetter char(1)              NULL"
                + "\r\n" + "     , OppositeCode   varchar(8)           NULL"
                + "\r\n" + "     , OppositeLabel  varchar(64)          NULL"
                + "\r\n" + "     , OppositeDescr  nvarchar(255)        NULL"
                + "\r\n" + "     , NominalValue   smallint             NULL"
                + "\r\n" + "     , Units          nvarchar(64)         NULL"
                + "\r\n" + "     , Finickiness    smallint             NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeCharacteristic] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [CharId] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");
			cmd.ExecuteNonQuery();
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeCharValueTable -->
		/// <summary>
		///      Builds the EndemeCharValue table
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeCharValueTable_old(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + ""
                + "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeCharValue]("
                + "\r\n" + "       CharId      uniqueidentifier NOT NULL"
                + "\r\n" + "     , EndemeKey   uniqueidentifier NOT NULL"
                + "\r\n" + "     , Code        varchar(8)           NULL"
                + "\r\n" + "     , Label       varchar(64)          NULL"
                + "\r\n" + "     , Descr       nvarchar(255)        NULL"
                + "\r\n" + "     , DataNumber  real                 NULL"
                + "\r\n" + "     , DataText    varchar(255)         NULL"
                + "\r\n" + "     , DataEndeme  uniqueidentifier     NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeCharValue] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [CharId] ASC,"
                + "\r\n" + "     [EndemeKey] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");
			cmd.ExecuteNonQuery();
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeIndexTable -->
		/// <summary>
		///      Builds the EndemeIndex table,
		///      normally this table is only accessed by triggers in Sql Server onthe Endeme table
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeIndexTable(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + ""
                + "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeIndex]("
                + "\r\n" + "       KeyId         smallint         NOT NULL"
                + "\r\n" + "     , EndemeKey     uniqueidentifier NOT NULL"
                + "\r\n" + "     , MatchStrength smallint             NULL"
                + "\r\n" + "     , Program       varchar(64)          NULL"
                + "\r\n" + "     , SetId         uniqueidentifier     NULL"
                + "\r\n" + "     , ReverseYN     char(1)              NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeIndex] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [KeyId] ASC,"
                + "\r\n" + "     [EndemeKey] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");
			cmd.ExecuteNonQuery();
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeIndexKeyTable -->
		/// <summary>
		///      Builds the EndemeIndexKey table,
		///      normally this table is only accessed by triggers in Sql Server onthe Endeme table
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeIndexKeyTable(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + "" + "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeIndexKey]("
                + "\r\n" + "       KeyId       smallint         NOT NULL"
                + "\r\n" + "     , MatchTo     uniqueidentifier     NULL"
                + "\r\n" + "     , Length      smallint             NULL"
                + "\r\n" + "     , FirstLetter char(1)              NULL"
                + "\r\n" + "     , ReverseYN   char(1)              NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeIndexKey_1] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [KeyId] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");
			cmd.ExecuteNonQuery();
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeMeaningTable -->
		/// <summary>
		///      Builds the EndemeMeaning table which contains meaning for the various positions in
		///      an endeme characteristic
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeMeaningTable_old(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + ""
                + "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeMeaning]"
                + "\r\n" + "      ( MeaningId       uniqueidentifier  NOT NULL"
                + "\r\n" + "      , CharId          uniqueidentifier  NOT NULL"
                + "\r\n" + "      , Position        smallint          NULL"
                + "\r\n" + "      , TextMeaning     nvarchar(255)     NULL"
                + "\r\n" + "      , NumericMeaning  real              NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeMeaning] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [MeaningId] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");
			cmd.ExecuteNonQuery();
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeListItemTable -->
		/// <summary>
		///      Builds the EndemeListItem table
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeListItemTable_old(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + ""
                + "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeListItem]("
                + "\r\n" + "       ItemId    uniqueidentifier NOT NULL"
                + "\r\n" + "     , ListId    uniqueidentifier NOT NULL"
                + "\r\n" + "     , ListOrder int              NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeListItem] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [ItemId] ASC,"
                + "\r\n" + "     [ListId] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");
			cmd.ExecuteNonQuery();
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeMemberTable -->
		/// <summary>
		/// EndemeMember
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>, old</remarks>
		public static void BuildEndemeMemberTable_old(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + ""
                + "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeMember]("
                + "\r\n" + "       UserId          uniqueidentifier NOT NULL"
                + "\r\n" + "     , OrganizationId  uniqueidentifier NOT NULL"
                + "\r\n" + "     , DateJoined      datetime             NULL"
                + "\r\n" + "     , DateLeft        datetime             NULL"
                + "\r\n" + "     , Role            uniqueidentifier     NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeMember] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [UserId] ASC,"
                + "\r\n" + "     [OrganizationId] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");
			cmd.ExecuteNonQuery();
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeOrganizationTable -->
		/// <summary>
		/// EndemeOrganization
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeOrganizationTable_old(string conn)
		{
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeOrganization]("
                + "\r\n" + "       OrganizationId    uniqueidentifier  NOT NULL"
                + "\r\n" + "     , OrganizationName  nvarchar(128)         NULL"
                + "\r\n" + "     , Descr             nvarchar(256)         NULL"
                + "\r\n" + "     , Parent            uniqueidentifier      NULL"
                + "\r\n" + "     , Profile           uniqueidentifier      NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeOrganization] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [OrganizationId] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");

			cmd.ExecuteNonQuery();
		}

        // -----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeSetTable -->
		/// <summary>
		///      Builds the EndemeSet table if there is not one already in the database
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeSetTable(string conn)
		{
			if (!RichDataTable.TableExists(conn, "EndemeSet"))
            {
				RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                    , "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeSet]("
                    + "\r\n" + "       SetId      uniqueidentifier NOT NULL"
                    + "\r\n" + "     , Name       varchar(64)          NULL"
                    + "\r\n" + "     , Version    smallint             NULL"
                    + "\r\n" + "     , Descr      nvarchar(255)        NULL"
                    + "\r\n" + "     , Cutoff     smallint             NULL"
                    + "\r\n" + "     , Resource   nvarchar(255)        NULL"
                    + "\r\n" + "     , Popularity int                  NULL"
                    + "\r\n" + "     , Owner      uniqueidentifier     NULL"
                    + "\r\n" + "     , Editing    uniqueidentifier     NULL"
                    + "\r\n" + "     , Parent     uniqueidentifier     NULL"
                    + "\r\n" + "  CONSTRAINT [PK_EndemeSet] PRIMARY KEY CLUSTERED "
                    + "\r\n" + " ("
                    + "\r\n" + "     [SetId] ASC"
                    + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                    + "\r\n" + " ) ON [PRIMARY]"
                    , Throws.Actions, "P");

                cmd.ExecuteNonQuery();
			}
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeTable -->
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>, old</remarks>
		public static void BuildEndemeTable(string conn)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".Endeme("
                + "\r\n" + "       EndemeId      uniqueidentifier NOT NULL"
                + "\r\n" + "     , SetId         uniqueidentifier     NULL"
                + "\r\n" + "     , AttributeFor  uniqueidentifier     NULL"
                + "\r\n" + "     , CharString    varchar(24)      NOT NULL"
                + "\r\n" + "     , Descr         nvarchar(255)        NULL"
                + "\r\n" + "     , Label         varchar(64)          NULL"
                + "\r\n" + "     , Jumble        uniqueidentifier     NULL"
                + "\r\n" + "     , Link          uniqueidentifier     NULL"
                + "\r\n" + "     , MatchWeight   smallint             NULL"
                + "\r\n" + "     , Parent        uniqueidentifier     NULL"
                + "\r\n" + "     , Usage         char(1)              NULL"
                + "\r\n" + "     , OrdinalIndex  varchar(24)          NULL"
                + "\r\n" + "     , GridIndex     int                  NULL"
                + "\r\n" + "     , HelperIndex   int                  NULL"
                + "\r\n" + "     , Version       varchar(8)           NULL"
                + "\r\n" + "     , DataText      nvarchar(4000)       NULL"
                + "\r\n" + "     , DataNumber    real                 NULL"
                + "\r\n" + "     , DataDateTime  datetime             NULL"
                + "\r\n" + "     , DataEndeme    uniqueidentifier     NULL"
                + "\r\n" + "     , DataTable     varchar(64)          NULL"
                + "\r\n" + "     , DataRow       uniqueidentifier     NULL"
                + "\r\n" + "     , DataColumn    varchar(64)          NULL"
                + "\r\n" + "     , A smallint NULL"
                + "\r\n" + "     , B smallint NULL"
                + "\r\n" + "     , C smallint NULL"
                + "\r\n" + "     , D smallint NULL"
                + "\r\n" + "     , E smallint NULL"
                + "\r\n" + "     , F smallint NULL"
                + "\r\n" + "     , G smallint NULL"
                + "\r\n" + "     , H smallint NULL"
                + "\r\n" + "     , I smallint NULL"
                + "\r\n" + "     , J smallint NULL"
                + "\r\n" + "     , K smallint NULL"
                + "\r\n" + "     , L smallint NULL"
                + "\r\n" + "     , M smallint NULL"
                + "\r\n" + "     , N smallint NULL"
                + "\r\n" + "     , O smallint NULL"
                + "\r\n" + "     , P smallint NULL"
                + "\r\n" + "     , Q smallint NULL"
                + "\r\n" + "     , R smallint NULL"
                + "\r\n" + "     , S smallint NULL"
                + "\r\n" + "     , T smallint NULL"
                + "\r\n" + "     , U smallint NULL"
                + "\r\n" + "     , V smallint NULL"
                + "\r\n" + "     , W smallint NULL"
                + "\r\n" + "     , X smallint NULL"
                + "\r\n" + "  CONSTRAINT [PK_Endeme] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [EndemeKey] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");

			cmd.ExecuteNonQuery();
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- BuildEndemeUserTable -->
		/// <summary>
		/// EndemeUser
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>TODO: write something to check for the existence of the table, old</remarks>
		public static void BuildEndemeUserTable_old(string conn)
		{
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " CREATE TABLE "+EndemeAccess.EnSchema+".[EndemeUser]("
                + "\r\n" + "       UserId           uniqueidentifier NOT NULL"
                + "\r\n" + "     , GivenName        nvarchar(32)         NULL"
                + "\r\n" + "     , FamilyName       nvarchar(32)         NULL"
                + "\r\n" + "     , UserName         varchar(32)          NULL"
                + "\r\n" + "     , Password         varchar(16)          NULL"
                + "\r\n" + "     , CognitiveProfile uniqueidentifier     NULL"
                + "\r\n" + "     , Email            nvarchar(512)        NULL"
                + "\r\n" + "  CONSTRAINT [PK_EndemeUser] PRIMARY KEY CLUSTERED "
                + "\r\n" + " ("
                + "\r\n" + "     [UserId] ASC"
                + "\r\n" + " )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
                + "\r\n" + " ) ON [PRIMARY]"
                , Throws.Actions, "P");

			cmd.ExecuteNonQuery();
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- DropEndemeTable -->
		/// <summary>
		///      Drops just the Endeme table
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>, old</remarks>
		public static void DropEndemeTable_old(string conn, EndemeSet exceptionActions, Endeme exceptionAction)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn, " DROP TABLE "+EndemeAccess.EnSchema+".Endeme;", Throws.Actions, exceptionAction);
			cmd.ExecuteNonQuery();
			throw new NotImplementedException();
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- DropEndemeTables -->
		/// <summary>
		///      Drops all of the endeme tables
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks>, old</remarks>
		public static void DropEndemeTables(string conn, EndemeSet actions, Endeme exceptionAction)
		{
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , " DROP TABLE "+EndemeAccess.EnSchema+".Endeme;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeCharacteristic;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeCharValue;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeIndex;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeIndexKey;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeListItem;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeMember;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeOrganization;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeSet;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeUser;"
                + " DROP TABLE "+EndemeAccess.EnSchema+".EndemeMeaning;"
                , Throws.Actions, exceptionAction);
			cmd.ExecuteNonQuery();
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeSetExists -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        public static bool EndemeSetExists_old(string setName, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "SELECT SetId FROM "+EndemeAccess.EnSchema+".EndemeSet WHERE Label = '"+setName+"'"
                , Throws.Actions, "P");
            RichDataTable table = new RichDataTable(cmd, null, "OneSet", SetIdColumn);
            return (table.Count > 0);
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- GetEndemeSet -->
		/// <summary>
		///      Retrieves the entire endeme set from the database given the set Id
        ///      RETRIEVE SET
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="setId"></param>
		/// <returns></returns>
		/// <remarks>, old</remarks>
		private static EndemeSet GetEndemeSet_old(Guid setId, string conn)
		{

			// --------------------------------------------------------------------------
			//  Get the knowledge table
			// --------------------------------------------------------------------------
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " SELECT c.CharId, c.Letter, c.Label, c.Descr, s.Label AS SetName"
                + "\r\n" + " FROM           "+EndemeAccess.EnSchema+".EndemeCharacteristic AS c"
                + "\r\n" + "     INNER JOIN "+EndemeAccess.EnSchema+".EndemeSet            AS s ON s.SetId = c.SetId"
                + "\r\n" + " WHERE   s.SetId   = '" + setId.ToString() + "'"
                , Throws.Actions, "P");
			RichDataTable setCharacteristic = new RichDataTable(cmd, null, "SetCharacteristic", SetIdColumn);


			// --------------------------------------------------------------------------
			//  Build the endeme set
			// --------------------------------------------------------------------------
			EndemeSet outputSet = default(EndemeSet);
			if (setCharacteristic != null && (setCharacteristic.Count > 0)) {
				string setName = setCharacteristic.StrValue(0, "SetName", "");
				outputSet = setCharacteristic.ToEndemeSet(setName, "Letter", "Label", "Descr");
			}
            else
                { outputSet = null; }

			return outputSet;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSet -->
        /// <summary>
        ///      Converts an excel table into an endeme set, if the columns have the names F1, F2, F3...
        ///      Where F3 is the character, F4 is the label and F5 is the description
        ///      RETRIEVE SET     
        /// </summary>
        /// <param name="endemeTable"></param>
        /// <param name="setName">this is actually a regex pattern if you want to do something more complex than just sending the name</param>
        /// <param name="setTitle"></param>
        /// <returns></returns>
        public static EndemeSet GetEndemeSet_old(RichDataTable endemeTable, string setName, string setTitle)
        {
            EndemeSet output = new EndemeSet(setTitle);
            for (int row = 0; row < endemeTable.Count; ++row)
            {
                string name = endemeTable.StrValue(row, "F1", "");
                if (Regex.IsMatch(name, setName, RegexOptions.IgnoreCase))
                {
                    char   letter = endemeTable.CharValue(row, "F3", ' ');
                    string label  = endemeTable.StrValue (row, "F4", "" );
                    string descr  = endemeTable.StrValue (row, "F5", "" );
                    output.Add(letter, label, descr);
                }
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSet -->
        /// <summary>
        ///      RETRIEVE SET
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="worksheet"></param>
        /// <param name="setName"></param>
        /// <param name="setName_2"></param>
        /// <returns></returns>
        public static EndemeSet GetEndemeSet_old(string excelPath, string worksheet, string setName, string setName_2)
        {
            RichDataTable sets = RichDataTable.ExcelToTable(excelPath, worksheet);
            EndemeSet efset = EndemeTableFactory_old.GetEndemeSet_old(sets, setName, setName);
            return efset;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSet -->
        /// <summary>
        ///      RETRIEVE SET
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static EndemeSet GetEndemeSet_old(string setName, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , " SELECT c.CharacteristicLetter, c.CharacteristicLabel, c.CharacteristicDescr"
                + " FROM "+EndemeAccess.EnSchema+".EndemeSet AS s"
                + "     LEFT OUTER JOIN "+EndemeAccess.EnSchema+".EndemeCharacteristic AS c ON c.EndemeSetId = s.EndemeSetId"
                + " WHERE s.EndemeSetLabel = '"+setName+"'"
                , Throws.Actions, "P");
            RichDataTable table = new RichDataTable(cmd, null, "OneSet", SetIdColumn);

            return table.ToEndemeSet(setName, "Letter", "Label", "Descr");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LoadEndemeSet -->
        /// <summary>
        ///      Reads a set out of the database
        ///           RETRIEVE SET
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static EndemeSet GetMostRecentEndemeSet_old(string setName, string conn)
        {
            // --------------------------------------------------------------------------
            //  Get set from database
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , " SELECT s.Label AS SetName, c.Letter, c.Label, c.Descr"
                + " FROM                "+EndemeAccess.EnSchema+".EndemeCharacteristic AS c WITH(NOLOCK)"
                + "     LEFT OUTER JOIN "+EndemeAccess.EnSchema+".EndemeSet            AS s WITH(NOLOCK) ON s.SetId = c.SetId"
                + " WHERE s.SetId IN ("
                + "                   SELECT TOP 1 s.setid"
                + "                   FROM "+EndemeAccess.EnSchema+".EndemeCharacteristic AS c"
                + "                       LEFT OUTER JOIN "+EndemeAccess.EnSchema+".EndemeSet AS s ON s.SetId = c.SetId"
                + "                   WHERE s.Label = '"+setName+"'"
                + "                   GROUP BY s.setid, s.VersionNumber"
                + "                   ORDER BY s.VersionNumber DESC"
                + "                  )"
                + " ORDER BY Letter"
                , Throws.Actions, "IP");
            RichDataTable table = new RichDataTable(cmd, null, "Characteristic", "EndemeCharacteristicID");


            // --------------------------------------------------------------------------
            //  Convert to set
            // --------------------------------------------------------------------------
            EndemeSet output = null;
            if (table.Count > 0)
                output = table.ToEndemeSet(setName, "SetId", "Letter", "Label", "Descr");
            else
                output = new EndemeSet(setName);

            return output;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- GetEndemeSet_old -->
		/// <summary>
		///      Gets an endeme set from a normal table in the database
        ///           RETRIEVE SET
		/// </summary>
		/// <param name="conn">the databse connectin to use</param>
		/// <param name="setName">the endeme set to be retrieved</param>
		/// <param name="tableName">the normal table containing the set</param>
		/// <param name="charColumn">characteristic character column</param>
		/// <param name="labelColumn">characteristic label column</param>
		/// <param name="descrColumn">characteristic description column</param>
		/// <returns></returns>
		/// <remarks>, old</remarks>
		public static EndemeSet GetEndemeSet_old(string conn, string setName, string tableName, string charColumn, string labelColumn, string descrColumn)
		{

			// ---------------------------------------------------------------------------
			//  Build the endeme set from the normal table
			// ---------------------------------------------------------------------------
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn, "SELECT * FROM " + tableName, Throws.Actions, "P");
			RichDataTable setTable = new RichDataTable(cmd, null, tableName, "");
			EndemeSet output = setTable.ToEndemeSet(setName, charColumn, labelColumn, descrColumn);

            return output;
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- GetEndemeSet_old -->
		/// <summary>
		///      Gets an endeme set from a knowledge table in the database
        ///           RETRIEVE SET
		/// </summary>
		/// <param name="conn">the databse connectin to use</param>
		/// <param name="setName">the endeme set to be retrieved</param>
		/// <param name="setVersion">the version of the endeme set</param>
		/// <returns></returns>
		/// <remarks>, old</remarks>
		public static EndemeSet GetEndemeSet_old(string conn, string setName, string setVersion)
		{

			// ---------------------------------------------------------------------------
			//  Get the knowledge table
			// ---------------------------------------------------------------------------
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , " SELECT c.CharId, c.Label, c.Descr, s.Label AS SetName"
                + " FROM           "+EndemeAccess.EnSchema+".EndemeCharacteristic AS c"
                + "     INNER JOIN "+EndemeAccess.EnSchema+".EndemeSet            AS s"
                + " WHERE   s.Label   = '" + setName + "'"
                + "     AND s.Version = '" + setVersion + "'"
                , Throws.Actions, "P");
			RichDataTable setCharacteristic = new RichDataTable(cmd, null, "SetCharacteristic", "EndemeCharacteristicID");


			// ---------------------------------------------------------------------------
			//  Build the endeme set
			// ---------------------------------------------------------------------------
			EndemeSet outputSet = default(EndemeSet);
			if ((setCharacteristic.Count > 0)) {
				outputSet = setCharacteristic.ToEndemeSet(setName, "CharId", "Label", "Descr");
			} else {
				outputSet = null;
			}

			return outputSet;
		}

		// -----------------------------------------------------------------------------------------
		/// <!-- GetEndemeSetHeader -->
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="setName"></param>
		/// <param name="setVersion"></param>
		/// <returns></returns>
		/// <remarks>, old</remarks>
		private static RichDataTable GetEndemeSetHeader_old(string conn, string setName, string setVersion)
		{

			// ---------------------------------------------------------------------------
			//  Prepare the command
			// ---------------------------------------------------------------------------
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , " SELECT *" + " FROM   "+EndemeAccess.EnSchema+".EndemeSet AS s"
                + " WHERE  s.Label   = '" + setName + "'"
                + "    AND s.VersionNumber = '" + TreatAs.IntValue(setVersion, 1) + "'"
                , Throws.Actions, "P");


			// ---------------------------------------------------------------------------
			//  Get the knowledge table
			// ---------------------------------------------------------------------------
			RichDataTable header = new RichDataTable(cmd, null, "EndemeSet", SetIdColumn);
			return header;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSetId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Guid GetEndemeSetId_old(string setName, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "SELECT * FROM "+EndemeAccess.EnSchema+".EndemeSet WHERE Label = '"+setName+"'"
                , Throws.Actions, "P");
            RichDataTable table = new RichDataTable(cmd, null, "OneSet", SetIdColumn);

            Guid setId = table.GuidValue(0, "SetId", Guid.Empty);
            return setId;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- InsertCharacteristic -->
		/// <summary>
		///      Inserts an endeme characteristic into the knowledge database
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="characteristic"></param>
		/// <param name="setId"></param>
		/// <remarks>, old</remarks>
		private static Guid InsertCharacteristic_old(EndemeCharacteristic characteristic, Guid setId, string conn)
		{
            Guid charKey = Guid.NewGuid();

			// ---------------------------------------------------------------------------
			//  Perform the insert
			// ---------------------------------------------------------------------------
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " INSERT INTO "+EndemeAccess.EnSchema+".EndemeCharacteristic"
                + "\r\n" + "        ( CharId,  SetId,  Letter,  Code,  Label,  Descr)"
                + "\r\n" + " VALUES (@CharId, @SetId, @Letter, @Code, @Label, @Descr)"
                , Throws.Actions, "P");
            cmd.AddParameter     ("@CharId", charKey                    );
            cmd.AddParameter     ("@SetId" , setId                      );
            cmd.AddParameter     ("@Letter", characteristic.Letter      );
            cmd.AddParameter_safe("@Code"  , characteristic.Code  ,   16);
            cmd.AddParameter_safe("@Label" , characteristic.Label ,  128);
            cmd.AddParameter_safe("@Descr" , characteristic.Descr , 1024);

            cmd.ExecuteNonQuery();

            return charKey;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- InsertEndemeSet -->
        /// <summary>
        ///      Inserts the entire endeme set to the database
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        public static Guid InsertEndemeSet_old(EndemeSet setName, string conn)
        {
            Guid setId = Guid.NewGuid();

            // --------------------------------------------------------------------------
            //  Save the set record
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "INSERT INTO "+EndemeAccess.EnSchema+".EndemeSet (SetId, Label, VersionNumber)"
                + " VALUES"
                + "     ( '" + setId.ToString() + "'"
                + "     , '" + __.FixApostrophe(setName.Label) + "'"
                + "     ,  " + 0
                + "     )"
                , Throws.Actions, "PI");
            cmd.ExecuteNonQuery();


            // --------------------------------------------------------------------------
            //  Save the characteristic records
            // --------------------------------------------------------------------------
            foreach (EndemeCharacteristic characteristic in setName.Characteristics())
            {
                Guid charId = Guid.NewGuid();
                RichSqlCommand cmd2 = new RichSqlCommand(CommandType.Text, conn
                    , "INSERT INTO "+EndemeAccess.EnSchema+".EndemeCharacteristic (CharId, SetId, Letter, Label, Descr)"
                    + " VALUES"
                    + "     ( '" + charId.ToString() + "'"
                    + "     , '" + setId.ToString() + "'"
                    + "     , '" + characteristic.Letter + "'"
                    + "     , '" + __.FixApostrophe(characteristic.Label) + "'"
                    + "     , '" + __.FixApostrophe(characteristic.Descr) + "'"
                    + "     )"
                    , Throws.Actions, "PI");
                cmd2.ExecuteNonQuery();
            }

            return setId;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- InsertEndemeSetHeader -->
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="inputSet"></param>
		/// <remarks>, old</remarks>
		private static Guid InsertEndemeSetHeader_old(string conn, EndemeSet inputSet)
		{
            if (string.IsNullOrWhiteSpace(inputSet.Label))
                Pause();

			// ---------------------------------------------------------------------------
			//  Make sure the setId is good
			// ---------------------------------------------------------------------------
			Guid setId = inputSet.SetId;
			if ((setId == null) | setId == Guid.Empty) {
				setId = Guid.NewGuid();
			}


			// ---------------------------------------------------------------------------
			//  Perform the insert
			// ---------------------------------------------------------------------------
            //RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
            //    , "\r\n" + " INSERT INTO "+EndemeAccess.EnSchemaName+".EndemeSet"
            //    + "\r\n" + "        (SetId, Label, SetResource, Cutoff, VersionNumber, Descr)"
            //    + "\r\n" + " VALUES ( '" + setId.ToString() + "'"
            //    + "\r\n" + "        , '" + inputSet.Label     + "'"
            //    + "\r\n" + "        , '" + inputSet.Resource + "'"
            //    + "\r\n" + "        , '" + inputSet.Cutoff   + "'"
            //    + "\r\n" + "        , ''" // + inputSet.Version  + "'"
            //    + "\r\n" + "        , ''" // + inputSet.Descr    + "'"
            //    + "\r\n" + "        )", Throws.Actions, "P");
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " INSERT INTO "+EndemeAccess.EnSchema+".EndemeSet"
                + "\r\n" + "        (SetId, Label, SetResource, VersionNumber, Descr)"
                + "\r\n" + " VALUES ( '" + setId.ToString() + "'"
                + "\r\n" + "        , '" + inputSet.Label     + "'"
                + "\r\n" + "        , '" + inputSet.Resource + "'"
                + "\r\n" + "        , ''" // + inputSet.Version  + "'"
                + "\r\n" + "        , ''" // + inputSet.Descr    + "'"
                + "\r\n" + "        )", Throws.Actions, "P");
            cmd.ExecuteNonQuery();

			return setId;
		}

        private static void Pause()
        {
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- PrependEndeme -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enPrepend"></param>
        /// <param name="enPrependTo"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Guid PrependEndeme_old(Endeme enPrepend, Guid enPrependTo, SqlConnection connection, SqlTransaction trx)
        {
            Endeme enFound = EndemeAccess.GetEndeme(enPrependTo, connection, trx);
            if (enFound != null && enFound != Endeme.Empty)
            {
                if (enFound.EnSet == enPrepend.EnSet & enFound != enPrepend)
                {
                    enFound = new Endeme(enPrepend.EnSet, enPrepend.ToString() + enFound.ToString());
                    EndemeAccess.UpdateEndeme(enFound, enPrependTo, connection, trx);
                }
                else { enPrependTo = Guid.Empty; }
            }
            else { enPrependTo = EndemeAccess.InsertEndeme(enPrepend, connection, trx); }

            return enPrependTo;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- RetrieveEndeme -->
        /// <summary>
        ///      Retrieves the endeme and maybe the endeme set as well
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="enRef"></param>
        /// <param name="conn"></param>
        /// <returns>Has a side effect of adding to enRef</returns>
        private static Endeme RetrieveEndeme_old(Guid endemeId, EndemeReference enRef, string conn)
        {
            // --------------------------------------------------------------------------
            //  Retrieve the endeme data
            // --------------------------------------------------------------------------
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "SELECT e.*, s.Label AS SetName FROM "+EndemeAccess.EnSchema+".Endeme AS e LEFT OUTER JOIN EndemeSet As s ON s.SetId = e.SetId WHERE EndemeId = @key"
                , Throws.Actions, "P");
            cmd.AddParameter("@key", endemeId);
            RichDataTable endeme = new RichDataTable(cmd, null, "Endeme", "EndemeID");


            // --------------------------------------------------------------------------
            //  Resolve the endeme set
            // --------------------------------------------------------------------------
            if (!enRef.SetRef.ContainsKey("wetland animals")) enRef.Add(EndemeSet.WetlandAnimals);
            string    setName = endeme.StrValue(0, "SetName", "Wetland animals");
            EndemeSet enSet   = EndemeSet.WetlandAnimals;
            if (enRef.SetRef.ContainsKey(setName.ToLower())) enSet = enRef[setName.ToLower()];
            else
            {
                enSet = GetEndemeSet_old(setName, conn);
                if (enSet != null && enSet.Count > 0) enRef.Add(enSet);
            }


            // --------------------------------------------------------------------------
            //  Construct the endeme
            // --------------------------------------------------------------------------
            Endeme en = new Endeme(enSet, endeme.StrValue(0, "CharString", ""));

            return en;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- StoreEndemeSet -->
		/// <summary>
		///      Inserts or updates an EndemeSet
		/// </summary>
		/// <param name="inputSet"></param>
		/// <param name="conn"></param>
		/// <returns></returns>
		/// <remarks>, old</remarks>
		public static Guid StoreEndemeSet_old(EndemeSet inputSet, string conn)
		{
            if (string.IsNullOrWhiteSpace(inputSet.Label))
                Pause();
			Guid key = Guid.Empty;


			// ---------------------------------------------------------------------------
			//  Manage the header
			// ---------------------------------------------------------------------------
			RichDataTable setHeader = GetEndemeSetHeader_old(conn, inputSet.Label, inputSet.Version);
			Guid setId = default(Guid);
			if (setHeader == null || setHeader.Count == 0)
            {
				setId = InsertEndemeSetHeader_old(conn, inputSet);
			}
            else
            {
				setId = setHeader.GuidValue(0, "SetId", Guid.Empty);
			}


			// ---------------------------------------------------------------------------
			//  Manage the detail
			// ---------------------------------------------------------------------------
			EndemeSet chars = GetEndemeSet_old(setId, conn);
			if (chars == null || chars.Count == 0)
            {
				// insert - insert each characteristic
				foreach (EndemeCharacteristic characteristic in inputSet.Characteristics())
                {
                    if (string.IsNullOrWhiteSpace(characteristic.Code))
                        characteristic.Code = characteristic.Label.ToUpper();
                    if (characteristic.Code.Length > 8)
                        characteristic.Code = characteristic.Code.Substring(0,8).Trim();
					InsertCharacteristic_old(characteristic, setId, conn);
				}
			}
            else
            {
				// update - clear out the characteristics and replace with new ones
			  //ClearSet(conn, setId); // if I clear out the set it breaks all sorts of connections
                // do this differently, instead of clear and insert, use update old ones and insert new ones, and leave replaced ones as orphans
				foreach (EndemeCharacteristic characteristic in inputSet.Characteristics())
                {
                    //RichDataTable characteristics = null;
                    //Characteristic_Insert(characteristics, setId, conn);
				}
			}

			return setId;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- StoreLabelAsCharacteristic -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="label"></param>
        /// <param name="typeSetId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Guid StoreLabelAsCharacteristic_old(char letter, string label, Guid typeSetId, string conn)
        {
            Guid charId = Guid.NewGuid();
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + "INSERT INTO "+EndemeAccess.EnSchema+".EndemeCharacteristic"
                + "\r\n" + "     ( CharId"
                + "\r\n" + "     , SetId"
                + "\r\n" + "     , Letter"
                + "\r\n" + "     , Code"
                + "\r\n" + "     , Label"
                + "\r\n" + "     , Descr"
                + "\r\n" + "     , OppositeLetter"
                + "\r\n" + "     , OppositeCode"
                + "\r\n" + "     , OppositeLabel"
                + "\r\n" + "     , OppositeDescr"
                + "\r\n" + "     , NominalValue"
                + "\r\n" + "     , Units"
                + "\r\n" + "     , Finickiness"
                + "\r\n" + "     , CharacteristicIsASet"
                + "\r\n" + "     )"
                + "\r\n" + " VALUES"
                + "\r\n" + "     ( '" + charId +"'"
                + "\r\n" + "     , '" + typeSetId + "'"
                + "\r\n" + "     , '" + letter                  + "'" // Letter
                + "\r\n" + "     , '" + __.DoubleQuotes(__.Truncate(label, 8  )) + "'" // Code
                + "\r\n" + "     , '" + __.DoubleQuotes(__.Truncate(label, 64 )) + "'" // Label
                + "\r\n" + "     , '" + __.DoubleQuotes(__.Truncate(label, 255)) + "'" // Descr
                + "\r\n" + "     , ''"
                + "\r\n" + "     , ''"
                + "\r\n" + "     , ''"
                + "\r\n" + "     , ''"
                + "\r\n" + "     , 0"
                + "\r\n" + "     , ''"
                + "\r\n" + "     , 0"
                + "\r\n" + "     , NULL"
                + "\r\n" + "     )"
                , Throws.Actions, "P");

            cmd.ExecuteNonQuery(); // TAG JRG START
            return charId;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- UpdateCharacteristic -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="characteristic"></param>
        /// <param name="charId"></param>
        /// <param name="setId"></param>
        /// <param name="conn"></param>
        private static void UpdateCharacteristic_old(EndemeCharacteristic characteristic, Guid charId, Guid setId, string conn)
        {
			// ---------------------------------------------------------------------------
			//  Perform the update
			// ---------------------------------------------------------------------------
			RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " UPDATE "+EndemeAccess.EnSchema+".EndemeCharacteristic"
                + "\r\n" + " SET    SetId = @SetId, Letter = @Letter, Code = @Code, Label =  @Label, Descr = @Descr)"
                + "\r\n" + " WHERE  CharId = @CharId"
                , Throws.Actions, "P");

            cmd.AddParameter     ("@CharId", charId                    );
            cmd.AddParameter     ("@SetId" , setId                     );
            cmd.AddParameter     ("@Letter", characteristic.Letter     );
            cmd.AddParameter_safe("@Code"  , characteristic.Code ,   16);
            cmd.AddParameter_safe("@Label" , characteristic.Label,  128);
            cmd.AddParameter_safe("@Descr" , characteristic.Descr, 1024);

            cmd.ExecuteNonQuery();
        }
    }
}