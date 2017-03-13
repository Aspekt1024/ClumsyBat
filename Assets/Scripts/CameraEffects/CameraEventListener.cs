public class CameraEventListener {

    public delegate void CameraTimedEvent(float time);

    public static CameraTimedEvent OnCameraShake;
    
    public static void CameraShake(float time)
    {
        if (OnCameraShake != null)
            OnCameraShake(time);
    }
}
