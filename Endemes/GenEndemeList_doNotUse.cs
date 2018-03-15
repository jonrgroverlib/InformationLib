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
using System.Collections.ObjectModel; // for 
using System.Linq;                    // for 
using System.Linq.Expressions;        // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    //// --------------------------------------------------------------------------------------------
    ///// <!-- GenEndemeList -->
    ///// <summary>
    /////      The GenEndemeList class contains a simple list of endemes
    ///// </summary>
    ///// <remarks>
    /////      GenEndemeLists are usually all of one EndemeSet, but this is no longer required
    /////      
    /////      add foreach
    /////      
    /////      do not use - the reason why is that this runs counter to my theory of data semantics
    ///// </remarks>
    //public class GenEndemeList_doNotUse<T> : Collection<T>, IEnumerable<Guid> //, IQueryable
    //{
    //    // ----------------------------------------------------------------------------------------
    //    //  Private members - private because the index has to be carefully managed
    //    // ----------------------------------------------------------------------------------------
    //    private Dictionary<Guid,GenEndemeItem_donotUse<T>> _list; // the authorititative source for content
    //    private List<Guid>                    _order   ; // the authorititative order
    //    private Dictionary<string,List<Guid>> _labelIdx; // index for the labels
    //    private Dictionary<string,List<Guid>> _codeIdx ; // TODO:build me


    //    // ----------------------------------------------------------------------------------------
    //    //  Read-only members/properties
    //    // ----------------------------------------------------------------------------------------
    //    public int       _Count { get { return _list.Count; } }
    //    public string    Name  { get { return _name      ; } } private string    _name;
    //    public EndemeSet Set   { get { return _set       ; } } private EndemeSet _set ;


    //    // ------------------------------------------------------------------------------
    //    //  Accessors
    //    // ------------------------------------------------------------------------------
    //    public GenEndemeItem_donotUse<T> this[Guid   key] { get { if (_list.ContainsKey(key)    ) return _list[key]      ; else return null; } }
    //    public GenEndemeItem_donotUse<T> this[int    i  ] { get { if (0 <= i && i < _order.Count) return _list[_order[i]]; else return null; } }
    //    public List<Guid> this[string lbl] { get { if (_labelIdx.ContainsKey(lbl)) return _labelIdx[lbl]  ; else return null; } } // TODO: need to convert to an GenEndemeElement list


    //    // ----------------------------------------------------------------------------------------
    //    //  Constructor
    //    // ----------------------------------------------------------------------------------------
    //    public GenEndemeList_doNotUse(string name, EndemeSet set) { Init(name, set ); }
    //    public GenEndemeList_doNotUse(string name               ) { Init(name, null); }
    //    private void Init(string name, EndemeSet set)
    //    {
    //        _name     = name;
    //        _list     = new Dictionary<Guid,GenEndemeItem_donotUse<T>>();
    //        _order    = new List<Guid>();
    //        _labelIdx = new Dictionary<string,List<Guid>>();
    //        _codeIdx  = new Dictionary<string,List<Guid>>();
    //        _set      = set;
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    //  Short methods, properties
    //    // ----------------------------------------------------------------------------------------
    //    public          Type              ElementType             { get { return typeof(Guid);                                                           } }
    //    public          IEnumerator<Guid> GetEnumerator       ()        { return _order.GetEnumerator();                                                 }
    //                    IEnumerator       IEnumerable.GetEnumerator()   { return GetEnumerator();                                                        }
    //    public          void              RemoveAt            (int ord) { Guid guid = _order[ord]; Remove(guid); if (!Sane) throw new NullReferenceException("GenEndemeList '"+Name+"' is insane."); }
    //    private         bool              Sane                    { get { bool sane = (_list.Count >= _labelIdx.Count); if (!sane) Pause(); return sane; } }
    //    public override string            ToString            ()        { return Set.Label + ":" + Name + " (" + _Count + ")";                             }

    //    //public          Expression        Expression                    { get; private set;                                                              }
    //    //       static   IEnumerable<Guid> OrderByLabel(GenEndemeList list) { List<Guid> ordered = list._list.OrderBy(x => x.Value.Label).Select(x => x.Key).ToList(); return ordered; }
    //    //public          IQueryProvider    Provider                      { get; private set;                                                              }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Add -->
    //    /// <summary>
    //    ///      Adds an element to the GenEndemeList
    //    /// </summary>
    //    /// <param name="guid"></param>
    //    /// <param name="element"></param>
    //    /// <returns>
    //    ///      Probably ought to return the element not just the Guid since the Element contains the Guid
    //    /// </returns>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public Guid Add(Guid guid, GenEndemeItem_donotUse<T> element) // <-----------------------------------------+
    //    {                                                                                        // |
    //        element.ItemKey = guid;                                                                  // |
    //        _list.Add (guid, element);                                                           // |
    //        _order.Add(guid);                                                                    // |
    //        AddToIndex(guid, element);                                                           // |
    //        if (element.ItemEndeme.EnSet == null) element.Set = Set;                                 // |
    //                                                                                             // |
    //        if (!Sane) throw new NullReferenceException("GenEndemeList '"+Name+"' is insane.");     // |
    //        return guid;                                                                         // |
    //    }                                                                                        // |
    //    public Guid Add(GenEndemeItem_donotUse<T> element                                 ) { return Add(Guid.NewGuid(), element);                                        } // <--+--+
    //    public Guid Add(string label             , string endeme, T value) { return Add(new GenEndemeItem_donotUse<T>(label, Set, endeme, value));                   } // ---+  |
    //    public Guid Add(string label, string code, string endeme, T value) { return Add(new GenEndemeItem_donotUse<T>(label, code, new Endeme(Set, endeme), value)); } // ------+

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- AddToIndex -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="guid"></param>
    //    /// <param name="element"></param>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    private void AddToIndex(Guid guid, GenEndemeItem_donotUse<T> element)
    //    {
    //        string label = element.ItemLabel;
    //        if (!_labelIdx.ContainsKey(label))
    //            _labelIdx.Add(label,new List<Guid>());
    //        _labelIdx[label].Add(guid);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- CopyAll -->
    //    /// <summary>
    //    ///      Makes a copy preserving the keys
    //    /// </summary>
    //    /// <returns></returns>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public GenEndemeList_doNotUse<T> CopyAll()
    //    {
    //        GenEndemeList_doNotUse<T> list = new GenEndemeList_doNotUse<T>(Name, this.Set);
    //        foreach (Guid key in this)
    //            list.Add(key, this[key].Copy());
    //        if (!list.Sane) throw new NullReferenceException("GenEndemeList '"+list.Name+"' is insane.");
    //        return list;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- DistinctEndemes -->
    //    /// <summary>
    //    ///      Returns a list of completely distinct endemes
    //    /// </summary>
    //    /// <returns></returns>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public List<Endeme> DistinctEndemes()
    //    {
    //        List<Endeme> list = new List<Endeme>();
    //        foreach (GenEndemeItem_donotUse<T> el in _list.Values)
    //            if (!list.Contains(el.ItemEndeme))
    //                list.Add(el.ItemEndeme);
    //        return list;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- GetFromCode -->
    //    /// <summary>
    //    ///      This is jsut a stopgap for now, it needs an index to run fast
    //    /// </summary>
    //    /// <param name="code"></param>
    //    /// <returns></returns>
    //    /// <remarks>TODO: fix me, need to build a code index or something</remarks>
    //    public List<GenEndemeItem_donotUse<T>> GetFromCode(string code)
    //    {
    //        List<GenEndemeItem_donotUse<T>> list = new List<GenEndemeItem_donotUse<T>>();
    //        foreach (GenEndemeItem_donotUse<T> el in _list.Values)
    //        {
    //            GenEndemeItem_donotUse<T> found = GenEndemeItem_donotUse<T>.Empty;
    //            if (el.ItemCode == code)
    //            {
    //                found = el;
    //                list.Add(found);
    //            }
    //        }

    //        return list;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Insert -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="ord"></param>
    //    /// <param name="element"></param>
    //    /// <returns></returns>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public GenEndemeItem_donotUse<T> Insert(int ord, GenEndemeItem_donotUse<T> element)
    //    {
    //        Guid guid = Guid.NewGuid();
    //        _list.Add(guid, element);
    //        _order.Insert(ord, guid);
    //        AddToIndex(guid, element);

    //        if (!Sane) throw new NullReferenceException("GenEndemeList '"+Name+"' is insane.");
    //        return element;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- MostLike -->
    //    /// <summary>
    //    ///      Returns the element most similar to the input endeme using the Match algorithm
    //    /// </summary>
    //    /// <param name="en"></param>
    //    /// <returns></returns>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public GenEndemeItem_donotUse<T> MostLike(Endeme en)
    //    {
    //        GenEndemeList_doNotUse<T> orderedList = this.OrderBy(en);
    //        if (orderedList._Count == 0) return null;
    //        GenEndemeItem_donotUse<T> element     = orderedList[orderedList._Count-1];
    //        GenEndemeItem_donotUse<T> el2         = this[element.ItemKey];
    //        el2.TempMatch = element.TempMatch;
    //        return el2;
    //    }

    //    public GenEndemeItem_donotUse<T> ExactlyLike(Endeme en)
    //    {
    //        int count = 0;
    //        GenEndemeItem_donotUse<T> item = null;
    //        for (int i = 0; i < this._Count; ++i)
    //        {
    //            if (this[i].ItemEndeme == en)
    //            {
    //                count++;
    //                item = this[i];
    //            }
    //        }
    //        if (count > 1) throw new Exception("boom");
    //        return item;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- OrderBy -->
    //    /// <summary>
    //    ///      Orders the elements in the list using the 'NewMatch' algorithm (produces a new list)
    //    /// </summary>
    //    /// <param name="sortEndeme"></param>
    //    /// <returns></returns>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public GenEndemeList_doNotUse<T> OrderBy(Endeme sortEndeme)
    //    {
    //        // --------------------------------------------------------------------------
    //        //  Put the keys in order, preserving the keys
    //        // --------------------------------------------------------------------------
    //        GenEndemeList_doNotUse<T> toOrder = this.CopyAll();
    //        foreach (Guid key in toOrder)
    //        {
    //            toOrder[key].TempMatch = sortEndeme.Match(toOrder[key].ItemEndeme);
    //            // TODO: change this to Match1
    //        }


    //        // --------------------------------------------------------------------------
    //        //  Order the list by match using Linq
    //        // --------------------------------------------------------------------------
    //        //List<Guid> orderedKeys = toOrder._list.OrderBy(x => x.Value.Match).Select(x => x.Key).ToList();
            

    //        // --------------------------------------------------------------------------
    //        //  Order the list by match without Linq
    //        // --------------------------------------------------------------------------
    //        Dictionary<Guid,double> quantRaw = new Dictionary<Guid,double>();
    //        for (int i = 0; i < toOrder._Count; ++i)
    //            quantRaw.Add(toOrder[i].ItemKey, toOrder[i].TempMatch);
    //        SortedDictionary<Guid,double> raw = new SortedDictionary<Guid,double>(quantRaw);
    //        List<Guid> orderedKeys = new List<Guid>();
    //        List<double> values = new List<double>(raw.Values);
    //        values.Sort();
    //        for (int i = 0; i < values.Count; ++i)
    //        {
    //            Guid g = Guid.Empty;
    //            foreach (Guid key in raw.Keys)
    //            {
    //                if (raw[key] == values[i] && raw[key] > 0)
    //                    g = key;
    //            }
    //            if (g != Guid.Empty)
    //            {
    //                orderedKeys.Add(g);
    //                raw.Remove(g);
    //            }
    //        }


    //        // --------------------------------------------------------------------------
    //        //  Put the list in order, preserving the keys
    //        // --------------------------------------------------------------------------
    //        GenEndemeList_doNotUse<T> ordered = new GenEndemeList_doNotUse<T>(Name, toOrder.Set);
    //        foreach (Guid key in orderedKeys)
    //        {
    //            ordered.Add(key, toOrder[key]);
    //        }

    //        if (!Sane) throw new NullReferenceException("GenEndemeList '"+Name+"' is insane.");
    //        return ordered;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Pause -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    private void Pause()
    //    {
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Remove -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="guid"></param>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public void Remove(Guid guid)
    //    {
    //        string label = _list[guid].ItemLabel;

    //        List<Guid> guids = _labelIdx[label];
    //        guids.Remove(guid);
    //        if (guids.Count == 0)
    //            _labelIdx.Remove(label);

    //        _order.Remove(guid);
    //        _list.Remove(guid);
    //        if (!Sane) throw new NullReferenceException("GenEndemeList '"+Name+"' is insane.");
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ToString -->
    //    /// <summary>
    //    ///       Determines whether all elements have unique endemes
    //    /// </summary>
    //    /// <returns></returns>
    //    /// <remarks>
    //    ///       TODO: add in the possibility that there may be more than one endeme set
    //    ///       alpha code
    //    /// </remarks>
    //    public bool UniquenessTest()
    //    {
    //        Dictionary<string,int> tally = new Dictionary<string,int>(_list.Count);
    //        for (int i = 0; i < _order.Count; ++i)
    //        {
    //            GenEndemeItem_donotUse<T> en = this[i];
    //            string key = en.ItemEndeme.ToString();
    //            if (!tally.ContainsKey(key))
    //               tally.Add(key, 0);
    //            tally[key]++;
    //        }

    //        if (tally.Count > _labelIdx.Count)  throw new ApplicationException("Missing item in GenEndeme list index");
    //        if (tally.Count < _labelIdx.Count)  throw new ApplicationException("Extra item in GenEndeme list index");

    //        return (_labelIdx.Count == _list.Count);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="enSet"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    internal bool ContainsSet(EndemeSet enSet)
    //    {
    //        foreach (Guid key in _list.Keys)
    //        {
    //            if (enSet == _list[key].ItemEndeme.EnSet)
    //                return true;
    //        }
    //        return false;
    //    }
    //}
}
