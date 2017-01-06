using UnityEngine;
using System.Collections;

public class Stalactite : MonoBehaviour {
    
    public struct StalacType
    {
        public PolygonCollider2D Collider;
        public SpriteRenderer Renderer;
        public Rigidbody2D Body;
        public StalAnimationHandler Anim;
        public StalDropComponent DropControl;
        public bool bIsActive;
        public bool bExploding;
    }

    public enum FallType
    {
        Custom,
        Standard,
        PreFall
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
        Stal.bIsActive = false;
        Player = FindObjectOfType<Player>();
        PlayerControl = Player.GetComponent<PlayerController>();
        Stal.Anim.enabled = true;
        Stal.Body.gravityScale = 0f;
    }

    void FixedUpdate()
    {
        if (!Stal.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

    private void GetStalComponents()
    {
        const string StalObject = "StalObject";
        const string TriggerObject = "StalTrigger";
        foreach (Transform StalChild in transform)
        {
            if (StalChild.name == StalObject)
            {
                Stal.Body = StalChild.GetComponent<Rigidbody2D>();
                Stal.Anim = StalChild.GetComponent<StalAnimationHandler>();
                Stal.Collider = StalChild.GetComponent<PolygonCollider2D>();
                Stal.Renderer = StalChild.GetComponent<SpriteRenderer>();
            }
            if (StalChild.name == TriggerObject)
            {
                Stal.DropControl = StalChild.GetComponent<StalDropComponent>();
            }
        }
    }

    public void ActivateStal(bool bDropEnabled, Vector2 TriggerPos)
    {
        Stal.bIsActive = true;
        Stal.bExploding = false;
        Stal.Collider.enabled = true;
        Stal.Renderer.enabled = true;
        UnstableStalactite = bDropEnabled;

        Stal.Anim.NewStalactite();
        Stal.DropControl.NewStalactite();
}
    public void DeactivateStal()
    {
        Stal.bIsActive = false;
        Stal.Collider.enabled = false;
        Stal.Renderer.enabled = false;
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
        Debug.Log(PauseGame);
        Paused = PauseGame;
        Stal.Anim.PauseAnimation(PauseGame);
        Stal.DropControl.SetPaused(PauseGame);
    }

    public void DestroyStalactite()
    {
        if (!Stal.bExploding)
        {
            Stal.bExploding = true;
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
            if (!Paused)
            {
                transform.position += new Vector3(bForward ? ImpactIntensity : -ImpactIntensity, 0f, 0f);
                bForward = !bForward;
                ImpactTime += Period;
            }
            yield return new WaitForSeconds(Period);
        }
        if (UnstableStalactite) { Stal.DropControl.Drop(); }
    }

    public bool IsPaused()
    {
        return Paused;
    }
}