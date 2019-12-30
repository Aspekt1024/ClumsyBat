using UnityEngine;
using System.Collections;
using ClumsyBat;
using ClumsyBat.Controllers;
using ClumsyBat.DataContainers;
using ClumsyBat.Players;

public class RushAbility : MonoBehaviour {

    public bool IsEnabled { get; private set; }
    public float CooldownRemaining { get; private set; }
    public event System.Action OnRushActivation = delegate { };
    
    private Player player;
    private Coroutine dashRoutine;

    private int numMothsToRecharge;
    private bool epicDashEnabled;
    private int maxCharges;

    private int numCharges;
    private int numMoths;

    private const float RushDuration = 0.33f;
    private const float RushSpeed = 15f;
    private const float CooldownDuration = 1.5f;
    
    public void Setup(Player playerRef)
    {
        player = playerRef;
        
        Toolbox.Instance.PlayerDashSpeed = RushSpeed;
        numCharges = 1;
        CooldownRemaining = 5f;
    }

    public void SetStats(AbilityContainer.AbilityType stats)
    {
        IsEnabled = stats.AbilityAvailable;
        numMoths = 0;
        numMothsToRecharge = 1;
        maxCharges = 2;
        epicDashEnabled = false;

        if (stats.AbilityLevel >= 2) { maxCharges = 3; }
        if (stats.AbilityLevel >= 3) { maxCharges = 4; }
        if (stats.AbilityLevel >= 4) { maxCharges = 5; }
        if (stats.AbilityLevel >= 5) { maxCharges = 6; }
        if (stats.AbilityEvolution == 2) { epicDashEnabled = true; }
    }

    private void Update ()
    {
        if (IsEnabled)
        {
            GameStatics.UI.GameHud.SetDashIndicator(1f - CooldownRemaining / CooldownDuration);
        }
        else
        {
            GameStatics.UI.GameHud.HideDashIndicator();
        }
        
        if (CooldownRemaining > 0)
        {
            CooldownRemaining -= Time.deltaTime;
        }
    }

    public bool Activate(MovementDirections dir)
    {
        if (PlayerManager.Instance.PossessedByPlayer)
        {
            if (!IsEnabled || CooldownRemaining > 0 || numCharges < 1) return false;
        }
        
        OnRushActivation.Invoke();
        CooldownRemaining = CooldownDuration;

        if (dashRoutine != null) StopCoroutine(dashRoutine);
        dashRoutine = StartCoroutine(DashSequence(dir));
        if (player.Controller is PlayerController)
        {
            GameStatics.Audio.Clumsy.PlaySound(ClumsySounds.ClumsyRush);
        }
        
        GameStatics.Data.Stats.TimesDashed++;

        return true;
    }

    public void Deactivate()
    {
        player.State.SetState(PlayerState.States.IsRushing, false);
        if (dashRoutine != null)
        {
            StopCoroutine(dashRoutine);
        }

        player.Physics.SetNormalSpeed();
        player.Physics.EnableGravity();
    }
    
    public void AddCharge()
    {
        numMoths++;
        if (numMoths == numMothsToRecharge)
        {
            if (numCharges < maxCharges)
            {
                numCharges++;
                numMoths = 0;
            }
            else
            {
                numMoths = numMothsToRecharge - 1;
            }
        }
    }

    private IEnumerator DashSequence(MovementDirections direction)
    {
        player.State.SetState(PlayerState.States.IsRushing, true);
        player.Physics.DisableGravity();
        player.Physics.SetVerticalVelocity(0);

        SetDashSpeed(direction);

        if (epicDashEnabled)
        {
            player.Animate(ClumsyAnimator.ClumsyAnimations.Rush);
            // TODO special dash
        }
        else
        {
            player.Animate(ClumsyAnimator.ClumsyAnimations.Rush);
        }
        
        yield return new WaitForSeconds(RushDuration);

        Deactivate();
    }

    private void SetDashSpeed(MovementDirections direction)
    {
        float speed = RushSpeed;
        if (direction == MovementDirections.Left)
        {
            player.FaceLeft();
            speed = -speed;
        }
        else
        {
            player.FaceRight();
        }
        player.Physics.SetHorizontalVelocity(speed);
    }
    
    public bool AbilityAvailable() { return IsEnabled && numCharges > 0; }
}
