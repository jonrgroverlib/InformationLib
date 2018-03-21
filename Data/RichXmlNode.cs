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
using InformationLib.SoftData;        // for TreatAs
using System;                         // for Math
using System.Collections.Generic;     // for List<>, Dictionary<,>
using System.Xml;                     // for 
using System.Xml.Schema;              // for 
using System.Text.RegularExpressions; // for Regex

namespace InformationLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RichXmlNode -->
    /// <summary>
    ///      Wraps an XmlNode and adds a few navigation and analysis methods,
    ///      it's actually sort of light because the XML modification features have been disabled.
    /// </summary>
    /// <remarks>
    ///      You can't subclass an XmlNode for many very good reasons, so this class only does the
    ///      simple stuff that has to do regarding looking at an already existing node,
    /// 
    ///      Nothing is changed or set,
    /// 
    ///      If you want a RichXmlNode class then subclass one of XmlNode's already derived types,
    ///      like make RichXmlElement out of XmlElement, which is what I did, and it didn't work
    ///      very well
    ///      
    ///      mostly beta code - used twice in production, needs testing and fixing
    /// </remarks>
    public class RichXmlNode
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- RealNode -->
        private XmlNode _node;
        /// <summary>
        ///      Yes, there is a real node hidden in here, consider this class a wrapper
        /// </summary>
        public XmlNode Node
        {
            get { return _node; }
        }
	

        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public RichXmlNode(XmlNode node)
        {
            _node = node;
        }


        // ----------------------------------------------------------------------------------------
        //  Get-only versions of all the XmlNode properties
        // ----------------------------------------------------------------------------------------
        public XmlAttributeCollection Attributes { get { if (_node == null) return null; else return _node.Attributes; } }


        public bool           HasChildNodes { get { if (_node == null) return false; else return _node.HasChildNodes; } }
        public bool           IsReadOnly    { get { return _node.IsReadOnly;    } }
        public XmlNodeType    NodeType      { get { return _node.NodeType;      } }
        public XmlDocument    OwnerDocument { get { if (_node == null) return null; else return _node.OwnerDocument; } }
        public IXmlSchemaInfo SchemaInfo    { get { if (_node == null) return null; else return _node.SchemaInfo;    } }


        public RichXmlNode FirstChild      { get { if (_node == null) return null; else return new RichXmlNode(_node.FirstChild);      } }
        public RichXmlNode LastChild       { get { if (_node == null) return null; else return new RichXmlNode(_node.LastChild);       } }
        public RichXmlNode NextSibling     { get { if (_node == null) return null; else return new RichXmlNode(_node.NextSibling);     } }
        public RichXmlNode ParentNode      { get { if (_node == null) return null; else return new RichXmlNode(_node.ParentNode);      } }
        public RichXmlNode PreviousSibling { get { if (_node == null) return null; else return new RichXmlNode(_node.PreviousSibling); } }


        public string BaseURI      { get { if (_node == null) return ""; else return _node.BaseURI;      } }
        public string InnerText    { get { if (_node == null) return ""; else return _node.InnerText;    } }
        public string InnerXml     { get { if (_node == null) return ""; else return _node.InnerXml;     } set { _node.InnerXml = value; } }
        public string LocalName    { get { if (_node == null) return ""; else return _node.LocalName;    } }
        public string Name         { get { if (_node == null) return ""; else return _node.Name;         } }
        public string NamespaceURI { get { if (_node == null) return ""; else return _node.NamespaceURI; } }
        public string OuterXml     { get { if (_node == null) return ""; else return _node.OuterXml;     } }
        public string Prefix       { get { if (_node == null) return ""; else return _node.Prefix;       } }
        public string Value        { get { if (_node == null) return ""; else return _node.Value;        } }


        // ----------------------------------------------------------------------------------------
        //  Methods not implemented
        // ----------------------------------------------------------------------------------------
        //_node.AppendChild();
        //_node.InsertAfter();
        //_node.InsertBefore();
        //_node.Normalize();
        //_node.PrependChild();
        public void RemoveAll() { _node.RemoveAll(); }
        public RichXmlNode RemoveChild(RichXmlNode node) { return (new RichXmlNode(_node.RemoveChild(node.Node))); }
        public RichXmlNode ReplaceChild(RichXmlNode node, RichXmlNode oldChild) { return new RichXmlNode(_node.ReplaceChild(node.Node, oldChild.Node)); }
        //_node.WriteTo();
        //_node.WriteContentTo();


        // ----------------------------------------------------------------------------------------
        /// <!-- RemoveSelf -->
        /// <summary>
        ///      Removes this node and all of its sub nodes
        /// </summary>
        /// <param name="node"></param>
        public RichXmlNode RemoveSelf()
        {
            RichXmlNode parent = this.ParentNode;
            return parent.RemoveChild(this);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- AttributeList -->
        /// <summary>
        ///      Returns a list of attributes for a particular node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public XmlAttributeCollection AttributeList()
        {
            XmlAttributeCollection attributes;


            if (NodeType == XmlNodeType.XmlDeclaration)
            {
                attributes = Attributes;
                string attr = Regex.Replace(OuterXml, "<[?]xml", "");
                attr = Regex.Replace(attr, "[?]>$", "");
                char[] on_spaces = {' '};
                string[] attrs = attr.Split(on_spaces);
                foreach (string str in attrs)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                    }
                }
            }
            else
            {
                attributes = Attributes;
            }

            return attributes;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- AttributeString -->
        /// <summary>
        ///      Returns the attribute string of a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string AttributeString()
        {
            string attr = "";
            string name;


            if (NodeType == XmlNodeType.XmlDeclaration)
            {
                attr = Regex.Replace(OuterXml, "<[?]xml", "");
                attr = Regex.Replace(attr, "[?]>$", "");
            }
            else
            {
                XmlAttributeCollection attributes = Attributes;
                string tagName = Tag;


                if (attributes != null)
                    foreach (XmlAttribute attribute in attributes)
                    {
                        name = attribute.Name;
                        if (tagName == "xs:schema" || name != "xmlns:xs")
                            attr += " " + name + "=\"" + attribute.Value + "\"";
                    }
            }


            return attr;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Attribute -->
        /// <summary>
        ///      Gets the value of a specified attribute if there is one
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string Attribute(string attributeName)
        {
            XmlAttributeCollection attributes = Attributes;
            string attr = "";


            // -------------------------------------------------------------------------90
            //  Returns the value of the specified attribute if it exists in the list
            // -------------------------------------------------------------------------90
            if (attributes != null)
                foreach (XmlAttribute attribute in attributes)
                    if (attribute.Name == attributeName)
                        attr = attribute.Value;
            return attr;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- AttributeValue -->
        /// <summary>
        ///      Gets the value of a specified attribute if there is one
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string AttributeValue(string attributeName)
        {
            XmlAttributeCollection attributes = this.Attributes;
            string attr = "";


            // -------------------------------------------------------------------------90
            //  Returns the value of the specified attribute if it exists in the list
            // -------------------------------------------------------------------------90
            if (attributes != null)
                foreach (XmlAttribute attribute in attributes)
                    if (attribute.Name == attributeName)
                        attr = attribute.Value;


            return attr;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- LightChildNodes -->
        /// <summary>
        ///      Maybe someday I'll make a LightXmlNodesList class
        /// </summary>
        public List<RichXmlNode> RichChildNodes
        {
            get
            {
                List<RichXmlNode> list = new List<RichXmlNode>();
                if (_node != null)
                    foreach (XmlNode child in _node.ChildNodes)
                        list.Add(new RichXmlNode(child));
                return list;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- XmlChildNodes -->
        /// <summary>
        /// 
        /// </summary>
        public XmlNodeList XmlNodeChildNodes
        {
            get { if (_node == null) return null; else return _node.ChildNodes; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CollectRestrictions -->
        /// <summary>
        ///      Records the restrictions for the simple type
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> CollectRestrictions()
        {
            Dictionary<string, List<string>> restrictions = new Dictionary<string, List<string>>();


            if (HasChildNodes)
                foreach (RichXmlNode childNode in RichChildNodes)
                {
                    string name = childNode.Name;
                    if (!restrictions.ContainsKey(name))
                        restrictions[name] = new List<string>();
                    string value = childNode.Attribute("value");
                    restrictions[name].Add(Entitize(value));
                }


            return restrictions;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Entitize -->
        /// <summary>
        ///      Converts angle brackets into the xml friendly entities &gt; and &lt;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Entitize(string value)
        {
            value = Regex.Replace(value, ">", "&gt;");
            value = Regex.Replace(value, "<", "&lt;");
            return value;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CountChildrenLike -->
        /// <summary>
        ///      Returns a count of the number of children with a particular tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public int CountChildrenLike(string tag)
        {
            int count = 0;
            foreach (RichXmlNode node in RichChildNodes)
                if (node.Tag == tag)
                    count++;
            return count;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ElementList -->
        /// <summary>
        ///      Recursively Traverses an XML document listing its elements
        /// </summary>
        /// <returns>list of elements</returns>
        public List<RichXmlNode> ElementList_recursive()
        {
            List<RichXmlNode> list = new List<RichXmlNode>();


            list.Add(this);


            if (HasGrandchildren())
            {
                foreach (RichXmlNode childNode in RichChildNodes)
                {
                    List<RichXmlNode> childList = childNode.ElementList_recursive();
                    foreach(RichXmlNode node in childList)
                        list.Add(node);
                }
            }


            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FindChild -->
        /// <summary>
        ///      Gets the child node with a particular tag
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="nodeTag"></param>
        /// <returns></returns>
        public XmlNode FindChild(string nodeTag)
        {
            string simpleTag = Regex.Replace(nodeTag, "^[^:]+:", "");
            XmlNode foundNode = null;
            foreach (RichXmlNode childNode in RichChildNodes)
                if (childNode.Tag == nodeTag || childNode.Tag == simpleTag)
                    { foundNode = childNode.Node; break; }
            return foundNode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FindChildren -->
        /// <summary>
        ///      Gets the child nodes with a particular tag
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeTag"></param>
        /// <returns></returns>
        public List<XmlNode> FindChildren(string nodeTag)
        {
            List<XmlNode> nodeList = new List<XmlNode>();
            foreach (RichXmlNode childNode in RichChildNodes)
                if (childNode.Tag == nodeTag)
                    nodeList.Add(childNode.Node);
            return nodeList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FlatTraversal -->
        /// <summary>
        ///      Recursively Traversals an XML document or sub-document returning its tags ina string
        /// </summary>
        /// <param name="node"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string FlatTraversal(int level)
        {
            string margin = Margin(level);
            string nodeTag;
            string str;


            if (HasChildNodes)
            {
                nodeTag = Tag;
                if (nodeTag == "xs:annotation")
                    str = "";
                else
                {
                 // string nodeType = node.NodeType.ToString();
                    switch (level)
                    {
                        case 0 : str = "";                                               break;
                        case 1 : str = "<" + nodeTag + AttributeString() + ">";          break;
                        default: str = margin + "<" + nodeTag + AttributeString() + ">"; break;
                    }


                    foreach (RichXmlNode childNode in RichChildNodes)
                        str += childNode.FlatTraversal(level + 1);


                    if (level > 0)  str += margin + "</" + nodeTag + ">";
                }
            }
            else
                str = margin + "<" + Tag + AttributeString() + LeafEnd();


            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetTag -->
        /// <summary>
        ///      Analyzes the outer XML to determine the outer element tag
        ///      , there's got to be a better way!
        /// </summary>
        /// <param name="outerXml">The outer XML of a node</param>
        /// <returns></returns>
        private static string GetTag(string outerXml)
        {
            string tag;
            tag = Regex.Replace(outerXml, " .+$", "", RegexOptions.Singleline);
            tag = Regex.Replace(tag, ">.+$", "", RegexOptions.Singleline);
            tag = Regex.Replace(tag, "^<", "");
            return tag;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HasGrandchildren -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool HasGrandchildren()
        {
            bool hasGrandkids = false;
            if (this.HasChildNodes)
                foreach (RichXmlNode child in RichChildNodes)
                    if (child.HasChildNodes)
                        { hasGrandkids = true; break; }
            return hasGrandkids;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IntValue -->
        /// <summary>
        ///      returns an integer or the default if it can not parse one
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>test me</remarks>
        public int IntValue(string xPath, int defaultValue)
        {
            string str = StrValue(xPath, "");
            int    num = TreatAs.IntValue(str, defaultValue);
            return num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LeafEnd -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string LeafEnd()
        {
            if (NodeType == XmlNodeType.XmlDeclaration)
                return "?>";
            else
                return  " />";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NullableDateTimeValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>test me</remarks>
        public DateTime? NullableDateTimeValue(string xPath, DateTime? defaultValue)
        {
            string    str  = StrValue(xPath, "");
            DateTime? time = TreatAs.NullableDateTimeValue(str, defaultValue);
            return    time;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PrettyTraversal -->
        /// <summary>
        ///      Recursively Traverses an XML document or sub-document returning its tags in a string
        /// </summary>
        /// <param name="node"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string PrettyTraversal_recursive(int level)
        {
            string margin = Margin(level);
            string nodeTag = Tag;
            string str;
            string outerXml = OuterXml;


            if (HasGrandchildren())
            {
                switch (level)
                {
                    case 0: str = ""; break;
                    case 1: str = "<" + nodeTag + AttributeString() + ">"; break;
                    default: str = margin + "<" + nodeTag + AttributeString() + ">"; break;
                }


                foreach (RichXmlNode childNode in RichChildNodes)
                    str += childNode.PrettyTraversal_recursive(level + 1);
                if (level == 1) str += "\r\n";
                if (level > 0) str += margin + "</" + nodeTag + ">";
            }
            else
            {
                str = margin + outerXml;
            }


            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Margin -->
        /// <summary>
        ///      A standard margin
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string Margin(int level)
        {
            if (level > 1)
                return "\r\n".PadRight(Math.Max(level * 4 - 4, 0));
            else
                return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MoreAttributes -->
        /// <summary>
        ///      Does stuff
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string MoreAttributes()
        {
            string attr = "";
            string name;


            if (NodeType == XmlNodeType.XmlDeclaration)
            {
                attr = Regex.Replace(OuterXml, "<[?]xml", "");
                attr = Regex.Replace(attr, "[?]>$", "");
            }
            else
            {
                XmlAttributeCollection attributes = Attributes;
                string tagName = Tag;


                if (attributes != null)
                    foreach (XmlAttribute attribute in attributes)
                    {
                        name = attribute.Name;
                        if ((tagName == "xs:schema" || name != "xmlns:xs")
                            && name != "ref")
                            attr += " " + name + "=\"" + attribute.Value + "\"";
                    }
            }


            return attr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StrValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>test me</remarks>
        public string StrValue(string xPath, string defaultValue)
        {
            string str = "";
            XmlNode node = this.Node.SelectSingleNode(xPath);
            if (node == null || node.InnerText == null)
                str = defaultValue;
            else
            {
                str = node.InnerText;
            }
            return str;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- Tag -->
        ///// <summary>
        /////      Returns the tag of the node (there's got to be a better way)
        ///// </summary>
        ///// <param name="node"></param>
        ///// <returns></returns>
        public string Tag_old { get { return GetTag(OuterXml); } }
        public string Tag
        {
            get
            {
             // string nameA = GetTag(OuterXml);
                string name = Name;
                return name;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tag = Tag;
            return "<" + tag + " (" + OuterXml.Length + ") />";
        }

    }
}
