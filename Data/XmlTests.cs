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
using System;                         // for Byte
using System.Data.SqlTypes;           // for SqlXml
using System.IO;                      // for MemoryStream
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- XmlTests -->
    /// <summary>
    ///      The XmlTests class tests the classes in the Xml Library
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public class XmlTests
    {
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


            FixXml_test();
            RichXmlDocument_Namespace_test();
            result += "\r\n" + "Xml test succeeded";


            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FixXml_test -->
        /// <summary>
        /// 
        /// </summary>
        public void FixXml_test()
        {
            Assert.ThingsAbout("RichXml", "FixXml");


            string input  = "<CHNwCommon xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://microsoft.com/HealthCare/HL7/2X\" /><EVN_EventType xmlns=\"\"><EVN_1_EventTypeCode>A08</EVN_1_EventTypeCode><EVN_2_DateTimeOfEvent>2009-11-03T09:46:31.0000000-05:00</EVN_2_DateTimeOfEvent><EVN_3_DateTimePlannedEvent>2009-11-03T09:46:31.0000000-05:00</EVN_3_DateTimePlannedEvent><EVN_4_EventReasonCode /><EVN_5_OperatorId>MWT</EVN_5_OperatorId><EVN_CHNw_MasterID /><EVN_CHNw_MRN>000520934</EVN_CHNw_MRN><EVN_CHNw_VisitNumber>0930700412</EVN_CHNw_VisitNumber></EVN_EventType>";
            string target = "<CHNwCommon xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://microsoft.com/HealthCare/HL7/2X\"><EVN_EventType xmlns=\"\"><EVN_1_EventTypeCode>A08</EVN_1_EventTypeCode><EVN_2_DateTimeOfEvent>2009-11-03T09:46:31.0000000-05:00</EVN_2_DateTimeOfEvent><EVN_3_DateTimePlannedEvent>2009-11-03T09:46:31.0000000-05:00</EVN_3_DateTimePlannedEvent><EVN_4_EventReasonCode /><EVN_5_OperatorId>MWT</EVN_5_OperatorId><EVN_CHNw_MasterID /><EVN_CHNw_MRN>000520934</EVN_CHNw_MRN><EVN_CHNw_VisitNumber>0930700412</EVN_CHNw_VisitNumber></EVN_EventType></CHNwCommon>";
            FixXml_testcase(input, target);
            input  = "<CHNwCommon xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://microsoft.com/HealthCare/HL7/2X\"></CHNwCommon><EVN_EventType xmlns=\"\"><EVN_1_EventTypeCode>A08</EVN_1_EventTypeCode><EVN_2_DateTimeOfEvent>2009-11-03T09:46:31.0000000-05:00</EVN_2_DateTimeOfEvent><EVN_3_DateTimePlannedEvent>2009-11-03T09:46:31.0000000-05:00</EVN_3_DateTimePlannedEvent><EVN_4_EventReasonCode /><EVN_5_OperatorId>MWT</EVN_5_OperatorId><EVN_CHNw_MasterID /><EVN_CHNw_MRN>000520934</EVN_CHNw_MRN><EVN_CHNw_VisitNumber>0930700412</EVN_CHNw_VisitNumber></EVN_EventType>";
            target = "<CHNwCommon xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://microsoft.com/HealthCare/HL7/2X\"><EVN_EventType xmlns=\"\"><EVN_1_EventTypeCode>A08</EVN_1_EventTypeCode><EVN_2_DateTimeOfEvent>2009-11-03T09:46:31.0000000-05:00</EVN_2_DateTimeOfEvent><EVN_3_DateTimePlannedEvent>2009-11-03T09:46:31.0000000-05:00</EVN_3_DateTimePlannedEvent><EVN_4_EventReasonCode /><EVN_5_OperatorId>MWT</EVN_5_OperatorId><EVN_CHNw_MasterID /><EVN_CHNw_MRN>000520934</EVN_CHNw_MRN><EVN_CHNw_VisitNumber>0930700412</EVN_CHNw_VisitNumber></EVN_EventType></CHNwCommon>";
            FixXml_testcase(input, target);
            FixXml_testcase("<CHNwCommon /><EVN_EventType />", "<CHNwCommon><EVN_EventType /></CHNwCommon>");
        }
        private static void FixXml_testcase(string input, string target)
        {
            SqlXml xml = ToSqlXml(input);
            SqlXml newXml = new SqlXml();


            // fixing the problem
            string str = Fix(xml);


            Assert.That(str, Is.equal_to, target);
            // converting back to a SqlXml
            newXml = ToSqlXml(str);
        }
        private static string Fix(SqlXml xml)
        {
            string str = xml.Value;
            str = Regex.Replace(str, @"^<([^<> ]+) ([^<>]+) />(.*)$", "<$1 $2>$3</$1>"); // most likely
            str = Regex.Replace(str, @"^<([^<> ]+) ([^<>]+)></\1>(.*)$", "<$1 $2>$3</$1>");
            str = Regex.Replace(str, @"^<([^<> ]+) />(.*)$", "<$1>$2</$1>");
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RichXmlDocument_Namespace_test -->
        /// <summary>
        /// 
        /// </summary>
        private void RichXmlDocument_Namespace_test()
        {
            Assert.ThingsAbout("RichXml", "world");


            RichXmlDocument xdoc = new RichXmlDocument();
            xdoc.LoadXml("<richxml:CHNwCommon xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:richxml=\"http://microsoft.com/HealthCare/HL7/2X\">"
                + "<EVN_EventType xmlns=\"\"><EVN_1_EventTypeCode>A08</EVN_1_EventTypeCode><EVN_2_DateTimeOfEvent>2008-11-07T15:09:33.0000000-05:00</EVN_2_DateTimeOfEvent><EVN_3_DateTimePlannedEvent xsi:nil=\"true\" /><EVN_4_EventReasonCode /><EVN_5_OperatorId>CHNwCommon</EVN_5_OperatorId><EVN_CHNw_MasterID>HNE1828095846</EVN_CHNw_MasterID><EVN_CHNw_MRN>01127821</EVN_CHNw_MRN><EVN_CHNw_VisitNumber>0830901493</EVN_CHNw_VisitNumber></EVN_EventType>"
                + "</richxml:CHNwCommon>");
            Assert.That(xdoc.NameSpace, Is.equal_to, "richxml");

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToSqlXml -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static SqlXml ToSqlXml(string str)
        {
            Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
            MemoryStream stream = new MemoryStream(bytes);
            SqlXml xml = new SqlXml(stream);
            return xml;
        }
    }
}
