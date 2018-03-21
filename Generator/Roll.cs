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
using InformationLib.SoftData;        // for 
using InformationLib.Testing ;        // for RandomSource
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Drawing;                 // for 
//using System.Linq;                  // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Generator
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Roll -->
    /// <summary>
    ///      The Roll class contains a bunch of functions many of which have to do with generating random things
    /// </summary>
    /// <remarks>production ready</remarks>
    public static class Roll
    {
        private static Random _r;


        // ----------------------------------------------------------------------------------------
        //  Short methods    most of these are production ready
        // ----------------------------------------------------------------------------------------
        public  static int  D2      { get { Init(); return 1 + _r.Next(2);                        } }
        public  static int  D6      { get { Init(); return 1 + _r.Next(6);                        } }
        public  static int  D16     { get { Init(); return 1 + _r.Next(16);                       } }
        public  static int  _4D6    { get { return D6 + D6 + D6 + D6;                             } }
        private static int  D     (int y) { Init(); return 1 + _r.Next(y);                        }
        public  static int  OneD  (int n) { Init(); return _r.Next(n) + 1;                        }
        public  static int  TwoD  (int n) { return OneD(n) + Plus1D(n);                           }
        public  static int  ThreeD(int n) { return OneD(n) + Plus1D(n) + Plus1D(n);               }
        public  static int  FourD (int n) { return OneD(n) + Plus3D(n);                           }
        public  static int  FiveD (int n) { return OneD(n) + Plus4D(n);                           }
        public  static int  SixD  (int n) { return OneD(n) + Plus4D(n) + Plus1D(n);               }
        public  static int  SevenD(int n) { return OneD(n) + Plus4D(n) + Plus1D(n) + Plus1D(n);   }
        // ----------------------------------------------------------------------------------------
        private static int  Plus1D(int n) { return _r.Next(n) + 1;                                }
        private static int  Plus3D(int n) { return Plus1D(n) + Plus1D(n) + Plus1D(n);             }
        private static int  Plus4D(int n) { return Plus3D(n) + Plus1D(n);                         }
        private static void Init  ()      { _r = RandomSource.New().Random;                   }


        // ----------------------------------------------------------------------------------------
        /// <!-- Jiggle -->
        /// <summary>
        ///      Jiggle a number by 1 within the specified limits
        /// </summary>
        /// <remarks>
        ///      This may belong in the RandomSource class
        /// </remarks>
        /// <param name="r"></param>
        /// <param name="min">lower inclusive limit</param>
        /// <param name="curr">the current value of the number to be modified</param>
        /// <param name="max">higher inclusive limit</param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public static int Jiggle(Random r, int min, int curr, int max)
        {
            curr = curr + r.Next(3) - 1;
            if (curr < min) curr = min;
            if (curr > max) curr = max;
            return curr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Max -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public static double Max(double a, double b, double c, double d)
        {
            double max = Math.Max(a, b);
            max = Math.Max(max, c);
            max = Math.Max(max, d);
            return max;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Nd6 -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Nd6(int n)
        {
            _r = RandomSource.New().Random;
            int sum = 0;
            for (int i = 0; i < n; ++i)
                sum += D6;
            return sum;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Point -->
        /// <summary>
        ///      Returns a random point
        /// </summary>
        /// <param name="X_hi"></param>
        /// <param name="Y_hi"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public static Point Point(int X_hi, int Y_hi)
        {
            return new Point(_r.Next(X_hi), _r.Next(Y_hi));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StdDeviation -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        //public static double StdDeviation(Dictionary<char, int> list)
        //{
        //    double average = list.Values.Average();
        //    double sumOfDifferencesSquared = list.Select(val => ((double)val.Value - average) * ((double)val.Value - average)).Sum();
        //    double sd = Math.Sqrt(sumOfDifferencesSquared / list.Count);
        //    return sd;
        //}
        //public static double StdDeviation(List<double> list)
        //{
        //    double average = list.Average();
        //    double sumOfDifferencesSquared = list.Select(val => ((double)val - average) * ((double)val - average)).Sum();
        //    double sd = Math.Sqrt(sumOfDifferencesSquared / list.Count);
        //    return sd;
        //}

        public static int XdY(int x, int y)
        {
            _r = RandomSource.New().Random;
            int sum = 0;
            for (int i = 0; i < x; ++i)
                sum += D(y);
            return sum;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- XdY -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xdy"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static int XdY(string xdy)
        {
            string[] hi = xdy.Split("d".ToCharArray());
            return XdY(TreatAs.IntValue(hi[0], 1), TreatAs.IntValue(hi[1], 1));
        }
    }
}
