using System;
using UnityEngine;
using System.Collections;

namespace ClumsyBat
{
    public class CameraManager
    {
        private Camera currentCamera;
        private readonly CameraShake shakeComponent = new CameraShake();
        private readonly CameraSqueeze squeezeComponent = new CameraSqueeze();

        public Camera CurrentCamera
        {
            get => currentCamera;
            private set
            {
                currentCamera = value;
                OnCameraChanged.Invoke(currentCamera);
            }
        }

        public Camera MenuCamera { get; }
        public Camera LevelCamera { get; }

        public event Action<Camera> OnCameraChanged = delegate { };

        public CameraManager()
        {
            MenuCamera = GameObject.Find("MenuCamera").GetComponent<Camera>();
            LevelCamera = GameObject.Find("LevelCamera").GetComponent<Camera>();

            CurrentCamera = MenuCamera;
        }

        public void SwitchToLevelCamera()
        {
            MenuCamera.enabled = false;
            LevelCamera.enabled = true;
            CurrentCamera = LevelCamera;
        }

        public void SwitchToMenuCamera()
        {
            MenuCamera.enabled = true;
            LevelCamera.enabled = false;
            CurrentCamera = MenuCamera;
        }

        public void SetEndPoint(float endPointX)
        {
            CurrentCamera.GetComponent<CameraFollowObject>()?.SetEndPoint(endPointX);
        }

        public void GotoPointImmediate(float point)
        {
            CurrentCamera.GetComponent<CameraFollowObject>()?.GotoPoint(point);
        }

        public void StartFollowing(Transform target = null, float followSpeed = CameraFollowObject.BASE_FOLLOW_SPEED)
        {
            CurrentCamera.GetComponent<CameraFollowObject>()?.StartFollowing(target, followSpeed);
        }

        public void StopFollowingAtEndPoint()
        {
            CurrentCamera.GetComponent<CameraFollowObject>()?.StopFollowingAtEndPoint();
        }

        public void StopFollowing()
        {
            CurrentCamera.GetComponent<CameraFollowObject>()?.StopFollowing();
        }

        public void Shake(float duration) => shakeComponent.Shake(CurrentCamera.transform, duration);

        public void Squeeze()
        {
            squeezeComponent.Squeeze(currentCamera);
        }
    }
}
