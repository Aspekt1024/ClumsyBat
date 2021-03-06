﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalAction : BaseAction {

    [Serializable]
    public struct StalSpawnType
    {
        public float delay;
        public int xPosIndexLower;
        public int xPosIndexUpper;
        public int inputID;
    }
    public enum Ifaces
    {
        Input, Output,
        GreenChance, GoldChance, BlueChance
    }
    
    public enum StalActions { Spawn, DropAll, AltDropFirst, AltSpawnFirst }
    public enum StalSpawnDirection { FromTop, FromBottom }
    public enum StalTypes { Stalactite, Crystal }

    public StalTypes StalType;
    public StalActions StalAction;
    public StalSpawnDirection SpawnDirection;
    public List<StalSpawnType> stalSpawns = new List<StalSpawnType>();
    
    public float GreenChance;
    public float GoldChance;
    public float BlueChance;

    private bool spawnPhase;
    private int[] spawnedStalPoolIndexes;
    private int[] spawnedStalPositionIndexes;

    private int spawnIndex;
    private bool awaitingDelay;
    private float delayTimer;
    private float delayDuration;

    private SpawnStalactites spawnAbility;
    private StalBossHandler bossStalHandler;
    
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
        bossStalHandler = GameObject.FindObjectOfType<StalBossHandler>();
        if (bossStalHandler == null)
            bossStalHandler = GameObject.FindGameObjectWithTag("Scripts").AddComponent<StalBossHandler>();
    }

    protected override void ActivateBehaviour()
    {
        if (spawnedStalPoolIndexes == null)
        {
            spawnedStalPoolIndexes = new int[stalSpawns.Count];
            spawnedStalPositionIndexes = new int[stalSpawns.Count];
        }

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
            Spawn();
        }
        else if (StalAction == StalActions.DropAll)
        {
            bossStalHandler.ClearTopStals(spawnedStalPositionIndexes);
            spawnAbility.DropAllStalactites();
        }
        else
        {
            if (spawnPhase)
            {
                Spawn();
            }
            else
            {
                bossStalHandler.ClearTopStals(spawnedStalPositionIndexes);
                spawnAbility.Drop(spawnedStalPoolIndexes);
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

    private void Spawn()
    {
        float spawnPos = GetSpawnPos();
        spawnedStalPoolIndexes[spawnIndex] = spawnAbility.Spawn(spawnPos, SpawnDirection, StalType, GreenChance, GoldChance, BlueChance, spawnedStalPositionIndexes[spawnIndex]);
    }

    private float GetSpawnPos()
    {
        GameObject posObj = GetInputObj(stalSpawns[spawnIndex].inputID);
        if (posObj != null)
        {
            DespawnIfProjectile(posObj);
            return posObj.transform.position.x;
        }
        else
        {
            int stalIndex = bossStalHandler.GetFreeTopStalIndex(stalSpawns[spawnIndex].xPosIndexLower, stalSpawns[spawnIndex].xPosIndexUpper);
            spawnedStalPositionIndexes[spawnIndex] = stalIndex;
            return bossStalHandler.ConvertIndexToPosition(stalIndex);
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
        if (!awaitingDelay || !IsActive) return;
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
