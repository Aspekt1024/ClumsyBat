using UnityEngine;
using System.Collections;

public class Hypersonic : MonoBehaviour {

    private bool bPaused = false;
    private bool bEnabled = false;
    private int AbilityLevel = 1;

    private Transform HypersonicBody;
    private SpriteRenderer HypersonicSprite;
    private Animator HypersonicAnimator;
    private CircleCollider2D HypersonicCollider;

    private Transform Lantern;

	void Start ()
    {
        Lantern = GameObject.Find("Lantern").GetComponent<Transform>();
        HypersonicBody = GetComponent<Transform>();
        HypersonicAnimator = GetComponentInChildren<Animator>();
        HypersonicSprite = GetComponentInChildren<SpriteRenderer>();
        HypersonicSprite.enabled = false;
        HypersonicCollider = GetComponent<CircleCollider2D>();
        HypersonicCollider.radius = 0.01f;
        HypersonicCollider.enabled = false;
    }

    public void ActivateHypersonic()
    {
        if (bEnabled || !bEnabled)
        {
            StartCoroutine("HypersonicAnimation");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "StalObject")
        {
            other.GetComponentInParent<Stalactite>().DestroyStalactite();
        }
    }

    private IEnumerator HypersonicAnimation()
    {
        HypersonicCollider.enabled = true;
        HypersonicSprite.enabled = true;
        HypersonicAnimator.Play("HyperGold", 0, 0f);

        const float ColliderMinScale = 0.01f;
        const float ColliderMaxScale = 1.4f;

        const float AnimationDuration = 0.5f;
        float AnimTimer = 0f;

        while (AnimTimer < AnimationDuration)
        {
            if (!bPaused)
            {
                AnimTimer += Time.deltaTime;
                HypersonicBody.position = new Vector3(Lantern.position.x, Lantern.position.y, Toolbox.Instance.ZLayers["Hypersonic"]);
                HypersonicCollider.radius = ColliderMinScale - (ColliderMinScale - ColliderMaxScale) * (AnimTimer / AnimationDuration);
            }
            yield return null;
        }
        HypersonicSprite.enabled = false;
        HypersonicCollider.enabled = false;
    }
}
