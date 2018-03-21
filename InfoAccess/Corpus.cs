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
using InformationLib.Data    ;        // for 
using InformationLib.HardData;        // for 
using InformationLib.Strings ;        // for _.GetStringFromFile
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.InfoAccess
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Corpus -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code - nearly beta</remarks>
    public class Corpus
    {
        // ----------------------------------------------------------------------------------------
        //  Members and Properties
        // ----------------------------------------------------------------------------------------
        public  Dictionary<string, int> WordTally   { get { return _wordTally  ; } } private Dictionary<string,int> _wordTally  ;
        public  Dictionary<string, int> RegularWord { get { return _regularWord; } } private Dictionary<string,int> _regularWord;
        public  Dictionary<string, int> ProperNoun  { get { return _properNoun ; } } private Dictionary<string,int> _properNoun ;

        public         string CorpusPath      { get; set; }
        private static string CommonSplitter = " .,;:?'()!_";
        private        string DisplayName     { get; set; }
        public         int    ContentStartCol { get; private set; }

        public string[] FullLine    { get { if (_lines    == null) { _lines    = SplitCorpus    (); } return _lines   ; } } private string[] _lines   ;
        public string[] MetaData    { get { if (_metaData == null) { _metaData = ExtractMetaData(0); } return _metaData; } } private string[] _metaData;
        public string[] LineContent { get { if (_content  == null) { _content  = ExtractContent (); } return _content ; } } private string[] _content ;


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public Corpus(string corpusPath, string displayName, int contentStartCol, int contentStartIdx)
        {
            // --------------------------------------------------------------------------
            //  1. Read the corpus
            // --------------------------------------------------------------------------
            DisplayName = displayName;
            ContentStartCol  = contentStartCol;
            if (corpusPath != CorpusPath)
                { CorpusPath = corpusPath;  string[] lines = FullLine; }


            // --------------------------------------------------------------------------
            //  1. Tokenize into a dictionary
            //  2. Move all lower case words to another dictionary
            //  3. Move all proper names to a third dictionary
            //  4. extract metadata for each line
            // --------------------------------------------------------------------------
            _wordTally   = BuildWordTally     (contentStartIdx);
            _regularWord = ExtractRegularWords();
            _properNoun  = ExtractProperNouns ();
            _metaData    = ExtractMetaData    (contentStartIdx);
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public static List<string> OrderWordsHighToLow(Dictionary<string, double> words) { return ( from word in words orderby word.Value descending select word.Key ).ToList<string>(); }
        public static List<string> OrderWordsLowToHigh(Dictionary<string, double> words) { return ( from word in words orderby word.Value ascending  select word.Key ).ToList<string>(); }


        // ----------------------------------------------------------------------------------------
        /// <!-- BuildWordTally -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        /// <remarks>relies on StartIndex and CorpusLines</remarks>
        private Dictionary<string, int> BuildWordTally(int contentStartIdx)
        {
            Dictionary<string, int> wordCount = new Dictionary<string, int>();

            for (int i = contentStartIdx; i < FullLine.Length; ++i)
            {
                string fullLine = FullLine[i];
                if (fullLine.Length > 0 &&
                    FullLine[i].Substring(0,1) != ">" &&
                    fullLine.Length > ContentStartCol)
                {
                    string line = FullLine[i].Substring(ContentStartCol);
                    string[] wordList = line.Split(CommonSplitter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < wordList.Length; ++j)
                    {
                        string oneword = wordList[j];
                        if (Regex.IsMatch(oneword, "[^-A-Za-z]"))
                            Pause();
                        if (wordCount.ContainsKey(oneword)) wordCount[oneword]++;
                        else wordCount.Add(oneword, 1);
                    }
                }
                else
                    Pause();
            }

            return wordCount;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Compare -->
        /// <summary>
        ///      Compares two corpuses
        /// </summary>
        /// <param name="corpusA"></param>
        /// <param name="corpusB"></param>
        /// <returns></returns>
        public static string Compare(Corpus corpusA, Corpus corpusB)
        {
            Dictionary<string, double> both    = Corpus.CompareCommon(corpusA, corpusB);
            Dictionary<string, double> thisOne = Corpus.CompareAll   (corpusA, corpusB);


            List<string> corpusB_Both = Corpus.OrderWordsLowToHigh(both   );
            List<string> corpusB_This = Corpus.OrderWordsLowToHigh(thisOne);
            List<string> corpusA_Both = Corpus.OrderWordsHighToLow(both   );
            List<string> corpusA_This = Corpus.OrderWordsHighToLow(thisOne);


            int colWidth = 24;

            string str = (corpusB.TotalWords().ToString() + " words").PadRight(colWidth*2)
                + (corpusA.TotalWords().ToString() + " words").PadRight(colWidth*2);
            str += "\r\n";
            str += (corpusB.DisplayName + "(both)").PadRight(colWidth)
                +  (corpusB.DisplayName + "(this)").PadRight(colWidth)
                +  (corpusA.DisplayName + "(both)").PadRight(colWidth)
                +  (corpusA.DisplayName + "(this)").PadRight(colWidth)
                ;

            str += "\r\n";
            str += "----------".PadRight(colWidth);
            str += "----------".PadRight(colWidth);
            str += "----------".PadRight(colWidth);
            str += "----------".PadRight(colWidth);

            for (int i = 0; i < 50; ++i)
            {
                str += "\r\n"
                    + corpusB_Both[i].PadRight(colWidth)
                    + corpusB_This[i].PadRight(colWidth)
                    + corpusA_Both[i].PadRight(colWidth)
                    + corpusA_This[i].PadRight(colWidth);
            }

            return str;
        }

        private static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CompareAll -->
        /// <summary>
        ///      Compares two corpuses returning scores for all words in either corpus
        /// </summary>
        /// <param name="myBible"></param>
        /// <param name="resumes"></param>
        /// <returns></returns>
        public static Dictionary<string, double> CompareAll(Corpus myBible, Corpus resumes)
        {
            double bibleCount  = myBible.TotalWords();
            double resumeCount = resumes.TotalWords();
            double ratio       = bibleCount / resumeCount;


            Dictionary<string, double> either = new Dictionary<string, double>();
            foreach (string word in myBible.RegularWord.Keys)
            {
                either.Add(word, myBible.RegularWord[word] * 2.0);
            }
            foreach (string word in resumes.RegularWord.Keys)
            {
                if (either.ContainsKey(word))
                {
                    double newValue = myBible.RegularWord[word] / resumes.RegularWord[word];
                    either[word] = newValue;
                }
                else
                    either.Add(word, 0.5 / resumes.RegularWord[word]);
            }
            return either;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CompareCommon -->
        /// <summary>
        ///      Compares two corpuses returning scores only for words in both
        /// </summary>
        /// <param name="myBible"></param>
        /// <param name="resumes"></param>
        /// <returns></returns>
        public static Dictionary<string, double> CompareCommon(Corpus myBible, Corpus resumes)
        {
            Dictionary<string, double> both  = new Dictionary<string, double>();
            Dictionary<string, double> maybe = new Dictionary<string, double>();
            foreach (string word in myBible.RegularWord.Keys) maybe.Add(word, 1);
            foreach (string word in resumes.RegularWord.Keys)
                if (maybe.ContainsKey(word)) both.Add(word, myBible.RegularWord[word] / resumes.RegularWord[word]);

            return both;
        }

        private string[] ExtractContent()
        {
            string[] output = new string[FullLine.Length];
            for (int i = 0; i < FullLine.Length; ++i)
            {
                string fullLine = FullLine[i];
                if (fullLine.Length > 0 &&
                    FullLine[i].Substring(0,1) != ">" &&
                    fullLine.Length > ContentStartCol)
                {
                    string line = FullLine[i].Substring(ContentStartCol);
                    string meta = FullLine[i].Substring(0,ContentStartCol);
                    output[i] = line;
                }
                else
                {
                    output[i] = "";
                }
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractMetaData -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string[] ExtractMetaData(int contentStartIdx)
        {
            string[] output = new string[FullLine.Length];
            for (int i = 0; i < FullLine.Length; ++i)
            {
                string fullLine = FullLine[i];
                if (fullLine.Length > 0 &&
                    FullLine[i].Substring(0,1) != ">" &&
                    fullLine.Length > ContentStartCol)
                {
                    string line = FullLine[i].Substring(ContentStartCol);
                    string meta = FullLine[i].Substring(0,ContentStartCol);
                    output[i] = meta;
                }
                else
                {
                    output[i] = "";
                }
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractProperNouns -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WordTally"></param>
        /// <param name="RegularWord"></param>
        /// <returns></returns>
        /// <remarks>relies on RegularWord and WordTally</remarks>
        private Dictionary<string, int> ExtractProperNouns()
        {
                        //  Resolve a list of keys

            Dictionary<string, int> proper = new Dictionary<string, int>();
            List<string> keys = WordTally.Keys.ToList();
            foreach (string word in keys)
            {
                if (WordTally[word] > 0)
                {
                    string lower = word.ToLower();
                    if (RegularWord.ContainsKey(lower))
                        RegularWord[lower] += WordTally[word];
                    else
                        proper.Add(word, WordTally[word]);
                    WordTally[word] = 0;
                }
            }

            return proper;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractRegularWords -->
        /// <summary>
        ///      Returns a dictionary of the words that are not proper nowns
        /// </summary>
        /// <param name="wordCount"></param>
        /// <returns></returns>
        /// <remarks>relies on WordTally</remarks>
        public Dictionary<string, int> ExtractRegularWords()
        {
            Dictionary<string, int> regular = new Dictionary<string, int>();
            List<string> keys = WordTally.Keys.ToList();
            int min = Convert.ToInt32('a');


            foreach (string word in keys)
            {
                if (Convert.ToInt32(word.ToCharArray(0, 1)[0]) >= min)
                {
                    regular.Add(word, WordTally[word]);
                    WordTally[word] = 0;
                }
                else if (word == "I")
                {
                    regular.Add(word, WordTally[word]);
                    WordTally[word] = 0;
                }
            }
            return regular;
        }

        public string GetLine(int lineNum)
        {
            if (0 <= lineNum && lineNum < FullLine.Length)
                return FullLine[lineNum];
            else return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OrderWordsByValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<string> OrderWordsByValue(string text)
        {
            string[] textWords = text.Split(CommonSplitter.ToCharArray());
            Dictionary<string, int> local = new Dictionary<string, int>();
            for (int i = 0; i < textWords.Length; ++i)
            {
                string textWord = textWords[i];
                if (!local.ContainsKey(textWord) && RegularWord.ContainsKey(textWord))
                    local.Add(textWord, RegularWord[textWord]);
            }

            List<string> sorted = ( from item in local orderby item.Value select item.Key ).ToList<string>();

            return sorted;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RandomLine -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public string RandomLine(Random r)
        {
            if (FullLine.Length > 0) return FullLine[r.Next(FullLine.Length)];
            else return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SplitCorpus -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string[] SplitCorpus()
        {
            string[] line = null;
            string doc = FilesIO.GetStringFromFile(CorpusPath, ""); line = doc.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return line;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TotalWords -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int TotalWords()
        {
            int count = 0;
            foreach (int val in RegularWord.Values)
                count += val;
            return count;
        }

    }
}
