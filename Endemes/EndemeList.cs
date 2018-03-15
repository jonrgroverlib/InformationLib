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
using System.Collections;             // for 
using System.Collections.Generic;     // for 
using System.Linq;                    // for 
using System.Linq.Expressions;        // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeList -->
    /// <summary>
    ///      The EndemeList class contains a simple list of endemes
    /// </summary>
    /// <remarks>
    ///      EndemeLists are usually all of one EndemeSet, but this is no longer required
    ///      
    ///      add foreach
    ///      not EndemeValue
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
    ///      beta code - nearly production ready, a very new class built using lessons learned from
    ///      a number of previous classes
    /// </remarks>
    public class EndemeList : IEnumerable<Guid> //, IQueryable
    {
        // ----------------------------------------------------------------------------------------
        //  Private members - private because the index has to be carefully managed
        // ----------------------------------------------------------------------------------------
        private Dictionary<Guid,EndemeItem  > _list    ; // the authorititative source for content
        private List<Guid>                    _order   ; // the authorititative order
        private Dictionary<string,List<Guid>> _labelIdx; // index for the labels
        private Dictionary<string,List<Guid>> _codeIdx ; // TODO:build me


        // ----------------------------------------------------------------------------------------
        //  Public members
        // ----------------------------------------------------------------------------------------
        public double          EqualityThreshold { get; set; }
        public EndemeReference EnRef             { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Read-only members/properties
        // ----------------------------------------------------------------------------------------
        public int       Count        { get { return _list.Count; } }
        public string    Label        { get { return _label     ; } } private string     _label;
        public EndemeSet DefaultEnSet { get { return _enSet     ; } } private EndemeSet  _enSet;


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        public EndemeItem   this[Guid   key] { get { if (_list.ContainsKey(key)    ) return _list[key]      ; else return null; } }
        public EndemeItem   this[int    i  ] { get { if (0 <= i && i < _order.Count) return _list[_order[i]]; else return null; } }
      //public List<Guid>   this[string lbl] { get { if (_labelIdx.ContainsKey(lbl)) return _labelIdx[lbl]  ; else return null; } } // TODO: need to convert to an EndemeElement list
        public EndemeItem   this[string en ]
        {   get { EndemeItem item = GetExactField(en); if (item == null) item = SetFieldExactly(en, new EndemeValue(null)); return item; }
            set { EndemeItem item = GetExactField(en); if (item == null) item = SetFieldExactly(en, value); else item.Item = value.Item; }
        }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeList (string label, EndemeSet enSet) { Init(label, enSet ); }
        public EndemeList (string label, EndemeReference enRef, double equalThreshold)
        {
            Init(label, null);
            if (enRef == null) throw new Exception("boom"); // EnRef = new EndemeReference();
            else this.EnRef = enRef;
            this.EqualityThreshold = equalThreshold;

            this._enSet = enRef.SetRef.FirstOrDefault(x => x.Key != "").Value;
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
      //public          Type              ElementType             { get { return typeof(Guid);                                                           } }
        public          IEnumerator<Guid> GetEnumerator       ()        { return _order.GetEnumerator();                                                 }
                        IEnumerator       IEnumerable.GetEnumerator()   { return GetEnumerator();                                                        }
        public          void              RemoveAt            (int ord) { Guid guid = _order[ord]; Remove(guid); if (!Sane) throw new NullReferenceException("EndemeList '"+Label+"' is insane."); }
        private         bool              Sane                    { get { bool sane = (_list.Count >= _labelIdx.Count); if (!sane) Pause(); return sane; } }
        public override string            ToString            ()        { return DefaultEnSet.Label + ":" + Label + " (" + Count + ")";                           }


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds an element to the EndemeList
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="element"></param>
        /// <returns>
        ///      Probably ought to return the element not just the Guid since the Element contains the Guid
        /// </returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public Guid Add(Guid guid, EndemeItem   element) // <---------------------------------------+
        {                                                                                        // |
            element.ItemKey = guid;                                                              // |
            _list.Add (guid, element);                                                           // |
            _order.Add(guid);                                                                    // |
            AddToIndex(guid, element);                                                           // |
            if (element.ItemEndeme.EnSet == null) element.EnSet = DefaultEnSet;                         // |
                                                                                                 // |
            if (!Sane) throw new NullReferenceException("EndemeList '"+Label+"' is insane.");    // |
            return guid;                                                                         // |
        }                                                                                        // |
        public Guid Add(EndemeItem element                                                  ) { return Add(Guid.NewGuid(), element);                                      } // <-+--+
        public Guid Add(string label             , string endeme, object value              ) { return Add(new EndemeItem(label, DefaultEnSet, endeme, new EndemeValue(value))); } // --+  |
        public Guid Add(string label, string code, string endeme, object value, double match) { Guid id = Add(new EndemeItem(label, new Endeme(DefaultEnSet, endeme), new EndemeValue(value))); this[id].ItemCode = code; this[id].TempMatch = match; return id; }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds and Endeme Item/value to the list of fields
        /// </summary>
        /// <param name="label"></param>
        /// <param name="enSet"></param>
        /// <param name="endeme"></param>
        /// <param name="value"></param>
        /// <remarks>
        ///      Fills in the endeme set, creates a new endeme item with the value,
        ///      adds it to the list of field values, adds the endeme set to the reference if needed
        /// </remarks>
        /// <returns></returns>
        public EndemeItem Add(string label, EndemeSet enSet, Endeme endeme, object value)
        {
            EndemeItem item = null;

            //if (enSet == null)
            //    Pause();
            if (endeme == null) return null;
            else
            {
                if (endeme.EnSet == null)  endeme.EnSet = enSet;
                if (endeme.EnSet != enSet) throw new Exception("boom");
                item = new EndemeItem(label, endeme, new EndemeValue(value));
                this.Add(item);
                if (enSet != null)
                {
                    if (this.EnRef[enSet.Label] == null) this.EnRef.Add(enSet);
                }
            }

            return item;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds and Endeme Item/value to the list of fields
        /// </summary>
        /// <param name="label"></param>
        /// <param name="enSet"></param>
        /// <param name="endeme"></param>
        /// <param name="value"></param>
        /// <remarks>gets the endeme set, creates a new endeme, Adds and EndemeItem to the list</remarks>
        public void Add(string label, string enSet, string endeme, string value)
        {
            EndemeSet endemeSet = this.EnRef[enSet];
            if (endemeSet == null || endemeSet.Count < 1) throw new Exception("Please do not use empty/unreferenced endeme sets {"+enSet+"} when storing endeme values.");
            Endeme en = new Endeme(endemeSet, endeme);
            this.Add(label, endemeSet, en, value);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddToIndex -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="element"></param>
        /// <remarks>beta code - nearly production ready</remarks>
        private void AddToIndex(Guid guid, EndemeItem element)
        {
            string label = element.ItemLabel;
            if (!_labelIdx.ContainsKey(label))
                _labelIdx.Add(label,new List<Guid>());
            _labelIdx[label].Add(guid);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiValue -->
        /// <summary>
        ///      useful for debugging
        /// </summary>
        public string AsciiValue { get
        {
            string str = "";
            if (this.Count > 0)
            {
                string delim = "";
                if (Count > 250)
                {
                    StringBuilder build = new StringBuilder(200 * AsciiItem(this[0]).Length);
                    for (int i = 0; i < 100; ++i) { build.Append(delim + AsciiItem(this[i])); delim = "\r\n"; }
                    build.Append(delim + "<snip "+(Count-200).ToString()+">");
                    for (int i = Count-100; i < Count; ++i) build.Append(delim + AsciiItem(this[i]));
                    str = build.ToString();
                }
                else
                {
                    for (int i = 0; i < this.Count; ++i) { str += delim + AsciiItem(this[i]); delim = "\r\n"; }
                }
            }
            return str;
        } }

        private string AsciiItem(EndemeItem item)
        {
            string str = "";
            if (item.Item != null && item.Item.GetType() == typeof(Endeme))
                str = "[" + item.HeaderString() + ((Endeme)item.Item.Value).FullDesignation + "]";
            else str = item.ToString();
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsSet -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enSet"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        internal bool ContainsSet(EndemeSet enSet)
        {
            foreach (Guid key in _list.Keys)
            {
                if (enSet == _list[key].ItemEndeme.EnSet)
                    return true;
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CopyAll -->
        /// <summary>
        ///      Makes a copy preserving the keys
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public EndemeList CopyAll()
        {
            EndemeList list = new EndemeList(Label, this.DefaultEnSet);
            foreach (Guid key in this)
                list.Add(key, this[key].Copy());
            if (!list.Sane) throw new NullReferenceException("EndemeList '"+list.Label+"' is insane.");
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DistinctEndemes -->
        /// <summary>
        ///      Returns a list of completely distinct endemes
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public List<Endeme> DistinctEndemes()
        {
            List<Endeme> list = new List<Endeme>();
            foreach (EndemeItem el in _list.Values)
                if (!list.Contains(el.ItemEndeme))
                    list.Add(el.ItemEndeme);
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Educe -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public EndemeItem Educe(string en)
        {
            EndemeItem item = this[en];
            if (item == null)
            {
                this.Add("", Endeme.Part(0,en), Endeme.Part(1,en), null);
                item = this[en];
            }
            return item;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExactlyLike -->
        /// <summary>
        ///      Returns an item only if it has exactly the same endeme
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public EndemeItem ExactlyLike(Endeme en)
        {
            int count = 0;
            EndemeItem item = null;
            for (int i = 0; i < this.Count; ++i)
            {
                if (this[i].ItemEndeme == en)
                {
                    count++;
                    item = this[i];
                }
            }
            if (count > 1) throw new Exception("boom");
            return item;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetExactField -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enSet"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public EndemeItem GetExactField(EndemeSet enSet, Endeme en)
        {
            if (en == null || enSet == null) return null;
            else
            {
                if (en.EnSet == null)      en.EnSet = enSet;
                if (en.EnSet != enSet)     throw new Exception("boom");
                if (!this.ContainsSet(enSet)) return null;
              //if (!this.EnRef.ContainsSet(enSet)) return null;
                EndemeItem item = this.ExactlyLike(en);
                return item;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetExactField -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enFormatted"></param>
        /// <returns></returns>
        public EndemeItem GetExactField(string enFormatted)
        {
            string setLabel = Endeme.Part(0,enFormatted);
            EndemeSet enSet = this.EnRef[setLabel];
            if (enSet == null || enSet.Count < 1 || string.IsNullOrEmpty(enSet.Label))
                Pause();
              //throw new KeyNotFoundException("Endeme set '"+setLabel+"' not found in endeme reference");
            return this.GetExactField(enSet, Endeme.Part(1,enFormatted));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetField -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enSet"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public EndemeItem GetField(EndemeSet enSet, Endeme en)
        {
            if (en == null || enSet == null) return null;
            else
            {
                if (en.EnSet == null)      en.EnSet = enSet;
                if (en.EnSet != enSet)     throw new Exception("boom");
                if (!this.ContainsSet(enSet)) return null;
                EndemeItem item = this.MostLike(en);
                return item;
            }
        }
        public EndemeItem GetField(string enFormatted) { return GetField(EnRef[Endeme.Part(0,enFormatted)], Endeme.Part(1,enFormatted)); }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetFromCode -->
        /// <summary>
        ///      This is jsut a stopgap for now, it needs an index to run fast
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <remarks>TODO: fix me, need to build a code index or something</remarks>
        public List<EndemeItem> GetFromCode(string code)
        {
            List<EndemeItem> list = new List<EndemeItem>();
            foreach (EndemeItem el in _list.Values)
            {
                EndemeItem found = EndemeItem.Empty;
                if (el.ItemCode == code)
                {
                    found = el;
                    list.Add(found);
                }
            }

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Initializes the endeme list
        /// </summary>
        /// <param name="label"></param>
        /// <param name="set"></param>
        private void Init(string label, EndemeSet set)
        {
            _label    = label;
            _list     = new Dictionary<Guid,EndemeItem>();
            _order    = new List<Guid>();
            _labelIdx = new Dictionary<string,List<Guid>>();
            _codeIdx  = new Dictionary<string,List<Guid>>();
            _enSet    = set;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Insert -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ord"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public EndemeItem Insert(int ord, EndemeItem element)
        {
            Guid guid = Guid.NewGuid();
            _list.Add(guid, element);
            _order.Insert(ord, guid);
            AddToIndex(guid, element);

            if (!Sane) throw new NullReferenceException("EndemeList '"+Label+"' is insane.");
            return element;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MostLike -->
        /// <summary>
        ///      Returns the element most similar to the input endeme using the Match algorithm
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public EndemeItem MostLike(Endeme en)
        {
            EndemeList orderedList = this.OrderBy(en);
            if (orderedList.Count == 0) return null;
            EndemeItem element     = orderedList[orderedList.Count-1];
            EndemeItem el2         = this[element.ItemKey];
            el2.TempMatch = element.TempMatch;
            return el2;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OrderBy -->
        /// <summary>
        ///      Orders the elements in the list using the 'NewMatch' algorithm (produces a new list)
        /// </summary>
        /// <param name="sortEndeme"></param>
        /// <returns></returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public EndemeList OrderBy(Endeme sortEndeme)
        {
            if (sortEndeme.EnSet == null)
                sortEndeme.EnSet = DefaultEnSet;


            // --------------------------------------------------------------------------
            //  Put the keys in order, preserving the keys
            // --------------------------------------------------------------------------
            EndemeList toOrder = this.CopyAll();
            foreach (Guid key in toOrder)
            {
                toOrder[key].TempMatch = sortEndeme.Match(toOrder[key].ItemEndeme, WeightFormula.Refined);
                // TODO: change this to Match1
            }


            // --------------------------------------------------------------------------
            //  Order the list by match using Linq  TODO: use this!
            // --------------------------------------------------------------------------
            List<Guid> orderedKeys = toOrder._list.OrderBy(x => x.Value.TempMatch).Select(x => x.Key).ToList();
            

            //// --------------------------------------------------------------------------
            ////  Order the list by match without Linq
            //// --------------------------------------------------------------------------
            //Dictionary<Guid,double> quantRaw = new Dictionary<Guid,double>();
            //for (int i = 0; i < toOrder.Count; ++i)
            //    quantRaw.Add(toOrder[i].ItemKey, toOrder[i].TempMatch);
            //SortedDictionary<Guid,double> raw = new SortedDictionary<Guid,double>(quantRaw);
            //List<Guid> orderedKeys = new List<Guid>();
            //List<double> values = new List<double>(raw.Values);
            //values.Sort();
            //for (int i = 0; i < values.Count; ++i)
            //{
            //    Guid g = Guid.Empty;
            //    foreach (Guid key in raw.Keys)
            //    {
            //        if (raw[key] == values[i] && raw[key] > 0)
            //            g = key;
            //    }
            //    if (g != Guid.Empty)
            //    {
            //        orderedKeys.Add(g);
            //        raw.Remove(g);
            //    }
            //}


            // --------------------------------------------------------------------------
            //  Put the list in order, preserving the keys
            // --------------------------------------------------------------------------
            EndemeList ordered = new EndemeList(Label, toOrder.DefaultEnSet);
            foreach (Guid key in orderedKeys)
            {
                ordered.Add(key, toOrder[key]);
            }

            if (!Sane) throw new NullReferenceException("EndemeList '"+Label+"' is insane.");
            return ordered;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pause -->
        /// <summary>
        /// 
        /// </summary>
        private void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Remove -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <remarks>beta code - nearly production ready</remarks>
        public void Remove(Guid guid)
        {
            string label = _list[guid].ItemLabel;

            List<Guid> guids = _labelIdx[label];
            guids.Remove(guid);
            if (guids.Count == 0)
                _labelIdx.Remove(label);

            _order.Remove(guid);
            _list.Remove(guid);
            if (!Sane) throw new NullReferenceException("EndemeList '"+Label+"' is insane.");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetField -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        public void SetField(EndemeSet enSet, Endeme endeme, object value)
        {
            // --------------------------------------------------------------------------
            //  Preprocess the endeme
            // --------------------------------------------------------------------------
            if (endeme == null)
                return;  // storage location not definced
            else
            {
                if (endeme.EnSet == null)  endeme.EnSet = enSet;
                if (endeme.EnSet != enSet) throw new Exception("error- storage endeme has wrong endeme set");


                // ----------------------------------------------------------------------
                //  Figure out what value to store and where to store the value
                // ----------------------------------------------------------------------
                EndemeItem item = this.MostLike(endeme);
                if (item == null || item.TempMatch < this.EqualityThreshold)
                     item = this.Add("", enSet, endeme, EndemeItem.Simple(new EndemeValue(value)));
                else item.Item = EndemeItem.Simple(new EndemeValue(value));


                //string str = AsciiValue;
            }
        }
        public void SetField(string enFormatted, object value) { SetField(this.EnRef[Endeme.Part(0,enFormatted)], Endeme.Part(1,enFormatted), value); }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetFieldExactly -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enSet"></param>
        /// <param name="endeme"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public EndemeItem SetFieldExactly(EndemeSet enSet, Endeme endeme, object value)
        {
            EndemeItem item = null;


            // --------------------------------------------------------------------------
            //  Preprocess the endeme
            // --------------------------------------------------------------------------
            if (endeme == null)
                return null;
            else
            {
                if (endeme.EnSet == null)  endeme.EnSet = enSet;
                if (endeme.EnSet != enSet) throw new Exception("boom");


                // ----------------------------------------------------------------------
                //  Figure out what value to store and where to store the value
                // ----------------------------------------------------------------------
                item = this.ExactlyLike(endeme);
                if (item == null) item = this.Add("", enSet, endeme, EndemeItem.Simple(new EndemeValue(value)));
                else              item.Item = EndemeItem.Simple(new EndemeValue(value));


              //string str = AsciiValue;
            }

            return item;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetFieldExactly -->
        /// <summary>
        ///      This can't be public because EndemeField knows nothing about existing endeme sets
        /// </summary>
        /// <param name="enFormatted"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public EndemeItem SetFieldExactly(string enFormatted, object value)
        {
            string setLabel = Endeme.Part(0,enFormatted);
            string enString = Endeme.Part(1,enFormatted);
            if (this.EnRef[setLabel] == null)
                Pause();
            EndemeItem item = this.SetFieldExactly(this.EnRef[setLabel], enString, value);
            return item;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToList -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<IEndemeItem> ToList()
        {
            List<IEndemeItem> list = new List<IEndemeItem>();
            for (int i = 0; i < Count; ++i) list.Add(this[i]);
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UniquenessTest -->
        /// <summary>
        ///      Determines whether all elements have unique endemes
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///      TODO: add in the possibility that there may be more than one endeme set
        ///      alpha code
        /// </remarks>
        public bool UniquenessTest()
        {
            Dictionary<string,int> tally = new Dictionary<string,int>(_list.Count);
            for (int i = 0; i < _order.Count; ++i)
            {
                EndemeItem en = this[i];
                string key = en.ItemEndeme.ToString();
                if (!tally.ContainsKey(key))
                   tally.Add(key, 0);
                tally[key]++;
            }

            if (tally.Count > _labelIdx.Count)  throw new ApplicationException("Missing item in endeme list index");
            if (tally.Count < _labelIdx.Count)  throw new ApplicationException("Extra item in endeme list index");

            return (_labelIdx.Count == _list.Count);
        }
    }
}
