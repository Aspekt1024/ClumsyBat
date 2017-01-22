using System;
using UnityEngine;
using System.Collections;

using StoryEventID = StoryEventControl.StoryEvents;
using PlayerSounds = ClumsyAudioControl.PlayerSounds;

public class Player : MonoBehaviour {

    #region Public GameObjects
    [HideInInspector]
    public StatsHandler Stats;  // TODO refer to DataHandler once created
    [HideInInspector]
    public Lantern Lantern;
    public FogEffect Fog;
    #endregion

    #region Abilities
    private Hypersonic _hypersonic;
    private RushAbility _rush;
    //private FlapComponent _flap;
    private Shield _shield;
    #endregion

    #region Clumsy Components
    private Rigidbody2D _playerRigidBody;
    private Rigidbody2D _lanternBody;
    private Animator _anim;
    private JumpClearance _clearance;
    private ClumsyAudioControl _audioControl;
    #endregion

    #region Clumsy Properties
    private readonly Vector3 _playerHoldingArea = new Vector3(-100f, 0f, 0f);        // Where Clumsy goes to die
    private readonly Vector2 _flapForce = new Vector2(0f, 480f);
    private readonly Vector2 _nudgeForce = new Vector2(0f, 400f);
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
        Dying,
        Dead,
        EndOfLevel
    }
    private PlayerState _state = PlayerState.Startup;
    #endregion
    
    private GameHandler _gameHandler;

    private void Awake()
    {
        _playerSpeed = 1f;
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerRigidBody.isKinematic = true;
        _playerRigidBody.gravityScale = GravityScale;
        _audioControl = gameObject.AddComponent<ClumsyAudioControl>();
        _gameHandler = FindObjectOfType<GameHandler>();
        _anim = GetComponent<Animator>();
        _lanternBody = GameObject.Find("Lantern").GetComponent<Rigidbody2D>();
        Lantern = _lanternBody.GetComponent<Lantern>();
        GameObject clearanceGameObj = GameObject.Find("JumpClearance");
        if (clearanceGameObj)
        {
            _clearance = clearanceGameObj.GetComponent<JumpClearance>();
        }
    }

    private void Start ()
    {
        CheckIfOffscreen();
        Stats = FindObjectOfType<StatsHandler>();
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
                    transform.position += Vector3.right * 0.03f; // TODO maybe add velocity rather than change displacement
                }
                else if (!_bPaused)
                {
                    _gameHandler.UpdateGameSpeed(1f);
                }
            }

            if (_bCaveEndReached)
            {
                float distance = Time.deltaTime * _playerSpeed * Toolbox.Instance.LevelSpeed;
                transform.position += new Vector3(distance, 0f, 0f);
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
        Fog = FindObjectOfType<FogEffect>();
        
        _rush.Setup(Stats, this, Lantern);
        _hypersonic.Setup(Stats, this, Lantern);
        _shield.Setup(Stats, this, Lantern);
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
        StartCoroutine("CaveExitAnimation");
    }

    private IEnumerator CaveExitAnimation()
    {
        _state = PlayerState.EndOfLevel;
        Vector2 targetExitPoint = new Vector2(Toolbox.TileSizeX / 2f, 0f);
        while (transform.position.x < targetExitPoint.x)
        {
            float xShift = Time.deltaTime * _playerSpeed * Toolbox.Instance.LevelSpeed * 2f; // TODO the *2f was added because I couldn't figure out why Clumsy's speed was ~halved in this animation
            float yShift = (targetExitPoint.y - transform.position.y) / (4 * Toolbox.Instance.LevelSpeed);
            transform.position += new Vector3(xShift, yShift, 0f);
            yield return null;
        }
        transform.position = _playerHoldingArea;
        _lanternBody.transform.position = new Vector3(_lanternBody.transform.position.x + .3f, _lanternBody.transform.position.y, _lanternBody.transform.position.z);
        _gameHandler.LevelComplete();
        Fog.EndOfLevel();
    }

    public IEnumerator CaveEntranceAnimation()
    {
        Vector3 startPoint = transform.position;
        Vector2 targetPoint = new Vector2(ClumsyX, 1.3f);

        while (transform.position.x < targetPoint.x)
        {
            float xPos = transform.position.x + Time.deltaTime * Toolbox.Instance.LevelSpeed * 1.7f;
            float xPercent = 1 - (targetPoint.x - transform.position.x) / (targetPoint.x - startPoint.x);
            float yPos = startPoint.y + (targetPoint.y - startPoint.y) * xPercent * xPercent * xPercent;
            transform.position = new Vector3(xPos, yPos, transform.position.z);
            yield return null;
        }
    }

    public void CaveEndReached()
    {
        _rush.CaveEndReached();
        _bCaveEndReached = true;
    }

    public void ActivateRush() { _rush.Activate(); }
    public void DeactivateRush() { _rush.Deactivate(); }
    public void ActivateHypersonic() { _hypersonic.ActivateHypersonic(); }
    public void AddShieldCharge() { _shield.AddCharge(); }

    public bool ActivateShield()
    {
        if (!_shield.IsAvailable()) return false;
        if (_shield.IsInUse()) return true;
        _shield.ConsumeCharge();
        return true;
    }

    public void ActivateJump()
    {
        Stats.TotalJumps++;
        _playerRigidBody.velocity = Vector2.zero;
        _playerRigidBody.AddForce(_flapForce);
        _audioControl.PlaySound(PlayerSounds.Flap);
        _anim.Play("Flap", 0, 0.5f);
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
        _gameHandler.UpdateGameSpeed(speed); // TODO Whatever sets this should actually be talking to the game handler
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
    
    // TODO manage collision events through the GameHandler
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_state != PlayerState.Normal) { return; }
        _gameHandler.Collision(other);
    }

    public void Die()
    {
        //OnDeath(EventArgs.Empty);   // Event!
        DisablePlayerController();
        _state = PlayerState.Dying;
        Stats.Deaths += 1;
        GetComponent<CircleCollider2D>().enabled = false;
        _playerRigidBody.gravityScale = GravityScale;
        _playerRigidBody.velocity = new Vector2(1, 0);
        _rush.GamePaused(true);
        Lantern.Drop();
        _gameHandler.Death();

        transform.localScale *= 4;
        transform.localRotation = Quaternion.identity;
        _anim.Play("Die", 0, 0.25f);
    }

    private IEnumerator GameOverSequence()
    {
        transform.position = _playerHoldingArea;
        _gameHandler.UpdateGameSpeed(0);
        if (!Stats.StoryData.EventCompleted(StoryEventID.FirstDeath))
        {
            yield return StartCoroutine(Stats.StoryData.TriggerEventCoroutine(StoryEventID.FirstDeath));
        }
        yield return new WaitForSeconds(1f);
        _gameHandler.GameOver();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        _gameHandler.TriggerEntered(other);
    }

    public void StartGame()
    {
        _state = PlayerState.Normal;
        _playerRigidBody.WakeUp();
        _playerRigidBody.isKinematic = false;
        Fog.Resume();
        _anim.enabled = true;
        _playerRigidBody.AddForce(_nudgeForce);
    }

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
    

    public void JumpIfClear()
    {
        if (_clearance.IsEmpty())
        {
            ActivateJump();
        }
        else
        {
            _playerRigidBody.velocity = Vector2.zero;
            _playerRigidBody.AddForce(_nudgeForce);
        }
    }

    public void FireballDeath()
    {
        _state = PlayerState.Dying;

    }

    public void StartFog() { Fog.StartOfLevel(); }
    public void PlaySound(PlayerSounds soundId) { _audioControl.PlaySound(soundId); }
    private void DisablePlayerController() { FindObjectOfType<PlayerController>().PauseInput(true); }
    public float GetHomePositionX() { return ClumsyX; }
    public float GetPlayerSpeed() { return _playerSpeed; }
    public bool AtCaveEnd() { return _bCaveEndReached; }
    public GameHandler GetGameHandler() { return _gameHandler; }
}
