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
using System.Collections.Generic;     // for List<>
using System.Data.SqlTypes;           // for 
using System.IO;                      // for File, StreamWriter, TextReader
using System.Text.RegularExpressions; // for Regex
using System.Web;                     // for HttpPostedFile

namespace InformationLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- FilesIO -->
    /// <summary>
    ///      The FilesIO class contains helpers for file and directory IO
    /// </summary>
    /// <remarks>production ready</remarks>
    public static class FilesIO
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- DeleteFile -->
        /// <summary>
        ///      Deletes a file if it exists unless it matches a particular Regex pattern
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="candy">use __.unless_name_matches</param>
        /// <param name="doNotOverWritePattern">do not delete the file if it matches this pattern</param>
        /// <remarks>beta code - used once in production</remarks>
        public static void DeleteFile(string filePath, string candy, string doNotOverWritePattern)
        {
            FileInfo deleteMe = new FileInfo(filePath);
            if (deleteMe.Exists)
            {
                if (string.IsNullOrEmpty(doNotOverWritePattern.Trim()) ||
                    !Regex.IsMatch(deleteMe.Name, doNotOverWritePattern, RegexOptions.IgnoreCase))
                {
                    deleteMe.Delete();
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FileBody -->
        /// <summary>
        ///      Returns the body of the file name with the spaces removed
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static string FileBody(string path)
        {
            string fileName = Regex.Replace(path, @"^.*[\\/]([^\\/]*)$", "$1");
            string body   = Regex.Replace(fileName, @"^(.*)(\.[^.]{1,4})$", "$1");
            return body;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FileSuffix -->
        /// <summary>
        ///      Returns the extension of a file name including the dot
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static string FileSuffix(string fileName)
        {
            string suffix = Regex.Replace(fileName, @"^(.*)(\.[^.]{1,4})$", "$2");
            if (!Regex.IsMatch(suffix, @"^(\.[^.]{1,4})$")) suffix = "";
            return suffix;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetByteArray -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static byte[] GetByteArray(HttpPostedFile postedFile)
        {
            byte[] byteArray = new byte[postedFile.ContentLength];
            HttpPostedFile Image = postedFile;
            Image.InputStream.Read(byteArray, 0, (int)postedFile.ContentLength);
            return byteArray;
        }

	    // -----------------------------------------------------------------------------------
	    /// <!-- GetFileList -->
        /// <summary>
        ///      Gets a file list from a directory list of files matching a particular regex pattern
        /// </summary>
        /// <param name="dirList">the list of full directory paths</param>
        /// <param name="pattern">file name pattern</param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static List<string> GetFileList(List<string> dirList, string pattern)
        {
		    List<string> fileList = new List<string>();

			// --------------------------------------------------------------------------
			//  Read list of vb files among directories
			// --------------------------------------------------------------------------
            for (int i = 0; i < dirList.Count; ++i)
            {
                string path = dirList[i];
				DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    // ------------------------------------------------------------------
                    //  Check the filenames in the specified directory
                    // ------------------------------------------------------------------
                    FileInfo[] file = dir.GetFiles();
                    for (int j = 0; j < file.Length; ++j)
                        if (Regex.IsMatch(file[j].Name, pattern))
                            fileList.Add(file[j].FullName);
                }
			}

            return fileList;
        }

	    // ----------------------------------------------------------------------------------------
	    /// <!-- GetFileList -->
	    /// <summary>
	    ///      Gets a list of files of a particular pattern under a particular directory, recursively
	    /// </summary>
	    /// <param name="mainDirPath"></param>
	    /// <param name="fileNamePattern">for example "\.vb$"</param>
	    /// <returns></returns>
	    /// <remarks></remarks>
        /// <remarks>beta code - used once in production</remarks>
        public static List<string> GetFileList(string mainDirPath, string fileNamePattern)
        {
		    List<string> file = new List<string>();

            // --------------------------------------------------------------------------
            //  Read list of directories, recursively, then check each one for the files
            // --------------------------------------------------------------------------
		    if (!string.IsNullOrEmpty(mainDirPath))
            {
			    DirectoryInfo mainDir = new DirectoryInfo(mainDirPath);
			    List<string>  dir = FilesIO.LoadSubDirectoryList(mainDir);
                file = GetFileList(dir, fileNamePattern);
		    }

		    return file;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetListFromFile -->
        /// <summary>
        ///      Gets a list of the lines in a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static List<string> GetListFromFile(string path)
        {
            string str = GetStringFromFile(path, "");
            str = str.ToLower();
            char[] delimiter = { '\n', '\r' };
            List<string> name = new List<string>(str.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetStringFromFile -->
        /// <summary>
        ///      Gets a non-humongous string from a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <remarks>production ready</remarks>
        public static string GetStringFromFile(string filePath, string defaultContent)
        {
            string content;
            TextReader tr;


            try
            {
                tr = new StreamReader(filePath);
                content = tr.ReadToEnd(); tr.Close();
            }
            catch { content = defaultContent; }

            return content;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InsureFile -->
        /// <summary>
        ///      Copies the file from the directory to the folder if necessary
        /// </summary>
        /// <param name="fromPath">filePath to get the file from if it does not exist</param>
        /// <param name="toFile"></param>
        public static FileInfo InsureFile(string fromPath, FileInfo toFile)
        {
            if (toFile.Exists) return toFile; else File.Copy(fromPath, toFile.FullName, false); return NoFile;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InsureNoFile -->
        /// <summary>
        ///      Insures that the file does not exist in the specified directory
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <remarks>If the file exists Copy and delete it // TODO: insure that the folder in 'pathto' exists first:</remarks>
        public static FileInfo InsureNoFile(string filePath, FileInfo cutFile)
        {
            if (!cutFile.Exists) return NoFile;
            else { File.Copy(cutFile.FullName, filePath, false); cutFile.Delete(); return cutFile; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ListSubDirectories -->
        /// <summary>
        ///      Returns the list of directories at and under root directory
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static List<DirectoryInfo> ListSubDirectories(DirectoryInfo root)
        {
            List<DirectoryInfo> dirList = new List<DirectoryInfo>();
            dirList.Add(root);

            for (int i = 0; i < dirList.Count; ++i)
            {
                DirectoryInfo[] dir = dirList[i].GetDirectories();
                for (int j = 0; j < dir.Length; ++j)
                {
                    dirList.Add(dir[j]);
                }
            }

            return dirList;
        }

	    // -----------------------------------------------------------------------------------
	    /// <!-- LoadStringFromFile -->
	    /// <summary>
	    ///      Loads a non-humungous string from a file
	    /// </summary>
	    /// <param name="filePath"></param>
	    /// <param name="defaultContent"></param>
        /// <remarks>
        ///      beta code - used once in production
        ///      wrap this into GetStringFromFile
        /// </remarks>
	    public static string LoadStringFromFile(string filePath, string defaultContent)
	    {
		    string content;
		    TextReader reader = null;


		    try
            {
			    reader = (new StreamReader(filePath));
			    content = reader.ReadToEnd();
			    reader.Close();
		    }
            catch { content = defaultContent; }

		    return content;
	    }

	    // ----------------------------------------------------------------------------------------
	    /// <!-- LoadSubDirectoryList -->
	    /// <summary>
	    ///      Recursively loads a list of directories under a particular root directory
	    /// </summary>
	    /// <param name="root"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
        /// <remarks>beta code - used once in production</remarks>
        public static List<string> LoadSubDirectoryList(DirectoryInfo root)
        {
		    List<string> dir = new List<string>();
            if (root.Exists)
            {
                dir = LoadSubDirectoryList(root, 0);
               	dir.Add(root.FullName);
            }
		    return dir;
        }

	    // ----------------------------------------------------------------------------------------
	    /// <!-- LoadSubDirectoryList -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainDir"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
	    private static List<string> LoadSubDirectoryList(DirectoryInfo mainDir, int level)
	    {
		    List<string> dirList = new List<string>();
            if (level > 10)
                Pause();

            if (mainDir.Exists)
            {
                DirectoryInfo[] di = mainDir.GetDirectories();
                if (di != null && di.Length > 0)
                {
                    for (int i = 0; i < di.Length; ++i)
                    {
                        DirectoryInfo dir = di[i];
                        dirList.Add(dir.FullName);
                        List<string> subdirList = LoadSubDirectoryList(dir, level+1);
                        AddTo(dirList, subdirList);
                    }
                }
            }

		    return dirList;
	    }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddTo -->
        /// <summary>
        ///      Adds the contents of one list of strings to another
        /// </summary>
        /// <param name="toThis">list to add to</param>
        /// <param name="addFromThis">list to add from</param>
        /// <remarks>copied here from __ to avoid dependency problems</remarks>
        /// <remarks>beta code - used once in production</remarks>
        private static void AddTo(List<string> toThis, List<string> addFromThis)
        {
            if (addFromThis.Count > 0)
            {
                for (int i = 0; i < addFromThis.Count; ++i)
                {
                    string listItem = addFromThis[i];
                    toThis.Add(listItem);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NewestFileDateByName -->
        /// <summary>
        ///      Identifies the most recent date based on the names of the files in the list
        /// </summary>
        /// <param name="fileList">file names have YYYYMMDDHHMMDD in them somewhere</param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static DateTime NewestFileDateByName(FileInfo[] fileList)
        {
            DateTime newest = (DateTime)SqlDateTime.MinValue;

            foreach (FileInfo fi in fileList)
            {
                string fileName = fi.Name;
                string strDate = Regex.Replace(fileName, "[^0-9]", "");
                strDate = Regex.Replace(strDate, "^(....)(..)(..)(..)(..)(.*)$", "$1/$2/$3 $4:$5:$6");
                DateTime? fileNameTime = NullableDateTimeValue(strDate, null);
                if (fileNameTime != null)
                {
                    if (fileNameTime > newest)
                        newest = (DateTime)fileNameTime;
                }
            }

            return newest;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NiceName -->
        /// <summary>
        ///      Returns a usable storage file name
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static string NiceName(string path)
        {
            int sectionLength = 8;


            // --------------------------------------------------------------------------
            //  Name components
            // --------------------------------------------------------------------------
            string suffix = FilesIO.FileSuffix(path);
            string body   = FilesIO.ThinFileBody(path);
            Random r      = RandomSource.New().Random;
            string num    = r.Next(10000).ToString().PadLeft(4, '0');


            // --------------------------------------------------------------------------
            //  Construct name
            // --------------------------------------------------------------------------
            string nice = "";
            if (body.Length > sectionLength*2)
            {
                string body1 = Regex.Replace(body, "^(.{"+sectionLength+"}).*$", "$1");
                string body2 = Regex.Replace(body, "^.*(.{"+sectionLength+"})$", "$1");

                nice = body1 + body2 + num + suffix;
            }
            else
                nice = body + num + suffix;


            return nice;
        }

        private static FileInfo NoFile { get { return new FileInfo("nonexistentfile"); } }

        // ----------------------------------------------------------------------------------------
        /// <!-- NullableDateTimeValue -->
        /// <summary>
        ///      Copies from TreatAs to eliminate namespace dependencies
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static DateTime? NullableDateTimeValue(object obj, DateTime? defaultValue)
        {
            DateTime? time = null;

            if (obj != null)
            {
                string str = obj.ToString();
                DateTime datetime;
                if (DateTime.TryParse(str, out datetime))
                {
                    time = datetime;
                }
            }

            return time;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OldestFileDateByName -->
        /// <summary>
        ///      Identifies the most recent date based on the names of the files in the list
        /// </summary>
        /// <param name="fileList">file names have YYYYMMDDHHMMDD in them somewhere</param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static DateTime OldestFileDateByName(FileInfo[] fileList)
        {
            DateTime oldest = DateTime.UtcNow;

            foreach (FileInfo fi in fileList)
            {
                string fileName = fi.Name;
                string strDate = Regex.Replace(fileName, "[^0-9]", "");
                strDate = Regex.Replace(strDate, "^(....)(..)(..)(..)(..)(.*)$", "$1/$2/$3 $4:$5:$6");
                DateTime? fileNameTime = NullableDateTimeValue(strDate, null);
                if (fileNameTime != null)
                {
                    if (fileNameTime < oldest)
                        oldest = (DateTime)fileNameTime;
                }
            }

            return oldest;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ThinFileBody -->
        /// <summary>
        ///      Returns the body of the file name with the spaces removed
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static string ThinFileBody(string path)
        {
            string body = FileBody(path);
            body = Regex.Replace(body, " ", "");
            return body;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteByteArrayToFile -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static string WriteByteArrayToFile(string filePath, byte[] byteArray)
        {
            string err = "";
            try
            {
                // -----------------------------------------------------------------------
                //  1. Open file for reading
                //  2. Write a block of bytes to this stream using data from a byte array.
                //  3. Close file stream
                //  4. Indicate success
                // -----------------------------------------------------------------------
                FileStream stream = new System.IO.FileStream(filePath, FileMode.Create, FileAccess.Write);
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Close();
                err = "";
            }
            catch (Exception ex)
            {
                err = ex.ToString();
            }
            return err;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteStringToFile -->
        /// <summary>
        ///      Writes a non-humongous string to a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <remarks>production ready</remarks>
        public static void WriteStringToFile(string filePath, string content, bool append)
        {
            if (content == null)
            {
                throw new ArgumentNullException("WriteStringToFile error - content is null");
            }
            else
            {
                string[] lines = new string[1];
                StreamWriter fileWriter = null;
                lines[0] = content;


                if (filePath == null || filePath.Length == 0) return;
                if (lines == null || lines.Length == 0) return;


                try
                {
                    if (Regex.IsMatch(filePath, "/"))
                        Pause();
                    else
                    {
                        if (append)
                            fileWriter = File.AppendText(filePath);
                        else
                            fileWriter = File.CreateText(filePath);
                        foreach (string line in lines) fileWriter.Write(line);
                    }
                }
                catch { throw new IOException("Unable to write string to file " + filePath); }
                finally { if (fileWriter != null)  fileWriter.Close(); }
            }
        }

        private static void Pause()
        {
        }
    }
}