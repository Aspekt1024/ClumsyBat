public class CameraEventListener {

    public delegate void CameraEventHandler();

    public static CameraEventHandler OnCameraShake;
    public static CameraEventHandler OnStopCameraShake;


    public static void CameraShake()
    {
        if (OnCameraShake != null)
            OnCameraShake();
    }

    public static void StopCameraShake()
    {
        if (OnStopCameraShake != null)
            OnStopCameraShake();
    }
}
