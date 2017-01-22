using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Moth : MonoBehaviour {

    #region Level Editor Inspector fields
    public MothColour Colour;
    public MothPathTypes PathType;
    #endregion

    private Transform _mothSprite;
    private Animator _mothAnimator;
    private Collider2D _mothCollider;
    private AudioSource _mothAudio;
    private bool _bIsActive;
    private MothStates _mothState = MothStates.Normal;
    private bool _bConsumption;
    private Transform _lantern;
    private MothInteractivity _mothInteractor;

    private bool _bReverseAngle;

    private enum MothAudioNames
    {
        Flutter,
        Morph,
        Consume
    }
    private readonly Dictionary<MothAudioNames, AudioClip> _mothAudioDict = new Dictionary<MothAudioNames, AudioClip>();

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
    public enum MothPathTypes
    {
        Figure8,
        Sine
    }

    private float _mothZLayer;
    private bool _paused;
    private float _speed;
    private float _phase;
    private const float Pi = Mathf.PI;
    
    private void Awake ()
    {
        _mothZLayer = Toolbox.Instance.ZLayers["Moth"];
        _mothAudio = GetComponent<AudioSource>();
        _mothInteractor = new MothInteractivity();
        foreach (Transform gameObj in transform)
        {
            if (gameObj.name == "MothTrigger")
            {
                _mothSprite = gameObj;
                _mothAnimator = gameObj.GetComponent<Animator>();
                _mothAnimator.enabled = true;
                _mothCollider = gameObj.GetComponent<Collider2D>();
            }
            else if (gameObj.name == "PathBox")
            {
                if (!Toolbox.Instance.Debug)
                {
                    Destroy(gameObj.gameObject);
                }
            }
        }
        GameObject lanternObj = GameObject.Find("Lantern");
        if (lanternObj) { _lantern = lanternObj.GetComponent<Transform>(); }

        LoadSoundClips();
    }
	
	private void FixedUpdate ()
    {
        if (!_bIsActive || _paused) { return; }
        MoveMothAlongPath(Time.deltaTime);
    }

    private void LoadSoundClips()
    {
        _mothAudioDict.Add(MothAudioNames.Consume, Resources.Load<AudioClip>("Audio/LanternConsumeMoth"));
    }

    private void MoveMothAlongPath(float time)
    {
        switch (PathType)
        {
            case MothPathTypes.Figure8:
                MothAlongFigure8(time);
                break;
            case MothPathTypes.Sine:
                MoveAlongSine(time);
                break;
        }
    }

    private void MoveAlongSine(float time)
    {
        float dist = 4f * time;
        const float oscillationSpeed = 0.65f;
        const float maxRotation = 90;

        _phase += time;
        if (_phase > oscillationSpeed)
        {
            _phase = 0;
            _bReverseAngle = !_bReverseAngle;
        }

        float rotationOffset;
        if (_bReverseAngle) { rotationOffset = maxRotation * (1 - _phase / oscillationSpeed); }
        else { rotationOffset = maxRotation * (_phase / oscillationSpeed); }
        float zRotation = -135 + rotationOffset;
        _mothSprite.transform.localRotation = Quaternion.AngleAxis(zRotation, Vector3.back);
        
        Vector3 mothAxis;
        float mothAngle;
        _mothSprite.transform.localRotation.ToAngleAxis(out mothAngle, out mothAxis);
        float xOffset = dist * Mathf.Sin(Pi / 180 * mothAngle * -mothAxis.z);
        float yOffset = dist * Mathf.Cos(Pi / 180 * mothAngle * -mothAxis.z);
        if (float.IsNaN(xOffset)) { xOffset = 0; }
        if (float.IsNaN(yOffset)) { yOffset = 0; }
        if (_mothState == MothStates.Normal)
        {
            transform.position += new Vector3(xOffset, yOffset, 0f);
        }
    }

    private void MothAlongFigure8(float time)
    {

        const float pathSpeed = 0.7f;
        _phase += Toolbox.Instance.LevelSpeed * time * pathSpeed;
        if (_phase > 2 * Pi)
        {
            _phase -= 2 * Pi;
        }

        float zRotation = (_phase > Pi ? -1 : 1) * _phase * 360 / Pi;
        _mothSprite.transform.localRotation = Quaternion.AngleAxis(zRotation, Vector3.back);
        
        Vector3 mothAxis;
        float mothAngle;
        _mothSprite.transform.localRotation.ToAngleAxis(out mothAngle, out mothAxis);
        float xOffset = 0.065f * pathSpeed * Mathf.Sin(Pi / 180 * mothAngle * -mothAxis.z);
        float yOffset = 0.06f * pathSpeed * Mathf.Cos(Pi / 180 * mothAngle * -mothAxis.z);
        if (float.IsNaN(xOffset)) { xOffset = 0; }
        if (float.IsNaN(yOffset)) { yOffset = 0; }
        if (_mothState == MothStates.Normal)
        {
            transform.position += new Vector3(_speed * Time.deltaTime, 0f, 0f);
            _mothSprite.transform.position += new Vector3(xOffset, yOffset, 0f);
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = -speed;
    }

    public void SetPaused(bool gamePaused)
    {
        _paused = gamePaused;
        if (_bIsActive)
        {
            _mothAnimator.enabled = !gamePaused;
        }
    }

    public void ConsumeMoth()
    {
        const float animDuration = 1f;
        _mothInteractor.ConsumeMoth(animDuration);

        if (!_bConsumption)
        {
            _bConsumption = true;
            _mothCollider.enabled = false;
            StartCoroutine("ConsumeAnim", animDuration);
        }
    }

    private IEnumerator ConsumeAnim(float animDuration)
    {
        PlayExplosionAnim();

        float lantenFollowTime = animDuration / 2f;
        float animTimer = 0f;
        Vector3 startPos = new Vector3();
        bool bStartPosSet = false;

        while (animTimer < animDuration)
        {
            if (!_paused)
            {
                animTimer += Time.deltaTime;
                if (animTimer > animDuration - lantenFollowTime)
                {
                    if (!bStartPosSet)
                    {
                        _mothState = MothStates.ConsumeFollow;
                        startPos = _mothSprite.transform.position;
                        bStartPosSet = true;
                    }
                    float timeRatio = (animTimer - (animDuration - lantenFollowTime)) / lantenFollowTime;
                    float xOffset = startPos.x - (startPos.x - _lantern.position.x) * timeRatio;
                    float yOffset = startPos.y - (startPos.y - _lantern.position.y) * timeRatio;
                    transform.position = new Vector3(xOffset, yOffset, startPos.z);
                }
            }
            yield return null;
        }
        _mothInteractor.ActivateAbility(Colour);
        ReturnToInactivePool();
        _bConsumption = false;
    }

    private void PlayExplosionAnim()
    {
        //_mothAudio.PlayOneShot(Resources.Load<AudioClip>("LanternConsumeMoth"));  // TODO moth morph sound
        switch (Colour)
        {
            case MothColour.Green:
                _mothAnimator.Play("MothGreenExplosion", 0, 0f);
                break;
            case MothColour.Blue:
                _mothAnimator.Play("MothBlueExplosion", 0, 0f);
                break;
            case MothColour.Gold:
                _mothAnimator.Play("MothGoldExplosion", 0, 0f);
                break;
        }
    }

    public void ReturnToInactivePool()
    {
        _mothAudio.PlayOneShot(_mothAudioDict[MothAudioNames.Consume]);
        transform.position = Toolbox.Instance.HoldingArea;
        _mothSprite.transform.position = transform.position;
        IsActive = false;
    }

    public bool IsActive
    {
        get { return _bIsActive; }
        set { _bIsActive = value; }
    }

    public void ActivateMoth(MothColour colour, MothPathTypes pathType = MothPathTypes.Figure8)
    {
        // TODO determine where in the vertical space the moth can spawn (endless mode only)
        //const float Range = 2f;
        //float MothYPos = Range * Random.value - Range / 2;

        _mothState = MothStates.Normal;
        _mothAnimator.enabled = true;
        _mothCollider.enabled = true;
        _phase = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y, _mothZLayer); // TODO remove this once we have everything nicely bundled in the 'Level' Gameobject
        _mothSprite.transform.position = transform.position;
        _bIsActive = true;
        PathType = pathType;
        Colour = colour;
        switch (colour)
        {
            case MothColour.Green:
                _mothAnimator.Play("MothGreenAnimation", 0, 0f);
                break;
            case MothColour.Blue:
                _mothAnimator.Play("MothBlueAnimation", 0, 0f);
                break;
            case MothColour.Gold:
                _mothAnimator.Play("MothGoldAnimation", 0, 0f);
                break;
        }
    }

    public void PauseAnimation()
    {
        // This was written so we could switch on the moth animation in Awake to avoid load times upon activation
        _mothAnimator.enabled = false;
    }
}
