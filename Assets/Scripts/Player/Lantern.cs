using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the physics of the Lantern carried by Clumsy
/// </summary>
public class Lantern : MonoBehaviour {

    private HingeJoint2D _lanternHinge;
    private Rigidbody2D _lanternBody;
    private PolygonCollider2D _lanternCollider;
    private SpriteRenderer _globe;
    private SpriteRenderer _light;
    private ParticleSystem shimmerEffect;
    
    public enum LanternColour
    {
        White,
        Green,
        Gold,
        Blue
    }
    private LanternColour _colour = LanternColour.Green;
    private bool _paused;
    private bool _bDropped;
    private bool _bColourChanging;
    private float _storedAngularVelocity;
    private Vector2 _storedVelocity;
    private Vector2 _lightScale;     // Flicker and change colour will be centered around the initial scale

    private void Start ()
    {
        _lanternHinge = GetComponent<HingeJoint2D>();
        _lanternBody = GetComponent<Rigidbody2D>();
        _lanternCollider = GetComponent<PolygonCollider2D>();
        GetChildComponents();

        _lanternCollider.enabled = false;
        _lightScale = _light.transform.localScale;
        SetColour();

        StartCoroutine(Flicker());
    }

    private void Update()
    {
        if (_lanternBody.velocity.magnitude < 0.1f)
        {
            shimmerEffect.Stop();
        }
        else if (shimmerEffect.isStopped)
        {
            shimmerEffect.Play();
        }
    }

    private IEnumerator Flicker()
    {
        const float period = 0.2f;
        float timer = 0f;
        bool bGrowing = true;
        const float size = 0.1f;
        
        while (true)
        {
            if (!_paused && !_bColourChanging)
            {
                timer += Time.deltaTime;
                if (timer >= period)
                {
                    timer -= period;
                    bGrowing = !bGrowing;
                }
                if (bGrowing)
                {
                    _light.transform.localScale = _lightScale * (1f + size * timer / period);
                }
                else
                {
                    _light.transform.localScale = _lightScale * (1f + size - size * timer / period);
                }
            }
            yield return null;
        }
    }

    private void GetChildComponents()
    {
        foreach (Transform childObj in transform)
        {
            switch (childObj.name)
            {
                case "LanternGlobe":
                    _globe = childObj.GetComponent<SpriteRenderer>();
                    break;
                case "LanternLight":
                    _light = childObj.GetComponent<SpriteRenderer>();
                    break;
                case "LanternShimmer":
                    shimmerEffect = childObj.GetComponent<ParticleSystem>();
                    break;
            }
        }
    }

    public void Drop()
    {
        Transform newObj = new GameObject("FreeLantern").transform;
        transform.parent = newObj;
        _bDropped = true;
        gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        _lanternHinge.enabled = false;
        _lanternBody.velocity = new Vector2(Random.Range(1f, 5f), 1f);
        _lanternBody.AddTorque(Random.Range(100f, 600f));
    }

    public void AddRushForce()
    {
        JointMotor2D lanternMotor = _lanternHinge.motor;
        lanternMotor.motorSpeed = 1000;
        _lanternHinge.motor = lanternMotor;
        StartCoroutine(EngageMotor());
    }

    private IEnumerator EngageMotor()
    {
        _lanternHinge.useMotor = true;
        shimmerEffect.Play();
        yield return new WaitForSeconds(0.2f);
        _lanternHinge.useMotor = false;
        shimmerEffect.Stop();
    }

    public void GamePaused(bool bPaused)
    {
        _paused = bPaused;
        if (_paused)
        {
            _storedVelocity = _lanternBody.velocity;
            _storedAngularVelocity = _lanternBody.angularVelocity;
            _lanternBody.angularVelocity = 0f;
            _lanternBody.velocity = Vector2.zero;
            _lanternBody.isKinematic = true;
        }
        else
        {
            _lanternBody.isKinematic = false;
            _lanternBody.velocity = _storedVelocity;
            _lanternBody.angularVelocity = _storedAngularVelocity;
        }
        _lanternHinge.enabled = (!_bDropped && !bPaused);
    }

    public void SetColourFromShieldCharges(int charges)
    {
        switch (charges)
        {
            case 0:
                ChangeColour(LanternColour.Green);
                break;
            case 1:
                ChangeColour(LanternColour.Gold);
                break;
            case 2:
                ChangeColour(LanternColour.Blue);
                break;
        }
    }

    public void ChangeColour(LanternColour lColour)
    {
        if (_colour == lColour)
        {
            return;
        }
        _colour = lColour;
        StartCoroutine(ColourChange());
    }

    private IEnumerator ColourChange()
    {
        _bColourChanging = true;

        float animTimer = 0f;
        const float shrinkDuration = 0.1f;
        const float pulseDuration = 0.1f;
        const float settleDuration = 0.2f;

        while (animTimer < shrinkDuration)
        {
            animTimer += Time.deltaTime;
            _light.transform.localScale = _lightScale * (1f - 0.8f * animTimer / shrinkDuration);
            yield return null;
        }
        SetColour();

        animTimer = 0f;
        while (animTimer < pulseDuration)
        {
            animTimer += Time.deltaTime;
            _light.transform.localScale = _lightScale * (0.2f + 1.3f * animTimer / shrinkDuration);
            yield return null;
        }

        animTimer = 0f;
        while (animTimer < settleDuration)
        {
            animTimer += Time.deltaTime;
            _light.transform.localScale = _lightScale * (1.5f - (0.5f * animTimer / settleDuration));
            yield return null;
        }
        _light.transform.localScale = _lightScale;
        _bColourChanging = false;
    }

    private void SetColour()
    {
        ParticleSystem.MainModule particleSettings = shimmerEffect.main;
        switch (_colour)
        {
            case LanternColour.Green:
                _globe.color = Toolbox.MothGreenColor;
                _light.color = Toolbox.MothGreenColor;
                particleSettings.startColor = Toolbox.MothGreenColor;
                break;
            case LanternColour.Gold:
                _globe.color = Toolbox.MothGoldColor;
                _light.color = Toolbox.MothGoldColor;
                particleSettings.startColor = Toolbox.MothGoldColor;
                break;
            case LanternColour.Blue:
                _globe.color = Toolbox.MothBlueColor;
                _light.color = Toolbox.MothBlueColor;
                particleSettings.startColor = Toolbox.MothBlueColor;
                break;
        }
    }
}
