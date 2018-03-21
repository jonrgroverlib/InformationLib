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
using System;
using System.Collections.Generic;     // for List
using System.IO;                      // for FileInfo, DirectoryInfo
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- PathSet -->
    /// <summary>
    ///      The PathSet class is for holding a list of paths or finding a list of equivalent paths,
    ///      generally the prefered path is the shortest equivalent one, the main reason I am doing
    ///      this is that the XML validator sometimes chokes on valid paths, when you are looking
    ///      for something this will give you back the paths that are valid to it
    /// 
    ///      This namespace is a primitive so it should use nothing but System references
    /// </summary>
    /// <remarks>deprecated</remarks>
    public class PathSet : List<string>
    {
        private List<bool>   _correct;      // this is whether the path leads to the target
        private List<string> _hint;         // this is a list of hints to use looking for the path
        private PathSlicer   _originalPath; // this is the original path to start looking from
        //private int          _preferred;    // this is which of the paths in the list is preferred
        private string       _targetFile;   // this is the target file name
        private List<string> _segment;      // a temporary list of hints and segments


        // ----------------------------------------------------------------------------------------
        /// <!-- PathSet constructor -->
        /// <summary>
        ///      Default constructor
        /// </summary>
        public PathSet()
        {
            _Init();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- PathSet constructor -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public PathSet(string pathOrTarget)
        {
            _Init();


            // --------------------------------------------------------------------------
            //  load up the data
            // --------------------------------------------------------------------------
            _originalPath = new PathSlicer(pathOrTarget);
            _targetFile = _originalPath.Name;


            // --------------------------------------------------------------------------
            //  do some analysis
            // --------------------------------------------------------------------------
            this.Add(_originalPath.Path);
            _correct.Add(_originalPath.FileExists());
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PathSet constructor -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="target"></param>
        public PathSet(string path, string target)
        {
            _Init();


            // --------------------------------------------------------------------------
            //  load up the data
            // --------------------------------------------------------------------------
            _targetFile = target;
            _originalPath = new PathSlicer(path);
            if (_originalPath.Name != target)
                _originalPath.Append(target);


            // --------------------------------------------------------------------------
            //  do some analysis
            // --------------------------------------------------------------------------
            this.Add(_originalPath.Path);
            _correct.Add(_originalPath.FileExists());
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _AddHint -->
        /// <summary>
        ///      Adds a hint to the hint list
        /// </summary>
        /// <param name="hint"></param>
        public void _Hint(string hint)
        {
            _hint.Add(hint);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _ClearHints -->
        /// <summary>
        ///      Clears the hint list
        /// </summary>
        public void _ClearHints()
        {
            _hint.Clear();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _AddSegments -->
        /// <summary>
        ///      Adds the specified list of segments to the temporary search segment list
        /// </summary>
        /// <param name="list"></param>
        private void _AddSegments(List<string> list)
        {
            bool notNeeded;
            foreach (string item in list)
            {
                notNeeded = string.IsNullOrEmpty(item)
                    || item == ".."
                    || item == _targetFile
                    || item.Contains(":")
                    || _segment.Contains(item);
                if (!notNeeded) _segment.Add(item);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Find -->
        /// <summary>
        ///      Finds the set of paths that will lead to the target starting two places:
        ///      oringinal path and working directory
        /// </summary>
        /// <returns></returns>
        public bool _Find()
        {
            _correct = new List<bool>();
            //_preferred = -1;
            this.Clear();


            PathSlicer workingDir = new PathSlicer((new DirectoryInfo(".")).FullName);


            // --------------------------------------------------------------------------
            //  take all the segments and throw them into the temporary hints directory
            // --------------------------------------------------------------------------
            _segment = new List<string>();
            _AddSegments(workingDir.Segments);
            _AddSegments(_originalPath.Segments);
            _AddSegments(_hint);


            // --------------------------------------------------------------------------
            //  starting from the roots of the wkg dir and the orig path, go looking
            // --------------------------------------------------------------------------
            _Find_recursive(workingDir.Root, 0, 10);
            _Find_recursive(_originalPath.Root, 0, 10);
            this._Sort();
            _Find(4, 10);


            return (this.Count > 0);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Find -->
        /// <summary>
        ///      Finds the set of paths that will lead to a new target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool _Find(string target)
        {
            _targetFile = target;
            return _Find();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Find_recursive -->
        /// <summary>
        ///      start with the root,
        ///      work recursively up into the hints,
        ///      add the paths that succeed
        /// </summary>
        /// <param name="dirpath"></param>
        /// <param name="level"></param>
        private void _Find_recursive(string dirpath, int level, int maxLevel)
        {
            string filepath = dirpath + "\\" + _targetFile;
            if ((new FileInfo(filepath)).Exists)
                this.Add(filepath);
            else
            {
                foreach (string hint in _segment)
                    // ------------------------------------------------------------------
                    //  Try adding a new item to the path
                    // ------------------------------------------------------------------
                    try
                    {
                        string newpath = dirpath + "\\" + hint;
                        if ((new DirectoryInfo(newpath)).Exists && level < 10)
                            _Find_recursive(newpath, level + 1, maxLevel);
                    }
                    catch { }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Find -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxBack"></param>
        /// <param name="maxLevel"></param>
        private void _Find(int maxBack, int maxLevel)
        {
            string start = "..";
            // --------------------------------------------------------------------------
            //  Backing down from the current location
            // --------------------------------------------------------------------------
            for (int i = 0; i < maxBack; ++i)
            {
                if ((new DirectoryInfo(start)).Exists && i < maxLevel)
                    _Find_recursive(start, i, maxLevel);
                start = start + "\\" + "..";
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Init -->
        /// <summary>
        ///      Default initialization
        /// </summary>
        private void _Init()
        {
            _correct = new List<bool>();
            _hint = new List<string>();
            _originalPath = new PathSlicer();
            //_preferred = -1;
            _targetFile = "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Sort -->
        /// <summary>
        ///      Sorts the list of paths by size from shortest to longest
        /// </summary>
        public void _Sort()
        {
            int count = Count;
            for (int i = 0; i < count; ++i)
                for (int j = i+1; j < count; ++j)
                    if (this[i].Length > this[j].Length)
                    {
                        string temp = this[i];
                        this[i] = this[j];
                        this[j] = temp;
                    }
        }
    }
}
