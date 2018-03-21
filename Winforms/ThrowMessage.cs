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
using InformationLib.Endemes ;        // for Endeme(4)
using InformationLib.HardData;        // for Open()
using InformationLib.Strings ;        // for Open()
using System;                         // for Exception
using System.Collections.Generic;     // for List<>
using System.Configuration;           // for ConnectionStringSettings etc.
using System.Management;  // add reference // for ManagementObject
using System.Data;                    // for commandType, DataException
using System.Data.Common;             // for DataReaderAdapter
using System.Data.SqlClient;          // for SqlCommand(2), SqlConnection(1)
using System.Diagnostics;             // for EventLog, EventLogEntryType, EventSourceCreationData
using System.Net;                     // for Dns
using System.Text;                    // for ASCIIEncoding
using System.Text.RegularExpressions; // for Regex
using System.Windows.Forms;           // for MessageBox

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ThrowMessage -->
    /// <summary>
    ///      WARNING: This may be deprecated code
    ///      
    ///      The Throw class manages the actions taken on an exception,
    /// 
    ///      WARNING: Set these members before Throw is called the first time:
    ///        1. DatabaseLogConn  - connection to log to database
    ///        2. EventSourceBuild - Windows event log source format
    ///        3. ExceptionActions - What to do on an exception
    /// </summary>
    /// <remarks>
    ///      DON'T REFACTOR!
    ///        The Throw class is stand-alone (except it uses the Endeme classes), therefore it has
    ///        cut-and-paste code from elsewhere, don't refactor in such a way as to create any
    ///        more dependencies
    /// 
    ///      EXAMPLE:
    ///        Throw.DatabaseLogConn = "CodeWhiteDatabaseConnection"; // for bug logging
    ///        Throw.EventSourceBuild("CodeWhite", "CodeWhiteLog"); // for Windows event logging
    /// 
    ///        Endeme onExceptions = ""
    ///            + ExceptionAction.Squawk
    ///            + ExceptionAction.Gat
    ///            + ExceptionAction.EventLog
    ///            + ExceptionAction.DataLog;
    ///        Throw.DefaultApplicationExceptionAction = onExceptions;
    /// 
    ///      DESIGN:
    ///        Throw is designed for the convenience of the caller as a simple replacement for 'throw',
    ///        therefore it has to be static,
    ///      
    /// 
    ///      alpha code
    /// </remarks>
    public static class ThrowMessage
    {
        // ----------------------------------------------------------------------------------------
        //  Data members
        // ----------------------------------------------------------------------------------------
        /// <summary>The connection to be used for the Log in the database</summary>
        public static string         DatabaseLogConn { get { return _databaseLogConn; } set { _databaseLogConn = value; } } private static string _databaseLogConn;  /// <summary>What to do if there is a bug</summary> 
        public static Endeme         ExceptionActions { get { return _actions; } set { _actions = value; } } private static Endeme _actions;         /// <summary>Store Gui Action Trace data here (I know, this should probably be an instantiatiable class)</summary>
        public static GuiActionTrace CurrentGat { get { return _trace; } set { _trace = value; } } private static GuiActionTrace _trace;
        public static EndemeSet      ExceptionAct { get { if (_exceptionAct ==null) _exceptionAct = new EndemeSet(); return _exceptionAct; } }   private static EndemeSet _exceptionAct;

        /// <summary>Windows event logs prefer this format for source of event data</summary>
        private static EventSourceCreationData _eventSource;
        private static string _lastMessage;
        private static string _prevMessage;


        // ----------------------------------------------------------------------------------------
        /// <!-- AppConfigConnectionString -->
        /// <summary>
        ///      Returns the desired connection string from the app.config file
        /// </summary>
        /// <remarks>
        ///      This is cut-and-pasted from DataAtomLib.Data.ConnectSource so that
        ///      DataAtomLib.Data can use Throw without bi-directional dependency
        /// </remarks>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static string AppConfigConnectionString(string connectionStringName)
        {
            // --------------------------------------------------------------------------
            //  Try 100 times to get the connection string array from app.config
            // --------------------------------------------------------------------------
            ConnectionStringSettingsCollection arr = null;
            int i;
            for (i = 0; (arr == null || arr.Count < 2) && i < 100; ++i)
                arr = ConfigurationManager.ConnectionStrings;
            if (arr == null || arr.Count < 2)
                throw new NoNullAllowedException("Configuration Collection not complete: " + arr.Count + " after " + i + " tries");


            // --------------------------------------------------------------------------
            //  Do more stuff
            // --------------------------------------------------------------------------
            ConnectionStringSettings settings = null;
            for (i = 0; settings == null && i < 100; ++i)
                settings = arr[connectionStringName];
            if (settings == null)
                throw new NoNullAllowedException("for some reason it could not access Connection Settings: " + arr.Count
                    + "\r\n" + " maybe the wrong connection name was supplied");


            string connectionStr = settings.ConnectionString;
            return connectionStr;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ApplicationLog_Insert -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="ips">the user's ip address(es)</param>
        /// <param name="macs">a list of the user's mac addresses</param>
        /// <param name="msg">The message itself</param>
        /// <param name="gat">the Gui Action Trace (literal or symbolic)</param>
        /// <returns></returns>
        private static Guid ApplicationLog_Insert(string app, string userid, string ips, string macs, string msg, string gat)
        {
            // --------------------------------------------------------------------------
            //  Preprocess the data
            // --------------------------------------------------------------------------
            if (string.IsNullOrEmpty(_databaseLogConn))
                throw new NoNullAllowedException("You Must make sure"
                    + " a db connection or connection name is stored in the Throw class"
                    + " before trying to log messages to a database!");
            msg = Regex.Replace(msg, "'", "''");
            gat = Regex.Replace(gat, "'", "''");
            Guid guid = Guid.NewGuid();


            // --------------------------------------------------------------------------
            //  Build the SQL command
            // --------------------------------------------------------------------------
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO CodeWhite.ApplicationLog"
                + "        (_ID, Application, Userid, IpAddress, MacAddresses, EventMessage, GuiActionTrace)"
                + " VALUES ('" + guid + "', '" + app + "', '" + userid + "', '" + ips + "', '" + macs + "', '" + msg + "', '" + gat + "')";
            string conn = AppConfigConnectionString(_databaseLogConn);
            cmd.Connection = new SqlConnection(conn);


            try
            {
                // ----------------------------------------------------------------------
                //  Do the actual insert
                // ----------------------------------------------------------------------
                InData.Open(cmd.Connection);
                try { cmd.ExecuteNonQuery(); }
                catch (Exception ex) { throw new DataException("query did not run properly", ex); }
                finally { InData.Close(cmd.Connection); }
            }
            catch (Exception ex) { string error = ex.Message; }


            return guid;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ApplicationLog_Select -->
        /// <summary>
        ///      Pulls an item out of the database log (used for testing the class)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static DataTable ApplicationLog_Select(Guid guid)
        {
            // --------------------------------------------------------------------------
            //  Prepare command to read an item out of the app log in the database
            // --------------------------------------------------------------------------
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = new SqlConnection(AppConfigConnectionString(_databaseLogConn));
            cmd.CommandText = "SELECT *"
                + " FROM ADTHL7warehouseRT.CodeWhite.vApplicationLog"
                + " WHERE _ID = '" + guid.ToString() + "'";


            // --------------------------------------------------------------------------
            //  Read an item out of the app log in the database
            // --------------------------------------------------------------------------
            DataTable table = new DataTable();
            InData.Open(cmd.Connection);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr != null)
                try
                {
                    DataAdapter da = new DataAdapter();
                    table = new DataTable();
                    da.FillTable(table, dr);
                    //da.FillFromReader(_table, dr);
                    //dr.Dispose();
                    //dr = null;
                }
                catch { }

            InData.Close(cmd.Connection);

            return table;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- A -->
        /// <summary>
        ///      Provides a breakpoint in development and throws an exception in production
        /// </summary>
        /// <param name="e">exception</param>
        /// <param name="actions">bug action</param>
        /// <returns>The id in the database if sent to the database</returns>
        public static Guid A(Exception ex, Endeme actions, EventSourceCreationData eventSource
            , GuiActionTrace gat)
        {
            if (_exceptionAct == null)
                FillExceptionAct();


            // --------------------------------------------------------------------------
            //  Handle missing exception actions with a default Squawk-Log-Pause
            // --------------------------------------------------------------------------
            if (_actions == null)
                _actions = "" + Squawk + EventLog + Pauses;
            if (actions == null)
                actions = _actions;


            // --------------------------------------------------------------------------
            //  Handle missing event log operational data by complaining
            // --------------------------------------------------------------------------
            if (eventSource == null)
                throw new NoNullAllowedException("EventSourceCreationData is null!"
                    + "\r\n" + " You may have to add code something like this before running Throw:"
                    + "\r\n" + "    Throw.EventSourceBuild(\"MyProj\", \"MyProLog\");"
                    );


            Guid guid = Guid.Empty;
            string strGat = "";


            // --------------------------------------------------------------------------
            //   Prepare message text
            // --------------------------------------------------------------------------
            string msg = "";
            if (ex.InnerException != null)
                msg = ex.InnerException.GetType().ToString()
                    + "\r\n" + ex.InnerException.Message
                    + "\r\n"
                    + "\r\n" + ex.Message;
            else msg = ex.GetType().ToString() + " - " + ex.Message;


            // --------------------------------------------------------------------------
            //   Determine action level
            // --------------------------------------------------------------------------
            //int idx = actions.ToString().IndexOf('L');
            //if (idx < 0)  idx = 11;
            //int actionLevel;
            //if (idx >= 0)  actionLevel = 22 - idx;
            //else  actionLevel = 20;  // default action level, action level is a callable/programmable cutoff point
            //// N is a mandatory cutoff point


            // --------------------------------------------------------------------------
            //  This is how to do it with an endeme
            // --------------------------------------------------------------------------
            if (msg != _lastMessage  // don't keep spewing out the same messages
                && msg != _prevMessage
                && !Regex.IsMatch(msg, "Deadlock victim", RegexOptions.IgnoreCase))  // I don't want to hear about deadlock victims
            {
                char[] act = actions.ToCharArray();
                bool run = true;
                for (int i = 0; run && i < act.Length; ++i)
                {
                    switch (act[i])
                    {
                      //case DataLog: guid = SendToDatabase("CodeWhite", msg, strGat); break;
                        case EventLog: SendToActionLog(msg, eventSource, strGat); break;
                        case Ignore: break;
                        case Email: break;
                        case None: run = false; break;
                        case Pauses: Pause(); break; // set a breakpoint here
                        case Squawk: MessageBox.Show(msg); break;
                        case Throws: throw ex; // turn this on in production and make sure it's handled
                    }
                }
            }
            _prevMessage = _lastMessage;
            _lastMessage = msg;


            return guid;
        }
        public static Guid A(Exception ex, Endeme actions, GuiActionTrace gat) { return A(ex, actions, _eventSource, gat); } /// <param name="level">level of error: 18 if generally good for programmer warnings, 21 is good for critical user level errors</param>
        public static Guid A(Exception ex, GuiActionTrace gat) { return A(ex, _actions, _eventSource, gat); } /// <param name="level">level of error: 18 if generally good for programmer warnings, 21 is good for critical user level errors</param><param name="actions">D)atalog, E)ventlog, G)ATtrace, I)gnore, eM)ail, N)one, P)ause, S)quawk, T)hrows</param>
        public static Guid A(Exception ex, Endeme actions) { return A(ex, actions, _eventSource, _trace); }
        public static Guid A(Exception ex) { return A(ex, _actions, _eventSource, _trace); }


        // ----------------------------------------------------------------------------------------
        //  Constants useful for construction of the endemes
        // ----------------------------------------------------------------------------------------
        /// <summary>useful to start a sequence with</summary>
        public const string Sequence = "";   /// <summary> D. datalog  - log the exception to the database</summary>
        public const char DataLog = 'D';  /// <summary> E. eventlog - log the exception to the action log</summary>
        public const char EventLog = 'E';  /// <summary> G. Gat      - include a GAT literal trace with an error message to the database</summary>
        public const char Gat = 'G';  /// <summary> I. ignore   - just pass on through without indication of any problem</summary>
        public const char Ignore = 'I';  /// <summary> L. level    - level of messages to include: high:errors only, medium: warnings too, low:info messages too</summary>
        public const char Level = 'L';  /// <summary> M. email    - send email somewhere - not set up yet</summary>
        public const char Email = 'M';  /// <summary> N. none     - the default, ends the interpretation chain</summary>
        public const char None = 'N';  /// <summary> P. pause    - stop at a breakpoint</summary>
        public const char Pauses = 'P';  /// <summary> S. squawk   - display the exception in a message box</summary>
        public const char Squawk = 'S';  /// <summary> T. throws   - throw an exception</summary>
        public const char Throws = 'T';


        // ----------------------------------------------------------------------------------------
        /// <!-- ExceptionAction_old -->
        /// <summary>
        ///      Contains various actions you can take when there is an exception thrown using this endeme set:
        ///      D)datalog, E)eventlog, G)GATtrace, I)ignore, M)email, N)none, P)pause, S)squawk, T)throws
        /// </summary>
        /// <remarks>
        ///      A bitwise enum worked better than a regular enum since we might want to both squawk and email,
        ///      An endeme works better than an bitwise enum because we might want to do these in a particular order
        /// </remarks>
        private static void FillExceptionAct()
        {
            _exceptionAct.Set('D', "datalog", "log the event to the database");
            _exceptionAct.Set('E', "eventlog", "log the event to the event log");
            _exceptionAct.Set('G', "gat", "include a GAT literal trace with an error message to the database");
            _exceptionAct.Set('I', "ignore", "ignore the exception");
            _exceptionAct.Set('M', "email", "email someone about the exception");
            _exceptionAct.Set('L', "level", "level of messages to include: errors > warnings > info");
            _exceptionAct.Set('N', "none", "take no action (action chain ends here)");
            _exceptionAct.Set('P', "pause", "pause the program (if in debugger mode)");
            _exceptionAct.Set('S', "squawk", "send an error message to the user");
            _exceptionAct.Set('T', "throw", "throw the exception (which may crash the program)");
        }                                                                                                                                          /// <param name="actions">D)atalog, E)ventlog, G)ATtrace, I)gnore, eM)ail, N)one, P)ause, S)quawk, T)hrows</param>


        //// ------------------------------------------------------------------------------
        ////  #1  This is the way to do it with a regular enum
        //// ------------------------------------------------------------------------------
        ///switch (action)
        ///{
        ///    case ExceptAction.none   : MessageBox.Show("ExceptAction has not been set yet and already there is an error!"); break;
        ///    case ExceptAction.crash  : throw e;  // turn this on in production and make sure it's handled
        ///    case ExceptAction.email  :                                break;
        ///    case ExceptAction.ignore :                                break;
        ///    case ExceptAction.log    : LogTheError(errorMessage);     break;
        ///    case ExceptAction.pause  : BreakPoint();                  break;  // set a breakpoint here
        ///    case ExceptAction.squawk : MessageBox.Show(errorMessage); break;
        ///    case ExceptAction.log | ExceptAction.squawk :
        ///        MessageBox.Show(errorMessage);
        ///        LogTheError(errorMessage);
        ///        break;
        ///}


        //// --------------------------------------------------------------------------
        ////  #2  This was the way to do it with a bit-wise enum
        //// --------------------------------------------------------------------------
        ///if (0 != (action & ExceptAction.none))    {                                };
        ///if (0 != (action & ExceptAction.email))   {                                };
        ///if (0 != (action & ExceptAction.ignore))  {                                };
        ///if (0 != (action & ExceptAction.datalog)) { LogTheError(errorMessage);     };
        ///if (0 != (action & ExceptAction.pause))   { BreakPoint();                  };
        ///if (0 != (action & ExceptAction.squawk))  { MessageBox.Show(errorMessage); };
        ///if (0 != (action & ExceptAction.crash))   { throw e;                       }


        //// --------------------------------------------------------------------------
        ////  #3  This is the way to do it with an endeme
        //// --------------------------------------------------------------------------
        ///char[] act = action.ToCharArray();
        ///bool run = true;
        ///for (int i = 0; run && i < act.Length; ++i)
        ///{
        ///    switch (act[i])
        ///    {
        ///        case ExceptAction.None     : run = false;                   break;
        ///        case ExceptAction.DataLog  : SendToDatabase(errorMessage);  break;
        ///        case ExceptAction.EventLog : SendToActionLog(errorMessage); break;
        ///        case ExceptAction.Ignore   :                                break;
        ///        case ExceptAction.Email    :                                break;
        ///        case ExceptAction.Pause    : BreakPoint();                  break;  // set a breakpoint here
        ///        case ExceptAction.Squawk   : MessageBox.Show(errorMessage); break;
        ///        case ExceptAction.Throws   : throw e;                               // turn this on in production and make sure it's handled
        ///    }
        ///}


        // ----------------------------------------------------------------------------------------
        /// <!-- FullErrorText -->
        /// <summary>
        ///      Returns a report with as much information as it can find about the exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string FullErrorText(Exception inputEx, string xml)
        {
            Exception ex = inputEx;
            string errorMessage = "";


            // --------------------------------------------------------------------------
            //  For each level, add exception data to error string, prepare next level
            // --------------------------------------------------------------------------
            while (ex != null)
            {
                errorMessage += XmlErrorText(ex, xml) + "\r\n" + LevelErrorText(ex);
                ex = ex.InnerException;
            }
            errorMessage += "\r\n" + HeadErrorText(inputEx);


            return errorMessage;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- HeadErrorText -->
        /// <summary>
        ///      Returns information about an error that does not change with error level
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string HeadErrorText(Exception ex)
        {
            // --------------------------------------------------------------------------
            //  General information
            // --------------------------------------------------------------------------
            string text = "Source method:  " + ex.Source
                + "\r\n" + "Target method:  " + ex.TargetSite
                + "\r\n" + "Stack Trace:\r\n" + ex.StackTrace
                + "\r\n" + "Help Link:      " + ex.HelpLink;
            return text;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- IfNotMatch -->
        /// <summary>
        ///      Throws if the string does not match the patter and returns a boolean
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IfNotMatch(string str, string pattern, string message)
        {
            bool notMatch = true;
            if (!Regex.IsMatch(str, pattern))
                ThrowMessage.A(new FormatException(message + ": " + str));
            else notMatch = false;
            return notMatch;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- IfNull -->
        /// <summary>
        ///      Throws if the object is null and returns a boolean
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <returns>returns true if the object is null</returns>
        public static bool IfNull(object obj, string message)
        {
            bool isNull = true;
            if (obj == null)
                ThrowMessage.A(new NoNullAllowedException(message));
            else isNull = false;
            return isNull;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- IfNullOrEmpty -->
        /// <summary>
        ///      Throws if the string is null or empty and returns a boolean
        /// </summary>
        /// <param name="str"></param>
        /// <param name="message">returns true if the string is null or empty</param>
        public static bool IfNullOrEmpty(string str, string message)
        {
            bool isBlank = true;
            if (string.IsNullOrEmpty(str))
                ThrowMessage.A(new NoNullAllowedException(message));
            else isBlank = false;
            return isBlank;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- IfTooLong -->
        /// <summary>
        ///      Throws an exception if the string is too long
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns>returns true if string is too long</returns>
        public static bool IfTooLong(string str, int maxLength)
        {
            bool tooLong = true;
            if (str != null && str.Length > maxLength)
                ThrowMessage.A(new OverflowException("string too long: " + str.Length + ">" + maxLength));
            else tooLong = false;
            return tooLong;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- IfUnequal -->
        /// <summary>
        ///      Throws an exception if the two objects are not the same when they should be
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="descrA"></param>
        /// <param name="descrB"></param>
        /// <param name="place"></param>
        /// <returns>returns true if they are equal</returns>
        public static bool IfUnequal(object objA, object objB, string descrA, string descrB, string place)
        {
            string message = "";
            bool equal = (objA == objB);


            if (!equal)
            {
                // ----------------------------------------------------------------------
                //  Inequality message
                // ----------------------------------------------------------------------
                if (objA == null) message = descrA + " is null and " + descrB + " is not at " + place;
                else if (objB == null) message = descrB + " is null and " + descrA + " is not at " + place;
                else message = descrA + " does not match " + descrB + " at " + place + "."
                         + "\r\n'" + objA.ToString() + "' != '" + objB.ToString() + "'";
                ThrowMessage.A(new DataException(message));
            }


            return equal;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Integer -->
        /// <summary>
        ///      Takes a shot at converting what is sent to it to an integer
        /// </summary>
        /// <remarks>
        ///      WARNING: this has been cut and paste from the '_' class, the reason I did this is
        ///      to keep there from being a dependency between this class on the '_' class
        /// </remarks>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static int Integer(object obj, int defaultValue)
        {
            if (obj == null)
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
                if (int.TryParse(str, out value)) return value;


                return defaultValue;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- LevelErrorText -->
        /// <summary>
        ///      Returns the message from one exception internalization level
        /// </summary>
        /// <param name="e">exception</param>
        /// <returns></returns>
        private static string LevelErrorText(Exception ex)
        {
            string error = ex.GetType().ToString() + " - " + ex.Message;
            foreach (object key in ex.Data)
                error += ", " + key.ToString() + ":" + ex.Data[key].ToString();
            return error;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Line -->
        /// <summary>
        ///      Retrieves a line from a string
        /// </summary>
        /// <param name="num">line number</param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string Line(int num, string str)
        {
            if (num < 0 || string.IsNullOrEmpty(str)) return "";
            string[] separators = { "\r\n", "\r", "\n", }; // I know \n\r does not exist, but lets take it as a line divider anyway, took it out
            string[] lines = str.Split(separators, StringSplitOptions.None);
            if (num < lines.Length) return lines[num];
            else return "";
        }


        //// ----------------------------------------------------------------------------------------
        ///// <!-- LocalIps -->
        ///// <summary>
        /////      Returns the IP address(es) of the local machine
        /////      Using host name (local machine name), get the IP address list
        ///// </summary>
        ///// <returns></returns>
        //private static List<string> LocalIPs()
        //{
        //    List<string> ipList = new List<string>();
        //    IPHostEntry entry = Dns.GetHostByName(SetLocalMachineName());
        //    IPAddress[] addr = entry.AddressList;
        //    for (int i = 0; i < addr.Length; i++)
        //        ipList.Add(addr[i].ToString());
        //    return ipList;
        //}


        // ----------------------------------------------------------------------------------------
        /// <!-- LocalMacAddresses -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static List<string> LocalMacAddresses()
        {
            List<string> macList = new List<string>();
            System.Management.ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection moList = mc.GetInstances();
            foreach (ManagementObject mo in moList)
            {
                string macAddr = mo["MACAddress"] as string;
                if (macAddr != null && macAddr.Trim() != "")
                    macList.Add(macAddr);
            }
            return macList;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- LocalMachineName -->
        /// <summary>
        ///      First get the host name of local machine
        /// </summary>
        /// <returns></returns>
        public static string SetLocalMachineName()
        {
            string name = Dns.GetHostName();  // host name of local machine
            if (string.IsNullOrEmpty(name)) name = Environment.MachineName;
            if (string.IsNullOrEmpty(name)) name = SystemInformation.ComputerName;
            if (string.IsNullOrEmpty(name)) name = Environment.GetEnvironmentVariable("COMPUTERNAME");
            return name;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Pause -->
        /// <summary>
        ///      This is a no-op method to be used to provide a place to set breaks to
        /// </summary>
        private static void Pause()
        {
        }


        //// ----------------------------------------------------------------------------------------
        ///// <!-- ReadyCheck -->
        ///// <summary>
        /////      If you're tracing actions you need a place to store the traces during exceptions
        /////      (and therefore a source from which they came)
        ///// </summary>
        //public static void ReadyCheck()
        //{
        //    if (ThrowMessage._eventSource == null)
        //        throw new NoNullAllowedException("Throw.ApplicationEventSourceData is null!");
        //}


        // ----------------------------------------------------------------------------------------
        /// <!-- Section -->
        /// <summary>
        ///      returns a ranged section from a large string
        /// </summary>
        /// <param name="from">string in the format "(123,45)" where the 1st number is the line and the 2nd number is position</param>
        /// <param name="str"></param>
        /// <param name="width">how long a string to return</param>
        /// <param name="offsetBack">how much to back up from the position</param>
        /// <returns></returns>
        private static string Section(string from, string str, int width, int offsetBack)
        {
            // --------------------------------------------------------------------------
            //  Do stuff
            // --------------------------------------------------------------------------
            string strL = Regex.Replace(from, @"^[(](\d+), *\d+[)]$", "$1");
            int lineNum = Integer(strL, 0) - 1;
            if (lineNum == -1) lineNum = 0;
            if (lineNum < 0) return "";
            string line = Line(lineNum, str);


            // --------------------------------------------------------------------------
            //  Do stuff
            // --------------------------------------------------------------------------
            string strC = Regex.Replace(from, @"^[(]\d+, *(\d+)[)]$", "$1");
            int charNum = Math.Max(Integer(strC, 0) - offsetBack - 1, 0);
            string section = "";
            if (charNum < line.Length)
            {
                if (line.Length >= width)
                    section = line.Substring(charNum, width);
                else section = line.Substring(charNum);
            }


            return section;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SendToActionLog -->
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///      Regarding recording user names, I found this on the net at
        ///      http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/33c95c8c-9af2-44dd-9e12-f59915341718/:
        ///         Q: I'm having a problem with my code writing to the Application EventLog.  I have set the code up as described and its writting all the infomration i expect bar one very important bit.  The username of who was carring out the action that caused the problem.  i have found no overload of the Evenlog.Writeevent method that will alllow me to set the username  so  i assumed it would take the details of the logged in domain user.
        ///         A: The user data is normally only used in the security log (which you can no longer write to as of XP SP2).  The .NET implementation will never write the user name when you use the EventLog class to write an event because it passes null to the API.  If you want to generate the user information then you will have to use P/Invoke.  Be warned however that you will need to marshal the event log record structure and you'll need to retrieve and marshal the user's SID.
        ///         Q: By any chance do yo uhave an example of this ? 
        ///         A: No I don't.  But it should be pretty straightforward.  The hardest part will be converting the SID to an managed buffer that you can use.  You can derive from EventLog but this won't help you much in this case but it will provide the rest of the log functionality without rewriting any code.
        ///            It might be easier to write the code in unmanaged code and expose it through an easy to use API or use C++/CLI which allows you to expose it to managed code but use unmanaged code to write it.
        ///            Michael Taylor - 2/14/07
        /// </remarks>
        /// <param name="eventMessage"></param>
        /// <param name="eventId"></param>
        /// <param name="eventType"></param>
        /// <param name="eventSourceData"></param>
        private static void SendToActionLog(string eventMessage, int eventId
            , EventLogEntryType eventType, EventSourceCreationData eventSourceData, string gat)
        {
            if (eventSourceData == null)
                throw new NoNullAllowedException("eventSourceData is null!");
            try
            {
                if (gat.Length > 2042)
                    gat = "... " + gat.Substring(gat.Length - 2042);
                if (gat.Length > 0)
                    gat = "\r\n" + gat;
                //byte[] arr = (new ASCIIEncoding()).GetBytes(gat);
                //string str = (new ASCIIEncoding()).GetString(arr);


                //Service to open and search event viewer - bytes
                //Environment.UserName;
                string source = eventSourceData.Source;
                //if (!EventLog.SourceExists(source, eventSourceData.MachineName)) // put on workstation
                //    EventLog.CreateEventSource(eventSourceData); // Requested registry access is not allowed
                //if (!EventLog.SourceExists(source))
                //    EventLog.CreateEventSource(source, eventSourceData.LogName);
                EventLog log = new EventLog();
                log.Source = source;
                log.WriteEntry(eventMessage + "\r\n" + Environment.UserName + gat
                    , eventType, eventId, (short)1);


                //EventLog myLog = new EventLog("CodeWhiteLog");
                //foreach (System.Diagnostics.EventLogEntry entry in myLog.Entries)
                //{
                //    string msg = entry.Message;
                //    string usr = entry.UserName;
                //}
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }
        private static void SendToActionLog(string eventMessage, EventSourceCreationData source)
        { SendToActionLog(eventMessage, 1000, EventLogEntryType.Error, source, ""); }
        private static void SendToActionLog(string eventMessage)
        { SendToActionLog(eventMessage, 1000, EventLogEntryType.Error, _eventSource, ""); }
        private static void SendToActionLog(string eventMessage, EventSourceCreationData source, string strGat)
        { SendToActionLog(eventMessage, 1000, EventLogEntryType.Error, _eventSource, strGat); }


        //// ----------------------------------------------------------------------------------------
        ///// <!-- SendToDatabase -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="errorMessage"></param>
        //private static Guid SendToDatabase(string app, string msg, string gat)
        //{
        //    Guid guid = ApplicationLog_Insert(app
        //        , Environment.UserName
        //        , Collate(LocalIPs(), ", ")
        //        , Collate(LocalMacAddresses(), ", ")
        //        , msg
        //        , gat);
        //    return guid;
        //}
        /// <summary>
        ///  This is cut and paste from _, don't refactor
        /// </summary>
        public static string Collate(List<string> list, string delim)
        {
            StringBuilder str = new StringBuilder();
            string d = "";
            foreach (string s in list) { str.Append(d + s); d = delim; }
            return str.ToString();
        }


        // --------------------------------------------------------------------------------------------
        /// <!-- DataAdapter -->
        public class DataAdapter : DbDataAdapter
        {
            public int FillTable(DataTable table, IDataReader rdr)
            {
                if (rdr == null)
                    throw new NoNullAllowedException("FillTable error - dataReader is null");
                return this.Fill(table, rdr);
            }
            protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) { return null; }
            protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) { return null; }
            protected override void OnRowUpdated(RowUpdatedEventArgs value) { }
            protected override void OnRowUpdating(RowUpdatingEventArgs value) { }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- XmlErrorText -->
        /// <summary>
        ///      Exception data for a particular exception level
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="e">exception</param>
        /// <returns></returns>
        private static string XmlErrorText(Exception ex, string xml)
        {
            string error = "";
            if (Regex.IsMatch(ex.Message, @"^There is an error in XML document \("))
            {
                string at = Regex.Replace(ex.Message, @"^.*([(]\d+, ?\d+[)]).*$", "$1");
                error += "\r\nError near:  "
                    + " '" + Regex.Replace(Section(at, xml, 66, 66), "^.*(<.*>.*<.*>)<?$", "$1") + "'";
            }
            return error;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- EventSourceBuild -->
        /// <summary>
        ///      Builds the event source creation data for logging using source and log names
        /// </summary>
        /// <param name="source">often a project name</param>
        /// <param name="logName"></param>
        /// <returns>constructed event source</returns>
        public static EventSourceCreationData EventSourceBuild(string source, string logName)
        {
            EventSourceCreationData eventSource;
            eventSource = new EventSourceCreationData(source, logName);
            eventSource.MachineName = ThrowMessage.SetLocalMachineName(); // local machine
            _eventSource = eventSource;
            return eventSource;
        }
    }
}
