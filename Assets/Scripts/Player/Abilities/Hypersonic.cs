using UnityEngine;
using System.Collections;

public class Hypersonic : MonoBehaviour {

    private AbilityContainer.AbilityType _hyperStats;

    private int _numPulses;
    private bool _bCanDestroyStals;
    private bool _bCanDestroyShrooms;
    private bool _bCanDestroySpiders;

    private Transform _hypersonicBody;
    private SpriteRenderer _hypersonicSprite;
    private Animator _hypersonicAnimator;
    private CircleCollider2D _hypersonicCollider;

    private bool _bPaused;
    
    private Lantern _lantern;
    
    private void Start ()
    {
        _hypersonicBody = GetComponent<Transform>();
        _hypersonicAnimator = GetComponentInChildren<Animator>();
        _hypersonicSprite = GetComponentInChildren<SpriteRenderer>();
        _hypersonicSprite.enabled = false;
        _hypersonicCollider = GetComponent<CircleCollider2D>();
        _hypersonicCollider.radius = 0.01f;
        _hypersonicCollider.enabled = false;
    }

    public void Setup(Player playerRef, Lantern lanternRef)
    {
        _hyperStats = GameData.Instance.Data.AbilityData.GetHypersonicStats();
        
        _lantern = lanternRef;

        SetAbilityAttributes();
    }

    private void SetAbilityAttributes()
    {
        _numPulses = 1;
        _bCanDestroyStals = true;
        if (_hyperStats.AbilityLevel >= 2) { _numPulses = 2; }
        if (_hyperStats.AbilityLevel >= 4) { _numPulses = 3; }
        _bCanDestroyShrooms = _hyperStats.AbilityLevel >= 3;
        _bCanDestroySpiders = _hyperStats.AbilityLevel >= 5;
    }

    public void ActivateHypersonic()
    {
        if (_hyperStats.AbilityAvailable)
        {
            StartCoroutine("HypersonicAbilityGo");
        }
    }

    public void ForceHypersonic()
    {
        StartCoroutine("HypersonicAbilityGo");
    }

    public void GamePaused(bool paused)
    {
        _bPaused = paused;
        _hypersonicAnimator.enabled = !paused;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "StalObject" && _bCanDestroyStals)
        {
            other.GetComponentInParent<Stalactite>().DestroyStalactite();
        }
        if (other.name == "SpiderObject" && _bCanDestroySpiders)
        {
            other.GetComponent<SpiderClass>().DestroySpider();
        }
        if (other.name == "ShroomObject" && _bCanDestroyShrooms)
        {
            other.GetComponent<Mushroom>().DestroyMushroom();
        }
        if (other.GetComponent<Boss>() != null)
        {
            other.GetComponent<Boss>().HitByHypersonic();
        }
    }

    private IEnumerator HypersonicAbilityGo()
    {
        _hypersonicCollider.enabled = true;
        _hypersonicSprite.enabled = true;

        float hypersonicIntervalTime = 0.7f;
        for (int i = 0; i < _numPulses; i++)
        {
            yield return StartCoroutine("HypersonicAnimation");
            yield return new WaitForSeconds(hypersonicIntervalTime);
        }

        _hypersonicSprite.enabled = false;
        _hypersonicCollider.enabled = false;
    }

    private IEnumerator HypersonicAnimation()
    {
        _hypersonicAnimator.Play("HyperGold", 0, 0f);

        const float colliderMinScale = 0.01f;
        const float colliderMaxScale = 1.4f;

        const float animationDuration = 0.5f;
        float animTimer = 0f;

        while (animTimer < animationDuration)
        {
            if (!_bPaused)
            {
                animTimer += Time.deltaTime;
                _hypersonicBody.position = new Vector3(_lantern.transform.position.x, _lantern.transform.position.y, Toolbox.Instance.ZLayers["Hypersonic"]);
                _hypersonicCollider.radius = colliderMinScale - (colliderMinScale - colliderMaxScale) * (animTimer / animationDuration);
            }
            yield return null;
        }
    }
}
