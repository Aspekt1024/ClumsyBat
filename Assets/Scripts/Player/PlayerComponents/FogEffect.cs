using UnityEngine;
using System.Collections;
using ClumsyBat;
using ClumsyBat.Players;

/// <summary>
/// Handles the "light source" of the Lantern
/// </summary>
public class FogEffect : MonoBehaviour {

    public Material Material;
    private Transform _lantern;
    private Player _player;

    private const float FullSizeDuration = 7f;      // How long the light source stays at max after activation
    private const float EcholocateScale = 10f;      // How large the echo vision can get
    private const float VisionSetupTime = .7f;      // How long it takes the vision to increase to full size
    private float _shrinkDuration = 4f;             // How long the echo vision takes to fade
    private float _minFogScale = -0.4f;

    private float _initialScale;
    private float _echoScale;
    private float _echolocateActivatedTime;         // Time the echo locate ability was activated

    private bool _bAbilityPaused = true;            // Handles whether the ability degenerates
    private bool _bAbilityActivating;               // Quick animation to increase field of view
    private bool _bAbilityAnimating;                // Startup Animation flag

    private bool _bIsMinimised;
    private float _minimiseStartTime;
    private float _scaleModifier = 1f;
    private const float MinimisedDuration = 5f;

    private const float PulseRadius = 0.4f;
    private const float PulseDuration = 0.6f;
    private float _pulseTimer;
    private bool _bPulseIncreasing = true;
    
    private Coroutine levelStartRoutine;
    private Coroutine changeScaleRoutine;
    private Coroutine expandRoutine;
    private Coroutine fadeRoutine;

    private enum FogStates
    {
        Normal,
        ExpandingToRemove,
        Disabled
    }
    private FogStates _state;
    
    private static readonly int LightDist = Shader.PropertyToID("_LightDist");
    private static readonly int PlayerPos = Shader.PropertyToID("_PlayerPos");
    private static readonly int DarknessAlpha = Shader.PropertyToID("_DarknessAlpha");

    public void Echolocate()
    {
        // Increase size from current scale to max scale
        _initialScale = _echoScale / _scaleModifier;
        _bAbilityActivating = true;
        _echolocateActivatedTime = Time.time;
    }

    public void EndOfLevel()
    {
        _bAbilityPaused = false;
        if (_echolocateActivatedTime + FullSizeDuration >= Time.time)
        {
            _echolocateActivatedTime = Time.time - FullSizeDuration;
        }
        _minFogScale = -3f;
        _shrinkDuration = 1.5f;
        fadeRoutine = StartCoroutine(FogFader());
    }

    public void Minimise()
    {
        if (!_bIsMinimised)
        {
            if (changeScaleRoutine != null) StopCoroutine(changeScaleRoutine);
            changeScaleRoutine = StartCoroutine(ChangeScale(0.2f));
        }
        _minimiseStartTime = Time.time;
        _bIsMinimised = true;
    }
    
    public void StartOfLevel()
    {
        if (!GameStatics.LevelManager.IsBossLevel)
        {
            SetInitialState();
        }
        levelStartRoutine = StartCoroutine(LevelStartAnim());
    }

    public void ExpandToRemove()
    {
        _state = FogStates.ExpandingToRemove;
        if (expandRoutine != null) StopCoroutine(expandRoutine);
        expandRoutine = StartCoroutine(ExpandFogCompletely());
    }

    public void Disable()
    {
        _state = FogStates.Disabled;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Pause() { _bAbilityPaused = true; }
    public void Resume() { _bAbilityPaused = false; }

    private void Start()
    {
        _player = GameStatics.Player.Clumsy;
        _lantern = _player.lantern.transform;

        Initialise();
    }

    public void Initialise()
    {
        _echolocateActivatedTime = Time.time;

        _state = FogStates.Normal;
        _bAbilityPaused = true;
        _bIsMinimised = false;
        _echoScale = -3f;

        Material.SetVector(PlayerPos, _lantern.position);
        Material.SetFloat(LightDist, _echoScale);
        Material.SetFloat(DarknessAlpha, 0.85f);
        
    }

    private void FixedUpdate()
    {
        var tf = transform;
        tf.position = new Vector3(GameStatics.Camera.CurrentCamera.transform.position.x, 0f, tf.position.z);
        Material.SetVector(PlayerPos, _lantern.position);
    }

    private void Update()
    {
        if (_state == FogStates.Disabled || _state == FogStates.ExpandingToRemove) { return; }
        if (_bAbilityPaused || _bAbilityAnimating)
        {
            _echolocateActivatedTime += Time.deltaTime;
        }
        else
        {
            if (_player != null && _player.State.IsPerched)
                _echolocateActivatedTime += Time.deltaTime * 2f / 3;

            _echoScale = GetEchoScale();
            if (_echoScale <= _minFogScale)
            {
                _echoScale = _minFogScale;
                GameStatics.Data.Stats.DarknessTime += Time.deltaTime;
            }
            
            _pulseTimer += Time.deltaTime;
            _echoScale += GetLightPulse();
            Material.SetFloat(LightDist, _echoScale);
        }
    }

    private float GetLightPulse()
    {
        // Makes the light source breathe
        if (_pulseTimer >= PulseDuration)
        {
            _pulseTimer -= PulseDuration;
            _bPulseIncreasing = !_bPulseIncreasing;
        }
        float pulse = PulseRadius * (_pulseTimer / PulseDuration);
        return (_bPulseIncreasing ? pulse : (PulseRadius - pulse));
    }

    private float GetEchoScale()
    {
        float echoTimer;
        float echoScale;

        if (_bAbilityActivating)
        {
            // Startup animation - increase the vision
            echoTimer = _echolocateActivatedTime + VisionSetupTime - Time.time;
            echoScale = _initialScale + (EcholocateScale - _initialScale) * (1f - echoTimer / VisionSetupTime);

            // If startup animation is complete, start diminishing the vision
            if (echoTimer <= 0)
            {
                _bAbilityActivating = false;
                _echolocateActivatedTime = Time.time;
            }
        }
        else if (_echolocateActivatedTime + FullSizeDuration < Time.time)
        {
            // Echolocate diminishing - decrease the vision
            echoTimer = _echolocateActivatedTime + FullSizeDuration + _shrinkDuration - Time.time;
            echoScale = EcholocateScale * echoTimer / _shrinkDuration;
        }
        else
        {
            echoScale = EcholocateScale;
        }

        echoScale *= _scaleModifier;
            
        if (_bIsMinimised && Time.time > _minimiseStartTime + MinimisedDuration)
        {
            if (changeScaleRoutine != null) StopCoroutine(changeScaleRoutine);
            StartCoroutine(ChangeScale(1f));
            _bIsMinimised = false;
        }

        return echoScale;
    }

    private IEnumerator LevelStartAnim()
    {
        _echoScale = -3f;
        
        _bAbilityPaused = false;
        _bAbilityAnimating = true;

        _echolocateActivatedTime = Time.time;

        float animTime = 0f;
        const float animDuration = 0.7f;
        while (animTime < animDuration)
        {
            animTime += Time.deltaTime;

            _echoScale = (EcholocateScale - 3 * (1 - animTime / animDuration)) * (animTime / animDuration);

            _pulseTimer += Time.deltaTime;
            _echoScale += GetLightPulse();
            Material.SetFloat(LightDist, _echoScale);

            yield return null;
        }
        _bAbilityAnimating = false;
    }

    private IEnumerator FogFader()
    {
        const float startAlpha = 0.85f;
        const float endAlpha = 0f;
        const float animDuration = 3f;
        float animTimer = 0f;

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            float alpha = startAlpha - (startAlpha - endAlpha) * (animTimer / animDuration);
            Material.SetFloat(DarknessAlpha, alpha);
            yield return null;
        }
    }
    
    private IEnumerator ChangeScale(float scale)
    {
        float startModifier = _scaleModifier;
        float animTimer = 0f;
        const float animDuration = 0.5f;

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            _scaleModifier = startModifier - (startModifier - scale) * (animTimer / animDuration);
            yield return null;
        }
        _scaleModifier = scale;
    }


    private IEnumerator ExpandFogCompletely()
    {
        float animTimer = 0f;
        const float animDuration = 1.5f;
        float echoStartScale = _echoScale;
        float echoEndScale = EcholocateScale * 2f;
        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            float scale = echoStartScale - (echoStartScale - echoEndScale) * (animTimer / animDuration);
            Material.SetFloat(LightDist, scale);
            yield return null;
        }
        Disable();
    }

    private void SetInitialState()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (expandRoutine != null) StopCoroutine(expandRoutine);
        if (changeScaleRoutine != null) StopCoroutine(changeScaleRoutine);
        if (levelStartRoutine != null) StopCoroutine(levelStartRoutine);
        
        _bIsMinimised = false;
        _bAbilityActivating = false;
        _bAbilityPaused = false;
        _bPulseIncreasing = false;
        _bAbilityAnimating = false;
        _scaleModifier = 1f;
        _echolocateActivatedTime = Time.time;
        const float startAlpha = 0.85f;
        
        GetComponent<SpriteRenderer>().enabled = true;
        Material.SetFloat(DarknessAlpha, startAlpha);
        _state = FogStates.Normal;
        
    }
}
