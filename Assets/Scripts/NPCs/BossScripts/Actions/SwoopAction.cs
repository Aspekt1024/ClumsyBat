using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwoopAction : BaseAction {

    public override void GameSetup(StateMachine owningContainer, BossData behaviour, GameObject bossReference)
    {
        base.GameSetup(owningContainer, behaviour, bossReference);
    }

    public override void ActivateBehaviour()
    {
        throw new NotImplementedException();
    }
}
