﻿//--------------------------------------------------------------------------------------------------
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    public class Soft<T> : ISoft
    {
        private List<ISoft> _valueChain;
        private object _value;
        public string StrValue {
            get { return TreatAs.StrValue(_value, ""); }
            set { _value = value; if (_valueChain == null) _valueChain = new List<ISoft>(); _valueChain.Add(this); } }
        public List<ISoft> Provenance { get; set; }

    }
}
