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
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeBalancer -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class EndemeBalancer
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double AvgTgt { get; set; }
        public double StdTgt { get; set; }
        public Endeme Avg    { get; set; }
        public Endeme Std    { get; set; }
        public Endeme Mult   { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeBalancer(double avgTgt, double stdTgt, Endeme avg, Endeme std)
        {
            Avg    = avg;
            AvgTgt = avgTgt;
            Std    = std;
            StdTgt = stdTgt;


            Mult = new Endeme(avg.EnSet);
            foreach (char c in std.Quant.RawKeys)
                Mult.Quant.Raw[c] = StdTgt / Std.Quant.Raw[c];
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Sigmoid -->
        /// <summary>
        ///      Keeps the normalized data within a specific range
        /// </summary>
        /// <param name="num"></param>
        /// <param name="maxRange">maximum possible value (min is 0)</param>
        /// <returns></returns>
        public static double Sigmoid(double num, double maxRange)
        {
            double power = 2.0 - (4*num/maxRange);
            double output = maxRange / (1 + Math.Pow(2.718281828, power));
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Normalize -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        internal double Normalize(double raw, char c)
        {
            double std = Std.Quant.Raw[c];
            double multiplier = StdTgt / std;             // range multiplier or divider
            //double multiplier = StdTgt / Std.Raw[c];    // range multiplier or divider
            Mult.Quant.Raw[c] = multiplier;
            //double multiplier = Mult.Raw[c];            // range multiplier or divider
            double avg = Avg.Quant.Raw[c];
            double zeroed = raw - avg;                    // moves the value close to zero
            double value = zeroed * multiplier + AvgTgt;  // change the range and move close to the average
            return value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Normalize -->
        /// <summary>
        ///      Normalizes the values in the raw array using a multiplier, offset, and sigmoid
        /// </summary>
        /// <param name="endeme"></param>
        public void Normalize(Endeme endeme)
        {
            List<char> keys = new List<char>(endeme.Quant.Raw.Count);
            foreach (char c in endeme.Quant.RawKeys)
                keys.Add(c);


            // --------------------------------------------------------------------------
            //  Normalize the raw data
            // --------------------------------------------------------------------------
            foreach (char c in keys)
            {
                double value = this.Normalize(endeme.Quant.Raw[c], c);
                value = EndemeBalancer.Sigmoid(value, this.AvgTgt * 2);
                endeme.Quant.Raw[c] = value;
            }

            // --------------------------------------------------------------------------
            //  Cook the endeme
            // --------------------------------------------------------------------------
            endeme.CookEndeme();
        }
    }
}
