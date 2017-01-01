using UnityEngine;
using System.Collections;

public class Stalactite : MonoBehaviour {
    
    public struct StalacType
    {
        public PolygonCollider2D Collider;
        public SpriteRenderer Renderer;
        public Animator Anim;
        public Vector2 initialPos;
        public bool bInitialPosSet;
        public bool bDropEnabled;
        public bool bIsActive;
    }

    private bool Paused = false;
    private float Speed;
    private float StalZLayer;

    private StalacType Stal;
    private bool bDropTriggered;
    public bool UnstableStalactite;
    private Player Player;

    void Awake ()
    {
        StalZLayer = Toolbox.Instance.ZLayers["Stalactite"];
        Stal.Collider = GetComponent<PolygonCollider2D>();
        Stal.Renderer = GetComponent<SpriteRenderer>();
        Stal.Anim = GetComponent<Animator>();
        Stal.bDropEnabled = false;
        Stal.bIsActive = false;
        Stal.bInitialPosSet = false;
        Player = FindObjectOfType<Player>();
        bDropTriggered = false;
        Stal.Anim.Play("Static", 0, 0f);
        Stal.Anim.enabled = true;
    }

    void FixedUpdate()
    {
        if (!Stal.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

    void Update ()
    {
        if (!Stal.bDropEnabled) { return; }
        if ((Player.transform.position.x + 10 > transform.position.x) && !bDropTriggered)
        {
            bDropTriggered = true;
            StartCoroutine("Shake");
        }
	}

    public void ActivateStal(bool bDropEnabled)
    {
        Stal.bIsActive = true;
        Stal.Collider.enabled = true;
        Stal.Renderer.enabled = true;
        Stal.bDropEnabled = bDropEnabled;
        bDropTriggered = false;
        if (!Stal.bInitialPosSet)
        {
            Stal.bInitialPosSet = true;
            Stal.initialPos = transform.position;
        }
        else
        {
            transform.position = new Vector3(Stal.initialPos.x, Stal.initialPos.y, StalZLayer);
        }
    }
    public void DeactivateStal()
    {
        // TODO replace collider and renderer with the Transform itself
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
        Paused = PauseGame;
    }

    IEnumerator Shake()
    {
        float ShakeTime = 0;
        bool bRotateForward = true;
        while (Stal.bDropEnabled)
        {
            if (!Paused)
            {
                if (bRotateForward)
                {
                    transform.Rotate(Vector3.forward * 3);
                }
                else
                {
                    transform.Rotate(Vector3.back * 3);
                }
                bRotateForward = !bRotateForward;
            }
            yield return new WaitForSeconds(0.09f);
            ShakeTime += 0.09f;
            if (ShakeTime > 1.2f)
            {
                break;
            }
        }
        if (Stal.bDropEnabled)
        {
            Stal.bDropEnabled = false;
            StartCoroutine("Falling");
        }
    }

    IEnumerator Falling()
    {
        while (transform.position.y > -20)
        {
            if (!Paused)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.25f, StalZLayer);
            }
            yield return null;
        }
    }

    public void DestroyStalactiteIfInScreen()
    {
        if (!Stal.bIsActive) { return; }
        Vector2 ScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (ScreenPosition.x < Screen.width)
        {
            Stal.Collider.enabled = false;
            StartCoroutine("CrumbleAnim");
        }
    }

    private IEnumerator CrumbleAnim()
    {
        Crumble();
        yield return new WaitForSeconds(0.67f);
        DeactivateStal();
    }

    public void Crumble()
    {
        Stal.Anim.enabled = true;
        Stal.Anim.Play("Crumble", 0, 0f);
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
        bool bCracked = false;
        
        while (ImpactTime < ImpactDuration)
        {
            transform.position += new Vector3(bForward ? 0.1f : -0.1f, 0f, 0f);
            bForward = !bForward;
            yield return new WaitForSeconds(0.08f);
            ImpactTime += 0.09f;
            if (!bCracked)
            {
                bCracked = true;
                Stal.Anim.Play("Crack", 0, 0f);
            }
        }
    }

    //public bool IsUnstable()
    //{
    //    return bDropTriggered;
    //}
}