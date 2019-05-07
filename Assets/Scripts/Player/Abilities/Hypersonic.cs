﻿using UnityEngine;
using System.Collections;
using ClumsyBat;
using ClumsyBat.DataContainers;
using ClumsyBat.Objects;
using ClumsyBat.Players;

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

    private AbilityContainer.AbilityType _hyperStats;
    
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

    public void Setup(Lantern lanternRef)
    {
        _lantern = lanternRef;
    }

    public void SetData(AbilityContainer.AbilityType stats)
    {
        _hyperStats = stats;

        _numPulses = 1;
        _bCanDestroyStals = true;
        if (_hyperStats.AbilityLevel >= 2) { _numPulses = 2; }
        if (_hyperStats.AbilityLevel >= 4) { _numPulses = 3; }
        _bCanDestroyShrooms = _hyperStats.AbilityLevel >= 3;
        _bCanDestroySpiders = _hyperStats.AbilityLevel >= 5;
    }

    public bool ActivateHypersonic()
    {
        var hyperStats = GameStatics.Data.Abilities.GetHypersonicStats();
        if (_hyperStats.AbilityAvailable)
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
        CameraEventListener.CameraShake(1f);
        // TODO play sound

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
                _hypersonicSprite.color = new Color(1, 1, 1, 1 - (animTimer / animationDuration));
            }
            yield return null;
        }
    }
}
