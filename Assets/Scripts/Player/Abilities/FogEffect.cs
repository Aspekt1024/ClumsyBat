using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the "light source" of the Lantern
/// </summary>
public class FogEffect : MonoBehaviour {

    public Material material;
    private Transform Lantern;

    private const float FullSizeDuration = 7f;      // How long the light source stays at max after activation
    private const float EcholocateScale = 10f;      // How large the echo vision can get
    private const float VisionSetupTime = .7f;      // How long it takes the vision to increase to full size
    private float ShrinkDuration = 4f;              // How long the echo vision takes to fade
    private float MinFogScale = -0.4f;

    private float InitialScale;
    private float EchoScale;
    private float EcholocateActivatedTime;          // Time the echo locate ability was activated

    private bool bAbilityPaused = true;             // Handles whether the ability degenerates
    private bool bAbilityActivating = false;        // Quick animation to increase field of view

    private bool bIsMinimised = false;
    private float MinimiseStartTime = 0f;
    private float ScaleModifier = 1f;
    private const float MinimisedDuration = 5f;

    private const float PulseRadius = 0.4f;
    private const float PulseDuration = 0.6f;
    private float PulseTimer;
    private bool bPulseIncreasing = true;

    private StatsHandler Stats = null;

    void Start()
    {
        Stats = FindObjectOfType<StatsHandler>();
        Lantern = GameObject.Find("Lantern").GetComponent<Transform>();

        EchoScale = 0f;
        EcholocateActivatedTime = Time.time;
        material.SetVector("_PlayerPos", Lantern.position);
        material.SetFloat("_LightDist", EchoScale);
        material.SetFloat("_DarknessAlpha", 0.85f);

        bIsMinimised = false;
        
    }
    void Update()
    {
        if (bAbilityPaused)
        {
            EcholocateActivatedTime += Time.deltaTime;
        }
        else
        {
            EchoScale = GetEchoScale();
            if (EchoScale <= MinFogScale)
            {
                EchoScale = MinFogScale;
                Stats.DarknessTime += Time.deltaTime;
            }
            
            PulseTimer += Time.deltaTime;
            EchoScale += GetLightPulse();
            material.SetFloat("_LightDist", EchoScale);
        }
        Vector4 pos = Lantern.position;
        material.SetVector("_PlayerPos", pos);
    }

    private float GetLightPulse()
    {
        // Makes the light source breathe
        if (PulseTimer >= PulseDuration)
        {
            PulseTimer -= PulseDuration;
            bPulseIncreasing = !bPulseIncreasing;
        }
        float Pulse = PulseRadius * (PulseTimer / PulseDuration);
        return (bPulseIncreasing ? Pulse : (PulseRadius - Pulse));
    }

    private float GetEchoScale()
    {
        float EchoTimer;
        float EchoScale;

        if (bAbilityActivating)
        {
            // Startup animation - increase the vision
            EchoTimer = EcholocateActivatedTime + VisionSetupTime - Time.time;
            EchoScale = InitialScale + (EcholocateScale - InitialScale) * (1f - EchoTimer / VisionSetupTime);

            // If startup animation is complete, start diminishing the vision
            if (EchoTimer <= 0)
            {
                bAbilityActivating = false;
                EcholocateActivatedTime = Time.time;
            }
        }
        else if (EcholocateActivatedTime + FullSizeDuration < Time.time)
        {
            // Echolocate diminishing - decrease the vision
            EchoTimer = EcholocateActivatedTime + FullSizeDuration + ShrinkDuration - Time.time;
            EchoScale = EcholocateScale * EchoTimer / ShrinkDuration;
        }
        else
        {
            EchoScale = EcholocateScale;
        }

        EchoScale *= ScaleModifier;
            
        if (bIsMinimised && Time.time > MinimiseStartTime + MinimisedDuration)
        {
            StartCoroutine("ChangeScale", 1f);
            bIsMinimised = false;
        }

        return EchoScale;
    }

    public void Echolocate()
    {
        // Increase size from current scale to max scale
        InitialScale = EchoScale / ScaleModifier;
        bAbilityActivating = true;
        EcholocateActivatedTime = Time.time;
    }

    public void EndOfLevel()
    {
        bAbilityPaused = false;
        if (EcholocateActivatedTime + FullSizeDuration >= Time.time)
        {
            EcholocateActivatedTime = Time.time - FullSizeDuration;
        }
        MinFogScale = -3f;
        ShrinkDuration = 1.5f;
        StartCoroutine("FogFader");
    }

    public void StartOfLevel()
    {
        StartCoroutine("LevelStartAnim");
    }

    private IEnumerator FogFader()
    {
        const float StartAlpha = 0.85f;
        const float EndAlpha = 0f;
        const float AnimDuration = 3f;
        float AnimTimer = 0f;

        while(AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float Alpha = StartAlpha - (StartAlpha - EndAlpha) * (AnimTimer / AnimDuration);
            material.SetFloat("_DarknessAlpha", Alpha);
            yield return null;
        }
    }

    private IEnumerator LevelStartAnim()
    {
        bAbilityPaused = true;

        yield return new WaitForSeconds(1f);

        float AnimTime = 0f;
        const float AnimDuration = 2.5f;
        yield return null;

        while (AnimTime < AnimDuration)
        {
            AnimTime += Time.deltaTime;

            EchoScale = EcholocateScale * (AnimTime / AnimDuration);

            PulseTimer += Time.deltaTime;
            EchoScale += GetLightPulse();
            material.SetFloat("_LightDist", EchoScale);

            yield return null;
        }

        bAbilityPaused = false;
    }

    public void Minimise()
    {
        if (!bIsMinimised)
        {
            StartCoroutine("ChangeScale", 0.2f);
        }
        MinimiseStartTime = Time.time;
        bIsMinimised = true;
    }

    private IEnumerator ChangeScale(float Scale)
    {
        float StartModifier = ScaleModifier;
        float AnimTimer = 0f;
        const float AnimDuration = 0.5f;

        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            ScaleModifier = StartModifier - (StartModifier - Scale) * (AnimTimer / AnimDuration);
            yield return null;
        }
        ScaleModifier = Scale;
    }

    public void Pause() { bAbilityPaused = true; }
    public void Resume() { bAbilityPaused = false; }
}
