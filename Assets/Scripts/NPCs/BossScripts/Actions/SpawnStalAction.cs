using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalAction : BaseAction {
    
    public enum Inputs
    {
        Main,
        PositionObj
    }

    public enum StalActions { Spawn, Drop, Alternate }
    public enum StalSpawnDirection { FromTop, FromBottom }
    public StalActions StalAction;
    public StalSpawnDirection SpawnDirection;

    private bool spawnPhase;

    private SpawnStalactites spawnAbility;
    
    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        spawnAbility = bossBehaviour.GetAbility<SpawnStalactites>();
        spawnPhase = true;
    }

    public override void ActivateBehaviour()
    {
        float spawnPosX = 0;
        GameObject posObj = GetInputObj((int)Inputs.PositionObj);
        if (posObj != null)
        {
            DespawnIfProjectile(posObj);
            spawnPosX = posObj.transform.position.x;
        }
        else
            spawnPosX = GameObject.FindGameObjectWithTag("Player").transform.position.x;
            

        if (StalAction == StalActions.Spawn)
            spawnAbility.Spawn(spawnPosX, SpawnDirection);
        else if (StalAction == StalActions.Drop)
            spawnAbility.Drop();
        else if (StalAction == StalActions.Alternate)
        {
            if (spawnPhase)
                spawnAbility.Spawn(spawnPosX, SpawnDirection);
            else
                spawnAbility.Drop();

            spawnPhase = !spawnPhase;
        }
        CallNext();
    }

    private GameObject GetInputObj(int inputId)
    {
        GameObject obj = null;
        foreach(var input in inputs)
        {
            if (input.identifier == inputId)
            {
                obj = input.connectedAction.GetObject(input.connectedInterfaceIndex);
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
