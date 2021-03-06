﻿using UnityEngine;
using System.Collections;
using ClumsyBat.Objects;
using ClumsyBat;

public class StalDropComponent : MonoBehaviour {

    [HideInInspector]
    public SpriteRenderer TriggerSprite;
    private Stalactite stal;
    private StalAnimationHandler anim;
    private Rigidbody2D stalBody;

    [HideInInspector]
    public const float FallDuration = 1.2f;
    [HideInInspector]
    public const float FallDistance = 20f;

    private bool isPaused;
    private Vector2 storedVelocity = Vector2.zero;
    private bool storedKinematicState;
    private const float shakeThresholdX = 6f;
    private GameObject rubblePrefab;
    private ParticleSystem rubbleEffect;

    private enum DropStates
    {
        None,
        Shaking,
        Falling
    }
    private DropStates _state;

    private void Awake()
    {
        TriggerSprite = GetComponent<SpriteRenderer>();
        stal = GetComponent<Stalactite>();
        stalBody = transform.GetComponent<Rigidbody2D>();
        
        rubblePrefab = Resources.Load<GameObject>("Obstacles/Stalactite/RubbleEffect");
    }

    private void FixedUpdate()
    {
        if (!GameStatics.StaticsInitiated) return;
        if (!stal.DropEnabled || !stal.isActiveAndEnabled || isPaused || _state == DropStates.Falling || !GameStatics.Player.Clumsy.State.IsAlive) return;
        
        if (GameStatics.Player.Clumsy.model.position.x > transform.position.x - stal.TriggerPosX)
        {
            Drop();
        }
        else
        {
            if (GameStatics.Player.Clumsy.model.position.x > transform.position.x - stal.TriggerPosX - shakeThresholdX && _state == DropStates.None)
            {
                StartCoroutine(Shake());
            }
        }
    }

    private void Update()
    {
        if (!GameStatics.StaticsInitiated) return;
        if (!GameStatics.Player.Clumsy.State.IsAlive && _state == DropStates.Shaking)
        {
            _state = DropStates.None;
        }
    }

    public void SetAnim(StalAnimationHandler animRef)
    {
        anim = animRef;
    }

    public void NewStalactite()
    {
        _state = DropStates.None;
        StopAllCoroutines();
        TriggerSprite.enabled = Toolbox.Instance.Debug && stal.DropEnabled;
    }

    public void Exploded()
    {
        if (rubbleEffect != null)
        {
            rubbleEffect.Stop();
        }
    }
    
    public void Drop()
    {
        if (_state != DropStates.Falling && stal.IsActive)
        {
            _state = DropStates.Falling;
            stal.SetState(Stalactite.StalStates.Falling);
            StartCoroutine(DropSequence());
        }
    }
    
    private IEnumerator DropSequence()
    {
        if (anim != null)
        {
            if (stal.SpawnDirection == SpawnStalAction.StalSpawnDirection.FromTop)
            {
                CreateRubbleEffect();
            }

            anim.CrackAndFall();
            while (!anim.ReadyToFall() || isPaused)
            {
                yield return null;
            }

            if (rubbleEffect != null)
            {
                rubbleEffect.Stop();
            }
        }
        
        GameStatics.Audio.Enemy.PlaySound(EnemySounds.StalactiteFall);
        float fallTime = 0f;
        float startingYPos = stalBody.transform.position.y;
        while (fallTime < FallDuration)
        {
            if (!isPaused)
            {
                fallTime += Time.deltaTime;
                float yPos = startingYPos - FallDistance * Mathf.Pow((fallTime / FallDuration), 2);
                stalBody.transform.position = new Vector3(stalBody.transform.position.x, yPos, stalBody.transform.position.z);
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                yield return null;
            }
        }
        stal.Deactivate();
    }
    
    IEnumerator Shake()
    {
        CreateRubbleEffect();
        GameStatics.Audio.Enemy.PlaySound(EnemySounds.StalactiteCreak);
        
        _state = DropStates.Shaking;
        const float shakeInterval = 0.07f;
        const float shakeIntensity = 1.8f;
        bool bRotateForward = true;
        while (_state == DropStates.Shaking)
        {
            if (!isPaused)
            {
                stalBody.transform.Rotate(new Vector3(0, 0, (bRotateForward ? shakeIntensity : -shakeIntensity)));
                bRotateForward = !bRotateForward;
            }
            yield return new WaitForSeconds(shakeInterval);
        }
    }

    private void CreateRubbleEffect()
    {
        if (rubbleEffect != null)
        {
            rubbleEffect.Play();
            return;
        }
        rubbleEffect = Instantiate(rubblePrefab).GetComponent<ParticleSystem>();
        rubbleEffect.transform.position = new Vector3(transform.position.x, 5f, transform.position.z - 0.1f);
        Destroy(rubbleEffect.gameObject, 3f);
    }

    public void SetPaused(bool bPaused)
    {
        if (stalBody == null) return;

        isPaused = bPaused;
        if (_state == DropStates.Falling)
        {
            if (isPaused)
            {
                storedVelocity = stalBody.velocity;
                stalBody.velocity = Vector2.zero;
                storedKinematicState = stalBody.isKinematic;
                stalBody.isKinematic = true;
            }
            else
            {
                stalBody.isKinematic = storedKinematicState;
                stalBody.velocity = storedVelocity;
            }
        }
    }
}
