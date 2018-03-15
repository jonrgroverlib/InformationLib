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
using System;                         // for 
using System.Collections.Generic;     // for 
using System.ComponentModel.DataAnnotations; // for [Key]
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex
using System.Xml.Serialization;       // for XmlAttribute

namespace InformationLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeSet -->
    /// <summary>
    ///      The EndemeSet class is a combinable, orderable list of characteristics,
    ///      The endeme set itself has no order and is a 'type' or 'schema' for endemes
    /// </summary>
    /// <remarks>production ready</remarks>
    public class EndemeSet
    {
        // ----------------------------------------------------------------------------------------
        //  Basics
        // ----------------------------------------------------------------------------------------
        public  Dictionary<char, EndemeCharacteristic> Characteristic { get { return _char; } private set { _char = value; } }
        private Dictionary<char, EndemeCharacteristic> _char;


        // ----------------------------------------------------------------------------------------
        //  Class members
        /* ------------------------------------------------------------------------------------- */                       /// <summary>The number of characteristics used for combinations</summary>
        [XmlAttribute()] public Guid   SetId     { get { return EndemeSetId      ; } set { EndemeSetId       = value; } }
        [XmlAttribute()] public string SetCode   { get { return EndemeSetCode    ; } set { EndemeSetCode     = value; } } /// <summary>The name of the endeme set, does it make sense to use this natural key?</summary>
        [XmlAttribute()] public string Label     { get { return EndemeSetLabel   ; } set { EndemeSetLabel    = value; } }
        [XmlAttribute()] public string SetDescr  { get { return EndemeSetDescr   ; } set { EndemeSetDescr    = value; } }
                         public Endeme EnDefault { get { return new Endeme(this, DefaultEndeme); } set { if (value != null) DefaultEndeme = value.ToString(); else DefaultEndeme = null; } } /// <summary>The resource of the endeme set - what the characetristics are balanced within. I do'nt know if this makes any sense to keep</summary>
        [XmlAttribute()] public string Resource  { get { return EndemeSetResource; } set { EndemeSetResource = value; } }
        [XmlAttribute()] public string Version   { get { return EndemeSetVersion ; } set { EndemeSetVersion  = value; } }


        // ----------------------------------------------------------------------------------------
        //  Table Members
        // ----------------------------------------------------------------------------------------
      //[Key]
        public Guid   EndemeSetId       { get; set; }  // primary key
        public string EndemeSetCode     { get; set; }
        public string EndemeSetLabel    { get; set; }
        public string EndemeSetDescr    { get; set; }
        public string DefaultEndeme     { get; set; }
        public string EndemeSetResource { get; set; }
        public string EndemeSetVersion  { get; set; }
        public bool   OrderIsImportant  { get; set; }
        public List<EndemeCharacteristic> EndemeCharacteristicList { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Accessor
        // ----------------------------------------------------------------------------------------
        public EndemeCharacteristic this[char   c  ] { get {                                  if (_char.ContainsKey(c) ) return _char[c];   return null; } }
        public EndemeCharacteristic this[string lbl] { get { foreach (char c in _char.Keys) { if (_char[c].Label == lbl) return _char[c]; } return null; } }


        // ----------------------------------------------------------------------------------------
        //  Constructors        production ready
        // ----------------------------------------------------------------------------------------
        public EndemeSet(                         bool orderImportant = true) { Init(Guid.Empty    , ""   , "", orderImportant); }
        public EndemeSet(           string label, bool orderImportant = true) { Init(Guid.NewGuid(), label, "", orderImportant); }
        public EndemeSet(Guid guid, string label, bool orderImportant = true) { Init(guid          , label, "", orderImportant); }


        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeSet Constructor -->
        /// <summary>
        ///      Builds a set by picking a letter for every key
        /// </summary>
        /// <param name="label"></param>
        /// <param name="list"></param>
        /// <remarks>alpha code</remarks>
        public EndemeSet(string label, List<string> list, bool orderImportant = true)
        {
            Init(Guid.NewGuid(), label, "", orderImportant);
            if (list.Count <= 24)
            {
                Dictionary<string,char> charFor = AssignCharactersToStrings(list);
                foreach (string key in charFor.Keys)
                {
                    char c = charFor[key];
                    Add(c, key, key);
                }
            }
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties          production ready
        // ----------------------------------------------------------------------------------------
        public  static  EndemeSet  Empty                 { get { return new EndemeSet();                                                                    } } /// <summary>Returns an ordered list of the characteristic letters</summary>
        private         List<char> LettersSorted         { get { List<char> list = LettersUnsorted; list.Sort();  return list;                              } } /// <summary>Returns an unordered array of the characteristic letters</summary>
        public          bool       Contains     (string label) { foreach (EndemeCharacteristic value in _char.Values) if (value.Label == label) return true; return false; }
        public          bool       Contains     (char   key  ) { return _char.ContainsKey(key);                                                             }
        public          int        Count                 { get { return _char.Count;                                                                        } }
        public override int        GetHashCode  ()             { SanityCheck(); int hash = (this.Label+this.Version+this.Count).GetHashCode(); return hash; }
        public          char[]     Letters               { get { return LettersUnsorted.ToArray();                                                          } }
        public          void       Pause        ()             {                                                                                            }
        public          char[]     UnsortedAlphabet      { get { return LettersUnsorted.ToArray();                                                          } }

        public static   bool operator !=(EndemeSet lhs, EndemeSet rhs) { return !(lhs == rhs); }
        public static   bool operator ==(EndemeSet lhs, EndemeSet rhs)
        {
            bool loren = object.ReferenceEquals(null, lhs); // loren = L)eft O)bject R)eference E)quals N)ull
            bool roren = object.ReferenceEquals(null, rhs); // roren = R)ight O)bject R)eference E)quals N)ull
            if (loren && roren) return true;
            if (loren || roren) return false;
            return lhs.Equals(rhs);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a characteristic to the endeme set
        /// </summary>
        /// <param name="cha"></param>
        /// <param name="label"></param>
        /// <param name="descr"></param>
        /// <remarks>production ready</remarks>
        public void Add(EndemeCharacteristic characteristic)
        {
            char cha = characteristic.Letter;
            if (cha > 'Z')
            {
                cha = (char)((int)cha - 32);  // convert to upper case
                characteristic.LetterToUpper();
            }
            if (!_char.ContainsKey(cha))
                _char.Add(characteristic.Letter, characteristic);
            else
                Pause();
        }
        public void Add(char cha, string label, string descr)
        {
            if (cha > 'Z') cha = cha.ToString().ToUpper().ToCharArray()[0];
            if (Regex.IsMatch(cha.ToString(), "^[A-Z]"))
                _char.Add(cha, new EndemeCharacteristic(cha, label, descr));
        }  // convert to upper case
        public void Add(char cha, string code, string label, string descr) { if (cha > 'Z') cha = (char)((int)cha - 32); _char.Add(cha, new EndemeCharacteristic(cha, code, label, descr)); }  // convert to upper case

        // ----------------------------------------------------------------------------------------
        /// <!-- Characteristics -->
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>production ready</remarks>
        public List<EndemeCharacteristic> Characteristics()
        {
            List<char> letters = LettersSorted;
            List<EndemeCharacteristic> list = new List<EndemeCharacteristic>();
            foreach (char c in letters)
                list.Add(_char[c]);
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Diff -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eq"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public int Diff(Endeme e1, Endeme e2)
        {
            int total = 0;
            foreach (char cha in _char.Keys)
                if (e1.Contains(cha) && e2.Contains(cha))
                    total += Math.Abs((e1.Index(cha)) - (e2.Index(cha)));
                else
                    total += 49;
            return total;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Returns similarity of two sets - same name, same version, same letters, same labels,
        ///      does not check other details
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (Object.ReferenceEquals(obj, this)) return true;
            if (obj.GetType() != typeof(EndemeSet)) return false;
            EndemeSet target = (EndemeSet)obj;


            bool same
                =  (this.Label    == target.Label   ) 
                && (this.Version == target.Version)
                && (this.Count   == target.Count  )
                ;
            if (same)
            {
                foreach (char c in _char.Keys)
                {
                    EndemeCharacteristic cha = this[c];
                    if (cha != target[c])
                    {
                        same = false;
                        break;
                    }
                }
                return same;
            }
            else
                return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Standard Initializer, covering all members
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="resource"></param>
        /// <remarks>production ready</remarks>
        private void Init(Guid id, string label, string resource, bool orderImportant)
        {
            SetId    = id;
            Label    = label;
            Resource = resource;
            _char    = new Dictionary<char, EndemeCharacteristic>();
            Version  = "1.0";
            OrderIsImportant = orderImportant;
            SanityCheck();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Labels -->
        /// <summary>
        ///      Returns a common delimited string containing the labels of all the characteristics
        /// </summary>
        /// <remarks>production ready</remarks>
        public string Labels()
        {
            string str = "";
            string delim = "";
            foreach (char key in _char.Keys)
                { str += delim + key + _char[key].Label;  delim = ", "; }
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Labels -->
        /// <summary>
        ///      Returns a common delimited string showing the labels of the first n characteristics
        /// </summary>
        /// <param name="endeme">string specifying a (partial) endeme</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public string Labels(string endeme)
        {
            string list = "";
            string delim = "";
            char[] arr = endeme.ToCharArray();
            foreach (char c in arr)
            {
                list += delim + Characteristic[c].Label;
                delim = ", ";
            }
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Letter -->
        /// <summary>
        ///      Returns the letter for the characteristic specified by the labelPattern
        /// </summary>
        /// <param name="labelPattern">the label or regex pattern for the label</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public char Letter(string labelPattern)
        {
            char got = ' ';
            foreach (char key in _char.Keys)
            {
                if (Regex.IsMatch(_char[key].Label, labelPattern))
                {
                    if (got == ' ')
                        got = key;
                    else
                        throw new ArgumentException("Pattern '" + labelPattern + "' matches multiple characteristics"
                            + " in endeme set " + Label);
                }
            }
            return got;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LettersUnsorted -->
        /// <summary>
        ///      Returns an unordered list of the characteristic letters, WARNING: this is not random
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        internal List<char> LettersUnsorted { get
        {
            List<char> list = new List<char>(_char.Count);
            foreach (char key in _char.Keys)
                list.Add(key);
            return list;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- RandomCharacteristic -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public char RandomCharacteristic()
        {
            Random r = RandomSource.New().Random;
            List<char> key = new List<char>(_char.Keys);
            return key[r.Next(key.Count)];
        }
        /// <summary>Returns a random characteristic label</summary>
        public string RandomLabel() { return _char[RandomCharacteristic()].Label; }

        // ----------------------------------------------------------------------------------------
        /// <!-- RandomEndeme -->
        /// <summary>
        ///      Returns a random endeme, part of this set
        /// </summary>
        /// <param name="length"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public Endeme RandomEndeme(int length, Random r)
        {
            SanityCheck();
            List<char>    list   = new List<char>(_char.Keys);
            int           count  = Math.Min(length, list.Count);
            StringBuilder endeme = new StringBuilder(count);


            for (int i = 0; i < count; ++i)
            {
                int n = r.Next(list.Count);
                endeme.Append(list[n]);
                list.RemoveAt(n);
            }

            return (new Endeme(this, endeme.ToString()));
        }
        /// <summary>Returns a random endeme, part of this set</summary>
        /// <remarks>production ready</remarks>
        public Endeme RandomEndeme(Random r)
        {
            return RandomEndeme(9876543, r);
            //SanityCheck();
            //List<char> list = new List<char>(_char.Keys);
            //int count = list.Count;
            //StringBuilder endeme = new StringBuilder(count);
            //for (int i = 0; i < count; ++i)
            //{
            //    int n = r.Next(list.Count);
            //    endeme.Append(list[n]);
            //    list.RemoveAt(n);
            //}
            //return (new Endeme(this, endeme.ToString()));
        }
        public Endeme RandomEndeme() { return RandomEndeme(RandomSource.New().Random); }

        // ----------------------------------------------------------------------------------------
        /// <!-- SanityCheck -->
        /// <summary>
        ///      A sanity check for the endeme set used in various commonly run places
        /// </summary>
        /// <remarks>production ready</remarks>
        private void SanityCheck()
        {
            try
            {

                if (string.IsNullOrEmpty(this.Label) && this.SetId == Guid.Empty) return;
                if (string.IsNullOrEmpty(this.Label) || Regex.IsMatch(this.Label, "^[ \t\r\n]*$")) throw new Exception("Endeme Set Name may not be blank.");
                if (string.IsNullOrEmpty(this.Version) || Regex.IsMatch(this.Version, "^[ \t\r\n]*$")) throw new Exception("Endeme Set " + this.Label + " Version may not be blank.");
            }
            catch (Exception ex)
            {
                Is.Trash(ex);
              //throw;
            }
        } 

        // ----------------------------------------------------------------------------------------
        /// <!-- Set -->
        /// <summary>
        ///      Adds a new characteristic or sets an existing one to the new value
        /// </summary>
        /// <param name="cha"></param>
        /// <param name="label"></param>
        /// <param name="descr"></param>
        /// <remarks>production ready</remarks>
        public void Set(char cha, string label, string descr)
        {
            if (_char.ContainsKey(cha))
                _char[cha].Set(cha, label, descr);
            else
                _char.Add(cha, new EndemeCharacteristic(cha, label, descr));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ShowDetails -->
        /// <summary>
        ///      Lists the details for the top num characteristics in the endeme
        /// </summary>
        /// <param name="num"></param>
        /// <param name="endeme"></param>
        /// <remarks>production ready</remarks>
        public string ShowDetails(int num, Endeme endeme)
        {
            string str = "";
            string delim = "";
            for (int i = 0; i < num; ++i)
            {
                EndemeCharacteristic c = _char[endeme[i]];
                str += delim + c.Letter + ". " + c.Label.PadRight(22) + " " + c.Descr;
                delim = "\r\n";
            }
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ShowCharacteristics -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public string ShowCharacteristics()
        {
            List<EndemeCharacteristic> list = Characteristics();
            int maxLen = 0;
            foreach (EndemeCharacteristic cha in list)
                maxLen = Math.Max(maxLen, cha.Label.Length);


            string delim = "";
            string str = "";
            foreach (EndemeCharacteristic cha in list)
            {
                str += delim + cha.Letter + ". " + cha.Label.PadRight(maxLen) + " - " + cha.Descr;
                delim = "\r\n";
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Straight -->
        /// <summary>
        ///      Returns an endeme using all of the endeme set characteristics in order
        /// </summary>
        /// <remarks>TODO: make this absolutely consistent</remarks>
        /// <remarks>production ready</remarks>
        public Endeme Straight
        {
            get
            {
                string str = "";
                foreach (char cha in _char.Keys)
                    str += cha;
                Endeme e = new Endeme(this, str);
                return e;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Returns the endeme set in 'standard' format
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public override string ToString()
        {
            List<char>    keys  = LettersSorted;
            StringBuilder str   = new StringBuilder(keys.Count * 10); // *Magic number* alert
            string        delim = "";
            str.Append(Label + ": ");


            foreach (char key in keys)
            {
                string label = _char[key].Label;
                string cha = key.ToString();
                str.Append(delim + cha + ")");


                if      (label == null)                       { str.Append("<NULL CHAR LABEL>");  }
                else if (string.IsNullOrWhiteSpace(label))    { str.Append("<EMPTY CHAR LABEL>"); }
                else if (cha == label.Trim().Substring(0, 1)) { str.Append(label.Substring(1));   }
                else                                          { str.Append(label);                }

                delim = ", ";
            }

            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WetlandAnimals -->
        /// <summary>
        ///      Test endeme set
        /// </summary>
        /// <remarks>production ready - for unit testing</remarks>
        public static EndemeSet WetlandAnimals { get
        {
            EndemeSet set = new EndemeSet("Wetland animals");
            set.Add('A', "Alligator", "");
            set.Add('B', "Beaver"   , "");
            set.Add('C', "Crocodile", "");
            set.Add('D', "Duck"     , "");
            set.Add('E', "Egret"    , "");
            set.Add('F', "Frog"     , "");
            set.Add('G', "Gecko"    , "");
            set.Add('H', "Herron"   , "");
            set.Add('I', "Insect"   , "");
            set.Add('J', "Jackal"   , "");
            set.Add('K', "Koala"    , "");
            set.Add('L', "Lizard"   , "");
            set.Add('M', "Muskrat"  , "");
            set.Add('N', "Newt"     , "");
            set.Add('O', "Otter"    , "");
            set.Add('P', "Puma"     , "");
            set.Add('Q', "Quahog"   , "");
            set.Add('R', "Reptile"  , "");
            set.Add('S', "Snake"    , "");
            set.Add('T', "Turtle"   , "");
            set.Add('U', "Ungulate" , "");
            set.Add('V', "Vole"     , "");
            return set;
        } }

        #region Set builder
        // this should be its own class sometime

        // ----------------------------------------------------------------------------------------
        /// <!-- AssignCharactersToStrings -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <remarks>pre-alpha code, TODO: write this</remarks>
        private Dictionary<string, char> AssignCharactersToStrings(List<string> list)
        {
            Dictionary<string, char> assignment = new Dictionary<string,char>();
            //  build the matrix


            //  grab some synonyms
            Dictionary<string,List<string>> synonyms = new Dictionary<string,List<string>>();
            AddToSynonyms(synonyms, "answer, reply, retort, response");
            AddToSynonyms(synonyms, "contrast, difference, dissimilarity");
            AddToSynonyms(synonyms, "cessation, end, ending, finish, halt"); // noun forms
            AddToSynonyms(synonyms, "halt, discontinue, end, stop, finish"); // transitive verb forms
            AddToSynonyms(synonyms, "cease, stop, halt, end, finish, quit"); // instransitive verb forms
            AddToSynonyms(synonyms, "plan, plot, scheme, intention");
            AddToSynonyms(synonyms, "procedure, method, way");


            //  set the singles
            //  plow
            //  assign the rest
            return assignment;
        }

        /// <remarks>pre-alpha code</remarks>
        private void AddToSynonyms(Dictionary<string, List<string>> synonyms, string str)
        {
            string[] array = str.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>(array);
            for (int i = 0; i < list.Count; ++i)
            {
                for (int j = 0; j < list.Count; ++j)
                {
                    if (i != j)
                    {
                        string word = list[i];
                        string synonymOfWord = list[j];

                        if (!synonyms.ContainsKey(word))
                            synonyms.Add(word,new List<string>());
                        List<string> synonymListOfWord = synonyms[word];
                        if (!synonymListOfWord.Contains(synonymOfWord))
                            synonyms[word].Add(synonymOfWord);
                    }
                }
            }
        }

        // Nouns:
        //disagreement, incompatibility, inequity, conflict
        //stop, terminate, conclude, close, discontinuance
        //Fear — fright, dread, terror, alarm, dismay, anxiety, scare, awe, horror, panic, apprehension
        //Idea — thought, concept, conception, notion, understanding, opinion, plan, view, belief
        //Mark — label, tag, price, ticket, impress, effect, trace, imprint, stamp, brand, sign, note, heed, notice, designate
        //Part — portion, share, piece, allotment, section, fraction, fragment
        //Place — space, area, spot, plot, region, location, situation, position, residence, dwelling, set, site, station, status, state
        //Plan — plot, scheme, design, draw, map, diagram, procedure, arrangement, intention, device, contrivance, method, way, blueprint
        //Predicament — quandary, dilemma, pickle, problem, plight, spot, scrape, jam
        //Story — tale, myth, legend, fable, yarn, account, narrative, chronicle, epic, sage, anecdote, record, memoir
        //Trouble — distress, anguish, anxiety, worry, wretchedness, pain, danger, peril, disaster, grief, misfortune, difficulty, concern, pains, inconvenience, exertion, effort

        // Verbs:
        //Anger — enrage, infuriate, arouse, nettle, exasperate, inflame, madden
        //Answer — reply, respond, retort, acknowledge
        //Ask– — question, inquire of, seek information from, put a question to, demand, request, expect, inquire, query, interrogate, examine, quiz
        //Begin — start, open, launch, initiate, commence, inaugurate, originate
        //Break — fracture, rupture, shatter, smash, wreck, crash, demolish, atomize
        //Come — approach, advance, near, arrive, reach
        //Cry — shout, yell, yowl, scream, roar, bellow, weep, wail, sob, bawl
        //Cut — gash, slash, prick, nick, sever, slice, carve, cleave, slit, chop, crop, lop, reduce
        //Decide — determine, settle, choose, resolve
        //Describe — portray, characterize, picture, narrate, relate, recount, represent, report, record
        //Destroy — ruin, demolish, raze, waste, kill, slay, end, extinguish
        //Do — execute, enact, carry out, finish, conclude, effect, accomplish, achieve, attain
        //Enjoy — appreciate, delight in, be pleased, indulge in, luxuriate in, bask in, relish, devour, savor, like
        //Explain — elaborate, clarify, define, interpret, justify, account for
        //Fall — drop, descend, plunge, topple, tumble
        //Fly — soar, hover, flit, wing, flee, waft, glide, coast, skim, sail, cruise
        //Get — acquire, obtain, secure, procure, gain, fetch, find, score, accumulate, win, earn, rep, catch, net, bag, derive, collect, gather, glean, pick up, accept, come by, regain, salvage
        //Go — recede, depart, fade, disappear, move, travel, proceed
        //Hate — despise, loathe, detest, abhor, disfavor, dislike, disapprove, abominate
        //Have — hold, possess, own, contain, acquire, gain, maintain, believe, bear, beget, occupy, absorb, fill, enjoy
        //Help — aid, assist, support, encourage, back, wait on, attend, serve, relieve, succor, benefit, befriend, abet
        //Hide — conceal, cover, mask, cloak, camouflage, screen, shroud, veil
        //Hurry — rush, run, speed, race, hasten, urge, accelerate, bustle
        //Hurt — damage, harm, injure, wound, distress, afflict, pain
        //Keep — hold, retain, withhold, preserve, maintain, sustain, support
        //Kill — slay, execute, assassinate, murder, destroy, cancel, abolish
        //Look — gaze, see, glance, watch, survey, study, seek, search for, peek, peep, glimpse, stare, contemplate, examine, gape, ogle, scrutinize, inspect, leer, behold, observe, view, witness, perceive, spy, sight, discover, notice, recognize, peer, eye, gawk, peruse, explore
        //Love — like, admire, esteem, fancy, care for, cherish, adore, treasure, worship, appreciate, savor
        //Make — create, originate, invent, beget, form, construct, design, fabricate, manufacture, produce, build, develop, do, effect, execute, compose, perform, accomplish, earn, gain, obtain, acquire, get
        //Move — plod, go, creep, crawl, inch, poke, drag, toddle, shuffle, trot, dawdle, walk, traipse, mosey, jog, plug, trudge, slump, lumber, trail, lag, run, sprint, trip, bound, hotfoot, high-tail, streak, stride, tear, breeze, whisk, rush, dash, dart, bolt, fling, scamper, scurry, skedaddle, scoot, scuttle, scramble, race, chase, hasten, hurry, hump, gallop, lope, accelerate, stir, budge, travel, wander, roam, journey, trek, ride, spin, slip, glide, slide, slither, coast, flow, sail, saunter, hobble, amble, stagger, paddle, slouch, prance, straggle, meander, perambulate, waddle, wobble, pace, swagger, promenade, lunge
        //Put — place, set, attach, establish, assign, keep, save, set aside, effect, achieve, do, build
        //Run — race, speed, hurry, hasten, sprint, dash, rush, escape, elope, flee
        //Say/Tell — inform, notify, advise, relate, recount, narrate, explain, reveal, disclose, divulge, declare, command, order, bid, enlighten, instruct, insist, teach, train, direct, issue, remark, converse, speak, affirm, suppose, utter, negate, express, verbalize, voice, articulate, pronounce, deliver, convey, impart, assert, state, allege, mutter, mumble, whisper, sigh, exclaim, yell, sing, yelp, snarl, hiss, grunt, snort, roar, bellow, thunder, boom, scream, shriek, screech, squawk, whine, philosophize, stammer, stutter, lisp, drawl, jabber, protest, announce, swear, vow, content, assure, deny, dispute
        //Show — display, exhibit, present, note, point to, indicate, explain, reveal, prove, demonstrate, expose
        //Stop — cease, halt, stay, pause, discontinue, conclude, end, finish, quit
        //Take — hold, catch, seize, grasp, win, capture, acquire, pick, choose, select, prefer, remove, steal, lift, rob, engage, bewitch, purchase, buy, retract, recall, assume, occupy, consume
        //Tell — disclose, reveal, show, expose, uncover, relate, narrate, inform, advise, explain, divulge, declare, command, order, bid, recount, repeat
        //Think — judge, deem, assume, believe, consider, contemplate, reflect, mediate
        //Use — employ, utilize, exhaust, spend, expend, consume, exercise

        // Adjectives:
        //Amazing — incredible, unbelievable, improbable, fabulous, wonderful, fantastic, astonishing, astounding, extraordinary
        //Angry — mad, furious, enraged, excited, wrathful, indignant, exasperated, aroused, inflamed
        //Awful — dreadful, terrible, abominable, bad, poor, unpleasant
        //Bad — evil, immoral, wicked, corrupt, sinful, depraved, rotten, contaminated, spoiled, tainted, harmful, injurious, unfavorable, defective, inferior, imperfect, substandard, faulty, improper, inappropriate, unsuitable, disagreeable, unpleasant, cross, nasty, unfriendly, irascible, horrible, atrocious, outrageous, scandalous, infamous, wrong, noxious, sinister, putrid, snide, deplorable, dismal, gross, heinous, nefarious, base, obnoxious, detestable, despicable, contemptible, foul, rank, ghastly, execrable
        //Beautiful — pretty, lovely, handsome, attractive, gorgeous, dazzling, splendid, magnificent, comely, fair, ravishing, graceful, elegant, fine, exquisite, aesthetic, pleasing, shapely, delicate, stunning, glorious, heavenly, resplendent, radiant, glowing, blooming, sparkling
        //Big — enormous, huge, immense, gigantic, vast, colossal, gargantuan, large, sizable, grand, great, tall, substantial, mammoth, astronomical, ample, broad, expansive, spacious, stout, tremendous, titanic, mountainous
        //Brave — courageous, fearless, dauntless, intrepid, plucky, daring, heroic, valorous, audacious, bold, gallant, valiant, doughty, mettlesome
        //Bright — shining, shiny, gleaming, brilliant, sparkling, shimmering, radiant, vivid, colorful, lustrous, luminous, incandescent, intelligent, knowing, quick-witted, smart, intellectual
        //Calm — quiet, peaceful, still, tranquil, mild, serene, smooth, composed, collected, unruffled, level-headed, unexcited, detached, aloof
        //Cool — chilly, cold, frosty, wintry, icy, frigid
        //Crooked — bent, twisted, curved, hooked, zigzag
        //Dangerous — perilous, hazardous, risky, uncertain, unsafe
        //Dark — shadowy, unlit, murky, gloomy, dim, dusky, shaded, sunless, black, dismal, sad
        //Definite — certain, sure, positive, determined, clear, distinct, obvious
        //Delicious — savory, delectable, appetizing, luscious, scrumptious, palatable, delightful, enjoyable, toothsome, exquisite
        //Dull — boring, tiring„ tiresome, uninteresting, slow, dumb, stupid, unimaginative, lifeless, dead, insensible, tedious, wearisome, listless, expressionless, plain, monotonous, humdrum, dreary
        //Eager — keen, fervent, enthusiastic, involved, interested, alive to
        //Fair — just, impartial, unbiased, objective, unprejudiced, honest
        //False — fake, fraudulent, counterfeit, spurious, untrue, unfounded, erroneous, deceptive, groundless, fallacious
        //Famous — well-known, renowned, celebrated, famed, eminent, illustrious, distinguished, noted, notorious
        //Fast — quick, rapid, speedy, fleet, hasty, snappy, mercurial, swiftly, rapidly, quickly, snappily, speedily, lickety-split, posthaste, hastily, expeditiously, like a flash
        //Fat — stout, corpulent, fleshy, beefy, paunchy, plump, full, rotund, tubby, pudgy, chubby, chunky, burly, bulky, elephantine
        //Funny — humorous, amusing, droll, comic, comical, laughable, silly
        //Good — excellent, fine, superior, wonderful, marvelous, qualified, suited, suitable, apt, proper, capable, generous, kindly, friendly, gracious, obliging, pleasant, agreeable, pleasurable, satisfactory, well-behaved, obedient, honorable, reliable, trustworthy, safe, favorable, profitable, advantageous, righteous, expedient, helpful, valid, genuine, ample, salubrious, estimable, beneficial, splendid, great, noble, worthy, first-rate, top-notch, grand, sterling, superb, respectable, edifying
        //Great — noteworthy, worthy, distinguished, remarkable, grand, considerable, powerful, much, mighty
        //Gross — improper, rude, coarse, indecent, crude, vulgar, outrageous, extreme, grievous, shameful, uncouth, obscene, low
        //Happy — pleased, contented, satisfied, delighted, elated, joyful, cheerful, ecstatic, jubilant, gay, tickled, gratified, glad, blissful, overjoyed
        //Important — necessary, vital, critical, indispensable, valuable, essential, significant, primary, principal, considerable, famous, distinguished, notable, well-known
        //Interesting — fascinating, engaging, sharp, keen, bright, intelligent, animated, spirited, attractive, inviting, intriguing, provocative, though-provoking, challenging, inspiring, involving, moving, titillating, tantalizing, exciting, entertaining, piquant, lively, racy, spicy, engrossing, absorbing, consuming, gripping, arresting, enthralling, spellbinding, curious, captivating, enchanting, bewitching, appealing
        //Lazy — indolent, slothful, idle, inactive, sluggish
        //Little — tiny, small, diminutive, shrimp, runt, miniature, puny, exiguous, dinky, cramped, limited, itsy-bitsy, microscopic, slight, petite, minute
        //Mischievous — prankish, playful, naughty, roguish, waggish, impish, sportive
        //Moody — temperamental, changeable, short-tempered, glum, morose, sullen, mopish, irritable, testy, peevish, fretful, spiteful, sulky, touchy
        //Neat — clean, orderly, tidy, trim, dapper, natty, smart, elegant, well-organized, super, desirable, spruce, shipshape, well-kept, shapely
        //New — fresh, unique, original, unusual, novel, modern, current, recent
        //Old — feeble, frail, ancient, weak, aged, used, worn, dilapidated, ragged, faded, broken-down, former, old-fashioned, outmoded, passe, veteran, mature, venerable, primitive, traditional, archaic, conventional, customary, stale, musty, obsolete, extinct
        //Popular — well-liked, approved, accepted, favorite, celebrated, common, current
        //Quiet — silent, still, soundless, mute, tranquil, peaceful, calm, restful
        //Right — correct, accurate, factual, true, good, just, honest, upright, lawful, moral, proper, suitable, apt, legal, fair
        //Scared — afraid, frightened, alarmed, terrified, panicked, fearful, unnerved, insecure, timid, shy, skittish, jumpy, disquieted, worried, vexed, troubled, disturbed, horrified, terrorized, shocked, petrified, haunted, timorous, shrinking, tremulous, stupefied, paralyzed, stunned, apprehensive
        //Slow — unhurried, gradual, leisurely, late, behind, tedious, slack
        //Strange — odd, peculiar, unusual, unfamiliar, uncommon, queer, weird, outlandish, curious, unique, exclusive, irregular
        //True — accurate, right, proper, precise, exact, valid, genuine, real, actual, trusty, steady, loyal, dependable, sincere, staunch
        //Ugly — hideous, frightful, frightening, shocking, horrible, unpleasant, monstrous, terrifying, gross, grisly, ghastly, horrid, unsightly, plain, homely, evil, repulsive, repugnant, gruesome
        //Unhappy — miserable, uncomfortable, wretched, heart-broken, unfortunate, poor, downhearted, sorrowful, depressed, dejected, melancholy, glum, gloomy, dismal, discouraged, sad
        //Wrong — incorrect, inaccurate, mistaken, erroneous, improper, unsuitable

        #endregion Set builder

    }
}
