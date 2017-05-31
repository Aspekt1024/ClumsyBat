using UnityEngine;
using System.Collections;

public class StalDropComponent : MonoBehaviour {

    [HideInInspector]
    public SpriteRenderer TriggerSprite;
    private Stalactite stal;
    private StalAnimationHandler anim;
    private Rigidbody2D stalBody;
    private Transform playerTf;
    private PlayerController playerControl;

    [HideInInspector]
    public const float FallDuration = 1.2f;
    [HideInInspector]
    public const float FallDistance = 20f;

    private bool isPaused;
    private Vector2 storedVelocity = Vector2.zero;
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
        if (!stal.DropEnabled || !stal.IsActive || isPaused || _state == DropStates.Falling || !playerControl.ThePlayer.IsAlive()) return;
        
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
        if (playerControl == null) return;
        if (!playerControl.ThePlayer.IsAlive() && _state == DropStates.Shaking)
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
        anim.CrackAndFall();
        while (!anim.ReadyToFall() || isPaused)
        {
            yield return null;
        }
        float fallTime = 0f;
        float startingYPos = stalBody.transform.position.y;
        while (fallTime < FallDuration)
        {
            if (!isPaused)
            {
                fallTime += Time.deltaTime;
                float yPos = startingYPos - FallDistance * Mathf.Pow((fallTime / FallDuration), 2);
                stalBody.transform.position = new Vector3(stalBody.transform.position.x, yPos, stalBody.transform.position.z);
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
            if (!isPaused)
            {
                stalBody.transform.Rotate(new Vector3(0, 0, (bRotateForward ? shakeIntensity : -shakeIntensity)));
                bRotateForward = !bRotateForward;
            }
            yield return new WaitForSeconds(shakeInterval);
        }
        //stalBody.transform.Rotate(Vector3.zero);
    }

    public void SetPaused(bool bPaused)
    {
        isPaused = bPaused;
        if (_state == DropStates.Falling)
        {
            if (isPaused)
            {
                storedVelocity = stalBody.velocity;
                stalBody.velocity = Vector2.zero;
                storedKinematicState = stalBody.isKinematic;
                stalBody.isKinematic = true;
            }
            else
            {
                stalBody.isKinematic = storedKinematicState;
                stalBody.velocity = storedVelocity;
            }
        }
    }

    private void GetComponentList()
    {
        Transform playerTf = Toolbox.Player.transform;
        if (playerTf != null)
        {
            playerControl = playerTf.GetComponent<PlayerController>();
        }
        
        TriggerSprite = GetComponent<SpriteRenderer>();
        stal = GetComponent<Stalactite>();
        anim =GetComponent<StalAnimationHandler>();
        stalBody = transform.GetComponent<Rigidbody2D>();
    }
}
