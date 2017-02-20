using UnityEngine;
using System.Collections;

public class Stalactite : Spawnable {
    
    public struct StalacType
    {
        public PolygonCollider2D Collider;
        public SpriteRenderer Renderer;
        public Rigidbody2D Body;
        public StalAnimationHandler Anim;
        public Transform StalTrigger;
        public StalDropComponent DropControl;
        public bool bExploding;
    }
    
    private StalacType _stal;
    
    // These variables are set in the level editor
    public bool UnstableStalactite;
    public bool Flipped; // TODO remove once we update the stalactite editor rotation thing

    private void Awake ()
    {
        GetStalComponents();
        IsActive = false;
        _stal.Anim.enabled = true;
        _stal.StalTrigger.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void FixedUpdate()
    {
        if (!IsActive || bPaused) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void GetStalComponents()
    {
        const string stalObject = "StalObject";
        const string triggerObject = "StalTrigger";
        foreach (Transform stalChild in transform)
        {
            if (stalChild.name == stalObject)
            {
                _stal.Body = stalChild.GetComponent<Rigidbody2D>();
                _stal.Anim = stalChild.GetComponent<StalAnimationHandler>();
                _stal.Collider = stalChild.GetComponent<PolygonCollider2D>();
                _stal.Renderer = stalChild.GetComponent<SpriteRenderer>();
            }
            if (stalChild.name == triggerObject)
            {
                _stal.StalTrigger = stalChild.transform;
                _stal.DropControl = stalChild.GetComponent<StalDropComponent>();
            }
        }
    }

    public void Activate(SpawnType spawnTf, bool dropEnabled, Vector2 triggerPos, float xOffset = 0)
    {
        transform.localPosition = spawnTf.Pos;
        _stal.Body.transform.localPosition = Vector3.zero;
        _stal.Body.transform.localScale = spawnTf.Scale;
        _stal.Body.transform.rotation = spawnTf.Rotation;
        _stal.StalTrigger.position = triggerPos + new Vector2(xOffset, 0f);

        _stal.Anim.NewStalactite();
        _stal.DropControl.NewStalactite();

        IsActive = true;
        _stal.bExploding = false;
        _stal.Collider.enabled = true;
        _stal.Renderer.enabled = true;
        UnstableStalactite = dropEnabled;

        _stal.StalTrigger.GetComponent<SpriteRenderer>().enabled = Toolbox.Instance.Debug && UnstableStalactite;
    }

    public override void PauseGame(bool gamePaused)
    {
        base.PauseGame(gamePaused);
        _stal.Anim.PauseAnimation(gamePaused);
        _stal.DropControl.SetPaused(gamePaused);
    }

    public void DestroyStalactite()
    {
        if (!_stal.bExploding)
        {
            _stal.bExploding = true;
            _stal.Collider.enabled = false;
            StartCoroutine("CrumbleAnim");
        }
    }

    private IEnumerator CrumbleAnim()
    {
        _stal.Anim.Explode();
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
        
        _stal.Anim.CrackOnImpact();
        while (impactTime < impactDuration)
        {
            if (!bPaused)
            {
                _stal.Body.transform.position += new Vector3(bForward ? impactIntensity : -impactIntensity, 0f, 0f);
                bForward = !bForward;
                impactTime += period;
            }
            yield return new WaitForSeconds(period);
        }
        if (UnstableStalactite) { _stal.DropControl.Drop(); }
    }

    public void Drop()
    {
        _stal.DropControl.Drop();
    }

    public bool IsPaused() { return bPaused; }
    public bool Active() { return IsActive; }
}