//-------------------------------------------------------------------------------------------------
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
//-------------------------------------------------------------------------------------------------
using InformationLib.Endemes;         // for 
using InformationLib.Testing;         // for Is.Trash
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Data;                    // for 
using System.Diagnostics;             // for 
using System.Configuration;           // for ConnectionStringSettings etc.
using System.Management;   // [ADD REFERENCE] ' for ManagementObject // for commandType, DataException
using System.Data.Common;             // for DataReaderAdapter
using System.Data.SqlClient;          // for SqlCommand, SqlConnection // for EventLog, EventLogEntryType, EventSourceCreationData
using System.Net;                     // for Dns
using System.Reflection;              // for 
using System.Text;                    // for ASCIIEncoding
using System.Text.RegularExpressions; // for Regex

// --------------------------------------------------------------------------
//  For various UI's
// --------------------------------------------------------------------------
// [ADD REFERENCE - System.Windows.Forms]         ' for MessageBox, SystemInformation
// [ADD REFERENCE - System.Web]                   ' for Page, MasterPage

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Throws -->
    /// <summary>
    ///      The Throws class manages the actions taken on an exception,
    /// 
    ///      WARNING: You might want to set these members before Throws is called the first time:
    ///        1. DatabaseLogConn  - connection to log to database   ' for bug logging
    ///        2. EventSourceBuild - Windows event log source format ' for Windows event logging
    ///        3. DefaultActions   - What to do on an exception
    /// </summary>
    /// <remarks>
    ///      DON'T REFACTOR!
    ///        The Throws class is stand-alone (except it uses the Endeme classes), therefore it has
    ///        cut-and-paste code from elsewhere, please don't refactor out the cut and paste code
    ///        in such a way as to create any more dependencies
    /// 
    ///      INITIALIZATION EXAMPLES:
    ///        Throws.DatabaseLogConn = "ComputerSupportDatabaseConnection"  ' for bug logging in database
    ///        Throws.EventSourceBuild("ComputerSupport", "ComputerSupportLog")    ' for Windows event logging
    ///        Endeme onExceptions = "SGLD"
    ///        Throws.DefaultActions = onExceptions
    ///        Throws.WebPage = Me
    /// 
    ///            OR
    /// 
    ///        If Throws.EventLog Is Nothing Then
    ///            Throws.Activate("ComputerSupport", "ComputerSupportLog", Me)
    ///        End If
    /// 
    ///      DESIGN:
    ///        Throws is designed for the convenience of the caller as a simple replacement for 'Throw',
    ///        therefore it has to be Shared, It is a good candidate for the singleton pattern if it
    ///        ever needs to get somewhat dynamic
    /// </remarks>
    public static class Throws
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Pause -->
        /// <summary>
        ///      This is a no-op method to be used to provide a place to set break points
        /// </summary>
        private static void Pause()
        {
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Actions -->
        /// <summary>
        ///      A)lert,C)ommon,D)atalog,E)mail,F)ill,G)AT,I)nternal,J)unk/ignore,L)og event,O)utput window,P)ause,Q)uit,R)ethrow,S)quawk,T)race,U)nique
        /// </summary>
        /// <remarks>production ready, TODO: add try/catch block to make this thread safe</remarks>
        public static EndemeSet Actions { get
        {   if ((                _actions == null))                                                                                                  //ABCDEFGHIJKLMNOPQRSTUV|
            { try {              _actions = new EndemeSet("Exception Action", true);                                                                 //----------------------+----------------------------------------------------
                                 _actions.Add('A', "Alert"   , "alert the support staff about the problem right now"                             );  //A]     -I-       -S-  |A. Alert    *helpdesk
                                 _actions.Add('B', "Bug"     , "continue the program, displaying a bug as in an 'Assert.That' statement"         );  //AB]D              -T- |B. Bug      Debug, Test, Assert, Equals (does this make any sense?, probably remove it)
                                 _actions.Add('C', "Common"  , "uses the default action list rather than the one passed to Throws.A"             );  // [C]D-       N   -S-  |C. Common   Normal, Standard, Default
                                 _actions.Add('D', "Datalog" , "log the event to the database"                                                   );  //  [D]  -I--L-         |D. Datalog  *Log to database, Insert
                                 _actions.Add('E', "Email"   , "email someone about the event/exception"                                         );  //   [E]     -M-        |E. Email    Mail
                                 _actions.Add('F', "Fill"    , "fill out the exception with a stack trace and source if there is not already one");  // -C-[F]          -S-  |F. Fill     *Source & Stack
                                 _actions.Add('G', "GAT"     , "include a literal 'GUI Action Trace' (GAT) with the error message"               );  //     [G]          -TU-|G. GAT      UI actions, Trace
                                 _actions.Add('H', "Here"    , "log the errors into a list internally here inside Throws"                        );  //      [H]I-L-     -T- |H. Here     compile errors here in Throws
                                 _actions.Add('I', "Internal", "log the errors into a list internally here inside Throws"                        );  //     -H[I] L-     -T- |I. Internal *compile errors here in Throws
                                 _actions.Add('J', "Junk"    , "ignore the event/exception entirely (prevents any action at all)"                );  //  -D- -I[J]   -P--S-  |J. Junk     *Ignore, Dump, Prevent, Scuttle
                                 _actions.Add('L', "Log"     , "log the exception to the Windows event log"                                      );  //   -E-    [L]         |L. Log      *Event
                                 _actions.Add('O', "Output"  , "send mesage to Visual Studio Output window"                                      );  //             [O]    -V|O. Ouput    Visual Studio output window
                                 _actions.Add('P', "Pause"   , "pause the program (if in debugger mode and you have set the breakpoint)"         );  //              [P]     |P. Pause    *the program
                                 _actions.Add('Q', "Quit"    , "take no further action (action chain ends here)"                                 );  //  -DE-      -N-[Q]S-  |Q. Quit     end, none, stop
                                 _actions.Add('R', "Rethrow" , "rethrow the exception (which may crash the program) preserving the stack trace"  );  //                [R]T- |R. Rethrow  *Throw
                                 _actions.Add('S', "Squawk"  , "send an error message to the user in some sort of pop-up"                        );  // -C-E-     -M--P-[S]U-|S. Squawk   *User, send popup to user no repeats
                                 _actions.Add('T', "Trace"   , "add a stack trace to the error message"                                          );  //                -S[T] |T. Trace    Stack trace
                                 _actions.Add('U', "Unique"  , "terminate the action string here if the error text is repeated"                  );  //            -N-    [U]|U. Unique   message, no repeats
            } catch { } } return _actions; } }                                                                                                       //----------------------+----------------------------------------------------
        private static EndemeSet _actions = null;                                                                                                    //ABCDEFGHIJKLMNOPQRSTUV|
        /// <summary>Store Gui Action Trace data here (I know, this should probably be an instantiatiable class)</summary>


        // ----------------------------------------------------------------------------------------
        //  Data members
        // ----------------------------------------------------------------------------------------
        /// <summary>The connection to be used for the Log in the database</summary>
        public static string DatabaseLogConn  { get { return _databaseLogConn; } set { _databaseLogConn = value; } } private static string _databaseLogConn; /// <summary>The table into which the logged errors are put</summary>
        public static string DatabaseLogTable { get; set; }  /// <summary>What to do if there is a bug</summary>
        public static Endeme DefaultActions   { get { return _defaultActions ; } set { _defaultActions  = value; } } private static Endeme _defaultActions ; /// <summary>local list of the errors kept here in Throws</summary>
        private static List<string> ErrorList { get; set; }


        // ----------------------------------------------------------------------------------------
        /// <!-- EventSource -->
        /// <summary>
        ///      Windows event logs prefer this format for source of event data
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static EventSourceCreationData EventLog
        {
            get { return _eventSource; }
        }

        private static EventSourceCreationData _eventSource;

        // ----------------------------------------------------------------------------------------
        /// <!-- OldMessages -->
        /// <summary>
        ///      View, clear or modify old message
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<string> OldMessages
        {
            get { return _oldMessages; }
        }

        private static List<string> _oldMessages;

        // ----------------------------------------------------------------------------------------
        /// <!-- A -->
        /// <summary>
        ///      Provides a breakpoint in development and throws an exception in production
        /// </summary>
        /// <param name="ex">exception</param>
        /// <param name="actions">bug action</param>
        /// <returns>The id in the database if sent to the database</returns>
        /// <remarks>production ready</remarks>
        private static Guid A(Exception ex, EndemeSet eSet, Endeme actions, EventSourceCreationData eventSource)
        {

            // --------------------------------------------------------------------------
            //   Prepare various things
            // --------------------------------------------------------------------------
            if ((_oldMessages == null))
            {
                _oldMessages = new List<string>();
            }
            actions = Insure(actions);
            string strGat = "";
            string msg = PrepareMessage(ex);


            // --------------------------------------------------------------------------
            //  8153 is usu. a normal condition regarding including nulls in SQL agregates
            //  I also don't want to hear about deadlock victims
            //  Also, if the actions endeme says ignore the exception, then ignore it
            // --------------------------------------------------------------------------
            Guid dbId = Guid.Empty;
            if (Regex.IsMatch(ex.Message, "(8153|Deadlock victim)", RegexOptions.IgnoreCase))
            {
                return dbId;
            }


            // --------------------------------------------------------------------------
            //  Meta-characteristics
            // --------------------------------------------------------------------------
            if (actions.Contains('J'))
            {
                return dbId;
            }
            if (actions.Contains('C'))
            {
                actions = _defaultActions;
            }


            // --------------------------------------------------------------------------
            //  This is how to do it with an endeme
            // --------------------------------------------------------------------------
            char[] act = actions.ToCharArray();
            bool run = true;
            int i = 0;
            while (run && i < act.Length)
            {
                switch (act[i])
                {
                    case 'A': SendAlert(ex, PageName);                         break; // send alert to support staff
                    case 'D': dbId = SendToDatabase(msg, "", "", "");          break; // TODO: rebuild, log the exception in a database table
                    case 'E': SendAlert(ex, msg);                              break; // send email, currently uses the same code as alert
                    case 'F': ex = FillOutException(ex, PageName);             break; // fills in missing members (stack trace) in the exception object
                    case 'G': strGat = "";                                     break; // TODO: needs to be rebuilt, include a GAT with a message
                    case 'H': ErrorList.Add(msg);                              break; // here: log the errors here in Throws
                    case 'I': ErrorList.Add(msg);                              break; // internal: log the errors here in Throws
                    case 'J': run = false;                                     break; // junk/ignore the exception, you're done handling the error
                    case 'L': if (eventSource != null) LogToActionLog(msg, eventSource, strGat);  break; // Log event to windows event log
                    case 'O': WriteToDebugWindow(msg);                         break; // Write to visual studio output window
                    case 'P': Pause();                                         break; // useful for development, set a breakpoint in Pause()
                    case 'Q': run = false;                                     break; // quit the error handling sequence, you're done handling the error
                    case 'R': Type ForExceptionType = typeof(System.Exception);       // I got this off the net, supposedly it         
                        BindingFlags flags = default(BindingFlags);                   // preserves the stack trace during a 'Throw ex',
                        flags = BindingFlags.Instance | BindingFlags.NonPublic;       // In theory you are never supposed to use       
                        ForExceptionType.GetMethod("InternalPreserveStackTrace", flags).Invoke(ex, null); // 'Throw ex' because it dumps the stack trace,  
                        throw ex;                                                     // but since this whole class is based on        
                                                                                      // 'Throw ex', the stack trace must be managed   
                    case 'S': Squawk(msg);                                     break; // MessageBox.Show(msg) or something
                    case 'T': msg = msg + Environment.StackTrace;              break; // include a stack trace with a messsage
                    case 'U': if (_oldMessages.Contains(msg)) { run = false; } break; // quit handling an error with an identical message to a previous one
                }
                i += 1;
            }

            _oldMessages.Add(msg);

            return dbId;
        }
        public static Guid A(Exception ex, EndemeSet eSet, Endeme actions             ) {                                                                                       return A(ex, eSet, actions, _eventSource); }
        public static Guid A(Exception ex, EndemeSet eSet, Endeme actions, InfoAspect with) { if (_eventSource == null) Activate(with.Project, with.Project+"Log", with.Sender, 0); return A(ex, eSet, actions, _eventSource); }


        // -----------------------------------------------------------------------------------------
        /// <!-- Activate -->
        /// <summary>
        ///      Prepares Throws and related systems for handling and logging exceptions
        /// </summary>
        /// <param name="source">usually a project name</param>
        /// <param name="eventLogName">a related log name</param>
        /// <param name="sender">you should usually pass 'Me' or 'this'</param>
        /// <param name="strAppIdNumber"></param>
        /// <returns>constructed event source</returns>
        /// <remarks>production ready</remarks>
        public static EventSourceCreationData Activate(string source, string eventLogName, object sender, object strAppIdNumber)
        {

            // --------------------------------------------------------------------------
            //  Prepare Throws to send popup messages to the user
            // --------------------------------------------------------------------------
            Type pageType = sender.GetType();
            //if (sender as System.Web.UI.MasterPage != null)
            //{
            //    MastPage = sender;
            //}
            //else if (sender as System.Web.UI.Page != null)
            //{
            //    WebPage = sender;
            //}
            //else if (sender as System.Windows.Forms.Form != null)
            //{
                FormPage = (System.Windows.Forms.Form)sender;
            //}
            //else if (sender as System.Windows.Controls.Page != null)
            //{
            //    WpfPage = sender;
            //}


            // --------------------------------------------------------------------------
            //  Prepare the a link to the windows event log for use
            // --------------------------------------------------------------------------
            EventSourceCreationData eventSource = null;
            eventSource = new EventSourceCreationData(source, eventLogName);
            eventSource.MachineName = Throws.SetLocalMachineName();
            // local machine
            _eventSource = eventSource;


            // --------------------------------------------------------------------------
            //  Prepare the database for use - TODO: write me
            // --------------------------------------------------------------------------
            ErrorList = new List<string>();


            // --------------------------------------------------------------------------
            //  Prepare an application manager instance for use
            // --------------------------------------------------------------------------
            //int appId = int.Parse(strAppIdNumber);
            try
            {
                //appmgr = new ER.ApplicationManagement(appId);
            }
            catch (Exception ex)
            {
                Throws.A(new AccessViolationException("The first time this happened it was because our rpgiis/erwebservices/AppMgmntWebSvc.asmx web service was down"
                    + "\r\n" + "the second time was when one of the C:\\ConfigurationStore directories was missing", ex), Throws.Actions, "SPA");
            }


            return eventSource;

        }


        // -----------------------------------------------------------------------------------------
        /// <!-- AppConfigConnectionString -->
        /// <summary>
        ///      Returns the desired connection string from the app.config file
        /// </summary>
        /// <remarks>
        ///      This is cut-and-pasted from DataAtomLib.Data.ConnectSource so that
        ///      DataAtomLib.Data can use Throw without bi-directional dependency
        /// </remarks>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private static string AppConfigConnectionString(string connectionStringName)
        {
            // --------------------------------------------------------------------------
            //  Try 100 times to get the connection string array from app.config
            // --------------------------------------------------------------------------
            ConnectionStringSettingsCollection arr = null;
            int i = 0;
            i = 0;
            while ((arr == null || arr.Count < 2) && i < 100)
            {
                arr = ConfigurationManager.ConnectionStrings;
                i += 1;
            }
            if (arr == null || arr.Count < 2)
            {
                throw new NoNullAllowedException("Configuration Collection not complete: " + arr.Count + " after " + i + " tries");
            }


            // --------------------------------------------------------------------------
            //  Do more stuff
            // --------------------------------------------------------------------------
            ConnectionStringSettings settings = null;
            i = 0;
            while (settings == null && i < 100)
            {
                settings = arr[connectionStringName];
                i += 1;
            }
            if (settings == null)
            {
                throw new NoNullAllowedException("for some reason it could not access Connection Settings: " + arr.Count + "\r\n" + " maybe the wrong connection name was supplied");
            }


            string connectionStr = settings.ConnectionString;
            return connectionStr;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- ApplicationLog_Insert -->
        /// <summary>
        ///      Inserts the error into the application log
        /// </summary>
        /// <param name="name">Name of the page, source, or application</param>
        /// <param name="userid"></param>
        /// <param name="ips">the user's ip address(es)</param>
        /// <param name="macs">a list of the user's mac addresses</param>
        /// <param name="msg">The message itself</param>
        /// <param name="gat">the Gui Action Trace (literal or symbolic)</param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private static Guid ApplicationLog_Insert(string name, string userid, string ips, string macs, string msg, string gat)
        {
            // --------------------------------------------------------------------------
            //  Preprocess the data
            // --------------------------------------------------------------------------
            if (string.IsNullOrEmpty(_databaseLogConn))
            {
                throw new NoNullAllowedException("You Must make sure" + " a db connection or connection name is stored in the Throw class" + " before trying to log messages to a database!");
            }
            msg = Regex.Replace(msg, "'", "''");
            gat = Regex.Replace(gat, "'", "''");
            Guid id = Guid.NewGuid();


            // --------------------------------------------------------------------------
            //  Build the SQL command
            // --------------------------------------------------------------------------
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO dbo.ApplicationLog" + "        (LogId, Application, UserId, IpAddress, MacAddresses, EventMessage, GuiActionTrace)" + " VALUES ('" + Convert.ToString(id) + "', '" + name + "', '" + userid + "', '" + ips + "', '" + macs + "', '" + msg + "', '" + gat + "')";
            string conn = AppConfigConnectionString(_databaseLogConn);
            cmd.Connection = new SqlConnection(conn);


            // --------------------------------------------------------------------------
            //  Attempt the insert
            // --------------------------------------------------------------------------
            if (!TryErrorInsert(cmd))
            {
                // -----------------------------------------------------------------------
                //  Construct the table if it does not yet exist
                // -----------------------------------------------------------------------
                //  TODO: write me
                //  CREATE TABLE ApplicationLog

                if (!TryErrorInsert(cmd))
                {
                    throw new DataException("The data logging insert query did not run properly");
                }
            }


            return id;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ApplicationLog_Select -->
        /// <summary>
        ///      Pulls an item out of the database log (used for testing the class)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public static DataTable ApplicationLog_Select(Guid guid)
        {
            // --------------------------------------------------------------------------
            //  Prepare command to read an item out of the app log in the database
            // --------------------------------------------------------------------------
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = new SqlConnection(AppConfigConnectionString(_databaseLogConn));
            cmd.CommandText = "SELECT *" + " FROM MyTable" + " WHERE _ID = '" + guid.ToString() + "'";


            // --------------------------------------------------------------------------
            //  Read an item out of the app log in the database
            // --------------------------------------------------------------------------
            DataTable table = new DataTable();
            Open(cmd.Connection);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr != null)
            {
                try
                {
                    DataAdapter da = new DataAdapter();
                    table = new DataTable();
                    //da.FillFromReader(_table, dr);
                    //dr.Dispose();
                    //dr = null;
                    da.FillTable(table, dr);
                }
                catch
                {
                }
            }

            Close(cmd.Connection);

            return table;
        }
        public static void Open (SqlConnection  connection) { try { WatchConn("Open" ); connection.Open (); } catch (Exception ex) { WatchFail(ex.Message); } }
        public static void Close(SqlConnection  connection) { try { WatchConn("Close"); connection.Close(); } catch (Exception ex) { WatchFail(ex.Message); } }
        private static void WatchConn(string step)
        {
        }
        private static void WatchFail(string msg)
        {
        }


        // ''' <param name="actions">D)atalog, E)ventlog, G)ATtrace, I)gnore, eM)ail, N)one, P)ause, S)quawk, T)hrows</param>

        // '''/ --------------------------------------------------------------------------
        // '''/  #1  This is the way to do it with a regular enum
        // '''/ --------------------------------------------------------------------------
        // '''switch (action)
        // '''{
        // '''    case ExceptAction.none   : MessageBox.Show("ExceptAction has not been set yet and already there is an error!"); break;
        // '''    case ExceptAction.crash  : throw e;  // turn this on in production and make sure it's handled
        // '''    case ExceptAction.email  :                                break;
        // '''    case ExceptAction.ignore :                                break;
        // '''    case ExceptAction.log    : LogTheError(errorMessage);     break;
        // '''    case ExceptAction.pause  : BreakPoint();                  break;  // set a breakpoint here
        // '''    case ExceptAction.squawk : MessageBox.Show(errorMessage); break;
        // '''    case ExceptAction.log | ExceptAction.squawk :
        // '''        MessageBox.Show(errorMessage);
        // '''        LogTheError(errorMessage);
        // '''        break;
        // '''}


        // '''/ --------------------------------------------------------------------------
        // '''/  #2  This was the way to do it with a bit-wise enum
        // '''/ --------------------------------------------------------------------------
        // '''if (0 != (action & ExceptAction.none))    {                                };
        // '''if (0 != (action & ExceptAction.email))   {                                };
        // '''if (0 != (action & ExceptAction.ignore))  {                                };
        // '''if (0 != (action & ExceptAction.datalog)) { LogTheError(errorMessage);     };
        // '''if (0 != (action & ExceptAction.pause))   { BreakPoint();                  };
        // '''if (0 != (action & ExceptAction.squawk))  { MessageBox.Show(errorMessage); };
        // '''if (0 != (action & ExceptAction.crash))   { throw e;                       }


        // '''/ --------------------------------------------------------------------------
        // '''/  #3  This is the way to do it with an endeme
        // '''/ --------------------------------------------------------------------------
        // '''char[] act = action.ToCharArray();
        // '''bool run = true;
        // '''for (int i = 0; run && i < act.Length; ++i)
        // '''{
        // '''    switch (act[i])
        // '''    {
        // '''        case ExceptAction.None     : run = false;                   break;
        // '''        case ExceptAction.DataLog  : SendToDatabase(errorMessage);  break;
        // '''        case ExceptAction.EventLog : SendToActionLog(errorMessage); break;
        // '''        case ExceptAction.Ignore   :                                break;
        // '''        case ExceptAction.Email    :                                break;
        // '''        case ExceptAction.Pause    : BreakPoint();                  break;  // set a breakpoint here
        // '''        case ExceptAction.Squawk   : MessageBox.Show(errorMessage); break;
        // '''        case ExceptAction.Throws   : throw e;                               // turn this on in production and make sure it's handled
        // '''    }
        // '''}


        // ----------------------------------------------------------------------------------------
        /// <!-- FillOutException -->
        /// <summary>
        ///      Fills out the exception members that are blank
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <remarks>alpha code</remarks>
        private static Exception FillOutException(Exception ex, string source)
        {

            // --------------------------------------------------------------------------
            //  If there is no Source, use the message
            // --------------------------------------------------------------------------
            if ((ex.Source == null))
            {
                ex.Source = source;
            }


            // --------------------------------------------------------------------------
            //  If the stack trace has been lost, use the current one
            // --------------------------------------------------------------------------
            if ((ex.StackTrace == null))
            {
                Type typ = typeof(System.Exception);
                //int privateInstance = BindingFlags.NonPublic | BindingFlags.Instance;
                //FieldInfo stackTraceField = ex.GetType().GetField("_remoteStackTraceString", privateInstance);
                //if ((stackTraceField != null))
                //{
                //    stackTraceField.SetValue(ex, Environment.StackTrace);
                //}
            }

            return ex;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FullErrorText -->
        /// <summary>
        ///      Returns a report with as much information as it can find about the exception
        /// </summary>
        /// <param name="inputEx"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
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
            string text = "Source method:  \r\nTarget method:  " + Convert.ToString(ex.TargetSite)
                + "\r\n" + "\r\nStack Trace:" + "\r\n" + ex.StackTrace + "\r\n" + "Help Link:      " + ex.HelpLink;
            return text;
        }


        //' ----------------------------------------------------------------------------------------
        // ''' <!-- IfEmpty -->
        // ''' <summary>
        // '''      Throws if the specified table is empty
        // ''' </summary>
        // ''' <param name="databaseName"></param>
        // ''' <param name="tableName"></param>
        // ''' <param name="conn"></param>
        //Public Shared Function IfEmpty(databaseName As String, tableName As String, conn As String) As Boolean
        //    Dim isEmpty As Boolean = True
        //    Dim cmd As New RichSqlCommand(CommandType.Text, conn, "SELECT count(*)" & " FROM " & databaseName & "." & tableName, Throws.Actions, "S")
        //    Dim num As Integer = CInt(cmd.ExecuteScalar(0))
        //    If num < 1 Then
        //        Throws.A(New DataException("Error - The " & tableName & " table is empty"), Throws.Actions, "P");
        //    Else
        //        isEmpty = False
        //    End If
        //    Return isEmpty
        //End Function

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
            {
                Throws.A(new FormatException(message + ": " + str), Throws.Actions, "P");
            }
            else
            {
                notMatch = false;
            }
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
        /// <remarks>alpha code</remarks>
        public static bool IfNull(object obj, string message)
        {
            bool isNull = true;
            if (obj == null)
            {
                Throws.A(new NoNullAllowedException(message), Throws.Actions, "P");
            }
            else
            {
                isNull = false;
            }
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
            {
                Throws.A(new NoNullAllowedException(message), Throws.Actions, "P");
            }
            else
            {
                isBlank = false;
            }
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
        /// <remarks>alpha code</remarks>
        public static bool IfTooLong(string str, int maxLength)
        {
            bool tooLong = true;
            if (str != null && str.Length > maxLength)
            {
                Throws.A(new OverflowException("string too long: " + str.Length + ">" + maxLength), Throws.Actions, "P");
            }
            else
            {
                tooLong = false;
            }
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
        /// <remarks>alpha code</remarks>
        public static bool IfUnequal(object objA, object objB, string descrA, string descrB, string place)
        {
            string message = "";
            bool equal = (object.ReferenceEquals(objA, objB));


            if (!equal)
            {
                // ----------------------------------------------------------------------
                //  Inequality message
                // ----------------------------------------------------------------------
                if (objA == null)      { message = descrA + " is null and " + descrB + " is not at " + place;                                                                       }
                else if (objB == null) { message = descrB + " is null and " + descrA + " is not at " + place;                                                                       }
                else                   { message = descrA + " does not match " + descrB + " at " + place + "." + "\r\n" + "'" + objA.ToString() + "' != '" + objB.ToString() + "'"; }
                Throws.A(new DataException(message), Throws.Actions, "P");
            }

            return equal;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- Increment -->
        /// <summary>
        ///      Increments the tally of an error message
        /// </summary>
        /// <param name="ErrorTally"></param>
        /// <param name="msg"></param>
        private static void Increment(Dictionary<string, int> dictionary, string msg)
        {
            if (!dictionary.ContainsKey(msg))
                dictionary.Add(msg, 0);
            dictionary[msg]++;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- Insure -->
        /// <summary>
        ///      Use default exception actions if necesssary, handles missing exception actions with a default Squawk-Log-Pause
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static Endeme Insure(Endeme actions)
        {
            if (_defaultActions == null)
            {
                _defaultActions = "SLP";
            }
            if (actions == null)
            {
                actions = _defaultActions;
            }
            return actions;
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
            {
                return defaultValue;
            }
            else
            {
                Type type = obj.GetType();
                string str = null;
                int value = 0;


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (object.ReferenceEquals(type, typeof(int)) || object.ReferenceEquals(type, typeof(Int16)) || object.ReferenceEquals(type, typeof(Int32)) || object.ReferenceEquals(type, typeof(Int64)))
                {
                    return Convert.ToInt32(obj);
                }
                str = obj.ToString();
                if (int.TryParse(str, out value))
                {
                    return value;
                }


                return defaultValue;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LevelErrorText -->
        /// <summary>
        ///      Returns the message from one exception internalization level
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        private static string LevelErrorText(Exception ex)
        {
            string myError = ex.GetType().ToString() + " - " + ex.Message;
            foreach (object key in ex.Data)
            {
                myError += ", " + key.ToString() + ":" + ex.Data[key].ToString();
            }
            return myError;
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
            if (num < 0 || string.IsNullOrEmpty(str))
            {
                return "";
            }
            string[] separators = {
				"\r\n",
				"\r",
				"\n"
			};
            // I know \n\r does not exist, but lets take it as a line divider anyway, took it out
            string[] lines = str.Split(separators, StringSplitOptions.None);
            if (num < lines.Length)
            {
                return lines[num];
            }
            else
            {
                return "";
            }
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- LocalIps -->
        ///// <summary>
        /////      Returns the IP address(es) of the local machine
        /////      Using host name (local machine name), get the IP address list
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks>alpha code</remarks>
        //private static List<string> LocalIPs()
        //{
        //    List<string> ipList = new List<string>();
        //    IPHostEntry entry = Dns.GetHostByName(SetLocalMachineName());
        //    IPAddress[] addr = entry.AddressList;
        //    for (int i = 0; i <= addr.Length - 1; i++)
        //    {
        //        ipList.Add(addr[i].ToString());
        //    }
        //    return ipList;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- LocalMacAddresses -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private static List<string> LocalMacAddresses()
        {
            List<string> macList = new List<string>();
            System.Management.ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection moList = mc.GetInstances();
            foreach (ManagementObject mo in moList)
            {
                string macAddr = mo["MACAddress"] as string;
                if (macAddr != null && !string.IsNullOrEmpty(macAddr.Trim()))
                {
                    macList.Add(macAddr);
                }
            }
            return macList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LocalMachineName -->
        /// <summary>
        ///      First get the host name of local machine
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public static string SetLocalMachineName()
        {
            string name = Dns.GetHostName();
            // host name of local machine
            if (string.IsNullOrEmpty(name))
            {
                name = Environment.MachineName;
            }
            if (string.IsNullOrEmpty(name))
            {
                name = System.Windows.Forms.SystemInformation.ComputerName;
            }
            if (string.IsNullOrEmpty(name))
            {
                name = Environment.GetEnvironmentVariable("COMPUTERNAME");
            }
            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PrepareMessage -->
        /// <summary>
        ///      Prepare message text
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <remarks>alpha code</remarks>
        private static string PrepareMessage(Exception ex)
        {
            string msg = "";
            if (ex.InnerException != null)
            {
                msg = ex.InnerException.GetType().ToString() + "\r\n" + ex.InnerException.Message + "\r\n" + "\r\n" + ex.Message;
            }
            else
            {
                msg = ex.GetType().ToString() + " - " + ex.Message;
            }
            return msg;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReadyCheck -->
        /// <summary>
        ///      If you're tracing actions you need a place to store the traces during exceptions
        ///      (and therefore a source from which they came)
        /// </summary>
        public static void ReadyCheck()
        {
            if (Throws._eventSource == null)
            {
                throw new NoNullAllowedException("Throw.ApplicationEventSourceData is null!");
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ClearCache -->
        /// <summary>
        ///      Clears the buffer used to prevent Throws from showing duplicate messages
        /// </summary>
        /// <remarks></remarks>
        public static void ClearCache()
        {
            _oldMessages = new List<string>();
            if ((_defaultActions == null))
            {
                _defaultActions = "P";
            }
            if ((_defaultActions == null | _defaultActions == null))
            {
                _defaultActions = new Endeme(_actions, "P");
            }
        }

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
        private static string Section(string @from, string str, int width, int offsetBack)
        {
            // --------------------------------------------------------------------------
            //  Do stuff
            // --------------------------------------------------------------------------
            string strL = Regex.Replace(@from, "^[(](\\d+), *\\d+[)]$", "$1");
            int lineNum = Integer(strL, 0) - 1;
            if (lineNum == -1)
            {
                lineNum = 0;
            }
            if (lineNum < 0)
            {
                return "";
            }
            string line__1 = Line(lineNum, str);


            // --------------------------------------------------------------------------
            //  Do stuff
            // --------------------------------------------------------------------------
            string strC = Regex.Replace(@from, "^[(]\\d+, *(\\d+)[)]$", "$1");
            int charNum = Math.Max(Integer(strC, 0) - offsetBack - 1, 0);
            string section__2 = "";
            if (charNum < line__1.Length)
            {
                if (line__1.Length >= width)
                {
                    section__2 = line__1.Substring(charNum, width);
                }
                else
                {
                    section__2 = line__1.Substring(charNum);
                }
            }


            return section__2;
        }


        // --------------------------------------------------------------------------------------------
        /// <!-- SendAlert -->
        /// <summary>
        ///      Sends an alert message to alert support staff that there is a problem in real time
        /// </summary>
        /// <param name="ex"></param>
        /// <remarks>I'm afraid this requires a stack trace</remarks>

        private static void SendAlert(Exception ex, string msg)
        {
            // --------------------------------------------------------------------------
            //  Fill in any missing items in the exception
            // --------------------------------------------------------------------------
            ex = FillOutException(ex, msg);


            // --------------------------------------------------------------------------
            //  Now send the email
            // --------------------------------------------------------------------------
            //if ((appmgr != null))
            //{
            //    ER.ErrorLogging errorLog = new ER.ErrorLogging(appmgr);
            //    if (errorLog != null)
            //    {
            //        try
            //        {
            //            errorLog.Write(ex);
            //        }
            //        catch (Exception ex2)
            //        {
            //        }
            //    }
            //}

        }


        // ----------------------------------------------------------------------------------------
        /// <!-- LogToActionLog -->
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
        /// <remarks>alpha code</remarks>
        private static void LogToActionLog(string eventMessage, int eventId, EventLogEntryType eventType, EventSourceCreationData eventSourceData, string gat)
        {
            if (eventSourceData == null)
            {
                throw new NoNullAllowedException("eventSourceData is null!");
            }
            try
            {
                if (gat.Length > 2042)
                {
                    gat = "... " + gat.Substring(gat.Length - 2042);
                }
                if (gat.Length > 0)
                {
                    gat = "\r\n" + gat;
                }
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


                //EventLog myLog = new EventLog("RPGLog");
                //foreach (System.Diagnostics.EventLogEntry entry in myLog.Entries)
                //{
                //    string msg = entry.Message;
                //    string usr = entry.UserName;
                //}
                log.WriteEntry(eventMessage + "\r\n" + Environment.UserName + gat, eventType, eventId, Convert.ToInt16(1));
            }
            catch (Exception ex)
            {
                string myError = ex.Message;
            }
        }
        private static void LogToActionLog(string eventMessage, EventSourceCreationData source)
        {
            LogToActionLog(eventMessage, 1000, EventLogEntryType.Error, source, "");
        }
        private static void LogToActionLog(string eventMessage)
        {
            LogToActionLog(eventMessage, 1000, EventLogEntryType.Error, _eventSource, "");
        }
        private static void LogToActionLog(string eventMessage, EventSourceCreationData source, string strGat)
        {
            LogToActionLog(eventMessage, 1000, EventLogEntryType.Error, _eventSource, strGat);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ShowRecordedErrors -->
        /// <summary>
        ///      Retuns a report of the errors collated internally in Throws using 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ShowRecordedErrors()
        {
            string delim = "";
            StringBuilder str = new StringBuilder("");
            if ((ErrorList != null))
            {
                foreach (string err in ErrorList)
                {
                    str.Append(delim + err);
                    delim = "\r\n";
                }
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SendToDatabase -->
        /// <summary>
        ///      Sends the error message to a specific database table
        /// </summary>
        /// <param name="msg"></param>
        /// <remarks>TODO: Write Me</remarks>
        private static Guid SendToDatabase(string msg, string data1, string data2, string data3)
        {
            Guid guid = Guid.Empty;
            return guid;
        }
        /// <summary>
        ///  This is cut and paste from __.Collate, don't refactor it out
        /// </summary>
        public static string Collate(List<string> list, string delim)
        {
            StringBuilder str = new StringBuilder();
            string d = "";
            foreach (string s in list)
            {
                str.Append(d + s);
                d = delim;
            }
            return str.ToString();
        }


        // -----------------------------------------------------------------------------------------
        /// <!-- ThrowIfThrowsNotActivated -->
        /// <summary>
        ///      Throws an exception if throws has not been properly activated
        /// </summary>
        /// <param name="eventSource"></param>
        /// <remarks></remarks>
        /// <remarks>production ready</remarks>
        private static void ThrowIfThrowsNotActivated(EventSourceCreationData eventSource)
        {
            // --------------------------------------------------------------------------
            //  Handle missing event log operational data by complaining
            // --------------------------------------------------------------------------
            if (eventSource == null)
            {
                // Activate(string source, string eventLogName, object sender, object strAppIdNumber)
                throw new NoNullAllowedException("EventSourceCreationData is null!"
                    + "\r\n" + " You may have to add code something like this before running Throws:"
                    + "\r\n" + "    Throws.Activate(\"MyProj\", \"MyProjLog\", this, 0)"
                    + "\r\n" + " or"
                    + "\r\n" + "    Throws.Activate(\"MyProj\", \"MyProjLog\", sender, 0)"
                    );
            }
            if (ErrorList == null)
            {
                throw new NoNullAllowedException("ErrorList is null");
            }
        }


        // --------------------------------------------------------------------------------------------
        /// <!-- DataAdapter -->
        /// <remarks>use method from HardData or DataAccess instead</remarks>
        public class DataAdapter : DbDataAdapter
        {
            public int FillTable(DataTable table, IDataReader rdr)
            {
                if (rdr == null)
                {
                    throw new NoNullAllowedException("FillTable error - dataReader is null");
                }
                return this.Fill(table, rdr);
            }
            protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
            {
                return null;
            }
            protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
            {
                return null;
            }
            protected override void OnRowUpdated(RowUpdatedEventArgs value)
            {
            }
            protected override void OnRowUpdating(RowUpdatingEventArgs value)
            {
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TryErrorInsert -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <remarks>alpha code</remarks>
        private static bool TryErrorInsert(SqlCommand cmd)
        {
            bool saved = false;
            try
            {
                // ----------------------------------------------------------------------
                //  Do the actual insert
                // ----------------------------------------------------------------------
                Open(cmd.Connection);
                try { cmd.ExecuteNonQuery(); saved = true; }
                catch (Exception ex)
                {
                    Is.Trash(ex);
                    //Throw New DataException("query did not run properly", ex)
                }
                finally { Close(cmd.Connection); }
            }
            catch (Exception ex) { string err = ex.Message; }

            return saved;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteToDebugWindow -->
        /// <summary>
        ///      Uses our delegate to write to the output window, do we even need a delegate here?
        /// </summary>
        /// <param name="msg"></param>
        /// <remarks></remarks>
        private static void WriteToDebugWindow(string msg)
        {
            //Throws t = new Throws();
            //MyDelSub del = null;
            //del = new MyDelSub(t.WriteToDebug);
            //del.Invoke(msg);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- XmlErrorText -->
        /// <summary>
        ///      Exception data for a particular exception level
        /// </summary>
        /// <param name="ex">exception</param>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static string XmlErrorText(Exception ex, string xml)
        {
            string myError = "";
            if (Regex.IsMatch(ex.Message, "^There is an error in XML document \\("))
            {
                string at = Regex.Replace(ex.Message, "^.*([(]\\d+, ?\\d+[)]).*$", "$1");
                myError += "\r\n" + "Error near:  " + " '" + Regex.Replace(Section(at, xml, 66, 66), "^.*(<.*>.*<.*>)<?$", "$1") + "'";
            }
            return myError;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteToDebug -->
        /// <summary>
        ///      Delegate experiment
        /// </summary>
        /// <param name="msg"></param>
        /// <remarks></remarks>
        private static void WriteToDebug(string msg)
        {
            Debug.WriteLine(msg);
        }


        #region "don't look, it's hideous and smells bad"


        private static System.Web.UI.MasterPage MastPage { get; set; }
        private static System.Web.UI.Page WebPage { get; set; }
        private static System.Windows.Forms.Form FormPage { get; set; }
        //private static System.Windows.Controls.Page WpfPage { get; set; }


        // -----------------------------------------------------------------------------------------
        /// <!-- PageName -->
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string PageName
        {
            get
            {
                string name = "";
                //if (string.IsNullOrEmpty(name) && (WpfPage != null))
                //{
                //    name = WpfPage.Title;
                //}
                //if (string.IsNullOrEmpty(name) && (WpfPage != null))
                //{
                //    name = WpfPage.Name;
                //}
                if (string.IsNullOrEmpty(name) && (WebPage != null))
                {
                    name = WebPage.Title;
                }
                if (string.IsNullOrEmpty(name) && (MastPage != null))
                {
                    name = MastPage.UniqueID;
                }
                if (string.IsNullOrEmpty(name) && (FormPage != null))
                {
                    name = FormPage.Text;
                }
                if (string.IsNullOrEmpty(name) && (FormPage != null))
                {
                    name = FormPage.Name;
                }
                return name;
            }
        }


        // -----------------------------------------------------------------------------------------
        /// <!-- Squawk -->
        /// <summary>
        ///      Puts a popup complaint on the screen for the user to see (irrespective of the application type)
        /// </summary>
        /// <param name="msg"></param>
        /// <remarks>This is very smelly code but hopefully I only ever have to do this sort of thing once</remarks>
        private static void Squawk(string msg)
        {
            // --------------------------------------------------------------------------
            //  Evaluate universe
            // --------------------------------------------------------------------------
            bool formish = (System.Reflection.Assembly.GetEntryAssembly() != null);
            bool webbish = (System.Web.HttpContext.Current != null);
            bool consolish = (!object.ReferenceEquals(Console.OpenStandardInput(1), System.IO.Stream.Null));
            bool interactive = Environment.UserInteractive;
          //int ctext = 0; // System.Threading.Thread.CurrentContext.ContextProperties.Count;


            if (webbish & !formish & !consolish & (MastPage != null | WebPage != null))
            {
                WebMessageBox(msg);
            }
            else if ((formish & !webbish & !consolish & FormPage != null))
            {
                System.Windows.Forms.MessageBox.Show(msg);
            }
            else if ((consolish & !formish & !webbish))
            {
                Console.WriteLine(msg);
            }
            else
            {
                try
                {
                    WebMessageBox(msg);
                }
                catch (Exception exA)
                {
                    try
                    {
                        System.Windows.Forms.MessageBox.Show(msg);
                    }
                    catch (Exception exB)
                    {
                        try
                        {
                            Console.WriteLine(msg);
                        }
                        catch (Exception exC)
                        {
                            Is.Trash(exC);
                        }
                        Is.Trash(exB);
                    }
                    Is.Trash(exA);
                }
            }
        }


        // -----------------------------------------------------------------------------------------
        /// <!-- WebMessageBox -->
        /// <summary>
        ///      Does something very similar to a Forms MessageBox for an ASP.NET web page
        /// </summary>
        /// <param name="msg"></param>
        /// <remarks></remarks>
        private static void WebMessageBox(string msg)
        {
            System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
            lbl.Text = "<script language='javascript'>" + Environment.NewLine + "window.alert('" + msg + "')</script>";
            if ((MastPage != null))
            {
                MastPage.Controls.Add(lbl);
            }
            else
            {
                WebPage.Controls.Add(lbl);
            }
        }


        #endregion

    }


    // ----------------------------------------------------------------------------------------
    /// <!-- MyDelSub -->
    /// <summary>
    ///      Part of my delegate experiment (see WriteToDebug)
    /// </summary>
    /// <param name="msg"></param>
    /// <remarks></remarks>
    public delegate void MyDelSub(string msg);

}
