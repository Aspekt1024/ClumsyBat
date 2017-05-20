﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalAction : BaseAction {

    [Serializable]
    public struct StalSpawnType
    {
        public float delay;
        public float xPosStart;
        public float xPosEnd;
        public int inputIndex;
    }
    public enum Ifaces
    {
        Main, PositionObj,

        Output
    }
    
    public enum StalActions { Spawn, Drop, AltDropFirst, AltSpawnFirst }
    public enum StalSpawnDirection { FromTop, FromBottom }
    public StalActions StalAction;
    public StalSpawnDirection SpawnDirection;
    public List<StalSpawnType> stalSpawns = new List<StalSpawnType>();

    private bool spawnPhase; // TODO To ensure we don't drop before it's spawned...

    private int spawnIndex;
    private bool awaitingDelay;
    private float delayTimer;
    private float delayDuration;

    private SpawnStalactites spawnAbility;
    
    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        spawnAbility = bossBehaviour.GetAbility<SpawnStalactites>();
        spawnPhase = StalAction == StalActions.AltSpawnFirst;
        awaitingDelay = false;
    }

    public override void ActivateBehaviour()
    {
        if (stalSpawns.Count == 0)
        {
            CallNext();
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
        var spawn = stalSpawns[spawnIndex];
        float spawnPosX = 0;
        GameObject posObj = GetInputObj(spawn.inputIndex);
        if (posObj != null)
        {
            DespawnIfProjectile(posObj);
            spawnPosX = posObj.transform.position.x;
        }
        else
        {
            spawnPosX = UnityEngine.Random.Range(spawn.xPosStart, spawn.xPosEnd);
            spawnPosX += GameObject.FindGameObjectWithTag("MainCamera").transform.position.x;
        }

        if (StalAction == StalActions.Spawn)
            spawnAbility.Spawn(spawnPosX, SpawnDirection);
        else if (StalAction == StalActions.Drop)
            spawnAbility.Drop();
        else
        {
            if (spawnPhase)
                spawnAbility.Spawn(spawnPosX, SpawnDirection);
            else
                spawnAbility.Drop();
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

    private void SpawnsComplete()
    {
        spawnPhase = !spawnPhase;
        CallNext();
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
            if (conn.ID == connID)
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
