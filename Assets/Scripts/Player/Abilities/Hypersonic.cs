using UnityEngine;
using System.Collections;

public class Hypersonic : MonoBehaviour {

    private bool bPaused = false;
    private bool bEnabled = false;
    private int AbilityLevel;

    private int NumPulses;
    private bool bCanDestroyStals;
    private bool bCanDestroyShrooms;
    private bool bCanDestroySpiders;

    private Transform HypersonicBody;
    private SpriteRenderer HypersonicSprite;
    private Animator HypersonicAnimator;
    private CircleCollider2D HypersonicCollider;

    StatsHandler Stats = null;
    private Lantern Lantern;

	void Start ()
    {
        HypersonicBody = GetComponent<Transform>();
        HypersonicAnimator = GetComponentInChildren<Animator>();
        HypersonicSprite = GetComponentInChildren<SpriteRenderer>();
        HypersonicSprite.enabled = false;
        HypersonicCollider = GetComponent<CircleCollider2D>();
        HypersonicCollider.radius = 0.01f;
        HypersonicCollider.enabled = false;
    }

    public void Setup(StatsHandler StatsRef, PlayerController PlayerRef, Lantern LanternRef)
    {
        Stats = StatsRef;
        bEnabled = Stats.AbilityData.GetHypersonicStats().AbilityAvailable;
        AbilityLevel = Stats.AbilityData.GetHypersonicStats().AbilityLevel;
        
        Lantern = LanternRef;

        SetAbilityAttributes();
    }

    private void SetAbilityAttributes()
    {
        NumPulses = 1;
        bCanDestroyStals = true;
        if (AbilityLevel >= 2) { NumPulses = 2; }
        if (AbilityLevel >= 3) { bCanDestroyShrooms = true; } else { bCanDestroyShrooms = false; }
        if (AbilityLevel >= 4) { NumPulses = 3; }
        if (AbilityLevel >= 5) { bCanDestroySpiders = true; } else { bCanDestroySpiders = false; }
    }

    public void ActivateHypersonic()
    {
        if (bEnabled || !bEnabled)
        {
            StartCoroutine("HypersonicAbilityGO");
        }
    }

    public void GamePaused(bool Paused)
    {
        bPaused = Paused;
        HypersonicAnimator.enabled = !Paused;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "StalObject" && bCanDestroyStals)
        {
            other.GetComponentInParent<Stalactite>().DestroyStalactite();
        }
        if (other.name == "SpiderObject" && bCanDestroySpiders)
        {
            other.GetComponent<SpiderClass>().DestroySpider();
        }
        if (other.name == "ShroomObject" && bCanDestroyShrooms)
        {
            other.GetComponent<Mushroom>().DestroyMushroom();
        }
    }

    private IEnumerator HypersonicAbilityGO()
    {
        HypersonicCollider.enabled = true;
        HypersonicSprite.enabled = true;

        float HypersonicIntervalTime = 0.7f;
        for (int i = 0; i < NumPulses; i++)
        {
            yield return StartCoroutine("HypersonicAnimation");
            yield return new WaitForSeconds(HypersonicIntervalTime);
        }

        HypersonicSprite.enabled = false;
        HypersonicCollider.enabled = false;
    }

    private IEnumerator HypersonicAnimation()
    {
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
                HypersonicBody.position = new Vector3(Lantern.transform.position.x, Lantern.transform.position.y, Toolbox.Instance.ZLayers["Hypersonic"]);
                HypersonicCollider.radius = ColliderMinScale - (ColliderMinScale - ColliderMaxScale) * (AnimTimer / AnimationDuration);
            }
            yield return null;
        }
    }
}
