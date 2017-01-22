using UnityEngine;
using System.Collections;

using GameStates = GameHandler.GameStates;
using StoryEventID = StoryEventControl.StoryEvents;

public class MothInteractivity : MonoBehaviour
{
    private GameHandler _gameHandler;
    private Player _thePlayer;

    private void Awake()
    {
        _gameHandler = FindObjectOfType<GameHandler>();
        _thePlayer = FindObjectOfType<Player>();
    }
    
    public IEnumerator ConsumeMoth(float animationWaitTime)
    {
        if (_thePlayer.Stats.MothsEaten > _thePlayer.Stats.MostMoths)
        {
            _thePlayer.Stats.MostMoths++;
        }
        _thePlayer.Stats.TotalMoths++;
        float animTimer = 0f;
        while (animTimer < animationWaitTime)
        {
            if (_gameHandler.GetGameState() != GameStates.Paused)
            {
                animTimer += Time.deltaTime;
            }
            yield return null;
        }
        _thePlayer.Stats.MothsEaten++;
    }

    public void ActivateAbility(Moth.MothColour colour)
    {
        switch (colour)
        {
            case Moth.MothColour.Green:
                _thePlayer.Lantern.ChangeColour(Lantern.LanternColour.Green);
                _thePlayer.Fog.Echolocate();
                break;
            case Moth.MothColour.Gold:
                _thePlayer.Lantern.ChangeColour(Lantern.LanternColour.Gold);
                _thePlayer.Stats.StoryData.TriggerEvent(StoryEventID.FirstGoldMoth);
                _thePlayer.ActivateHypersonic();
                _thePlayer.Fog.Echolocate();
                break;
            case Moth.MothColour.Blue:
                _thePlayer.Lantern.ChangeColour(Lantern.LanternColour.Blue);
                _thePlayer.Fog.Echolocate();
                break;
        }

        _thePlayer.AddShieldCharge();
    }
}
