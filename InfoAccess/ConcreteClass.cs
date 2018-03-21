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
	/// <!-- MemberListConcrete -->
    /// <summary>
    ///      members of a class - native L3
    /// </summary>
    /// <remarks>alpha code - deprecated by Graph etc.</remarks>
    public class MemberListConcrete
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        string MemberLabel    { get; set; }
        string ClassName      { get; set; }
        string ClassNamespace { get; set; }
        List<MemberListConcrete> LocalMember { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public MemberListConcrete(string myNamespace, string className                    ) { Init(myNamespace, className, ""         ); }
        public MemberListConcrete(string myNamespace, string className, string memberLabel) { Init(myNamespace, className, memberLabel); }

        private void Init(string myNamespace, string className, string memberLabel)
        {
            MemberLabel    = memberLabel;
            ClassNamespace = myNamespace;
            ClassName      = className  ;
            LocalMember    = new List<MemberListConcrete>();
        }

        public void Add(string memberClassNamespace, string memberClassName, string memberLabel)
        {
            LocalMember.Add(new MemberListConcrete(memberClassNamespace, memberClassName, memberLabel));
        }

        public override string ToString()
        {
            string members = "";
            string delim   = "";
            foreach (MemberListConcrete item in LocalMember)
            {
                members += (delim + item.MemberLabel);
                delim = ", ";
            }
            return ClassName + " : " + members;
        }
    }

    /// <summary>
    ///  native L2
    /// </summary>
    public class InheritClassConcrete //
    {
        string ClassName;
        string ClassNamespace;
        string ParentClassName;
        string ParentNamespace;

        public InheritClassConcrete(string name, string parent)
        {
            ClassName       = name  ;
            ParentClassName = parent;
        }

        public InheritClassConcrete(string myNamespace, string name, string parentNamespace, string parentName)
        {
            ClassNamespace  = myNamespace    ;
            ClassName       = name           ;
            ParentNamespace = parentNamespace;
            ParentClassName = parentName     ;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(ClassNamespace.Trim()) && string.IsNullOrEmpty(ParentNamespace.Trim()))
                return ClassName + " : " + ParentClassName;
            else return ClassNamespace + "." + ClassName + " : " + ParentNamespace + "." + ParentClassName;
        }
    }

    /// <summary>
    ///    native L1
    /// </summary>
    public class ForeignKeyConcrete //
    {
      //string DatabaseName  ;
      //string SchemaName    ;
        string TableName     ;
        string ColumnLabel   ;

      //string ToDatabaseName;
      //string ToSchemaName  ;
        string ToTableName   ;
        string ToPrimaryKey  ;

        public ForeignKeyConcrete(string tableName, string columnLabel, string toTableName, string toPrimaryKey)
        {
            TableName    = tableName   ;
            ColumnLabel  = columnLabel ;
            ToTableName  = toTableName ;
            ToPrimaryKey = toPrimaryKey;
        }

        public override string ToString()
        {
            return TableName + "." + ColumnLabel + " >- " + ToTableName + "." + ToPrimaryKey;
        }
    }
}
