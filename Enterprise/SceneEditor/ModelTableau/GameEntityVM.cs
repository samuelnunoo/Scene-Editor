using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace LoadScene.SceneEditor.UIElements
{
    public class GameEntityVM: ViewModel
    {

        private string _model;


        public GameEntityVM(string model)
        {
            this._model = model;
        }

        [DataSourceProperty]
        public string GameModelID
        {
            get { return _model;}
            
            set
            {
                if (!(this._model != value))
                    return;
                this._model = value;
                this.OnPropertyChanged(nameof(GameModelID));
            }
        }
    }
}