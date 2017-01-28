using UnityEngine;
using System.Collections;

public class SpiderClass : Spawnable {
    
    public struct SpiderType
    {
        public Collider2D Collider;
        public Renderer Renderer;
        public Animator Anim;
        public bool SpiderSwings;
    }
    private SpiderType _spider;
    public bool SwingingSpider; // For editor
    private Player _player;

    private enum SpiderStates { Swinging, Falling, Normal, PreparingDrop }
    private SpiderStates _spiderState = SpiderStates.Normal;

    private void Awake()
    {
        GetSpiderComponents();
        _spider.Anim.Play("Normal", 0, 0f);
        _spider.Anim.enabled = true;
        IsActive = false;
    }

    private void FixedUpdate()
    {
        if (!IsActive) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void Update()
    {
        if (_spider.SpiderSwings) { return; } // TODO define this
        if (!(_player.transform.position.x + 6f > transform.position.x) || _spiderState != SpiderStates.Normal)
            return;
        _spiderState = SpiderStates.PreparingDrop;
        StartCoroutine("Drop");
    }

    private void GetSpiderComponents()
    {
        _spider.Collider = GetComponent<Collider2D>();
        _spider.Renderer = GetComponent<SpriteRenderer>();
        _spider.Anim = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
    }

    public void Activate(SpawnType spawnTf, bool spiderSwings)
    {
        base.Activate(transform, spawnTf);
        _spider.SpiderSwings = spiderSwings;
        _spiderState = SpiderStates.Normal;
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
            if (!bPaused)
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
        StartCoroutine("Falling");
    }

    private IEnumerator Falling()
    {
        _spiderState = SpiderStates.Falling;
        while (transform.position.y > -20)
        {
            if (!bPaused)
            {
                transform.position -= new Vector3(0f, 0.05f, 0f);
            }
            yield return null;
        }
    }

    private IEnumerator KillIt()
    {
        _spider.Anim.enabled = true;
        //Spider.Anim.Play("Crumble", 0, 0f);   // TODO anim
        yield return new WaitForSeconds(0.67f);
        SendToInactivePool();
    }
}
