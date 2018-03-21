//-------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms of
// the GNU Lesser General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with
// InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//-------------------------------------------------------------------------------------------------
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Runtime.CompilerServices;// for [CallerMemberName] in C# 4.5
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Testing  // add to this namespace very sparingly and use only System references
{
    // --------------------------------------------------------------------------------------------
    //  This file contains three classes and an enumeration: Assert, Result, ResultStep and ResultStatus
    //
    //     The heavy lifting however is done by another method outside this file called Is.Ok
    // --------------------------------------------------------------------------------------------


    // --------------------------------------------------------------------------------------------
    /// <!-- ResultStatus -->
    /// <summary>
    ///      An enumeration containing various states for a test or a test sequence, usually focused on results
    /// </summary>
    public enum ResultStatus { Crashed, Failed, Ignore, Ok, SmokeTest, Untested } // Ignore: ignore failures        maybe add: InProgress, PoorTest, Start


    // --------------------------------------------------------------------------------------------
    /// <!-- Assert -->
    /// <summary>
    ///      The Assert class provides a set of methods for testing units and functions,
    ///      and provides a structure for processing failed tests
    /// </summary>
    /// <remarks>
    ///      I prefer that tests for each namespace/folder be located inside that namespace/folder
    ///      
    ///      this code is a bit slippery, it is used differently than expected
    ///       - ThingsAbout - initiates a test sequence
    ///       - Conclusion  - both combines a series of results into one and provides output for display
    /// </remarks>
    public static class Assert
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private static Result MyResult { get { if (_result == null) _result = new Result(""); return _result; } set { _result = value; } } private static Result _result;


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        /* ------------------------------------------------------------------------------------- */                                                       /// <summary>when debugging it's nice to be able to roll over a property to see what is going on</summary>
        public  static string Detail                            { get { return MyResult.AsciiDetailResults;                                           } } /// <summary>Completes an assertion sequence, summarizing the currrent one, and hiding the results of the sub-test internally</summary>
        public  static Result Conclusion                        { get { MyResult.Summarize();                                        return MyResult; } } /// <summary>another property to see what is going on</summary>
      //public  static Result Current                           { get { return MyResult;                                                              } } /// <summary>Begins a new assertion sequence, erasing the previous one</summary>
        public  static Result ThingsAbout(string testName           ) { _result = new Result(testName);                              return MyResult; }   /// <summary>Begins a new assertion sequence, erasing the previous one</summary>
        public  static Result ThingsAbout(string name, string method) { _result = new Result(name, method);                          return MyResult; }   /// <summary>Unspecified crash</summary>
        public  static Result Crash      ()                           { MyResult += Result.Crashed;                                  return MyResult; }   /// <summary>crash with exception</summary>
        public  static Result Crash      (Exception ex              ) { MyResult += Result.Crashed; MyResult.Last.Text = ex.Message; return MyResult; }   /// <summary>crash with message</summary>
        public  static Result Crash      (string    message         ) { MyResult += Result.Crashed; MyResult.Last.Text = message;    return MyResult; }
        public  static void   SmokeTest  ()                           { if (StringIsNullOrWhiteSpace(MyResult.Errors)) MyResult += Result.SmokeTest; else MyResult += Result.SmokeTest; }
        public  static ResultStatus MostRecentResult            { get { return MyResult.Last.TestResult;                                              } }


        // ----------------------------------------------------------------------------------------
        /// <!-- That -->
        /// <summary>
        ///      Performs a binary operation check but sets failures to specified status
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="operation"></param>
        /// <param name="rhs"></param>
        /// <param name="statusIfFail"></param>
        /// <param name="name">often class name but sometimes test sequence name</param>
        /// <param name="text">often method name but sometimes test name</param>
        /// <returns></returns>
        /// <remarks>Is.Ok here does the heavy lifting</remarks>
        public static Result That(object lhs, string operation, object rhs, ResultStatus statusIfFail, string name = "", string text = "")      //public static AssertResult That(object lhs, string operation, object rhs, [System.Runtime.CompilerServices.CallerMemberName]string text = "") // C# 4.5
        {
            name = MyResult.BestName(name);

            bool ok = Is.Ok(lhs, operation, rhs);
            Result result = new Result(ok, name, MyResult.BestText(name, text));
            if (!ok) result.Last.TestResult = statusIfFail;
            MyResult += result;
            return MyResult;
        } /// <summary>Performs a specified binary operation check</summary>
        public static Result That(object lhs, string operation, object rhs, string name = "", string text = "")
        {
          //return That(lhs, operation, rhs, ResultStatus.Failed, name, text);
            name = MyResult.BestName(name);
            text = MyResult.BestText(name, text);
            Result result = new Result(lhs, operation, rhs, name, text);
            MyResult += result;
            return MyResult;
        }
        public static Result That(object lhs, string operation, object rhs, string text) { return That(lhs, operation, rhs, "", text); }

        public static void ClearErrors()
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StringContainsStuff -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool StringContainsStuff(string str)
        {
          //return (!string.IsNullOrWhiteSpace(str));
            if (str == null                     ) return false;
            if (string.IsNullOrEmpty(str.Trim())) return false;
            if (Regex.IsMatch(str, @"^\s*$")    ) return false;
            return true;
        }
        private static bool StringIsNullOrWhiteSpace(string str) { return !StringContainsStuff(str); }
    }


    // --------------------------------------------------------------------------------------------
    /// <!-- Result -->
    /// <summary>
    ///      The Result class contains results from a series of Assert.That() checks, stored as ResultSteps
    /// </summary>
    /// <remarks>
    ///      Warning: The code sometimes affects the result list when it looks like it could be merely returning a result
    ///      
    ///      The Is.Ok method does most of the heavy lifting
    ///      
    ///      Add, And and Append are two distinctly different operation
    ///      And is like a logical piecewise AND of two steps into one
    ///      Add and Append add a new step to the list
    ///      
    ///      Summarize   - combines a series of results into one result
    /// </remarks>
    public class Result
    {
        // ----------------------------------------------------------------------------------------
        //  Member properties
        /* ------------------------------------------------------------------------------------- */ /// <summary>This is where most of the result data is kept</summary>
        private List<ResultStep> Step      { get; set; }                                            /// <summary>Something to call this test sequence</summary>
        public         string TestSequence { get; set; }                                            /// <summary>For test step detail display</summary>
        private static string TestPrefix   { get { return "((((((((((   "     ; } }                 /// <summary>For test step detail display</summary>
        private static string TestSuffix   { get { return " test   ))))))))))"; } }                 /// <summary>General error information</summary>
        public         string Errors       { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public Result(                                          string testSequenceName   ) { Init("", "", "", ResultStatus.Untested, ""  , ""    ); TestSequence = testSequenceName; }
        public Result(                                          string name, string method) { Init("", "", "", ResultStatus.Untested, name, method);                                  }
        public Result(bool   ok                               , string name, string method) { Init("", "", "", ResultStatusValue(ok), name, method);                                  }
        public Result(object lhs, string operation, object rhs, string name, string method) { Init(lhs, operation, rhs, ResultStatusValue(Is.Ok(lhs, operation, rhs)), name, method); Errors = Is.SetErrors + Is.SameErrors; }


        // ----------------------------------------------------------------------------------------
        //  Short accessors and properties
        /* ------------------------------------------------------------------------------------- */
        private ResultStep this[int i] { get { if (IsEmpty || i >= Step.Count) return null; return Step[i]                          ; } }
        public  bool       IsEmpty     { get {                                              return (Count == 0)                     ; } }
        public  bool       IsTrue      { get {                                              return (FailedCount == 0 && OkCount > 0); } }
        public  ResultStep Last        { get { ResultStep item = Step[Step.Count-1];        return item                             ; } }

        public int Count         { get { return Step.Count; } }
        public int OkCount       { get { int count = 0; for (int i=0;i<Count;++i) if (Step[i].TestResult == ResultStatus.Ok     || Step[i].TestResult == ResultStatus.SmokeTest) count++; return count; } }
        public int FailedCount   { get { int count = 0; for (int i=0;i<Count;++i) if (Step[i].TestResult == ResultStatus.Failed || Step[i].TestResult == ResultStatus.Crashed  ) count++; return count; } }
        public int UntestedCount { get { int count = 0; for (int i=0;i<Count;++i) if (Step[i].TestResult == ResultStatus.Ignore || Step[i].TestResult == ResultStatus.Untested ) count++; return count; } }


        // ----------------------------------------------------------------------------------------
        //  Methods for initiating new result lists
        /* ------------------------------------------------------------------------------------- */
        public static Result Crashed   { get { Result result = new Result(false, "", ""); result.Last.TestResult = ResultStatus.Crashed  ; return result; } }
        public static Result Empty     { get { Result result = new Result("");                                                             return result; } }
        public static Result Failed    { get { Result result = new Result(false, "", "");                                                  return result; } }
        public static Result Ignore    { get { Result result = new Result(false, "", ""); result.Last.TestResult = ResultStatus.Ignore   ; return result; } }
        public static Result Ok        { get { Result result = new Result(true , "", "");                                                  return result; } }
        public static Result SmokeTest { get { Result result = new Result(false, "", ""); result.Last.TestResult = ResultStatus.SmokeTest; return result; } }
        public static Result Start     { get { Result result = Empty;                                                                      return result; } }


        // ----------------------------------------------------------------------------------------
        //  Short operators and methods
        /* ------------------------------------------------------------------------------------- */                                                                  /// <summary>concatenates two result lists</summary>
        public  static Result operator +(Result     lhs   , Result rhs) { Result result = Add(lhs, rhs);                                            return result; } /// <summary>Ands the right and left hand sides into one step</summary>
        public  static Result operator &(Result     lhs   , Result rhs) { if (lhs == null) return null; Result result = Empty.And(lhs).And(rhs);    return result; } /// <summary>Appends a new result step on the the end of the result list</summary>
        private        Result And       (Result     result            ) { for (int i = 0; i < result.Count; ++i) And(result, i);                    return this  ; }
        private        Result Append    (ResultStep item              ) { Append("", "", "", item.TestResult, item.Name, item.Text, item.Internal); return this  ; }
        private        Result Clear     (                             ) { Step   = new List<ResultStep>();                                          return this  ; }
        private        Result Copy      (                             ) { Result result = new Result(TestSequence); result.Append(this);            return result; }

        private static ResultStatus ResultStatusValue(bool ok) { ResultStatus result = ResultStatus.Failed; if (ok) result = ResultStatus.Ok;  return result; }


        // ----------------------------------------------------------------------------------------
        //  Methods
        // ----------------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Appends rhs onto lhs
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private static Result Add(Result lhs, Result rhs)
        {
            if (lhs == null) return null;
            Result result = lhs.Copy().Append(rhs);
            if (StringContainsStuff(rhs.Errors))
                result.Errors = rhs.Errors;
            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- And -->
        /// <summary>
        ///      And's this result into the current result list item
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="operation"></param>
        /// <param name="rhs"></param>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Is.Ok here does the heavy lifting
        ///      'And' and 'Or' operate within one list item, & operates on the list itself
        /// </remarks>
        public Result And(object lhs, string operation, object rhs, string name = "", string method = "")
        {
            Last.Assure(name, method);
            if (Last.TestResult != ResultStatus.Failed)
            {
                if (Is.Ok(lhs, operation, rhs)) Last.TestResult = ResultStatus.Ok;
                else Last.TestResult = ResultStatus.Failed;
            }
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- And -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result And(Result result, int item)
        {
            Last.Assure(result[item].Name, result[item].Text);
            if (Last.TestResult != ResultStatus.Failed &&
                Last.TestResult != ResultStatus.Crashed) // keep taking the last result as long as nothing went wrong
            {
                if (result[item].TestResult != ResultStatus.Untested)
                {
                    Last.TestResult  = result[item].TestResult ;
                    Last.Name        = result[item].Name       ;
                    Last.Text        = result[item].Text       ;
                    Last.Operation   = result[item].Operation  ;
                    Last.TargetValue = result[item].TargetValue;
                    Last.TestValue   = result[item].TestValue  ;
                    Last.Internal    = result[item].Internal   ;
                }
            }
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Append -->
        /// <summary>
        ///      Appends the steps of a new Result on to the ResultStep list
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Result Append(Result result)
        {
            if (result.Count > 0)
            {
                if (Last.TestResult != ResultStatus.Untested)
                    Step.Add(result[0]);
                else
                    And(result, 0);
                for (int i = 1; i < result.Count; ++i)
                    Step.Add(result[i]);
            }
            return this;
        }
        private Result Append(object lhs, string operation, object rhs, ResultStatus status, string name, string method, Result inner)
        {
            Step.Add(new ResultStep(lhs, operation, rhs, status, name, method, inner)); return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiDetailResults -->
        /// <summary>
        ///      Returns resutl step detail including multiple levels when there is a failure
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///      recursive
        /// </remarks>
        public string AsciiDetailResults { get
        {
            string str = "";
            string delim = TestPrefix;
            if (StringContainsStuff(TestSequence))
                { str = TestSequence + ":"; delim = "\r\n" + TestPrefix; }
            int width = Math.Min(60, MaxNameLength);


            for (int i = 0; i < Step.Count; ++i)
            {
                // ------------------------------------------------------------------
                //  Return the current level
                // ------------------------------------------------------------------
                string methodText = FormatMethodText(Step, i);
                string innerText = methodText;
                if (innerText.Length  > 43)
                    innerText = innerText.Substring (0,43);
                string stepStr = Step[i].ToString().ToUpper();


                str += delim
                    + (Step[i].Name + " " + innerText).PadRight(width)
                    + TestSuffix
                    + " " + Step[i].ToString().ToUpper();
                if (Regex.IsMatch(Step[i].ToString(), "fail" , RegexOptions.IgnoreCase))  str += ": " + Errors + " - " + Step[i].TestValue + " " + Step[i].Operation + " " + Step[i].TargetValue;
                if (Regex.IsMatch(Step[i].ToString(), "crash", RegexOptions.IgnoreCase))  str += ": " + Errors + " - " + methodText;

                delim = "\r\n" + TestPrefix;


                // ------------------------------------------------------------------
                //  Return the next level down (This could be made recursive)
                // ------------------------------------------------------------------
                if (Regex.IsMatch(stepStr, "(crash|fail)", RegexOptions.IgnoreCase) && Step[i].Internal != null)
                {
                    string subBlock = Step[i].Internal.AsciiDetailResults;
                    subBlock = Regex.Replace(subBlock, "^[^\r]*UNTESTED\r\n", "", RegexOptions.Singleline);
                    if (Regex.IsMatch(subBlock, "^ *\\(")) str += "\r\n";
                    str += Regex.Replace(subBlock, "^", "    ", RegexOptions.Multiline);
                }
            }
            return str;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- BestName -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal string BestName(string name)
        {
            if (StringIsNullOrWhiteSpace(name) && StringContainsStuff(Last.Name   ))  name = this.Last.Name   ;
            if (StringIsNullOrWhiteSpace(name) && StringContainsStuff(TestSequence))  name = this.TestSequence;
            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- BestName -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal string BestText(string name, string text)
        {
            if (StringIsNullOrWhiteSpace(text) && StringContainsStuff(TestSequence) && name != TestSequence)  text = TestSequence;
            if (StringIsNullOrWhiteSpace(text) && StringContainsStuff(Last.Text   )                        )  text = Last.Text;
            return text;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StringContainsStuff -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool StringContainsStuff(string str)
        {
          //return (!string.IsNullOrWhiteSpace(str));
            if (str == null                     ) return false;
            if (string.IsNullOrEmpty(str.Trim())) return false;
            if (Regex.IsMatch(str, @"^\s*$")    ) return false;
            return true;
        }
        private static bool StringIsNullOrWhiteSpace(string str) { return !StringContainsStuff(str); }

        // ----------------------------------------------------------------------------------------
        /// <!-- FormatResult -->
        /// <summary>
        ///      Text is used for various things besides method name
        /// </summary>
        /// <param name="step"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private string FormatMethodText(List<ResultStep> step, int i)
        {
            string text = Step[i].Text;
            string methodText = "";
            if (text == null)
                    methodText = "null";
            else methodText = Regex.Replace(Step[i].Text, "[\r\n]", " ", RegexOptions.Singleline);
            if (methodText.Length > 80) methodText = methodText.Substring(0,80);

            return methodText;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FormatLastResult -->
        /// <summary>
        ///      Formats the test result
        /// </summary>
        /// <returns></returns>
        public string FormatLastResult()
        {
            string delim = "\r\n" + TestPrefix;
            return delim + (Last.Name + " " + Last.Text).PadRight(32) + TestSuffix + " " + Last.ToString().ToUpper();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="operation"></param>
        /// <param name="rhs"></param>
        /// <param name="status"></param>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private Result Init(object lhs, string operation, object rhs, ResultStatus status, string name, string method)
        {
            Step = new List<ResultStep>(); Append(lhs, operation, rhs, status, name, method, null ); return this  ;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MaxNameLength -->
        /// <summary>
        /// 
        /// </summary>
        private int MaxNameLength { get
        {
            int    width = 0;
            for (int i = 0; i < Step.Count; ++i)
                width = Math.Max((Step[i].Name + " " + Step[i].Text).Length, width);
            return width;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Or -->
        /// <summary>
        ///      Or's this result into the current result list item
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="operation"></param>
        /// <param name="rhs"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Is.Ok here does the heavy lifting
        ///      'And' and 'Or' operate within one list item, & operates on the list itself
        /// </remarks>
        public Result Or(object lhs, string operation, object rhs, string name = "", string text = "")
        {
            Last.Assure(name, text);
            if (Last.TestResult != ResultStatus.Ok)
            {
                if (Is.Ok(lhs, operation, rhs)) Last.TestResult = ResultStatus.Ok;
                else Last.TestResult = ResultStatus.Failed;
            }
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Summarize -->
        /// <summary>
        ///      Takes a list of results and boils it down into one summary result
        /// </summary>
        /// <returns></returns>
        public Result Summarize()
        {
            Result result = Start;
            for (int i = 0; i < Count; ++i) result = result.And(this, i);
            ResultStep step = result[0];
            step.Internal = this.Copy();


            if (StringContainsStuff(TestSequence) && StringIsNullOrWhiteSpace(step.Name))
            {
                step.Name = TestSequence;
                step.Text = "test";
            }
            result.TestSequence = TestSequence;


            Step = new List<ResultStep>();
            Append(step);
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SummaryMessage -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string SummaryMessage()
        {
            int numOk       = OkCount;
            int numFailed   = FailedCount;
            int numUntested = UntestedCount;


            string message  = TestSequence;
            if (numFailed == 0 && numOk > 0) message += " succeeded";
            else                             message += " " + numOk + " succeeded" + ", " + numFailed + " failed";
            if (numUntested > 0)             message += ", " + numUntested + " untested";
            return message;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            string delim  = "";
            for (int i = 0; i < Count; ++i)
                { result += delim + Step[i].Formatted(); delim = ","; }
            return result;
        }


        // --------------------------------------------------------------------------------------------
        /// <!-- ResultStep -->
        /// <summary>
        ///      The ResultStep class provides a place to put the results of one step in an Assert test
        ///      sequence (stored in the Result class) including a place to put condensed results (see Summarize())
        /// </summary>
        /// <remarks>we could put all sorts of stuff in here, like timing and stack trace</remarks>
        public class ResultStep
        {
            // ----------------------------------------------------------------------------------------
            //  Members
            // ----------------------------------------------------------------------------------------
            public ResultStatus TestResult  { get; set; }
            public string       Text        { get; set; } // usually method name
            public string       Name        { get; set; } // usually class name, sometimes test name
            public Result       Internal    { get; set; } // gets filled by Summarize()
            public string       TestValue   { get; set; }
            public string       Operation   { get; set; }
            public string       TargetValue { get; set; }


            // ----------------------------------------------------------------------------------------
            //  Constructor
            // ----------------------------------------------------------------------------------------
            public ResultStep(object testValue, string operation, object tgtValue, ResultStatus result, string name, string text, Result inner)
            { if (result == ResultStatus.Failed) Pause(); Init(testValue, operation, tgtValue, result, name, text, inner); }


            // ----------------------------------------------------------------------------------------
            //  Short methods
            // ----------------------------------------------------------------------------------------
            internal void   Assure   (string name, string method) { if (StringIsNullOrWhiteSpace(Name)) Name = name; if (StringIsNullOrWhiteSpace(Text)) Text = method; }
            public   string Formatted()                           { return Name + "." + Text + ":" + ToString().ToUpper(); }
            private  void   Pause    ()                           {            }


            // ----------------------------------------------------------------------------------------
            /// <!-- Init -->
            /// <summary>
            ///      Initializes a step object
            /// </summary>
            /// <param name="result"></param>
            /// <param name="name"></param>
            /// <param name="method"></param>
            private void Init(object lhs, string operation, object rhs, ResultStatus result, string name, string method, Result inner)
            {
                TestResult = result   ;
                Operation  = operation;
                Name       = name     ;
                Text       = method   ;
                Internal   = inner    ;
                if (lhs == null) TestValue   = "null"; else TestValue   = lhs.ToString();
                if (rhs == null) TargetValue = "null"; else TargetValue = rhs.ToString();
           }

            // ----------------------------------------------------------------------------------------
            /// <!-- ToString -->
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string output = "";
                switch (TestResult)
                {
                    case ResultStatus.Failed    : output = "Failed"    ; break;
                    case ResultStatus.Crashed   : output = "Crashed"   ; break;
                    case ResultStatus.Ignore    : output = "Ignored"   ; break;
                    case ResultStatus.SmokeTest : output = "Smoke Test"; break;
                    case ResultStatus.Ok        : output = "Ok"        ; break;
                    case ResultStatus.Untested  : output = "Untested"  ; break;
                    default : output = "Please add me to the 'switch (TestResult)' case statement"; break;
                }
                return output;
            }
        }
    }
}
