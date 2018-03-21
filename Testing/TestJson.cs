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
using System;                         // for 
using System.Collections;             // for 
using System.Collections.Generic;     // for 
using System.Globalization;           // for 
using System.Linq;                    // for 
using System.Reflection;              // for 
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Testing  // add to this namespace very sparingly and use only System references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- TestJson -->
    /// <summary>
    ///      WARNING: this is not production quality code!
    ///      The TestJson class is a simple little Json serializer I built to do some testing,
    ///      the main benefit is that I get to control the exceptions
    /// </summary>
    /// <remarks>
    ///      This code is not production quality!
    ///      Only use it for testing or debugging!
    ///      
    ///      You should use a real Json serializer if you have anything more serious to do
    /// </remarks>
    public class TestJson
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public static int Indent { get { return _indent; } set { _indent = Math.Max(0, value); } } // defaults to 4
        private static int _indent = 4;
        private static int greatestDepth = 0;   // records greatest recursion depth of the most recent tokenization


        // ----------------------------------------------------------------------------------------
        //  Methods to use Lists as stacks
        // ----------------------------------------------------------------------------------------
        private static T    Bottom<T>(List<T> stack        ) { return stack[0];                   }
        private static T    Dig   <T>(List<T> stack, int i ) { return stack[stack.Count-1-i];     }
        private static T    Pop   <T>(List<T> stack        ) { T obj = stack.Last(); stack.RemoveAt(stack.Count - 1); return obj; }
        private static void PopIf <T>(List<T> stack, T item) { if (stack.Count > 0 && Top(stack).ToString() == item.ToString()) Pop(stack); }
        private static void Push  <T>(List<T> stack, T item) { stack.Add(item);                   }
        private static T    Second<T>(List<T> stack        ) { return stack[stack.Count-2];       }
        private static T    Top   <T>(List<T> stack        ) { return stack.Last();               }


        // ----------------------------------------------------------------------------------------
        /// <!-- StackPattern -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delim"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static bool StackPattern(List<string> delim, string pattern)
        {
            char[] cha = pattern.ToCharArray();
            for (int i = 0; i < cha.Length; ++i)
            {
                if (!((string)Dig(delim, i) == cha[i].ToString()))
                    return false;
            }
            return true;
        }
        private static bool PleaseAddToList(List<string> delim) { return (Top(delim) == "[" || StackPattern(delim, ",[")); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Deserialize -->
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            // --------------------------------------------------------------------------
            //  Get token list, clean token list
            // --------------------------------------------------------------------------
            string[] tokenList = TokensFor(json).ToArray();
            for (int i = 0; i < tokenList.Length; ++i) tokenList[i] = tokenList[i].Trim();
            if (tokenList[0] != "{")
                throw new NotImplementedException("RichJson.Deserialization - this deserializer only deserializes unnamed objects");


            // --------------------------------------------------------------------------
            //  instantiate empty object and collection variables
            // --------------------------------------------------------------------------
            List<string> delim = new List<string>();  // include
            List<string> name = new List<string>();  // include
            List<object> active = new List<object>();  // include

            //  temp variables:
            Type type = null;
            object value = null;  // include
            object output = null;
            PropertyInfo prop = null;


            // --------------------------------------------------------------------------
            //  Yet another stream parser
            // --------------------------------------------------------------------------
            for (int idx = 0; idx < tokenList.Length; ++idx)
            {
                string token = tokenList[idx];  // include
                if (idx == 5171 || idx == 5271)
                    Pause();
                switch (token)
                {
                    case "": break;
                    case ":": Push(delim, token); break;
                    case ",":
                        if (delim.Count > 1 && StackPattern(delim, ":,") && Top(name) == "Item") { Pop(delim); Pop(name); } // 'empty' item in json
                        else Push(delim, token);
                        break;
                    case "{":
                        if (idx == 0) { type = typeof(T); }  // it's the instance
                        else { type = Top(active).GetType();
                            if      (type.Name == "Dictionary`2" ) { type = type.GetGenericArguments()[1]; }  // it's a dictionary parameter
                            else if (type.Name == "List`1"       ) { type = type.GetGenericArguments()[0]; }
                            else if (type.Name == "ICollection`1") { type = type.GetGenericArguments()[0]; type = type.GetGenericArguments()[1]; }
                            else                                   { type = type.GetProperty(Top(name)).PropertyType; }  // it's an object parameter
                        }
                        try { value = Activator.CreateInstance(type); } catch (Exception ex) { Is.Trash(ex); throw; }
                        Push(active, value);
                        Push(delim, token);
                        break;
                    case "[":
                        type = PropertyOrFieldType(Top(active), Top(name));
                        if      (              type == null                )          { value = new List<object>();                                                                      }
                        else if (              type.Name == "Byte[]"       )          { value = new List<byte>  ();                                                                      }
                        else if (              type.Name == "Char[]"       )          { value = new List<char>  ();                                                                      }
                        else if (              type.Name == "ICollection`1")          { value = Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetGenericArguments()[0])); }
                        else if (Regex.IsMatch(type.FullName, "IEnumerable`1\\[\\[")) { value = new List<object>();                                                                      }
                        else                                                      try { value = Activator.CreateInstance(type); } catch (Exception ex) { Is.Trash(ex); }
                        Push(active, value);
                        Push(delim, token);
                        break;
                    case "}":
                        value = Pop(active);
                        if          (active.Count > 0)
                        {   type = Top(active).GetType();
                            if      (type.Name == "Dictionary`2") { ((IDictionary)Top(active)).Add(Convert.ChangeType(Pop(name), type.GetGenericArguments()[0]), value); }
                            else if (type.Name == "List`1"      ) { ((IList)      Top(active)).Add(value); }
                            else                                  { prop = type.GetProperty(Pop(name), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); try { prop.SetValue(Top(active), value, null); } catch (Exception ex) { Is.Trash(ex); throw; } } }
                        else                                      { output = (T)value; }
                        Pop(delim); PopIf(delim, ":"); PopIf(delim, ",");
                        break;
                    case "]":
                        value = Pop(active);
                        if (active.Count > 0)
                        {
                            type = Top(active).GetType();
                            string myName = Top(name);
                            prop = type.GetProperty(Pop(name), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (prop != null)
                            {   try { if (prop.PropertyType.Name == "Byte[]" && value.GetType().Name == "List`1") { value = ((List<byte>)value).ToArray(); } prop.SetValue(Top(active), value, null); }
                                catch (ArgumentException ex) { if (ex.Message != "Property set method not found.") throw; }
                                catch (Exception ex) { Is.Trash(ex); throw; }
                            }
                            else
                            {
                                Pause();
                            }
                        }
                        Pop(delim); PopIf(delim, ":"); PopIf(delim, ",");
                        break;
                    default:
                        if (token == "$ref-0")
                        {
                            type = Top(active).GetType();
                            if (type.Name == "List`1")
                            {
                                ((IList)Top(active)).Add(Bottom(active));
                            }
                            else
                            {
                                string myName = Pop(name);
                                PropertyInfo myProp = type.GetProperty(myName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                myProp.SetValue(Top(active), Bottom(active), null);
                                Pop(delim);
                            }
                            PopIf(delim, ",");
                        }
                        else if (Regex.IsMatch(token, "^.ref-[0-9]+"))
                        {
                            string[] strToken = Regex.Split(token, "^(.ref-)([0-9]+)(.*)$");
                            object obj = FindItemX(active, IntValue(strToken[2], 0));
                            type = Top(active).GetType();
                            if (type.Name == "List`1")
                            {
                                Type obtype = obj.GetType();
                                ((IList)Top(active)).Add(obj);
                            }
                            else
                            {
                                string myName = Pop(name);
                                PropertyInfo myProp = type.GetProperty(myName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                myProp.SetValue(Top(active), obj, null);
                                Pop(delim);
                            }
                            PopIf(delim, ",");
                        }
                        else
                        {
                            string[] str = Regex.Split(token, "^(\")(.*)(\")$", RegexOptions.Singleline);
                            string text = token;
                            if (str.Length > 1) text = str[2];


                            if      (delim.Count > 0 && Top(delim) == ":") { AddProperty(Top(active), Pop(name), text); Pop(delim); PopIf(delim, ","); }
                            else if (PleaseAddToList(delim)              ) { AddToList  (Top(active), text);                        PopIf(delim, ","); }
                            else if (                   Top(delim) == ",") { if (Second(delim) == "{")                              Push (name, text); }
                            else                                           {                                                        Push (name, text); }
                        }
                        break;
                }
                if (delim.Count > 1 && Top(delim) == ",")
                {
                    if (Second(delim) == "," || Second(delim) == ":")
                        Pause();
                }
            }

            return (T)output;
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
        /// <!-- FindItemX -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        /// <param name="objIdx"></param>
        /// <returns></returns>
        private static object FindItemX(List<object> active, int objIdx)
        {
            List<object> obj = new List<object>();
            for (int i = 0; i < active.Count && obj.Count <= objIdx; ++i)
                if (active[i].GetType().GetInterface("IEnumerable") == null)
                    obj.Add(active[i]);
            return obj[objIdx];
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddToList -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="text"></param>
        private static void AddToList(object list, string text)
        {
            Type listType = list.GetType();
            Type[] arg = listType.GetGenericArguments();
            Type type = arg[0];
            object item = Convert.ChangeType(text, type);
            ((IList)list).Add(item);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PropertyOrFieldType -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static Type PropertyOrFieldType(object active, string name)
        {
            Type type = active.GetType();
            Type output = null;
            string tagtn = type.Name;
            PropertyInfo prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop == null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    output = field.FieldType;
            }
            else
            {
                output = prop.PropertyType;
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddProperty -->
        /// <summary>
        ///      Fills a property value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        internal static void AddProperty(object obj, string name, string text)
        {
            Type objType = obj.GetType();
            Type type = PropertyOrFieldType(obj, name);

            if (type == null)
            {
                if (objType.Name == "Dictionary`2")
                {
                    object key   = name;
                    if (Regex.IsMatch(objType.FullName, "\\[\\[System.Char")) { key = Convert.ToChar(name); }


                    object value = text;
                    if (Regex.IsMatch(objType.FullName, "\\],\\[System.Double")) { value = Convert.ToDouble(text); }


                    ((IDictionary)obj).Add(key, value);
                }
                else
                    obj = text;
            }
            else
            {
                PropertyInfo prop = objType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                try
                {
                    if (text == "null") { }
                    //else if (type.Name == "Endeme")
                    //{
                    //    string[] str = Regex.Split(text, "^(.*)(:)(.*)$");
                    //    Endeme en = null;
                    //    if (string.IsNullOrEmpty(str[1].Trim())) en = new Endeme(str[3]); else en = new Endeme(new EndemeSet(str[1]), str[3]);
                    //    prop.SetValue(obj, en, null);
                    //}
                    else if (Regex.IsMatch(type.FullName, "DateTime"                   )) { prop.SetValue(obj, DateTime.Parse(text, null, DateTimeStyles.RoundtripKind), null); }
                    else if (Regex.IsMatch(type.Name    , "^(String)$"                 )) { prop.SetValue(obj, Convert .ChangeType      (text, type), null); }
                  //else if (Regex.IsMatch(type.Name    , "^(Guid)$"                   )) { prop.SetValue(obj, Guid    .Parse           (text      ), null); }
                    else if (Regex.IsMatch(type.FullName, "Nullable`1\\[\\[System.Guid")) { prop.SetValue(obj,          GuidValue       (text      ), null); }
                    else if (Regex.IsMatch(type.FullName, "System.Int32"               )) { prop.SetValue(obj, int     .Parse           (text      ), null); }
                    else if (Regex.IsMatch(type.Name    , "^Byte\\[\\]"                )) { prop.SetValue(obj, Convert .FromBase64String(text      ), null); }
                    else                                                                  { prop.SetValue(obj, Convert .ChangeType      (text, type), null); }
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == "Property set method not found.") { }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Is.Trash(ex);
                    throw;
                }
            }
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
        public static Guid GUID(string strGuid64)
        {
            string value = strGuid64.Replace("_", "/").Replace("-", "+");
            byte[] buffer = Convert.FromBase64String(value + "==");
            Guid guid = new Guid(buffer);
            return guid;
        }
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
        private static Regex _isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

        // ----------------------------------------------------------------------------------------
        /// <!-- AddData -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <param name="text"></param>
        private static void AddData(object obj, PropertyInfo prop, string text)
        {
            prop.SetValue(obj, Convert.FromBase64String(text), null);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FormatTestScript -->
        /// <summary>
        ///      Formats a json string for a test script and embeds it in a test script shell
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static string FormatTestScript(string json, string className, string methodName)
        {
            string tab     = "    ";
            string indent2 = "\r\n" + tab + tab;
            string indent3 = "\r\n" + tab + tab + tab;
            string indent4 = "\r\n" + tab + tab + tab + tab;
            string formattedJsonString = json;


            // --------------------------------------------------------------------------
            //  Format json string
            // --------------------------------------------------------------------------
            formattedJsonString = Regex.Replace(formattedJsonString, "(\\\\)"                               , "$1$1");
            formattedJsonString = Regex.Replace(formattedJsonString, "\""                                   , "\\\"");
            formattedJsonString = Regex.Replace(formattedJsonString, "^"                                    , indent3 + tab + "+ \"\\r\\n\" + \"", RegexOptions.Multiline);
            formattedJsonString = Regex.Replace(formattedJsonString, "^" + indent3 + tab + "[+] ..r.n. [+] ", indent3 + "string json" + className + " = ");
            formattedJsonString = Regex.Replace(formattedJsonString, "\r"                                   , "\"\r"                             , RegexOptions.Singleline);
            formattedJsonString += "\"";


            // --------------------------------------------------------------------------
            //  Place inside test method shell
            // --------------------------------------------------------------------------
            string output
                = tab + tab + ""
                + indent2 + "// ----------------------------------------------------------------------------------------"
                + indent2 + "/// <!-- " + methodName + "_comparisontest -->"
                + indent2 + "/// <summary>"
                + indent2 + "/// "
                + indent2 + "/// </summary>public void " + methodName + "_comparisontest()"
                + indent2 + "{"
                + indent3 + "// --------------------------------------------------------------------------"
                + indent3 + "//  Common test variables"
                + indent3 + "// --------------------------------------------------------------------------"
                + indent3 + "string   className  = \"" + className + "\";"
                + indent3 + "string   methodName = \"" + methodName + "\";"
                + indent3 + "DateTime start     = DateTime.UtcNow.AddMinutes(-2);"
                + indent3 + "Assert.ThingsAbout(className, methodName);"
                + ""
                + ""
                + indent3 + "// --------------------------------------------------------------------------"
                + indent3 + "//  Build input test object"
                + indent3 + "// --------------------------------------------------------------------------"
                + formattedJsonString + ";"
                + indent3 + className + " obj = RichJson.Deserialize<" + className + ">(json" + className + ");"
                + ""
                + ""
                + indent3 + "int a = 0;"
                + indent3 + "int c = 0;"
                + indent3 + "try"
                + indent3 + "{"
                + indent4 + "// ----------------------------------------------------------------------"
                + indent4 + "//  Run test"
                + indent4 + "// ----------------------------------------------------------------------"
                + indent4 + "a = _adminSvc." + methodName + "(obj, \"A\", methodName);"
                + indent4 + "c = _adminSvc." + methodName + "(obj, \"C\", methodName);"
                + ""
                + indent4 + "Assert.That(a, Is.equal_to, c, className, methodName);"
                + indent3 + "}"
                + indent3 + "catch (Exception ex)"
                + indent3 + "{"
                + indent4 + "Assert.Crash(ex);"
                + indent3 + "}"
                + indent3 + "finally"
                + indent3 + "{"
                + indent4 + "// ----------------------------------------------------------------------"
                + indent4 + "//  Cleanup"
                + indent4 + "// ----------------------------------------------------------------------"
                + indent3 + "}"
                + ""
                + ""
                + indent3 + "// --------------------------------------------------------------------------"
                + indent3 + "//  Return results"
                + indent3 + "// --------------------------------------------------------------------------"
                + indent3 + "_result += Assert.Conclusion;"
                + indent2 + "}";

            return output;
        }

        private static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- JsonToken -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static string JsonToken(string token)
        {
            if (Regex.IsMatch(token, "^(false|true|.ref-[0-9]|[-0-9Ee]+)$")) return token;
            string output = token;
            if (Regex.IsMatch(token, "^\".*\"$", RegexOptions.Singleline))
                output = Regex.Replace(Regex.Replace(output, "^\"", ""), "\"$", "");
            return output;
        }

        // ----------------------------------------------------------------------------------------
        //  Not thread safe
        // ----------------------------------------------------------------------------------------
        private static List<object> startObj { get { return _startObj; } set { _startObj = value; } }
        private static List<object> _startObj;

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeObject -->
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Warning - this JSON serializer is vulernable to infinite recursion loops
        /// </remarks>
        internal static string SerializeObject<T>(T obj, int depth)
        {
            // --------------------------------------------------------------------------
            //  Avoid infinite loops
            // --------------------------------------------------------------------------
            for (int i = 0; i < startObj.Count; ++i)
                if ((object)obj == startObj[i])
                    return "$ref-" + i.ToString();
            if (obj == null) Push(startObj, Guid.NewGuid());
            else Push(startObj, obj);


            // --------------------------------------------------------------------------
            //  Do stuff
            // --------------------------------------------------------------------------
            if (Indent < 1) Indent = 4;
            Type type = obj.GetType();
            PropertyInfo[] propList = type.GetProperties(BindingFlags.Public|BindingFlags.Instance|BindingFlags.SetProperty|BindingFlags.NonPublic);
            string json = "{";


            if (propList.Length > 0)
            {
                json = "{";
                if (depth > 0)
                    json = "\r\n".PadRight(2+depth*Indent) + "{";
                string delim = " ";
                for (int idx = 0; idx < propList.Length; ++idx)
                {
                    PropertyInfo property = propList[idx];
                    if (property.Name == "OverviewMessageHtml")
                        Pause();
                    if (property.CanWrite)
                    {
                        string jsonValue = SerializeProperty(obj, property, depth);
                        json += delim + "\"" + property.Name + "\"" + ":" + jsonValue;
                        delim = "\r\n".PadRight(2+depth*Indent) + ", ";
                    }
                    else
                    {
                        try
                        {
                            string jsonValue = SerializeProperty(obj, property, depth);
                            json += delim + "\"" + property.Name + "\"" + ":" + jsonValue;
                            delim = "\r\n".PadRight(2+depth*Indent) + ", ";
                        }
                        catch (TargetParameterCountException) { } // normal exceptions
                        catch (TargetInvocationException) { }
                        catch (Exception ex)
                        {
                            Is.Trash(ex);
                            throw;
                        }
                    }
                }
                json += "\r\n".PadRight(2+depth*Indent)+"}";
            }
            else
            {
                json = "\"" + obj.ToString() + "\"";
            }


            // --------------------------------------------------------------------------
            //  Administrative tasks
            // --------------------------------------------------------------------------
            Pop(startObj);
            return json;
        }
        public static string Serialize             <T>(T obj                                     ) { startObj = new List<object>(); return                  SerializeObject<T>(obj, 0);                         }
        public static string SerializeForTestScript<T>(T obj, string className, string methodName) { startObj = new List<object>(); return FormatTestScript(SerializeObject<T>(obj, 0), className, methodName); }

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeProperty -->
        /// <summary>
        ///      Part of serialization
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string SerializeProperty<T>(T obj, PropertyInfo property, int depth)
        {
            Type   type  = property.PropertyType;
            object value = null;
            string json  = "";


            try
            {
                value = property.GetValue(obj, null);


                // ----------------------------------------------------------------------
                //  Serialize the value
                // ----------------------------------------------------------------------
                // serialize simple and primitive types:
                if      (value == null)                                    { json =         "null"                     ; }
                else if (Regex.IsMatch(type.Name, "^(String|Char|Guid)$")) { json =  "\"" + value.ToString()    + "\"" ; }
                else if (Regex.IsMatch(type.Name, "^(Boolean)$"         )) { json =         value.ToString().ToLower() ; }
                else if (Regex.IsMatch(type.Name, "^(Double|Int32)$"    )) { json =         value.ToString()           ; }
                // serialize collections:
                else if (              type.Name == "Byte[]"             ) { json = SerializeData      (value)         ; }
                else if (              type.Name == "Dictionary`2"       ) { json = SerializeDictionary(value, depth+1); }
                else if (              type.Name == "List`1"             ) { json = SerializeList      (value, depth+1); }
                else if (              type.Name == "ICollection`1"      ) { json = SerializeArray     (value, depth+1); }
                else if (              type.Name == "IEnumerable`1"      ) { json = SerializeArray     (value, depth+1); }
                else if (Regex.IsMatch(type.Name,  @"\[\]"              )) { json = SerializeArray     (value, depth+1); }
                // serialize objects:
                else if (              type.Name == "Endeme"             ) { json = SerializeEndeme    (value)         ; }
                else if (Regex.IsMatch(type.FullName, "DateTime"        )) { json = SerializeDateTime  (value)         ; }
                else                                                       { json = SerializeObject    (value, depth+1); }
            }
            catch (TargetParameterCountException) { }  // 'normal' excpetions
            catch (TargetInvocationException) { }  // 'normal' excpetions
            catch (Exception ex)
            {
                Is.Trash(ex);
                throw;
            }

            return json;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeData -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string SerializeData(object value)
        {
            Byte[] data = (Byte[])value;

            string text = Convert.ToBase64String(data);

            return "\"" + text + "\"";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeArray -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <remarks>almost identical to SerializeList</remarks>
        private static string SerializeArray(object value, int depth)
        {
            IEnumerable<object> list  = ((IEnumerable)value).Cast<object>();
            StringBuilder       json  = new StringBuilder();
            int                 count = 0;
            foreach (object item in list) count++;


            if (count == 0) json.Append("[ ]");
            else
            {
                // ----------------------------------------------------------------------
                //  Standard list version
                // ----------------------------------------------------------------------
                string delim = " ";
                json.Append("\r\n".PadRight(2+depth*Indent) + "[");
                foreach (object item in list)
                {
                    json.Append(delim + SerializeObject(item, depth+1));
                    delim = "\r\n".PadRight(2 + depth * Indent) + ", ";
                }
                json.Append("\r\n".PadRight(2 + depth * Indent) + "]");


                // ----------------------------------------------------------------------
                //  Compact version
                // ----------------------------------------------------------------------
                if (json.Length - (Indent*depth+3)*(count+1) <= 256)
                {
                    delim = "";
                    json = new StringBuilder();
                    json.Append("[");
                    foreach (object item in list)
                        { json.Append(delim + SerializeObject(item, depth+1)); delim = ","; }
                    json.Append("]");
                }
            }

            return json.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeDateTime -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string SerializeDateTime(object value)
        {
            DateTime dt = (DateTime)value;
            if (dt.Kind == DateTimeKind.Utc)
                 return "\"" + dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"  ) + "\"";
            else return "\"" + dt.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") + "\"";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeDictionary -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static string SerializeDictionary(object value, int depth)
        {
            string      json        = "";
            string      delim       = "";
            IDictionary associateOf = (IDictionary)value;
            int count = associateOf.Count;


            if (count == 0) json += "{ }";
            else
            {
                // ----------------------------------------------------------------------
                //  Standard version
                // ----------------------------------------------------------------------
                json = "\r\n";
                json += "".PadRight(depth*Indent) + "{ ";
                foreach (object key in associateOf.Keys)
                {
                    object val = associateOf[key];
                    Type keytype = key.GetType();
                    if (Regex.IsMatch(keytype.Name, "^(String|Char|Guid)$"))
                        json += delim + "\"" + key.ToString() + "\":";
                    else json += delim + key.ToString() + ":";
                    json += " " + SerializeObject(val, depth + 1);
                    delim = "\r\n".PadRight(2 + depth * Indent) + ", ";
                }
                json += "\r\n".PadRight(2 + depth * Indent) + "}";


                // ----------------------------------------------------------------------
                //  Compact version
                // ----------------------------------------------------------------------
                int len = json.Length + 2 - (Indent*depth+4)*(count+1);
                if (len <= 256)
                {
                    delim = "";
                    json = "{";
                    foreach (object key in associateOf.Keys)
                    {
                        object val = associateOf[key];
                        Type keytype = key.GetType();
                        if (Regex.IsMatch(keytype.Name, "^(String|Char|Guid)$"))
                            json += delim + "\"" + key.ToString() + "\":";
                        else json += delim + key.ToString() + ":";
                        json += SerializeObject(val, depth + 1);
                        delim = ",";
                    }
                    json += "}";
                }
            }

            return json;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeEndeme -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string SerializeEndeme(object value)
        {
            return "\""
                + StrValue(value, "SetName", "")
                + ":"
                + value.ToString()
                + "\"";
        }

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

        // ----------------------------------------------------------------------------------------
        /// <!-- SerializeList -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static string SerializeList (object value, int depth)
        {
            IList  list  = (IList)value;
            string json  = "";
            int count = 0;
            foreach (object item in list) count++;


            if (count == 0) json = "[ ]";
            else
            {
                // ----------------------------------------------------------------------
                //  Standard list version
                // ----------------------------------------------------------------------
                string delim = " ";
                json   = "\r\n".PadRight(2+depth*Indent) + "[";
                foreach (object item in list)
                {
                    json += delim + SerializeObject(item, depth+1);
                    delim = "\r\n".PadRight(2 + depth * Indent) + ", ";
                }
                json += "\r\n".PadRight(2 + depth * Indent) + "]";


                // ----------------------------------------------------------------------
                //  Compact version
                // ----------------------------------------------------------------------
                if (json.Length - (Indent*depth+3)*(count+1) < 256)
                {
                    delim = "";
                    json  = "[";
                    foreach (object item in list)
                        { json += delim + SerializeObject(item, depth+1); delim = ","; }
                    json += "]";
                }
            }

            return json;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TokensFor -->
        /// <summary>
        ///      Returns a clean token list for the tokens in a json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        /// <remarks>
        ///      A very inefficient json string tokenizer
        ///      Warning - this tokenizer does not handle escaped quotes within strings properly
        /// </remarks>
        internal static List<string> TokensFor(string json, int longStringLength, int depth)
        {
            greatestDepth = Math.Max(depth, greatestDepth);
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(json.Trim()))
                return list;
            json = json.Trim();


            if (depth == 100)
                Pause();


            // --------------------------------------------------------------------------
            //  Divide the json string based on syntax
            // --------------------------------------------------------------------------
            string[] quote = null;
            string[] colon = { "" };
            string[] comma = { "" };
            string[] other = { "" };


            if (json.Length > longStringLength)
            {
                // ----------------------------------------------------------------------
                //  long json
                //
                //    Note: The logic for longer JSON strings here is designed to keep
                //    recursion depth from being too excessive, only the code
                //    under 'short json' is actually needed to divide the json string
                // ----------------------------------------------------------------------
                MatchCollection mc = Regex.Matches(json, "\"[- !#%-'.0-9@A-Z_`a-z~]*\"", RegexOptions.Singleline); // Match on strings made of characters that are not particularly special for either JSON or Regex
                if (mc.Count > 1)
                {
                    // ------------------------------------------------------------------
                    //  Divide on the middle quoted string
                    // ------------------------------------------------------------------
                    string middle = mc[mc.Count / 2].Value;
                    quote                        = Regex.Split(json, "^(.*)([^=;])("+middle+")([^a-z_<])(.*)$", RegexOptions.Singleline);
                    if (quote.Length == 1) colon = Regex.Split(json, "^(:)(.*)$"                              , RegexOptions.Singleline);  //  divide on the colon character
                    if (quote.Length == 1) comma = Regex.Split(json, "^(.*)([^a-df-km-z])(,)(.*)$"            , RegexOptions.Singleline);  //  divide on comma
                    if (quote.Length == 1) other = Regex.Split(json, "^(.*)(\\[|\\]|\\{|\\})(.*)$"            , RegexOptions.Singleline);
                }
                else
                {
                    // ------------------------------------------------------------------
                    //  Few string matches - divide on the last quoted string
                    // ------------------------------------------------------------------
                    quote                        = Regex.Split(json, "^(.*)([^=;])(\"[^\"]*\")([^a-z_<])(.*)$", RegexOptions.Singleline);
                    if (quote.Length == 1) colon = Regex.Split(json, "^(:)(.*)$"                              , RegexOptions.Singleline);
                    if (quote.Length == 1 && !IsHtmlString(json))                                        
                                           comma = Regex.Split(json, "^(.*)([^a-df-k-m-z])(,)(.*)$"           , RegexOptions.Singleline);
                    if (quote.Length == 1) other = Regex.Split(json, "^(.*)(\\[|\\]|\\{|\\})(.*)$"            , RegexOptions.Singleline);
                }
            }
            else
            {
                    // ------------------------------------------------------------------
                    //  short json - you can use just this code if you don't mind extremely deep recursion
                    // ------------------------------------------------------------------
                    quote                        = Regex.Split(json, "^(.*)([^=;])(\"[^\"]*\")([^a-z_<])(.*)$", RegexOptions.Singleline);  //  divide on a simple quoted string
                    if (quote.Length == 1) colon = Regex.Split(json, "^(:)(.*)$"                              , RegexOptions.Singleline);  //  divide on the colon character
                    if (quote.Length == 1) comma = Regex.Split(json, "^(.*)([^a-df-km-z])(,)(.*)$"            , RegexOptions.Singleline);  //  divide on comma
                    if (quote.Length == 1) other = Regex.Split(json, "^(.*)(\\[|\\]|\\{|\\})(.*)$"            , RegexOptions.Singleline);  //  divide on other token characterers important in json syntax
            }


            // --------------------------------------------------------------------------
            //  Recurse on divided json string
            // --------------------------------------------------------------------------
            if      (json.Length  == 1) {                                                                         list.Add(json);   } // single character tokens
            else if (quote.Length >  1) { list.AddRange(TokensFor(quote[1]+quote[2], longStringLength, depth+1)); list.Add(quote[3]); list.AddRange(TokensFor(quote[4]+quote[5], longStringLength, depth+1)); }
            else if (colon.Length >  1) {                                                                         list.Add(colon[1]); list.AddRange(TokensFor(colon[2]         , longStringLength, depth+1)); }
            else if (comma.Length >  1) { list.AddRange(TokensFor(comma[1]+comma[2], longStringLength, depth+1)); list.Add(comma[3]); list.AddRange(TokensFor(comma[4]         , longStringLength, depth+1)); }
            else if (other.Length >  1) { list.AddRange(TokensFor(other[1]         , longStringLength, depth+1)); list.Add(other[2]); list.AddRange(TokensFor(other[3]         , longStringLength, depth+1)); }
            else                        { if (json != "null") JsonToken(json);                                    list.Add(json);   } // the pause is so you can follow the addition of uncommon 'primitives' to the list

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsHtmlString -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static bool IsHtmlString(string json)
        {
            if (!Regex.IsMatch(json, "^\".*\"$", RegexOptions.Singleline))  return false;
            return true;
        }

        public static List<string> TokensFor(string json) { greatestDepth = 0; return TokensFor(json, 50, 0); }
    }
}
