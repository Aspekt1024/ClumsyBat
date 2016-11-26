using UnityEngine;
using System.Collections;

public class Moth : MonoBehaviour {
    
    private Transform MothSprite = null;
    private Animator MothAnimator = null;
    private bool bIsActive = false;
    public bool IsGold;

    private const float MothZLayer = 1f;

    private bool Paused = false;
    private float Speed = 0;
    private float Phase = 0;
    private const float Pi = Mathf.PI;
    
    void Awake ()
    {
        foreach (Transform GO in transform)
        {
            if (GO.name == "MothSprite")
            {
                MothSprite = GO;
                MothAnimator = GO.GetComponent<Animator>();
                MothAnimator.enabled = false;
            }
        }
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
        transform.position += new Vector3(XOffset + Speed * Time.deltaTime, YOffset, 0);
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

    public void ReturnToInactivePool()
    {
        transform.position = Toolbox.Instance.HoldingArea;
        IsActive = false;
    }

    public bool IsActive
    {
        get
        {
            return bIsActive;
        }
        set
        {
            bIsActive = value;
        }
    }

    public void ActivateMoth(bool _isGold)
    {
        // TODO determine where in the vertical space the moth can spawn
        const float Range = 2f;
        float MothYPos = Range * Random.value - Range / 2;
        transform.position = new Vector3(transform.position.x, MothYPos, MothZLayer); // TODO replace this?
        bIsActive = true;
        IsGold = _isGold;
        if (IsGold)
        {
            MothAnimator.Play("MothGoldAnimation", 0, 0f);
        }
        else
        {
            MothAnimator.Play("MothGreenAnimation", 0, 0f);
        }
    }

}
