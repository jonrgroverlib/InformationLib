//-------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify it under the terms of
// the GNU Lesser General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with
// InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//-------------------------------------------------------------------------------------------------
using System.Text;

namespace InformationLib.Testing  // add to this namespace very sparingly and use only System references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- LpgplLicense -->
    /// <summary>
    ///      In the spirit of information programming, the GNU Lesser General Public License is
    ///      provided as a machine readable string
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class LpgplLicense
    {
        public static string LpgplText { get
        {
            StringBuilder license = new StringBuilder();

            license.Append(         "                   GNU LESSER GENERAL PUBLIC LICENSE                    ");
            license.Append("\r\n" + "                       Version 3, 29 June 2007                          ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + " Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>    ");
            license.Append("\r\n" + " Everyone is permitted to copy and distribute verbatim copies           ");
            license.Append("\r\n" + " of this license document, but changing it is not allowed.              ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  This version of the GNU Lesser General Public License incorporates    ");
            license.Append("\r\n" + "the terms and conditions of version 3 of the GNU General Public         ");
            license.Append("\r\n" + "License, supplemented by the additional permissions listed below.       ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  0. Additional Definitions.                                            ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  As used herein, \"this License\" refers to version 3 of the GNU Lesser");
            license.Append("\r\n" + "General Public License, and the \"GNU GPL\" refers to version 3 of the  ");
            license.Append("\r\n" + "GNU General Public License.                                             ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  \"The Library\" refers to a covered work governed by this License,    ");
            license.Append("\r\n" + "other than an Application or a Combined Work as defined below.          ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  An \"Application\" is any work that makes use of an interface provided");
            license.Append("\r\n" + "by the Library, but which is not otherwise based on the Library.        ");
            license.Append("\r\n" + "Defining a subclass of a class defined by the Library is deemed a mode  ");
            license.Append("\r\n" + "of using an interface provided by the Library.                          ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  A \"Combined Work\" is a work produced by combining or linking an     ");
            license.Append("\r\n" + "Application with the Library.  The particular version of the Library    ");
            license.Append("\r\n" + "with which the Combined Work was made is also called the \"Linked       ");
            license.Append("\r\n" + "Version\".                                                              ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  The \"Minimal Corresponding Source\" for a Combined Work means the    ");
            license.Append("\r\n" + "Corresponding Source for the Combined Work, excluding any source code   ");
            license.Append("\r\n" + "for portions of the Combined Work that, considered in isolation, are    ");
            license.Append("\r\n" + "based on the Application, and not on the Linked Version.                ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  The \"Corresponding Application Code\" for a Combined Work means the  ");
            license.Append("\r\n" + "object code and/or source code for the Application, including any data  ");
            license.Append("\r\n" + "and utility programs needed for reproducing the Combined Work from the  ");
            license.Append("\r\n" + "Application, but excluding the System Libraries of the Combined Work.   ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  1. Exception to Section 3 of the GNU GPL.                             ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  You may convey a covered work under sections 3 and 4 of this License  ");
            license.Append("\r\n" + "without being bound by section 3 of the GNU GPL.                        ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  2. Conveying Modified Versions.                                       ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  If you modify a copy of the Library, and, in your modifications, a    ");
            license.Append("\r\n" + "facility refers to a function or data to be supplied by an Application  ");
            license.Append("\r\n" + "that uses the facility (other than as an argument passed when the       ");
            license.Append("\r\n" + "facility is invoked), then you may convey a copy of the modified        ");
            license.Append("\r\n" + "version:                                                                ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   a) under this License, provided that you make a good faith effort to ");
            license.Append("\r\n" + "   ensure that, in the event an Application does not supply the         ");
            license.Append("\r\n" + "   function or data, the facility still operates, and performs          ");
            license.Append("\r\n" + "   whatever part of its purpose remains meaningful, or                  ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   b) under the GNU GPL, with none of the additional permissions of     ");
            license.Append("\r\n" + "   this License applicable to that copy.                                ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  3. Object Code Incorporating Material from Library Header Files.      ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  The object code form of an Application may incorporate material from  ");
            license.Append("\r\n" + "a header file that is part of the Library.  You may convey such object  ");
            license.Append("\r\n" + "code under terms of your choice, provided that, if the incorporated     ");
            license.Append("\r\n" + "material is not limited to numerical parameters, data structure         ");
            license.Append("\r\n" + "layouts and accessors, or small macros, inline functions and templates  ");
            license.Append("\r\n" + "(ten or fewer lines in length), you do both of the following:           ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   a) Give prominent notice with each copy of the object code that the  ");
            license.Append("\r\n" + "   Library is used in it and that the Library and its use are           ");
            license.Append("\r\n" + "   covered by this License.                                             ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   b) Accompany the object code with a copy of the GNU GPL and this     ");
            license.Append("\r\n" + "   license document.                                                    ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  4. Combined Works.                                                    ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  You may convey a Combined Work under terms of your choice that,       ");
            license.Append("\r\n" + "taken together, effectively do not restrict modification of the         ");
            license.Append("\r\n" + "portions of the Library contained in the Combined Work and reverse      ");
            license.Append("\r\n" + "engineering for debugging such modifications, if you also do each of    ");
            license.Append("\r\n" + "the following:                                                          ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   a) Give prominent notice with each copy of the Combined Work that    ");
            license.Append("\r\n" + "   the Library is used in it and that the Library and its use are       ");
            license.Append("\r\n" + "   covered by this License.                                             ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   b) Accompany the Combined Work with a copy of the GNU GPL and this   ");
            license.Append("\r\n" + "   license document.                                                    ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   c) For a Combined Work that displays copyright notices during        ");
            license.Append("\r\n" + "   execution, include the copyright notice for the Library among        ");
            license.Append("\r\n" + "   these notices, as well as a reference directing the user to the      ");
            license.Append("\r\n" + "   copies of the GNU GPL and this license document.                     ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   d) Do one of the following:                                          ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "       0) Convey the Minimal Corresponding Source under the terms of    ");
            license.Append("\r\n" + "       this License, and the Corresponding Application Code in a form   ");
            license.Append("\r\n" + "       suitable for, and under terms that permit, the user to           ");
            license.Append("\r\n" + "       recombine or relink the Application with a modified version of   ");
            license.Append("\r\n" + "       the Linked Version to produce a modified Combined Work, in the   ");
            license.Append("\r\n" + "       manner specified by section 6 of the GNU GPL for conveying       ");
            license.Append("\r\n" + "       Corresponding Source.                                            ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "       1) Use a suitable shared library mechanism for linking with the  ");
            license.Append("\r\n" + "       Library.  A suitable mechanism is one that (a) uses at run time  ");
            license.Append("\r\n" + "       a copy of the Library already present on the user's computer     ");
            license.Append("\r\n" + "       system, and (b) will operate properly with a modified version    ");
            license.Append("\r\n" + "       of the Library that is interface-compatible with the Linked      ");
            license.Append("\r\n" + "       Version.                                                         ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   e) Provide Installation Information, but only if you would otherwise ");
            license.Append("\r\n" + "   be required to provide such information under section 6 of the       ");
            license.Append("\r\n" + "   GNU GPL, and only to the extent that such information is             ");
            license.Append("\r\n" + "   necessary to install and execute a modified version of the           ");
            license.Append("\r\n" + "   Combined Work produced by recombining or relinking the               ");
            license.Append("\r\n" + "   Application with a modified version of the Linked Version. (If       ");
            license.Append("\r\n" + "   you use option 4d0, the Installation Information must accompany      ");
            license.Append("\r\n" + "   the Minimal Corresponding Source and Corresponding Application       ");
            license.Append("\r\n" + "   Code. If you use option 4d1, you must provide the Installation       ");
            license.Append("\r\n" + "   Information in the manner specified by section 6 of the GNU GPL      ");
            license.Append("\r\n" + "   for conveying Corresponding Source.)                                 ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  5. Combined Libraries.                                                ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  You may place library facilities that are a work based on the         ");
            license.Append("\r\n" + "Library side by side in a single library together with other library    ");
            license.Append("\r\n" + "facilities that are not Applications and are not covered by this        ");
            license.Append("\r\n" + "License, and convey such a combined library under terms of your         ");
            license.Append("\r\n" + "choice, if you do both of the following:                                ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   a) Accompany the combined library with a copy of the same work based ");
            license.Append("\r\n" + "   on the Library, uncombined with any other library facilities,        ");
            license.Append("\r\n" + "   conveyed under the terms of this License.                            ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "   b) Give prominent notice with the combined library that part of it   ");
            license.Append("\r\n" + "   is a work based on the Library, and explaining where to find the     ");
            license.Append("\r\n" + "   accompanying uncombined form of the same work.                       ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  6. Revised Versions of the GNU Lesser General Public License.         ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  The Free Software Foundation may publish revised and/or new versions  ");
            license.Append("\r\n" + "of the GNU Lesser General Public License from time to time. Such new    ");
            license.Append("\r\n" + "versions will be similar in spirit to the present version, but may      ");
            license.Append("\r\n" + "differ in detail to address new problems or concerns.                   ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  Each version is given a distinguishing version number. If the         ");
            license.Append("\r\n" + "Library as you received it specifies that a certain numbered version    ");
            license.Append("\r\n" + "of the GNU Lesser General Public License \"or any later version\"       ");
            license.Append("\r\n" + "applies to it, you have the option of following the terms and           ");
            license.Append("\r\n" + "conditions either of that published version or of any later version     ");
            license.Append("\r\n" + "published by the Free Software Foundation. If the Library as you        ");
            license.Append("\r\n" + "received it does not specify a version number of the GNU Lesser         ");
            license.Append("\r\n" + "General Public License, you may choose any version of the GNU Lesser    ");
            license.Append("\r\n" + "General Public License ever published by the Free Software Foundation.  ");
            license.Append("\r\n" + "                                                                        ");
            license.Append("\r\n" + "  If the Library as you received it specifies that a proxy can decide   ");
            license.Append("\r\n" + "whether future versions of the GNU Lesser General Public License shall  ");
            license.Append("\r\n" + "apply, that proxy's public statement of acceptance of any version is    ");
            license.Append("\r\n" + "permanent authorization for you to choose that version for the          ");
            license.Append("\r\n" + "Library.                                                                ");

            return license.ToString();
        } }
    }
}
