﻿using UnityEngine;
using System.Collections;

public class SpiderClass : MonoBehaviour {
    
    public struct SpiderType
    {
        public Collider2D Collider;
        public Renderer Renderer;
        public Animator Anim;
        public Vector2 initialPos;
        public bool bInitialPosSet;
        public bool bSwingEnabled;
        public bool bIsActive;
    }

    private bool Paused = false;
    private float Speed;
    private float SpiderZLayer;

    private SpiderType Spider;
    public bool SwingingSpider;
    private Player Player;

    private enum SpiderStates
    {
        Swinging,
        Falling,
        Normal,
        PreparingDrop
    }
    private SpiderStates SpiderState = SpiderStates.Normal;

    void Awake()
    {
        SpiderZLayer = Toolbox.Instance.ZLayers["Spider"];
        Spider.Collider = GetComponent<Collider2D>();
        Spider.Renderer = GetComponent<SpriteRenderer>();
        Spider.Anim = GetComponent<Animator>();
        Spider.bSwingEnabled = false;
        Spider.bIsActive = false;
        Spider.bInitialPosSet = false;
        Player = FindObjectOfType<Player>();
        Spider.Anim.Play("Normal", 0, 0f);
        Spider.Anim.enabled = true;
    }

    void FixedUpdate()
    {
        if (!Spider.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

    void Update()
    {
        if (Spider.bSwingEnabled) { return; } // TODO define this
        if ((Player.transform.position.x + 6f > transform.position.x) && SpiderState == SpiderStates.Normal)
        {
            SpiderState = SpiderStates.PreparingDrop;
            StartCoroutine("Drop");
        }
    }

    public void ActivateSpider(bool bDropEnabled)
    {
        Spider.bIsActive = true;
        Spider.Collider.enabled = true;
        Spider.Renderer.enabled = true;
        Spider.bSwingEnabled = bDropEnabled;
        SpiderState = SpiderStates.Normal;
        if (!Spider.bInitialPosSet)
        {
            Spider.bInitialPosSet = true;
            Spider.initialPos = transform.position;
        }
        else
        {
            transform.position = new Vector3(Spider.initialPos.x, Spider.initialPos.y, SpiderZLayer);
        }
    }
    public void DeactivateSpider()
    {
        Spider.bIsActive = false;
        Spider.Collider.enabled = false;
        Spider.Renderer.enabled = false;
    }

    public bool IsActive()
    {
        return Spider.bIsActive;
    }

    public void SetSpeed(float _speed)
    {
        Speed = _speed;
    }

    public void SetPaused(bool PauseGame)
    {
        Paused = PauseGame;
    }

    IEnumerator Drop()
    {
        float ShakeTime = 0;
        const float ShakeDuration = 0.6f;
        bool bRotateForward = true;

        while (ShakeTime < ShakeDuration)
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
        }
        StartCoroutine("Falling");
    }

    IEnumerator Falling()
    {
        SpiderState = SpiderStates.Falling;
        while (transform.position.y > -20)
        {
            if (!Paused)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, SpiderZLayer);
            }
            yield return null;
        }
    }

    public void DestroySpiderIfInScreen()
    {
        if (!Spider.bIsActive) { return; }
        Vector2 ScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (ScreenPosition.x < Screen.width)
        {
            Spider.Collider.enabled = false;
            StartCoroutine("DestroySpider");
        }
    }

    private IEnumerator DestroySpider()
    {
        KillIt();
        yield return new WaitForSeconds(0.67f);
        DeactivateSpider();
    }

    public void KillIt()
    {
        Spider.Anim.enabled = true;
        Spider.Anim.Play("Crumble", 0, 0f);
    }
}
