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
using InformationLib.Testing;          // for RandomSource
using InformationLib.HardData;        // for 
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Data;                    // for DataSet
using System.Data.Common;             // for DbCommand
using System.Globalization;           // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Generator
{
	// --------------------------------------------------------------------------------------------
	/// <!-- LabelType -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code</remarks>
    public enum LabelType
    {
        word      = 0,
        name      = 1,
        fullname  = 2,
        place     = 3,
        placetype = 4,
        female    = 5,
        male      = 6,
        surname   = 7,
        any       = 8,
    }


    // --------------------------------------------------------------------------------------------
    // <!-- DbLabels -->
    /// <summary>
    ///     The DbLabels class retrieves labels once no matter how many times it's instantiated
    /// </summary>
    public class DbLabels
    {
        //private static RichDataTable _rtNames;
        //private static RichDataTable _rtPlaces;
        //private static RichDataTable _rtWords;


        // ----------------------------------------------------------------------------------------
        /// <!-- DbLabels member, constructor and instantiator -->
        private static DbLabels _dbLabels;
        /// <summary>
        ///      This is the singleton constructor
        /// </summary>
        /// <remarks>connection closed by ExecuteDataSet</remarks>
        private DbLabels()
        {
            //Database db = DatabaseFactory.CreateDatabase("CodeWhiteDatabaseConnection");
            //DbCommand dbCommand = db.GetStoredProcCommand("Test.GetLabelData");
            //RichDataTable rt = new RichDataTable(db.ExecuteDataSet(dbCommand).Tables[0]);
            //_labels = rt._ToDataSet("label_source");


            //_rtNames = new RichDataTable(_labels.Tables["TestName"]);
            //_rtWords = new RichDataTable(_labels.Tables["TestWord"]);
            //_rtPlaces = new RichDataTable(_labels.Tables["TestPlace"]);
        }
        /// <summary>
        ///      Instantiates the database label source singleton
        /// </summary>
        public static DbLabels New()
        {
            if (_dbLabels == null)
                _dbLabels = new DbLabels();
            return _dbLabels;
        }


        //// ----------------------------------------------------------------------------------------
        ///// <!-- Labels member and property -->
        ///// <remarks>
        /////      _labels is the resource that needs to be protected in this singleton
        ///// </remarks>
        //DataSet _labels;
        ///// <summary>
        /////      Accesses the database label source
        ///// </summary>
        //public DataSet Labels
        //{
        //    get { return _labels; }
        //}


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateUglyLabel -->
        /// <summary>
        ///      Creates a really ugly letter string including some caps and some spaces
        /// </summary>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string CreateUglyLabel(int lowLength, int highLength)
        {
            if (highLength == int.MaxValue)
                highLength = int.MaxValue - 1;
            Random r = RandomSource.New().Random;
            string str = CreateUglyLabel(r.Next(1, highLength + 1));
            for (int i = 0; str.Length < lowLength && i < 10; ++i)
                str = CreateUglyLabel(r.Next(1, highLength + 1));
            return str;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateUglyLabel -->
        /// <summary>
        ///      Creates a really ugly letter string including some caps and some spaces
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateUglyLabel(int length)
        {
            Random r = RandomSource.New().Random;
            string str = "";


            for (int i = 0; i < length; ++i)
            {
                int num;
                switch (r.Next(i + 1))
                {
                    case 0: num = r.Next(65, 90); break;
                    case 4: num = 32; break;
                    case 10: num = 95; break;
                    default: num = r.Next(98, 123); break;
                }
                char cha = Convert.ToChar(num);
                str += cha;
            }
            return str;
        }


        //public static string CreatePrettyLabel(int length, LabelType type)
        //{
        //    return New().CreatePrettyLabel_nonstatic(length, type);
        //}


        // ----------------------------------------------------------------------------------------
        /// <!-- CreatePrettyLabel -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreatePrettyLabel(int length, LabelType type)
        {
       //     New();
            string label;


            switch (type)
            {
                case LabelType.name   : label = "Jon Grover"; break; // DrawLabel(length, _rtNames);  break;
                case LabelType.place  : label = "Indianapolis"; break; // DrawLabel(length, _rtPlaces); break;
                case LabelType.word   : label = "courage"; break; // DrawLabel(length, _rtWords);  break;
                default:
                    switch (RandomSource.New().Random.Next(3))
                    {
                        case 0: label = "Jon Grover"; break; // DrawLabel(length, _rtNames);  break;
                        case 1: label = "Indianapolis"; break; // DrawLabel(length, _rtPlaces); break;
                        default: label = "courage"; break; // DrawLabel(length, _rtWords);  break;
                    }
                    break;
            }


            return label;
        }


        //// ----------------------------------------------------------------------------------------
        ///// <!-- DrawLabel -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="length"></param>
        ///// <param name="r"></param>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //private static string DrawLabel(int length, RichDataTable dt)
        //{
        //    Random r = RandomSource.New().Random;
        //    string label;


        //    // --------------------------------------------------------------------------
        //    //  A nice random label
        //    // --------------------------------------------------------------------------
        //    label = dt._Str(r.Next(dt.Count), 0, CreateUglyLabel(length));


        //    // --------------------------------------------------------------------------
        //    //  A few more tries if it was too long
        //    // --------------------------------------------------------------------------
        //    for (int i = 0; i < 10 && label.Length > length; ++i)
        //        label = dt._Str(r.Next(dt.Count), 0, CreateUglyLabel(length));
        //    if (label.Length > length)
        //        label = CreateUglyLabel(length);


        //    label = new CultureInfo("en").TextInfo.ToTitleCase(label.ToLower());
        //    return label;
        //}


    }
}
