using UnityEngine;

public class StalAnimationHandler : MonoBehaviour
{
    private float _normCrackTime;

    private StalBehaviour _behaviour;
    private Animator _anim;
    private Stalactite _stal;

    public enum StalBehaviour
    {
        Normal,
        Impacted,
        Cracking,
        Falling,
        Exploding
    }

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _stal = GetComponentInParent<Stalactite>();
        NewStalactite();
    }

    void Update()
    {
        if (!_stal.Active()) { return; }
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Crack") && _anim.enabled)
        {
            _normCrackTime = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_behaviour == StalBehaviour.Impacted)
            {
                // If Clumsy hit a static stalactite then exit on frame 4
                if (_normCrackTime * 7 > 3)
                {
                    _anim.enabled = false;
                }
            }
        }
        else
        {
            _normCrackTime = 0f;
        }
    }

    public void NewStalactite()
    {
        _anim.enabled = true;
        _anim.Play("Static", 0, 0f);
        _behaviour = StalBehaviour.Normal;
    }

    public bool IsUncracked()
    {
        if (_behaviour == StalBehaviour.Normal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CrackAndFall()
    {
        _anim.enabled = true;
        _behaviour = StalBehaviour.Cracking;
        _anim.Play("Crack", 0, _normCrackTime);
    }

    public void CrackOnImpact()
    {
        _anim.enabled = true;
        if (_behaviour == StalBehaviour.Normal)
        {
            _behaviour = StalBehaviour.Impacted;
            _anim.Play("Crack", 0, _normCrackTime);
        }
    }

    public void Explode()
    {
        _anim.enabled = true;
        _behaviour = StalBehaviour.Exploding;
        if (_normCrackTime > 0f)
        {
            int crackFrameNum = Mathf.FloorToInt(7 * _normCrackTime);
            if (crackFrameNum < 6)
            {
                const float normCrumbleTime = 3f / 16; // Start from frame 4 of 16
                _anim.Play("Crumble", 0, normCrumbleTime);
            }
        }
        else
        {
            _anim.Play("Crumble", 0, 0f);
        }
    }

    public bool ReadyToFall()
    {
        return _normCrackTime * 7 >= 6;
    }

    public void PauseAnimation(bool paused)
    {
        _anim.speed = (paused ? 0f : 1f);
    }
}

