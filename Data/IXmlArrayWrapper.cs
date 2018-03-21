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
using System.Collections.Generic;     // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
	// --------------------------------------------------------------------------------------------
	/// <!-- IXmlArrayWrapper -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>beta code - used once in production</remarks>
    public interface IXmlArrayWrapper
    {
        Dictionary<string,int> IdIndex   { get; }
        Dictionary<string,int> CodeIndex { get; }
        Dictionary<string,int> TextIndex { get; }
        bool   ContainsId  (string id);
        bool   ContainsCode(string code);
        bool   ContainsText(string text);


        int    TrueLength { get; }
        void   Init       (int length);
        IXmlArrayWrapper GetCopy();


        int    Length     { get; }
        void   SetLength  (int length);


        // ------------------------------------------------------------------------------
        //  This stuff may be dumb, maybe I should get rid of it?:
        // ------------------------------------------------------------------------------
        object GetById(string id);  void SetById(int index, IXmlArrayWrapper source, string id);
        string GetId  (int index);  void SetId  (int index, string value);
        string GetCode(int index);  void SetCode(int index, string value);
        string GetText(int index);  void SetText(int index, string value);
    }
}
