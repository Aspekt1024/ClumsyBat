﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : BaseAction {

    public override void ActivateBehaviour()
    {
        // Do nothing
    }

    public override GameObject GetObject(int id)
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public override Vector2 GetPosition(int id)
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }
}
