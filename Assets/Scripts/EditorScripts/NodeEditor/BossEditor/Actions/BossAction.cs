using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAction : BaseAction {

    protected override void ActivateBehaviour()
    {
        // Do nothing
    }

    public override GameObject GetObject(int id)
    {
        return GameObject.FindGameObjectWithTag("Boss");
    }

    public override Vector2 GetPosition(int id)
    {
        return boss.GetComponent<Boss>().Body.position;
    }
}
