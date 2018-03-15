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
	/// <!-- EndemeTextFormat -->
    /// <summary>
    ///      The EndemeTextFormat class implements formatted endeme to string functions
    /// </summary>
    /// <remarks>alpha code</remarks>
    public static class EndemeTextFormat
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- FourEndemes -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="s1"></param>
        /// <param name="e2"></param>
        /// <param name="e3"></param>
        /// <param name="e4"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        /// <remarks>does not belong in this class, put into an EndemeFormat class</remarks>
        public static string FourEndemes(Endeme e1, string s1, Endeme e2, Endeme e3, Endeme e4, int width)
        {
            StringBuilder str = new StringBuilder();
            str.Append("|" + SetLength(width, " " + e1.SetName + ": " + s1)
                + "| " + e2.SetName.PadRight(width-1, ' ')
                + "| " + e3.SetName.PadRight(width-1, ' ')
                + "| " + e4.SetName.PadRight(width-1, ' ')
                + "|");
            str.Append("\r\n");
            str.Append("| ".PadRight(width, '-') + " "
                + "| ".PadRight(width, '-') + " "
                + "| ".PadRight(width, '-') + " "
                + "| ".PadRight(width, '-') + " "
                + "|");
            string delim = "\r\n";
            for (int i = 0; i < 22; ++i)
            {
                str.Append(delim);
                str.Append("|" + FormatEndemeLeaf(e1, i, width));
                str.Append("|" + FormatEndemeLeaf(e2, i, width));
                str.Append("|" + FormatEndemeLeaf(e3, i, width));
                str.Append("|" + FormatEndemeLeaf(e4, i, width));
                str.Append("|");
                delim = "\r\n";
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FormatEndemeLeaf -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="num"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static string FormatEndemeLeaf(Endeme e, int num, int width)
        {
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] c = e.ToCharArray();
            string str = "";
            if (num < c.Length)
                str = " " + c[num] + " |"
                    + " " + e.Index(alphabet[num]).ToString().PadLeft(2,' ')
                    + " " + e.EnSet.Characteristic[alphabet[num]].Display;
            return SetLength(width, str);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Hyphenate -->
        /// <summary>
        ///      Hyphenates an endeme
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string Hyphenate(Endeme en)
        {
            string str = "";
            if (en.Count >  0) str +=       en.ToString(0 ,3);
            if (en.Count >  3) str += "-" + en.ToString(3 ,4);
            if (en.Count >  7) str += "-" + en.ToString(7 ,4);
            if (en.Count > 11) str += "-" + en.ToString(11,4);
            if (en.Count > 15) str += "-" + en.ToString(15,4);
            if (en.Count > 19) str += "-" + en.ToString(19,3);
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetLength -->
        /// <summary>
        ///      Returns a string exactly as long as specified
        /// </summary>
        /// <remarks>
        ///      This is copied from __ to keep this namespace atomic
        /// </remarks>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string SetLength(int width, string str)
        {
            if (str == null) str = "";
            str = str.PadRight(width+1, ' ').Substring(0,width);
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Show() -->
        /// <summary>
        ///      Returns a string displaying the N most important characteristics etc
        /// </summary>
        /// <param name="num"></param>
        /// <param name="width"></param>
        /// <param name="wordLen"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Deprecated
        /// </remarks>
        public static string Show(Endeme en, int num, int width, string divider, int wordLen, string delim)
        {
            if (num > width) num = width;
            string str = "";
            string pattern = "";
            string replace = "";
            StringBuilder letters = new StringBuilder();


            if (en.IsEmpty) str = "";
            else
            {
                str = en.ToString(); // new string(_string.ToArray());
                num = Math.Max(Math.Min(str.Length, num), 0);
                switch(width - num)
                {
                    case 0 :
                        letters = new StringBuilder(str);
                        break;
                    case 1 : case 2 :
                        if (num > 5)
                             letters.Append(Regex.Replace(str, "^(...)(..)", "$1-$2"));
                        else letters.Append(EndemeTextFormat.SetLength(num, str));
                        break;
                    case 3 :
                        switch(num)
                        {
                            case  0 : pattern = "^.*$"                                  ; replace = ""           ; break;
                            case  1 : pattern = "^(.).*$"                               ; replace = "$1"         ; break;
                            case  2 : pattern = "^(..).*$"                              ; replace = "$1"         ; break;
                            case  3 : pattern = "^(...).*$"                             ; replace = "$1"         ; break;
                            case  4 : pattern = "^(....).*$"                            ; replace = "$1"         ; break;
                            case  5 : pattern = "^(...)(..).*$"                         ; replace = "$1-$2"      ; break;
                            case  6 : pattern = "^(...)(...).*$"                        ; replace = "$1-$2"      ; break;
                            case  7 : pattern = "^(...)(....).*$"                       ; replace = "$1-$2"      ; break;
                            case  8 : pattern = "^(...)(.....).*$"                      ; replace = "$1-$2"      ; break;
                            case  9 : pattern = "^(...)(...).*(...)$"                   ; replace = "$1-$2--$3"  ; break;
                            case 10 : pattern = "^(...)(....).*(...)$"                  ; replace = "$1-$2--$3"  ; break;
                            case 11 : pattern = "^(...)(.....).*(...)$"                 ; replace = "$1-$2--$3"  ; break;
                            case 12 : pattern = "^(...)(....)(..).*(...)$"              ; replace = "$1-$2-$3-$4"; break;
                            case 13 : pattern = "^(...)(....)(...).*(...)$"             ; replace = "$1-$2-$3-$4"; break;
                            case 14 : pattern = "^(...)(....)(....).*(...)$"            ; replace = "$1-$2-$3-$4"; break;
                            case 15 : pattern = "^(...)(....)(....).*(....)$"           ; replace = "$1-$2-$3-$4"; break;
                            case 16 : pattern = "^(...)(....)(....).*(.....)$"          ; replace = "$1-$2-$3-$4"; break;
                            case 17 : pattern = "^(...)(....)(....).*(......)$"         ; replace = "$1-$2-$3-$4"; break;
                            case 18 : pattern = "^(...)(....)(....).*(.......)$"        ; replace = "$1-$2-$3-$4"; break;
                            case 19 : pattern = "^(...)(....)(....).*(........)$"       ; replace = "$1-$2-$3-$4"; break;
                            case 20 : pattern = "^(...)(....)(......).*(.......)$"      ; replace = "$1-$2-$3-$4"; break;
                            case 21 : pattern = "^(...)(....)(.......).*(.......)$"     ; replace = "$1-$2-$3-$4"; break;
                            case 22 : pattern = "^(...)(....)(........).*(.......)$"    ; replace = "$1-$2-$3-$4"; break;
                            default : pattern = "^(...)(....)(.......*)(.......)$"      ; replace = "$1-$2-$3-$4"; break;
                        }
                        letters.Append(Regex.Replace(str, pattern, replace));
                        break;
                  //  case 4 :
                  //      switch(num)
                  //      {
                  //          case  0 : pattern = "^.*$"                                  ; replace = ""              ; break;
                  //          case  1 : pattern = "^(.).*$"                               ; replace = "$1"            ; break;
                  //          case  2 : pattern = "^(..).*$"                              ; replace = "$1"            ; break;
                  //          case  3 : pattern = "^(...).*$"                             ; replace = "$1"            ; break;
                  //          case  4 : pattern = "^(....).*$"                            ; replace = "$1"            ; break;
                  //          case  5 : pattern = "^(...)(..).*$"                         ; replace = "$1-$2"         ; break;
                  //          case  6 : pattern = "^(...)(...).*$"                        ; replace = "$1-$2"         ; break;
                  //          case  7 : pattern = "^(...)(....).*$"                       ; replace = "$1-$2"         ; break;
                  //          case  8 : pattern = "^(...)(.....).*$"                      ; replace = "$1-$2"         ; break;
                  //          case  9 : pattern = "^(...)(...).*(...)$"                   ; replace = "$1-$2---$3"    ; break;
                  //          case 10 : pattern = "^(...)(....).*(...)$"                  ; replace = "$1-$2---$3"    ; break;
                  ///* TAG */ case 11 : pattern = "^(...)(.....).*(...)$"                 ; replace = "$1-$2---$3"    ; break;
                  //          case 12 : pattern = "^(...)(....)(..).*(...)$"              ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 13 : pattern = "^(...)(....)(...).*(...)$"             ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 14 : pattern = "^(...)(....)(....).*(...)$"            ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 15 : pattern = "^(...)(....)(....).*(....)$"           ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 16 : pattern = "^(...)(....)(....).*(.....)$"          ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 17 : pattern = "^(...)(....)(....).*(......)$"         ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 18 : pattern = "^(...)(....)(....).*(.......)$"        ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 19 : pattern = "^(...)(....)(....).*(........)$"       ; replace = "$1-$2-$3--$4"  ; break;
                  //          case 20 : pattern = "^(...)(....)(......).*(....)(...)$"    ; replace = "$1-$2-$3-$4-$5"; break;
                  //          case 21 : pattern = "^(...)(....)(.......).*(....)(...)$"   ; replace = "$1-$2-$3-$4-$5"; break;
                  //          case 22 : pattern = "^(...)(....)(........).*(....)(...)$"  ; replace = "$1-$2-$3-$4-$5"; break;
                  //          default : pattern = "^(...)(....)(.......*)(....)(...)$"    ; replace = "$1-$2-$3-$4-$5"; break;
                  //      }
                  //      letters.Append(Regex.Replace(str, pattern, replace));
                  //      break;
                    case 5 : case 6 : case 7 : case 8 : case 9 : case 10 :
                        switch(num)
                        {
                            case  0 : pattern = "^.*$"                                  ; replace = ""                 ; break;
                            case  1 : pattern = "^(.).*$"                               ; replace = "$1"               ; break;
                            case  2 : pattern = "^(..).*$"                              ; replace = "$1"               ; break;
                            case  3 : pattern = "^(...).*$"                             ; replace = "$1"               ; break;
                            case  4 : pattern = "^(....).*$"                            ; replace = "$1"               ; break;
                            case  5 : pattern = "^(...)(..).*$"                         ; replace = "$1-$2"            ; break;
                            case  6 : pattern = "^(...)(...).*$"                        ; replace = "$1-$2"            ; break;
                            case  7 : pattern = "^(...)(....).*$"                       ; replace = "$1-$2"            ; break;
                            case  8 : pattern = "^(...)(.....).*$"                      ; replace = "$1-$2"            ; break;
                            case  9 : pattern = "^(...)(...).*(...)$"                   ; replace = "$1-$2----$3"      ; break;
                            case 10 : pattern = "^(...)(....).*(...)$"                  ; replace = "$1-$2----$3"      ; break;
                            case 11 : pattern = "^(...)(.....).*(...)$"                 ; replace = "$1-$2----$3"      ; break;
                            case 12 : pattern = "^(...)(....)(..).*(...)$"              ; replace = "$1-$2-$3---$4"    ; break;
                            case 13 : pattern = "^(...)(....)(...).*(...)$"             ; replace = "$1-$2-$3---$4"    ; break;
                            case 14 : pattern = "^(...)(....)(....).*(...)$"            ; replace = "$1-$2-$3---$4"    ; break;
                            case 15 : pattern = "^(...)(....)(....).*(....)$"           ; replace = "$1-$2-$3---$4"    ; break;
                            case 16 : pattern = "^(...)(....)(....).*(..)(...)$"        ; replace = "$1-$2-$3--$4-$5"  ; break;
                            case 17 : pattern = "^(...)(....)(....).*(...)(...)$"       ; replace = "$1-$2-$3--$4-$5"  ; break;
                            case 18 : pattern = "^(...)(....)(....).*(....)(...)$"      ; replace = "$1-$2-$3--$4-$5"  ; break;
                            case 19 : pattern = "^(...)(....)(....).*(.....)(...)$"     ; replace = "$1-$2-$3--$4-$5"  ; break;
                            case 20 : pattern = "^(...)(....)(....)(..).*(....)(...)$"  ; replace = "$1-$2-$3-$4-$5-$6"; break;
                            case 21 : pattern = "^(...)(....)(....)(...).*(....)(...)$" ; replace = "$1-$2-$3-$4-$5-$6"; break;
                            case 22 : pattern = "^(...)(....)(....)(....).*(....)(...)$"; replace = "$1-$2-$3-$4-$5-$6"; break;
                            default : pattern = "^(...)(....)(....)(...*)(....)(...)$"  ; replace = "$1-$2-$3-$4-$5-$6"; break;
                        }
                        letters.Append(Regex.Replace(str, pattern, replace));
                        break;
                    case 4 :
                    default :
                        switch(num)
                        {
                            case  0 : pattern = "^.*$"                                  ; replace = ""              ; break;
                            case  1 : pattern = "^(.).*$"                               ; replace = "$1"            ; break;
                            case  2 : pattern = "^(..).*$"                              ; replace = "$1"            ; break;
                            case  3 : pattern = "^(...).*$"                             ; replace = "$1"            ; break;
                            case  4 : pattern = "^(....).*$"                            ; replace = "$1"            ; break;
                            case  5 : pattern = "^(...)(..).*$"                         ; replace = "$1-$2"         ; break;
                            case  6 : pattern = "^(...)(...).*$"                        ; replace = "$1-$2"         ; break;
                            case  7 : pattern = "^(...)(....).*$"                       ; replace = "$1-$2"         ; break;
                            case  8 : pattern = "^(...)(.....).*$"                      ; replace = "$1-$2"         ; break;
                            case  9 : pattern = "^(...)(...).*(...)$"                   ; replace = "$1-$2--$3"     ; break;
                            case 10 : pattern = "^(...)(....).*(...)$"                  ; replace = "$1-$2--$3"     ; break;
                  /* TAG */ case 11 : pattern = "^(...)(.....).*(...)$"                 ; replace = "$1-$2--$3"     ; break;
                            case 12 : pattern = "^(...)(....)(..).*(...)$"              ; replace = "$1-$2-$3--$4"  ; break;
                            case 13 : pattern = "^(...)(....)(...).*(...)$"             ; replace = "$1-$2-$3--$4"  ; break;
                            case 14 : pattern = "^(...)(....)(....).*(...)$"            ; replace = "$1-$2-$3--$4"  ; break;
                            case 15 : pattern = "^(...)(....)(....).*(....)$"           ; replace = "$1-$2-$3--$4"  ; break;
                            case 16 : pattern = "^(...)(....)(....).*(.....)$"          ; replace = "$1-$2-$3--$4"  ; break;
                            case 17 : pattern = "^(...)(....)(....).*(......)$"         ; replace = "$1-$2-$3--$4"  ; break;
                            case 18 : pattern = "^(...)(....)(....).*(.......)$"        ; replace = "$1-$2-$3--$4"  ; break;
                            case 19 : pattern = "^(...)(....)(....).*(........)$"       ; replace = "$1-$2-$3--$4"  ; break;
                            case 20 : pattern = "^(...)(....)(......).*(....)(...)$"    ; replace = "$1-$2-$3-$4-$5"; break;
                            case 21 : pattern = "^(...)(....)(.......).*(....)(...)$"   ; replace = "$1-$2-$3-$4-$5"; break;
                            case 22 : pattern = "^(...)(....)(........).*(....)(...)$"  ; replace = "$1-$2-$3-$4-$5"; break;
                            default : pattern = "^(...)(....)(.......*)(....)(...)$"    ; replace = "$1-$2-$3-$4-$5"; break;
                        }
                        letters.Append(Regex.Replace(str, pattern, replace));
                        break;
                }
            }


            // -------------------------------------------------------------------------
            //  Add some words if there is room
            // -------------------------------------------------------------------------
            if (en.EnSet != null && width > letters.Length + 5)
            {
                string d = divider;
                for (int i = 0; i < en.Count && i < 7; ++i)
                {
                    letters.Append(d + EndemeTextFormat.SetLength(wordLen, en.EnSet.Characteristic[en[i]].Label));
                    d = delim;
                }
                d = divider;
                for (int i = Math.Max(0, en.Count-3); i < en.Count; ++i)
                {
                    char ch = en[i];
                    string addme = en.EnSet.Characteristic[ch].Label;
                    addme = Regex.Replace(addme, "^.* vs ", "-");
                    letters.Append(d + EndemeTextFormat.SetLength(wordLen, addme));
                    d = delim;
                }
                d = divider;
                for (int i = 7; i < en.Count-3; ++i)
                {
                    letters.Append(d + EndemeTextFormat.SetLength(wordLen, en.EnSet.Characteristic[en[i]].Label));
                    d = delim;
                }
            }
            return EndemeTextFormat.SetLength(width, letters.ToString());
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Show -->
        /// <summary>
        ///      Shows an endeme in groups of three characters
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string Show(Endeme en, int width)
        {
            string endeme = en.ToString();
            endeme = Regex.Replace(endeme, "^(...)(.)", "$1-$2");
            endeme = Regex.Replace(endeme, "^(...-...)(.)", "$1-$2");
            endeme = Regex.Replace(endeme, "^(...-...-...)(.)", "$1-$2");
            if (endeme.Length > 20)
            {
                endeme = Regex.Replace(endeme, "(.)(...)$", "$1-$2");
                endeme = Regex.Replace(endeme, "(.)(...-...)$", "$1-$2");
                endeme = Regex.Replace(endeme, "(.)(...-...-...)$", "$1-$2");
            }
            else
            {
                endeme = Regex.Replace(endeme, "^(...-...-...-....)(.)", "$1-$2");
                endeme = Regex.Replace(endeme, "^(...-...-...-....-...)(.)", "$1-$2");
                endeme = Regex.Replace(endeme, "^(...-...-...-....-...-...)(.)", "$1-$2");
            }
            while (endeme.Length > width)
            {
                int chop = (int)(0.7 * endeme.Length);
                endeme = endeme.Substring(0, chop-1) + "|" + endeme.Substring(chop + 1);
            }
            return endeme.PadRight(width);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Show_complex -->
        /// <summary>
        ///      Returns a string displaying the endeme in the specified width
        /// </summary>
        /// <remarks>
        ///      112-2334-4556-6778-7654-321
        ///         1    3    5    4    2
        /// </remarks>
        /// <param name="width"></param>
        /// <param name="divs"></param>
        /// <param name="divider"></param>
        /// <returns></returns>
        public static string Show_complex(Endeme en, int width, int divs, string divider)
        {
            // --------------------------------------------------------------------------
            //  Handle some error conditions
            // --------------------------------------------------------------------------
            if (width <= 0) return "";
            if (en.IsEmpty)
                return divider.PadRight(divs,divider.ToCharArray()[0]).PadRight(width);
            if (divs >= width)
                return "".PadRight(width, divider.ToCharArray()[0]);


            // --------------------------------------------------------------------------
            //  Calculate how many characteristics to show
            // --------------------------------------------------------------------------
            divs = Math.Min(5, divs);
            int num = width - divs * divider.Length;
            if (en.EnSet == null)  num = Math.Min(num, en.Count);
            else              num = Math.Min(num, en.EnSet.Count);


            // --------------------------------------------------------------------------
            //  Remove characters from the endeme string as necessary to format
            // --------------------------------------------------------------------------
            if (width <= 8)                 return EndemeTextFormat.Show_positive(en, width, divs, num, divider);
            if (num < en.ToString().Length) return EndemeTextFormat.Show_partial (en, width, divs, num, divider);
            else                            return EndemeTextFormat.Show_perfect (en, width, divs, num, divider);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Show_positive -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="divs"></param>
        /// <param name="num"></param>
        /// <param name="divider"></param>
        /// <returns></returns>
        public static string Show_positive(Endeme en, int width, int divs, int num, string divider)
        {
            string endeme = en.ToString();
            endeme = endeme.Substring(0, width - divs);
            int len = endeme.Length;


            if (divs == 1)
            {
                if      (len >= 8) endeme = Regex.Replace(endeme, "^(.......)", "$1-");
                else if (len >= 3) endeme = Regex.Replace(endeme, "^(...)"    , "$1-");
                else               endeme = endeme + "-";
            }


            if (divs >  1) { if (len >= 10) endeme = Regex.Replace(endeme, "^(.......)"               , "$1-"); else endeme = endeme + "-"; }
            if (divs >= 2) { if (len >=  3) endeme = Regex.Replace(endeme, "^(...)"                   , "$1-"); else endeme = endeme + "-"; }
            if (divs >= 3) { if (len >= 17) endeme = Regex.Replace(endeme, "^(...-....-........)"     , "$1-"); else endeme = endeme + "-"; }
            if (divs >= 4) { if (len >= 20) endeme = Regex.Replace(endeme, "^(...-....-........-....)", "$1-"); else endeme = endeme + "-"; }
            if (divs >= 5) { if (len >= 13) endeme = Regex.Replace(endeme, "^(...-....-....)"         , "$1-"); else endeme = endeme + "-"; }
            return endeme.PadRight(width);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Show_partial -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="divs"></param>
        /// <param name="divider"></param>
        /// <returns></returns>
        public static string Show_partial(Endeme en, int width, int divs, int num, string divider)
        {
            string endeme = en.ToString();
            int lenA = endeme.Length;


            // --------------------------------------------------------------------------
            //  Construct the initial partial endeme
            // --------------------------------------------------------------------------
            int len1 = (int)(0.5 + 2.0 * num / 3.0);
            int len2 = (int)(0.5 + 1.0 * num / 3.0);
            string part1 = endeme.Substring(0, len1);
            string part2 = endeme.Substring(lenA - len2, len2);
            if (divs > 0) endeme = part1 + "_" + part2;
            else endeme = part1 + part2;


            // --------------------------------------------------------------------------
            //  Insert more dividers
            // --------------------------------------------------------------------------
            int len = endeme.Length;
            if (divs >= 2 && endeme.Length < width)
            {
                if (part1.Length >= 6) endeme = Regex.Replace(endeme, "^(.......)", "$1-");
                else endeme = Regex.Replace(endeme, "_", "_-");
            }
            if (divs >= 3 && endeme.Length < width) endeme = Regex.Replace(endeme, "^(...)", "$1-");
            if (divs >= 4 && endeme.Length < width) endeme = Regex.Replace(endeme, "(...)$", "-$1");
            if (divs >= 5 && endeme.Length < width)
            {
                if (part2.Length >= 7) endeme = Regex.Replace(endeme, "(........)$", "-$1");
                else endeme = Regex.Replace(endeme, "_", "_-");
            }

            endeme = Regex.Replace(endeme, "_", "-");
            endeme = Regex.Replace(endeme, "-", divider);

            return endeme;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Show_perfect -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="divs"></param>
        /// <param name="divider"></param>
        /// <returns></returns>
        public static string Show_perfect(Endeme en, int width, int divs, int num, string divider)
        {
            string endeme = en.ToString();

            // --------------------------------------------------------------------------
            //  Insert dividers
            // --------------------------------------------------------------------------
            int len = endeme.Length;
            if (divs >= 1 && len >= 10) endeme = Regex.Replace(endeme, "^(.......)", "$1-");
            if (divs >= 2 && len >= 14) endeme = Regex.Replace(endeme, "(.......)$", "-$1");
            if (divs >= 3 && len >=  3) endeme = Regex.Replace(endeme, "^(...)", "$1-");
            if (divs >= 4 && len >=  6) endeme = Regex.Replace(endeme, "(...)$", "-$1");
            if (divs >= 5 && len >= 18) endeme = Regex.Replace(endeme, "^(.............)", "$1-");

            return endeme.PadRight(width);
        }
    }
}
