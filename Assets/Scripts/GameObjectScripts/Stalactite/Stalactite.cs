using UnityEngine;
using System.Collections;

public class Stalactite : MonoBehaviour {
    
    public struct StalacType
    {
        public PolygonCollider2D Collider;
        public SpriteRenderer Renderer;
        public Rigidbody2D Body;
        public SpriteRenderer TriggerSprite;
        public Animator Anim;
        public StalState State;
        public bool bIsActive;
    }

    public enum FallType
    {
        Custom,
        NoFall,
        Under_Easy,
        Under_Hard,
        Under_Dash,
        Over_Easy,
        Over_Hard,
        PreFall_Early,
        PreFall_VeryEarly,
        PostFall
    }

    public enum StalState
    {
        Normal,
        Shaking,
        Falling,
        Exploding
    }

    private bool Paused = false;
    private float Speed;
    private bool bCracked;

    private StalacType Stal;
    private Player Player;
    private PlayerController PlayerControl;
    
    // These variables are set in the level editor
    // UnstableStalactite is the only one used during gameplay
    public bool UnstableStalactite;
    public FallType FallPreset;
    public bool Flipped;

    void Awake ()
    {
        GetStalComponents();
        Stal.State = StalState.Normal;
        Stal.bIsActive = false;
        Player = FindObjectOfType<Player>();
        PlayerControl = Player.GetComponent<PlayerController>();
        Stal.Anim.Play("Static", 0, 0f);
        Stal.Anim.enabled = true;
        bCracked = false;
    }

    void FixedUpdate()
    {
        if (!Stal.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

    void Update ()
    {
        if (!UnstableStalactite) { return; }
        if ((Player.transform.position.x + 10 > transform.position.x) && Stal.State == StalState.Normal && PlayerControl.IsAlive())
        {
            Stal.State = StalState.Shaking;
            StartCoroutine("Shake");
        }

        if (!PlayerControl) { return; } // Editor clumsy has no player control
        if (!PlayerControl.IsAlive() && Stal.State == StalState.Shaking)
        {
            Stal.State = StalState.Normal;
        }
	}

    private void GetStalComponents()
    {
        foreach (Transform StalChild in transform)
        {
            if (StalChild.name == "StalObject")
            {
                Stal.Body = StalChild.GetComponent<Rigidbody2D>();
                Stal.Anim = StalChild.GetComponent<Animator>();
                Stal.Collider = StalChild.GetComponent<PolygonCollider2D>();
                Stal.Renderer = StalChild.GetComponent<SpriteRenderer>();
            }
            if (StalChild.name == "StalTrigger")
            {
                Stal.TriggerSprite = StalChild.GetComponent<SpriteRenderer>();
            }
        }
    }

    public void ActivateStal(bool bDropEnabled, Vector2 TriggerPos)
    {
        Stal.bIsActive = true;
        Stal.Collider.enabled = true;
        Stal.Renderer.enabled = true;
        UnstableStalactite = bDropEnabled;
        Stal.TriggerSprite.enabled = Toolbox.Instance.Debug && UnstableStalactite;
        Stal.Anim.Play("Static", 0, 0f);
        bCracked = false;
    }
    public void DeactivateStal()
    {
        // TODO Stal.GO (transform.gameObject).SetActive(false);
        Stal.bIsActive = false;
        Stal.Collider.enabled = false;
        Stal.Renderer.enabled = false;
    }

    void OnTriggerEnter2D()
    {
        if (UnstableStalactite && Stal.State != StalState.Falling)
        {
            Drop();
        }
    }

    private void Drop()
    {
        Stal.Anim.enabled = false;  // TODO play drop animation once available

        Stal.State = StalState.Falling;
        Stal.Body.velocity = new Vector2(0f, -2f);
        Stal.Body.isKinematic = false;
    }

    public bool IsActive()
    {
        return Stal.bIsActive;
    }

    public void SetSpeed(float _speed)
    {
        Speed = _speed;
    }

    public void SetPaused(bool PauseGame)
    {
        Paused = PauseGame;
    }

    IEnumerator Shake()
    {
        const float ShakeInterval = 0.09f;
        float ShakeTime = 0;
        bool bRotateForward = true;
        while (Stal.State == StalState.Shaking)
        {
            if (!Paused)
            {
                if (bRotateForward)
                {
                    Stal.Body.transform.Rotate(Vector3.forward * 2.5f);
                }
                else
                {
                    Stal.Body.transform.Rotate(Vector3.back * 2.5f);
                }
                bRotateForward = !bRotateForward;
            }
            yield return new WaitForSeconds(ShakeInterval);
            ShakeTime += ShakeInterval;
            if (ShakeTime > 1f && !bCracked)
            {
                bCracked = true;
                Stal.Anim.Play("SlowCrack", 0, 0f);
            }
        }
        Stal.Body.transform.Rotate(Vector3.zero);   // Prevents rotating once we exit the while loop
    }

    public void DestroyStalactite()
    {
        if (Stal.State != StalState.Exploding)
        {
            Stal.State = StalState.Exploding;
            Stal.Collider.enabled = false;
            StartCoroutine("CrumbleAnim");
        }
    }

    private IEnumerator CrumbleAnim()
    {
        Stal.Anim.enabled = true;
        Stal.Anim.Play("Crumble", 0, 0f);
        yield return new WaitForSeconds(0.67f);
        DeactivateStal();
    }

    public void Crack()
    {
        Stal.Anim.enabled = true;
        StartCoroutine("Impact");
    }

    private IEnumerator Impact()
    {
        float ImpactTime = 0;
        const float ImpactDuration = 0.3f;
        bool bForward = true;
        
        while (ImpactTime < ImpactDuration && !bCracked)
        {
            transform.position += new Vector3(bForward ? 0.1f : -0.1f, 0f, 0f);
            bForward = !bForward;
            yield return new WaitForSeconds(0.08f);
            ImpactTime += 0.09f;
            if (!bCracked)
            {
                bCracked = true;
                Stal.Anim.enabled = true;
                Stal.Anim.Play("Crack", 0, 0f);
            }
        }
        if (UnstableStalactite) { Drop(); }
    }
}