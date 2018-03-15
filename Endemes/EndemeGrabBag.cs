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
using InformationLib.Testing;         // for RandomSource
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeGrabBag -->
    /// <summary>
    ///      The EndemeGrabBag provides a random selection bag based on an endeme without replacement
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class EndemeGrabBag
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private List<char> _bag;
        private Endeme     _endeme;
        private int        _size;


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeGrabBag(Endeme endeme, int size)
        {
            _endeme = endeme;
            _size   = size;
            _bag    = SelectionBag(endeme, size);
            if (Count > OriginalCount)
                throw new Exception("boom");
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public int Count         { get { if (_bag == null) return 0; else return _bag.Count; } }
        public int OriginalCount { get { return _size;      } }


        // ----------------------------------------------------------------------------------------
        /// <!-- SelectionBag -->
        /// <summary>
        ///      Places a bunch of marbles (chars) in a bag for random selection
        /// </summary>
        /// <param name="size">number of marbles in bag (try a multiple of 8 <= 120)</param>
        /// <returns></returns>
        private List<char> SelectionBag(Endeme e, int size)
        {
            Random r = RandomSource.New().Random;
            if (this.Count > this.OriginalCount)
                throw new Exception("SelectionBag");


            // ----------------------------------------------------------------
            //  Define number of each kind of marble (character)
            // ----------------------------------------------------------------
            char[] endeme = e.ToCharArray();
            string str = "";
            switch (size)
            {
                case 128: str = "@>=;:98765544332221111"; break;
                case 120: str = "@><;987665443322221111"; break;
                case 112: str = "?=;:987654433322211111"; break;
                case 104: str = "><:9876654433222211111"; break;
                case  96: str = "=;:9776544333222111110"; break;
                case  88: str = "<:98765544332221111110"; break;
                case  80: str = ";987665443322221111110"; break;
                case  72: str = ":877654433322211111100"; break;
                case  64: str = "9876544333222111111000"; break;
                case  60: str = "8766544332222111111000"; break;
                case  56: str = "8765543332221111110000"; break;
                case  52: str = "7665443322221111110000"; break;
                case  48: str = "7655433322211111100000"; break;
                case  44: str = "6554433222211111100000"; break;
                case  40: str = "6544333222111111000000"; break;
                case  36: str = "5544332221111110000000"; break;
                case  32: str = "5433322221111110000000"; break;
                case  28: str = "4433222211111100000000"; break;
                case  24: str = "4332222111111000000000"; break;
                case  20: str = "3322221111110000000000"; break;
                case  16: str = "3222111111100000000000"; break;
                case  14: str = "2222111111000000000000"; break;
                case  12: str = "2221111110000000000000"; break;
                case  10: str = "2211111100000000000000"; break;
                case   8: str = "2111111000000000000000"; break;
                case   7: str = "2111110000000000000000"; break;
                case   6: str = "1111110000000000000000"; break;
                case   5: str = "1111100000000000000000"; break;
                case   4: str = "1111000000000000000000"; break;
                case   3: str = "1110000000000000000000"; break;
                case   2: str = "1100000000000000000000"; break;
                case   1: str = "1000000000000000000000"; break;
                default : str = "";
                    Dictionary<char, int> cha = new Dictionary<char, int>(22);
                    foreach (char c in endeme)     cha.Add(c, 0);
                    for (int i = 0; i < size; ++i) cha[e.RandomLetter(0.89)]++;
                    var query = from ch in cha orderby cha.Values descending select cha;
                    str = "";
                    foreach (var item in query)
                    {
                        str = str + item.ToString();
                    }
                    break; // TODO: build the function that approximates these series (.89 * one before)
            }


            // ----------------------------------------------------------------
            //  Put marbles (characters) in bag
            // ----------------------------------------------------------------
            List<int>  quantity = StringToNums(str);
            List<char> bag      = new List<char>(size);
            for (int i = 0; i < quantity.Count; ++i)
                for (int j = 0; j < quantity[i]; ++j)
                    if (i < endeme.Length)
                        bag.Add(endeme[i]);
                    else if (endeme.Length > 0)
                        bag.Add(endeme[0]);


            return bag;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- StringToNums -->
        /// <summary>
        ///      Converts a string of digits into a list of those digits: 0123456789:;&lt;=>?
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<int> StringToNums(string str)
        {
            List<char> chars = new List<char>(str.ToCharArray());
            var numbers = from c in chars select (int)c-48;
            List<int> nums = new List<int>();
            foreach (var num in numbers) nums.Add(num);
            return nums;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- DrawLetter -->
        /// <summary>
        ///      Draws a character from the bag
        /// </summary>
        /// <returns></returns>
        public char DrawLetter()
        {
            char c;
            if (_bag.Count > 0)
            {
                Random r = RandomSource.New().Random;
                int idx = r.Next(_bag.Count);
                c = _bag[idx];
                _bag.RemoveAt(idx);
            }
            else
                c = ' ';
            return c;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_bag == null) return "0/0";
            else return "" + _bag.Count + "/" + _size;
        }

    }
}
