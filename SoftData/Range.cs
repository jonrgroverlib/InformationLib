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
using System;                         // for Random
using System.Collections.Generic;     //
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Range -->
    /// <summary>
    ///      The Range class handles double and int range information
    /// </summary>
    /// <remarks>beta code - might be production ready</remarks>
    public class Range
    {

        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double Highest       { get { return      _highest; } set { _highest = value; } }  private double _highest;
        public int     Hi            { get { return (int)_highest; }                           }
        public double Lowest        { get { return      _lowest;  } set { _lowest  = value; } }  private double _lowest;
        public int     Lo            { get { return (int)_lowest;  }                           }
        public bool    LowInclusive  { get; set; }
        public bool    HighInclusive { get; set; }


        // ----------------------------------------------------------------------------------------
        ///  Range constructors
        // ----------------------------------------------------------------------------------------
        public Range()                                /* 0 - 100% */ { Init(0.0  , 100.0, true, false); }
        public Range(double highest)                 /* 0 to n-1 */ { Init(0.0  , highest, true, false); }
        public Range(double lowest, double highest) /* x to y-1 */ { Init(lowest, highest, true, false); }
        public Range(double lowest, double highest, bool lowInclusive, bool highIncusive)
                                                                     { Init(lowest, highest, lowInclusive, highIncusive); }


        // ----------------------------------------------------------------------------------------
        //  Short Methods and Properties
        // ----------------------------------------------------------------------------------------
        public int     Max     { get { if (HighInclusive) return Hi     ; else return Hi      - 1;          } }
        public int     Min     { get { if (LowInclusive ) return Lo     ; else return Lo      + 1;          } }
        public double Maximum { get { if (HighInclusive) return Highest; else return Highest - 0.0000001; } } // TODO: fix these
        public double Minimum { get { if (LowInclusive ) return Lowest ; else return Lowest  + 0.0000001; } } // TODO: fix


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateNum -->
        /// <summary>
        ///      Creates a double number in the range specified
        /// </summary>
        /// <returns></returns>
        public double CreateNum(Random r)
        {
            double num;
            num = CreateNum(r, _lowest, _highest);
            for (int i = 0; !InRange(num) && i < 10; ++i)
                num = CreateNum(r, _lowest, _highest);
            return num;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateNum -->
        /// <summary>
        ///      Creates a double number in a range that is slightly larger than the one specified
        /// </summary>
        /// <param name="lowest"></param>
        /// <param name="highest"></param>
        /// <returns></returns>
        private static double CreateNum(Random r, double lowest, double highest)
        {
            double factor = 1000.0;
            double num;


            if (highest * factor < double.MaxValue && lowest * factor > double.MinValue)
                num = (double)r.Next((int)(lowest * factor), (int)(highest * factor) + 1) / factor;
            else
                num = (double)r.Next((int)lowest, (int)highest);
            return num;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Initializes the object, used by the constructors
        /// </summary>
        /// <param name="lowest"></param>
        /// <param name="highest"></param>
        /// <param name="lowInclusive"></param>
        /// <param name="highIncusive"></param>
        private void Init(double lowest, double highest, bool lowInclusive, bool highIncusive)
        {
            _lowest       = lowest;
            _highest      = highest;
            LowInclusive  = lowInclusive;
            HighInclusive = highIncusive;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InRange -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool InRange(double num)
        {
            if ( LowInclusive  && num <  _lowest )  return false;
            if ( HighInclusive && num >  _highest)  return false;
            if (!LowInclusive  && num <= _lowest )  return false;
            if (!HighInclusive && num >= _highest)  return false;

            return true;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- InRange -->
        /// <summary>
        ///      Returns whether B is in range of A and C are in order  (A <= B < C)
        /// </summary>
        /// <param name="lo"></param>
        /// <param name="mid"></param>
        /// <param name="hi"></param>
        /// <returns></returns>
        public static bool InRange(long lo, long num, long hi) { return (lo <= num && num < hi); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Length -->
        /// <summary>
        ///      Returns the number of integers in the range (could be 1 low in cases of non-integer double ranges)
        /// </summary>
        public int Length { get
        {
            int diff = Hi - Lo;
            if ( LowInclusive &&  HighInclusive) diff++;
            if (!LowInclusive && !HighInclusive) diff--;
            return diff;
        } }


        public double Middle { get
        {
            return Lowest + (Length / 2.0);
        } }


        // ----------------------------------------------------------------------------------------
        /// <!-- SetLow -->
        /// <summary>
        ///      Specifies the low end of the range
        /// </summary>
        /// <param name="strNum"></param>
        /// <param name="isInclusive"></param>
        public void SetLow(string strNum, bool isInclusive)
        {
            if (!double.TryParse(strNum, out _lowest))
                _lowest = 0.0;
            LowInclusive = isInclusive;
        }
        public void SetLow(double num, bool isInclusive)
        {
            _lowest = num;
            LowInclusive = isInclusive;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SetHigh -->
        /// <summary>
        ///      Specifies the high end of the range
        /// </summary>
        /// <param name="strNum"></param>
        /// <param name="isInclusive"></param>
        public void SetHigh(string strNum, bool isInclusive)
        {
            if (!double.TryParse(strNum, out _highest))
                _highest = 0.0;
            HighInclusive = isInclusive;
        }
        public void SetHigh(double num, bool isInclusive)
        {
            _highest = num;
            HighInclusive = isInclusive;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "";
            str += _lowest;
            if (LowInclusive) str += " <= "; else str += " < ";
            str += "x";
            if (HighInclusive) str += " <= "; else str += " < ";
            str += _highest;
            return str;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Wrap -->
        /// <summary>
        ///      Wraps the input number into the range so if the range is 1-10, 12 becomes 2
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Wrap(int value)
        {
            int diff = Length;

            if ( HighInclusive) { while (value >  Hi) value = value - diff; }
            if (!HighInclusive) { while (value >= Hi) value = value - diff; }
            if ( LowInclusive ) { while (value <  Lo) value = value + diff; }
            if (!LowInclusive ) { while (value <= Lo) value = value + diff; }

            return value;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Truncate -->
        /// <summary>
        ///      Truncates the input number into the range so if the range is 1-10, 12 becomes 10
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Truncate(int value)
        {
            if ( HighInclusive) { while (value >  Hi) value = Hi;     }
            if (!HighInclusive) { while (value >= Hi) value = Hi - 1; }
            if ( LowInclusive ) { while (value <  Lo) value = Lo;     }
            if (!LowInclusive ) { while (value <= Lo) value = Lo + 1; }

            return value;
        }


    }
}
