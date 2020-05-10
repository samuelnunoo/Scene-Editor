using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using UIExtenderLib.Interface;

namespace LoadScene.SceneEditor.UIElements
{
    public class GameEntityVM: ViewModel
    {

        private MBBindingList<PrefabVM> _prefabs;
        private XMLParse _xmlParse;
        private MBBindingList<PrefabVM> _pagemodel;
        private IEnumerator<PrefabVM> _enumerator;

        public GameEntityVM()
        {
            this._prefabs =new MBBindingList<PrefabVM>();
            this._xmlParse = new XMLParse();
            AddPrefabList(_xmlParse.Entities);
            this._enumerator = _prefabs.GetEnumerator();
            PageRange(20);
            

        }
        
        
        public void  AddPrefab(string id)
        {
            if (GameEntity.PrefabExists(id))
            {
                var prefab = new PrefabVM();
                prefab.SetModelID(id);
                _prefabs.Add(prefab);
            }
        }
                


                
        public void AddPrefabList(List<string> list)
        {
            foreach (var prefab in list)
            {
                var data = prefab.Clone().ToString();
                AddPrefab(data);
            }
        }




        public void PageRange(int range)
        {
            var list = new MBBindingList<PrefabVM>();

   
            for (int i = 0; i < range; i++)
            {
                if (_enumerator.Current == null)
                {
                    _enumerator.MoveNext();
                }
            
                list.Add(_enumerator.Current);
                _enumerator.MoveNext();
            }

            this._pagemodel = list; 
        }
        
        
        
        
        [DataSourceProperty]
        public MBBindingList<PrefabVM> GameEntityList
        {
            get
            {
                return _pagemodel;

            }

            set
            {
                if (value != _prefabs)
                {
                    _prefabs = value;
                    
                }
                
                this.OnPropertyChanged(nameof(GameEntityList));
                
            }
        }



        
        
        
        
        
    }
}