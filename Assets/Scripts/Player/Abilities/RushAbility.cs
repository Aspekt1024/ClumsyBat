using UnityEngine;
using System.Collections;

public class RushAbility : MonoBehaviour {

    private AbilityContainer.AbilityType _rushStats;
    private Player _thePlayer;
    private GameUI _gameHud;
    private Coroutine dashRoutine;

    private int _numMothsToRecharge;
    private bool _epicDashEnabled;
    private int _maxCharges;

    private int _numCharges;
    private int _numMoths;
    private bool _bIsRushing;
    private float _cooldownRemaining;

    private const float RushDuration = 0.33f;
    private const float RushSpeed = 15f;
    private const float CooldownDuration = 1.5f;

    private PlayerController.SwipeDirecitons direction;
    
    public void Setup(Player playerRef)
    {
        _rushStats = GameData.Instance.Data.AbilityData.GetDashStats();

        _thePlayer = playerRef;
        _gameHud = FindObjectOfType<GameUI>();

        SetAbilityAttributes();
        SetupHudBar();

        Toolbox.Instance.PlayerDashSpeed = RushSpeed;
        _numCharges = 1;
        _gameHud.SetCooldownTimer(5f);
        _cooldownRemaining = 5f;
    }

    private void Update ()
    {
        if (Toolbox.Instance.GamePaused) return;
        _cooldownRemaining -= Time.deltaTime;
    }

    public void Activate(PlayerController.SwipeDirecitons dir)
    {
        if (!_rushStats.AbilityAvailable) { return; }
        if (_cooldownRemaining > 0) { return; }
        // TODO charges
        
        direction = dir;

        GameData.Instance.Data.Stats.TimesDashed++;
        _gameHud.SetCooldownTimer(CooldownDuration);
        _cooldownRemaining = CooldownDuration;
        if (dashRoutine != null) StopCoroutine(dashRoutine);
        dashRoutine = StartCoroutine(DashSequence());
    }

    public void Deactivate()
    {
        _bIsRushing = false;
        if (dashRoutine != null)
            StopCoroutine(dashRoutine);

        if (GameData.Instance.IsBossLevel())
            _thePlayer.SetPlayerSpeed(0);
        else
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

        SetDashSpeed();

        if (_epicDashEnabled)
        {
            _thePlayer.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Rush);
            // TODO special dash
        }
        else
        {
            _thePlayer.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Rush);
        }

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

        Deactivate();
    }

    private void SetDashSpeed()
    {
        float speed = RushSpeed;
        if (GameData.Instance.IsBossLevel())
        {
            if (_thePlayer.IsFacingRight() && direction == PlayerController.SwipeDirecitons.Left)
                _thePlayer.FaceLeft();
            else if (!_thePlayer.IsFacingRight() && direction == PlayerController.SwipeDirecitons.Right)
                _thePlayer.FaceRight();

            if (direction == PlayerController.SwipeDirecitons.Left)
                speed = -speed;
        }
        _thePlayer.SetPlayerSpeed(speed);
    }
    
    public bool IsActive() { return _bIsRushing; }
    public bool AbilityAvailable() { return _rushStats.AbilityAvailable && _numCharges > 0; }
}
