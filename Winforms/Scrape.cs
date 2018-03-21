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
using InformationLib.SoftData;        // for TreatAs
using System;                         // for DateTime
using System.Collections.Generic;     // for Dictionary
using System.ComponentModel;          // for BindingList
using System.Reflection;              // for PropertyInfo, BindingFlags
using System.Text.RegularExpressions; // for Regex
using System.Windows.Forms;           // for CheckedListBox

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Scrape -->
    /// <summary>
    ///      The Scrape class gets data from form controls and provides protection from SQL
    ///      injection (see also the Plaster class)
    /// </summary>
    /// <remarks>
    ///      This is primarily a static utility class, but since many of the methods here have a
    ///      'form' parameter, it is also an instantiable class with one data member
    /// 
    ///      Scrape does not use Plaster although Plaster uses Scrape
    ///      
    /// 
    ///      old beta code
    /// </remarks>
    public class Scrape
    {
        // ----------------------------------------------------------------------------------------
        //  Instance members
        // ----------------------------------------------------------------------------------------
        public Form _Form { set { _form = value; } } private Form _form;


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public Scrape(Form form) { _form = form; }


        // ----------------------------------------------------------------------------------------
        /// <!-- Active -->
        /// <summary>
        ///      Checks whether the control exists and is active in the current form
        ///      Determines whether the specified control exists
        /// </summary>
        /// <param name="form"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool Active(Form form, Control control)
        {
            if (form == null)
                MessageBox.Show("there is no form recorded");
            bool isActive = (form != null && control != null && form.Contains(control)); //  && !control.IsDisposed
            return isActive;
        }
        public bool Active(Control control) { return Active(_form, control); }
        public static bool Active(Form form, string controlName) { return Active(form, Control(form, controlName)); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Active -->
        /// <summary>
        ///      Checks whether the particular tool strip item is active on the form
        /// </summary>
        /// <param name="form"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Active(Form form, ToolStripItem item)
        {
            if (form != null && item != null)
            {
                if (item.Owner != null
                    && item.Owner.Parent == form)
                    return true;
                if (item.OwnerItem != null
                    && item.OwnerItem.Owner != null
                    && item.OwnerItem.Owner.Parent == form)
                    return true;
                if (item.OwnerItem != null
                    && item.OwnerItem.OwnerItem != null
                    && item.OwnerItem.OwnerItem.Owner != null
                    && item.OwnerItem.OwnerItem.Owner.Parent == form)
                    return true;
            }
            return false;
        }
        public bool Active(ToolStripItem item) { return Active(_form, item); }


        // ----------------------------------------------------------------------------------------
        /// <!-- AsNoon -->
        /// <summary>
        ///      Returns the specified date with the time set to noon
        /// </summary>
        /// <remarks>
        ///      Not presently used
        /// </remarks>
        /// <param name="pick"></param>
        public static TimeDate_old AsNoon(Form form, DateTimePicker pick)
        {
            if (Active(form, pick))
                return new TimeDate_old(pick.Value, DateTimeKind.Utc).AddHours(12);
            else
                return TimeDate_old.Null;
        }
        public InformationLib.SoftData.TimeDate_old AsNoon(DateTimePicker pick) { return AsNoon(_form, pick); }


        // ----------------------------------------------------------------------------------------
        /// <!-- AutoComplete -->
        /// <summary>
        ///      Auto-completion of typing into a combo box
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="a"></param>
        /// <param name="blnLimitToList"></param>
        public static void AutoComplete(ComboBox cb, KeyPressEventArgs a, bool blnLimitToList)
        {
            string strFindStr = "";

            if (a.KeyChar == (char)8)
            {
                if (cb.SelectionStart <= 1) { cb.Text = "";  return; }
                if (cb.SelectionLength == 0)
                    strFindStr = cb.Text.Substring(0, cb.Text.Length - 1);
                else strFindStr = cb.Text.Substring(0, cb.SelectionStart - 1);
            }
            else
            {
                if (cb.SelectionLength == 0)
                    strFindStr = cb.Text + a.KeyChar;
                else strFindStr = cb.Text.Substring(0, cb.SelectionStart) + a.KeyChar;
            }

            int intIdx = -1;


            // --------------------------------------------------------------------------
            //  Search the string in the ComboBox list
            // --------------------------------------------------------------------------
            intIdx = cb.FindString(strFindStr);

            if (intIdx != -1)
            {
                cb.SelectedText = "";
                cb.SelectedIndex = intIdx;
                cb.SelectionStart = strFindStr.Length;
                cb.SelectionLength = cb.Text.Length;
                a.Handled = true;
            }
            else
                a.Handled = blnLimitToList;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- BestLabel -->
        /// <summary>
        ///      For example: "(Save)"
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        public static string BestLabel(Form form, ToolStripItem btn)
        {
            if (Active(form, btn))
            {
                string best = Regex.Replace(btn.Text, "&", "");
                if (string.IsNullOrEmpty(btn.Text))
                {
                    if (string.IsNullOrEmpty(btn.ToolTipText)) best = "(" + btn.Name + ")";
                    else best = "(" + btn.ToolTipText + ")";
                }
                return best;
            }
            else
                return "";
        }
        public static string BestLabel(Form form, ButtonBase btn)
        {
            if (Active(form, btn))
            {
                string best = Regex.Replace(btn.Text, "&", "");
                if (string.IsNullOrEmpty(btn.Text))
                    best = "(" + btn.Name + ")";
                return best;
            }
            else
                return "";
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Bool -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Boolean Bool(Form form, Control control, bool defaultValue)
        {
            Boolean value = defaultValue;
            if (Active(form, control))
            {
                if (control.GetType() == typeof(RadioButton))
                    value = ((RadioButton)control).Checked;
                else
                    value = TreatAs.BoolValue(control.Text, defaultValue);
            }
            return value;
        }
        public Boolean Bool(Control control, bool defaultValue) { return Bool(_form, control, defaultValue); }
        public static bool Bool(Form form, string controlName, bool defaultValue) { return Bool(form, Control(form, controlName), defaultValue); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Control -->
        /// <summary>
        ///      Returns the specified control or a dummy control if not found
        /// </summary>
        /// <param name="form"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public static Control Control(Form form, string controlName)
        {
            Control control = new Control();
            if (form != null && !string.IsNullOrEmpty(controlName))
            {
                Control[] ctl = form.Controls.Find(controlName, true);
                if (ctl.Length > 0)
                    control = ctl[0];
            }
            return control;
        }
        public Control Control(string controlName) { return Control(_form, controlName); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Dates -->
        /// <summary>
        ///      operates off of the .Text property of the control
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="defaultValue"></param>
        /// <returns>will not return null, if null is passed to it will return Date.Null</returns>
        public static TimeDate_old Dates(Form form, Control control, TimeDate_old defaultDate)
        {
            TimeDate_old date;
            if (defaultDate == null) defaultDate = TimeDate_old.Null;
            date = defaultDate.DateOnly;
            if (Active(form, control))
            {
                if (Regex.IsMatch(control.Text, "^[0-3]?[0-9]/[0-3]?[0-9]/....$"))
                    date = (new TimeDate_old(control.Text, DateTimeKind.Utc));
                else
                    date = (new TimeDate_old(control.Text, DateTimeKind.Local)).Utc();
                if (date.IsNull)
                    date = defaultDate;
            }
            return date;
        }
        public TimeDate_old Dates(Control control, TimeDate_old defaultDate) { return Dates(_form, control, defaultDate); }
        public static TimeDate_old Dates(Form form, string controlName, TimeDate_old defaultDate) { return Dates(form, Control(form, controlName), defaultDate); }        /// <summary>Assumes Date.Null for default</summary>
        public static TimeDate_old Dates(Form form, string controlName) { return Dates(form, controlName, TimeDate_old.Null); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Dates -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="dateField"></param>
        /// <param name="timeField"></param>
        /// <param name="defaultDate"></param>
        /// <returns></returns>
        public static TimeDate_old Dates(Form form, string dateField, string timeField, TimeDate_old defaultDate)
        {
            TimeDate_old date = new TimeDate_old(
                  Dates(form, dateField, defaultDate).AddHours(12).DateOnly
                , Time(form, timeField, defaultDate.CLRFormat.TimeOfDay)
                , DateTimeKind.Local);
            return date;
        }
        public static TimeDate_old Dates(Form form, DateTimePicker dateField, DateTimePicker timeField, TimeDate_old defaultDate)
        {
            TimeDate_old date = defaultDate;
            if (Active(form, dateField) && Active(form, timeField))
            {
                date = new TimeDate_old(
                      Dates(form, dateField, defaultDate).AddHours(6).DateOnly
                    , Time(form, timeField, defaultDate.CLRFormat.TimeOfDay)
                    , DateTimeKind.Local);
            }
            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Dictionary -->
        /// <summary>
        ///      Extracts the values from a ComboBox into a Dictionary<string,string>
        /// </summary>
        /// <param name="drop"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Dictionary(Form form, ComboBox drop)
        {
            Dictionary<string, string> hash = new Dictionary<string, string>();
            if (Active(form, drop))
                foreach (ListItem item in drop.Items)
                    hash.Add(item.ID, item.Descr);
            return hash;
        }
        public Dictionary<string, string> Dictionary(ComboBox drop) { return Dictionary(_form, drop); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Digits -->
        /// <summary>
        ///      Almost the same thing as OnlyDigits
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="controlName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Digits(Form form, string controlName, string defaultValue)
        {
            return OnlyDigits(Text(form, controlName, defaultValue));
        }
        public string Digits(string controlName, string defaultValue) { return Digits(_form, controlName, defaultValue); }


        // ----------------------------------------------------------------------------------------
        /// <!-- FindIndexOf -->
        /// <summary>
        ///      Search the combobox manually for value
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FindIndexOf(Form form, ComboBox drop, object value)
        {
            int idx = -1;
            if (drop.Items.Count > 0)
            {
                Type type = drop.Items[0].GetType();
                switch (type.Name)
                {
                    case "ListItem" : idx = FindIndexOf_ListItem(form, drop, value);  break;
                    default         : idx = FindIndexOf_generic(form, drop, value);   break;
                }
            }
            return idx;
        }
        public int FindIndexOf(ComboBox drop, object value) { return FindIndexOf(_form, drop, value); }


        // ----------------------------------------------------------------------------------------
        /// <!-- FindIndexOf_generic -->
        /// <summary>
        ///      If we don't know the class of the items in the combobox, we must use reflection
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int FindIndexOf_generic(Form form, ComboBox drop, object value)
        {
            int idx = -1;
            if (Active(form, drop))
            {
                if (!string.IsNullOrEmpty(drop.ValueMember) && drop.Items.Count > 0)
                {
                    string valueMember = drop.ValueMember;
                    int count = drop.Items.Count;
                    object item = drop.Items[0];
                    Type type = item.GetType();


                    for (int i = 0; idx < 0 && i < count; ++i)
                    {
                        // -----------------------------------------------------------------90
                        //  Check new property instance value for equality
                        // -----------------------------------------------------------------90
                        PropertyInfo itemProperty = type.GetProperty(valueMember
                            , BindingFlags.Public | BindingFlags.Instance);
                        if (itemProperty == null)
                            Pause();
                        else
                        {
                            object itemValue = itemProperty.GetValue(drop.Items[i], null);
                            if (itemValue == value)
                                idx = i;
                        }
                    }
                }
            }
            return idx;
        }

        private static void Pause()
        {
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FindIndexOf_ListItem -->
        /// <summary>
        ///      If the items in the combobox are ListItem's then we can cast to them to ListItem
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int FindIndexOf_ListItem(Form form, ComboBox drop, object value)
        {
            int idx = -1;
            if (Active(form, drop))
            {
                int count = drop.Items.Count;
                for (int i = 0; idx < 0 && i < count; ++i)
                {
                    ListItem item = (ListItem)drop.Items[i];
                    if (item.ID == value.ToString())
                        idx = i;
                }
            }
            return idx;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- LocalToUtc -->
        /// <summary>
        ///      Converts a screen element containing a local date/time to a UTC Date
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static TimeDate_old LocalToUtc(Form form, TextBox txt)
        {
            if (Active(form, txt))
                return new TimeDate_old(OnlyDateTime(txt.Text), DateTimeKind.Local).Utc();
            else
                return TimeDate_old.Null;
        }
        public TimeDate_old LocalToUtc(TextBox txt) { return LocalToUtc(_form, txt); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Name -->
        /// <summary>
        ///      Returns the name of the control if it is not null
        /// </summary>
        /// <param name="control"></param>
        /// <param name="defaultName"></param>
        /// <returns></returns>
        public static string Name(Form form, Control control, string defaultName)
        {
            if (Active(form, control))
                return control.Name;
            else
                return defaultName;
        }
        public string Name(Control control, string defaultName) { return Name(_form, control, defaultName); }


        // ----------------------------------------------------------------------------------------
        /// <!-- OnlyDigits -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string OnlyDigits(Form form, TextBox txt)
        {
            if (Active(form, txt))
                return Regex.Replace(txt.Text, "[^0-9]", "");
            else
                return "";
        }
        public string OnlyDigits(TextBox txt) { return OnlyDigits(_form, txt); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Radio -->
        /// <summary>
        ///      extracts data from a named radio button
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="ifTrue"></param>
        /// <param name="ifFalse"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Radio(Form form, Control control, string ifTrue, string ifFalse, string defaultValue)
        {
            if (defaultValue == null) defaultValue = "";
            string value = defaultValue;
            if (Active(form, control))
            {
                if (control.GetType() == typeof(RadioButton))
                {
                    if (((RadioButton)control).Checked)
                        value = ifTrue;
                    else value = ifFalse;
                }
            }
            return value;
        }
        public string Radio(Control control, string ifTrue, string ifFalse, string defaultValue) { return Radio(_form, control, ifTrue, ifFalse, defaultValue); }
        public static string Radio(Form form, string controlName, string ifTrue, string ifFalse, string defaultValue) { return Radio(form, Control(form, controlName), ifTrue, ifFalse, defaultValue); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Radio -->
        /// <summary>
        ///      Extracts boolean condition from two named radio buttons, one indicating true and one false
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="radTrue"></param>
        /// <param name="radFalse"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool Radio(Form form, Control radTrue, Control radFalse, bool defaultValue)
        {
            bool value = defaultValue;
            if (Active(form, radTrue)
                && Active(form, radFalse)
                && (radTrue.GetType() == typeof(RadioButton))
                && (radFalse.GetType() == typeof(RadioButton))
                )
            {
                if (((RadioButton)radTrue).Checked) value = true;
                if (((RadioButton)radFalse).Checked) value = false;
            }
            return value;
        }
        public bool Radio(Control radTrue, Control radFalse, bool defaultValue) { return Radio(_form, radTrue, radFalse, defaultValue); }
        public static bool Radio(Form form, string trueCtlName, string falseCtlName, bool defaultValue) { return Radio(form, Control(form, trueCtlName), Control(form, falseCtlName), defaultValue); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Text -->
        /// <summary>
        ///      Extracts text data from a control on a form
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Text(Form form, Control control, string defaultValue)
        {
            string str = defaultValue;
            if (Active(form, control))
            {
                Type type = control.GetType();
                switch (type.Name.ToString())
                {
                    case "GroupBox": throw new Exception("ack"); // radio buttons
                    default: str = WithoutSqlInjection(control.Text); break;
                }
            }
            return str;
        }
        public string Text(Control control, string defaultValue) { return Text(_form, control, defaultValue); }
        public static string Text(Form form, string controlName, string defaultValue) { return Text(form, Control(form, controlName), defaultValue); }


        // ----------------------------------------------------------------------------------------
        /// <!-- TextOf -->
        /// <summary>
        ///      Returns the visible text of the control
        /// </summary>
        public static string TextOf(Form form, Button btn)
        {
            if (Active(form, btn))
                return Regex.Replace(btn.Text, "&", "");
            else
                return "";
        }
        public string TextOf(Button btn) { return TextOf(_form, btn); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Time -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="defaultTime"></param>
        /// <returns></returns>
        public static TimeSpan Time(Form form, Control control, TimeSpan defaultTime)
        {
            TimeSpan time = defaultTime;
            if (Active(form, control))
            {
                if (control.GetType() == typeof(DateTimePicker))
                {
                    DateTimePicker picker = ((DateTimePicker)control);
                    time = picker.Value.TimeOfDay;
                }
                else
                    time = TreatAs.Time(OnlyTime(control.Text), defaultTime);
            }
            return time;
        }
        public TimeSpan Time(Control control, TimeSpan defaultTime) { return Time(_form, control, defaultTime); }
        public static TimeSpan Time(Form form, string controlName, TimeSpan defaultTime) { return Time(form, Control(form, controlName), defaultTime); }


        // ----------------------------------------------------------------------------------------
        /// <!-- ValidPickerDate -->
        /// <summary>
        ///      Converts a date to one accepted by a DateTimePicker
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ValidPickerDate(object obj)
        {
            DateTime date = TimeDate_old.ClrDate(obj, DateTimePicker.MinimumDateTime);
            if (date < DateTimePicker.MinimumDateTime)
                return DateTimePicker.MinimumDateTime;
            if (date > DateTimePicker.MaximumDateTime)
                return DateTimePicker.MaximumDateTime;
            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Value -->
        /// <summary>
        ///      A more powerful version like SelectedValue.ToString()
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="textOK">is it OK to use thetext value of the Dropdown if the SelectedValue isn't found?</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Value(Form form, Control control, bool textOK, string defaultValue)
        {
            string str = defaultValue;
            if (Active(form, control))
            {
                str = "XXYYZZ";
                Type type = control.GetType();
                switch (type.Name.ToString())
                {
                    case "ComboBox" :
                        ComboBox drop = (ComboBox)control;
                        str = TreatAs.StrValue(drop.SelectedValue, "XXYYZZ");
                        if (textOK) // is it OK to use thetext value of the Dropdown if the SelectedValue isn't found?
                        {
                            if (str == "XXYYZZ")  str = WithoutSqlInjection(drop.SelectedText);
                            if (str == "")        str = WithoutSqlInjection(drop.Text);
                        }
                        if (str == "XXYYZZ")  str = defaultValue;
                        break;
                    default : str = WithoutSqlInjection(control.Text); break;
                }
            }
            return str;
        }
        public        string Value(Control control,               bool textOK, string defaultValue) { return Value(_form, control,                    textOK, defaultValue); }
        public static string Value(Form form, string controlName, bool textOK, string defaultValue) { return Value(form,  Control(form, controlName), textOK, defaultValue); }
        public static string Value(Form form, string controlName, bool textOK                     ) { return Value(form,  controlName,                textOK, ""          ); }


        #region String Utilities


        // ----------------------------------------------------------------------------------------
        /// <!-- OnlyDateTime -->
        /// <summary>
        ///      Extracts only characters from a string that can be used for time
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string OnlyDateTime(string str) { return Regex.Replace(str, "[^-0-9:./ PAMpam]", "");  }


        // ----------------------------------------------------------------------------------------
        /// <!-- OnlyDigits -->
        /// <summary>
        ///      Removes all non-digits from a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string OnlyDigits(string str) { return Regex.Replace(str, "[^0-9]", "");          }


        // ----------------------------------------------------------------------------------------
        /// <!-- OnlyMath -->
        /// <summary>
        ///      Removes all non-math symbols froma string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string OnlyMath(string str) { return Regex.Replace(str, @"[^\-+*/%0-9 ^]", ""); }


        // ----------------------------------------------------------------------------------------
        /// <!-- OnlyTime -->
        /// <summary>
        ///      Extracts only characters from a string that can be used for time
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string OnlyTime(string str) { return Regex.Replace(str, "[^-0-9:.PAMpam]", "");  }


        // ----------------------------------------------------------------------------------------
        /// <!-- WithoutSqlInjection -->
        /// <summary>
        ///      Clobbers most SQL injection attempts while maintaining the sense of the comments
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string WithoutSqlInjection(string str)
        {
            // sql injections can be hidden with inline comments
            str = Regex.Replace(str, @"\*/", "}");
            str = Regex.Replace(str, @"/\*", "{");

            // parens and brackets are often used
            str = Regex.Replace(str, @"\(", "{");
            str = Regex.Replace(str, @"\)", "}");
            str = Regex.Replace(str, @"\[", "{");
            str = Regex.Replace(str, @"\]", "}");

            // quotes and semicolons can begin them
            str = Regex.Replace(str, "['\"]", "`");
            str = Regex.Replace(str, ";", ".");

            // various symbols are used
            str = Regex.Replace(str, "&",   " and ");
            str = Regex.Replace(str, @"\+", " plus ");
            str = Regex.Replace(str, @"\*", " star ");
            str = Regex.Replace(str, "=",   " equals ");
            str = Regex.Replace(str, "%",   " percent ");
            str = Regex.Replace(str, "<",   " is less than ");
            str = Regex.Replace(str, ">",   " is greater than ");

            // special variables and hex values
            str = Regex.Replace(str, "@@", " at at ");
            str = Regex.Replace(str, "0x", "zero x ");

            // line comments can end them
            str = Regex.Replace(str, "__",  "--");
            str = Regex.Replace(str, "-_",  "--");
            str = Regex.Replace(str, "--$", "-");
            str = Regex.Replace(str, "-- ", "- ");
            str = Regex.Replace(str, "--", "__");
            str = Regex.Replace(str, "_-", "__");

            // stored procedures may be invoked
            str = Regex.Replace(str, "xp_",  "xp-",  RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "sp_",  "sp-",  RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "st_",  "st-",  RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "usp_", "usp-", RegexOptions.IgnoreCase);

            // running other things might be tried
            str = Regex.Replace(str, "sys",   " system ",    RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "exec ", " executive ", RegexOptions.IgnoreCase);

            // compound word commands
            str = Regex.Replace(str, "waitfor",   "wait for",   RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "rollback",  "roll back",  RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "password",  "pass word",  RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "shutdown",  "shut down",  RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "benchmark", "bench mark", RegexOptions.IgnoreCase);

            // table mangling statements
            str = Regex.Replace(str, "drop +table",     "drop-table",     RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "insert +into",    "insert-into",    RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "delete +from",    "delete-from",    RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "create +table",   "create-table",   RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "truncate +table", "truncate-table", RegexOptions.IgnoreCase);

            // the infamous union command
            str = Regex.Replace(str, "union +select",      "union-select",     RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "union +all +select", "union-all-select", RegexOptions.IgnoreCase);

            return str;
        }


        #endregion String Utilities


        // ----------------------------------------------------------------------------------------
        /// <!-- Find -->
        /// <summary>
        ///      Finds a pattern in a dropdown list
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int Find(ComboBox drop, string regexPattern)
        {
            regexPattern = Regex.Replace(regexPattern, @"\$", @"");
            int count = drop.Items.Count;
            int idx = -1;
            
            
            try
            {
                for (int i = 0; i < count && idx < 0; ++i)
                {
                    ListItem item = (ListItem)drop.Items[i];
                    if (Regex.IsMatch(item.Display, regexPattern, RegexOptions.IgnoreCase))
                        idx = i;
                }
            }
            catch { }
            
            
            return idx;
        }
        
        
        // ----------------------------------------------------------------------------------------
        /// <!-- FilterIn -->
        /// <summary>
        ///      Filters a dropdown list to only the items that match the pattern
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int FilterIn(ComboBox drop, string regexPattern)
        {
            regexPattern = Regex.Replace(regexPattern, @"\$", @"");
            int count = drop.Items.Count;
            int idx = -1;
            
            
            BindingList<ListItem> tempDiagnosisList = new BindingList<ListItem>();
            
            
            try
            {
                for (int i = 0; i < count && idx < 0; ++i)
                {
                    ListItem item = (ListItem)drop.Items[i];
                    if (Regex.IsMatch(item.Display, regexPattern, RegexOptions.IgnoreCase))
                        tempDiagnosisList.Add(item);
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            
            
            drop.DataSource = tempDiagnosisList;
            
            
            return idx;
        }
        
    }
}
