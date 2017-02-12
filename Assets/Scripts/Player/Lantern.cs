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

        StartCoroutine("Flicker");
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
            }
        }
    }

    public void Drop()
    {
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
        StartCoroutine("EngageMotor");
    }

    private IEnumerator EngageMotor()
    {
        _lanternHinge.useMotor = true;
        yield return new WaitForSeconds(0.2f);
        _lanternHinge.useMotor = false;
    }

    public void GamePaused(bool bPaused)
    {
        _paused = bPaused;
        if (_paused)
        {
            _storedVelocity = _lanternBody.velocity;
            _lanternBody.angularVelocity = 0f;
            _lanternBody.velocity = Vector2.zero;
            _lanternBody.isKinematic = true;
        }
        else
        {
            _lanternBody.isKinematic = false;
            _lanternBody.velocity = _storedVelocity;
        }
        _lanternHinge.enabled = (!_bDropped && !bPaused);
    }

    public void ChangeColour(LanternColour lColour)
    {
        if (_colour == lColour)
        {
            return;
        }
        _colour = lColour;
        StartCoroutine("ColourChange");
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
        switch (_colour)
        {
            case LanternColour.Green:
                _globe.color = new Color(110 / 255f, 229 / 255f, 119 / 255f);
                _light.color = new Color(110 / 255f, 229 / 255f, 119 / 255f);
                break;
            case LanternColour.Gold:
                _globe.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);
                _light.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);
                break;
            case LanternColour.Blue:
                _globe.color = new Color(151 / 255f, 147 / 255f, 231 / 255f);
                _light.color = new Color(151 / 255f, 147 / 255f, 231 / 255f);
                break;
        }
    }
}
