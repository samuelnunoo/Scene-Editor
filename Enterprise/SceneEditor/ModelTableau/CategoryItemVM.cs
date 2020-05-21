using TaleWorlds.Core.ViewModelCollection;

namespace LoadScene.SceneEditor.UIElements
{
    public class CategoryItemVM : SelectorItemVM
    {

        private string _category;

        public CategoryItemVM(string item) : base(item)
        {
            
        }




        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }




    }
}