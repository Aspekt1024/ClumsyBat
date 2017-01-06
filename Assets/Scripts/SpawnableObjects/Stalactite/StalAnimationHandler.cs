using UnityEngine;
using System.Collections;

public class StalAnimationHandler : MonoBehaviour
{
    private float NormCrackTime;

    private StalBehaviour Behaviour;
    private Animator Anim;
    private Stalactite Stal;

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
        Anim = GetComponent<Animator>();
        Stal = GetComponentInParent<Stalactite>();
        NewStalactite();
    }

    void Update()
    {
        if (!Stal.IsActive()) { return; }
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Crack") && Anim.enabled)
        {
            NormCrackTime = Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (Behaviour == StalBehaviour.Impacted)
            {
                // If Clumsy hit a static stalactite then exit on frame 4
                if (NormCrackTime * 7 > 3)
                {
                    Anim.enabled = false;
                }
            }
        }
        else
        {
            NormCrackTime = 0f;
        }
    }

    public void NewStalactite()
    {
        Anim.enabled = true;
        Anim.Play("Static", 0, 0f);
        Behaviour = StalBehaviour.Normal;
    }

    public bool IsUncracked()
    {
        if (Behaviour == StalBehaviour.Normal)
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
        Anim.enabled = true;
        Behaviour = StalBehaviour.Cracking;
        Anim.Play("Crack", 0, NormCrackTime);
    }

    public void CrackOnImpact()
    {
        Anim.enabled = true;
        if (Behaviour == StalBehaviour.Normal)
        {
            Behaviour = StalBehaviour.Impacted;
            Anim.Play("Crack", 0, NormCrackTime);
        }
    }

    public void Explode()
    {
        Anim.enabled = true;
        Behaviour = StalBehaviour.Exploding;
        if (NormCrackTime > 0f)
        {
            int CrackFrameNum = Mathf.FloorToInt(7 * NormCrackTime);
            if (CrackFrameNum < 6)
            {
                float NormCrumbleTime = 3 / 16; // Start from frame 3 of 16
                Anim.Play("Crumble", 0, NormCrumbleTime);
            }
        }
        else
        {
            Anim.Play("Crumble", 0, 0f);
        }
    }

    public bool ReadyToFall()
    {
        if (NormCrackTime * 7 >= 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PauseAnimation(bool Paused)
    {
        Anim.speed = (Paused ? 0f : 1f);
    }
}

