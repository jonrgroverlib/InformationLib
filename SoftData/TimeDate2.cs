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
using System.IO;                      // for 
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex
//using System.Threading.Tasks;         // for 

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- TimeDate2 -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>a stub for the new TimeDate class if I ever build it</remarks>
    public class TimeDate2
    {
        // ------------------------------------------------------------------------------
        //  Members
        // ------------------------------------------------------------------------------
        private DateTime _dateTime { get; set; }
        private bool     _isNull   { get; set; }
        private decimal  _timeZone { get; set; }
        private bool     _isDlst   { get; set; }


        // ------------------------------------------------------------------------------
        //  Constructor
        // ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
        /// <!-- NewestFileDateByName -->
        /// <summary>
        ///      Identifies the most recent date based on the names of the files in the list
        /// </summary>
        /// <param name="fileList">file names have YYYYMMDDHHMMDD in them somewhere</param>
        /// <returns></returns>
        public static DateTime NewestFileDateByName(FileInfo[] fileList)
        {
            DateTime newest = TimeDate_old.MinSqlValue.CLRFormat;

            foreach (FileInfo fi in fileList)
            {
                string fileName = fi.Name;
                string strDate = Regex.Replace(fileName, "[^0-9]", "");
                strDate = Regex.Replace(strDate, "^(....)(..)(..)(..)(..)(.*)$", "$1/$2/$3 $4:$5:$6");
                DateTime? fileNameTime = TreatAs.NullableDateTimeValue(strDate, null);
                if (fileNameTime != null)
                {
                    if (fileNameTime > newest)
                        newest = (DateTime)fileNameTime;
                }
            }

            return newest;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OldestFileDateByName -->
        /// <summary>
        ///      Identifies the most recent date based on the names of the files in the list
        /// </summary>
        /// <param name="fileList">file names have YYYYMMDDHHMMDD in them somewhere</param>
        /// <returns></returns>
        public static DateTime OldestFileDateByName(FileInfo[] fileList)
        {
            DateTime oldest = DateTime.UtcNow;

            foreach (FileInfo fi in fileList)
            {
                string fileName = fi.Name;
                string strDate = Regex.Replace(fileName, "[^0-9]", "");
                strDate = Regex.Replace(strDate, "^(....)(..)(..)(..)(..)(.*)$", "$1/$2/$3 $4:$5:$6");
                DateTime? fileNameTime = TreatAs.NullableDateTimeValue(strDate, null);
                if (fileNameTime != null)
                {
                    if (fileNameTime < oldest)
                        oldest = (DateTime)fileNameTime;
                }
            }

            return oldest;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Max -->
        /// <summary>
        ///      Returns the most recent of two dates
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static DateTime Max(DateTime dateTime1, DateTime dateTime2)
        {
            DateTime maxDate;
            if (dateTime1 > dateTime2)
                 { maxDate = dateTime1; }
            else { maxDate = dateTime2; }
            return maxDate;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Min -->
        /// <summary>
        ///      Returns the oldest of two dates
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static DateTime Min(DateTime dateTime1, DateTime dateTime2)
        {
            DateTime minDate;
            if (dateTime1 < dateTime2)
                 { minDate = dateTime1; }
            else { minDate = dateTime2; }
            return minDate;
        }
    }
}
