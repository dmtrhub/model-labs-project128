using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIM.Model;
using CIM.Manager;

namespace CIM.Handler
{
    class CIMXMLReaderHandler : IHandler
    {
        internal enum Level
        {
            Root = 0,
            Element,
            Property
        };

        #region fields

        internal const string rdfRootQ = "rdf:RDF";
        internal const string rdfRoot = "rdf";
        internal const string rdfID = "rdf:ID";
        internal const string rdfid = "rdf:id";
        internal const string rdfId = "rdf:Id";
        internal const string xmlnsPrefix = "xmlns:";
        internal const string rdfResource = "rdf:resource";
        internal const string rdfType = "rdf:type";
        internal const string rdfDescription = "rdf:Description";

        private string content = string.Empty; //// text content of subelement  

        //// current level in xml document
        private Level inLevel = Level.Root;

        private CIMModel model; //// object CIM model which content is being loaded during parsing process

        private CIMObject currentElement; //// current element                
        private ObjectAttribute currentAttribute; //// current subelement (attribute of CIM object)

        // For tracking CIMType when parsing <rdf:Description>
        private bool insideRdfDescription = false;

        /// <summary>
        /// Gets the CIMModel object which is finall product of parsing RDF document.
        /// </summary>
        public CIMModel Model
        {
            get
            {
                return model;
            }
        }

        #endregion

        #region IHandler Members

        public void StartDocument(string filePath)
        {
            currentElement = null;
            currentAttribute = null;
            if (model == null)
            {
                model = new CIMModel();
                model.SourcePath = filePath;
            }
            inLevel = Level.Root;
            insideRdfDescription = false;
        }

        public void StartElement(string localName, string qName, SortedList<string, string> atts)
        {
            string type = localName;
            string namespaceName = string.Empty;
            if (qName.Contains(StringManipulationManager.SeparatorColon))
            {
                namespaceName = qName.Substring(0, qName.IndexOf(StringManipulationManager.SeparatorColon));
            }

            if (string.Compare(qName, rdfRootQ, true) == 0)
            {
                //// rdf:RDF element - extract attributes
                if ((atts != null) && (atts.Count > 0))
                {
                    for (int i = 0; i < atts.Count; i++)
                    {
                        model.AddRDFAttribute(atts.ElementAt(i).Key, atts.ElementAt(i).Value);
                    }
                }
            }
            else if (string.Compare(qName, rdfDescription, true) == 0)
            {
                // Start of <rdf:Description>
                inLevel = Level.Element;
                insideRdfDescription = true;
                currentElement = new CIMObject(model.ModelContext, ""); // CIMType will be set when we find <rdf:type>
                string id = TryReadRDFIdAttribute(atts);
                if (id == null && atts.ContainsKey("rdf:about"))
                {
                    id = atts["rdf:about"];
                    // Remove leading '#' if present
                    if (id.StartsWith("#"))
                        id = id.Substring(1);
                }
                if (id != null)
                {
                    currentElement.ID = id;
                }
                currentElement.NamespaceString = namespaceName;
            }
            else if (insideRdfDescription && string.Compare(qName, rdfType, true) == 0 && inLevel == Level.Element)
            {
                // <rdf:type rdf:resource="...#Curve"/>
                if (atts != null && atts.ContainsKey(rdfResource))
                {
                    string resource = atts[rdfResource];
                    int idx = resource.LastIndexOf('#');
                    if (idx >= 0 && idx + 1 < resource.Length)
                    {
                        string cimType = resource.Substring(idx + 1);
                        currentElement.CIMType = cimType;
                    }
                }
            }
            else if (insideRdfDescription && inLevel == Level.Element)
            {
                // New attribute/property inside <rdf:Description>
                inLevel = Level.Property;
                currentAttribute = new ObjectAttribute(model.ModelContext, type);

                currentAttribute.FullName = type;
                currentAttribute.NamespaceString = namespaceName;

                ReadAndProcessAttributeValue(currentAttribute, atts);
            }
        }

        private void ReadAndProcessAttributeValue(ObjectAttribute objectAttribute, SortedList<string, string> atts)
        {
            if (objectAttribute != null)
            {
                //// if object attribute contains reference to another element, 
                //// it is being read and put on stack of that element (parent element)                
                if (atts != null && atts.ContainsKey(rdfResource))
                {
                    string parentId = atts[rdfResource];
                    objectAttribute.Value = parentId;
                    objectAttribute.IsReference = true;
                }
            }
        }

        //// Method tries to read the "rdf:ID" ettribute from list of attributes
        //// and returns it's value or null value if such attribute can't be found.
        private string TryReadRDFIdAttribute(SortedList<string, string> atts)
        {
            string rdfIdentificator = null;
            if (atts.ContainsKey(rdfID))
            {
                rdfIdentificator = atts[rdfID];
            }
            else if (atts.ContainsKey(rdfid))
            {
                rdfIdentificator = atts[rdfid];
            }
            else if (atts.ContainsKey(rdfId))
            {
                rdfIdentificator = atts[rdfId];
            }
            return rdfIdentificator;
        }

        public void EndElement(string localName, string qName)
        {
            string type = localName;

            if (string.Compare(qName, rdfDescription, true) == 0)
            {
                // End of <rdf:Description>
                inLevel = Level.Root;
                insideRdfDescription = false;
                if (currentElement != null)
                {
                    if (model.GetObjectByID(currentElement.ID) == null)
                    {
                        model.InsertObjectInModelMap(currentElement);
                    }
                    currentElement = null;
                }
            }
            else if (insideRdfDescription && inLevel == Level.Property)
            {
                inLevel = Level.Element;
                content = content.Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    if (currentAttribute != null)
                    {
                        //// if attribute value wasn't set as the reference, we read the text content and set it in value
                        if (string.IsNullOrEmpty(currentAttribute.Value))
                        {
                            currentAttribute.Value = content;
                        }
                    }
                    content = string.Empty;
                }

                AddAttributeToCurrentElement();
            }
        }

        public void StartPrefixMapping(string prefix, string uri)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = string.Empty;
            }
            model.AddRDFNamespace(prefix, uri);
        }

        public void EndDocument()
        {
        }

        public void Characters(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                content = text;
            }
            else
            {
                content = string.Empty;
            }
        }

        //// Method adds the currentAttribute to currentElement if both of them exists,
        //// ie. adds currentAttribute to embeddedElement.
        private void AddAttributeToCurrentElement()
        {
            if (currentAttribute != null)
            {
                if ((inLevel == Level.Element) && (currentElement != null))
                {
                    //check if it is a duplicate attribute
                    currentElement.AddAttribute(currentAttribute);
                }
            }
        }

        #endregion
    }
}