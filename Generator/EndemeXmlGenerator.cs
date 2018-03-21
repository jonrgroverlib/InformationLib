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
using InformationLib.Data    ;        // for 
using InformationLib.Endemes ;        // for 
using InformationLib.HardData;        // for 
using InformationLib.Strings ;        // for 
using InformationLib.Testing ;        // for RandomSource
using System;                         // for Random
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Generator
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public class EndemeXmlGenerator : AbstractXmlGenerator
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- CreateDate -->
        /// <summary>
        ///      Creates a date string in one of these formats:   1999-05-31-05:00    1999-05-31Z
        ///      1999-05-31    1999-05--
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        protected override string CreateDate(string name)
        {
            RandomSource rs = RandomSource.New();
            int hoursOffset = rs.Skew(-1000, 1000);
            DateTime time = DateTime.UtcNow.AddHours(hoursOffset);
            Random r = rs.Random;
            string data = "";


            switch (r.Next(2))
            {
             // case 0 : data = time.ToString("yyyy-MM--") + CreateTimeZone(r);  break;
                default: data = time.ToString("yyyy-MM-dd") + CreateTimeZone(r); break;
            }

            return data;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateDateTime -->
        /// <summary>
        ///      One of the following formats:
        /// 1999-05-31T13:20:00.01-05:00
        /// +14:00..-14:00
        /// 1999-05-31
        /// 13:20:00-05:00
        /// 1999-05
        /// --05-17
        /// ---15
        /// --11--
        /// </summary>
        /// <returns></returns>
        protected override string CreateDateTime(string name)
        {
            string data = "";
            RandomSource rs = RandomSource.New();
            DateTime dtm = DateTime.UtcNow.AddHours(rs.Skew(-100, 100));
            Random r =  rs.Random;


            switch (r.Next(1))
            {
                case 0: data = dtm.ToString("yyyy-MM-ddTHH:mm:ss") + CreateTimeZone(r);  break;
                //case 1: data = dtm.ToString("yyyy-MM-dd")          + CreateTimeZone(r); break;
                //case 2: data = dtm.ToString("HH:mm:ss")            + CreateTimeZone(r); break;
            }

            return data;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateNumElements -->
        /// <summary>
        ///      Comes up with a random number based on the minimum and maximum bounds for a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override int CreateNumElements(RichXmlNode node)
        {
            if (node == null)
                return -999;


            string minOccurs = node.Attribute("minOccurs");  //  included minBound
            string maxOccurs = node.Attribute("maxOccurs");
            Random r = RandomSource.New().Random;
            int minNum = 1;
            int maxNum = 1;
            int num = 1;


            if (node.Attribute("name") == "PV2_26")
                Pause();


            // --------------------------------------------------------------------------
            //  Determine number of elements to create
            // --------------------------------------------------------------------------
            if (!string.IsNullOrEmpty(minOccurs)) minNum = _gen_.Integer(minOccurs, 0);
            if (!string.IsNullOrEmpty(maxOccurs))
                maxNum = _gen_.Integer(maxOccurs, minNum + r.Next(1, 4) * r.Next(0, 5));
            num = r.Next(minNum, maxNum + 1);


            // --------------------------------------------------------------------------
            //  If something strange happened...
            // --------------------------------------------------------------------------
            if (minNum > maxNum)
                throw new ArithmeticException("XmlGenerator.NumElements error - min number greater than max number");

            return num;
        }

        private void Pause()
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateNumberOfElements -->
        /// <summary>
        ///      Comes up with a random number based on the minimum and maximum bounds for a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override int CreateNumberOfElements(RichXmlNode node)
        {
            int num = CreateNumElements(node);
            return num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreatePrettyLabel -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string CreatePrettyLabel(string name)
        {
            string data;
            RandomSource rs = RandomSource.New();
            //Random r = rs.Random;
            string labelType = LblType(name);   // encapsulate
            switch (labelType)
            {
                case "name" : data = DbLabels.CreatePrettyLabel(rs.Skew(1, 512), LabelType.name); break;
                default: data = DbLabels.CreatePrettyLabel(rs.Skew(1, 512), LabelType.any); break;
            }
            return data;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateString -->
        /// <summary>
        ///      Creates a string to meet a particular set of restrictions
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string CreateString(NodeCreationCommand cmd, string name)
        {
            string str = "";

            if (Regex.IsMatch(name, "amount", RegexOptions.IgnoreCase))
                Pause();


            // --------------------------------------------------------------------------
            //  Use the restrictions (new <--> now)
            // --------------------------------------------------------------------------
            RandomSource rs = RandomSource.New();
            Random r = rs.Random;
            if (cmd.Enumerations != null && cmd.Enumerations.Count > 0)
            {
                int e = r.Next(cmd.Enumerations.Count);
                str = cmd.Enumerations[e];
            }
            else
            {
                switch (cmd.MaxLength)
                {
                    case -1:   // encapsulate
                        if (string.IsNullOrEmpty(cmd.RegexPattern))
                        {
                            bool found = false;   // encapsulate
                            if (!found && IsNumericField(name))
                                { found = true;  str = new RegexGenerator("[0-9]").CreateString(r, rs.Skew(1, 49), cmd.WhiteSpace); }
                            if (!found && IsDateTime(name))
                            { found = true;  str = CreateDateTime(name); }
                            if (!found)  str = CreatePrettyLabel(name);   // encapsulate
                         }
                        else
                        {
                            bool found = false;
                            if (IsNumericField(name))   // encapsulate
                            {
                                str = new RegexGenerator(cmd.RegexPattern).CreateString(r, rs.Skew(7, 50), cmd.WhiteSpace);
                                found = true;
                            }
                            if (IsDigitField(name))   // encapsulate
                            {
                                str = new RegexGenerator(cmd.RegexPattern).CreateString(r, 1, cmd.WhiteSpace);
                                found = true;
                            }
                            if (!found)
                                str = new RegexGenerator(cmd.RegexPattern).CreateString(r, cmd.WhiteSpace);
                        }
                        break;
                    case 0: str = ""; break;
                    default:
                        if (!string.IsNullOrEmpty(cmd.RegexPattern))
                        {
                            // ----------------------------------------------------------
                            //  convert a Regex pattern into a random string that it will match
                            // ----------------------------------------------------------
                            RegexGenerator genRegex = new RegexGenerator(cmd.RegexPattern);
                            str = genRegex.CreateString(r, cmd.WhiteSpace);
                            for (int i = 0; str.Length > cmd.MaxLength && i < 10; i++)
                                str = genRegex.CreateString(r, cmd.WhiteSpace);
                        }
                        else   // encapsulate
                            str = DbLabels.CreatePrettyLabel(r.Next(1, cmd.MaxLength + 1), LabelType.any);
                        break;
                }
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CreateTimeZone -->
        /// <summary>
        ///      Creates a validXML time zone
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected override string CreateTimeZone(Random r)
        {
            string timeZone = "";

            switch (r.Next(4))
            {
                case 0: timeZone = "-0" + r.Next(0, 10) + ":00"; break;
                case 1: timeZone = "+0" + r.Next(0, 10) + ":00"; break;
                case 2: timeZone = "+00:00";  break;
                case 3: timeZone = "Z";       break;
                default: timeZone = "";       break;
            }


            return timeZone;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsDateTime -->
        /// <summary>
        ///      Identifies special date and time type fields
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override bool IsDateTime(string name)
        {
            return Regex.IsMatch(name, "(date|time)", RegexOptions.IgnoreCase);   // encapsulate
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsDigitField -->
        /// <summary>
        ///      Identifies special digit type fields
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override bool IsDigitField(string name)
        {
            return Regex.IsMatch(name, "digit", RegexOptions.IgnoreCase)
                || Regex.IsMatch(name, "priority", RegexOptions.IgnoreCase)
                || Regex.IsMatch(name, "order$", RegexOptions.IgnoreCase);   // encapsulate
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsIdField -->
        /// <summary>
        ///      Identifies special ID type fields
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override bool IsIdField(string name)
        {
            return Regex.IsMatch(name, "(ID)$", RegexOptions.IgnoreCase)
                || Regex.IsMatch(name, "SetID", RegexOptions.IgnoreCase);   // encapsulate
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsNumericField -->
        /// <summary>
        ///      Identifies special ID type fields
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override bool IsNumericField(string name)
        {
            return Regex.IsMatch(name, "(days|minutes|number|ID)$", RegexOptions.IgnoreCase)
                || Regex.IsMatch(name, "SetID", RegexOptions.IgnoreCase);   // encapsulate
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LblType -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string LblType(string name)
        {
            if (Regex.IsMatch(name, "name", RegexOptions.IgnoreCase))  return "name";   // encapsulate
            if (Regex.IsMatch(name, "iii", RegexOptions.IgnoreCase))  return "suffix";   // encapsulate
            return "any";
        }
    }
}
