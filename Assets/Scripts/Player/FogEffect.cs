using UnityEngine;
using System.Collections;

public class FogEffect : MonoBehaviour {

    public Material material;
    private Transform Lantern;

    private const float FullSizeDuration = 7f;      // How long the light source stays at max after activation
    private const float EcholocateScale = 10f;      // How large the echo vision can get
    private const float ShrinkDuration = 4f;        // How long the echo vision takes to fade
    private const float VisionSetupTime = .7f;      // How long it takes the vision to increase to full size
    private const float MinFogScale = -0.4f;

    private float InitialScale;
    private float EchoScale;
    private float EcholocateActivatedTime;   // Time the echo locate ability was activated

    private bool bAbilityPaused = true;      // Handles whether the ability degenerates
    private bool bAbilityActivating = false; // Quick animation to increase field of view
    private bool bPlayerIsAlive = true;

    private bool bIsMinimised = false;
    private float MinimiseStartTime;
    private const float MinimisedDuration = 5f;

    private const float PulseRadius = 0.4f;
    private const float PulseDuration = 0.6f;
    private float PulseTimer;
    private bool bPulseIncreasing = true;

    private StatsHandler Stats = null;

    void Start()
    {
        Stats = FindObjectOfType<StatsHandler>();
        Lantern = GameObject.Find("Clumsy").GetComponent<Transform>();  // TODO Change Clumsy to Lantern once it can be centered

        EchoScale = EcholocateScale;
        EcholocateActivatedTime = Time.time;
        material.SetVector("_PlayerPos", Lantern.position);
        material.SetFloat("_LightDist", EcholocateScale);

        bIsMinimised = false;
        
    }
    void Update()
    {
        if (!bPlayerIsAlive) { return; }

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
            if (PulseTimer >= PulseDuration)
            {
                PulseTimer -= PulseDuration;
                bPulseIncreasing = !bPulseIncreasing;
            }
            float Pulse = PulseRadius * (PulseTimer / PulseDuration);
            EchoScale += (bPulseIncreasing ? Pulse : (PulseRadius - Pulse));
            material.SetFloat("_LightDist", EchoScale);
        }
        Vector4 pos = Lantern.position;
        material.SetVector("_PlayerPos", pos);
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

        if (bIsMinimised)
        {
            if (MinimiseStartTime + MinimisedDuration < Time.time)
            {
                bIsMinimised = false;
            }
            else
            {
                EchoScale *= 0.2f;
            }
        }

        return EchoScale;
    }

    public void Echolocate()
    {
        // Increase size from current scale to max scale
        InitialScale = EchoScale;
        bAbilityActivating = true;
        EcholocateActivatedTime = Time.time;
    }

    public void PlayerDeath()
    {
        bPlayerIsAlive = false;
        EchoScale = MinFogScale;
    }

    public void Minimise()
    {
        // TODO give this an animation to decrease and increase FoV
        MinimiseStartTime = Time.time;
        bIsMinimised = true;
    }

    public void Pause() { bAbilityPaused = true; }
    public void Resume() { bAbilityPaused = false; }
}
