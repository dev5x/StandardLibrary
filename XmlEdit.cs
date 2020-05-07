using System;
using System.IO;
using System.Text;
using System.Xml;

/* dev5x.com (c) 2020
 * 
 * XML Class
 * XML file editor

    Manage an XML file with this structure:
    <ROOT>
   	    <ROOT_CHILD_ELEMENT Name="AttributeValue1">
   		    <ELEMENT1>Value</ELEMENT1>
   		    <ELEMENT2>Value</ELEMENT2>
   	    </ROOT_CHILD_ELEMENT>
   	    <ROOT_CHILD_ELEMENT Name="AttributeValue2">
   		    <ELEMENT1>Value</ELEMENT1>
   		    <ELEMENT2>Value</ELEMENT2>
   	    </ROOT_CHILD_ELEMENT>
    </ROOT>
*/

namespace dev5x.StandardLibrary
{
    public sealed class XmlEdit : BaseClass
    {
        #region Constructor
        private XmlDocument _xDoc = new XmlDocument();
        private readonly string _xmlFile = string.Empty;

        public XmlEdit(string xmlPathName, string xmlFileName, string rootElement)
        {
            try
            {
                _xmlFile = Path.Combine(xmlPathName, xmlFileName);

                // Create the XML file if it does not exist
                if (!File.Exists(_xmlFile))
                {
                    using (XmlTextWriter xWriter = new XmlTextWriter(_xmlFile, Encoding.UTF8))
                    {
                        xWriter.Formatting = Formatting.Indented;
                        xWriter.WriteStartDocument();
                        xWriter.WriteStartElement(rootElement);
                        xWriter.WriteEndElement();
                        xWriter.WriteEndDocument();
                    }
                }

                // Load the file for this instance
                _xDoc.Load(_xmlFile);
            }
            catch { }
        }
        #endregion

        #region Public Methods

        public void AddElement(string rootChildElementName, string elementAttributeName, string elementAttributeValue)
        {
            // Add new child element under the root
            try
            {
                XmlNode xNodeRoot = _xDoc.DocumentElement;
                bool elementExists = false;

                // Test to see if element and attribute exists exists
                if (xNodeRoot.HasChildNodes)
                {
                    foreach (XmlNode xRootChild in xNodeRoot.ChildNodes)
                    {
                        if (xRootChild.Name == rootChildElementName &&
                            xRootChild.Attributes[0].Name == elementAttributeName &&
                            xRootChild.Attributes[0].Value == elementAttributeValue)
                        {
                            elementExists = true;
                            break;
                        }
                    }
                }

                if (!elementExists)
                {
                    // Element not found, add to file
                    XmlElement newElement = _xDoc.CreateElement(rootChildElementName);
                    newElement.SetAttribute(elementAttributeName, elementAttributeValue);
                    _xDoc.DocumentElement.AppendChild(newElement);
                    _xDoc.Save(_xmlFile);
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("AddElement - " + ex.Message);
            }
        }

        public void DeleteElement(string rootChildElementName, string elementAttributeName, string elementAttributeValue)
        {
            // Delete the element name under the root if it exists
            try
            {
                XmlNode xNodeRoot = _xDoc.DocumentElement;
                bool elementDeleted = false;

                if (xNodeRoot.HasChildNodes)
                {
                    foreach (XmlNode xRootChild in xNodeRoot.ChildNodes)
                    {
                        if (xRootChild.Name == rootChildElementName &&
                            xRootChild.Attributes[0].Name == elementAttributeName &&
                            xRootChild.Attributes[0].Value == elementAttributeValue)
                        {
                            xNodeRoot.RemoveChild(xRootChild);
                            elementDeleted = true;
                            break;
                        }
                    }
                }

                if (elementDeleted)
                {
                    _xDoc.Save(_xmlFile);
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("DeleteElement - " + ex.Message);
            }

        }

        public void SetElementValue(string rootChildElementName, string elementAttributeName, string elementAttributeValue, string childElementName, string childElementValue)
        {
            // Set the value of the child element under the element where the attribute matches
            try
            {
                XmlNode xNodeRoot = _xDoc.DocumentElement;
                bool elementFound = false;

                if (xNodeRoot.HasChildNodes)
                {
                    foreach (XmlNode xRootChild in xNodeRoot.ChildNodes)
                    {
                        if (xRootChild.Name == rootChildElementName &&
                            xRootChild.Attributes[0].Name == elementAttributeName &&
                            xRootChild.Attributes[0].Value == elementAttributeValue)
                        {
                            foreach (XmlNode xNode in xRootChild)
                            {
                                if (xNode.Name == childElementName)
                                {
                                    xNode.InnerText = childElementValue;
                                    elementFound = true;
                                    break;
                                }
                            }

                            if (!elementFound)
                            {
                                XmlElement newElement = _xDoc.CreateElement(childElementName);
                                newElement.InnerText = childElementValue;
                                xRootChild.AppendChild(newElement);
                            }

                            _xDoc.Save(_xmlFile);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("SetElementValue - " + ex.Message);
            }
        }

        public string GetElementValue(string rootChildElementName, string elementAttributeName, string elementAttributeValue, string childElementName, string defaultValue)
        {
            // Get the value of the child element under the rootchild element where the attribute matches
            try
            {
                XmlNode xNodeRoot = _xDoc.DocumentElement;

                if (xNodeRoot.HasChildNodes)
                {
                    foreach (XmlNode xRootChild in xNodeRoot.ChildNodes)
                    {
                        if (xRootChild.Name == rootChildElementName &&
                            xRootChild.Attributes[0].Name == elementAttributeName &&
                            xRootChild.Attributes[0].Value == elementAttributeValue)
                        {
                            if (xRootChild.HasChildNodes)
                            {
                                foreach (XmlNode xNode in xRootChild)
                                {
                                    if (xNode.Name == childElementName)
                                    {
                                        return xNode.InnerText;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                return defaultValue; // No match, return DefaultValue
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetElementValue - " + ex.Message);
                return defaultValue;
            }
        }

        public void EditAttributeValue(string rootChildElementName, string elementAttributeName, string oldValue, string newValue)
        {
            // Update the attribute value if it exists
            try
            {
                XmlNode xNodeRoot = _xDoc.DocumentElement;
                bool valueChanged = false;

                if (xNodeRoot.HasChildNodes)
                {
                    foreach (XmlNode xRootChild in xNodeRoot.ChildNodes)
                    {
                        if (xRootChild.Name == rootChildElementName &&
                            xRootChild.Attributes[0].Name == elementAttributeName &&
                            xRootChild.Attributes[0].Value == oldValue)
                        {
                            xRootChild.Attributes[0].Value = newValue;
                            valueChanged = true;
                            break;
                        }
                    }
                }
                if (valueChanged)
                {
                    _xDoc.Save(_xmlFile);
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("EditAttributeName - " + ex.Message);
            }
        }

        public string[] GetAttributeNames()
        {
            // Return an array containing the first attribute for each child element under the root 
            //<ROOT>
            //  <CHILD1 Name="Attribute1">
            try
            {
                StringBuilder sb = new StringBuilder();
                XmlNode xNodeRoot = _xDoc.DocumentElement;

                if (xNodeRoot.HasChildNodes)
                {
                    foreach (XmlNode xRootChild in xNodeRoot.ChildNodes)
                    {
                        sb.Append(xRootChild.Attributes[0].Value + "|");
                    }
                }

                if (sb.Length > 0)
                {
                    return sb.ToString().Substring(0, sb.Length - 1).Split('|'); // Remove the last "|" and create array
                }
                else
                {
                    return new string[0];
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetAttributeNames - " + ex.Message);
                return new string[0];
            }
        }
        #endregion
    }
}