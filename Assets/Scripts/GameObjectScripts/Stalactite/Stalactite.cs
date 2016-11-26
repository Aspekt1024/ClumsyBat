using UnityEngine;
using System.Collections;

public class Stalactite : MonoBehaviour {
    
    public struct StalacType
    {
        public PolygonCollider2D Collider;
        public SpriteRenderer Renderer;
        public Vector2 initialPos;
        public bool bInitialPosSet;
        public bool bDropEnabled;
        public bool bIsActive;
    }

    private bool Paused = false;
    private float Speed;
    private const float StalZLayer = 4;

    private StalacType Stal;
    private bool bDropTriggered;
    public bool UnstableStalactite;
    private Player Player;

    void Awake ()
    {
        Stal.Collider = GetComponent<PolygonCollider2D>();
        Stal.Renderer = GetComponent<SpriteRenderer>();
        Stal.bDropEnabled = false;
        Stal.bIsActive = false;
        Stal.bInitialPosSet = false;
        Player = FindObjectOfType<Player>();
        bDropTriggered = false;
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
            DeactivateStal();
            // TODO set animation etc
        }
    }

    //public bool IsUnstable()
    //{
    //    return bDropTriggered;
    //}
}