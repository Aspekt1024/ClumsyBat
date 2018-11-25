using UnityEngine;
using System.Collections;
using ClumsyBat.Players;

public class SpiderClass : Spawnable {

    public bool SwingingSpider;
    public Vector2 WebAnchorPoint;
    [HideInInspector] public bool IsFalling;    // used in animator

    public struct SpiderType
    {
        public Collider2D Collider;
        public Renderer Renderer;
        public Animator Anim;
        public bool SpiderSwings;
        public Vector2 AnchorPoint;
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
        if (!IsActive) { return; }
        MoveLeft(Time.fixedDeltaTime);
    }

    private void Update()
    {
        web.MoveLeft(Time.deltaTime, Speed);
        web.UpdateWebSprites();

        if (player == null) { return; }
        if (!(player.Model.transform.position.x + 7f > transform.position.x + (spider.SpiderSwings ? spider.AnchorPoint.x : 0f)) || _spiderState != SpiderStates.Normal)
            return;
        _spiderState = SpiderStates.PreparingDrop;
        StartCoroutine(Drop());
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

    public void Activate(SpawnType spawnTf, bool spiderSwings, Vector2 anchorPoint)
    {
        base.Activate(transform, spawnTf);
        spider.SpiderSwings = spiderSwings;
        spider.AnchorPoint = anchorPoint;
        _spiderState = SpiderStates.Normal;
        web.Activate(spiderSwings, anchorPoint);
    }
    
    public bool Active() { return IsActive; }
    public void DestroySpider() { if (IsActive) StartCoroutine(KillIt()); }

    private void OnCollisionEnter2D(Collision2D other)
    {
        StopAllCoroutines();
        web.Collision();
    }

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

        if (spider.SpiderSwings)
        {
            body.isKinematic = false;
            body.AddForce(new Vector2(-700, -1000));
        }
        else
        {
            StartCoroutine(Falling());
        }
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
                web.UpdateDrop();
            }
            else
            {
                gravitySet = false;
                body.velocity = Vector2.zero;
            }
            yield return null;
        }
        web.Disengage();
        SendToInactivePool();
    }

    private IEnumerator KillIt()
    {
        spider.Anim.enabled = true;
        web.Disengage();
        body.isKinematic = false;
        //Spider.Anim.Play("Crumble", 0, 0f);   // TODO anim
        yield return new WaitForSeconds(1f);
        SendToInactivePool();
    }
}
