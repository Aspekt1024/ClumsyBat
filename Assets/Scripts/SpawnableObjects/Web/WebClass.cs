using UnityEngine;
using System.Collections;

public class WebClass : Spawnable {

    public struct WebType
    {
        public Collider2D Collider;
        public Renderer Renderer;
        public Animator Anim;
        public Vector2 InitialPos;
        public bool SpecialWeb;
    }
    private WebType _web;

    // Editor properties
    public bool SpecialWeb;
    
    private void Awake()
    {
        _web.Collider = GetComponent<Collider2D>();
        _web.Renderer = GetComponent<SpriteRenderer>();
        _web.Anim = GetComponent<Animator>();
        _web.SpecialWeb = false;
        _web.Anim.Play("Normal", 0, 0f);
        _web.Anim.enabled = true;
        IsActive = false;
    }

    private void FixedUpdate()
    {
        if (!IsActive) { return; }
        MoveLeft(Time.deltaTime);
    }

    public void Activate(SpawnType spawnTf, bool bDropEnabled)
    {
        base.Activate(transform, spawnTf);
        _web.SpecialWeb = bDropEnabled;
    }
    
    public void DestroyWeb()
    {
        if (!IsActive) { return; }
        _web.Collider.enabled = false;
        // TODO break animation
        StartCoroutine("BreakWebAnim");
        _web.Collider.enabled = false;
    }

    private IEnumerator BreakWebAnim()
    {
        _web.Anim.enabled = true;
        _web.Anim.Play("Break", 0, 0f);
        yield return new WaitForSeconds(0.67f); // TODO Set this
        SendToInactivePool();
    }

    public bool Active() { return IsActive; }
}
