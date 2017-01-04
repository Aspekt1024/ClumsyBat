using UnityEngine;
using System.Collections;

public class Moth : MonoBehaviour {
    
    private Transform MothSprite = null;
    private Animator MothAnimator = null;
    private Collider2D MothCollider = null;
    private bool bIsActive = false;
    public MothColour Colour;
    private MothStates MothState = MothStates.Normal;
    private bool bConsumption = false;

    private Transform Lantern = null;

    private enum MothStates
    {
        Normal,
        ConsumeFollow
    }

    public enum MothColour
    {
        Green,
        Gold,
        Blue
    }

    private float MothZLayer;

    private bool Paused = false;
    private float Speed = 0;
    private float Phase = 0;
    private const float Pi = Mathf.PI;
    
    void Awake ()
    {
        MothZLayer = Toolbox.Instance.ZLayers["Moth"];
        foreach (Transform GO in transform)
        {
            if (GO.name == "MothTrigger")
            {
                MothSprite = GO;
                MothAnimator = GO.GetComponent<Animator>();
                MothAnimator.enabled = false;
                MothCollider = GO.GetComponent<Collider2D>();
            }
            else if (GO.name == "PathBox")
            {
                if (!Toolbox.Instance.Debug)
                {
                    Destroy(GO.gameObject);
                }
            }
        }
        Lantern = GameObject.Find("Lantern").GetComponent<Transform>();
    }
	
	void Update ()
    {
        if (!bIsActive || Paused) { return; }
        MoveMothAlongPath();
    }

    private void MoveMothAlongPath()
    {
        float PathSpeed = 0.7f;
        Phase += Toolbox.Instance.LevelSpeed * Time.deltaTime * PathSpeed;
        if (Phase > 2 * Pi)
        {
            Phase -= 2 * Pi;
        }

        float ZRotation = (Phase > Pi ? -1 : 1) * Phase * 360 / Pi;
        MothSprite.transform.localRotation = Quaternion.AngleAxis(ZRotation, Vector3.back);

        Vector3 MothAxis;
        float MothAngle;
        MothSprite.transform.localRotation.ToAngleAxis(out MothAngle, out MothAxis);
        float XOffset = 0.065f * PathSpeed * Mathf.Sin(Pi / 180 * MothAngle * -MothAxis.z);
        float YOffset = 0.06f * PathSpeed * Mathf.Cos(Pi / 180 * MothAngle * -MothAxis.z);
        if (float.IsNaN(XOffset)) { XOffset = 0; }
        if (float.IsNaN(YOffset)) { YOffset = 0; }
        if (MothState == MothStates.Normal)
        {
            transform.position += new Vector3(Speed * Time.deltaTime, 0f, 0f);
            MothSprite.transform.position += new Vector3(XOffset, YOffset, 0f);
        }
    }

    public void SetSpeed(float _speed)
    {
        Speed = -_speed;
    }

    public void SetPaused(bool GamePaused)
    {
        Paused = GamePaused;
        MothAnimator.enabled = !GamePaused;
    }

    public float ConsumeMoth()
    {
        const float AnimDuration = 1f;
        if (!bConsumption)
        {
            bConsumption = true;
            MothCollider.enabled = false;
            StartCoroutine("ConsumeAnim", AnimDuration);
        }
        return AnimDuration;
    }

    private IEnumerator ConsumeAnim(float AnimDuration)
    {
        PlayExplosionAnim();

        float LantenFollowTime = AnimDuration / 2f;
        float AnimTimer = 0f;
        Vector3 StartPos = new Vector3();
        bool bStartPosSet = false;

        while (AnimTimer < AnimDuration)
        {
            if (!Paused)
            {
                AnimTimer += Time.deltaTime;
                if (AnimTimer > AnimDuration - LantenFollowTime)
                {
                    if (!bStartPosSet)
                    {
                        MothState = MothStates.ConsumeFollow;
                        StartPos = MothSprite.transform.position;
                        bStartPosSet = true;
                    }
                    float TimeRatio = (AnimTimer - (AnimDuration - LantenFollowTime)) / LantenFollowTime;
                    float XOffset = StartPos.x - (StartPos.x - Lantern.position.x) * TimeRatio;
                    float YOffset = StartPos.y - (StartPos.y - Lantern.position.y) * TimeRatio;
                    MothSprite.transform.position = new Vector3(XOffset, YOffset, StartPos.z);
                }
            }
            yield return null;
        }
        ReturnToInactivePool();
        bConsumption = false;
    }

    private void PlayExplosionAnim()
    {
        switch (Colour)
        {
            case MothColour.Green:
                MothAnimator.Play("MothGreenExplosion", 0, 0f);
                break;
            case MothColour.Blue:
                MothAnimator.Play("MothBlueExplosion", 0, 0f);
                break;
            case MothColour.Gold:
                MothAnimator.Play("MothGoldExplosion", 0, 0f);
                break;
        }
    }

    public void ReturnToInactivePool()
    {
        transform.position = Toolbox.Instance.HoldingArea;
        MothSprite.transform.position = transform.position;
        IsActive = false;
    }

    public bool IsActive
    {
        get { return bIsActive; }
        set { bIsActive = value; }
    }

    public void ActivateMoth(MothColour _colour)
    {
        // TODO determine where in the vertical space the moth can spawn
        //const float Range = 2f;
        //float MothYPos = Range * Random.value - Range / 2;

        MothCollider.enabled = true;
        Phase = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y, MothZLayer); // TODO replace this?
        MothSprite.transform.position = transform.position;
        bIsActive = true;
        Colour = _colour;
        switch (_colour)
        {
            case MothColour.Green:
                MothAnimator.Play("MothGreenAnimation", 0, 0f);
                break;
            case MothColour.Blue:
                MothAnimator.Play("MothBlueAnimation", 0, 0f);
                break;
            case MothColour.Gold:
                MothAnimator.Play("MothGoldAnimation", 0, 0f);
                break;
        }
    }

}
