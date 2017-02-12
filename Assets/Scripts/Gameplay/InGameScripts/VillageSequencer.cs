using System.Collections;
using UnityEngine;

public class VillageSequencer : MonoBehaviour
{
    private struct PlayerProps
    {
        public Player Player;
        public PlayerController Controller;
        public Rigidbody2D Body;
    }
    private PlayerProps _player;
    private GameHandler _gameHandler;

    private bool _bPausedForSpeech;

    private void OnEnable() { EventListener.OnTooltipActioned += OnTooltipActioned; }
    private void OnDisable() { EventListener.OnTooltipActioned -= OnTooltipActioned; }

    private void Start()
    {
        _player.Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _player.Controller = _player.Player.GetComponent<PlayerController>();
        _player.Body = _player.Player.GetComponent<Rigidbody2D>();
        _gameHandler = GameObject.FindGameObjectWithTag("Scripts").GetComponent<GameHandler>();
    }

    public IEnumerator StartSequence()
    {
        _player.Controller.PauseInput(true);
        _player.Body.isKinematic = true;
        _player.Body.velocity = Vector2.zero;
        _player.Player.Fog.ExpandToRemove();
        
        yield return StartCoroutine("MoveToPedestal");
        yield return StartCoroutine("VillageSpeech");
        yield return new WaitForSeconds(1.5f);
        _gameHandler.LevelComplete();
    }

    private IEnumerator VillageSpeech()
    {
        TooltipHandler tth = FindObjectOfType<TooltipHandler>();
        if (GameData.Instance.Level == LevelProgressionHandler.Levels.Main2)
        {
            TooltipHandler.DialogueId eventId = TooltipHandler.DialogueId.HypersonicVillagePt1;
            _bPausedForSpeech = true;
            tth.ShowDialogue(eventId, TooltipHandler.WaitType.VillageSpeech);
            while (_bPausedForSpeech)
            {
                yield return null;
            }

            // TODO here's hypersonic sequence
            // TODO move this to the hypersonic ability class?
            var hypersonic = GameData.Instance.Data.AbilityData.GetHypersonicStats();
            hypersonic.AbilityUnlocked = true;
            hypersonic.AbilityAvailable = true;
            hypersonic.AbilityLevel = 1;
            hypersonic.AbilityEvolution = 1;
            GameData.Instance.Data.AbilityData.SaveHypersonicStats(hypersonic);

            yield return new WaitForSeconds(0.5f);
            _player.Player.ForceHypersonic();
            yield return new WaitForSeconds(1f);
            
            eventId = TooltipHandler.DialogueId.HypersonicVillagePt2;
            _bPausedForSpeech = true;
            tth.ShowDialogue(eventId, TooltipHandler.WaitType.VillageSpeech);
            while (_bPausedForSpeech)
            {
                yield return null;
            }
        }
    }

    private void OnTooltipActioned()
    {
        _bPausedForSpeech = false;
    }

    private IEnumerator MoveToPedestal()
    {
        var animTimer = 0f;
        const float animDuration = 0.7f;

        var startPos = _player.Body.position;
        var endPos = new Vector2(-3f, -0.7f);

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            _player.Body.position = startPos - (startPos - endPos) * (animTimer / animDuration);
            yield return null;
        }
        // TODO perch (after clumsy has an animator controller
    }
}
