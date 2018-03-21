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
using InformationLib.Data        ;    // for 
using InformationLib.Endemes     ;    // for EndemeItem
using InformationLib.Endemes.Tree;    // for 
using InformationLib.SoftData    ;    // for TreatAs.StrValue
using InformationLib.Testing     ;    // for Here
using System;                         // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.InfoAccess
{
	// ---------------------------------------------------------------------------------------------
	/// <!-- InfoAccessTests -->
	/// <summary>
	///      The InfoAccessTests class tests the classes in the AccessTests Library
	/// </summary>
	/// <remarks>
    /// production ready - unit test code
	/// </remarks>
	///[TestFixture]
	public class InfoAccessTests
	{
		// -----------------------------------------------------------------------------------------
		//  Members
		// -----------------------------------------------------------------------------------------
		private        Result _result;
		private static string _conn;


		// -----------------------------------------------------------------------------------------
		/// <!-- AllTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            return UnconnectedTests()
              //+ ConnectedTests(Connection string)
                ;
        }

		// -----------------------------------------------------------------------------------------
		/// <!-- ConnectedTests -->
		/// <summary>
		///      Tests the InfoAccess folder in ways that need a connection
		/// </summary>
		/// <returns></returns>
		public string ConnectedTests(string conn)
		{

			string result = "";
			_conn = conn;


			// ---------------------------------------------------------------------------
			//  InfoAccess folder tests needing a connection
			// ---------------------------------------------------------------------------
			BuildSchema_Test                  ();
			BuildDatabase_Test                ();
			DeleteKnowledgeDatabase_Test      ();
            EndemeCollection_Constructor_test ();
          //EndemeCollection_StdDeviation_test();
            EndemeLeaf_Constructor_test       ();
			InfoPath_test                     ();
            L4_test                           ();
                                                                                                                                                                                             

			result += "\r\n" + "Connected info access tests succeeded";
			return result;

		}

		// -----------------------------------------------------------------------------------------
		/// <!-- UnconnectedTests -->
		/// <summary>
		///      Tests the InfoAccess folder in ways that do not need a connection
		/// </summary>
		/// <returns></returns>
		public string UnconnectedTests()
		{
			string result = "";


			// ---------------------------------------------------------------------------
			//  InfoAccess folder tests not needing a connection
			// ---------------------------------------------------------------------------


            EndemeValueAddStrValue_test();


            result += "\r\n" + "Nonconnected info access tests succeeded";
			return result;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeValue_test -->
        /// <summary>
        /// 
        /// </summary>
        private void EndemeValueAddStrValue_test()
        {
            Assert.ThingsAbout("EndemeList", "Add");


            EndemeReference enRef = new EndemeReference();

            enRef.Add(new EndemeSet("Identity"));                                                                                        
            enRef["Identity"   ].Add('A', "Alias"            , ""                                                                        );
            enRef["Identity"   ].Add('B', "Birthday"         , ""                                                                        );
            enRef["Identity"   ].Add('C', "Card number"      , ""                                                                        );
            enRef["Identity"   ].Add('D', "DL number"        , "driver's license"                                                        );
            enRef["Identity"   ].Add('E', "Email"            , ""                                                                        );
            enRef["Identity"   ].Add('F', "First name"       , ""                                                                        );
            enRef["Identity"   ].Add('G', "Gender"           , ""                                                                        );
            enRef["Identity"   ].Add('H', "Honorific"        , "prefix (Mr, Dr...)"                                                      );
            enRef["Identity"   ].Add('I', "Id"               , ""                                                                        );
            enRef["Identity"   ].Add('J', "Judicial number"  , ""                                                                        );
            enRef["Identity"   ].Add('K', "King"             , "father's X"                                                              );
            enRef["Identity"   ].Add('L', "Last name"        , ""                                                                        );
            enRef["Identity"   ].Add('M', "Middle name"      , ""                                                                        );
            enRef["Identity"   ].Add('N', "Nick name"        , ""                                                                        );
            enRef["Identity"   ].Add('O', "Oceanic number"   , ""                                                                        );
            enRef["Identity"   ].Add('P', "Postfix"          , "suffix"                                                                  );
            enRef["Identity"   ].Add('Q', "Queen"            , "mother's X"                                                              );
            enRef["Identity"   ].Add('R', "Race"             , ""                                                                        );
            enRef["Identity"   ].Add('S', "Social number"    , ""                                                                        );
            enRef["Identity"   ].Add('T', "Tag, userid"      , ""                                                                        );
            enRef["Identity"   ].Add('U', "Unmarried name"   , "maiden name"                                                             );
            enRef["Identity"   ].Add('V', "Visa number"      , "passport number"                                                         );

            enRef.Add(new EndemeSet("Demographic"));                                                                                     
            enRef["Demographic"].Add('A', "Age"              , "Stage of life"                                                           );
            enRef["Demographic"].Add('B', "Body Shape"       , "Robustness/Weight/Physique/Girth"                                        );
            enRef["Demographic"].Add('C', "Country"          , "Continent/Region"                                                        );
            enRef["Demographic"].Add('D', "Damage"           , "Natural Damage"                                                          );
            enRef["Demographic"].Add('E', "Experience"       , ""                                                                        );
            enRef["Demographic"].Add('F', "Flying"           , "Air Speed"                                                               );
            enRef["Demographic"].Add('G', "Gender"           , "Sex"                                                                     );
            enRef["Demographic"].Add('H', "Hit Points"       , ""                                                                        );
            enRef["Demographic"].Add('I', "Insanity"         , "madness/Sanity"                                                          );
            enRef["Demographic"].Add('J', "Jesus"            , "Gift/Grace/Faith/Religion"                                               );
            enRef["Demographic"].Add('K', "Kingdom"          , "Ethnicity/Nation/State/People"                                           );
            enRef["Demographic"].Add('L', "Level"            , ""                                                                        );
            enRef["Demographic"].Add('M', "Movement"         , "Speed/Ground/Land"                                                       );
            enRef["Demographic"].Add('N', "Nature"           , "Personality/Temperment"                                                  );
            enRef["Demographic"].Add('O', "Organization"     , "School/Business/Guild"                                                   );
            enRef["Demographic"].Add('P', "Position"         , "Social Status/class"                                                     );
            enRef["Demographic"].Add('Q', "Quester"          , "Player"                                                                  );
            enRef["Demographic"].Add('R', "Race"             , "Species"                                                                 );
            enRef["Demographic"].Add('S', "Swimming"         , "Sailing"                                                                 );
            enRef["Demographic"].Add('T', "Tallness"         , "Height/Size/Length"                                                      );
            enRef["Demographic"].Add('U', "Underwater move"  , ""                                                                        );
            enRef["Demographic"].Add('V', ""                 , ""                                                                        );

            EndemeList field = new EndemeList("test", enRef, 0.73);
            field.Add("HeroName"  , "Identity"   , "A" , "");
            field.Add("Species"   , "Demographic", "R" , "");
            field.Add("Gender"    , "Demographic", "G" , "");

            field.Add("First Name", "Identity"   , "F" , "");
            field.Add("Last Name" , "Identity"   , "L" , "");
            field.Add("Full Name" , "Identity"   , "LF", "");

            //field.SetFieldExactly("Identity:F" , "Jon"       ); // field.SetField(field.EnRef[EndemeField.Part(0,"Identity:F" )], EndemeField.Part(1,"Identity:F" ), "Jon"       ); // field["Identity:F"]  = "Jon"       ;
            //field.SetFieldExactly("Identity:L" , "Grover"    ); // field.SetField(field.EnRef[EndemeField.Part(0,"Identity:L" )], EndemeField.Part(1,"Identity:L" ), "Grover"    ); // field["Identity:L"]  = "Grover"    ;
            //field.SetFieldExactly("Identity:LF", "Jon Grover"); // field.SetField(field.EnRef[EndemeField.Part(0,"Identity:LF")], EndemeField.Part(1,"Identity:LF"), "Jon Grover"); // field["Identity:LF"] = "Jon Grover";

            EndemeValue elFirst = field.GetField("Identity:F" ).Item; // field["Identity:F" ];
            EndemeValue elLast  = field.GetField("Identity:L" ).Item; // field["Identity:L" ];
            EndemeValue elFull  = field.GetField("Identity:LF").Item; // field["Identity:LF"];
            string first = TreatAs.StrValue(field.GetField("Identity:F" ), "");
            string last  = TreatAs.StrValue(field.GetField("Identity:L" ), "");
            string full  = TreatAs.StrValue(field.GetField("Identity:LF"), "");
            //string first = field.StrValue("Identity:F" , "");
            //string last  = field.StrValue("Identity:L" , "");
            //string full  = field.StrValue("Identity:LF", "");

            Assert.That(first, Is.equal_to, "Jon"       );
            Assert.That(last , Is.equal_to, "Grover"    );
            Assert.That(full , Is.equal_to, "Jon Grover");

            _result += Assert.Conclusion;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- EndemeCollection_StdDeviation_test -->
        ///// <summary>
        ///// 
        ///// </summary>
        ////[Test]
        //public void EndemeCollection_StdDeviation_test()
        //{
        //    //  Test the general std deviation algorithm
        //    List<double> numList = new List<double>(8);
        //    numList.Add(2.0);
        //    numList.Add(4.0);
        //    numList.Add(4.0);
        //    numList.Add(4.0);
        //    numList.Add(5.0);
        //    numList.Add(5.0);
        //    numList.Add(7.0);
        //    numList.Add(9.0);
        //    double num = EndemeList.StdDeviation(numList);
        //    ok &= Assert.That(num, __.equals, 2.0);


        //    //  Test an empty list
        //    List<Endeme> endeme = new List<Endeme>(4);
        //    EndemeList list = new EndemeList(12, WetlandAnimals);
        //    Endeme stdNone = list.StdDeviation();
        //    ok &= Assert.That(stdNone.Count, __.equals, 0);


        //    //  Test a single item list
        //    endeme.Add(new Endeme(WetlandAnimals, "TUKHQDNVILGRSOCBFAJPME"));  endeme[0].GenerateRaw_geometric(0.8);  list.Add(endeme[0], null);
        //    Endeme stdOne = list.StdDeviation();
        //    ok &= Assert.That(stdOne.Quant.Raw['C'], __.equals, 0.0);


        //    endeme.Add(new Endeme(WetlandAnimals, "TMVLJNFGSUKBPOEDHCIQRA"));  endeme[1].GenerateRaw_geometric(0.8);  list.Add(endeme[1], null);
        //    endeme.Add(new Endeme(WetlandAnimals, "NTSVPCGUOKARBDLIHFJQME"));  endeme[2].GenerateRaw_geometric(0.8);  list.Add(endeme[2], null);
        //    endeme.Add(new Endeme(WetlandAnimals, "NGCPVBUIEDHFKALTOSQJRM"));  endeme[3].GenerateRaw_geometric(0.8);  list.Add(endeme[3], null);
        //    Endeme std = list.StdDeviation();
        //    ok &= Assert.That(17.008 < std.Quant.Raw['J'] && std.Quant.Raw['J'] < 17.009);
        //    Endeme avg = list.Average();
        //    ok &= Assert.That(11.501 < avg.Quant.Raw['J'] && avg.Quant.Raw['J'] < 11.502);


        //    //Endeme other = list.StdDeviation(); //list.StdDeviations();
        //    //Endeme avgs = list.Averages();
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- WetlandAnimals -->
        /// <summary>
        ///      Test endeme set
        /// </summary>
        internal static EndemeSet WetlandAnimals { get
        {
            EndemeSet set = new EndemeSet("animal");
            set.Add('A', "alligator", "");
            set.Add('B', "beaver",    "");
            set.Add('C', "crocodile", "");
            set.Add('D', "duck",      "");
            set.Add('E', "egret",     "");
            set.Add('F', "frog",      "");
            set.Add('G', "gecko",     "");
            set.Add('H', "herron",    "");
            set.Add('I', "insect",    "");
            set.Add('J', "jackal",    "");
            set.Add('K', "koala",     "");
            set.Add('L', "lizard",    "");
            set.Add('M', "muskrat",   "");
            set.Add('N', "newt",      "");
            set.Add('O', "otter",     "");
            set.Add('P', "puma",      "");
            set.Add('Q', "quahog",    "");
            set.Add('R', "reptile",   "");
            set.Add('S', "snake",     "");
            set.Add('T', "turtle",    "");
            set.Add('U', "ungulate",  "");
            set.Add('V', "vole",      "");
            return set;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeCollection_Constr_test -->
        /// <summary>
        /// 
        /// </summary>
        //[Test]
        public void EndemeCollection_Constructor_test()
        {
            //_result = "UNTESTED";
            //EndemeSet animals = WetlandAnimals;
            //EndemeList list = new EndemeList(100, animals, true);
            //Random r = EndemeRandomSource.New().Random;
            //for (int i = 0; i < 100; i++)
            //{
            //    list.Add(Guid.Empty, i.ToString(), animals.RandomEndeme(r));
            //}
            //List<Guid> key = list["47"];
            //Endeme     e   = list[key[0]];
            //string     str = e.ToString();
            //Assert.That(str.Length, __.equals, 22);

            //_result = "OK";
        }

		// ----------------------------------------------------------------------------------------
		/// <!-- BuildDatabase_Test -->
		/// <summary>
		///      Tests various knowledge database construction methods
		/// </summary>
		///[Test]

		public void BuildDatabase_Test()
		{
		    RichSqlCommand.BuildSchema(EndemeAccess.EnSchema, "generictestaccount", _conn, Throws.Actions, "O");

			EndemeTableFactory_old.BuildEndemeTable              (_conn);
			EndemeTableFactory_old.BuildEndemeCharacteristicTable(_conn);
			EndemeTableFactory_old.BuildEndemeIndexKeyTable      (_conn);
			EndemeTableFactory_old.BuildEndemeIndexTable         (_conn);
			EndemeTableFactory_old.BuildEndemeSetTable           (_conn);

			EndemeTableFactory_old.BuildEndemeCharValueTable_old     (_conn);
			EndemeTableFactory_old.BuildEndemeListItemTable_old      (_conn);
			EndemeTableFactory_old.BuildEndemeMemberTable_old        (_conn);
			EndemeTableFactory_old.BuildEndemeOrganizationTable_old  (_conn);
			EndemeTableFactory_old.BuildEndemeUserTable_old          (_conn);
			EndemeTableFactory_old.BuildEndemeMeaningTable_old       (_conn);

			// check to see that the tables exist
		}


		// ----------------------------------------------------------------------------------------
		/// <!-- DeleteDatabase_Test -->
		/// <summary>
		///      Tests whether the database tables delete properly
		/// </summary>
		///[Test]

		public void DeleteKnowledgeDatabase_Test()
		{
			EndemeTableFactory_old.DropEndemeTables(                               _conn, Throws.Actions, "O");
		    RichSqlCommand    .DropSchema      (EndemeAccess.EnSchema, _conn, Throws.Actions, "O");

			// check to see that there are no remaining Info.* objects in the database
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- BuildSchema_Test -->
		/// <summary>
		///      Tests the RichDataTable.Add method
		/// </summary>
		///[Test]
		public void BuildSchema_Test()
		{
			EndemeTableFactory_old.DropEndemeTables(                                                     _conn, Throws.Actions, "I");
			RichSqlCommand    .BuildSchema     (EndemeAccess.EnSchema, "generictestaccount", _conn, Throws.Actions, "O");
			RichSqlCommand    .DropSchema      (EndemeAccess.EnSchema                      , _conn, Throws.Actions, "O");
		}

		// ----------------------------------------------------------------------------------------
		/// <!-- Address -->
		/// <summary>
		///      A)ddress, B)ox
		/// </summary>
		internal static EndemeSet Address {
			get {
				if ((_address == null)) {
					_address = new EndemeSet("Contact");
					_address.Add('A', "Address"      , "street address");
					_address.Add('B', "Box"          , "p o box, post box, (extended zip)");
					_address.Add('C', "County"       , "Jarldom, parish, township, borough department, burgh shire, Local governmental area, district, commune, administrative division");
					_address.Add('D', "Department"   , "location within address, indoors, department");
					_address.Add('E', "EW longitude" , "longitude");
					_address.Add('F', "Flat num"     , "Room/apt num, apartment number, suite number, office number, room apartment suite floor quarters");
					_address.Add('G', "Government"   , "country, nation, people, land");
					_address.Add('H', "House num"    , "House number, street number");
					_address.Add('I', "Intersection" , "Intersection, Mile marker, cross street");
					_address.Add('J', "Journey"      , "rural route, state road, highway");
					_address.Add('K', "Keep"         , "Location at address, outdoors, building");
					_address.Add('L', "Level"        , "Floor");
					_address.Add('M', "Mailing addr" , "address to mail to");
					_address.Add('N', "NS latitude"  , "parallel");
					_address.Add('O', "Official"     , "Official, primary residence, permanent");
					_address.Add('P', "Postal code"  , "Postal code, zip code");
					_address.Add('Q', "Quadrant"     , "Quadrant of a city - NE, SW etc.");
					_address.Add('R', "Real address" , "Real address, actual physical Address");
					_address.Add('S', "State"        , "province");
					_address.Add('T', "Town"         , "city, town, village");
					_address.Add('U', "UN region"    , "region");
					_address.Add('V', "Vacation home", "vacation home, alternate, summer, winter, temporary");
				}
				return _contact;
			}
		}
		private static EndemeSet _address = null;


		// ----------------------------------------------------------------------------------------
		/// <!-- Contact -->
		/// <summary>
		///      A)lternate, B)log
		/// </summary>
		internal static EndemeSet Contact {
			get {
				if ((_contact == null)) {
					_contact = new EndemeSet("Contact");
					_contact.Add('A', "Alternate", "secondary");
					_contact.Add('B', "Blog", "web site, url");
					_contact.Add('C', "Cell", "mobile, smart");
					_contact.Add('D', "Day", "daytime");
					_contact.Add('E', "Email", "electronic mail");
					_contact.Add('F', "Favored", "preferred");
					_contact.Add('G', "General", "primary");
					_contact.Add('H', "Home", "domicile");
					_contact.Add('I', "IM account", "instant messaging account");
					_contact.Add('J', "Job", "work, office");
					_contact.Add('K', "Kept", "rarely used, n-ary, priority, importance, frequency, preference, favor, attention, use");
					_contact.Add('L', "Land line", "secure");
					_contact.Add('M', "Machine", "ip address, network");
					_contact.Add('N', "Night", "evening");
					_contact.Add('O', "Own", "personal, private, individual, my own");
					_contact.Add('P', "Phone", "telephone, voice");
					_contact.Add('Q', "Quick", "quick response, pager");
					_contact.Add('R', "Real time", "chat, im protocol");
					_contact.Add('S', "Social media", "Social media site");
					_contact.Add('T', "Twitter", "Twitter account");
					_contact.Add('U', "User account", "User/social media account");
					_contact.Add('V', "Voice mail", "Voice mail, Messsages recorder, answering service");
				}
				return _contact;
			}
		}
		private static EndemeSet _contact = null;


		// ----------------------------------------------------------------------------------------
		/// <!-- Identity -->
		/// <summary>
		///      A)lias, B)irthday
		/// </summary>
		/// <remarks>
		///      TODO: Send and get this from the knowledge database, rather than hardcoding it here
		///      The main benefit of having it here at all is for the summary above
		/// </remarks>
		internal static EndemeSet Identity {
			get {
				if ((_identity == null)) {
					_identity = new EndemeSet("Identity");
					_identity.Add('A', "Alias", "pseudonym, pen name");
					_identity.Add('B', "Birthday", "date or day of birth");
					_identity.Add('C', "Card number", "unspecified card number");
					_identity.Add('D', "DL number", "driver's license number");
					// remove this because we need driver's license state too
					_identity.Add('E', "Email", "email address");
					_identity.Add('F', "First name", "given name");
					_identity.Add('G', "Gender", "sex");
					_identity.Add('H', "Honorific", "name prefix");
					_identity.Add('I', "Id", "unspecified id number or code");
					_identity.Add('J', "Justice", "justice system or incarceration number");
					_identity.Add('K', "King's", "father's or paternal");
					_identity.Add('L', "Last name", "family name");
					_identity.Add('M', "Middle name", "middle name or initial, may be given or a previous family name");
					_identity.Add('N', "Nick name", "");
					_identity.Add('O', "Origin number", "birth certificate: certificate number");
					_identity.Add('P', "Postfix", "professional or generational suffix");
					_identity.Add('Q', "Queen's", "mother's of maternal");
					_identity.Add('R', "Race", "for fascist countries that keep track of such things");
					_identity.Add('S', "Social", "Social system or National identification number");
					_identity.Add('T', "Tag", "Username, userid, handle, login");
					_identity.Add('U', "Unmarried", "maiden name");
					_identity.Add('V', "Visa number", "passport number");
				}
				return _identity;
			}
		}
		private static EndemeSet _identity = null;

        public void L4_test()
        {
            Assert.ThingsAbout("hi", "world");


            // --------------------------------------------------------------------------
            // L4: (constructs at this level should be able to subsume the other levels)
            // --------------------------------------------------------------------------
            GraphMain graph = new GraphMain();
            GraphNode node1 = new GraphNode("Jon");
            GraphNode node2 = new GraphNode("Grover");
            GraphEdge edge  = new GraphEdge(node1, node2);

            graph.Add(node1);
            graph.Add(node2);
            graph.Add(edge);
            string strEdge = edge.ToString();
            Assert.That(strEdge, Is.equal_to, "Jon -> Grover");


            // --------------------------------------------------------------------------
            // L3 concrete class: (there is no language, system or tool yet)
            // --------------------------------------------------------------------------
            MemberListConcrete dog = new MemberListConcrete("AnimalNamespace", "Dog");
            dog.Add("AnimalNamespace", "AppendageList", "Paws");
            dog.Add("AnimalNamespace", "Sense"        , "Nose");
            string strMemberList = dog.ToString();
            Assert.That(strMemberList , Is.equal_to, "Dog : Paws, Nose");

            // --------------------------------------------------------------------------
            // L3 abstract class: (there is no language, system or tool yet)
            // --------------------------------------------------------------------------
            MemberList dog2 = new MemberList("AnimalNamespace", "Dog");
            dog2.Add("AnimalNamespace", "AppendageList", "Paws");
            dog2.Add("AnimalNamespace", "Sense"        , "Nose");
            strMemberList = dog2.ToString();
            Assert.That(strMemberList , Is.equal_to, "Dog : AnimalNamespace.AppendageList.Paws -> AnimalNamespace.Dog., AnimalNamespace.Sense.Nose -> AnimalNamespace.Dog.");



            // --------------------------------------------------------------------------
            // L2 concrete class: (oo languages)
            // --------------------------------------------------------------------------
            InheritClassConcrete inheritRelation = new InheritClassConcrete("Dog", "Animal");
            Assert.That(inheritRelation.ToString(), Is.equal_to, "Dog : Animal");
            inheritRelation = new InheritClassConcrete("AnimalNamespace", "Dog", "AnimalNamespace", "Animal");
            Assert.That(inheritRelation.ToString(), Is.equal_to, "AnimalNamespace.Dog : AnimalNamespace.Animal");

            // --------------------------------------------------------------------------
            // L2 abstract class: (oo languages) - basing L2 on L4
            // --------------------------------------------------------------------------
            InheritClass inheritRelation2 = new InheritClass("Dog", "Animal");
            Assert.That(inheritRelation2.ToString(), Is.equal_to, "Dog : Animal");
            inheritRelation2 = new InheritClass("AnimalNamespace", "Dog", "AnimalNamespace", "Animal");
            Assert.That(inheritRelation2.ToString(), Is.equal_to, "AnimalNamespace.Dog : AnimalNamespace.Animal");



            // --------------------------------------------------------------------------
            // L1 concrete class: (databases)
            // --------------------------------------------------------------------------
            ForeignKeyConcrete join = new ForeignKeyConcrete("EggDetail", "ParentEggID", "EggMaster", "EggID");
            string strJoin = join.ToString();
            Assert.That(strJoin, Is.equal_to, "EggDetail.ParentEggID >- EggMaster.EggID");

            // --------------------------------------------------------------------------
            // L1 abstract class: (databases) basing L1 on L4
            // --------------------------------------------------------------------------
            ForeignKey join2 = new ForeignKey("EggDetail", "ParentEggID", "EggMaster", "EggID");
            string strJoin2 = join2.ToString();
            Assert.That(strJoin2, Is.equal_to, "EggDetail.ParentEggID >- EggMaster.EggID");


            _result += Assert.Conclusion;
        }


		// -----------------------------------------------------------------------------------------
		/// <!-- InfoPath_test -->
		/// <summary>
		///      Tests information paths, and endeme nodes, node connectors, lists, leaves and components
		/// </summary>
		///[Test]

		public void InfoPath_test()
		{
            Assert.ThingsAbout("hi", "world");


			string path = null;
			path = "/(2)Identity:FN";

			path = "/(0)Identity:FN=Jon";
			path = "/(0)Identity:FN(2)=Jagged";
			path = "/(1)Identity:FN(2)";
			// --> Jagged
			path = "/(2)Identity:FN";
			// --> Jon, Jagged

			path = "/(0)Identity:F=Jonathan";
			path = "/(0)Identity:M=Robb";
			path = "/(0)Identity:L=Grover";
			path = "/(0)Identity:P=III";
			path = "/(0)Identity:QUL=Van Groos";
			path = "/(0)Identity:G=Male";

			path = "/(0.02)Resident/Identity:QUL(0.02)=Van Groos";

			path = "/(0)Contact:CA(2)=317-405-7864";
			path = "/(0)Contact:EF=jonrgrover@gmail.com";

			path = "/(0)Address:MC=Indianapolis";

			EndemeUnit leaf1 = new EndemeUnit(new Endeme(InfoAccessTests.Identity, "FN"));
			leaf1.Text = "Jon";
			EndemeUnit leaf2 = new EndemeUnit(new Endeme(InfoAccessTests.Contact, "EF"));
			leaf1.Text = "jonrgrover@gmail.com";
			EndemeUnit leaf3 = new EndemeUnit(new Endeme(InfoAccessTests.Address, "MC"));
			leaf1.Text = "Indianapolis";

            Is.Trash(path);

            _result += Assert.Conclusion;
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeLeaf_Constructor_test -->
        /// <summary>
        /// 
        /// </summary>
        private void EndemeLeaf_Constructor_test()
        {
            Assert.ThingsAbout("hi", "world");


            Endeme endeme = InfoAccessTests.WetlandAnimals.RandomEndeme();
            EndemeUnit item = new EndemeUnit(endeme);
            item.Label = "hi";
            item.Number = 7;
            item.Text = "fosdfjej";

            Assert.That(item.Endeme.ToString(), Is.equal_to, endeme.ToString());

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Knowledge_test -->
        /// <summary>
        /// 
        /// </summary>
        public static void Knowledge_test()
        {
            DateTime loc      = DateTime.Now;
            DateTime utc      = loc.ToUniversalTime();
            string   strUtc   = utc.ToString();
            string   strLoc   = loc.ToString();
            TimeSpan span     = loc - utc;
            bool     dltime   = loc.IsDaylightSavingTime();
            decimal  timezone = span.Hours + span.Minutes/30.0M;
            object   tz       = TimeZone.CurrentTimeZone;


            EndemeNode list1 = new EndemeNode();
            EndemeUnit item1 = null;
            item1 = (EndemeUnit)list1.AddItem(EndemeSet.WetlandAnimals.RandomEndeme(), "A"       );
            item1 = (EndemeUnit)list1.AddItem(EndemeSet.WetlandAnimals.RandomEndeme(), "B"       );
            item1 = (EndemeUnit)list1.AddItem(EndemeSet.WetlandAnimals.RandomEndeme(), "Carrot"  );
            item1 = (EndemeUnit)list1.AddItem(EndemeSet.WetlandAnimals.RandomEndeme(), "C"       );
            item1 = (EndemeUnit)list1.AddItem(EndemeSet.WetlandAnimals.RandomEndeme(), "C"       );
            item1 = (EndemeUnit)list1.AddItem(EndemeSet.WetlandAnimals.RandomEndeme(), "Ducky", 1);


            EndemeNode list2 = (EndemeNode)list1["C"];  ReflectorTests.Equality("EndemeCollection", "[str]", "C", list2.Count, 2);
            EndemeNode list3 = (EndemeNode)list1[0];    ReflectorTests.Equality("EndemeCollection", "[0]", 0, list3.Count, 5);

            Endeme e = EndemeSet.WetlandAnimals.RandomEndeme();
            EndemeNode list4 = (EndemeNode)list1[e];         ReflectorTests.Equality("EndemeCollection", "[e]"        , e, list4.Count, 6);
            EndemeNode combo = (EndemeNode)list1["C"][0][e]; ReflectorTests.Equality("EndemeCollection", "[C][0][set]", 0, combo.Count, 2);
            EndemeUnit       item2 = (EndemeUnit)list1.GetItem(0); ReflectorTests.Equality("EndemeCollection", "GetItem"    , 0, item2.Count, 1);

            EndemeNode list5 = new EndemeNode();
            EndemeNode list6 = (EndemeNode)list5.AddList(list1, EndemeSet.WetlandAnimals.RandomEndeme(), "Eagle"  );
            EndemeNode list7 = (EndemeNode)list5.AddList(list1, EndemeSet.WetlandAnimals.RandomEndeme(), "Owl"    );
            EndemeNode list8 = (EndemeNode)list5.AddList(list1, EndemeSet.WetlandAnimals.RandomEndeme(), "Pelican");
            ReflectorTests.Equality("EndemeCollection", "AddList"    , 0, list5.Count, 3);


            EndemeNode knowledge = new EndemeNode();

            knowledge.Enter("/Identity:1-F(1)"  , "hi world F");
            knowledge.Enter("/Identity:.09-G(1)", "hi world G");
            knowledge.Enter("/Identity:H(1)"    , "hi world H");
            knowledge.Enter("/Identity:I"       , "hi world I");
            knowledge.Enter("/Identity:1-J"     , "hi world J");
            knowledge.Enter("/Identity:.09-K"   , "hi world K");

            EndemeHierarchy each1 = knowledge.QueryOne("/Identity:1-G(1)");
            ReflectorTests.Equality("EndemeCollection", "QueryOne", "/Identity:1-G(1)", each1.Value, "hi world G");


            knowledge.Enter("/story part:.9-GTV(1)/identity:1-F(1)", "Gnoephoe");
            EndemeHierarchy each2 = knowledge.QueryOne("/story part:.9-GTV(1)");
            Type type = each2.GetType();
            EndemeUnit sub = (EndemeUnit)((EndemeNode)each2).GetItem(0);
            ReflectorTests.Equality("EndemeCollection", "GetItem", "0", sub.Value, "Gnoephoe");
            EndemeHierarchy each3 = knowledge.QueryOne("/story part:.9-GTV(1)/identity:1-F(1)");
            ReflectorTests.Equality("EndemeCollection", "QueryOne", "/story part:.9-GTV(1)/identity:1-F(1)", each3.Value, "Gnoephoe");

                                                                                          // A  Associate of X       loyalty       servant / henchman / involved / partner to X
            knowledge.Enter("/story part:.9-GTV/identity:1-L(1)"   , "Brew"            ); // B  Band of X            membership    group of X
                                                                                          // C  Competetor of X      rivalry       rival
            knowledge.Enter("/story part:1-VRPAGT/identity:1-F(1)" , "Pashalia"        ); // D  Destroyer of X       destruction   killer of X
            knowledge.Enter("/story part:1-VRPAGT/identity:1-L(1)" , "au Taq"          ); // E  Enemy of X           evil          villain
                                                                                          // F  Fighter              combat        combatant / fighting
            knowledge.Enter("/story part:1-RPVAGT/identity:1-H(1)" , "Queen"           ); // G  Guardian of X        prevention    preventer of X
            knowledge.Enter("/story part:1-RPVAGT/identity:1-F(1)" , "Chelaune"        ); // H  Hero of X            goodness      good
            knowledge.Enter("/story part:1-RPVAGT/identity:1-L(1)" , "au Taq"          ); // I  Investigator of X    investigation detective / inquiry / query / suspect / possible
                                                                                          // J  Junk / object        owner         thing / owner / McGuffin / item
            knowledge.Enter("/story part:1-PRVEGTH/identity:1-H(1)", "King"            ); // K  Knows / witness of X knowledge     knowledge belief / witness of X / saw
            knowledge.Enter("/story part:1-PRVEGTH/identity:1-F(1)", "Mekton"          ); // L  Local                non traveling	
            knowledge.Enter("/story part:1-PRVEGTH/identity:1-L(1)", "au Taq"          ); // M  Monster              monstrous     creature
                                                                                          // N  Next X               subsequent    future x
            knowledge.Enter("/story part:1-LOFHAP/identity:1-F(1)" , "Zelda"           ); // O  Official of X        leadership    leader of X
            knowledge.Enter("/story part:1-LOFHAP/identity:1-P(1)" , "The Amazon Queen"); // P  Patron of X          patronage     client / customer
                                                                                          // Q  Quester of X         traveling     company / party / traveler
            knowledge.Enter("/story part:1-FAH/identity:1-F(1)"    , "Lida"            ); // R  Related to X         family	
                                                                                          // S  Spy / scout          secret	
            knowledge.Enter("/story part:1-ARGT/identity:1-F(1)"   , "Ilso"            ); // T  Thief / kidnapper    taker         stealer / capturer of X
            knowledge.Enter("/story part:1-ARGT/identity:1-L(1)"   , "Brew"            ); // U  Unknowing            unknown       unbelieving / nonbeliever / ignorant / amnesia / sceptic
                                                                                          // V  Victim               missing       lost / damsel
            knowledge.Enter("/story part:1-OAPSGT/identity:1-H(1)" , "Page"            ); // 
            knowledge.Enter("/story part:1-OAPSGT/identity:1-F(1)" , "Jeffrey"         ); // 
                                                                                          // 
            knowledge.Enter("/story part:1-AGTPRLH/identity:1-H(1)", "Captain"         ); // 
            knowledge.Enter("/story part:1-AGTPRLH/identity:1-F(1)", "Bravard"         ); // 
            knowledge.Enter("/story part:1-AGTPRLH/identity:1-A(1)", "Barvard"         ); // 

            knowledge.Enter("/player/Identity:1-F(1)"   , "Jon"     );
            knowledge.Enter("/player/Identity:1-L(1)"   , "Grover"  );


            //txtMain.Text = (new History()).Generate('y').Display();
            //txtMain.Text = (new MapTests()).AllTests() + (new EndemeTests()).AllTests();
        }
    }
}