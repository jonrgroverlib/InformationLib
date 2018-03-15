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
using InformationLib.Testing;         // for RandomSource
using System;                         // for Console, Exception
using System.Collections.Generic;     // for Dictionary, List
using System.Configuration; /* add reference */ // for ConnectionStringSettingsCollection, ConfigurationManager
using System.Data;                    // for DataTable, DataColumn, DataRow
using System.Data.SqlClient;          // for SqlConnection, SqlCommand, SqlDataReader
using System.Diagnostics;             // for DebuggerStepThrough, StopWatch
using System.Text.RegularExpressions; // for Regex, RegexOptions

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- InfoAspect -->
    /// <summary>
    ///      The InfoAspect class contains methods that are useful for the program to know about itself
    /// </summary>
    /// <remarks>
    ///      This is a poor-man's version of aspect oriented programming,
    ///      it relies on including an aspect parameter in each method, and passing them down the call stack
    /// 
    /// 
    ///      Module dependency structure
    ///      ---------------------------
    /// 
    /// 
    ///        Micro   InfoAccess   Vertical   Winform     --(high level)
    ///         |  \       |           |          |
    ///         |   \      +-----+-----+          |
    ///         |    \           |                |
    ///         |     +----> DataAccess           |        --(low level) 
    ///         |                |                |
    ///         +----------------+                |
    ///         |                |                |
    ///         |         +------+-----+----------+
    ///         |         |      |     |          |
    ///      Endemes   HardData  |  SoftData   Strings     --[primitive modules]
    ///         |         |      |     |          |
    ///         +---------+------+-----+----------+
    ///                          | 
    ///                       Common 
    ///                      
    ///      
    ///      Aspects in this module will handle:
    ///       - connections
    ///       - performance
    ///       - logging
    ///       - exception handling (wires together logging, exceptions, and tests)
    ///       - the active working directory
    ///       - the active user
    ///       - automated tests
    ///       - GUI action trace (GAT)
    ///       - random numbers
    ///      
    ///      beta code - has been used in production once
    /// </remarks>
    public class InfoAspect : IDisposable
    {
        // ----------------------------------------------------------------------------------------
        //  Public Members
        // ----------------------------------------------------------------------------------------
                                                                         /// <summary>the role or access of the user running the application</summary>
        public        int                      AccessId    { get; set; } /// <summary>calling depth in the program</summary>
        public        int                      Level       { get; set; } /// <summary>the stack of method names - a poor man's stack trace // this implements a stack</summary>
        public        List<string>             Method      { get; set; } /// <summary>The file system root for the program's operations</summary>
        public        string                   Root        { get; set; } /// <summary>time just before the aspect was created</summary>
        public        DateTime                 Start       { get; set; } /// <summary>the user presently running the application</summary>
        public        int                      UserId      { get; set; } /// <summary>method performance members</summary>
        public static Dictionary<string, long> Performance { get; set; }
        public        Random                   r           { get; set; } /// <summary>contains a bunch of active endeme sets (should this be here?)</summary>
        public        EndemeReference          EnRef       { get; set; } /// <summary>sender or this for 'main method'</summary>
        public        object                   Sender      { get; set; } /// <summary>Project name</summary>
        public        string                   Project     { get; set; }
        public        string                   MainConn    { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Flags
        // ----------------------------------------------------------------------------------------
        public static bool MeasurePerformance { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Private Members
        // ----------------------------------------------------------------------------------------
        public  string        Conn             { get { return _inputConn       ; } }
        public  SqlConnection OutputConnection { get { return _outputConnection; }                                                                                  } private SqlConnection _outputConnection;
        public  SqlConnection InputConnection  { get { return _inputConnection ; }                                                                                  } private SqlConnection _inputConnection;
        private string        OutputConn       { get { return _outputConn      ; } set { _outputConn = value; _outputConnection = new SqlConnection(_outputConn); } } private string        _outputConn;
        private string        InputConn        { get { return _inputConn       ; } set { _inputConn  = value; _inputConnection  = new SqlConnection(_inputConn ); } } private string        _inputConn;


        private static Dictionary<string, Stopwatch> Watch         { get;                  set;                                                           } // method performance member
        private static Stopwatch                     _measureWatch { get;                  set;                                                           }


        // ----------------------------------------------------------------------------------------
        //  Overloaded Operators
        // ----------------------------------------------------------------------------------------
        /// <summary>'exits' a method</summary>
        public static InfoAspect operator --(InfoAspect a) { a.Pop(); return a; }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        //public Aspect(object sender, string conn, string root, int level)
        //{
        //    Sender = sender;
        //    Root   = root  ;
        //    Level  = level ;
        //    Method = new List<string>();
        //    Start  = DateTime.Now.AddMilliseconds(-100);
        //    r      = RandomSource.New().Random;


        //    InputConn  = conn;
        //    OutputConn = conn;
        //    if (!string.IsNullOrEmpty(conn))  _inputConnection  = new SqlConnection(conn);
        //    if (!string.IsNullOrEmpty(conn))  _outputConnection = new SqlConnection(conn);
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"   >name of project</param>
        /// <param name="sender"    >this or sender</param>
        /// <param name="inputConn" >connection</param>
        /// <param name="outputConn">maybe the same or different than the inputConn</param>
        /// <param name="root"      >root or working directory</param>
        /// <param name="level"     >usually starts at 0</param>
        public InfoAspect(string project, object sender, string inputConn, string outputConn, string root, int level)
        {
            Project    = project;
            Sender     = sender;
            Root       = root;
            Level      = level;
            Method     = new List<string>();
            Start      = DateTime.Now.AddMilliseconds(-100);
            r          = RandomSource.New().Random;
            InputConn  = inputConn ; if (!string.IsNullOrEmpty(inputConn .Trim())) _inputConnection  = new SqlConnection(inputConn );
            OutputConn = outputConn; if (!string.IsNullOrEmpty(outputConn.Trim())) _outputConnection = new SqlConnection(outputConn);
        }


        // ----------------------------------------------------------------------------------------
        //  Connections and Transactions
        // ----------------------------------------------------------------------------------------
        private static void Open     (SqlConnection connection) { try { WatchConn("Open" ); connection.Open (); } catch (Exception ex) { WatchFail(ex.Message); } }
        private static void Close    (SqlConnection connection) { try { WatchConn("Close"); connection.Close(); } catch (Exception ex) { WatchFail(ex.Message); } }
        private static void WatchConn(string step)
        {
        }
        private static void WatchFail(string msg)
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InputConnectionString -->
        /// <summary>
        /// 
        /// </summary>
        public static string InputConnectionString { get
        {
            return InfoAspect.GetConnectionString(ConfigurationManager.ConnectionStrings, "InputConnection");
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- OutputConnectionString -->
        /// <summary>
        /// 
        /// </summary>
        public static string OutputConnectionString { get
        {
            return InfoAspect.GetConnectionString(ConfigurationManager.ConnectionStrings, "OutputConnection");
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetConnectionString -->
        /// <summary>
        ///      Returns a connection string given its name
        /// </summary>
        /// <param name="connectionStringSettingsCollection"></param>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public static string GetConnectionString(ConnectionStringSettingsCollection connectionStringSettingsCollection
            , string connectionStringName)
        {
            string str = "";
            try
            {
                ConnectionStringSettingsCollection list = connectionStringSettingsCollection;
                Dictionary<string, string> conn = new Dictionary<string, string>(list.Count);
                foreach (ConnectionStringSettings item in list)
                {
                    conn.Add(item.Name, item.ConnectionString);
                }
                str = conn[connectionStringName];
            }
            catch (Exception ex)
            {
                throw new Exception("Connection string '"+connectionStringName+"' not found in config file.", ex);
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Dispose -->
        /// <summary>
        ///      Disposes the aspect's connection
        /// </summary>
        public void Dispose()
        {
            if (_outputConnection != null && _outputConnection.State == ConnectionState.Open)
                Close(_outputConnection);
            _outputConnection.Dispose();
            if (_inputConnection != null && _inputConnection.State == ConnectionState.Open)
                Close(_inputConnection);
            _inputConnection.Dispose();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Enter -->
        /// <summary>
        ///      To enter a method to measure its performance, must be followed by aspect--
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [DebuggerStepThrough()] public InfoAspect Enter(string className, string methodName     ) { Push(className+"."+methodName); return this; }
      //[DebuggerStepThrough()] public Aspect Enter([CallerMemberName]string methodName = "") { Push(methodName              ); return this; }
        [DebuggerStepThrough()] public InfoAspect Enter() { Push("unknown methodname " + this.Level); return this; }

        // ----------------------------------------------------------------------------------------
        /// <!-- Measuring -->
        /// <summary>
        ///      Run before Measured with the same parameter
        /// </summary>
        /// <param name="method"></param>
        /// <remarks>ABCDEFGHIJKLMNOP</remarks>
        public static void Measuring(string method)
        {
            try
            {
                if (Watch == null)
                    Watch = new Dictionary<string, Stopwatch>();
                if (Watch.ContainsKey(method))
                {
                    Watch[method].Reset();
                    Watch[method].Start();
                }
                else
                {
                    Watch.Add(method, new Stopwatch());
                    Watch[method].Start();
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Measured -->
        /// <summary>
        ///      Run after Measuring with the same parameter
        /// </summary>
        /// <param name="method"></param>
        public static void Measured(string method)
        {
            if (method == "") return;
            try
            {
                Watch[method].Stop();

                if (Performance == null)
                    Performance = new Dictionary<string,long>();
                if (!Performance.ContainsKey(method))
                    Performance.Add(method,0);
                Performance[method] += Watch[method].Elapsed.Ticks;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Open -->
        /// <summary>
        ///      Opens the aspect's connection
        /// </summary>
        public void Open()
        {
            Open(InputConnection );
            Open(OutputConnection);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pause -->
        /// <summary>
        ///      The pause that refreshes
        /// </summary>
        private static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pop -->
        /// <summary>
        ///      Pops a method name off the performance stack etc.
        /// </summary>
        private void Pop()
        {
            if (OutputConnection != null && OutputConnection.State == ConnectionState.Closed)
            {
                throw new Exception(TopMethod + " - Output Connection is closed");
            }
            if (InputConnection != null && InputConnection.State == ConnectionState.Closed)
            {
                throw new Exception(TopMethod + " - Input Connection is closed");
            }

            if (MeasurePerformance)
            {
                InfoAspect.Measured(TopMethod);
                Level--;
                if (Method.Count > 0)
                    Method.RemoveAt(Method.Count - 1); // pop the stack
                else
                    Pause();
            }

            if (Level != Method.Count+1)
                Pause();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Push -->
        /// <summary>
        ///      Pushes a new method name onto the performance stack and checks some anomalies
        /// </summary>
        /// <param name="methodName"></param>
        [DebuggerStepThrough()]
        private void Push(string methodName)
        {
            if (OutputConnection != null && OutputConnection.State == ConnectionState.Closed)
            {
                throw new Exception(TopMethod + " - Output Connection is closed");
            }
            if (InputConnection != null && InputConnection.State == ConnectionState.Closed)
            {
                throw new Exception(TopMethod + " - Input Connection is closed");
            }

            if (MeasurePerformance)
            {
                Method.Add(methodName); // push the stack
                InfoAspect.Measuring(methodName);
                Level++;
            }
            if (Level != Method.Count+1)
                Pause();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Rethrow -->
        /// <summary>
        ///      Rethrows taking the Aspect performance stack into account,
        ///      use this in methods called with aspect.Enter() instead of throw
        /// </summary>
        /// <param name="ex"></param>
        public void Rethrow(Exception ex)
        {
            string methodName = TopMethod;
            Pop();
            throw new Exception(methodName + " - " + ex.Message, ex);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TopMethod -->
        /// <summary>
        ///      Returns the top method on the performance stack
        /// </summary>
        public string TopMethod
        {
            get
            {
                if (Method.Count > 0)
                    return Method[Method.Count - 1];
                else
                {
                    return "";
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TopMethod + ":" + (Level-2) + "/" + (Method.Count-1);
        }
    }
}