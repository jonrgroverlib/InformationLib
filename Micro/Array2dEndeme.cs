//-------------------------------------------------------------------------------------------------
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
//-------------------------------------------------------------------------------------------------
using InformationLib.Endemes;         // for Endeme
using InformationLib.SoftData;        // for TreatAs
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Micro
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Array2dEndeme -->
    /// <summary>
    ///      The Array2dEndeme class contains an array of endemes
    /// </summary>
    public class Array2dEndeme
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private Endeme[,] _array;
        public int    Height                 { get { return _array.GetUpperBound(0) + 1; } }
        public int    Width                  { get { return _array.GetUpperBound(1) + 1; } }


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        public Endeme this[int row, int col] {
            get { if (InRange(row,col)) return _array[row,col]; else return new Endeme(); }
            set { if (InRange(row,col)) _array[row, col] = value; }
        }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        /// <summary></summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="template">a starter endeme to make copies of, a null here may help avoid an out of memory exception</param>
        public Array2dEndeme(int height, int width, Endeme template)
        {
            _array = new Endeme[height,width];
            if (template != null)  // a null here may halp avoid an out of memory exception
            {
                for (int i = 0; i < height; ++i)
                    for (int j = 0; j < width; ++j)
                        _array[i, j] = template.Copy();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddValue -->
        /// <summary>
        ///      Sets a raw value for a particular endeme character
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="c"></param>
        /// <param name="value"></param>
        public void AddValue(int row, int col, char c, int value)
        {
            if (InRange(row, col))
            {
                Endeme e = _array[row, col];
                e.Quant.Raw[c] += value;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Compose -->
        /// <summary>
        ///      Composes a string version ofthe entire array
        /// </summary>
        /// <returns></returns>
        public string Compose()
        {
            StringBuilder str = new StringBuilder();
            string lineDelim = "";
            for (int i = 0; i < _array.GetUpperBound(0)+1; ++i)
            {
                str.Append(lineDelim);
                str.Append(lineDelim + Compose(i));
                lineDelim = "\r\n";
                str.Append(lineDelim + Compose(i));
            }

            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Compose -->
        /// <summary>
        ///      Composes one line
        /// </summary>
        /// <param name="_array"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private string Compose(int line)
        {
            StringBuilder str = new StringBuilder();
            string charDelim = "";
            for (int j = 0; j < Width; ++j)
            {
                str.Append(charDelim
                    + _array[line,j][0]
                    + _array[line,j][1]
                    + _array[line,j][2]);
                charDelim = " ";
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InRange -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool InRange(int row, int col)
        {
            if (row < 0 || col < 0) return false;
            if (row < Height && col < Width) return true;
            else return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CookEndemes -->
        /// <summary>
        ///      Uses the values tallies in the endemes to figure out what the endemes are
        /// </summary>
        public void CookEndemes()
        {
            int height = _array.GetUpperBound(0) + 1;
            int width  = _array.GetUpperBound(1) + 1;
            for (int row = 0; row < height; ++row)
                for (int col = 0; col < width; ++col)
                    if (_array[row,col] != null)
                        _array[row,col].CookEndeme();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      The first major test is whether height and width are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            if (!(obj is Array2dEndeme)) return false;
            Array2dEndeme that = (Array2dEndeme)obj;
            if (that.Height != Height) return false;
            if (that.Width != Width) return false;
            bool equals = true;
            for (int i = 0; equals && i < Height; ++i)
            {
                for (int j = 0; equals && j < Width; ++j)
                {
                    Endeme e1 = this[i, j];
                    Endeme e2 = that[i, j];
                    equals = e1.Equals(e2);
                }
            }
            return equals;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetHashCode -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Height * Width;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PickEndeme -->
        /// <summary>
        ///      Picks an endeme for a league map letter from a large spot array
        /// </summary>
        /// <param name="spot"></param>
        /// <param name="r"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="squareSize"></param>
        /// <returns></returns>
        public Endeme PickEndeme(Random r, int lat, int lon, int squareSize)
        {
            // --------------------------------------------------------------------------
            //  Do some calculations
            // --------------------------------------------------------------------------
            int U = (int)((2.0 * lat) / squareSize); // distance from north
            int distD = lat - squareSize * U;
            int L = (int)((2.0 * lon) / squareSize); // the left spot
            int distR = lon - squareSize * L;


            // --------------------------------------------------------------------------
            //  Pick the endeme
            // --------------------------------------------------------------------------
            if (r.Next(squareSize) < distD) U++; // maybe switch to the spot below based on distance from above spot
            if (r.Next(squareSize) < distR) L++; // maybe switch to the spot right based on distance from left spot
            Endeme endeme = this[U, L];
            return endeme;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiArt -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellHeight"></param>
        /// <param name="cellWidth"></param>
        /// <returns></returns>
        public string AsciiArt(int cellHeight, int cellWidth)
        {
            Array2dChar cha = new Array2dChar(Height*(cellHeight+1)+1,Width*(cellWidth+1)+1);

            // --------------------------------------------------------------------------
            //  Horizontal lines
            // --------------------------------------------------------------------------
            for (int y = 0; y < cha.Height; ++y)
            {
                int mod = y % (cellHeight + 1);
                if (mod == 0) for (int x = 0; x < cha.Width; ++x) cha[y, x] = '-';
                else          for (int x = 0; x < cha.Width; ++x) cha[y, x] = '|';
            }


            // --------------------------------------------------------------------------
            //  Vertical lines
            // --------------------------------------------------------------------------
            for (int y = 0; y < cha.Height; ++y)
                for (int x = 0; x < cha.Width; ++x)
                {
                    int mod = y % (cellHeight + 1) + x % (cellWidth + 1);
                    if (mod == 0)
                        cha[y, x] = '+';
                }


            // --------------------------------------------------------------------------
            //  Endeme array letters
            // --------------------------------------------------------------------------
            for (int y = 0; y < this.Height; ++y)
                for (int x = 0; x < this.Width; ++x)
                {
                    Endeme en = this[y,x];
                    int i = y * (cellHeight + 1) + 1;
                    int j0 = x * (cellHeight + 1) + 1;


                    int j = j0;  cha[i,j] = TreatAs.CharValue(en[0], ' ', ' ');
                    j++;         cha[i,j] = TreatAs.CharValue(en[1], ' ', ' ');
                    j = j0; i++; cha[i,j] = TreatAs.CharValue(en[2], ' ', ' ');
                    j++;         cha[i,j] = TreatAs.CharValue(en[3], ' ', ' ');
                }



            string str  = "" ;
            string line = "+";
            for (int j = 0; j < this.Width; ++j)
                line += "--+";

            for (int i = 0; i < this.Height; ++i)
            {
                str += line;
                string delim = "\r\n|";
                for (int j = 0; j < this.Width; ++j)
                {
                    str += delim + "AB";
                    delim = "|";
                }
                str += delim;


                line = "\r\n+";
                for (int j = 0; j < this.Width; ++j)
                    line += "--+";
            }
            str += line;
            //return str;

            return cha.AsciiArt("", ' ');
        }
    }
}
