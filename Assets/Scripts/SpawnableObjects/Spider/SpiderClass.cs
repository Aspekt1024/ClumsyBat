using UnityEngine;
using System.Collections;

public class SpiderClass : Spawnable {

    public bool SwingingSpider;
    [HideInInspector] public bool IsFalling;    // used in animator

    public struct SpiderType
    {
        public Collider2D Collider;
        public Renderer Renderer;
        public Animator Anim;
        public bool SpiderSwings;
    }
    private SpiderType spider;
    private WebString web;
    private Player player;

    private enum SpiderStates { Swinging, Falling, Normal, PreparingDrop }
    private SpiderStates _spiderState = SpiderStates.Normal;

    private void Awake()
    {
        GetSpiderComponents();
        spider.Anim.Play("Normal", 0, 0f);
        spider.Anim.enabled = true;
        IsActive = false;
        body.isKinematic = true;
    }

    private void FixedUpdate()
    {
        web.MoveLeft(Time.deltaTime, Speed);

        if (!IsActive) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void Update()
    {
        if (spider.SpiderSwings || player == null) { return; } // TODO define this
        if (!(player.transform.position.x + 6f > transform.position.x) || _spiderState != SpiderStates.Normal)
            return;
        _spiderState = SpiderStates.PreparingDrop;
        StartCoroutine("Drop");
    }

    private void GetSpiderComponents()
    {
        body = GetComponent<Rigidbody2D>();
        spider.Collider = GetComponent<Collider2D>();
        spider.Renderer = GetComponent<SpriteRenderer>();
        spider.Anim = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        web = new WebString(transform);
    }

    public void Activate(SpawnType spawnTf, bool spiderSwings)
    {
        base.Activate(transform, spawnTf);
        spider.SpiderSwings = spiderSwings;
        _spiderState = SpiderStates.Normal;
        web.Activate();
    }
    
    public bool Active() { return IsActive; }
    public void DestroySpider() { StartCoroutine("KillIt"); }

    private IEnumerator Drop()
    {
        float shakeTime = 0;
        const float shakeDuration = 0.6f;
        bool bRotateForward = true;

        while (shakeTime < shakeDuration)
        {
            if (!IsPaused)
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
            shakeTime += 0.09f;
        }
        StartCoroutine(Falling());
    }

    private IEnumerator Falling()
    {
        _spiderState = SpiderStates.Falling;
        bool gravitySet = false;
        while (transform.position.y > -5)
        {
            if (!IsPaused && IsFalling)
            {
                if (!gravitySet)
                {
                    gravitySet = true;
                    body.isKinematic = false;
                    body.AddForce(Vector2.down * 10f);
                    web.Disengage();
                }
                web.Update();
            }
            else
            {
                gravitySet = false;
                body.velocity = Vector2.zero;
                web.Engage();
            }
            yield return null;
        }
        web.Disable();
        SendToInactivePool();
    }

    private IEnumerator KillIt()
    {
        spider.Anim.enabled = true;
        //Spider.Anim.Play("Crumble", 0, 0f);   // TODO anim
        yield return new WaitForSeconds(0.67f);
        SendToInactivePool();
    }
}
