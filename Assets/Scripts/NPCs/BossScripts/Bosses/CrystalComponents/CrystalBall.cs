﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBall : MonoBehaviour {

    public int ID;
    public Transform EndPosition;
    public float ActiveDuration;
    
    [HideInInspector] public HypersonicEventBoss Parent;
    [HideInInspector] public bool IsActive;

    private bool isDeactivating;

    private float activeTimer;
    private float rotationSpeed;
    private Animator mothAnim;
    private SpriteRenderer crystalLight;
    private SpriteRenderer crystalRenderer;
    private ParticleSystem effects;

    private const int numMoths = 20;
    private Animator[] moths;

    private BrokenCrystalBall brokenCrystal;
    
    private void Start ()
    {
        crystalRenderer = GetComponent<SpriteRenderer>();

		foreach(Transform tf in transform)
        {
            if (tf.name == "Moth")
            {
                mothAnim = tf.GetComponent<Animator>();
            }
            else if (tf.name == "Light")
            {
                crystalLight = tf.GetComponent<SpriteRenderer>();
            }
        }

        effects = Instantiate(Resources.Load<GameObject>("Effects/CrystalShimmer")).GetComponent<ParticleSystem>();
        effects.transform.SetParent(transform);
        effects.transform.position = transform.position;
        effects.Stop();
        crystalLight.enabled = false;
        mothAnim.Play("MothGoldCaptured");
        mothAnim.speed = 0;

        SetupMoths();
        brokenCrystal = Instantiate(Resources.Load<GameObject>("Collectibles/BrokenCrystalBall")).GetComponent<BrokenCrystalBall>();
        brokenCrystal.transform.SetParent(transform);
        brokenCrystal.transform.position = transform.position;
    }
	
	private void Update () {
        if (!Toolbox.Instance.GamePaused && IsActive)
        {
            activeTimer += Time.deltaTime;
            if (activeTimer > ActiveDuration)
            {
                StartCoroutine(Deactivate());
            }
        }

        rotationSpeed = Mathf.Lerp(rotationSpeed, Random.Range(5f, 100f), Time.deltaTime);
        mothAnim.transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);

	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (Parent.EventStarted && (!IsActive || isDeactivating))
            {
                StartCoroutine(Activate());
            }

            Parent.CrystalTriggered(ID);
        }
        else if (other.tag == "Hypersonic")
        {
            Shatter();
        }

    }

    private IEnumerator Activate()
    {
        isDeactivating = false;
        IsActive = true;
        activeTimer = 0f;
        crystalLight.enabled = true;
        crystalLight.transform.localScale = Vector3.one;
        crystalRenderer.color = new Color(212/255f,195/255f,126/255f);
        mothAnim.speed = Random.Range(0.5f, 1f);
        effects.Play();
        
        yield return StartCoroutine(PulseActive());
        StartCoroutine(PulseLight());
    }

    private IEnumerator Deactivate()
    {
        const float animDuration = 0.4f;
        float animTimer = 0f;
        float targetScale = 0.1f;

        isDeactivating = true;

        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                float scale = Mathf.Lerp(0.6f, targetScale, animTimer / animDuration);
                crystalLight.transform.localScale = new Vector3(scale, scale, 1f);
            }
            if (!isDeactivating) yield break;

            yield return null;
        }
        IsActive = false;
        crystalLight.enabled = false;
        crystalRenderer.color = Color.white;
        isDeactivating = false;
        mothAnim.Play("MothGoldCaptured", 0, 0f);
        mothAnim.speed = 0;
    }

    private IEnumerator PulseLight()
    {
        const float minScale = 0.6f;
        const float maxScale = 0.75f;
        const float period = 0.4f;

        bool isIncreasing = false;
        float scale = 1f;
        float timer = 0f;

        crystalLight.enabled = true;
        crystalLight.color = Color.white;
        while (IsActive && !isDeactivating)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                if (timer > period)
                {
                    timer = 0;
                    isIncreasing = !isIncreasing;
                }

                if (isIncreasing)
                    scale = Mathf.Lerp(minScale, maxScale, timer / period);
                else
                    scale = Mathf.Lerp(maxScale, minScale, timer / period);

                crystalLight.transform.localScale = new Vector3(scale, scale, 1f);
            }
            yield return null;
        }
    }

    public void Explode()
    {
        StartCoroutine(PulseActive());
        StartCoroutine(SpawnMothEssence());

        isDeactivating = false;
        IsActive = false;
        crystalRenderer.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);

        mothAnim.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void SetupMoths()
    {
        moths = new Animator[numMoths];
        for (int i = 0; i < numMoths; i++)
        {
            moths[i] = Instantiate(Resources.Load<GameObject>("Collectibles/MothEssence")).GetComponent<Animator>();
            moths[i].transform.position = Toolbox.Instance.HoldingArea;
        }
    }
    
    private IEnumerator SpawnMothEssence()
    {
        Vector2[] essencePositions = new Vector2[numMoths];
        float[] essenceDelays = new float[numMoths];
        bool[] essenceCollections = new bool[numMoths];
        for (int i = 0; i < numMoths; i++)
        {
            essencePositions[i] = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
            essenceDelays[i] = Random.Range(0f, 4f);
            moths[i].Play("MothGoldCaptured", 0, 0f);
            moths[i].speed = 0f;
            moths[i].transform.position = transform.position;
            moths[i].transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }
        StartCoroutine(MoveMothEssence());

        float timer = 0f;
        float duration = 1f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < numMoths; i++)
            {
                moths[i].transform.position = Vector2.Lerp(transform.position, essencePositions[i], timer / duration);
            }
            yield return null;
        }
        
        timer = 0f;
        int essenceCollected = 0;
        while (essenceCollected < numMoths)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < numMoths; i++)
            {
                if (timer > essenceDelays[i])
                {
                    float ratio = (timer - essenceDelays[i]) / duration;
                    if (ratio <= 1)
                    {
                        moths[i].transform.position = Vector2.Lerp(essencePositions[i], Toolbox.Player.Lantern.transform.position, ratio);
                    }
                    else if (!essenceCollections[i])
                    {
                        essenceCollections[i] = true;
                        essenceCollected++;
                        moths[i].GetComponent<SpriteRenderer>().enabled = false;
                        Toolbox.Player.Lantern.ChangeColour(Lantern.LanternColour.Gold);
                        // TODO play sound
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator MoveMothEssence()
    {
        while (true)
        {
            for (int i = 0; i < numMoths; i++)
            {
                moths[i].transform.position = Vector3.Lerp(moths[i].transform.position, moths[i].transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f), Time.deltaTime);
                moths[i].transform.localScale = Vector2.Lerp(moths[i].transform.localScale, Vector3.one * Random.Range(0.2f, 0.7f), Time.deltaTime);
            }
            yield return null;
        }
    }

    public IEnumerator CrystalFloat()
    {
        StartCoroutine(PulseActive());

        bool isRising = Random.Range(0, 2) == 1;

        const float height = 0.1f;
        float lowPoint = EndPosition.position.y - height / 2f;
        float period = Random.Range(0.9f, 1.5f);
        float timer = 0f;

        while (true)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                if (timer > period)
                {
                    isRising = !isRising;
                    timer = 0f;
                }
                float yPos = lowPoint;
                if (isRising)
                {
                    yPos += Mathf.Lerp(0f, height, timer / period);
                }
                else
                {
                    yPos += Mathf.Lerp(height, 0f, timer / period);
                }
                transform.position = new Vector3(transform.position.x,yPos, transform.position.z);
            }
            yield return null;
        }
    }

    private IEnumerator PulseActive()
    {
        effects.Play();

        float timer = 0f;
        const float duration = 0.3f;

        crystalLight.enabled = true;
        while (timer < duration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                crystalLight.transform.localScale = Vector3.Lerp(Vector2.one * 0.1f, Vector2.one * 5f, timer / duration);
                crystalLight.color = new Color(1, 1, 1, 1 - timer / duration);
            }
            yield return null;
        }
        crystalLight.enabled = false;
    }

    private void Shatter()
    {
        brokenCrystal.gameObject.SetActive(true);
        brokenCrystal.Shatter();
        crystalRenderer.enabled = false;
    }
}
