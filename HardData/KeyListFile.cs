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
using System.Collections.Generic;     // for Dictionary, List
using System.IO;                      // for File, StreamWriter, TextReader
using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- KeyListFile -->
    /// <summary>
    ///      The KeyListFile class provides a parsed structure to contain the values in a list by their keys
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class KeyListFile : Dictionary<string,List<string>>
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private char   _keyDelimiter ;
        private char   _itemDelimiter;
        private string _fileInput    ;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public KeyListFile() { }
        public KeyListFile(char keyDelimiter, char itemDelimiter)
        {
            _keyDelimiter  = keyDelimiter ;
            _itemDelimiter = itemDelimiter;
        }
        public KeyListFile(string fileInput, char keyDelimiter, char itemDelimiter)
        {
            _fileInput     = fileInput    ;
            _keyDelimiter  = keyDelimiter ;
            _itemDelimiter = itemDelimiter;
            Parse(fileInput);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ConstructOrderedReport -->
        /// <summary>
        ///      Converts a dictionary of keys and value list into a string which may be output into a file
        ///      We are assuming all ordering is in string order not numeric order
        /// </summary>
        /// <param name="valueToKeyList"></param>
        /// <returns></returns>
        public string ConstructOrderedReport(bool keysInNumericOrder, bool valuesInNumericOrder)
        {
            List<string> keyList = new List<string>(Keys);

            if (keysInNumericOrder)
                keyList.OrderBy(c => int.Parse(c)).ToList();
            else
                keyList.Sort();

            StringBuilder str = new StringBuilder();
            string lineDelim = "";


            foreach (string key in keyList)
            {
                str.Append(lineDelim + key + _keyDelimiter);


                // ----------------------------------------------------------------------
                //  Sort the list of values (although the input test file numbers are ordered, I do not assume this will always be the case)
                // ----------------------------------------------------------------------
                List<string> valueList = this[key];
                if (valuesInNumericOrder)
                    valueList.OrderBy(c => int.Parse(c)).ToList();
                else
                    valueList.Sort();


                foreach (string value in valueList)
                {
                    str.Append("" + _itemDelimiter + value);
                }


                lineDelim = "\r\n";
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConvertValuesAndKeys -->
        /// <summary>
        ///      Returns a KeyListFile identifying which values are used by which keys and 
        /// </summary>
        /// <returns></returns>
        public KeyListFile ConvertValuesAndKeys(char keyDelimiter, char itemDelimiter)
        {
            // --------------------------------------------------------------------------
            //  Initialize the conversion
            // --------------------------------------------------------------------------
            KeyListFile valuesBecomeKeys = new KeyListFile(keyDelimiter, itemDelimiter);

            foreach (string formerKey in this.Keys)
            {
                List<string> formerValue = this[formerKey];
                for (int i = 0; i < formerValue.Count; ++i)
                {
                    if (!valuesBecomeKeys.ContainsKey(formerValue[i]))
                        valuesBecomeKeys.Add(formerValue[i],new List<string>());
                }
            }

            // --------------------------------------------------------------------------
            //  Fill the conversion
            // --------------------------------------------------------------------------
            foreach (string formerKey in this.Keys)
            {
                List<string> formerValue = this[formerKey];
                for (int i = 0; i < formerValue.Count; ++i)
                    valuesBecomeKeys[formerValue[i]].Add(formerKey);
            }

            return valuesBecomeKeys;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetStringFromFile -->
        /// <summary>
        ///      Gets a non-humongous string from a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static string GetStringFromFile(string filePath, string defaultContent)
        {
            string content;
            TextReader reader;


            try
            {
                reader = new StreamReader(filePath);
                content = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                content = ex.Message;
                content = defaultContent;
            }

            return content;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Parse -->
        /// <summary>
        ///      Parses a string from a file where each line has a key followed by a specified delimiter
        ///      followed by a list of items delimited by specified delimiters
        /// </summary>
        /// <param name="fileInput"></param>
        /// <param name="keyDelimiter"></param>
        /// <param name="itemDelimiter"></param>
        /// <returns></returns>
        /// <remarks>WARNING: there is no protection against having the delimiters in the content of the file</remarks>
        private void Parse(string fileInput)
        {
            string[] line = fileInput.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            for (int i = 0; i < line.Length; ++i)
            {
                // ----------------------------------------------------------------------
                //  Get the key and value list for the line
                // ----------------------------------------------------------------------
                line[i] = line[i].Trim();
                string   key       = Regex.Replace(line[i], "" + _keyDelimiter + ".*$", ""); // remove text starting with the delimiter
                string   valueList = Regex.Replace(line[i], "^.*" + _keyDelimiter     , ""); // remove text up to the delimiter
                string[] value     = valueList.Split((""+_itemDelimiter).ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


                //  Store the key and value list for the line
                List<string> val = new List<string>(value);
                this.Add(key, val);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TestKeyListFile -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="targetPath"></param>
        /// <remarks>
        ///      Usage:
        ///      KeyListFile.TestKeyListFile("C:\\docs\\input.txt", "C:\\docs\\output.txt");
        /// </remarks>
        public static void TestKeyListFile(string inputPath, string targetPath)
        {
            // --------------------------------------------------------------------------
            //  Produce a report of what keys contain what values in their value list
            // --------------------------------------------------------------------------
            string      fileInput        = KeyListFile.GetStringFromFile(inputPath, "");
            KeyListFile valuesforEachKey = new KeyListFile(fileInput, ':', ' ');
            KeyListFile keysForEachValue = valuesforEachKey.ConvertValuesAndKeys(':', ' ');
            string      report           = keysForEachValue.ConstructOrderedReport(false, true);


            // --------------------------------------------------------------------------
            //  Test and output the report
            // --------------------------------------------------------------------------
            string target = KeyListFile.GetStringFromFile(targetPath, "");
            if (report == target) Console.Write("success\r\n");
            else Console.Write("not quite\r\n");
            Console.Write(report);
        }
    }
}
