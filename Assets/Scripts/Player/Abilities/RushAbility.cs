using UnityEngine;
using System.Collections;

public class RushAbility : MonoBehaviour {

    private AbilityContainer.AbilityType _rushStats;
    private Player _thePlayer;
    private Rigidbody2D _playerBody;
    private Lantern _lantern;
    private GameUI _gameHud;
    private GameHandler _gameHandler;

    private int _numMothsToRecharge;
    private bool _epicDashEnabled;
    private int _maxCharges;

    private int _numCharges;
    private int _numMoths;
    private bool _bIsRushing;
    private bool _bDisabled;
    private float _rushTimeRemaining;
    private float _cooldownRemaining;

    private const float RushDuration = 0.26f;
    private const float RushSpeed = 7f;
    private const float NormalSpeed = 1f;
    private const float CooldownDuration = 1.5f;

    private bool _bPaused;
    private bool _bAtCaveEnd;
    
    public void Setup(Player playerRef, Lantern lanternRef)
    {
        _rushStats = GameData.Instance.Data.AbilityData.GetRushStats();

        _thePlayer = playerRef;
        _lantern = lanternRef;
        _playerBody = _thePlayer.GetComponent<Rigidbody2D>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameHandler = FindObjectOfType<GameHandler>();

        SetAbilityAttributes();
        SetupHudBar();
    }

    private void Update ()
    {
        if (_bPaused) { return; }

        _cooldownRemaining -= Time.deltaTime;
        _rushTimeRemaining -= Time.deltaTime;
        _gameHud.SetCooldown(1f - Mathf.Clamp(_cooldownRemaining / CooldownDuration, 0f, 1f));

        if (_bDisabled)
        {
            _rushTimeRemaining = 0f;
            _bIsRushing = false;
        }

        if (_bIsRushing)
        {
            GameData.Instance.Data.Stats.DashDistance += Time.deltaTime * RushSpeed * Toolbox.Instance.LevelSpeed;
            if (_rushTimeRemaining <= 0f)
            {
                StartCoroutine("RushEndAnimation");
            }
        }
    }

    public void Activate()
    {
        if (!_rushStats.AbilityAvailable) { return; }
        if (_cooldownRemaining > 0) { return; }
        // TODO charges

        _bDisabled = false;
        GameData.Instance.Data.Stats.TimesDashed++;
        _cooldownRemaining = CooldownDuration;
        StartCoroutine("RushStartAnimation");
    }

    public void Deactivate()
    {
        _bDisabled = true;   // Sets the flag for the coroutine to deactivate
    }

    private void SetAbilityAttributes()
    {
        _numMoths = 0;
        _numMothsToRecharge = 3;
        _maxCharges = 1;
        _epicDashEnabled = false;

        if (_rushStats.AbilityLevel >= 2) { _numMothsToRecharge = 2; }
        if (_rushStats.AbilityLevel >= 3) { _maxCharges = 2; }
        if (_rushStats.AbilityLevel >= 4) { _numMothsToRecharge = 1; }
        if (_rushStats.AbilityLevel >= 5) { _maxCharges = 3; }
        if (_rushStats.AbilityEvolution == 2) { _epicDashEnabled = true; }
    }

    public void MothConsumed()
    {
        _numMoths++;
        if (_numMoths == _numMothsToRecharge)
        {
            _numCharges = Mathf.Clamp(_numCharges++, 0, _maxCharges);
            _numMoths = (_numCharges == _maxCharges ? 1 : 0);
        }
        _gameHud.SetCooldown((float)_numMoths / _numMothsToRecharge);
    }

    private void SetupHudBar()
    {
        _gameHud.ShowCooldown(_rushStats.AbilityAvailable);
    }

    public void GamePaused(bool paused)
    {
        _bPaused = paused;
    }

    private IEnumerator RushStartAnimation()
    {
        _bIsRushing = true;
        _rushTimeRemaining = RushDuration;
        _thePlayer.SetGravity(0f);
        _thePlayer.SetVelocity(new Vector2(8f, 0f));
        if (_epicDashEnabled)
        {
            _thePlayer.SetAnimation("Rush"); // TODO special dash
        }
        else
        {
            _thePlayer.SetAnimation("Rush");
        }
        _lantern.AddRushForce();

        const float startupDuration = 0.07f;
        float animTimer = 0f;
        while (animTimer < startupDuration && !_bDisabled)
        {
            if (!_bPaused)
            {
                animTimer += Time.deltaTime;
            }
            yield return null;
        }

        if (_bPaused) yield break;
        _thePlayer.SetPlayerSpeed(RushSpeed);
        _thePlayer.SetVelocity(Vector2.zero);
        _gameHandler.UpdateGameSpeed(RushSpeed);
    }

    private IEnumerator RushEndAnimation()
    {
        _thePlayer.SetPlayerSpeed(NormalSpeed);
        _gameHandler.UpdateGameSpeed(NormalSpeed);
        _bIsRushing = false;
        _thePlayer.SetGravity(-1f);  // -1 resets to default gravity defined by PlayerController

        while (_playerBody.position.x > Toolbox.PlayerStartX && !_bAtCaveEnd && !_bDisabled)
        {
            if (!_bPaused)
            {
                float newSpeed = 3 - (2 * _playerBody.position.x / (_playerBody.position.x + Toolbox.PlayerStartX));
                _thePlayer.SetVelocity(new Vector2(-newSpeed, _playerBody.velocity.y));
            }
            yield return null;
        }
        if (!_bDisabled)
        {
            _thePlayer.SetVelocity(new Vector2(0, _playerBody.velocity.y));
            _thePlayer.SetAnimation("Flap");
        }
    }

    public void CaveEndReached() { _bAtCaveEnd = true; }
    public bool IsActive() { return !_bDisabled; }
    public bool AbilityAvailable() { return _rushStats.AbilityAvailable && _numCharges > 0; }
}
