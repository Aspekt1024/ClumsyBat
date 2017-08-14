using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipButtonEffects : MonoBehaviour {

    public RectTransform LanternLight;
    public RectTransform LanternGlobe;

    [HideInInspector] public bool IsEnabled;

    private Image lightImage;
    private Image globeImage;
    private ParticleSystem shimmerEffect;
    private bool isPaused;
    private bool animationsPaused;

    public void DisplayIdle()
    {
        lightImage.enabled = false;
        globeImage.enabled = true;
        globeImage.color = new Color(0.4f, 0.4f, 0.4f, 0.4f);
        shimmerEffect.Stop();
    }

    public void ShowNewTip()
    {
        DisplayActive();
        StartCoroutine(PulseOn(LanternLight));
    }

    public void DisplayActive()
    {
        lightImage.enabled = true;
        globeImage.enabled = true;
        globeImage.color = Color.white;
        shimmerEffect.Play();
    }


    private void Start ()
    {
        lightImage = LanternLight.GetComponent<Image>();
        globeImage = LanternGlobe.GetComponent<Image>();
        shimmerEffect = LanternGlobe.GetComponentInChildren<ParticleSystem>();
        StartCoroutine(PulseImage(LanternLight, 1.7f));
        StartCoroutine(FloatImage(LanternGlobe, 0.8f));
        DisplayIdle();
	}

    private void Update()
    {
        if (isPaused == Toolbox.Instance.GamePaused) return;

        isPaused = Toolbox.Instance.GamePaused;
        if (isPaused)
            DisplayIdle();
        else if (IsEnabled)
            DisplayActive();
    }

    private IEnumerator PulseOn(RectTransform rt)
    {
        float timer = 0f;
        float flashTimer = 0f;
        const float flashDuration = 0.2f;
        const float interval = 0.84f;
        const int numPulses = 5;

        for (int i = 0; i < numPulses; i++)
        {
            flashTimer = 0f;
            timer = 0f;

            animationsPaused = true;
            float originalScale = rt.localScale.x;

            while (flashTimer < flashDuration)
            {
                rt.localScale = Vector3.one * Mathf.Lerp(0.1f, 2.7f, flashTimer / flashDuration);
                flashTimer += Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
            rt.localScale = Vector3.one * originalScale;
            animationsPaused = false;

            while (timer < interval)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

    }

    private IEnumerator PulseImage(RectTransform rt, float frequency)
    {
        float timer = 0f;
        bool isIncreasing = true;
        float originalSize = rt.localScale.x;
        const float minSize = 0.9f;
        const float maxSize = 1.1f;

        while (true)
        {
            if (!animationsPaused)
            {
                timer += Time.deltaTime;
                if (timer * frequency > 1f)
                {
                    timer = 0f;
                    isIncreasing = !isIncreasing;
                }
                float scale = 0f;
                if (isIncreasing)
                    scale = Mathf.Lerp(minSize, maxSize, timer * frequency);
                else
                    scale = Mathf.Lerp(maxSize, minSize, timer * frequency);

                rt.localScale = Vector3.one * scale;
            }
            yield return null;
        }
    }

    private IEnumerator FloatImage(RectTransform rt, float frequency)
    {
        float timer = 0f;
        bool isRising = false;
        float originalPos = rt.position.y;
        const float distance = 0.04f;

        while (true)
        {
            if (!animationsPaused)
            {
                timer += Time.deltaTime;
                if (timer * frequency > 1f)
                {
                    timer = 0f;
                    isRising = !isRising;
                }
                float yDiff = 0f;
                if (isRising)
                    yDiff = Mathf.Lerp(0f, -distance, timer * frequency);
                else
                    yDiff = Mathf.Lerp(-distance, 0f, timer * frequency);

                rt.position = new Vector3(rt.position.x, originalPos + yDiff, rt.position.z);
            }
            yield return null;
        }
    }
}
