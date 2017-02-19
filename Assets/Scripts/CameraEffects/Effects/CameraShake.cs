using UnityEngine;

public class CameraShake : MonoBehaviour {

    private Transform _camera;
    private Vector3 _startPos;

    private float _xDeviation;

    private const float ShakeRate = 0.05f;
    private const float Radius = 0.1f;

    private void Awake()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void OnEnable()
    {
        CameraEventListener.OnCameraShake += Shake;
        CameraEventListener.OnStopCameraShake += StopShake;
    }
    private void OnDisable()
    {
        CameraEventListener.OnCameraShake -= Shake;
        CameraEventListener.OnStopCameraShake -= StopShake;
    }

    public void Shake()
    {
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

    public void StopShake()
    {
        _camera.position = new Vector3(_camera.position.x - _xDeviation, 0f, _startPos.z);
        CancelInvoke();
        _xDeviation = 0f;
    }
}
