﻿
using LoadScene.SceneEditor.UIElements;
using LoadScene.UIElements;
using SandBox.Quests.QuestBehaviors;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;

namespace LoadScene.SceneEditor.MissionViews


{
    partial class SceneEditorMissionView : MissionView
    {

        private Ray _mouseRay;
        private Vec3 rayBegin;
        private Vec3 rayEnd;
        private GauntletLayer _gauntletLayer;

        
        public override void OnMissionScreenPreLoad()
        {
            base.OnMissionScreenPreLoad();

            //Implement Handler and LoadScreen
            base.OnMissionScreenInitialize();
          




        }

        public void CreateLayout()
        {
            
         
            this._gauntletLayer = new GauntletLayer(1,"GauntletLayer");
            this._gauntletLayer.InputRestrictions.SetInputRestrictions(true,InputUsageMask.All);
            this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
            this._gauntletLayer.IsFocusLayer = true;
            //ScreenManager.TrySetFocus((ScreenLayer) this._gauntletLayer);
            MissionScreen.AddLayer((ScreenLayer) this._gauntletLayer);
            this._gauntletLayer.LoadMovie("ModelTableau", new GameEntityVM());
           
            
            //this._testGlobalLayer = new testGlobalLayer();
            //this._testGlobalLayer.Initialize(); // screen where
            //MissionScreen.AddLayer(this._testGlobalLayer.Layer);
            //MissionScreen
            // ScreenManager.AddGlobalLayer(this._testGlobalLayer, true);
            //ScreenManager.TrySetFocus(this._testGlobalLayer.Layer);
            
            
            

        }
        public override void OnMissionScreenTick(float dt)
            {
                this.MissionScreen.SceneLayer.InputRestrictions.SetMouseVisibility(true);
                base.OnMissionScreenTick(dt); 
                //  CameraLogicTick()
                EntityInteractionsTick();
                
            
            }
            

            
            public override void OnMissionScreenInitialize()
            {

           
                base.OnMissionScreenInitialize();
                CreateLayout();

          



            }
    }
    
    
    
    
    
}