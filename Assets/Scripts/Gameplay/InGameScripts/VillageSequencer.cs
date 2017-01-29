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
        yield return new WaitForSeconds(1.5f);
        _gameHandler.LevelComplete();
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
    }
}
