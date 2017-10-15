using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : BaseAction {

    protected override void ActivateBehaviour()
    {
        // Do nothing
    }

    public override GameObject GetObject(int id)
    {
        return Toolbox.Player.gameObject;
    }

    public override Vector2 GetPosition(int id)
    {
        return Toolbox.Player.transform.position;
    }
}
