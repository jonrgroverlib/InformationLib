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
using System.Collections;             // for 
using System.Collections.Generic;     // for 
using System.Data;                    // for 
using System.Diagnostics;             // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // ---------------------------------------------------------------------------------------------
    /// <!-- EndemeMeaning -->
    /// <summary>
    ///      The EndemeMeaning class contains information about a particular EndemeCharacteristic
    ///      Meaning, It is also roughly parallel in level to a FuzzyTerm, although a FuzzyTerm can
    ///      be more than one Meaning
    /// </summary>
    /// <remarks>
    /// 
    ///      Situation 1:
    ///      ------------
    ///      If the FuzzyTerm measurement is the Meaning then
    ///      EndemeCharacteristic is parallel with FuzzyMeasure and
    ///      FuzzyTerm is above EndemeMeaning
    ///      In this case a FuzzyTerm can have multiple EndemeMeanings
    /// 
    ///      (EndemeNode)                 (?)
    ///         |                           |
    ///      (EndemeNodeConnector) = = = (FuzzyList)
    ///         |                           |
    ///      EndemeSet, (Endeme)            |
    ///      (EndemeItem)                   |
    ///         |                           |
    ///         |                           |
    ///      EndemeCharacteristic  = = = FuzzyMeasure, 
    ///         |                        (FuzzyItem)
    ///         |                           |
    ///         |                        FuzzyTerm
    ///         |
    ///      EndemeMeaning                
    /// 
    /// 
    /// 
    ///      Situation 2:
    ///      ------------
    ///      If the EndemeMeaning is a FuzzyTerm then
    ///      EndemeMeaning is parellel with FuzzyTerm and
    ///      the FuzzyMeasure is probably one Endeme Characteristic
    ///      but could be more than one characteristic
    /// 
    ///      (EndemeNode)                  (?)
    ///         |                           |
    ///      (EndemeNodeConnector) = = = (FuzzyList)
    ///         |                           |
    ///      EndemeSet, (Endeme)            |
    ///      (EndemeItem)                   |
    ///         |                        FuzzyMeasure, 
    ///         |                        (FuzzyItem)
    ///      EndemeCharacteristic           |
    ///         |                           |
    ///         |                           |
    ///      EndemeMeaning = = = = = = = FuzzyTerm
    /// 
    /// 
    ///      Which situation is the more normal?
    ///      probably the first one, in the first one various endeme Meanings would be in various places on the measure spectrum
    ///      
    /// 
    ///      alpha code, a stub of a class not ready to build yet
    /// </remarks>
    public class EndemeMeaning
    {

        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public Int16 Position { get; set; }
        /// <summary>good for connection with FuzzyTerms terms</summary>
        public string TextMeaning { get; set; }
        /// <summary>good for connection with FuzzyTerms values</summary>
        public double NumericMeaning { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeMeaning()
        {
            TextMeaning = "";
            Position = 0;
        }
        public EndemeMeaning(Int16 positionIdx, string posMeaning)
        {
            TextMeaning = posMeaning;
            if ((0 <= positionIdx & positionIdx <= 32))
            {
                Position = positionIdx;
            }
            else
            {
                throw new ArgumentOutOfRangeException("EndemeMeaning positions must be between 0 and 32, you passed " + positionIdx);
            }
        }

    }

}
