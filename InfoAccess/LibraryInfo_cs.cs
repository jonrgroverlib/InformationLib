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
using InformationLib.HardData;        // for FileIO.GetFileList
using System;                         // for 
using System.Collections.Generic;     // for 
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.InfoAccess
{
    // --------------------------------------------------------------------------------------------
    /// <!-- LibraryInfo_cs -->
    /// <summary>
    ///      The LibraryInfo_cs class analyses the structure of a c# code library
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class LibraryInfo_cs
    {
        // ----------------------------------------------------------------------------------------
        //  Properties
        // ----------------------------------------------------------------------------------------
        public List<string>    FilePaths   { get { return _filePaths;   } set { _filePaths   = value; } }  private List<string>    _filePaths  ;
        public List<ClassInfo> Classes     { get { return _classes;     } set { _classes     = value; } }  private List<ClassInfo> _classes    ;
        public string          LibraryPath { get { return _libraryPath; } set { _libraryPath = value; } }  private string          _libraryPath;
        public string          FileFilter  { get { return _fileFilter;  } set { _fileFilter  = value; } }  private string          _fileFilter ;


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public LibraryInfo_cs(string libPath, string filePattern)
        {
            LibraryPath = libPath;
            FileFilter  = filePattern;
            FilePaths   = FilesIO.GetFileList(LibraryPath, FileFilter);
            Classes     = new List<ClassInfo>();
            foreach (string filePath in FilePaths)
            {
                ClassInfo.AddTo(Classes, LoadClassInfo_cs(filePath));
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Display -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Display()
        {
            StringBuilder str = new StringBuilder("Contents of " + LibraryPath + ":");
            str.Append("\r\n");
            str.Append("\r\n");
            string delim = "";
            foreach (string item in FilePaths) {
                str.Append(delim + item);
                delim = "\r\n";
            }
            str.Append("\r\n");
            str.Append("\r\n");
            delim = "";
            foreach (ClassInfo item in Classes) {
                str.Append(delim + item.Display());
                delim = "\r\n";
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetClassBody_cs -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string GetClassBody_cs(string str)
        {
            string classBody = Regex.Replace(str, "^`class +[^ ]+ "                             , ""   , RegexOptions.Singleline);
            classBody =  Regex.Replace(classBody, "^([^']*)'.*$"                                , "$1" , RegexOptions.Singleline);
            classBody =  Regex.Replace(classBody, "^ *:[^(]*(public|private|internal|protected)", " $1", RegexOptions.Singleline); // if the class inherits from another
            classBody =  Regex.Replace(classBody, "^ *:[^(]*$"                                  , ""   , RegexOptions.Singleline); // if the class contains no methods
            return classBody;
        }

	    // ----------------------------------------------------------------------------------------
	    /// <!-- GetMethodList_cs -->
	    /// <summary>
	    ///      Parses out the methods from the class
	    /// </summary>
	    /// <param name="classBody"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private List<string> GetMethodList_cs(string classBody)
	    {
		    // --------------------------------------------------------------------------
		    //  Preprocess the class body
		    // --------------------------------------------------------------------------"
            classBody = Regex.Replace(classBody, "[\\[][^\\[\\]]+[\\]]"                        , ""          , RegexOptions.Singleline);
            classBody = Regex.Replace(classBody, "\\(int\\)"                                   , ""                                   );
            classBody = Regex.Replace(classBody, "^ *(private|public|internal|protected)[^(]+$", ""          , RegexOptions.Multiline ); // remove members
            classBody = Regex.Replace(classBody, "[^(]+ *= *new [^(]+[(][)];"                  , ""          , RegexOptions.Singleline); // remove members // WeakRefDictionary<string, object> _weakDictionary = new WeakRefDictionary<string, object>(); 
            classBody = Regex.Replace(classBody, " }([ \r\n]*)(public|private|internal|protected)( [^(]+\\([^(]*\\)[ \r\n]*){"                          , " '$1$2$3` ", RegexOptions.Singleline); // identify method bodies
            classBody = Regex.Replace(classBody, " }([ \r\n]*)(public|private|internal|protected)( [^(]+\\([^(]*\\) where [^(]+ : [^(]+\\(\\)[ \r\n]*){", " '$1$2$3` ", RegexOptions.Singleline); // identify method bodies // public TEntity GetEntity<TEntity>(string key) where TEntity : EntityBase, new() 
            classBody = Regex.Replace(classBody, "^([ \r\n]*)(public|private|internal|protected)( [^(]+\\([^(]*\\)[ \r\n]*){" , " $1$2$3` " , RegexOptions.Singleline);
            classBody = Regex.Replace(classBody, "}[ \r\n]*$"                                                                 , "'"         , RegexOptions.Singleline);


		    // --------------------------------------------------------------------------
		    //  Parse out the methods - TODO: add in Properties
		    // --------------------------------------------------------------------------
		    List<string> mList = new List<string>();
		    string last = classBody + "hi world";
		    string str = classBody;
		    while ((0 < str.Length && str.Length < last.Length)) {
			    string methodName = Regex.Replace(str, "[ \r\n]*`.*$"                                  , "", RegexOptions.Singleline);
                methodName = Regex.Replace(methodName, "^[ \r\n]*(public|private|internal|protected) *", "", RegexOptions.Singleline);
                methodName = Regex.Replace(methodName, "^.* ([^ ]+ *\\()"                              , "$1"                       );
                methodName = Regex.Replace(methodName, "[\r\n]"                                        , "", RegexOptions.Singleline);
                methodName = Regex.Replace(methodName, "^.*public"                                     , "", RegexOptions.Singleline);
			    if ((methodName.Length < 256)) {
				    mList.Add(methodName);
			    }
			    last = str;
			    str = Regex.Replace(str, "^[^']*'", "");
		    }

		    return mList;
	    }

        // -----------------------------------------------------------------------------------
	    /// <!-- LoadClassInfo_cs -->
	    /// <summary>
	    ///      Parses the class and method info out of a .cs file
	    /// </summary>
	    /// <param name="filePath">path of cs file to load class info from</param>
	    /// <returns></returns>
	    /// <remarks>include modules</remarks>
	    private List<ClassInfo> LoadClassInfo_cs(string filePath)
	    {
		    string str1 = FilesIO.LoadStringFromFile(filePath, "");
            if (Regex.IsMatch(filePath, "Test.aspx.cs$"))
            {
                Pause();
            }


            // --------------------------------------------------------------------------
            //  Extract a list of inported/used namespaces
            // --------------------------------------------------------------------------
            string importStr = Regex.Replace(str1     , "^using "  , "* ", RegexOptions.Multiline);
            importStr        = Regex.Replace(importStr, "^[^*].*$" , ""  , RegexOptions.Multiline);
            importStr        = Regex.Replace(importStr, "^[*] "    , ""  , RegexOptions.Multiline);
            string[] imp     = importStr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            // --------------------------------------------------------------------------
            //  Preprocess the file body
            // --------------------------------------------------------------------------
            string str10 = Preprocess_cs(str1);


		    List<ClassInfo> classList = new List<ClassInfo>();


            //// --------------------------------------------------------------------------
            ////  Parse out nested classes
            //// --------------------------------------------------------------------------
            //string classSegment = "";
            //while ((Regex.IsMatch(str10, "[`][^'`]+[`][^'`]+['][^'`]+[']", RegexOptions.Singleline)))
            //{
            //    classSegment     = Regex.Replace(str10       , "^.*[`][^'`]+[`]([^'`]+)['][^'`]+['].*$", "$1"   , RegexOptions.Singleline);
            //    string className = Regex.Replace(classSegment, "^(Class|Module) +([^ (]+)[ (].*$"      , "$1 $2", RegexOptions.Singleline);
            //    string classBody = Regex.Replace(classSegment, "^(Class|Module) +([^ (]+)"             , ""     , RegexOptions.Singleline);
            //    classList.Add(new ClassInfo(className, filePath));
            //    classList[classList.Count - 1].Method = GetMethodList(classBody);
            //    str10 = Regex.Replace(str10, "^(.*[`][^'`]+)[`][^'`]+[']([^'`]+['].*)$", "$1 $2", RegexOptions.Singleline);
            //}


            // --------------------------------------------------------------------------
            //  Parse out the classes and modules
            // --------------------------------------------------------------------------
            string str = str10;
            string last = str + "hi world";
            while ((str.Length > 0 && str.Length < last.Length))
            {
                string className = Regex.Replace(str, "^`(class) +([^ (]+)[ (].*$", "$1 $2", RegexOptions.Singleline);
                if (Regex.IsMatch(className, "^class SqlAccessMasterProviderBase"))
                {
                    Pause();
                }
                if (className.Length < 255)
                {
                    classList.Add(new ClassInfo(className, filePath));
                    string classBody = GetClassBody_cs(str);
                    classList[classList.Count - 1].Method = GetMethodList_cs(classBody);
                }
                else
                    Pause();
                last = str;
                str = Regex.Replace(str, "^`[^`]*", "");
            }


		    return classList;
	    }

        private void Pause()
        {
        }

	    // ----------------------------------------------------------------------------------------
	    /// <!-- Preprocess_cs -->
        /// <summary>
        ///      Preprocesses the file body of a cs file
        /// </summary>
        /// <param name="str1"></param>
        /// <returns></returns>
        private string Preprocess_cs(string str1)
        {
            // --------------------------------------------------------------------------
            //
            // --------------------------------------------------------------------------
            string str1b = Regex.Replace(str1 , "^using.*$"                      , ""      , RegexOptions.Multiline );
            string str2  = Regex.Replace(str1b, "#(end)?region.*$"               , ""      , RegexOptions.Multiline ); // remove region tags
            string str3  = Regex.Replace(str2 , "//.*$"                          , ""      , RegexOptions.Multiline ); // remove commented lines
            string str4  = Regex.Replace(str3 , "([\r\n]+)"                      , " \r\n ", RegexOptions.Singleline); // remove multiple carriage returns
            string str5  = Regex.Replace(str4 , "\t"                             , "    "  , RegexOptions.Singleline); // remove tabs
            string str6a = Regex.Replace(str5 , "`"                              , "JeSuS"                          ); // we are going to use ` later
            string str6b = Regex.Replace(str6a, "'"                              , "jEsUs"                          ); // we are going to use ' later
            string str6c = Regex.Replace(str6b, "^[ \r\n]*namespace [^{]+{(.*)}[ \r\n]*$", "$1", RegexOptions.Singleline);
            string str6d = Regex.Replace(str6c, "^ *\\[Serializable\\(\\)\\]"    , ""      , RegexOptions.Multiline );
            string str6e = Regex.Replace(str6d, "\r\n *\r\n"                     , "\r\n"  , RegexOptions.Singleline); // remove multiple carriage returns
            string str6f = Regex.Replace(str6e, "\r\n *\r\n"                     , "\r\n"  , RegexOptions.Singleline); // remove multiple carriage returns
            string str6g = Regex.Replace(str6f, "\r\n *\r\n"                     , "\r\n"  , RegexOptions.Singleline); // remove multiple carriage returns
            string str7a = Regex.Replace(str6g, "}([\r\n ]*)(public|private|protected|internal)( +class )", "'$1$2$3", RegexOptions.Singleline); // identify the end of a class
            string str7b = Regex.Replace(str7a, "}[\r\n ]*$"                     , "'"     , RegexOptions.Singleline);
            string str8a = Regex.Replace(str7b, "\\[class\\]"                    , ""                               );
            string str8b = Regex.Replace(str8a, "(where T[A-Za-z]* : )class"     , "$1clas_s"                       ); // where T : class 
            string str8c = Regex.Replace(str8b, "[\"][A-Za-z0-9. ]+[\"]"         , "\"\""                           ); // "Cannot find the unique instance of the class."
            string str9  = Regex.Replace(str8c, " (class)([^{]+){"               , " `$1$2", RegexOptions.Singleline);
            string str10 = Regex.Replace(str9 , "^[^`]*"                         , ""      , RegexOptions.Singleline);

            return str10;
        }

    }
}
