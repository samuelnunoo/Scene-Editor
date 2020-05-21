using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Xml;
using System.Text.RegularExpressions;

//using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using HarmonyLib;
using NetworkMessages.FromServer;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using BinaryReader = System.IO.BinaryReader;

namespace LoadScene.SceneEditor.UIElements
{
    public class XMLParse
    {
        
        private ResourceDepot _depot;
        private bool _complete = false;
        private Dictionary<string, Dictionary<string, List<PrefabVM>>> _dictionary;

        
        public bool Complete => _complete;
        public Dictionary<string, Dictionary<string, List<PrefabVM>>>  Entities => _dictionary;

        
        
        public XMLParse()
        {
            this._dictionary = new Dictionary<string, Dictionary<string, List<PrefabVM>>>();
            this._depot = new ResourceDepot(BasePath.Name);
            this._depot.AddLocation("Modules/Native/Prefabs");
            this._depot.CollectResources();

           
          
            //Loop
            var files = this._depot.GetFiles("",".xml");
            foreach (var xmlFile in files)
            {
          
                // XML Names 
                var file_name = new Regex("\\w*\\.xml").Match(xmlFile).Value;
                var category_name = new Regex("^[a-z]*").Match(file_name).Value;
                var subcategory_name = new Regex("(?<=_)[a-z]*").Match(file_name).Value;
                
                


           
                

          
                //Check if Exists 
                if (!_dictionary.ContainsKey(category_name))
                { _dictionary.Add(category_name,new Dictionary<string, List<PrefabVM>>());
                }

                if (!_dictionary[category_name].ContainsKey(subcategory_name))
                {
                    _dictionary[category_name].Add(subcategory_name, new List<PrefabVM>());
                }
                
                
                //Add to Dictionary 
                GetNames(xmlFile, _dictionary[category_name][subcategory_name]);


            }

            _complete = true;
         

        }

        

        public void GetNames(string file,List<PrefabVM> location)
        {

            var scene = Scene.CreateNewScene(true);
           
            
            //Iterate
            using (XmlReader reader = XmlReader.Create(file))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        
                        if (reader.Name == "game_entity")
                        {
                            
                            var name = reader.GetAttribute("name").ToString();
                            var check = location.Select(vm => vm.ModelID).Where(id => id.Equals(name));
                            if (!check.Contains(name))
                            {
                                try
                                {
                                    
                                 
                                  if (GameEntity.PrefabExists(name) && name.Length > 3)
                                  {

                                      var item = GameEntity.Instantiate(scene, name, false);
                                        var prefab = new PrefabVM();
                                        prefab.SetModelID(name);
                                        location.Add(prefab);
                                        item.Remove();
                                        




                                  }
                                  
                               
                                }
                                catch ( Exception e )
                                {
                             
                                }



                            }
                           
                        }

                        reader.ReadSubtree();
                    }
                }
            }

           

        }








    }
}