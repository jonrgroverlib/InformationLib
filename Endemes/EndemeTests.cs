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
using System;                         // for Random
using System.Collections.Generic;     // for List
using System.Linq;                    // for orderby
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeTests -->
    /// <summary>
    ///      The EndemeTests class tests the classes in the Endemes Library
    /// </summary>
    /// <remarks>
    ///      Exception
    ///      False, fails
    ///      Not ready, not important, not required for group success
    ///      True, ok, succeeds
    /// 
    ///      production ready unit test code
    /// </remarks>
    public class EndemeTests
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private string _oldResult;  // write me out
        private Result _result;


        // ----------------------------------------------------------------------------------------
        /// <!-- QuickUnitTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string QuickUnitTests()
        {
            _result = new Result("Endeme tests");


            Is_Ok_test ();


            // --------------------------------------------------------------------------
            //  Math methods
            // --------------------------------------------------------------------------
            Endeme_Shifted_test     ();            Endeme_Triangle_test    ();


            // --------------------------------------------------------------------------
            //  Endeme tests
            // --------------------------------------------------------------------------
            Endeme_Constructor_test ();   Endeme_CharAccessor_test();
            Endeme_Accesssor_test   ();   Endeme_ShiftedMatch_test();
            Endeme_AddOperator_test ();   Endeme_AddToLetter_test ();
            Endeme_Bag_test         ();   Endeme_BitwiseAnd_test  ();
            Endeme_BuildString_test ();   Endeme_Contains_test    ();
            Endeme_Copy_test        ();   Endeme_Count_test       ();
            Endeme_Difference_test  ();   Endeme_First_Test       ();
            Endeme_Match_test       (); //Endeme_Minus_test       ();
            Endeme_MultiMatch_test  ();   Endeme_Times_test       ();


            Endeme en = new Endeme();
            en.First(3);


            // --------------------------------------------------------------------------
            //  Endeme Set class related tests
            // --------------------------------------------------------------------------
            EndemeCharacteristic_Constructor_test();
            EndemeHelpers_MoveLetter_test        ();
            EndemeSet_Constructor_test           ();
            EndemeSet_ToString_test              ();
            EndemeTextFormat_Show_test           ();


            // -------------------------------------------------------------------------
            //  Endeme actuator class related tests          
            // -------------------------------------------------------------------------
            EndemeDefinition_By_Weight_test      ();

            EndemeActuator_SmokeTest             ();
            EndemeActuator_Ordered_test          (); // Ordered
            EndemeActuator_HasSets_test          (); // HasSets
            EndemeActuator_HasValue_test         (); // HasVals
            EndemeActuator_HasChars_test         (); // HasChar
            EndemeActuator_Weights_test          (); // Weights
            EndemeActuator_Multifactor_test      ();

            EndemeReference_Constructor_test     ();
            EndemeProfile_Constructor_test       ();
            EndemeObject_Constructor_test        ();


            // -------------------------------------------------------------------------
            //  Endeme List class related tests          
            // -------------------------------------------------------------------------
            EndemeBalancer_Constructor_test      ();
            EndemeList_Constructor_test          ();
            EndemeList_Add_test                  ();
            EndemeList_OrderBy_test              ();
            EndemeList_test                      ();

                                                    
                                                    
          //GenEndemeItem_Constructor_test       ();
          //GenEndemeList_Constructor_test       ();
          //GenEndemeList_Add_test               ();


            //"\r\nEndeme tests succeeded";
            return _result.AsciiDetailResults + "\r\n" + _result.SummaryMessage();
        }

        #region Endeme class tests

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_CharAccessor_test -->
        /// <summary>
        /// 
        /// </summary>
        private void Endeme_CharAccessor_test()
        {
            Assert.ThingsAbout("Endeme", "CharAccessor");

            Endeme en = new Endeme(Fruits, "ABCQ");
            int idxA = en['A'];  Assert.That(idxA, Is.equal_to,  0);
            int idxQ = en['Q'];  Assert.That(idxQ, Is.equal_to,  3);
            int idxB = en['B'];  Assert.That(idxB, Is.equal_to,  1);
            int idxC = en['C'];  Assert.That(idxC, Is.equal_to,  2);
            int idxE = en['E'];  Assert.That(idxE, Is.equal_to, -1);

            _result += Assert.Conclusion;
        }

        private void Endeme_Shifted_test()
        {
            Assert.ThingsAbout("Endeme", "Shifted");


            Assert.That(Endeme.Shifted(-4), Is.equal_to,     -8);
            Assert.That(Endeme.Shifted(-3), Is.equal_to,     -4);
            Assert.That(Endeme.Shifted(-2), Is.equal_to,     -2);
            Assert.That(Endeme.Shifted(-1), Is.equal_to,     -1);
            Assert.That(Endeme.Shifted( 0), Is.equal_to,      0);
            Assert.That(Endeme.Shifted( 1), Is.equal_to,      1);
            Assert.That(Endeme.Shifted( 2), Is.equal_to,      2);
            Assert.That(Endeme.Shifted( 3), Is.equal_to,      4);
            Assert.That(Endeme.Shifted( 4), Is.equal_to,      8);
            Assert.That(Endeme.Shifted( 5), Is.equal_to,     16);
            Assert.That(Endeme.Shifted( 6), Is.equal_to,     32);
            Assert.That(Endeme.Shifted( 7), Is.equal_to,     64);
            Assert.That(Endeme.Shifted( 8), Is.equal_to,    128);
            Assert.That(Endeme.Shifted( 9), Is.equal_to,    256);
            Assert.That(Endeme.Shifted(10), Is.equal_to,    512);
            Assert.That(Endeme.Shifted(11), Is.equal_to,   1024);
            Assert.That(Endeme.Shifted(12), Is.equal_to,   2048);
            Assert.That(Endeme.Shifted(13), Is.equal_to,   4096);
            Assert.That(Endeme.Shifted(14), Is.equal_to,   8192);
            Assert.That(Endeme.Shifted(15), Is.equal_to,  16384);
            Assert.That(Endeme.Shifted(16), Is.equal_to,  32768);
            Assert.That(Endeme.Shifted(17), Is.equal_to,  65536);
            Assert.That(Endeme.Shifted(18), Is.equal_to, 131072);
            Assert.That(Endeme.Shifted(19), Is.equal_to, 262144);

            string detail = Assert.Detail;


            _result += Assert.Conclusion;
        }

        private void Endeme_ShiftedMatch_test()
        {
            Assert.ThingsAbout("Endeme", "ShiftedMatch");

            Endeme en1 = new Endeme(Fruits, "A");
            Endeme en2 = new Endeme(Fruits, "B");
            Endeme en3 = new Endeme(Fruits, "AB");
            Endeme en4 = new Endeme(Fruits, "BA");
            Endeme en5 = new Endeme(Fruits, "ABC");
            Endeme en6 = new Endeme(Fruits, "DEF");
            Endeme en7 = new Endeme(Fruits, "ABCDEFGHIJKLMNOPQRSTUV");
            Endeme en8 = new Endeme(Fruits, "VUTSRQPONMLKJIHGFEDCBA");
            Endeme en9 = new Endeme(Fruits, "ABCDEF");

            int match11 = en1.ShiftedMatch(11, en1);  Assert.That(match11, Is.equal_to, 1048576);
            int match12 = en1.ShiftedMatch(11, en2);  Assert.That(match12, Is.equal_to,       0);
            int match13 = en1.ShiftedMatch(11, en3);  Assert.That(match13, Is.equal_to, 1048576);
            int match14 = en1.ShiftedMatch(11, en4);  Assert.That(match14, Is.equal_to,  524288);
            int match15 = en1.ShiftedMatch(11, en5);  Assert.That(match15, Is.equal_to, 1048576);
            int match16 = en1.ShiftedMatch(11, en6);  Assert.That(match16, Is.equal_to,       0);
            int match17 = en1.ShiftedMatch(11, en7);  Assert.That(match17, Is.equal_to, 1048576);
            int match18 = en1.ShiftedMatch(11, en8);  Assert.That(match18, Is.equal_to, -524288);
            int match19 = en1.ShiftedMatch(11, en9);  Assert.That(match19, Is.equal_to, 1048576);
            //
            int match21 = en2.ShiftedMatch(11, en1);  Assert.That(match21, Is.equal_to,       0); // A");
            int match22 = en2.ShiftedMatch(11, en2);  Assert.That(match22, Is.equal_to, 1048576); // B");
            int match23 = en2.ShiftedMatch(11, en3);  Assert.That(match23, Is.equal_to,  524288); // AB");
            int match24 = en2.ShiftedMatch(11, en4);  Assert.That(match24, Is.equal_to, 1048576); // BA");
            int match25 = en2.ShiftedMatch(11, en5);  Assert.That(match25, Is.equal_to,  524288); // ABC");
            int match26 = en2.ShiftedMatch(11, en6);  Assert.That(match26, Is.equal_to,       0); // DEF");
            int match27 = en2.ShiftedMatch(11, en7);  Assert.That(match27, Is.equal_to,  524288); // ABCDEFGHIJKLMNOPQRSTUV");
            int match28 = en2.ShiftedMatch(11, en8);  Assert.That(match28, Is.equal_to, -262144); // VUTSRQPONMLKJIHGFEDCBA");
            int match29 = en2.ShiftedMatch(11, en9);  Assert.That(match29, Is.equal_to,  524288); // ABCDEF");
            //
            int match31 = en3.ShiftedMatch(11, en1);  Assert.That(match31, Is.equal_to, 1048576);  // A");
            int match32 = en3.ShiftedMatch(11, en2);  Assert.That(match32, Is.equal_to,  524288);  // B");
            int match33 = en3.ShiftedMatch(11, en3);  Assert.That(match33, Is.equal_to, 1310720);  // AB");
            int match34 = en3.ShiftedMatch(11, en4);  Assert.That(match34, Is.equal_to, 1048576);  // BA");
            int match35 = en3.ShiftedMatch(11, en5);  Assert.That(match35, Is.equal_to, 1310720);  // ABC");
            int match36 = en3.ShiftedMatch(11, en6);  Assert.That(match36, Is.equal_to,       0);  // DEF");
            int match37 = en3.ShiftedMatch(11, en7);  Assert.That(match37, Is.equal_to, 1310720);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match38 = en3.ShiftedMatch(11, en8);  Assert.That(match38, Is.equal_to, -655360);  // VUTSRQPONMLKJIHGFEDCBA");
            int match39 = en3.ShiftedMatch(11, en9);  Assert.That(match39, Is.equal_to, 1310720);  // ABCDEF");
            //                Shifted       1
            int match41 = en4.ShiftedMatch(11, en1);  Assert.That(match41, Is.equal_to,  524288);  // A");
            int match42 = en4.ShiftedMatch(11, en2);  Assert.That(match42, Is.equal_to, 1048576);  // B");
            int match43 = en4.ShiftedMatch(11, en3);  Assert.That(match43, Is.equal_to, 1048576);  // AB");
            int match44 = en4.ShiftedMatch(11, en4);  Assert.That(match44, Is.equal_to, 1310720);  // BA");
            int match45 = en4.ShiftedMatch(11, en5);  Assert.That(match45, Is.equal_to, 1048576);  // ABC");
            int match46 = en4.ShiftedMatch(11, en6);  Assert.That(match46, Is.equal_to,       0);  // DEF");
            int match47 = en4.ShiftedMatch(11, en7);  Assert.That(match47, Is.equal_to, 1048576);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match48 = en4.ShiftedMatch(11, en8);  Assert.That(match48, Is.equal_to, -524288);  // VUTSRQPONMLKJIHGFEDCBA");
            int match49 = en4.ShiftedMatch(11, en9);  Assert.That(match49, Is.equal_to, 1048576);  // ABCDEF");
            //                Shifted       1
            int match51 = en5.ShiftedMatch(11, en1);  Assert.That(match51, Is.equal_to, 1048576);  // A");
            int match52 = en5.ShiftedMatch(11, en2);  Assert.That(match52, Is.equal_to,  524288);  // B");
            int match53 = en5.ShiftedMatch(11, en3);  Assert.That(match53, Is.equal_to, 1310720);  // AB");
            int match54 = en5.ShiftedMatch(11, en4);  Assert.That(match54, Is.equal_to, 1048576);  // BA");
            int match55 = en5.ShiftedMatch(11, en5);  Assert.That(match55, Is.equal_to, 1376256);  // ABC");
            int match56 = en5.ShiftedMatch(11, en6);  Assert.That(match56, Is.equal_to,       0);  // DEF");
            int match57 = en5.ShiftedMatch(11, en7);  Assert.That(match57, Is.equal_to, 1376256);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match58 = en5.ShiftedMatch(11, en8);  Assert.That(match58, Is.equal_to, -688128);  // VUTSRQPONMLKJIHGFEDCBA");
            int match59 = en5.ShiftedMatch(11, en9);  Assert.That(match59, Is.equal_to, 1376256);  // ABCDEF");
            //                Shifted       1
            int match61 = en6.ShiftedMatch(11, en1);  Assert.That(match61, Is.equal_to,       0);  // A");
            int match62 = en6.ShiftedMatch(11, en2);  Assert.That(match62, Is.equal_to,       0);  // B");
            int match63 = en6.ShiftedMatch(11, en3);  Assert.That(match63, Is.equal_to,       0);  // AB");
            int match64 = en6.ShiftedMatch(11, en4);  Assert.That(match64, Is.equal_to,       0);  // BA");
            int match65 = en6.ShiftedMatch(11, en5);  Assert.That(match65, Is.equal_to,       0);  // ABC");
            int match66 = en6.ShiftedMatch(11, en6);  Assert.That(match66, Is.equal_to, 1376256);  // DEF");
            int match67 = en6.ShiftedMatch(11, en7);  Assert.That(match67, Is.equal_to,  172032);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match68 = en6.ShiftedMatch(11, en8);  Assert.That(match68, Is.equal_to,  -86016);  // VUTSRQPONMLKJIHGFEDCBA");
            int match69 = en6.ShiftedMatch(11, en9);  Assert.That(match69, Is.equal_to,  172032);  // ABCDEF");
            //                Shifted       1
            int match71 = en7.ShiftedMatch(11, en1);  Assert.That(match71, Is.equal_to,  1048576);  // A");
            int match72 = en7.ShiftedMatch(11, en2);  Assert.That(match72, Is.equal_to,   524288);  // B");
            int match73 = en7.ShiftedMatch(11, en3);  Assert.That(match73, Is.equal_to,  1310720);  // AB");
            int match74 = en7.ShiftedMatch(11, en4);  Assert.That(match74, Is.equal_to,  1048576);  // BA");
            int match75 = en7.ShiftedMatch(11, en5);  Assert.That(match75, Is.equal_to,  1376256);  // ABC");
            int match76 = en7.ShiftedMatch(11, en6);  Assert.That(match76, Is.equal_to,   172032);  // DEF");
            int match77 = en7.ShiftedMatch(11, en7);  Assert.That(match77, Is.equal_to,  1747626);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match78 = en7.ShiftedMatch(11, en8);  Assert.That(match78, Is.equal_to, -1398100);  // VUTSRQPONMLKJIHGFEDCBA");
            int match79 = en7.ShiftedMatch(11, en9);  Assert.That(match79, Is.equal_to,  1397760);  // ABCDEF");
            //                Shifted       1
            int match81 = en8.ShiftedMatch(11, en1);  Assert.That(match81, Is.equal_to,  -524288);  // A");
            int match82 = en8.ShiftedMatch(11, en2);  Assert.That(match82, Is.equal_to,  -262144);  // B");
            int match83 = en8.ShiftedMatch(11, en3);  Assert.That(match83, Is.equal_to,  -655360);  // AB");
            int match84 = en8.ShiftedMatch(11, en4);  Assert.That(match84, Is.equal_to,  -524288);  // BA");
            int match85 = en8.ShiftedMatch(11, en5);  Assert.That(match85, Is.equal_to,  -688128);  // ABC");
            int match86 = en8.ShiftedMatch(11, en6);  Assert.That(match86, Is.equal_to,   -86016);  // DEF");
            int match87 = en8.ShiftedMatch(11, en7);  Assert.That(match87, Is.equal_to, -1398100);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match88 = en8.ShiftedMatch(11, en8);  Assert.That(match88, Is.equal_to,  1747626);  // VUTSRQPONMLKJIHGFEDCBA");
            int match89 = en8.ShiftedMatch(11, en9);  Assert.That(match89, Is.equal_to,  -698880);  // ABCDEF");
            //                Shifted       1
            int match91 = en9.ShiftedMatch(11, en1);  Assert.That(match91, Is.equal_to,  1048576);  // A");
            int match92 = en9.ShiftedMatch(11, en2);  Assert.That(match92, Is.equal_to,   524288);  // B");
            int match93 = en9.ShiftedMatch(11, en3);  Assert.That(match93, Is.equal_to,  1310720);  // AB");
            int match94 = en9.ShiftedMatch(11, en4);  Assert.That(match94, Is.equal_to,  1048576);  // BA");
            int match95 = en9.ShiftedMatch(11, en5);  Assert.That(match95, Is.equal_to,  1376256);  // ABC");
            int match96 = en9.ShiftedMatch(11, en6);  Assert.That(match96, Is.equal_to,   172032);  // DEF");
            int match97 = en9.ShiftedMatch(11, en7);  Assert.That(match97, Is.equal_to,  1397760);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match98 = en9.ShiftedMatch(11, en8);  Assert.That(match98, Is.equal_to,  -698880);  // VUTSRQPONMLKJIHGFEDCBA");
            int match99 = en9.ShiftedMatch(11, en9);  Assert.That(match99, Is.equal_to,  1397760);  // ABCDEF");

            string detail = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_SimpleMatch_test -->
        /// <summary>
        /// 
        /// </summary>
        private void Endeme_MultiMatch_test()
        {
            Assert.ThingsAbout("Endeme", "MultiplyMatch");

            Endeme en1 = new Endeme(Fruits, "A");
            Endeme en2 = new Endeme(Fruits, "B");
            Endeme en3 = new Endeme(Fruits, "AB");
            Endeme en4 = new Endeme(Fruits, "BA");
            Endeme en5 = new Endeme(Fruits, "ABC");
            Endeme en6 = new Endeme(Fruits, "DEF");
            Endeme en7 = new Endeme(Fruits, "ABCDEFGHIJKLMNOPQRSTUV");
            Endeme en8 = new Endeme(Fruits, "VUTSRQPONMLKJIHGFEDCBA");
            Endeme en9 = new Endeme(Fruits, "ABCDEF");

            int match11 = en1.MultiplyMatch(14, en1);  Assert.That(match11, Is.equal_to,  196);
            int match12 = en1.MultiplyMatch(14, en2);  Assert.That(match12, Is.equal_to,    0);
            int match13 = en1.MultiplyMatch(14, en3);  Assert.That(match13, Is.equal_to,  196);
            int match14 = en1.MultiplyMatch(14, en4);  Assert.That(match14, Is.equal_to,  182);
            int match15 = en1.MultiplyMatch(14, en5);  Assert.That(match15, Is.equal_to,  196);
            int match16 = en1.MultiplyMatch(14, en6);  Assert.That(match16, Is.equal_to,    0);
            int match17 = en1.MultiplyMatch(14, en7);  Assert.That(match17, Is.equal_to,  196);
            int match18 = en1.MultiplyMatch(14, en8);  Assert.That(match18, Is.equal_to,  -98);
            int match19 = en1.MultiplyMatch(14, en9);  Assert.That(match19, Is.equal_to,  196);

            int match21 = en2.MultiplyMatch(14, en1);  Assert.That(match21, Is.equal_to,    0);
            int match22 = en2.MultiplyMatch(14, en2);  Assert.That(match22, Is.equal_to,  196);
            int match23 = en2.MultiplyMatch(14, en3);  Assert.That(match23, Is.equal_to,  182);
            int match24 = en2.MultiplyMatch(14, en4);  Assert.That(match24, Is.equal_to,  196);
            int match25 = en2.MultiplyMatch(14, en5);  Assert.That(match25, Is.equal_to,  182);
            int match26 = en2.MultiplyMatch(14, en6);  Assert.That(match26, Is.equal_to,    0);
            int match27 = en2.MultiplyMatch(14, en7);  Assert.That(match27, Is.equal_to,  182);
            int match28 = en2.MultiplyMatch(14, en8);  Assert.That(match28, Is.equal_to,  -84);
            int match29 = en2.MultiplyMatch(14, en9);  Assert.That(match29, Is.equal_to,  182);

            int match31 = en3.MultiplyMatch(14, en1);  Assert.That(match31, Is.equal_to,  196);
            int match32 = en3.MultiplyMatch(14, en2);  Assert.That(match32, Is.equal_to,  182);
            int match33 = en3.MultiplyMatch(14, en3);  Assert.That(match33, Is.equal_to,  365);
            int match34 = en3.MultiplyMatch(14, en4);  Assert.That(match34, Is.equal_to,  364);
            int match35 = en3.MultiplyMatch(14, en5);  Assert.That(match35, Is.equal_to,  365);
            int match36 = en3.MultiplyMatch(14, en6);  Assert.That(match36, Is.equal_to,    0);
            int match37 = en3.MultiplyMatch(14, en7);  Assert.That(match37, Is.equal_to,  365);
            int match38 = en3.MultiplyMatch(14, en8);  Assert.That(match38, Is.equal_to, -176);
            int match39 = en3.MultiplyMatch(14, en9);  Assert.That(match39, Is.equal_to,  365);
  
            int match41 = en4.MultiplyMatch(14, en1);  Assert.That(match41, Is.equal_to,  182);  // A");
            int match42 = en4.MultiplyMatch(14, en2);  Assert.That(match42, Is.equal_to,  196);  // B");
            int match43 = en4.MultiplyMatch(14, en3);  Assert.That(match43, Is.equal_to,  364);  // AB");
            int match44 = en4.MultiplyMatch(14, en4);  Assert.That(match44, Is.equal_to,  365);  // BA");
            int match45 = en4.MultiplyMatch(14, en5);  Assert.That(match45, Is.equal_to,  364);  // ABC");
            int match46 = en4.MultiplyMatch(14, en6);  Assert.That(match46, Is.equal_to,    0);  // DEF");
            int match47 = en4.MultiplyMatch(14, en7);  Assert.That(match47, Is.equal_to,  364);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match48 = en4.MultiplyMatch(14, en8);  Assert.That(match48, Is.equal_to, -175);  // VUTSRQPONMLKJIHGFEDCBA");
            int match49 = en4.MultiplyMatch(14, en9);  Assert.That(match49, Is.equal_to,  364);  // ABCDEF");

            int match51 = en5.MultiplyMatch(14, en1);  Assert.That(match51, Is.equal_to,  196);  // A");
            int match52 = en5.MultiplyMatch(14, en2);  Assert.That(match52, Is.equal_to,  182);  // B");
            int match53 = en5.MultiplyMatch(14, en3);  Assert.That(match53, Is.equal_to,  365);  // AB");
            int match54 = en5.MultiplyMatch(14, en4);  Assert.That(match54, Is.equal_to,  364);  // BA");
            int match55 = en5.MultiplyMatch(14, en5);  Assert.That(match55, Is.equal_to,  509);  // ABC");
            int match56 = en5.MultiplyMatch(14, en6);  Assert.That(match56, Is.equal_to,    0);  // DEF");
            int match57 = en5.MultiplyMatch(14, en7);  Assert.That(match57, Is.equal_to,  509);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match58 = en5.MultiplyMatch(14, en8);  Assert.That(match58, Is.equal_to, -236);  // VUTSRQPONMLKJIHGFEDCBA");
            int match59 = en5.MultiplyMatch(14, en9);  Assert.That(match59, Is.equal_to,  509);  // ABCDEF");

            int match61 = en6.MultiplyMatch(14, en1);  Assert.That(match61, Is.equal_to,    0);  // A");
            int match62 = en6.MultiplyMatch(14, en2);  Assert.That(match62, Is.equal_to,    0);  // B");
            int match63 = en6.MultiplyMatch(14, en3);  Assert.That(match63, Is.equal_to,    0);  // AB");
            int match64 = en6.MultiplyMatch(14, en4);  Assert.That(match64, Is.equal_to,    0);  // BA");
            int match65 = en6.MultiplyMatch(14, en5);  Assert.That(match65, Is.equal_to,    0);  // ABC");
            int match66 = en6.MultiplyMatch(14, en6);  Assert.That(match66, Is.equal_to,  509);  // DEF");
            int match67 = en6.MultiplyMatch(14, en7);  Assert.That(match67, Is.equal_to,  392);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match68 = en6.MultiplyMatch(14, en8);  Assert.That(match68, Is.equal_to, -119);  // VUTSRQPONMLKJIHGFEDCBA");
            int match69 = en6.MultiplyMatch(14, en9);  Assert.That(match69, Is.equal_to,  392);  // ABCDEF");

            int match71 = en7.MultiplyMatch(14, en1);  Assert.That(match71, Is.equal_to,  196);  // A");
            int match72 = en7.MultiplyMatch(14, en2);  Assert.That(match72, Is.equal_to,  182);  // B");
            int match73 = en7.MultiplyMatch(14, en3);  Assert.That(match73, Is.equal_to,  365);  // AB");
            int match74 = en7.MultiplyMatch(14, en4);  Assert.That(match74, Is.equal_to,  364);  // BA");
            int match75 = en7.MultiplyMatch(14, en5);  Assert.That(match75, Is.equal_to,  509);  // ABC");
            int match76 = en7.MultiplyMatch(14, en6);  Assert.That(match76, Is.equal_to,  392);  // DEF");
            int match77 = en7.MultiplyMatch(14, en7);  Assert.That(match77, Is.equal_to, 1155);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match78 = en7.MultiplyMatch(14, en8);  Assert.That(match78, Is.equal_to, -616);  // VUTSRQPONMLKJIHGFEDCBA");
            int match79 = en7.MultiplyMatch(14, en9);  Assert.That(match79, Is.equal_to,  811);  // ABCDEF");

            int match81 = en8.MultiplyMatch(14, en1);  Assert.That(match81, Is.equal_to,  -98);  // A");
            int match82 = en8.MultiplyMatch(14, en2);  Assert.That(match82, Is.equal_to,  -84);  // B");
            int match83 = en8.MultiplyMatch(14, en3);  Assert.That(match83, Is.equal_to, -176);  // AB");
            int match84 = en8.MultiplyMatch(14, en4);  Assert.That(match84, Is.equal_to, -175);  // BA");
            int match85 = en8.MultiplyMatch(14, en5);  Assert.That(match85, Is.equal_to, -236);  // ABC");
            int match86 = en8.MultiplyMatch(14, en6);  Assert.That(match86, Is.equal_to, -119);  // DEF");
            int match87 = en8.MultiplyMatch(14, en7);  Assert.That(match87, Is.equal_to, -616);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match88 = en8.MultiplyMatch(14, en8);  Assert.That(match88, Is.equal_to, 1155);  // VUTSRQPONMLKJIHGFEDCBA");
            int match89 = en8.MultiplyMatch(14, en9);  Assert.That(match89, Is.equal_to, -328);  // ABCDEF");

            int match91 = en9.MultiplyMatch(14, en1);  Assert.That(match91, Is.equal_to,  196);  // A");
            int match92 = en9.MultiplyMatch(14, en2);  Assert.That(match92, Is.equal_to,  182);  // B");
            int match93 = en9.MultiplyMatch(14, en3);  Assert.That(match93, Is.equal_to,  365);  // AB");
            int match94 = en9.MultiplyMatch(14, en4);  Assert.That(match94, Is.equal_to,  364);  // BA");
            int match95 = en9.MultiplyMatch(14, en5);  Assert.That(match95, Is.equal_to,  509);  // ABC");
            int match96 = en9.MultiplyMatch(14, en6);  Assert.That(match96, Is.equal_to,  392);  // DEF");
            int match97 = en9.MultiplyMatch(14, en7);  Assert.That(match97, Is.equal_to,  811);  // ABCDEFGHIJKLMNOPQRSTUV");
            int match98 = en9.MultiplyMatch(14, en8);  Assert.That(match98, Is.equal_to, -328);  // VUTSRQPONMLKJIHGFEDCBA");
            int match99 = en9.MultiplyMatch(14, en9);  Assert.That(match99, Is.equal_to,  811);  // ABCDEF");

            string detail = Assert.Detail;

            _result += Assert.Conclusion;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void Endeme_Triangle_test()
        {
            Assert.ThingsAbout("Endeme", "Triangle");


            Assert.That(Endeme.Triangle(-9), Is.equal_to,    -45);
            Assert.That(Endeme.Triangle(-8), Is.equal_to,    -36);
            Assert.That(Endeme.Triangle(-7), Is.equal_to,    -28);
            Assert.That(Endeme.Triangle(-6), Is.equal_to,    -21);
            Assert.That(Endeme.Triangle(-5), Is.equal_to,    -15);
            Assert.That(Endeme.Triangle(-4), Is.equal_to,    -10);
            Assert.That(Endeme.Triangle(-3), Is.equal_to,     -6);
            Assert.That(Endeme.Triangle(-2), Is.equal_to,     -3);
            Assert.That(Endeme.Triangle(-1), Is.equal_to,     -1);
            Assert.That(Endeme.Triangle( 0), Is.equal_to,      0);
            Assert.That(Endeme.Triangle( 1), Is.equal_to,      1);
            Assert.That(Endeme.Triangle( 2), Is.equal_to,      3);
            Assert.That(Endeme.Triangle( 3), Is.equal_to,      6);
            Assert.That(Endeme.Triangle( 4), Is.equal_to,     10);
            Assert.That(Endeme.Triangle( 5), Is.equal_to,     15);
            Assert.That(Endeme.Triangle( 6), Is.equal_to,     21);
            Assert.That(Endeme.Triangle( 7), Is.equal_to,     28);
            Assert.That(Endeme.Triangle( 8), Is.equal_to,     36);
            Assert.That(Endeme.Triangle( 9), Is.equal_to,     45);
            Assert.That(Endeme.Triangle(10), Is.equal_to,     55);
            Assert.That(Endeme.Triangle(11), Is.equal_to,     66);
            Assert.That(Endeme.Triangle(12), Is.equal_to,     78);
            Assert.That(Endeme.Triangle(13), Is.equal_to,     91);
            Assert.That(Endeme.Triangle(14), Is.equal_to,    105);
            Assert.That(Endeme.Triangle(15), Is.equal_to,    120);

            string detail = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Accesssor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Accesssor_test()
        {
            Assert.ThingsAbout("Endeme", "Accessor");


            Endeme en = new Endeme(WetlandAnimals, "ABCDEFGHIJKLMNOPQRSTUV");
            int  posC = en['C'];  Assert.That(posC, Is.equal_to,  2 );
            char chaN = en[3]  ;  Assert.That(chaN, Is.equal_to, 'D');

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_AddOperator_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_AddOperator_test()
        {
            Assert.ThingsAbout("Endeme", "AddOperator");

            Endeme e1 = new Endeme(WetlandAnimals, "ABCD");
            Endeme e2 = new Endeme(WetlandAnimals, "CDEF");
            Endeme e3 = e1 + e2;
            Assert.That(e3.ToString(), Is.equal_to, "CDABEF");

            
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_AddToLetter_test -->
        /// <summary>
        ///      Tests the AddToLetter method in the Endeme class
        /// </summary>
        public void Endeme_AddToLetter_test()
        {
            Assert.ThingsAbout("Endeme", "AddToLetter");

            Endeme en = new Endeme(WetlandAnimals, "");
            en.Plus('A', 4);
            en.Plus('B', 3);
            en.Plus('C', 7);
            en.Plus('D', 5);
            en.CookEndeme();

            Assert.That(en.ToString(), Is.equal_to, "CDAB");

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Bag_test -->
        /// <summary>
        ///      Tests the Bag method in the Endeme class
        /// </summary>
        //[Test]
        public void Endeme_Bag_test()
        {
            Assert.ThingsAbout("Endeme", "Bag");


            Endeme endeme = new Endeme(WetlandAnimals, "RPIQLTSJAOUDBCFEHNKVMG");
            EndemeGrabBag bag;
            bag = new EndemeGrabBag(endeme, 64);  Assert.That(bag.Count, Is.equal_to, 64);
            bag = new EndemeGrabBag(endeme, 32);  Assert.That(bag.Count, Is.equal_to, 32);
            bag = new EndemeGrabBag(endeme, 16);  Assert.That(bag.Count, Is.equal_to, 16);
            bag = new EndemeGrabBag(endeme,  8);  Assert.That(bag.Count, Is.equal_to,  8);
            bag = new EndemeGrabBag(endeme,  4);  Assert.That(bag.Count, Is.equal_to,  4);
            bag = new EndemeGrabBag(endeme,  2);  Assert.That(bag.Count, Is.equal_to,  2);
            bag = new EndemeGrabBag(endeme,  1);  Assert.That(bag.Count, Is.equal_to,  1);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_BitwiseAnd_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_BitwiseAnd_test()
        {
            Assert.ThingsAbout("Endeme", "BitwiseAnd");


            Endeme_BitwiseAnd_testcase("ABCDE"   , "MOUSE"   , "E"     );
            Endeme_BitwiseAnd_testcase("ABCD"    , ""        , ""      );
            Endeme_BitwiseAnd_testcase(""        , "ABCD"    , ""      );
            Endeme_BitwiseAnd_testcase("DONALD"  , "DUCK"    , "D"     );
            Endeme_BitwiseAnd_testcase("JON"     , "GROVER"  , "O"     );
            Endeme_BitwiseAnd_testcase("MUTING"  , "FJORDS"  , ""      );
            Endeme_BitwiseAnd_testcase("ELEPHANT", "ALPHABET", "ELPHAT");

            _result += Assert.Conclusion;
        }
        private static void Endeme_BitwiseAnd_testcase(string endeme1, string endeme2, string tgt)
        {
            Endeme en1 = new Endeme(WetlandAnimals, endeme1);
            Endeme en2 = new Endeme(WetlandAnimals, endeme2);
            Endeme en3 = en1 & en2;
            Assert.That(en3, Is.equal_to, new Endeme(WetlandAnimals, tgt));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_BuildString_test -->
        /// <summary>
        ///      Tests BuildString, Also tests BuildIndex since these two methods are closely associated
        /// </summary>
        //[Test]
        public void Endeme_BuildString_test()
        {
            Assert.ThingsAbout("Endeme", "BuildString");


            //  Test BuildString and BuildIndex without an endeme set involved
            Endeme_BuildString_testcase();

            //  Test BuildString and BuildIndex with an endeme set involved
            Endeme_BuildString_testcase(WetlandAnimals);


            Endeme en = new Endeme();                                     
            en.TestBuildString("ABC");
            string idx = en.CollateIndex(",", 3);
            Assert.That(idx, Is.equal_to, "3,4,5");

            en = new Endeme();                                     
            en.TestBuildString("ABC".ToCharArray(), WetlandAnimals);

            string detail = Assert.Detail;


            _result += Assert.Conclusion;
        }
        private static void Endeme_BuildString_testcase(EndemeSet wetlandAnimals)
        {
            // create an empty endeme of WetlandAnimals
            Endeme e = new Endeme(wetlandAnimals);


            // use BuildString and the other std initializers
            string endeme = "ABCDCDEFGHIJKLM";
            e.TestBuildString(endeme);
            e.TestBuildIndex();
            //e.InitRaw();


            // check the index using a number
            int len = e.Count / 2;
            char c = e[len];
            int idx = e[c];
            Assert.That(idx, Is.equal_to, len);


            // check the index using a character
            char j = 'J';
            int idxj = e[j];
            char j2 = e[idxj];
            Assert.That(j2, Is.equal_to, j);


            // test to see that there are not duplicate characters
            Dictionary<char, int> test = new Dictionary<char, int>();
            for (int i = 0; i < e.Count; ++i)
            {
                char ctest = e[i];
                if (test.ContainsKey(ctest))
                    test[ctest] = 2;
                else test.Add(ctest, 1);
            }
            Assert.That(test.ContainsValue(2), Is.equal_to, false);
        }
        private static void Endeme_BuildString_testcase()
        {
            // create an empty endeme
            Endeme e = new Endeme();


            // use BuildString and the other std initializers
            string endeme = "ABCDCDEFGHIJKLM";
            e.TestBuildString(endeme);
            e.TestBuildIndex();
            //e.InitRaw();


            // check the index using a number
            int len = e.Count / 2;
            char c = e[len];
            int idx = e[c];
            Assert.That(idx, Is.equal_to, len);


            // check the index using a character
            char j = 'J';
            int idxj = e[j];
            char j2 = e[idxj];
            Assert.That(j2, Is.equal_to, j);


            // test to see that there are not duplicate characters
            Dictionary<char, int> test = new Dictionary<char, int>();
            for (int i = 0; i < e.Count; ++i)
            {
                char ctest = e[i];
                if (test.ContainsKey(ctest))
                    test[ctest] = 2;
                else test.Add(ctest, 1);
            }
            Assert.That(test.ContainsValue(2), Is.equal_to, false);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LongTests -->
        /// <summary>
        ///      Runs endeme namespace tests that take a long time to run
        /// </summary>
        /// <returns></returns>
        public string LongTests()
        {
           // Assert.ThingsAbout("InData", "DatabaseIdentity");
            string result = "";

            EndemeCompares_test();

            result += "\r\nEndeme tests succeeded";
            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        //[Test]
        public void Endeme_Constructor_test()
        {
            Assert.ThingsAbout("Endeme", "Constructor");


            EndemeConstructor_testcase1();

            EndemeConstructor_testcase2("ABCDEFGHIJKLMNOPQRSTUV"                    , 22, "ABCDEFGHIJKLMNOPQRSTUV"    );
            EndemeConstructor_testcase2("abcdefghijklmnopqrstuv"                    , 22, "ABCDEFGHIJKLMNOPQRSTUV"    );
            EndemeConstructor_testcase2("AABCDEFGHIJKLMNOPQRSTUV"                   , 22, "ABCDEFGHIJKLMNOPQRSTUV"    );
            EndemeConstructor_testcase2("ABCDEFGHIJKLMNOPQRSTUVWXYZ"                , 26, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            EndemeConstructor_testcase2(""                                          ,  0, ""                          );
            EndemeConstructor_testcase2("regf78o$y4trg745hgh(*^uihd"                , 11, "REGFOYTHUID"               );
                                                                                                                           
            EndemeConstructor_testcase3("ABCDEFGHIJKLMNOPQRSTUVWXYZ", WetlandAnimals, 22, "ABCDEFGHIJKLMNOPQRSTUV"    );
            EndemeConstructor_testcase3(""                          , WetlandAnimals,  0, ""                          );
            EndemeConstructor_testcase3("ABCEDA"                    , WetlandAnimals,  5, "ABCED"                     );

            _result += Assert.Conclusion;
        }
        private static void EndemeConstructor_testcase1()
        {
            //EndemeSet set = new EndemeSet();
            Endeme e = new Endeme();
            Assert.That(e.IsEmpty, Is.equal_to, true);
        }
        private static void EndemeConstructor_testcase2(string endeme, int length, string target)
        {
            Endeme e = new Endeme(endeme);
            int eLength = e.ToCharArray().Length;
            Assert.That(eLength, Is.equal_to, length);
            string str = e.ToString();
            Assert.That(str, Is.equal_to, target);
        }
        //private static void EndemeConstructor_testcase2(string endeme, int length, string target)
        //{
        //    EndemeSet set = new EndemeSet();
        //    Endeme e = new Endeme(set, endeme);
        //    int eLength = e.ToCharArray().Length;
        //    ok &= Assert.That(eLength, __.equals, length);
        //    string str = e.ToString();
        //    ok &= Assert.That(str, __.equals, target);
        //}
        private static void EndemeConstructor_testcase3(string endeme, EndemeSet set, int length, string target)
        {
            Endeme e = new Endeme(set, endeme);
            int eLength = e.ToCharArray().Length;
            Assert.That(eLength, Is.equal_to, length);
            string str = e.ToString();
            Assert.That(str, Is.equal_to, target);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Contains_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Contains_test()
        {
            Assert.ThingsAbout("Endeme", "Contains");


            Endeme en = new Endeme("ABC");
            Assert.That(en.Contains('Q'), Is.equal_to, false);
            Assert.That(en.Contains('B'), Is.equal_to, true);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Copy_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Copy_test()
        {
            Assert.ThingsAbout("Endeme", "Copy");


            Endeme_Copy_testcase("ABQNOPD");

            _result += Assert.Conclusion;
        }
        private static void Endeme_Copy_testcase(string endeme)
        {
            Endeme en1 = new Endeme(WetlandAnimals, endeme);
            Endeme en2 = en1.Copy();

            Assert.That(object.ReferenceEquals(en1,en2), Is.equal_to, false);
            Assert.That(en2, Is.equal_to, en1);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Count_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Count_test()
        {
            Assert.ThingsAbout("Endeme", "Count");

            Endeme en = new Endeme(WetlandAnimals, "ABQNOPD");
            Assert.That(en.Count, Is.equal_to, 7);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Difference_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Difference_test()
        {
            Assert.ThingsAbout("Endeme", "Difference");


            Endeme_Difference_testcase("ABCDE"   , "MOUSE"   , "MOUS"  );
            Endeme_Difference_testcase("ABCD"    , ""        , ""      );
            Endeme_Difference_testcase(""        , "ABCD"    , "ABCD"  );
            Endeme_Difference_testcase("DONALD"  , "DUCK"    , "UCK"   );
            Endeme_Difference_testcase("JON"     , "GROVER"  , "GRVE"  );
            Endeme_Difference_testcase("MUTING"  , "FJORDS"  , "FJORDS");
            Endeme_Difference_testcase("ELEPHANT", "ALPHABET", "BA"    );

            _result += Assert.Conclusion;
        }
        private static void Endeme_Difference_testcase(string endeme1, string endeme2, string tgt)
        {
            Endeme en1 = new Endeme(WetlandAnimals, endeme1);
            Endeme en2 = new Endeme(WetlandAnimals, endeme2);
            Endeme en3 = en1 % en2;
            Assert.That(en3, Is.equal_to, new Endeme(WetlandAnimals, tgt));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_First_Test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_First_Test()
        {
            Endeme en = new Endeme(WetlandAnimals);
            string first = en.First(3);
            EndemeSet set1 = en.EnSet;
            en = "ABC";
            first = en.First(3);
            EndemeSet set2 = en.EnSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Match_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Match_test()
        {
            Assert.ThingsAbout("Endeme", "Match");


            List<EndemeMatchTest> list = new List<EndemeMatchTest>();
		    Dictionary<string, EndemeTestItem> test = TestList();
		    Dictionary<double, List<string>> results = new Dictionary<double, List<string>>();


		    foreach (string name in test.Keys)
            {
			    double match = test[name].Endeme1.Match(test[name].Endeme2, WeightFormula.Refined);
			    if (!results.ContainsKey(match)) { results.Add(match, new List<string>()); }
			    results[match].Add(name);
		    }
		    int score = CalculateEffectiveness(results, test);

            Assert.That(score, Is.less_than_or_equal_to, 24);


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Minus_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Minus_test()
        {
            Assert.ThingsAbout("Endeme", "Minus");

            Endeme_Minus_testcase("ABCDE"   , "MOUSE"   , "MOUS"  );
            Endeme_Minus_testcase("ABCD"    , ""        , "ABCD"  );
            Endeme_Minus_testcase(""        , "ABCD"    , "ABCD"  );
            Endeme_Minus_testcase("DONALD"  , "DUCK"    , "UCK"   );
            Endeme_Minus_testcase("JON"     , "GROVER"  , "GRVE"  );
            Endeme_Minus_testcase("MUTING"  , "FJORDS"  , "FJORDS");
            Endeme_Minus_testcase("ELEPHANT", "ALPHABET", "BA"    );

            _result += Assert.Conclusion;
        }
        private static void Endeme_Minus_testcase(string endeme1, string endeme2, string tgt)
        {
            Endeme en1 = new Endeme(WetlandAnimals, endeme1);
            Endeme en2 = new Endeme(WetlandAnimals, endeme2);
            Endeme en3 = en1 - en2;
            Assert.That(en3, Is.equal_to, new Endeme(WetlandAnimals, tgt));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Endeme_Times_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Endeme_Times_test()
        {
            Assert.ThingsAbout("Endeme", "Times");

            Endeme_Times_testcase("ABCD"  , "CAB"   , "ACBD");
            Endeme_Times_testcase("ABC"   , "BACK"  , "ABCK", "BACK");
            Endeme_Times_testcase("MUTING", "FJORDS", "FMJUOTIRDNGS", "MFUJTORINDSG");
            Endeme_Times_testcase("ABC"   , ""      , "ABC");
            Endeme_Times_testcase(""      , "ABC"   , "ABC");

            _result += Assert.Conclusion;
        }
        private static void Endeme_Times_testcase(string lhs, string rhs, string tgt)
        {
            Endeme en1 = new Endeme(WetlandAnimals, lhs);
            Endeme en2 = new Endeme(WetlandAnimals, rhs);
            Endeme en3 = en1 * en2;
            Assert.That(en3.ToString(), Is.equal_to, tgt);
        }
        private static void Endeme_Times_testcase(string lhs, string rhs, string tgt1, string tgt2)
        {
            Endeme en1 = new Endeme(WetlandAnimals, lhs);
            Endeme en2 = new Endeme(WetlandAnimals, rhs);
            Endeme en3 = en1 * en2;

            Assert.That(en3.ToString(), Is.equal_to, tgt1).Or(en3.ToString(), Is.equal_to, tgt2);
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- EndemeCompares_test -->
        /// <summary>
        ///      Matches the endeme pairs for current match?
        /// </summary>
        /// <remarks></remarks>
        public void EndemeCompares_test()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();


            if (true)
            {
		        Dictionary<string, EndemeTestItem> test = TestList();
                foreach (string name in test.Keys) AddMatch_testcase(test, name, 5.0, 2.4, 1.0, 5.0);
                EndemeTestItem.RankResults(test);
                result.Add("AddMatch", EndemeTestItem.CalculateTable(test) + "\r\n" + EndemeTestItem.EffectivenessOf(test));
            }


	        if (true)
            {
		        Dictionary<string, EndemeTestItem> test = TestList();
                foreach (string name in test.Keys) RegularMatch_testcase(test, name);
                EndemeTestItem.RankResults(test);
                result.Add("RegularMatch", EndemeTestItem.CalculateTable(test) + "\r\n" + EndemeTestItem.EffectivenessOf(test));
	        }


	        List<string> rptn = new List<string>();
            string brief = "";
            string delim = "";
            for (int g = 0; g < 8; ++g)
	        {
                for (int d = 0; d < 7; ++d)
                {
                  //Dictionary<double, List<string>> results = new Dictionary<double, List<string>>();
                    double geom = 0.81  + (double)g / 200.0;
                    double dir2 = 0.014 + (double)d / 1000.0;


                    Dictionary<string, EndemeTestItem> test = TestList();
                    foreach (string name in test.Keys)  MatchTestMatch_testcase(test, name, geom, 214.0, 428.0, 700.0, 12500.0, dir2);
                    EndemeTestItem.RankResults(test);
                    int m = EndemeTestItem.EffectivenessOf(test);
                    rptn.Add("Geom:" + geom + "  " + "Dir2:" + dir2 + "\r\n" + EndemeTestItem.CalculateTable(test) + "\r\n" + m);


                    brief += delim + "Match:" + m + "  " + "Geom:" + geom.ToString().PadRight(5,'0') + "  " + "Dir2:" + dir2.ToString().PadRight(5,'0');
                    delim = "\r\n";
                }
	        }
            result.Add("MatchTestMatch", brief);


	        // -------------------------------------------------------------------------------
	        //  Match the endeme pairs using Levenshtein Distance
	        // -------------------------------------------------------------------------------
	        if (true)
            {
		        Dictionary<string, EndemeTestItem> test = TestList();
		        foreach (string name in test.Keys)  EnLevenshteinMatch_testcase(test, name);
                EndemeTestItem.RankResults(test);
                result.Add("LevenshteinMatch", EndemeTestItem.CalculateTable(test) + "\r\n" + EndemeTestItem.EffectivenessOf(test));
	        }


            //// -------------------------------------------------------------------------------
            ////  Match the endeme pairs for match2
            //// -------------------------------------------------------------------------------
            //string rpt1 = "";
            //if (true) {
            //    Dictionary<string, EndemeTestItem> test = TestList();
            //    Dictionary<double, List<string>> results = new Dictionary<double, List<string>>();
            //    foreach (string name in test.Keys) {
            //        double match = test[name].Endeme1.Match2(test[name].Endeme2);
            //        if (!results.ContainsKey(match)) {
            //            results.Add(match, new List<string>());
            //        }
            //        results[match].Add(name);
            //    }
            //    int m = CalculateEffectiveness(results, test);
            //    rpt1 = CalculateTable(results, test) + "\r\n" + m;
            //    _result = _result + "\r\n" + "Match2:" + m;
            //}


	        // -------------------------------------------------------------------------------
	        //  Match the endeme pairs for SimpleMatch
	        // -------------------------------------------------------------------------------
	        if (true)
            {
		        Dictionary<string, EndemeTestItem> test = TestList();
		        foreach (string name in test.Keys)  SimpleMatch_testcase(test, name);
                EndemeTestItem.RankResults(test);
                result.Add("Simple", EndemeTestItem.CalculateTable(test) + "\r\n" + EndemeTestItem.EffectivenessOf(test));
	        }


            _oldResult += "\r\n\r\n" + "AddMatch"         +":" + "\r\n" + result["AddMatch"        ];
            _oldResult += "\r\n\r\n" + "RegularMatch"     +":" + "\r\n" + result["RegularMatch"    ];
            _oldResult += "\r\n\r\n" + "MatchTestMatch"   +":" + "\r\n" + result["MatchTestMatch"  ];
            _oldResult += "\r\n\r\n" + "LevenshteinMatch" +":" + "\r\n" + result["LevenshteinMatch"];
            _oldResult += "\r\n\r\n" + "Simple"           +":" + "\r\n" + result["Simple"          ];


            // -------------------------------------------------------------------------------
            //  Match the endeme pairs for FibonMatch 1
            // -------------------------------------------------------------------------------
            string str = "";
            double bestMatch = 100000.0;
            for (int i = 0; i <= 23; i++)
            {
                double m = EndemeFibonMatch1_testcase(FibonValue[i]);
                if (m < bestMatch)
                {
                    bestMatch = m;
                }
                str = str + "\r\n" + FibonValue[i].ToString().PadLeft(5) + " " + EndemeFibonMatch1_testcase(FibonValue[i]).ToString().PadLeft(3);
            }
            _oldResult = _oldResult + "\r\n" + "Fibon1:" + "Best - Score:" + bestMatch;


         //   // -------------------------------------------------------------------------------
         //   //  Match the endeme pairs for FibonMatch 1
         //   // -------------------------------------------------------------------------------
         //   string str2 = "";
	        //double bestMatch2 = 100000.0;
	        //for (int i = 0; i <= 23; i++)
         //   {
	        //    FibonValue[Endeme.NO_INDEX] = FibonValue[i];
	        //    string rpt4 = "";
	        //    int m = 0;
	        //    if (true)
         //       {
		       //     Dictionary<string, EndemeTestItem> test = TestList();
		       //     foreach (string name in test.Keys)
         //               test[name].Result = FibonMatch1(test[name].Endeme1, test[name].Endeme2);
		       //     m = EndemeTestItem.EffectivenessOf(test);
         //           rpt4 = m + "\r\n" + EndemeTestItem.CalculateTable(test);
	        //    }

		       // if (m < bestMatch2)
         //       {
			      //  bestMatch2 = m;
		       // }
		       // str2 = str2 + "\r\n" + FibonValue[i].ToString().PadLeft(5) + " " + EndemeFibonMatch1_testcase(FibonValue[i]).ToString().PadLeft(3);
	        //}
	        //_result = _result + "\r\n" + "Fibon5:" + "Best - Score:" + bestMatch2;


	        // -------------------------------------------------------------------------------
	        //  Match the endeme pairs for FibonMatch 2,3,4
	        // -------------------------------------------------------------------------------
	        _oldResult = _oldResult + "\r\n" + "Fibon2:" + FibonMatch2_test();
	        _oldResult = _oldResult + "\r\n" + "Fibon3:" + FibonMatch3_test();
	        _oldResult = _oldResult + "\r\n" + "Fibon4:" + FibonMatch4_test();


        }

        // -----------------------------------------------------------------------------------------
        //  Fibon 1
        private static int EndemeFibonMatch1_testcase(double defaultNum)
        {
	        FibonValue[Endeme.NO_INDEX] = defaultNum;
	        string rpt4 = "";
	        int m = 0;
	        if (true)
            {
		        Dictionary<string, EndemeTestItem> test = TestList();
		        Dictionary<double, List<string>> results = new Dictionary<double, List<string>>();
		        foreach (string name in test.Keys)
                {
			        double match = FibonMatch1(test[name].Endeme1, test[name].Endeme2);
                    test[name].Result = match;
			        if (!results.ContainsKey(match)) { results.Add(match, new List<string>()); }
			        results[match].Add(name);
		        }
		        m = EndemeTestItem.EffectivenessOf(test);
                m = CalculateEffectiveness(results, test);
		        rpt4 = m + "\r\n" + CalculateTable(results, test);
	        }
	        return m;
        }

        // -----------------------------------------------------------------------------------------
        //  Fibon 2
        public string FibonMatch2_test()
        {

	        // -------------------------------------------------------------------------------
	        //  Match the endeme pairs for FibonMatch 2
	        // -------------------------------------------------------------------------------
	      //string str2 = "";
	        //str2 = str2 && AntiFiboValue(0).ToString()
	        Random r = RandomSource.New().Random;
	        int bestScore = 1000;
	        double bestDefault = 50.0;
	        double bestAntiPct = 0.5;
	        double bestGeometric = 0.5;
	        double bestAntiGeom = 0.2;

	        double total = 0.0;
	        double count = 0.0;
	        double num = 0.0;

	        for (int j = 0; j <= 1000; j++)
            {
		        double antiPct = 0.0;
		        // r.NextDouble/100.0

		        double geometric = r.NextDouble();
		        double antiGeom = r.NextDouble();

		        SetFibonValue(20000.0, geometric);
		        SetAntiFiboValue(antiGeom);
		        int i = r.Next(23);
		        double defaultNum = FibonValue[i];


		        int score = EndemeFibonMatch2_testcase(defaultNum, antiPct);
		        //str2 = str2 _
		        //    & VbCrLf & FibonValue(i).ToString().PadLeft(5) _
		        //    & " " & score.ToString().PadLeft(3) _
		        //    & " " & Convert.ToInt32(100.0 * antiPct).ToString() & "%"

		        if (score < bestScore)
                {
			        bestScore     = score;
			        bestDefault   = defaultNum;
			        bestAntiPct   = antiPct;
			        bestGeometric = geometric;
			        bestAntiGeom  = antiGeom;
		        }

		        if (score <= 48) {
			        total = total + geometric;
			        count = count + 1.0;
			        num   = num + defaultNum;
		        }
	        }

	        double aScore = total / count;
	        double aNum = num / count;

	        string fib2Result = "Best -" + "  Score:" + Truncate(bestScore.ToString(), 6) + "  Default Num:" + Truncate(bestDefault.ToString(), 6) + "  Percent:" + Truncate(bestAntiPct.ToString(), 6) + "  Geometric:" + Truncate(bestGeometric.ToString(), 6) + "  AntiGeom:" + Truncate(bestAntiGeom.ToString(), 6);

	        return fib2Result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Truncate -->
        /// <summary>
        ///      Truncates a string if it is longer than the max length
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Truncate(string str, int maxlen)
        {
            if (str.Length > maxlen)
                return str.Substring(0, maxlen);
            else
                return str;
        }

        private static int EndemeFibonMatch2_testcase(double defaultNum, double percent)
        {
	        FibonValue[Endeme.NO_INDEX] = defaultNum;
	        AntiFiboValue[Endeme.NO_INDEX] = 0.0;
	        string rpt4 = "";
	        int m = 0;
	        if (true) {
		        Dictionary<string, EndemeTestItem> test = TestList();
		        Dictionary<double, List<string>> results = new Dictionary<double, List<string>>();
		        foreach (string name in test.Keys) {
			        double match = FibonMatch2(test[name].Endeme1, test[name].Endeme2, percent);
                    test[name].Result = match;
			        if (!results.ContainsKey(match)) {
				        results.Add(match, new List<string>());
			        }
			        results[match].Add(name);
		        }
                m = EndemeTestItem.EffectivenessOf(test);
		        m = CalculateEffectiveness(results, test);
		        rpt4 = CalculateTable(results, test) + "\r\n" + m + "    " + (100.0 * percent).ToString() + "%";
	        }
	        return m;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- FibonMatch2 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double FibonMatch2(Endeme endeme1, Endeme endeme2, double percent)
        {

	        char[] c = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
	        double total = 0.0;
	        for (int i = 0; i <= c.Length - 1; i++) {
		        int idx1 = endeme1.Index(c[i]);
		        int idx2 = endeme2.Index(c[i]);
		        double pair = FibonValue[idx1] * FibonValue[idx2];
		        total = total + pair;
	        }

	        Endeme endeme1r = endeme1.Reverse;
	        Endeme endeme2r = endeme2.Reverse;
	        double anti = 0.0;
	        for (int i = 0; i <= c.Length - 1; i++) {
		        int idx1 = endeme1.Index(c[i]);
		        int idx2 = endeme2.Index(c[i]);
		        double pair = AntiFiboValue[idx1] * AntiFiboValue[idx2];
		        anti = anti + pair;
	        }

	        return total - percent * anti;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- Fibon 3 -->
        public string FibonMatch3_test()
        {

	        // -------------------------------------------------------------------------------
	        //  Match the endeme pairs for FibonMatch 3
	        // -------------------------------------------------------------------------------
	        //string str2 = "";
	        Random r = RandomSource.New().Random;
	        int bestScore = 1000000;
	        double bestOneMissed = 50.0;
	        double bestBothMissed = 50.0;
	        double bestGeometric = 0.5;


	        double total = 0.0;
	        double count = 0.0;
	        double num1 = 0.0;
	        double num2 = 0.0;
	        double totGeom = 0.0;
	        double totScore = 0.0;

	        for (int j = 0; j <= 200; j++) {

		        //Dim geometric  As Double  = r.NextDouble
		        //SetFibonValue(geometric)
		        //Dim oneMissed  As Double = FibonValue(r.Next(22))*FibonValue(r.Next(22)) : If r.Next(2) = 0 Then : oneMissed  = -1.0 * oneMissed  : End If
		        //Dim bothMissed As Double = FibonValue(r.Next(22))*FibonValue(r.Next(22)) : If r.Next(2) = 0 Then : bothMissed = -1.0 * bothMissed : End If


		        double geometric = 0.6 + r.NextDouble() / 10.0;
		        // = 0.6494
		        SetFibonValue(20000.0, geometric);
		        double oneMissed = 2000000.0 * r.NextDouble();
		        // + 600000.0 ' = 840000.0
		        double bothMissed = oneMissed;
		        //(r.NextDouble()/100.0 + 0.995D)



		        int score = EndemeFibonMatch3_testcase(oneMissed, bothMissed);

		        if (score < bestScore) {
			        bestScore = score;
			        bestOneMissed = oneMissed;
			        bestGeometric = geometric;
			        bestBothMissed = bothMissed;
		        }

		        if (score <= 44) {
			        total = total + geometric;
			        count = count + 1.0;
			        num1 = num1 + oneMissed;
			        num2 = num2 + bothMissed;
			        totGeom = totGeom + geometric;
			        totScore = totScore + score;
		        }
	        }

	        double aScore = totScore / count;
	        double aNum1 = num1 / count;
	        double aNum2 = num2 / count;
	        double aGeom = totGeom / count;


	        string fib2Result = "Best -" + "  Score:" + Truncate(aScore.ToString(), 7) + "  Geometric:" + Truncate(aGeom.ToString(), 7) + "  1 missed:" + Truncate(aNum1.ToString(), 7) + "  both missed:" + Truncate(aNum2.ToString(), 7) + "  count:" + Truncate(count.ToString(), 7);

	        return fib2Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oneMissingNum"></param>
        /// <param name="bothMissingNum"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static int EndemeFibonMatch3_testcase(double oneMissingNum, double bothMissingNum)
        {
	        FibonValue[Endeme.NO_INDEX] = 0.0;

	        string rpt4 = "";
	        int m = 0;
	        if (true) {
		        Dictionary<string, EndemeTestItem> test = TestList();
		        Dictionary<double, List<string>> results = new Dictionary<double, List<string>>();
		        foreach (string name in test.Keys) {
			        double match = FibonMatch3(test[name].Endeme1, test[name].Endeme2, oneMissingNum, bothMissingNum);
                    test[name].Result = match;
			        if (!results.ContainsKey(match)) {
				        results.Add(match, new List<string>());
			        }
			        results[match].Add(name);
		        }
		        m = CalculateEffectiveness(results, test);
		      //m = EndemeTestItem.EffectivenessOf(test);
		        rpt4 = CalculateTable(results, test) + "\r\n" + m;
	        }
	        return m;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- FibonMatch3 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double FibonMatch3(Endeme endeme1, Endeme endeme2, double oneMissingNum, double bothMissingNum)
        {

	        char[] c = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
	        double total = 0.0;
	        for (int i = 0; i <= c.Length - 1; i++) {
		        int idx1 = endeme1.Index(c[i]);
		        int idx2 = endeme2.Index(c[i]);

		        double pair = 0.0;
		        if (idx1 == Endeme.NO_INDEX || idx2 == Endeme.NO_INDEX) {
			        pair = oneMissingNum;
		        } else {
			        pair = FibonValue[idx1] * FibonValue[idx2];
		        }

		        total = total + pair;
	        }

	        return total;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- Fibon 4 -->
        public string FibonMatch4_test()
        {

	        // -------------------------------------------------------------------------------
	        //  Match the endeme pairs for FibonMatch 4
	        // -------------------------------------------------------------------------------
	      //string str2 = "";
	        Random r = RandomSource.New().Random;

	        int    bestScore     = 10000; double totScore  = 0.0;
            double bestStart     =  0.0;  double totStart  = 0.0;
	        double bestGeometric =  0.5;  double totGeom   = 0.0;
	        double bestOneMissed = 50.0;  double totOne1   = 0.0;
	        double bestBothMiss  = 50.0;  double totBoth2  = 0.0;
	        double bestDirect    =  0.0;  double totDirect = 0.0;
	        double bestOneBoost  =  0.0;  double totBoost  = 0.0;
            double bestSizer     =  0.0;  double totSizer  = 0.0;
            string bestReport    = ""  ;


	        double count = 0.0;


	        for (int j = 0; j <= 1000; j++)
            {
		        //Dim geometric  As Double  = r.NextDouble
		        //SetFibonValue(geometric)
		        //Dim oneMissed  As Double = FibonValue(r.Next(22))*FibonValue(r.Next(22)) : If r.Next(2) = 0 Then : oneMissed  = -1.0 * oneMissed  : End If
		        //Dim bothMissed As Double = FibonValue(r.Next(22))*FibonValue(r.Next(22)) : If r.Next(2) = 0 Then : bothMissed = -1.0 * bothMissed : End If


                double starter    =   100.0; // 235.0  + 10.0  * r.NextDouble();
		        double geometric  =    0.84; // 0.835 +  0.01 * r.NextDouble(); // = 0.6494 //  = 0.72945
		        SetFibonValue(starter, geometric);

                double oneMissed  =   214.0; //   210.0 +     10.0 * r.NextDouble(); // 600000.0 ' = 840000.0 //  = 575000.0;
		        double bothMissed =   428.0; //   425.0 +      5.0 * r.NextDouble(); // oneMissed * 1.0;		    // (r.NextDouble()/100.0 + 0.995D)
                double sizer      =   700.0; //   690.0 +     20.0 * r.NextDouble();
		        double oneBoost   =  5000.0; //     1.0 +   10000.0 * r.NextDouble(); // = 208500000.0;
		        double direct     = 20000.0; // 19800.0 +    400.0 * r.NextDouble(); // ' 2000.0 * r.NextDouble - 1000.0 // = 475500000.0
                //Fibon4:Best - Score:10 Start:100 Geom:0.840000 1-missed:214 2-missed:428 sizer:700 1-boost:5000 direct:20000 count:1001
                //Fibon4:Best - Score:14 Start:100 Geom:0.840000 1-missed:214 2-missed:428 sizer:700 1-boost:5000 direct:20000 count:1001
                //Fibon4:Best - Score:14 Start:100 Geom:0.840000 1-missed:214 2-missed:428 sizer:700 1-boost:5021.74 direct:20000 count:175
                //Fibon4:Best - Score:14 Start:100 Geom:0.839999 1-missed:215.09 2-missed:428 sizer:700 1-boost:5383.94 direct:20000 count:785
                //Fibon4:Best - Score:14 Start:100 Geom:0.839999 1-missed:215.30 2-missed:428 sizer:700 1-boost:5390.51 direct:20000 count:788
                //Fibon4:Best - Score:14 Start:100 Geom:0.839999 1-missed:225.10 2-missed:428 sizer:700 1-boost:5389.51 direct:20000 count:381
                //Fibon4:Best - Score:14 Start:100 Geom:0.840000 1-missed:238.89 2-missed:427.66 sizer:700 1-boost:5498.83 direct:20000 count:868
                //Fibon4:Best - Score:14 Start:100 Geom:0.840000 1-missed:237.91 2-missed:427.87 sizer:700 1-boost:5501.40 direct:20000 count:266
                //Fibon4:Best - Score:14 Start:100 Geom:0.839999 1-missed:234.84 2-missed:427.67 sizer:700.031 1-boost:5498.22 direct:20000 count:656
                //Fibon4:Best - Score:14 Start:100 Geom:0.839999 1-missed:235.38 2-missed:428.47 sizer:698.620 1-boost:5497.19 direct:20000 count:503
                //Fibon4:Best - Score:14 Start:100 Geom:0.839999 1-missed:234.59 2-missed:428.19 sizer:710.160 1-boost:5494.31 direct:20000 count:424
                //Fibon4:Best - Score:14 Start:100 Geom:0.840000 1-missed:234.89 2-missed:427.62 sizer:711.384 1-boost:5609.40 direct:20004.09 count:271
                //Fibon4:Best - Score:14 Start:100 Geom:0.840000 1-missed:232.30 2-missed:425.94 sizer:689.891 1-boost:5539.94 direct:20041.64 count:178
                //Fibon4:Best - Score:14 Start:100 Geom:0.840000 1-missed:227.02 2-missed:421.69 sizer:677.905 1-boost:5383.91 direct:20213.81 count:101
                //Fibon4:Best - Score:14 Start:100 Geom:0.84 1-missed:222.35 2-missed:412.11 sizer:658.391 1-boost:5316.42 direct:20791.07 count:5
                //Fibon4:Best - Score:14 Start:100 Geom:0.840203 1-missed:221.12 2-missed:432.16 sizer:744.288 1-boost:5494.68 direct:19746.76 count:4
                //Fibon4:Best - Score:15.8 Start:100 Geom:0.840469 1-missed:242.72 2-missed:411.47 sizer:586.924 1-boost:5640.09 direct:19169.51 count:67
                //Fibon4:Best - Score:16 Start:100 Geom:0.839260 1-missed:262.16 2-missed:424.17 sizer:545.665 1-boost:5914.83 direct:19285.48 count:2
                //Fibon4:Best - Score:17.3 Start:100 Geom:0.835224 1-missed:233.69 2-missed:367.08 sizer:450.024 1-boost:5691.51 direct:18260.82 count:6
                //Fibon4:Best - Score:18 Start:100 Geom:0.830700 1-missed:202.70 2-missed:302.33 sizer:377.153 1-boost:6263.69 direct:15834.51 count:1
                //Fibon4:Best - Score:19.5 Start:100 Geom:0.834958 1-missed:371.38 2-missed:454.29 sizer:260.840 1-boost:5992.09 direct:19493.42 count:4
                //Fibon4:Best - Score:20 Start:100 Geom:0.840700 1-missed:256.36 2-missed:389.53 sizer:597.433 1-boost:4582.24 direct:23454.50 count:0
                //Fibon4:Best - Score:20 Start:100 Geom:0.847511 1-missed:439.27 2-missed:602.56 sizer:587.163 1-boost:6191.22 direct:17376.14 count:0
                //Fibon4:Best - Score:22 Start:100 Geom:0.846675 1-missed:524.86 2-missed:597.67 sizer:294.913 1-boost:5797.88 direct:19409.56 count:0
                //Fibon4:Best - Score:26 Start:100 Geom:0.813865 1-missed:125.29 2-missed:282.36 sizer:662.250 1-boost:6176.36 direct:22217.04 count:0
                //Fibon4:Best - Score:20 Start:200 Geom:0.836403 1-missed:816.85 2-missed:1292.0 sizer:1982.28 1-boost:9432.25 direct:102820.6 count:0
                //Fibon4:Best - Score:18 Start:240 Geom:0.824 1-missed:810.82 2-missed:1773.1 sizer:3345.57 1-boost:11128.0 direct:108589.2 count:172
                //Fibon4:Best - Score:18 Start:237.9 Geom:0.824361 1-missed:821.48 2-missed:1761.1 sizer:3267.85 1-boost:10927.6 direct:107850.9 count:62
                //Fibon4:Best - Score:18 Start:239.4 Geom:0.823836 1-missed:802.53 2-missed:1804.1 direct:105511.4 1-boost:10980.8 sizer:3508.67 count:354
                //Fibon4:Best - Score:19.8 Start:239.4 Geom:0.824823 1-missed:801.27 2-missed:1799.5 direct:105046.2 1-boost:11006.1 sizer:3493.92 count:919
                //Fibon4:Best - Score:22 Start:239.3 Geom:0.823727 1-missed:802.25 2-missed:1799.4 direct:105347.7 1-boost:11047.6 sizer:3517.66 count:420
                //Fibon4:Best - Score:22 Start:239.5 Geom:0.823503 1-missed:804.98 2-missed:1800.5 direct:105510.9 1-boost:11192.7 sizer:3497.78 count:956
                //Fibon4:Best - Score:22 Start:239.7 Geom:0.823499 1-missed:804.43 2-missed:1798.7 direct:105758.8 1-boost:10996.6 sizer:3498.33 count:622
                //Fibon4:Best - Score:22 Start:239.4 Geom:0.823463 1-missed:801.14 2-missed:1799.1 direct:105309.0 1-boost:10967.0 sizer:3520.17 count:486
                //Fibon4:Best - Score:22 Start:238.9 Geom:0.823812 1-missed:809.03 2-missed:1774.7 direct:106868.1 1-boost:10961.8 sizer:3526.29 count:150
                //Fibon4:Best - Score:22 Start:239.9 Geom:0.822351 1-missed:810.71 2-missed:1757.8 direct:105328.1 1-boost:10582.2 sizer:3673.25 count:19
                //Fibon4:Best - Score:22 Start:236.1 Geom:0.823060 1-missed:790.92 2-missed:1758.3 direct:99563.89 1-boost:10136.1 sizer:3775.92 count:6
                //Fibon4:Best - Score:23.6 Start:236.1 Geom:0.826991 1-missed:694.53 2-missed:1820.7 direct:113168.3 1-boost:10262.7 sizer:3977.32 count:26
                //Fibon4:Best - Score:23.3 Start:218.5 Geom:0.814632 1-missed:591.41 2-missed:1413.2 direct:101876.2 1-boost:9358.05 sizer:2915.34 count:9
                //Fibon4:Best - Score:25.2 Start:212.5 Geom:0.822165 1-missed:471.77 2-missed:1387.7 direct:94177.62 1-boost:7239.85 sizer:3143.03 count:10
                //Fibon4:Best - Score:27.6666 Start:220.4 Geom:0.8130293 1-missed:610.3028 2-missed:1454.257 direct:91480.2749 1-boost:5021.56237 sizer:2834.33482 count:24
                //Fibon4:Best - Score:29.75 Start:199.6 Geom:0.8204564 1-missed:502.4873 2-missed:1536.299 direct:66161.8591 1-boost:5899.57555 sizer:3702.72472 count:8
                //Fibon4:Best - Score:32 Start:183.7 Geom:0.8080350 1-missed:300.1622 2-missed:1384.262 direct:45758.8117 1-boost:4331.30266 sizer:4193.20998 count:0
                //Fibon4:Best - Score:32 Start:176.7 Geom:0.7952038 1-missed:831.4626 2-missed:1217.809 direct:41299.0573 1-boost:8986.31873 sizer:1255.32357 count:0
                //Fibon4:Best - Score:60 Start:141.9 Geom:0.7927021 1-missed:529.2668 2-missed:588.2692 direct:28740.9234 1-boost:7350.19572 sizer:13376.7265 count:0
                //Fibon4:Best - Score:88 Start:140.4 Geom:0.7599331 1-missed:1935.076 2-missed:1229.463 direct:26359.3443 1-boost:10850.7561 sizer:8246.70687 count:0
                //Fibon4:Best - Score:24 Start:1406. Geom:0.7645957 1-missed:40792.96 2-missed:44197.48 direct:3955031.12 1-boost:108965.825 sizer:7644.12683 count:0
                //Fibon4:Best - Score:22 Start:1954. Geom:0.7568232 1-missed:37680.00 2-missed:37830.28 direct:3720273.86 1-boost:2824275.42 sizer:635.808210 count:0
                //Fibon4:Best - Score:22 Start:18283 Geom:0.7321937 1-missed:595948.2 2-missed:597547.8 direct:430770527. 1-boost:156622341. sizer:1434.74028 count:0
                //Fibon4:Best - Score:22 Start:17768 Geom:0.7312085 1-missed:462576.7 2-missed:529807.0 direct:414073354. 1-boost:151348754. sizer:17.6007510 count:0
		        //Fibon4:Best -  Score:6  Geometric:0.7294496  1 missed:574993.9  both missed:574993.9  direct:475489068.  singleboost:208493462.  count:1001
		        //Fibon4:Best -  Score:6  Geometric:0.7294577  1 missed:575097.5  both missed:575097.5  direct:475507764.  singleboost:208303844.  count:482
		        //Fibon4:Best -  Score:6  Geometric:0.7294673  1 missed:574879.0  both missed:574879.0  direct:475653181.  singleboost:208352630.  count:180
		        //Fibon4:Best -  Score:6  Geometric:0.7294821  1 missed:575379.7  both missed:575379.7  direct:475878467.  singleboost:208284845.  count:280


                string rpt = EndemeFibonMatch4_testcase(oneMissed, bothMissed, direct, bestScore, oneBoost, sizer);
		        int score = int.Parse(Regex.Replace(rpt, @"^\s*([0-9]+)\s+.*$", "$1", RegexOptions.Singleline));

		        if (score < bestScore)
                {
			        bestScore     = score     ;
                    bestStart     = starter   ;
			        bestGeometric = geometric ;
			        bestOneMissed = oneMissed ;
			        bestBothMiss  = bothMissed;
			        bestDirect    = direct    ;
			        bestOneBoost  = oneBoost  ;
                    bestSizer     = sizer     ;
                    bestReport    = rpt       ;
		        }

		        if (score <= 14)
                {
			        count     = count     + 1.0       ;
                    totStart  = totStart  + starter   ;
			        totOne1   = totOne1   + oneMissed ;
			        totBoth2  = totBoth2  + bothMissed;
			        totGeom   = totGeom   + geometric ;
			        totScore  = totScore  + score     ;
			        totDirect = totDirect + direct    ;
			        totBoost  = totBoost  + oneBoost  ;
                    totSizer  = totSizer  + sizer     ;
		        }
	        }


            string fib2Result = "Best -";
            if (count > 0)
            {
	            double avgScore  = totScore  / count;
	            double avgOne1   = totOne1   / count;
	            double avgBoth2  = totBoth2  / count;
	            double avgGeom   = totGeom   / count;
	            double avgDirect = totDirect / count;
	            double avgBoost  = totBoost  / count;
                double avgSizer  = totSizer  / count;
                double avgStart  = totStart  / count;


	            fib2Result = fib2Result
                    + " Score:"     + Truncate(avgScore .ToString(), 4)
                    + " Start:"     + Truncate(avgStart .ToString(), 5)
                    + " Geom:"      + Truncate(avgGeom  .ToString(), 8)
                    + " 1-missed:"  + Truncate(avgOne1  .ToString(), 6)
                    + " 2-missed:"  + Truncate(avgBoth2 .ToString(), 6)
                    + " sizer:"     + Truncate(avgSizer .ToString(), 7)
                    + " 1-boost:"   + Truncate(avgBoost .ToString(), 7)
                    + " direct:"    + Truncate(avgDirect.ToString(), 8)
                    + " count:"     + Truncate(count    .ToString(), 5);
            }
            else
            {
	            fib2Result = fib2Result
                    + " Score:"     + Truncate(bestScore    .ToString(), 4)
                    + " Start:"     + Truncate(bestStart    .ToString(), 5)
                    + " Geom:"      + Truncate(bestGeometric.ToString(), 8)
                    + " 1-missed:"  + Truncate(bestOneMissed.ToString(), 6)
                    + " 2-missed:"  + Truncate(bestBothMiss .ToString(), 6)
                    + " sizer:"     + Truncate(bestSizer    .ToString(), 7)
                    + " 1-boost:"   + Truncate(bestOneBoost .ToString(), 7)
                    + " direct:"    + Truncate(bestDirect   .ToString(), 8)
                    + " count:"     + Truncate(count        .ToString(), 5);
            }


	        return fib2Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oneMissingNum"></param>
        /// <param name="bothMissingNum"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string EndemeFibonMatch4_testcase(double oneMissingNum, double bothMissingNum, double directMatch, int bestScore, double singleBoost, double something)
        {
	        FibonValue[Endeme.NO_INDEX] = 0.0;

	        string rpt4 = "";
	        int m = 0;
	        if (true)
            {
		        Dictionary<string, EndemeTestItem> test = TestList();
		        Dictionary<double, List<string>> results = new Dictionary<double, List<string>>();
		        foreach (string name in test.Keys)
                {
			        double match = FibonMatch4(test[name].Endeme1, test[name].Endeme2, oneMissingNum, bothMissingNum, directMatch, singleBoost, something);
                    test[name].Result = match;
			        if (!results.ContainsKey(match)) { results.Add(match, new List<string>()); } results[match].Add(name);
		        }
		        m = CalculateEffectiveness(results, test);
              //m = EndemeTestItem.EffectivenessOf(test);
		        rpt4 = m + "\r\n"
                    + " Score:"     + Truncate(m             .ToString(), 4)
                    + " Start:"     + Truncate(""            .ToString(), 5)
                    + " Geom:"      + Truncate(""            .ToString(), 8)
                    + " 1-missed:"  + Truncate(oneMissingNum .ToString(), 6)
                    + " 2-missed:"  + Truncate(bothMissingNum.ToString(), 6)
                    + " sizer:"     + Truncate(something     .ToString(), 7)
                    + " 1-boost:"   + Truncate(singleBoost   .ToString(), 7)
                    + " direct:"    + Truncate(directMatch   .ToString(), 8)
                    + " count:"     + Truncate(1             .ToString(), 5)
                    + CalculateTable(results, test);
		        if (m < bestScore)
                {
			        Pause();
		        }
	        }
	        return rpt4;
        }

        public static void Pause()
        {
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- FibonMatch4 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double FibonMatch4(Endeme endeme1, Endeme endeme2, double oneMissing, double bothMissing, double directMatch, double singleBoost, double something)
        {
            if (endeme1.EnSet != endeme2.EnSet) return 0.0;


            // --------------------------------------------------------------------------
            //  normal processing
            // --------------------------------------------------------------------------
	        char[] c = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
	        double total       = 0.0;
	        double mutualCount = 0.1;
	        for (int i = 0; i <= c.Length - 1; i++)
            {
		        int idx1 = endeme1.Index(c[i]);
		        int idx2 = endeme2.Index(c[i]);

		        double pair = 0.0;
		        if      (idx1 == Endeme.NO_INDEX && idx2 == Endeme.NO_INDEX) { pair = bothMissing; }
                else if (idx1 == Endeme.NO_INDEX || idx2 == Endeme.NO_INDEX) { pair = oneMissing; }
                else
                {
			        pair = FibonValue[idx1] * FibonValue[idx2];
			        mutualCount = mutualCount + 1;
		        }
		        total = total + pair;
	        }


            // --------------------------------------------------------------------------
            //  Calculate exact match count
            // --------------------------------------------------------------------------
	        double exactMatchCount = 0.0;
	        for (int i = 0; i <= c.Length - 1; i++)
            {
		        int idx = endeme1.Index(c[i]);
                if (idx == endeme2.Index(c[i])&& idx != Endeme.NO_INDEX) { exactMatchCount = exactMatchCount + 1; }
	        }


            // --------------------------------------------------------------------------
            //  Handle exact match situations
            // --------------------------------------------------------------------------
	        if ((exactMatchCount > 0)) { total = total + (directMatch * exactMatchCount / (Math.Max(endeme1.Count, endeme2.Count))); }
	        if (exactMatchCount == 1 && endeme1.Count == 1 && endeme2.Count == 1) { total = total + singleBoost; }


            // --------------------------------------------------------------------------
            //  Handle length difference situations
            // --------------------------------------------------------------------------
            double maxlen = (double)Math.Max(endeme1.Count, endeme2.Count);
            double minlen = (double)Math.Min(endeme1.Count, endeme2.Count);
            if (maxlen > 0) total = total + something * minlen / maxlen;

	        return (total - 6353.113279)/51776.45491;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- FibonValue -->
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<int, double> FibonValue { get
        {
		    if (_fibonValue == null) {
			    _fibonValue = new Dictionary<int, double>(25);
			    _fibonValue.Add(  0, 20000  );
			    _fibonValue.Add(  1, 12000  );
			    _fibonValue.Add(  2,  8000  );
			    _fibonValue.Add(  3,  5000  );
			    _fibonValue.Add(  4,  3000  );
			    _fibonValue.Add(  5,  2000  );
			    _fibonValue.Add(  6,  1200  );
			    _fibonValue.Add(  7,   800  );
			    _fibonValue.Add(  8,   500  );
			    _fibonValue.Add(  9,   300  );
			    _fibonValue.Add( 10,   200  );
			    _fibonValue.Add( 11,   120  );
			    _fibonValue.Add( 12,    80  );
			    _fibonValue.Add( 13,    50  );
			    _fibonValue.Add( 14,    30  );
			    _fibonValue.Add( 15,    20  );
			    _fibonValue.Add( 16,    12  );
			    _fibonValue.Add( 17,     8  );
			    _fibonValue.Add( 18,     5  );
			    _fibonValue.Add( 19,     3  );
			    _fibonValue.Add( 20,     2  );
			    _fibonValue.Add( 21,     1.2);
			    _fibonValue.Add( 22,     0.8);
			    _fibonValue.Add( 23,     0.5);
			    _fibonValue.Add(Endeme.NO_INDEX, 0.0);
		    }

		    return _fibonValue;
	    } } private static Dictionary<int, double> _fibonValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometric">a number between 0.0 and 1.0, recommend 0.63</param>
        /// <remarks></remarks>
        public static void SetFibonValue(double start, double geometric)
        {
	        _fibonValue = new Dictionary<int, double>(25);
	        for (int i = 0; i <= 23; i++) {
		        _fibonValue.Add(i, start);
		        start = start * geometric;
	        }
	        _fibonValue.Add(Endeme.NO_INDEX, 0.0);
        }

        public static Dictionary<int, double> AntiFiboValue {
	        get {
		        if (_antiFiboValue == null) {
			        _antiFiboValue = new Dictionary<int, double>(25);
			        _antiFiboValue.Add(  0, 20000      );
			        _antiFiboValue.Add(  1,  8000      );
			        _antiFiboValue.Add(  2,  3000      );
			        _antiFiboValue.Add(  3,  1200      );
			        _antiFiboValue.Add(  4,   500      );
			        _antiFiboValue.Add(  5,   200      );
			        _antiFiboValue.Add(  6,    80      );
			        _antiFiboValue.Add(  7,    30      );
			        _antiFiboValue.Add(  8,    12      );
			        _antiFiboValue.Add(  9,     5      );
			        _antiFiboValue.Add( 10,     2      );
			        _antiFiboValue.Add( 11,     0.8    );
			        _antiFiboValue.Add( 12,     0.3    );
			        _antiFiboValue.Add( 13,     0.12   );
			        _antiFiboValue.Add( 14,     0.05   );
			        _antiFiboValue.Add( 15,     0.02   );
			        _antiFiboValue.Add( 16,     0.008  );
			        _antiFiboValue.Add( 17,     0.003  );
			        _antiFiboValue.Add( 18,     0.0012 );
			        _antiFiboValue.Add( 19,     0.0005 );
			        _antiFiboValue.Add( 20,     0.0002 );
			        _antiFiboValue.Add( 21,     8E-05  );
			        _antiFiboValue.Add( 22,     3E-05  );
			        _antiFiboValue.Add( 23,     1.2E-05);
			        _antiFiboValue.Add(Endeme.NO_INDEX,     0.0    );
		        }
		        return _antiFiboValue;
	        }
        }

        private static Dictionary<int, double> _antiFiboValue;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometric">a number between 0.0 and 1.0, recommend 0.63</param>
        /// <remarks></remarks>
        public static void SetAntiFiboValue(double geometric)
        {
	        _antiFiboValue = new Dictionary<int, double>(25);
	        double start = 20000.0;
	        for (int i = 0; i <= 23; i++) {
		        _antiFiboValue.Add(i, start);
		        start = start * geometric;
	        }
	        _antiFiboValue.Add(Endeme.NO_INDEX, 0.0);
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- FibonMatch1 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double FibonMatch1(Endeme endeme1, Endeme endeme2)
        {

	        char[] c = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
	        double total = 0.0;
	        for (int i = 0; i <= c.Length - 1; i++) {
		        int idx1 = endeme1.Index(c[i]);
		        int idx2 = endeme2.Index(c[i]);
		        double pair = FibonValue[idx1] * FibonValue[idx2];
		        total = total + pair;
	        }

	        return total;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- SimpleMatch -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double SimpleMatch(Endeme endeme1, Endeme endeme2)
        {
	        double dist = 0.0;
	        double num = 4.0;
	        for (int i = 0; i <= endeme1.Count - 1; i++) {
		        char c = endeme1[i];
		        if ((endeme2.Contains(c))) {
			        dist = dist + Math.Abs(endeme1[c] - endeme2[c]);
		        } else {
			        dist = dist + num;
		        }
	        }
	        for (int i = 0; i <= endeme2.Count - 1; i++) {
		        char c = endeme2[i];
		        if ((endeme1.Contains(c))) {
			        dist = dist + Math.Abs(endeme1[c] - endeme2[c]);
		        } else {
			        dist = dist + num;
		        }
	        }

	        return 100.0 - dist;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- RegularMatch_testcase -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static void RegularMatch_testcase(Dictionary<string, EndemeTestItem> test, string name)
        {
            Endeme endeme1 = test[name].Endeme1;
            Endeme endeme2 = test[name].Endeme2;

            double match = endeme1.Match(endeme2, WeightFormula.Refined);

            test[name].Result = match;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- MatchTestMatch_testcase -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        /// <param name="name"></param>
        /// <param name="geom"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="dir2"></param>
        private void MatchTestMatch_testcase(Dictionary<string, EndemeTestItem> test, string name, double geom, double v1, double v2, double v3, double v4, double dir2)
        {
            Endeme endeme1 = test[name].Endeme1;
            Endeme endeme2 = test[name].Endeme2;

            double match = endeme1.MatchTestMatch(endeme2, geom, v1, v2, v3, v4, dir2);

            test[name].Result = match;
        }

        private void EnLevenshteinMatch_testcase(Dictionary<string, EndemeTestItem> test, string name)
        {
            Endeme endeme1 = test[name].Endeme1;
            Endeme endeme2 = test[name].Endeme2;

            double match = EnLevenshteinMatch(endeme1, endeme2);

            test[name].Result = match;
        }

        private void SimpleMatch_testcase(Dictionary<string, EndemeTestItem> test, string name)
        {
            Endeme endeme1 = test[name].Endeme1;
            Endeme endeme2 = test[name].Endeme2;

            double match = SimpleMatch(endeme1, endeme2);

            test[name].Result = match;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- AddMatch_testcase -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        private static void AddMatch_testcase(Dictionary<string, EndemeTestItem> test, string name, double f1, double f2, double f3, double f4)
        {
            Endeme endeme1 = test[name].Endeme1;
            Endeme endeme2 = test[name].Endeme2;



            Random r  = RandomSource.New().Random;
            Endeme e3 = endeme1 + endeme2;
            int    c1 = endeme1.Count;
            int    c2 = endeme2.Count;
            double c3 = 1.5*(c1 + c2);

            int LD12 = LevenshteinDistance(endeme1.ToString(), endeme2.ToString());
            int LD13 = LevenshteinDistance(endeme1.ToString(), e3.ToString());
            int LD23 = LevenshteinDistance(endeme2.ToString(), e3.ToString());

            //int d1b = __.SimpleDistance(e3.ToString(), endeme1.ToString());
            //int d2b = __.SimpleDistance(e3.ToString(), endeme2.ToString());

            double zeroto1 = (double)(c3 - LD12 - LD13 - LD23) / (c3);
            double factor1 = f3 + (double)c3 / f4;
            double factor2 = r.NextDouble() / 100000.0;
            double match = (f1 * zeroto1 - f2) * factor1 + factor2;

            //string measures = endeme1.ToString() + "\t" + endeme2.ToString() + "\t" + e3.ToString()
            //    + "\t" + d12 + "\t" + d13 + "\t" + d23
            //    + "\t" + c1 + "\t" + c2 + "\t" + c3
            //    + "\t" + zeroto1 + "\t" + output;



            test[name].Result = match;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LevenshteinDistance -->
        /// <summary>
        ///      Reutrns the Levenshtein distance between two string, Warning: case sensitive
        /// </summary>
        /// <param name="source">the so-called 'source' string, the order doesn't matter however</param>
        /// <param name="target">the so-called 'target' string, the order doesn't matter however</param>
        /// <returns></returns>
        /// <remarks>
        ///      d has (m+1)*(n+1) values, for all i and j, d[i,j] will hold the Levenshtein distance
        ///      between the first i characters of s and the first j characters of t;
        ///      
        ///      test me
        /// </remarks>
        public static int LevenshteinDistance(string source, string target)
        {
            // --------------------------------------------------------------------------
            //  Prepare the matrix:
            // --------------------------------------------------------------------------
            char[] src = (" " + source).ToCharArray();
            char[] tgt = (" " + target).ToCharArray();
            int[,] dist = new int[src.Length,tgt.Length]; // [m, n] // distance matrix
            for (int i = 0; i < src.Length; ++i) for (int j = 0; j < tgt.Length; ++j)
                    dist[i,j] = Math.Max(i,j);

 
            // --------------------------------------------------------------------------
            //  Calculate the matrix:
            // --------------------------------------------------------------------------
            for (int j = 1; j < tgt.Length; ++j)
                for (int i = 1; i < src.Length; ++i)
                    if (src[i] == tgt[j]) dist[i,j] = dist[i-1,j-1]; // no operation required
                    else                  dist[i,j] = Minimum(dist[i-1,j]+1, dist[i,j-1]+1, dist[i-1,j-1]+1); // Min( deletion , insertion , substitution);
            return dist[src.Length-1, tgt.Length-1];
        }

        /// <summary>Minimum of three integers, used only here</summary>
        private static int Minimum(int a, int b, int c) { return Math.Min(Math.Min(a,b),c); }

        // -----------------------------------------------------------------------------------------
        /// <!-- EnLevMatch -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double EnLevenshteinMatch(Endeme endeme1, Endeme endeme2)
        {
	        double dist = 100.0 - LevenshteinDistance(endeme1.ToString(), endeme2.ToString());
	        return dist;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- CalculateEffectiveness -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameOf"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int CalculateEffectiveness(Dictionary<double, List<string>> nameOf, Dictionary<string, EndemeTestItem> en)
        {

	        // -------------------------------------------------------------------------------
	        //  Sort the results
	        // -------------------------------------------------------------------------------
	        List<double> orderedResultList = new List<double>();
	        foreach (double num in nameOf.Keys)
            {
		        orderedResultList.Add(num);
	        }
	        orderedResultList.Sort();


	        // -------------------------------------------------------------------------------
	        //  Compare actual rank to desired rank
	        // -------------------------------------------------------------------------------
	        int diff = 0;
	        int rank = 0;
	        for (int i = 0; i <= orderedResultList.Count - 1; i++)
            {
		        double result = orderedResultList[i];
		        for (int j = 0; j <= nameOf[result].Count - 1; j++) 
                {
			        string name = nameOf[result][j];
			        diff = diff + Math.Abs(rank - en[name].TgtRank);
			        rank = rank + 1;
		        }
	        }

	        return diff;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="test"></param>
        ///// <returns></returns>
        //private string CalculateTable(Dictionary<string, EndemeTestItem> test)
        //{
        //    var sortedDict = from entry in test orderby entry.Value.Result ascending select entry;

        //    string diff = "";
        //    foreach (KeyValuePair<string, EndemeTestItem> item in sortedDict)
        //    {
        //        EndemeTestItem val = item.Value;
        //        diff = diff + "\r\n" + Math.Abs(val.TgtRank - val.CurRank).ToString().PadLeft(2)
        //            + "  " + val.Result.ToString().PadLeft(20)
        //            + "  " + item.Key.PadRight(3)
        //            + " (" + (val.TgtRank + "-" + val.CurRank.ToString()).PadLeft(5) + " = " + (val.TgtRank - val.CurRank).ToString().PadLeft(3) + ")"
        //            + "  '" + (val.Endeme1.ToString() + "'").PadLeft(25) + " v '" + val.Endeme2.ToString() + "'";
        //    }

        //    return diff;
        //}

        // ---------------------------------------------------------------------------------------------
        /// <!-- CalculateTable -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameOf"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string CalculateTable(Dictionary<double, List<string>> nameOf, Dictionary<string, EndemeTestItem> en)
        {

	        // -------------------------------------------------------------------------------
	        //  Sort the results
	        // -------------------------------------------------------------------------------
	        List<double> orderedResultList = new List<double>();
	        foreach (double num in nameOf.Keys)
            {
		        orderedResultList.Add(num);
	        }
	        orderedResultList.Sort();


	        // -------------------------------------------------------------------------------
	        //  Compare actual rank to desired rank
	        // -------------------------------------------------------------------------------
	        string diff = "";
	        int rank = 0;
	        for (int i = 0; i <= orderedResultList.Count - 1; i++)
            {
		        double result = orderedResultList[i];
		        for (int j = 0; j <= nameOf[result].Count - 1; j++)
                {
			        string name = nameOf[result][j];
			        int calc = Math.Abs(rank - en[name].TgtRank);
			        diff = diff + "\r\n" + calc.ToString().PadLeft(2).PadRight(6) + "  " + result.ToString().PadLeft(20) + "  " + name.PadRight(3) + " (" + (en[name].TgtRank + "-" + rank.ToString()).PadLeft(5) + " = " + calc.ToString().PadLeft(2) + ")" + "  '" + (en[name].Endeme1.ToString() + "'").PadLeft(25) + " v '" + en[name].Endeme2.ToString() + "'";
			        rank = rank + 1;
		        }
	        }

	        return diff;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- TestList -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Dictionary<string, EndemeTestItem> TestList()
        {
	        Dictionary<string, EndemeTestItem> en = new Dictionary<string, EndemeTestItem>();

	        en.Add("D0", new EndemeTestItem("ABCDEFGHIJKLMNOPQRSTUV", "VUTSRQPONMLKJIHGFEDCBA",  0)); // (the full reverse case)
	        en.Add("D1", new EndemeTestItem("ABCDEFGH"              , "IJKLMNOP"              ,  1)); // total miss on 8
	        en.Add("D2", new EndemeTestItem("ABCD"                  , "EFGH"                  ,  2)); // total miss on 4
	        en.Add("D3", new EndemeTestItem("ABC"                   , ""                      ,  3));
	        en.Add("D4", new EndemeTestItem("ABC"                   , "DEF"                   ,  4)); // total miss on 3
	        en.Add("D5", new EndemeTestItem("AB"                    , ""                      ,  5));
	        en.Add("D6", new EndemeTestItem("A"                     , "CB"                    ,  6));
	        en.Add("D7", new EndemeTestItem("A"                     , ""                      ,  7));
	        en.Add("D8", new EndemeTestItem("AB"                    , "CD"                    ,  8)); // total miss on 2
	        en.Add("C2", new EndemeTestItem("A"                     , "B"                     ,  9)); // total miss on 1
	        en.Add("D9", new EndemeTestItem("ABC"                   , "CDE"                   , 10));
	        en.Add("C0", new EndemeTestItem("A"                     , "ABCDEFG"               , 11)); // subset 1 of 7
	        en.Add("C1", new EndemeTestItem("A"                     , "ABCD"                  , 12)); // subset 1 of 4
	        en.Add("C3", new EndemeTestItem("A"                     , "ABC"                   , 13)); // subset 1 of 3
	        en.Add("C4", new EndemeTestItem("ABC"                   , "ADE"                   , 14));
	        en.Add("C5", new EndemeTestItem("A"                     , "AB"                    , 15)); // subset 1 of 2
	        en.Add("C6", new EndemeTestItem("ABCD"                  , "DCBA"                  , 16)); // 4 letter reverse case
	        en.Add("C7", new EndemeTestItem("ABC"                   , "CBA"                   , 17)); // 3 letter reverse case
	        en.Add("C8", new EndemeTestItem("AB"                    , "BA"                    , 18)); // 2 letter reverse case
	        en.Add("C9", new EndemeTestItem("ABC"                   , "ACB"                   , 19)); // flip at position 2/3
	        en.Add("B0", new EndemeTestItem("ABCD"                  , "ADCB"                  , 20));
	        en.Add("B1", new EndemeTestItem("ABCD"                  , "ABDC"                  , 21)); // flip at position 3/4
	        en.Add("B2", new EndemeTestItem("ABC"                   , "ABCD"                  , 22));
	        en.Add("B3", new EndemeTestItem("A"                     , "A"                     , 23)); // 1 letter perfect match
	        en.Add("B4", new EndemeTestItem("ABCDEFGHIJKLMNOPQRSTUV", "ABCDEFGHIJK"           , 24));
	        en.Add("B5", new EndemeTestItem("AB"                    , "AB"                    , 25)); // 2 letter perfect match
	        en.Add("B6", new EndemeTestItem("ABC"                   , "ABC"                   , 26)); // 3 letter perfect match
	        en.Add("B7", new EndemeTestItem("ABCDEFGHIJKLMNOPQRSTUV", "ABCDEFGHIJKLMNOPQRSVUT", 27));
	        en.Add("B8", new EndemeTestItem("ABCD"                  , "ABCD"                  , 28)); // 4 letter perfect match
	        en.Add("B9", new EndemeTestItem("ABCDEFGHIJKLMNOPQRSTUV", "ABCDEFGHIJKLMRQPONSTUV", 29));
	        en.Add("A0", new EndemeTestItem("ABCDEFG"               , "ABCDEFG"               , 30)); // 7 letter perfect match
	        en.Add("A1", new EndemeTestItem("ABCDEFGHIJKLMNOPQRSTUV", "ABCDEFGHIJKLMNOPQRSTUV", 31)); // (the full perfect match case)

	        return en;
        }

        #endregion

        #region EndemeActuator tests

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeActuator_HasChars_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeActuator_HasChars_test()
        {
            Assert.ThingsAbout("EndemeActuator", "HasChars");

            Result r = null;


            // --------------------------------------------------------------------------
            //  Construct universe 1
            // --------------------------------------------------------------------------
            EndemeReference  enRef = new EndemeReference().Add(WetlandAnimals).Add(Fruits);
            EndemeDefinition defn = new EndemeDefinition("Test1", enRef);
            defn.Add(new EndemeObject("Fruit:ABCD+Animal:ABCD", enRef, 's'   ));
            r = Assert.That(defn.Count, Is.equal_to, 1);
            r = Assert.That(defn[0].Item.CharValue, Is.equal_to, 's');
            defn["Fruit:ABCD+Animal:BAC"].Item = 'q';
          //r = Assert.That(defn["Fruit:ABCD+Animal:BAC"].Item.CharValue, Is.equal_to, 'q');
          //r = Assert.That(defn["Fruit:ABCD+Animal:ABCD"].Item.CharValue, Is.equal_to, 's');


            // -------------------------------------------------------------------------
            //  Construct universe 2
            // --------------------------------------------------------------------------
            EndemeDefinition field = new EndemeDefinition("Test2", enRef);
            field.Add(new EndemeObject("Fruit:ACEG+Animal:ABCD", enRef, "For Dinner"   ));
            field.Add(new EndemeObject("Fruit:EFGH+Animal:EFGH", enRef, "For Lunch"    ));
            field.Add(new EndemeObject("Fruit:ABCD+Animal:ACES", enRef, "For Breakfast"));


            // --------------------------------------------------------------------------
            //  Run tests
            // --------------------------------------------------------------------------

            // --------------------------------------------------------------------------
            //  Test single has-char actuator
            // --------------------------------------------------------------------------
            EndemeDefinition list1 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB")      , true ));  // must have both A and B
            r = Assert.That(list1.Count, Is.equal_to, 1);
            r = Assert.That(list1[0].Item.StrValue, Is.equal_to, "For Dinner"   );


            EndemeDefinition list2 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB")      , false));  // include all items, order by number of A and B
            r = Assert.That(list2.Count, Is.equal_to, 3);
            r = Assert.That(list2[0].Item.StrValue, Is.equal_to, "For Dinner"   );
            r = Assert.That(list2[1].Item.StrValue, Is.equal_to, "For Breakfast");
            r = Assert.That(list2[2].Item.StrValue, Is.equal_to, "For Lunch"    );


            EndemeDefinition list3 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB"), 1, 2, true ));  // must have at least 1 of A and B, then order by number of A and B
            r = Assert.That(list3.Count, Is.equal_to, 2);
            r = Assert.That(list3[0].Item.StrValue, Is.equal_to, "For Dinner"   );
            r = Assert.That(list3[1].Item.StrValue, Is.equal_to, "For Breakfast");


            EndemeDefinition list4 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB"), 0, 2, true));  // include all items, order by number of A and B
            r = Assert.That(list4.Count, Is.equal_to, 3);
            r = Assert.That(list4[0].Item.StrValue, Is.equal_to, "For Dinner"   );
            r = Assert.That(list4[1].Item.StrValue, Is.equal_to, "For Breakfast");
            r = Assert.That(list4[2].Item.StrValue, Is.equal_to, "For Lunch"    );


            EndemeDefinition list5 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB"), 0, 1, true));  // include all items, order by number of A and B
            r = Assert.That(list5.Count, Is.equal_to, 2);
            r = Assert.That(list5[0].Item.StrValue, Is.equal_to, "For Breakfast");
            r = Assert.That(list5[1].Item.StrValue, Is.equal_to, "For Lunch"    );


            EndemeDefinition list6 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB"), 2   , true));  // include all items, order by number of A and B
            r = Assert.That(list6.Count, Is.equal_to, 1);
            r = Assert.That(list6[0].Item.StrValue, Is.equal_to, "For Dinner"   );


            EndemeDefinition list7 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB"), 1   , true));  // include all items, order by number of A and B
            r = Assert.That(list7.Count, Is.equal_to, 1);
            r = Assert.That(list7[0].Item.StrValue, Is.equal_to, "For Breakfast");


            EndemeDefinition list8 = field.By(new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "AB"), 0   , true));  // include all items, order by number of A and B
            r = Assert.That(     list8.Count, Is.equal_to, 1);
            r = Assert.That(     list8[0].Item.StrValue, Is.equal_to, "For Lunch"    );


            EndemeActuator    act9 = new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "A"), true).HasChars(new Endeme(Fruits, "CD"), true);
            EndemeDefinition list9 = field
                          .By(act9);
            r = Assert.That(     list9.Count, Is.equal_to, 1);
            r = Assert.That(     list9[0].Item.StrValue, Is.equal_to, "For Breakfast");


            EndemeActuator    act10 = new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "A")).HasChars(new Endeme(Fruits, "CD"));
            EndemeDefinition list10 = field
                          .By(act10);
            r = Assert.That(     list10.Count, Is.equal_to, 1);
            r = Assert.That(     list10[0].Item.StrValue, Is.equal_to, "For Breakfast");


            EndemeActuator    act11 = new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "A"), false).HasChars(new Endeme(Fruits, "CD"), false);
            EndemeDefinition list11 = field
                          .By(act11);
            r = Assert.That(     list11.Count, Is.equal_to, 3);
            r = Assert.That(     list11[0].Item.StrValue, Is.equal_to, "For Breakfast");
            r = Assert.That(     list11[1].Item.StrValue, Is.equal_to, "For Dinner");
            r = Assert.That(     list11[2].Item.StrValue, Is.equal_to, "For Lunch");


            EndemeActuator act12 = new EndemeActuator().HasChars(new Endeme(WetlandAnimals, "A"));
          //act12.HasChars[0].



            //Endeme en = new Endeme(EndemeTests.WetlandAnimals, "ABC");
            //EndemeDefinition field = new EndemeDefinition("Hi", null);


            //for (int i = 0; i < actuator2.HasChar.Count; ++i)
            //{
            //    CharFactor factor = (CharFactor)actuator2.HasChar[i];
            //    int lower = factor.LowCount;
            //    int upper = factor.HighCount;
            //    Endeme tgt = factor.EnTarget;
            //}

            //for (int i = 0; i < actuator2.YetSets.Count; ++i)
            //{
            //    SetFactor factor = (SetFactor)actuator2.YetSets[i];
            //    int lower = factor.LowCount;
            //    int upper = factor.HighCount;
            //    EndemeSet tgt = factor.SetTarget;
            //}
            
            //return null;


            string str = Assert.Detail;

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeActuator_HasSets_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeActuator_HasSets_test()
        {
            Assert.ThingsAbout("EndemeActuator", "HasSets");


            EndemeReference  enRef = new EndemeReference().Add(WetlandAnimals).Add(Fruits);


            // -------------------------------------------------------------------------
            //  Construct universe
            // --------------------------------------------------------------------------
            EndemeDefinition field = new EndemeDefinition("Test2", enRef);
            field.Add(new EndemeObject("Fruit:ABCD"            , enRef, "Snack"        ));
            field.Add(new EndemeObject("Animal:ACES"           , enRef, "Feast"        ));
            field.Add(new EndemeObject("Fruit:ACEG+Animal:ABCD", enRef, "For Dinner"   ));
            field.Add(new EndemeObject("Fruit:EFGH+Animal:EFGH", enRef, "For Lunch"    ));
            field.Add(new EndemeObject("Fruit:ABCD+Animal:ACES", enRef, "For Breakfast"));

            EndemeActuator act5 = new EndemeActuator().HasSet(Fruits).HasSet(TinyEndemeSet);
            EndemeDefinition result5 = field.By(act5);
            Assert.That(result5.Count, Is.equal_to, 0);

            EndemeActuator act4 = new EndemeActuator().HasSet(TinyEndemeSet);
            EndemeDefinition result4 = field.By(act4);
            Assert.That(result4.Count, Is.equal_to, 0);

            EndemeActuator act3 = new EndemeActuator().HasSet(Fruits).HasSet(WetlandAnimals);
            EndemeDefinition result3 = field.By(act3);
            Assert.That(result3.Count, Is.equal_to, 3);

            EndemeActuator act2 = new EndemeActuator().HasSet(Fruits);
            EndemeDefinition result2 = field.By(act2);
            Assert.That(result2.Count, Is.equal_to, 4);

            EndemeActuator act1 = new EndemeActuator().HasSet(WetlandAnimals);
            EndemeDefinition result1 = field.By(act1);
            Assert.That(result1.Count, Is.equal_to, 4);




            string str = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeActuator_HasValue_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeActuator_HasValue_test()
        {
            Assert.ThingsAbout("EndemeActuator", "HasValue");


            // --------------------------------------------------------------------------
            //  Construct universe
            // --------------------------------------------------------------------------
            EndemeReference enRef = new EndemeReference().Add(WetlandAnimals).Add(Fruits);
            EndemeDefinition field = new EndemeDefinition("Test1", enRef);
            field.Add(new EndemeObject("Fruit:AQCD=Punch+Animal:ABCD=7"  , enRef, 'a'   ));
            field.Add(new EndemeObject("Fruit:ABCD=Punch+Animal:ABCD=7"  , enRef, 'b'   ));
            field.Add(new EndemeObject("Fruit:ABCD=+Animal:ABCD"         , enRef, 'c'   ));
            field.Add(new EndemeObject("Fruit:ABCD=Punch+Animal:ABCD"    , enRef, 'd'   ));
            field.Add(new EndemeObject("Fruit:ABCD=Punch+Animal:ABCD=Pet", enRef, 'e'   ));

            EndemeProfile prof = new EndemeProfile();
            prof.Add(new EndemeItem(new Endeme(Fruits, "ABCD"), new EndemeValue("Punch")));
            prof.Add(new EndemeItem(new Endeme(WetlandAnimals, "ABCD"), new EndemeValue(7)));
            field.Add(prof, 'f');

            //prof.Add(new EndemeItem(Fruits, "ABCD", "Wassail"));
            //prof.Add(new EndemeItem("Label", Fruits, "ABCD", "Wassail"));


            // --------------------------------------------------------------------------
            //  Run tests
            // --------------------------------------------------------------------------
            EndemeActuator   act5   = new EndemeActuator()
                .HasValue(new Endeme(Fruits, "ABCD"), "Punch")
                .HasValue(new Endeme(WetlandAnimals, "ABCD"), 7);
            EndemeDefinition found5 = field.By(act5);
            Assert.That(found5.Count, Is.equal_to, 2);


            EndemeActuator   act4   = new EndemeActuator()
                .HasValue(new Endeme(Fruits, "ABCD"), "Punch")
                .HasValue(new Endeme(WetlandAnimals, "ABCD"), "7");
            EndemeDefinition found4 = field.By(act4);
            Assert.That(found4.Count, Is.equal_to, 1);


            EndemeActuator   act3   = new EndemeActuator()
                .HasValue(new Endeme(Fruits, "ABCD"), "Punch")
                .HasValue(new Endeme(Fruits, "ABCD"), "Cider");
            EndemeDefinition found3 = field.By(act3);
            Assert.That(found3.Count, Is.equal_to, 0);
            string hi = found3[0].ToString();


            EndemeActuator   act0   = new EndemeActuator().HasValue(new Endeme(Fruits, "ABCD"), "Soda");
            EndemeDefinition found0 = field.By(act0);
            Assert.That(found0.Count, Is.equal_to, 0);

            EndemeActuator   act1   = new EndemeActuator().HasValue(new Endeme(Fruits, "ABCD"), "Punch");
            EndemeDefinition found1 = field.By(act1);
            Assert.That(found1.Count, Is.equal_to, 4);

            EndemeActuator   act2   = new EndemeActuator()
                .HasValue(new Endeme(Fruits, "ABCD"), "Punch")
                .HasValue(new Endeme(WetlandAnimals, "ABCD"), "Pet");
            EndemeDefinition found2 = field.By(act2);
            Assert.That(found2.Count, Is.equal_to, 1);


            string str = Assert.Detail;

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeActuator_Multifactor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeActuator_Multifactor_test()
        {
            Assert.ThingsAbout("EndemeActuator", "Multiple factors");


            // --------------------------------------------------------------------------
            //  Construct universe
            // --------------------------------------------------------------------------
            EndemeReference enRef = new EndemeReference().Add(WetlandAnimals).Add(Fruits);
            EndemeDefinition field = new EndemeDefinition("Test1", enRef);
            field.Add(new EndemeObject("Fruit:AQCD=Punch+Animal:ABCD=7"  , enRef, 'c'   ));
            field.Add(new EndemeObject("Fruit:ABCD=Punch+Animal:ABCD=7"  , enRef, 'c'   ));
            field.Add(new EndemeObject("Fruit:ABCD=+Animal:ABCD"         , enRef, 'a'   ));
            field.Add(new EndemeObject("Fruit:ABCD=Punch+Animal:ABCD"    , enRef, 'b'   ));
            field.Add(new EndemeObject("Fruit:ABCD=Punch+Animal:ABCD=Pet", enRef, 'c'   ));


            // --------------------------------------------------------------------------
            //  Run tests
            // --------------------------------------------------------------------------
            EndemeActuator   act3   = new EndemeActuator()
                .HasValue(new Endeme(WetlandAnimals, "ABCD"), 7)
                .HasChars(new Endeme(Fruits        , "AB"  ));
            EndemeDefinition found3 = field.By(act3);
            Assert.That(found3.Count, Is.equal_to, 1);


            string str = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeActuator_Ordered_test -->
        /// <summary>
        /// 
        /// </summary>
        private void EndemeActuator_Ordered_test()
        {
            Assert.ThingsAbout("EndemeActuator", "Ordered");


            // --------------------------------------------------------------------------
            //  Construct universe
            // --------------------------------------------------------------------------
            EndemeReference enRef = new EndemeReference().Add(WetlandAnimals).Add(Fruits);
            EndemeDefinition field = new EndemeDefinition("Test1", enRef);
            field.Add(new EndemeObject("Fruit:ABCD" , enRef, 1));
            field.Add(new EndemeObject("Fruit:ABC"  , enRef, 1));
            field.Add(new EndemeObject("Fruit:AB"   , enRef, 1));
            field.Add(new EndemeObject("Fruit:EABCD", enRef, 1));
            field.Add(new EndemeObject("Fruit:EABC" , enRef, 1));


            EndemeActuator   act1   = new EndemeActuator().HasOrder("ABC");
            EndemeDefinition result = field.By(act1);
            Assert.That(result.Count, Is.equal_to, 4);


            string str = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeActuator_SmokeTest -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeActuator_SmokeTest()
        {
            Assert.ThingsAbout("EndemeActuator", "Constructor");


            try
            {
                Endeme en = new Endeme(EndemeTests.WetlandAnimals, "ABC");
                EndemeDefinition field = new EndemeDefinition("Hi", null);


                EndemeActuator actuator = new EndemeActuator(0)
                    .AndWeight(en      , 19, 5 , 3.14)
                    .AndWeight(en      , 1 , 3 , 1.0 )
                    .AndWeight(en      , 19, 5 , 1.0 , WeightFormula.Distance)
                    .AndOrder ("ABC"           , 1.01)
                    .AndOrder ("DEF"   , 1 , 18, 1.0 )
                    .AndWeight(en      , 19, 5 , 1.0 , WeightFormula.Refined, 0,4,2,5,7)
                    .AndChars (en      , 2 , 5 , 1.0 )
                    .AndChars (en      , 1     , 1.0 )
                    .ButNotSet(en.EnSet              )
                    .AndSet   (en.EnSet, 1 , 2 , 1.0 )
                    .AndSet   (en.EnSet        , 0.0 )
                    .HasSet   (en.EnSet        , true);

                EndemeObject obj = field[actuator];
                field.Add(new EndemeProfile(), 1);
                Assert.SmokeTest();
            }
            catch { Assert.Crash(); }

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeActuator_Weights_test -->
        /// <summary>
        /// 
        /// </summary>
        private void EndemeActuator_Weights_test()
        {
            Assert.ThingsAbout("EndemeActuator", "Weights");


            EndemeReference  enRef = new EndemeReference().Add(WetlandAnimals).Add(Fruits);


            {
                // --------------------------------------------------------------------------
                //  Construct universe
                // --------------------------------------------------------------------------
                EndemeDefinition field = new EndemeDefinition("Test1", enRef);
                field.Add(new EndemeObject("Fruit:ABC   ", enRef, 12));
                field.Add(new EndemeObject("Fruit:ABCDEF", enRef, 14));


                EndemeActuator actABC = new EndemeActuator();
                actABC.AndWeight(new Endeme(Fruits,"ABC"), 22, 21, 1.0, WeightFormula.FullMedian, 1);
                EndemeDefinition resultABC = field.By(actABC);
            }


            {
                // --------------------------------------------------------------------------
                //  Construct universe
                // --------------------------------------------------------------------------
                EndemeDefinition field = new EndemeDefinition("Test1", enRef);

                field.Add(new EndemeObject("Fruit:A       +Animal:ABCD=01", enRef, 01));
                field.Add(new EndemeObject("Fruit:        +Animal:ABCD=02", enRef, 02));
                field.Add(new EndemeObject("Fruit:AB      +Animal:ABCD=03", enRef, 03));
                field.Add(new EndemeObject("Fruit:CB      +Animal:ABCD=04", enRef, 04));
                field.Add(new EndemeObject("Fruit:CBA     +Animal:ABCD=05", enRef, 05));
                field.Add(new EndemeObject("Fruit:CD      +Animal:ABCD=06", enRef, 06));
                field.Add(new EndemeObject("Fruit:CDE     +Animal:ABCD=07", enRef, 07));
                field.Add(new EndemeObject("Fruit:ABC     +Animal:DRAB=08", enRef, 08));
                field.Add(new EndemeObject("Fruit:ABC     +Animal:CARB=09", enRef, 09));
                field.Add(new EndemeObject("Fruit:ABC     +Animal:BACK=10", enRef, 10));
                field.Add(new EndemeObject("Fruit:ABC     +Animal:BARK=11", enRef, 11));
                field.Add(new EndemeObject("Fruit:ABC     +Animal:ABCD=12", enRef, 12));
                field.Add(new EndemeObject("Fruit:ABCD    +Animal:ABCD=13", enRef, 13));
                field.Add(new EndemeObject("Fruit:ABCDEFG +Animal:ABCD=14", enRef, 14));
                field.Add(new EndemeObject("Fruit:ABCDEFGH+Animal:ABCD=15", enRef, 15));
                field.Add(new EndemeObject("Fruit:ABDC    +Animal:ABCD=16", enRef, 16));
                field.Add(new EndemeObject("Fruit:ACB     +Animal:ABCD=17", enRef, 17));
                field.Add(new EndemeObject("Fruit:ADCB    +Animal:ABCD=18", enRef, 18));
                field.Add(new EndemeObject("Fruit:ADE     +Animal:ABCD=19", enRef, 19));
                field.Add(new EndemeObject("Fruit:B       +Animal:ABCD=20", enRef, 20));
                field.Add(new EndemeObject("Fruit:BA      +Animal:ABCD=21", enRef, 21));
                field.Add(new EndemeObject("Fruit:DCBA    +Animal:ABCD=22", enRef, 22));
                field.Add(new EndemeObject("Fruit:DEF     +Animal:ABCD=23", enRef, 23));
                field.Add(new EndemeObject("Fruit:EFGH    +Animal:ABCD=24", enRef, 24));
                field.Add(new EndemeObject("Fruit:IJKLMNOP+Animal:ABCD=25", enRef, 25));


                // ----------------------------------------------------------------------
                //  Run tests
                // ----------------------------------------------------------------------
                EndemeActuator act1 = new EndemeActuator()
                    .AndWeight(new Endeme(Fruits,"A"), 22, 21);
                EndemeDefinition result1 = field.By(act1);


                EndemeActuator act2 = new EndemeActuator()
                    .AndWeight(new Endeme(Fruits,"A"), 22, 10)
                    .AndWeight(new Endeme(WetlandAnimals, "BARD"), 22, 21);
                EndemeDefinition result2 = field.By(act2);


                EndemeActuator act3 = new EndemeActuator()
                    .AndWeight(new Endeme(Fruits,"ABC"), 22, 21);
                EndemeDefinition result3 = field.By(act3);


                EndemeActuator act4 = new EndemeActuator();
                act4.AndWeight(new Endeme(Fruits,"ABC"), 22, 21, 1.0, WeightFormula.FullMedian, 10);
                EndemeDefinition result4 = field.By(act4);
            }


            {
                // ----------------------------------------------------------------------
                //  A particular test ordering
                // ----------------------------------------------------------------------
                EndemeDefinition simple = new EndemeDefinition("Test1", enRef);

                simple.Add(new EndemeObject("Fruit:V                     +Animal:ABCD=1", enRef, 1));
                simple.Add(new EndemeObject("Fruit:A                     +Animal:ABCD=1", enRef, 1));
                simple.Add(new EndemeObject("Fruit:AB                    +Animal:ABCD=2", enRef, 2));
                simple.Add(new EndemeObject("Fruit:ABC                   +Animal:ABCD=3", enRef, 3));
                simple.Add(new EndemeObject("Fruit:ABCD                  +Animal:ABCD=4", enRef, 4));
                simple.Add(new EndemeObject("Fruit:ABCDE                 +Animal:ABCD=5", enRef, 5));
                simple.Add(new EndemeObject("Fruit:ABCDEF                +Animal:ABCD=6", enRef, 6));
                simple.Add(new EndemeObject("Fruit:ABCDEFG               +Animal:ABCD=7", enRef, 7));
                simple.Add(new EndemeObject("Fruit:ABCDEFGH              +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHI             +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJ            +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJK           +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKL          +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLM         +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMN        +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNO       +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNOP      +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNOPQ     +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNOPQR    +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNOPQRS   +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNOPQRST  +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNOPQRSTU +Animal:ABCD=8", enRef, 8));
                simple.Add(new EndemeObject("Fruit:ABCDEFGHIJKLMNOPQRSTUV+Animal:ABCD=8", enRef, 8));

                //EndemeActuator act1 = new EndemeActuator()
                //    .AndWeight(new Endeme(Fruits,"ABCDEFGH"), 22, 21);
                //EndemeDefinition result1 = simple.By(act1);
                //EndemeObject  hi   = result1[0];
                //EndemeProfile pro   = hi.ItemProfile;
                //EndemeItem    hithere = pro[0];
                //string        str1 = hithere.ItemProfile.ToString();
                //Assert.That(str1, Is.equal_to, "ABCDEFGH");


                EndemeActuator act2 = new EndemeActuator();
              //act2.AndWeight(new Endeme(Fruits,"ABCDEFGHIJKLMNOP"), 22, 21, 1.0, WeightFormula.FullMedian, 10);
                act2.AndWeight(new Endeme(Fruits,"ABCDEFGHIJKLMNOP"), WeightFormula.FullMedian, 10, 1.0);
                EndemeDefinition result2 = simple.By(act2);


                EndemeActuator   act3    = new EndemeActuator().AndWeight(new Endeme(Fruits,"ABCDEFGH"), 22, 21, 1.0, WeightFormula.FullMedian, 10);
                EndemeDefinition result3 = simple.By(act3);
                Assert.That(result3[0].ItemProfile[0].ItemEndeme.ToString(), Is.equal_to, "ABCDEFGH");


                EndemeActuator act4 = new EndemeActuator();
                act4.AndWeight(new Endeme(Fruits,"ABCDEFGHIJKLMNOPQRSTUV"), 22, 21, 1.0, WeightFormula.FullMedian, 10);
                EndemeDefinition result4 = simple.By(act4);
                Assert.That(result4[0].ItemProfile[0].ItemEndeme.ToString(), Is.equal_to, "ABCDEFGHIJKLMNOPQRSTUV");
            }


            string st = Assert.Detail;


            {
                // --------------------------------------------------------------------------
                //  Construct universe
                // --------------------------------------------------------------------------
                EndemeDefinition field = new EndemeDefinition("Test1", enRef);

                field.Add(new EndemeObject("Fruit:A   +Animal:ABCD=1", enRef, 1));
                field.Add(new EndemeObject("Fruit:BA  +Animal:ABCD=2", enRef, 2));
                field.Add(new EndemeObject("Fruit:CBA +Animal:ABCD=3", enRef, 3));
                field.Add(new EndemeObject("Fruit:DCBA+Animal:ABCD=4", enRef, 4));

                EndemeActuator act = new EndemeActuator();
                act.AndWeight(new Endeme(Fruits,"A"), 22, 21, 1.0, WeightFormula.FullMedian, 10);
                EndemeDefinition result = field.By(act);
            }


            {
                // --------------------------------------------------------------------------
                //  Construct universe
                // --------------------------------------------------------------------------
                EndemeDefinition field = new EndemeDefinition("Test1", enRef);

                field.Add(new EndemeObject("Fruit:ABC     +Animal:ABCD=12", enRef, 12));
                field.Add(new EndemeObject("Fruit:A       +Animal:ABCD=01", enRef, 01));
                field.Add(new EndemeObject("Fruit:AB      +Animal:ABCD=03", enRef, 03));
                field.Add(new EndemeObject("Fruit:CB      +Animal:ABCD=04", enRef, 04));
                field.Add(new EndemeObject("Fruit:CBA     +Animal:ABCD=05", enRef, 05));
                field.Add(new EndemeObject("Fruit:CD      +Animal:ABCD=06", enRef, 06));
                field.Add(new EndemeObject("Fruit:CDE     +Animal:ABCD=07", enRef, 07));
                field.Add(new EndemeObject("Fruit:ABCD    +Animal:ABCD=13", enRef, 13));
                field.Add(new EndemeObject("Fruit:ABCDEFG +Animal:ABCD=14", enRef, 14));
                field.Add(new EndemeObject("Fruit:ABCDEFGH+Animal:ABCD=15", enRef, 15));
                field.Add(new EndemeObject("Fruit:ABDC    +Animal:ABCD=16", enRef, 16));
                field.Add(new EndemeObject("Fruit:ACB     +Animal:ABCD=17", enRef, 17));
                field.Add(new EndemeObject("Fruit:ADCB    +Animal:ABCD=18", enRef, 18));
                field.Add(new EndemeObject("Fruit:ADE     +Animal:ABCD=19", enRef, 19));
                field.Add(new EndemeObject("Fruit:B       +Animal:ABCD=20", enRef, 20));
                field.Add(new EndemeObject("Fruit:BA      +Animal:ABCD=21", enRef, 21));
                field.Add(new EndemeObject("Fruit:DCBA    +Animal:ABCD=22", enRef, 22));
                field.Add(new EndemeObject("Fruit:DEF     +Animal:ABCD=23", enRef, 23));
                field.Add(new EndemeObject("Fruit:EFGH    +Animal:ABCD=24", enRef, 24));
                field.Add(new EndemeObject("Fruit:IJKLMNOP+Animal:ABCD=25", enRef, 25));

                EndemeActuator act = new EndemeActuator();
                act.AndWeight(new Endeme(Fruits,"A"), 22, 21, 1.0, WeightFormula.FullMedian);
                EndemeDefinition result = field.By(act);

            }

            string str = Assert.Detail;


            _result += Assert.Conclusion;
        }

        #endregion EndemeActuator tests

        #region other endeme classes

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeArray_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeArray_Constructor_test()
        {
            Assert.ThingsAbout("EndemeArray", "Constructor");


            EndemeDefinition enArr  = new EndemeDefinition("Big Array");
            EndemeReference  enDice = TestReference;
            EndemeObject     enOb   = new EndemeObject("Animal:ABC*Fruit:DEF", enDice, 5); // "item 1", 
            enArr.Add(enOb);
            enArr.Add(new EndemeObject("Fruit:DEF*Animal:ABC", enDice, 7)); // "item 1", 

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeList_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeList_Constructor_test()
        {
            Assert.ThingsAbout("EndemeList", "Constructor");


            EndemeList list = new EndemeList("Wetland animals", WetlandAnimals);
            Assert.That(list.Count, Is.equal_to, 0);
            Assert.That(list.DefaultEnSet  , Is.equal_to, WetlandAnimals);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeBalancer_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeBalancer_Constructor_test()
        {
            Assert.ThingsAbout("EndemeBalancer", "Constructor");


            Endeme avg = WetlandAnimals.RandomEndeme();
            Endeme std = WetlandAnimals.RandomEndeme();
            EndemeBalancer norm = new EndemeBalancer(100.0, 10.0, avg, std);
            Assert.That(norm.StdTgt, Is.equal_to, 10.0);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeCharacteristic_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeCharacteristic_Constructor_test()
        {
            Assert.ThingsAbout("EndemeCharacteristic", "Constructor");


            EndemeCharacteristic ec = new EndemeCharacteristic('C', "Cat", "Felines");
            Assert.That(ec.Label, Is.equal_to, "Cat");

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeDefinition_By_Weight_test -->
        /// <summary>
        /// 
        /// </summary>
        private void EndemeDefinition_By_Weight_test()
        {
            Assert.ThingsAbout("EndemeDefinition", "HasChars");

          //Result r = null;


            EndemeReference  enRef = new EndemeReference().Add(TinyEndemeSet);

            // --------------------------------------------------------------------------
            //  Construct universe
            // --------------------------------------------------------------------------
            EndemeDefinition field = new EndemeDefinition("Test1", enRef);
            field.Add(new EndemeObject("TinySet:AB  ", enRef, 12));
            field.Add(new EndemeObject("TinySet:ABC ", enRef, 12));
            field.Add(new EndemeObject("TinySet:ABCD", enRef, 14));


            EndemeActuator act = new EndemeActuator();
            act.AndWeight(new Endeme(TinyEndemeSet,"AB"), 22, 21, 1.0, WeightFormula.FullMedian, 7);
            EndemeDefinition result = field.By(act);

            string str = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeList_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeList_test()
        {
            Assert.ThingsAbout("EndemeList", "various");


            EndemeReference enRef = new EndemeReference();
            enRef.Add(WetlandAnimals);
            EndemeList field   = new EndemeList("Test", enRef, 90.0);
            field["Animal:LR"] = new EndemeItem("ABC");
            field["Animal:A" ] = new EndemeItem(42);
            field["Animal:LR"] = new EndemeItem("DEF");
            int    age  = 0;
            string code = "";
            try
            {
                age  = (int)   field["Animal:A" ].Item.Value;
                code = (string)field["Animal:LR"].Item.Value;
            }
            catch (Exception ex)
            {
                Assert.Crash(ex);
            }
            Assert.That(age , Is.equal_to, 42);
            Assert.That(code, Is.equal_to, "DEF");

            string str = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeHelpers_MoveLetter_test -->
        /// <summary>
        /// 
        /// </summary>
        //[Test]
        public void EndemeHelpers_MoveLetter_test()
        {
            Assert.ThingsAbout("EndemeHelpers", "MoveLetter");


            string str;
            str = MoveLetter("hi", "i",  1); Assert.That(str, Is.equal_to, "ih");
            str = MoveLetter("hi", "i",  2); Assert.That(str, Is.equal_to, "hi");
            str = MoveLetter("hi", "i", -1); Assert.That(str, Is.equal_to, "hi");
            str = MoveLetter("hi", "i", -2); Assert.That(str, Is.equal_to, "ih");

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MoveLetter -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">string to modify</param>
        /// <param name="letter">string (letter) to move</param>
        /// <param name="toPosition">1 to n or -1 to -n</param>
        /// <returns></returns>
        public static string MoveLetter(string str, string letter, int toPosition)
        {
            //  remove the letter
            string s2 = Regex.Replace(str, letter, "");

            if (toPosition < 0)
            {
                toPosition = -toPosition;
                toPosition--; // convert 1 to N to 0 to n-1
                s2 = Regex.Replace(s2, "(.{" + toPosition + "})$", letter + "$1");
            }
            else
            {
                toPosition--; // convert 1 to N to 0 to n-1
                s2 = Regex.Replace(s2, "^(.{" + toPosition + "})", "$1" + letter);
            }

            return s2;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeList_OrderBy_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeList_OrderBy_test()
        {
            Assert.ThingsAbout("EndemeList", "OrderBy");


            EndemeList list = new EndemeList("test", Fruits);
            list.Add("Item A", "ABC", null);
            list.Add("Item B", "DEF", null);

            EndemeList list2 = list.OrderBy("ABC");  Assert.That(list2[1].ItemEndeme.ToString(), Is.equal_to, "ABC");
            EndemeList list3 = list.OrderBy("DEF");  Assert.That(list3[1].ItemEndeme.ToString(), Is.equal_to, "DEF");
            EndemeList list4 = list.OrderBy("DC" );  Assert.That(list4[1].ItemEndeme.ToString(), Is.equal_to, "DEF");
            EndemeList list5 = list.OrderBy("AF" );  Assert.That(list5[1].ItemEndeme.ToString(), Is.equal_to, "ABC");

            string detail = Assert.Detail;


            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeList_Add_test -->
        public void EndemeList_Add_test()
        {
            Assert.ThingsAbout("EndemeList", "Add");


            EndemeList list = new EndemeList("Wetland animals", WetlandAnimals);
            list.Add("hiworld", "ABC", null);
          //list.Add(new EndemeElement("hiworld", list.Set, "ABC", null));

            _result += Assert.Conclusion;
        }

        //public void GenEndemeList_Add_test()
        //{
        //    Assert.ThingsAbout("GenEndemeList", "Add");


        //    GenEndemeList_doNotUse<string> list = new GenEndemeList_doNotUse<string>("Wetland animals", WetlandAnimals);
        //    list.Add("My Name", "ABC", "Jon Grover");
        //    list.Add("My Age", "DEF", "54");
        //    string name = list[0][0];

        //    _result += Assert.Conclusion;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeObject_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeObject_Constructor_test()
        {
            Assert.ThingsAbout("EndemeObject", "Constructor");

          //EndemeObject     enObj1 = new EndemeObject ();
            EndemeReference enRef  = TestReference;
            EndemeObject     enObj2 = new EndemeObject ("Animal:ABC*Fruit:DEF", enRef, 1); // "world2", 
            EndemeProfile    enProf = new EndemeProfile("Animal:ABC*Fruit:DEF", enRef);
            EndemeObject     enObj3 = new EndemeObject ("hi", enProf, 2);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeProfile_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeProfile_Constructor_test()
        {
            Assert.ThingsAbout("EndemeProfile", "Constructor");


            //EndemeReference enRef = new EndemeReference();
            //enRef.Add(WetlandAnimals);
            //enRef.Add(Fruits);
            EndemeReference enRef = TestReference;


            EndemeProfile path = new EndemeProfile();
            Assert.That(path.ToString(), Is.equal_to, "");

            path = new EndemeProfile("Animal:ABC*Fruit:DEF", enRef);
            Assert.That(path.ToString(), Is.equal_to, "Animal:ABC*Fruit:DEF");

            path = new EndemeProfile("Job:ABC+Identity:FL", enRef);
            Assert.That(path.ToString(), Is.equal_to, "Job:ABC+Identity:FL");

            path = new EndemeProfile("Job:ABC!Identity:FL", null);
            Assert.That(path.ToString(), Is.equal_to, "Job:ABC!Identity:FL");

            path = new EndemeProfile("Animal:ABC", enRef);
            Assert.That(path.ToString(), Is.equal_to, "Animal:ABC");

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeReference_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeReference_Constructor_test()
        {
            Assert.ThingsAbout("EndemeReference", "Constructor");


            EndemeReference enDice = TestReference;
            Assert.That(enDice.Count, Is.equal_to, 2);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeSet_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeSet_Constructor_test()
        {
            Assert.ThingsAbout("EndemeSet", "Constructor");
            EndemeSet animals = WetlandAnimals;
            Assert.That(animals.Count, Is.equal_to, 22);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeSet_ToString_test -->
        /// <summary>
        /// 
        /// </summary>
        //[Test]
        public void EndemeSet_ToString_test()
        {
            Assert.ThingsAbout("EndemeSet", "ToString");


            EndemeSet animals = WetlandAnimals;
            string str = animals.ToString();
            Assert.That(str.Length, Is.greater_than, 20);
            Assert.That(str, Is.equal_to, "Animal A)alligator, B)beaver, C)crocodile, D)duck, E)egret, F)frog, G)gecko, H)herron, I)insect, J)jackal, K)koala, L)lizard, M)muskrat, N)newt, O)otter, P)puma, Q)quahog, R)reptile, S)snake, T)turtle, U)ungulate, V)vole");

            _result += Assert.Conclusion;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- EndemeSet_ToString_test -->
        ///// <summary>
        ///// 
        ///// </summary>
        ////[Test]
        //public void GenEndemeItem_Constructor_test()
        //{
        //    Assert.ThingsAbout("GenEndemeItem", "Constructor");


        //    GenEndemeItem_donotUse<string> gen = null;
        //    gen = new GenEndemeItem_donotUse<string>("My Name", new Endeme(WetlandAnimals, "ABC"), "Jon Grover");
        //    string str = gen[0];
        //    Assert.That(str, Is.equal_to, "Jon Grover");

        //    _result += Assert.Conclusion;
        //}

        //// ----------------------------------------------------------------------------------------
        ///// <!-- GenEndemeList_Constructor_test -->
        ///// <summary>
        ///// 
        ///// </summary>
        //public void GenEndemeList_Constructor_test()
        //{
        //    Assert.ThingsAbout("GenEndemeList", "Constructor");


        //    GenEndemeList_doNotUse<string> list = new GenEndemeList_doNotUse<string>("Wetland animals", WetlandAnimals);
        //    Assert.That(list.Count, Is.equal_to, 0);
        //    Assert.That(list.Set  , Is.equal_to, WetlandAnimals);

        //    _result += Assert.Conclusion;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeTextFormat_Show_test -->
        /// <summary>
        /// 
        /// </summary>
        public void EndemeTextFormat_Show_test()
        {
            Assert.ThingsAbout("EndemeTextFormat", "Show");


            Endeme endeme = new Endeme(WetlandAnimals, "MKACLENSJHUFIGBVTQDPOR");
            string str = "";
            str = EndemeTextFormat.Show_complex(endeme, 27, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHU-FIGB-VTQD-POR"); // perfect, common
            str = EndemeTextFormat.Show_complex(endeme, 27, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIGB-VTQD-POR ");
            str = EndemeTextFormat.Show_complex(endeme, 27, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIGB-VTQDPOR  ");
            str = EndemeTextFormat.Show_complex(endeme, 27, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGB-VTQDPOR   ");
            str = EndemeTextFormat.Show_complex(endeme, 27, 1, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGBVTQDPOR    ");
            str = EndemeTextFormat.Show_complex(endeme, 27, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIGBVTQDPOR     ");
            str = EndemeTextFormat.Show_complex(endeme, 26, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIG--VTQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 26, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIGB-VTQD-POR"); // perfect, common
            str = EndemeTextFormat.Show_complex(endeme, 26, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIGB-VTQDPOR ");
            str = EndemeTextFormat.Show_complex(endeme, 26, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGB-VTQDPOR  ");
            str = EndemeTextFormat.Show_complex(endeme, 26, 1, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGBVTQDPOR   ");
            str = EndemeTextFormat.Show_complex(endeme, 26, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIGBVTQDPOR    ");
            str = EndemeTextFormat.Show_complex(endeme, 25, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFI--VTQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 25, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIG-VTQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 25, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIGB-VTQDPOR"); // perfect
            str = EndemeTextFormat.Show_complex(endeme, 25, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGB-VTQDPOR ");
            str = EndemeTextFormat.Show_complex(endeme, 25, 1, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGBVTQDPOR  ");
            str = EndemeTextFormat.Show_complex(endeme, 25, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIGBVTQDPOR   ");
            str = EndemeTextFormat.Show_complex(endeme, 24, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFI--TQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 24, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFI-VTQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 24, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFIG-VTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 24, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGB-VTQDPOR"); // perfect, common
            str = EndemeTextFormat.Show_complex(endeme, 24, 1, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGBVTQDPOR ");
            str = EndemeTextFormat.Show_complex(endeme, 24, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIGBVTQDPOR  ");
            str = EndemeTextFormat.Show_complex(endeme, 23, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUF--TQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 23, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFI-TQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 23, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFI-VTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 23, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIG-VTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 23, 1, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFIGBVTQDPOR"); // perfect
            str = EndemeTextFormat.Show_complex(endeme, 23, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIGBVTQDPOR ");
            str = EndemeTextFormat.Show_complex(endeme, 22, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHU--TQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 22, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUF-TQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 22, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUFI-TQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 22, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFI-VTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 22, 1, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIG-VTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 22, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIGBVTQDPOR"); // perfect
            str = EndemeTextFormat.Show_complex(endeme, 21, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHU--QD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 21, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHU-TQD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 21, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHUF-TQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 21, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUFI-TQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 21, 1, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFI-VTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 21, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIGVTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 20, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJH--QD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 20, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHU-QD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 20, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJHU-TQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 20, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHUF-TQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 20, 1, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFI-TQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 20, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFIVTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 18, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJ--D-POR");
            str = EndemeTextFormat.Show_complex(endeme, 18, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJ-QD-POR");
            str = EndemeTextFormat.Show_complex(endeme, 18, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-SJH-QDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 18, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJHU-QDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 18, 1, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHU-TQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 18, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHUFTQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 15, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN----POR");
            str = EndemeTextFormat.Show_complex(endeme, 15, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN--D-POR");
            str = EndemeTextFormat.Show_complex(endeme, 15, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN-S-DPOR");
            str = EndemeTextFormat.Show_complex(endeme, 15, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN-SJ-DPOR");
            str = EndemeTextFormat.Show_complex(endeme, 15, 1, "-");  Assert.That(str, Is.equal_to, "MKACLENSJ-QDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 15, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSJHQDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 12, 5, "-");  Assert.That(str, Is.equal_to, "MKA-CL----OR");
            str = EndemeTextFormat.Show_complex(endeme, 12, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CL---POR");
            str = EndemeTextFormat.Show_complex(endeme, 12, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CLE--POR");
            str = EndemeTextFormat.Show_complex(endeme, 12, 2, "-");  Assert.That(str, Is.equal_to, "MKACLEN--POR");
            str = EndemeTextFormat.Show_complex(endeme, 12, 1, "-");  Assert.That(str, Is.equal_to, "MKACLEN-DPOR");
            str = EndemeTextFormat.Show_complex(endeme, 12, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENSDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 11, 5, "-");  Assert.That(str, Is.equal_to, "MKA-C----OR");
            str = EndemeTextFormat.Show_complex(endeme, 11, 4, "-");  Assert.That(str, Is.equal_to, "MKA-CL---OR");
            str = EndemeTextFormat.Show_complex(endeme, 11, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CL--POR");
            str = EndemeTextFormat.Show_complex(endeme, 11, 2, "-");  Assert.That(str, Is.equal_to, "MKACLE--POR");
            str = EndemeTextFormat.Show_complex(endeme, 11, 1, "-");  Assert.That(str, Is.equal_to, "MKACLEN-POR"); // common
            str = EndemeTextFormat.Show_complex(endeme, 11, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENDPOR");
            str = EndemeTextFormat.Show_complex(endeme, 10, 5, "-");  Assert.That(str, Is.equal_to, "MKA-----OR");
            str = EndemeTextFormat.Show_complex(endeme, 10, 4, "-");  Assert.That(str, Is.equal_to, "MKA-C---OR");
            str = EndemeTextFormat.Show_complex(endeme, 10, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CL--OR");
            str = EndemeTextFormat.Show_complex(endeme, 10, 2, "-");  Assert.That(str, Is.equal_to, "MKACL--POR");
            str = EndemeTextFormat.Show_complex(endeme, 10, 1, "-");  Assert.That(str, Is.equal_to, "MKACLE-POR");
            str = EndemeTextFormat.Show_complex(endeme, 10, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENPOR");
            // phase change here from endeme, a 33% low, 67% high approach to 100% high
            str = EndemeTextFormat.Show_complex(endeme,  8, 5, "-");  Assert.That(str, Is.equal_to, "MKA-----");
            str = EndemeTextFormat.Show_complex(endeme,  8, 4, "-");  Assert.That(str, Is.equal_to, "MKA-C---");
            str = EndemeTextFormat.Show_complex(endeme,  8, 3, "-");  Assert.That(str, Is.equal_to, "MKA-CL--");
            str = EndemeTextFormat.Show_complex(endeme,  8, 2, "-");  Assert.That(str, Is.equal_to, "MKA-CLE-");
            str = EndemeTextFormat.Show_complex(endeme,  8, 1, "-");  Assert.That(str, Is.equal_to, "MKA-CLEN"); // common
            str = EndemeTextFormat.Show_complex(endeme,  8, 0, "-");  Assert.That(str, Is.equal_to, "MKACLENS");
            str = EndemeTextFormat.Show_complex(endeme,  7, 5, "-");  Assert.That(str, Is.equal_to, "MK-----" );
            str = EndemeTextFormat.Show_complex(endeme,  7, 4, "-");  Assert.That(str, Is.equal_to, "MKA----" );
            str = EndemeTextFormat.Show_complex(endeme,  7, 3, "-");  Assert.That(str, Is.equal_to, "MKA-C--" );
            str = EndemeTextFormat.Show_complex(endeme,  7, 2, "-");  Assert.That(str, Is.equal_to, "MKA-CL-" );
            str = EndemeTextFormat.Show_complex(endeme,  7, 1, "-");  Assert.That(str, Is.equal_to, "MKA-CLE" );
            str = EndemeTextFormat.Show_complex(endeme,  7, 0, "-");  Assert.That(str, Is.equal_to, "MKACLEN" ); // common
            str = EndemeTextFormat.Show_complex(endeme,  5, 5, "-");  Assert.That(str, Is.equal_to, "-----"   );
            str = EndemeTextFormat.Show_complex(endeme,  5, 4, "-");  Assert.That(str, Is.equal_to, "M----"   );
            str = EndemeTextFormat.Show_complex(endeme,  5, 3, "-");  Assert.That(str, Is.equal_to, "MK---"   );
            str = EndemeTextFormat.Show_complex(endeme,  5, 2, "-");  Assert.That(str, Is.equal_to, "MKA--"   );
            str = EndemeTextFormat.Show_complex(endeme,  5, 1, "-");  Assert.That(str, Is.equal_to, "MKA-C"   );
            str = EndemeTextFormat.Show_complex(endeme,  5, 0, "-");  Assert.That(str, Is.equal_to, "MKACL"   );
            str = EndemeTextFormat.Show_complex(endeme,  3, 5, "-");  Assert.That(str, Is.equal_to, "---"     );
            str = EndemeTextFormat.Show_complex(endeme,  3, 4, "-");  Assert.That(str, Is.equal_to, "---"     );
            str = EndemeTextFormat.Show_complex(endeme,  3, 3, "-");  Assert.That(str, Is.equal_to, "---"     );
            str = EndemeTextFormat.Show_complex(endeme,  3, 2, "-");  Assert.That(str, Is.equal_to, "M--"     );
            str = EndemeTextFormat.Show_complex(endeme,  3, 1, "-");  Assert.That(str, Is.equal_to, "MK-"     );
            str = EndemeTextFormat.Show_complex(endeme,  3, 0, "-");  Assert.That(str, Is.equal_to, "MKA"     ); // common
            str = EndemeTextFormat.Show_complex(endeme,  1, 5, "-");  Assert.That(str, Is.equal_to, "-"       );
            str = EndemeTextFormat.Show_complex(endeme,  1, 4, "-");  Assert.That(str, Is.equal_to, "-"       );
            str = EndemeTextFormat.Show_complex(endeme,  1, 3, "-");  Assert.That(str, Is.equal_to, "-"       );
            str = EndemeTextFormat.Show_complex(endeme,  1, 2, "-");  Assert.That(str, Is.equal_to, "-"       );
            str = EndemeTextFormat.Show_complex(endeme,  1, 1, "-");  Assert.That(str, Is.equal_to, "-"       );
            str = EndemeTextFormat.Show_complex(endeme,  1, 0, "-");  Assert.That(str, Is.equal_to, "M"       );
            str = EndemeTextFormat.Show_complex(endeme,  0, 5, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme,  0, 4, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme,  0, 3, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme,  0, 2, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme,  0, 1, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme,  0, 0, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme, -1, 5, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme, -1, 4, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme, -1, 3, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme, -1, 2, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme, -1, 1, "-");  Assert.That(str, Is.equal_to, ""        );
            str = EndemeTextFormat.Show_complex(endeme, -1, 0, "-");  Assert.That(str, Is.equal_to, ""        );

            _result += Assert.Conclusion;
        }

        #endregion

        #region test sets

        // ----------------------------------------------------------------------------------------
        /// <!-- Fruits -->
        /// <summary>
        ///      Fruit:
        ///      A)pple, B)anana, C)herry, D)ate, E)lderberry, F)ig, G)rape, H)uckleberry,
        ///      I)ta palm, J)ujube, K)umquat, L)emon, M)ango, N)ectarine, O)range, P)lum,
        ///      Q)uince, R)aspberry, S)trawberry, T)angerine, U)gli fruit, V)watermellon
        /// </summary>
        private static EndemeSet Fruits { get
        {
            EndemeSet set = new EndemeSet("Fruit");
            set.Add('A', "apple"      , "");
            set.Add('B', "banana"     , "");
            set.Add('C', "cherry"     , "");
            set.Add('D', "date"       , "");
            set.Add('E', "elderberry" , "");
            set.Add('F', "fig"        , "");
            set.Add('G', "grape"      , "");
            set.Add('H', "huckleberry", "");
            set.Add('I', "ita palm"   , "");
            set.Add('J', "jujube"     , "");
            set.Add('K', "kumquat"    , "");
            set.Add('L', "lemon"      , "");
            set.Add('M', "mango"      , "");
            set.Add('N', "nectarine"  , "");
            set.Add('O', "orange"     , "");
            set.Add('P', "plum"       , "");
            set.Add('Q', "quince"     , "");
            set.Add('R', "raspberry"  , "");
            set.Add('S', "strawberry" , "");
            set.Add('T', "tangerine"  , "");
            set.Add('U', "ugli fruit" , "");
            set.Add('V', "watermellon", "");
            return set;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- TinyEnSet -->
        /// <summary>
        ///      Animal: 
        ///      A)lligator, B)eaver, C)rocodile, D)uck, E)gret, F)rog
        /// </summary>
        public static EndemeSet TinyEndemeSet { get
        {
            EndemeSet set = new EndemeSet("TinySet");
            set.Add('A', "alligator", "");
            set.Add('B', "beaver",    "");
            set.Add('C', "crocodile", "");
            set.Add('D', "duck",      "");
            set.Add('E', "egret",     "");
            set.Add('F', "frog",      "");
            return set;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- WetlandAnimals -->
        /// <summary>
        ///      Animal: 
        ///      A)lligator, B)eaver, C)rocodile, D)uck, E)gret, F)rog, G)ecko, H)erron, I)nsect,
        ///      J)ackal, K)oala, L)izard, M)uskrat, N)ewt, O)tter, P)uma, Q)uahog, R)eptile,
        ///      S)nake, T)urtle, U)ngulate, V)ole     
        /// </summary>
        public static EndemeSet WetlandAnimals { get
        {
            EndemeSet set = new EndemeSet("Animal");
            set.Add('A', "alligator", "");
            set.Add('B', "beaver",    "");
            set.Add('C', "crocodile", "");
            set.Add('D', "duck",      "");
            set.Add('E', "egret",     "");
            set.Add('F', "frog",      "");
            set.Add('G', "gecko",     "");
            set.Add('H', "herron",    "");
            set.Add('I', "insect",    "");
            set.Add('J', "jackal",    "");
            set.Add('K', "koala",     "");
            set.Add('L', "lizard",    "");
            set.Add('M', "muskrat",   "");
            set.Add('N', "newt",      "");
            set.Add('O', "otter",     "");
            set.Add('P', "puma",      "");
            set.Add('Q', "quahog",    "");
            set.Add('R', "reptile",   "");
            set.Add('S', "snake",     "");
            set.Add('T', "turtle",    "");
            set.Add('U', "ungulate",  "");
            set.Add('V', "vole",      "");
            return set;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Is_Ok_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Is_Ok_test()
        {
            Assert.ThingsAbout("Is", "Ok");

            Assert.That(Is.Ok(3.5, Is.the_same_as     , 3.5), Is.equal_to, true );  Assert.That(Is.Ok(3.5, Is.the_same_as     , 2.3), Is.equal_to, false);
            Assert.That(Is.Ok(3.5, Is.equal_to        , 3.5), Is.equal_to, true );  Assert.That(Is.Ok(3.5, Is.equal_to        , 2.3), Is.equal_to, false);
            Assert.That(Is.Ok(3.5, Is.the_same_sets_as, 3.5), Is.equal_to, true );  Assert.That(Is.Ok(3.5, Is.the_same_sets_as, 2.3), Is.equal_to, false);
            Assert.That(Is.Ok(3.5, Is.not_equal_to    , 3.5), Is.equal_to, false);  Assert.That(Is.Ok(3.5, Is.not_equal_to    , 2.3), Is.equal_to, true );

            Assert.That(Is.Ok("hi world", Is.the_same_as     , "hi world"), Is.equal_to, true);  Assert.That(Is.Ok("hi world", Is.the_same_as     , "hi there"), Is.equal_to, false);
            Assert.That(Is.Ok("hi world", Is.equal_to        , "hi world"), Is.equal_to, true);  Assert.That(Is.Ok("hi world", Is.equal_to        , "hi there"), Is.equal_to, false);
            Assert.That(Is.Ok("hi world", Is.the_same_sets_as, "hi world"), Is.equal_to, true);  Assert.That(Is.Ok("hi world", Is.the_same_sets_as, "hi there"), Is.equal_to, false);
            Assert.That(Is.Ok("hi world", Is.not_equal_to    , "hi world"), Is.equal_to, false); Assert.That(Is.Ok("hi world", Is.not_equal_to    , "hi there"), Is.equal_to, true );
            
            string detail = Assert.Detail;
            _result += Assert.Conclusion;
        }

        private static EndemeReference TestReference { get
        {
            if (_testRef == null)
            {
                _testRef = new EndemeReference();
                _testRef.Add(WetlandAnimals);
                _testRef.Add(Fruits);
            }
            return _testRef;
        } }
        private static EndemeReference _testRef;

        #endregion
    }

    #region Test classes

    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeMatchTest -->
    /// <summary>
    ///      The EndemeMatchTest class supports endeme matching tests
    /// </summary>
    public class EndemeMatchTest
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public string Name    { get; set; }
        public string Endeme1 { get; set; }
        public string Endeme2 { get; set; }
        public double MinTgt  { get; set; }
        public double MaxTgt  { get; set; }
        public double Match   { get; set; }
        public int    Ranking { get; set; }
        public bool   Ok      { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeMatchTest(string endeme1, string endeme2, double tgtLo, double tgtHi, int tgtRank)
        {
            Name    = endeme1 + "," + endeme2;
            Endeme1 = endeme1;
            Endeme2 = endeme2;
            MaxTgt  = tgtHi  ;
            MinTgt  = tgtLo  ;
            Ranking = tgtRank;
            Match   = -1.0   ;
            Ok      = true   ;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RunMatch_testCase()
        {
            Endeme en1 = new Endeme(EndemeSet.WetlandAnimals, Endeme1);
            Endeme en2 = new Endeme(EndemeSet.WetlandAnimals, Endeme2);
            double match1 = en1.Match(en2, WeightFormula.Refined);
            double match2 = en2.Match(en1, WeightFormula.Refined);
            Match = match1;


            Assert.That(match2, Is.equal_to, match1);
            Assert.That(MinTgt, Is.less_than_or_equal_to, match1);
            Assert.That(match1, Is.less_than_or_equal_to, MaxTgt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "" + Ok.ToString().PadRight(6) + Ranking.ToString().PadLeft(2) + ", " + (((int)(0.5+1000*Match))/10.0).ToString().PadLeft(3) + "%, " + Name;
        }
    }

    // ---------------------------------------------------------------------------------------------
    /// <!-- EndemeTestItem -->
    /// <summary>
    ///      The EndemeTestItem class supports endeme matching tests
    /// </summary>
    /// <remarks></remarks>
    public class EndemeTestItem
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
	    public Endeme Endeme1;
	    public Endeme Endeme2;
        public int    SrcRank;  // the rank resulting from the test run
	    public int    TgtRank;  // the target rank for the test run
	    public double Result;   // the raw result of the test run


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
	    public EndemeTestItem(string en1, string en2, int rank)
	    {
		    Endeme1 = new Endeme(EndemeSet.WetlandAnimals, en1);
		    Endeme2 = new Endeme(EndemeSet.WetlandAnimals, en2);
		    TgtRank = rank;
            SrcRank = 0;
            Result  = 0.0;
	    }


        // ----------------------------------------------------------------------------------------
        /// <!-- CalculateTable -->
        /// <summary>
        ///      Formats the results of a test run
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public static string CalculateTable(Dictionary<string, EndemeTestItem> test)
        {
            var sortedDict = from entry in test orderby entry.Value.Result ascending select entry;

            string diff  = "";
            string delim = "";
            foreach (KeyValuePair<string, EndemeTestItem> item in sortedDict)
            {
                EndemeTestItem val = item.Value;
                diff += delim + Math.Abs(val.TgtRank - val.SrcRank).ToString().PadLeft(2)
                    + "  " + val.Result.ToString().PadLeft(20)
                    + "  " + item.Key.PadRight(3)
                    + " (" + (val.TgtRank + "-" + val.SrcRank.ToString()).PadLeft(5) + " = " + (val.TgtRank - val.SrcRank).ToString().PadLeft(3) + ")"
                    + "  '" + (val.Endeme1.ToString() + "'").PadLeft(25) + " v '" + val.Endeme2.ToString() + "'";
                delim = "\r\n";
            }

            return diff;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EffectivenessOf -->
        /// <summary>
        ///      Calculates the reverse of the effectiveness of a test run - the lower the number, the better
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public static int EffectivenessOf(Dictionary<string, EndemeTestItem> test)
        {
            int effectiveness = 0;
            foreach (KeyValuePair<string, EndemeTestItem> item in test)
            {
                effectiveness += Math.Abs(item.Value.SrcRank - item.Value.TgtRank);
            }

            return effectiveness;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RankResults -->
        /// <summary>
        ///      Ranks the results of a test run
        /// </summary>
        /// <param name="test"></param>
        /// <remarks>you must run this before running EffectivenessOf or ?</remarks>
        public static void RankResults(Dictionary<string, EndemeTestItem> test)
        {
            var sortedDict = from entry in test orderby entry.Value.Result ascending select entry;

            int i = 0;
            foreach (KeyValuePair<string, EndemeTestItem> item in sortedDict)
            {
                item.Value.SrcRank = i;
                i++;
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
            return "" + " (tgt:" + TgtRank + "),(cur:" + SrcRank + "),(" + Result + ")" + Endeme1.ToString().PadLeft(22) + " vs " + Endeme2.ToString().PadRight(22);
        }
    }

    #endregion
}
