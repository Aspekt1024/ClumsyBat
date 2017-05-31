using UnityEngine;
using System.Collections;

public class Stalactite : Spawnable {
    
    public bool DropEnabled;
    [Range(2.5f, 7.5f)]
    public float TriggerPosX;
    
    public enum StalStates
    {
        Normal, Falling, Exploding, Forming, Broken
    }
    private StalStates state;

    private Collider2D stalCollider;
    private SpriteRenderer stalRenderer;
    private Rigidbody2D body;
    private StalAnimationHandler anim;
    private StalDropComponent dropControl;
    private bool isExploding;
    
    private const string BrokenStalPath = "Obstacles/Stalactite/BrokenStal";
    private const string UnbrokenStalPath = "Obstacles/Stalactite/UnbrokenStal";
    private GameObject StalPrefabUnbroken;
    private GameObject StalPrefabBroken;

    private void Awake ()
    {
        GetStalComponents();
        stalRenderer.enabled = false;
        IsActive = false;
        anim.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!IsActive || IsPaused) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsBreakable() || state == StalStates.Broken) return;

        if (other.tag == "Boss")
        {
            Break();
        }
    }

    private void GetStalComponents()
    {
        StalPrefabUnbroken = Instantiate(Resources.Load<GameObject>(UnbrokenStalPath), transform);
        StalPrefabUnbroken.SetActive(true);

        stalCollider = GetComponent<PolygonCollider2D>();
        stalRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        dropControl = gameObject.AddComponent<StalDropComponent>();
        anim = gameObject.AddComponent<StalAnimationHandler>();
    }

    public void Activate(StalPool.StalType stalProps, float xOffset = 0)
    {
        if (StalPrefabBroken != null) Destroy(StalPrefabBroken);

        StalPrefabUnbroken.SetActive(true);
        StalPrefabUnbroken.transform.position = transform.position;

        transform.localPosition = stalProps.SpawnTransform.Pos + Vector2.right * xOffset;
        transform.localScale = stalProps.SpawnTransform.Scale;
        transform.rotation = stalProps.SpawnTransform.Rotation;
        TriggerPosX = stalProps.TriggerPosX;

        anim.NewStalactite();
        dropControl.NewStalactite();

        isExploding = false;
        IsActive = true;
        stalCollider.enabled = true;
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
        if (isExploding || state == StalStates.Forming) return;

        isExploding = true;
        stalCollider.enabled = false;
        StartCoroutine(CrumbleAnim());
        IsActive = false;
    }

    private IEnumerator CrumbleAnim()
    {
        anim.Explode();
        yield return new WaitForSeconds(0.67f);
        SendToInactivePool();
    }
    
    private void Break()
    {
        state = StalStates.Broken;

        if (StalPrefabBroken == null)
            StalPrefabBroken = Instantiate(Resources.Load<GameObject>(BrokenStalPath), transform);

        StalPrefabUnbroken.SetActive(false);
        StalPrefabBroken.SetActive(true);
        gameObject.GetComponent<Rigidbody2D>().Sleep();
        StartCoroutine(DissolveBrokenStalactite());
    }
    
    private IEnumerator DissolveBrokenStalactite()
    {
        float timer = 0;
        const float timeBeforeDestroy = 4f;

        while (timer < timeBeforeDestroy)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;
            yield return null;
        }

        transform.position = Toolbox.Instance.HoldingArea;
        IsActive = false;
    }

    public void Crack()
    {
        anim.CrackOnImpact();
        StartCoroutine(Impact());
    }
    
    private IEnumerator Impact()
    {
        float impactTime = 0;
        const float impactDuration = 0.25f;
        const float impactIntensity = 0.07f;
        const float period = 0.04f;
        bool bForward = true;

        while (impactTime < impactDuration)
        {
            if (!Toolbox.Instance.GamePaused)
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
    
    public bool IsForming() { return state == StalStates.Forming; }
    public bool IsFalling() { return state == StalStates.Falling; }
    public bool IsBreakable() { return state == StalStates.Falling || state == StalStates.Normal; }
    public void SetState(StalStates newState) { state = newState; }
    
}