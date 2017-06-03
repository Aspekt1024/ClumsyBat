﻿using UnityEngine;

public abstract class Spawnable : MonoBehaviour {
    public bool IsPaused;
    public bool IsActive;
    protected float Speed;
    
    public struct SpawnType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
    }

    public void SetSpeed(float speed){ Speed = speed; }
    public virtual void PauseGame(bool gamePaused) { IsPaused = gamePaused; }
    
    public virtual void SendToInactivePool()
    {
        IsActive = false;
        ResetObjectState();
        transform.position = Toolbox.Instance.HoldingArea;
    }

    protected virtual void ResetObjectState()
    {
        transform.localRotation = new Quaternion();
    }

    public virtual void Activate(Transform tf, SpawnType spawnTf)
    {
        SetTransform(tf, spawnTf);
        IsActive = true;
    }
    
    public void SetTransform(Transform objTf, SpawnType spawnTf)
    {
        objTf.localPosition = new Vector3(spawnTf.Pos.x, spawnTf.Pos.y, 0f);
        objTf.localScale = spawnTf.Scale;
        objTf.rotation = spawnTf.Rotation;
    }

    protected void MoveLeft(float time) { transform.position += Vector3.left * time * Speed; }
    protected void ActivateSpawnable(Vector3 position) { transform.localPosition = position; }
}
