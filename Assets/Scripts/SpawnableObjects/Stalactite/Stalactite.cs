
using UnityEngine;
using System.Collections;

// TODO there are really two different objects described in this and they should be split up.
// They work fine, but i don't like it. Fix by either creating a base class, or using composition
public class Stalactite : Spawnable {

    public bool DropEnabled;
    [Range(2.5f, 7.5f)]
    public float TriggerPosX;

    public SpawnStalAction.StalTypes Type;

    public enum StalStates
    {
        Normal, Falling, Exploding, Forming, Broken
    }
    private StalStates state;

    private Collider2D stalCollider;
    private SpriteRenderer stalRenderer;
    private StalAnimationHandler anim;
    private StalDropComponent dropControl;
    private bool isExploding;
    
    private const string unbrokenStalPath = "Obstacles/Stalactite/UnbrokenStal";
    private const string brokenStalPath = "Obstacles/Stalactite/BrokenStal";
    private const string unbrokenCrystalPath = "Obstacles/Stalactite/UnbrokenCrystal";
    private const string brokenCrystalPath = "Obstacles/Stalactite/BrokenCrystal";
    private GameObject stalUnbroken;
    private GameObject stalBroken;

    private GameObject stalPrefabUnbroken;
    private GameObject stalPrefabBroken;
    private GameObject crystalPrefabBroken;
    private GameObject crystalPrefabUnbroken;
    
    private Transform moth;
    private Animator mothAnim;
    private MothPool mothPool;
    private Moth.MothColour color;
    private float greenMothChance;
    private float goldMothChance;
    private float blueMothChance;

    private void Awake ()
    {
        GetStalComponents();

        stalRenderer.enabled = false;   // used for editor only.
        IsActive = false;
        anim.enabled = true;
    }

    private void Start()
    {
        GameHandler gh = GameObject.FindGameObjectWithTag("Scripts").GetComponent<GameHandler>();
        if (gh == null) return; // Level editor has no game handler
        mothPool = gh.GetMothPool();
    }

    private void FixedUpdate()
    {
        if (!IsActive || Toolbox.Instance.GamePaused) return;

        MoveLeft(Time.deltaTime);

        if (Type == SpawnStalAction.StalTypes.Crystal)
            moth.Rotate(Vector3.back, 64 * Time.deltaTime);
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
        crystalPrefabUnbroken = Resources.Load<GameObject>(unbrokenCrystalPath);
        crystalPrefabBroken = Resources.Load<GameObject>(brokenCrystalPath);
        stalPrefabUnbroken = Resources.Load<GameObject>(unbrokenStalPath);
        stalPrefabBroken = Resources.Load<GameObject>(brokenStalPath);
        
        stalCollider = GetComponent<PolygonCollider2D>();
        stalRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        dropControl = gameObject.AddComponent<StalDropComponent>();
        anim = gameObject.AddComponent<StalAnimationHandler>();
    }

    private void GetMothComponents()
    {
        foreach (Transform tf in stalUnbroken.transform)
        {
            if (tf.name == "Moth")
            {
                mothAnim = tf.GetComponent<Animator>();
                moth = tf;
                break;
            }
        }
    }

    public void Activate(StalPool.StalType stalProps, float xOffset = 0)
    {
        Type = stalProps.Type;
        isExploding = false;
        IsActive = true;

        if (stalBroken != null) Destroy(stalBroken);
        if (stalUnbroken != null) Destroy(stalUnbroken);

        if (Type == SpawnStalAction.StalTypes.Crystal)
        {
            stalUnbroken = Instantiate(crystalPrefabUnbroken, transform);
            GetMothComponents();
            stalCollider.enabled = false;
        }
        else
        {
            stalUnbroken = Instantiate(stalPrefabUnbroken, transform);
            anim.SetAnimator(stalUnbroken.GetComponent<Animator>());
            anim.NewStalactite();
            dropControl.SetAnim(anim);
            stalCollider.enabled = true;
        }

        stalUnbroken.SetActive(true);
        stalUnbroken.transform.position = transform.position;

        transform.localPosition = stalProps.SpawnTransform.Pos + Vector2.right * xOffset;
        transform.localScale = stalProps.SpawnTransform.Scale;
        transform.rotation = stalProps.SpawnTransform.Rotation;
        TriggerPosX = stalProps.TriggerPosX;

        dropControl.NewStalactite();

        DropEnabled = stalProps.DropEnabled;
        greenMothChance = stalProps.GreenMothChance;
        goldMothChance = stalProps.GoldMothChance;
        blueMothChance = stalProps.BlueMothChance;

        if (Type == SpawnStalAction.StalTypes.Crystal)
        {
            ActivateCrystal();
        }
    }

    private void ActivateCrystal()
    {
        color = DetermineMothColor();
        stalUnbroken.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
        
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
        if (isExploding || state == StalStates.Broken) return;

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

        if (stalBroken != null)
            Destroy(stalBroken);
        
        if (Type == SpawnStalAction.StalTypes.Crystal)
        {
            stalBroken = Instantiate(crystalPrefabBroken, transform);
            Toolbox.MainAudio.PlaySound(Toolbox.MainAudio.BreakCrystal);
        }
        else
        {
            stalBroken = Instantiate(stalPrefabBroken, transform);
        }
        
        stalUnbroken.SetActive(false);
        stalBroken.SetActive(true);
        gameObject.GetComponent<Rigidbody2D>().Sleep();
        StartCoroutine(DissolveBrokenStalactite());
    }

    private IEnumerator DissolveBrokenStalactite()
    {
        float timer = 0;
        const float timeBeforeDestroy = 4f;

        while (timer < timeBeforeDestroy && IsActive)
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

    private Moth.MothColour DetermineMothColor()
    {
        float colorTotals = greenMothChance + goldMothChance + blueMothChance;
        if (colorTotals <= 0) return Moth.MothColour.Green;

        float weightedGreen = greenMothChance / colorTotals;
        float weightedGold = goldMothChance / colorTotals;
        float weightedBlue = blueMothChance / colorTotals;

        float randomVal = Random.Range(0f, 1f);

        if (randomVal < weightedGreen)
            return Moth.MothColour.Green;
        else if (randomVal < weightedGreen + weightedGold)
            return Moth.MothColour.Gold;
        else
            return Moth.MothColour.Blue;
    }

    private void SpawnMoth()
    {
        Vector2 spawnLoc = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        spawnLoc += new Vector2(Toolbox.PlayerCam.transform.position.x, 0f);
        mothPool.ActivateMothFromEssence(moth.transform.position, spawnLoc, color, 5f);
    }

    public void Drop()
    {
        if (IsActive)
            dropControl.Drop();
    }

    public override void SendToInactivePool()
    {
        base.SendToInactivePool();
        if (stalBroken != null) Destroy(stalBroken);
        if (stalUnbroken != null) Destroy(stalUnbroken);
    }

    public bool IsForming() { return state == StalStates.Forming; }
    public bool IsFalling() { return state == StalStates.Falling; }
    public bool IsBroken () { return state == StalStates.Broken; }
    public bool IsBreakable() { return state == StalStates.Falling || state == StalStates.Normal; }
    public void SetState(StalStates newState) { state = newState; }
    
}