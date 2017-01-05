using UnityEngine;
using System.Collections;

public class Stalactite : MonoBehaviour {
    
    public struct StalacType
    {
        public PolygonCollider2D Collider;
        public SpriteRenderer Renderer;
        public Rigidbody2D Body;
        public SpriteRenderer TriggerSprite;
        public StalAnimationHandler Anim;
        public StalState State;
        public bool bIsActive;
    }

    public enum FallType
    {
        Custom,
        Standard,
        PreFall
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
    
    private StalacType Stal;
    private Player Player;
    private PlayerController PlayerControl;
    
    // These variables are set in the level editor
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
        Stal.Anim.enabled = true;
    }

    void FixedUpdate()
    {
        if (!Stal.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

    void Update ()
    {
        if (!UnstableStalactite) { return; }
        if ((Player.transform.position.x + 7 > transform.position.x) && Stal.State == StalState.Normal && PlayerControl.IsAlive())
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
                Stal.Anim = StalChild.GetComponent<StalAnimationHandler>();
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
        Stal.Anim.NewStalactite();
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
            StartCoroutine("Drop");
        }
    }

    private IEnumerator Drop()
    {
        Stal.Anim.CrackAndFall();
        while (!Stal.Anim.ReadyToFall())
        {
            yield return null;
        }
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
        const float ShakeInterval = 0.07f;
        const float ShakeIntensity = 1.8f;
        bool bRotateForward = true;
        while (Stal.State == StalState.Shaking)
        {
            if (!Paused)
            {
                Stal.Body.transform.Rotate(new Vector3(0, 0, (bRotateForward ? ShakeIntensity : - ShakeIntensity)));
                bRotateForward = !bRotateForward;
            }
            yield return new WaitForSeconds(ShakeInterval);
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
        Stal.Anim.Explode();
        yield return new WaitForSeconds(0.67f);
        DeactivateStal();
    }

    public void Crack()
    {
        StartCoroutine("Impact");
    }

    private IEnumerator Impact()
    {
        float ImpactTime = 0;
        const float ImpactDuration = 0.25f;
        const float ImpactIntensity = 0.07f;
        const float Period = 0.04f;
        bool bForward = true;
        
        Stal.Anim.CrackOnImpact();
        while (ImpactTime < ImpactDuration)
        {
            transform.position += new Vector3(bForward ? ImpactIntensity : -ImpactIntensity, 0f, 0f);
            bForward = !bForward;
            yield return new WaitForSeconds(Period);
            ImpactTime += Period;
        }
        if (UnstableStalactite) { StartCoroutine("Drop"); }
    }
}