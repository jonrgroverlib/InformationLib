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
using System.Data;                    // for 
using System.IO;                      // for 
//using System.Linq;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- CsvFile -->
    /// <summary>
    ///      The CsvFile class handles interactions between csv files and data tables
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public class CsvFile
    {

        // ----------------------------------------------------------------------------------------
        /// <!-- InsureFileHeader -->
        /// <summary>
        ///      Insures that the csv file used has a header row, both creates the path and writes the copy file
        /// </summary>
        /// <param name="importFilePath1"></param>
        /// <param name="headerLine"></param>
        /// <param name="keepHeaders">
        ///      Indicates if to keep or strip off customer supplied headers
        /// </param>
        /// <returns></returns>
        public static string InsureFileHeader(string importFilePath1, string headerLine
            , bool keepHeaders, string copyOfprefix)
        {
            string importFilePath = importFilePath1;


            if (keepHeaders)
            {
                // ----------------------------------------------------------------------
                //  Come up with a new path for a new import file if there is no header row in the csv file
                // ----------------------------------------------------------------------
                string content = InData.GetStringFromFile(importFilePath1);

                if (string.IsNullOrEmpty(content.Trim()) ||
                    Regex.IsMatch(content.Substring(0, 1), "[0-9]"))
                {
                    importFilePath = InData.PrependToFileName(copyOfprefix, importFilePath1);
                    importFilePath = Regex.Replace(importFilePath, ".txt$", ".csv");
                }


                // ----------------------------------------------------------------------
                //  Prepend the header if it is needed
                // ----------------------------------------------------------------------
                if (string.IsNullOrEmpty(content.Trim()) ||
                    Regex.IsMatch(content.Substring(0, 1), "[0-9]"))
                {
                    // There is no header, add one
                    string content2 = headerLine + "\r\n" + content;
                    InData.WriteStringToFile(importFilePath, content2, false);
                }
            }
            else
            {
                string content1 = InData.GetStringFromFile(importFilePath1);
                string content2 = content1;
                if (!string.IsNullOrEmpty(content1.Trim()) && !Regex.IsMatch(content1.Substring(0, 1), "[0-9]"))
                {
                    //  There is a header, strip it off and add one
                    content2 = Regex.Replace(content1, "^[^\r\n]*[\r\n]*", "", RegexOptions.Singleline);
                }


                importFilePath = InData.PrependToFileName(copyOfprefix, importFilePath1);
                importFilePath = Regex.Replace(importFilePath, ".txt$", ".csv");


                // There is no header, add one
                string content3 = "";
                content3 = headerLine + "\r\n" + content2;
                InData.WriteStringToFile(importFilePath, content3, false);
            }


            return importFilePath;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ValidateFileExtension -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileExt"></param>
        /// <param name="strAllowedExt"></param>
        /// <returns></returns>
        public static bool ValidateFileExtension(string strFileExt, string[] strAllowedExt)
        {
            Boolean fileOK = false;

            if (strFileExt != string.Empty)
            {
                String fileExtension =
                    System.IO.Path.GetExtension(strFileExt).ToLower();
                //String[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg" };
                for (int i = 0; i < strAllowedExt.Length; i++)
                {
                    if (fileExtension == strAllowedExt[i])
                    {
                        fileOK = true;
                    }
                }
            }
            return fileOK;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteData -->
        /// <summary>
        ///      Writes Data To a Csv File
        /// </summary>
        /// <param name="data"></param>
        /// <param name="toFilePath"></param>
        /// <param name="strRows"></param>
        public static void WriteData(DataTable data, string toFilePath, string strRows)
        {
            int i = 0;
            StreamWriter strmWrtr = new StreamWriter(toFilePath, false);


            // --------------------------------------------------------------------------
            //  Write column headers
            // --------------------------------------------------------------------------
            for (i = 0; i < data.Columns.Count - 1; i++)
            {
                strmWrtr.Write(data.Columns[i].ColumnName + ",");
            }
            strmWrtr.Write(data.Columns[i].ColumnName);
            strmWrtr.WriteLine();


            // --------------------------------------------------------------------------
            //  Write cell data
            // --------------------------------------------------------------------------
            foreach (DataRow row in data.Rows)
            {
                object[] array = row.ItemArray;
                for (i = 0; i < array.Length - 1; i++)
                {
                    string cell = array[i].ToString();
                    if (Regex.IsMatch(array[i].ToString(), ","))
                        cell = @"""" + array[i].ToString() + @"""";
                    strmWrtr.Write(cell + ",");
                }
                strmWrtr.Write(array[i].ToString());
                strmWrtr.WriteLine();

            }
            strmWrtr.Write("Total Row(s) : " + strRows);
            strmWrtr.WriteLine();


            strmWrtr.Close();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteImportedCsvFile -->
        /// <summary>
        ///      Writes the 'imported' table to a csv file making sure the extension is .csv
        /// </summary>
        /// <param name="imported">a table of the records that have been imported</param>
        /// <param name="sourceImportData"></param>
        /// <param name="root"></param>
        public static void WriteImportedCsvFile(DataTable imported, DataTable sourceImportData, string path, int level)
        {
            path = Regex.Replace(path, ".txt$", ".csv");
            if ((imported != null) && (imported.Rows.Count > 0))
            {
                CsvFile.WriteData(imported, path, imported.Rows.Count + " out of " + sourceImportData.Rows.Count);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteInvalidCsvFile -->
        /// <summary>
        ///      Writes the 'invalid' table to a csv file making sure the extension is .csv
        /// </summary>
        /// <param name="invalid">a table of records that are invalid for one or more reasons</param>
        /// <param name="importData"></param>
        /// <param name="root"></param>
        public static void WriteInvalidCsvFile(DataTable invalid, DataTable importData, string path, int level)
        {
            path = Regex.Replace(path, ".txt$", ".csv");
            if ((invalid != null) && (invalid.Rows.Count > 0))
            {
                CsvFile.WriteData(invalid, path, invalid.Rows.Count + " out of " + importData.Rows.Count);
            }
        }


        public static StreamWriter strWtr { get; set; }
    }
}