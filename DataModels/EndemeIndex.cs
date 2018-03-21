using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // for [Key]
using System.Linq;
using System.Text;

namespace InformationLib.DataModels
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeIndex -->
    /// <summary>
    ///      The EndemeIndex class contains objects that parallel the EndemeIndex table
    /// </summary>
    public class EndemeIndex
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
      //[Key]
        public int         EndemeKeyId         { get; set; }  // primary key
        public int         EndemeLargeId       { get; set; }  // primary key
        public int         MatchStrength       { get; set; }
        public int         DocumentTransformId { get; set; }
        public Guid        EndemeSetId         { get; set; }
        public EndemeTable EndemeTable         { get; set; }
        public List<EndemeLarge> EndemeLargeList { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeIndex()
        {
        }

    }
}
