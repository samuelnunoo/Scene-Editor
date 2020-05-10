﻿using LoadScene.SceneEditor.MissionViews;
 using SandBox;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
 using TaleWorlds.MountAndBlade;

 namespace LoadScene.SceneEditor
{
    public class CustomMissionManagerHandler : CampaignMissionManager, EditorSceneHandler
    {
        private ScreenBase _screen;
        
        IMission EditorSceneHandler.LoadSceneEditor() { return CustomMissionManager.OpenSceneEditor(); }
      
        void EditorSceneHandler.LoadScreen() { 
            Game.Current.GameStateManager.CleanAndPushState((GameState) Game.Current.GameStateManager.CreateState<CustomEditorState>(), 0);
            
        }

        ScreenBase EditorSceneHandler.GetScreen()
        {
            return _screen;
        }


        void EditorSceneHandler.PlaceItem(string item)
        {
            Mission.Current.GetMissionBehaviour<SceneEditorMissionView>().PlaceItem(item);
            
        }
    }
}