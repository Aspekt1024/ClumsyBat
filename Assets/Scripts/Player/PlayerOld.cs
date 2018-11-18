//using System;
//using UnityEngine;
//using System.Collections;

//using PlayerSounds = ClumsyAudioControl.PlayerSounds;
//using ClumsyAnimations = ClumsyAnimator.ClumsyAnimations;
//using ClumsyBat;

//public class PlayerOld : MonoBehaviour {
    
//    #region Clumsy Components
//    public ClumsyAnimator Anim;
//    private Rigidbody2D _playerRigidBody;
//    private Rigidbody2D _lanternBody;
//    private JumpClearance _clearance;
//    private ClumsyAudioControl _audioControl;
//    private PlayerControllerOld _playerController;
//    private Collider2D _playerCollider;
//    private SpriteRenderer _playerRenderer;
//    #endregion

//    #region Clumsy Properties
//    private readonly Vector3 _playerHoldingArea = new Vector3(-100f, 0f, 0f);        // Where Clumsy goes to die
//    private readonly Vector2 _nudgeVelocity = new Vector2(0f, 5f);
//    private const float ClumsyX = -5f;
//    private const float GravityScale = 3f;
//    private Vector2 _savedVelocity = Vector2.zero;
//    private float playerSpeed;
//    private float previousPosX;
//    private Vector3 lastContactPoint;

//    private bool _bPaused;
//    private bool inSecretExit;

//    private Coroutine hoverRoutine;

//    public bool ExitViaSecretPath;
    
//    private GameHandler _gameHandler;
    
//    public void ExitAutoFlightReached()
//    {
//        _state = PlayerState.EndOfLevel;
//        StartCoroutine(CaveExitAnimation());
//    }

//    private IEnumerator CaveExitAnimation()
//    {
//        float animTimer = 0f;
//        const float animDuration = 0.9f;
//        Vector3 originalPos = transform.position;
//        Vector3 targetExitPoint = new Vector3(transform.position.x + Toolbox.TileSizeX / 2f, -0.5f, originalPos.z);

//        while (animTimer < animDuration)
//        {
//            animTimer += Time.deltaTime;
//            float animRatio = animTimer / animDuration;
//            transform.position = Vector3.Lerp(originalPos, targetExitPoint, animRatio);
//            yield return null;
//        }
//        transform.position = _playerHoldingArea;
//        _lanternBody.transform.position += new Vector3(.3f, 0f, 0f);
//        _gameHandler.LevelComplete();
//    }

//    public IEnumerator CaveEntranceAnimation()
//    {
//        Anim.PlayAnimation(ClumsyAnimations.Hover);
//        _state = PlayerState.Startup;
//        float animTimer = 0f;
//        const float animDuration = 0.63f;
//        Vector2 startPos = new Vector2(-Toolbox.TileSizeX / 2, -0.7f);
//        Vector2 targetPos = new Vector2(ClumsyX, 1.3f);

//        while (animTimer < animDuration)
//        {
//            animTimer += Time.deltaTime;
//            float animRatio = animTimer / animDuration;
//            float xPos = startPos.x - (startPos.x - targetPos.x) * animRatio;
//            float yPos = startPos.y - (startPos.y - targetPos.y) * Mathf.Pow(animRatio, 2);
//            transform.position = new Vector3(xPos, yPos, transform.position.z);
//            yield return null;
//        }

//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
//        _playerRigidBody.velocity = new Vector2(0, 3f);
//        _state = PlayerState.Normal;
//    }
    
//    public void ActivateRush(PlayerControllerOld.SwipeDirecitons direction) { _rush.Activate(direction); }
//    public void DeactivateRush() { _rush.Deactivate(); }
//    public void ActivateHypersonic() { _hypersonic.ActivateHypersonic(); }
//    public void ForceHypersonic() { _hypersonic.ForceHypersonic(); }
//    public void AddShieldCharge() { _shield.AddCharge(); }
//    public void AddDashCharge() { _rush.AddCharge(); }

//    public int GetShieldCharges() { return _shield.GetCharges(); }
    
//    private void OnCollisionEnter2D(Collision2D other)
//    {
//        _flap.CancelIfMoving();
        
//        if (other.gameObject.name.Contains("Cave") || other.gameObject.name.Contains("Entrance") || other.gameObject.name.Contains("Exit"))
//        {
//            if (Mathf.Abs(other.contacts[0].normal.x) > 0.8f) return;

//            TryPerch(other.gameObject);
//        }
//        else
//        {
//            if (other.contacts.Length > 0)
//            {
//                lastContactPoint = other.contacts[0].point;
//            }
//            _gameHandler.Collision(other);
//        }
//    }
    


//    private IEnumerator SecretPathWinSequence()
//    {
//        _state = PlayerState.EndOfLevel;
//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
//        _playerCollider.enabled = false;

//        float timer = 0f;
//        const float duration = 1f;
//        while (timer < duration)
//        {
//            timer += Time.deltaTime;
//            yield return null;
//        }

//        ExitViaSecretPath = true;
//        _gameHandler.LevelComplete();
//        Fog.EndOfLevel();
//        transform.position = _playerHoldingArea;
//    }

//    public void StartGame()
//    {
//        _state = PlayerState.Normal;
//        _playerRigidBody.WakeUp();
//        _playerRigidBody.isKinematic = false;
//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
//        Fog.Resume();
//        _playerRigidBody.velocity = _nudgeVelocity;
//    }

//    public void PauseGame()
//    {
//        _bPaused = true;
//        _savedVelocity = _playerRigidBody.velocity;
//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
//        _playerRigidBody.velocity = Vector2.zero;
//        Fog.Pause();
//        Lantern.GamePaused(true);
//        AbilitiesPaused(true);
//    }

//    public void ResumeGame()
//    {
//        if (!IsPerched() && _state != PlayerState.Hovering)
//        {
//            _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
//            _playerRigidBody.velocity = _savedVelocity;
//        }
//        _bPaused = false;
//        Fog.Resume();
//        Lantern.GamePaused(false);
//        AbilitiesPaused(false);
//    }

//    private void AbilitiesPaused(bool bPauseAbility)
//    {
//        _hypersonic.GamePaused(bPauseAbility);
//    }

//    private void BounceIfBottomCave(string objName)
//    {
//        if (objName.Contains("Bottom") || (!objName.Contains("Top") && transform.position.y < 0f))
//            _playerRigidBody.velocity = _nudgeVelocity;
//    }

//    public void JumpIfClear()
//    {
//        if (_state == PlayerState.Hovering) return;

//        if (_clearance.IsEmpty())
//        {
//            ActivateJump();
//        }
//        else
//        {
//            _playerRigidBody.velocity = _nudgeVelocity;
//        }
//    }
    
//    public void EnableHover()
//    {
//        _state = PlayerState.Hovering;
//        _playerRigidBody.velocity = Vector2.zero;
//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
//        _playerController.PauseInput(true);
//        _flap.CancelIfMoving();
//        Anim.PlayAnimation(ClumsyAnimations.Hover);

//        if (hoverRoutine != null)
//            StopCoroutine(hoverRoutine);
//        hoverRoutine = StartCoroutine(Hover(transform.position.y));
//    }

//    public void DisableHover()
//    {
//        _state = PlayerState.Normal;
//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
//        _playerController.PauseInput(false);
//        StopCoroutine(hoverRoutine);
//    }

//    private IEnumerator Hover(float startY)
//    {
//        const float dist = 0.3f;
//        const float interval = 0.4f;
//        float timer = 0f;
//        bool rising = true;
        
//        while (true)
//        {
//            timer += Time.deltaTime;
//            if (timer > interval)
//            {
//                timer -= interval;
//                rising = !rising;
//            }
//            transform.position += new Vector3(0f, (rising ? dist : -dist) * Time.deltaTime, 0f);
//            yield return null;
//        }
//    }
    
//    // TODO can call directly into the coroutine
//    public void Stun(float duration)
//    {
//        StartCoroutine(StunAnim(duration));
//    }

//    public IEnumerator StunAnim(float stunDuration)
//    {
//        float stunTimer = 0f;
//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
//        Anim.PlayAnimation(ClumsyAnimations.WingClose);
//        _playerController.PauseInput(true);
//        // TODO flash?
//        while (stunTimer < stunDuration)
//        {
//            if (!_bPaused)
//            {
//                stunTimer += Time.deltaTime;
//            }
//            yield return null;
//        }
//        _playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
//        Anim.PlayAnimation(ClumsyAnimations.FlapBlink);
//        _playerController.PauseInput(false);
//    }

//    public void HitByObject()
//    {
//        _flap.CancelIfMoving();
//    }
//    public void StartFog() { Fog.StartOfLevel(); }
//    public void PlaySound(PlayerSounds soundId) { _audioControl.PlaySound(soundId); }
//    public float GetHomePositionX() { return ClumsyX; }
//    public bool IsPerched() { return _state == PlayerState.Perched; }
//    public bool IsPerchedOnTop() { return _perch.IsPerchedOnTop(); }
//    public bool TouchReleasedOnBottom() { return _perch.bJumpOnTouchRelease && _perch.IsPerchedOnBottom(); }
//    public bool CanRush() { return _rush.AbilityAvailable(); }
//    public GameHandler GetGameHandler() { return _gameHandler; }
//    public void SetStateToPerched() { _state = PlayerState.Perched; }
//    public void SetStateToUnperched() { _state = PlayerState.Normal; }
//    public bool GameHasStarted() { return _state != PlayerState.Startup; }
//    public void SetMovementMode(FlapComponent.MovementMode moveMode) { _flap.Mode = moveMode; }
//    public Rigidbody2D GetBody() { return _playerRigidBody; }
//    public Collider2D GetCollider() { return _playerCollider; }
//    public SpriteRenderer GetRenderer() { return _playerRenderer; }
//    public bool IsRushing() { return _rush.IsActive(); }
//    public bool IsFacingRight() { return _flap.IsFacingRight(); }
//    public void FaceRight() { _flap.FaceRight(); }
//    public void FaceLeft() { _flap.Faceleft(); }
//    public PlayerControllerOld GetPlayerController() { return _playerController; }
//    public Vector3 GetLastContactPoint() { return lastContactPoint; }
    
//}
