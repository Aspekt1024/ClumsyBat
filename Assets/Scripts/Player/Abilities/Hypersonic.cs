using UnityEngine;
using System.Collections;

public class Hypersonic : MonoBehaviour {

    private Transform HypersonicBody;
    private SpriteRenderer HypersonicSprite;
    private Animator HypersonicAnimator;

    private Transform Lantern;

	void Start ()
    {
        Lantern = GameObject.Find("Lantern").GetComponent<Transform>();
        HypersonicBody = GetComponent<Transform>();
        HypersonicAnimator = GetComponentInChildren<Animator>();
        HypersonicSprite = GetComponentInChildren<SpriteRenderer>();
        HypersonicSprite.enabled = false;
	}

    public void ActivateHypersonic()
    {
        StartCoroutine("HypersonicAnimation");
    }

    private IEnumerator HypersonicAnimation()
    {
        HypersonicSprite.enabled = true;
        HypersonicAnimator.Play("HyperGold", 0, 0f);

        const float AnimationDuration = 0.5f;
        float AnimTimer = 0f;

        while (AnimTimer < AnimationDuration)
        {
            AnimTimer += Time.deltaTime;
            HypersonicBody.position = new Vector3(Lantern.position.x, Lantern.position.y, Toolbox.Instance.ZLayers["Hypersonic"]);
            yield return null;
        }
        HypersonicSprite.enabled = false;
    }
}
