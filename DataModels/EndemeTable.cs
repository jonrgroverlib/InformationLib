using InformationLib.Endemes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace InformationLib.DataModels
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeTable -->
    /// <summary>
    ///      The EndemeTable class contains objects that parallel the Endeme table
    /// </summary>
    public class EndemeTable
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
      //[Key]
        public int       EndemeId        { get; set; }  // primary key
        public Guid      EndemeSetId     { get; set; }
        public string    EndemeString    { get; set; }
        public string    EndemeCode      { get; set; }
        public string    EndemeLabel     { get; set; }
        public string    EndemeDescr     { get; set; }
        public bool      RawValue        { get; set; }
        public bool      MultipleUse     { get; set; }
        public byte[]    ValueBinary     { get; set; }
        public DateTime  ValueDateTime   { get; set; }
        public double    ValueFloat      { get; set; }
        public decimal   ValueNumber     { get; set; }
        public string    ValueText       { get; set; }
        public int       EndemeProfileId { get; set; }
        public EndemeSet EndemeSet       { get; set; }
        public List<EndemeIndex> EndemeIndexList { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeTable()
        {
        }

    }


}
