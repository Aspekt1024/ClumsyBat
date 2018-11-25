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
        GetNPCComponents();
    }
    
    public void Spawn(SpawnType spawnTf, NpcTypes type)
    {
        base.Spawn(transform, spawnTf);
        // TODO set sprite based on NPC type
    }

    protected override void Init()
    {

    }

    private void GetNPCComponents() { }
}
