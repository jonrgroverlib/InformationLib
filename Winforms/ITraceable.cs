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
using System.Windows.Forms;           // for many
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ITraceable -->
    /// <summary>
    ///      The ITraceable interface makes forms actions traceable
    /// </summary>
    /// <remarks>old beta code</remarks>
    public interface ITraceable
    {
        bool IsShown { get; set; }
        void Trace(Form form);
        void Trace(Form form,                                                ButtonBase       btn );
        void Trace(Form form,                                                ToolStripItem    btn );
        void Trace(Form form,              string msgDataDescr, string  lbl, ToolStripTextBox txt );
        void Trace(Form form, Control pnl,                                   string           msg );
        void Trace(Form form, Control pnl,                                   CheckBox         chk );
        void Trace(Form form, Control pnl, string msgDataDescr,              RadioButton      rad );
        void Trace(Form form, Control pnl, string msgDataDescr, Control lbl, CheckedListBox   list);
        void Trace(Form form, Control pnl, string msgDataDescr, Control lbl, ComboBox         drop);
        void Trace(Form form, Control pnl, string msgDataDescr, Control lbl, DateTimePicker   pick);
        void Trace(Form form, Control pnl, string msgDataDescr, Control lbl, TextBox          txt );
        void Trace(Form form, Control pnl, string msgDataDescr, string  lbl, DateTimePicker   pick);
        void Trace(Form form, Control pnl, string msgDataDescr, string  lbl, ComboBox         drop);
        void Trace(Form form, Control pnl, string msgDataDescr, string  lbl, RadioButton      rad );
        void Trace(Form form, ToolStripMenuItem menu,                        ToolStripItem    item);
        void Trace(Form form, ToolStripMenuItem menu, ToolStripItem submenu, ToolStripItem    item);
        void TraceClosing();
        void TraceClosing(Form form, ButtonBase btn);
        void TraceClosing(Form form, ToolStripButton btn);
    }
}
