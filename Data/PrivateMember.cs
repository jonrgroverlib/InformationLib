//-------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation, either version 3
// of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//-------------------------------------------------------------------------------------------------
using InformationLib.SoftData  ;      // for TreatAs.StrValue
using System;
using System.Collections.Generic;
using System.IO;                      // for FileInfo
using System.Linq;
using System.Reflection;              // for BindingFlags
using System.Text;

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- PrivateMember -->
    /// <summary>
    ///      The PrivateMember class is mainly a group of reflection helper methods for testing 
    /// </summary>
    public class PrivateMember
    {
        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        /* ------------------------------------------------------------------------------------- */ /// <summary>Determines whether a particular file exists in the location identified by a folder path in a string member</summary>
        //public static bool FileExistsAt(object inObject, string folderMemberName, string fileName) { return FileAt(inObject, folderMemberName, fileName).Exists  ; }
        //public static void DeleteFileAt(object inObject, string folderMemberName, string fileName) {        FileAt(inObject, folderMemberName, fileName).Delete(); }


        // ----------------------------------------------------------------------------------------
        /// <!-- BoolValueSet -->
        /// <summary>
        ///      Changes a boolean member value
        /// </summary>
        /// <param name="inObject">the object containing the member</param>
        /// <param name="memberName">the name of the member</param>
        /// <param name="value"></param>
        /// <returns>the original boolean value or false if it was null or not boolean</returns>
        public static bool BoolValueSet(object inObject, string memberName, bool value)
        {
            FieldInfo member = inObject.GetType().GetField(memberName
                , BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.NonPublic
                | BindingFlags.Static);
            object before = member.GetValue(inObject);
            member.SetValue(inObject, value);
            return TreatAs.BoolValue(before, false);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FileAt -->
        /// <summary>
        ///      Returns infor for a particular file exists
        ///      in the location identified by a folder path contained in a string member
        /// </summary>
        /// <param name="inObject"></param>
        /// <param name="folderMemberName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileInfo FileAt(object inObject, string folderMemberName, string fileName)
        {
            if (string.IsNullOrEmpty(fileName.Trim())) fileName = Guid.NewGuid().ToString();
            string folder = PrivateMember.StrValue(inObject, folderMemberName);
            if (string.IsNullOrEmpty(fileName.Trim())) return new FileInfo(folder);
            else return new FileInfo(folder + "\\" + fileName);
        }
        public static DirectoryInfo FolderAt(object inObject, string rootMemberName, string folderName)
        {
            if (string.IsNullOrEmpty(folderName.Trim())) folderName = "JeSuScHrIsT";
            string root   = PrivateMember.StrValue(inObject, rootMemberName);
            if (string.IsNullOrEmpty(folderName.Trim())) return new DirectoryInfo(root);
            else return new DirectoryInfo(root + "\\" + folderName);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrValue -->
        /// <summary>
        ///      Grabs the string value of a private member, regardless of its type
        /// </summary>
        /// <param name="inObject">the object containing the member</param>
        /// <param name="memberName">the name of the member</param>
        /// <returns></returns>
        public static string StrValue(object inObject, string memberName)
        {
            FieldInfo member = inObject.GetType().GetField(memberName
                , BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.NonPublic
                | BindingFlags.Static);
            if (member == null) return "";
            else return TreatAs.StrValue(member.GetValue(inObject), "");
        }
    }
}
