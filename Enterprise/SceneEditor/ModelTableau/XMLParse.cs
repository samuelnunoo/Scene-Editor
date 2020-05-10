using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

//using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using NetworkMessages.FromServer;
using TaleWorlds.Library;

namespace LoadScene.SceneEditor.UIElements
{
    public class XMLParse
    {

        private List<string> _entities;
        private ResourceDepot _depot;
        private bool _complete = false;



        public bool Complete => _complete;
        public List<string> Entities => _entities;

        public XMLParse()
        {
            this._entities = new List<string>();
            this._depot = new ResourceDepot(BasePath.Name);
            this._depot.AddLocation("Modules/Native/Prefabs");
            this._depot.CollectResources();
            var files = this._depot.GetFiles("",".xml");
            foreach (var xmlFile in files)
            {
                GetNames(xmlFile);
            }

            _complete = true;


        }

        public void GetNames(string file)
        {
            using (XmlReader reader = XmlReader.Create(file))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "game_entity")
                        {
                            var name = reader.GetAttribute("name").Clone().ToString();
                            if (!_entities.Contains(name))
                            {
                                _entities.Add(name);
                            }
                           
                        }
                    }
                }
            }
        }
    
        
    }
}