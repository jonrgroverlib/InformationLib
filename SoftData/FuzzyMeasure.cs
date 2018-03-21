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
	/// <!-- FuzzyMeasure -->
	/// <summary>
	///      The FuzzyMeasure class contains a set of fuzzy terms related by the same measurement
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
	public class FuzzyMeasure
	{

		// -----------------------------------------------------------------------------------------
		//  Members and Properties
		// -----------------------------------------------------------------------------------------
		/// <summary>Contains the various fuzzy terms related to the set's measurement</summary>
		public List<FuzzyTerm> Term {
			get { return _term; }
		}
		private List<FuzzyTerm> _term;
		/// <summary>name of the measurement</summary>
		public string Measurement {
			get { return _measurement; }
		}

		private string _measurement;

		// -----------------------------------------------------------------------------------------
		//  Constructor
		// -----------------------------------------------------------------------------------------
		public FuzzyMeasure(string name)
		{
			_term = new List<FuzzyTerm>();
			_measurement = name;
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- Add -->
		/// <summary>
		/// 
		/// </summary>
		/// <param name="termLabel"></param>
		/// <param name="lowestIn"></param>
		/// <param name="lowIn"></param>
		/// <param name="highIn"></param>
		/// <param name="highestIn"></param>
		/// <remarks></remarks>
		public void Add(string termLabel, double lowestIn, double lowIn, double highIn, double highestIn)
		{
			_term.Add(new FuzzyTerm(termLabel, lowestIn, lowIn, highIn, highestIn));
		}
		/// <param name="termItem">a fully formed FuzzyTerm</param>
		public void Add(FuzzyTerm termItem)
		{
			_term.Add(termItem);
		}
		/// <param name="termLabel">A term label without a definition</param>
		public void Add(string termLabel)
		{
			_term.Add(new FuzzyTerm(termLabel));
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- BestTerm -->
		/// <summary>
		///      Returns the most truthful term in the set
		/// </summary>
		/// <param name="measure"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public FuzzyTerm BestTerm(double measure)
		{
			FuzzyTerm found = new FuzzyTerm("dummy");
			double truth = 0.0;
			foreach (FuzzyTerm item in _term) {
				if ((item.TruthValue(measure) > truth)) {
					truth = item.TruthValue(measure);
					found = item.Copy();
				}
			}
			return found;
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- Best -->
		/// <summary>
		///      Returns the most truthful term in the set along with its truth value
		/// </summary>
		/// <param name="measure"></param>
		/// <param name="term">replaces this with the best term</param>
		/// <returns>The best term's truth value</returns>
		/// <remarks></remarks>
		public double Best(double measure, ref FuzzyTerm term)
		{
			term = BestTerm(measure);
			return BestTruthValue(measure);
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- BestTruthValue -->
		/// <summary>
		///      Returns the highest truthfulness in the set
		/// </summary>
		/// <param name="measure"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public double BestTruthValue(double measure)
		{
			double truth = 0.0;
			foreach (FuzzyTerm item in _term) {
				if ((item.TruthValue(measure) > truth)) {
					truth = item.TruthValue(measure);
				}
			}
			return truth;
		}


		// -----------------------------------------------------------------------------------------
		/// <!-- TruthList -->
		/// <summary>
		/// 
		/// </summary>
		/// <param name="measure"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public Dictionary<FuzzyTerm, double> TruthList(double measure)
		{
			Dictionary<FuzzyTerm, double> list = new Dictionary<FuzzyTerm, double>();

			foreach (FuzzyTerm item in _term) {
				list.Add(item.Copy(), item.TruthValue(measure));
			}

			return list;
		}

	}

}