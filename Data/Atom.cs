using System.Collections.Generic;

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    //  Isotopes, atoms and elements
    /* ----------------------------------------------------------------------------------------- */  /// <summary>Isotope: a List of object-object Dictionaries</summary>
    public class Isotope : List<Dictionary<object, object>> { }                                      /// <summary>Atom   : a List of string-object Dictionaries</summary>
    public class Atom    : List<Dictionary<string, object>> { }                                      /// <summary>Carbon : a List of string-string Dictionaries</summary>
    public class Carbon  : List<Dictionary<string, string>> { }
                                                                                                     /// <summary>Lithium: a (string-key) Dictionary of int-string Dictionaries</summary>
    public class Lithium : Dictionary<string, Dictionary<int, string>> { }


    // --------------------------------------------------------------------------------------------
    //  Particles and sub-particles
    /* ----------------------------------------------------------------------------------------- */  /// <summary>Particle: an object-object Dictionary[object,string]</summary>
    public class Particle : Dictionary<object, object> { }                                           /// <summary>Proton  : a  string-object Dictionary[string,object]</summary>
    public class Proton   : Dictionary<string, object> { }                                           /// <summary>Neutron : a  string-string Dictionary[string,string]</summary>
    public class Neutron  : Dictionary<string, string> { }                                           /// <summary>Electron: a  string-int    Dictionary[string,int   ]</summary>
    public class Electron : Dictionary<string, int   > { }                                           /// <summary>Quark   : an int   -string Dictionary[int   ,string]</summary>
    public class Quark    : Dictionary<int   , string> { }
}
