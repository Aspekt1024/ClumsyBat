using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    private int _shieldCharges;
    private const int MaxCharges = 2;

    private Player _thePlayer;
    private Rigidbody2D _playerBody;
    private GameHandler _gameHandler;
    private Lantern lantern;
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
            lantern.SetColourFromShieldCharges(_shieldCharges);
            StartCoroutine(ShieldUp());
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
        yield return StartCoroutine(Knockback());

        _thePlayer.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Hover);

        _state = ShieldStates.Idle;
        _thePlayer.GetRenderer().color = new Color(1f, 1f, 1f);
    }

    private IEnumerator Knockback()
    {
        float knockbackTimer = 0f;
        const float knockbackDuration = 0.7f;

        float directionModifier = _thePlayer.IsFacingRight() ? -1f : 1f;

        while (knockbackTimer < knockbackDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                knockbackTimer += Time.deltaTime;
                _playerBody.velocity = new Vector2(Mathf.Lerp(directionModifier * 7f, 0f, knockbackTimer/knockbackDuration), _playerBody.velocity.y);
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
        lantern = lanternRef;

        lantern.SetColourFromShieldCharges(_shieldCharges);
    }

    public void AddCharge()
    {
        if (_shieldCharges < MaxCharges)
        {
            _shieldCharges++;
            lantern.SetColourFromShieldCharges(_shieldCharges);
        }
    }

    public bool IsAvailable() { return (_shieldCharges > 0 || _state == ShieldStates.Activated); }
    public bool IsInUse() { return _state == ShieldStates.Activated; }

}
