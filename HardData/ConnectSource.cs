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
using System;                         // for 
using System.Configuration; // add reference // for ConnectionStringSettings etc.
using System.Data;                    // for NoNullAllowedException
using System.Data.SqlClient;          // for SqlConnection
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ConnectSource -->
    /// <summary>
    ///      Maybe this will someday be a singleton like RandomSource
    /// </summary>
    /// <remarks>production ready - most of this is commented out however</remarks>
    public static class ConnectSource
    {
        //// ----------------------------------------------------------------------------------------
        ///// <!-- AppConfigConnectionString -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="conn"></param>
        ///// <returns></returns>
        //private static string AppConfigConnectionString(string connectionStringName)
        //{
        //    // --------------------------------------------------------------------------
        //    //  Try 100 times to get the connection string array from app.config
        //    // --------------------------------------------------------------------------
        //    ConnectionStringSettingsCollection arr = null;
        //    int i;
        //    for (i = 0; (arr == null || arr.Count < 2) && i < 100; ++i)
        //        arr = ConfigurationManager.ConnectionStrings;
        //    if (arr == null || arr.Count < 1)
        //        throw new NoNullAllowedException("Configuration Collection not complete: " + arr.Count + " after " + i + " tries");


        //    // --------------------------------------------------------------------------
        //    //  Do more stuff
        //    // --------------------------------------------------------------------------
        //    ConnectionStringSettings settings = null;
        //    for (i = 0; settings == null && i < 100; ++i)
        //        settings = arr[connectionStringName];
        //    if (settings == null && arr.Count == 1)
        //        settings = arr[0];
        //    if (settings == null)
        //        throw new NoNullAllowedException("for some reason it could not access Connection Settings: " + arr.Count
        //            + "\r\n" + " maybe the wrong connection name was supplied");


        //    string connectionStr = settings.ConnectionString;
        //    return connectionStr;
        //}

        //// ----------------------------------------------------------------------------------------
        ///// <!-- Connection -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="conn"></param>
        ///// <returns></returns>
        //public static SqlConnection Connection(string conn)
        //{
        //    // --------------------------------------------------------------------------
        //    //  conn is either a connection string or a connection string name
        //    // --------------------------------------------------------------------------
        //    SqlConnection connection = new SqlConnection();
        //    if (Regex.IsMatch(conn, ";"))
        //        connection.ConnectionString = conn;
        //    else connection.ConnectionString = _.Connection(conn, 100);
        //    return connection;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- Resolve -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string Resolve(string conn)
        {
            if (!Regex.IsMatch(conn, ";"))
                conn = Connection(conn, 100);
            return conn;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Connection -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static SqlConnection Connection(string conn)
        {
            if (!string.IsNullOrEmpty(conn))
            {
                // --------------------------------------------------------------------------
                //  conn is either a connection string or a connection string name
                // --------------------------------------------------------------------------
                SqlConnection connection = new SqlConnection();
                if (Regex.IsMatch(conn, ";"))
                    connection.ConnectionString = conn;
                else connection.ConnectionString = Connection(conn, 100);
                return connection;
            }
            else
                return null;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Connection -->
        /// <summary>
        ///      Gets a connection string out of the app.config file
        /// </summary>
        /// <param name="connectionStringName">connection tag</param>
        /// <param name="tries">number of tries t make to get the connection string from the app.config file</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Connection(string connectionStringName, int tries)
        {
            if (tries < 1) tries = 100;

            // --------------------------------------------------------------------------
            //  Try 100 times to get the connection string array from app.config
            // --------------------------------------------------------------------------
            ConnectionStringSettingsCollection arr = null;
            int i;
            for (i = 0; (arr == null || arr.Count < 2) && i < tries; ++i)
                arr = ConfigurationManager.ConnectionStrings;
            if (arr == null || arr.Count < 1)
                throw new NoNullAllowedException("Configuration Collection not complete: " + arr.Count + " after " + i + " tries");


            // --------------------------------------------------------------------------
            //  Do more stuff
            // --------------------------------------------------------------------------
            ConnectionStringSettings settings = null;
            for (i = 0; settings == null && i < tries; ++i)
                settings = arr[connectionStringName];
            if (settings == null && arr.Count == 1)
                settings = arr[0];
            if (settings == null)
                throw new NoNullAllowedException("for some reason it could not access Connection Settings: " + arr.Count
                    + "\r\n" + " maybe the wrong connection name was supplied");


            string connectionStr = settings.ConnectionString;
            return connectionStr;
        }
    }
}
