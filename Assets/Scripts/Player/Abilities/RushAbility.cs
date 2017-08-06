using UnityEngine;
using System.Collections;

public class RushAbility : MonoBehaviour {

    private AbilityContainer.AbilityType _rushStats;
    private Player _thePlayer;
    private Rigidbody2D _playerBody;
    private Lantern _lantern;
    private GameUI _gameHud;

    private int _numMothsToRecharge;
    private bool _epicDashEnabled;
    private int _maxCharges;

    private int _numCharges;
    private int _numMoths;
    private bool _bIsRushing;
    private float _cooldownRemaining;

    private const float RushDuration = 0.33f;
    private const float RushSpeed = 15f;
    private const float NormalSpeed = 1f;
    private const float CooldownDuration = 1.5f;

    private Coroutine dashRoutine;
    
    public void Setup(Player playerRef, Lantern lanternRef)
    {
        _rushStats = GameData.Instance.Data.AbilityData.GetDashStats();

        _thePlayer = playerRef;
        _lantern = lanternRef;
        _playerBody = _thePlayer.GetComponent<Rigidbody2D>();
        _gameHud = FindObjectOfType<GameUI>();

        SetAbilityAttributes();
        SetupHudBar();

        Toolbox.Instance.PlayerDashSpeed = RushSpeed;
    }

    private void Update ()
    {
        if (Toolbox.Instance.GamePaused) { return; }

        _cooldownRemaining -= Time.deltaTime;
        _gameHud.SetCooldown(1f - Mathf.Clamp(_cooldownRemaining / CooldownDuration, 0f, 1f));

    }

    public void Activate()
    {
        if (!_rushStats.AbilityAvailable) { return; }
        if (_cooldownRemaining > 0) { return; }
        // TODO charges
        
        GameData.Instance.Data.Stats.TimesDashed++;
        _cooldownRemaining = CooldownDuration;
        dashRoutine = StartCoroutine(DashSequence());
    }

    public void Deactivate()
    {
        StopCoroutine(dashRoutine);
        _thePlayer.SetPlayerSpeed(Toolbox.Instance.PlayerSpeed);
        _thePlayer.SetGravity(Toolbox.Instance.GravityScale);
    }

    private void SetAbilityAttributes()
    {
        _numMoths = 0;
        _numMothsToRecharge = 1;
        _maxCharges = 2;
        _epicDashEnabled = false;

        if (_rushStats.AbilityLevel >= 2) { _maxCharges = 3; }
        if (_rushStats.AbilityLevel >= 3) { _maxCharges = 4; }
        if (_rushStats.AbilityLevel >= 4) { _maxCharges = 5; }
        if (_rushStats.AbilityLevel >= 5) { _maxCharges = 6; }
        if (_rushStats.AbilityEvolution == 2) { _epicDashEnabled = true; }
    }

    public void AddCharge()
    {
        _numMoths++;
        if (_numMoths == _numMothsToRecharge)
        {
            if (_numCharges < _maxCharges)
            {
                _numCharges++;
                _numMoths = 0;
            }
            else
            {
                _numMoths = _numMothsToRecharge - 1;
            }
        }
        _gameHud.SetCooldown((float)_numMoths / _numMothsToRecharge);
    }

    private void SetupHudBar()
    {
        _gameHud.ShowCooldown(_rushStats.AbilityAvailable);
    }

    private IEnumerator DashSequence()
    {
        _bIsRushing = true;
        _thePlayer.SetGravity(0f);
        _thePlayer.SetVelocity(Vector2.zero);
        _thePlayer.SetPlayerSpeed(RushSpeed);
        if (_epicDashEnabled)
        {
            _thePlayer.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Rush);
            // TODO special dash
        }
        else
        {
            _thePlayer.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Rush);
        }
        //_lantern.AddRushForce();

        float timer = 0f;

        while (timer < RushDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                GameData.Instance.Data.Stats.DashDistance += Time.deltaTime * RushSpeed;
            }
            yield return null;
        }

        _thePlayer.SetGravity(Toolbox.Instance.GravityScale);
        _thePlayer.SetPlayerSpeed(Toolbox.Instance.PlayerSpeed);
    }
    
    public bool IsActive() { return _bIsRushing; }
    public bool AbilityAvailable() { return _rushStats.AbilityAvailable && _numCharges > 0; }
}
