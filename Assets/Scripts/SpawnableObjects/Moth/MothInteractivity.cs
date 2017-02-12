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
    
    public IEnumerator ConsumeMoth(float animationWaitTime)
    {
        if (_data.Stats.MothsEaten > _data.Stats.MostMoths)
        {
            _data.Stats.MostMoths++;
        }
        _data.Stats.TotalMoths++;
        float animTimer = 0f;
        while (animTimer < animationWaitTime)
        {
            if (_gameHandler.GetGameState() != GameStates.Paused)
            {
                animTimer += Time.deltaTime;
            }
            yield return null;
        }
        _data.Stats.MothsEaten++;
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
                _data.StoryData.TriggerEvent(StoryEventID.FirstGoldMoth);
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
