using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalAction : BaseAction {

    [Serializable]
    public struct StalSpawnType
    {
        public float delay;
        public float xPosStart;
        public float xPosEnd;
        public int inputID;
    }
    public enum Ifaces
    {
        Input, Output,
        GreenChance, GoldChance, BlueChance
    }
    
    public enum StalActions { Spawn, Drop, AltDropFirst, AltSpawnFirst }
    public enum StalSpawnDirection { FromTop, FromBottom }
    public enum StalTypes { Stalactite, Crystal }

    public StalTypes StalType;
    public StalActions StalAction;
    public StalSpawnDirection SpawnDirection;
    public List<StalSpawnType> stalSpawns = new List<StalSpawnType>();

    public float GreenChance;
    public float GoldChance;
    public float BlueChance;

    private bool spawnPhase; // TODO To ensure we don't drop before it's spawned...

    private int spawnIndex;
    private bool awaitingDelay;
    private float delayTimer;
    private float delayDuration;

    private SpawnStalactites spawnAbility;
    private StalBossHandler bossStals;
    
    public override void GameSetup(BehaviourSet behaviourSet, BossData bossData, GameObject bossReference)
    {
        base.GameSetup(behaviourSet, bossData, bossReference);
        spawnAbility = base.bossData.GetAbility<SpawnStalactites>();
        spawnPhase = StalAction == StalActions.AltSpawnFirst;
        awaitingDelay = false;
        GetBossStals();
    }

    private void GetBossStals()
    {
        bossStals = GameObject.FindObjectOfType<StalBossHandler>();
        if (bossStals == null)
            bossStals = GameObject.FindGameObjectWithTag("Scripts").AddComponent<StalBossHandler>();
    }

    public override void ActivateBehaviour()
    {
        if (stalSpawns.Count == 0)
        {
            IsActive = false;
            CallNext((int)Ifaces.Output);
        }
        else
        {
            SetupStalSpawn(0);
        }
    }

    private void SetupStalSpawn(int index)
    {
        spawnIndex = index;
        delayDuration = stalSpawns[spawnIndex].delay;
        delayTimer = 0f;
        awaitingDelay = true;
    }

    private void SpawnStal()
    {
        if (StalAction == StalActions.Spawn)
        {
            spawnAbility.Spawn(GetSpawnPosition(), SpawnDirection, StalType, GreenChance, GoldChance, BlueChance);
        }
        else if (StalAction == StalActions.Drop)
        {
            bossStals.ClearTopStals();
            spawnAbility.Drop();
        }
        else
        {
            if (spawnPhase)
            {
                spawnAbility.Spawn(GetSpawnPosition(), SpawnDirection, StalType, GreenChance, GoldChance, BlueChance);
            }
            else
            {
                bossStals.ClearTopStals();
                spawnAbility.Drop();
            }
        }
        
        if (spawnIndex == stalSpawns.Count - 1)
        {
            SpawnsComplete();
        }
        else
        {
            SetupStalSpawn(spawnIndex + 1);
        }
    }

    private float GetSpawnPosition()
    {
        GameObject posObj = GetInputObj(stalSpawns[spawnIndex].inputID);
        if (posObj != null)
        {
            DespawnIfProjectile(posObj);
            return posObj.transform.position.x;
        }
        else
        {
            //spawnPosX = UnityEngine.Random.Range(spawn.xPosStart, spawn.xPosEnd);
            //spawnPosX += GameObject.FindGameObjectWithTag("MainCamera").transform.position.x;
            return bossStals.GetFreeTopStalXPos();
        }
    }

    private void SpawnsComplete()
    {
        IsActive = false;
        spawnPhase = !spawnPhase;
        CallNext((int)Ifaces.Output);
    }

    public override void Tick(float deltaTime)
    {
        if (!awaitingDelay) return;
        delayTimer += deltaTime;
        if (delayTimer > delayDuration)
        {
            awaitingDelay = false;
            SpawnStal();
        }
    }

    private GameObject GetInputObj(int connID)
    {
        GameObject obj = null;
        foreach(var conn in connections)
        {
            if (conn.ID == connID && conn.IsConnected())
            {
                obj = conn.ConnectedInterface.Action.GetObject(conn.ConnectedInterface.ID);
                break;
            }
        }
        return obj;
    }

    private void DespawnIfProjectile(GameObject obj)
    {
        Projectile proj = obj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.DespawnToEarth();
        }
    }
}
