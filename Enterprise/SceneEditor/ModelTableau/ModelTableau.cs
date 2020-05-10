﻿﻿using System;

using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;


  namespace LoadScene.SceneEditor.UIElements
{
    public class ModelTableau
    {
        
        
        //Class Variables 
        private Scene _tableauScene;
        private GameEntity _model;
        private bool _initialized;
        private MatrixFrame _modelFrame;
        private MatrixFrame _initialFrame;
        
        
        //Camera Vars
        private Vec3 _curCamDisplacement = Vec3.Zero;
        private string _stringId;
        private bool _isRotating;
        private bool _isTranslating;
        private bool _isRotatingByDefault;
        private int _tableauSizeX;
        private int _tableauSizeY;
        private float _cameraRatio;
        private Camera _camera;
        private Vec3 _midPoint;
        private const float InitialCamFov = 1f;
        private float _curZoomSpeed;
        private bool _isEnabled;
        private float _panRotation;
        private float _tiltRotation;
        
    



        //Public Variables 
        public Scene TableauScene => _tableauScene;
        public ModelTableau()
        {
            this.SetEnabled(true);
        }
        public Texture Texture { get; private set; }
        private TableauView View
        {
            get
            {
                return (NativeObject) this.Texture != (NativeObject) null ? this.Texture.TableauView : (TableauView) null;
            }
        }
        
        
        
        //Functions 

        public void SetGameModelID(string id)
        {
            this._stringId = id;
            this.Recalculate();
        }
        
        
         public void Recalculate()
    {
      if (this._stringId.IsStringNoneOrEmpty())
        return;

      this.RefreshModelTableau();
        
        //Camera Updates 
        if ((NativeObject) this._model != (NativeObject) null)
        {
            float num1 = Screen.RealScreenResolutionWidth / (float) this._tableauSizeX;
            float num2 = Screen.RealScreenResolutionHeight / (float) this._tableauSizeY;
            float num3 = (double) num1 > (double) num2 ? num1 : num2;
            if ((double) num3< 1.0)
            {
                Vec3 globalBoxMax = this._model.GlobalBoxMax;
                Vec3 globalBoxMin = this._model.GlobalBoxMin;
                this._modelFrame = this._model.GetFrame();
                float length = this._modelFrame.rotation.f.Length;
                this._modelFrame.rotation.Orthonormalize();
                this._modelFrame.rotation.ApplyScaleLocal(length * num3);
                this._model.SetFrame(ref this._modelFrame);
                if (globalBoxMax.NearlyEquals(this._model.GlobalBoxMax, 1E-05f) && globalBoxMin.NearlyEquals(this._model.GlobalBoxMin, 1E-05f))
                {
                    this._model.SetBoundingboxDirty();
                    this._model.RecomputeBoundingBox();
                }
                this._modelFrame.origin += (globalBoxMax + globalBoxMin - this._model.GlobalBoxMax - this._model.GlobalBoxMin) * 0.5f;
                this._model.SetFrame(ref this._modelFrame);
                this._midPoint = (this._model.GlobalBoxMax + this._model.GlobalBoxMin) * 0.5f + (globalBoxMax + globalBoxMin - this._model.GlobalBoxMax - this._model.GlobalBoxMin) * 0.5f;
            }
            else
                this._midPoint = (this._model.GlobalBoxMax + this._model.GlobalBoxMin) * 0.5f;
         
            this.ResetCamera();
        }
        this._isRotatingByDefault = false;
        this._isRotating = false;


    }
         
         
         //Functions Oteher
         
        public void Initialize()
        {
            
            //Create Scene
            this._isRotatingByDefault = false;
            this._isRotating = false;
            this._isTranslating = false;
            this._tableauScene = Scene.CreateNewScene(true);
            this._tableauScene.SetName( nameof (_model));
            this._tableauScene.DisableStaticShadows(true);
            this._tableauScene.SetAtmosphereWithName("character_menu_a");
            Vec3 direction = new Vec3(1f,-1f,-1f,-1f);
            this._tableauScene.SetSunDirection(ref direction);

            
            //Reset Camera and Init
            this.ResetCamera();
            this._initialized = true;
            



        }
        private void RefreshModelTableau()
        {
            if (!this._initialized)
                this.Initialize();

            if (this._model != null)
            {
                this._model.Remove();
                this._model = (GameEntity) null;
            }
            

            this._model = GameEntity.Instantiate(this._tableauScene, _stringId, MatrixFrame.Identity);
            

       
            TableauView view = this.View;
            if ((NativeObject) view != (NativeObject) null)
            {
                float radius = (this._model.GetBoundingBoxMax() - this._model.GetBoundingBoxMin()).Length * 2f;
                Vec3 origin = this._model.GetGlobalFrame().origin;
                view.SetFocusedShadowmap(true, ref origin, radius);
            }

            this._initialFrame = this._model.GetFrame();
            Vec3 eulerAngles = this._initialFrame.rotation.GetEulerAngles();
            this._panRotation = eulerAngles.x;
            this._tiltRotation = eulerAngles.z;
        }
        protected void SetEnabled(bool enabled)
        {
            this._isRotatingByDefault = false;
            this._isRotating = false;
            this.ResetCamera();
            this._isEnabled = enabled;
            TableauView view = this.View;
            if (!((NativeObject) view != (NativeObject) null))
                return;
            view.SetEnable(this._isEnabled);
        }
        public void SetTargetSize(int width, int height)
        {
            this._isRotating = false;
            if (width <= 0 || height <= 0)
            {
                this._tableauSizeX = 10;
                this._tableauSizeY = 10;
            }
            else
            {
                this._tableauSizeX = width;
                this._tableauSizeY = height;
            }
            this._cameraRatio = (float) this._tableauSizeX / (float) this._tableauSizeY;
            TableauView view = this.View;
            if ((NativeObject) view != (NativeObject) null)
                view.SetEnable(false);
            this.Texture = TableauView.AddTableau(new RenderTargetComponent.TextureUpdateEventHandler(this.TableauMaterialTabInventoryItemTooltipOnRender), (object) this._tableauScene, this._tableauSizeX, this._tableauSizeY);
        }

        public void OnFinalize()
        {
            if ((NativeObject) this._camera != (NativeObject) null)
            {
                this._camera.ReleaseCameraEntity();
                this._camera = (Camera) null;
            }
            TableauView view = this.View;
            if ((NativeObject) view != (NativeObject) null)
                view.SetEnable(false);
            if (!((NativeObject) this._tableauScene != (NativeObject) null))
                return;
            this._tableauScene.ClearAll();
            this._tableauScene = (Scene) null;
            
        }
        private void TableauMaterialTabInventoryItemTooltipOnRender(Texture sender, EventArgs e)
        {
          TableauView tableauView = this.View;
          if ((NativeObject) tableauView == (NativeObject) null)
          {
            tableauView = sender.TableauView;
            tableauView.SetEnable(this._isEnabled);
          }
       
          
            tableauView.SetRenderWithPostfx(true);
            tableauView.SetClearColor(0U);
            tableauView.SetScene(this._tableauScene);
            if ((NativeObject) this._camera == (NativeObject) null)
            {
              this._camera = Camera.CreateCamera();
              this._camera.SetViewVolume(true, -0.5f, 0.5f, -0.5f, 0.5f, 0.01f, 100f);
              this.ResetCamera();
              tableauView.SetSceneUsesSkybox(false);
            }
            tableauView.SetCamera(this._camera);
            if (this._isRotatingByDefault)
              this.UpdateRotation(1f, 0.0f);
            tableauView.SetDeleteAfterRendering(false);
            tableauView.SetContinuousRendering(true);
          
        }
        
        
        
        // Camera Logic 
        private void TranslateCamera(bool value)
        {
            this.TranslateCameraAux(value);
        }
        private void TranslateCameraAux(bool value)
        {
            this._isRotatingByDefault = !value && this._isRotatingByDefault;
            this._isTranslating = value;
            this.UpdateMouseLock();
        }

        private void SetCamFovHorizontal(float camFov)
        {
            this._camera.SetFovHorizontal(camFov, 1f, 0.1f, 50f);
        }
        private void MakeCameraLookMidPoint()
        {
            this._camera.Position = this._midPoint + this._camera.Frame.rotation.TransformToParent(this._curCamDisplacement) - this._camera.Direction * (this._midPoint.Length * 0.5263158f);
        }
        private void ResetCamera()
        {
            this._curCamDisplacement = Vec3.Zero;
            this._curZoomSpeed = 0.0f;
            if (!((NativeObject) this._camera != (NativeObject) null))
                return;
            this._camera.Frame = MatrixFrame.Identity;
            this.SetCamFovHorizontal(1f);
            this.MakeCameraLookMidPoint();
        }
        
        public void RotateItem(bool value)
        {
            this._isRotatingByDefault = !value && this._isRotatingByDefault;
            this._isRotating = value;
            this.UpdateMouseLock();
        }
        
        private void UpdateRotation(float mouseMoveX, float mouseMoveY)
        {
            if (!this._initialized)
                return;
            this._panRotation += mouseMoveX * ((float) Math.PI / 720f);
            this._tiltRotation += mouseMoveY * ((float) Math.PI / 720f);
            this._tiltRotation = MathF.Clamp(this._tiltRotation, -2.984513f, -0.1570796f);
            MatrixFrame frame1 = this._model.GetFrame();
            Vec3 vec3 = (this._model.GetBoundingBoxMax() + this._model.GetBoundingBoxMin()) * 0.5f;
            MatrixFrame identity1 = MatrixFrame.Identity;
            identity1.origin = vec3;
            MatrixFrame identity2 = MatrixFrame.Identity;
            identity2.origin = -vec3;
            MatrixFrame matrixFrame = frame1 * identity1;
            matrixFrame.rotation = Mat3.Identity;
            matrixFrame.rotation.ApplyScaleLocal(this._initialFrame.rotation.GetScaleVector());
            matrixFrame.rotation.RotateAboutSide(this._tiltRotation);
            matrixFrame.rotation.RotateAboutUp(this._panRotation);
            MatrixFrame frame2 = matrixFrame * identity2;
            
            this._model.SetFrame(ref frame2);
        }
        
        private void UpdatePosition(float mouseMoveX, float mouseMoveY)
        {
            if (!this._initialized)
                return;
            this._curCamDisplacement += new Vec3(mouseMoveX / (float) -this._tableauSizeX, mouseMoveY / (float) this._tableauSizeY, 0.0f, -1f) * (2.2f * this._camera.HorizontalFov);
            this.MakeCameraLookMidPoint();
        }
        public void Zoom(double value)
        {
            this._curZoomSpeed -= (float) (value / 1000.0);
        }

  
        public void OnTick(float dt)
        {
            try
            {
                var width = _model.GlobalBoxMax.X - _model.GlobalBoxMin.X;
                var height = _model.GlobalBoxMax.Y - _model.GlobalBoxMin.Y;
                var length = _model.GlobalBoxMax.Z - _model.GlobalBoxMin.Z;

                var frame2 = MatrixFrame.Identity;
                
                //Iitial Position
                
                
                var area = Math.Max(Math.Max(width, height), length);
                var fov = _camera.GetFovHorizontal();

                frame2.rotation.RotateAboutSide(0.8f);
                frame2.rotation.RotateAboutForward(0f);

                var center = _model.GlobalBoxMin+ (_model.GlobalBoxMax - _model.GlobalBoxMin) / 2;
               // var biasedCenter = Vec3.Lerp( center, _model.CenterOfMass, 0.6f);
                var biasedCenter = center.Z > _model.CenterOfMass.Z
                    ? Vec3.Lerp(center, _model.CenterOfMass, 0.6f )
                    : center;
                var testing = Vec3.Vec3Min(center, _model.CenterOfMass);
                var distance = (float) ((area / 2) / Math.Tan(fov / 2));
                
                var position = center + new Vec3(0,-1f,1.0f) * distance;

              

                var frame = MatrixFrame.Identity;
                _camera.Frame = new MatrixFrame(frame2.rotation,position*1f);
                

              
                
        
                
            }
            catch
            {
                
            }
           
            
      
            

          
            
            

        }
        
        private void TickCameraZoom(float dt)
        {
            if (!((NativeObject) this._camera != (NativeObject) null))
                return;
            this.SetCamFovHorizontal(MBMath.ClampFloat(this._camera.HorizontalFov + this._curZoomSpeed, 0.1f, 2f));
            if ((double) dt <= 0.0)
                return;
            this._curZoomSpeed = MBMath.Lerp(this._curZoomSpeed, 0.0f, MBMath.ClampFloat(dt * 25.9f, 0.0f, 1f), 1E-05f);
        }
        private void UpdateMouseLock()
        {
            int num = this._isRotating ? 1 : (this._isTranslating ? 1 : 0);
            MouseManager.LockCursorAtCurrentPosition(num != 0);
            MouseManager.ShowCursor(num == 0);
        }
        
        
    }
    
}