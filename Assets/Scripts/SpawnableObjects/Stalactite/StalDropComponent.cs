using UnityEngine;
using System.Collections;

public class StalDropComponent : MonoBehaviour {

    [HideInInspector]
    public SpriteRenderer TriggerSprite;
    private Stalactite _stal;
    private StalAnimationHandler _anim;
    private Rigidbody2D _stalBody;
    private Transform _playerBody;
    private PlayerController _playerControl;

    [HideInInspector]
    public const float FallDuration = 1.2f;
    [HideInInspector]
    public const float FallDistance = 20f;

    private bool _paused;
    private Vector2 _storedVelocity = Vector2.zero;

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

    private void Update()
    {
        if (!_stal.Active() || !_stal.UnstableStalactite) { return; }

        if ((_playerBody.position.x + 8f > transform.position.x) && _state == DropStates.None && _playerControl.ThePlayer.IsAlive())
        {
            _state = DropStates.Shaking;
            StartCoroutine("Shake");
        }

        if (!_playerControl) { return; } // Editor clumsy has no player control
        if (!_playerControl.ThePlayer.IsAlive() && _state == DropStates.Shaking)
        {
            _state = DropStates.None;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_stal.UnstableStalactite) { Drop(); }
    }

    public void NewStalactite()
    {
        _state = DropStates.None;
        StopAllCoroutines();
        TriggerSprite.enabled = Toolbox.Instance.Debug && _stal.UnstableStalactite;
    }
    
    public void Drop()
    {
        if (_state != DropStates.Falling)
        {
            _state = DropStates.Falling;
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
    }
    
    IEnumerator Shake()
    {
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
                _stalBody.isKinematic = true;
            }
            else
            {
                _stalBody.isKinematic = false;
                _stalBody.velocity = _storedVelocity;
            }
        }
    }

    private void GetComponentList()
    {
        GameObject thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer)
        {
            _playerBody = GameObject.FindGameObjectWithTag("Player").transform;
            _playerControl = _playerBody.GetComponent<PlayerController>();
        }
        
        TriggerSprite = GetComponent<SpriteRenderer>();
        _stal = GetComponentInParent<Stalactite>();
        _anim = _stal.GetComponentInChildren<StalAnimationHandler>();

        foreach (Transform tf in _stal.GetComponent<Transform>())
        {
            if (tf.name == "StalObject")
            {
                _stalBody = tf.GetComponent<Rigidbody2D>();
            }
        }
    }
}
