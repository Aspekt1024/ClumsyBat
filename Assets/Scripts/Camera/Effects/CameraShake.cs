using System.Collections;
using UnityEngine;
using ClumsyBat;

public class CameraShake {

    private Transform _camera;
    private Vector3 _startPos;

    private float xDeviation;

    private const float Radius = 0.1f;

    private float shakeTimer;

    private Coroutine shakeRoutine;
    
    public void Shake(Transform cameraTransform, float timeSeconds)
    {
        _camera = cameraTransform;
        shakeTimer = timeSeconds;
        _startPos = new Vector3(_camera.position.x - xDeviation, _camera.position.y, _camera.position.z);
        xDeviation = 0f;

        if (shakeRoutine != null)
        {
            GameStatics.GameManager.StopCoroutine(shakeRoutine);
        }
        shakeRoutine = GameStatics.GameManager.StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        var timer = 0f;
        while (timer < shakeTimer)
        {
            timer += Time.deltaTime;
            ShakeCam();
            yield return null;
        }
        StopShake();
    }

    private void ShakeCam()
    {
        Vector2 rPos = Random.insideUnitCircle * Radius;
        _camera.position = new Vector3(_camera.position.x - xDeviation + rPos.x, rPos.y, _startPos.z);
        xDeviation = rPos.x;
    }

    private void StopShake()
    {
        _camera.position = new Vector3(_camera.position.x - xDeviation, 0f, _startPos.z);
        xDeviation = 0f;
    }
}
