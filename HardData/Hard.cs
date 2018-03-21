using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationLib.HardData
{
    /// <summary>
    ///    The Hard class is the opposite of TreatAs, it throws exceptions when things don't convert properly
    /// </summary>
    public class Hard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="dateDescr"></param>
        /// <returns></returns>
        public static DateTime Date(string strDate, string dateDescr)
        {
            DateTime date;
            bool parseFrom = DateTime.TryParse(strDate, out date);
            if (parseFrom == false)
                throw new FormatException("Error: "+dateDescr+" is malformed.");
            return date;
        }
    }
}
