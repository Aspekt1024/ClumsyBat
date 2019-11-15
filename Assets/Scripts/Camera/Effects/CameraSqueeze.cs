using System.Collections;
using UnityEngine;

namespace ClumsyBat
{
    public class CameraSqueeze
    {
        private Coroutine squeezeRoutine;
        private float originalOrthSize;
        private Camera currentCamera;
        
        private const float SqueezeAmount = 0.008f;

        public void Squeeze(Camera camera)
        {
            StopSqueeze();
            currentCamera = camera;
            if (GameStatics.GameManager.IsInMenu) return;
            squeezeRoutine = GameStatics.GameManager.StartCoroutine(SqueezeRoutine());
        }
        
        private IEnumerator SqueezeRoutine()
        {
            if (currentCamera == null) yield break;

            originalOrthSize = currentCamera.orthographicSize;

            float timer = 0f;
            const float duration = 0.07f;
            while (timer < duration)
            {
                timer += Time.deltaTime;

                float ratio = timer / duration;
                currentCamera.orthographicSize = originalOrthSize * (1 - SqueezeAmount * ratio);

                yield return null;
            }

            timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float ratio = 1f - timer / duration;
                currentCamera.orthographicSize = originalOrthSize * (1 - SqueezeAmount * ratio);

                yield return null;
            }

            currentCamera.orthographicSize = originalOrthSize;
        }

        private void StopSqueeze()
        {
            if (squeezeRoutine == null) return;
            GameStatics.GameManager.StopCoroutine(squeezeRoutine);
            
            if (currentCamera == null) return;
            currentCamera.orthographicSize = originalOrthSize;
        }
    }
}