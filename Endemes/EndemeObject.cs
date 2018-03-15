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
using System.Linq;                    // for 
using System.Runtime.CompilerServices; // for IndexerName
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeObject -->
    /// <summary>
    ///      The EndemeObject class contains an endeme object with its data semantic address
    /// </summary>
    /// <remarks>
    ///      It is like an EndemeItem on steroids
    ///
    ///
    ///      Endeme object/class dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = 0, 1, n(infinity)
    ///      ------------------------------------
    ///
    ///           [-------------------- ACTION --------------------]   [-------------------------- STATE -------------------------]   [----------------------------- DATABASE -----------------------------]           
    ///       _   |                                                |   |                                                          |   |                                                                    |   _     _ 
    ///        |                    EndemeActuator.cs                                                                                                                          EndemeDataAccess               |       |
    ///  Info  |  |                                                |   |                                                          |   |                                             |                      |  | Info  |
    ///        |                                                                                                                                                                                              |       |
    ///  Level |  |                                                |   |                         (objects)                        |   |       (tables)                              |                      |  | Level |
    ///        |                                                              +  -  -  -  -  -  -  - +  -  -  -  -  -  -  -  -  -    -  -  -  -  -  -  -  -  +--+--+---+--+--+--+--++------+                  |       |
    ///        |  |                                                |   |      |                      |                            |   |                      |  |  |   |  |  |  |  |       |               |  |       |
    ///        |                                                              |                 EndemeField (*,*,*)*n                                        |  |  |   |  |  |  |  |       |                  |       |
    ///        |  |                                                |   |      |                      |                            |   |                      |  |  |   |  |  |  |  |       |               |  |       |
    ///        |                                                              +----------------------+                                                       |  |  |   |  |  |  |  |       |                  |       |
    ///        |  | . . . . . . . . . . . . . . . . . . . . . . . .|.. |      |                      |                            |   |                      |  |  |   |  |  |  |  |       |               |  |       |
    ///       _|    .                           +------------+       .        |                 EndemeDefinition (n,n,n+m)*n                                 |  |  |   |  |  |  |  |       |                  |_     _|
    ///           | .                           |             \    | ..| . . .|. . . . . . . . . . . | . . . . . . . . . . . . . .|. .|. . . . . . . . . . . | .|. |.. |  |  |  |  |       |               |           
    ///       _     .                      EndemeActuator      +-------------------------------------+-----------------------------------------+-------------+  |  | . |  |  |  |  |       |                   _     _ 
    ///        |  | .                          /|\                 |   |      |                      |                            |   |       /                 |  | . |  |  |  |  |       |               |  |       |
    ///        |    .                       ActuatorList                      |                      |                                  (endeme actuator table) |  | . |  |  |  |  |       |                  |       |
    ///        |  | .                          /|\                 |   |      |                      |                            |   |  [stores ACTUATOR]      |  | . |  |  |  |  |       |               |  |       |
    ///        |    .     +--------+------------+----+-------+---------------------------------------------------------------------------------+----------------+  | . |  |  |  |  |       |                  |       |
    ///        |  | .     |        |            |    |       |     |   |      |                      |                            |   |       /                    | . |  |  |  |  |       |               |  |       |
    ///        |    .     |        |     OrderFactor |       |       . . . . .|. . . . . . . . . . . | . . . . . . . . . . .            (endeme factor table)      | . |  |  |  |  |       |                  |       |
    ///  High  |  | .     |        |            |    |       |     | . |      |               .     /|\                    .      |   |  [stores ~FACTOR]          | . |  |  |  |  |       |               |  | High  |
    ///        |    .     |   WeightFactor      | CharFactor |       .   EndemeList (1,1,1)*n . EndemeObject (n,n,n+m)     . . . . . . . . . . . . . . . . . . . . |.. |  |  |  |  |       |                  |       |
    ///  Level |  | .     |     |     |         |    |       |     | . |      |               .      |                            |   |                            | . |  |  |  |  |       |               |  | Level |
    ///        |    . SetFactor | WeightFormula |    | ValueFactor   .        |             +--------+-----------------------------------------+-------------------+ . |  |  |  |  |       |                  |       |
    ///        |  | .   |   |   |               |    |    |  |  |  | . |      |            /  .      |                            |   |       /                      . |  |  |  |  |       |               |  |       |
    ///        |    .   |   +---+--------------++----+----+-------------------+           /   . EndemeProfile (n,n,n)                   (endeme profile table)       . |  |  |  |  |       |                  |       |
    ///        |  | .   |                      |             |  |  | . |      |          /    .      |           \|/              |   |  [stores PROFILE,OBJECT,     . |  |  |  |  |       |               |  |       |
    ///        |    .   |             IEndemeActuatorFactor  |  +----------+  +----------------------+    EndemeReference (n,0,0)         DEFINITION,FIELD]          . |  |  |  |  |       |                  |       |
    ///        |  | . . | . . . . . . . . . . .|. . . . . . .|. . .|.. |   |           /      . . . /|\ . . . . . . | . . . . . . |. .|. . . . . . . . . . . . . . . . |  |  |  |  |       |               |  |       |
    ///        |        |                      |             |       .     |          /         EndemeItem (1,1,1)  |                                                . |  |  |  |  |       |                  |       |
    ///       _|  |     |                      |             |     | . |   |         /               |              |             |   |        +-----------------------+  |  |  |  |       |               |  |_     _|
    ///                 |                      |             |       .     |  +-----+    +------+----+              |                         /                      .    |  |  |  |       |                           
    ///       _   |     |                      |             |     | . |   |  |     |    |     /     |              |             |   | (endeme index table)         .    |  |  |  |       |               |   _     _ 
    ///        |        |                      |             |       .     |  |  IEndemeItem  /      |              |                    [indexes ENDEME,LIST]       .    |  |  |  |       |                  |       |
    ///        |  |     |                      |             |     | . |   |  |              /       |  Endeme      |  Endeme     |   |                              .    |  |  |  |       |               |  |       |
    ///        |        |                      |             |       .    EndemeValue (0,0,1)        |  TextFormat  |  GrabBag                                       .    +  |  |  |       |                  |       |
    ///        |  |     |                      |             |     | . |      |                      |       |      |       |     |   |                              .   /   +  |  |       |               |  |       |
    ///        |        |                      +-------------+----------------+----------------------+-------+--------------+------------------+------------------------+   /   +  |       |                  |       |
    ///        |  |     |                                          | . |                             |              |             |   |       /                      .     /   /   +       |               |  |       |
    ///        |        |                                            .                          Endeme (1,1,0)      |                   (endeme table) [stores       .    /   /   /        |                  |       |
    ///        |  |     |                                          | . |                       /    \|/             |             |   |  LIST,ENDEME,ITEM,VALUE]     .   /   /   /         |               |  |       |
    ///  Low   |        |                                            . .. . . . . . . . . . . / . . .|. . . . . . ./|\ . . . . . . . . . . . . . . . . . . . . . . . .  /   /   /          |                  |       |
    ///        |  |     +----------------------------------------------------------------------------+--------------+--------------------------+-----------------------+   /   /    +------+------+        |  |       |
    ///  Level |                                                                            / .      |                                        /                      .    /   /     |             |           | Low   |
    ///        |  |                                                |   |                   /  . EndemeSet (1,0,0)                 |   | (endeme set table)           .   /   /      |       RichDataTable  |  |       |
    ///        |                                                                          /   .      |                                   [stores SET]                .  /   /       |           |   |         | Level |
    ///        |  |                                                |   |                 /    . . . /|\ . . . . . . . . . . . . . |. .|. . . . . . . . . . . . . . . . /   /    RichSqlCommand  |   |      |  |       |
    ///        |                                                              +---------+     .      +-----------------------------------------+----------------------+   /         |     |     |   |         |       |
    ///        |  |                                                |   |     /|\              .      |                            |   |       /                      .   /          |     +-----+   |      |  |       |
    ///        |                                                         EndemeQuantification .     /|\                                 (endeme characteristic table).  /           |     |         |         |       |
    ///        |  |                                                |   | (0,0,0)              . EndemeCharacteristic (0,0,0)      |   |  [stores CHARACTERISTIC]     . /            |  InData       |      |  |       |
    ///        |  |                                                |   |                      . . . .|. . . . . . . . . . . . . . |. .|. . . . . . . . . . . . . . . ./             |  Throws       __.    |  |       |
    ///        |                                                                                     +-----------------------------------------+---------------------+              |  TreatAs.     Is.       |       |
    ///        |  |                                                |   |                            /|\                           |   |       /                                     |             FilesIO  |  |       |
    ///        |                                                                                EndemeMeaning (0,0,0)                   (endeme meaning table)               ConnectSource    RandomSource    |       |
    ///       _|  |                                                |   |                                                          |   |  [stores MEANING]                                                  |  |_     _|
    ///           [-------------------- ACTION --------------------]   [-------------------------- STATE -------------------------]   [----------------------------- DATABASE -----------------------------]           
    /// </remarks>
    public class EndemeObject : IEndemeItem
    {
        // ----------------------------------------------------------------------------------------
        //  Member properties
        // ----------------------------------------------------------------------------------------
        public EndemeProfile ItemProfile { get; set; } // make this the parent class?

        public Guid          ItemKey     { get { return _ItemKey   ; } set { _ItemKey    = value; } } Guid   _ItemKey   ;
        public string        ItemLabel   { get { return _ItemLabel ; } set { _ItemLabel  = value; } } string _ItemLabel ;
        public double        TempMatch   { get { return _TempMatch ; } set { _TempMatch  = value; } } double _TempMatch ; // ephemeral data regarding matching of the endeme profile with another endeme profile
        public string        TableName   { get { return _TableName ; } set { _TableName  = value; } } string _TableName ;
        public int           RowId       { get { return _RowId     ; } set { _RowId      = value; } } int    _RowId     ;
        public string        ColumnName  { get { return _ColumnName; } set { _ColumnName = value; } } string _ColumnName;

        public EndemeValue   Item        { get { if (_value == null || _value.Count < 1) return _value_0; else return _value_0 ; }
                                           set { if (_value == null  ) _value = new List<EndemeValue>(1);
                                                 if (_value.Count < 1) _value.Add(Simple(value)); else _value[0] = Simple(value); _value_0 = Simple(value);
                                         }     } private List<EndemeValue> _value; private EndemeValue _value_0;


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        [IndexerName("IloveJesus")]
        public EndemeValue this[int i]
        {
            get { if (_value.Count > i) return _value[i]; else return null; }
            set { if (_value.Count <= i) for (int j = _value.Count; j < i; ++j) _value.Add(EndemeValue.Empty); _value.Add(value); }
        }


        // --------------------------------------------------------------------------------------
        //  Constructors
        // --------------------------------------------------------------------------------------
        public  EndemeObject(string profile, EndemeReference enRef, EndemeValue   value) { Init(""   , new EndemeProfile(profile, enRef), value); }
        public  EndemeObject(string label  , EndemeProfile profile, EndemeValue   value) { Init(label, profile                          , value); }
        private EndemeObject(string label  , EndemeProfile profile, EndemeValue[] value) { Init(label, profile                          , value); }


        // ----------------------------------------------------------------------------------------
        //  Short methods
        // ----------------------------------------------------------------------------------------
        public void         AddSegment(Endeme en        ) { ItemProfile.Add(en)           ; }
        public EndemeObject Copy      (                 ) { return Copy_simple    ()      ; }


        // ----------------------------------------------------------------------------------------
        //  Short properties
        // ----------------------------------------------------------------------------------------
        public static EndemeObject     Empty      { get { return new EndemeObject("", EndemeProfile.Empty, new EndemeValue(null));                          } }
        public        List<EndemeItem> EnSet      { get { return ItemProfile.Segment; } set { ItemProfile.Segment = value;                                  } }
        public        EndemeReference  EnRef      { get { if (ItemProfile != null && ItemProfile.EnRef != null) return ItemProfile.EnRef; else return null; } }
        public        int              ValueCount { get { return _value.Count           ; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- CalculateTempMatch -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double CalculateTempMatch()
        {
            if (ItemProfile[0] == null) return 0.0;
            else
            {
                double output = 0.0;
                for (int i = 0; i < ItemProfile.Count; ++i)
                    output += ItemProfile[i].TempMatch;
                return output;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Copy -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EndemeObject Copy_simple()
        {
            EndemeObject enObj = new EndemeObject(ItemLabel, ItemProfile.Copy(), _value_0.Copy()); //ToArray());
            enObj.Item.Label = ItemLabel;
            enObj.ItemKey    = this.ItemKey;  // should this be copied?
            enObj.TempMatch  = 0.0;
            return enObj;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HeaderString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string HeaderString()
        {
            string str = "";
            str += "<" + ItemLabel + ">";
            if (ItemProfile == null)
            {
                str += "endeme object has null profile";
            }
            else
            {
                str += ItemProfile.ToString();
            }

            return str;
        }
        
        // --------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="code"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <param name="match"></param>
        private void Init(string label, EndemeProfile path, object value)
        {
            if (value != null && value.GetType().Name == "EndemeObject")
                throw new Exception("You may not put an EndemeObject inside an EndemeObject.");
            Init(label, path, 0.0);
            Item = new EndemeValue(value);
        }
        private void Init(string label, EndemeProfile path, EndemeValue value)
        {
            Init(label, path, 0.0);
            _value_0 = value; //.ToList<EndemeValue>();  // with linq
          //_value = new List<EndemeValue>(value);
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="code"></param>
        /// <param name="endeme"></param>
        /// <param name="match"></param>
        /// <remarks>alpha code</remarks>
        private void Init(string label, EndemeProfile path, double match)
        {
            if (path != null && path.Count == 0)
                Pause();
            ItemProfile = path ;
            ItemLabel   = label;
          //if (ItemProfile != null)  ItemProfile.TempMatch.Add(match);
            _value = new List<EndemeValue>(1);
        }
        private void Init(string label, string code, Endeme endeme, double match) { Init(label, new EndemeProfile(endeme), match); }

        private void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Simple -->
        /// <summary>
        ///      Absorbs undefined values (i.e values without profiles) (i.e.a pathless EndemeObjects)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static EndemeValue Simple(EndemeValue input)
        {
            EndemeValue storeValue = new EndemeValue(input);
            if (input                 != null               &&
                input.Value           != null               &&
                input.Value.GetType() == typeof(EndemeItem) &&
                ((EndemeObject)input.Value).ItemProfile == null)
                   storeValue = ((EndemeObject)input.Value).Item;
            return storeValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public override string ToString()
        {
            string str   = HeaderString();
            string match = (TempMatch + 0.0005).ToString();  if (match.Length > 5) match = match.Substring(0,5);  str += "("+match+")";
            string value = "null";                           if (_value_0 != null) value = _value_0.ToString();   str += "=" + value;
            return "[" + str + "]";
        }
    }
}
