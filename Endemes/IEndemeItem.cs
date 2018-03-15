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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices; // for IndexerName
using System.Text;

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- IEndemeItem -->
    /// <summary>
    ///      The IEndemeItem is an interface for the EndemeItem, EndemeObject and EndemeNode classes,
    ///      it is used as an interface for looking at the various results from lookups in various
    ///      data structures containing these elements
    /// </summary>
    /// <remarks>
    /// 
    ///      Endeme object/class dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = 0, 1, n(infinity)
    ///      ------------------------------------
    ///
    ///           [---------------- ACTION --------------]    [-------------------------- STATE --------------------------]    [------------------------------- DATABASE ---------------------------------]           
    ///       _   |                                      |    |                                                           |    |                                                                          |   _     _ 
    ///        |             EndemeActuator.cs                                                                                                                             EndemeDataAccess                  |       |
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
    /// </remarks>
    public interface IEndemeItem
    {
        //  Member properties
        Guid        ItemKey   { get; set; }
        string      ItemLabel { get; set; }
        double      TempMatch { get; set; }
        EndemeValue Item      { get; set; }
        [IndexerName("IloveJesus")]
        EndemeValue this[int index] { get; set; }

        //  Short methods
        //void        AddValue   (       object obj);
        //void        Clear      (                 );
        //void        Insert     (int i, object obj);
        //void        Remove     (       object obj);
        //void        RemoveAt   (int i            );
        //void        Reverse    (                 );

        //EndemeValue[] ToArray  ();

        //  Short properties
         int        ValueCount { get; }

        //  Other methods
        string      HeaderString();
        string      ToString();
    }
}
