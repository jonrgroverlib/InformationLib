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
using System.Data;                    // for NoNullAllowedException
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Winform
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Plaster -->
    /// <summary>
    ///      The ListItem class lists items, it is designed to go into a BindingList and to be indexed
    /// </summary>
    public class ListItem : IListData
    {
        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public ListItem()
        { _id = ""; _descr = ""; }
        public ListItem(string id, string descr)
        { _id = id;  _descr = descr; }
        public ListItem(string id, string code, string descr)
        { _id = id; _code = code; _descr = descr; }
        public ListItem(string id, string code, string descr, string display)
        { _id = id; _code = code; _descr = descr; _display = display; }


        // ----------------------------------------------------------------------------------------
        /// <!-- Id, Code, Description, Display -->                                                                                                                               /// <summary>Implements ILookupData.ID</summary>
        public string ID          { get { return _id;      } set { if (value == null) throw new NoNullAllowedException();  else  _id      = value; } }  private string _id;   /// <summary>Implements ILookupData.Code</summary>
        public string Code        { get { return _code;    } set { if (value == null) throw new NoNullAllowedException();  else  _code    = value; } }  private string _code; /// <summary>Implements ILookupData.Text</summary>
        public string Descr       { get { return _descr;   } set { if (value == null) throw new NoNullAllowedException();  else  _descr   = value; } }  private string _descr;
        public string Display     { get { return _display; } set { if (value == null) throw new NoNullAllowedException();  else  _display = value; } }  private string _display;


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Converts the list item to a string
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_display))  return _id + " " + _code + ": " + _descr;
            else return _display;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Four point check for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)                          return false;
            if (ReferenceEquals(obj, this))           return true;
            if (!(obj.GetType() == typeof(ListItem))) return false;
            if (((ListItem)obj)._id == this._id)      return true;
            else                                      return false;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- GetHashCode -->
        /// <summary>
        ///      Returns a hash code for the object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

    }
}
