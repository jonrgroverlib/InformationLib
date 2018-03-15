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
using System.Collections.Generic;     // for 
using System.ComponentModel.DataAnnotations; // for [Key]
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeCharacteristic -->
    /// <summary>
    ///      Endemes and EndemeSets are made up of Characteristics that parallel the EndemeCharacteristic table
    /// </summary>
    /// <remarks>production ready</remarks>
    public class EndemeCharacteristic
    {
        // ----------------------------------------------------------------------------------------
        //  Table Members
        // ----------------------------------------------------------------------------------------
      //[Key] public char      EndemeCharLetter { get; set; }  // primary key
      //[Key] public Guid      EndemeSetId      { get; set; }  // primary key
              public char      EndemeCharLetter { get; set; }  // primary key
              public Guid      EndemeSetId      { get; set; }  // primary key
              public string    EndemeCharCode   { get; set; }
              public string    EndemeCharLabel  { get; set; }
              public string    EndemeCharDescr  { get; set; }
              public Guid      EndemeCharIsASet { get; set; }
              public EndemeSet EndemeSet        { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Class Members
        // ----------------------------------------------------------------------------------------
        public char   Letter { get { return EndemeCharLetter; } private set { EndemeCharLetter = value; } }
        public string Code   { get { return EndemeCharCode  ; }         set { EndemeCharCode   = value; } }
        public string Label  { get { return EndemeCharLabel ; }         set { EndemeCharLabel  = value; } }
        public string Descr  { get { return EndemeCharDescr ; }         set { EndemeCharDescr  = value; } }


        /// <summary>Stores what each value of a characteristic means</summary>
        public List<EndemeMeaning> EndemeMeaningList
        {
            get { if (_positionMeaning == null) _positionMeaning = new List<EndemeMeaning>(); return _positionMeaning; }
            set { _positionMeaning = value; }
        }
        private List<EndemeMeaning> _positionMeaning;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeCharacteristic()
        {
	        Code = ""; Label = ""; Descr = "";
	        // for serialization only
        }
        public EndemeCharacteristic(char cha, string label, string descr)
        {
            Set(cha, label, descr);
        }
        public EndemeCharacteristic(char cha, string code, string label, string descr)
        {
            Set(cha, label, descr);
            if (code == null) code = "";
            if (code.Length > 8) throw new ArgumentOutOfRangeException("Endeme Characteristic code should not exceed 8 characters.");
            EndemeCharCode = code;
        }


        // ----------------------------------------------------------------------------------------
        //  Operators
        // ----------------------------------------------------------------------------------------
        /// <summary>Two characteristics are unequal if their letter or label are different</summary>
        public static bool operator !=(EndemeCharacteristic a, EndemeCharacteristic b)
        {
            return !(a == b);
        }
        /// <summary>Two characteristics are equal if their letter and label are the same</summary>
        public static bool operator ==(EndemeCharacteristic a, EndemeCharacteristic b)
        {
            if ((object)a == null && (object)b == null) return true;
            if ((object)a == null) return false;
            return a.Equals(b);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Display -->
        /// <summary>
        /// 
        /// </summary>
        public string Display { get
        {
            if (string.IsNullOrEmpty(EndemeCharLabel) ||
                Regex.IsMatch(EndemeCharLabel, "^[ \t\r\n]*$") ||
                EndemeCharLabel.ToCharArray()[0] == EndemeCharLetter)
            {
                return Regex.Replace(EndemeCharLabel, "^(.)", "$1)");
            }
            else return "" + EndemeCharLetter + ")" + EndemeCharLabel;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Two characteristics are equal if their letter and label are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public override bool Equals(object obj)
        {
            if (obj == null                                  ) return false;
            if (System.Object.ReferenceEquals(this, obj)     ) return true;
            if (obj.GetType() != typeof(EndemeCharacteristic)) return false;
            EndemeCharacteristic target = (EndemeCharacteristic)obj;
            bool same = this.Letter == target.Letter && this.Label == target.Label;
            return same;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetHashCode -->
        /// <summary>
        ///      Two characteristics are equal if their letter and label are the same
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public override int GetHashCode()
        {
            int hash = (EndemeCharLabel+EndemeCharLetter).GetHashCode();
            return hash;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LetterToUpper -->
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>production ready</remarks>
        internal void LetterToUpper()
        {
            if ('a' <= Letter && Letter <= 'z')
            {
                EndemeCharLetter = (char)((int)EndemeCharLetter - 32);  // convert to upper case
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Set -->
        /// <summary>
        ///      Sets the value of a characteristic
        /// </summary>
        /// <param name="cha"></param>
        /// <param name="label"></param>
        /// <param name="descr"></param>
        /// <param name="units"></param>
        /// <param name="value"></param>
        /// <remarks>production ready</remarks>
        internal void Set(char cha, string label, string descr)
        {
            EndemeCharLetter = cha;
            EndemeCharLabel  = label;
            EndemeCharDescr  = descr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public override string ToString()
        {
            return Letter.ToString() + ". " + Label + " - " + Descr;
        }

    }
}
