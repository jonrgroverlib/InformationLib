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
using System;                         // for Enum, Type, Random
using System.Collections;             // for Hashtable
using System.Collections.Generic;     // for List<>
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- IChange -->
    /// <summary>
    /// 
    /// </summary>
    public interface IChange
    {
        void CheckChanges(EnumList change, object entity);
        void MakeChanges(EnumList change, object entity);
    }


    // --------------------------------------------------------------------------------------------
    /// <!-- EnumList -->
    /// <summary>
    ///      The EnumList class manages a list of values of a particuar enumerated type
    ///      Usage:
    ///      The EnumList class can be used to manage a list of changes to be made to an HL7 XML
    /// </summary>
    /// <remarks>deprecated, roll this functionality into the endeme folder</remarks>
    public class EnumList
    {
        // ----------------------------------------------------------------------------------------
        //  Data members
        // ----------------------------------------------------------------------------------------
        private Type _enumType;        // the subject enumerated type
        private List<Enum> _items;     // A list of items from the enumeration
        private List<string> _values;  // What it was changed to


        // ----------------------------------------------------------------------------------------
        /// <!-- EnumList indexer -->
        /// <summary>
        ///      Gets or sets the item in the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Enum this[int index]
        {
            get { return (Enum)_items[index]; }
            set { Store(index, value); }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- EnumList constructor -->
        /// <summary>
        ///      Hide default constructor
        /// </summary>
        private EnumList()
        {
            _items  = new List<Enum>();
            _values = new List<string>();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- EnumList constructor -->
        /// <summary>
        ///      List is useless without an enumeration list to work with
        /// </summary>
        /// <param name="enumType"></param>
        public EnumList(Type enumType)
        {
            _enumType = enumType;
            _items    = new List<Enum>();
            _values   = new List<string>();
        }


        // ----------------------------------------------------------------------------------------
        //  Brief methods
        // ----------------------------------------------------------------------------------------
        public int  Count  { get { return _items.Count; } }
        public int  Length { get { return Enum.GetValues(_enumType).Length; } }
        public Enum EnumObject(int num)     { return EnumObject(num.ToString()); }
        public Enum EnumObject(string name) { return (Enum)Enum.Parse(_enumType, name); }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateOrderedList -->
        /// <summary>
        ///      Fills the enum list with a list of items in the order of the enum
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public EnumList CreateOrderedList(int count)
        {
            int maxCount = Enum.GetValues(_enumType).Length;
            count = Math.Min(count, maxCount);
            for (int i = 0; i < count; ++i)
                this[i] = EnumObject(i);
            return this;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateRandomList -->
        /// <summary>
        ///      Fills the enum list with a random list of items
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public EnumList CreateRandomList(Random r, int count)
        {
            //Random r = RandomSource.New().Random;


            int maxCount = Enum.GetValues(_enumType).Length;
            count = Math.Min(count, maxCount);
            for (int i = 0; i < count; ++i)
                this[i] = RandomItem(r);
            return this;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- InList -->
        /// <summary>
        ///      Determines whether a item is already in the list
        /// </summary>
        /// <param name="chg"></param>
        /// <returns></returns>
        public bool InList(Enum chg)
        {
            bool found = false;
            foreach (Enum c in _items)
                if (chg.Equals(c)) { found = true; break; }
            return found;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ItemsLeft -->
        /// <summary>
        ///      When the list is fairly full it makes sense to work from the ones not yet used
        /// </summary>
        /// <returns></returns>
        public List<Enum> ItemsLeft()
        {
            List<Enum> list = new List<Enum>();
            Hashtable hash = new Hashtable();


            int count = Enum.GetValues(_enumType).Length;
            for (int i = 0; i < count; ++i)
                hash[EnumObject(i).ToString()] = InList(EnumObject(i));
            foreach (string name in hash.Keys)
                if (!(bool)hash[name]) list.Add(EnumObject(name));
            return list;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Matches -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Matches(int num, object obj)
        {
            string expectedValue = _values[num];
            string actualValue = Str(obj, "");
            return (expectedValue == actualValue);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- RandomItem -->
        /// <summary>
        ///      
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Enum RandomItem(Random r)
        {
            Enum chg;
            if (Count * 4 > Length * 3)
            {
                List<Enum> list = ItemsLeft();
                chg = list[r.Next(Length - Count)];
            }
            else
            {
                chg = RandomItem_simple(r);
                for (int i = 0; InList(chg) && i < 100; ++i)
                    chg = RandomItem_simple(r);
            }
            return chg;
        }
        private Enum RandomItem_simple(Random r)
        {
            return EnumObject(r.Next(Enum.GetValues(_enumType).Length));
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Record -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="obj"></param>
        public void Record(int num, object obj)
        {
            _values[num] = Str(obj, "");
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Bump -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bump"></param>
        /// <param name="maxWork"></param>
        private void Bump(int bump, int maxWork)
        {
            for (int i = 0; _items.Count <= bump && i < maxWork; ++i)
            {
                _items.Add(EnumObject(0));
                _values.Add(null);
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Store -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void Store(int index, Enum value)
        {
            Bump(index, 128);
            _items[index] = value;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Str -->
        /// <summary>
        ///      Converts an object to a string defaulting as specified if it is a null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        public static string Str(object obj, string defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
                return defaultValue;
            else
                return obj.ToString();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Returns the list of enum items
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            string delim = "";
            foreach (string name in Enum.GetNames(_enumType))
                { str.Append(delim + name); delim = ", "; }
            return str.ToString();
        }

    }
}