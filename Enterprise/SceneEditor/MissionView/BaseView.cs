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
using TaleWorlds.TwoDimension;

namespace LoadScene.SceneEditor.MissionViews


{
    partial class SceneEditorMissionView : MissionView
    {

        private Ray _mouseRay;
        private Vec3 rayBegin;
        private Vec3 rayEnd;
        private GauntletLayer _gauntletLayer;
        private bool _Spectator = true;
        private SpriteCategory _spriteCategory;

        
        public override void OnMissionScreenPreLoad()
        {
            base.OnMissionScreenPreLoad();

            //Implement Handler and LoadScreen
            base.OnMissionScreenInitialize();

          





        }

        public void CreateLayout()
        {
            
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
            this._spriteCategory= spriteData.SpriteCategories["ui_crafting"];
            this._spriteCategory.Load((ITwoDimensionResourceContext) resourceContext, uiResourceDepot);
         
            this._gauntletLayer = new GauntletLayer(1,"GauntletLayer");
            this._gauntletLayer.InputRestrictions.SetInputRestrictions(true,InputUsageMask.All);
            this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
            this._gauntletLayer.IsFocusLayer = true;
            //ScreenManager.TrySetFocus((ScreenLayer) this._gauntletLayer);
            MissionScreen.AddLayer((ScreenLayer) this._gauntletLayer);
            this._gauntletLayer.LoadMovie("ScrollGrid", new GameEntityVM());
           
            
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


        public void SpectatorToggle()
        {
            _Spectator = !_Spectator;
            MissionScreen.IsCheatGhostMode = _Spectator;

        }
            public override void OnMissionScreenInitialize()
            {

           
                base.OnMissionScreenInitialize();
                CreateLayout();
                MissionScreen.IsCheatGhostMode = _Spectator;

          



            }
    }
    
    
    
    
    
}