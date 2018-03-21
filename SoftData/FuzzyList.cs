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
	/// <!-- FuzzyList -->
	/// <summary>
	///      The FuzzyList class contains a list of FuzzyItems
	/// </summary>
	/// <remarks>
	/// 
	///      Parallel Levels of Complexity Among Various Information Structures:
	/// 
	/// 
	///      (EndemeNode)                  (?)
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
	///      EndemeValue                    
	/// 
	/// 
	///      Endeme structures and Fuzzy structures don't always map on to each other
	///      but sometimes they do, for example, sometimes a FuzzyTerm is equivalent to aa
	///      EndemeValue, and at other times a FuzzyMeasure is equivalent to an EndemeCharacteristic
	///      Non-equivalencies tend to drift up in the direction indicated
	///      FuzzyList is usually above EndemeList because it contains items of more than one measure
    ///      
    ///      alpha code
	/// </remarks>
	public class FuzzyList
	{

		// ----------------------------------------------------------------------------------------
		//  Members
		// ----------------------------------------------------------------------------------------
		/// <summary>The list itself</summary>

		private List<FuzzyItem> _list;

		// ----------------------------------------------------------------------------------------
		//  Constructors
		// ----------------------------------------------------------------------------------------
		private FuzzyList()
		{
			Init(22);
		}
		public FuzzyList(int capacity)
		{
			Init(capacity);
		}


		// ----------------------------------------------------------------------------------------
		/// <!-- Init -->
		/// <summary>
		///      Initializes stuff
		/// </summary>
		/// <param name="capacity"></param>
		public void Init(int capacity)
		{
			_list = new List<FuzzyItem>(capacity);
		}

	}

}