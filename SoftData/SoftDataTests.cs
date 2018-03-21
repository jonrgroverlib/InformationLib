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
using System.Web.UI.WebControls;      // for CheckBox

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- SoftDataTests -->
    /// <summary>
    ///      The SoftDataTests class tests the classes in the SoftData Library
    /// </summary>
    /// <remarks>production ready unit test code</remarks>
    public class SoftDataTests
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private Result _result;


        // ----------------------------------------------------------------------------------------
        /// <!-- AllTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            _result = new Result("Soft data tests");


            // --------------------------------------------------------------------------
            //  Constructor tests
            // --------------------------------------------------------------------------
            TreatAs_BoolValue_test ();
            TreatAs_IntValue_test  ();
            TreatAs_LongValue_test ();
            TreatAs_StrValue_test  ();
            Range_Wrap_test        ();        


			// --------------------------------------------------------------------------
			//  Tests
			// --------------------------------------------------------------------------
			FuzzySet_Constructor_test();


            return _result.AsciiDetailResults + "\r\n" + _result.SummaryMessage();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Range_Wrap_test -->
        /// <summary>
        ///      Tests the Wrap method in the Range class
        /// </summary>
        //[Test]
        public void Range_Wrap_test()
        {
            Assert.ThingsAbout("Range", "Wrap");

            Range_Wrap_testcase(0, 5, true , false, -3, 2);
            Range_Wrap_testcase(0, 5, true , false, -2, 3);
            Range_Wrap_testcase(0, 5, true , false, -1, 4);
            Range_Wrap_testcase(0, 5, true , false,  0, 0);
            Range_Wrap_testcase(0, 5, true , false,  1, 1);
            Range_Wrap_testcase(0, 5, true , false,  2, 2);
            Range_Wrap_testcase(0, 5, true , false,  3, 3);
            Range_Wrap_testcase(0, 5, true , false,  4, 4);
            Range_Wrap_testcase(0, 5, true , false,  5, 0);
            Range_Wrap_testcase(0, 5, true , false,  6, 1);
            Range_Wrap_testcase(0, 5, true , false,  7, 2);
            Range_Wrap_testcase(0, 5, true , false,  8, 3);
            Range_Wrap_testcase(0, 5, false, false, -3, 1);
            Range_Wrap_testcase(0, 5, false, false, -2, 2);
            Range_Wrap_testcase(0, 5, false, false, -1, 3);
            Range_Wrap_testcase(0, 5, false, false,  0, 4);
            Range_Wrap_testcase(0, 5, false, false,  1, 1);
            Range_Wrap_testcase(0, 5, false, false,  2, 2);
            Range_Wrap_testcase(0, 5, false, false,  3, 3);
            Range_Wrap_testcase(0, 5, false, false,  4, 4);
            Range_Wrap_testcase(0, 5, false, false,  5, 1);
            Range_Wrap_testcase(0, 5, false, false,  6, 2);
            Range_Wrap_testcase(0, 5, false, false,  7, 3);
            Range_Wrap_testcase(0, 5, false, false,  8, 4);
            Range_Wrap_testcase(0, 5, true , true , -3, 3);
            Range_Wrap_testcase(0, 5, true , true , -2, 4);
            Range_Wrap_testcase(0, 5, true , true , -1, 5);
            Range_Wrap_testcase(0, 5, true , true ,  0, 0);
            Range_Wrap_testcase(0, 5, true , true ,  1, 1);
            Range_Wrap_testcase(0, 5, true , true ,  2, 2);
            Range_Wrap_testcase(0, 5, true , true ,  3, 3);
            Range_Wrap_testcase(0, 5, true , true ,  4, 4);
            Range_Wrap_testcase(0, 5, true , true ,  5, 5);
            Range_Wrap_testcase(0, 5, true , true ,  6, 0);
            Range_Wrap_testcase(0, 5, true , true ,  7, 1);
            Range_Wrap_testcase(0, 5, true , true ,  8, 2);
            Range_Wrap_testcase(0, 5, false, true , -3, 2);
            Range_Wrap_testcase(0, 5, false, true , -2, 3);
            Range_Wrap_testcase(0, 5, false, true , -1, 4);
            Range_Wrap_testcase(0, 5, false, true ,  0, 5);
            Range_Wrap_testcase(0, 5, false, true ,  1, 1);
            Range_Wrap_testcase(0, 5, false, true ,  2, 2);
            Range_Wrap_testcase(0, 5, false, true ,  3, 3);
            Range_Wrap_testcase(0, 5, false, true ,  4, 4);
            Range_Wrap_testcase(0, 5, false, true ,  5, 5);
            Range_Wrap_testcase(0, 5, false, true ,  6, 1);
            Range_Wrap_testcase(0, 5, false, true ,  7, 2);
            Range_Wrap_testcase(0, 5, false, true ,  8, 3);
            Range_Wrap_testcase(2, 7, true , false, -3, 2);
            Range_Wrap_testcase(2, 7, true , false, -2, 3);
            Range_Wrap_testcase(2, 7, true , false, -1, 4);
            Range_Wrap_testcase(2, 7, true , false,  0, 5);
            Range_Wrap_testcase(2, 7, true , false,  1, 6);
            Range_Wrap_testcase(2, 7, true , false,  2, 2);
            Range_Wrap_testcase(2, 7, true , false,  3, 3);
            Range_Wrap_testcase(2, 7, true , false,  4, 4);
            Range_Wrap_testcase(2, 7, true , false,  5, 5);
            Range_Wrap_testcase(2, 7, true , false,  6, 6);
            Range_Wrap_testcase(2, 7, true , false,  7, 2);
            Range_Wrap_testcase(2, 7, true , false,  8, 3);
            Range_Wrap_testcase(2, 7, false, false, -3, 5);
            Range_Wrap_testcase(2, 7, false, false, -2, 6);
            Range_Wrap_testcase(2, 7, false, false, -1, 3);
            Range_Wrap_testcase(2, 7, false, false,  0, 4);
            Range_Wrap_testcase(2, 7, false, false,  1, 5);
            Range_Wrap_testcase(2, 7, false, false,  2, 6);
            Range_Wrap_testcase(2, 7, false, false,  3, 3);
            Range_Wrap_testcase(2, 7, false, false,  4, 4);
            Range_Wrap_testcase(2, 7, false, false,  5, 5);
            Range_Wrap_testcase(2, 7, false, false,  6, 6);
            Range_Wrap_testcase(2, 7, false, false,  7, 3);
            Range_Wrap_testcase(2, 7, false, false,  8, 4);
            Range_Wrap_testcase(2, 7, true , true , -3, 3);
            Range_Wrap_testcase(2, 7, true , true , -2, 4);
            Range_Wrap_testcase(2, 7, true , true , -1, 5);
            Range_Wrap_testcase(2, 7, true , true ,  0, 6);
            Range_Wrap_testcase(2, 7, true , true ,  1, 7);
            Range_Wrap_testcase(2, 7, true , true ,  2, 2);
            Range_Wrap_testcase(2, 7, true , true ,  3, 3);
            Range_Wrap_testcase(2, 7, true , true ,  4, 4);
            Range_Wrap_testcase(2, 7, true , true ,  5, 5);
            Range_Wrap_testcase(2, 7, true , true ,  6, 6);
            Range_Wrap_testcase(2, 7, true , true ,  7, 7);
            Range_Wrap_testcase(2, 7, true , true ,  8, 2);
            Range_Wrap_testcase(2, 7, false, true , -3, 7);
            Range_Wrap_testcase(2, 7, false, true , -2, 3);
            Range_Wrap_testcase(2, 7, false, true , -1, 4);
            Range_Wrap_testcase(2, 7, false, true ,  0, 5);
            Range_Wrap_testcase(2, 7, false, true ,  1, 6);
            Range_Wrap_testcase(2, 7, false, true ,  2, 7);
            Range_Wrap_testcase(2, 7, false, true ,  3, 3);
            Range_Wrap_testcase(2, 7, false, true ,  4, 4);
            Range_Wrap_testcase(2, 7, false, true ,  5, 5);
            Range_Wrap_testcase(2, 7, false, true ,  6, 6);
            Range_Wrap_testcase(2, 7, false, true ,  7, 7);
            Range_Wrap_testcase(2, 7, false, true ,  8, 3);

            _result += Assert.Conclusion;
        }
        private static void Range_Wrap_testcase(double low, double high, bool lowInc, bool highInc, int input, int target)
        {
            Range range = new Range(low, high, lowInc, highInc);
            int value = range.Wrap( input);
            Assert.That(value, Is.equal_to, target);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TreatAs_BoolValue_test -->
        /// <summary>
        /// 
        /// </summary>
        private void TreatAs_BoolValue_test()
        {
            Assert.ThingsAbout("TreatAs", "BoolValue");

            TreatAs_BoolValue_testcase(null, true , true );
            TreatAs_BoolValue_testcase(null, false, false);

            // Standard values
            TreatAs_BoolValue_testcase(true , true , true );
            TreatAs_BoolValue_testcase(true , false, true );
            TreatAs_BoolValue_testcase(false, true , false);
            TreatAs_BoolValue_testcase(false, false, false);
            TreatAs_BoolValue_testcase(-1   , true , true );
            TreatAs_BoolValue_testcase(-1   , false, true );
            TreatAs_BoolValue_testcase( 0   , true , false);
            TreatAs_BoolValue_testcase( 0   , false, false);
            TreatAs_BoolValue_testcase( 1   , true , true );
            TreatAs_BoolValue_testcase( 1   , false, true );
            TreatAs_BoolValue_testcase( 2   , true , true );
            TreatAs_BoolValue_testcase( 2   , false, true );

            //  Handled by SqlBoolean
            TreatAs_BoolValue_testcase("True" , true , true );
            TreatAs_BoolValue_testcase("True" , false, true );
            TreatAs_BoolValue_testcase("False", true , false);
            TreatAs_BoolValue_testcase("False", false, false);
            TreatAs_BoolValue_testcase("Null" , true , true );
            TreatAs_BoolValue_testcase("Null" , false, false);
            TreatAs_BoolValue_testcase("TRUE" , true , true );
            TreatAs_BoolValue_testcase("TRUE" , false, true );
            TreatAs_BoolValue_testcase("FALSE", true , false);
            TreatAs_BoolValue_testcase("FALSE", false, false);
            TreatAs_BoolValue_testcase("T"    , true , true );
            TreatAs_BoolValue_testcase("T"    , false, true );
            TreatAs_BoolValue_testcase("F"    , true , false);
            TreatAs_BoolValue_testcase("F"    , false, false);
            TreatAs_BoolValue_testcase("Y"    , true , true );
            TreatAs_BoolValue_testcase("Y"    , false, true );
            TreatAs_BoolValue_testcase("N"    , true , false);
            TreatAs_BoolValue_testcase("N"    , false, false);
            TreatAs_BoolValue_testcase("YES"  , true , true );
            TreatAs_BoolValue_testcase("YES"  , false, true );
            TreatAs_BoolValue_testcase("NO"   , true , false);
            TreatAs_BoolValue_testcase("NO"   , false, false);
            TreatAs_BoolValue_testcase("Yes"  , true , true );
            TreatAs_BoolValue_testcase("Yes"  , false, true );
            TreatAs_BoolValue_testcase("No"   , true , false);
            TreatAs_BoolValue_testcase("No"   , false, false);
            TreatAs_BoolValue_testcase("1"    , true , true );
            TreatAs_BoolValue_testcase("1"    , false, true );
            TreatAs_BoolValue_testcase("0"    , true , false);
            TreatAs_BoolValue_testcase("0"    , false, false);

            //  special cases
            TreatAs_BoolValue_testcase(Guid.NewGuid(), true, true);
            TreatAs_BoolValue_testcase(Guid.NewGuid(), false, true);
            TreatAs_BoolValue_testcase(Guid.Empty    , true, false);
            TreatAs_BoolValue_testcase(Guid.Empty    , false, false);

            CheckBox chk = new CheckBox();
            TreatAs_BoolValue_testcase(chk, true, false); TreatAs_BoolValue_testcase(chk, false, false);
            chk.Checked = true ; TreatAs_BoolValue_testcase(chk, true, true ); TreatAs_BoolValue_testcase(chk, false, true );
            chk.Checked = false; TreatAs_BoolValue_testcase(chk, true, false); TreatAs_BoolValue_testcase(chk, false, false);

            _result += Assert.Conclusion;
        }
        private static void TreatAs_BoolValue_testcase(object obj, bool defaultValue, bool target)
        {
            bool value = TreatAs.BoolValue(obj, defaultValue);
            Assert.That(value, Is.equal_to, target);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TreatAs_IntValue_test -->
        /// <summary>
        /// 
        /// </summary>
        private void TreatAs_IntValue_test()
        {
            Assert.ThingsAbout("TreatAs", "IntValue");


            TreatAs_IntValue_testcase(0             ,  200,  0            );
            TreatAs_IntValue_testcase(3             ,  200,  3            );
            TreatAs_IntValue_testcase(0L            ,  200,  0            );
            TreatAs_IntValue_testcase(3L            ,  200,  3            );
            TreatAs_IntValue_testcase(-3            ,  200, -3            );
            TreatAs_IntValue_testcase(-3L           ,  200, -3            );
            TreatAs_IntValue_testcase("2"           ,  200,  2            );
            TreatAs_IntValue_testcase('A'           ,  200,  65           );
            TreatAs_IntValue_testcase('2'           ,  200,  50           );
            TreatAs_IntValue_testcase(45.55m        ,  200,  45           );
            TreatAs_IntValue_testcase("1234"        ,  200,  1234         );
            TreatAs_IntValue_testcase(null          , 2001,  2001         );
            TreatAs_IntValue_testcase("-12345"      ,  200, -12345        );
            TreatAs_IntValue_testcase(54321.3345612 ,  200,  54321        );
            TreatAs_IntValue_testcase(2147483647L   ,  200,  2147483647   );
            TreatAs_IntValue_testcase(1000000000D   ,  200,  1000000000   );
            TreatAs_IntValue_testcase("2147483647"  ,  200,  2147483647   );
            TreatAs_IntValue_testcase(-2147483648L  ,  200, -2147483648   );
            TreatAs_IntValue_testcase(Int32.MaxValue,  200, Int32.MaxValue);
            TreatAs_IntValue_testcase(Int32.MinValue,  200, Int32.MinValue);
            TreatAs_IntValue_testcase(2147482342342343647L, 200, 200);

            _result += Assert.Conclusion;
        }
        private static void TreatAs_IntValue_testcase(object obj, int defaultValue, int target)
        {
            int value = TreatAs.IntValue(obj, defaultValue);
            Assert.That(value, Is.equal_to, target);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TreatAs_LongValue_test -->
        /// <summary>
        /// 
        /// </summary>
        private void TreatAs_LongValue_test()
        {
            Assert.ThingsAbout("TreatAs", "LongValue");


            TreatAs_LongValue_testcase(0, 200L, 0L);
            TreatAs_LongValue_testcase(3, 200L, 3L);
            TreatAs_LongValue_testcase(0L, 200L, 0L);
            TreatAs_LongValue_testcase(3L, 200L, 3L);
            TreatAs_LongValue_testcase(-3, 200L, -3L);
            TreatAs_LongValue_testcase(-3L, 200L, -3L);
            TreatAs_LongValue_testcase('A', 200L, 65L);
            TreatAs_LongValue_testcase(45.55m, 200L, 45L);
            TreatAs_LongValue_testcase("1234", 200L, 1234L);
            TreatAs_LongValue_testcase(null, 200L, 200L);
            TreatAs_LongValue_testcase("-1234", 200L, -1234L);
            TreatAs_LongValue_testcase(543.3345612, 200L, 543L);
            TreatAs_LongValue_testcase(2147483647L, 200L, 2147483647L);
            TreatAs_LongValue_testcase(2147483648L, 200L, 2147483648L);
            TreatAs_LongValue_testcase("2147483648", 200L, 2147483648L);
            TreatAs_LongValue_testcase(-2147483648L, 200L, -2147483648L);
            TreatAs_LongValue_testcase(-2147483649L, 200L, -2147483649L);
            TreatAs_LongValue_testcase("-2147483649", 200L, -2147483649L);
            TreatAs_LongValue_testcase(Int64.MaxValue, 200L, Int64.MaxValue);
            TreatAs_LongValue_testcase(Int64.MinValue, 200L, Int64.MinValue);
            TreatAs_LongValue_testcase(1000000000000000000D, 200L, 1000000000000000000L);

            _result += Assert.Conclusion;
        }
        private static void TreatAs_LongValue_testcase(object obj, long defaultValue, long target)
        {
            Int64 value = TreatAs.LongValue(obj, defaultValue);
            Assert.That(value, Is.equal_to, target);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TreatAs_StrValue_test -->
        /// <summary>
        /// 
        /// </summary>
        private void TreatAs_StrValue_test()
        {
            Assert.ThingsAbout("TreatAs", "StrValue");


            TreatAs_StrValue_testcase(""  , "hi", ""   );
            TreatAs_StrValue_testcase(147 , "hi", "147");
            TreatAs_StrValue_testcase(null, "hi", "hi" );

            _result += Assert.Conclusion;
        }
        private static void TreatAs_StrValue_testcase(object obj, string defaultValue, string target)
        {
            string value = TreatAs.StrValue(obj, defaultValue);
            Assert.That(value, Is.equal_to, target);
        }

		// ----------------------------------------------------------------------------------------
		/// <!-- FuzzySet_Constructor_test -->
		/// <summary>
		///      Tests the FuzzySet Constructor  method in the FuzzySet class
		/// </summary>
		///[Test]
		private void FuzzySet_Constructor_test()
		{
            Assert.ThingsAbout("FuzzySet", "Constructor");




            _result += Assert.Conclusion;
		}
    }
}
