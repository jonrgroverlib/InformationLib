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
using InformationLib.Testing;         // for Here
using System;                         // for Guid
using System.Collections.Generic;     // for List<>
using System.Data.SqlTypes;           // for SqlBoolean
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- DataTests -->
    /// <summary>
    ///      The DataTests class tests the classes in the Data Library
    /// </summary>
    public class DataTests
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private Result _result;


        // ----------------------------------------------------------------------------------------
        /// <!-- AllTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            string result = "";


            string b = SqlBoolean.True.ToString();
            b = SqlBoolean.False.ToString();
            b = SqlBoolean.Null.ToString();


            RichDataTable_RichSqlCommand_test();
            RichDataTable_Add_test();           
            RichDataTable_Copy_test();          
            RichDataTable_Distinct_test();      
            RichDataTable_Split_test();         
            RichDataTable_XmlField_test();      
            RichSqlCommand_ExecuteReader_test();      


            result += "\r\n" + "test done";
            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichDataTable_Split_test -->
        /// <summary>
        /// 
        /// </summary>
        public void RichDataTable_Split_test()
        {
            Assert.ThingsAbout("RichDataTable", "Split");


            RichDataTable rt = SimpleTestTable();
            List<RichDataTable> table = rt._Split("Age");
            Assert.That(table.Count, Is.equal_to, 2);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichDataTable_Distinct_test -->
        /// <summary>
        /// 
        /// </summary>
        public void RichDataTable_Distinct_test()
        {
            Assert.ThingsAbout("RichDataTable", "Distinct");

            RichDataTable rt = SimpleTestTable();
            List<object> list = rt.Distinct("Age");
            Assert.That(list.Count, Is.equal_to, 2);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichDataTable_Add_test -->
        /// <summary>
        /// 
        /// </summary>
        public void RichDataTable_Add_test()
        {
            Assert.ThingsAbout("RichDataTable", "Add");

            RichDataTable rt = SimpleTestTable();
            RichDataTable rt2 = rt.Clone();
            Assert.That(rt2.Columns[1].ColumnName, Is.equal_to, rt.Columns[1].ColumnName);

            rt2.Add(1, rt); // _CopyRowToTable(1, rt2);
            rt2.Add(0, rt); // _CopyRowToTable(0, rt2);
            int age1 = rt.IntValue(1, 1, 1);
            int age2 = rt2.IntValue(0, 1, 2);
            Assert.That(age2, Is.equal_to, age1);
            string name1 = rt.StrValue(0, 0, "a");
            string name2 = rt2.StrValue(1, 0, "b");
            Assert.That(name2, Is.equal_to, name1);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichDataTable_Copy_test -->
        /// <summary>
        /// 
        /// </summary>
        public void RichDataTable_Copy_test()
        {
            Assert.ThingsAbout("RichDataTable", "Copy");

            RichDataTable rt = SimpleTestTable();
            RichDataTable rt2 = rt.Copy();
            int row = rt.Count - 1;
            Assert.That(rt2.StrValue(row, 0, "b"), Is.equal_to, rt2.StrValue(row, 0, "a"));
            Assert.That(rt2.IntValue(row, 1, 1)  , Is.equal_to, rt2.IntValue(row, 1, 0));

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichDataTable_RichSqlCommand_test -->
        /// <summary>
        /// 
        /// </summary>
        public void RichDataTable_RichSqlCommand_test()
        {
            Assert.ThingsAbout("RichDataTable", "RichSqlCommand");


            //_result = "UNTESTED";
            //RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, DBrules.Conn
            //    , "SELECT * FROM Star.Facility"
            //    , ExceptionAction.Squawk);
            //RichDataTable facility = new RichDataTable(cmd, trx, "Facility");
            //ok &= Assert.That(facility.Columns.Count > 0);
            //_result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichDataTable_XmlField_test -->
        /// <summary>
        /// 
        /// </summary>
        public void RichDataTable_XmlField_test()
        {
            Assert.ThingsAbout("RichDataTable", "XmlField");


            //// --------------------------------------------------------------------------
            ////  Test the _XmlField method
            //// --------------------------------------------------------------------------
            //Guid patientGuid = new Guid("e5dd0aff-e5b5-4d2e-b476-00fb50f59f04"); // An A08
            //RichDataTable patient = (new HL7Procedure()).Test_ADTrecord_Read(patientGuid);
            //Assert.That(string.IsNullOrEmpty(patient.Errors));
            //string xml = patient.StrValue(0, "ADTrecord", "");
            //Assert.That(!string.IsNullOrEmpty(xml));
            //string patientVisitNumber = patient._XmlStr(0, "ADTrecord", "", CommonXpath.VisitNumber);
            //Assert.That(patientVisitNumber, __.equals, "0830901493");
            //_result += Assert.Conclusion;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- RichSqlCommand_ExecuteReader_test -->
        /// <summary>
        /// 
        /// </summary>
        public void RichSqlCommand_ExecuteReader_test()
        {
            Assert.ThingsAbout("RichSqlCommand", "ExecuteReader");
            //RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, DBrules.Conn
            //    , "SELECT * FROM Star.Facility"
            //    , ExceptionAction.Squawk);
            //cmd._Open();
            //cmd._StatementCompleted = false;
            //SqlDataReader dr = cmd.ExecuteReader();
            //Assert.That(dr.FieldCount > 0);
            //cmd.Connection.Close();
            //_result += Assert.Conclusion;
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
            rt.Add("Name", typeof(string));
            rt.Add("Age",  typeof(int));
            int row;
            row = rt.Add(); rt.Rows[row]["Name"] = "Fred";    rt.Rows[row]["Age"] = 26;
            row = rt.Add(); rt.Rows[row]["Name"] = "Jon";     rt.Rows[row]["Age"] = 48;
            row = rt.Add(); rt.Rows[row]["Name"] = "Melinda"; rt.Rows[row]["Age"] = 26;
            return rt;
        }
    }
}

