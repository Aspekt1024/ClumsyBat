﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAction : BaseAction {

    public override void ActivateBehaviour()
    {
        // Do nothing
    }

    public override GameObject GetObject(int id)
    {
        return GameObject.FindGameObjectWithTag("Boss");
    }
}