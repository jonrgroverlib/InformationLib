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
using System.Data.SqlClient;          // for 
using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeReference -->
    /// <summary>
    ///      The EndemeReference class manages a list of Endeme Sets
    /// </summary>
    /// <remarks>
    /// 
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
    public class EndemeReference
    {
        // ----------------------------------------------------------------------------------------
        //  Member
        // ----------------------------------------------------------------------------------------
        public Dictionary<string,EndemeSet> SetRef { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Accessor
        // ----------------------------------------------------------------------------------------
        public EndemeSet this[string setName] { get { return GetEnSet(setName.ToLower()); } set { SetEnSet(setName.ToLower(), value); } }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeReference() { SetRef = new Dictionary<string,EndemeSet>(); }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public EndemeReference Add     (EndemeSet enSet                    ) { if (!this.Contains(enSet.Label.ToLower())) SetRef.Add(enSet.Label.ToLower(), enSet); else Pause(); return this; }
        public bool            Contains(EndemeSet enSet                    ) { return SetRef.ContainsValue(enSet)                                                                            ; }
        public bool            Contains(string    setLabel                 ) { return SetRef.ContainsKey(setLabel)                                                                           ; }
        public int             Count                                   { get { return SetRef.Count;                                                                                            } }
        public EndemeSet       GetEnSet(string    setLabel                 ) { if ( this.Contains(setLabel)             ) return SetRef[setLabel];                           else return null; }
        public void            SetEnSet(string    setLabel, EndemeSet value) { if (!this.Contains(setLabel)             ) Add(value); SetRef[setLabel] = value;                                }


        // ----------------------------------------------------------------------------------------
        //  Methods
        // ----------------------------------------------------------------------------------------


        private void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (SetRef == null) return "null"; return SetRef.Count.ToString() + " sets";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Make sure that the endeme set is in the endeme refernce
        /// </summary>
        /// <param name="endeme"></param>
        internal EndemeSet Educe(Endeme endeme)
        {
            if (endeme != null && endeme.EnSet != null)
            {
                if (this.Contains(endeme.EnSet))
                    return endeme.EnSet;
                else
                {
                    this.Add(endeme.EnSet);
                    return endeme.EnSet;
                }
            }
            return null;
        }

    }
}

