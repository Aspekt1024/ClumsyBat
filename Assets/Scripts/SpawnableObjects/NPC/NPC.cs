using UnityEngine;

public class NPC : Spawnable {

    public enum NpcTypes
    {
        Nomee,
    }

    // Editor properties
    public NpcTypes Type;

    private void Awake()
    {
        IsActive = false;
        GetNPCComponents();
    }

    private void FixedUpdate()
    {
        if (!IsActive || IsPaused) { return; }
        MoveLeft(Time.fixedDeltaTime);
    }

    public void Activate(SpawnType spawnTf, NpcTypes type)
    {
        base.Activate(transform, spawnTf);
        // TODO set sprite based on NPC type
    }

    private void GetNPCComponents() { }
}
