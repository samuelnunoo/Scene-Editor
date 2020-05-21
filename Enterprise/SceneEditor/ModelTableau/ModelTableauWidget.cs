﻿﻿using System.Runtime.InteropServices;
  using TaleWorlds.GauntletUI;

  namespace LoadScene.SceneEditor.UIElements

  {
      public class ModelTableauWidget : TaleWorlds.GauntletUI.TextureWidget
      {

          private string _gamemodelid;
          private ModelTableauTextureProvider _modelTableau;


          public ModelTableauWidget(UIContext context) : base(context)
          {
              this.TextureProviderName = "ModelTableauTextureProvider";
          }
          
          
          [Editor(false)]
          public string GameModelID
          {
              get
              {
                  return this._gamemodelid; 
              }
              set
              {
                  if (!(value != this._gamemodelid))
                      return;
                  this._gamemodelid= value;
                  this.OnPropertyChanged((object) value, nameof (GameModelID));
                  this.SetTextureProviderProperty(nameof (GameModelID), (object) value);
              }
          }

          
          
 
      }
  }