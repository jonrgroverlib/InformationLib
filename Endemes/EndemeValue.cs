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
using InformationLib.Testing;         // for 
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeValue -->
    /// <summary>
    ///      The EndemeValue class contains a value to be referenced through semantic data endemes
    /// </summary>
    /// <remarks>
    /// 
    ///      Related classes
    ///      ---------------                                                                                (1-1,1)             ((1-1,1)*n)
    ///                                                                                 +----------------+- EndemeItem ------+- EndemeList -------+
    ///                                                                                /                /                    |                    |
    ///                                                                               /                /                     |                    |
    ///                                                                              /  IEndemeItem --+                      |                    |
    ///                                                                             /                 |   IEnumerable[Guid> -+                    |  (*)
    ///                                                              (1-1-0)       /    (0-0-1)       |                      |                    +- EndemeField 
    ///                                            +--------------+- Endeme ------+     EndemeValue --+                      |                    |
    ///                                           /              /                 \                   \                     |                    |
    ///  (0-0-0)          (0-0-0)                /  (1-0-0)     /  (n-0-0)          \   (n-n-0)         \   (n-n-1)          |  ((n-n-1)*n)       | 
    ///  EndemeMeaning -- EndemeCharacteristic -+-- EndemeSet -+-- EndemeDictionary -+- EndemeProfile ---+- EndemeObject ----+- EndemeDefinition -+ - - - - - - - EndemeAccess
    ///
    ///      Pre-alpha code
    /// </remarks>
    public class EndemeValue
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public object Value { get; set; }
        public string Label { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Casting (implicit) - mostly primitives but including endemes and excluding dates
        // ----------------------------------------------------------------------------------------
        public static implicit operator EndemeValue(bool      bit  ) { return new EndemeValue(bit  ); }
        public static implicit operator EndemeValue(byte      bits ) { return new EndemeValue(bits ); }
        public static implicit operator EndemeValue(Int16     small) { return new EndemeValue(small); }
        public static implicit operator EndemeValue(int       med  ) { return new EndemeValue(med  ); }
        public static implicit operator EndemeValue(long      large) { return new EndemeValue(large); }
        public static implicit operator EndemeValue(decimal   dec  ) { return new EndemeValue(dec  ); }
        public static implicit operator EndemeValue(float     real ) { return new EndemeValue(real ); }
        public static implicit operator EndemeValue(double    num  ) { return new EndemeValue(num  ); }
        public static implicit operator EndemeValue(char      cha  ) { return new EndemeValue(cha  ); }
        public static implicit operator EndemeValue(string    str  ) { return new EndemeValue(str  ); }
        public static implicit operator EndemeValue(Endeme    en   ) { return new EndemeValue(en   ); }
        public static implicit operator EndemeValue(EndemeSet enSet) { return new EndemeValue(enSet); }


        // ----------------------------------------------------------------------------------------
        //  Casting (explicit) - mostly primitives but including endemes and excluding dates
        // ----------------------------------------------------------------------------------------
        public bool      BoolValue { get {              if (Value.GetType() == typeof(bool))     return (bool)Value;      else return false          ; } }
        public byte      ByteValue { get { byte    byt; if (byte   .TryParse(StrValue, out byt)) return byt;              else return 0              ; } }
        public Int16     SmlValue  { get { Int16   sml; if (Int16  .TryParse(StrValue, out sml)) return sml;              else return 0              ; } }
        public int       IntValue  { get { int     med; if (int    .TryParse(StrValue, out med)) return med;              else return 0              ; } }
        public long      BigValue  { get { long    big; if (long   .TryParse(StrValue, out big)) return big;              else return 0              ; } }
        public decimal   DecValue  { get { decimal dec; if (decimal.TryParse(StrValue, out dec)) return dec;              else return 0.0M           ; } }
        public float     FltValue  { get { float   num; if (float  .TryParse(StrValue, out num)) return num;              else return 0.0F           ; } }
        public double    DblValue  { get { double  num; if (double .TryParse(StrValue, out num)) return num;              else return 0.0            ; } }
        public char      CharValue { get {              if (Value is char)                       return (char)Value;      else return '\0'           ; } }
        public string    StrValue  { get {              if (Value != null)                       return Value.ToString(); else return ""             ; } }
        public EndemeSet SetValue  { get { Fix();       if (Value is EndemeSet)                  return (EndemeSet)Value; else return EndemeSet.Empty; } }
        public Endeme    EnValue   { get
        {
            if (Value != null && Value.GetType() == typeof(EndemeValue)) { Value = ((EndemeValue)Value).Value; Pause(); } // this should not be needed
            if (Value != null && Value.GetType() == typeof(Endeme)) return (Endeme)Value; else return Endeme.Empty;
        } }


        // ----------------------------------------------------------------------------------------
        //  Operators
        // ----------------------------------------------------------------------------------------
        public static bool operator == (EndemeValue lhs, EndemeValue rhs)
        {
            return ( object.ReferenceEquals(null, lhs) &&  object.ReferenceEquals(null, rhs) ||
                    !object.ReferenceEquals(null, lhs) && !object.ReferenceEquals(null, rhs) && Is.Ok(lhs.Value, Is.the_same_sets_as, rhs.Value));
        }
        public static bool operator != (EndemeValue lhs, EndemeValue rhs) { return !(lhs == rhs); }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeValue(object obj)
        {
            Value = obj;
            if (Value != null && Value.GetType() == typeof(EndemeValue)) Value = ((EndemeValue)Value).Value;
            if (Value != null && Value.GetType() == typeof(EndemeValue)) { Value = ((EndemeValue)Value).Value; Pause(); }
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public          EndemeValue Copy()             { EndemeValue value = new EndemeValue(Value); value.Label = Label; return value; }
        public static   EndemeValue Empty        { get { return new EndemeValue(null);                                                  } }
        public override bool        Equals(object obj) { if (obj == null || obj.GetType() != typeof(EndemeValue)) return false; return Is.Ok(this, Is.the_same_sets_as, (EndemeValue)obj); }
        public override int         GetHashCode()      { return this.ToString().GetHashCode();                                          }
        public          bool        IsString     { get { return (this.Type == typeof(string));                                          } }
        public override string      ToString()         { if (Value == null) return "[null]"; else return Value.ToString();              }
        public          Type        Type         { get { return Value.GetType();                                                        } }


        private void Fix()
        {
            if (Value != null && Value.GetType() == typeof(EndemeValue)) { Value = ((EndemeValue)Value).Value; Pause(); } // this should not be needed
        }


        private void Pause()
        {
        }
    }
}
