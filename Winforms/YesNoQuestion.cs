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
using System.Windows.Forms;           // for DialogResult(2)
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- YesNoQuestion -->
    /// <summary>
    ///      Puts up a dialog asking a Yes/No question
    /// </summary>
    /// <remarks>old beta code</remarks>
    public class YesNoQuestion
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Text, Title -->                                                                       /// <summary>Text of the question</summary>
        public string Text  { get { return _text;  } set { _text  = value; } }  private string _text;  /// <summary>Title on the dialog box</summary>
        public string Title { get { return _title; } set { _title = value; } }  private string _title;
        public DialogResult Answer { get { return _answer; } } private DialogResult _answer;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public YesNoQuestion()                          { _title = "";    _text = "";   _answer = DialogResult.None; }
        public YesNoQuestion(string text)               { _title = "";    _text = text; _answer = DialogResult.None; }
        public YesNoQuestion(string text, string title) { _title = title; _text = text; _answer = DialogResult.None; }


        // ----------------------------------------------------------------------------------------
        /// <!-- Yes, No -->
        /// <summary>Asks the user a yes/no question</summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public bool Yes(string question) { Text = question;  return Yes(); }
        public bool No (string question) { Text = question;  return !Yes(); }
        public bool Yes()
        {
            _answer = MessageBox.Show(Text, Title, MessageBoxButtons.YesNo);
            return (_answer == DialogResult.Yes);
        }
    }
}
