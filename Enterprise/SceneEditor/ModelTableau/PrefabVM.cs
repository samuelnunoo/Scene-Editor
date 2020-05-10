﻿using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
 using TaleWorlds.Engine;
 using TaleWorlds.Library;
 using UIExtenderLib.Interface;

 namespace LoadScene.SceneEditor.UIElements
{
    
    public class PrefabVM : ViewModel
    {
        
        private string _gamemodelid;
        private EditorSceneHandler _handler;

        public PrefabVM()
        {
            this._handler = new CustomMissionManagerHandler();
        }

        [DataSourceProperty]
        public string  ModelID
        {
            get
            {
                return _gamemodelid;
            }
            set
            {
                if (!(_gamemodelid != value))
                    return;
                _gamemodelid = value;
                this.OnPropertyChanged(nameof (ModelID));
                
            }
        }

        [DataSourceMethod]
        private void Place()
        {
            this._handler.PlaceItem(this._gamemodelid);
            
        }

        public void SetModelID(string id)
        {
            this.ModelID = id;
        }
        


    }
}