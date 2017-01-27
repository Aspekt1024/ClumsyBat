using UnityEngine;

public abstract class Spawnable : MonoBehaviour {
    protected bool bPaused;
    protected bool IsActive;
    protected float Speed;
    
    public struct SpawnType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
    }

    public void SetSpeed(float speed){ Speed = speed; }
    public virtual void PauseGame(bool paused) { bPaused = paused; }
    
    public virtual void SendToInactivePool()
    {
        transform.position = Toolbox.Instance.HoldingArea;
        IsActive = false;
    }

    // TODO activate
    public virtual void Activate(Transform tf, SpawnType spawnTf, float zLayer)
    {
        SetTransform(tf, spawnTf, zLayer);
    }
    
    public void SetTransform(Transform objTf, SpawnType spawnTf, float zLayer)
    {
        objTf.localPosition = new Vector3(spawnTf.Pos.x, spawnTf.Pos.y, zLayer);
        objTf.localScale = spawnTf.Scale;
        objTf.rotation = spawnTf.Rotation;
    }

    protected void MoveLeft(float time) { transform.position += Vector3.left * time * Speed; }
    protected void ActivateSpawnable(Vector3 position) { transform.localPosition = position; }
}
