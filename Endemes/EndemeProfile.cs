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
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeProfile -->
    /// <summary>
    ///      The EndemeProfile class stores an endeme path to one or more endeme objects,
    ///      Delimiters between endemes: !*+^    Delimiter within endemes: :
    /// </summary>
    /// <remarks>
    ///      EndemeObjects are not the same as objects although each one contains an object
    ///      Maybe this should be EndemeProfile
    /// 
    ///      Endeme object/class/interface/table dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = { 0, 1, n(infinity) }     31 classes, 7 tables, 2 interfaces, 1 enum
    ///      ----------------------------------------------------
    ///
    ///           [---------------------- ACTION ------------------]   [-------------------------- STATE -------------------------]   [----------------------------- DATABASE -----------------------------]           
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
    ///        |  | .                          /|\                 |   |      |                      |                            |   |  [ACTUATOR]             |  | . |  |  |  |  |       |               |  |       |
    ///        |    .     +--------+------------+----+-------+---------------------------------------------------------------------------------+----------------+  | . |  |  |  |  |       |                  |       |
    ///        |  | .     |        |            |    |       |     |   |      |                      |                            |   |       /                    | . |  |  |  |  |       |               |  |       |
    ///        |    .     |        |     OrderFactor |       |       . . . . .|. . . . . . . . . . . | . . . . . . . . . . .            (endeme factor table)      | . |  |  |  |  |       |                  |       |
    ///  High  |  | .     |        |            |    |       |     | . |      |               .     /|\                    .      |   |  [~FACTOR]                 | . |  |  |  |  |       |               |  | High  |
    ///        |    .     |   WeightFactor      | CharFactor |       .   EndemeList (1,1,1)*n . EndemeObject (n,n,n+m)     . . . . . . . . . . . . . . . . . . . . |.. |  |  |  |  |       |                  |       |
    ///  Level |  | .     |     |     |         |    |       |     | . |      |               .      |                            |   |                            | . |  |  |  |  |       |               |  | Level |
    ///        |    . SetFactor | WeightFormula |    | ValueFactor   .        |             +--------+-----------------------------------------+-------------------+ . |  |  |  |  |       |                  |       |
    ///        |  | .   |   |   |               |    |    |  |  |  | . |      |            /  .      |                            |   |       /                      . |  |  |  |  |       |               |  |       |
    ///        |    .   |   +---+--------------++----+----+-------------------+           /   . EndemeProfile (n,n,n)                   (endeme profile table)       . |  |  |  |  |       |                  |       |
    ///        |  | .   |                      |             |  |  | . |      |          /    .      |           \|/              |   |  [stores PROFILE,OBJECT,     . |  |  |  |  |       |               |  |       |
    ///        |    .   |             IEndemeActuatorFactor  |  +----------+  +----------------------+    EndemeReference (n,0,0)         DEFINITION,FIELD]          . |  |  |  |  |       |                  |       |
    ///        |  | . . | . . . . . . . . . . .|. . . . . . .|. . .|.. |   |           /      . . . /|\ . . . . . . | . . . . . . |. .|. . . . . . . . . . . . . . . . |  |  |  |  |       |               |  |       |
    ///        |        |                      |             |       .     |          /         EndemeItem (1,1,1)  |                                                . |  |  |  |  |       |                  |       |
    ///       _|  |     |                      |             |     | . |   |         /               |              |             |   |                              . |  |  |  |  |       |               |  |_     _|
    ///                 |                      |             |       .     |  +-----+    +------+----+              |                                                . |  |  |  |  |       |                           
    ///       _   |     |                      |             |     | . |   |  |     |    |     /     |              |             |   |                              . |  |  |  |  |       |               |   _     _ 
    ///        |        |                      |             |       .     |  |  IEndemeItem  /      |              |                          +-----------------------+  |  |  |  |       |                  |       |
    ///        |  |     |                      |             |     | . |   |  |              /       |  Endeme      |  Endeme     |   |       /                      .    |  |  |  |       |               |  |       |
    ///        |        |                      |             |       .    EndemeValue (0,0,1)        |  TextFormat  |  GrabBag          (endeme index table)         .    +  |  |  |       |                  |       |
    ///        |  |     |                      |             |     | . |      |                      |       |      |       |     |   |  [indexes ENDEME,LIST]       .   /   +  |  |       |               |  |       |
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
    ///        |  |                                                |   |     /|\              .     /|\                           |   |       /                      .   /          |     +-----+   |      |  |       |
    ///        |                                                         EndemeQuantification . EndemeCharacteristic (0,0,0)            (endeme characteristic table).  /           |     |         |         |       |
    ///        |  |                                                |   | (0,0,0)              .      |                            |   |  [stores CHARACTERISTIC]     . /            |  InData       |      |  |       |
    ///        |  |                                                |   |                      . . . .|. . . . . . . . . . . . . . |. .|. . . . . . . . . . . . . . . ./             |  Throws       __.    |  |       |
    ///        |                                                                                     +-----------------------------------------+---------------------+              |  TreatAs.     Is.       |       |
    ///        |  |                                                |   |                            /|\                           |   |      ./                                     |             FilesIO  |  |       |
    ///        |                                                                                EndemeMeaning (0,0,0)                   (endeme meaning table)               ConnectSource    RandomSource    |       |
    ///       _|  |                                                |   |                                                          |   |  [stores MEANING]                                                  |  |_     _|
    ///           [---------------------- ACTION ------------------]   [-------------------------- STATE -------------------------]   [----------------------------- DATABASE -----------------------------]           
    /// </remarks>
    public class EndemeProfile
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public List<EndemeItem> Segment     { get; set; }
        public char             Operator    { get; set; } // wondering if operator should be a single character rather than a list *
      //public List<double>     TempMatch   { get; set; }
        public EndemeDefinition ParentArray { get; set; } // a reference to the endeme register that this is part of
        public EndemeReference  EnRef       { get; set; } // a reference to the endeme reference that this is part of


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        public EndemeItem this[string    setName] { get { int i = IndexOf(setName); if (i >= 0) return Segment[i]; else return null; } }
        public EndemeItem this[EndemeSet enSet  ] { get { int i = IndexOf(enSet)  ; if (i >= 0) return Segment[i]; else return null; } }
        public EndemeItem this[int       i      ] { get { if (0 <= i && i < Segment.Count)      return Segment[i]; else return null; } }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeProfile(                                         ) { Init();                                                              }
        public EndemeProfile(EndemeItem enItem                        ) { Init(); Segment.Add(enItem)                ; Operator = '+'        ; }
        public EndemeProfile(string     profile, EndemeReference enRef) { Init(); FillSegments(profile, enRef)       ; FillOperators(profile); }
        public EndemeProfile(Endeme     en                            ) { Init(); Segment.Add(new EndemeItem(en, "")); Operator = '+'        ; }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public        void          Add     (Endeme     en    ) { EndemeItem item = new EndemeItem(en, ""); item.TempMatch = 0.0; Segment.Add(item); }
        public        void          Add     (EndemeItem item  ) {                                           item.TempMatch = 0.0; Segment.Add(item); }
        public        bool          Contains(EndemeSet  enSet ) { return (IndexOf(enSet) >= 0)                           ; }
        public        int           Count                 { get { return Segment.Count                                   ; } }
        public static EndemeProfile Empty                 { get { return new EndemeProfile()                             ; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- BuildSegment -->
        /// <summary>
        ///      Converts a formatted endeme item string into an endeme item with a string value
        /// </summary>
        /// <param name="enItemFormatted"></param>
        /// <param name="enRef"></param>
        /// <returns></returns>
        public static EndemeItem BuildSegment(string enItemFormatted, EndemeReference enRef)
        {
            // --------------------------------------------------------------------------
            //  Resolve endeme set
            // --------------------------------------------------------------------------
            string    label = Endeme.Part(0,enItemFormatted);
            EndemeSet enSet = null;
            if (enRef != null)   enSet = enRef[label];
            if (enSet == null || string.IsNullOrEmpty(enSet.Label))
                enSet = new EndemeSet(label);


            // --------------------------------------------------------------------------
            //  Build endeme item
            // --------------------------------------------------------------------------
            EndemeItem enItem = new EndemeItem(new Endeme(enSet, Endeme.Part(1,enItemFormatted)), "");
            enItem.Item = Endeme.Part(2,enItemFormatted);
            return enItem;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Contains -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enTarget"></param>
        /// <returns></returns>
        internal bool Contains(Endeme enTarget)
        {
            Endeme enTgt = enTarget;
            bool   found = false;
            for (int idx = 0; idx < Count; ++idx)
                if (this[idx].ItemEndeme.Contains(enTgt))
                    found = true;
            return found;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsCount -->
        /// <summary>
        ///      Determines the number of segments containing the count of the target endeme
        /// </summary>
        /// <param name="enTarget"></param>
        /// <returns></returns>
        internal int ContainsCount(EndemeProfile profile, Endeme enTarget)
        {
            Endeme enTgt = enTarget;
            int    found = 0;
            for (int j = 0; j < profile.Count; ++j)
                found += profile[j].ItemEndeme.ContainsCount(enTgt);
            return found;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Copy -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EndemeProfile Copy()
        {
            EndemeProfile profile = new EndemeProfile();
            profile.Operator      = this.Operator;
          //profile.TempMatch     = new List<double>(this.TempMatch);
            profile.ParentArray   = this.ParentArray;

            
            profile.Segment = new List<EndemeItem>(this.Segment.Count);
            for (int i = 0; i < Segment.Count; ++i)
                profile.Segment.Add(this.Segment[i].Copy());
            return profile;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FillOperators -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void FillOperators(string path)
        {
            char[] op = Regex.Replace(path, "[^!*+^]", "").ToCharArray();
            if (op.Length > 0) Operator = op[0]; else Operator = '+'; // default first op to second else default Op is '+'
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FillSegments -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="enRef"></param>
        private void FillSegments(string path, EndemeReference enRef)
        {
            string[] seq = path.Split("|!*+^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < seq.Length; ++i)
                Segment.Add(BuildSegment(seq[i], enRef));
        }

        private int IndexOf(EndemeSet enSet  ) { for (int i = 0; i < this.Segment.Count; ++i) if (Segment[i].EnSet       == enSet  ) return i; return -1; }
        private int IndexOf(string    setName) { for (int i = 0; i < this.Segment.Count; ++i) if (Segment[i].EnSet.Label == setName) return i; return -1; }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            Segment   = new List<EndemeItem>();
            Operator  = '+';
          //TempMatch = new List<double>();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsSubsetOf -->
        /// <summary>
        ///      Determines whether the input profile is a subset of this profile
        /// </summary>
        /// <param name="profilethis"></param>
        /// <returns></returns>
        public bool IsSubsetOf(EndemeProfile profile)
        {
            Dictionary<EndemeItem, int> index = new Dictionary<EndemeItem,int>(profile.Count);
            for (int i = 0; i < profile.Count; ++i) index.Add(profile.Segment[i],i);
            for (int j = 0; j < Segment.Count; ++j) if (!index.ContainsKey(Segment[j])) return false;
            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Match -->
        /// <summary>
        ///      Match two profiles, saving the results internally
        /// </summary>
        /// <param name="matchProfile"></param>
        internal void Match(EndemeProfile matchProfile)
        {
            for (int i = 0; i < this.Segment.Count; ++i)
            {
                EndemeSet enSet = this.Segment[i].EnSet;
                if (matchProfile.Contains(enSet))
                {
                    EndemeItem segment2 = matchProfile[enSet];
                    EndemeItem segment1 = Segment[i];
                    segment1.TempMatch = segment1.ItemEndeme.Match(segment2.ItemEndeme, WeightFormula.Refined);
                  //TempMatch[i] = segment1.ItemEndeme.Match(segment2.ItemEndeme, WeightFormula.Refined);
                }
            }
        }

        private void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetLabels -->
        /// <summary>
        ///      Retursn a list of the endeme set labels
        /// </summary>
        public List<string> SetLabels { get
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Segment.Count; ++i)
            {
                Endeme en = Segment[i].ItemEndeme;
                if (en != null && en.EnSet != null) list.Add(en.EnSet.Label);
            }
            return list;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Sets -->
        /// <summary>
        /// 
        /// </summary>
        public List<EndemeSet> Sets { get
        {
            List<EndemeSet> list = new List<EndemeSet>();
            for (int i = 0; i < Segment.Count; ++i)
            {
                EndemeItem enItem = Segment[i];
                if (enItem != null && enItem.EnSet != null) list.Add(enItem.EnSet);
            }
            return list;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "";
            string delim = "";
            for (int i = 0; i < Segment.Count; ++i)
            {
                string val = Segment[i].Item.ToString();
                if (string.IsNullOrEmpty(val))
                     str += delim + Segment[i].ItemEndeme.FullDesignation;
                else str += delim + Segment[i].ItemEndeme.FullDesignation + ":" + val;
                delim = Operator.ToString();
            }
            return str;
        }
    }
}
