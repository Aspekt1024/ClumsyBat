using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    private int _shieldCharges;
    private const int MaxCharges = 1;
    private bool _bPaused;

    private Player _thePlayer;
    private Rigidbody2D _playerBody;
    private GameHandler _gameHandler;
    //private StatsHandler Stats;

    private enum ShieldStates
    {
        Idle,
        Activated
    }
    private ShieldStates _state = ShieldStates.Idle;

    public void ConsumeCharge()
    {
        if (_state == ShieldStates.Idle && _shieldCharges > 0)
        {
            Toolbox.MainAudio.PlaySound(Toolbox.MainAudio.Shield);
            _shieldCharges--;
            StartCoroutine("ShieldUp");
        }
        else if (_playerBody.position.y < 0f)
        {
            _playerBody.velocity = new Vector2(_playerBody.velocity.x, 4f);
        }
    }

    private IEnumerator ShieldUp()
    {
        _state = ShieldStates.Activated;
        _thePlayer.GetRenderer().color = new Color(0.6f, 0.6f, 1f);
        // TODO animations
        // PlayerAnim.Play("Knockback", 0, 0f);
        yield return StartCoroutine("Knockback");

        _thePlayer.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Hover);
        yield return StartCoroutine("MoveForward");

        _state = ShieldStates.Idle;
        _thePlayer.GetRenderer().color = new Color(1f, 1f, 1f);
    }

    private IEnumerator Knockback()
    {
        float knockbackTimer = 0f;
        const float knockbackDuration = 0.3f;

        if (!_thePlayer.AtCaveEnd()) { _playerBody.velocity = new Vector2(-7f, _playerBody.velocity.y); }
        
        while (knockbackTimer < knockbackDuration)
        {
            if (!_bPaused)
            {
                float displacementX = _playerBody.position.x - _thePlayer.GetHomePositionX();
                if (displacementX < -1f)
                {
                    _playerBody.position -= new Vector2(displacementX + 1f, 0f);
                }

                if (!_thePlayer.AtCaveEnd())
                {
                    if (displacementX > 0f)
                    {
                        _playerBody.position -= new Vector2(displacementX, 0f);
                    }
                    _playerBody.GetComponent<Player>().SetPlayerSpeed(-4f);     // TODO set knockback speed;
                }
                knockbackTimer += Time.deltaTime;
            }
            yield return null;
        }
    }

    private IEnumerator MoveForward()
    {
        float moveForwardTimer = 0f;
        const float moveForwardDuration = 0.4f;
        float playerStartX = _playerBody.position.x;
        float playerEndX = _thePlayer.GetHomePositionX();

        while (moveForwardTimer < moveForwardDuration && !_thePlayer.AtCaveEnd())
        {
            if (!_bPaused && _thePlayer.IsAlive())
            {
                moveForwardTimer += Time.deltaTime;
                _playerBody.position = new Vector2(playerStartX - (playerStartX - playerEndX) * (moveForwardTimer / moveForwardDuration), _playerBody.position.y);
            }
            yield return null;
        }
    }

    public void Setup(Player playerRef, Lantern lanternRef)
    {
        //ShieldStats = GameData.Instance.Data.AbilityData.GetShieldStats(); // TODO Save Shield Stats
        //SetAbilityAttributes();

        _thePlayer = playerRef;
        _playerBody = _thePlayer.GetComponent<Rigidbody2D>();
        _gameHandler = _thePlayer.GetGameHandler();

    }

    public void GamePaused(bool bGamePaused) { _bPaused = bGamePaused; }
    public bool IsAvailable() { return (_shieldCharges > 0 || _state == ShieldStates.Activated); }
    public void AddCharge() { if (_shieldCharges < MaxCharges) { _shieldCharges++; } }
    public bool IsInUse() { return _state == ShieldStates.Activated; }

}
