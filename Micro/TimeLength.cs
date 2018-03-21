//-------------------------------------------------------------------------------------------------
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
//-------------------------------------------------------------------------------------------------
using InformationLib.Endemes;         // for  
using System;                         // for  
using System.Collections.Generic;     // for 
//using System.Linq;                  // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for 

namespace InformationLib.Micro
{
    // --------------------------------------------------------------------------------------------
    /// <!-- TimeSpace -->
    /// <summary>
    ///      The TimeSpace class contains date-time-length-unit-timespan-schedule information
    /// </summary>
    public class TimeLength
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private DateTime _preciseDateTime;
        //private Decimal  _preciseLength  ;
        private Endeme   _dateTimeLength ;


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public TimeLength()
        {
        }
        public TimeLength(DateTime dt)
        {
            _preciseDateTime = dt;
            _dateTimeLength = new Endeme(TimeSet, "JK");


            _dateTimeLength.Add('A');
            if (dt.IsDaylightSavingTime()) _dateTimeLength.Add('S');
            _dateTimeLength.Add("NS");
        }


        /// <summary>
        /// 
        /// </summary>
        public List<string> WeekDays { get
        {
            if (_weekdays == null || _weekdays.Count < 1)
            {
                _weekdays = new List<string>(65);
                _weekdays.Add("DMTWJFS"     );  // D)omingo
                _weekdays.Add("D"           );  // M)onday
                _weekdays.Add("DM"          );  // T)uesday
                _weekdays.Add("DMT"         );  // W)ednesday
                _weekdays.Add("DMTW"        );  // J)ueves
                _weekdays.Add("DMTWJ"       );  // F)riday
                _weekdays.Add("DMTWJF"      );  // S)abado
                _weekdays.Add("D_T"         );
                _weekdays.Add("D__W"        );
                _weekdays.Add("D_T_J"       );
                _weekdays.Add( "MT"         );
                _weekdays.Add( "MTW"        );
                _weekdays.Add( "MTWJ"       );
                _weekdays.Add( "MTWJF"      );
                _weekdays.Add( "MTWJFS"     );
                _weekdays.Add( "M"          );
                _weekdays.Add( "M_W"        );
                _weekdays.Add( "M__J"       );
                _weekdays.Add( "M_W_F"      );
                _weekdays.Add(  "TW"        );
                _weekdays.Add(  "TWJ"       );
                _weekdays.Add(  "T"         );
                _weekdays.Add(  "TWJF"      );
                _weekdays.Add(  "TWJFS"     );
                _weekdays.Add(  "TWJFSD"    );
                _weekdays.Add(  "T__F"      );
                _weekdays.Add(  "T_J"       );
                _weekdays.Add(  "T_J_S"     );
                _weekdays.Add(   "WJ"       );
                _weekdays.Add(   "WJF"      );
                _weekdays.Add(   "WJFS"     );
                _weekdays.Add(   "WJFSD"    );
                _weekdays.Add(   "W"        );
                _weekdays.Add(   "WJFSDM"   );
                _weekdays.Add(   "W_F"      );
                _weekdays.Add(   "W__S"     );
                _weekdays.Add(   "W_F_D"    );
                _weekdays.Add(    "JF"      );
                _weekdays.Add(    "JFS"     );
                _weekdays.Add(    "JFSD"    );
                _weekdays.Add(    "JFSDM"   );
                _weekdays.Add(    "JFSDMT"  );
                _weekdays.Add(    "J"       );
                _weekdays.Add(    "J_S"     );
                _weekdays.Add(    "J__D"    );
                _weekdays.Add(    "J_S_M"   );
                _weekdays.Add(     "FS"     );
                _weekdays.Add(     "FSD"    );
                _weekdays.Add(     "F"      );
                _weekdays.Add(     "FSDM"   );
                _weekdays.Add(     "FSDMT"  );
                _weekdays.Add(     "FSDMTW" );
                _weekdays.Add(     "F_D"    );
                _weekdays.Add(     "F__M"   );
                _weekdays.Add(     "F_D_T"  );
                _weekdays.Add(      "SD"    );
                _weekdays.Add(      "SDM"   );
                _weekdays.Add(      "SDMT"  );
                _weekdays.Add(      "SDMTW" );
                _weekdays.Add(      "SDMTWJ");
                _weekdays.Add(      "S_M"   );
                _weekdays.Add(      "S__T"  );
                _weekdays.Add(      "S_M_W" );
                _weekdays.Add(      "S"     );
            }
            return _weekdays;
        } } private List<string> _weekdays;

        public List<TimeLengthUnit> TimeLengthUnit { get
        {
            if (_timeUnit == null || _timeUnit.Count < 1)
            {
                _timeUnit = new List<TimeLengthUnit>();
                _timeUnit.Add(new TimeLengthUnit("1.89E+14 planck"        ,          3.12E-21   ,            1.04E-29   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2.10E+14 planck"        ,          4E-21      ,            1.33E-29   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("6.25 zm"                ,          6.24E-21   ,            2.08E-29   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 zm"                   ,          8E-21      ,            2.67E-29   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("12.5 zm"                ,          1.25E-20   ,            4.16E-29   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 zm"                  ,          1.6E-20    ,            5.34E-29   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("25 zm"                  ,          2.49E-20   ,            8.32E-29   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 zm"                  ,          3.2E-20    ,            1.07E-28   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("50 zm"                  ,          4.98855E-20,            1.664E-28  , 'A'));
                _timeUnit.Add(new TimeLengthUnit("64 zm"                  ,          6.4E-20    ,            2.13E-28   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("100 zm"                 ,          9.97711E-20,            3.32801E-28, 'A'));
                _timeUnit.Add(new TimeLengthUnit("1/8 atm"                ,          1.25E-19   ,            4.17E-28   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("200 zm"                 ,          1.99542E-19,            6.65601E-28, 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 zm"                 ,          2.5E-19    ,            8.34E-28   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("higgs Boson"            ,          3.99084E-19,            1.3312E-27 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 zm"                 ,          5E-19      ,            1.67E-27   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("798 zm"                 ,          7.98168E-19,            2.66241E-27, 'A'));
                _timeUnit.Add(new TimeLengthUnit("attometer"              ,          1E-18      ,            3.34E-27   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.6 atm"                ,          1.59634E-18,            5.32481E-27, 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 atm"                  ,          2E-18      ,            6.67E-27   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.2 atm"                ,          3.19267E-18,            1.06496E-26, 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 atm"                  ,          4E-18      ,            1.33E-26   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("6.4 atm"                ,          6.38535E-18,            2.12992E-26, 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 atm"                  ,          8E-18      ,            2.67E-26   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("12.8 atm"               ,          1.27707E-17,            4.25985E-26, 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 atm"                 ,          1.6E-17    ,            5.34E-26   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("25.5 atm"               ,          2.55414E-17,            8.5197E-26 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 atm"                 ,          3.2E-17    ,            1.07E-25   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("51 atm"                 ,          5.10828E-17,            1.70394E-25, 'A'));
                _timeUnit.Add(new TimeLengthUnit("64 atm"                 ,          6.4E-17    ,            2.13E-25   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("102 atm"                ,          1.02166E-16,            3.40788E-25, 'A'));
                _timeUnit.Add(new TimeLengthUnit("1/8 fm"                 ,          1.25E-16   ,            4.17E-25   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("204 atm"                ,          2.04331E-16,            6.81576E-25, 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 atm"                ,          2.5E-16    ,            8.34E-25   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("409 atm"                ,          4.08662E-16,            1.36315E-24, 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 atm"                ,          5E-16      ,            1.67E-24   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("817.325 atm"            ,          8.17325E-16,            2.7263E-24 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("femtometer"             ,          1E-15      ,            3.34E-24   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("neutron"                ,          1.63465E-15,            5.45261E-24, 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 fm"                   ,          2E-15      ,            6.67E-24   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.2693 fm"              ,          3.2693E-15 ,            1.09052E-23, 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 fm"                   ,          4E-15      ,            1.33E-23   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("6.5386 fm"              ,          6.5386E-15 ,            2.18104E-23, 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 fm"                   ,          8E-15      ,            2.67E-23   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("13.077 fm"              ,          1.30772E-14,            4.36209E-23, 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 fm"                  ,          1.6E-14    ,            5.34E-23   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("26.15 fm"               ,          2.61544E-14,            8.72417E-23, 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 fm"                  ,          3.2E-14    ,            1.07E-22   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("52.3 fm"                ,          5.23088E-14,            1.74483E-22, 'A'));
                _timeUnit.Add(new TimeLengthUnit("64 fm"                  ,          6.4E-14    ,            2.13E-22   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("105 fm"                 ,          1.04618E-13,            3.48967E-22, 'A'));
                _timeUnit.Add(new TimeLengthUnit("1/8 pcm"                ,          1.25E-13   ,            4.17E-22   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("209 fm"                 ,          2.09235E-13,            6.97934E-22, 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 fm"                 ,          2.5E-13    ,            8.34E-22   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("418.5 fm"               ,          4.1847E-13 ,            1.39587E-21, 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 fm"                 ,          5E-13      ,            1.67E-21   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("836.94 fm"              ,          8.3694E-13 ,            2.79173E-21, 'A'));
                _timeUnit.Add(new TimeLengthUnit("picometer"              ,          1E-12      ,            3.34E-21   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.674 pm"               ,          1.67388E-12,            5.58347E-21, 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 pcm"                  ,          2E-12      ,            6.67E-21   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.348 pm"               ,          3.34776E-12,            1.11669E-20, 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 pcm"                  ,          4E-12      ,            1.33E-20   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("atom"                   ,          6.69552E-12,            2.23339E-20, 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 pcm"                  ,          8E-12      ,            2.67E-20   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("13.391 pm"              ,          1.3391E-11 ,            4.46678E-20, 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 pcm"                 ,          1.6E-11    ,            5.34E-20   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("26.78 pm"               ,          2.67821E-11,            8.93355E-20, 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 pcm"                 ,          3.2E-11    ,            1.07E-19   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("53.56 pm"               ,          5.35642E-11,            1.78671E-19, 'A'));
                _timeUnit.Add(new TimeLengthUnit("1 angstrom"             ,          6.4E-11    ,            2.13E-19   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("helix?"                 ,          1.07128E-10,            3.57342E-19, 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 angstrom"             ,          1.25E-10   ,            4.17E-19   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("214.257 pm"             ,          2.14257E-10,            7.14684E-19, 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 angstrom"             ,          2.5E-10    ,            8.34E-19   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("428.513 pm"             ,          4.28513E-10,            1.42937E-18, 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 pcm"                ,          5E-10      ,            1.67E-18   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("857.027 pm"             ,          8.57027E-10,            2.85874E-18, 'A'));
                _timeUnit.Add(new TimeLengthUnit("nanometer"              ,          0.000000001,            3.34E-18   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.71405 nm"             ,          1.71405E-09,            5.71747E-18, 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 nm"                   ,          0.000000002,            6.67E-18   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.42811 nm"             ,          3.42811E-09,            1.14349E-17, 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 nm"                   ,          0.000000004,            1.33E-17   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("6.85622 nm"             ,          6.85622E-09,            2.28699E-17, 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 nm"                   ,          0.000000008,            2.67E-17   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("13.7124 nm"             ,          1.37124E-08,            4.57398E-17, 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 nm"                  ,          0.000000016,            5.34E-17   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("virus?"                 ,          2.74249E-08,            9.14796E-17, 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 nm"                  ,          0.000000032,            1.07E-16   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("54.8497 nm"             ,          5.48497E-08,            1.82959E-16, 'A'));
                _timeUnit.Add(new TimeLengthUnit("64 nm"                  ,          0.000000064,            2.13E-16   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("109.7 nm"               ,          1.09699E-07,            3.65918E-16, 'A'));
                _timeUnit.Add(new TimeLengthUnit("emission from H atoms"  ,          0.000000125,            4.17E-16   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("219.4 nm"               ,          2.19399E-07,            7.31836E-16, 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 nm"                 ,          0.00000025 ,            8.34E-16   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("visible wavelength?"    ,          4.38798E-07,            1.46367E-15, 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 nm"                 ,          0.0000005  ,            1.67E-15   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("877.596 nm"             ,          8.77596E-07,            2.92735E-15, 'A'));
                _timeUnit.Add(new TimeLengthUnit("micrometer"             ,          0.000001   ,            3.34E-15   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.76 nm"                ,          1.75519E-06,            5.85469E-15, 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 mcm"                  ,          0.000002   ,            6.67E-15   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.51 mcm"               ,          3.51038E-06,            1.17094E-14, 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 mcm"                  ,          0.000004   ,            1.33E-14   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("cell?"                  ,          7.02076E-06,            2.34188E-14, 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 mcm"                  ,          0.000008   ,            2.67E-14   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("14.0 mcm"               ,          1.40415E-05,            4.68375E-14, 'A'));
                _timeUnit.Add(new TimeLengthUnit("mil"                    ,          2.54E-05   ,            8.47E-14   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("mil2"                   ,          2.80831E-05,            9.36751E-14, 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 mcm"                 ,          0.000032   ,            1.07E-13   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("56.2 mcm"               ,          5.61661E-05,            1.8735E-13 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("64 mcm"                 ,          0.000064   ,            2.13E-13   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("112.332 mcm"            ,          0.000112332,            3.747E-13  , 'A'));
                _timeUnit.Add(new TimeLengthUnit("hair"                   ,          0.000125   ,            4.17E-13   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("224.664 nm"             ,          0.000224664,            7.49401E-13, 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 mcm"                ,          0.00025    ,            8.34E-13   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("449.329 mcm"            ,          0.000449329,            1.4988E-12 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 mcm"                ,          0.0005     ,            1.67E-12   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("898.658 mcm"            ,          0.000898658,            2.9976E-12 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("millimeter"             ,          0.001      ,            3.34E-12   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("seed"                   ,          0.001797316,            5.9952E-12 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("line"                   ,          0.0021167  ,            7.06E-12   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.595 mm"               ,          0.003594631,            1.19904E-11, 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 mm"                   ,          0.004      ,            1.33E-11   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("7.189 mm"               ,          0.007189263,            2.39808E-11, 'A'));
                _timeUnit.Add(new TimeLengthUnit("centimeter"             ,          0.01       ,            3.34E-11   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("14.379 mm"              ,          0.014378525,            4.79616E-11, 'A'));
                _timeUnit.Add(new TimeLengthUnit("inch"                   ,          0.0254     ,            8.47E-11   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("inch2"                  ,          0.028757051,            9.59233E-11, 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 inches"               ,          0.0508     ,            1.69E-10   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("5.75141 cm"             ,          0.057514101,            1.91847E-10, 'A'));
                _timeUnit.Add(new TimeLengthUnit("decimeter"              ,          0.1        ,            3.34E-10   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("11.5028202 cm"          ,          0.115028202,            3.83693E-10, 'A'));
                _timeUnit.Add(new TimeLengthUnit("6 inches"               ,          0.2        ,            6.67E-10   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("23 cm"                  ,          0.230056404,            7.67386E-10, 'A'));
                _timeUnit.Add(new TimeLengthUnit("foot (~1ns)"            ,          0.299792227,            1.00E-09   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("cubit"                  ,          0.460112808,            1.53477E-09, 'A'));
                _timeUnit.Add(new TimeLengthUnit("yard"                   ,          0.899376681,            3.00E-09   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("yard2"                  ,          0.920225616,            3.06954E-09, 'A'));
                _timeUnit.Add(new TimeLengthUnit("meter"                  ,          1          ,            3.34E-09   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("fathom"                 ,          1.840451231,            6.13909E-09, 'A'));
                _timeUnit.Add(new TimeLengthUnit("shake"                  ,          2.99792227 ,            1.00E-08   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.680902462 m"          ,          3.680902462,            1.22782E-08, 'A'));
                _timeUnit.Add(new TimeLengthUnit("5 m"                    ,          5          ,            1.67E-08   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("rod2"                   ,          7.361804932,            2.45564E-08, 'A'));
                _timeUnit.Add(new TimeLengthUnit("dekameter"              ,         10          ,            3.34E-08   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("14.72360985 m"          ,         14.72360986 ,            4.91127E-08, 'A'));
                _timeUnit.Add(new TimeLengthUnit("chain"                  ,         20.1168     ,            6.71E-08   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("29.4472197 m"           ,         29.44721973 ,            9.82254E-08, 'A'));
                _timeUnit.Add(new TimeLengthUnit("40 m"                   ,         40          ,            0.000000133   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("58.8944394 m"           ,         58.89443945 ,            0.000000196451, 'A'));
                _timeUnit.Add(new TimeLengthUnit("80 m"                   ,         80          ,            0.000000267   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("furlong2"               ,         117.7888789 ,            0.000000392902, 'A'));
                _timeUnit.Add(new TimeLengthUnit("furlong"                ,         201.168     ,            0.000000671   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("235.6 m"                ,         235.5777578 ,            0.000000785803, 'A'));
                _timeUnit.Add(new TimeLengthUnit("microsecond"            ,         299.792227  ,            0.000001      , 'B'));
                _timeUnit.Add(new TimeLengthUnit("471 m"                  ,         471.1555156 ,            0.00000157161 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 mcs"                  ,         599.584454  ,            0.000002      , 'B'));
                _timeUnit.Add(new TimeLengthUnit("942 m"                  ,         942.3110313 ,            0.00000314321 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("mile (1609m)"           ,        1609.34      ,            0.00000537    , 'B'));
                _timeUnit.Add(new TimeLengthUnit("mile2 (1885m)"          ,        1884.622063  ,            0.00000628643 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 miles"                ,        3218.68      ,            0.0000107     , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3769 m"                 ,        3769.244125  ,            0.0000125729  , 'A'));
                _timeUnit.Add(new TimeLengthUnit("league"                 ,        6437.36      ,            0.0000215     , 'B'));
                _timeUnit.Add(new TimeLengthUnit("7538.5 m"               ,        7538.48825   ,            0.0000251457  , 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 miles"                ,       12874.72      ,            0.0000429     , 'B'));
                _timeUnit.Add(new TimeLengthUnit("50 mcs"                 ,       15076.9765    ,            0.0000502914  , 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 miles"               ,       25749.44      ,            0.000085891   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("league2"                ,       30153.953     ,            0.000100583   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 miles"               ,       51498.88      ,            0.000171782   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("201 mcs"                ,       60307.906     ,            0.000201166   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 mcs"                ,       74948.05675   ,            0.00025       , 'B'));
                _timeUnit.Add(new TimeLengthUnit("402 mcs"                ,      120615.812     ,            0.000402331   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 mcs"                ,      149896.1135    ,            0.0005        , 'B'));
                _timeUnit.Add(new TimeLengthUnit("805 mcs"                ,      241231.624     ,            0.000804663   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("millisecond"            ,      299792.227     ,            0.001         , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.61 ms"                ,      482463.247     ,            0.001609325   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 ms"                   ,      599584.454     ,            0.002         , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.22 ms"                ,      964926.494     ,            0.003218651   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 ms"                   ,     1199168.91      ,            0.004         , 'B'));
                _timeUnit.Add(new TimeLengthUnit("6.44 ms"                ,     1929852.99      ,            0.006437302   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("centisecond"            ,     2997922.27      ,            0.01          , 'B'));
                _timeUnit.Add(new TimeLengthUnit("12.875 ms"              ,     3859705.98      ,            0.012874603   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("earth radius"           ,     6378137.01      ,            0.021275191   , 'B'));
                _timeUnit.Add(new TimeLengthUnit("radia"                  ,     7719411.95      ,            0.025749207   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("1/32 ms"                ,     9368507.1       ,            0.03125       , 'B'));
                _timeUnit.Add(new TimeLengthUnit("51.5 ms"                ,    15438823.9       ,            0.051498413   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("1/16 ms"                ,    18737014.2       ,            0.0625        , 'B'));
                _timeUnit.Add(new TimeLengthUnit("103 ms"                 ,    30877647.8       ,            0.102996826   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("125 ms"                 ,    37474028.4       ,            0.125         , 'B'));
                _timeUnit.Add(new TimeLengthUnit("206 ms"                 ,    61755295.6       ,            0.205993652   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 ms"                 ,    74948056.7       ,            0.25          , 'B'));
                _timeUnit.Add(new TimeLengthUnit("hlfbeat"                ,   123510591.5       ,            0.411987305   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("half second"            ,   149896113.5       ,            0.5           , 'B'));
                _timeUnit.Add(new TimeLengthUnit("0.8234 s"               ,   247021183         ,            0.823974609   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("second"                 ,   299792227         ,            1             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.64795 s"              ,   494042366         ,            1.647949219   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 seconds"              ,   599584454         ,            2             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.2959 s"               ,   988084731         ,            3.295898438   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("5 seconds"              ,  1498961135         ,            5             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("6.5918 sec"             ,  1976169462         ,            6.591796875   , 'A'));
                _timeUnit.Add(new TimeLengthUnit("decasecond"             ,  2997922270         ,           10             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("13.184 s"               ,  3952338924         ,           13.18359375    , 'A'));
                _timeUnit.Add(new TimeLengthUnit("15 sec"                 ,  4496883405         ,           15             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("26.37 s"                ,  7904677848         ,           26.3671875     , 'A'));
                _timeUnit.Add(new TimeLengthUnit("30 sec"                 ,  8993766810         ,           30             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("52.7 s"                 , 15809355700         ,           52.734375      , 'A'));
                _timeUnit.Add(new TimeLengthUnit("minute"                 , 17987533620         ,           60             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("moment2"                , 31618711391         ,          105.46875       , 'A'));
                _timeUnit.Add(new TimeLengthUnit("120 sec"                , 35975067240         ,          120             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("211 sec"                , 63237422781         ,          210.9375        , 'A'));
                _timeUnit.Add(new TimeLengthUnit("moment"                 , 67453251075         ,          225             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("AU2"                    , 1.26475E+11         ,          421.875         , 'A'));
                _timeUnit.Add(new TimeLengthUnit("AU"                     , 1.50E+11            ,          499.012271      , 'B'));
                _timeUnit.Add(new TimeLengthUnit("ke2"                    , 2.5295E+11          ,          843.75          , 'A'));
                _timeUnit.Add(new TimeLengthUnit("ke"                     , 2.59E+11            ,          864             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("28.125 min"             , 5.05899E+11         ,         1687.5           , 'A'));
                _timeUnit.Add(new TimeLengthUnit("half hour"              , 5.39626E+11         ,         1800             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("56 minutes"             , 1.0118E+12          ,         3375             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("hour"                   , 1.07925E+12         ,         3600             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.9 hours"              , 2.0236E+12          ,         6750             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 hours"                , 2.1585E+12          ,         7200             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("225 minutes"            , 4.0472E+12          ,        13500             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 hours"                , 4.31701E+12         ,        14400             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("7.5    hrs"             , 8.09439E+12         ,        27000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("shift"                  , 8.63402E+12         ,        28800             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("15 hours"               , 1.61888E+13         ,        54000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("day"                    , 2.5902E+13          ,        86400             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("30 hours"               , 3.23776E+13         ,       108000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 days"                 , 5.18041E+13         ,       172800             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("60 hours"               , 6.47551E+13         ,       216000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 days"                 , 1.03608E+14         ,       345600             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("5 days"                 , 129510241856000     ,       432000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("week"                   , 1.81314E+14         ,       604800             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("10 days"                , 2.5902E+14          ,       864000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("fortnight"              , 3.62629E+14         ,      1209600             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("20 days"                , 5.18041E+14         ,      1728000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("month"                  , 7.88E+14            ,      2629746             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("40 days"                , 1.03608E+15         ,      3456000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 months"               , 1.58E+15            ,      5259492             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("80 days"                , 2.07216E+15         ,      6912000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("season/quarter"         , 2.37E+15            ,      7889238             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("160 days"               , 4.14433E+15         ,     13824000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("half year"              , 4.73E+15            ,     15778476             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("320 days"               , 8.28866E+15         ,     27648000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("year"                   , 9.46053E+15         ,     31556952             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("21 months"              , 1.65773E+16         ,     55296000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("parsec"                 , 3.09E+16            ,    102937959             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("42 months"              , 3.31546E+16         ,    110592000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("olympiad"               , 3.78E+16            ,    126227808             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("84 months"              , 6.63092E+16         ,    221184000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("decade"                 , 9.46E+16            ,    315569520             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("14 years"               , 1.32618E+17         ,    442368000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("indiction"              , 1.89E+17            ,    631139040             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("28 years"               , 2.65237E+17         ,    884736000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("jubilee"                , 4.73E+17            ,   1577847600             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("56 years"               , 5.30474E+17         ,   1769472000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("century"                , 9.46E+17            ,   3155695200             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("112 yrs"                , 1.06095E+18         ,   3538944000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("200 yrs"                , 1.89E+18            ,   6311390400             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("224 yrs"                , 2.1219E+18          ,   7077888000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("400 yrs"                , 3.78E+18            ,  12622780800             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("449 yrs"                , 4.24379E+18         ,  14155776000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 yrs"                , 4.73E+18            ,  15778476000             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("897 yrs"                , 8.48758E+18         ,  28311552000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("millenium"              , 9.46E+18            ,  31556952000             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1794 yrs"               , 1.69752E+19         ,  56623104000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 millenia"             , 1.89E+19            ,  63113904000             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3589 yrs"               , 3.39503E+19         , 113246208000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 millenia"             , 3.78E+19            , 126228000000             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("7177 yrs"               , 6.79007E+19         , 226000000000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 millenia"             , 7.57E+19            , 252456000000             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("14 millenia"            , 1.35801E+20         , 453000000000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 kyrs"                , 1.51E+20            , 504911000000             , 'B'));
                _timeUnit.Add(new TimeLengthUnit("29 kyrs"                , 2.71603E+20         , 906000000000             , 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 kyrs"                , 3.03E+20            , 1.00982E+12              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("57 kyrs"                , 5.43205E+20         , 1.81E+12                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("64 kyrs"                , 6.05E+20            , 2.01964E+12              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("115 kyrs"               , 1.08641E+21         , 3.62E+12                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("modern humans"          , 1.20E+22            , 4.00E+13                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("5th day"                , 2.17282E+21         , 7.25E+12                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 kyrs"               , 2.37E+21            , 7.88924E+12              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("459 kyrs"               , 4.34564E+21         , 1.45E+13                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 kyrs"               , 4.73E+21            , 1.57785E+13              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("919 kyrs"               , 8.69129E+21         , 2.90E+13                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("Ma"                     , 9.46E+21            , 3.1557E+13               , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.84 Ma"                , 1.73826E+22         , 5.80E+13                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 Ma"                   , 1.89E+22            , 6.31139E+13              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("4th day"                , 3.47651E+22         , 1.16E+14                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 Ma"                   , 3.78E+22            , 1.26228E+14              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("7.35 Ma"                , 6.95303E+22         , 2.32E+14                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("8 Ma"                   , 7.57E+22            , 2.52456E+14              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("14.7 Ma"                , 1.39061E+23         , 4.64E+14                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("16 Ma"                  , 1.51E+23            , 5.04911E+14              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("29.4 ma"                , 2.78121E+23         , 9.28E+14                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("32 Ma"                  , 3.03E+23            , 1.00982E+15              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3rd day"                , 5.56242E+23         , 1.86E+15                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("64 Ma"                  , 6.05E+23            , 2.01964E+15              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("118 Ma"                 , 1.11248E+24         , 3.71E+15                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("galactic year"          , 1.20E+24            , 4.00E+15                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("235 Ma(gy2)"            , 2.22497E+24         , 7.42E+15                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("250 Ma"                 , 2.37E+24            , 7.88924E+15              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("470 Ma"                 , 4.44994E+24         , 1.48E+16                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("500 Ma"                 , 4.73E+24            , 1.57785E+16              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("2nd day"                , 8.89988E+24         , 2.97E+16                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("1 Ga"                   , 9.46E+24            , 3.1557E+16               , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1.88 Ga"                , 1.77998E+25         , 5.94E+16                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("2 Ga"                   , 1.89E+25            , 6.31139E+16              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("3.763 Ga"               , 3.55995E+25         , 1.19E+17                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("4 Ga"                   , 3.78E+25            , 1.26228E+17              , 'B'));
                _timeUnit.Add(new TimeLengthUnit("7.5259 Ga"              , 7.1199E+25          , 2.37E+17                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("Hubble (14.4 Ga)"       , 1.36E+26            , 4.54E+17                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("1st day"                , 1.42398E+26         , 4.75E+17                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("28.2 Ga"                , 2.72E+26            , 9.09E+17                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("30 Ga"                  , 2.84796E+26         , 9.50E+17                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("56.4 Ga"                , 5.45E+26            , 1.82E+18                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("60 Ga"                  , 5.69592E+26         , 1.90E+18                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("112.8 Ga"               , 1.09E+27            , 3.64E+18                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("120 Ga"                 , 1.13918E+27         , 3.80E+18                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("225.6 Ga"               , 2.18E+27            , 7.27E+18                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("unity"                  , 2.27837E+27         , 7.60E+18                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("451.2 Ga"               , 4.36E+27            , 1.45E+19                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("482 Ga"                 , 4.55674E+27         , 1.52E+19                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("902 Ga"                 , 8.72E+27            , 2.91E+19                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("963 Ga"                 , 9.11347E+27         , 3.04E+19                 , 'A'));
                _timeUnit.Add(new TimeLengthUnit("1804 Ga"                , 1.74E+28            , 5.82E+19                 , 'B'));
                _timeUnit.Add(new TimeLengthUnit("stellar era"            , 1.82269E+28         , 6.08E+19                 , 'A'));
            }
            return _timeUnit;
        } }
        private List<TimeLengthUnit> _timeUnit; // 299792226.5

        // ----------------------------------------------------------------------------------------
        /// <!-- TimeSet -->
        /// <summary>
        /// 
        /// </summary>
        public static EndemeSet TimeSet { get
        {   if (                 _timeSet == null || _timeSet.Count < 1)
            { try {              _timeSet = new EndemeSet("TimeSpace");
                                 _timeSet.Add('A', "AD"         , "after"                                                                   ); // end
                                 _timeSet.Add('B', "B-ord0"     , ""                                                                        );
                                 _timeSet.Add('C', "Chances"    , "floating likelihood,never,always,percentages of the time,rarely,often"   );
                                 _timeSet.Add('D', "Duration"   , "floating precision,period,date only,datetime,range - timespan - interval");
                                 _timeSet.Add('E', "Ending"     , "floating past,before,yesterday - range direction,until"                  );
                                 _timeSet.Add('F', "From"       , "floating future,after,tomorrow - range direction, starting,beginning"    );
                                 _timeSet.Add('G', "G-units0"   , ""                                                                        );
                                 _timeSet.Add('H', "H-units1"   , ""                                                                        );
                                 _timeSet.Add('I', "I-units2"   , ""                                                                        );
                                 _timeSet.Add('J', "J-units3"   , ""                                                                        );
                                 _timeSet.Add('K', "K-units4"   , ""                                                                        );
                                 _timeSet.Add('L', "Local time" , ""                                                                        );
                                 _timeSet.Add('M', "M-ord1"     , ""                                                                        );
                                 _timeSet.Add('N', "Now"        , "today,current"                                                           ); // end
                                 _timeSet.Add('O', "Of"         , "ordinal,xofy,day of week,month,year,friday,time only"                    );
                                 _timeSet.Add('P', "P-ord2"     , ""                                                                        );
                                 _timeSet.Add('Q', "Q-ord3"     , ""                                                                        );
                                 _timeSet.Add('R', "Repeating"  , "floating period,frequency,hertz,wavelength,meters,periodic,period"       );
                                 _timeSet.Add('S', "Savings"    , "leap year,day,extra day,daylight savings time,extra hour"                ); // end
                                 _timeSet.Add('T', "Timezone"   , "floating vs unspecified timezone"                                        );
                                 _timeSet.Add('U', "Unspecified", "general (vs specific),unfixed,floating"                                  );
                                 _timeSet.Add('V', "V-gregorian", "floating Julian,roman,hebrew,hindu,mayan,regnal,Julian"                  );
            } catch { } } return _timeSet; } }
        private static EndemeSet _timeSet;
    }

    /// <summary>
    /// 
    /// </summary>
    public class TimeLengthUnit
    {
        public string Label   { get; set; }
        public double Meters  { get; set; }
        public double Seconds { get; set; }
        public char   UnitSet { get; set; }

        public TimeLengthUnit(string label, double meters, double seconds, char unitSet)
        {
            Label   = label  ;
            Meters  = meters ;
            Seconds = seconds;
            UnitSet = unitSet;
        }
    }
}






























































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































