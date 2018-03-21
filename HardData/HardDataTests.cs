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
using InformationLib.Testing;         // for Assert, Is
using System;                         // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
	// --------------------------------------------------------------------------------------------
	/// <!-- DataOrientedTests -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>production ready unit test code</remarks>
    public class HardDataTests
    {
		// -----------------------------------------------------------------------------------------
		//  Members
		// -----------------------------------------------------------------------------------------
        private        Result _result;
	  //private static string _conn;


        // ----------------------------------------------------------------------------------------
        /// <!-- AllTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string QuickUnitTests()
        {
            _result = new Result("Hard Data tests");


            InData_DatabaseIdentity_test  ();
            InData_DetectSqlInjection_test();
            InData_Sort_test              ();


            return _result.AsciiDetailResults + "\r\n" + _result.SummaryMessage();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InData_DatabaseIdentity_test -->
        /// <summary>
        /// 
        /// </summary>
        public void InData_DatabaseIdentity_test()
        {
            Assert.ThingsAbout("InData", "DatabaseIdentity");

            InData_DatabaseIdentity_testcase( "Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;"                                                             , "myDataBase");
            InData_DatabaseIdentity_testcase( "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"                                              , "myDataBase");
            InData_DatabaseIdentity_testcase(@"Server=myServerName\myInstanceName;Database=myDataBase;User Id=myUsername;Password=myPassword;"                                  , "myDataBase");
            InData_DatabaseIdentity_testcase( "Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;MultipleActiveResultSets=true;"                               , "myDataBase");
            InData_DatabaseIdentity_testcase(@"Server=.\SQLExpress;AttachDbFilename=C:\MyFolder\MyDataFile.mdf;Database=dbname;Trusted_Connection=Yes;"                         , "dbname");
            InData_DatabaseIdentity_testcase(@"Server=.\SQLExpress;AttachDbFilename=|DataDirectory|mydbfile.mdf;Database=dbname;Trusted_Connection=Yes;"                        , "dbname"    );
            InData_DatabaseIdentity_testcase( "Data Source=190.190.200.100,1433;Network Library=DBMSSOCN;Initial Catalog=myDataBase;User ID=myUsername;Password=myPassword;"    , "myDataBase");
            InData_DatabaseIdentity_testcase(@"Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=SSPI;User ID=myDomain\myUsername;Password=myPassword;", "myDataBase");

            _result += Assert.Conclusion;
        }
        private Result InData_DatabaseIdentity_testcase(string conn, string target)
        {
            string identity = InData.DatabaseIdentity(conn);
            Result result   = Assert.That(identity, Is.equal_to, target, "InData", "DatabaseIdentity");
            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InData_DetectSqlInjection_test -->
        /// <summary>
        /// 
        /// </summary>
        public void InData_DetectSqlInjection_test()
        {
            Assert.ThingsAbout("InData", "DetectSqlInjection");


            InData_DetectSqlInjection_testcase("-1 UNION SELECT 1,pass,cc FROM users WHERE uname='test'"   , true);
            InData_DetectSqlInjection_testcase("a';DROP TABLE users; SELECT * FROM userinfo WHERE 't' = 't", true);
            InData_DetectSqlInjection_testcase("-1 UNION SELECT 1, 2, 3", true );
            InData_DetectSqlInjection_testcase("1; DROP TABLE users;"   , true );
            InData_DetectSqlInjection_testcase("'' OR '1'='1' -- ';"    , true );
            InData_DetectSqlInjection_testcase("password’ OR 1=1"       , true );
            InData_DetectSqlInjection_testcase("5' AND '1'='2"          , true );
            InData_DetectSqlInjection_testcase("5' OR '1'='1"           , true );
            InData_DetectSqlInjection_testcase("sp_droplogin"           , true );
            InData_DetectSqlInjection_testcase("sp_addlogin"            , true );
            InData_DetectSqlInjection_testcase("xp_cmdshell"            , true );
            InData_DetectSqlInjection_testcase("sysmessages"            , true );
            InData_DetectSqlInjection_testcase("sysobjects"             , true );
            InData_DetectSqlInjection_testcase("sysservers"             , true );
            InData_DetectSqlInjection_testcase("xp_regread"             , true );
            InData_DetectSqlInjection_testcase("xp_regaddmultistring"   , true );
            InData_DetectSqlInjection_testcase("xp_regdeletekey"        , true );
            InData_DetectSqlInjection_testcase("xp_regdeletevalue"      , true );
            InData_DetectSqlInjection_testcase("xp_regenumkeys"         , true );
            InData_DetectSqlInjection_testcase("xp_regenumvalues"       , true );
            InData_DetectSqlInjection_testcase("xp_regread"             , true );
            InData_DetectSqlInjection_testcase("xp_regremovemultistring", true );
            InData_DetectSqlInjection_testcase("xp_regwrite"            , true );
            InData_DetectSqlInjection_testcase("xp_regread"             , true );
            InData_DetectSqlInjection_testcase("xp_regenumvalues"       , true );
            InData_DetectSqlInjection_testcase("xp_servicecontrol"      , true );
            InData_DetectSqlInjection_testcase("xp_availablemedia"      , true );
            InData_DetectSqlInjection_testcase("xp_enumdsn"             , true );
            InData_DetectSqlInjection_testcase("xp_loginconfig"         , true );
            InData_DetectSqlInjection_testcase("xp_makecab"             , true );
            InData_DetectSqlInjection_testcase("xp_ntsec_enumdomains"   , true );
            InData_DetectSqlInjection_testcase("xp_terminate_process"   , true );
            InData_DetectSqlInjection_testcase("xp_webserver"           , true );
            InData_DetectSqlInjection_testcase("sp_makewebtask"         , true );


            InData_DetectSqlInjection_testcase("Jon Grover"             , false);
            InData_DetectSqlInjection_testcase("Mrs. O'Leary's cow"     , false);
            InData_DetectSqlInjection_testcase("Union of Concerned Scientists", false);


            _result += Assert.Conclusion;
        }
        private Result InData_DetectSqlInjection_testcase(string injectionString, bool injectionYN)
        {
            bool   isInjection = InData.DetectSqlInjection(injectionString);
            Result ok          = Assert.That(isInjection, Is.equal_to, injectionYN, "InData", "DetectSqlInjection");
            return ok;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InData_Sort_test -->
        /// <summary>
        /// 
        /// </summary>
        public void InData_Sort_test()
        {
            Assert.ThingsAbout("InData", "Sort");




            _result += Assert.Conclusion;
        }
    }
}
