using UnityEngine;
using System.Collections;

public class StalDropComponent : MonoBehaviour {

    [HideInInspector]
    public SpriteRenderer TriggerSprite;
    private Stalactite stal;
    private StalAnimationHandler _anim;
    private Rigidbody2D _stalBody;
    private Transform playerTf;
    private PlayerController _playerControl;

    [HideInInspector]
    public const float FallDuration = 1.2f;
    [HideInInspector]
    public const float FallDistance = 20f;

    private bool _paused;
    private Vector2 _storedVelocity = Vector2.zero;
    private bool storedKinematicState;
    private const float shakeThresholdX = 6f;

    private enum DropStates
    {
        None,
        Shaking,
        Falling
    }
    private DropStates _state;

    private void Awake()
    {
        GetComponentList();
    }

    private void FixedUpdate()
    {
        if (!stal.DropEnabled || !stal.Active() || _paused || _state == DropStates.Falling || !_playerControl.ThePlayer.IsAlive()) return;
        
        if (playerTf.transform.position.x > transform.position.x - stal.TriggerPosX)
        {
            Drop();
        }
        else
        {
            if (playerTf.transform.position.x > transform.position.x - stal.TriggerPosX - shakeThresholdX && _state == DropStates.None)
            {
                StartCoroutine("Shake");
            }
        }
    }

    private void Update()
    {
        if (_playerControl == null) return;
        if (!_playerControl.ThePlayer.IsAlive() && _state == DropStates.Shaking)
        {
            _state = DropStates.None;
        }
    }

    public void NewStalactite()
    {
        _state = DropStates.None;
        StopAllCoroutines();
        TriggerSprite.enabled = Toolbox.Instance.Debug && stal.DropEnabled;
    }
    
    public void Drop()
    {
        if (_state != DropStates.Falling)
        {
            _state = DropStates.Falling;
            stal.SetState(Stalactite.StalStates.Falling);
            StartCoroutine("DropSequence");
        }
    }
    
    private IEnumerator DropSequence()
    {
        _anim.CrackAndFall();
        while (!_anim.ReadyToFall() || _paused)
        {
            yield return null;
        }
        float fallTime = 0f;
        float startingYPos = _stalBody.transform.position.y;
        while (fallTime < FallDuration)
        {
            if (!_paused)
            {
                fallTime += Time.deltaTime;
                float yPos = startingYPos - FallDistance * Mathf.Pow((fallTime / FallDuration), 2);
                _stalBody.transform.position = new Vector3(_stalBody.transform.position.x, yPos, _stalBody.transform.position.z);
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                yield return null;
            }
        }
        stal.SendToInactivePool();
    }
    
    IEnumerator Shake()
    {
        _state = DropStates.Shaking;
        const float shakeInterval = 0.07f;
        const float shakeIntensity = 1.8f;
        bool bRotateForward = true;
        while (_state == DropStates.Shaking)
        {
            if (!_paused)
            {
                _stalBody.transform.Rotate(new Vector3(0, 0, (bRotateForward ? shakeIntensity : -shakeIntensity)));
                bRotateForward = !bRotateForward;
            }
            yield return new WaitForSeconds(shakeInterval);
        }
        _stalBody.transform.Rotate(Vector3.zero);   // Prevents rotating once we exit the while loop
    }

    public void SetPaused(bool bPaused)
    {
        _paused = bPaused;
        if (_state == DropStates.Falling)
        {
            if (_paused)
            {
                _storedVelocity = _stalBody.velocity;
                _stalBody.velocity = Vector2.zero;
                storedKinematicState = _stalBody.isKinematic;
                _stalBody.isKinematic = true;
            }
            else
            {
                _stalBody.isKinematic = storedKinematicState;
                _stalBody.velocity = _storedVelocity;
            }
        }
    }

    private void GetComponentList()
    {
        Transform playerTf = Toolbox.Player.transform;
        if (playerTf != null)
        {
            _playerControl = playerTf.GetComponent<PlayerController>();
        }
        
        TriggerSprite = GetComponent<SpriteRenderer>();
        stal = GetComponent<Stalactite>();
        _anim =GetComponent<StalAnimationHandler>();
        _stalBody = transform.GetComponent<Rigidbody2D>();
    }
}
