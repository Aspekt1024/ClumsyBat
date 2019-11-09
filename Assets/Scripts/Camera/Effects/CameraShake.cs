using UnityEngine;
using System.Collections.Generic;
using ClumsyBat;

public class CameraShake : MonoBehaviour {

    private Transform _camera;
    private Vector3 _startPos;

    private float _xDeviation;

    private const float ShakeRate = 0.05f;
    private const float Radius = 0.1f;

    private float shakeTimer;
    private bool bShakeActive;

    private void Start()
    {
        _camera = GameStatics.Camera.CurrentCamera.transform;
    }

    private void OnEnable()
    {
        CameraEventListener.OnCameraShake += Shake;
    }
    private void OnDisable()
    {
        CameraEventListener.OnCameraShake -= Shake;
    }

    private void Update()
    {
        if (!bShakeActive) return;

        shakeTimer -= Time.deltaTime;
        if (shakeTimer < 0)
        {
            bShakeActive = false;
            StopShake();
        }
    }

    public void Shake(float timeSeconds)
    {
        bShakeActive = true;
        shakeTimer = timeSeconds;
        _startPos = new Vector3(_camera.position.x - _xDeviation, _camera.position.y, _camera.position.z);
        _xDeviation = 0f;
        CancelInvoke();
        InvokeRepeating("ShakeCam", 0f, ShakeRate);
    }

    private void ShakeCam()
    {
        Vector2 rPos = Random.insideUnitCircle * Radius;
        _camera.position = new Vector3(_camera.position.x - _xDeviation + rPos.x, rPos.y, _startPos.z);
        _xDeviation = rPos.x;
    }

    private void StopShake()
    {
        _camera.position = new Vector3(_camera.position.x - _xDeviation, 0f, _startPos.z);
        CancelInvoke();
        _xDeviation = 0f;
    }
}
