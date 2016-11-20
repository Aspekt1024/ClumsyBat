using UnityEngine;
using System.Collections;

public class FogEffect : MonoBehaviour {

    public Material material;
    public GameObject Player;

    private const float EcholocateScale = 10f;      // How large the echo vision can get (should fill screen)
    private const float EcholocateDuration = 10f;   // How long the echo vision will fade
    private const float VisionSetupTime = .7f;      // How long it takes the vision to increase to full size
    private const float MinFogScale = -2f;          // Fade-off area is +3 to EchoScale

    private float InitialScale;
    private float EchoScale;
    private float EcholocateActivatedTime;   // Time the echo locate ability was activated

    private bool bAbilityPaused = true;      // Handles whether the ability degenerates
    private bool bAbilityActivating = false; // Quick animation to increase field of view
    private bool bPlayerIsAlive = true;

    private bool bIsMinimised = false;
    private float MinimiseStartTime;
    private const float MinimisedDuration = 5f;

    private StatsHandler Stats = null;

    void Start()
    {
        Stats = FindObjectOfType<StatsHandler>();

        EchoScale = EcholocateScale;
        EcholocateActivatedTime = Time.time;
        material.SetVector("_PlayerPos", Player.transform.position);
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
            material.SetFloat("_LightDist", EchoScale);
        }
        Vector4 pos = Player.transform.position;
        material.SetVector("_PlayerPos", pos);
    }

    float GetEchoScale()
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
        else
        {
            // Echolocate diminishing - decrease the vision
            EchoTimer = EcholocateActivatedTime + EcholocateDuration - Time.time;
            EchoScale = EcholocateScale * EchoTimer / EcholocateDuration;
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
        MinimiseStartTime = Time.time;
        bIsMinimised = true;
    }

    public void Pause() { bAbilityPaused = true; }
    public void Resume() { bAbilityPaused = false; }
}
