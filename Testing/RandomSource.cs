//-------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation, either version 3
// of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//-------------------------------------------------------------------------------------------------
using System;                         // for Random
using System.Collections.Generic;     // for List<>
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Testing  // add to this namespace very sparingly and use only System references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeRandomSource -->
    /// <summary>
    ///      The EndemeRandomSource class manages random numbers using the singleton pattern
    /// </summary>
    /// <remarks>
    ///      The EndemeDice class is a direct copy of the 'RandomSource' class from the
    ///      'Core' namespace, to prevent dependency between the 'Endemes' and 'Core' namespaces
    ///      because 'Endemes' are primitives and should only reference 'System' namespaces
    ///      
    ///      production ready
    /// </remarks>
    public class RandomSource
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeDice member, constructor and instantiator -->
        private static RandomSource _rs;
        // ------------------------------ //  this is private because it instantiates the
        //  DON'T CHANGE THIS TO PUBLIC!  //  resource managed by the singleton pattern,
        // ------------------------------ //  instead use:   RandomSource.New();
        private RandomSource() { ReSeed(); }
        // ------------------------------ //  this is private because it instantiates the
        //  DON'T CHANGE THIS TO PUBLIC!  //  resource managed by the singleton pattern,
        // ------------------------------ //  instead use:   RandomSource.New();
        /// <summary>
        ///      Instantiates the random source singleton
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static RandomSource New()
        {
            if (_rs == null)
                _rs = new RandomSource();
            return _rs;
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties     alpha code  (Random is production ready)
        /* ------------------------------------------------------------------------------------- */                                         /// <summary>Accesses the random source</summary><remarks>Random _r is the resource that needs to be protected in this singleton</remarks>
        public Random Random                { get { return _r;                                                       } } private Random _r; /// <summary>Returns a random string from a list of strings</summary>
        public string Draw  (List<string> option) { return option[Random.Next(option.Count)];                        }                      /// <summary>Returns a random string from a list of strings</summary>
        public object Draw  (List<object> option) { return option[Random.Next(option.Count)];                        }                      /// <summary>Reruns your list of random numbers, producing the 'same' numbers in the 'same' order</summary>
        public void   RePlay(                   ) { _r = new Random(_seed);                                          }                      /// <summary>Starts over your list of random numbers again starting from a seed you provide</summary>
        public void   ReSeed(int newSeed        ) { _seed = newSeed; _r = new Random(_seed);                         }                      /// <summary>Starts over your list of random numbers again seeding from the current time stamp</summary>
        public void   ReSeed(                   ) { _seed = new Random().Next(int.MaxValue); _r = new Random(_seed); }                      /// <summary>Returns the actual value of the actual seed</summary>
        public int    Seed                  { get { return _seed;                                                    } } private int _seed;


        // ----------------------------------------------------------------------------------------
        /// <!-- Bell -->
        /// <summary>
        ///      Returns a number from a bell shape frequency ranged number between 2 numbers
        /// </summary>
        /// <param name="inclusiveMinNum"></param>
        /// <param name="exclusiveMaxNum"></param>
        /// <returns></returns>
        /// <remarks>beta code</remarks>
        public int Bell(int inclusiveMinNum, int exclusiveMaxNum)
        {
            Random r = New().Random;
            int n1 = 0;
            int n2 = 0;
            int n3 = 0;
            int inclusiveMaxNum = exclusiveMaxNum - 1;
            int range = exclusiveMaxNum - inclusiveMinNum;


            int factor = 2;
            n1 = 2 + range/3;
            n2 = 2 + range/3;
            n3 = 2 + range/3;
            while (n1 + n2 + n3 > range + factor)
            {
                if (n1 + n2 + n3 > range + factor) n3--;
                if (n1 + n2 + n3 > range + factor) n2--;
                if (n1 + n2 + n3 > range + factor) n1--;
            }


            int num = r.Next(n1) + r.Next(n2) + r.Next(n3) + inclusiveMinNum;

            return num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NextUInt -->
        /// <summary>
        ///      Returns a uint in cases where you want a uint randomly over its full range
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code</remarks>
        private uint NextUInt()
        {
            byte[] buff = new byte[4];
            Random.NextBytes(buff);
            uint addme = BitConverter.ToUInt32(buff, 0);
            return addme;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NextUInt -->
        /// <summary>
        ///      Returns a random unsigned integer up to one below maxValue
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <remarks>beta code</remarks>
        public uint NextUInt(uint maxValue)
        {
            if (maxValue <= int.MaxValue)
                return (uint)Random.Next((int)maxValue);
            else
            {
                uint num = (uint)Random.Next((int)(maxValue/2));
                num <<= 1;
                if (maxValue % 2 == 1)
                    num += (uint)Random.Next(2);
                return num;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NextULong -->
        /// <summary>
        ///      Returns a random unsigned long number
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <remarks>beta code</remarks>
        public ulong NextULong(ulong maxValue)
        {
            if (maxValue <= uint.MaxValue)
                return (ulong)(NextUInt((uint)maxValue));
            else
            {
                maxValue >>= 32;
                ulong num = ((ulong)NextUInt((uint)maxValue));
                num <<= 32;
                num += NextUInt();
                return num;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Skew -->
        /// <summary>
        ///      Comes up with a number from a skew shape frequency ranged number between 2 numbers
        /// </summary>
        /// <param name="inclusiveMinNum"></param>
        /// <param name="exclusiveMaxNum"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public int Skew(int inclusiveMinNum, int exclusiveMaxNum)
        {
            int offset = -inclusiveMinNum;
            Random r = New().Random;


            double newmax = exclusiveMaxNum + offset;
            double cbroot = Math.Pow(newmax, 1.0/3.0);
            int a = 3 + (int)cbroot;
            double sqroot = Math.Sqrt(newmax / a);
            int b = 3 + (int)sqroot;
            int c = 3 + (int)(newmax / a / b);


            int num = r.Next(a) * r.Next(b) * r.Next(c);
            for (int i = 0; (num < 0 || num >= newmax) && i < 100; ++i)
                num = r.Next(a) * r.Next(b) * r.Next(c);

            return num - offset;
        }
    }
}
