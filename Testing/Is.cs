//-------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation, either version 3
// of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//-------------------------------------------------------------------------------------------------
using System;                         // for ArgumentOutOfRangeException, Char, Convert, DateTime, Exception, Int16, MethodAccessException, NotImplementedException, String, Type
using System.Collections;             // for IDictionary
using System.Collections.Generic;     // for Dictionary, List
using System.Data.SqlTypes;           // for SqlBoolean, SqlByte, SqlDateTime, SqlDecimal, SqlDouble, SqlGuid, SqlInt16, SqlInt32, SqlInt64, SqlMoney, SqlSingle, SqlString, SqlXml
using System.Linq;
using System.Reflection;              // for BindingFlags, PropertyInfo, TargetInvocationException, TargetParameterCountException
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Testing  // add to this namespace very sparingly and use only System references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Is -->
    /// <summary>
    ///      The Is class contains
    ///      1) syntactic candy for Assert.That
    ///      2) extremenly common very basic methods related to conditionals 'Is.Null'
    ///      3) recursive 'ok' checks for Assert.That
    ///      Mostly it is for testing, debugging and information oriented equality testing
    /// </summary>
    /// <remarks>
    ///      keep as much here private or internal as possible
    ///      Actually, this is starting to morph into an information oriented equality test
    /// </remarks>
    public class Is
    {
        // ----------------------------------------------------------------------------------------
        //  Syntactic parameter candy         beta code - for debugging
        // ----------------------------------------------------------------------------------------
        public const string a_subset_of              = "Is a Subset Of (deprecated)"; // binary operator // I may be deprecating this one, one object contains another as far as member values, either the same or null in the subset object
        public const string equal_to                 = "Equals"                     ; // binary operator
        public const string existing_file            = "File Exists"                ; // unary operator  // EqualTo, GreaterThan, GTorEq, LessThan, LTorEq, NotEqualTo, OkIfFileDoesNotExists, OkIfFileExists, OkUnlessNameMatches, OkIfNameMatches
        public const string greater_than             = "Is Greater Than"            ; // binary operator
        public const string greater_than_or_equal_to = "Is Greater Than Or Equal To"; // binary operator
        public const string less_than                = "Is Less Than"               ; // binary operator
        public const string less_than_or_equal_to    = "Is Less Than Or Equal To"   ; // binary operator
        public const string missing_file             = "File Not Exists"            ; // unary operator
        public const string not_equal_to             = "Is Not Equal To"            ; // binary operator
        public const string regex_match_to           = "Is a REGEX match To"        ; // binary operator
        public const string the_same_as              = "Is The Same As"             ; // binary operator // functionally the same (same parameter values in the same order)
        public const string the_same_sets_as         = "Is The Same Sets As"        ; // binary operator // the same parameter values - but collections may be in any order - also be called 'the_same_content_as'
        public const string unless_name_matches      = "Unless Name Matches"        ; // binary operator // for pattern matching (I have not implemented this one)


        // ----------------------------------------------------------------------------------------
        //  Ok members
        // ----------------------------------------------------------------------------------------
        private static object startL    ;
        private static object startR    ;
        public  static string SameErrors;
        public  static string SetErrors ;


        // ----------------------------------------------------------------------------------------
        //  Code conditions for debugging and testing
        // ----------------------------------------------------------------------------------------
        public static void Ignored ()          { }
        public static void Tested  ()          { }
        public static void Untested()          { }
        public static void Unused  ()          { }
        public static void UsedBy  (string by) { }


        // ----------------------------------------------------------------------------------------
        /// <!-- AggressiveToString -->
        /// <summary>
        ///      Like ToString(), but if the object type is too complex to have a useful ToString() result,
        ///      then it constructs one based on its properties
        /// </summary>
        /// <param name="ofMe"></param>
        /// <returns></returns>
        private static string AggressiveToString(object ofMe)
        {
            if (ofMe == null)  return "null";  // the rule of three and human pattern matching - is it a line or a curve?
            string output = ofMe.ToString();


            if (WeakToStringMethod(ofMe)) // construct a string from various object members
            {
                // ----------------------------------------------------------------------
                //  'try' variables
                // ----------------------------------------------------------------------
                Type           type     = ofMe.GetType();
                PropertyInfo[] propList = type.GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
                int            idx      = 0;
                object         value    = null;


                try
                {
                    output = "";
                    string delim = "";

                    for (idx = 0; idx < propList.Length; ++idx)
                    {
                        // --------------------------------------------------------------
                        //  Add a property string (if the property is not too complex)
                        // --------------------------------------------------------------
                        if (propList[idx].CanWrite)
                        {
                            value = propList[idx].GetValue(ofMe, null);
                            if (value == null)                 { output += delim + "null";           delim = "_"; }
                            else if (!WeakToStringMethod(value)) { output += delim + value.ToString(); delim = "_"; }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Is.Trash(ex);
                }

            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WeakToStringMethod -->
        /// <summary>
        ///      Determines whether an object type is too complex to have a useful ToString() result
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool WeakToStringMethod(object obj)
        {
            if (obj == null) return false;
            string str = obj.ToString();
            return (str == obj.GetType().FullName || Regex.IsMatch(str, @"\["));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CountCollectionSize -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="maxCount">will not return a number higher than this</param>
        /// <returns></returns>
        private static int CountCollectionSize(object value, PropertyInfo property, int maxCount)
        {
            ParameterInfo[] param = property.GetIndexParameters();
            if (param.Length != 1)
                throw new NotImplementedException("boom - wrong collection size counter index dimensionality: " + param.Length);


            int count = 0;

            if (Regex.IsMatch(value.GetType().FullName, "Dictionary"))
            {
                IDictionary associateOf = (IDictionary)value;
                foreach (object key in associateOf.Keys)
                    count++;
            }
            else
            {
                if (!Regex.IsMatch(param[0].ParameterType.FullName, "^System.Int"))
                    throw new NotImplementedException("kaboom - wrong collection size counter index type: " + param[0].ParameterType.FullName);
                try
                {
                    for (int i = 0; i < maxCount; ++i)
                    {
                        object item = property.GetValue(value, new object[] { i });
                        count++;
                    }
                }
                catch (TargetInvocationException ex) // ignore this exception, we are simply looking for the end of the collection
                {
                    if (ex.InnerException.GetType() != typeof(ArgumentOutOfRangeException)) throw;
                }
            }

            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InRange -->
        /// <summary>
        ///      Determines whether a datetime is within a particular datetime range (inclusive)
        /// </summary>
        /// <param name="low"></param>
        /// <param name="date"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        private static bool InRange(DateTime low, DateTime date, DateTime high)
        {
            bool lowEnough  = (date <= high);
            bool highEnough = (low  <= date);
            return (lowEnough && highEnough);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NearlyEqual -->
        /// <summary>
        ///      26 ms = 1280 days, divided by 256 4 times, rounded up,
        ///      it covers the accuracy vagaries of the DateTime class and the machines it runs on
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="msTolerance">how many milliseconds the datetime can be off by</param>
        /// <returns></returns>
        private static bool NearlyEqual(DateTime left, DateTime right, int msTolerance)
        {
            bool     ok = false;

            if      (msTolerance > 1000)     { ok = InRange(right.AddMilliseconds(-msTolerance), left , right.AddMilliseconds(msTolerance)); }
            else if (left .Millisecond == 0 && right.Millisecond == 0)
                                             { ok = InRange(right.AddMilliseconds(-1)          , left , right.AddMilliseconds(1)          ); }
            else if (left .Millisecond == 0) { ok = InRange(right.AddMilliseconds(-1000)       , left , right.AddMilliseconds(msTolerance)); }
            else if (right.Millisecond == 0) { ok = InRange(left .AddMilliseconds(-1000)       , right, left .AddMilliseconds(msTolerance)); }
            else                             { ok = InRange(right.AddMilliseconds(-msTolerance), left , right.AddMilliseconds(msTolerance)); }

            return ok;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Null -->
        /// <summary>
        ///      Determines whether an object is one or another of the standard null values
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static bool Null(object obj)
        {
            // --------------------------------------------------------------------------
            //  Check for standard null values
            // --------------------------------------------------------------------------
            if (obj == null || obj == DBNull.Value)
                return true;


            string type = obj.GetType().ToString();
            if (Regex.IsMatch(type, "^System.Data.SqlTypes"))
            {
                // ----------------------------------------------------------------------
                //  Investigate the specific Sql types for null value
                // ----------------------------------------------------------------------
                switch (type)
                {
                    case "System.Data.SqlTypes.SqlBoolean" : if (((SqlBoolean )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlByte"    : if (((SqlByte    )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlBytes"   : if (((SqlBytes   )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlChars"   : if (((SqlChars   )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlDateTime": if (((SqlDateTime)obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlDecimal" : if (((SqlDecimal )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlDouble"  : if (((SqlDouble  )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlGuid"    : if (((SqlGuid    )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlInt16"   : if (((SqlInt16   )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlInt32"   : if (((SqlInt32   )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlInt64"   : if (((SqlInt64   )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlMoney"   : if (((SqlMoney   )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlSingle"  : if (((SqlSingle  )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlString"  : if (((SqlString  )obj).IsNull) return true; break;
                    case "System.Data.SqlTypes.SqlXml"     : if (((SqlXml     )obj).IsNull) return true; break;
                }
            }
            else if (Is.NullableValueType(obj))
            {
                // ----------------------------------------------------------------------
                //  Investigate the specific nullable types for null value
                // ----------------------------------------------------------------------
                if (Regex.IsMatch(type, "int"    , RegexOptions.IgnoreCase) && !((int?    )obj).HasValue) return true;
                if (Regex.IsMatch(type, "double" , RegexOptions.IgnoreCase) && !((double? )obj).HasValue) return true;
                if (Regex.IsMatch(type, "bool"   , RegexOptions.IgnoreCase) && !((bool?   )obj).HasValue) return true;
                if (Regex.IsMatch(type, "decimal", RegexOptions.IgnoreCase) && !((decimal?)obj).HasValue) return true;
                if (Regex.IsMatch(type, "long"   , RegexOptions.IgnoreCase) && !((long?   )obj).HasValue) return true;
                if (Regex.IsMatch(type, "float"  , RegexOptions.IgnoreCase) && !((float?  )obj).HasValue) return true;
            }

            return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NullableValueType -->
        /// <summary>
        ///      
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>
        ///      adapted from http://stackoverflow.com/questions/374651/how-to-check-if-an-object-is-nullable
        /// </remarks>
        public static bool NullableValueType<T>(T obj)
        {
            if (obj == null) return false; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return false; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Ok -->
        /// <summary>
        ///      Performs specified binary operation checks
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="operation"></param>
        /// <param name="rhs"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth">recursion depth</param>
        /// <returns></returns>
        /// <remarks>
        ///      Not intended to be used outside the Assert classes,
        ///      Nonetheless I have started using this for information equality
        /// </remarks>
        private static bool Ok(object lhs, string operation, object rhs, int dateTolerance, int depth)
        {
            depth++;
            bool ok = true;


            // --------------------------------------------------------------------------
            //  Resist infinite loops
            // --------------------------------------------------------------------------
            if (lhs != null && rhs != null)
            {
                // Avoid infinite loops:
                if (depth == 0) { startL = lhs; startR = rhs; }
                else if (lhs == startL && rhs == startR)  return true;
                else if (lhs == startL || rhs == startR)  return false;
            }


            // --------------------------------------------------------------------------
            //  Null value comparisons
            // --------------------------------------------------------------------------
            if (lhs == null && rhs == null)
                switch (operation) { case Is.greater_than : case Is.less_than : case Is.not_equal_to : return false; default : return true; }


            // --------------------------------------------------------------------------
            //  DateTime comparisons
            // --------------------------------------------------------------------------
            if (lhs != null && rhs != null && lhs.GetType() == typeof(DateTime) && rhs.GetType() == typeof(DateTime))
            {
                DateTime left  = (DateTime)lhs;
                DateTime right = (DateTime)rhs;

                switch (operation)
                {
                    case Is.equal_to                 : ok =  NearlyEqual(left, right, dateTolerance)                 ; break;
                    case Is.a_subset_of              : ok =  NearlyEqual(left, right, 10000        )                 ; break;
                    case Is.the_same_sets_as         : ok =  NearlyEqual(left, right, 100000       )                 ; break;
                    case Is.the_same_as              : ok =  NearlyEqual(left, right, dateTolerance)                 ; break;
                    case Is.greater_than_or_equal_to : ok = (NearlyEqual(left, right, dateTolerance) || left > right); break;
                    case Is.less_than_or_equal_to    : ok = (NearlyEqual(left, right, dateTolerance) || left < right); break;
                    case Is.greater_than             : ok =                                             left > right ; break;
                    case Is.less_than                : ok =                                             left < right ; break;
                    case Is.not_equal_to             : ok = !NearlyEqual(left, right, dateTolerance)                 ; break;
                    case Is.regex_match_to           : ok = Regex.IsMatch(left.ToString(), right.ToString())         ; break;
                    default                          : throw new NotImplementedException("Error - operation '" + operation + "' has not been implemented for "+lhs.GetType().Name+" in Is.Ok.");
                }
                return ok;
            }


            // --------------------------------------------------------------------------
            //  Throw on problematic operation
            // --------------------------------------------------------------------------
            {
                switch (operation)
                {
                    case Is.less_than     : case Is.less_than_or_equal_to    : case Is.equal_to       : case Is.not_equal_to : 
                    case Is.greater_than  : case Is.greater_than_or_equal_to : case Is.the_same_as    : 
                    case Is.a_subset_of   : case Is.the_same_sets_as         : case Is.regex_match_to :  break;
                    case Is.existing_file : throw new MethodAccessException  ("Error - '" + operation + "' is a unary operation not a binary operation.");
                    case Is.missing_file  : throw new MethodAccessException  ("Error - '" + operation + "' is a unary operation not a binary operation.");
                    default               : throw new NotImplementedException("Error - operation '" + operation + "' has not been implemented in Is.Ok");
                }
            }
             

            // --------------------------------------------------------------------------
            //  Normal value comparisons
            // --------------------------------------------------------------------------
            if (lhs != null && rhs != null)
            {
                switch (operation)
                {
                    case Is.regex_match_to           : ok = Regex.IsMatch(lhs.ToString(), rhs.ToString()); break;
                    case Is.greater_than_or_equal_to : ok =  (RealValue(lhs, 0.0) >= RealValue(rhs, 1.0)); break;
                    case Is.less_than_or_equal_to    : ok =  (RealValue(lhs, 1.0) <= RealValue(rhs, 0.0)); break;
                    case Is.greater_than             : ok =  (RealValue(lhs, 0.0) >  RealValue(rhs, 1.0)); break;
                    case Is.less_than                : ok =  (RealValue(lhs, 1.0) <  RealValue(rhs, 0.0)); break;
                    case Is.not_equal_to             : ok = !(lhs == rhs || lhs.Equals(rhs) );             break;
                    case Is.equal_to                 : ok =  (lhs == rhs || lhs.Equals(rhs) );             break;
                    case Is.the_same_as              : ok =  (lhs == rhs || lhs.Equals(rhs) || (lhs != rhs) &&  Is.TheSame (lhs,rhs,dateTolerance,depth) ); break;
                    case Is.the_same_sets_as         : ok =  (lhs == rhs || lhs.Equals(rhs) || (lhs != rhs) &&  Is.SameSets(lhs,rhs,dateTolerance,depth) ); break;
                    case Is.a_subset_of              : ok =  (lhs == rhs || lhs.Equals(rhs) || (lhs != rhs) && (Is.TheSame (lhs,rhs,dateTolerance,depth) || Is.Subset(lhs,rhs,100000,depth))); break;
                    //      ------------------------          ---------- || --------------- || ---------------------------------------------------------
                    //      operation                         simple eq. || object equality || if not simple equality, check for special 'equality'
                }
                return ok;
            }


            // --------------------------------------------------------------------------
            //  One value is null and the other is not
            // --------------------------------------------------------------------------
            if (lhs == null || rhs == null)
            {
                switch (operation)
                {
                    case Is.not_equal_to : ok = true;          break;
                    case Is.a_subset_of  : ok = (lhs == null); break;
                    default              : ok = false;         break;
                }
                return ok;
            }

            return ok; // good enough for information work
        } /// <summary>Returns whether the specified binary check is true, WARNING: not entirely reliable</summary><param name="operation">the specified binary check, for example Is.equal_to</param><remarks>WARNING: not entirely reliable</remarks>
        public static bool Ok(object lhs, string operation, object rhs) { return Ok(lhs, operation, rhs, 1200, -1); }

        // ----------------------------------------------------------------------------------------
        /// <!-- RealValue -->
        /// <summary>
        ///      Does its best to return a double value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static double RealValue(object obj, double defaultValue)
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
            //  Aggressive Parsing - what does this do?
            // --------------------------------------------------------------------------
            str = Regex.Replace(str, "[^-0-9.]", ""   );
            str = Regex.Replace(str, "-"       , "Q"  );
            str = Regex.Replace(str, "^Q"      , "-"  );
            str = Regex.Replace(str, "Q"       , ""   );
            str = Regex.Replace(str, "[.]"     , "Q"  );
            str = Regex.Replace(str, "^([^.])Q", "$1.");
            str = Regex.Replace(str, "Q"       , ""   );
            if (double.TryParse(str, out value)) return value;

            return defaultValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Record... -->
        /// <summary>
        ///      Records and plays back a bunch of strings, used for debugging
        /// </summary>
        /// <param name="path"></param>
        public static void Recorded(string path)
        {
            if (_record == null) _record = new List<string>();
            _record.Add(DateTime.Now.ToString() + " " + path);
        }
        public static string Records { get
        {
            if (_record == null) _record = new List<string>();
            string str = "";
            string delim = "";
            for (int i = 0; i < Is._record.Count; ++i)
                { str += delim + _record[i]; delim = "\r\n"; }
            return str;
        } }
        public static void NowClearingRecords() { _record = null; }
        private static List<string> _record;

        // ----------------------------------------------------------------------------------------
        /// <!-- Subset -->
        /// <summary>
        ///      Compares each parameter of an object to determine if the lhs is a subset of the rhs
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <remarks>
        ///      assumes ok = true unless a mismatch gets found'
        ///      I have not kept this method up with the other two, so it is quite unreliable
        /// </remarks>
        private static bool Subset(object lhs, object rhs, int dateTolerance, int depth)
        {
            if (depth > 4) return true; // a cheap way to avoid infinite recursion


            // --------------------------------------------------------------------------
            //  Important variables
            // --------------------------------------------------------------------------
            string         ac         = "Start"; // ac = active comments
            bool           ok         = true;
            Type           leftType   = null;
            Type           rightType  = null;
            PropertyInfo[] propListL  = null;
            PropertyInfo[] propListR  = null;
            int            idx        = 0;
            PropertyInfo   propertyL  = null;
            PropertyInfo   propertyR  = null;
            object         leftValue  = null;
            object         rightValue = null;


            try
            {
                // ----------------------------------------------------------------------
                //  Property lists
                // ----------------------------------------------------------------------
                leftType  = lhs.GetType();
                rightType = rhs.GetType();
                propListL = leftType .GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
                propListR = rightType.GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);


                if (leftType != rightType)                                               { ok = NotTheSameSetBecause("types don't match"); return false; }
                if (propListL.Length != propListR.Length)                                { ok = NotTheSameSetBecause("property counts don't match"); return false; }


                for (idx = 0; idx < propListL.Length; ++idx)
                {
                    // ------------------------------------------------------------------
                    ac = "Compare a property ";
                    // ------------------------------------------------------------------
                    propertyL = propListL[idx];
                    propertyR = propListR[idx];
                    if (propertyL.Name != propertyR.Name ||
                        propertyL.PropertyType != propertyR.PropertyType)                { ok = NotTheSameSetBecause("property names or types don't match"); return false; }


                    if (propertyL.CanWrite && propertyR.CanWrite)
                    {
                        leftValue  = propertyL.GetValue(lhs, null);
                        rightValue = propertyR.GetValue(rhs, null);
                        if (leftValue != null)
                        {
                            bool itemOk = true;
                            if (propertyL.PropertyType == typeof(DateTime))
                            {
                                itemOk = NearlyEqual((DateTime)leftValue, (DateTime)rightValue, dateTolerance);
                            }
                            else
                            {
                                itemOk = Ok(leftValue, Is.a_subset_of, rightValue, dateTolerance, depth);
                            }
                            if (!itemOk)
                                return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Is.Trash(ex);
                throw;
            }
            finally
            {
                if (!ok)
                    Pause();
                Is.Trash(ac);
            }

            return true;
        }


        // ----------------------------------------------------------------------------------------
        //  ValueDictionary
        // ----------------------------------------------------------------------------------------
        private static Dictionary<string, PropertyInfo> ValueDictionary   (PropertyInfo[] list)                     {  Dictionary<string,PropertyInfo> dv = new Dictionary<string, PropertyInfo>();     for (int i = 0; i < list.Length; ++i) { PropertyInfo value = list[i];                                    if (value != null) { string ky = ValueKey(value); if (!dv.ContainsKey(ky)) dv.Add(ky, value); else Pause(); } } return dv; }
        private static Dictionary<string, object>       ValueDictionary   (IList          list)                     {  Dictionary<string,object>       dv = new Dictionary<string, object>(list.Count); for (int i = 0; i < list.Count ; ++i) { object       value = list[i];                                    if (value != null) { string ky = ValueKey(value); if (!dv.ContainsKey(ky)) dv.Add(ky, value);               } } return dv; }
        private static Dictionary<string, object>       ValueDictionary<T>(T obj, PropertyInfo property, int count) {  Dictionary<string,object>       dv = new Dictionary<string, object>(count);      for (int i = 0; i < count      ; ++i) { object       value = property.GetValue(obj, new object[] { i }); if (value != null) { string ky = ValueKey(value); if (!dv.ContainsKey(ky)) dv.Add(ky, value);               } } return dv; }


        // ----------------------------------------------------------------------------------------
        /// <!-- Mismatch -->
        /// <summary>
        ///      Formats what the mismatch is
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="property"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        private static string Mismatch(object leftValue, PropertyInfo property, object rightValue)
        {
            string str = "";
            string strL = "null";  if (leftValue  != null) strL = leftValue .ToString();
            string strR = "null";  if (rightValue != null) strR = rightValue.ToString();
            if (strL == property.PropertyType.FullName) str = "";
            str = property.Name + ": " + strL + " != " + strR;
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameArray -->
        /// <summary>
        ///      Determines if the values of two arrays are the same and in the same order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs">int[] or something</param>
        /// <param name="rhs">int[] or something</param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static bool SameArray<T>(T lhs, T rhs, int dateTolerance, int depth)
        {
            bool ok = true;

            // --------------------------------------------------------------------------
            //  Compare items when left and right sides are arrays
            // --------------------------------------------------------------------------
            IList<object> listL = (IList<object>)lhs;
            IList<object> listR = (IList<object>)rhs;
            if (listL.Count != listR.Count)                                              { ok = NotTheSameBecause   ("array lengths are not the same"); return false; }
            for (int i = 0; i < listL.Count; ++i)
            {
                object left  = listL[i];
                object right = listR[i];
                if (!Ok(left, Is.the_same_as     , right, dateTolerance, depth))         { ok = NotTheSameBecause   ("array item values not equal");    return false; }
            }

            return ok;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameArraySet -->
        /// <summary>
        ///      Determines if the values of two arrays are the same regardless of their order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static bool SameArraySet<T>(T lhs, T rhs, int dateTolerance, int depth)
        {
            bool  ok = true;
            IList leftList  = (IList)lhs;
            IList rightList = (IList)rhs;
            if (leftList.Count != rightList.Count)                                       { ok = NotTheSameSetBecause("array lengths are not the same"); return false; }


            // --------------------------------------------------------------------------
            //  Put the lists into value based dictionaries
            // --------------------------------------------------------------------------
            Dictionary<string,object> left  = ValueDictionary(leftList);
            Dictionary<string,object> right = ValueDictionary(rightList);
            if (left.Count != right.Count)                                               { NotTheSameSetBecause("not the same number of distinct values"); return false; }


            // --------------------------------------------------------------------------
            //  Check for equal dictionaries
            // --------------------------------------------------------------------------
            foreach (string key in left.Keys)
            {
                if (!right.ContainsKey(key))                                             { NotTheSameSetBecause("missing right key"); return false; }
                if (!Ok(left[key], Is.the_same_sets_as, right[key], dateTolerance, depth)) { NotTheSameSetBecause("values don't match"); return false; }
            }

            return ok;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameDictionary -->
        /// <summary>
        ///      Determines whether two dictionaries have the same key/content pairs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static bool SameDictionary<T>(T lhs, T rhs, int dateTolerance, int depth)
        {
            IDictionary left  = (IDictionary)lhs;
            IDictionary right = (IDictionary)rhs;
            foreach (object key in left.Keys)
                if (!Ok(left[key], Is.the_same_as, right[key], dateTolerance, depth))    { NotTheSameSetBecause("values don't match"); return false; }
            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameList -->
        /// <summary>
        ///      Determines whether two lists are the same including order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="propertyL"></param>
        /// <param name="propertyR"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static bool SameList<T>(T lhs, T rhs, PropertyInfo propertyL, PropertyInfo propertyR, int dateTolerance, int depth)
        {
            // --------------------------------------------------------------------------
            //  Generic collection case
            // --------------------------------------------------------------------------
            object leftValue  = null;
            object rightValue = null;
            int countL = CountCollectionSize(lhs, propertyL, 100000); // we can do 100000 because this code is only for autotest
            int countR = CountCollectionSize(rhs, propertyR, 100000);
            if (countL != countR)                                                        { NotTheSameBecause(countL.ToString() + " != " + countR); return false; }


            // --------------------------------------------------------------------------
            //  Compare the items in the left and right sides of the collection parameter
            // --------------------------------------------------------------------------
            for (int i = 0; i < countL; ++i)
            {
                leftValue  = propertyL.GetValue(lhs, new object[] { i });
                rightValue = propertyR.GetValue(rhs, new object[] { i });
                if (!Ok(leftValue, Is.the_same_as, rightValue, dateTolerance, depth)) { NotTheSameBecause(propertyL.Name); return false; }
            }

            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameList -->
        /// <summary>
        ///      Determines whether two lists of strings have the same contents in the same order
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool SameList<T>(List<T> left, List<T> right, int dateTolerance, int depth)
        {
            if (left.Count != right.Count)                                               { NotTheSameBecause(left.Count.ToString() + " != " + right.Count); return false; }
            for (int idx = 0; idx < left.Count; ++idx)
                if (!Ok(left[idx],Is.the_same_as,right[idx], dateTolerance, depth))      { NotTheSameBecause("values don't match"); return false; }
            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameListSet -->
        /// <summary>
        ///      Determines whether two lists are the same regardless of order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhList"></param>
        /// <param name="rhList"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static bool SameListSet<T>(List<T> lhList, List<T> rhList, int dateTolerance, int depth)
        {
            // --------------------------------------------------------------------------
            //  Put the lists into value based dictionaries
            // --------------------------------------------------------------------------
            if (lhList.Count != rhList.Count)                                            { NotTheSameSetBecause("lists not the same length"); return false; }
            Dictionary<string,object> left  = left  = ValueDictionary(lhList); // left  = new Dictionary<string, object>(lhList.Count); for (idx = 0; idx < lhList.Count; ++idx) { object value = lhList[idx]; if (value != null) left .Add(ValueKey(value), value); }
            Dictionary<string,object> right = right = ValueDictionary(rhList); // right = new Dictionary<string, object>(rhList.Count); for (idx = 0; idx < rhList.Count; ++idx) { object value = rhList[idx]; if (value != null) right.Add(ValueKey(value), value); }
            if (left.Count != right.Count)                                               { NotTheSameSetBecause("lists have different number of distinct values"); return false; }


            // --------------------------------------------------------------------------
            //  Check for equal dictionaries
            // --------------------------------------------------------------------------
            foreach (string key in left.Keys)
            {
                if (!right.ContainsKey(key))                                               { NotTheSameSetBecause("right key missing"); return false; }
                if (!Ok(left[key], Is.the_same_sets_as, right[key], dateTolerance, depth)) { NotTheSameSetBecause("values don't match"); return false; }
            }

            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameListSet -->
        /// <summary>
        ///      Determines whether two lists are the same regardless of order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="propertyL"></param>
        /// <param name="propertyR"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same datemilliseconds</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static bool SameListSet<T>(T lhs, T rhs, PropertyInfo propertyL, PropertyInfo propertyR, int dateTolerance, int depth)
        {
            // --------------------------------------------------------------------------
            //  Generic collection case
            // --------------------------------------------------------------------------
            Dictionary<string,object> leftValue  = null;
            Dictionary<string,object> rightValue = null;
            object left  = null;
            object right = null;
            string key   = "";


            try
            {
                int countL = CountCollectionSize(lhs, propertyL, 100000); // we can limit to 100000 because this code is only for autotest
                int countR = CountCollectionSize(rhs, propertyR, 100000);
                if (countL != countR)                                                    { NotTheSameSetBecause("counts don't match"); return false; }
                rightValue = new Dictionary<string, object>(countR);


                // ----------------------------------------------------------------------        
                //  Convert the lists into a dictionaries
                // ----------------------------------------------------------------------
                leftValue  = ValueDictionary(lhs, propertyL, countL);
                rightValue = ValueDictionary(rhs, propertyR, countR);
                foreach (string str in rightValue.Keys) if (!leftValue.ContainsKey(str)) { NotTheSameSetBecause("left key missing"); return false; }


                // ----------------------------------------------------------------------
                //  Compare the values on the left and right sides of the collection parameter
                // ----------------------------------------------------------------------
                foreach (string eachKey in leftValue.Keys)
                {
                    key  = eachKey;
                    left = leftValue[key];
                    if (!rightValue.ContainsKey(key))                                    { NotTheSameSetBecause("right key missing"); return false; }
                    else right = rightValue[key];
                    if (!Ok(left, Is.the_same_as, right, dateTolerance, depth))          { NotTheSameSetBecause("values don't match"); return false; }
                }
            }
            catch (Exception ex)
            {
                Is.Trash(ex);
            }

            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameSet -->
        /// <summary>
        ///      Determines whether two lists of strings have the same contents regardless of order
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool SameSet(List<string> left, List<string> right)
        {
            if (left.Count != right.Count)                                               { NotTheSameSetBecause("counts don't match"); return false; }

            Dictionary<String,int> total = new Dictionary<String,int>();
            for (int i = 0; i < left.Count; ++i)
            {
                if (total.ContainsKey(left [i])) total[left [i]]++; else total.Add(left [i], 1);
                if (total.ContainsKey(right[i])) total[right[i]]++; else total.Add(right[i], 1);
            }
            foreach (int count in total.Values) { if (count % 2 == 1)                    { NotTheSameSetBecause("mismatch"); return false; } }

            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SameSets -->
        /// <summary>
        ///      Compares each parameter of an object allowing collections to be in different orders
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="depth">recursion depth</param>
        /// <returns></returns>
        /// <remarks>looks for problems and returns true if none found</remarks>
        private static bool SameSets<T>(T lhs, T rhs, int dateTolerance, int depth)
        {
            if (depth > 7) return true; // a cheap way to avoid infinite recursion


            // --------------------------------------------------------------------------
            //  Important variables
            // --------------------------------------------------------------------------
            PropertyInfo[]                  leftArray = null, rightArray = null;
            Dictionary<string,PropertyInfo> leftList  = null, rightList  = null;
            PropertyInfo                    leftProp  = null, rightProp  = null;
            Type                            leftType  = null, rightType  = null;
            object                          leftValue = null, rightValue = null;


            string ac  = "Start"; // ac = active comments
            string key = "";
            bool   ok  = true;
            leftList   = new Dictionary<string, PropertyInfo>();
            rightList  = new Dictionary<string, PropertyInfo>();


            try
            {
                // ----------------------------------------------------------------------
                ac = "types";
                // ----------------------------------------------------------------------
                leftType  = lhs.GetType();
                rightType = rhs.GetType();
                if (leftType != rightType)                                                                   { ok = NotTheSameSetBecause(leftType.Name + " != " + rightType.Name); return false; }


                // ----------------------------------------------------------------------
                ac = "Property lists"; // property lists are not guaranteed to be in the same order
                // ----------------------------------------------------------------------
                leftArray  = leftType .GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
                rightArray = rightType.GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
                if (leftArray.Length != rightArray.Length)                                                   { ok = NotTheSameSetBecause(leftType.Name + " property list length"); return false; }


                // ----------------------------------------------------------------------
                ac = "Check for lhs-rhs arrays the same";
                // ----------------------------------------------------------------------
                if (leftType.FullName == "System.Byte[]")
                {
                    if (!((Byte[])(object)lhs).SequenceEqual((Byte[])(object)rhs))                           { ok = NotTheSameSetBecause("byte arrays not equal");                 return false; }
                }
                else if (Regex.IsMatch(leftType.FullName, @"\[\]$"))
                {
                    if (!SameArraySet(lhs, rhs, dateTolerance, depth))                                       { ok = NotTheSameSetBecause("arrays are not the same");               return false; }
                }


                // ----------------------------------------------------------------------
                ac = "Simple values compare easily";
                // ----------------------------------------------------------------------
                //if (leftType == typeof(string) && rightType == typeof(string))
                //{
                //    if (lhs.ToString() != rhs.ToString())                                                    { ok = NotTheSameSetBecause("string values don't match");             return false; }
                //} else
                if (leftArray.Length == 0 && rightArray.Length == 0 ||
                    leftType == typeof(string) && rightType == typeof(string))
                {
                    if (leftType == null ^ rightType == null)                                                { ok = NotTheSameSetBecause("simple values null match problem");      return false; }
                    if (!lhs.Equals(rhs))                                                                    { ok = NotTheSameSetBecause("simple values not equal: '" + lhs.ToString() + "' vs '" + rhs.ToString() + "'"); return false; }
                    if (!Ok(lhs, Is.equal_to, rhs, dateTolerance, depth))                                    { ok = NotTheSameSetBecause("simple values not equal: '" + lhs.ToString() + "' vs '" + rhs.ToString() + "'"); return false; }
                }
                else
                {
                    // ------------------------------------------------------------------
                    ac = "Property dictionaries for complex values";
                    // ------------------------------------------------------------------
                    leftList  = ValueDictionary(leftArray);
                    rightList = ValueDictionary(rightArray);


                    foreach (string eachKey in leftList.Keys)
                    {
                        key = eachKey;


                        if (key == "Char Chars;")
                            Pause();


                        // --------------------------------------------------------------
                        ac = "Compare a property";
                        // --------------------------------------------------------------    
                        if (!rightList.ContainsKey(key))                                                     { ok = NotTheSameSetBecause("rhs mising " + key); return false; }
                        leftProp  = leftList [key];
                        rightProp = rightList[key];
                        if (leftProp.Name != rightProp.Name ||
                            leftProp.PropertyType != rightProp.PropertyType)                                { ok = NotTheSameSetBecause(leftProp.Name + " != " + rightProp.Name); return false; }


                        if (leftProp.CanWrite && rightProp.CanWrite)
                        {
                            //  perparing for a switch
                            string dtName = ""; if (leftProp.DeclaringType != null) dtName = leftProp.DeclaringType.Name;
                            string prName = leftProp.Name;
                            string ptName = leftProp.PropertyType.Name;
                            string name = prName;
                            if (prName == "Item") name = ptName + "." + prName;
                            if (prName == "Item" && dtName == "Dictionary`2") name = dtName + "." + prName;


                            if (prName == "Item")
                            {
                                // ------------------------------------------------------
                                ac = "Various collections";
                                // ------------------------------------------------------
                                if      (dtName == "Dictionary`2"                       ) { if (!SameDictionary(lhs                      , rhs                      , dateTolerance, depth)) { ok = NotTheSameSetBecause("Dictionary`2"    ); return false; } }
                                else if (ptName == "String"                             ) { if (!SameListSet   ((List<String>)(object)lhs, (List<String>)(object)rhs, dateTolerance, depth)) { ok = NotTheSameSetBecause("String"          ); return false; } }
                                else if (ptName == "Char"                               ) { if (!SameListSet   ((List<Char>  )(object)lhs, (List<Char>  )(object)rhs, dateTolerance, depth)) { ok = NotTheSameSetBecause("Char"            ); return false; } }
                                else                                                      { if (!SameListSet   (lhs                      , rhs, leftProp, rightProp , dateTolerance, depth)) { ok = NotTheSameSetBecause("collection"      ); return false; } }
                            }
                                else if (prName == "System.Collections.IDictionary.Item") { if (!SameDictionary(lhs                      , rhs                      , dateTolerance, depth)) { ok = NotTheSameSetBecause("IDictionary.Item"); return false; } }
                                else if (prName == "System.Collections.IList.Item"      ) { if (!SameListSet   (lhs                      , rhs, leftProp, rightProp , dateTolerance, depth)) { ok = NotTheSameSetBecause("IList.Item"      ); return false; } }
                            else
                            {
                                // ------------------------------------------------------
                                ac = "Normal case";
                                // ------------------------------------------------------
                                leftValue  = leftProp.GetValue(lhs, null);
                                rightValue = rightProp.GetValue(rhs, null);
                                if (!Ok(leftValue, Is.the_same_sets_as, rightValue, dateTolerance, depth))   { ok = NotTheSameSetBecause(Mismatch(leftValue, leftProp, rightValue)); return false; }
                            }
                        }
                        else if (leftProp.CanWrite || rightProp.CanWrite)                                   { ok = NotTheSameSetBecause(Mismatch(leftValue, leftProp, rightValue)); return false; }
                        else
                        {
                            try
                            {
                                // ------------------------------------------------------
                                ac = "Non-writable case";
                                // ------------------------------------------------------
                                leftValue  = leftProp.GetValue(lhs, null);
                                rightValue = rightProp.GetValue(rhs, null);
                                if (leftValue == null || rightValue == null)
                                     { if (!(leftValue == null && rightValue == null))                       { ok = NotTheSameSetBecause(Mismatch(leftValue, leftProp, rightValue)); return false; } }
                                else { if (leftValue.ToString() != rightValue.ToString())                    { ok = NotTheSameSetBecause(Mismatch(leftValue, leftProp, rightValue)); return false; } }
                            }
                            catch (TargetInvocationException) { }
                            catch (TargetParameterCountException) { }
                            catch (Exception ex) // ignore exceptions in nonwritable parameters because they may have to do with internal property calculations which the object may not be ready for
                            {
                                Is.Trash(ex);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { ok = false; Is.Trash(ex); throw; } finally { Is.Trash(ac); }

            return ok;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TheSame -->
        /// <summary>
        ///      Compares each parameter, insisting that ordered collections be in the same order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="dateTolerance">in milliseconds - dates different up to this amount will be considered the same date</param>
        /// <param name="depth">recursion depth</param>
        /// <returns></returns>
        /// <remarks>looks for problems and returns true if none found</remarks>
        private static bool TheSame<T>(T lhs, T rhs, int dateTolerance, int depth)
        {
            if (depth > 4) return true; // a cheap way to avoid infinite recursion


            // --------------------------------------------------------------------------
            //  Important variables
            // --------------------------------------------------------------------------
            PropertyInfo[]                  leftArray = null, rightArray = null;
            Dictionary<string,PropertyInfo> leftList  = null, rightList  = null;
            PropertyInfo                    leftProp  = null, rightProp  = null;
            Type                            leftType  = null, rightType  = null;
            object                          leftValue = null, rightValue = null;


            string ac  = "Start"; // ac = active comments
            bool   ok  = true;
            string key = "";
            leftList   = new Dictionary<string, PropertyInfo>();
            rightList  = new Dictionary<string, PropertyInfo>();


            try
            {
                // ----------------------------------------------------------------------
                ac = "types";
                // ----------------------------------------------------------------------
                leftType  = lhs.GetType();
                rightType = rhs.GetType();
                if (leftType != rightType)                                                                   { ok = NotTheSameBecause   (leftType.Name + " != " + rightType.Name); return false; }


                // ----------------------------------------------------------------------
                ac = "Property lists"; // property lists are not guaranteed to be in the same order
                // ----------------------------------------------------------------------
                leftArray  = leftType .GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
                rightArray = rightType.GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
                if (leftArray.Length != rightArray.Length)                                                   { ok = NotTheSameBecause   (leftType.Name + " property list length"); return false; }


                // ----------------------------------------------------------------------
                ac = "Check for lhs-rhs arrays the same";
                // ----------------------------------------------------------------------
                if (leftType.FullName == "System.Byte[]")
                {
                    if (!((Byte[])(object)lhs).SequenceEqual((Byte[])(object)rhs))                           { ok = NotTheSameBecause   ("byte arrays not equal");                 return false; }
                }
                else if (Regex.IsMatch(leftType.FullName, @"\[\]$"))
                {
                    if (!SameArray   (lhs, rhs, dateTolerance, depth))                                       { ok = NotTheSameBecause   ("arrays are not the same");               return false; }
                }


                // ----------------------------------------------------------------------
                ac = "Simple values compare easily";
                // ----------------------------------------------------------------------
                //if (leftType == typeof(string) && rightType == typeof(string))
                //{
                //    if (lhs.ToString() != rhs.ToString())                                                    { ok = NotTheSameSetBecause("string values don't match");             return false; }
                //} else
                if (leftArray.Length == 0 && rightArray.Length == 0 ||
                    leftType == typeof(string) && rightType == typeof(string))
                {
                    if (leftType == null ^ rightType == null)                                                { ok = NotTheSameBecause   ("simple values null match problem");      return false; }
                    if (!lhs.Equals(rhs))                                                                    { ok = NotTheSameBecause   ("simple values not equal: '" + lhs.ToString() + "' vs '" + rhs.ToString() + "'"); return false; }
                    if (!Ok(lhs, Is.equal_to, rhs, dateTolerance, depth))                                    { ok = NotTheSameBecause   ("simple values not equal: '" + lhs.ToString() + "' vs '" + rhs.ToString() + "'"); return false; }
                }
                else
                {
                    // ----------------------------------------------------------------------
                    ac = "Property dictionaries for complex values";
                    // ----------------------------------------------------------------------
                    leftList  = ValueDictionary(leftArray);
                    rightList = ValueDictionary(rightArray);


                    foreach (string eachKey in leftList.Keys)
                    {
                        key = eachKey;


                        if (key == "Char Chars;")
                            Pause();


                        // ------------------------------------------------------------------
                        ac = "Compare a property";
                        // ------------------------------------------------------------------
                        if (!rightList.ContainsKey(key))                                                     { ok = NotTheSameBecause   ("rhs mising " + key); return false; }
                        leftProp  = leftList [key];
                        rightProp = rightList[key];
                        if (leftProp.Name != rightProp.Name ||
                            leftProp.PropertyType != rightProp.PropertyType)                                { ok = NotTheSameBecause   (leftProp.Name + " != " + rightProp.Name); return false; }


                        if (leftProp.CanWrite && rightProp.CanWrite)
                        {
                            //  perparing for a switch
                            string dtName = ""; if (leftProp.DeclaringType != null) dtName = leftProp.DeclaringType.Name;
                            string prName = leftProp.Name;
                            string ptName = leftProp.PropertyType.Name;
                            string name = prName;
                            if (prName == "Item") name = ptName + "." + prName;
                            if (prName == "Item" && dtName == "Dictionary`2") name = dtName + "." + prName;


                            if (prName == "Item")
                            {
                                // ----------------------------------------------------------
                                ac = "Various collections";
                                // ----------------------------------------------------------
                                if      (dtName == "Dictionary`2"                       ) { if (!SameDictionary(lhs                      , rhs                      , dateTolerance, depth)) { ok = NotTheSameBecause   ("Dictionary`2"    ); return false; } }
                                else if (ptName == "String"                             ) { if (!SameList      ((List<String>)(object)lhs, (List<String>)(object)rhs, dateTolerance, depth)) { ok = NotTheSameBecause   ("String"          ); return false; } }
                                else if (ptName == "Char"                               ) { if (!SameList      ((List<Char>  )(object)lhs, (List<Char>  )(object)rhs, dateTolerance, depth)) { ok = NotTheSameBecause   ("Char"            ); return false; } }
                                else                                                      { if (!SameList      (lhs                      , rhs, leftProp, rightProp , dateTolerance, depth)) { ok = NotTheSameBecause   ("collection"      ); return false; } }
                            }                                                                                                                                                                                           
                                else if (prName == "System.Collections.IDictionary.Item") { if (!SameDictionary(lhs                      , rhs                      , dateTolerance, depth)) { ok = NotTheSameBecause   ("IDictionary.Item"); return false; } }
                                else if (prName == "System.Collections.IList.Item"      ) { if (!SameList      (lhs                      , rhs, leftProp, rightProp , dateTolerance, depth)) { ok = NotTheSameBecause   ("IList.Item"      ); return false; } }
                            else
                            {
                                // ----------------------------------------------------------
                                ac = "Normal case";
                                // ----------------------------------------------------------
                                leftValue  = leftProp.GetValue(lhs, null);
                                rightValue = rightProp.GetValue(rhs, null);
                                if (!Ok(leftValue, Is.the_same_as     , rightValue, dateTolerance, depth))   { ok = NotTheSameBecause   (Mismatch(leftValue, leftProp, rightValue)); return false; }
                            }
                        }
                        else if (leftProp.CanWrite || rightProp.CanWrite)                                   { ok = NotTheSameBecause   (Mismatch(leftValue, leftProp, rightValue)); return false; }
                        else
                        {
                            try
                            {
                                // ----------------------------------------------------------
                                ac = "Non-writable case";
                                // ----------------------------------------------------------
                                leftValue  = leftProp.GetValue(lhs, null);
                                rightValue = rightProp.GetValue(rhs, null);
                                if (leftValue == null || rightValue == null)
                                     { if (!(leftValue == null && rightValue == null))                       { ok = NotTheSameBecause   (Mismatch(leftValue, leftProp, rightValue)); return false; } }
                                else { if (leftValue.ToString() != rightValue.ToString())                    { ok = NotTheSameBecause   (Mismatch(leftValue, leftProp, rightValue)); return false; } }
                            }
                            catch (TargetInvocationException) { }
                            catch (TargetParameterCountException) { }
                            catch (Exception ex) // ignore exceptions in nonwritable parameters because they may have to do with internal property calculations which the object may not be ready for
                            {
                                Is.Trash(ex);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                Is.Trash(ex);
                throw;
            }
            finally
            {
                Is.Trash(ac);
            }

            return ok;
        }

        // ----------------------------------------------------------------------------------------
        //  Comparison alert methods for debugging
        // ----------------------------------------------------------------------------------------
        private static bool NotTheSameSetBecause(string err)
        {
            if (!string.IsNullOrEmpty(err.Trim()))
            {
                if (string.IsNullOrEmpty(SetErrors.Trim()))
                     SetErrors = err;
                else SetErrors = err + " " + SetErrors;
            }
            return false;
        }
        private static bool NotTheSameBecause(string err)
        {
            if (!string.IsNullOrEmpty(err.Trim()))
            {
                if (string.IsNullOrEmpty(SameErrors.Trim()))
                     SameErrors = err;
                else SameErrors = err + ", " + SameErrors;
            }
            return false;
        }
        private static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Valuekey -->
        /// <summary>
        ///      Useful for converting lists to dictionaries for comparison
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string ValueKey(PropertyInfo property)
        {
            return property.PropertyType.Name + " " + property.Name + ";";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Valuekey -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ValueKey(object obj)
        {
            if (obj == null)  return "null";
            if (obj.ToString() == obj.GetType().FullName)
            {
                Type                            fromValue = obj.GetType();
                PropertyInfo[]                  rawList   = fromValue.GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
                Dictionary<string,PropertyInfo> propHash  = ValueDictionary(rawList);
                List<PropertyInfo>              propList  = propHash.OrderBy(x => x.Key).Select(x => x.Value).ToList();


                string key = "";
                string delim = "";
                for (int i = 0; i < propList.Count; ++i)
                {
                    PropertyInfo property = propList[i];
                    string name = property.Name;
                    if (property.CanWrite && Regex.IsMatch(name, "(name|label|id)", RegexOptions.IgnoreCase))
                    {
                        object value = property.GetValue(obj, null);
                        if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
                        {
                            key += delim + value.ToString();
                            delim = "_";
                        }
                        else
                            Pause();
                    }
                }


                if (string.IsNullOrEmpty(key))
                     return obj.GetHashCode().ToString();
                else return key;
            }
            else
            {
                return obj.ToString();
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Trash -->
        /// <summary>Bit buckets for things you only want to view in debug mode</summary>
        public static void Trash(Exception ex ) { ex  = null; }
        public static void Trash(string    str) { str = null; }
        public static void Trash(bool      ok ) {             }
    }
}
