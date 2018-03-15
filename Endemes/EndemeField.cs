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
using System.Data.SqlTypes;           // for IsNull
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex
using System.Windows.Forms;

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeField -->
    /// <summary>
    ///      The EndemeField class implements a sparse '2D' associative array of endemes
    /// </summary>
    /// <remarks>
    ///      This is an attempt at data semantics or something
    ///      
    ///      Endeme object/class dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = 0, 1, n(infinity)
    ///      ------------------------------------
    ///
    ///           [---------------- ACTION --------------]    [-------------------------- STATE --------------------------]    [------------------------------- DATABASE ---------------------------------]           
    ///       _   |                                      |    |                                                           |    |                                                                          |   _     _ 
    ///        |             EndemeActuator.cs                                                                                                                             EndemeAccess                  |       |
    ///  Info  |  |                                      |    |                                                           |    |                                                 |                        |  | Info  |
    ///        |                                                                                                                                                                                             |       |
    ///  Level |  |                                      |    |                         (objects)                         |    |                                                 |                        |  | Level |
    ///        |                                                                            +  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -+--+--+--+--+--+--+--------+--------+                  |       |
    ///       _|  |                                      |    |                             |                             |    |                      |  |  |  |  |  |  |                 |               |  |_     _|
    ///                                                                                     |                                                         |  |  |  |  |  |  |                 |                           
    ///       _   |                                      |    |                             |                             |    |                      |  |  |  |  |  |  |                 |               |   _     _ 
    ///        |                                                                       EndemeField (*-*-1)*n                                          |  |  |  |  |  |  |                 |                  |       |
    ///        |  |                                      |    |                             |                             |    |                      |  |  |  |  |  |  |                 |               |  |       |
    ///        |                                                     +----------------------+                                                         |  |  |  |  |  |  |                 |                  |       |
    ///        |  |                                      |    |      |                      |                             |    |                      |  |  |  |  |  |  |                 |               |  |       |
    ///        |                                                EndemeList (1-1,1)*n   EndemeDefinition (n-n-1)*n                                     |  |  |  |  |  |  |                 |                  |       |
    ///        |  |                                      |    |      |                      |                             |    |                      |  |  |  |  |  |  |                 |               |  |       |
    ///        |                           +------------------------------------------------+-------------------------------------------+-------------+  |  |  |  |  |  |                 |                  |       |
    ///  High  |  |                        |             |    |     /|\                    /|\                            |    |       /                 |  |  |  |  |  |                 |               |  | High  |
    ///        |               EndemeActuator                   EndemeItem (1-1,1)     EndemeObject (n-n-1)                      (endeme actuator table) |  |  |  |  |  |                 |                  |       |
    ///  Level |  |                /|\                   |    |      |                      |                             |    |                         |  |  |  |  |  |                 |               |  | Level |
    ///        |                ActuatorList                         +------+           +---+--+----------------------------------------+----------------+  |  |  |  |  |                 |                  |       |
    ///        |  |                /|\                   |    |      |       \         /       |                          |    |       /                    |  |  |  |  |                 |               |  |       |
    ///        |          +-------+-+-----+-------+                  |        +-------+   EndemeProfile (n-n-0)                  (endeme profile table)     |  |  |  |  |                 |                  |       |
    ///        |  |       |       |       |       |      |    |      |        |       |        |                          |    |                            |  |  |  |  |                 |               |  |       |
    ///        |     CharFactor   |  WeightFactor |                  |        |  IEndemeItem   +-------+                                                    |  |  |  |  |                 |                  |       |
    ///        |  | 'C'   | 'O'   | 'W'   |   'S' |      |    |      |        |                |      \|/                 |    |                            |  |  |  |  |                 |               |  |       |
    ///       _|          |  OrderFactor  |    SetFactor             |  EndemeValue (0-0-1)    |  EndemeReference (n-0-0)                                   |  |  |  |  |                 |                  |_     _|
    ///           |       |       |       |       |  |   |    |      |                         |       |                  |    |                            |  |  |  |  |                 |               |           
    ///                   +-------+-+-----+-------+  |               |                         |       |                                +-------------------+  |  |  |  |                 |                           
    ///       _   |                 |                |   |    |      |                         |       |                  |    |       /                       |  |  |  |                 |               |   _     _ 
    ///        |              IEndemeActuatorFactor  |               |  EndemeTextFormat       |       |  EndemeGrabBag          (endeme index table)          |  |  |  |                 |                  |       |
    ///        |  |                 |                |   |    |      |       |                /|\      |       |          |    |                               |  |  |  |                 |               |  |       |
    ///        |                    +--------------------------------+-------+--------------+--+---------------+------------------------+----------------------+  |  |  |                 |                  |       |
    ///        |  |                                  |   |    |                             |          |                  |    |       /                          |  |  |                 |               |  |       |
    ///        |                                     |                                 Endeme (1-1-0)  |                         (endeme table)                   |  |  |      +----------+-----+            |       |
    ///        |  |                                  |   |    |                        |   \|/        /|\                 |    |                                  |  |  |      |                |         |  |       |
    ///        |                                     +--------------------------------------+--+-------+--------------------------------+-------------------------+  |  |  RichSqlCommand   RichDataTable    |       |
    ///  Low   |  |                                      |    |                        |       |                          |    |       /                             |  |      |                |         |  | Low   |
    ///        |                                                                       |  EndemeSet (1-0-0)                      (endeme set table)                  |  |      +-----+----------+            |       |
    ///  Level |  |                                      |    |                        |       |                          |    |                                     |  |      |     |          |         |  | Level |
    ///        |                                                        +--------------+       +----------------------------------------+----------------------------+  |      |  InData     Is.             |       |
    ///        |  |                                      |    |        /|\                    /|\                         |    |       /                                |      |  Throws     RandomSource |  |       |
    ///        |                                                   EndemeQuantification   EndemeCharacteristic (0-0-0)           (endeme characteristic table)          |      |  TreatAs.   FilesIO         |       |
    ///        |  |                                      |    |    (0-0-0)                     |                          |    |                                        |      |             __.          |  |       |
    ///        |                                                                               +----------------------------------------+-------------------------------+      |                             |       |
    ///        |  |                                      |    |                               /|\                         |    |       /                                   ConnectSource                  |  |       |
    ///       _|                                                                          EndemeMeaning (0-0-0)                  (endeme meaning table)                                                      |_     _|
    ///           |                                      |    |                                                           |    |                                                                          |           
    ///           [---------------- ACTION --------------]    [-------------------------- STATE --------------------------]    [------------------------------- DATABASE ---------------------------------]           
    ///      
    ///      alpha code - actively being developed
    /// </remarks>
    public class EndemeField
    {
        // ----------------------------------------------------------------------------------------
        //  Member parameters
        // ----------------------------------------------------------------------------------------
        public EndemeList     ListField { get { return _field   ; } } private EndemeList     _field;
        public EndemeDefinition RegField  { get { return _register; } } private EndemeDefinition _register;
        public string         Label     { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        public EndemeValue this[string profile] { get { return GetValue(profile); } set { SetValue(profile, value); } }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeField(string label, EndemeReference enRef, double equalThreshold)
        {
            Label     = label;
            _field    = new EndemeList    (label, enRef, equalThreshold);
            _register = new EndemeDefinition(label, enRef);
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public int Count { get { return RegField.Count + ListField.Count; } }


        public void Add(IEndemeItem item)
        {
            if (item == null) return;
            if (item.GetType() == typeof(EndemeItem))
                ListField.Add((EndemeItem)item);
            else
                RegField.Add((EndemeObject)item);
        }

        internal void Add(EndemeProfile profile, EndemeValue value)
        {
            RegField.Add(new EndemeObject(value.Label, profile, value));
        }

        public void Add(Endeme profile, EndemeValue value)
        {
            ListField.Add(new EndemeItem(value.Label, profile, value));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiValue -->
        /// <summary>
        ///      useful for debugging
        /// </summary>
        public string AsciiValue { get
        {
            StringBuilder str = new StringBuilder(ListField.Count + RegField.Count);
            string delim = "";


            // --------------------------------------------------------------------------
            //  Add all the endeme list fields
            // --------------------------------------------------------------------------
            for (int i = 0; i < ListField.Count; ++i)
            {
                EndemeItem item = null;
                item = ListField[i];
                if (item.Item != null && item.Item.GetType() == typeof(Endeme))
                    str.Append(delim + "[" + item.HeaderString() + ((Endeme)item.Item.Value).FullDesignation + "]");
                else str.Append(delim + item.ToString()); // + " = " + ValueToString(item.ItemValue);
                delim = "\r\n";
            }


            // --------------------------------------------------------------------------
            //  Add all the endeme register fields
            // --------------------------------------------------------------------------
            for (int i = 0; i < RegField.Count; ++i)
            {
                EndemeObject item = null;
                item = RegField[i];
                if (item.Item != null && item.Item.GetType() == typeof(Endeme))
                     str.Append(delim + "[" + item.HeaderString() + ((Endeme)item.Item.Value).FullDesignation + "]");
                else str.Append(delim + item.ToString()); // + " = " + ValueToString(item.ItemValue);
                delim = "\r\n";
            }

            return str.ToString();
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- For -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public IEndemeItem For(string profile)
        {
            if (Regex.IsMatch(profile, "[*!]")) return RegField .Educe(profile);
            else                                return ListField.Educe(profile);
        }

        public EndemeValue GetValue(string profile)
        {
            if (Regex.IsMatch(profile, "[*!]")) return RegField .Educe(profile).Item;
            else                                return ListField.Educe(profile).Item;
        }

        public void SetValue(string profile, EndemeValue value)
        {
            if (Regex.IsMatch(profile, "[*!]"))
            {
                EndemeObject enObj = RegField.Educe(profile);
                enObj.Item = value;
            }
            else ListField.Educe(profile).Item = value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetMatchList -->
        /// <summary>
        ///      Orders the list by the sort profile
        /// </summary>
        /// <param name="sortProfile"></param>
        /// <returns></returns>
        public List<IEndemeItem> GetMatchList(string sortProfile)
        {
            List<IEndemeItem> list = new List<IEndemeItem>();
            if (Regex.IsMatch(sortProfile, "[*!]"))
            {
                EndemeDefinition part = RegField.PartNotHaving(RegField.EnRef["DSVQAHMU"]);
                list = part.OrderBy(new EndemeProfile(sortProfile, RegField.EnRef)).ToList();
            }
            else
            {
                EndemeItem item = EndemeProfile.BuildSegment(sortProfile, ListField.EnRef);
                list = ListField.OrderBy(item.ItemEndeme).ToList();
            }
            return list;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- SetItemLabel -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="profile"></param>
        ///// <param name="label"></param>
        //public void SetItemLabel(string profile, string label)
        //{
        //    this.For(profile).ItemLabel = label;
        //    //if (Regex.IsMatch(profile, "[*!]")) RegField .Educe(profile).ItemLabel = label;
        //    //else                                ListField.Educe(profile).ItemLabel = label;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _field.Count.ToString() + " values in " + _field.Count + " sets out of " + _field.EnRef + " sets";
        }

        //public void Add(EndemeItem item)
        //{
        //    ListField.Add(item);
        //}
        //public void Add(EndemeObject item)
        //{
        //    RegField.Add(item);
        //}

        public EndemeList OrderBy(Endeme en)
        {
            return ListField.OrderBy(en);
        }
    }
}
