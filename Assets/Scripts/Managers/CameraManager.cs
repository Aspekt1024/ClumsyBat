using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyBat.Managers
{
    public class CameraManager : ManagerBase<CameraManager>
    {
        public Camera MenuCamera;
        public Camera LevelCamera;

        private Camera currentCamera;
        
        public static event Action<Camera> OnCameraChanged = delegate { }; // TODO if isBeingDestroyed, don't add to this

        protected override void Awake()
        {
            base.Awake();
            SwitchToMenuCamera();
        }

        public static void SwitchToLevelCamera()
        {
            Instance.MenuCamera.enabled = false;
            Instance.LevelCamera.enabled = true;
            Instance.currentCamera = Instance.LevelCamera;
            OnCameraChanged.Invoke(Instance.LevelCamera);
        }

        public static void SwitchToMenuCamera()
        {
            Instance.MenuCamera.enabled = true;
            Instance.LevelCamera.enabled = false;
            Instance.currentCamera = Instance.MenuCamera;
            OnCameraChanged.Invoke(Instance.MenuCamera);
        }
        
        public static Camera CurrentCamera
        {
            get { return Instance.currentCamera; }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnCameraChanged = null;
        }
    }
}
