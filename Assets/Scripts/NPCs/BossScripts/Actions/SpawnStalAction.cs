using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStalAction : BaseAction {
    
    public enum StalActions
    {
        Spawn, Drop, Alternate
    }
    public StalActions StalAction;

    private bool spawnPhase;

    private SpawnStalactites spawnAbility;
    
    public override void GameSetup(BossDataContainer owningContainer, BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
        spawnAbility = (SpawnStalactites)bossBehaviour.GetAbility<SpawnStalactites>();
        spawnPhase = true;
    }

    public override void ActivateBehaviour()
    {
        if (StalAction == StalActions.Spawn)
            spawnAbility.Spawn();
        else if (StalAction == StalActions.Drop)
            spawnAbility.Drop();
        else if (StalAction == StalActions.Alternate)
        {
            if (spawnPhase)
                spawnAbility.Spawn();
            else
                spawnAbility.Drop();

            spawnPhase = !spawnPhase;
        }

        CallNext();
    }
}
