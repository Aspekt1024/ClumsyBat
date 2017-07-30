using UnityEngine;
using System.Collections;

using GameStates = GameHandler.GameStates;
using StoryEventID = StoryEventControl.StoryEvents;

public class MothInteractivity : MonoBehaviour
{
    private GameHandler _gameHandler;
    private Player _thePlayer;
    private DataHandler _data;

    private void Awake()
    {
        _data = GameData.Instance.Data;
        _gameHandler = FindObjectOfType<GameHandler>();
        _thePlayer = FindObjectOfType<Player>();
    }
    
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
                // TODO some colour
                break;
        }

        _thePlayer.AddShieldCharge();
        _thePlayer.Fog.Echolocate();
    }
}
