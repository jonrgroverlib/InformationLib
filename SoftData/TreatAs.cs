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
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Data;                    // for DataException
using System.Data.SqlTypes;           // for SqlDateTime
using System.Drawing;                 // for Image
using System.Globalization;           // for NumberStyles, CultureInfo
using System.IO;                      // for File, StreamWriter, TextReader
using System.Linq;                    // for 
using System.Data.Linq;
//using System.Numerics;                // for BigInteger
using System.Reflection;              // BindingFlags, PropertyInfo
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for XmlNode

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- TreatAs -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>production ready</remarks>
    public static class TreatAs
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- BoolValue -->
        /// <summary>
        ///      Takes its best shot at converting whatever is sent to it to a boolean value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue">returns this for blank, null, or unidentified</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static bool BoolValue(object obj, bool defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                Type type = obj.GetType();
                string str;
                //bool value;


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (type == typeof(bool) || type == typeof(Boolean))
                    return (bool)obj;
                str = obj.ToString();
                //if (bool.TryParse(str, out value))  return value;


                // ----------------------------------------------------------------------
                //  Blank values
                // ----------------------------------------------------------------------
                string guidtype = typeof(Guid).ToString();
                if (type.ToString() == "System.Guid")
                {
                    Guid g = (Guid)obj;
                    if (g.CompareTo(Guid.Empty) == 0) return false;
                    else return true;
                }


                if (type.Name == "CheckBox")
                {
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)obj;
                    return chk.Checked;
                }

                if (str == type.ToString())
                    throw new DataException("_.Bool - ToString() not defined in any useful way for " + type.ToString());


                SqlBoolean sb = SqlBool(str, defaultValue);
                if (sb.IsNull) return defaultValue;
                else return ((bool)SqlBool(str, defaultValue));
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ByteValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte[] ByteValue(object obj, byte[] defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                Type type = obj.GetType();
                byte[] output = null;


                // already the right type:
                if      (type == typeof(byte[])) return (byte[])obj;
              //else if (type == typeof(Byte[])) return (Byte[])obj; // I think this is the same
                // primitive types:
                else if (type == typeof(decimal) || type == typeof(Decimal)) { output = BitConverter.GetBytes(Convert.ToDouble((decimal)obj)); } // 96 (this works but it is probably lossy)
                else if (type == typeof(long   ) || type == typeof(Int64  )) { output = BitConverter.GetBytes((long  )obj); } // 64
                else if (type == typeof(double ) || type == typeof(Double )) { output = BitConverter.GetBytes((double)obj); } // 64
                else if (type == typeof(ulong  ) || type == typeof(UInt64 )) { output = BitConverter.GetBytes((ulong )obj); } // 64
                else if (type == typeof(int    ) || type == typeof(Int32  )) { output = BitConverter.GetBytes((int   )obj); } // 32
                else if (type == typeof(float  ) || type == typeof(Single )) { output = BitConverter.GetBytes((float )obj); } // 32
                else if (type == typeof(ushort ) || type == typeof(UInt16 )) { output = BitConverter.GetBytes((ushort)obj); } // 32
                else if (type == typeof(short  ) || type == typeof(Int16  )) { output = BitConverter.GetBytes((short )obj); } // 16
                else if (type == typeof(uint   ) || type == typeof(UInt32 )) { output = BitConverter.GetBytes((uint  )obj); } // 16
                else if (type == typeof(byte   ) || type == typeof(Byte   )) { output = BitConverter.GetBytes((byte  )obj); } // 8
                else if (type == typeof(char   ) || type == typeof(Char   )) { output = BitConverter.GetBytes((char  )obj); } // 2
                else if (type == typeof(bool   ) || type == typeof(Boolean)) { output = BitConverter.GetBytes((bool  )obj); } // 1
                // types with special conversion methods:
                else if (type == typeof(string    )) { output = Encoding.Unicode.GetBytes((string    )obj);                } // variable
              //else if (type == typeof(String    )) { output = Encoding.Unicode.GetBytes((string    )obj);                } // variable // I think this is the same
              //else if (type == typeof(BigInteger)) { output =                          ((BigInteger)obj).ToByteArray() ; } // variable
                else if (type == typeof(Binary    )) { output =                          ((Binary    )obj).ToArray    () ; } // variable
                else if (type == typeof(Guid      )) { output =                          ((Guid      )obj).ToByteArray() ; } // 128
                else if (type == typeof(DateTime  )) { output = BitConverter   .GetBytes(((DateTime  )obj).ToBinary   ()); } // 64
                // using memory streams:
                else if (type == typeof(Image     )) { using (Image img = ((Image)obj)) { using (MemoryStream stream = new MemoryStream()) { img.Save(stream, img.RawFormat); output = stream.ToArray(); } } }
                else                        { BinaryFormatter bf = new BinaryFormatter(); using (MemoryStream stream = new MemoryStream()) { bf.Serialize(stream, obj);       output = stream.ToArray(); } }

                return output;
            }
        }
        public static byte ByteValue(object obj, byte defaultValue)        { byte[] array = ByteValue(obj, new byte[] { defaultValue }); if (array.Length > 0) return array[0]; else return defaultValue; }

        // ----------------------------------------------------------------------------------------
        /// <!-- CharValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static char CharValue(object obj, char defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                Type type = obj.GetType();
                string str;


                if (   type == typeof(int)   || type == typeof(Int32) || type == typeof(uint)   || type == typeof(UInt32)
                    || type == typeof(long)  || type == typeof(Int64) || type == typeof(ulong)  || type == typeof(UInt64)
                    || type == typeof(short) || type == typeof(Int16) || type == typeof(ushort) || type == typeof(UInt16)
                    )
                {
                    int num = IntValue(obj, (int)defaultValue);
                    if (num > 9)
                        try   {  return Convert.ToChar(num); }
                        catch {  return defaultValue;        }
                }


                str = StrValue(obj, defaultValue.ToString());
                if (str.Length > 0)
                    return str.ToCharArray()[0];
                else
                    return defaultValue;
            }
        }
        public static char CharValue(object obj, char minChar, char defaultValue)
        {
            char c = CharValue(obj, defaultValue);
            if (c < minChar)
                c = defaultValue;
            return c;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Clip -->
        /// <summary>
        ///       Constrains an integer to a specific range
        /// </summary>
        /// <param name="num"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        public static int Clip(int num, int lowerLimit, int upperLimit)
        {
            return Math.Min(Math.Max(num, lowerLimit), upperLimit);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DecValue -->
        /// <summary>
        ///      Takes a shot at converting what is sent to it to a decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static decimal DecValue(object obj, decimal defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                Type type = obj.GetType();
                string str;
                decimal value;


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (type == typeof(decimal) || type == typeof(Decimal))
                    return (decimal)obj;
                str = obj.ToString();
                if (decimal.TryParse(str, out value)) return value;


                return defaultValue;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FloatValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">object to be cast as a float</param>
        /// <param name="defaultValue">value to be returned if you can't cast</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static float FloatValue(object obj, float defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                Type type = obj.GetType();
                string str;
                float value;


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (type == typeof(float)) return (float)obj;
                if (type == typeof(double) || type == typeof(Double))
                {
                    double num = (double)obj;
                    if (float.MinValue <= num && num <= float.MaxValue) return (float)num;
                    else return defaultValue;
                }
                str = obj.ToString();
                if (float.TryParse(str, out value)) return value;


                return defaultValue;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GuidValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static Guid GuidValue(object obj, Guid defaultValue)
        {
            Guid g = new Guid();
            if (!Is.Null(obj))
            {
                if (obj.GetType() == typeof(Guid)) return (Guid)obj;


                // ----------------------------------------------------------------------
                //  Try various string formats
                // ----------------------------------------------------------------------
                string str = obj.ToString();
                if (str.Length == 22 || str.Length == 24)
                    try { g = GUID(str); return g; }
                    catch { }
                else
                    if (IsGuid(str, out g)) return g;


                g = defaultValue;
            }
            else
                g = defaultValue;


            return defaultValue;
        }

        public static Guid? GuidValue(object obj)
        {
            if (Is.Null(obj))
            {
                return null;
            }
            else
            {
                if (obj.GetType() == typeof(Guid)) return (Guid)obj;


                Guid g = new Guid();
                string str = obj.ToString();
                if (str.Length == 22 || str.Length == 24)
                    try { g = GUID(str); return g; }
                    catch { }
                else
                    if (IsGuid(str, out g)) return g;

                return null;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GUID -->
        /// <summary>
        ///      Convert base64 string format to GUID (From Dave Transom's C# Vitamins)
        /// </summary>
        /// <param name="strGuid64"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static Guid GUID(string strGuid64)
        {
            string value = strGuid64.Replace("_", "/").Replace("-", "+");
            byte[] buffer = Convert.FromBase64String(value + "==");
            Guid guid = new Guid(buffer);
            return guid;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GUID -->
        /// <summary>
        ///      Converts a GUID to a base 64 string (From Dave Transom's C# Vitamins)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string GUID(Guid guid, string defaultValue)
        {
            string str64;
            try
            {
                string encoded = Convert.ToBase64String(guid.ToByteArray());
                str64 = encoded.Replace("/", "_").Replace("+", "-").Substring(0, 22);
            }
            catch { str64 = defaultValue; }
            return str64;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Guid_0123456789abcdefx2 -->
        /// <summary>
        ///      {01234567-89ab-cdef-0123-456789abcdef}
        /// </summary>
        public static Guid Guid_0123456789abcdefx2
        { get { Guid g = new Guid("01234567-89ab-cdef-0123-456789abcdef"); return g; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Guid_Guid_01234567x4 -->
        /// <summary>
        ///      {00001111-2222-3333-4444-555566667777}
        /// </summary>
        public static Guid Guid_01234567x4
        { get { Guid g = new Guid("00001111-2222-3333-4444-555566667777"); return g; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- GuidChange -->
        /// <summary>
        ///      Did the Guid change and did the conversion succeed?
        /// </summary>
        /// <param name="initialID"></param>
        /// <param name="finalID"></param>
        /// <returns>1:yes,changed  0:no,did not changed  -1:conversion failed,final guid empty</returns>
        public static int GuidChange(Guid initialID, Guid finalID)
        {
            int val;
            if (finalID.Equals(Guid.Empty)) val = -1;
            else if (finalID.Equals(initialID)) val = 0;
            else val = 1;
            return val;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IntArray -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int[] IntArray(string str, int defaultValue)
        {
            char[] cha = str.ToCharArray();
            int[] array = new int[cha.Length];
            for (int i = 0; i < cha.Length; ++i)
            {
                int num = 0;
                if (int.TryParse(cha[i].ToString(), out num))
                    array[i] = num;
                else
                    array[i] = TreatAs.IntValue(cha[i], defaultValue);
            }

            return array;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IntValue -->
        /// <summary>
        ///       Returns an integer from a string within a specific range with a specific default
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultNum"></param>
        /// <param name="lowerLimit"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static int IntValue(string strNum, int lowerLimit, int defaultNum, int upperLimit)
        {
            int num;
            int result = defaultNum;
            if (int.TryParse(strNum, out num))
                result = Clip(num, lowerLimit, upperLimit);
            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IntValue -->
        /// <summary>
        ///      Takes a shot at converting what is sent to it to an integer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static int IntValue(object obj, int defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                int value = 0;
                value = NumToInt(obj             , defaultValue); if (value != defaultValue) return value;
                value = IntValue(obj, "Value"    , defaultValue); if (value != defaultValue) return value;
                value = IntValue(obj, "ItemValue", defaultValue); if (value != defaultValue) return value;
                value = IntValue(obj, "Text"     , defaultValue); if (value != defaultValue) return value;
                return  IntValue(obj.ToString(), defaultValue);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IntValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static byte[] ByteValue(object obj)
        {
            if (Is.Null(obj)) return new byte[0];
            else
            {
                Type typ = obj.GetType();
                byte[] output = new byte[0];


                if (typ == typeof(byte  )) { output = new byte[1]; output[0] = (byte)obj; return output; }
                if (typ == typeof(byte[])) { return (byte[])obj; }

                if (typ == typeof(int  )) { return BitConverter.GetBytes((int  )obj); }
                if (typ == typeof(Int16)) { return BitConverter.GetBytes((Int16)obj); }
                if (typ == typeof(Int32)) { return BitConverter.GetBytes((Int32)obj); }
                if (typ == typeof(Int64)) { return BitConverter.GetBytes((Int64)obj); }

                string input = obj.ToString();
                output = Encoding.ASCII.GetBytes(input);
              //output = ASCIIEncoding.Default.GetBytes(input);

                return output;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IntValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static int IntValue(object obj, string propertyName, int defaultValue)
        {
            Type         inst  = obj.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo prop  = inst.GetProperty(propertyName, flags);
            if (prop != null)
            {
                object val = prop.GetValue(obj, null);
                return IntValue(val, defaultValue);
            }
            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NumToInt -->
        /// <summary>
        ///      Converts a number to an integer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static int NumToInt(object obj, int defaultValue)
        {
            Type type = obj.GetType();

            // ----------------------------------------------------------------------
            //  Standard approaches
            // ----------------------------------------------------------------------
            if (type == typeof(int)     || type == typeof(Int32)  ) { return (int)       obj; }
            if (type == typeof(short)   || type == typeof(Int16)  ) { return (int)(short)obj; }
            if (type == typeof(decimal) || type == typeof(Decimal)) { try { return Decimal.ToInt32((decimal)obj); } catch { return defaultValue; } }
            if (type == typeof(double)  || type == typeof(Double) ) { try { return (int)           (double) obj ; } catch { return defaultValue; } }
            if (type == typeof(float )  || type == typeof(Single) ) { try { return (int)           (float)  obj ; } catch { return defaultValue; } }

            if (type == typeof(long)    || type == typeof(Int64)  )
            {
                long longNum = (long)obj;
                int intNum = (int)longNum;
                if (intNum == longNum) return intNum; else return defaultValue;
            }


            if (type == typeof(char)) { return (Convert.ToInt32((char)obj)); }

            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrToInt -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static int IntValue(string str, int defaultValue)
        {
            decimal decValue;
            if (decimal.TryParse(str, out decValue))
            {
                if (int.MinValue <= decValue && decValue <= int.MaxValue) return (int)decValue;
                else                                                      return defaultValue;
            }
            int intValue; if (int.TryParse (str, out intValue))           return intValue;
            else                                                          return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LongValue -->
        /// <summary>
        ///      Rounds down
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static long LongValue(object obj, long defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                Type type = obj.GetType();


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (type == typeof(long)    || type == typeof(Int64)  )         return           (long)         obj ;
                if (type == typeof(int)     || type == typeof(Int32)  )         return           (long)(int)    obj ;
                if (type == typeof(short)   || type == typeof(Int16)  )         return           (long)(short)  obj ;
                if (type == typeof(char))                                       return Convert.ToInt64((char)   obj);
                if (type == typeof(decimal) || type == typeof(Decimal)) { try { return Decimal.ToInt64((decimal)obj); } catch { return defaultValue; } }
                if (type == typeof(double)  || type == typeof(Double) ) { try { return           (long)(double) obj ; } catch { return defaultValue; } }
                if (type == typeof(float)   || type == typeof(Single) ) { try { return           (long)(float)  obj ; } catch { return defaultValue; } }


                // ----------------------------------------------------------------------
                //  Make one last attempt
                // ----------------------------------------------------------------------
                string str;
                str = obj.ToString();
                decimal decValue;  if (decimal.TryParse(str, out decValue )) return (long)decValue;
                long    longValue; if (Int64.TryParse  (str, out longValue)) return longValue;


                return defaultValue;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsGuid -->
        private static Regex _isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
        /// <summary>
        ///      VIA Scott Galloway's Blog - In this implementation you pass in a Guid as an 'out'
        ///      parameter along with the string you want to test - it then fills in the Guid and
        ///      returns true / false depending on whether the Guid was valid... 
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        internal static bool IsGuid(string candidate, out Guid output)
        {
            bool isValid = false;
            output = Guid.Empty;
            if (candidate != null)
            {
                if (_isGuid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }
            return isValid;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Largest -->
        /// <summary>
        ///      Returns the largest item in the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static int Largest(int[] list)
        {
            int max;
            max = Int32.MinValue;
            for (int i = 0; i < list.Length - 1; i++)
            {
                if (list[i] > max) max = list[i];
            }
            return max;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NullableDateTimeValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static DateTime? NullableDateTimeValue(object obj, DateTime? defaultValue)
        {
            DateTime? time = null;

            if (obj != null)
            {
                string str = obj.ToString();
                DateTime datetime;
                if (DateTime.TryParse(str, out datetime))
                {
                    time = datetime;
                }
            }

            return time;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ObjValue -->
        /// <summary>
        ///      Returns the default if the object is null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static object ObjValue(object value, object defaultValue)
        {
            if (value ==null) return defaultValue; else return value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RealValue -->
        /// <summary>
        ///      Does its best to return a double value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double RealValue(object obj, double defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;


            // --------------------------------------------------------------------------
            //  Simple casting
            // --------------------------------------------------------------------------
            Type type = obj.GetType();
            if (type == typeof(float ) || type == typeof(double) || type == typeof(decimal) ||
                type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal)
               )
            {
                return (double)obj;
            }


            // --------------------------------------------------------------------------
            //  Conversions
            // --------------------------------------------------------------------------
            if (type == typeof(bool   ) || type == typeof(byte  ) || type == typeof(sbyte  ) ||
                type == typeof(Boolean) || type == typeof(Byte  ) || type == typeof(SByte  ) ||
                type == typeof(ushort ) || type == typeof(uint  ) || type == typeof(ulong  ) ||
                type == typeof(UInt16 ) || type == typeof(UInt32) || type == typeof(UInt64 ) ||
                type == typeof(short  ) || type == typeof(int   ) || type == typeof(long   ) ||
                type == typeof(Int16  ) || type == typeof(Int32 ) || type == typeof(Int64  )
               )
            {
                return Convert.ToDouble(obj);
            }


            // --------------------------------------------------------------------------
            //  Simple Parsing
            // --------------------------------------------------------------------------
            string str = obj.ToString().Trim();
            double value;
            if (double.TryParse(str, out value)) return value;


            // --------------------------------------------------------------------------
            //  Aggressive Parsing
            // --------------------------------------------------------------------------
            str = Regex.Replace(str, "[^-0-9.]", "" );
            str = Regex.Replace(str, "-"  , "Q");  str = Regex.Replace(str, "^Q"      ,   "-");  str = Regex.Replace(str, "Q", "");
            str = Regex.Replace(str, "[.]", "Q");  str = Regex.Replace(str, "^([^.])Q", "$1.");  str = Regex.Replace(str, "Q", "");
            if (double.TryParse(str, out value)) return value;

            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RealValue -->
        /// <summary>
        ///      Returns a double based on input object and culture
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double RealValue(object obj, CultureInfo culture, double defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;


            // --------------------------------------------------------------------------
            //  Simple casting
            // --------------------------------------------------------------------------
            Type type = obj.GetType();
            if (type == typeof(float ) || type == typeof(double) || type == typeof(decimal) ||
                type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal) ||
                type == typeof(short ) || type == typeof(int   ) || type == typeof(long   ) ||
                type == typeof(Int16 ) || type == typeof(Int32 ) || type == typeof(Int64  )
               )
            {
                return (double)obj;
            }


            // --------------------------------------------------------------------------
            //  Conversions
            // --------------------------------------------------------------------------
            if (type == typeof(bool   ) || type == typeof(byte  ) || type == typeof(sbyte  ) ||
                type == typeof(Boolean) || type == typeof(Byte  ) || type == typeof(SByte  ) ||
                type == typeof(ushort ) || type == typeof(uint  ) || type == typeof(ulong  ) ||
                type == typeof(UInt16 ) || type == typeof(UInt32) || type == typeof(UInt64 )
               )
            {
                return System.Convert.ToDouble(obj);
            }


            // --------------------------------------------------------------------------
            //  Culture based parsing
            // --------------------------------------------------------------------------
            string str = obj.ToString().Trim();
            double value;
            if (double.TryParse(str, NumberStyles.Number, culture, out value)) return value;


            // --------------------------------------------------------------------------
            //  Aggressive parsing
            // --------------------------------------------------------------------------
            str = Regex.Replace(str, "[^-0-9.,]", "" );
            str = Regex.Replace(str, "-"  , "Q");  str = Regex.Replace(str, "^Q"      ,   "-");  str = Regex.Replace(str, "Q", "");
            if (double.TryParse(str, NumberStyles.Number, culture, out value)) return value;

            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SqlBool -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static SqlBoolean SqlBool(object obj, SqlBoolean defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                Type type = obj.GetType();


                // ----------------------------------------------------------------------
                //  Common situations
                // ----------------------------------------------------------------------
                if (type == typeof(SqlBoolean) || type == typeof(Boolean) || type == typeof(bool))
                    return (SqlBoolean)obj;
                string str = obj.ToString();
                if (str == type.ToString())
                    throw new DataException("_.SqlBool - ToString() not defined in any useful way for " + type.ToString());


                // ----------------------------------------------------------------------
                //  Convert string
                // ----------------------------------------------------------------------
                return SqlBool(str, defaultValue);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SqlBool -->
        /// <summary>
        ///      Tries to figure out whether a value in a column means true or false
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static SqlBoolean SqlBool(string str, SqlBoolean defaultValue)
        {
            // --------------------------------------------------------------------------
            //  Common values
            // --------------------------------------------------------------------------
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            try { SqlBoolean value = SqlBoolean.Parse(str); return value; }
            catch { }


            // --------------------------------------------------------------------------
            //  Additional approaches - 
            //  1/0, acceptable/unacceptable, complete/incomplete, correct/incorrect
            //  , engaged/disengaged, equal/unequal, good/bad, high/low, in/out
            //  , non-null/null, ok/not ok, open/closed, positive/negative
            //  , present/absent, right/wrong, true/false, yes/no
            //  etc.
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(str, "^ *(abnorm|cancel|clos|debug|delet|dis|fail|hid|inactive|inc|invalid|kill|lock|mis|negat|not|nul|reject|stop|un|wors)"                                     , RegexOptions.IgnoreCase)) return false;
            if (Regex.IsMatch(str, "^ *(absent|bad|blank|down|error|evil|false|garbage|idle|lo|lose|lost|low|never|nil|no|nil|not ok|off|out|passive|poor|quit|reject|wrong)$"                 , RegexOptions.IgnoreCase)) return false;

            if (Regex.IsMatch(str, "^ *(accept|activ|approv|assign|can|comp|correct|enabl|engag|excel|has|info|is|liv|match|non-nul|nonnul|posit|process|select|start|succe|valid|viab|visib)" , RegexOptions.IgnoreCase)) return true;
            if (Regex.IsMatch(str, "^ *(best|better|continue|equal|fair|fine|good|hi|high|in|normal|ok|on|open|pass|pending|present|right|running|trace|true|up|usable|used|win|won|yes)$"     , RegexOptions.IgnoreCase)) return true;


            // --------------------------------------------------------------------------
            //  First letters, numbers and symbols:
            //  Y/N, T/F, R/W, A/U, E/D, G/B, H/L, X/O, C/Q, P/I, V/K, JMSZ?, +/-, &/|, 1-9/0, >/<
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(str, "^ *[YTRAEGHXCPV]$", RegexOptions.IgnoreCase)) return true;
            if (Regex.IsMatch(str, "^ *[NFWUDBLOQIK]$", RegexOptions.IgnoreCase)) return false;
            if (Regex.IsMatch(str, "^ *[+1-9&>]$"     , RegexOptions.IgnoreCase)) return true;
            if (Regex.IsMatch(str, "^ *[1-9][0-9]+$"  , RegexOptions.IgnoreCase)) return true;
            if (Regex.IsMatch(str, "^ *[-0|<]$"       , RegexOptions.IgnoreCase)) return false;


            // --------------------------------------------------------------------------
            //  Give up
            // --------------------------------------------------------------------------
            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrValue -->
        /// <summary>
        ///      
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string StrValue(object obj, bool defaultValue, string trueValue, string falseValue)
        {
            bool valueIsTrue = TreatAs.BoolValue(obj, defaultValue);
            if (valueIsTrue) return trueValue; else return falseValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrValue -->
        /// <summary>
        ///      Converts an object to a string defaulting as specified if it is a null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string StrValue(object obj, string defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            else
            {
                string str = "";
                str = StrValue(obj, "Text"     , defaultValue); if (str != defaultValue) return str;
                str = StrValue(obj, "Value"    , defaultValue); if (str != defaultValue) return str;
                str = StrValue(obj, "ItemValue", defaultValue); if (str != defaultValue) return str;
                str = StrValue(obj, "Name"     , defaultValue); if (str != defaultValue) return str;
                str = StrValue(obj, "Label"    , defaultValue); if (str != defaultValue) return str;
                return obj.ToString();
            }
        }

        internal static string StrValue(object obj)
        {
            if (Is.Null(obj)) return null;
            else
            {
                string str = "";
                str = StrValue(obj, "Text"     , null); if (str != null) return str;
                str = StrValue(obj, "Value"    , null); if (str != null) return str;
                str = StrValue(obj, "ItemValue", null); if (str != null) return str;
                return obj.ToString();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrValue -->
        /// <summary>
        ///      WARNING: do not use unless you know what you are doing
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string StrValue(object obj, string propertyName, string defaultValue)
        {
            Type         inst  = obj.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo prop  = inst.GetProperty(propertyName, flags);
            if (prop != null)
            {
                object val = prop.GetValue(obj, null);
                return StrValue(val, defaultValue);
            }
            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GuidValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid GuidValue(object obj, string propertyName, Guid defaultValue)
        {
            if (obj == null) return defaultValue;
            Type         inst  = obj.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo prop  = inst.GetProperty(propertyName, flags);


            if (prop != null)
            {
                object val = prop.GetValue(obj, null);
                if (val == null) return defaultValue;
                else return GuidValue(val, defaultValue);
            }
            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Time -->
        /// <summary>
        ///
        /// </summary>
        /// <param name="time"></param>
        /// <param name="defaultTime"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static TimeSpan Time(string str, TimeSpan defaultValue)
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            else
            {
                TimeSpan time;
                if (TimeSpan.TryParse(str, out time)) return time;
                else return defaultValue;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SqlXml -->
        /// <summary>
        ///      Example Usage:  SqlXml sx = SqlXml(tiny);
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static SqlXml SqlXml(string str)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            XmlNodeReader rdr = new XmlNodeReader(doc);
            SqlXml xml = new SqlXml(rdr);
            return xml;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToSqlXml -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static SqlXml ToSqlXml(string str)
        {
            Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
            MemoryStream stream = new MemoryStream(bytes);
            SqlXml xml = new SqlXml(stream);
            return xml;
        }

        // ----------------------------------------------------------------------------------------
        //  DateTime stuff
        // ----------------------------------------------------------------------------------------
        public static DateTime? UtcValue(SqlDateTime dateTime)
        {
            if (Is.Null(dateTime)) return null;
            else return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }
        public static DateTime? UtcValue(DateTime dateTime)
        {
            if (Is.Null(dateTime)) return null;
            else return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        public static DateTime? UtcValue(object obj)
        {
            if (Is.Null(obj)) return null;
            if (obj.GetType() == typeof(SqlDateTime)) return UtcValue((SqlDateTime)obj);
            if (obj.GetType() == typeof(DateTime   )) return UtcValue((DateTime   )obj);
            else return null;
        }

        public static DateTime UtcValue(SqlDateTime dateTime, DateTime defaultValue)
        {
            if (Is.Null(dateTime)) return defaultValue;
            else return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }
        public static DateTime UtcValue(DateTime dateTime, DateTime defaultValue)
        {
            if (Is.Null(dateTime)) return defaultValue;
            else return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        public static DateTime UtcValue(object obj, DateTime defaultValue)
        {
            if (Is.Null(obj)) return defaultValue;
            if (obj.GetType() == typeof(SqlDateTime)) return UtcValue((SqlDateTime)obj, defaultValue);
            if (obj.GetType() == typeof(DateTime   )) return UtcValue((DateTime   )obj, defaultValue);
            else return defaultValue;
        }

    }
}
