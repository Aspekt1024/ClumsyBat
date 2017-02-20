using System;
using UnityEngine;
using System.Collections;

using StoryEventID = StoryEventControl.StoryEvents;
using PlayerSounds = ClumsyAudioControl.PlayerSounds;

public class Player : MonoBehaviour {

    #region Public GameObjects
    [HideInInspector]
    public Lantern Lantern;
    public FogEffect Fog;
    #endregion

    #region Abilities
    private Hypersonic _hypersonic;
    private RushAbility _rush;
    private FlapComponent _flap;
    private Shield _shield;
    private PerchComponent _perch;
    #endregion

    #region Clumsy Components
    private Rigidbody2D _playerRigidBody;
    private Rigidbody2D _lanternBody;
    private Animator _anim;
    private JumpClearance _clearance;
    private ClumsyAudioControl _audioControl;
    private PlayerController _playerController;
    #endregion

    #region Clumsy Properties
    private readonly Vector3 _playerHoldingArea = new Vector3(-100f, 0f, 0f);        // Where Clumsy goes to die
    private readonly Vector2 _nudgeVelocity = new Vector2(0f, 5f);
    private const float ClumsyX = -5f;
    private const float GravityScale = 3f;
    private Vector2 _savedVelocity = Vector2.zero;
    private float _playerSpeed;

    private bool _bPaused;
    private bool _bCaveEndReached;

    private enum PlayerState
    {
        Startup,
        Normal,
        Perched,
        Hovering,
        Dying,
        Dead,
        EndOfLevel
    }
    private PlayerState _state = PlayerState.Startup;
    #endregion
    
    private GameHandler _gameHandler;
    private DataHandler _data;

    private void Awake()
    {
        _data = GameData.Instance.Data;
        _playerSpeed = 1f;
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerRigidBody.isKinematic = true;
        _playerRigidBody.gravityScale = GravityScale;
        _audioControl = gameObject.AddComponent<ClumsyAudioControl>();
        _gameHandler = FindObjectOfType<GameHandler>();
        _anim = GetComponent<Animator>();
        _lanternBody = GameObject.Find("Lantern").GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        Lantern = _lanternBody.GetComponent<Lantern>();
        GameObject clearanceGameObj = GameObject.Find("JumpClearance");
        if (clearanceGameObj)
        {
            _clearance = clearanceGameObj.GetComponent<JumpClearance>();
        }
    }

    private void Start ()
    {
        SetupAbilities();
    }

    private void Update ()
    {
        if (_state == PlayerState.Normal)
        {
            if (_clearance) { _clearance.transform.position = transform.position; }

            if (!_shield.IsInUse())
            {
                if (transform.position.x < ClumsyX)
                {
                    _gameHandler.UpdateGameSpeed(0);
                    transform.position += Vector3.right * 0.03f;
                }
                else if (!_bPaused)
                {
                    _gameHandler.UpdateGameSpeed(1f);
                }
            }

            if (_bCaveEndReached)
            {
                float distance = Time.deltaTime * _playerSpeed * Toolbox.Instance.LevelSpeed;
                _gameHandler.MovePlayerAtCaveEnd(distance);
            }
        }
        CheckIfOffscreen();
    }

    private void SetupAbilities()
    {
        GameObject abilityScripts = new GameObject("Ability Scripts");
        _rush = abilityScripts.AddComponent<RushAbility>();
        _shield = abilityScripts.AddComponent<Shield>();
        _hypersonic = FindObjectOfType<Hypersonic>();
        _perch = abilityScripts.AddComponent<PerchComponent>();
        _flap = new FlapComponent(this);
        Fog = FindObjectOfType<FogEffect>();
        
        _rush.Setup(this, Lantern);
        _hypersonic.Setup(this, Lantern);
        _shield.Setup(this, Lantern);
    }

    public bool IsAlive()
    {
        return _state != PlayerState.Dead && _state != PlayerState.Dying;
    }

    public void ExitAutoFlightReached()
    {
        CircleCollider2D clumsyCollider = GetComponent<CircleCollider2D>();
        clumsyCollider.enabled = false;
        _playerRigidBody.isKinematic = true;
        _state = PlayerState.EndOfLevel;
        StartCoroutine("CaveExitAnimation");
    }

    private IEnumerator CaveExitAnimation()
    {
        float animTimer = 0f;
        const float animDuration = 0.9f;
        Vector2 targetExitPoint = new Vector2(Toolbox.TileSizeX / 2f, 0f);
        Vector2 originalPos = transform.position;

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            float animRatio = animTimer / animDuration;
            float xPos = originalPos.x - (originalPos.x - targetExitPoint.x) * animRatio;
            float yPos = originalPos.y - (originalPos.y - targetExitPoint.y) * Mathf.Sqrt(animRatio);
            transform.position = new Vector3(xPos, yPos, transform.position.z);
            yield return null;
        }
        transform.position = _playerHoldingArea;
        _lanternBody.transform.position += new Vector3(.3f, 0f, 0f);
        _gameHandler.LevelComplete();
        Fog.EndOfLevel();
    }

    public IEnumerator CaveEntranceAnimation()
    {
        _state = PlayerState.Startup;
        float animTimer = 0f;
        const float animDuration = 0.63f;
        Vector2 startPos = new Vector2(-Toolbox.TileSizeX / 2, -0.7f);
        Vector2 targetPos = new Vector2(ClumsyX, 1.3f);

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            float animRatio = animTimer / animDuration;
            float xPos = startPos.x - (startPos.x - targetPos.x) * animRatio;
            float yPos = startPos.y - (startPos.y - targetPos.y) * Mathf.Pow(animRatio, 2);
            transform.position = new Vector3(xPos, yPos, transform.position.z);
            yield return null;
        }

        _playerRigidBody.velocity = new Vector2(0, 3f);
        _state = PlayerState.Normal;
    }

    public void CaveEndReached()
    {
        _rush.CaveEndReached();
        _bCaveEndReached = true;
    }

    public void ActivateRush() { _rush.Activate(); }
    public void DeactivateRush() { _rush.Deactivate(); }
    public void ActivateHypersonic() { _hypersonic.ActivateHypersonic(); }
    public void ForceHypersonic() { _hypersonic.ForceHypersonic(); }
    public void AddShieldCharge() { _shield.AddCharge(); }

    // TODO why is this here and not in shield? Rename if we're doing something else here.
    public bool ActivateShield()
    {
        if (!_shield.IsAvailable()) return false;
        if (_shield.IsInUse()) return true;
        _shield.ConsumeCharge();
        return true;
    }

    public void ActivateJump(InputManager.TapDirection tapDir = InputManager.TapDirection.Center)
    {
        if (_state == PlayerState.Perched)
        {
            _perch.Unperch();
        }
        else
        {
            _flap.Flap(tapDir);
        }
    }

    public void UnperchBottom()
    {
        _flap.Flap();
    }
    public void SetGravity(float gravity)
    {
        _playerRigidBody.gravityScale = Math.Abs(gravity - (-1f)) < 0.001f ? GravityScale : gravity;
    }
    public void SetVelocity(Vector2 velocity)
    {
        _playerRigidBody.velocity = velocity;
    }
    public void SetAnimation(string animationName)
    {
        _anim.Play(animationName, 0, 0f);
    }
    public void SetPlayerSpeed(float speed)
    {
        _playerSpeed = speed;
    }

    private void CheckIfOffscreen()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.y > Screen.height || screenPosition.y < 0 || screenPosition.x < -1f)
        {
            if (_state == PlayerState.Normal || _state == PlayerState.Dying)
            {
                if (_state == PlayerState.Normal) { Die(); }
                _state = PlayerState.Dead;
                transform.position = _playerHoldingArea;
                StartCoroutine("GameOverSequence");
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_state != PlayerState.Normal) { return; }
        if (other.gameObject.name.Contains("Cave") || other.gameObject.name.Contains("Entrance") || other.gameObject.name.Contains("Exit"))
        {
            if(_shield.IsInUse() || _playerController.InputPaused()) { return; }
            _perch.Perch(other.gameObject.name, _playerController.TouchHeld());
        }
        else
        {
            _gameHandler.Collision(other);
        }
    }

    public void Die()
    {
        _perch.Unperch();
        EventListener.Death();
        DisablePlayerController();
        _state = PlayerState.Dying;
        _data.Stats.Deaths += 1;
        GetComponent<CircleCollider2D>().enabled = false;
        _playerRigidBody.gravityScale = GravityScale;
        _playerRigidBody.velocity = new Vector2(1, 0);
        _rush.GamePaused(true);
        Lantern.Drop();

        transform.localScale *= 4;
        transform.localRotation = Quaternion.identity;
        _anim.Play("Die", 0, 0.25f);
    }

    private IEnumerator GameOverSequence()
    {
        transform.position = _playerHoldingArea;
        _gameHandler.UpdateGameSpeed(0);
        if (!_data.StoryData.EventCompleted(StoryEventID.FirstDeath))
        {
            yield return _data.StoryData.StartCoroutine("TriggerEventCoroutine", StoryEventID.FirstDeath);
        }
        yield return new WaitForSeconds(1f);
        _gameHandler.GameOver();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        _gameHandler.TriggerEntered(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _gameHandler.TriggerExited(other);
    }

    public void StartGame()
    {
        _state = PlayerState.Normal;
        _playerRigidBody.WakeUp();
        _playerRigidBody.isKinematic = false;
        Fog.Resume();
        _anim.enabled = true;
        _playerRigidBody.velocity = _nudgeVelocity;
    }

    // TODO clean code practices suggests PauseGame() and ResumeGame() are separate functions.
    public void PauseGame(bool gamePaused)
    {
        _bPaused = gamePaused;
        if (_bPaused)
        {
            _savedVelocity = _playerRigidBody.velocity;
            _playerRigidBody.Sleep();
            Fog.Pause();
        }
        else
        {
            _playerRigidBody.WakeUp();
            _playerRigidBody.velocity = _savedVelocity;
            Fog.Resume();
        }
        _anim.enabled = !gamePaused;
        Lantern.GamePaused(gamePaused);
        AbilitiesPaused(gamePaused);
    }

    private void AbilitiesPaused(bool bPauseAbility)
    {
        _rush.GamePaused(_state == PlayerState.Dead || bPauseAbility);
        _hypersonic.GamePaused(bPauseAbility);
        _shield.GamePaused(bPauseAbility);
    }

    private void BounceIfBottomCave(string objName)
    {
        if (objName.Contains("Bottom") || (!objName.Contains("Top") && transform.position.y < 0f))
            _playerRigidBody.velocity = _nudgeVelocity;
    }

    public void JumpIfClear()
    {
        if (_clearance.IsEmpty())
        {
            ActivateJump();
        }
        else
        {
            _playerRigidBody.velocity = _nudgeVelocity;
        }
    }


    public void EnableHover()
    {
        _state = PlayerState.Hovering;
        _playerRigidBody.velocity = Vector2.zero;
        _playerRigidBody.isKinematic = true;
        _playerController.PauseInput(true);
        StartCoroutine("Hover", transform.position.y);
    }

    public void DisableHover()
    {
        _state = PlayerState.Normal;
        _playerRigidBody.isKinematic = false;
        _playerController.PauseInput(false);
        StopCoroutine("Hover");
    }

    private IEnumerator Hover(float startY)
    {
        const float dist = 0.3f;
        const float interval = 0.4f;
        float timer = 0f;
        bool rising = true;
        
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                timer -= interval;
                rising = !rising;
            }
            transform.position += new Vector3(0f, (rising ? dist : -dist) * Time.deltaTime, 0f);
            yield return null;
        }
    }

    public void FaceOtherDirection()
    {
        Vector3 currentPos = transform.position;
        Vector3 lanternPos = _lanternBody.position;
        transform.parent.localScale = new Vector3(-transform.parent.localScale.x, transform.parent.localScale.y, transform.parent.localScale.z);
        transform.position = currentPos;
        _lanternBody.position = lanternPos;
        // TODO lantern hinge constraints
    }

    public void StartFog() { Fog.StartOfLevel(); }
    public void PlaySound(PlayerSounds soundId) { _audioControl.PlaySound(soundId); }
    private void DisablePlayerController() { FindObjectOfType<PlayerController>().PauseInput(true); }
    public float GetHomePositionX() { return ClumsyX; }
    public float GetPlayerSpeed() { return _playerSpeed; }
    public bool AtCaveEnd() { return _bCaveEndReached; }
    public bool IsPerched() { return _state == PlayerState.Perched; }
    public bool IsPerchedOnTop() { return _perch.IsPerchedOnTop(); }
    public bool TouchReleasedOnBottom() { return _perch.bJumpOnTouchRelease && _perch.IsPerchedOnBottom(); }
    public bool CanRush() { return _rush.AbilityAvailable(); }
    public GameHandler GetGameHandler() { return _gameHandler; }
    public void SwitchPerchState() { _state = _state == PlayerState.Perched ? PlayerState.Normal : PlayerState.Perched; }
    public bool GameHasStarted() { return _state != PlayerState.Startup; }
    public void SetMovementMode(FlapComponent.MovementMode moveMode) { _flap.Mode = moveMode; }
}
