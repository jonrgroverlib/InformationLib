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
using InformationLib.Testing ;        // for Here
using System;                         // for DateTime
using System.ComponentModel;          // for BindingList
using System.Text.RegularExpressions; // for Regex
using System.Windows.Forms;           // for Form

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Plaster -->
    /// <summary>
    ///      The Plaster class is the opposite of Scrape, it plasters stuff back on to a form
    /// </summary>
    /// <remarks>
    ///      This is primarily a static utility class,
    ///      but since every static method here has a 'form' parameter,
    ///      it makes sense to have instantiable method versions too
    /// 
    ///      Plaster uses Scrape although Scrape does not use Plaster
    ///      
    /// 
    ///      old beta code
    /// </remarks>
    public class Plaster
    {
        // ----------------------------------------------------------------------------------------
        //  Instance members
        // ----------------------------------------------------------------------------------------
        public Form _Form { set { _form = value; } } private Form _form;


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public Plaster(Form form) { _form = form; }


        public static void AutoComplete(ComboBox cb, KeyPressEventArgs a, bool blnLimitToList)
        { Scrape.AutoComplete(cb, a, blnLimitToList); }


        // ----------------------------------------------------------------------------------------
        /// <!-- BindTo -->
        /// <summary>
        ///      Displays a lookup binding list in a combo box
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="blist"></param>
        public static void BindTo(Form form, ComboBox drop
            , BindingList<ListItem> blist, string valueMember, string displayMember)
        {
            drop.DisplayMember    = displayMember;
            drop.ValueMember      = valueMember;
            drop.DataSource       = blist;
            drop.MaxDropDownItems = 22;
        }
        public void BindTo(ComboBox drop, BindingList<ListItem> blist, string valueMember, string displayMember)
        { BindTo(_form, drop, blist, valueMember, displayMember); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Complain -->
        /// <summary>
        ///      Complains about the date being too far in the future
        /// </summary>
        /// <param name="label">field name</param>
        /// <param name="limit"></param>
        private static void Complain(string label, TimeSpan limit)
        {
            string msg = "";
            if      (limit.TotalMilliseconds < 3000) msg = ""+(int) limit.TotalMilliseconds+" milliseconds";
            else if (limit.TotalSeconds      < 180 ) msg = ""+(int) limit.TotalSeconds     +" seconds"     ;
            else if (limit.TotalMinutes      < 180 ) msg = ""+(int) limit.TotalMinutes     +" minutes"     ;
            else if (limit.TotalHours        < 72  ) msg = ""+(int) limit.TotalHours       +" hours"       ;
            else if (limit.TotalDays         < 35  ) msg = ""+(int) limit.TotalDays        +" days"        ;
            else if (limit.TotalDays         < 320 ) msg = ""+(int)(limit.TotalDays/7)     +" weeks"       ;
            else if (limit.TotalDays         < 1094) msg = ""+(int)(limit.TotalDays/30.436)+" months"      ;
            else                                     msg = ""+(int)(limit.TotalDays/365.23)+" years"       ;
            MessageBox.Show(label + " may not be set more than " + msg + " into the future.");
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- DataSource -->
        /// <summary>
        ///      Sets the datasource for a control
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="ds"></param>
        public static void DataSource(Form form, Control control, object dataSource)
        {
            if (Scrape.Active(form, control))
            {
                Type type = control.GetType();
                switch (type.Name.ToString())
                {
                    case "ComboBox": ((ComboBox)control).DataSource = dataSource; break;
                    default: break;
                }
            }
        }
        // instantiable version:
        public void DataSource(Control control, object dataSource) { DataSource(_form, control, dataSource); }
        //public static void DataSource(Form form, string controlName, object dataSource) { DataSource(form, Scrape.Control(form, controlName), dataSource); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Date -->
        /// <summary>
        ///      Sets the date time picker date, defaulting to Now
        /// </summary>
        /// <param name="pick"></param>
        /// <param name="date"></param>
        public static void Dates(Form form, DateTimePicker pick, TimeDate_old date, TimeDate_old defaultDate, Label nullImage)
        {
            if (Scrape.Active(form, pick))
            {
                pick.Visible = true;
                nullImage.Visible = false;
                if (date.IsNull || date.CLRFormat < pick.MinDate || date.CLRFormat > pick.MaxDate)
                {
                    if (defaultDate.IsNull)
                        nullImage.Visible = true; // this is broken
                    else
                        pick.Value = defaultDate.CLRFormat;
                }
                else
                    pick.Value = Scrape.ValidPickerDate(date.CLRFormat);
            }
        }
        public void Dates(DateTimePicker pick, TimeDate_old date, TimeDate_old defaultDate, Label nullImage) { Dates(_form, pick, date, defaultDate, nullImage); }
        public static void Dates(Form form, DateTimePicker pick, DateTime date, Label nullImage) { Dates(form, pick, new TimeDate_old(date), TimeDate_old.Now, nullImage); }
        public static void Dates(Form form, TextBox txtDateTime, TimeDate_old date, string format) { if (Scrape.Active(form, txtDateTime)) txtDateTime.Text = date.ToString(format); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Dates -->
        /// <summary>
        ///      Sets the date field of a control
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="date"></param>
        public static void Dates(Form form, Control control, TimeDate_old date)
        {
            if (Scrape.Active(form, control))
            {
                DateTime clrDate = TimeDate_old.ClrDate(date);
                Type type = control.GetType();
                switch (type.Name.ToString())
                {
                    case "DateTimePicker": Dates(form, ((DateTimePicker)control), clrDate, new Label()); break;
                    default: control.Text = date.ToString(); break;
                }
            }
        }
        // instantiable version:
        public void Dates(Control control, TimeDate_old date) { Dates(_form, control, date); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Dates -->
        /// <summary>
        ///      Sets the date field of a control
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="date"></param>
        public static void Dates(Form form, Control control, TimeDate_old date, TimeDate_old defaultDate, Label nullImage)
        {
            if (Scrape.Active(form, control))
            {
                Type type = control.GetType();
                switch (type.Name.ToString())
                {
                    case "DateTimePicker": Dates(form, ((DateTimePicker)control), date, defaultDate, nullImage); break;
                    default: control.Text = date.ToString(); break;
                }
            }
        }
        // instantiable version:
        public void Dates(Control control, TimeDate_old date, TimeDate_old defaultDate, Label nullImage) { Dates(_form, control, date, defaultDate, nullImage); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Dropdown -->
        /// <summary>
        ///      Fills a combo box with a binding list
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="blist"></param>
        public static void Dropdown(Form form, Control control
            , BindingList<ListItem> blist, string valueMember, string displayMember)
        {
            if (Scrape.Active(form, control))
            {
                ComboBox drop = (ComboBox)control;
                BindTo(form, drop, blist, valueMember, displayMember);
            }
        }
        // instantiable version:
        public void Dropdown(Control control
            , BindingList<ListItem> blist, string valueMember, string displayMember)
        { Dropdown(_form, control, blist, valueMember, displayMember); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Enabled -->
        /// <summary>
        ///      Sets the Enabled field of a control
        /// </summary>
        /// <param name="form"></param>
        /// <param name="control"></param>
        /// <param name="enabled"></param>
        public static void Enabled(Form form, Control control, bool enabled)
        {
            if (Scrape.Active(form, control)) control.Enabled = enabled;
        }
        /// <summary>Sets the Enabled field of a control - instantiable version</summary>
        public void Enabled(Control control, bool enabled) { Enabled(_form, control, enabled); }


        // ----------------------------------------------------------------------------------------
        /// <!-- FromTo -->
        /// <summary>
        ///      Adds the selected item from the source list if it is not already in the destination list
        /// </summary>
        /// <param name="form">allows no-action if the controls are not on the form</param>
        /// <param name="drop"></param>
        /// <param name="list"></param>
        public static void FromTo(Form form, ComboBox dropFrom, CheckedListBox listTo)
        {
            if (Scrape.Active(form, dropFrom) && Scrape.Active(form, listTo))
            {
                // ---------------------------------------------------------------------90
                //  Find the item in the 'From' list
                // ---------------------------------------------------------------------90
                int idxFrom = dropFrom.SelectedIndex;
                if (idxFrom >= 0)
                {
                    ListItem itemFrom = (ListItem)dropFrom.Items[idxFrom];


                    // -----------------------------------------------------------------90
                    //  Add the item to the 'To' list
                    // -----------------------------------------------------------------90
                    int idxTo = listTo.FindString(itemFrom.Display);
                    if (idxTo < 0)
                        idxTo = listTo.FindString(itemFrom.Descr);
                    if (idxTo < 0)
                    {
                        string codeFrom = itemFrom.Code;
                        for (int i = 0; i < listTo.Items.Count; ++i)
                        {
                            ListItem item = (ListItem)listTo.Items[i];
                            if (codeFrom == item.Code)
                                idxTo = i;
                        }
                    }
                    if (idxTo < 0)
                    {
                        string idFrom  = dropFrom.SelectedValue.ToString();
                        string code    = itemFrom.Code;
                        string descr   = itemFrom.Descr;
                        string display = itemFrom.Display;
                        string idTo    = (listTo.Items.Count + 1).ToString();
                        ListItem itemTo = new ListItem(idTo, code, descr, display);
                        listTo.Items.Add(itemTo, true);
                    }
                }
            }
        }
        // instantiable version:
        public void FromTo(ComboBox drop, CheckedListBox list) { FromTo(_form, drop, list); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Radio -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form">allows no-action if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Radio(Form form, Control control, bool value)
        {
            if (Scrape.Active(form, control))
            {
                if (control.GetType() == typeof(RadioButton))
                {
                    RadioButton r = ((RadioButton)control);
                    r.Checked = value;
                }
            }
        }
        // instantiable version:
        public void Radio(Control control, bool value) { Radio(_form, control, value); }


        // ----------------------------------------------------------------------------------------
        /// <!-- SetByIndex -->
        /// <summary>
        ///      Set what you can based on index
        /// </summary>
        /// <remarks>
        ///      SelectedValue can not be set until after the ComboBox is displayed to the user
        /// </remarks>
        /// <param name="drop"></param>
        /// <param name="idx"></param>
        private static void SetByIndex(Form form, ComboBox drop, int idx)
        {
            if (Scrape.Active(form, drop) && idx >= 0)
            {
                drop.SelectedIndex = idx;
                try { drop.SelectedValue = ((ListItem)drop.Items[idx]).ID; } catch { }
                drop.Text = drop.Items[idx].ToString();
            }
        }
        // instantiable version:
        private void SetByIndex(ComboBox drop, int idx) { SetByIndex(_form, drop, idx); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Text -->
        /// <summary>
        ///      Puts text values into controls on a form given the control
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="text"></param>
        public static void Text(Form form, Control control, string text)
        {
            if (Scrape.Active(form, control))
            {
                Type type = control.GetType();
                switch (type.Name.ToString())
                {
                    case "ComboBox": ComboBox drop = (ComboBox)control; Value(form, ref drop, text); break;
                    case "GroupBox": control.Text = text; break; // radio buttons
                    case "TextBox" : control.Text = text; break;
                    default:         control.Text = text; break;
                }
            }
        }
        // instantiable version:
        public void Text(Control control, string text) { Text(_form, control, text); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Text -->
        /// <summary>
        ///      Sets the text field of a control
        /// </summary>
        public static void Text(Form form, ComboBox drop, string value)
        {
            if (Scrape.Active(form, drop))
            {
                string text = "";
                try { drop.SelectedValue = value; text = drop.Text; }
                catch { text = ""; }
                if (string.IsNullOrEmpty(text)) drop.Text = value;
            }
        }
        // instantiable version:
        public void Text(ComboBox drop, string value) { Text(_form, drop, value); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Toggle -->
        /// <summary>
        ///      Toggles the particular checked item specified
        /// </summary>
        /// <param name="clb"></param>
        /// <param name="idx"></param>
        public static void Toggle(Form form, CheckedListBox clb, int idx)
        {
            if (Scrape.Active(form, clb))
            {
                //if (_.InRange(-1, idx, clb.Items.Count))
                if (-1 < idx && idx <= clb.Items.Count)
                {
                    if (clb.GetItemCheckState(idx) == CheckState.Checked)
                        clb.SetItemCheckState(idx, CheckState.Unchecked);
                    else
                        clb.SetItemCheckState(idx, CheckState.Checked);
                }
                else
                    Pause();
            }
        }
        // instantiable version:
        public void Toggle(CheckedListBox clb, int idx) { Toggle(_form, clb, idx); }


        private static void Pause()
        {
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Valid -->
        /// <summary>
        ///      'Insures' that a date is not set too far in the future
        /// </summary>
        /// <param name="label">date field name</param>
        /// <param name="limit"></param>
        /// <param name="pickDate"></param>
        /// <returns></returns>
        private static DateTime _lastDateValue; // to avoid infinite loops
        public static bool Valid(string label, TimeSpan limit, DateTimePicker pickDate)
        {
            bool valid = true;
            if (pickDate != null)
            {
                if (_lastDateValue == null || pickDate.Value.Ticks != _lastDateValue.Ticks)
                {
                    _lastDateValue = pickDate.Value;
                    DateTime threshold = DateTime.Now + limit;
                    if (pickDate.Value.Date > threshold.Date)
                    {
                        Complain(label, limit);
                        pickDate.Value = DateTime.Now;
                        pickDate.Focus();
                        valid = false;
                    }
                }
                _lastDateValue = pickDate.Value;
            }
            return valid;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Valid -->
        /// <summary>
        ///      'Insures' that a time is not set too far in the future
        /// </summary>
        /// <param name="label">name of time field</param>
        /// <param name="limit"></param>
        /// <param name="pickDate"></param>
        /// <param name="pickTime"></param>
        /// <returns></returns>
        private static DateTime _lastTimeValue; // to avoid infinite loops
        public static bool Valid(string label, TimeSpan limit, DateTimePicker pickDate, DateTimePicker pickTime)
        {
            bool valid = true;
            if (pickDate != null && pickTime != null)
            {
                if (_lastTimeValue == null || pickTime.Value.Ticks != _lastTimeValue.Ticks)
                {
                    DateTime threshold = DateTime.Now + limit;
                    if (pickDate != null
                        && pickTime != null
                        && pickDate.Value.Date == threshold.Date
                        && pickTime.Value.TimeOfDay > threshold.TimeOfDay)
                    {
                        _lastTimeValue = pickTime.Value;
                        Complain(label, limit);
                        pickTime.Value = DateTime.Now;
                        pickTime.Focus();
                        valid = false;
                    }
                    _lastTimeValue = pickTime.Value;
                }
            }
            return valid;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Value -->
        /// <summary>
        ///      Given a form and a named control try to set the control's value
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void Value(Form form, Control control, string value)
        {
            if (Scrape.Active(form, control))
            {
                ComboBox drop = (ComboBox)control;
                Value(form, ref drop, value);
            }
        }
        // instantiable version:
        public void Value(Control control, string value) { Value(_form, control, value); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Value -->
        /// <summary>
        ///      Given a combo box and a value select the item in the drop list that matches if any
        /// </summary>
        /// <remarks>
        ///      This gets complicated because SelectedValue can not be set or retrieved until after
        ///      the ComboBox is displayed to the user
        /// </remarks>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="drop"></param>
        /// <param name="value"></param>
        public static void Value(Form form, ref ComboBox drop, object value)
        {
            if (Scrape.Active(form, drop))
            {
                if (Is.Null(value))
                    drop.Text = "";
                else
                {
                    string str = TreatAs.StrValue(value, "");


                    int oldIndex = drop.SelectedIndex;
                    drop.SelectedIndex = -1;
                    string oldText = drop.Text;
                    drop.Text = "";


                    // ---------------------------------------------------------------------90
                    //  Try to select by value (this should work under optimal conditions)
                    // ---------------------------------------------------------------------90
                    try { drop.SelectedValue = value; }
                    catch { }
                    if (drop.SelectedValue == null || drop.SelectedValue.ToString() == "")
                        try { drop.SelectedValue = (object)str; }
                        catch { }
                    if (drop.SelectedValue == null || drop.SelectedValue.ToString() == "")
                        try { drop.SelectedValue = TreatAs.IntValue(value, -1); }
                        catch { }


                    // ---------------------------------------------------------------------90
                    //  Try to select item by value, by item and by a sequential search
                    // ---------------------------------------------------------------------90
                    if (drop.SelectedIndex < 0) SetByIndex(form, drop, drop.Items.IndexOf(value));
                    if (drop.SelectedIndex < 0) try { drop.SelectedItem = value; }
                        catch { }
                    if (drop.SelectedIndex < 0) try { drop.SelectedItem = (object)str; }
                        catch { }
                    if (drop.SelectedIndex < 0) SetByIndex(form, drop, Scrape.FindIndexOf(form, drop, value));


                    // ---------------------------------------------------------------------90
                    //  Give up and just set the text to the value
                    // ---------------------------------------------------------------------90
                    if (drop.SelectedIndex < 0) SetByIndex(form, drop, drop.FindStringExact(str));
                    if (drop.SelectedIndex < 0 && str.Length > 6) SetByIndex(form, drop, drop.FindString(str));
                    if (drop.SelectedIndex < 0)
                    {
                        if (drop.SelectedText == null || drop.SelectedText == "")
                            try { drop.SelectedText = str; }
                            catch { }
                        if (drop.Text == null || drop.Text == "")
                            drop.Text = str;
                    }

                }
            }
        }
        // instantiable version:
        public void Value(ref ComboBox drop, object value) { Value(_form, ref drop, value); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Visible -->
        /// <summary>
        ///      Sets the visible field of the control
        /// </summary>
        /// <param name="form">allows no-op if the control is not on the form</param>
        /// <param name="control"></param>
        /// <param name="visible"></param>
        public static void Visible(Form form, Control control, bool visible)
        {
            if (Scrape.Active(form, control)) control.Visible = visible;
        }
        // instantiable version:
        public void Visible(Control control, bool visible) { Visible(_form, control, visible); }
        //public static void Visible(Form form, string controlName, bool visible) { Visible(form, Scrape.Control(form, controlName), visible); }


    }
}
