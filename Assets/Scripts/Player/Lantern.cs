using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the physics of the Lantern carried by Clumsy
/// </summary>
public class Lantern : MonoBehaviour {

    private HingeJoint2D LanternHinge = null;
    private Rigidbody2D LanternBody = null;
    private PolygonCollider2D LanternCollider = null;
    private SpriteRenderer Globe = null;
    private SpriteRenderer Light = null;
    
    public enum LanternColour
    {
        White,
        Green,
        Gold,
        Blue
    }
    private LanternColour Colour = LanternColour.Green;
    private bool Paused;
    private bool bColourChanging = false;
    private Vector2 LightScale;     // Flicker and change colour will be centered around the initial scale

    void Start ()
    {
        LanternHinge = GetComponent<HingeJoint2D>();
        LanternBody = GetComponent<Rigidbody2D>();
        LanternCollider = GetComponent<PolygonCollider2D>();
        GetChildComponents();

        LanternCollider.enabled = false;
        LightScale = Light.transform.localScale;
        SetColour();

        StartCoroutine("Flicker");
    }

    private IEnumerator Flicker()
    {
        const float Period = 0.2f;
        float Timer = 0f;
        bool bGrowing = true;
        const float Size = 0.1f;

        while (true)
        {
            if (!Paused && !bColourChanging)
            {
                Timer += Time.deltaTime;
                if (Timer >= Period)
                {
                    Timer -= Period;
                    bGrowing = !bGrowing;
                }
                if (bGrowing)
                {
                    Light.transform.localScale = LightScale * (1f + Size * Timer / Period);
                }
                else
                {
                    Light.transform.localScale = LightScale * (1f + Size - Size * Timer / Period);
                }
            }
            yield return null;
        }
    }

    private void GetChildComponents()
    {
        foreach (Transform ChildObj in transform)
        {
            switch (ChildObj.name)
            {
                case "LanternGlobe":
                    Globe = ChildObj.GetComponent<SpriteRenderer>();
                    break;
                case "LanternLight":
                    Light = ChildObj.GetComponent<SpriteRenderer>();
                    break;
            }
        }
    }

    public void Drop()
    {
        gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        LanternHinge.enabled = false;
        LanternBody.velocity = new Vector2(Random.Range(1f, 5f), 1f);
        LanternBody.AddTorque(Random.Range(100f, 600f));
    }

    public void AddRushForce()
    {
        JointMotor2D LanternMotor = LanternHinge.motor;
        LanternMotor.motorSpeed = 1000;
        LanternHinge.motor = LanternMotor;
        StartCoroutine("EngageMotor");
    }

    private IEnumerator EngageMotor()
    {
        LanternHinge.useMotor = true;
        yield return new WaitForSeconds(0.2f);
        LanternHinge.useMotor = false;
    }

    public void GamePaused(bool bPaused)
    {
        Paused = bPaused;
        LanternHinge.enabled = !bPaused;
        LanternBody.isKinematic = bPaused;
        if (bPaused)
        {
            LanternBody.velocity = Vector3.zero;
        }
    }

    public void ChangeColour(LanternColour LColour)
    {
        if (Colour == LColour)
        {
            return;
        }
        Colour = LColour;
        StartCoroutine("ColourChange");
    }

    private IEnumerator ColourChange()
    {
        bColourChanging = true;

        float AnimTimer = 0f;
        const float ShrinkDuration = 0.1f;
        const float PulseDuration = 0.1f;
        const float SettleDuration = 0.2f;

        while (AnimTimer < ShrinkDuration)
        {
            AnimTimer += Time.deltaTime;
            Light.transform.localScale = LightScale * (1f - AnimTimer / ShrinkDuration);
            yield return null;
        }
        SetColour();

        AnimTimer = 0f;
        while (AnimTimer < PulseDuration)
        {
            AnimTimer += Time.deltaTime;
            Light.transform.localScale = LightScale * (1.5f * AnimTimer / ShrinkDuration);
            yield return null;
        }

        AnimTimer = 0f;
        while (AnimTimer < SettleDuration)
        {
            AnimTimer += Time.deltaTime;
            Light.transform.localScale = LightScale * (1.5f - (0.5f * AnimTimer / SettleDuration));
            yield return null;
        }
        Light.transform.localScale = LightScale;
        bColourChanging = false;
    }

    private void SetColour()
    {
        switch (Colour)
        {
            case LanternColour.Green:
                Globe.color = new Color(110 / 255f, 229 / 255f, 119 / 255f);
                Light.color = new Color(110 / 255f, 229 / 255f, 119 / 255f);
                break;
            case LanternColour.Gold:
                Globe.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);
                Light.color = new Color(212 / 255f, 195 / 255f, 126 / 255f);
                break;
            case LanternColour.Blue:
                Globe.color = new Color(151 / 255f, 147 / 255f, 231 / 255f);
                Light.color = new Color(151 / 255f, 147 / 255f, 231 / 255f);
                break;
        }
    }
}
