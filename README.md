# InformationLib
This is the README for InformationLib.

SUMMARY
InformationLib allows programmers to embed meaning and information in code in a form that is machine-processable.
The basic concept is something called an 'endeme'. Endemes can be described and used various ways. Use programmer terminology an
endeme is an instance of an ordered bitwise enumeration. An 'endeme set' is the unordered list of concepts to be ordered into an endeme.
In other words it is sort of a 'schema' for endemes.

ENDEMES
The way endemes become meaning is that endemes are ordered. Order gets importance (priority). Importance begets meaning. Endemes can also
be described as list of user or programmer concepts that a user or programmer has placed in order of importance. When an endeme is 
attached to a piece of data as metadata, it describes the meaning of the data within the concepts and context of the application or
function. Multiple endemes can define placement of one piece of data within a large 'information base', similar to a relational
database, or a hierarchical system such as JSON, however the placement definition is neither directly relational nor hierarchical.
It is conceptual. sometimes conceptual placement is relational and sometimes it is hierarchical but these are both subsets of the space.
An 'endeme profile' defines the position of a piece of data in concept space.

ENDEME CHARACTERISTICS
An 'endeme characteristic' is one particular concept in an endeme or endeme set. endeme sets optimally contain 22 concepts, for and
generally starting with each of the letters 'A' through 'V'. In practice, endeme sets generally don't exceed 23 concepts, and
can have as few as six. A useful practical limit for an endeme set is 24 items. If you're going beyond 24 items then you are probably
either just making a list or you are conflating concepts which should belong to two endeme sets. There are lots of good data
structures to contain lists.

TYPES OF ENDEMES
various types of endemes / endeme sets include 'resource', 'logical', 'combinatoric'.  Resource endeme sets generally contain a list
of related resources in which each of the resources has a different quantity or priority in relationship to the others. Resource
endemes tend to contains all of the endeme characterstics in the set. Full permutation mathematics generally apply. Logical endemes 
and endeme sets tend to contain building block concepts that when arranged in some order, create a new more complex concept. Endemes
of this sort generally contain 1-8 of the building block concepts of the set. Combinatoric sendems / endeme sets are essentiall
bitwise enumerations we are all familiar with. There are more types.

NAMESPACES
The libarary also contains various supporting namespaces for data access, string manipulation, and various forms of soft data access.
Soft data access is different from standard data and data access in that is designed to have default values and rarely throw exceptions.
It more of an 'information oriented' way to work with variables. It is beter for some situations and worse for others.
There are some moribund namespaces in the library. The active and generally up to date namespaces are:
Data (but avoid Xml), DataModels, Endemes (but avoid Tree), HardData, SoftData (but avoid TimeDate), Strings (but avoid Diff)
and Testing (but avoid TestJson). I have confidence that the core each of these namespaces works well. SoftData avoids excpetions
and HardData throws exceptions. The more problematic namespaces are Generator, InfoAccess, Micro, Vertical, Winform.

AS A .NET PROJECT
The library is designed to work with .NET as a 'leaf' project libaray. In other words it is brought into a .NET solution just like any
other project, and it has no dependencies on any other projects external to the 'system.' namespace. There is a small amount of linq
in the project which you will have to remove or rewrite if you want to use this project with a very old version of .NET or C#. The
library should work pretty well back to version 2.0.

STATUS
The libaray is a research library and will change as my work with meaning and information develops more. There is some unit testing
but not enough. 


