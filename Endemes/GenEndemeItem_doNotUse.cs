//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
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
using System.Collections.ObjectModel; // for
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    //// --------------------------------------------------------------------------------------------
    ///// <!-- GenEndemeItem -->
    ///// <summary>
    /////      The GenEndemeItem class contains element in an GenEndemeList
    ///// </summary>
    ///// <remarks>
    /////      Do not use - the reason why is that this runs counter to my theory of data semantics
    ///// </remarks>
    //public class GenEndemeItem_donotUse<T> : Collection<T>
    //{
    //    // --------------------------------------------------------------------------------------
    //    //  Members
    //    // --------------------------------------------------------------------------------------
    //    public Endeme ItemEndeme { get; set; } // make this the parent class?
    //    public Guid   ItemKey    { get; set; }
    //    public string ItemCode   { get; set; }
    //    public string ItemLabel  { get; set; }
    //    public double TempMatch  { get; set; } // ephemeral data regarding matching of the GenEndemeItem with another endeme
    //    public GenEndemeList_doNotUse<T> ParentList { get; set; } // a reference to the list that this is part of


    //    // --------------------------------------------------------------------------------------
    //    //  Properties
    //    // --------------------------------------------------------------------------------------
    //    public        EndemeSet        Set   { get { return ItemEndeme.EnSet; } set { ItemEndeme.EnSet = value; } }
    //    public static GenEndemeItem_donotUse<T> Empty { get { return new GenEndemeItem_donotUse<T>();                             } }

    //    // --------------------------------------------------------------------------------------
    //    //  Constructors
    //    // --------------------------------------------------------------------------------------
    //    public  GenEndemeItem_donotUse(string label,                Endeme endeme, T value              ) { Init(label, ""  , endeme                 , value, 0.0  ); }
    //    public  GenEndemeItem_donotUse(string label, EndemeSet set, string endeme, T value              ) { Init(label, ""  , new Endeme(set, endeme), value, 0.0  ); }
    //    private GenEndemeItem_donotUse(string label,                Endeme endeme, T value, double match) { Init(label, ""  , endeme                 , value, match); }
    //    public  GenEndemeItem_donotUse(string label, string code  , Endeme endeme, T value              ) { Init(label, code, endeme                 , value, 0.0  ); }
    //    public  GenEndemeItem_donotUse(string label, string code  , Endeme endeme, T value, double match) { Init(label, code, endeme                 , value, match); }
    //    private GenEndemeItem_donotUse(                                                                 ) { Init(""   , ""  , Endeme.Empty                  , 0.0  ); }


    //    // --------------------------------------------------------------------------------------
    //    //  Short methods
    //    // --------------------------------------------------------------------------------------
    //    internal GenEndemeItem_donotUse<T> Copy() { return new GenEndemeItem_donotUse<T>(ItemLabel, ItemCode, ItemEndeme.Copy(), this[0], TempMatch); }


    //    // --------------------------------------------------------------------------------------
    //    /// <!-- Init -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="label"></param>
    //    /// <param name="endeme"></param>
    //    /// <param name="value"></param>
    //    /// <param name="match"></param>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public void Init(string label, string code, Endeme endeme, T value, double match)
    //    {
    //        Init(label, code, endeme, match);
    //        Init(value);
    //    }
    //    private void Init(string label, string code, Endeme endeme, double match)
    //    {
    //        ItemEndeme = endeme;
    //        ItemLabel  = label ;
    //        ItemCode   = code  ;
    //        TempMatch  = match ;
    //    }
    //    private void Init(T value)
    //    {
    //        if (value != null && Regex.IsMatch(value.GetType().Name, "EndemeItem"))
    //            throw new AggregateException("You may not put a "+value.GetType().Name+" inside a GenEndemeItem.");
    //        switch (this.Count)
    //        {
    //            case 0  : this.Add(value);                   break;
    //            case 1  : this[0] = value;                   break;
    //            default : this[0] = value; this.RemoveAt(1); break;
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ToString -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    /// <remarks>beta code - nearly production ready</remarks>
    //    public override string ToString()
    //    {
    //        string str = "";
    //        if (ItemEndeme == null)
    //        {
    //            str += "endeme item has null endeme";
    //        }
    //        else
    //        {
    //            if (ItemEndeme.EnSet == null)
    //                 str += "?";
    //            else str += ItemEndeme.EnSet.Label;
    //            str += ":" + ItemEndeme.ToString();
    //        }
    //        str += " ("+Regex.Replace(TempMatch.ToString(),"^(....).*$", "$1")+")";
    //        str += " " + ItemLabel;
    //        if (this[0] == null) str = ":?"; else str += ":" + this[0].ToString();
    //        return "[" + str + "]";
    //    }
    //}
}

