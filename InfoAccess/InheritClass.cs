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
using System.Collections.Generic;     // for 
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.InfoAccess
{
	// --------------------------------------------------------------------------------------------
	/// <!-- InheritClass -->
    /// <summary>
    ///      Classes inherit from other classes, the InheritClass class contains metadata for them
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class InheritClass
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        GraphNode MyClass;
        GraphNode ParentClass;
        GraphEdge Inheritance;


        public InheritClass(string name, string parent)
        {
            MyClass     = new GraphNode(name  );
            ParentClass = new GraphNode(parent);
            Inheritance = new GraphEdge(ParentClass, MyClass, "Inherits");
        }

        public InheritClass(string myNamespace, string className, string parentNamespace, string parentName)
        {
            MyClass     = new GraphNode(className ); MyClass.Container     = myNamespace    ;
            ParentClass = new GraphNode(parentName); ParentClass.Container = parentNamespace;
            Inheritance = new GraphEdge(ParentClass, MyClass, "Inherits");
        }


        public override string ToString()
        {
            return MyClass.ToString() + Inheritance.Connector + ParentClass.ToString();
        }
    }
}
