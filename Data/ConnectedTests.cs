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
using InformationLib.Endemes ;        // for Endeme, EndemeSet
using InformationLib.HardData;        // for Commit, Rollback
using InformationLib.Testing ;        // for Here
using System;                         // for 
using System.Data;                    // for 
using System.Data.SqlClient;          // for SqlConnection
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ConnectedTests -->
    /// <summary>
    ///      The ConnectedTests class has various tests that require a connection
    /// </summary>
    public class ConnectedTests
    {
        // -----------------------------------------------------------------------------------------
        //  Members
        // -----------------------------------------------------------------------------------------
        private Result         _result;
        private SqlConnection  connection;
        private SqlTransaction trx;

        // -----------------------------------------------------------------------------------------
        //  Constructors
        // -----------------------------------------------------------------------------------------
        private ConnectedTests() { }
        public ConnectedTests(SqlConnection testConnection)
        {
            if (testConnection == null || string.IsNullOrEmpty(testConnection.ConnectionString))
                { throw new NoNullAllowedException("you must have a real connection to perform connected tests."); }
            connection = testConnection;
        }


        // -----------------------------------------------------------------------------------------
        /// <!-- AllTests -->
        /// <summary>
        ///      B)test blew up, C)chain failure, F)test failed, I)test incomplete, S)test succeeded, N)(wrong error results)?
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            string result = "";
            trx = null;

            try
            {
                InData.Open(connection);
                trx = InData.Begin(connection);


                // --------------------------------------------------------------------------
                //  RichSqlCommand tests
                // --------------------------------------------------------------------------


                // --------------------------------------------------------------------------
                //  EndemeAccess tests
                // --------------------------------------------------------------------------
                EndemeAccess_HappyPath_tests    ();
                EndemeAccess_PrependEndeme_tests();


                result += "\r\n" + "Connected tests succeeded";
            }
            catch (Exception ex) { result += "\r\n" + "Connected tests failed with message " + ex.Message; }
            finally              { if (trx        != null) { InData.Rollback(trx);     }
                                   if (connection != null) { InData.Close(connection); }                         }
            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeAccess_HappyPath_tests -->
        /// <summary>
        ///      Methods used as intended, no null, empty or border tests
        /// </summary>
        /// <remarks></remarks>
        private void EndemeAccess_HappyPath_tests()
        {
            Assert.ThingsAbout("EndemeAccess", "HappyPath");

            EndemeSet enSet1 = EndemeTests.WetlandAnimals;
            Guid      setID1 = EndemeAccess.InsertEndemeSet(enSet1      , connection, trx);
            Guid      setID2 = EndemeAccess.GetEndemeSetID (enSet1.Label, connection, trx);
            EndemeSet enSet2 = EndemeAccess.GetEndemeSet   (setID2      , connection, trx);
            Endeme    en1    = new Endeme                      (enSet2      , "ABC"          );
            Guid      enID   = EndemeAccess.InsertEndeme   (en1         , connection, trx);
            Endeme    en2    = EndemeAccess.GetEndeme      (enID        , connection, trx);
            Endeme    en3    = en2.Copy(); en3.Add("DEF");

            int status = EndemeAccess.UpdateEndeme(en3, enID, connection, trx);

            Assert.That(setID2        , Is.equal_to    , setID1        );
            Assert.That(enSet2.Count  , Is.equal_to    , 22            );
            Assert.That(enSet2.Label  , Is.equal_to    , enSet1.Label  );
            Assert.That(en2.ToString(), Is.equal_to    , en1.ToString());
            Assert.That(en3.ToString(), Is.equal_to    , "ABCDEF"      );
            Assert.That(enSet1.SetId     , Is.not_equal_to, Guid.Empty    );

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeAccess_PrependEndeme_tests -->
        /// <summary>
        /// 
        /// </summary>
        private void EndemeAccess_PrependEndeme_tests()
        {
            Assert.ThingsAbout("EndemeAccess", "PrependEndeme");

            // --------------------------------------------------------------------------
            //  Educe set
            // --------------------------------------------------------------------------
            Guid setID = EndemeAccess.GetEndemeSetID("Animal", connection, trx);
            EndemeSet enSet = null;
            if (setID == Guid.Empty)
            {
                EndemeAccess.InsertEndemeSet(EndemeTests.WetlandAnimals, connection, trx);
            }
            else
            {
                enSet = EndemeAccess.GetEndemeSet(setID, connection, trx);
            }

            // --------------------------------------------------------------------------
            //  Test prepend
            // --------------------------------------------------------------------------
            Endeme en1 = new Endeme                          (enSet   , "ABCD");
            Guid   id1 = EndemeAccess.InsertEndeme       (en1     , connection, trx);
            Endeme en2 = new Endeme                          (enSet   , "MOUSE");
            Guid   id2 = EndemeTableFactory_old.PrependEndeme_old(en2, id1, connection, trx);
            Endeme en3 = EndemeAccess.GetEndeme          (id2     , connection, trx);

            Assert.That(en3.ToString(), Is.equal_to, "MOUSEABCD");

            _result += Assert.Conclusion;
        }


    }
}
