using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClumsyMainMenu : MonoBehaviour {

    private Vector2 target = new Vector2(0, 0);
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer sprite;
    private ClumsyAnimator animControl;
    private Lantern lantern;
    private Transform lanternHinge;

    private bool isPerched;
    private bool isUnPerching;
    private bool targetReached;
    private bool isFollowingUserInput;

    private Coroutine dashRoutine;

    public void SetPosition(Vector2 pos)
    {
        isPerched = false;
        body.isKinematic = false;
        targetReached = true;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        lanternHinge.transform.position = transform.position;
        lantern.transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y - 0.7f, lantern.transform.position.z);
    }

    public void MoveToPoint(Vector2 pos)
    {
        isFollowingUserInput = false;
        targetReached = false;
        target = pos;

        if (isPerched) Unperch();
        animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Flap);
    }

    public void RemainUnperched()
    {
        if (isPerched) Unperch();
    }

    public bool TargetReached()
    {
        return targetReached;
    }

    public bool IsPerched()
    {
        return isPerched;
    }

    public void Dash()
    {
        if (dashRoutine != null) StopCoroutine(dashRoutine);
        dashRoutine = StartCoroutine(DashRoutine());
    }

    public void ClumsyTapped()
    {
        if (isPerched)
        {
            if (Random.Range(0f, 1f) > 0.8f)
            {
                MoveToPoint(transform.position + Vector3.up * 2f);
                anim.Play("FlapBlink", 0, 0f);
            }
            else
                anim.Play("Tapped", 0, 0f);
        }
    }

    public void StopFollowingUserInput()
    {
        isFollowingUserInput = false;
    }
    public void StartFollowingUserInput()
    {
        isFollowingUserInput = true;
    }

    private void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        animControl = GetComponent<ClumsyAnimator>();
        GetLanternComponents();
        targetReached = true;
        isFollowingUserInput = true;
    }

    private void FixedUpdate ()
    {
        lanternHinge.transform.position = transform.position;
        if (isFollowingUserInput && (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)))
        {
            Camera cam = FindObjectOfType<Camera>();
            target = cam.ScreenToWorldPoint(Input.mousePosition);
            targetReached = false;
            if (Vector2.Distance(target, transform.position) > 0.2f)
            {
                if (isPerched) Unperch();
                return;
            }
        }
        
        if (isPerched || targetReached) return;
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        float xDist = target.x - transform.position.x;
        if (Mathf.Abs(xDist) > 0.2f)
        {
            MoveHorizontally();
        }
        if (transform.position.y < target.y - 0.3f)
        {
            MoveVertically();
        }

        if (Vector2.Distance(transform.position, target) < 0.35f)
        {
            targetReached = true;
        }
    }

    private void MoveHorizontally()
    {
        float xDist = Mathf.Lerp(transform.position.x, target.x, Time.deltaTime * 4f) - transform.position.x;
        float maxDist = 6f * Time.deltaTime;
        xDist = Mathf.Clamp(xDist, -maxDist, maxDist);

        sprite.flipX = xDist < 0;

        transform.position += Vector3.right * xDist;
    }

    private void MoveVertically()
    {
        bool falling = body.velocity.y < 0f;
        body.velocity = new Vector2(body.velocity.x, 5.5f);

        var animState = anim.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName("Flap") || animState.IsName("FlapBlink"))
        {
            if (animState.normalizedTime < .7f && !falling) return;
        }
        
        if (Random.Range(0f, 1f) > 0.3f)
        {
            anim.Play("FlapSlower", 0, 0f);
        }
        else
        {
            animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.FlapBlink);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isPerched) return;
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("Caves"))
        {
            Perch();
        }
    }

    private void Perch()
    {
        isPerched = true;
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        if (transform.position.y > Camera.main.transform.position.y)
            animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Perch);
        else
            animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Land);
    }

    private void Unperch()
    {
        if (transform.position.y < Camera.main.transform.position.y && target.y > transform.position.y)
        {
            isPerched = false;
            body.isKinematic = false;
            transform.position += Vector3.up * 0.5f;

            if (isPerched)
                animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.FlapBlink);
        }
        else if (transform.position.y > Camera.main.transform.position.y && target.y < transform.position.y)
        {
            isPerched = false;
            body.isKinematic = false;
            transform.position += Vector3.down * 0.5f;
            animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Unperch);
        }
    }

    private IEnumerator DashRoutine()
    {
        float timer = 0f;
        const float duration = 0.6f;
        const float speed = 17f;

        body.isKinematic = true;
        body.velocity = Vector2.zero;
        animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Rush);
        anim.speed = 0.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position += Vector3.right * speed * Time.deltaTime;
            yield return null;
        }
        body.isKinematic = false;
        anim.speed = 1f;
        animControl.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Hover);

    }

    private void GetLanternComponents()
    {
        lantern = transform.parent.GetComponentInChildren<Lantern>();
        foreach (Transform tf in transform.parent.transform)
        {
            if (tf.name == "LanternJoint")
                lanternHinge = tf;
        }
    }
}
