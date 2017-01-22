using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Moth : MonoBehaviour {

    #region Level Editor Inspector fields
    public MothColour Colour;
    public MothPathTypes PathType;
    #endregion

    [HideInInspector]
    public Transform MothSprite;
    private Animator _mothAnimator;
    private Collider2D _mothCollider;
    private AudioSource _mothAudio;
    private MothStates _mothState = MothStates.Normal;
    private bool _bConsumption;
    private Transform _lantern;
    private MothInteractivity _mothInteractor;
    private MothPathHandler _pathHandler;

    private bool _bReverseAngle;

    private enum MothAudioNames
    {
        Flutter, Morph, Consume
    }
    private readonly Dictionary<MothAudioNames, AudioClip> _mothAudioDict = new Dictionary<MothAudioNames, AudioClip>();

    private enum MothStates
    {
        Normal, ConsumeFollow
    }

    public enum MothColour
    {
        Green, Gold, Blue
    }
    public enum MothPathTypes
    {
        Infinity, Clover, Figure8, Spiral, Sine
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
        _mothInteractor = gameObject.AddComponent<MothInteractivity>();
        _pathHandler = new MothPathHandler(this);
        foreach (Transform gameObj in transform)
        {
            if (gameObj.name == "MothTrigger")
            {
                MothSprite = gameObj;
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
        if (!IsActive || _paused) { return; }
        MoveMothAlongPath(Time.deltaTime);
        if (_mothState == MothStates.ConsumeFollow) { return; }
        transform.position -= Vector3.left * Time.deltaTime * _speed;
    }

    private void LoadSoundClips()
    {
        _mothAudioDict.Add(MothAudioNames.Consume, Resources.Load<AudioClip>("Audio/LanternConsumeMoth"));
    }

    private void MoveMothAlongPath(float time)
    {
        if (PathType == MothPathTypes.Sine)
            MoveAlongSine(time);
        else
            _pathHandler.MoveAlongClover(time);
    }

    // TODO move to path handler
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
        MothSprite.transform.localRotation = Quaternion.AngleAxis(zRotation, Vector3.back);
        
        Vector3 mothAxis;
        float mothAngle;
        MothSprite.transform.localRotation.ToAngleAxis(out mothAngle, out mothAxis);
        float xOffset = dist * Mathf.Sin(Pi / 180 * mothAngle * -mothAxis.z);
        float yOffset = dist * Mathf.Cos(Pi / 180 * mothAngle * -mothAxis.z);
        if (float.IsNaN(xOffset)) { xOffset = 0; }
        if (float.IsNaN(yOffset)) { yOffset = 0; }
        if (_mothState == MothStates.Normal)
        {
            transform.position += new Vector3(xOffset, yOffset, 0f);
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = -speed;
    }

    public void SetPaused(bool gamePaused)
    {
        _paused = gamePaused;
        if (IsActive)
        {
            _mothAnimator.enabled = !gamePaused;
        }
    }

    public void ConsumeMoth()
    {
        const float animDuration = 1f;
        _mothInteractor.StartCoroutine("ConsumeMoth", animDuration);

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
                        startPos = MothSprite.position;
                        bStartPosSet = true;
                    }
                    float timeRatio = (animTimer - (animDuration - lantenFollowTime)) / lantenFollowTime;
                    float xOffset = startPos.x - (startPos.x - _lantern.position.x) * timeRatio;
                    float yOffset = startPos.y - (startPos.y - _lantern.position.y) * timeRatio;
                    MothSprite.position = new Vector3(xOffset, yOffset, startPos.z);
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
        MothSprite.transform.position = transform.position;
        IsActive = false;
    }

    public bool IsActive { get; set; }

    public void ActivateMoth(MothColour colour, MothPathTypes pathType = MothPathTypes.Spiral)
    {
        // TODO determine where in the vertical space the moth can spawn (endless mode only)
        //const float Range = 2f;
        //float MothYPos = Range * Random.value - Range / 2;

        _mothState = MothStates.Normal;
        _mothAnimator.enabled = true;
        _mothCollider.enabled = true;
        _phase = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y, _mothZLayer); // TODO remove this once we have everything nicely bundled in the 'Level' Gameobject
        MothSprite.transform.position = transform.position;
        IsActive = true;
        PathType = pathType;    // TODO move to path handler
        _pathHandler.SetPathType(pathType);
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
