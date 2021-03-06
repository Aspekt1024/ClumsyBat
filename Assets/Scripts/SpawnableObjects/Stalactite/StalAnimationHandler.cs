﻿using ClumsyBat.Objects;
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
        _anim = GetComponentInChildren<Animator>();
        _stal = GetComponent<Stalactite>();
        NewStalactite();
    }

    public void SetAnimator(Animator anim)
    {
        _anim = anim;
    }

    void Update()
    {
        if (!_stal.isActiveAndEnabled || _stal.IsBroken || _anim == null || !_anim.gameObject.activeSelf) return;
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
        _behaviour = StalBehaviour.Normal;
        if (_anim == null || !gameObject.activeSelf) return;

        _anim.enabled = true;
        _anim.Play("Static", 0, 0f);
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
        if (_anim == null || !_anim.gameObject.activeSelf || _stal.IsBroken) return;
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
        if (_anim == null) return;
        _anim.enabled = true;
        _behaviour = StalBehaviour.Exploding;
        float normCrumbleTime = 0f;
        if (_normCrackTime > 0f)
        {
            int crackFrameNum = Mathf.FloorToInt(7 * _normCrackTime);
            if (crackFrameNum < 6 || true)
                normCrumbleTime = 3f / 16; // Start from frame 4 of 16
            else
                normCrumbleTime = 4f / 16;
        }
        _anim.Play("Crumble", 0, normCrumbleTime);
    }

    public bool ReadyToFall()
    {
        return _normCrackTime * 7 >= 6;
    }

    public void PauseAnimation(bool paused)
    {
        if (_anim == null) return;
        _anim.speed = (paused ? 0f : 1f);
    }
}

