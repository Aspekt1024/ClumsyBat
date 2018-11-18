using ClumsyBat;
using UnityEngine;

public class PlayerAction : BaseAction {

    protected override void ActivateBehaviour()
    {
        // Do nothing
    }

    public override GameObject GetObject(int id)
    {
        return GameStatics.Player.Clumsy.gameObject;
    }

    public override Vector2 GetPosition(int id)
    {
        return GameStatics.Player.Clumsy.transform.position;
    }
}
