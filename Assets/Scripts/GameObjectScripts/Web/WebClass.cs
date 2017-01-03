using UnityEngine;
using System.Collections;

public class WebClass : MonoBehaviour {

    public struct WebType
    {
        public Collider2D Collider;
        public Renderer Renderer;
        public Animator Anim;
        public Vector2 initialPos;
        public bool bSpecialWeb;
        public bool bIsActive;
    }
 
    private float Speed;

    private WebType Web;
    public bool SpecialWeb;
    
    void Awake()
    {
        Web.Collider = GetComponent<Collider2D>();
        Web.Renderer = GetComponent<SpriteRenderer>();
        Web.Anim = GetComponent<Animator>();
        Web.bSpecialWeb = false;
        Web.bIsActive = false;
        Web.Anim.Play("Normal", 0, 0f);
        Web.Anim.enabled = true;
    }

    void FixedUpdate()
    {
        if (!Web.bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

    void Update()
    {

    }

    public void ActivateWeb(bool bDropEnabled)
    {
        Web.bIsActive = true;
        Web.Collider.enabled = true;
        Web.Renderer.enabled = true;
        Web.bSpecialWeb = bDropEnabled;
    }
    public void DeactivateWeb()
    {
        Web.bIsActive = false;
        Web.Collider.enabled = false;
        Web.Renderer.enabled = false;
    }

    public bool IsActive()
    {
        return Web.bIsActive;
    }

    public void SetSpeed(float _speed)
    {
        Speed = _speed;
    }

    public void SetPaused(bool PauseGame)
    {
        Web.Anim.enabled = PauseGame;
    }

    public void DestroyWeb()
    {
        if (!Web.bIsActive) { return; }
        Vector2 ScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (ScreenPosition.x < Screen.width)
        {
            Web.Collider.enabled = false;
            // TODO break animation
            StartCoroutine("BreakWebAnim");
        }
        Web.Collider.enabled = false;
    }

    private IEnumerator BreakWebAnim()
    {
        Web.Anim.enabled = true;
        Web.Anim.Play("Break", 0, 0f);
        yield return new WaitForSeconds(0.67f); // TODO Set this
        DeactivateWeb();
    }
}
