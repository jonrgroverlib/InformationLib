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
using InformationLib.SoftData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationLib.Micro
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Array2dChar -->
    /// <summary>
    /// 
    /// </summary>
    public class Array2dChar
    {
        // ----------------------------------------------------------------------------------------
        //  Members, properties, accessors
        // ----------------------------------------------------------------------------------------
        private char[,] _array;
        public  int     Height          { get { return _array.GetUpperBound(0) + 1;                    } }
        public  int     Width           { get { return _array.GetUpperBound(1) + 1;                    } }
        public  char this[int y, int x] { get { if (InRange(x,y)) return _array[y,x]; else return ' '; }
                                          set { if (InRange(x,y)) _array[y,x] = value;                 } }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public Array2dChar(int height, int width)
        {
            _array = new char[height,width];
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Compose -->
        /// <summary>
        ///      Composes a string version of the designated area of the array
        /// </summary>
        /// <param name="vert">the vertical range of the area to  be composed</param>
        /// <param name="horiz">the horizonatal range of the area to be composed</param>
        /// <returns></returns>
        public string Compose(Range vert, Range horiz)
        {
            StringBuilder str = new StringBuilder();
            string delim = "";
            for (int i = vert.Lo; i < vert.Hi; ++i)
            {
                str.Append(delim);
                str.Append(delim + Compose(i, horiz));
                delim = "\r\n";
                str.Append(delim + Compose(i, horiz));
            }

            return str.ToString();
        }
        /// <summary>Composes a string version of the entire array</summary>
        public string Compose() { return Compose(new Range(0, Height), new Range(0, Width )); }

        // ----------------------------------------------------------------------------------------
        /// <!-- Compose -->
        /// <summary>
        ///      Composes one line
        /// </summary>
        /// <param name="_array"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private string Compose(int line, Range ew)
        {
            StringBuilder str = new StringBuilder();
            string charDelim = "";
            for (int j = ew.Lo; j < ew.Hi; ++j)
            {
                str.Append(charDelim + _array[line,j] + _array[line,j] + _array[line,j]);
                charDelim = " ";
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Fill -->
        /// <summary>
        /// 
        /// </summary>
        public void Fill(char L, char R, char U, char D, double percent)
        {
            Random r = new Random();

            int maxRow = _array.GetUpperBound(0);
            int maxCol = _array.GetUpperBound(1);
            for (int i = 0; i < maxRow+1; ++i)
                for (int j = 0; j < maxCol+1; ++j)
                    _array[i,j] = (char)(r.Next(65, 87));


            for (int i = 0; i < maxRow+1; ++i)
            {
                if (100 * percent > r.Next(100)) _array[i,0] = L;
                if (100 * percent > r.Next(100)) _array[i,maxCol] = R;
            }


            for (int j = 0; j < maxCol+1; ++j)
            {
                if (100 * percent > r.Next(100)) _array[0,j] = U;
                if (100 * percent > r.Next(100)) _array[maxRow,j] = D;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Friends -->
        /// <summary>
        ///      
        /// </summary>
        /// <param name="c"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        internal int Friends(char c, int X, int Y)
        {
            int value = 0;

            char f0 = this[Y+1,X  ];
            char f1 = this[Y-1,X  ];
            char f2 = this[Y  ,X+1];
            char f3 = this[Y  ,X-1];

            if (c == f0) value++;
            if (c == f1) value++;
            if (c == f2) value++;
            if (c == f3) value++;

            return value;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Friends -->
        /// <summary>
        ///      Returns number of matches (Friends) around the second XY of the caracter at the first X,Y
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <returns></returns>
        public int Friends(int X1, int Y1, int X2, int Y2)
        {
            char c = this[Y1,X1];
            return this.Friends(c,X2,Y2);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InRange -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool InRange(int x, int y)
        {
            if (0 <= x && x < Width &&
                0 <= y && y < Height) return true;
            else return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Swap -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void Swap(int x1, int y1, int x2, int y2)
        {
            if (InRange(x1,y1) && InRange(x2,y2))
            {
                char temp = this[y2,x2];
                this[y2,x2] = this[y1,x1];
                this[y1,x1] = temp;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AsciiArt -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AsciiArt(string joinChar, char emptyLetter)
        {
            string str   = "";
            string delim = "";
            string join  = "";


            for (int j = 0; j < Height; ++j)
            {
                str += delim;
                join = "";
                for (int i = 0; i < Width; ++i)
                {
                    char c = this[j,i];
                    if (c == ' ')
                        c = emptyLetter;
                    str += join + c;
                    join = joinChar;
                }

                delim = "\r\n";
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str   = "";
            string delim = "";


            for (int j = 0; j < Height; ++j)
            {
                str += delim;
                for (int i = 0; i < Width; ++i) { str += this[j,i]; }
                delim = "\r\n";
            }

            return str;
        }
    }
}
