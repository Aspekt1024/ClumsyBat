using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Moth : Spawnable {

    #region Level Editor Inspector fields
    public MothColour Colour;
    public MothPathHandler.MothPathTypes PathType;
    #endregion

    [HideInInspector]
    public Transform MothSprite;
    private Animator _mothAnimator;
    private Collider2D _mothCollider;
    private AudioSource _mothAudio;
    private MothStates _mothState = MothStates.Normal;
    private Transform _lantern;
    private MothInteractivity _mothInteractor;
    private MothPathHandler _pathHandler;
    private bool _bConsumption;
    private readonly Dictionary<MothAudioNames, AudioClip> _mothAudioDict = new Dictionary<MothAudioNames, AudioClip>();

    private enum MothAudioNames { Flutter, Morph, Consume }
    private enum MothStates { Normal, ConsumeFollow }
    public enum MothColour { Green, Gold, Blue }
    
    private void Awake ()
    {
        IsActive = false;
        GetMothComponents();
        LoadSoundClips();
    }
	
	private void FixedUpdate ()
    {
        if (!IsActive || bPaused) { return; }
        _pathHandler.MoveAlongPath(Time.deltaTime);

        if (_mothState == MothStates.ConsumeFollow) { return; }
        MoveLeft(Time.deltaTime);
    }

    private void LoadSoundClips()
    {
        _mothAudioDict.Add(MothAudioNames.Consume, Resources.Load<AudioClip>("Audio/LanternConsumeMoth"));
    }

    public override void PauseGame(bool gamePaused)
    {
        base.PauseGame(gamePaused);
        if (IsActive) { _mothAnimator.enabled = !gamePaused; }
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
            if (!bPaused)
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
        SendToInactivePool();
        _bConsumption = false;
    }

    private void PlayNormalAnimation()
    {
        switch (Colour)
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

    public override void SendToInactivePool()
    {
        base.SendToInactivePool();
        MothSprite.transform.position = transform.position;
        _mothAudio.PlayOneShot(_mothAudioDict[MothAudioNames.Consume]);
    }

    public void Activate(SpawnType spawnTf, MothColour colour, MothPathHandler.MothPathTypes pathType = MothPathHandler.MothPathTypes.Spiral)
    {
        // TODO determine where in the vertical space the moth can spawn ie Raycast (endless mode only)
        //const float Range = 2f;
        //float MothYPos = Range * Random.value - Range / 2;
        base.Activate(transform, spawnTf);
        _mothState = MothStates.Normal;
        _mothAnimator.enabled = true;
        _mothCollider.enabled = true;
        MothSprite.transform.position = transform.position;
        _pathHandler.SetPathType(pathType);
        Colour = colour;
        PlayNormalAnimation();
    }

    public IEnumerator SpawnFromEssence(float despawnTimer)
    {
        _mothCollider.enabled = false;
        PlayExplosionAnim();
        yield return StartCoroutine("WaitSeconds", 0.9f);
        PlayNormalAnimation();
        yield return StartCoroutine("WaitSeconds", 0.2f);
        _mothCollider.enabled = true;

        if (Mathf.Abs(despawnTimer) > 0.001)
        {
            yield return StartCoroutine("WaitSeconds", despawnTimer);

            if (IsActive)
            {
                _mothCollider.enabled = false;
                PlayExplosionAnim();
                yield return StartCoroutine("WaitSeconds", 0.9f);
                base.SendToInactivePool();
                MothSprite.transform.position = transform.position;
            }
        }
    }

    private IEnumerator WaitSeconds(float secs)
    {
        float timer = 0f;
        while (timer < secs)
        {
            if (!bPaused)
                timer += Time.deltaTime;
            yield return null;
        }
    }

    private void GetMothComponents()
    {
        _mothAudio = GetComponent<AudioSource>();
        _mothInteractor = gameObject.AddComponent<MothInteractivity>();
        _pathHandler = new MothPathHandler(this);
        foreach (Transform tf in transform)
        {
            if (tf.name == "MothTrigger")
            {
                MothSprite = tf;
                _mothAnimator = tf.GetComponent<Animator>();
                _mothAnimator.enabled = true;
                _mothCollider = tf.GetComponent<Collider2D>();
            }
            else if (tf.name == "PathBox")
            {
                if (!Toolbox.Instance.Debug)
                {
                    Destroy(tf.gameObject);
                }
            }
        }
        GameObject lanternObj = GameObject.Find("Lantern");
        if (lanternObj) { _lantern = lanternObj.GetComponent<Transform>(); }
    }

    public void PauseAnimation()
    {
        // This was written so we could switch on the moth animation in Awake to avoid load times upon activation
        _mothAnimator.enabled = false;
    }
}
