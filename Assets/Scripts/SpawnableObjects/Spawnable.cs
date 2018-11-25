using UnityEngine;

public abstract class Spawnable : MonoBehaviour {

    protected float Speed;
    protected Rigidbody2D body;

    public struct SpawnType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
    }
    
    protected abstract void Init();

    public virtual void Spawn(Transform tf, SpawnType spawnTf)
    {
        Init();
        SetTransform(tf, spawnTf);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetTransform(Transform objTf, SpawnType spawnTf)
    {
        objTf.localPosition = new Vector3(spawnTf.Pos.x, spawnTf.Pos.y, 0f);
        objTf.localScale = new Vector3(spawnTf.Scale.x, spawnTf.Scale.y, 1f);
        objTf.rotation = spawnTf.Rotation;
    }
}
