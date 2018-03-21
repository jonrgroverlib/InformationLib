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
using Microsoft.VisualBasic;          // for 
using System;                         // for 
using System.Collections;             // for 
using System.Collections.Generic;     // for 
using System.Data;                    // for 
using System.Diagnostics;             // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.InfoAccess
{
    //// --------------------------------------------------------------------------------------------
    ///// <!-- KnowledgeTests -->
    ///// <summary>
    /////      Tables can relate through foreign keys, the KnowledgeTests class contains metadata for them
    ///// </summary>
    ///// <remarks>alpha code</remarks>
    //public class KnowledgeTests
    //{
    //    // -----------------------------------------------------------------------------------------
    //    //  Members
    //    // -----------------------------------------------------------------------------------------
    //    private        Result _result;
    //    private static string _conn;


    //    public string AllTests()
    //    {
    //        _result = "UNTESTED";
    //        string result = "";

    //        EndemeLeaf_Constructor_test();
    //        InfoPath_test              ();
    //        L4_test                    ();                                                                


    //        result += "\r\n" + "Knowledge tests done";
    //        return result;
    //    }

    //}


	// ---------------------------------------------------------------------------------------------
	/// <!-- Osmosis -->
	/// <summary>
	///      The Osmosis class handles osmotic conversion of data to information or back
	/// </summary>
	/// <remarks>No design as yet</remarks>
	public class Osmosis
	{

	}


	// ---------------------------------------------------------------------------------------------
	/// <!-- Standard -->
	/// <summary>
	///      The Standard class performs standard interactions with the Knowledge database or excel file (see Direct)
	/// </summary>
	/// <remarks>
	///      Namespace Dependency Hierarchy:
	///      
	/// 
	///           Data Apps        Info Apps                  (application types)
	///               |                |
	///               |                |
	///               |            InfoAccess
	///               |                |
	///          +----+----+  +--------+----------+           (advanced namespaces)
	///          |         |  |        |          |
	///          |      DataAccess     |          |
	///          |          |          |          |
	///          +----------+----------+          |
	///          |          |          |          |
	///      SoftData    Strings    Endemes    FuzzyLogic     (primitive namespaces)
	///          |          |          |          |
	///          +----------+-----+----+----------+
	///                           |
	///                        System.*
	/// 
    /// 
    ///      deprecated
	/// </remarks>
	public class Standard
	{

	}

}
