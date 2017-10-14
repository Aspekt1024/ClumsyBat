using UnityEngine;
using System.Collections;

using GameStates = GameHandler.GameStates;

// TODO remove this class - replaced and handled by Lantern
public class MothInteractivity : MonoBehaviour
{
    private Player _thePlayer;

    private void Awake()
    {
        _thePlayer = FindObjectOfType<Player>();
    }
    
    // TODO remove this;
    public void ActivateAbility(Moth.MothColour colour)
    {
        switch (colour)
        {
            case Moth.MothColour.Green:
                break;
            case Moth.MothColour.Gold:
                _thePlayer.ActivateHypersonic();
                break;
            case Moth.MothColour.Blue:
                // TODO some function
                break;
        }

        _thePlayer.AddShieldCharge();
        _thePlayer.AddDashCharge();
        _thePlayer.Fog.Echolocate();
    }
}
