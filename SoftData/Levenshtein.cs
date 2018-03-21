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
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- LevenshteinMatrix -->
    /// <summary>
    ///      The LevenschteinMatrix class computes a Levenschtein matrix, trace and edit series for
    ///      two strings
    /// </summary>
    /// <remarks>
    ///      From Wikipedia:
    /// 
    ///        int LevenschteinDistance(char s[1..m], char t[1..n])
    ///         {
    ///           // d is a table with m+1 rows and n+1 columns
    ///           declare int d[0..m, 0..n]
    ///          
    ///           for i from 0 to m
    ///             d[i, 0] := i // deletion
    ///           for j from 0 to n
    ///             d[0, j] := j // insertion
    ///          
    ///           for j from 1 to n
    ///           {
    ///             for i from 1 to m
    ///             {
    ///               if s[i] = t[j] then 
    ///                 d[i, j] := d[i-1, j-1]
    ///               else
    ///                 d[i, j] := minimum
    ///                            (
    ///                              d[i-1, j] + 1,  // deletion
    ///                              d[i, j-1] + 1,  // insertion
    ///                              d[i-1, j-1] + 1 // substitution
    ///                            )
    ///             }
    ///           }
    ///          
    ///           return d[m, n]
    ///         }
    ///         
    ///      this is the broken one
    /// </remarks>
    public class Levenshtein
    {
        //// --------------------------------------------------------------------------------------
        ////  Data members
        //// --------------------------------------------------------------------------------------
        //int[,]  _matrix;
        ///// <remarks>
        /////      To do this properly we use a LenvenschteinElement for the matrix to hold whether
        /////      each element is in the trace or not:
        ///// </remarks>
        //bool[,] _tr; // is this cell part of the trace?
        //char[]  _ci;
        //char[]  _cj;
        //LevenschteinTrace _trace;


        //// --------------------------------------------------------------------------------------
        ////  Constructors
        //// --------------------------------------------------------------------------------------
        //public Levenshtein()                         { Calculate(null, null); }
        //public Levenshtein(string strA, string strB) { Calculate(strA, strB); }


        //// --------------------------------------------------------------------------------------
        ///// <!-- Calculate -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="strA"></param>
        ///// <param name="strB"></param>
        //public void Calculate(string strA, string strB)
        //{
        //    CalculateMatrix(strA, strB);
        //    InitTraceMatrix();
        //    CalculateTrace();
        //    _trace.CalculateEdits();
        //}


        //// ----------------------------------------------------------------------------------------
        ///// <!-- CalculateMatrix -->
        ///// <summary>
        /////      Builds the Levenschtein matrix in order to get the Levenschtein distance
        ///// </summary>
        /////      Example matrix with trace:
        /////      (in this case the Levenschtein distance is 6)
        ///// 
        /////             a l p h a b e t i c
        /////          [0]1 2 3 4 5 6 7 8 9 10
        /////        e  1[1]2 3 4 5 6 6 7 8 9
        /////        l  2 2[1]2 3 4 5 6 7 8 9
        /////        e  3 3[2]2 3 4 5 5 6 7 8
        /////        p  4 4 3[2]3 4 5 6 6 7 8
        /////        h  5 5 4 3[2]3 4 5 6 7 8
        /////        a  6 5 5 4 3[2 3]4 5 6 7
        /////        n  7 6 6 5 4 3 3[4]5 6 7
        /////        t  8 7 7 6 5 4 4 4[4 5]6
        /////        s  9 8 8 7 6 5 5 5 5 5[6]
        ///// 
        ///// <param name="strA"></param>
        ///// <param name="strB"></param>
        //private void CalculateMatrix(string strA, string strB)
        //{
        //    // --------------------------------------------------------------------------
        //    //  Strip off leading matching characters
        //    // --------------------------------------------------------------------------
        //    int leading = LeadingMatching(strA, strB);
        //    if (leading > 1)
        //    {
        //        strA = strA.Substring(leading-1);
        //        strB = strB.Substring(leading-1);
        //    }


        //    // --------------------------------------------------------------------------
        //    //  Strip off trailing matching characters
        //    // --------------------------------------------------------------------------
        //    int trailing = TrailingMatching(strA, strB);
        //    if (1 < trailing && trailing <= strA.Length && trailing <= strB.Length)
        //    {
        //        strA = strA.Substring(0,strA.Length - trailing);
        //        strB = strB.Substring(0,strB.Length - trailing);
        //    }



        //    // --------------------------------------------------------------------------
        //    //  Initialize character arrays
        //    // --------------------------------------------------------------------------
        //    _ci = ((string)(" " + strA)).ToCharArray();
        //    _cj = ((string)(" " + strB)).ToCharArray();


        //    // --------------------------------------------------------------------------
        //    //  Initialize matrix and character arrays
        //    // --------------------------------------------------------------------------
        //    try
        //    {
        //        _estDistance = Math.Abs(_ci.Length - _cj.Length);
        //        _matrix = new int[_ci.Length, _cj.Length];
        //        for (int i = 0; i < _ci.Length; ++i) _matrix[i, 0] = i;
        //        for (int j = 0; j < _cj.Length; ++j) _matrix[0, j] = j;


        //        // ----------------------------------------------------------------------
        //        //  Calculate matrix
        //        // ----------------------------------------------------------------------
        //        for (int i = 1; i < _ci.Length; ++i)
        //            for (int j = 1; j < _cj.Length; ++j)
        //                if (_ci[i] == _cj[j]) _matrix[i, j] = _matrix[i-1, j-1];
        //                else
        //                    _matrix[i, j] = 1 + Math.Min(Math.Min(_matrix[i-1, j-1], _matrix[i, j-1]), _matrix[i-1, j]);
        //    }
        //    catch (Exception e)
        //    {
        //        string msg = e.Message;
        //    }
        //}

        //// ----------------------------------------------------------------------------------------
        ///// <!-- LevenshteinDistance -->
        ///// <summary>
        /////      Reutrns the Levenshtein distance between two string, Warning: case sensitive
        ///// </summary>
        ///// <param name="source">the so-called 'source' string, the order doesn't matter however</param>
        ///// <param name="target">the so-called 'target' string, the order doesn't matter however</param>
        ///// <returns></returns>
        ///// <remarks>d has (m+1)*(n+1) values, for all i and j, d[i,j] will hold the Levenshtein distance between the first i characters of s and the first j characters of t;</remarks>
        //public static int LevenshteinDistance(string source, string target)
        //{
        //    // --------------------------------------------------------------------------
        //    //  Prepare the matrix:
        //    // --------------------------------------------------------------------------
        //    char[] src = (" " + source).ToCharArray();
        //    char[] tgt = (" " + target).ToCharArray();
        //    int[,] dist = new int[src.Length,tgt.Length]; // [m, n] // distance matrix
        //    for (int i = 0; i < src.Length; ++i) for (int j = 0; j < tgt.Length; ++j)
        //            dist[i,j] = Math.Max(i,j);

 
        //    // --------------------------------------------------------------------------
        //    //  Calculate the matrix:
        //    // --------------------------------------------------------------------------
        //    for (int j = 1; j < tgt.Length; ++j)
        //        for (int i = 1; i < src.Length; ++i)
        //            if (src[i] == tgt[j]) dist[i,j] = dist[i-1,j-1]; // no operation required
        //            else                  dist[i,j] = Minimum(dist[i-1,j]+1, dist[i,j-1]+1, dist[i-1,j-1]+1); // Min( deletion , insertion , substitution);
        //    return dist[src.Length-1, tgt.Length-1];
        //}

        ///// <summary>Minimum of three integers, used only here</summary>
        //private static int Minimum(int a, int b, int c) { return Math.Min(Math.Min(a,b),c); }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- CalculateTrace -->
        ///// <summary>
        ///// 
        ///// </summary>
        //private void CalculateTrace()
        //{
        //    string str;
        //    if (_matrix.Length < 1000)
        //        str = this.ToString();
        //    try
        //    {
        //        _trace = new LevenschteinTrace(Math.Max(_ci.Length, _cj.Length));
        //        int i = _ci.Length-1;
        //        int j = _cj.Length-1;
        //        if (i > 0 && j > 0)
        //        {
        //            // ------------------------------------------------------------------
        //            ///  Normal trace calculation
        //            // ------------------------------------------------------------------
        //            for (int t = Math.Max(_ci.Length, _cj.Length) - 1; i > 0 && j > 0; t--)
        //            {
        //                _trace.Add(i, j, _matrix[i, j], _ci[i], _cj[j]);
        //                _tr[i, j] = true;
        //                int current = _matrix[i, j];
        //                int decrj = _matrix[i, j - 1];
        //                int decri = _matrix[i - 1, j];
        //                int decrij = _matrix[i - 1, j - 1];
        //                if (decrij <= decri && decrij <= decrj && decrij < current) { i--; j--; }
        //                else if (decri < decrj) { i--; }
        //                else if (decrj < decri) { j--; }
        //                else { i--; j--; }
        //            }
        //        }
        //        else if (i == 0 && j > 0)
        //        {
        //            for (int t = _cj.Length - 1; j > 0; t--)
        //            {
        //                _trace.Add(i, j, _matrix[i, j], _ci[i], _cj[j]);
        //                _tr[i, j] = true;
        //                j--;
        //            }
        //        }
        //        else if (i > 0 && j == 0)
        //        {
        //            for (int t = _ci.Length - 1; i > 0; t--)
        //            {
        //                _trace.Add(i, j, _matrix[i, j], _ci[i], _cj[j]);
        //                _tr[i, j] = true;
        //                i--;
        //            }
        //        }
        //    }
        //    catch { }
        //}


        //// ----------------------------------------------------------------------------------------
        ///// <!-- Distance -->
        ///// <summary>
        ///// 
        ///// </summary>
        //public int Distance { get
        //{
        //    if (_matrix == null) return -_estDistance;
        //    else return _matrix[_ci.Length-1,_cj.Length-1];
        //} }
        //private int _estDistance;


        //// ----------------------------------------------------------------------------------------
        ///// <!-- InitTraceMatrix -->
        ///// <summary>
        /////      Initializes the trace matrix which exists only to ease ToString()
        ///// </summary>
        //private void InitTraceMatrix()
        //{
        //    try
        //    {
        //        _tr = new bool[_ci.Length, _cj.Length];
        //        for (int i = 0; i < _ci.Length; ++i)
        //            for (int j = 0; j < _cj.Length; ++j)
        //                _tr[i, j] = false;
        //    }
        //    catch { }
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- LeadingMatching -->
        /// <summary>
        ///      Determines how far into two strings they both match
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static int LeadingMatching(string strA, string strB)
        {
            // --------------------------------------------------------------------------
            //  Initialize variables
            // --------------------------------------------------------------------------
            char[] A = (strA + " ").ToCharArray();
            char[] B = (strB + ".").ToCharArray();
            int lenA = A.Length;
            int lenB = B.Length;


            // --------------------------------------------------------------------------
            //  Determine trailing match
            // --------------------------------------------------------------------------
            if (lenA == 0 || lenB == 0 || A[0] != B[0]) return 0;
            if (lenA == 1 || lenB == 1 || A[1] != B[1]) return 1;


            int len = Math.Min(A.Length, B.Length);
            int leading = -1;
            for (int i = 0; i < len && leading == -1; ++i)
                if (A[i] != B[i])
                    leading = i;
            return leading;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- TrailingMatching -->
        /// <summary>
        ///      Determines how far from the end of two strings they both match
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static int TrailingMatching(string strA, string strB)
        {
            // --------------------------------------------------------------------------
            //  Initialize variables
            // --------------------------------------------------------------------------
            char[] A = (" " + strA).ToCharArray();
            char[] B = ("." + strB).ToCharArray();
            int lenA = A.Length;
            int lenB = B.Length;
            int i;


            // --------------------------------------------------------------------------
            //  Determine trailing match
            // --------------------------------------------------------------------------
            if (lenA == 0 || lenB == 0 || A[lenA - 1] != B[lenB - 1]) return 0;
            if (lenA == 1 || lenB == 1 || A[lenA - 2] != B[lenB - 2]) return 1;
            int len = Math.Min(lenA, lenB);
            for (i = 1; i < len && A[lenA - i] == B[lenB - i]; ++i) { }
            return i - 1;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- ToString -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public override string ToString()
        //{
        //    StringBuilder m = new StringBuilder((1 + _ci.Length) * (_cj.Length) * 3);
        //    m.Append("   ");
        //    for (int j = 0; j < _cj.Length; ++j) m.Append(((int)_cj[j]).ToPaddedString(3));
        //    for (int i = 0; i < _ci.Length; ++i)
        //    {
        //        // ----------------------------------------------------------------------
        //        //  Add a cell
        //        // ----------------------------------------------------------------------
        //        m.Append("\r\n" + ((int)_ci[i]).ToPaddedString(3));
        //        for (int k = 0; k < _cj.Length; ++k)
        //            if (_tr[i,k]) m.Append("<" + _matrix[i, k].ToString().PadLeft(2));
        //            else          m.Append(" " + _matrix[i, k].ToString().PadLeft(2));
        //        m.Append(" ");
        //    }


        //    string str = m.ToString();
        //    str = Regex.Replace(str, @"<([0-9 ][0-9]) ", "<$1]");
        //    str = Regex.Replace(str, @" ([0-9 ][0-9])<", " $1[");
        //    str = Regex.Replace(str, @"<", " ");
        //    return str;
        //}



        //// ----------------------------------------------------------------------------------------
        ///// <!-- Trace -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public LevenschteinTrace Trace { get { return _trace; } }


    }


    // --------------------------------------------------------------------------------------------
    /// <!-- LevenschteinTrace -->
    /// <summary>
    /// 
    /// </summary>
    public class LevenschteinTrace
    {
        // ----------------------------------------------------------------------------------------
        //  Data members (these could all be bundled into a List<LevenschteinStep> class)
        // ----------------------------------------------------------------------------------------
        List<int>  _i;     // in reverse order
        List<int>  _j;     // in reverse order
        List<int>  _depth; // in reverse order
        List<char> _ci;    // in reverse order
        List<char> _cj;    // in reverse order
        List<char> _cut;   /// this is what's cut at each step in the edit trace; in reverse order
        List<char> _add;   /// this is what's added at each step in the edit trace; in reverse order


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public LevenschteinTrace()        { Init(4);   }
        public LevenschteinTrace(int len) { Init(len); }


        private void Init(int len)
        {
            _i     = new List<int> (len); // size = length of the trace
            _j     = new List<int> (len);
            _depth = new List<int> (len);
            _ci    = new List<char>(len);
            _cj    = new List<char>(len);
            _cut   = new List<char>(len);
            _add   = new List<char>(len);
        }


        public int Count { get { return _i.Count; } } // all the arrays should be the same length


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="depth"></param>
        /// <param name="ci"></param>
        /// <param name="cj"></param>
        public void Add(int i, int j, int depth, char ci, char cj)
        {
            _i.Add(i);
            _j.Add(j);
            _depth.Add(depth);
            _ci.Add(ci);
            _cj.Add(cj);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CalculateEdits -->
        /// <summary>
        ///      Determines the steps to edit the first string to become the second
        /// </summary>
        public void CalculateEdits()
        {
            int count = _i.Count;
            if (count > 0)
            {
                char ci;
                char cj;
                int t;
                for (t = 0; t < count-1; ++t)
                {
                    ci = _ci[t];
                    cj = _cj[t];


                    // ------------------------------------------------------------------
                    //  Check for an edit at this step
                    // ------------------------------------------------------------------
                    int depth     = _depth[t];
                    int nextDepth = _depth[t+1];
                    if (nextDepth < depth)
                    {
                        int i = _i[t];
                        int j = _j[t];
                        int nextI = _i[t+1];
                        int nextJ = _j[t+1];


                        // --------------------------------------------------------------
                        //  Determine edit
                        // --------------------------------------------------------------
                        if      (nextI <  i && nextJ <  j)  { _cut.Add(ci);      _add.Add(cj);      }
                        else if (nextI <  i && nextJ == j)  { _cut.Add(ci);      _add.Add((char)0); }
                        else if (nextI == i && nextJ <  j)  { _cut.Add((char)0); _add.Add(cj);      }
                        else throw new Exception("trace corrupt!");
                    }
                    else { _cut.Add(ci); _add.Add(cj); }
                }


                // ----------------------------------------------------------------------
                //  Handle the edit in the final stap
                // ----------------------------------------------------------------------
                _cut.Add(_ci[_ci.Count-1]);
                _add.Add(_cj[_cj.Count-1]);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Condense -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        private static string Condense(string edit)
        {
            edit = HideSyntax(edit);  /// hide arrows and square bracket


            edit = Regex.Replace(edit, @"\+<(.)>,<(.)-'(.)>"                       , "<$2-'$1$3>"     ); /// add char to ends of arrows
            edit = Regex.Replace(edit, @"-<(.)>,<(.)-'(.)>"                        , "<$1$2-'$3>"     ); /// add char to beginnings of arrows
            edit = Regex.Replace(edit, @"<([^<>]+)-'([^<>]+)>,<([^<>]+)-'([^<>]+)>", "<$1$3-'$2$4>"   ); /// combine two arrows
            edit = Regex.Replace(edit, @"<([^<>]+)-'([^<>]+)>,<([^<>]+)-'([^<>]+)>", "<$1$3-'$2$4>"   ); /// combine two arrows
            edit = Regex.Replace(edit, @"<([^<>]+)-'([^<>]+)>,<([^<>]+)-'([^<>]+)>", "<$1$3-'$2$4>"   ); /// combine two arrows
            edit = Regex.Replace(edit, @"<([^<>]+)-'([^<>]+)>,<([^<>]+)-'([^<>]+)>", "<$1$3-'$2$4>"   ); /// combine two arrows
            edit = Regex.Replace(edit, @"<([^<>]+)-'([^<>]+)>,<([^<>]+)-'([^<>]+)>", "<$1$3-'$2$4>"   ); /// combine two arrows


            edit = Regex.Replace(edit, @"<(.)>,([+-])<([^<>]+)\1>,<([^<>]+)>"      , "$2<$1$3>,<$1$4>");
            edit = Regex.Replace(edit, @"([+-])<([^<>]+)>,\1<([^<>]+)>"            , "$1<$2$3>"       ); /// combine two plusses or two minuses
            edit = Regex.Replace(edit, @"<(.)>,([+-])<([^<>]+)\1>,<([^<>]+)>"      , "$2<$1$3>,<$1$4>");
            edit = Regex.Replace(edit, @"([+-])<([^<>]+)>,\1<([^<>]+)>"            , "$1<$2$3>"       ); /// combine two plusses or two minuses
                                                                                                         /// 

            edit = Regex.Replace(edit, "-<([^<>]+)>,<([^<>]+)-'([^<>]+)>"          , "<$1$2-'$3>"     ); /// bring in a minused 'pre-' to an arrow


            edit = RestoreSyntax(edit);  /// restore the arrows and square brackets


            return edit;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Edit1to2_condensed -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Edit1to2_condensed()
        {
            string edit = Edit1to2_medium();
            edit = Condense(edit);
            edit = HandleWhitespace(edit);
            return edit;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Edit1to2_long -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Edit1to2_long()
        {
            StringBuilder str = new StringBuilder();
            string delim = "";
            for (int t = Count-1; t >= 0 ; --t) // t is for trace step
            {
                char cut = _cut[t];
                char add = _add[t];


                str.Append(delim);
                switch (EditType(cut, add))
                {
                    case '=' : str.Append(      cut             ); break;
                    case '0' : str.Append("="                   ); break;
                    case '+' : str.Append("+"              + add); break;
                    case '-' : str.Append("-" + cut             ); break;
                    case '>' : str.Append(      cut + "->" + add); break;
                }

                delim = ",";
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Edit1to2_medium -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Edit1to2_medium()
        {
            StringBuilder str = new StringBuilder();
            string d1 = "";
            string d2 = "[";
            char lastMode = ' ';
            for (int t = Count-1; t >= 0 ; --t) // t is for trace step, remember the trace is in reverse order
            {
                char cut = _cut[t];
                char add = _add[t];

                //string debug = ToString();


                char type = EditType(cut, add);


                if (type != lastMode || lastMode == ' ')
                    // ------------------------------------------------------------------
                    //  When mode changes, do it the long way (see Edit1to2_long)
                    // ------------------------------------------------------------------
                    switch (type)
                    {
                        case '=' : str.Append(d1       + d2 + cut             );  break;
                        case '0' : str.Append(d1       + d2 + "="             );  break;
                        case '+' : str.Append(d1 + "+" + d2              + add);  break;
                        case '-' : str.Append(d1 + "-" + d2 + cut             );  break;
                        case '>' : str.Append(d1       + d2 + cut + "->" + add);  break;
                    }
                else
                    // ------------------------------------------------------------------
                    //  If mode stays the same, continue, using an abbreviated append
                    // ------------------------------------------------------------------
                    switch (type)
                    {
                        case '=' : str.Append(          cut             );  break;
                        case '0' : str.Append(          "="             );  break;
                        case '+' : str.Append(                       add);  break;
                        case '-' : str.Append(          cut             );  break;
                        case '>' : str.Append(d1 + d2 + cut + "->" + add);  break;
                    }


                lastMode = type;
                d1 = "],";
                d2 = "[";
            }
            str.Append("]");
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Edit1to2_short -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Edit1to2_short()
        {
            string edit = Edit1to2_medium();
            edit = Condense(edit);
            edit = HandleWhitespace(edit);
            edit = RemoveUnchangingContent(edit);
            return edit;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EditType -->
        /// <summary>
        ///      Figures out what type of edit is being done at this step in the trace
        /// </summary>
        /// <param name="cut"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        private static char EditType(char cut, char add)
        {
            char c0 = (char)0;
            char type; // edit type


            if      (cut       == add      ) type = '=';
            else if (cut == c0 && add == c0) type = '0';
            else if (cut == c0             ) type = '+';
            else if (             add == c0) type = '-';
            else                             type = '>';

            return type;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HandleWhitespace -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        private static string HandleWhitespace(string edit)
        {
            edit = HideSyntax(edit);
            // "<order-'ORDER>,< >,<by-'BY>" ---> "<order by-'ORDER BY>"
            edit = Regex.Replace(edit, @"<([^<>]+)-'([^<>]+)>,< >,<([^<>]+)-'([^<>]+)>", "<$1 $3-'$2 $4>");
            edit = RestoreSyntax(edit);


            return edit;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HideSyntax -->
        /// <summary>
        ///      Hides ticks, chevrons, and edit syntax, this is paired with RestoreSyntax
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        private static string HideSyntax(string edit)
        {
            //  hide arrows and ticks
            edit = Regex.Replace(edit, "'", "MTICK");
            edit = Regex.Replace(edit, @"\->", "-'");

            //  hide chevrons
            edit = Regex.Replace(edit, "<", "LCHEV");
            edit = Regex.Replace(edit, ">", "RCHEV");

            //  hide edit-brackets
            edit = Regex.Replace(edit, @"^\[", "<", RegexOptions.Singleline);
            edit = Regex.Replace(edit, @"\],([+-]?)\[", ">,$1<");
            edit = Regex.Replace(edit, @"\]$", ">", RegexOptions.Singleline);
            return edit;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RemoveUnchangingContent -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        private static string RemoveUnchangingContent(string edit)
        {
            edit = HideSyntax(edit);
            edit = Regex.Replace(edit, @"-<([^<'>]+)>,<(.)>,-<([^<'>]+)\2>", "-<$1$2$3>", RegexOptions.Singleline);
            edit = Regex.Replace(edit, @",<[^<'>]*>"  , ",<>", RegexOptions.Singleline);
            edit = Regex.Replace(edit, @"^<[^<'>]*>,?", ""   , RegexOptions.Singleline);
            edit = Regex.Replace(edit, @",<>$"        , ""   , RegexOptions.Singleline);          
            edit = RestoreSyntax(edit);

            return edit;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RestoreSyntax -->
        /// <summary>
        ///      Restores ticks, chevrons, and edit syntax, this is paired with HideSyntax
        /// </summary>
        /// <param name="edit"></param>
        /// <returns></returns>
        private static string RestoreSyntax(string edit)
        {
            //  restore edit-brackets
            edit = Regex.Replace(edit, "<", "[");
            edit = Regex.Replace(edit, ">", "]");

            //  restore chevrons
            edit = Regex.Replace(edit, "LCHEV", "<");
            edit = Regex.Replace(edit, "RCHEV", ">");

            //  restore arrows and ticks
            edit = Regex.Replace(edit, @"\-'", "->");
            edit = Regex.Replace(edit, "MTICK", "'");
            return edit;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- Contains -->
        ///// <summary>
        /////      Determines whether the coordinate pair is in the trace
        ///// </summary>
        ///// <remarks>
        /////      This works but it's been commented out because if used it could create some very 
        /////      inefficient code
        ///// </remarks>
        ///// <param name="i"></param>
        ///// <param name="j"></param>
        ///// <returns></returns>
        //public bool Contains(int i, int j)
        //{
        //    int count = _i.Count;
        //    for (int t = 0; t < count; ++t)
        //        if (_i[t] == i && _j[t] == j)
        //            return true;
        //    return false;
        //}
    }
}
