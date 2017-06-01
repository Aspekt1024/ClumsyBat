
using UnityEngine;
using System.Collections;

public class Stalactite : Spawnable {

    public SpawnStalAction.StalTypes Type;
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
    
    private const string brokenStalPath = "Obstacles/Stalactite/BrokenStal";
    private const string unbrokenStalPath = "Obstacles/Stalactite/UnbrokenStal";
    private GameObject stalPrefabUnbroken;
    private GameObject stalPrefabBroken;
    
    private Transform moth;
    private Animator mothAnim;
    private MothPool mothPool;
    private Moth.MothColour color;

    private void Awake ()
    {
        GetStalComponents();

        stalRenderer.enabled = false;   // used for editor only.
        IsActive = false;
        anim.enabled = true;
    }

    private void Start()
    {
        mothPool = GameObject.FindGameObjectWithTag("Scripts").GetComponent<GameHandler>().GetMothPool();
    }

    private void FixedUpdate()
    {
        if (!IsActive || Toolbox.Instance.GamePaused) return;

        moth.Rotate(Vector3.back, 64 * Time.deltaTime);
        MoveLeft(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsBreakable()) return;

        if (other.tag == "Boss")
        {
            Break();
        }
        else if (other.tag == "Player" && Type == SpawnStalAction.StalTypes.Crystal)
        {
            Break();
            SpawnMoth();
        }
    }

    private void GetStalComponents()
    {
        stalPrefabUnbroken = Instantiate(Resources.Load<GameObject>(unbrokenStalPath), transform);
        stalPrefabUnbroken.SetActive(true);

        stalCollider = GetComponent<PolygonCollider2D>();
        stalRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        dropControl = gameObject.AddComponent<StalDropComponent>();
        anim = gameObject.AddComponent<StalAnimationHandler>();
        anim.SetAnimator(stalPrefabUnbroken.GetComponent<Animator>());
        
        foreach (Transform tf in stalPrefabUnbroken.transform)
        {
            if (tf.name == "Moth")
            {
                moth = tf;
                mothAnim = moth.GetComponent<Animator>();
            }
        }
    }

    public void Activate(StalPool.StalType stalProps, float xOffset = 0)
    {
        if (stalPrefabBroken != null) Destroy(stalPrefabBroken);

        stalPrefabUnbroken.SetActive(true);
        stalPrefabUnbroken.transform.position = transform.position;

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
        Type = stalProps.Type;
        
        if (Type == SpawnStalAction.StalTypes.Crystal)
        {
            ActivateCrystal();
        }
        else
        {
            moth.gameObject.SetActive(false);
        }
    }

    private void ActivateCrystal()
    {
        moth.gameObject.SetActive(true);
        stalPrefabUnbroken.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);

        string mothAnimationName = "";
        switch (color)
        {
            case Moth.MothColour.Blue:
                mothAnimationName = "MothBlueCaptured";
                break;

            case Moth.MothColour.Gold:
                mothAnimationName = "MothGoldCaptured";
                break;

            case Moth.MothColour.Green:
                mothAnimationName = "MothGreenCaptured";
                break;
        }

        mothAnim.Play(mothAnimationName, 0, 0f);
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

        if (stalPrefabBroken == null)
            stalPrefabBroken = Instantiate(Resources.Load<GameObject>(brokenStalPath), transform);

        if (Type == SpawnStalAction.StalTypes.Crystal)
            BreakCrystal();

        stalPrefabUnbroken.SetActive(false);
        stalPrefabBroken.SetActive(true);
        gameObject.GetComponent<Rigidbody2D>().Sleep();
        StartCoroutine(DissolveBrokenStalactite());

    }

    private void BreakCrystal()
    {
        foreach (SpriteRenderer r in stalPrefabBroken.GetComponentsInChildren<SpriteRenderer>())
        {
            r.color = new Color(1f, 1f, 1f, 0.7f);
            r.gameObject.layer = LayerMask.NameToLayer("Rubble");
        }
        
        moth.gameObject.SetActive(false);
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

        SendToInactivePool();
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
    
    private void SpawnMoth()
    {
        Vector2 spawnLoc = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        spawnLoc += new Vector2(GameObject.FindGameObjectWithTag("MainCamera").transform.position.x, 0f);
        mothPool.ActivateMothFromEssence(moth.transform.position, spawnLoc, color, 5f);
    }

    public void Drop()
    {
        dropControl.Drop();
    }
    
    public bool IsForming() { return state == StalStates.Forming; }
    public bool IsFalling() { return state == StalStates.Falling; }
    public bool IsBroken () { return state == StalStates.Broken; }
    public bool IsBreakable() { return state == StalStates.Falling || state == StalStates.Normal; }
    public void SetState(StalStates newState) { state = newState; }
    
}