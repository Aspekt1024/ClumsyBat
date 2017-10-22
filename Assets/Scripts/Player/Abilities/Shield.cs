using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    private int _shieldCharges;
    private const int MaxCharges = 2;
    private const float SHIELD_DURATION = 1.1f;
    private const float KNOCKBACK_DURATION = 0.55f;

    private Player _thePlayer;
    private Rigidbody2D _playerBody;
    private Lantern lantern;
    private ShieldEffect effect;

    private enum ShieldStates
    {
        Idle,
        Knockback,
        Recovering
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
        _state = ShieldStates.Knockback;
        effect.BeginShieldEffect(SHIELD_DURATION);
        _thePlayer.GetRenderer().color = new Color(0.6f, 0.6f, 1f);
        SetPlayerColliders("PlayerIgnoreObstacles");

        yield return StartCoroutine(Knockback());

        SetPlayerColliders("Player");
        _thePlayer.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Hover);

        _state = ShieldStates.Idle;
        _thePlayer.GetRenderer().color = new Color(1f, 1f, 1f);
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

    private IEnumerator Knockback()
    {
        float knockbackTimer = 0f;
        
        float directionModifier = GetDirectionModifier();

        while (knockbackTimer < SHIELD_DURATION)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                knockbackTimer += Time.deltaTime;
                if (knockbackTimer < KNOCKBACK_DURATION)
                {
                    _playerBody.velocity = new Vector2(Mathf.Lerp(directionModifier * 7f, 0f, knockbackTimer / SHIELD_DURATION), _playerBody.velocity.y);
                }
            }
            yield return null;
        }
    }

    private float GetDirectionModifier()
    {
        Vector3 playerPos = _thePlayer.transform.position;
        Vector3 colliderPos = _thePlayer.GetLastContactPoint();
        if (playerPos.x > colliderPos.x)
        {
            return 1f;
        }
        else
        {
            return -1f;
        }
    }

    public void Setup(Player playerRef, Lantern lanternRef)
    {
        //ShieldStats = GameData.Instance.Data.AbilityData.GetShieldStats(); // TODO Save Shield Stats
        //SetAbilityAttributes();

        _thePlayer = playerRef;
        _playerBody = _thePlayer.GetBody();
        lantern = lanternRef;
        effect = _thePlayer.GetComponentInChildren<ShieldEffect>();

        _shieldCharges = 1;
        lantern.SetColourFromShieldCharges(_shieldCharges);
    }

    public void AddCharge()
    {
        if (_shieldCharges < MaxCharges)
        {
            _shieldCharges++;
        }
    }

    public int GetCharges()
    {
        return _shieldCharges;
    }

    public bool IsAvailable() { return (_shieldCharges > 0 || _state == ShieldStates.Knockback); }
    public bool IsInUse() { return _state == ShieldStates.Knockback || _state == ShieldStates.Recovering; }

}
