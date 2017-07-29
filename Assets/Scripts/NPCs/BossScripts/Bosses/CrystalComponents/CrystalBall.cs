using System.Collections;
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
        crystalLight.enabled = false;
        mothAnim.Play("MothGoldCaptured");
        mothAnim.speed = 0;
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
        if (Parent.EventStarted && (!IsActive || isDeactivating))
        {
            StartCoroutine(Activate());
        }

        Parent.CrystalTriggered(ID);
    }

    private IEnumerator Activate()
    {
        const float animDuration = 0.4f;
        float animTimer = 0f;

        isDeactivating = false;
        IsActive = true;
        activeTimer = 0f;
        crystalLight.enabled = true;
        crystalRenderer.color = new Color(212/255f,195/255f,126/255f);
        mothAnim.speed = Random.Range(0.5f, 1f);

        crystalLight.transform.localScale = Vector2.one * 0.01f;
        float targetScale = 0.75f;

        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                crystalLight.transform.localScale = Vector2.Lerp(crystalLight.transform.localScale, Vector2.one * targetScale, animTimer / animDuration);
            }
            yield return null;
        }

        StartCoroutine(PulseLight());
    }

    private IEnumerator Deactivate()
    {
        const float animDuration = 0.7f;
        float animTimer = 0f;
        float targetScale = 0.1f;

        isDeactivating = true;

        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                crystalLight.transform.localScale = Vector2.Lerp(Vector2.one * 0.6f, Vector2.one * targetScale, animTimer / animDuration);
            }
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
        float scale = crystalLight.transform.localScale.x;
        float timer = 0f;

        while (IsActive || !isDeactivating)
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
                {
                    scale = Mathf.Lerp(minScale, maxScale, timer / period);
                }
                else
                {
                    scale = Mathf.Lerp(maxScale, minScale, timer / period);
                }
                crystalLight.transform.localScale = Vector2.one * scale;
            }
            yield return null;
        }
    }

    public IEnumerator CrystalFloat()
    {
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
}
