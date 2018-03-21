using InformationLib.Endemes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // for [Key]
using System.Linq;
using System.Text;

namespace InformationLib.DataModels
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeProfileTable -->
    /// <summary>
    ///      The EndemeProfileTable class contains objects that parallel the EndemeProfile table
    /// </summary>
    public class EndemeProfileTable
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
      //[Key]
        public Guid     EndemeProfileId  { get; set; }  // primary key
        public string   DataLabel        { get; set; }
        public string   DataValueString  { get; set; }
        public string   DataTableName    { get; set; }
        public int      DataRowId        { get; set; }
        public string   DataColumnName   { get; set; }
        public double   ProfileMatch     { get; set; }
        public string   ProfileTbdColumn { get; set; }
        public List<EndemeCharacteristic> EndemeCharacteristicList { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeProfileTable()
        {
        }

    }

}
