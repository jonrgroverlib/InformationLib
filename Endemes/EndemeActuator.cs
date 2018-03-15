//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with
// InformationLib. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- WeightFormula -->
    /// <summary>
    ///      Recommend 'Multiply' for most uses
    /// </summary>
    public enum WeightFormula { Distance, FullMedian, Levenschtein, Refined }


    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeActuator -->
    /// <summary>
    ///      The EndemeActuator class 'uses' an endeme register Has: chars, Ordered, Weighting, Sets (COWS)
    /// </summary>
    /// <remarks>
    ///      Endemes are a 'good enough' structure, If an endeme does not meet your heuristic, list
    ///      or structure needs, create or use a different structure.
    ///      
    ///      1. Set Logic Per Endeme
    ///          has X of (0,1,2…)
    ///          has all of (X=n)
    ///          has one of (X=1)
    ///          has none of (X=0)
    ///          has a few of (X=2to5&lt;n)
    ///          has a few of (X=4)
    ///          has all but one of (X=n-1)
    ///            - single characteristic
    ///            - characteristic combo
    ///            - different endeme length	[FACTOR]
    ///          Logic factor (0,1,2,3)	[FACTOR]
    ///      2. Order Logic (Regex) Per Endeme
    ///            - match by exact logic
    ///            - contains substring permutation
    ///            - scattered sub-permutation
    ///            - exact match
    ///            - same first three (first X)
    ///          Endeme Regex factor (0,1,2,3)	[FACTOR]
    ///      3. Weighting (Match) Per Endeme
    ///            - mutual characteristic       (factor)
    ///          Match Weight
    ///          in the high end/beginning	the beginning, High front end
    ///          in the low endthe end
    ///          in the end	High back end - full endeme
    ///          in the middle
    ///          in the upper middle	high middle - high position x
    ///          in the lower middle
    ///          Match By
    ///            - by linear distance	linear distance match
    ///            - by position multiplication	position multiplication match
    ///            - by Levenschtein distance
    ///            - by ?
    ///            - by ?
    ///            - by ?
    ///          Informed By	see weighting numbers
    ///            - Weighting Breadth	[FACTOR]
    ///            - Position Normalization	[FACTOR]
    ///          Complications
    ///            - direct match position count (factor)	[FACTOR]
    ///            - One missing	[FACTOR]
    ///            - Both missing	[FACTOR]
    ///          Endeme Weighting factor (0,1,2,3)	[FACTOR]
    ///          Deprecated
    ///            - anti-geometric match table  (factor)
    ///            - geometric match table       (factor)
    ///      4. Set Logic Per Endeme Set
    ///            - treat different endeme sets differently
    ///          has X of (0,1,2…)
    ///          has all of (X=n)
    ///          has one of (X=1)
    ///          has none of (X=0)
    ///          has a few of (X=2to5&lt;n)
    ///          has a few of (X=4)
    ///          has all but one of (X=n-1)
    ///            - single endeme set
    ///            - endeme set list
    ///            - One missing
    ///            - Both missing
    ///          Endeme Set Logic factor (0,1,2,3)	[FACTOR]
    ///      5?. Global Factors Per Endeme Set
    ///          information based logic?
    ///      
    ///    
    ///      Usually these items are additive, they may however override other results
    ///       - Is it better to have this information inside an item or globally?
    ///      
    /// 
    /// 
    ///      Endeme object/class/interface/table dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = { 0, 1, n(infinity) }     31 classes, 7 tables, 2 interfaces, 1 enum
    ///      ----------------------------------------------------
    ///
    ///           [-------------------- ACTION ------------------]    [-------------------------- STATE -------------------------]    [---------------------------- DATABASE ---------------------------------]           
    ///       _   |                                              |    |                                                          |    |                                                                       |   _     _ 
    ///        |                  EndemeActuator.cs                                                                                                                            EndemeAccess                  |       |
    ///  Info  |  |                                              |    |                                                          |    |                                              |                        |  | Info  |
    ///        |                                                                                                                                                                                                 |       |
    ///  Level |  |                                              |    |                         (objects)                        |    |       (tables)                               |                        |  | Level |
    ///        |                                                             +  -  -  -  -  -  -  - +  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  +--+--+--+--+--+--+-----+------+                    |       |
    ///       _|  |                                              |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |  |_     _|
    ///                                                                      |                 EndemeField (*,*,*)*n                                         |  |  |  |  |  |  |            |                             
    ///       _   |                                              |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |   _     _ 
    ///        |                                                             +----------------------+                                                        |  |  |  |  |  |  |            |                    |       |
    ///        |  |                                              |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                          +----------------+            EndemeList (1,1,1)*n   EndemeDefinition (n,n,n+m)*n                                  |  |  |  |  |  |  |            |                    |       |
    ///        |  |                       |                 \    |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                     EndemeActuator          +--------------------------------------+------------------------------------------+-------------+  |  |  |  |  |  |            |                    |       |
    ///        |  |                      /|\                     |    |      |                      |                            |    |       /                 |  |  |  |  |  |            |                 |  |       |
    ///        |                      ActuatorList                           |                      |                                   (endeme actuator table) |  |  |  |  |  |            |                    |       |
    ///  High  |  |                      /|\                     |    |      |                     /|\                           |    |  [~FACTOR,ACTUATOR]     |  |  |  |  |  |            |                 |  | High  |
    ///        |       +----------+-------+-------+-------+                  |                 EndemeObject (n,n,n+m)                                           |  |  |  |  |  |            |                    |       |
    ///  Level |  |    |          |       |       |       |      |    |      |                      |                            |    |                         |  |  |  |  |  |            |                 |  | Level |
    ///        |  |    |    WeightFactor  |   CharFactor  |                  |             +--------+------------------------------------------+----------------+  |  |  |  |  |            |                    |       |
    ///        |  |    |       |  |       |       |       |      |    |      |            /         |                            |    |       /                    |  |  |  |  |            |                 |  |       |
    ///        |    SetFactor  |  |  OrderFactor  |  ValueFactor             |           /     EndemeProfile (n,n,n)                    (endeme profile table)     |  |  |  |  |            |                    |       |
    ///        |  |    |  |    |  |       |       |    |  |  |   |    |      |          /           |           \|/              |    |  [stores PROFILE,OBJECT,   |  |  |  |  |            |                 |  |       |
    ///        |       |  +-------+-------+-------+----+---------------------+----------------------+    EndemeReference (n,0,0)          DEFINITION,FIELD]        |  |  |  |  |            |                    |       |
    ///        |  |    |       |          |               |  |   |    |               /            /|\             |             |    |                            |  |  |  |  |            |                 |  |       |
    ///       _|       |   WeightFormula  |               |  |                       /         EndemeItem (1,1,1)  |                                               |  |  |  |  |            |                    |_     _|
    ///           |    |                  |               |  |   |    |             /               |              |             |    |                            |  |  |  |  |            |                 |           
    ///                |            IEndemeActuatorFactor |  +------------+  +-----+    +------+----+              |                                               |  |  |  |  |            |                             
    ///       _   |    |                  |               |      |    |   |  |     |    |     /     |              |             |    |                            |  |  |  |  |            |                 |   _     _ 
    ///        |       |                  |               |               |  |  IEndemeItem  /      |              |                           +-------------------+  |  |  |  |            |                    |       |
    ///        |  |    |                  |               |      |    |   |  |              /       |  Endeme      |  Endeme     |    |       /                       |  |  |  |            |                 |  |       |
    ///        |       |                  |               |              EndemeValue (0,0,1)        |  TextFormat  |  GrabBag           (endeme index table)          |  |  |  |            |                    |       |
    ///        |  |    |                  |               |      |    |      |                      |       |      |       |     |    |  [indexes ENDEME,LIST]        |  |  |  |            |                 |  |       |
    ///        |       |                  +---------------+------------------+----------------------+-------+--------------+-------------------+----------------------+  |  |  |            |                    |       |
    ///        |  |    |                                         |    |                             |              |             |    |       /                          +  |  |            |                 |  |       |
    ///        |       |                                                                       Endeme (1,1,0)      |                    (endeme table) [stores LIST,    /   +  |            |                    |       |
    ///        |  |    |                                         |    |                       /    \|/            /|\            |    |  ENDEME,QUANTIF.,ITEM,VALUE]   /   /   +            |                 |  |       |
    ///        |       +----------------------------------------------------------------------------+--------------+---------------------------+----------------------+   /   /    +--------+-------+            |       |
    ///  Low   |  |                                              |    |                     /       |                            |    |       /                          +   /     |                |         |  | Low   |
    ///        |                                                                           /   EndemeSet (1,0,0)                        (endeme set table)               |  +  RichSqlCommand   RichDataTable    |       |
    ///  Level |  |                                              |    |                   /         |                            |    |  [stores SET]                    |  |      |     |          |   |     |  | Level |
    ///        |                                                             +-----------+          +--+---------------------------------------+-------------------------+  |      |     +----------+   |        |       |
    ///        |  |                                              |    |     /|\                    /|\                           |    |       /                             |      |     |              |     |  |       |
    ///        |                                                        EndemeQuantification   EndemeCharacteristic (0,0,0)             (endeme characteristic table)       |      |  InData     Is.    |        |       |
    ///        |  |                                              |    | (0,0,0)                     |                            |    |  [stores CHARACTERISTIC]            |      |  Throws     RandomSource |  |       |
    ///        |                                                                                    +------------------------------------------+----------------------------+      |  TreatAs.   FilesIO         |       |
    ///        |  |                                              |    |                            /|\                           |    |       /                                    |             __.          |  |       |
    ///       _|                                                                               EndemeMeaning (0,0,0)                    (endeme meaning table)                 ConnectSource                     |_     _|
    ///           |                                              |    |                                                          |    |  [stores MEANING]                                                     |           
    ///           [-------------------- ACTION ------------------]    [-------------------------- STATE -------------------------]    [---------------------------- DATABASE ---------------------------------]           
    /// </remarks>
    public class EndemeActuator
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        internal ActuatorList<SetFactor>    HasSets { get; private set; } // endeme set combinations
        internal ActuatorList<CharFactor>   HasChar { get; private set; } // endeme characteristic combinations
        internal ActuatorList<OrderFactor>  Ordered { get; private set; } // endeme characteristic permutations
        internal ActuatorList<WeightFactor> Weights { get; private set; } // endeme characteristic weightings
        internal ActuatorList<ValueFactor>  HasVals { get; private set; } // endeme item values - data defined by info


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeActuator()                                { Init(1,1,1,1,1,0);                        }
        public EndemeActuator(double endemeSetOrderImportance) { Init(1,1,1,1,1,endemeSetOrderImportance); }


        private void Init(double cFactor, double oFactor, double sFactor, double vFactor, double wFactor, double endemeSetOrderImportance)
        {
            HasChar = new ActuatorList<CharFactor>  (cFactor, 0);
            Ordered = new ActuatorList<OrderFactor> (oFactor, 0);
            HasSets = new ActuatorList<SetFactor>   (sFactor, endemeSetOrderImportance);
            HasVals = new ActuatorList<ValueFactor> (vFactor, 0);
            Weights = new ActuatorList<WeightFactor>(wFactor, 0);
        }


        // ----------------------------------------------------------------------------------------
        //  Factor builder, 'And' here is an English 'and' not a logical 'and' meaning 'and include this factor'
        /* ------------------------------------------------------------------------------------- */                                                                                                                          /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasOrder (string    tgtOrder                                                , bool required = true ) { Ordered.And(tgtOrder                                , 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasOrder (string    tgtOrder                          , int loEnd, int hiEnd, bool required = true ) { Ordered.And(tgtOrder                  , loEnd, hiEnd, 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasChars (Endeme    tgtChars, int loCount, int hiCount, int loEnd, int hiEnd, bool required = true ) { HasChar.And(tgtChars, loCount, hiCount, loEnd, hiEnd, 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasChars (Endeme    tgtChars, int loCount, int hiCount                      , bool required = true ) { HasChar.And(tgtChars, loCount, hiCount              , 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasChars (Endeme    tgtChars, int eqCount                                   , bool required = true ) { HasChar.And(tgtChars, eqCount                       , 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasChars (Endeme    tgtChars                                                , bool required = true ) { HasChar.And(tgtChars, tgtChars.Count                , 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasSet   (EndemeSet tgtEnSet                                                , bool required = true ) { HasSets.And(tgtEnSet, 1                             , 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasSet   (EndemeSet tgtEnSet, int loCount, int hiCount                      , bool required = true ) { HasSets.And(tgtEnSet, loCount, hiCount              , 1.0   , required); return this; } /// <summary>'Has' here tends toward being a requirement and is sort of like a logical 'and'</summary>
        public EndemeActuator HasValue (Endeme    tgtDefn , EndemeValue tgtValue                          , bool required = true ) { HasVals.And(tgtDefn , tgtValue, 1                   , 1.0   , required); return this; }
        public EndemeActuator NotValue (Endeme    tgtDefn , EndemeValue tgtValue                                                 ) { HasVals.And(tgtDefn , tgtValue, 0                   , 1.0   , true    ); return this; }
        public EndemeActuator ButNotSet(EndemeSet tgtEnSet                                                                       ) { HasSets.Not(tgtEnSet                                , 1.0   , true    ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndValue (Endeme    tgtDefn , EndemeValue tgtValue                          , double factor = 1.0  ) { HasVals.And(tgtDefn , tgtValue, 1                   , factor, false   ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndChars (Endeme    tgtChars, int eqCount                                   , double factor = 1.0  ) { HasChar.And(tgtChars, eqCount                       , factor, false   ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndChars (Endeme    tgtChars, int loCount, int hiCount                      , double factor = 1.0  ) { HasChar.And(tgtChars, loCount, hiCount              , factor, false   ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndSet   (EndemeSet tgtEnSet, int loCount, int hiCount                      , double factor = 1.0  ) { HasSets.And(tgtEnSet, loCount, hiCount              , factor, false   ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndSet   (EndemeSet tgtEnSet                                                , double factor = 1.0  ) { HasSets.And(tgtEnSet, 1                             , factor, false   ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndOrder (string    tgtOrder                                                , double factor = 1.0  ) { Ordered.And(tgtOrder                                , factor, false   ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndOrder (string    tgtOrder                        , int loEnd , int hiEnd , double factor = 1.0  ) { Ordered.And(tgtOrder                  , loEnd, hiEnd, factor, false   ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndWeight(Endeme    tgtMatch, WeightFormula weightFormula, int precision    , double factor = 1.0  ) { Weights.And(tgtMatch, 0, 22, factor, weightFormula, precision         ); return this; }
        public EndemeActuator AndWeight(Endeme    tgtMatch                        , int midPos, int radius, double factor = 1.0  ) { Weights.And(tgtMatch                , midPos, radius, factor          ); return this; } /// <summary>'And' here is an English 'and' sort of like 'and include this factor' not a logical 'and'</summary>
        public EndemeActuator AndWeight(Endeme    tgtMatch                        , int midPos, int radius, double factor, WeightFormula algorithm = WeightFormula.Refined, int precision = 7, int normalization = 0
            , int onemissing = 0, int bothmissing = 0, int lengthFactor = 0, int directMatch = 0)
            { Weights.And(tgtMatch, midPos, radius, factor, algorithm, precision, normalization, onemissing, bothmissing, lengthFactor, directMatch); return this; }


        // ----------------------------------------------------------------------------------------
        /// <!-- InitWeights -->
        /// <summary>
        /// 
        /// </summary>
        internal void InitWeights()
        {
            if (Weights.Count > 0)
            {
                foreach (WeightFactor wgt in Weights)
                {
                    wgt.BestMatch  = wgt.EnTarget.MatchBest (wgt.EnTarget         , wgt.Formula, wgt.Precision);
                    wgt.WorstMatch = wgt.EnTarget.MatchWorst(wgt.EnTarget.Opposite, wgt.Formula, wgt.Precision);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Returns counts of the actuator lists
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return "COSVW:" + Count(HasChar) + Count(Ordered) + Count(HasSets) + Count(HasVals) + Count(Weights); }
        private string Count(IList<IEndemeActuatorFactor> list) { if (0 <= list.Count && list.Count < 10) return list.Count.ToString(); else return "*"; }

    }

    // --------------------------------------------------------------------------------------------
    /// <!-- ActuatorList -->
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">a type implementing IEndemeActuatorFactor</typeparam>
    public class ActuatorList<T> : List<IEndemeActuatorFactor>, IList<IEndemeActuatorFactor>
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double OrderImportance  { get; set; }   // importance of the order of the factors within this list of factors
        public double FactorImportance { get; set; }   // importance of this kind of facto among all the factors


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public ActuatorList(double factorImportance, double orderImportance)
        {
            FactorImportance = factorImportance;
            OrderImportance  = orderImportance;
        }


        // ----------------------------------------------------------------------------------------
        //  Add is to be used only here
        /* ------------------------------------------------------------------------------------- */  /// <returns>the index of the factor just added</returns>
        private new int Add(IEndemeActuatorFactor item) { int idx = Count; base.Add(item); return idx; }


        // ----------------------------------------------------------------------------------------
        //  Various sorts of adding to an actuator list, 'And' here is an English 'and' not a logical 'and'
        /* ------------------------------------------------------------------------------------- */                                                                                                     /// <summary>match weight</summary>
        internal int And(Endeme    tgtMatch, int center , int radius                    , double factor, WeightFormula formula = WeightFormula.Refined, int precision = 7, int normalization=0, int onemissing=0, int bothmissing=0, int lengthFactor=0, int directMatch = 0)
                                                                                                                        { return Add(Factor(tgtMatch                  , center, radius, factor, formula, precision, normalization, onemissing, bothmissing, lengthFactor, directMatch)); } /// <summary>permutation</summary>
        internal int And(string    tgtOrder                                             , double factor, bool required) { return Add(Factor(tgtOrder                  , 1     , 22    , factor, required)); } /// <summary>permutation</summary>
        internal int And(string    tgtOrder                       , int loEnd, int hiEnd, double factor, bool required) { return Add(Factor(tgtOrder                  , loEnd , hiEnd , factor, required)); } /// <summary>logic</summary>
        internal int And(Endeme    tgtChars, int loCount, int hiCt, int loEnd, int hiEnd, double factor, bool required) { return Add(Factor(tgtChars, loCount, hiCt   , loEnd , hiEnd , factor, required)); } /// <summary>logic</summary>
        internal int And(Endeme    tgtChars, int loCount, int hiCt                      , double factor, bool required) { return Add(Factor(tgtChars, loCount, hiCt                   , factor, required)); } /// <summary>logic</summary>
        internal int And(Endeme    tgtChars, int loCount                                , double factor, bool required) { return Add(Factor(tgtChars, loCount, loCount                , factor, required)); } /// <summary>logic</summary>
        internal int And(EndemeSet tgtEnSet, int loCount                                , double factor, bool required) { return Add(Factor(tgtEnSet, loCount, loCount                , factor, required)); } /// <summary>logic</summary>
        internal int And(EndemeSet tgtEnSet, int loCount, int hiCt                      , double factor, bool required) { return Add(Factor(tgtEnSet, loCount, hiCt                   , factor, required)); } /// <summary>logic</summary>
        internal int And(Endeme    tgtDefn , EndemeValue tgtValue, int count            , double factor, bool required) { return Add(Factor(tgtValue, count  , tgtDefn                , factor, required)); }
        internal int Not(EndemeSet tgtEnSet                                             , double factor, bool required) { return Add(Factor(tgtEnSet, 0      , 0                      , factor, required)); }


        // ----------------------------------------------------------------------------------------
        //  Four different kinds of actuator - chars, order, weight, sets:
        // ----------------------------------------------------------------------------------------
        private IEndemeActuatorFactor Factor(string      tgtPattern                        , int loEnd, int hiEnd, double factor, bool required) { return new OrderFactor (factor, required) { Pattern     = tgtPattern                                       , LowPosition = loEnd, HighPosition = hiEnd}; }
        private IEndemeActuatorFactor Factor(Endeme      tgtChars, int loCount, int hiCount, int loEnd, int hiEnd, double factor, bool required) { return new CharFactor  (factor, required) { EnTarget    = tgtChars, LowCount = loCount, HighCount = hiCount, LowPosition = loEnd, HighPosition = hiEnd}; }
        private IEndemeActuatorFactor Factor(Endeme      tgtChars, int loCount, int hiCount                      , double factor, bool required) { return new CharFactor  (factor, required) { EnTarget    = tgtChars, LowCount = loCount, HighCount = hiCount                                           }; }
        private IEndemeActuatorFactor Factor(EndemeSet   tgtEnSet, int loCount, int hiCount                      , double factor, bool required) { return new SetFactor   (factor, required) { SetTarget   = tgtEnSet, LowCount = loCount, HighCount = hiCount                                           }; }
        private IEndemeActuatorFactor Factor(Endeme      tgtMatch, int center , int radius                       , double factor               ) { return new WeightFactor(factor, false   ) { EnTarget    = tgtMatch                                         , Center   = center  , Radius    = radius  }; }
        private IEndemeActuatorFactor Factor(EndemeValue tgtValue, int count               , Endeme tgtDefinition, double factor, bool required) { return new ValueFactor (factor, required) { ValueTarget = tgtValue, LowCount = count  , HighCount = count  , EnTarget = tgtDefinition                 }; }


        // ----------------------------------------------------------------------------------------
        /// <!-- Factor -->
        /// <summary>
        ///      The weight factor is a bit more complex than the other factors
        /// </summary>
        /// <param name="tgt"></param>        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="factor"></param>
        /// <param name="formula"></param>
        /// <param name="normalization"></param>
        /// <param name="onemissing"></param>
        /// <param name="bothmissing"></param>
        /// <param name="lengthFactor"></param>
        /// <param name="directMatch"></param>
        /// <returns></returns>
        private IEndemeActuatorFactor Factor(Endeme tgt, int center, int radius, double factor, WeightFormula formula
            , int precision, int normalization, int onemissing, int bothmissing, int lengthFactor, int directMatch)
        {
            WeightFactor item = (WeightFactor)Factor(tgt, center, radius, factor);

            item.Formula              = formula      ;
            item.Precision            = precision    ;
            item.Normalization        = normalization;
            item.OneMissing           = onemissing   ;
            item.BothMissing          = bothmissing  ;
            item.LengthFactor         = lengthFactor ;
            item.DirectMatchSubFactor = directMatch  ;

            return item;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CharMatch -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemProfile"></param>
        /// <returns></returns>
        internal double CharMatch(EndemeProfile itemProfile)
        {
            double charMatch = 0.0;
            if (this.Count > 0)
            {
                bool   charFound = true;
                double total     = 0.0;
                double totalMax  = 0.0;
                foreach (CharFactor tgt in this)
                {
                    int count = itemProfile.ContainsCount(itemProfile, tgt.EnTarget);
                    total    += tgt.Factor * count;
                    totalMax += tgt.Factor * tgt.HighCount;
                    charFound &= (!tgt.Required || tgt.LowCount <= count && count <= tgt.HighCount);
                }
                if (charFound && totalMax > 0) charMatch = total / totalMax;
                if (!charFound) charMatch = -1.0;
            }

            return charMatch;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SetMatch -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemProfile"></param>
        /// <returns></returns>
        internal double SetMatch(EndemeProfile itemProfile)
        {
            double setMatch = 0.0;
            if (this.Count > 0)
            {
                bool   setFound        = true;
                double total           = 0.0;
                double totalMax        = 0.0;
                bool   requiredMissing = false;
                foreach (SetFactor tgt in this)
                {
                    bool found = tgt.FoundInProfile(itemProfile);
                    if (found) total += 1;
                    if (tgt.Required && !found)
                        requiredMissing = true;
                    totalMax += 1;
                    setFound |= found;
                }
                if (setFound && totalMax > 0) setMatch = total / totalMax;
                if (requiredMissing) setMatch = -1.0;
            }

            return setMatch;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OrderMatch -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemProfile"></param>
        /// <returns></returns>
        internal double OrderMatch(EndemeProfile itemProfile)
        {
            double orderMatch = 0.0;

            if (this.Count > 0)
            {
                bool orderFound = true;
                double total = 0.0;
                double totalMax = 0.0;
                bool requiredMissing = false;

                foreach (OrderFactor tgt in this)
                {
                    bool found = tgt.FoundInProfile(itemProfile);
                    if (tgt.Required && !found)
                        requiredMissing = true;
                    totalMax += 1;
                    if (found) total += 1;
                    orderFound |= found;
                    if (orderFound && totalMax > 0)  orderMatch = total / totalMax;
                    if (requiredMissing) orderMatch = -1.0;
                }
            }

            return orderMatch;
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <!-- IEndemeActuatorFactor -->
    /// <summary>
    /// 
    /// </summary>
    public interface IEndemeActuatorFactor
    {
        // types of logic:
        double Factor       { get; set; } // normalize to 1.0 \_ if the factor != 1, required is usually false
        bool   Required     { get; set; } // usually false    /  if required is true, factor is usually 1.0
        Endeme EnTarget     { get; } // nominally primary
        string Pattern      { get; } // usually derived
                            
        // types of range:  
        int    Radius       { get; } // usually derived or constant
        int    Center       { get; } // usually derived or constant 
        int    LowPosition  { get; } // usually derived or constant
        int    HighPosition { get; } // usually derived or constant
        int    LowCount     { get; } // AND:has all of (X=n), OR:has one of (X=1), has none of (X=0), has a few of (X=2to5&lt;n), has a few of (X=4), has all but one of (X=n-1)
        int    HighCount    { get; } // AND:has all of (X=n), OR:has one of (X=1), has none of (X=0), has a few of (X=2to5&lt;n), has a few of (X=4), has all but one of (X=n-1)
    }

    // --------------------------------------------------------------------------------------------
    /// <!-- CharFactor -->
    /// <summary>
    ///      The CharFactor class actuates based on endeme characteristic existence (combinations)
    /// </summary>
    public class CharFactor : IEndemeActuatorFactor
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double Factor       { get; set; }
/* T */ public bool   Required     { get; set; }
/* d */ public Endeme EnTarget     { get; set; }
        public string Pattern      { get { return EnTarget.ToString(); } } // derived
                                   
        public int    Center       { get { return Math.Max(1, Math.Min(22, HighPosition - Radius               )); } } // derived
        public int    Radius       { get { return Math.Max(1, Math.Min(22, (1 + HighPosition - LowPosition) / 2)); } } // derived
        public int    LowPosition  { get; set; } // usually 1
        public int    HighPosition { get; set; } // usually 22
/* d */ public int    LowCount     { get; set; } // low logic value
/* d */ public int    HighCount    { get; set; } // hi logic value


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public CharFactor(                      double factor, bool required) { Init(1    , 22   , factor, required); }
        public CharFactor(int loEnd, int hiEnd, double factor, bool required) { Init(loEnd, hiEnd, factor, required); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowEnd"></param>
        /// <param name="highEnd"></param>
        /// <param name="factor"></param>
        /// <param name="required"></param>
        private void Init(int lowEnd, int highEnd, double factor, bool required)
        {
            Factor       = factor;
            HighPosition = highEnd;
            LowPosition  = lowEnd;
            Required     = required;
        }


        public override string ToString()
        {
            return "Has " + EnTarget.FullDesignation;
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <!-- OrderFactor -->
    /// <summary>
    ///      The OrderFactor class actuates based on endeme characteristic order (permutations)
    /// </summary>
    public class OrderFactor : IEndemeActuatorFactor
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double Factor       { get; set; }
        public bool   Required     { get; set; }
        public Endeme EnTarget     { get { return new Endeme(Pattern);              } } // derived
        public string Pattern      { get; set; }
                                   
        public int    Center       { get { return (HighPosition + LowPosition) / 2; } } // derived
        public int    Radius       { get { return 1 + HighPosition - LowPosition;   } } // derived
        public int    LowPosition  { get; set; }
        public int    HighPosition { get; set; }
        public int    LowCount     { get { return EnTarget.Count; } }
        public int    HighCount    { get { return 22;             } }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public OrderFactor(double factor, bool required)
        {
            Factor = factor;
            Required = required;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FoundInProfile -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        internal bool FoundInProfile(EndemeProfile profile)
        {
            bool found = false;
            string pattern = Pattern;
            if (!Regex.IsMatch(pattern, "[*]"))
                pattern = Regexize(pattern);
            foreach (EndemeItem segment in profile.Segment)
            {
                if (Regex.IsMatch(segment.ItemEndeme.ToString(), pattern))
                    found = true;
            }
            return found;
        }

        private string Regexize(string pattern)
        {
            string output = Regex.Replace(pattern, "(.)", "$1.*");
            return Regex.Replace(output, "..$", "");
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <!-- SetFactor -->
    /// <summary>
    ///      The SetFactor class actuates based on endeme set existence vs nonexistence (set combinations)
    /// </summary>
    public class SetFactor : IEndemeActuatorFactor
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double    Factor       { get; set; }
        public bool      Required     { get; set; }
        public string    Pattern      { get { return EnTarget.ToString();        } } // derived
        public Endeme    EnTarget     { get { return SetTarget.EnDefault;        } } // derived
        public EndemeSet SetTarget    { get; set; } // primary
                                      
        public int       Center       { get { return (HighCount + LowCount) / 2; } } // derived
        public int       Radius       { get { return 1 + HighCount - LowCount;   } } // derived
        public int       LowPosition  { get { return 1 ; } }
        public int       HighPosition { get { return 22; } }
        public int       LowCount     { get; set; } // 0 or 1
        public int       HighCount    { get; set; } // 0 or 1


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public SetFactor(double factor, bool required)
        {
            Factor = factor;
            Required = required;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FoundInProfile -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        internal bool FoundInProfile(EndemeProfile profile)
        {
            bool found = false;
            foreach (EndemeItem segment in profile.Segment)
                if (segment.EnSet.Label == this.SetTarget.Label)
                    found = true;
            return found;
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <!-- ValueFactor -->
    /// <summary>
    ///      The ValueFactor class actuates based on endeme item values
    /// </summary>
    public class ValueFactor : IEndemeActuatorFactor
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double      Factor       { get; set; }
        public bool        Required     { get; set; }
        public string      Pattern      { get { return ValueTarget.StrValue; } }
        public Endeme      EnTarget     { get; set; } // primary
        public EndemeSet   SetTarget    { get; set; } // primary
        public EndemeValue ValueTarget  { get; set; } // primary
                                      
        public int         Center       { get { return (HighCount + LowCount) / 2; } } // derived
        public int         Radius       { get { return 1 + HighCount - LowCount;   } } // derived
        public int         LowPosition  { get { return 1 ; } }
        public int         HighPosition { get { return 22; } }
        public int         LowCount     { get { return _lowCount ; } set { if (0 <= value && value <= 1) _lowCount  = value; else throw new ArgumentException("lowCount is out of range 0 to 1"); } } int _lowCount ; // 0 or 1
        public int         HighCount    { get { return _highCount; } set { if (0 <= value && value <= 1) _highCount = value; else throw new ArgumentException("highCount is out of range 0 to 1"); } } int _highCount; // 0 or 1


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public ValueFactor(double factor, bool required)
        {
            Factor = factor;
            Required = required;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FoundInProfile -->
        /// <summary>
        ///      Looks to see if this value is found in the input profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Profiles often contain string values of other types so string equality is checked first
        /// </remarks>
        internal bool FoundInProfile(List<EndemeItem> profile)
        {
            bool found = false;
            foreach (EndemeItem segment in profile)
            {
                if (segment.ItemEndeme == EnTarget)
                {
                    // ------------------------------------------------------------------
                    //  Check to see if the values are the 'same'
                    // ------------------------------------------------------------------
                    EndemeValue value = segment.Item;
                    bool sameValue = (value.IsString && value.StrValue == ValueTarget.StrValue);
                    if (!sameValue) sameValue = (value == ValueTarget);
                    found |= sameValue;
                }

                if (found) break;
            }
            return found;
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <!-- WeightFactor -->
    /// <summary>
    ///      The WeightFactor class actuates based on endeme weighting matches (weightings)
    /// </summary>
    public class WeightFactor : IEndemeActuatorFactor
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public double        Factor        { get; set; }
        public bool          Required      { get; set; } // expected to be false
        public Endeme        EnTarget      { get; set; }
        public string        Pattern       { get { return EnTarget.ToString();     } } // derived
                                           
        public int           Center        { get; set; } // primary // usually 20
        public int           Radius        { get; set; } // primary // usually 15
        public int           LowPosition   { get { return Math.Max(0 , Center - 2*Radius); } } // derived
        public int           HighPosition  { get { return Math.Min(22, Center + 2*Radius); } } // derived
        public int           LowCount      { get { return 0;                               } } // derived
        public int           HighCount     { get { return EnTarget.Count;                  } } // derived
                             
        public double        BestMatch     { get; set; }
        public double        WorstMatch    { get; set; }

        public WeightFormula Formula       { get; set; }
        public int           Precision     { get; set; } // a number between 0 and 20, recommend 7
        public int           Normalization { get; set; }
        public int           OneMissing    { get; set; }
        public int           BothMissing   { get; set; }
        public int           LengthFactor  { get; set; }
        public int    DirectMatchSubFactor { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public WeightFactor(double factor, bool required)
        {
            Factor = factor;
            Required = required;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CalculateFactorWeight -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        internal double CalculateFactorWeight(List<EndemeItem> profile)
        {
            double bestWeight = 0.0;
            foreach (EndemeItem segment in profile)
            {
                segment.TempMatch = EnTarget.Match(segment.ItemEndeme, Formula, Precision);
                bestWeight = Math.Max(bestWeight, segment.TempMatch);
            }
            return bestWeight;
        }

    }
}
