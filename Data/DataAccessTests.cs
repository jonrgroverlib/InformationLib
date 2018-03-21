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
using InformationLib.Endemes ;        // for 
using InformationLib.HardData;        // for ConnectionSource
using InformationLib.Strings ;        // for __.IsNull
using InformationLib.Testing ;        // for RandomSource
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Data;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
	// ---------------------------------------------------------------------------------------------
	/// <!-- DataAccessTests -->
	/// <summary>
	///      The DataAccessTests class tests the classes in the DataAccess Library
	/// </summary>
    /// <remarks>production ready</remarks>
	///[TestFixture]
	public class DataAccessTests
	{
		// -----------------------------------------------------------------------------------------
		//  Members
		// -----------------------------------------------------------------------------------------
        private        Result _result;
		private static string _conn;


		// -----------------------------------------------------------------------------------------
		/// <!-- AllTests -->
        /// <summary>
        ///      Some or these tests require a connection to a database
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            _conn = ConnectSource.Connection("CurrentConnection", 100);
            return UnconnectedTests()
                + ConnectedTests()
                ;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- ConnectedTests -->
		/// <summary>
		///      Tests the DataAccess folder in ways that need a connection
		/// </summary>
		/// <returns></returns>
		public string ConnectedTests()
		{
            string result = "";


            // ---------------------------------------------------------------------------
            //  DataAccess folder tests needing a connection
            // ---------------------------------------------------------------------------


            result += "\r\n" + "Connected data access tests succeeded";
            return result;

		}

		// -----------------------------------------------------------------------------------------
		/// <!-- UnconnectedTests -->
		/// <summary>
		///      Tests the DataAccess folder in ways that do not need a connection
		/// </summary>
		/// <returns></returns>
		public string UnconnectedTests()
		{
            _result = new Result("Unconnected data access tests");


            // ---------------------------------------------------------------------------
            //  DataAccess folder tests not needing a connection
            // ---------------------------------------------------------------------------
            TestJson_TokensFor_test        ();
            TestJson_Deserialization_test  ();


            RichDataTable_Constructor_tests();

            Attest_tests                   ();
            RichDataTable_Add_tests        ();
            RichDataTable_CharValue_tests  ();
            RichDataTable_Count_tests      ();
            RichDataTable_CreateCsCode_test();
            RichDataTable_DecValue_test    ();
            RichDataTable_IntValue_tests   ();
            RichDataTable_LongValue_tests  ();
            RichDataTable_StrValue_tests   ();
            RichDataTable_TimeValue_tests  ();
            RichDataTable_ToEndemeSet_tests();
                                              
            Tally_test                     ();                     


            // ---------------------------------------------------------------------------
            //  DataAccess folder tests needing a connection but not using it
            // ---------------------------------------------------------------------------
            RichSqlCommand_AddParameter_test();


            string myresult = _result.AsciiDetailResults;
            return _result.AsciiDetailResults + "\r\n" + _result.SummaryMessage();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- TestJson_TokensFor_test -->
        /// <summary>
        /// 
        /// </summary>
        private void TestJson_TokensFor_test()
        {
            Assert.ThingsAbout("RichJson", "TokensFor");



            // --------------------------------------------------------------------------
            //  Build input test object
            // --------------------------------------------------------------------------
            string jsonConfig2 = "{ \"ClientId\":97"
                + "\r\n" + ", \"Client\":"
                + "\r\n" + "    { \"ClientId\":97"
                + "\r\n" + "    , \"DistrictId\":\"73619\""
                + "\r\n" + "    , \"Name\":\"BEDFORD CENTRAL SCHOOLS (WEBEDGE)\""
                + "\r\n" + "    , \"ARNumber\":73619"
                + "\r\n" + "    , \"FtpConfig\":null"
                + "\r\n" + "    , \"HasStudentReporting\":false"
                + "\r\n" + "    }"
                + "\r\n" + ", \"SiteTitle\":\"STUDENT PORTFOLIO\""
                + "\r\n" + ", \"NavigationTitle\":\"Welcome\""
                + "\r\n" + ", \"WelcomeMessageHtml\":\"<p><span style=\"font-family: Tahoma;\">Welcome to your child’s Assessment Portfolio!</span></p>"
                + "\r\n" + "<p style=\"font-family: Tahoma;\"><span><br />We are pleased to have </span><span>launched the &quot;Assessment Portfolio&quot; as an enhancement to the eSD Parent Portal. This site contributes to our </span>efforts to streamline and simplify school:home communications around student learning and achievement. In 2015-16, the <span>Assessment Portfolio will include end of year report cards and state assessment reports. We anticipate the potential for including additional assessment-related documents in the future. For some assessments, you will be able to view multiple years to get a clearer picture of your child&#39;s progress.</span></p>"
                + "\r\n" + "<p style=\"font-family: Tahoma;\"><br />Specific contact information is on each of the individual assessment pages. If multiple years are being stored you will see them displayed as links on the left side of the page.</p>"
                + "\r\n" + "<p><span style=\"font-family: Tahoma;\">Thank you for visiting your child’s assessment portfolio, and please contact your school principal if you have questions or suggestions.</span>&nbsp;</p>\""
                + "\r\n" + ", \"ContactMessageHtml\":\"&nbsp;vvxcvx\""
                + "\r\n" + ", \"DistrictLogoUrl\":null"
                + "\r\n" + "}";
            string[] config2 = TestJson.TokensFor(jsonConfig2).ToArray();
            Assert.That(config2.Length, Is.equal_to, 53, "TokensFor:PortalConfiguration");


            // --------------------------------------------------------------------------
            //  Build input test object
            // --------------------------------------------------------------------------
            string jsonCDT1 = "{ \"ClientDocumentTypeId\":150"
                + "\r\n" + ", \"ClientId\":58"
                + "\r\n" + ", \"Client\":null"
                + "\r\n" + ", \"DocumentTypeId\":1"
                + "\r\n" + ", \"DocumentType\":"
                + "\r\n" + "    { \"DocumentTypeId\":1"
                + "\r\n" + "    , \"Name\":\"NYS Assessment Reports\""
                + "\r\n" + "    , \"Description\":\"New York State English, Math, and Science Reports\""
                + "\r\n" + "    , \"SplitFile\":\"N/A\""
                + "\r\n" + "    , \"ReplaceExisting\":false"
                + "\r\n" + "    , \"IsStudentReporting\":false"
                + "\r\n" + "    , \"ExpectedPages\":2"
                + "\r\n" + "    , \"UploadUtility\":false"
                + "\r\n" + "    , \"DocumentMain\":null"
                + "\r\n" + "    , \"DocumentTransform\":null"
                + "\r\n" + "    , \"ClientDocumentTypes\":[$ref-0]"
                + "\r\n" + "    }"
                + "\r\n" + ", \"Active\":true"
                + "\r\n" + ", \"OverviewMessageHtml\":\"<p><span style=\"font-family: Arial;\">September 2015</span></p><p>"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\"><br />Dear Parent(s)</p><p>"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\">"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\">&nbsp;</p><p style=\"color: #000000; font-family: Arial;\">Accompanying this letter you will find the Parent Reports for the New York State English Language Arts (ELA) and Mathematics assessments for your child. These assessments were administered to students in New York State this past spring.&nbsp;&nbsp;</p><p>"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\">"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\"><br />As the state continues to transition towards full implementation of the Common Core State Standards in 2022, the state assessments and the parent reports continue to evolve. It is important to read the parent reports thoroughly as the information provided by the state has changed from years past. One such change is that students who have scored at level 2 are now considered to have met current New York State learning standards for High School graduation but have only partially met Common Core standards for the grade level. For more in depth information regarding the state testing program as well as helpful information about the parent reports, New York State has created an online parent resource center for 3-8 testing at: <a href=\"https://www.engageny.org/resource/parent-resources-grades-3-8-ela-mathematics-tests\">https://www.engageny.org/resource/parent-resources-grades-3-8-ela-mathematics-tests</a></p><p style=\"color: #000000; font-family: Arial;\">&nbsp;</p><p>"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\">As always, in the weeks to come we will be undertaking a full analysis of the data, making changes to our program where it seems appropriate and providing additional support for students who require it. However, I want to emphasize that we provide an excellent educational program here in Plainedge. Our students have excelled in academics, athletics, music, and the arts. Our graduation rate is consistently over 95%, with nearly 70% of our students receiving Advanced Regents diplomas. Over 90% of our students attend college and our students are awarded between 3-4 million dollars annually in scholarships. For the&nbsp;third year in a row, New York State has designated the Plainedge High School as a High Performing “Reward School”. &nbsp;It is through this lens that we will further examine the 3-8 test scores to determine how we can best support our students in achieving the Common Core State Standards.&nbsp;&nbsp;<br />"
                + "\r\n" + "&nbsp;</p><p>"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\">Since the New York State Assessments represent only one snapshot of student performance, we will be continuing our use of NWEA, a norm referenced instructional assessment tool in grades K-8 to ensure our students are making grade appropriate progress. Additionally, NWEA provides teachers with instructional recommendations for all students and has been an effective measure of student growth for over 30 years. Please expect to see Parent Reports for these assessments as well during the course of the school year. For more information on NWEA, please go to <a href=\"http://www.nwea.org\">www.nwea.org</a>.<br />"
                + "\r\n" + "&nbsp;</p><p>"
                + "\r\n" + "</p><p style=\"color: #000000; font-family: Arial;\">As always, if you have any specific questions about the ELA or Math assessments please don’t hesitate to contact your child’s classroom teacher.<br /></p><p style=\"color: #000000; font-family: Arial;\">&nbsp;</p><p style=\"color: #000000; font-family: Arial;\">As always, it is an honor to work on behalf of the children of the Plainedge community. </p><p style=\"color: #000000; font-family: Arial;\">&nbsp;</p><p style=\"color: #000000; font-family: Arial;\">&nbsp;</p><p style=\"color: #000000; font-family: Arial;\">Kindest regards,</p><span style=\"color: #000000; font-family: Arial;\"><p style=\"color: #000000; font-family: Arial;\"><br />Tony Sinanis</p><div style=\"color: #000000; font-family: Arial;\"></div></span><div style=\"color: #000000; font-family: Arial;\">&nbsp;</div><div style=\"color: #000000; font-family: Arial;\">&nbsp;</div><div>&nbsp;</div>\""
                + "\r\n" + ", \"ContactMessageHtml\":\"&nbsp;fdsfdsf\""
                + "\r\n" + ", \"DisplayOrder\":1"
                + "\r\n" + "}";
            string[] listCDT1 = TestJson.TokensFor(jsonCDT1).ToArray();
            Assert.That(listCDT1.Length, Is.equal_to, 83, "TokensFor:ClientDocumentType");


            string jsonStudent = "{ \"StudentId\":10368"
                + "\r\n" + ", \"ExternalId\":\"3357\""
                + "\r\n" + ", \"DistrictId\":\"74328\""
                + "\r\n" + ", \"FirstName\":\"Massimo\""
                + "\r\n" + ", \"LastName\":\"Speach\""
                + "\r\n" + ", \"MiddleInitial\":null"
                + "\r\n" + ", \"Address1\":\"34 Mead Place\""
                + "\r\n" + ", \"Address2\":null"
                + "\r\n" + ", \"City\":\"Rye\""
                + "\r\n" + ", \"State\":\"NY\""
                + "\r\n" + ", \"Phone\":\"914-921-1956\""
                + "\r\n" + ", \"Email\":null"
                + "\r\n" + ", \"Zip\":\"10580\""
                + "\r\n" + ", \"GradeLevel\":null"
                + "\r\n" + ", \"LocalID\":\"10368\""
                + "\r\n" + ", \"Location\":null"
                + "\r\n" + ", \"SubLocation\":null"
                + "\r\n" + ", \"DocumentMain\":null"
                + "\r\n" + "}";
            string[] listStudent = TestJson.TokensFor(jsonStudent).ToArray();
            Assert.That(listStudent.Length, Is.greater_than_or_equal_to, 73, "TokensFor:EnChar");

            string test = TestJson.FormatTestScript(jsonStudent, "RichJson", "FormatTestScript:EndemeCharacteristic");


            // --------------------------------------------------------------------------
            //  Build input test object
            // --------------------------------------------------------------------------
            string jsonConfig1 = "{ \"ClientId\":58"
                + "\r\n" + ", \"Client\":"
                + "\r\n" + "    { \"ClientId\":58"
                + "\r\n" + "    , \"HasStudentReporting\":false"
                + "\r\n" + "    }"
                + "\r\n" + ", \"SiteTitle\":\"STUDENT PORTFOLIO\""
                + "\r\n" + ", \"NavigationTitle\":\"Welcome\""
                + "\r\n" + ", \"WelcomeMessageHtml\":\"<p><strong>Welcome to your child’s <a name=\"_GoBack\" style=\"width: 20px; height: 20px; text-indent: 20px; background-image: url(https://www.edgedocllc.com/webEDGE/CuteSoft_Client/CuteEditor/Load.ashx?type=image&amp;file=anchor.gif); background-repeat: no-repeat;\"></a>Assessment Portfolio!</strong></p>  <p>The Plainedge Public Schools &nbsp;is committed to supporting academic success and to promoting the growth of all students. You can help by maintaining an active role in your child&#39;s education and their progress.</p>  <p>This site provides your child&#39;s assessment results to help you understand their strengths and learning needs. Assessment results may be viewed as soon as they are available. For some assessments, you will be able to view multiple years to get a clearer picture of your child&#39;s progress.</p>  <p>Specific contact information is on each of the individual assessment pages. If multiple years are being stored you will see them displayed as links on the left side of the page.</p>  <p>We are committed to delivering information to you in a green manner whenever possible. </p>  <p>Thank you for visiting your child’s assessment portfolio.</p>  <p>&nbsp;</p>\""
                + "\r\n" + ", \"ContactMessageHtml\":\"&nbsp;gfdgdfg\""
                + "\r\n" + ", \"DistrictLogoUrl\":null"
                + "\r\n" + "}";
            string[] config1 = TestJson.TokensFor(jsonConfig1).ToArray();
            Assert.That(config1.Length, Is.equal_to, 35, "TokensFor:PortalConfiguration");


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TestJson_Deserialization_test -->
        /// <summary>
        /// 
        /// </summary>
        public void TestJson_Deserialization_test()
        {
            EndemeSet            enSet   = EndemeTests.WetlandAnimals;
            Endeme               en      = new Endeme(enSet, "JON GROVER"); en.Quant.Raw['B'] = 2;
            EndemeCharacteristic enChar  = enSet['A'];
            EndemeQuantification enQuant = en.Quant;

            Assert.ThingsAbout("RichJson");


            //  Serializing
            string   jsonEnChar = TestJson.Serialize(enChar );
            string   jsonQuant  = TestJson.Serialize(enQuant);
            string   jsonEnSet  = TestJson.Serialize(enSet  );
            string   jsonEndeme = TestJson.Serialize(en     );


            //  Tokens
            string[] listEnChar = TestJson.TokensFor(jsonEnChar).ToArray(); Assert.That(listEnChar.Length, Is.greater_than_or_equal_to,   38, "TokensFor:EnChar");
            string[] listQuant  = TestJson.TokensFor(jsonQuant ).ToArray(); Assert.That(listQuant .Length, Is.greater_than_or_equal_to,  333, "TokensFor:Quant" );
            string[] listEnSet  = TestJson.TokensFor(jsonEnSet ).ToArray(); Assert.That(listEnSet .Length, Is.greater_than_or_equal_to, 1127, "TokensFor:EnSet" );
            string[] listEndeme = TestJson.TokensFor(jsonEndeme).ToArray(); Assert.That(listEndeme.Length, Is.greater_than_or_equal_to, 1531, "TokensFor:Endeme");


            //  Deserializing
            EndemeCharacteristic enChar2  = TestJson.Deserialize<EndemeCharacteristic>(jsonEnChar); Assert.That(enChar2 , Is.the_same_as, enChar , "Deserialize:jsonEnChar");
            EndemeQuantification enQuant2 = TestJson.Deserialize<EndemeQuantification>(jsonQuant ); Assert.That(enQuant2, Is.the_same_as, enQuant, "Deserialize:jsonQuant" );
            EndemeSet            enSet2   = TestJson.Deserialize<EndemeSet>           (jsonEnSet ); Assert.That(enSet2  , Is.the_same_as, enSet  , "Deserialize:jsonEnSet" );
            Endeme               en2      = TestJson.Deserialize<Endeme>              (jsonEndeme); Assert.That(en2     , Is.the_same_as, en     , "Deserialize:jsonEndeme");


            string detail = Assert.Detail;
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichDataTable_Constructor_tests -->
        /// <summary>
        /// 
        /// </summary>
        private void RichDataTable_Constructor_tests()
        {
            Assert.ThingsAbout("RichDataTable", "Constructor");

            RichDataTable tbl = new RichDataTable("My table", "hiworld");

            Dictionary<string,List<Ticker>> all = new Dictionary<string,List<Ticker>>();
            string symbol = "AAPL";
            if (!all.ContainsKey(symbol))
                all.Add(symbol, new List<Ticker>());
            all[symbol].Add(new Ticker(351.29M));

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Attest_tests -->
        /// <summary>
        ///      Tests the RichDataTable._Top method
        /// </summary>
        ///[Test]
        public void Attest_tests()
        {
            Assert.ThingsAbout("Attest");


            _result += Assert.Conclusion;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- YNtable -->
		/// <summary>
		///      Returns a Yes-No selection table
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public static RichDataTable YNtable()
		{
            RichDataTable table = new RichDataTable("YN", "Letter");
            string value = "Letter";
            string label = "Label" ;
            table.Add(value, typeof(string));
            table.Add(label, typeof(string));
            table.Add(value, "Y", label, "Yes");
            table.Add(value, "N", label, "No" );
            return table;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_Add_tests -->
		/// <summary>
		///      Tests the RichDataTable.Add method
		/// </summary>
		///[Test]
		public void RichDataTable_Add_tests()
		{
            Assert.ThingsAbout("RichDataTable", "Add");


            RichDataTable table = YNtable();
            RichDataTable table2 = YNtable();


            // ---------------------------------------------------------------------------
            //  Add a blank local row
            // ---------------------------------------------------------------------------
            int row = table.Add();
            Assert.That(row, Is.equal_to, 2);
            Assert.That(table.Count, Is.equal_to, 3);
            Assert.That(table.StrValue(2, "Label", "hi"), Is.equal_to, "hi");
            table.Rows[2]["Label"] = "Maybe";
            table.Rows[2]["Letter"] = 'M';
            Assert.That(table.CharValue(2, "Letter",'h'), Is.equal_to, 'M');
            char str = table.CharValue(table.Count - 1, "Letter", ' ');
            Assert.That(str, Is.equal_to, 'M');


            // ---------------------------------------------------------------------------
			//  Add a local row
			// ---------------------------------------------------------------------------
			DataRow r = table.NewRow();
			r["Letter"] = 'S';
			r["Label"] = "Should be";
			table.Add(r);
			Assert.That(table.Count, Is.equal_to, 4);
			Assert.That(table.CharValue(3, "Letter", 'h'), Is.equal_to, 'S');


			// ---------------------------------------------------------------------------
			//  Add a row from another table
			// ---------------------------------------------------------------------------
			DataRow r2 = table2.NewRow();
			r2["Letter"] = 'R';
			r2["Label"] = "Refuse to answer";
            table2.Add(r2);
			int rnum = table.Add(r2);
			Assert.That(table.Count, Is.equal_to, 5);
			Assert.That(table.CharValue(4, "Letter", 'h'), Is.equal_to, 'R');


			// ---------------------------------------------------------------------------
			//  Add a row from another table
			// ---------------------------------------------------------------------------
			table.Add(1, table2);
			Assert.That(table.Count, Is.equal_to, 6);
			Assert.That(table.CharValue(5, "Letter", 'h'), Is.equal_to, 'N');


			// ---------------------------------------------------------------------------
			//  Add a new column
			// ---------------------------------------------------------------------------
			table.Add("Number", typeof(int));
            Assert.That(table.IntValue(0, "Number", 447), Is.equal_to, 447);
            Assert.That(table.Columns.Count, Is.equal_to, 3);
			table.Rows[0]["Number"] = 0;
			table.Rows[1]["Number"] = 1;
			table.Rows[2]["Number"] = 2;
			table.Rows[3]["Number"] = 3;
			table.Rows[4]["Number"] = 4;
			table.Rows[5]["Number"] = 5;
			Assert.That(table.IntValue(2, "Number", 447), Is.equal_to, 2);
			Assert.That(table.Sum("Number", 3.0), Is.equal_to, 15.0);


			// ---------------------------------------------------------------------------
			//  Add two value columns
			// ---------------------------------------------------------------------------
			table.Add("Letter", "D", "Label", "Don't know");
			Assert.That(table.Count, Is.equal_to, 7);
			Assert.That(table.CharValue(6, "Letter", 'h'), Is.equal_to, 'D');


            _result += Assert.Conclusion;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_CharValue_tests -->
		/// <summary>
		///      Tests the RichDataTable.CharValue method
		/// </summary>
		///[Test]
		public void RichDataTable_CharValue_tests()
		{
            Assert.ThingsAbout("RichDataTable", "CharValue");


            RichDataTable table = YNtable();


			table.Add("StrChar", typeof(string));    Assert.That(table.CharValue(0, "StrChar", 'Z'), Is.equal_to, 'Z');
			table.Add("IntChar", typeof(Int32));     Assert.That(table.CharValue(0, "IntChar", 'Z'), Is.equal_to, 'Z');
			table.Add("NewChar", typeof(char));	     Assert.That(table.CharValue(0, "NewChar", 'Z'), Is.equal_to, 'Z');
                                                     
			table.Rows[0]["StrChar"] = "A";          Assert.That(table.CharValue(0, "StrChar", 'Z'), Is.equal_to, 'A');
			table.Rows[0]["IntChar"] = 65;           Assert.That(table.CharValue(0, "IntChar", 'Z'), Is.equal_to, 'A');
			table.Rows[0]["NewChar"] = 'A';          Assert.That(table.CharValue(0, "NewChar", 'Z'), Is.equal_to, 'A');

			table.Rows[0]["StrChar"] = "Laguna Bay"; Assert.That(table.CharValue(0, "StrChar", 'Z'), Is.equal_to, 'L');
			table.Rows[0]["IntChar"] = 76;	         Assert.That(table.CharValue(0, "IntChar", 'Z'), Is.equal_to, 'L');
			table.Rows[0]["NewChar"] = 'l';	         Assert.That(table.CharValue(0, "NewChar", 'Z'), Is.equal_to, 'l');
                                                     
			table.Rows[0]["StrChar"] = "";	         Assert.That(table.CharValue(0, "StrChar", 'Z'), Is.equal_to, 'Z');
			table.Rows[0]["IntChar"] = 0;	         Assert.That(table.CharValue(0, "IntChar", 'Z'), Is.equal_to, '0');
			table.Rows[0]["NewChar"] = "\0";         Assert.That(table.CharValue(0, "NewChar", 'Z'), Is.equal_to, '\0');
                                                     
			table.Rows[0]["IntChar"] = 931;	         Assert.That(table.CharValue(0, "IntChar", 'Z'), Is.equal_to, (char)931);  // Greek sigma


            _result += Assert.Conclusion;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- SimpleTestTable -->
        /// <summary>
        ///      Returns a simple two column thre row table for testing various Data methods
        /// </summary>
        /// <returns></returns>
        private static RichDataTable SimpleTestTable()
        {
            RichDataTable rt = new RichDataTable();
            rt.Add("Name"  , typeof(string  ));
            rt.Add("Age"   , typeof(int     ));
            rt.Add("Start" , typeof(DateTime));
            rt.Add("Weight", typeof(double  ));
            rt.Add("Pay"   , typeof(decimal ));
            rt.Add("Active", typeof(bool    ));

            int row;
            row = rt.Add(); rt.Rows[row]["Name"] = "Fred"   ; rt.Rows[row]["Age"] = 26; rt.Rows[row]["Start"] = DateTime.Now; rt.Rows[row]["Weight"] = 234.0; rt.Rows[row]["Pay"] = 100.0M; rt.Rows[row]["Active"] = false;
            row = rt.Add(); rt.Rows[row]["Name"] = "Jon"    ; rt.Rows[row]["Age"] = 48; rt.Rows[row]["Start"] = DateTime.Now; rt.Rows[row]["Weight"] = 191.3; rt.Rows[row]["Pay"] = 100.0M; rt.Rows[row]["Active"] = true ;
            row = rt.Add(); rt.Rows[row]["Name"] = "Melinda"; rt.Rows[row]["Age"] = 26; rt.Rows[row]["Start"] = DateTime.Now; rt.Rows[row]["Weight"] = 123.0; rt.Rows[row]["Pay"] = 100.0M; rt.Rows[row]["Active"] = true ;
            row = rt.Add();

            return rt;
        }

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_Count_tests -->
		/// <summary>
		///      Tests the RichDataTable.Count method
		/// </summary>
		///[Test]
		public void RichDataTable_Count_tests()
		{
            Assert.ThingsAbout("RichDataTable", "Count");


            bool ok = true;

			RichDataTable table = new RichDataTable();
            Assert.That(table.Count, Is.equal_to, 0);
			table = YNtable();
            Assert.That(table.Count, Is.equal_to, 2);
			for (Int32 ct = 3; ct <= 200000; ct++)
            {
				table.Add();
	            ok &= (table.Count == ct);
			}
            Assert.That(ok, Is.equal_to, true);
			table.RemoveAt(10000);
            Assert.That(table.Count, Is.equal_to, 199999);

			table.Dispose();


            _result += Assert.Conclusion;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_CreateCsCode_test -->
        /// <summary>
        /// 
        /// </summary>
        private void RichDataTable_CreateCsCode_test()
        {
            Assert.ThingsAbout("RichDataTable", "CreateCsCode");

            RichDataTable table = SimpleTestTable();
            string code = table.CreateCsCode();
            Assert.That(code.Length, Is.greater_than, 0);



            DataTable testTable = new DataTable();
            testTable.Columns.Add(new DataColumn("Name" , typeof(String)));
            testTable.Columns.Add(new DataColumn("Age"  , typeof(Int32)));
            testTable.Columns.Add(new DataColumn("Start", typeof(DateTime)));
            testTable.Columns.Add(new DataColumn("Real" , typeof(Double)));
            testTable.Columns.Add(new DataColumn("Pay"  , typeof(Decimal)));
            
            int tgtRow = 0;
            tgtRow = testTable.Rows.Count;
            testTable.Rows.Add(testTable.NewRow());
            {
                testTable.Rows[tgtRow]["Name" ] = "Fred";
                testTable.Rows[tgtRow]["Age"  ] = 26;
                testTable.Rows[tgtRow]["Start"] = "1/7/2016 10:52:52 AM";
                testTable.Rows[tgtRow]["Real" ] = 0;
                testTable.Rows[tgtRow]["Pay"  ] = 100M;
            }
            tgtRow = testTable.Rows.Count;
            testTable.Rows.Add(testTable.NewRow());
            {
                testTable.Rows[tgtRow]["Name" ] = "Jon";
                testTable.Rows[tgtRow]["Age"  ] = 48;
                testTable.Rows[tgtRow]["Start"] = "1/7/2016 10:52:52 AM";
                testTable.Rows[tgtRow]["Real" ] = 0;
                testTable.Rows[tgtRow]["Pay"  ] = 100M;
            }
            tgtRow = testTable.Rows.Count;
            testTable.Rows.Add(testTable.NewRow());
            {
                testTable.Rows[tgtRow]["Name" ] = "Melinda";
                testTable.Rows[tgtRow]["Age"  ] = 26;
                testTable.Rows[tgtRow]["Start"] = "1/7/2016 10:52:52 AM";
                testTable.Rows[tgtRow]["Real" ] = 0;
                testTable.Rows[tgtRow]["Pay"  ] = 100M;
            }
            tgtRow = testTable.Rows.Count;
            testTable.Rows.Add(testTable.NewRow());
            {
                testTable.Rows[tgtRow]["Name" ] = System.DBNull.Value;
                testTable.Rows[tgtRow]["Age"  ] = System.DBNull.Value;
                testTable.Rows[tgtRow]["Start"] = System.DBNull.Value;
                testTable.Rows[tgtRow]["Real" ] = System.DBNull.Value;
                testTable.Rows[tgtRow]["Pay"  ] = System.DBNull.Value;
            }


            //RichDataTable dt = new RichDataTable(testTable, "Name");
            //string code = dt.AsciiBuildCode;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Tally_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Tally_test()
        {
            Assert.ThingsAbout("Tally");


            for (int n = 0; n < 1; ++n)
            {
                Tally_testcase("Bell Curve", 1, 101, 100000);
                Tally_testcase("Bell Curve", 3, 5,   100000);
                Tally_testcase("Bell Curve", 1, 7,   100000);
                Tally_testcase("Bell Curve", 3, 19,  100000);
                Tally_testcase("Bell Curve", 0, 21,  100000);
                Tally_testcase("Bell Curve", 0, 11,  100000);
            }


            _result += Assert.Conclusion;
        }
        private static void Tally_testcase(string title, int inclMin, int exclMax, int numRuns)
        {
            int    range     = exclMax - inclMin - 1;
            double halfway   = (double)range / 2.0 + inclMin;
            int    expcTotal = (int)(halfway * numRuns);
            int    expcCount = range + 1;


            Tally tally = new Tally();
            //tally.Bell(inclMin, exclMax, numRuns);
            tally.Clear();
            for (int i = 0; i < numRuns; ++i)
                tally.Incr(RandomSource.New().Bell(inclMin, exclMax), 2);
            string hist   = tally.Histogram(25);
            string report = tally.Report(title, 30);


            double avg = tally.Average();             Assert.That(halfway   - 0.5    , Is.less_than            , avg   ).And(avg   , Is.less_than            , halfway + 0.5    );
            int total  = tally.TotalValue();          Assert.That(expcTotal * 0.995  , Is.less_than            , total ).And(total , Is.less_than            , expcTotal * 1.005);
            int min    = Integers(tally.Min()   , 2); Assert.That(inclMin   - 1      , Is.less_than            , min   ).And(min   , Is.less_than            , inclMin + 2      );
            int max    = Integers(tally.Max()   , 2); Assert.That(exclMax   - 3      , Is.less_than            , max   ).And(max   , Is.less_than            , exclMax          );
            int median = Integers(tally.Median(), 0); Assert.That(halfway   - 0.5    , Is.less_than_or_equal_to, median).And(median, Is.less_than_or_equal_to, halfway + 0.5    );
            int mode   = Integers(tally.Mode()  , 0); Assert.That((halfway-0.5)*0.9  , Is.less_than_or_equal_to, mode  ).And(mode  , Is.less_than_or_equal_to, (halfway+0.5)*1.1);
            int atMode = tally.MaxTally;              Assert.That(numRuns / expcCount, Is.less_than_or_equal_to, atMode).And(atMode, Is.less_than            , numRuns          );
            int items  = tally.Count;                 Assert.That(expcCount-2        , Is.less_than_or_equal_to, items ).And(items , Is.less_than_or_equal_to, expcCount        );

            int count  = tally.TotalCount;            Assert.That(count, Is.equal_to, numRuns);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Integers -->
        /// <summary>
        ///      Takes a shot at converting what is sent to it to an integer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int Integers(object obj, int defaultValue)
        {
            if (__.IsNull(obj))
                return defaultValue;
            else
            {
                Type type = obj.GetType();
                string str;
                int value;


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32)
                    || type == typeof(Int64))
                    return (int)obj;
                str = obj.ToString();
                if (int.TryParse(str, out value))  return value;


                return defaultValue;
            }
        }

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_DecValue_test -->
		/// <summary>
		///      Tests the RichDataTable.DecValue method
		/// </summary>
		///[Test]
		public void RichDataTable_DecValue_test()
		{
            Assert.ThingsAbout("RichDataTable", "DecValue");


			RichDataTable table = YNtable();

			table.Add("Whole", typeof(int)   ); Assert.That(table.DecValue(0, "Whole", -2.2m), Is.equal_to, -2.2m);
            table.Add("Digit", typeof(char)  ); Assert.That(table.DecValue(0, "Digit", -2.2m), Is.equal_to, -2.2m);
            table.Add("Real" , typeof(double)); Assert.That(table.DecValue(0, "Real" , -2.2m), Is.equal_to, -2.2m);
            table.Add("Small", typeof(float) ); Assert.That(table.DecValue(0, "Small", -2.2m), Is.equal_to, -2.2m);
            table.Add("Short", typeof(Int16) ); Assert.That(table.DecValue(0, "Short", -2.2m), Is.equal_to, -2.2m);
            table.Add("Large", typeof(Int64) ); Assert.That(table.DecValue(0, "Large", -2.2m), Is.equal_to, -2.2m);
            table.Add("Alpha", typeof(string)); Assert.That(table.DecValue(0, "Alpha", -2.2m), Is.equal_to, -2.2m);

            table.Rows[0]["Whole"] =  1   ;     Assert.That(table.DecValue(0, "Whole", -2), Is.equal_to, 1m   );
            table.Rows[0]["Digit"] = '1'  ;     Assert.That(table.DecValue(0, "Digit", -2), Is.equal_to, 1m   );
            table.Rows[0]["Real"]  =  1.9 ;     Assert.That(table.DecValue(0, "Real" , -2), Is.equal_to, 1.9m );
            table.Rows[0]["Small"] =  1.1 ;     Assert.That(table.DecValue(0, "Small", -2), Is.equal_to, 1.1m );
            table.Rows[0]["Short"] =  1   ;     Assert.That(table.DecValue(0, "Short", -2), Is.equal_to, 1m   );
            table.Rows[0]["Large"] =  1L  ;     Assert.That(table.DecValue(0, "Large", -2), Is.equal_to, 1m   );
            table.Rows[0]["Alpha"] = "1.1";     Assert.That(table.DecValue(0, "Alpha", -2), Is.equal_to, 1.1m );

            table.Rows[1]["Whole"] =  0   ;     Assert.That(table.DecValue(1, "Whole", -2), Is.equal_to, 0m   );
            table.Rows[1]["Digit"] = '0'  ;     Assert.That(table.DecValue(1, "Digit", -2), Is.equal_to, 0m   );
            table.Rows[1]["Real"]  =  0   ;     Assert.That(table.DecValue(1, "Real" , -2), Is.equal_to, 0m   );
            table.Rows[1]["Small"] =  0   ;     Assert.That(table.DecValue(1, "Small", -2), Is.equal_to, 0m   );
            table.Rows[1]["Short"] =  0   ;     Assert.That(table.DecValue(1, "Short", -2), Is.equal_to, 0m   );
            table.Rows[1]["Large"] =  0L  ;     Assert.That(table.DecValue(1, "Large", -2), Is.equal_to, 0m   );
            table.Rows[1]["Alpha"] = "0"  ;     Assert.That(table.DecValue(1, "Alpha", -2), Is.equal_to, 0m   );

            table.Add("Letter", "M", "Label", "Maybe");
            table.Rows[2]["Whole"] =  -1  ;     Assert.That(table.DecValue(2, "Whole", -2), Is.equal_to, -1m  );
            table.Rows[2]["Digit"] = '-'  ;     Assert.That(table.DecValue(2, "Digit", -2), Is.equal_to, -2m  );
            table.Rows[2]["Real"]  =  -1.9;     Assert.That(table.DecValue(2, "Real" , -2), Is.equal_to, -1.9m);
            table.Rows[2]["Small"] =  -1.1;     Assert.That(table.DecValue(2, "Small", -2), Is.equal_to, -1.1m);
            table.Rows[2]["Short"] =  -1  ;     Assert.That(table.DecValue(2, "Short", -2), Is.equal_to, -1m  );
            table.Rows[2]["Large"] =  -1L ;     Assert.That(table.DecValue(2, "Large", -2), Is.equal_to, -1m  );
            table.Rows[2]["Alpha"] = "-1" ;     Assert.That(table.DecValue(2, "Alpha", -2), Is.equal_to, -1m  );

            table.Add("Letter", "D", "Label", "Don't know");
            table.Rows[3]["Whole"] =  21   ;    Assert.That(table.DecValue(3, "Whole", -2), Is.equal_to, 21m  );
            table.Rows[3]["Digit"] = '1'   ;    Assert.That(table.DecValue(3, "Digit", -2), Is.equal_to, 1m   );
            table.Rows[3]["Real"]  =  21.9 ;    Assert.That(table.DecValue(3, "Real" , -2), Is.equal_to, 21.9m);
            table.Rows[3]["Small"] =  21.1 ;    Assert.That(table.DecValue(3, "Small", -2), Is.equal_to, 21.1m);
            table.Rows[3]["Short"] =  21   ;    Assert.That(table.DecValue(3, "Short", -2), Is.equal_to, 21m  );
            table.Rows[3]["Large"] =  21L  ;    Assert.That(table.DecValue(3, "Large", -2), Is.equal_to, 21m  );
            table.Rows[3]["Alpha"] = "21.1";    Assert.That(table.DecValue(3, "Alpha", -2), Is.equal_to, 21.1m);


            _result += Assert.Conclusion;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_IntValue_test -->
		/// <summary>
		///      Tests the RichDataTable.IntValue method
		/// </summary>
		///[Test]
		public void RichDataTable_IntValue_tests()
		{
            Assert.ThingsAbout("RichDataTable", "IntValue");


            RichDataTable test = YNtable();
            
            test.Add("Whole", typeof(int)   ); Assert.That(test.IntValue(0, "Whole", -2), Is.equal_to, -2);
            test.Add("Digit", typeof(char)  ); Assert.That(test.IntValue(0, "Digit", -2), Is.equal_to, -2);
            test.Add("Real" , typeof(double)); Assert.That(test.IntValue(0, "Real" , -2), Is.equal_to, -2);
            test.Add("Small", typeof(float) ); Assert.That(test.IntValue(0, "Small", -2), Is.equal_to, -2);
            test.Add("Short", typeof(Int16) ); Assert.That(test.IntValue(0, "Short", -2), Is.equal_to, -2);
            test.Add("Large", typeof(Int64) ); Assert.That(test.IntValue(0, "Large", -2), Is.equal_to, -2);
            test.Add("Alpha", typeof(string)); Assert.That(test.IntValue(0, "Alpha", -2), Is.equal_to, -2);
                                                                           
            test.Rows[0]["Whole"] = 1     ;    Assert.That(test.IntValue(0, "Whole", -2), Is.equal_to,  1);
            test.Rows[0]["Digit"] = '1'   ;    Assert.That(test.IntValue(0, "Digit", -2), Is.equal_to, 49);
            test.Rows[0]["Real" ] = 1.9   ;    Assert.That(test.IntValue(0, "Real" , -2), Is.equal_to,  1);
            test.Rows[0]["Small"] = 1.1   ;    Assert.That(test.IntValue(0, "Small", -2), Is.equal_to,  1);
            test.Rows[0]["Short"] = 1     ;    Assert.That(test.IntValue(0, "Short", -2), Is.equal_to,  1);
            test.Rows[0]["Large"] = 1L    ;    Assert.That(test.IntValue(0, "Large", -2), Is.equal_to,  1);
            test.Rows[0]["Alpha"] = "1.1" ;    Assert.That(test.IntValue(0, "Alpha", -2), Is.equal_to,  1);
                                                                          
            test.Rows[1]["Whole"] = 0     ;    Assert.That(test.IntValue(1, "Whole", -2), Is.equal_to,  0);
            test.Rows[1]["Digit"] = '0'   ;	   Assert.That(test.IntValue(1, "Digit", -2), Is.equal_to, 48);
            test.Rows[1]["Real" ] = 0.0   ;    Assert.That(test.IntValue(1, "Real" , -2), Is.equal_to,  0);
            test.Rows[1]["Small"] = 0.0   ;    Assert.That(test.IntValue(1, "Small", -2), Is.equal_to,  0);
            test.Rows[1]["Short"] = 0     ;    Assert.That(test.IntValue(1, "Short", -2), Is.equal_to,  0);
            test.Rows[1]["Large"] = 0L    ;    Assert.That(test.IntValue(1, "Large", -2), Is.equal_to,  0);
            test.Rows[1]["Alpha"] = "0"   ;    Assert.That(test.IntValue(1, "Alpha", -2), Is.equal_to,  0);
                                                                                      
            test.Add("Letter", "M", "Label", "Maybe");                                
            test.Rows[2]["Whole"] = -1    ;    Assert.That(test.IntValue(2, "Whole", -2), Is.equal_to, -1);
            test.Rows[2]["Digit"] = '-'   ;    Assert.That(test.IntValue(2, "Digit", -2), Is.equal_to, 45);
            test.Rows[2]["Real" ] = -1.9  ;    Assert.That(test.IntValue(2, "Real" , -2), Is.equal_to, -1);
            test.Rows[2]["Small"] = -1.1  ;    Assert.That(test.IntValue(2, "Small", -2), Is.equal_to, -1);
            test.Rows[2]["Short"] = -1    ;    Assert.That(test.IntValue(2, "Short", -2), Is.equal_to, -1);
            test.Rows[2]["Large"] = -1L   ;    Assert.That(test.IntValue(2, "Large", -2), Is.equal_to, -1);
            test.Rows[2]["Alpha"] = "-1"  ;    Assert.That(test.IntValue(2, "Alpha", -2), Is.equal_to, -1);
                                                                                             
            test.Add("Letter", "D", "Label", "Don't know");                                  
            test.Rows[3]["Whole"] = 21    ;    Assert.That(test.IntValue(3, "Whole", -2), Is.equal_to, 21);
            test.Rows[3]["Digit"] = '1'   ;    Assert.That(test.IntValue(3, "Digit", -2), Is.equal_to, 49);
            test.Rows[3]["Real" ] = 21.9  ;    Assert.That(test.IntValue(3, "Real" , -2), Is.equal_to, 21);
            test.Rows[3]["Small"] = 21.1  ;    Assert.That(test.IntValue(3, "Small", -2), Is.equal_to, 21);
            test.Rows[3]["Short"] = 21    ;    Assert.That(test.IntValue(3, "Short", -2), Is.equal_to, 21);
            test.Rows[3]["Large"] = 21L   ;    Assert.That(test.IntValue(3, "Large", -2), Is.equal_to, 21);
            test.Rows[3]["Alpha"] = "21.1";    Assert.That(test.IntValue(3, "Alpha", -2), Is.equal_to, 21);


            _result += Assert.Conclusion;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_LongValue_test -->
		/// <summary>
		///      Tests the RichDataTable.LongValue method
		/// </summary>
		///[Test]
		public void RichDataTable_LongValue_tests()
		{
            Assert.ThingsAbout("RichDataTable", "LongValue");


            RichDataTable test = YNtable();
            
            test.Add("Whole", typeof(int)   );    Assert.That(test.LongValue(0, "Whole", 2147483648L), Is.equal_to, 2147483648L);
            test.Add("Digit", typeof(char)  );    Assert.That(test.LongValue(0, "Digit", 2147483648L), Is.equal_to, 2147483648L);
            test.Add("Large", typeof(double));    Assert.That(test.LongValue(0, "Large", 2147483648L), Is.equal_to, 2147483648L);
            test.Add("Small", typeof(float) );    Assert.That(test.LongValue(0, "Small", 2147483648L), Is.equal_to, 2147483648L);
            test.Add("Short", typeof(Int16) );    Assert.That(test.LongValue(0, "Short", 2147483648L), Is.equal_to, 2147483648L);
            test.Add("Longs", typeof(Int64) );    Assert.That(test.LongValue(0, "Longs", 2147483648L), Is.equal_to, 2147483648L);
            test.Add("Alpha", typeof(string));    Assert.That(test.LongValue(0, "Alpha", 2147483648L), Is.equal_to, 2147483648L);
                                                                                        
            test.Rows[0]["Whole"] = 1   ;         Assert.That(test.LongValue(0, "Whole", 2147483648L), Is.equal_to,  1L);
            test.Rows[0]["Digit"] = '1' ;         Assert.That(test.LongValue(0, "Digit", 2147483648L), Is.equal_to, 49L);
            test.Rows[0]["Large"] = 1.9 ;         Assert.That(test.LongValue(0, "Large", 2147483648L), Is.equal_to,  1L);
            test.Rows[0]["Small"] = 1.1 ;         Assert.That(test.LongValue(0, "Small", 2147483648L), Is.equal_to,  1L);
            test.Rows[0]["Short"] = 1   ;         Assert.That(test.LongValue(0, "Short", 2147483648L), Is.equal_to,  1L);
            test.Rows[0]["Longs"] = 1L  ;         Assert.That(test.LongValue(0, "Longs", 2147483648L), Is.equal_to,  1L);
            test.Rows[0]["Alpha"] = "1.1";        Assert.That(test.LongValue(0, "Alpha", 2147483648L), Is.equal_to,  1L);
                                                                                        
            test.Rows[1]["Whole"] = 0   ;         Assert.That(test.LongValue(1, "Whole", 2147483648L), Is.equal_to,  0L);
            test.Rows[1]["Digit"] = '0' ;         Assert.That(test.LongValue(1, "Digit", 2147483648L), Is.equal_to, 48L);
            test.Rows[1]["Large"] = 0.0 ;         Assert.That(test.LongValue(1, "Large", 2147483648L), Is.equal_to,  0L);
            test.Rows[1]["Small"] = 0.0 ;         Assert.That(test.LongValue(1, "Small", 2147483648L), Is.equal_to,  0L);
            test.Rows[1]["Short"] = 0   ;         Assert.That(test.LongValue(1, "Short", 2147483648L), Is.equal_to,  0L);
            test.Rows[1]["Longs"] = 0L  ;         Assert.That(test.LongValue(1, "Longs", 2147483648L), Is.equal_to,  0L);
            test.Rows[1]["Alpha"] = "0" ;         Assert.That(test.LongValue(1, "Alpha", 2147483648L), Is.equal_to,  0L);
                                                                                                   
            test.Add("Letter", "M", "Label", "Maybe");                                             
            test.Rows[2]["Whole"] = -1  ;         Assert.That(test.LongValue(2, "Whole", 2147483648L), Is.equal_to, -1L);
            test.Rows[2]["Digit"] = '-' ;         Assert.That(test.LongValue(2, "Digit", 2147483648L), Is.equal_to, 45L);
            test.Rows[2]["Large"] = -1.9;         Assert.That(test.LongValue(2, "Large", 2147483648L), Is.equal_to, -1L);
            test.Rows[2]["Small"] = -1.1;         Assert.That(test.LongValue(2, "Small", 2147483648L), Is.equal_to, -1L);
            test.Rows[2]["Short"] = -1  ;         Assert.That(test.LongValue(2, "Short", 2147483648L), Is.equal_to, -1L);
            test.Rows[2]["Longs"] = -1L ;         Assert.That(test.LongValue(2, "Longs", 2147483648L), Is.equal_to, -1L);
            test.Rows[2]["Alpha"] = "-1";         Assert.That(test.LongValue(2, "Alpha", 2147483648L), Is.equal_to, -1L);
                                                                                                      
            test.Add("Letter", "D", "Label", "Don't know");                                           
            test.Rows[3]["Whole"] = 21  ;         Assert.That(test.LongValue(3, "Whole", 2147483648L), Is.equal_to, 21L);
            test.Rows[3]["Digit"] = '1' ;         Assert.That(test.LongValue(3, "Digit", 2147483648L), Is.equal_to, 49L);
            test.Rows[3]["Large"] = 21.9;         Assert.That(test.LongValue(3, "Large", 2147483648L), Is.equal_to, 21L);
            test.Rows[3]["Small"] = 21.1;         Assert.That(test.LongValue(3, "Small", 2147483648L), Is.equal_to, 21L);
            test.Rows[3]["Short"] = 21  ;         Assert.That(test.LongValue(3, "Short", 2147483648L), Is.equal_to, 21L);
            test.Rows[3]["Longs"] = 21L ;         Assert.That(test.LongValue(3, "Longs", 2147483648L), Is.equal_to, 21L);
            test.Rows[3]["Alpha"] = "21.1";       Assert.That(test.LongValue(3, "Alpha", 2147483648L), Is.equal_to, 21L);
                                                                                                         
            test.Add("Letter", "D", "Label", "Don't know");                                              
            test.Rows[3]["Large"] = 2147483649L;  Assert.That(test.LongValue(3, "Large", 2147483648L), Is.equal_to, 2147483649L);
            test.Rows[3]["Small"] = 2200000000L;  Assert.That(test.LongValue(3, "Small", 2147483648L), Is.equal_to, 2200000000L);
            test.Rows[3]["Longs"] = 2147483649L;  Assert.That(test.LongValue(3, "Longs", 2147483648L), Is.equal_to, 2147483649L);
            test.Rows[3]["Alpha"] = "2147483649"; Assert.That(test.LongValue(3, "Alpha", 2147483648L), Is.equal_to, 2147483649L);


            _result += Assert.Conclusion;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_StrValue_tests -->
		/// <summary>
		///      Tests the RichDataTable.StrValue method
		/// </summary>
		///[Test]
		public void RichDataTable_StrValue_tests()
		{
            Assert.ThingsAbout("RichDataTable", "StrValue");


            RichDataTable test = YNtable();

            test.Add("Whole", typeof(int)     ); Assert.That(test.StrValue(0, "Whole", "ABC"), Is.equal_to, "ABC");
            test.Add("Digit", typeof(char)    ); Assert.That(test.StrValue(0, "Digit", "ABC"), Is.equal_to, "ABC");
            test.Add("Real" , typeof(double)  ); Assert.That(test.StrValue(0, "Real" , "ABC"), Is.equal_to, "ABC");
            test.Add("Small", typeof(float)   ); Assert.That(test.StrValue(0, "Small", "ABC"), Is.equal_to, "ABC");
            test.Add("Short", typeof(Int16)   ); Assert.That(test.StrValue(0, "Short", "ABC"), Is.equal_to, "ABC");
            test.Add("Large", typeof(Int64)   ); Assert.That(test.StrValue(0, "Large", "ABC"), Is.equal_to, "ABC");
            test.Add("Alpha", typeof(string)  ); Assert.That(test.StrValue(0, "Alpha", "ABC"), Is.equal_to, "ABC");
            test.Add("Stuff", typeof(DateTime)); Assert.That(test.StrValue(0, "Stuff", "ABC"), Is.equal_to, "ABC");
                                                                              
            test.Rows[0]["Whole"] =  1   ;       Assert.That(test.StrValue(0, "Whole", "ABC"), Is.equal_to, "1"  );
            test.Rows[0]["Digit"] = '1'  ;       Assert.That(test.StrValue(0, "Digit", "ABC"), Is.equal_to, "1"  );
            test.Rows[0]["Real" ] =  1.9 ;       Assert.That(test.StrValue(0, "Real" , "ABC"), Is.equal_to, "1.9");
            test.Rows[0]["Small"] =  1.1 ;       Assert.That(test.StrValue(0, "Small", "ABC"), Is.equal_to, "1.1");
            test.Rows[0]["Short"] =  1   ;       Assert.That(test.StrValue(0, "Short", "ABC"), Is.equal_to, "1"  );
            test.Rows[0]["Large"] =  1L  ;       Assert.That(test.StrValue(0, "Large", "ABC"), Is.equal_to, "1"  );
            test.Rows[0]["Alpha"] = "1.1";       Assert.That(test.StrValue(0, "Alpha", "ABC"), Is.equal_to, "1.1");
            test.Rows[0]["Stuff"] = new DateTime();                                 
 
			test.Rows[1]["Whole"] =  0  ;		 Assert.That(test.StrValue(1, "Whole", "ABC"), Is.equal_to, "0");
			test.Rows[1]["Digit"] = '0' ;		 Assert.That(test.StrValue(1, "Digit", "ABC"), Is.equal_to, "0");
			test.Rows[1]["Real" ] =  0.0;		 Assert.That(test.StrValue(1, "Real" , "ABC"), Is.equal_to, "0");
			test.Rows[1]["Small"] =  0.0;		 Assert.That(test.StrValue(1, "Small", "ABC"), Is.equal_to, "0");
			test.Rows[1]["Short"] =  0  ;		 Assert.That(test.StrValue(1, "Short", "ABC"), Is.equal_to, "0");
			test.Rows[1]["Large"] =  0L ;		 Assert.That(test.StrValue(1, "Large", "ABC"), Is.equal_to, "0");
			test.Rows[1]["Alpha"] = "0" ;		 Assert.That(test.StrValue(1, "Alpha", "ABC"), Is.equal_to, "0");
                                                                                            
			test.Add("Letter", "M", "Label", "Maybe");                                      
			test.Rows[2]["Whole"] =  -1  ;		 Assert.That(test.StrValue(2, "Whole", "ABC"), Is.equal_to, "-1"  );
			test.Rows[2]["Digit"] = '-'  ;		 Assert.That(test.StrValue(2, "Digit", "ABC"), Is.equal_to, "-"   );
			test.Rows[2]["Real" ] =  -1.9;		 Assert.That(test.StrValue(2, "Real" , "ABC"), Is.equal_to, "-1.9");
			test.Rows[2]["Small"] =  -1.1;		 Assert.That(test.StrValue(2, "Small", "ABC"), Is.equal_to, "-1.1");
			test.Rows[2]["Short"] =  -1  ;		 Assert.That(test.StrValue(2, "Short", "ABC"), Is.equal_to, "-1"  );
			test.Rows[2]["Large"] =  -1L ;		 Assert.That(test.StrValue(2, "Large", "ABC"), Is.equal_to, "-1"  );
			test.Rows[2]["Alpha"] = "-1" ;		 Assert.That(test.StrValue(2, "Alpha", "ABC"), Is.equal_to, "-1"  );
                                                                                              
			test.Add("Letter", "D", "Label", "Don't know");                                   
			test.Rows[3]["Whole"] =  21   ;      Assert.That(test.StrValue(3, "Whole", "ABC"), Is.equal_to, "21"  );
			test.Rows[3]["Digit"] = '1'   ;      Assert.That(test.StrValue(3, "Digit", "ABC"), Is.equal_to, "1"   );
			test.Rows[3]["Real" ] =  21.9 ;      Assert.That(test.StrValue(3, "Real" , "ABC"), Is.equal_to, "21.9");
			test.Rows[3]["Small"] =  21.1 ;      Assert.That(test.StrValue(3, "Small", "ABC"), Is.equal_to, "21.1");
			test.Rows[3]["Short"] =  21   ;      Assert.That(test.StrValue(3, "Short", "ABC"), Is.equal_to, "21"  );
			test.Rows[3]["Large"] =  21L  ;      Assert.That(test.StrValue(3, "Large", "ABC"), Is.equal_to, "21"  );
			test.Rows[3]["Alpha"] = "21.1";      Assert.That(test.StrValue(3, "Alpha", "ABC"), Is.equal_to, "21.1");


            _result += Assert.Conclusion;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_TimeValue_test -->
		/// <summary>
		///      Tests the RichDataTable.TimeValue method
		/// </summary>
		///[Test]
		public void RichDataTable_TimeValue_tests()
		{
            Assert.ThingsAbout("RichDataTable", "TimeValue");


			RichDataTable test = YNtable();

            //test.Add("JrgDate", typeof(TimeDate_old));			Assert.That(test.TimeValue(0, "JrgDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, TimeDate_old.Null);
            //test.Add("SqlDate", typeof(SqlDateTime));			Assert.That(test.TimeValue(0, "SqlDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, TimeDate_old.Null);
            //test.Add("ClrDate", typeof(DateTime));			Assert.That(test.TimeValue(0, "ClrDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, TimeDate_old.Null);
            //test.Add("StrDate", typeof(string));			Assert.That(test.TimeValue(0, "StrDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, TimeDate_old.Null);

            //TimeDate_old timeDateNow = TimeDate_old.Now;
            //test.Rows[0]["JrgDate"] = timeDateNow;			Assert.That(test.TimeValue(0, "JrgDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, timeDateNow);
            //test.Rows[0]["SqlDate"] = timeDateNow.SQLFormat;			Assert.That(test.TimeValue(0, "SqlDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, timeDateNow);
            //test.Rows[0]["ClrDate"] = timeDateNow.CLRFormat;			Assert.That(test.TimeValue(0, "ClrDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, timeDateNow);
            //test.Rows[0]["StrDate"] = timeDateNow.ISOformat;			Assert.That(test.TimeValue(0, "StrDate", DateTimeKind.Unspecified, TimeDate_old.Null), __.equals, timeDateNow);

            //test.Rows[0]["JrgDate"] = TimeDate_old.MinSqlValue;			Assert.That(test.TimeValue(0, "JrgDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinSqlValue);
            //test.Rows[0]["SqlDate"] = TimeDate_old.MinSqlValue.SQLFormat;			Assert.That(test.TimeValue(0, "SqlDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinSqlValue);
            //test.Rows[0]["ClrDate"] = TimeDate_old.MinSqlValue.CLRFormat;			Assert.That(test.TimeValue(0, "ClrDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinSqlValue);
            //test.Rows[0]["StrDate"] = TimeDate_old.MinSqlValue.ISOformat;			Assert.That(test.TimeValue(0, "StrDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinSqlValue);

            //test.Rows[0]["JrgDate"] = TimeDate_old.MinClrValue;			Assert.That(test.TimeValue(0, "JrgDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinClrValue);
            //test.Rows[0]["SqlDate"] = TimeDate_old.MinClrValue.SQLFormat;			Assert.That(test.TimeValue(0, "SqlDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinClrValue);
            //test.Rows[0]["ClrDate"] = TimeDate_old.MinClrValue.CLRFormat;			Assert.That(test.TimeValue(0, "ClrDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinClrValue);
            //test.Rows[0]["StrDate"] = TimeDate_old.MinClrValue.ISOformat;			Assert.That(test.TimeValue(0, "StrDate", DateTimeKind.Utc, TimeDate_old.Null), __.equals, TimeDate_old.MinClrValue);

            //test.Rows[0]["StrDate"] = "3/17/2001";			Assert.That(test.TimeValue(0, "StrDate", DateTimeKind.Utc, TimeDate_old.Null).ToString(), __.equals, "3/17/2001 12:00:00 AM Utc");
            //DateTime dateTimeNow = DateTime.Now;
            //test.Rows[0]["ClrDate"] = dateTimeNow;			Assert.That(test.TimeValue(0, "ClrDate", DateTimeKind.Local, TimeDate_old.Null).CLRFormat.ToString(), __.equals, dateTimeNow.ToString());

            _result += Assert.Conclusion;
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- RichDataTable_ToEndemeSet_tests -->
		/// <summary>
		///      Tests the RichDataTable.ToEndemeSet method
		/// </summary>
		///[Test]
		public void RichDataTable_ToEndemeSet_tests()
		{
            Assert.ThingsAbout("RichDataTable", "ToEndemeSet");


			RichDataTable test = new RichDataTable();
			EndemeSet eSet = test.ToEndemeSet("YN", "", "", "");
            Assert.That(eSet.Count, Is.equal_to, 0);

			test = YNtable();
			eSet = test.ToEndemeSet("YN", "Letter", "Label", "Label");
            Assert.That(eSet['N'].Label, Is.equal_to, "No");
            Assert.That(eSet['Y'].Label, Is.equal_to, "Yes");
            Assert.That(eSet.Count, Is.equal_to, 2);
		  //Assert.That(eSet.Labels(), __.equals, "No, Yes");


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichSqlCommand_AddParameter_test -->
        /// <summary>
        ///      Tests the RichSqlCommand.AddParameter method
		/// </summary>
		/// <remarks></remarks>
		///[Test]
		public void RichSqlCommand_AddParameter_test()
		{
            Assert.ThingsAbout("RichSqlCommand", "AddParameter");


			RichSqlCommand cmd = new RichSqlCommand(CommandType.StoredProcedure, _conn
                , "SELECT * FROM Students WHERE StudentID = @StudentID"
                , Throws.Actions, "HIPLANET")
                .AddParameter("@StudentID", 3);
            IDataParameter param = (IDataParameter)cmd.Parameters[1];
            Assert.That((int)param.Value, Is.equal_to, 3);
            Assert.That(param.ParameterName, Is.equal_to, "@StudentID");
            Assert.That(cmd.Parameters.Count, Is.equal_to, 2);
            //RichSqlCommand_AddParameter_testcase(cmd, 2, SqlDbType.VarChar, "Name", "Jon", "AnsiString");
            //RichSqlCommand_AddParameter_testcase(cmd, 3, SqlDbType.Int, "Number", 12, "Int32");


            _result += Assert.Conclusion;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- Integer -->
        /// <summary>
        ///      Takes a shot at converting what is sent to it to an integer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int Integer(object obj, int defaultValue)
        {
            if (Is.Null(obj))
                return defaultValue;
            else
            {
                Type type = obj.GetType();
                string str;
                int value;


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32)
                    || type == typeof(Int64))
                    return (int)obj;
                str = obj.ToString();
                if (int.TryParse(str, out value))  return value;


                return defaultValue;
            }
        }
    }

    public class Ticker
    {
        public decimal Price { get; set; }

        public Ticker(decimal price)
        {
            Price = price;
        }
    }
}