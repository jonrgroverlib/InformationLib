//--------------------------------------------------------------------------------------------------
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
//--------------------------------------------------------------------------------------------------
using InformationLib.Testing;         // for Here
using System;                         // for 
using System.ComponentModel;          // for BindingList
using System.Text.RegularExpressions; // for Regex
using System.Windows.Forms;           // for CheckBox

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- UITests -->
    /// <summary>
    ///      The UITests class tests the classes in the UI Library
    /// </summary>
    public class UITests
    {
        private Result _result;


        // ----------------------------------------------------------------------------------------
        /// <!-- AllTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            _result = new Result("Hard Data tests");


            Plaster_ComboBoxValue_test();
            Scrape_OnlyDateTime_test  ();                  


            return _result.AsciiDetailResults + "\r\n" + _result.SummaryMessage();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Plaster_ComboBoxValue_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Plaster_ComboBoxValue_test()
        {
            Assert.ThingsAbout("Plaster", "ComboBoxValue");

            Plaster_ComboBoxValue_testcase1();  // simple drop with displaymembers only
            Plaster_ComboBoxValue_testcase2();  // more complex with both display and value members
            Plaster_ComboBoxValue_testcase3();  // this one uses Question which has to be handled with reflection

            _result += Assert.Conclusion;
        }
        private static void Plaster_ComboBoxValue_testcase1()
        {
            string[] list = { "high", "medium", "low" };
            ComboBox drop = new ComboBox();
            foreach (string str in list) drop.Items.Add(str);
            Assert.That(drop.Items.Count, Is.equal_to, 3);


            Assert.That(drop.IsDisposed, Is.equal_to, false);
            Plaster_ComboBoxValue_testcase(drop, "medium", "medium", 1);
            Assert.That(drop.IsDisposed, Is.equal_to, false);
            Plaster_ComboBoxValue_testcase(drop, "ok",     "ok",    -1);
            Plaster_ComboBoxValue_testcase(drop, ""  ,     ""  ,    -1);
            Plaster_ComboBoxValue_testcase(drop, null,     ""  ,    -1);
            drop.Dispose();
        }
        private static void Plaster_ComboBoxValue_testcase2()
        {
            ComboBox drop = DropHiMedLowBindingList;


            // --------------------------------------------------------------------------
            //  Drop values into it
            // --------------------------------------------------------------------------
            Plaster_ComboBoxValue_testcase(drop, "hi"    , "high"  ,  0);
            Plaster_ComboBoxValue_testcase(drop, "medium", "medium",  1);
            Plaster_ComboBoxValue_testcase(drop, "over"  , "over"  , -1);
            Plaster_ComboBoxValue_testcase(drop, ""      , ""      , -1);
            Plaster_ComboBoxValue_testcase(drop, null    , ""      , -1);
        }
        private static ComboBox DropHiMedLowBindingList { get
        {
            // --------------------------------------------------------------------------
            //  Make a test list
            // --------------------------------------------------------------------------
            BindingList<ListItem> list = new BindingList<ListItem>();
            list.Add(new ListItem("hi", "high"));
            list.Add(new ListItem("med", "medium"));
            list.Add(new ListItem("lo", "low"));


            // --------------------------------------------------------------------------
            //  Put it in a combo box
            // --------------------------------------------------------------------------
            ComboBox drop = new ComboBox();
            foreach (ListItem item in list)
                drop.Items.Add(item);  // we have to do it this way because we don't displaying the ComboBox
            drop.DisplayMember = "Descr";
            drop.ValueMember = "ID";


            return drop;
        } }
        private static void Plaster_ComboBoxValue_testcase3()
        {
            // --------------------------------------------------------------------------
            //  Make a test list
            // --------------------------------------------------------------------------
            BindingList<YesNoQuestion> list = new BindingList<YesNoQuestion>();
            list.Add(new YesNoQuestion("hi",  "high?"));
            list.Add(new YesNoQuestion("med", "medium?"));
            list.Add(new YesNoQuestion("lo",  "low?"));


            // --------------------------------------------------------------------------
            //  Put it in a combo box
            // --------------------------------------------------------------------------
            ComboBox drop = new ComboBox();
            foreach (YesNoQuestion item in list)
                drop.Items.Add(item);  // we have to do it this way because we don't displaying the ComboBox
            drop.DisplayMember = "Title";
            drop.ValueMember   = "Text";


            // --------------------------------------------------------------------------
            //  Drop values into it
            // --------------------------------------------------------------------------
            Plaster_ComboBoxValue_testcase(drop, "hi",      "high?",   0);
            Plaster_ComboBoxValue_testcase(drop, "med",     "medium?", 1);
            Plaster_ComboBoxValue_testcase(drop, "over?",   "over?",   -1);
            Plaster_ComboBoxValue_testcase(drop, "medium?", "medium?", 1);
            Plaster_ComboBoxValue_testcase(drop, "",        "",        -1);
            Plaster_ComboBoxValue_testcase(drop, null,      "",        -1);
        }
        private static void Plaster_ComboBoxValue_testcase(ComboBox drop, string input
            , string strTgt, int idxTgt)
        {
            Assert.That(drop.IsDisposed, Is.equal_to, false);
            Form form = new Form();
            form.Controls.Add(drop);  Assert.That(drop.IsDisposed, Is.equal_to, false);
            //_result += "";
            Plaster.Value(form, ref drop, input);
            Assert.That(drop.Text, Is.equal_to, strTgt);
            Assert.That(drop.SelectedIndex, Is.equal_to, idxTgt);
            Assert.That(drop.IsDisposed, Is.equal_to, false);
            //form.Dispose();
            //Assert.That(!drop.IsDisposed);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Scrape_OnlyDate_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Scrape_OnlyDateTime_test()
        {
            Assert.ThingsAbout("Scrape", "OnlyDateTime");


            string output = Scrape.OnlyDateTime("09/29/2008 04:44:00 PM");
            Assert.That(output, Is.equal_to, "09/29/2008 04:44:00 PM");

            _result += Assert.Conclusion;
        }

        //// --------------------------------------------------------------------------------------
        ///// <!-- ThreeValueCheckBox_test -->
        ///// <summary>
        ///// 
        ///// </summary>
        //private void ThreeValueCheckBox_test()
        //{
        //    _result = "UNTESTED";
        //    CheckBox chk = new CheckBox();
        //    string nullIndicator = "unknown value";
        //    string setIndicator = "known value";
        //    string label = "Label";


        //    chk.Text = label+" ("+nullIndicator+")";
        //    chk.Checked = false;     /* input  <------- state information ------->  <--- result ---> */ 
        //    ThreeValueCheckBox_teststep(false, false, label+" ("+nullIndicator+")", SqlBoolean.Null,  chk, nullIndicator, setIndicator);
        //    ThreeValueCheckBox_teststep(true,  true,  label+" ("+setIndicator+")",  SqlBoolean.True,  chk, nullIndicator, setIndicator);
        //    ThreeValueCheckBox_teststep(false, false, label+" ("+setIndicator+")",  SqlBoolean.False, chk, nullIndicator, setIndicator);
        //    ThreeValueCheckBox_teststep(true,  true,  label+" ("+nullIndicator+")", SqlBoolean.Null,  chk, nullIndicator, setIndicator);
        //    ThreeValueCheckBox_teststep(false, false, label+" ("+nullIndicator+")", SqlBoolean.Null,  chk, nullIndicator, setIndicator);


        //    chk.Text = label;
        //    chk.Checked = false;     /* input   state info   <--- result ---> */ 
        //    ThreeValueCheckBox_teststep(false, false, label, SqlBoolean.False, chk, nullIndicator, setIndicator);
        //    ThreeValueCheckBox_teststep(true,  true,  label, SqlBoolean.True,  chk, nullIndicator, setIndicator);
        //}
        //private static void ThreeValueCheckBox_teststep(bool input
        //    , bool checkTarget, string labelTarget, SqlBoolean valueTarget, CheckBox chk, string nullIndicator, string setIndicator)
        //{
        //    SqlBoolean result;
        //    chk.Checked = input;
        //    result = _.ThreeValueCheckBox(chk, nullIndicator, setIndicator, true);
        //    ok &= Assert.That(result, __.equals, valueTarget);
        //    ok &= Assert.That(chk.Text, __.equals, labelTarget);
        //    ok &= Assert.That(chk.Checked, __.equals, checkTarget);
        //}

    }
}
