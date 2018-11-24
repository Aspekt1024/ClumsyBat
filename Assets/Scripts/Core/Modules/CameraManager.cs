using System;
using UnityEngine;
using System.Collections;

namespace ClumsyBat
{
    public class CameraManager
    {
        private Camera _currentCamera;

        public Camera CurrentCamera
        {
            get { return _currentCamera; }
            private set
            {
                _currentCamera = value;
                OnCameraChanged.Invoke(_currentCamera);
            }
        }

        public Camera MenuCamera { get; private set; }
        public Camera LevelCamera { get; private set; }

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

        public void Squeeze()
        {
            GameStatics.GameManager.StartCoroutine(SqueezeRoutine());
        }

        private IEnumerator SqueezeRoutine()
        {
            if (GameStatics.GameManager.IsInMenu) yield break;

            var originalSize = CurrentCamera.orthographicSize;

            float timer = 0f;
            const float duration = 0.07f;
            while (timer < duration)
            {
                timer += Time.deltaTime;

                float ratio = timer / duration;
                CurrentCamera.orthographicSize = originalSize * (1 - 0.006f * ratio);

                yield return null;
            }

            timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float ratio = 1f - timer / duration;
                CurrentCamera.orthographicSize = originalSize * (1 - 0.006f * ratio);

                yield return null;
            }

            CurrentCamera.orthographicSize = originalSize;
        }
    }
}
