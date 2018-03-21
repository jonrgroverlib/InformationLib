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
using InformationLib.Endemes;         // for Endeme, EndemeSet
using System;                         // for Exception
using System.Collections.Generic;     // for Dictioanry, List
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Tally -->
    /// <summary>
    ///      The Tally class is a wrapper for Dictionary&lt;string,int> including a bunch of tally statistical methods
    /// </summary>
    /// <remarks>
    ///      this class is often overkill, unless you need the statistical methods,
    ///      it may just be best to use Dictionary&lt;string,int> directly
    ///      
    ///      beta code
    /// </remarks>
    public class Tally : Dictionary<string,int>
    {
        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public Tally() { }
        public Tally(int capacity) : base(capacity) { }


        // ----------------------------------------------------------------------------------------
        /// <!-- Average -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Average()
        {
            return ((double)TotalValue() / (double)TotalCount);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Collate -->
        /// <summary>
        ///      Collates the tallied keys into a string using the specified delimiter
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        public string Collate(string delim)
        {
            string str = "";
            string del = "";
            foreach (string key in this.Keys())
            {
                str += del + key;
                del = delim;
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Histogram -->
        /// <summary>
        ///      Returns an ASCII art histogram
        /// </summary>
        /// <returns></returns>
        public string Histogram(double divisor)
        {
            StringBuilder str = new StringBuilder();
            List<string> keys = Keys();
            string delim = "";
            int num;


            int width = (int)(this[Mode()] / divisor) + 1;
            int runningCount = 0;


            // --------------------------------------------------------------------------
            //  Turn the tally list into a histogram
            // --------------------------------------------------------------------------
            foreach (string key in keys)
            {
                int count = this[key];
                num = (int)((1.0 * count) / divisor + 0.5);
                str.Append(delim + key + "\t");
                str.Append(Stars(num, '*'));
                str.Append(Stars(width - num, ' '));
                str.Append("\t" + count);
                runningCount += count;
                str.Append("\t" + runningCount);
                delim = "\r\n";
            }


            return str.ToString();
        }


        public string Histogram()          { return Histogram(1.0); }
        public string Histogram(int width) { return Histogram((double)MaxTally/(double)width); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Incr -->
        /// <summary>
        ///      Increments one item in the tally,
        ///      adds the item if it is not there yet starting it with 1
        /// </summary>
        /// <param name="count"></param>
        /// <param name="msg"></param>
        public void Incr(string value)
        {
            if (ContainsKey(value))
                this[value]++;
            else Add(value, 1);
        }
        public void Incr(string value, bool includeBlanks)
        {
            if (includeBlanks || !string.IsNullOrEmpty(value))
                { if (ContainsKey(value)) this[value]++; else Add(value, 1); }
        }
        /// <summary>
        ///      Stores zero padded numbers as string labels so they can be sorted alphabetically,
        ///      if you don't want zero padding, turn them into strings first
        /// </summary>
        /// <param name="label"></param>
        /// <param name="zeroPaddingWidth">the width of the number when stored as a string and when displayed</param>
        public void Incr(int value, int zeroPaddingWidth)
        {
            Incr(value.ToString().PadLeft(zeroPaddingWidth, '0'));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Key -->
        /// <summary>
        ///      Returns key n where 0 is the highest key (in alphabetical order)
        /// </summary>
        /// <param name="num">0 is the highest</param>
        /// <returns></returns>
        /// <remarks>TODO: convert this to a List&lt;string> property</remarks>
        public string Key(int num)
        {
            List<string> keyList = Keys();
            if (num < keyList.Count)
                return keyList[keyList.Count-num-1];
            else return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Keys -->
        /// <summary>
        ///      Returns a sorted list of keys (overrides Keys), sorts alphabetically, or
        ///      numerically if you put them in with enough zero padding
        /// </summary>
        /// <returns></returns>
        public new List<string> Keys()
        {
            List<string> list = new List<string>(this.Count);
            foreach (string key in base.Keys)
                list.Add(key);
            list.Sort();
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Max -->
        /// <summary>
        ///      Returns the manimum (or final) key, works whether the numbers are labels or strings
        /// </summary>
        /// <returns></returns>
        public string Max()
        {
            List<string> key = Keys();
            return key[key.Count-1];
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MaxTally -->
        /// <summary>
        ///      Returns the maximum tally
        /// </summary>
        /// <returns></returns>
        public int MaxTally { get { return this[Mode()]; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Median -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Median()
        {
            List<string> keys = Keys();
            int halfWay = TotalCount / 2;
            string median = "";


            int runningTalley = 0;
            foreach (string key in keys)
            {
                runningTalley += this[key];
                if (runningTalley > halfWay)  { median = key;  break; }
            }
            return median;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Min -->
        /// <summary>
        ///      Returns the minimum (or first) key, works whether the numbers are labels or strings
        /// </summary>
        /// <returns></returns>
        public string Min()
        {
            List<string> key = Keys();
            if (key.Count > 0)  return key[0];
            else return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Mode -->
        /// <summary>
        ///      This works whether the labels are numbers or strings
        /// </summary>
        /// <returns></returns>
        public string Mode()
        {
            List<string> keys = Keys();


            string mode = "";
            int maxCount = 0;
            foreach (string key in keys)
                if (this[key] > maxCount)
                    { maxCount = this[key];  mode = key; }
            return mode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Report -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportTitle"></param>
        /// <returns></returns>
        public string Report(string reportTitle, int width)
        {
            try
            {
                StringBuilder str = new StringBuilder(reportTitle);
                str.Append("\r\n");
                str.Append("\r\n" + ToString());
                str.Append("\r\n");
                double total = TotalValue(); str.Append("\r\nTotal:   " + total);
                int    count = TotalCount;   str.Append("\r\nCount:   " + count);
                int    items = Count;        str.Append("\r\nItems:   " + items);
                string min   = Min();        str.Append("\r\nMin:     " + min  + " (" + this[min]  + ")");
                double mean  = Average();    str.Append("\r\nMean:    " + mean);
                string med   = Median();     str.Append("\r\nMedian:  " + med  + " (" + this[med]  + ")");
                string mode  = Mode();       str.Append("\r\nMode:    " + mode + " (" + this[mode] + ")");
                string max   = Max();        str.Append("\r\nMax:     " + max  + " (" + this[max]  + ")");
                str.Append("\r\n");
                str.Append("\r\n" + Histogram(width));
                return str.ToString();
            }
            catch (Exception ex)
            {
                return "Tally.Report encountered an error - " + ex.Message;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Stars -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="cha"></param>
        /// <returns></returns>
        public static string Stars(int num, char cha)
        {
            string stars = "";
            for (int i = 0; i < num; ++i)
                stars += cha;
            return stars;
        }

        public List<string> ToList()
        {
            List<string> list = new List<string>();
            foreach (string key in this.Keys())
            {
                list.Add(key);
            }
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Turns the tally list into a comma and colon delimited string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder(Count);
            string delim = "";


            // --------------------------------------------------------------------------
            //  Do stuff
            // --------------------------------------------------------------------------
            List<string> list = Keys();
            foreach (string key in list)
            {
                str.Append(delim + key + ":" + this[key]);
                delim = ", ";
            }

            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndeme -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Endeme ToEndeme()
        {
            Endeme en = new Endeme();
            if (Keys().Count <= 24)
            {
                Dictionary<char,string> keyLetters = new Dictionary<char,string>();
                //  build a set by getting a letter for every key
                EndemeSet set = new EndemeSet("Tally", Keys());
                //  TODO: write this - build the endeme
            }
            return en;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Top3 -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>just for testing</remarks>
        public string Top3()
        {
            List<string> keys = Keys();
            string top = keys[keys.Count-1] + " " + keys[keys.Count-2] + " " + keys[keys.Count-3];
            return top;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TotalCount -->
        /// <summary>
        ///      Returns the total tally by adding up all the sub-tallys, works whether the numbers
        ///      are labels or strings
        /// </summary>
        /// <returns></returns>
        public int TotalCount
        {
            get
            {
                List<string> keys = Keys();
                int total = 0;
                foreach (string key in keys)
                    total += this[key];
                return total;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TotalValue -->
        /// <summary>
        ///      Returns the total tally by adding up all the sub-tallys, only works if the labels
        ///      are numbers
        /// </summary>
        /// <returns></returns>
        public int TotalValue()
        {
            List<string> keys = Keys();
            int total = 0;
            foreach (string key in keys)
            {
                int val = 0;
                int value;
                if (int.TryParse(key, out value))
                    val = value;
                total += val * this[key];
            }
            return total;
        }
    }

    //// --------------------------------------------------------------------------------------------
    ///// <!-- EndemeTestTally -->
    ///// <summary>
    ///// 
    ///// </summary>
    //public class EndemeTestTally : Dictionary<string,int>
    //{
    //    // ----------------------------------------------------------------------------------------
    //    //  Constructors
    //    // ----------------------------------------------------------------------------------------
    //    public EndemeTestTally() { }
    //    public EndemeTestTally(int capacity) : base(capacity) { }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Average -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public double Average()
    //    {
    //        return ((double)TotalValue() / (double)TotalCount);
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Histogram -->
    //    /// <summary>
    //    ///      Returns an ASCII art histogram
    //    /// </summary>
    //    /// <returns></returns>
    //    public string Histogram(double divisor)
    //    {
    //        StringBuilder str = new StringBuilder();
    //        List<string> keys = Keys();
    //        string delim = "";
    //        int num;


    //        int width = (int)(this[Mode()] / divisor) + 1;
    //        int runningCount = 0;


    //        // --------------------------------------------------------------------------
    //        //  Turn the EndemeTestTally list into a histogram
    //        // --------------------------------------------------------------------------
    //        foreach (string key in keys)
    //        {
    //            int count = this[key];
    //            num = (int)((1.0 * count) / divisor + 0.5);
    //            str.Append(delim + key + "\t");
    //            str.Append(Stars(num, '*'));
    //            str.Append(Stars(width - num, ' '));
    //            str.Append("\t" + count);
    //            runningCount += count;
    //            str.Append("\t" + runningCount);
    //            delim = "\r\n";
    //        }


    //        return str.ToString();
    //    }


    //    public string Histogram()          { return Histogram(1.0); }
    //    public string Histogram(int width) { return Histogram((double)MaxTally/(double)width); }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Incr -->
    //    /// <summary>
    //    ///      Increments one item in the EndemeTestTally,
    //    ///      adds the item if it is not there yet starting it with 1
    //    /// </summary>
    //    /// <param name="count"></param>
    //    /// <param name="msg"></param>
    //    public void Incr(string value)
    //    {
    //        if (ContainsKey(value))
    //            this[value]++;
    //        else Add(value, 1);
    //    }
    //    public void Incr(string value, bool includeBlanks)
    //    {
    //        if (includeBlanks || !string.IsNullOrEmpty(value))
    //            { if (ContainsKey(value)) this[value]++; else Add(value, 1); }
    //    }
    //    /// <summary>
    //    ///      Stores zero padded numbers as string labels so they can be sorted alphabetically,
    //    ///      if you don't want zero padding, turn them into strings first
    //    /// </summary>
    //    /// <param name="label"></param>
    //    /// <param name="zeroPaddingWidth">the width of the number when stored as a string and when displayed</param>
    //    public void Incr(int value, int zeroPaddingWidth)
    //    {
    //        Incr(value.ToString().PadLeft(zeroPaddingWidth, '0'));
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Keys -->
    //    /// <summary>
    //    ///      Returns a sorted list of keys (overrides Keys), sorts alphabetically, or
    //    ///      numerically if you put them in with enough zero padding
    //    /// </summary>
    //    /// <returns></returns>
    //    private new List<string> Keys()
    //    {
    //        List<string> list = new List<string>(this.Count);
    //        foreach (string key in base.Keys)
    //            list.Add(key);
    //        list.Sort();
    //        return list;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Max -->
    //    /// <summary>
    //    ///      Returns the manimum (or final) key, works whether the numbers are labels or strings
    //    /// </summary>
    //    /// <returns></returns>
    //    public string Max()
    //    {
    //        List<string> key = Keys();
    //        return key[key.Count-1];
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- MaxTally -->
    //    /// <summary>
    //    ///      Returns the maximum EndemeTestTally
    //    /// </summary>
    //    /// <returns></returns>
    //    public int MaxTally { get { return this[Mode()]; } }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Median -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public string Median()
    //    {
    //        List<string> keys = Keys();
    //        int halfWay = TotalCount / 2;
    //        string median = "";


    //        int runningTalley = 0;
    //        foreach (string key in keys)
    //        {
    //            runningTalley += this[key];
    //            if (runningTalley > halfWay)  { median = key;  break; }
    //        }
    //        return median;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Min -->
    //    /// <summary>
    //    ///      Returns the minimum (or first) key, works whether the numbers are labels or strings
    //    /// </summary>
    //    /// <returns></returns>
    //    public string Min()
    //    {
    //        List<string> key = Keys();
    //        if (key.Count > 0)  return key[0];
    //        else return "";
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Mode -->
    //    /// <summary>
    //    ///      This works whether the labels are numbers or strings
    //    /// </summary>
    //    /// <returns></returns>
    //    public string Mode()
    //    {
    //        List<string> keys = Keys();


    //        string mode = "";
    //        int maxCount = 0;
    //        foreach (string key in keys)
    //            if (this[key] > maxCount)
    //                { maxCount = this[key];  mode = key; }
    //        return mode;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Report -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="reportTitle"></param>
    //    /// <returns></returns>
    //    public string Report(string reportTitle, int width)
    //    {
    //        try
    //        {
    //            StringBuilder str = new StringBuilder(reportTitle);
    //            str.Append("\r\n");
    //            str.Append("\r\n" + ToString());
    //            str.Append("\r\n");
    //            double total = TotalValue(); str.Append("\r\nTotal:   " + total);
    //            int    count = TotalCount;   str.Append("\r\nCount:   " + count);
    //            int    items = Count;        str.Append("\r\nItems:   " + items);
    //            string min   = Min();        str.Append("\r\nMin:     " + min  + " (" + this[min]  + ")");
    //            double mean  = Average();    str.Append("\r\nMean:    " + mean);
    //            string med   = Median();     str.Append("\r\nMedian:  " + med  + " (" + this[med]  + ")");
    //            string mode  = Mode();       str.Append("\r\nMode:    " + mode + " (" + this[mode] + ")");
    //            string max   = Max();        str.Append("\r\nMax:     " + max  + " (" + this[max]  + ")");
    //            str.Append("\r\n");
    //            str.Append("\r\n" + Histogram(width));
    //            return str.ToString();
    //        }
    //        catch (Exception ex)
    //        {
    //            return "EndemeTestTally.Report encountered an error - " + ex.Message;
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Stars -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="num"></param>
    //    /// <param name="cha"></param>
    //    /// <returns></returns>
    //    public static string Stars(int num, char cha)
    //    {
    //        string stars = "";
    //        for (int i = 0; i < num; ++i)
    //            stars += cha;
    //        return stars;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ToString -->
    //    /// <summary>
    //    ///      Turns the EndemeTestTally list into a comma and colon delimited string
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToString()
    //    {
    //        StringBuilder str = new StringBuilder(Count);
    //        string delim = "";


    //        // --------------------------------------------------------------------------
    //        //  Do stuff
    //        // --------------------------------------------------------------------------
    //        List<string> list = Keys();
    //        foreach (string key in list)
    //        {
    //            str.Append(delim + key + ":" + this[key]);
    //            delim = ", ";
    //        }


    //        return str.ToString();
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- TotalCount -->
    //    /// <summary>
    //    ///      Returns the total EndemeTestTally by adding up all the sub-tallys, works whether the numbers
    //    ///      are labels or strings
    //    /// </summary>
    //    /// <returns></returns>
    //    public int TotalCount
    //    {
    //        get
    //        {
    //            List<string> keys = Keys();
    //            int total = 0;
    //            foreach (string key in keys)
    //                total += this[key];
    //            return total;
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- TotalValue -->
    //    /// <summary>
    //    ///      Returns the total EndemeTestTally by adding up all the sub-tallys, only works if the labels
    //    ///      are numbers
    //    /// </summary>
    //    /// <returns></returns>
    //    public int TotalValue()
    //    {
    //        List<string> keys = Keys();
    //        int total = 0;
    //        foreach (string key in keys)
    //        {
    //            int val = 0;
    //            int value;
    //            if (int.TryParse(key, out value))
    //                val = value;
    //            total += val * this[key];
    //        }
    //        return total;
    //    }
    //}
}
