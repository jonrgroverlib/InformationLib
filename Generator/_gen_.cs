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
using System;                                     // for DateTime, Guid
using System.Collections.Generic;                 // for Dictionary<,>
using System.Data;                                // for NoNullAllowedException
using System.Data.SqlTypes;                       // for SqlDateTime
using System.IO;                                  // for File, StreamWriter, TextReader
using System.Reflection;                          // for BindingFlags, MethodInfo
using System.Text;                                // for UTF8Encoding
using System.Text.RegularExpressions;             // for Regex
using System.Xml;                                 // for XmlNode
using System.Xml.Schema;                          // for XmlSchemaSet etc
using System.Windows.Forms;     // add reference  // for CheckBox, ComboBox, KeyPressEventArgs

// ------------------------------------------------------------------------------------------------
/// <!-- namespace CHNw.CommonLib.Core -->
/// <summary>
///      The CHNw.CommonLib.Core directory is a place to put various core classes, enums, and
///      interfaces that work together with the static _ methods.
/// Core namespace is an atom
/// </summary>
namespace InformationLib.Generator
{
    // --------------------------------------------------------------------------------------------
    /// <!-- _ -->
    /// <summary>
    ///      A copy of the functions needed from __  , refactor until removed
    /// </summary>
    /// <remarks>beta code - used once in production, move this stuff to __</remarks>
    public static class _gen_
    {
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
            if (IsNull(obj))
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
        /// <!-- Str -->
        /// <summary>
        ///      Converts an object to a string defaulting as specified if it is a null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        public static string Str(object obj, string defaultValue)
        {
            if (IsNull(obj)) return defaultValue;
            else
            {
                if (obj.GetType() == typeof(Control))
                    return ((Control)obj).Text;
                return obj.ToString();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InOrder, InRange -->
        /// <summary>
        ///      Returns whether A, B and C are in order  (A <= B <= C)
        /// </summary>
        /// <param name="lo"></param>
        /// <param name="mid"></param>
        /// <param name="hi"></param>
        /// <returns></returns>
        public static bool InOrder(long lo, long mid, long hi) { return (lo <= mid && mid <= hi); }
        /// <summary>Returns whether a number is in range (lo &lt;= num &lt; hi)</summary>
        public static bool InRange(long lo, long num, long hi) { return (lo <= num && num < hi); }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsNull -->
        /// <summary>
        ///      Determines whether an object is one or another of the standard null values
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            // --------------------------------------------------------------------------
            //  Check for standard null values
            // --------------------------------------------------------------------------
            if (obj == null || obj == DBNull.Value)
                return true;


            // --------------------------------------------------------------------------
            //  Investigate the specific Sql types for null value
            // --------------------------------------------------------------------------
            string type = obj.GetType().ToString();
            if (Regex.IsMatch(type, "^System.Data.SqlTypes"))
                switch (type)
                {
                    case "System.Data.SqlTypes.SqlBoolean" : if (((SqlBoolean) obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlByte"    : if (((SqlByte)    obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlDateTime": if (((SqlDateTime)obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlDecimal" : if (((SqlDecimal) obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlDouble"  : if (((SqlDouble)  obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlGuid"    : if (((SqlGuid)    obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlInt16"   : if (((SqlInt16)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlInt32"   : if (((SqlInt32)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlInt64"   : if (((SqlInt64)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlMoney"   : if (((SqlMoney)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlSingle"  : if (((SqlSingle)  obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlString"  : if (((SqlString)  obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlXml"     : if (((SqlXml)     obj).IsNull)  return true; break;
                }


            return false;
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
                throw new NoNullAllowedException("WriteStringToFile error - content is null");
            }
            else
            {
                string[] lines = new string[1];
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
                catch { throw new IOException("Unable to write string to file "+filePath); }
                finally { if (fileWriter != null)  fileWriter.Close(); }
            }
        }
    }
}
