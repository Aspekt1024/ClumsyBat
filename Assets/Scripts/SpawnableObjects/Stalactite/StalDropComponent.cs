using UnityEngine;
using System.Collections;

public class StalDropComponent : MonoBehaviour {

    [HideInInspector]
    public SpriteRenderer TriggerSprite = null;
    private Stalactite Stal = null;
    private StalAnimationHandler Anim = null;
    private Rigidbody2D StalBody = null;
    private Transform PlayerBody = null;
    private PlayerController PlayerControl = null;

    [HideInInspector]
    public const float FallDuration = 1.2f;
    [HideInInspector]
    public const float FallDistance = 20f;

    private bool Paused = false;
    private Vector2 StoredVelocity = Vector2.zero;

    private enum DropStates
    {
        None,
        Shaking,
        Falling
    }
    private DropStates State;
    
    void Awake()
    {
        GetComponentList();
    }

    void Update()
    {
        if (!Stal.IsActive() || !Stal.UnstableStalactite) { return; }

        if ((PlayerBody.position.x + 8f > transform.position.x) && State == DropStates.None && PlayerControl.IsAlive())
        {
            State = DropStates.Shaking;
            StartCoroutine("Shake");
        }

        if (!PlayerControl) { return; } // Editor clumsy has no player control
        if (!PlayerControl.IsAlive() && State == DropStates.Shaking)
        {
            State = DropStates.None;
        }
    }

    void OnTriggerEnter2D()
    {
        if (Stal.UnstableStalactite)
        {
            Drop();
        }
    }

    public void NewStalactite()
    {
        TriggerSprite.enabled = Toolbox.Instance.Debug && Stal.UnstableStalactite;
    }
    
    public void Drop()
    {
        if (State != DropStates.Falling)
        {
            State = DropStates.Falling;
            StartCoroutine("DropSequence");
        }
    }
    
    private IEnumerator DropSequence()
    {
        Anim.CrackAndFall();
        while (!Anim.ReadyToFall() || Paused)
        {
            yield return null;
        }
        float FallTime = 0f;
        float StartingYPos = StalBody.transform.position.y;
        while (FallTime < FallDuration)
        {
            if (!Paused)
            {
                FallTime += Time.deltaTime;
                float YPos = StartingYPos - FallDistance * Mathf.Pow((FallTime / FallDuration), 2);
                StalBody.transform.position = new Vector3(StalBody.transform.position.x, YPos, StalBody.transform.position.z);
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                yield return null;
            }
        }
    }
    
    IEnumerator Shake()
    {
        const float ShakeInterval = 0.07f;
        const float ShakeIntensity = 1.8f;
        bool bRotateForward = true;
        while (State == DropStates.Shaking)
        {
            if (!Paused)
            {
                StalBody.transform.Rotate(new Vector3(0, 0, (bRotateForward ? ShakeIntensity : -ShakeIntensity)));
                bRotateForward = !bRotateForward;
            }
            yield return new WaitForSeconds(ShakeInterval);
        }
        StalBody.transform.Rotate(Vector3.zero);   // Prevents rotating once we exit the while loop
    }

    public void SetPaused(bool bPaused)
    {
        Paused = bPaused;
        if (State == DropStates.Falling)
        {
            if (Paused)
            {
                StoredVelocity = StalBody.velocity;
                StalBody.velocity = Vector2.zero;
                StalBody.isKinematic = true;
            }
            else
            {
                StalBody.isKinematic = false;
                StalBody.velocity = StoredVelocity;
            }
        }
    }

    private void GetComponentList()
    {
        PlayerBody = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerControl = PlayerBody.GetComponent<PlayerController>();
        
        TriggerSprite = GetComponent<SpriteRenderer>();
        Stal = GetComponentInParent<Stalactite>();
        Anim = Stal.GetComponentInChildren<StalAnimationHandler>();

        foreach (Transform TF in Stal.GetComponent<Transform>())
        {
            if (TF.name == "StalObject")
            {
                StalBody = TF.GetComponent<Rigidbody2D>();
            }
        }
    }
}
