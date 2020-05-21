using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using UIExtenderLib.Interface;

namespace LoadScene.SceneEditor.UIElements
{
    public class GameEntityVM: ViewModel
    {
        
        private XMLParse _xmlParse;
        private MBBindingList<PrefabVM> _pagemodel;
        private IEnumerator<PrefabVM> _enumerator;
        private MBBindingList<String> _categories;
        private SelectorVM<CategoryItemVM> _selector;
        private string _searchText;
        private string _prevSearchText;
        private int _amount = 20;
        private MBBindingList<CategoryItemVM> _selectedCategory;
        private CustomMissionManagerHandler _handler;
        


        //Init
        public GameEntityVM()
        {
            
            //Load Parser 
            this._xmlParse = new XMLParse();
            
           
            
            //Get Categories
            _categories = new MBBindingList<string>();
            SetCategories();
            
            
            //Set Selector
            this.PrimarySelector = new SelectorVM<CategoryItemVM>(this._categories,0,null);
            this.PrimarySelector.SetOnChangeAction( new Action<SelectorVM<CategoryItemVM>>(this.UpdatePrimaryUsageIndex) );
            
            //Set Default Category
            _selectedCategory = new MBBindingList<CategoryItemVM>();

            //Example Search
            _searchText = "door";
            _amount = 20;
            Search();


        }
        
        //Primary Selector 
        [DataSourceProperty]
        public SelectorVM<CategoryItemVM> PrimarySelector
        {
            get
            {
                return _selector;
            }
            set
            {
                if (!(value != this._selector))
                    return;
                _selector = value;
                
                this.OnPropertyChanged(nameof (PrimarySelector));
            }
        }
        
        private void UpdatePrimaryUsageIndex(SelectorVM<CategoryItemVM> selector)
        {
            if (selector.SelectedIndex == -1)
                return;
            
            //Reset selectedCategory
            SelectedCategory = new MBBindingList<CategoryItemVM>();
            
            //Add Category Items 
            foreach (var category in _xmlParse.Entities[selector.SelectedItem.StringItem])
            {
                var temp = new CategoryItemVM(category.Key);
                temp.Category = selector.SelectedItem.StringItem;
                SelectedCategory.Add(temp);
            }
            
          
        }
        
        
        //Selected Catgory
        public MBBindingList<CategoryItemVM> SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }

            set
            {
                if (!(value != this._selectedCategory))
                    return;
                _selectedCategory = value;
                
                this.OnPropertyChanged(nameof (SelectedCategory));
                
            }

        }
        
        
        
        
        
        //Category Selectors 
        public Dictionary<string,List<PrefabVM>> GetCategory(string category)
        {
            if (!_xmlParse.Entities.ContainsKey(category))
                return null;

            return _xmlParse.Entities[category];
        }

        public List<PrefabVM> GetSubCategory(Dictionary<string,List<PrefabVM>> category, string subcategory)
        {
            if (!category.ContainsKey(subcategory))
                return null;

            return category[subcategory];
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

        
        //Category Views 
        public List<string> CategoryList()
        {
            List<string> category = new List<string>();

            foreach (var key in _xmlParse.Entities.Keys)
            {
                category.Add(key);
                
            }

            return category;
        }
        public List<string> SubCategoryList(Dictionary<string, List<PrefabVM>> category)
        {
            List<string> subcategory = new List<string>();

            foreach (var sc in category.Keys)
            {
                subcategory.Add(sc);
                
            }

            return subcategory;
        }
        
  
        //Get and Set Categories
        public void SetCategories()
        {
            foreach (var category in _xmlParse.Entities)
            {
                if (category.Key == "")
                {
                    continue;
                }
                _categories.Add(category.Key);
                
               
                
            }
        }


        [DataSourceProperty]
        public MBBindingList<String> GetCategories
        {
            get
            {
                return _categories;

            }

            set
            {
                if (!(value != this._categories))
                    return;
                _categories = value;
                
                this.OnPropertyChanged(nameof (GetCategories));
                
            }
        }
        
        
        
        //Game Entity Views 
        public MBBindingList<PrefabVM> Enumerate(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (_enumerator.Current == null)
                {
                    _enumerator.MoveNext();
                }

                _pagemodel.Add(_enumerator.Current);
                _enumerator.MoveNext();

            }

            return _pagemodel;
        }
        public MBBindingList<PrefabVM> Filter(List<PrefabVM> subcategory, int amount)
        {

            //Category Change Check 
            var enumerator = subcategory.GetEnumerator();
            if (!_enumerator.Equals(enumerator))
            {
                _enumerator = enumerator;
                _pagemodel = new MBBindingList<PrefabVM>();
            }


            //Add amount to PageModel
            return Enumerate(amount);

         

        }
       




        // DataSource Properties
        [DataSourceProperty]
        public MBBindingList<PrefabVM> GameEntityList
        {
            get
            {
                return _pagemodel;

            }

            set
            {
                if (value != _pagemodel)
                {
                    _pagemodel= value;
                    
                }
                
                this.OnPropertyChanged(nameof(GameEntityList));
                
            }
        }


        [DataSourceProperty]
        public string SearchText
        {
            get
            {
                return this._searchText;
            }
            set
            {
                if (!(value != this._searchText))
                    return;
                _searchText = value;
                
                this.OnPropertyChanged(nameof (SearchText));
            }
        }
        
        
        //DataSource Methods
        [DataSourceMethod]
        public void Search()
        {

            if (_searchText == _prevSearchText)
                return;
            
            // Clear Page Model 
            _pagemodel = new MBBindingList<PrefabVM>();
            List<PrefabVM> result = new List<PrefabVM>();
            
            

            foreach (var category in _xmlParse.Entities)
            {
                foreach (var sub_category in category.Value)
                {
                    result.AddRange(sub_category.Value.Select(name => name).Where(name => name.ModelID.Contains(_searchText) ));
                    
                }
                
            }


            _enumerator = result.GetEnumerator();

            _prevSearchText = _searchText;
            var entities = Enumerate(_amount);
            if (entities.Count < 1)
                return;

            GameEntityList = entities;




        }



        [DataSourceMethod]
        public void Test()
        {
            
        }
        
       



        
        
        
        
        
    }
}