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
using InformationLib.Testing;         // for RandomSource
using System;                         // for Math
using System.Collections.Generic;     // for List, Dictionary
using System.Linq;                    // for orderby
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeOrderingCreationType -->
    /// <summary>
    ///      Directions for building an endeme from an input or values
    /// </summary>
    /// <remarks>beta code</remarks>
    public enum EndemeOrderingCreationType { ByCharacterOrder, ByCharacterFrequency, ByRawValue }


    // --------------------------------------------------------------------------------------------
    /// <!-- Endeme -->
    /// <summary>
    ///      An endeme is a data structure for storing information, it is an instance of a
    ///      permutable combinable list of characteristics and works a bit like an ordered bitwise
    ///      enum, The first letter is generally considered position 0
    /// </summary>
    /// <remarks>
    ///      Data vs Information vs Talent
    /// 
    ///      The reason it is so hard to define the distinction between data and information is
    ///      that the distinction is partially based on the talent of the reader. The general (very loose)
    ///      definition of information is that 'information is data that has meaning to people'.
    ///      Different people have different talent for interpreting data and are therefore able to process data different
    ///      ways. So different people are able to understand different data sets. To them it is
    ///      information, to others it has no meaning and therefore it is only data.
    /// 
    ///      For example someone with high levels of inductive reasoning aptitude will be able to
    ///      process larger amounts of more diffuse data
    ///      into meaning that will a person with low inductive reasoning. Someone who is very
    ///      good at deduction will be able to take data in the form of logic and basic principles
    ///      and apply them to an understanding rooted in concrete systems where another person would
    ///      not find meaning in them. Someone good at recognizing particular pattern commonalities
    ///      might find that certain data show an obvious match whereas other people would not see
    ///      the match. In each case a particular data set would be information for some people and
    ///      data for others.
    /// 
    ///      One could define information as data that is understandable to most people, or as data
    ///      understandable to a few people, but then where do you draw the lines?
    /// 
    ///      I have developed a data structure which I call an endeme, which I believe stores
    ///      information rather than data. Essentially, it provides people a list of 'factors'
    ///      presented from strongest to weakest in order, to describe something. Clearly,
    ///      this might be more understandable to some people than others.
    ///      In a more basic sense it is information rather than data because the
    ///      strengths of the factors are defined relative to each other rather than in hard numbers.
    /// 
    ///      So I have snuck in here a second definition of information vs data. Information is fuzzy
    ///      and data is precise. In an array of data, you might have a list of 22 people with
    ///      precise ages identified for each. In an endeme, you would have the same set of people
    ///      ordered by age without giving specific ages, but only given perhaps an average age for 
    ///      the group. Whe it comes to ages, basic data is probably more useful. When it comes to
    ///      combinations of concepts, endemes would be more useful because concepts tend to be
    ///      fuzzy anyway.
    ///      
    ///      Another way of defining information is something that has meaning. Endemes store information
    ///      because they store the relative importance of things with each other, and importance is a form
    ///      of meaning, perhaps the most important form of meaning. In other words what is most important
    ///      to someone is what has the most meanin to them.
    ///
    /// 
    ///      Operator Overloading:
    /// 
    ///      Binary  Op Name         Op Description                                                    Order or type    What if sets do not match                   Commutative
    ///      ------  --------------  ----------------------------------------------------------------  ---------------  ------------------------------------------  -----------
    ///      *       match strength  returns a number identifying how well two endemes match           int              returns 0 if they are of different sets     Yes - mostly
    ///      /       similarity      returns an endeme identifying what matches most to least          match order      returns null if they are of different sets  Yes - mostly
    ///      %       difference      returns an endeme identifying what does not match most to least   notmatch order   returns null if they are of different sets  Yes - mostly
    ///      +       plus            increases the items of one endeme by another                      reorders chars   returns null if they are of different sets  Yes - mostly
    ///      -       minus           reduces the items of one endeme by another                        reorders chars   returns null if they are of different sets  No  AB-BC = ABC, BC-AB = CBA
    ///      &       bitwise and     returns an endeme with only those items that are in both          left en order    returns null if they are of different sets  No  ABC&DCB = BC, DCB&ABC = CB
    ///      |       bitwise or      returns an endeme with those items that are in either             left en order    returns null if they are of different sets  No  AB|BC = ABC, BC|AB = BCA
    ///      ^       bitwise xor     returns an endeme with those items in one xor the other           left en order    returns null if they are of different sets  No  AB^BC = AC, BC^AB = CA
    ///      &lt;&lt; left-shift     discards the strongest characteristic(s)                          no reorder                                                   No
    ///      >>      right-shift     discards the weakest characteristic(s) (there is no actual shift) no reorder                                                   No
    /// 
    ///      Comparison
    ///      ----------
    ///      ==      equality        true if has same values and set                                   bool             returns false if they are of different sets Yes
    ///      !=      inequality      false if has same values and set                                  bool             returns true if they are of different sets  Yes
    ///      <, >, <=, >=, &&, ||    [undefined], Iguess we could compare counts :/
    /// 
    ///      Unary
    ///      -----
    ///      +       plus            returns the endeme                                                no reorder       returns the endeme
    ///      -       minus           returns the reverse of the endeme                                 reverse order    returns the reverse of the endeme
    ///      ~       bitwise not     returns the negative of the endeme                                random order     returns reverse order if opposites are defined
    ///      ++      increment       increments the numeric values of the endeme                       no reorder       returns the endeme
    ///      --      decrement       decrements the numeric values of the endeme                       no reorder       removes characteristics that drop to/below zero
    ///      !, true, false          [undefined]
    ///      
    ///      Accessors
    ///      ---------
    ///      [int]   char accessor   returns the char at this position, 0 is the strongest             char
    ///      [char]  pos accessor    returns the position of this char, 0 is the strongest             int
    ///      
    ///      Shortcut
    ///      --------
    ///      +=, -=, /=, %=, &=, |=, ^=, <<=, >>=       [defined by C# and the overloads above]
    ///      *=                      [undefined]
    ///      
    /// 
    ///      Endeme classes
    ///      --------------
    ///      (#sets-#endemes,#values)
    ///      I think I'll make it so the EndemeList chain does not require an EndemeReference and the EndemeDefinition chain does
    ///      
    ///                                                                                   (?-?,?)                  (?-?,?)
    ///                                                                                   EndemeHierarchy -+------ EndemeNode -+
    ///                                                                                                     \                   \
    ///                                                                                   (1-1,0)            \                   +- EndemeKey          (1-n,n)      
    ///                                                                                +- EndemeTextFormat    \    (?-?,?)      /                      EndemeGrabBag
    ///                                         (0-1,0)                               /   +--------------------+-- EndemeUnit -+                 
    ///                                         EndemeQuantification -+              /   /                    /                                              (?-?,?)
    ///                                                                \   (1-1,0)  /   /                    /     (1-1,1)                ((1-1,1)*n)        EndemeBalancer
    ///                                                  +--------------+- Endeme -++--+-----------------------+-- EndemeItem ----------- EndemeList --+       
    ///                                                 /              /             \  \    (0-0,1)       /  /                                         \      (1-2,2)
    ///                                                /              /               \  \   EndemeValue -+--+                                           \     EndemeJoin
    ///                                               /              /                 \  \                   \                                           \    
    ///      (0-0,0)          (0-0,0)                /  (1-0,0)     /  (n-0,0)          \  \   (n-n,0)         \   (n-n,1)                ((n-n,1)*n)      \   ((*-*,1)*n)  
    ///      EndemeMeaning -- EndemeCharacteristic -+-- EndemeSet -+-- EndemeReference -----+- EndemeProfile ---+- EndemeObject -------+- EndemeDefinition -+- EndemeField - - - - - - - - - - EndemeAccess
    ///                                                             \                     \                                           / 
    ///                                                              +---------------------+- [various] - Actuator - EndemeActuator -+  
    ///                                                                                   /
    ///                                                           IEndemeActuatorFactor -+
    ///
    ///
    ///      ABCDEFGHIJKLMNOPQRSTUV|lt Endeme~               class group       notes / alternative labels
    ///      ----------------------+-----------------------------------------------------------------------
    ///      A]         L          |A. EndemeActuator        core class        
    ///      [B]                   |B. EndemeBalancer        helper class      
    ///       [C]                  |C. EndemeCharacteristic  core class        
    ///        [D]F          -R-   |D. EndemeDefinition      core class        (2)Field
    ///         [E]                |E. Endeme                core class        
    ///          [F]          R    |F. EndemeField           core class        (2)/Register
    ///       B   [G]              |G. EndemeGrabBag         helper class      
    ///      A C   [H]          T  |H.  [EndemeHierarchy]    hierarchy class   (Tree/Abstract Tree Component/Hierarchy)
    ///          E  [I]            |I. EndemeItem            core class        (2)Element
    ///              [J]      R    |J.  [EndemeJoin]         table class       relation
    ///               [K]   P  S   |K.  [EndemeKey]          hierarchy class   (Key/Profile Segment)
    ///      A         [L]         |L. EndemeList            core class        (2)Array/Dictionary
    ///                 [M]        |M. EndemeMeaning         core class        
    ///       B          [N]   ST  |N.  [EndemeNode]         hierarchy class   (Node/stem/trunk/branch)
    ///          E   I    [O]      |O. EndemeObject          core class        (2)Item/Element
    ///      A         K   [P]     |P. EndemeProfile         core class        Key/Arrangement
    ///                     [Q]    |Q. EndemeQuantification  core class        
    ///        -D-           [R]   |R. EndemeReference       core class        Reference / Set reference/dictioanry
    ///                       [S]  |S. EndemeSet             core class        
    ///      A    F            [T] |T. EndemeTextFormat      helper class      Ascii text format
    ///          E      L       [U]|U.  [EndemeUnit]         hierarchy class   (Unit/Element/Leaf)
    ///            G             [V|V. EndemeValue           core class        Gellish Value maybe
    ///      ----------------------+-----------------------------------------------------------------------
    ///      ABCDEFGHIJKLMNOPQRSTUV|  14 core classes, 4 hierarchy classes, 3 helper classes, 1 table class
    /// 
    ///      EndemeValue - be like Gellish?
    /// 
    ///      Arrangement - Node - Tree - Leaf - Type - Class - queue - relation
    ///      'Profile' could be 'Key'
    ///      a list is made up of items in a list
    ///      a array is made up of elements in a array
    ///
    ///
    ///      Endeme object/class dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = 0, 1, n(infinity)
    ///      ------------------------------------
    ///
    ///           [------------------ ACTION -----------------]    [-------------------------- STATE -------------------------]    [---------------------------- DATABASE ---------------------------------]           
    ///       _   |                                           |    |                                                          |    |                                                                       |   _     _ 
    ///        |                EndemeActuator.cs                                                                                                                           EndemeAccess                  |       |
    ///  Info  |  |                                           |    |                                                          |    |                                              |                        |  | Info  |
    ///        |                                                                                                                                                                                              |       |
    ///  Level |  |                                           |    |                         (objects)                        |    |                                              |                        |  | Level |
    ///        |                                                                                 +  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  +--+--+--+--+--+--+-----+------+                    |       |
    ///       _|  |                                           |    |                             |                            |    |                      |  |  |  |  |  |  |            |                 |  |_     _|
    ///                                                                                          |                                                        |  |  |  |  |  |  |            |                             
    ///       _   |                                           |    |                             |                            |    |                      |  |  |  |  |  |  |            |                 |   _     _ 
    ///        |                                                                            EndemeField (*-*-1)*n                                         |  |  |  |  |  |  |            |                    |       |
    ///        |  |                                           |    |                             |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                                                          +----------------------+                                                        |  |  |  |  |  |  |            |                    |       |
    ///        |  |                                           |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                                                     EndemeList (1-1-1)*n   EndemeDefinition (n-n-2n)*n                                   |  |  |  |  |  |  |            |                    |       |
    ///        |  |                                           |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                                +------------------------------------------------+------------------------------------------+-------------+  |  |  |  |  |  |            |                    |       |
    ///  High  |  |                             |             |    |      |                     /|\                           |    |       /                 |  |  |  |  |  |            |                 |  | High  |
    ///        |                    EndemeActuator                        |                 EndemeObject (n-n-2n)                    (endeme actuator table) |  |  |  |  |  |            |                    |       |
    ///  Level |  |                      |                    |    |      |                      |                            |    |  [~FACTOR,ACTUATOR]     |  |  |  |  |  |            |                 |  | Level |
    ///        |  |                      |                                |            +---------+------------------------------------------+----------------+  |  |  |  |  |            |                    |       |
    ///        |  |                     /|\                   |    |      |           /          |                            |    |       /                    |  |  |  |  |            |                 |  |       |
    ///        |                     ActuatorList                         |          /      EndemeProfile (n-n-n)                    (endeme profile table)     |  |  |  |  |            |                    |       |
    ///        |  |                     /|\                   |    |      |         /            |           \|/              |    |  [stores PROFILE,OBJECT,   |  |  |  |  |            |                 |  |       |
    ///        |       +-------+-------+-+-----+-------+                  +-----------------------+   EndemeReference (n-0-0)          DEFINITION,FIELD]        |  |  |  |  |            |                    |       |
    ///        |  |    |       |       |       |       |      |    |              /             /|\             |             |    |                            |  |  |  |  |            |                 |  |       |
    ///       _|       |  CharFactor   |  WeightFactor |                         /          EndemeItem (1-1,1)  |                                               |  |  |  |  |            |                    |_     _|
    ///           |    |       |       |       |       |      |    |            /                |              |             |    |                            |  |  |  |  |            |                 |           
    ///             SetFactor  |  OrderFactor  |  ValueFactor            +-----+    +------+-----+              |                                               |  |  |  |  |            |                             
    ///       _   |    |  |    |       |       |       |  |   |    |     |     |    |     /      |              |             |    |                            |  |  |  |  |            |                 |   _     _ 
    ///        |       |  +----+-------+-+-----+-------+  +-----------+  |  IEndemeItem  /       |              |                           +-------------------+  |  |  |  |            |                    |       |
    ///        |  |    |                 |                    |    |  |  |              /        |  Endeme      |  Endeme     |    |       /                       |  |  |  |            |                 |  |       |
    ///        |       |           IEndemeActuatorFactor              EndemeValue (0-0-1)        |  TextFormat  |  GrabBag           (endeme index table)          |  |  |  |            |                    |       |
    ///        |  |    |                 |                    |    |       |                     |       |      |       |     |    |  [indexes ENDEME,LIST]        |  |  |  |            |                 |  |       |
    ///        |       |                 +---------------------------------+---------------------+-------+--------------+-------------------+----------------------+  |  |  |            |                    |       |
    ///        |  |    |                                      |    |                             |              |             |    |       /                          +  |  |            |                 |  |       |
    ///        |       |                                                                    Endeme (1-1-0)      |                    (endeme table) [stores LIST,    /   +  |            |                    |       |
    ///        |  |    |                                      |    |                       /    \|/            /|\            |    |  ENDEME,QUANTIF.,ITEM,VALUE]   /   /   +            |                 |  |       |
    ///        |       +-------------------------------------------------------------------------+--------------+---------------------------+----------------------+   /   /    +--------+-------+            |       |
    ///  Low   |  |                                           |    |                     /       |                            |    |       /                          +   /     |                |         |  | Low   |
    ///        |                                                                        /   EndemeSet (1-0-0)                        (endeme set table)               |  +  RichSqlCommand   RichDataTable    |       |
    ///  Level |  |                                           |    |                   /         |                            |    |  [stores SET]                    |  |      |     |          |   |     |  | Level |
    ///        |                                                          +-----------+          +--+---------------------------------------+-------------------------+  |      |     +----------+   |        |       |
    ///        |  |                                           |    |     /|\                    /|\                           |    |       /                             |      |     |              |     |  |       |
    ///        |                                                     EndemeQuantification   EndemeCharacteristic (0-0-0)             (endeme characteristic table)       |      |  InData     Is.    |        |       |
    ///        |  |                                           |    | (0-0-0)                     |                            |    |  [stores CHARACTERISTIC]            |      |  Throws     RandomSource |  |       |
    ///        |                                                                                 +------------------------------------------+----------------------------+      |  TreatAs.   FilesIO         |       |
    ///        |  |                                           |    |                            /|\                           |    |       /                                    |             __.          |  |       |
    ///       _|                                                                            EndemeMeaning (0-0-0)                    (endeme meaning table)                 ConnectSource                     |_     _|
    ///           |                                           |    |                                                          |    |  [stores MEANING]                                                     |           
    ///           [------------------ ACTION -----------------]    [------------------------------ STATE ---------------------]    [---------------------------- DATABASE ---------------------------------]           
    /// 
    ///      production ready
    /// </remarks>
    public class Endeme
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        //    Because there is derived information, the authoritative, source and the derived source
        //    both have to remain private, because you have to change both at the same time under control
        // ----------------------------------------------------------------------------------------
        private  List<char>            _string { get; set; }                           // _string is the authoritative information source
        private  Dictionary<char, int> _index;                                         // _index is the derived information source
        internal int                   Size    { get { return _size; } private set { _size = value; } }  private int        _size = 22; // 17 is a 'magic number', it produces no negative raw matches for a 22 char endeme set - I should probably switch to 26
        public   EndemeSet             EnSet   { get { return _set;  }         set { _set  = value; } }  private EndemeSet  _set;
        public   EndemeQuantification  Quant   { get; set; }


        public static int NO_INDEX = -99;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public Endeme(                                          ) { Init(null, null                ); } //     _isEmpty = true);  }
        public Endeme(EndemeSet set                             ) { Init(set , null                ); } //     _isEmpty = true);  }
        public Endeme(EndemeSet set, Dictionary<char,int> index ) { Init(set , IndexToString(index)); } //     _isEmpty = false); }
        public Endeme(EndemeSet set,               string endeme) { Init(set , endeme              ); } //     _isEmpty = false); }
        public Endeme(                             string endeme) { Init(null, endeme              ); } //     _isEmpty = false); }
        public Endeme(char[] arr, EndemeOrderingCreationType eot) { _set = null; BuildString(arr, eot); BuildIndex(); Quant = new EndemeQuantification(); } //_isEmpty = false; }


        // ----------------------------------------------------------------------------------------
        //  Short properties and accessors    production ready
        /* ------------------------------------------------------------------------------------- */                                /// <summary>returns character at position, 0 is highest</summary>
        public        char   this[int  i]    { get { if (!IsEmpty && _string.Count > i) return _string[i]   ; return '\0';     } } /// <summary>returns position of character, 0 is highest</summary>
        public        int    this[char c]    { get { if (_index.ContainsKey(c))         return _index[c]    ; return  -1 ;     } } /// <summary>Returns the number of characteristics in this endeme</summary>
        public        int    Count           { get { if (_string != null)               return _string.Count; return   0 ;     } } /// <summary>Returns the number of actual letters in the endeme (may be less than the number in the endeme set)</summary>
        public        string FirstTwo        { get {                                    return First(2);                       } } /// <summary>Returns the set name and the endeme string delimited by a colon</summary>
        public        string FullDesignation { get {                                    return (SetName + ":" + ToString());   } }
        public static Endeme Empty           { get {                                    return new Endeme();                   } } /// <summary>Indicates whether the endeme is empty (null or count = 0)</summary>
        public        bool   IsEmpty         { get {                         return (_string == null || _string.Count == 0);   } } /// <summary>Returns the full endeme that is the most opposite endeme to the current one</summary>
        public        Endeme Opposite        { get { return new Endeme(EnSet, (EnSet.Straight - this).ToString() + Reverse.ToString()); } } /// <summary>Returns the name of the endeme set</summary>
        public        string SetName         { get { if (_set    != null)               return _set.Label   ; return  "" ;     } }
        // ----------------------------------------------------------------------------------------
        //  Short methods       beta code to production ready
        /* ------------------------------------------------------------------------------------- */                                                                          /// <summary>Returns the array of letters</summary>
        public          char[] ToCharArray  ()                   {                           return _string.ToArray();                                                     } /// <summary>Adds a value to the quantification of an endeme characteristic</summary>
        public          void   Plus         (char c , int num  ) { if ('A' <= c && c <= 'Z') Quant.Raw[c] += num;                                                          } /// <summary>Adds a character to the index and the authoritative source _string</summary>
        private         void   Add          (char c , int i    ) { if (_index == null) _index = new Dictionary<char,int>(24); _string.Add(c); _index.Add(c, _index.Count); } /// <summary>Adds a character to the index and the authoritative source _string</summary>
        public          Endeme Add          (char c            ) { if (!Contains(c)) Add(c, _string.Count);                                             return this;       } /// <summary>Adds a string of characters to the index and the authoritative source _string</summary>
        public          int    Index        (char c            ) { if (IsEmpty) return NO_INDEX; else if (_index.ContainsKey(c)) return _index[c]; else return NO_INDEX;   } /// <summary>Used for Equals() and GetHashCode()</summary>
        public override int    GetHashCode  (                  ) { return FullDesignation.GetHashCode();                                                                   } /// <summary>Returns a straight string of just the letters or blank</summary>
        public override string ToString     (                  ) { if (IsEmpty || _string == null) return ""; else return (new string(_string.ToArray()));                 }
        public          string ToString     (int ix0, int len  ) { string str = this.ToString().PadRight(22).Substring(ix0, len);                       return str ;       } /// <summary>Returns a string of the first n characteristics comma delimited</summary><param name="num">number of characteristics labels to show</param>
        public          string Labels       (int    num        ) { return _set.Labels(First(num));                                                                         } /// <summary>Returns the first n items as a string of letters</summary>
        public void GenerateCalc_geometric  (                  ) { GenerateCalc_geometric( 0.93         ); }
        public void GenerateCalc_linear     (                  ) { GenerateCalc_linear   (-100.0 / _size); }
        public void GenerateRaw_geometric   (                  ) { GenerateRaw_geometric ( 0.93         ); }
        public void GenerateRaw_linear      (                  ) { GenerateRaw_linear    (-100.0 / _size); }
        /* with loops: */                                                                                                                                                  /// 
        public          Endeme Add          (string letters    ) { char[] c = letters.ToCharArray();              for (int i = 0; i < c.Length     ; ++i)       { if (!Contains(c[i])) Add(c[i]);        } return this ; }
        public          string First        (int    num        ) { char[] c = _string.ToArray(); string str = ""; for (int i = 0; i < c.Length && i < num; ++i) { str += c[i];                           } return str  ; }
        internal        int    MultiplyMatch(int n, Endeme en  ) { int match = 0;      foreach (char c in EnSet.LettersUnsorted) { if (this[c] >= 0 && en[c] >= 0) match += (n - this[c]) * (n - en[c]); } return match; } /// <summary>Increments the raw value for each characteristic</summary>
        internal        int    ShiftedMatch (int n, Endeme en  ) { int match = 0;      foreach (char c in EnSet.LettersUnsorted) { if (this[c] >= 0 && en[c] >= 0) match += Shifted (n-this[c]) * Shifted (n-en[c]); } return match; }
        internal        int    FiboMatch    (int n, Endeme en  ) { int match = 0;      foreach (char c in EnSet.LettersUnsorted) { if (this[c] >= 0 && en[c] >= 0) match += FiboDec (n-this[c]) * FiboDec (n-en[c]); } return match; }
        internal        int    SquareMatch  (int n, Endeme en  ) { int match = 0;      foreach (char c in EnSet.LettersUnsorted) { if (this[c] >= 0 && en[c] >= 0) match += Square  (n-this[c]) * Square  (n-en[c]); } return match; }
        internal        int    SimpleMatch  (int n, Endeme en  ) { int match = 0;      foreach (char c in EnSet.LettersUnsorted) { if (this[c] >= 0 && en[c] >= 0) match += Triangle(n-this[c]) * Triangle(n-en[c]); } return match; }

        private int Square(int p)
        {
            if (p>=0) return p*p;
            else return -1*p*p;
        }
        internal static int Triangle(int n)
        {
            if (n >=0) return (n*n+n)/2;
            else       return (n*n-n)/-2;
        }
        internal static int    Shifted(int n)
        {
            if (n > 1) return 2<<(n-2);
            if (n > -2) return n;
            return -1*(2<<(-2-n));
        }
        internal static int FiboDec(int n)
        {
            int output = 0;
            switch (n)
            {
                case  0: output =     0; break;
                case  1: output =     1; break;  case  -1: output =     -1; break;
                case  2: output =     1; break;  case  -2: output =     -1; break;
                case  3: output =     2; break;  case  -3: output =     -2; break;
                case  4: output =     3; break;  case  -4: output =     -3; break;
                case  5: output =     5; break;  case  -5: output =     -5; break;
                case  6: output =     8; break;  case  -6: output =     -8; break;
                case  7: output =    12; break;  case  -7: output =    -12; break;
                case  8: output =    20; break;  case  -8: output =    -20; break;
                case  9: output =    30; break;  case  -9: output =    -30; break;
                case 10: output =    50; break;  case -10: output =    -50; break;
                case 11: output =    80; break;  case -11: output =    -80; break;
                case 12: output =   120; break;  case -12: output =   -120; break;
                case 13: output =   200; break;  case -13: output =   -200; break;
                case 14: output =   300; break;  case -14: output =   -300; break;
                case 15: output =   500; break;  case -15: output =   -500; break;
                case 16: output =   800; break;  case -16: output =   -800; break;
                case 17: output =  1200; break;  case -17: output =  -1200; break;
                case 18: output =  2000; break;  case -18: output =  -2000; break;
                case 19: output =  3000; break;  case -19: output =  -3000; break;
                case 20: output =  5000; break;  case -20: output =  -5000; break;
                case 21: output =  8000; break;  case -21: output =  -8000; break;
                case 22: output = 12000; break;  case -22: output = -12000; break;
                case 23: output = 20000; break;  case -23: output = -20000; break;
                case 24: output = 30000; break;  case -24: output = -30000; break;
            }
            return output;
        }
        internal static int FiboBin(int n)
        {
            int output = 0;
            switch (n)
            {
                case  0: output =   0; break;                                   // 00000000
                case  1: output =   1; break;  case  -1: output =   -1; break;  // 00000001
                case  2: output =   1; break;  case  -2: output =   -1; break;  // 00000001
                case  3: output =   2; break;  case  -3: output =   -2; break;  // 00000010
                case  4: output =   3; break;  case  -4: output =   -3; break;  // 00000011
                case  5: output =   4; break;  case  -5: output =   -4; break;  // 00000100
                case  6: output =   8; break;  case  -6: output =   -8; break;  // 00001000
                case  7: output =  12; break;  case  -7: output =  -12; break;  // 00001100
                case  8: output =  16; break;  case  -8: output =  -16; break;  // 00010000
                case  9: output =  32; break;  case  -9: output =  -32; break;  // 00100000
                case 10: output =  48; break;  case -10: output =  -48; break;  // 00110000
                case 11: output =  64; break;  case -11: output =  -64; break;  // 01000000
                case 12: output = 128; break;  case -12: output = -128; break;  // 10000000
            }
            return output;
        }
        
        /// <summary>Increments the raw value for each characteristic</summary>
        private         Endeme Increment    (                  ) {                     foreach (char c in this.ToCharArray()   ) { Quant.Raw[c]++; }                                                         return this ; } /// <summary>Decrements the raw value for each characteristic</summary>
        private         Endeme Decrement    (                  ) {                     foreach (char c in this.ToCharArray()   ) { Quant.Raw[c]--; if (Quant.Raw[c] <= 0) Remove(c);                       } return this ; } /// <summary>Generates the Calc values from the endeme string using a geometric slope</summary><param name="geom">usually between 0.0 and 1.0, 0.93 is good</param>
        public void GenerateCalc_geometric  (double geom       ) { double num = 100.0; foreach (char c in _string.ToArray()    ) { Quant.Calc[c] = num; num = num * geom ;                                 }               } /// <summary>Generates the Calc values from the endeme string using a linear slope</summary><param name="slope">usually negative, -4.55 is a good number</param>
        public void GenerateCalc_linear     (double slope      ) { double num = 100.0; foreach (char c in _string.ToArray()    ) { Quant.Calc[c] = num; num = num + slope;                                 }               } /// <summary>Generates the raw values from the endeme string using a geometric slope</summary><param name="geom">usually between 0.0 and 1.0, 0.93 is good</param>
        public void GenerateRaw_geometric   (double geom       ) { double num = 100.0; foreach (char c in _string.ToArray()    ) { Quant.Raw[c]  = num; num = num * geom ;                                 }               } /// <summary>Generates the raw values from the endeme string using a linear slope</summary><param name="slope">usually negative, -4.55 is a good number</param>
        public void GenerateRaw_linear      (double slope      ) { double num = 100.0; foreach (char c in _string.ToArray()    ) { Quant.Raw[c]  = num; num = num + slope;                                 }               }
        // ----------------------------------------------------------------------------------------
        //  Casting (implicit operators)    production ready
        // ----------------------------------------------------------------------------------------
      //public static implicit operator  Endeme (char   c  ) { return new Endeme(c.ToString()); }
        public static implicit operator  Endeme (string str) { return new Endeme(str         ); }
        // ----------------------------------------------------------------------------------------
        //  Short operators    alpha code to production ready
        /* ------------------------------------------------------------------------------------- */                                 ///<summary>WARNING - endeme addition is not associative, use parentheses when you have more than 2 endemes!</summary>
        public static Endeme operator  +(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.Add       (rhs); } ///<summary>Adds a character to an endeme if it is not already there</summary>
        public static Endeme operator  +(Endeme lhs, char c    ) { if (lhs == null) return null; else return lhs.Add       (c  ); }  ///<summary>WARNING - endeme subtraction is not associative, use parentheses when you have more than 2 endemes!</summary>
        public static Endeme operator  -(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.Minus     (rhs); } ///<summary>WARNING - endeme multiplication is not associative, use parentheses when you have more than 2 endemes!</summary>
        public static Endeme operator  *(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.Times     (rhs); } ///<summary>Identifies the similarities between two endemes, assumes a 22 character EndemeSet, the results will be a off otherwise</summary>
        public static Endeme operator  /(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.Similarity(rhs); } ///<summary>Identifies the differences between two endemes, assumes a 22 character EndemeSet, the results will be a off otherwise, similarity plus differnce should equal the original lhs</summary>
        public static Endeme operator  %(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.Difference(rhs); } /// <summary>'bitwise' AND for when an Endeme is used as a bitwise enumeration</summary>
        public static Endeme operator  &(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.And       (rhs); } /// <summary>'bitwise' OR for when an Endeme is used as a bitwise enumeration</summary>
        public static Endeme operator  |(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.Or        (rhs); } /// <summary>'bitwise' XOR for when an Endeme is used as a bitwise enumeration</summary>
        public static Endeme operator  ^(Endeme lhs, Endeme rhs) { if (lhs == null) return null; else return lhs.Xor       (rhs); } /// <summary>Chops the strongest n characteristics off the front of the endeme</summary>
        public static Endeme operator <<(Endeme lhs, int    num) { if (lhs == null) return null; else return lhs.LeftShift (num); } /// <summary>Chops the weakest n characteristics off the end of the endeme</summary>
        public static Endeme operator >>(Endeme lhs, int    num) { if (lhs == null) return null; else return lhs.RightShift(num); } /// <summary>returns the endeme itself</summary>
        public static Endeme operator  +(Endeme lhs            ) { if (lhs == null) return null; else return lhs                ; } /// <summary>returns a reverse of the endeme</summary>
        public static Endeme operator  -(Endeme lhs            ) { if (lhs == null) return null; else return lhs.Reverse        ; } /// <summary>returns the characteristics that were not in the endeme, same as !</summary>
        public static Endeme operator  ~(Endeme lhs            ) { if (lhs == null) return null; else return lhs.Negation  (   ); } /// <summary>returns the characteristics that were not in the endeme, same as ~</summary>
        public static Endeme operator  !(Endeme lhs            ) { if (lhs == null) return null; else return lhs.Negation  (   ); } /// <summary>increments the endeme characteristic quantities</summary>
        public static Endeme operator ++(Endeme lhs            ) { if (lhs == null) return null; else return lhs.Increment (   ); } /// <summary>decrements the endeme characteristic quantities, removing characteristics as the drop to zero or below</summary>
        public static Endeme operator --(Endeme lhs            ) { if (lhs == null) return null; else return lhs.Decrement (   ); } /// <summary>!= based on set name and endeme string</summary>
        public static bool   operator !=(Endeme lhs, Endeme rhs) {                  return !(lhs == rhs);                         } /// <summary>>= based on count comparisons</summary>
        public static bool   operator >=(Endeme lhs, Endeme rhs) {                  return !(lhs <  rhs);                         } /// <summary>&lt;= based on count comparisons</summary>
        public static bool   operator <=(Endeme lhs, Endeme rhs) {                  return !(lhs >  rhs);                         } /// <summary>> based on count comparisons</summary>
        public static bool   operator  >(Endeme lhs, Endeme rhs) { if (lhs == rhs ) return false; if (lhs == null) return false; if (rhs == null) return true ; return (lhs.Count > rhs.Count); } /// <summary>&lt; based on count comparisons</summary>
        public static bool   operator  <(Endeme lhs, Endeme rhs) { if (lhs == rhs ) return false; if (lhs == null) return true ; if (rhs == null) return false; return (lhs.Count < rhs.Count); } /// <summary>== based on set name and endeme string</summary>
        public static bool   operator ==(Endeme lhs, Endeme rhs)
        {
            bool loren = object.ReferenceEquals(null, lhs); // loren = L)eft O)bject R)eference E)quals N)ull
            bool roren = object.ReferenceEquals(null, rhs); // roren = R)ight O)bject R)eference E)quals N)ull
            if (loren && roren) return true;
            if (loren || roren) return false;
            return (lhs.FullDesignation == rhs.FullDesignation);
        }


        // --------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a string of characters to the index and the authoritative source _string
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public Endeme Add(Endeme rhs)
        {
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();

            // --------------------------------------------------------------------------
            //  for each letter found in both, calculate
            // --------------------------------------------------------------------------
            Endeme output = new Endeme(this.EnSet);
            for (int i = 0; i < alphabet.Length; ++i)
            {
                char c = alphabet[i];
                if (this.Contains(c) || rhs.Contains(c)) output.Add(c);

                if (this.Contains(c) && rhs.Contains(c)) output.Quant.Raw[c] = 25 + ((22-this[c]) + (22-rhs[c])) - Math.Abs((22-this[c]) - (22-rhs[c]));
                else if (this.Contains(c))               output.Quant.Raw[c] = (22-this[c]);
                else if (rhs.Contains(c))                output.Quant.Raw[c] = (22-rhs[c]);
                else                                     output.Quant.Raw[c] = -1;
            }


            output.CookEndeme();
            return output;
        }

        // --------------------------------------------------------------------------------------
        /// <!-- And -->
        /// <summary>
        ///      'Bitwise AND' of two endemes
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme And(Endeme rhs)
        {
            // --------------------------------------------------------------------------
            //  Standard checks
            // --------------------------------------------------------------------------
            if (rhs == null   ) return Copy();  // I'm not sure this is quite kosher, the raw quants will be wrong
            if (Equals(rhs)   ) return Copy();  // If they are the same, just return a copy
            if (EnSet != rhs.EnSet) return Copy();


            // --------------------------------------------------------------------------
            //  Do the calculation
            // --------------------------------------------------------------------------
            string str = "";
            foreach (char c in this.ToCharArray())
                if (rhs.Contains(c)) str += c;
            Endeme endeme = new Endeme(this.EnSet, str);
            return endeme;
        }

        // --------------------------------------------------------------------------------------
        /// <!-- BuildIndex -->
        /// <summary>
        ///      Refreshes the endeme index
        /// </summary>
        /// <remarks>production ready</remarks>
        private void BuildIndex()
        {
            if (_string == null) { _index = new Dictionary<char, int>(); }
            else
            {
                _index = new Dictionary<char, int>(_string.Count);
                for (int i = 0; i < _string.Count; ++i)
                    _index.Add(_string[i], i);
            }
        }
        /// <summary>Treat as private, public only for testing purposes</summary>
        public void TestBuildIndex() { BuildIndex(); }

        // --------------------------------------------------------------------------------------
        /// <!-- BuildString -->
        /// <summary>
        ///      Adds each unique letter to the endeme, Treat as private, internal only for testing purposes
        /// </summary>
        /// <param name="endeme"></param>
        /// <remarks>production ready</remarks>
        private void BuildString(string endeme)
        {
            // --------------------------------------------------------------------------
            //  Build an endeme string out of chars A-Z only:
            // --------------------------------------------------------------------------
            if (string.IsNullOrEmpty(endeme))
                endeme = "";
            char[] cha = Regex.Replace(endeme.ToUpper(), "[^A-Z]", "").ToCharArray();
            if (_set == null || _set.Count == 0)
                BuildString(cha);
            else
                BuildString(cha, _set);
        }
        public void TestBuildString(string endeme) { BuildString(endeme); }

        // ----------------------------------------------------------------------------------------
        /// <!-- BuildString -->
        /// <summary>
        ///      Without a set, add all the letters (once only for each letter)
        /// </summary>
        /// <param name="cha"></param>
        /// <remarks>production ready</remarks>
        private void BuildString(char[] cha)
        {
            _string = new List<char>();
            _index = new Dictionary<char, int>();
            int count = cha.Length;
            for (int i = 0; i < count; ++i)
                if (!Contains(cha[i]))
                    Add(cha[i], i);
        }
        private void BuildString(char[] arr, EndemeOrderingCreationType type)
        {
            if (type == EndemeOrderingCreationType.ByCharacterFrequency)
                throw new NotImplementedException("BuildEndemeByCharacterFrequency does not work in C# 2.0");
                //arr = ArrayByFrequency(arr);
            BuildString(arr);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- BuildString -->
        /// <summary>
        ///      With a set, add only the letters in the set (once each)
        /// </summary>
        /// <remarks>
        ///      Treat as private, internal only for testing purposes
        /// </remarks>
        /// <param name="endeme"></param>
        /// <param name="set"></param>
        /// <remarks>production ready</remarks>
        private void BuildString(char[] cha, EndemeSet set)
        {
            _string = new List<char>();
            _index = new Dictionary<char, int>();
            int count = cha.Length;
            for (int i = 0; i < count; ++i)
                if (!Contains(cha[i]) && set.Contains(cha[i]))
                    Add(cha[i], i);
        }
        public void TestBuildString(char[] cha, EndemeSet set) { BuildString(cha, set); }

        // ----------------------------------------------------------------------------------------
        /// <!-- CalcMaxMatch -->
        /// <summary>
        ///      Returns a number between 0.0 and 1.0 inclusive indicating the number to be returned
        ///      from the best possible match of two endemes based on the lengths of the two endemes
        /// </summary>
        /// <param name="loCount">length of the shorter endeme</param>
        /// <param name="hiCount">length of the longer endeme</param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private double CalcMaxMatch(double loCount, double hiCount)
        {
            if (loCount == hiCount) return 1.0;
            int setCount = 22;
            if (EnSet != null)
                setCount = EnSet.Count;


            double maxMatch = loCount / hiCount
                + loCount * (hiCount - loCount)
                          / (setCount * setCount)
                          * (1 + (hiCount * (setCount - hiCount)) / (setCount * 5));
            return Math.Max(0.0, Math.Min(1.0, maxMatch));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CollateIndex -->
        /// <summary>
        ///      Returns a string containing the numbers in the index offset by a particular value
        /// </summary>
        /// <param name="join"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public string CollateIndex(string join, int offset)
        {
            StringBuilder str = new StringBuilder();
            List<char> keys = KeysSorted;
            string delim = "";
            foreach (char key in keys)
            {
                str.Append(delim + (_index[key] + offset));
                delim = join;
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CookEndeme -->
        /// <summary>
        ///      Takes the raw assembled data and cooks it into the endeme
        /// </summary>
        /// <remarks>production ready</remarks>
        public void CookEndeme()
        {
            string endeme = "";


            // --------------------------------------------------------------------------
            //  Collate the endeme string in the order of the Quant.Raw data using Linq
            // --------------------------------------------------------------------------
            //var sortedList = (from d in Quant.Raw where d.Value > 0 orderby d.Value descending select d);
            //foreach (var item in sortedList) endeme += item.Key;


            // --------------------------------------------------------------------------
            //  Collate the endeme string in the order of the Quant.Raw data without Linq
            // --------------------------------------------------------------------------
            SortedDictionary<char,double> raw = new SortedDictionary<char,double>(Quant.Raw);
            List<double> values = new List<double>(raw.Values);
            values.Sort();
            for (int i = values.Count - 1; i >= 0; --i)
            {
                char c = '_';
                foreach (char key in raw.Keys)
                {
                    if (raw[key] == values[i] && raw[key] > 0)
                        c = key;
                }
                if (c != '_')
                {
                    endeme += c;
                    raw.Remove(c);
                }
            }


            _index = new Dictionary<char, int>(endeme.Length);
            BuildString(endeme);
            BuildIndex();
            //_isEmpty = false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Contains -->
        /// <summary>
        ///      This is killing our performance (49.264%)
        ///      I must use a hash to fix this
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public bool Contains(char cha)
        {
            if (_index == null)
                return _string.Contains(cha);   // this is a sequential search and therefore very slow
            else
                return _index.ContainsKey(cha); // you want to use the index if it exists
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Contains -->
        /// <summary>
        ///      Returns true if all items in combo are contained in endeme
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public bool Contains(string combo)
        {
            combo = combo.ToUpper();
            combo = Regex.Replace(combo, "[^A-Z]", "");
            char[] cha = combo.ToCharArray();
            bool contains = true;
            for (int i = 0; i < cha.Length; ++i)
                contains &= Contains(cha[i]);
            return contains;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsCount -->
        /// <summary>
        ///      Returns the number of characters this endeme contains
        /// </summary>
        /// <param name="endeme"></param>
        /// <returns></returns>
        public int ContainsCount(string endeme)
        {
            endeme     = endeme.ToUpper();
            endeme     = Regex.Replace(endeme, "[^A-Z]", "");
            char[] cha = endeme.ToCharArray();
            int contains = 0;
            for (int i = 0; i < cha.Length; ++i)
                if (Contains(cha[i]))
                    contains++;
            return contains;
        } /// <summary>Returns the number of characters this endeme contains as long as the sets are the same</summary>
        public int ContainsCount(Endeme enTgt) { if (enTgt.EnSet == this.EnSet) return ContainsCount(enTgt.ToString()); else return 0; }

        // ----------------------------------------------------------------------------------------
        /// <!-- Contains -->
        /// <summary>
        ///      Determine if entgt is completely contained in ensrc (this), thes sets have to be the same
        /// </summary>
        /// <param name="enTgt"></param>
        /// <returns></returns>
        public bool Contains(Endeme enTgt)
        {
            if (enTgt.EnSet == this.EnSet) return Contains(enTgt.ToString()); else return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsOneOf -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ContainsOneOf(string str)
        {
	        bool found = false;
	        foreach (char c in str.ToCharArray()) found = found | this.Contains(c);
	        return found;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Copy -->
        /// <summary>
        ///      Returns a copy of itself
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public Endeme Copy()
        {
            Endeme e = new Endeme();
            e._string  = this.ToList();  // this is a copy
            //e._isEmpty = _isEmpty;       // this is a copy
            e._set     = EnSet;            // do not copy the set, just include the reference
            e.Quant    = Quant;          // do not copy, just include the reference for now
            e.BuildIndex();              // this is a copy
            return e;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Difference -->
        /// <summary>
        ///      Related to Minus
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme Difference(Endeme rhs)
        {
            // --------------------------------------------------------------------------
            //  Standard checks
            // --------------------------------------------------------------------------
            if (rhs == null   ) return Copy();
            if (Equals(rhs)   ) return Empty ;
            if (EnSet != rhs.EnSet) return Copy();


            // --------------------------------------------------------------------------
            //  Do the calculation
            // --------------------------------------------------------------------------
            Endeme endeme = new Endeme(this.EnSet);
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
            {
                endeme.Quant.Raw[c] = 0;
                if (this.Contains(c)) endeme.Quant.Raw[c] -= (22 - this[c]);
                if (rhs .Contains(c)) endeme.Quant.Raw[c] += (22 - rhs [c]);
            }
            endeme.CookEndeme();

            return endeme;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Checks to see if the core parts (string and set) of the endeme are identical
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public override bool Equals(object obj)
        {
            if (   obj == null                      )  return false;
            if (   object.ReferenceEquals(obj, this))  return true;
            if (! (obj is Endeme)                   )  return false;
            return (this.FullDesignation == ((Endeme)obj).FullDesignation);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FillOut -->
        /// <summary>
        ///      Fills out an endeme to the number of letters requested
        /// </summary>
        /// <param name="r"></param>
        /// <param name="minLength">minimum length of the fill</param>
        /// <returns></returns>
        public Endeme FillOut(int minLength, Random r)
        {
            this.EnSet.RandomEndeme();
            List<char>    list   = EnSet.LettersUnsorted;
            int           count  = Math.Min(minLength, list.Count);
            StringBuilder endeme = new StringBuilder(count);


            for (int i = 0; i < count; ++i)
            {
                int n = r.Next(list.Count);
                char c = list[n];
                if (!_string.Contains(c))
                {
                    endeme.Append(c);
                    Add(c);
                }
                list.RemoveAt(n);
            }

            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IndexToString -->
        /// <summary>
        ///      Converts a dictionary of character positions into an Endeme
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        private static string IndexToString(Dictionary<char,int> positions)
        {
            List<char> e = new List<char>(positions.Count);
            for (int i = 0; i < 32 + positions.Count; ++i) e.Add(' ');
            foreach (char cha in positions.Keys)
            {
                int pos = 32 - positions[cha];
                e[pos] = cha;
            }
            string endeme = new string(e.ToArray());
            endeme = Regex.Replace(endeme, " ", "");
            return endeme;
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Initializes an Endeme object
        /// </summary>
        /// <param name="set"></param>
        /// <param name="endeme"></param>
        /// <param name="isEmpty"></param>
        /// <remarks>production ready</remarks>
        private void Init(EndemeSet set, string endeme)
        {
            // --------------------------------------------------------------------------
            //  Put the string together
            // --------------------------------------------------------------------------
            _set  = set;
            if (_set != null && _set.Count > 0)
                _size = _set.Count;
            else _size = 22;
            //if (endeme == null && set != null)
            //    BuildString(set.Letters);
            //else
            //    BuildString(endeme);
            if (!(string.IsNullOrEmpty(endeme) || Regex.IsMatch(endeme, "^[ \t\r\n]*$")))
                BuildString(endeme);
            else
            {
                _string = new List<char>();
                _index =new Dictionary<char,int>();
            }


            // --------------------------------------------------------------------------
            //  Calculate indexing and such
            // --------------------------------------------------------------------------
            BuildIndex();
            Quant = new EndemeQuantification();
        }

        // --------------------------------------------------------------------------------------
        /// <!-- KeysSorted -->
        /// <summary>
        ///      Returns an alphabetically sorted list of the letters in the endeme
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        private List<char> KeysSorted { get
        {
            List<char> keys = new List<char>(_index.Keys.Count);
            foreach (char key in _index.Keys)
                keys.Add(key);
            keys.Sort();
            return keys;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- LeftShift -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme LeftShift(int num)
        {
            if (num <= 0) return Copy();
            if (num >= Count) return new Endeme(EnSet);
            string endeme = this.ToString().Substring(num);
            return new Endeme(EnSet, endeme);
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- Match -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme2"></param>
        /// <returns>
        ///      1.            similar endeme sets
        ///      2.            similarity on the far end for full matches
        ///      3. [handled?] differences for when match is vs blank or something
        ///      4. [handled?] difference between and and or
        ///      5.            give a bump to endemes athat are the same length with the same letters, especially first 3
        ///      6. [handled?] no exactly the same results
        /// </returns>
        public double Match(Endeme endeme2, WeightFormula formula, int precision = 7)
        {
            double match = -100000.0;
            if (this.EnSet != null && endeme2.EnSet != null && this.EnSet.Label == endeme2.EnSet.Label) switch (formula)
            {
                case WeightFormula.Refined    : match = RefinedMatch   (this, endeme2, 0.84, 214.0, 428.0, 700.0, 20000.0, 0.01); break;
                case WeightFormula.FullMedian : match = FullMatchMedian(this, endeme2, precision); break;
            }
            return match;
        }
        public double MatchBest(Endeme endeme2, WeightFormula formula, int precision)
        {
            double match = -100000.0;
            if (this.EnSet.Label == endeme2.EnSet.Label) switch (formula)
            {
                case WeightFormula.Refined    : match = RefinedMatch (this, endeme2, 0.84, 214.0, 428.0, 700.0, 20000.0, 0.01); break;
                case WeightFormula.FullMedian : match = FullMatchBest(this, endeme2, precision); break;
            }
            return match;
        }
        public double MatchWorst(Endeme endeme2, WeightFormula formula, int precision)
        {
            double match = -100000.0;
            if (this.EnSet.Label == endeme2.EnSet.Label) switch (formula)
            {
                case WeightFormula.Refined    : match = RefinedMatch  (this, endeme2, 0.84, 214.0, 428.0, 700.0, 20000.0, 0.01); break;
                case WeightFormula.FullMedian : match = FullMatchWorst(this, endeme2, precision); break;
            }
            return match;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- FullMatchMedian -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <param name="numTries"></param>
        /// <returns></returns>
        /// <remarks>
        ///         2 * (int)(32/n) + 1
        ///     2 | 33
        ///     3 | 
        ///     4 | 17
        ///     8 | 9
        ///    16 | 5
        /// </remarks>
        private List<int> FullMatchList(Endeme endeme1, Endeme endeme2, int precision)
        {
            int numTries = (int)Math.Pow(2,precision)+1;
            Random r = RandomSource.New().Random;
            List<int> match = new List<int>();


            int mod = 0;
            if (endeme1.Count != endeme2.Count)
            {
                double count1 = endeme1.Count;
                double count2 = endeme2.Count;
                if (endeme1.Count == 0 || endeme2.Count == 0)
                {
                    count1 += 1;
                    count2 += 1;
                }
                mod = (int)(0.5 - 4096.0 * Math.Sqrt(count1 / count2 + count2 / count1 - 2));
            }


            // --------------------------------------------------------------------------
            //  Fill out both endemes, multiply all positions, add to list n times
            // --------------------------------------------------------------------------
            for (int i = 0; i < numTries; ++i)
            {
                Endeme en1 = endeme1.Copy().FillOut(endeme1.EnSet.Count, r);
                Endeme en2 = endeme2.Copy().FillOut(endeme2.EnSet.Count, r);
              //match.Add(en1.FiboMatch(11,en2));
              //match.Add(en1.ShiftedMatch(11,en2));
              //match.Add(en1.MultiplyMatch(14,en2));
              //match.Add(en1.SquareMatch(14,en2));
              //match.Add(en1.SquareMatch(12,en2));
              //match.Add(en1.SquareMatch(11,en2));
              //match.Add(en1.SimpleMatch(12,en2));


              //int simple = en1.SimpleMatch(12,en2);
                int simple = en1.SquareMatch(12,en2);
                match.Add(simple + mod);
            }


            // --------------------------------------------------------------------------
            //  return the median match
            // --------------------------------------------------------------------------
            match.Sort();
            return match;
        }
        private double FullMatchBest(Endeme endeme1, Endeme endeme2, int precision)
        {
            return FullMatchList(endeme1, endeme2, precision).Max();
        }
        private double FullMatchMedian(Endeme endeme1, Endeme endeme2, int precision)
        {
            List<int> match = FullMatchList(endeme1, endeme2, precision);
            return match[(int)(match.Count/2.0)];
        }
        private double FullMatchWorst(Endeme endeme1, Endeme endeme2, int precision)
        {
            return FullMatchList(endeme1, endeme2, precision)[0];
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- Match -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endeme1"></param>
        /// <param name="endeme2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static double RefinedMatch(Endeme endeme1, Endeme endeme2, double geom, double oneMissing, double bothMissing, double something, double directMatch, double exactMatch)
        {
            if (endeme1.ToString() == endeme2.ToString())
                return 51776.45491 * (1.0 + (endeme1.Count - 22.0) * exactMatch);


            MatchTableJustInTime(geom);
            if (endeme1.EnSet != endeme2.EnSet) return 0.0;


            // --------------------------------------------------------------------------
            //  normal processing
            // --------------------------------------------------------------------------
	        char[] c = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
	        double total       = 0.0;
	        double mutualCount = 0.1;
	        for (int i = 0; i <= c.Length - 1; i++)
            {
		        int idx1 = endeme1.Index(c[i]);
		        int idx2 = endeme2.Index(c[i]);

		        double pair = 0.0;
		        if      (idx1 == NO_INDEX && idx2 == NO_INDEX) { pair = bothMissing; }
                else if (idx1 == NO_INDEX || idx2 == NO_INDEX) { pair = oneMissing; }
                else
                {
			        pair = MatchValue[idx1] * MatchValue[idx2];
			        mutualCount = mutualCount + 1;
		        }
		        total = total + pair;
	        }


            // --------------------------------------------------------------------------
            //  Handle exact match situations
            // --------------------------------------------------------------------------
	        double exactMatchCount = 0.0;
	        for (int i = 0; i <= c.Length - 1; i++)
            {
		        int idx = endeme1.Index(c[i]);
                if (idx == endeme2.Index(c[i])&& idx != NO_INDEX) { exactMatchCount = exactMatchCount + 1; }
	        }
	        if ((exactMatchCount > 0)) { total = total + (directMatch * exactMatchCount / (Math.Max(endeme1.Count, endeme2.Count))); }


            // --------------------------------------------------------------------------
            //  Handle length difference situations
            // --------------------------------------------------------------------------
            double maxlen = (double)Math.Max(endeme1.Count, endeme2.Count);
            double minlen = (double)Math.Min(endeme1.Count, endeme2.Count);
            if (maxlen > 0) total = total + something * minlen / maxlen;

	        return (total - 6353.113279);
        }
        internal double MatchTestMatch(Endeme endeme2, double geom, double oneMissing, double bothMissing, double something, double directMatch1, double exactMatch)
        {
            return RefinedMatch(this, endeme2, geom, oneMissing, bothMissing, something, directMatch1, exactMatch);
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- MatchTableJustInTime -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        private static Dictionary<int, double> MatchTableJustInTime(double geom)
        {
            if (_match == null || _match.Count < 2 || _match[0] != 100.0 || _match[1] != 100.0*geom)
            {
                // ----------------------------------------------------------------------
                //  Just in time build the match value dictionary
                // ----------------------------------------------------------------------
			    _match = new Dictionary<int, double>(25);
			    double start = 100.0;
			    for (int i = 0; i <= 23; i++) {
				    _match.Add(i, start);
				    start = start * geom;
			    }
			    _match.Add(NO_INDEX, 0.0);
            }
            return _match;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- MatchValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        private static Dictionary<int, double> MatchValue { get
        {
		    if (_match == null)
            {
			    _match = new Dictionary<int, double>(25);
			    double start = 100.0;
			    for (int i = 0; i < 24; i++) { _match.Add(i, start); start = start * 0.84; }
			    _match.Add(NO_INDEX, 0.0);
		    }
		    return _match;
	    } } private static Dictionary<int, double> _match;

        // ----------------------------------------------------------------------------------------
        /// <!-- Minus_old -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <remarks>almost identical code to Difference</remarks>
        /// <remarks>alpha code</remarks>
        private Endeme Minus_old(Endeme rhs)
        {
            // --------------------------------------------------------------------------
            //  Standard checks
            // --------------------------------------------------------------------------
            if (rhs == null   ) return Copy();
            if (Equals(rhs)   ) return Empty ;
            if (EnSet != rhs.EnSet) return Copy();


            // --------------------------------------------------------------------------
            //  Do the calculation
            // --------------------------------------------------------------------------
            Endeme endeme = new Endeme(this.EnSet);
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
            {
                endeme.Quant.Raw[c] = 32;
                if (this.Contains(c)) endeme.Quant.Raw[c] -= (22 - this[c]);
                if (rhs .Contains(c)) endeme.Quant.Raw[c] += (22 - rhs [c]);
            }
            endeme.CookEndeme();

            return endeme;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Minus -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <remarks>almost identical code to Difference</remarks>
        /// <remarks>alpha code</remarks>
        private Endeme Minus(Endeme rhs)
        {
            List<char> cha = _string;
            Endeme en = Copy();
            for (int i = 0; i < rhs._string.Count; ++i)
            {
                en._string.Remove(rhs._string[i]);
            }
            return new Endeme(EnSet, en.ToString());
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Move -->
        /// <summary>
        ///      Moves characters to the beginning and/or ending of the endeme
        /// </summary>
        /// <param name="toFront">the characters to move to the front</param>
        /// <param name="toBack">the characters to move to the back</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public Endeme Move(string toFront, string toBack)
        {
            Endeme temp = this;

            if (!string.IsNullOrEmpty(toFront)) temp = new Endeme(EnSet, toFront + Regex.Replace(temp.ToString(), "[" + toFront + "]", ""));
            if (!string.IsNullOrEmpty(toBack )) temp = new Endeme(EnSet, Regex.Replace(temp.ToString(), "[" + toBack  + "]", "") + toBack);
            this._string = temp._string;
            this._index  = temp._index;

            return this;
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Negation -->
        /// <summary>
        ///      Returns an endeme with all of and only the missing characteristics
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme Negation()
        {
            Endeme en = this.EnSet.RandomEndeme();
            foreach (char c in this.ToCharArray())
                en.Remove(c);
            return en;
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Of -->
        /// <summary>
        ///      Converts whatever is sent to it to an endeme
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Endeme Of(object obj)
        {
            if (obj == null) return new Endeme();
            Type typ = obj.GetType();

            // maybe do reflection to see if there is an endeme member of the object instead of adding this dependency
            if (typ == typeof(IEndemeItem))
            {
                EndemeItem item = (EndemeItem)obj;
                if (item.Item == null) return item.ItemEndeme;
                if (item.Item.Value.GetType() == typeof(Endeme)) return (Endeme)item.Item.Value;
                return new Endeme(item.EnSet, item.Item.ToString());
            }

            if (typ == typeof(Endeme)) return (Endeme)obj;
            string endeme = obj.ToString();
            return new Endeme(endeme);
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Or -->
        /// <summary>
        ///      'Bitwise' OR of two endemes
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme Or(Endeme rhs)
        {
            // --------------------------------------------------------------------------
            //  Standard checks
            // --------------------------------------------------------------------------
            if (rhs == null   ) return Copy();  // I'm not sure this is quite kosher, the raw quants will be wrong
            if (Equals(rhs)   ) return Copy();  // If they are the same, just return a copy
            if (EnSet != rhs.EnSet) return Copy();


            // --------------------------------------------------------------------------
            //  Do the calculation
            // --------------------------------------------------------------------------
            string str = "";
            foreach (char c in this.ToCharArray())                        str += c;
            foreach (char c in rhs .ToCharArray()) if (!this.Contains(c)) str += c;
            return new Endeme(this.EnSet, str);
        }

        // --------------------------------------------------------------------------------------
        /// <!-- GapsBeforeN -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="endeme"></param>
        /// <param name="relevantChar"></param>
        /// <returns></returns>
        private int GapsBeforeN(int n, Endeme endeme, List<char> relevantChar)
        {
            char c = this[n];
            int gapCount = 0;
            for (int i = 0; i < relevantChar.Count; ++i) { if (relevantChar[i] == c) break; else if (!this.Contains(relevantChar[i])) gapCount++; }
            return gapCount;
        }

        private Endeme Mask(List<char> mask)
        {
            Endeme en = new Endeme(this.EnSet);
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
                if (this.Contains(c) && mask.Contains(c)) en += c;
            return en;
        }
        private static long NFactorial(int N) { int fact = 1; for (int n = 1; n <= N; ++n) fact *= n; return fact; }

        // --------------------------------------------------------------------------------------
        /// <!-- OrdinalOf -->
        /// <summary>
        ///      Given a subset of the endeme set, what is the ordinal of the characters relevant to the subset
        /// </summary>
        /// <param name="relevantChars"></param>
        /// <returns></returns>
        public long OrdinalOf(string relevantChars)
        {
            List<char> cha = new List<char>(relevantChars.ToCharArray());
            Endeme enPart = this.Mask(cha);
            long ord = 0;
            if (enPart.Count > 0) { ord = 1; for (int i = 0; i < cha.Count; ++i) ord += enPart.OrdinalNof(i, cha); }
            return ord;
        }

        // --------------------------------------------------------------------------------------
        /// <!-- OrdinalNof -->
        /// <summary>
        ///      The part of the ordinal calculaton for one letter among the relevant characters
        /// </summary>
        /// <param name="n">the char in the endeme part</param>
        /// <param name="relevantChar">the list of relevant characters</param>
        /// <returns></returns>
        private long OrdinalNof(int n, List<char> relevantChar)
        {
            long output = 0;
            int baseNum = relevantChar.Count-n;
            if (this.Count > n)
            {
                long num1   = GapsBeforeN(n,this,relevantChar);
                long E      = (long)(2.7182818284590452353602875m * (decimal)NFactorial(baseNum));
                long num2   = GapsBeforeN(n-1,this,relevantChar);
                long suffix = 0; if (this[n] > this[n-1]) suffix = 1;

                output = num1*(E-2)/(baseNum-1) - num2*(E-1)/baseNum + suffix;
            }
            return output;
        }

        private void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Part -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="enFormatted"></param>
        /// <returns></returns>
        public static string Part(int n, string enFormatted)
        {
            string[] str = enFormatted.Split(":=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (str.Length > n) return str[n]; else return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RandomLetter -->
        /// <summary>
        ///      Picks a letter, the first being th most likely
        /// </summary>
        /// <param name="geom">the likelihood of not pocking the first letter, a number between 0.001 and 1.0, recommend 0.8</param>
        /// <returns>change this to use the new approach</returns>
        /// <remarks>production ready</remarks>
        public char RandomLetter(double geometric)
        {
            Random r = RandomSource.New().Random;
            char letter = ' ';
            for (int i = 0; i < _string.Count && letter == ' '; ++i)
                if (r.NextDouble() > geometric)
                    letter = _string[i];
            if (letter == ' ')
            {
                double num = Math.Abs(r.Next(6) + r.Next(6) + r.Next(6) + r.Next(6) - 8.5) - 0.5;
                if ((int)num < _string.Count)
                    letter = _string[(int)num];
            }

            return letter;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RandomLetter -->
        /// <summary>
        ///      Picks a letter, the first being th most likely
        /// </summary>
        /// <param name="level">ranges from 2:1 to 12:22</param>
        /// <returns>change this to use the new approach</returns>
        /// <remarks>
        ///       1  2  1/22 * (49/36)^10
        ///       2   
        ///       3  3  1/22 * (49/36)^9
        ///       4  4  1/22 * (49/36)^8
        ///       5   
        ///       6  5  1/22 * (49/36)^7
        ///       7   
        ///       8  6  1/22 * (49/36)^6
        ///       9   
        ///      10  7  1/22 * (49/36)^5
        ///      11   
        ///      12    
        ///      13  8  1/22 * (49/36)^4
        ///      14   
        ///      15  9  1/22 * (49/36)^3
        ///      16   
        ///      17   
        ///      18  10  1/22 * (49/36)^2
        ///      19    
        ///      20  11  1/22 * (49/36)
        ///      21    
        ///      22  12  1/22
        ///      
        /// 
        ///      production ready
        /// </remarks>
        public char RandomLetter(int end)
        {
            if (_string.Count == 0)
                return ' ';
            else
            {
                Random r = RandomSource.New().Random;
                char letter = ' ';

                double power = (22.0 - end) / 2.0;
                double factor = Math.Pow(49.0 / 36.0, power);
                double naive = factor / 22;

                double remaining = 1.0;
                double adjusted = naive;
                double percent = 0.0;


                int num = _string.Count;
                while (num >= _string.Count)
                {
                    num = _string.Count;
                    for (int position = 0; num >= _string.Count && position < 22; ++position)
                    {
                        remaining = remaining - percent;
                        adjusted = 22 * naive / (22 - position);
                        double ran = r.Next(10000);
                        if (ran <= adjusted * 10000)
                            num = position;
                    }
                }


                if ((int)num < _string.Count)
                    letter = _string[(int)num];
                return letter;
            }
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Remove -->
        /// <summary>
        ///      Removes a characteristic from the endeme
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public Endeme Remove(char c)
        {
            if (this.Contains(c))
            {
                if (_index == null)
                    _index = new Dictionary<char, int>(24);
                _string.Remove(c);
                _index.Remove(c);
            }

            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Reverse -->
        /// <summary>
        ///      Returns the reverse of the endeme
        /// </summary>
        /// <remarks>production ready</remarks>
        public Endeme Reverse { get
        {
            Endeme en = new Endeme();
            en._string = this.ToList();
            en._string.Reverse();
            en.BuildIndex();
            //en._isEmpty = _isEmpty;
            return en;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- RightShift -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme RightShift(int num)
        {
            if (num <= 0) return Copy();
            if (num >= Count) return new Endeme(EnSet);
            string endeme = this.ToString().Substring(0,Count-num);
            return new Endeme(EnSet, endeme);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Similarity -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private Endeme Similarity(Endeme rhs)
        {
            // --------------------------------------------------------------------------
            //  Standard checks
            // --------------------------------------------------------------------------
            if (rhs == null   ) return Empty ;
            if (Equals(rhs)   ) return Copy(); // If they are the same, just return a copy
            if (EnSet != rhs.EnSet) return Empty ;


            // --------------------------------------------------------------------------
            //  Do the calculation
            // --------------------------------------------------------------------------
            Endeme endeme = new Endeme(this.EnSet);
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
            {
                endeme.Quant.Raw[c] = 0.5;
                if (this.Contains(c)) endeme.Quant.Raw[c] += (22 - this[c]);
                if (rhs .Contains(c)) endeme.Quant.Raw[c] -= (22 - rhs [c]);
            }
            endeme.CookEndeme();

            return endeme;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Times -->
        /// <summary>
        ///      Overloads the * operator by merging the endemes using addition and linear series
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme Times(Endeme rhs)
        {
            // --------------------------------------------------------------------------
            //  Standard checks
            // --------------------------------------------------------------------------
            if (rhs == null      ) return Copy();  // I'm not sure this is quite kosher, the raw quants will be wrong
            if (Equals(rhs)      ) return Copy();  // If they are the same, just return a copy

            if (rhs.EnSet == null) // this is horrible, two different very different functionalities, one designed for += strings and one designed for + Endeme
            {
                //Add(rhs.ToString());
                return Copy();
            }
            else
            {
                if (rhs.EnSet != EnSet) return Copy();


                // --------------------------------------------------------------------------
                //  Do the calculation
                // --------------------------------------------------------------------------
                Endeme endeme = new Endeme(this.EnSet);
                foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
                {
                    endeme.Quant.Raw[c] = 0;
                    if (this.Contains(c))
                        endeme.Quant.Raw[c] += (endeme.EnSet.Count + 1 - this[c]);
                    if (rhs.Contains(c))
                        endeme.Quant.Raw[c] += (endeme.EnSet.Count + 1 - rhs[c]);
                }
                endeme.CookEndeme();

                return endeme;
            }
        }

        //// --------------------------------------------------------------------------------------
        ///// <!-- Times -->
        ///// <summary>
        /////      Overloads the * operator by merging the endemes using addition and geometric series
        ///// </summary>
        ///// <param name="rhs"></param>
        ///// <returns></returns>
        //private Endeme Times(Endeme rhs)
        //{
        //    Endeme enLhs = this.Copy();
        //    enLhs.GenerateCalc_geometric(0.93);
        //    Endeme enRhs = rhs.Copy();
        //    enRhs.GenerateCalc_geometric(0.93);


        //    // --------------------------------------------------------------------------
        //    //  Calculation a new endeme based on the values in 'Calc'
        //    // --------------------------------------------------------------------------
        //    Endeme endeme = new Endeme(this.EnSet);
        //    foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
        //    {
        //        endeme.Quant.Raw[c] = 0;
        //        if (enLhs.Contains(c)) endeme.Quant.Raw[c] += enLhs.Quant.Calc[c];
        //        if (enRhs.Contains(c)) endeme.Quant.Raw[c] += enRhs.Quant.Calc[c];
        //    }
        //    endeme.CookEndeme();

        //    return endeme;
        //}

        // --------------------------------------------------------------------------------------
        /// <!-- ToList -->
        /// <summary>
        ///      Returns a copy of the list of characters
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public List<char> ToList()
        {
            if (_string == null) return new List<char>();
            else
            {
                char[] cha = new char[_string.Count];
                this._string.CopyTo(cha);
                List<char> list = new List<char>(cha);
                return list;
            }
        }

        // --------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public string ToString(bool longFormat)
        {
            if (longFormat)
            {
                string setName;

                if (IsEmpty) return "empty";
                if (_set == null) setName = "[no set]"; else setName = _set.Label;
                if (string.IsNullOrEmpty(setName)) setName = "[unnamed set]";
                return (setName + ":" + ToString());
            }
            else
                return ToString();
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Xor -->
        /// <summary>
        ///      Returns the xor of this with an input endeme
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        private Endeme Xor(Endeme rhs)
        {
            // --------------------------------------------------------------------------
            //  Standard checks
            // --------------------------------------------------------------------------
            if (rhs == null   ) return Copy();  // I'm not sure this is quite kosher, the raw quants will be wrong
            if (Equals(rhs)   ) return Copy();  // If they are the same, just return a copy
            if (EnSet != rhs.EnSet) return Copy();


            // --------------------------------------------------------------------------
            //  Do the calculation
            // --------------------------------------------------------------------------
            string str = "";
            foreach (char c in this.ToCharArray()) if (!rhs .Contains(c)) str += c;
            foreach (char c in rhs .ToCharArray()) if (!this.Contains(c)) str += c;
            return new Endeme(this.EnSet, str);
        }


        public string OrderStr(string amen)
        {
            Dictionary<char,int> cha = new Dictionary<char,int>();
            char[] c = amen.ToCharArray();
            for (int i = 0; i < c.Length; ++i)
            {
                if (!cha.ContainsKey(c[i])) cha.Add(c[i],i);
            }

            char[] en = this.ToString().ToCharArray();
            string output = "";
            for (int i = 0; i < en.Length; ++i)
            {
                if (cha.ContainsKey(en[i]))
                    output += en[i];
            }

            return output;
        }
    }
}


