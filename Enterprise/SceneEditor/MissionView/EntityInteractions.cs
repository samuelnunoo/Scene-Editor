using System;
using System.Collections.Generic;
using System.Threading;
using LoadScene.SceneEditor.UIElements;
using SandBox.View.Map;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;



namespace LoadScene.SceneEditor.MissionViews
{ 
  
    
    partial class SceneEditorMissionView {

        private List<GameEntity> _entities;
        private EditorSceneHandler _handler;
        private testGlobalLayer _testGlobalLayer;
        private bool _initialset;
        private bool _boost = false;
        
        public List<GameEntity> GetEntities()
        {
            //Get Entities 
            this._entities = new List<GameEntity>();
            this.Mission.Scene.GetEntities(ref this._entities);
            return this._entities;

        }

        private GameEntity _item;

     
      
        
        public void MoveItem(GameEntity item)
        {
            Vec2 position = Input.GetMousePositionRanged();
            Vec3 rayBegin;
            Vec3 rayEnd;
            this.MissionScreen.ScreenPointToWorldRay(position,out  rayBegin, out rayEnd ); //
            var xy = rayEnd.AsVec2;
            var terrainHeight = Mission.Scene.GetTerrainHeight(xy);
            rayEnd = new Vec3(xy, terrainHeight);    
            item.SetLocalPosition(rayEnd);
            
            return;
            GameEntity entity;
                var rayThickness = 0.01f;
            var dir = MissionScreen.CombatCamera.Direction;
            var backupRayEnd = rayEnd;
            for(;;)
            {
                this.Mission.Scene.RayCastForClosestEntityOrTerrain(rayBegin, rayEnd, out var collisionDistance,
                    out entity);
                if (entity == null)
                {
                    if (float.IsNaN(collisionDistance))
                    {
                        // air?
                        break;
                    }
                    // terrain
                    break;
                }

                if (float.IsNaN(collisionDistance))
                {
                    // not air?
                    break;
                }

                if (!rayEnd.IsValid)
                {
                    rayEnd = backupRayEnd;
                    break;
                }
                if (!rayBegin.IsValid)
                {
                    break;
                }

                collisionDistance += rayThickness;
                rayBegin = rayBegin.Project(dir,collisionDistance);
                rayEnd = rayEnd.Project(dir,collisionDistance);
            } 

            Debug.Print("Break Here");

            
            // ray marching;
            // after this ray cast, if you wish to move through the collision point
            // you set the rayBegin for the next cast to the rayEnd of the current cast
            // and increment it by rayThickness (0.01f)
            // you continue this in a loop to "march" the ray out until you collide with something you want or the rayEnd has travelled far enough away
            
            //Normalize()
            item.SetLocalPosition(rayEnd);
        }

        public void PlaceItem(string item)
        {
            
            Vec2 position = Input.GetMousePositionRanged();
            Vec3 rayBegin;
            Vec3 rayEnd;
            this.MissionScreen.ScreenPointToWorldRay(position,out  rayBegin, out rayEnd );
            Mat3 rotation = Mat3.Identity;
            this._initialset = true;
            
            
            
            MatrixFrame frame = new MatrixFrame(rotation,rayEnd);
            var entity = GameEntity.Instantiate(Mission.Current.Scene,item, frame);
            _item = entity;
        }
        public void RotateHorizontal(GameEntity item,bool left)
        {
            var degree = 0.01f;
            if (!left)
            {
                degree *= -1;
            }
            if (_boost)
            {
                degree *= 1.4f;
            }
            
            Mat3 rotation = item.GetFrame().rotation;
            rotation.RotateAboutSide(degree);
            Vec3 position = item.GetFrame().origin;
            MatrixFrame frame = new MatrixFrame(rotation,position);
            
            
            item.SetFrame(ref frame);


        }

        public void RotateVertical(GameEntity item, bool up)
        {
            var degree = 0.1f;
            if (!up)
            {
                degree *= -1;
            }

            if (_boost)
            {
                degree *= 1.4f;
            }
            Mat3 rotation = item.GetFrame().rotation;
            rotation.RotateAboutUp(degree);
            Vec3 position = item.GetFrame().origin;
            MatrixFrame frame = new MatrixFrame(rotation,position);
            
            
            item.SetFrame(ref frame);
            
        }
        
        
        public void EntityInteractionsTick()
        {
            if (Input.IsKeyDown(InputKey.LeftMouseButton) || _initialset == true)
            {

                if (_item == null)
                {
                    
                    _item = GetCollidedEntity();

                }
                if (_item != null && !Input.IsKeyReleased(InputKey.LeftMouseButton) ||  _item!= null && _initialset ==true)
                {
                    MoveItem(_item);
                    if (Input.IsKeyDown(InputKey.Q) && _item!=null)
                    {
                        RotateVertical(_item,true);
                    }
                    if (Input.IsKeyDown(InputKey.E) && _item!=null)
                    {
                        RotateVertical(_item,false);
                    }
                    if (Input.IsKeyDown(InputKey.R) && _item!=null)
                    {
                        RotateHorizontal(_item,true);
                    }
                    if (Input.IsKeyDown(InputKey.T) && _item!=null)
                    {
                        RotateHorizontal(_item,false);
                    }
                    if (Input.IsKeyDown(InputKey.LeftShift) && _item!=null)
                    {
                        _boost = true;
                    }
                    if (Input.IsKeyReleased(InputKey.LeftShift) && _item!=null)
                    {
                        _boost = false;
                    }
                    
                    
                    
                }
                
            }


            if (Input.IsKeyReleased(InputKey.LeftMouseButton) && _initialset ==false)
            {
                _item = null; 
            }

            if (Input.IsKeyPressed(InputKey.LeftMouseButton) && _initialset == true)
            {
                _initialset = false;
            }

           
          
        }
        
        
 
        
        private GameEntity GetCollidedEntity()
        {
  
            this.MissionScreen.ScreenPointToWorldRay(this.Input.GetMousePositionRanged(), out rayBegin, out rayEnd);
            GameEntity collidedEntity;
            this.Mission.Scene.RayCastForClosestEntityOrTerrain(rayBegin, rayEnd, out float _, out collidedEntity, 0.3f, BodyFlags.CommonFocusRayCastExcludeFlags);
            while ((NativeObject) collidedEntity != (NativeObject) null && (NativeObject) collidedEntity.Parent != (NativeObject) null)
                collidedEntity = collidedEntity.Parent;
            if (collidedEntity != null)
            {
                InformationManager.DisplayMessage(new InformationMessage(collidedEntity.ToString()));
            }
         
            return collidedEntity;
        }

 
        private void GetCursorIntersectionPoint(
            ref Vec3 clippedMouseNear,
            ref Vec3 clippedMouseFar,
            out float closestDistanceSquared,
            out Vec3 intersectionPoint,
            ref PathFaceRecord currentFace,
            BodyFlags excludedBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
        {
            double num = (double) (clippedMouseFar - clippedMouseNear).Normalize();
            this.MissionScreen.SceneLayer.SceneView.GetScene().GetBoundingBox(out Vec3 _, out Vec3 _);
            Vec3 direction = clippedMouseFar - clippedMouseNear;
            float maxDistance = direction.Normalize();
            this._mouseRay.Reset(clippedMouseNear, direction, maxDistance);
            intersectionPoint = Vec3.Zero;
            closestDistanceSquared = 1E+12f;
            float collisionDistance;
            if (this.MissionScreen.SceneLayer.SceneView.GetScene().RayCastForClosestEntityOrTerrain(clippedMouseNear, clippedMouseFar, out collisionDistance, 0.01f, excludedBodyFlags))
            {
                closestDistanceSquared = collisionDistance * collisionDistance;
                intersectionPoint = clippedMouseNear + direction * collisionDistance;
            }
            currentFace = Campaign.Current.MapSceneWrapper.GetFaceIndex(intersectionPoint.AsVec2);
        }
        
    }
    
    
}