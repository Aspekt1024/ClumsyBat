using UnityEngine;
using System.Collections;

public class Stalactite : Spawnable {
    
    public bool DropEnabled;
    [Range(2.5f, 7.5f)]
    public float TriggerPosX;
    
    public enum StalStates
    {
        Normal, Falling, Exploding, Forming
    }
    private StalStates state;

    private Collider2D stalCollider;
    private SpriteRenderer stalRenderer;
    private Rigidbody2D body;
    private StalAnimationHandler anim;
    private StalDropComponent dropControl;
    private PhysicsStal breakComponent;
    private bool isExploding;
    
    private void Awake ()
    {
        GetStalComponents();
        IsActive = false;
        anim.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!IsActive || bPaused) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void GetStalComponents()
    {
        body = GetComponent<Rigidbody2D>();
        stalCollider = GetComponent<PolygonCollider2D>();
        stalRenderer = GetComponent<SpriteRenderer>();
        anim = gameObject.AddComponent<StalAnimationHandler>();
        dropControl = gameObject.AddComponent<StalDropComponent>();
        breakComponent = gameObject.AddComponent<PhysicsStal>();
    }

    public void Activate(StalPool.StalType stalProps, float xOffset = 0)
    {
        transform.localPosition = stalProps.SpawnTransform.Pos + Vector2.right * xOffset;
        transform.localScale = stalProps.SpawnTransform.Scale;
        transform.rotation = stalProps.SpawnTransform.Rotation;
        TriggerPosX = stalProps.TriggerPosX;

        anim.NewStalactite();
        dropControl.NewStalactite();

        isExploding = false;
        IsActive = true;
        stalCollider.enabled = true;
        stalRenderer.enabled = true;
        DropEnabled = stalProps.DropEnabled;
    }
    
    public override void PauseGame(bool gamePaused)
    {
        base.PauseGame(gamePaused);
        anim.PauseAnimation(gamePaused);
        dropControl.SetPaused(gamePaused);
    }

    public void DestroyStalactite()
    {
        if (!isExploding && state != StalStates.Forming)
        {
            isExploding = true;
            stalCollider.enabled = false;
            StartCoroutine("CrumbleAnim");
        }
    }

    private IEnumerator CrumbleAnim()
    {
        anim.Explode();
        yield return new WaitForSeconds(0.67f);
        SendToInactivePool();
    }

    public void Crack()
    {
        StartCoroutine("Impact");
    }

    private IEnumerator Impact()
    {
        float impactTime = 0;
        const float impactDuration = 0.25f;
        const float impactIntensity = 0.07f;
        const float period = 0.04f;
        bool bForward = true;
        
        anim.CrackOnImpact();
        while (impactTime < impactDuration)
        {
            if (!bPaused)
            {
                body.transform.position += new Vector3(bForward ? impactIntensity : -impactIntensity, 0f, 0f);
                bForward = !bForward;
                impactTime += period;
            }
            yield return new WaitForSeconds(period);
        }
        if (DropEnabled) { dropControl.Drop(); }
    }

    public void Drop()
    {
        dropControl.Drop();
    }

    public bool IsPaused() { return bPaused; }
    public bool Active() { return IsActive; }
    public bool IsForming() { return state == StalStates.Forming; }
    public bool IsFalling() { return state == StalStates.Falling; }
    public bool IsBreakable() { return state == StalStates.Falling || state == StalStates.Normal; }
    public void SetState(StalStates newState) { state = newState; }
    
}