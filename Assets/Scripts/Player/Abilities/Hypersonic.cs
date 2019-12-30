using UnityEngine;
using System.Collections;
using ClumsyBat;
using ClumsyBat.DataContainers;
using ClumsyBat.Objects;

public class Hypersonic : MonoBehaviour
{

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
        gameObject.tag = "Hypersonic";
    }

    public void Setup(Lantern lanternRef)
    {
        _lantern = lanternRef;
    }

    public void SetData(AbilityContainer.AbilityType stats)
    {
        _numPulses = 1;
        _bCanDestroyStals = true;
        if (stats.AbilityLevel >= 2) { _numPulses = 2; }
        if (stats.AbilityLevel >= 4) { _numPulses = 3; }
        _bCanDestroyShrooms = stats.AbilityLevel >= 3;
        _bCanDestroySpiders = stats.AbilityLevel >= 5;
    }

    public bool ActivateHypersonic()
    {
        var hyperStats = GameStatics.Data.Abilities.GetHypersonicStats();
        if (hyperStats.AbilityAvailable)
        {
            StartCoroutine(HypersonicAbilityGo());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ForceHypersonic()
    {
        StartCoroutine(HypersonicAbilityGo());
        return true;
    }

    public void GamePaused(bool paused)
    {
        _bPaused = paused;
        _hypersonicAnimator.enabled = !paused;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Stalactite" && _bCanDestroyStals)
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
    }

    private IEnumerator HypersonicAbilityGo()
    {
        GameStatics.Data.Stats.HypersonicCount++;
        GameStatics.Camera.Shake(1f);
        GameStatics.Audio.Clumsy.PlaySound(ClumsySounds.Hypersonic);

        _hypersonicCollider.enabled = true;
        _hypersonicSprite.enabled = true;

        for (int i = 0; i < _numPulses; i++)
        {
            StartCoroutine(HypersonicAnimation());
            const float hypersonicIntervalTime = 0.7f;
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
                _hypersonicSprite.color = new Color(1, 1, 1, 1 - (animTimer / animationDuration));
            }
            yield return null;
        }
    }
}
