using UnityEngine;
using System.Collections;

public class Stalactite : Spawnable {

    public enum StalStates
    {
        Normal, Falling, Exploding, Forming
    }
    private StalStates state;

    // TODO replace this struct with... individual properties... no need for a struct
    private struct StalacType
    {
        public PolygonCollider2D Collider;
        public SpriteRenderer Renderer;
        public Rigidbody2D Body;
        public StalAnimationHandler Anim;
        public StalDropComponent DropControl;
        public bool bExploding;
    }
    private StalacType stal;
    
    // These variables are set in the level editor
    public bool DropEnabled;
    public float TriggerPosX;

    private void Awake ()
    {
        GetStalComponents();
        IsActive = false;
        stal.Anim.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!IsActive || bPaused) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void GetStalComponents()
    {
        stal.Body = GetComponent<Rigidbody2D>();
        stal.Collider = GetComponent<PolygonCollider2D>();
        stal.Renderer = GetComponent<SpriteRenderer>();
        stal.Anim = gameObject.AddComponent<StalAnimationHandler>();
        stal.DropControl = gameObject.AddComponent<StalDropComponent>();
    }

    public void Activate(SpawnType spawnTf, bool dropEnabled, float triggerPosX, float xOffset = 0)
    {
        transform.localPosition = spawnTf.Pos;
        stal.Body.transform.localPosition = Vector3.zero;
        stal.Body.transform.localScale = spawnTf.Scale;
        stal.Body.transform.rotation = spawnTf.Rotation;
        TriggerPosX = xOffset + triggerPosX;

        stal.Anim.NewStalactite();
        stal.DropControl.NewStalactite();

        IsActive = true;
        stal.bExploding = false;
        stal.Collider.enabled = true;
        stal.Renderer.enabled = true;
        DropEnabled = dropEnabled;

        // TODO display trigger box in debug mode
        //stal.StalTrigger.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug && DropEnabled;
    }

    public override void PauseGame(bool gamePaused)
    {
        base.PauseGame(gamePaused);
        stal.Anim.PauseAnimation(gamePaused);
        stal.DropControl.SetPaused(gamePaused);
    }

    public void DestroyStalactite()
    {
        if (!stal.bExploding && state != StalStates.Forming)
        {
            stal.bExploding = true;
            stal.Collider.enabled = false;
            StartCoroutine("CrumbleAnim");
        }
    }

    private IEnumerator CrumbleAnim()
    {
        stal.Anim.Explode();
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
        
        stal.Anim.CrackOnImpact();
        while (impactTime < impactDuration)
        {
            if (!bPaused)
            {
                stal.Body.transform.position += new Vector3(bForward ? impactIntensity : -impactIntensity, 0f, 0f);
                bForward = !bForward;
                impactTime += period;
            }
            yield return new WaitForSeconds(period);
        }
        if (DropEnabled) { stal.DropControl.Drop(); }
    }

    public void Drop()
    {
        stal.DropControl.Drop();
    }

    public bool IsPaused() { return bPaused; }
    public bool Active() { return IsActive; }
    public bool IsForming() { return state == StalStates.Forming; }
    public bool IsFalling() { return state == StalStates.Falling; }
    public void SetState(StalStates newState) { state = newState; }
}