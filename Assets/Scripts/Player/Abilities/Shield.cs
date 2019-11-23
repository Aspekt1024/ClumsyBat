using UnityEngine;
using System.Collections;
using ClumsyBat.Players;
using ClumsyBat;

public class Shield : MonoBehaviour {

    private int _shieldCharges;
    private const int MaxCharges = 2;
    private const float SHIELD_DURATION = 1.1f;

    private Player _thePlayer;
    private Lantern lantern;
    private ShieldEffect effect;
    
    private enum ShieldStates
    {
        Idle,
        Knockback,
        Recovering
    }
    private ShieldStates _state = ShieldStates.Idle;

    /// <summary>
    /// Returns true if the shield is active
    /// </summary>
    public bool Activate()
    {
        if (_state == ShieldStates.Knockback || _state == ShieldStates.Recovering) return true;
        if (_shieldCharges <= 0) return false;

        GameStatics.Audio.Main.PlaySound(MainSounds.Shield);
        _shieldCharges--;
        lantern.SetColourFromShieldCharges(_shieldCharges);
        StartCoroutine(ShieldUp());
        return true;
    }

    private IEnumerator ShieldUp()
    {
        _state = ShieldStates.Knockback;
        effect.BeginShieldEffect(SHIELD_DURATION);
        //SetPlayerColliders("PlayerIgnoreObstacles");

        yield return new WaitForSeconds(SHIELD_DURATION);

        //SetPlayerColliders("Player");
        _thePlayer.Animate(ClumsyAnimator.ClumsyAnimations.Hover);

        _state = ShieldStates.Idle;
    }

    private void SetPlayerColliders(string layerName)
    {
        int layerMask = LayerMask.NameToLayer(layerName);
        Transform[] childTfArray = _thePlayer.GetComponentsInChildren<Transform>();

        _thePlayer.gameObject.layer = layerMask;
        for (int i = 0; i < childTfArray.Length; i++)
        {
            if (childTfArray[i].name == "Clumsy" || childTfArray[i].name == "ShieldObject")
            {
                childTfArray[i].gameObject.layer = layerMask;
            }
        }
    }
    
    public void Setup(Player playerRef)
    {
        _thePlayer = playerRef;
        lantern = _thePlayer.lantern;
        effect = _thePlayer.GetComponentInChildren<ShieldEffect>();

        _shieldCharges = 1;
        lantern.SetColourFromShieldCharges(_shieldCharges);
    }

    public void SetCharges(int charges)
    {
        if (charges == _shieldCharges) return;
        _shieldCharges = charges;
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

    public int GetCharges()
    {
        return _shieldCharges;
    }

    public bool IsAvailable() { return (_shieldCharges > 0 || _state == ShieldStates.Knockback); }
    public bool IsInUse() { return _state == ShieldStates.Knockback || _state == ShieldStates.Recovering; }

    public void SetStats(ClumsyBat.DataContainers.AbilityContainer.AbilityType shieldStats)
    {
        // TODO implement shield stats
    }
}
