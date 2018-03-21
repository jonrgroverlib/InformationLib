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

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
	// ---------------------------------------------------------------------------------------------
	/// <!-- FuzzyItem -->
	/// <summary>
	///      The FuzzyItem class contains an instance of a FuzzyMeasure
	/// </summary>
	/// <remarks>
	/// 
	///      Parallel Levels of Complexity Among Various Information Structures:
	/// 
	///      (EndemeNode)                 (?)
	///         |                           |
	///      (EndemeNodeConnector) = = = (FuzzyList)
	///         |                           |
	///      EndemeSet, (Endeme)            |
	///      (EndemeItem)                   |
	///         |                        FuzzyMeasure, 
	///         |                  = ? = (FuzzyItem)
	///      EndemeCharacteristic           |
	///         |                           |
	///         |                        FuzzyTerm
	///         |                  = ? = 
	///      EndemeMeaning
	/// 
	/// 
	///      Endeme structures and Fuzzy structures don't always map on to each other
	///      but sometimes they do, for example, sometimes a FuzzyTerm is equivalent to an
	///      EndemeMeaning, and at other times a FuzzyMeasure is equivalent to an EndemeCharacteristic
	///      Non-equivalencies tend to drift up or down in the direction indicated
    ///      
    ///      alpha code
	/// </remarks>
	public class FuzzyItem
	{

		// -------------------------------------------------------------------------------
		//  Members
		// -------------------------------------------------------------------------------
		public FuzzyMeasure Measure { get; set; }
		public FuzzyTerm Value { get; set; }


		// -------------------------------------------------------------------------------
		//  Constructors
		// -------------------------------------------------------------------------------
		public FuzzyItem(FuzzyMeasure myMeasure)
		{
			Measure = myMeasure;
		}
		public FuzzyItem(FuzzyMeasure myMeasure, FuzzyTerm term)
		{
			Measure = myMeasure;
			Value = term;
		}


	}

}