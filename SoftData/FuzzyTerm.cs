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
	/// <!-- FuzzyTerm -->
	/// <summary>
	///      The FuzzyTerm class contains one amongst a number of terms in a FuzzySet
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
	///      EndemeValue          
	/// 
	/// 
	///      Endeme structures and Fuzzy structures don't always map on to each other
	///      but sometimes they do, for example, sometimes a FuzzyTerm is equivalent to aa
	///      EndemeValue, and at other times a FuzzyMeasure is equivalent to an EndemeCharacteristic
	///      Non-equivalencies tend to drift up or down in the direction indicated
    ///      
    ///      alpha code
	/// </remarks>
	public class FuzzyTerm
	{

		// -----------------------------------------------------------------------------------------
		//  Members and Properties
		// -----------------------------------------------------------------------------------------
		public string Term {
			get { return _term; }
		}

		private string _term;

		// -----------------------------------------------------------------------------------------
		//  Value Properties
		// -----------------------------------------------------------------------------------------
		public double Lowest { get; set; }
		public double Low { get; set; }
		public double High { get; set; }
		public double Highest { get; set; }


		// -----------------------------------------------------------------------------------------
		//  Constructor
		// -----------------------------------------------------------------------------------------
		public FuzzyTerm(string term)
		{
			_term = term;
			Lowest = 0.0;
			Low = 1.0;
			High = 2.0;
			Highest = 3.0;
		}
		public FuzzyTerm(string term, double lowestIn, double lowIn, double highIn, double highestIn)
		{
			_term = term;
			Lowest = lowestIn;
			Low = lowIn;
			High = highIn;
			Highest = highestIn;
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- AndTruth -->
		/// <summary>
		///      The 'AND' of two truth values
		/// </summary>
		/// <param name="tval1"></param>
		/// <param name="tval2"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static double AndTruth(double tval1, double tval2)
		{
			return tval1 * tval2;
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- Between -->
		/// <summary>
		///      Identifies whetehr the center number is between the outer numbers
		/// </summary>
		/// <param name="val1"></param>
		/// <param name="val2"></param>
		/// <param name="val3"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private bool Between(double val1, double val2, double val3)
		{
			if ((val1 < val2 & val2 < val3)) {
				return true;
			} else {
				return false;
			}
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- Copy -->
		/// <summary>
		///      Provides a copy of the fuzzy term
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public FuzzyTerm Copy()
		{
			FuzzyTerm newFuzzy = new FuzzyTerm(_term, Lowest, Low, High, Highest);
			return newFuzzy;
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- NotTruthh -->
		/// <summary>
		///      The 'NOT' of a truth value
		/// </summary>
		/// <param name="tval"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static double NotTruth(double tval)
		{
			return 1.0 - tval;
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- OrTruth -->
		/// <summary>
		///      The 'OR' of two truth values
		/// </summary>
		/// <param name="tval1"></param>
		/// <param name="tval2"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static double OrTruth(double tval1, double tval2)
		{
			return (1.0 - tval1) * (1.0 - tval2);
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- TruthValue -->
		/// <summary>
		///      Returns a number indicating the amount of truth in the input
		/// </summary>
		/// <param name="measure"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public double TruthValue(double measure)
		{
			if ((measure <= Lowest) | (Highest <= measure)) {
				return 0.0;
			} else if (Between(Lowest, measure, Low)) {
				return (measure - Lowest) / (Low - Lowest);
			} else if (Between(High, measure, Highest)) {
				return (Highest - measure) / (Highest - High);
			} else {
				return 1.0;
			}
		}

	}

}