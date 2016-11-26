using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public delegate void PlayerDeathHandler(object sender, EventArgs e);

/// <summary>
/// Handles the player input (touch screen)
/// Determines how the player jumps and what happens when they hit an object
/// </summary>
public class PlayerController : MonoBehaviour
{
    public FogEffect Fog;
    public LevelScript Level;
    public Text PauseText;

    private Vector3 PlayerStartPos = new Vector3(-5.5f, -1.5f, -2f);
    private Vector3 PlayerHoldingArea = new Vector3(-10f, -10f, 0f);
    private Rigidbody2D PlayerRigidBody;

    private bool bGameStarted = false;
    private bool bGamePaused = false;
    private bool bGameResuming = false;
    private bool bAtEnd = false;
    private float ResumeTimerStart;
    private float ResumeTimer = 3f;
    private Vector2 SavedVelocity = Vector2.zero;

    private bool bIsAlive = true;
    private bool bIsRushing = false;
    private bool bGameOverOverlay = false;

    public Vector2 JumpForce = new Vector2(0, 700);
    private const float GravityScale = 3;
    private float PlayerSpeed;
    private float RushStartTime;
    private const float RushTimer = 0.26f;
    private const float RushSpeed = 7f;

    private GameObject InputObject;
    private SwipeManager InputManager;
    private Animator Anim;

    public event PlayerDeathHandler PlayerDeath; // not currently used. Kept for reference (events!)
    
    protected virtual void OnDeath(EventArgs e)
    {
        if (PlayerDeath != null)
        {
            PlayerDeath(this, e);
        }
    }
    
    void Start()
    {
        PlayerSpeed = 1f;
        PlayerRigidBody = GetComponent<Rigidbody2D>();
        PlayerRigidBody.isKinematic = true;
        PlayerRigidBody.gravityScale = GravityScale;

        Anim = GetComponent<Animator>();
        Anim.enabled = false;

        InputObject = new GameObject();
        InputManager = InputObject.AddComponent<SwipeManager>();
    }
    
    void Update()
    {
        if (bGamePaused || bGameResuming || bGameOverOverlay) { return; }

        if (bIsAlive)
        {
            if (bGameStarted && !bGameResuming)
            {
                Level.AddDistance(Time.deltaTime, PlayerSpeed, bIsRushing);
                if (Time.time - RushStartTime > RushTimer && bIsRushing)
                {
                    StartCoroutine("EndRushAnim");
                }
                if (Level.AtCaveEnd())
                {
                    bAtEnd = true;
                    transform.position += new Vector3(Time.deltaTime * PlayerSpeed * Toolbox.Instance.LevelSpeed, 0f, 0f);
                }
            }

            if (InputManager.SwipeRegistered())
            {
                Rush();
            }
            if (InputManager.TapRegistered())
            {
                Jump();
            }
        }

        // For debugging
        if (Input.GetKeyUp("space"))
        {
            Jump();
        } else if (Input.GetKeyUp("a"))
        {
            Rush();
        }

        // Die by being off-screen
        Vector2 ScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (ScreenPosition.y > Screen.height || ScreenPosition.y < 0)
        {
            if (bIsAlive)
            {
                Die();
                transform.position = PlayerHoldingArea;
            }
            else
            {
                Level.ShowGameoverMenu();
                bGameOverOverlay = true;
            }
        }
    }

    void Jump()
    {
        if (!bIsAlive) { return; }

        if (!bGameStarted) { StartGame(); }
        if (bGamePaused) { ResumeGame(); }

        Level.Stats.TotalJumps++;
        PlayerRigidBody.velocity = Vector2.zero;
        PlayerRigidBody.AddForce(JumpForce);
        GetComponent<AudioSource>().Play();
        Anim.Play("Flap", 0, 0.5f);
    }

    void Rush()
    {
        if (!bIsAlive || bIsRushing || bGamePaused || !bGameStarted) { return; }
        Level.Stats.TimesDashed++;
        StartCoroutine("StartRushAnim");
        // TODO Rush cooldown and failed rush when on cooldown?
    }

    // Die by Collision
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<Stalactite>())
        {
            Level.Stats.ToothDeaths++;
        }
        else
        {
            Level.Stats.RockDeaths++;
        }
        Die();
    }

    void Die()
    {
        Level.Stats.Deaths += 1;
        OnDeath(EventArgs.Empty);
        bIsAlive = false;
        GetComponent<CircleCollider2D>().enabled = false;
        PlayerRigidBody.gravityScale = GravityScale;
        PlayerRigidBody.velocity = new Vector2(1, 0);
        Level.HorribleDeath();
        Fog.PlayerDeath();
        
        Anim.Play("Die", 0, 0.25f);
        //Anim.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.name)
        {
            case "MothTrigger":
                Level.Stats.MothsEaten++;
                if (Level.Stats.MothsEaten > Level.Stats.MostMoths)
                {
                    Level.Stats.MostMoths++;
                }
                Level.Stats.TotalMoths++;
                Moth MothScript = other.GetComponentInParent<Moth>();
                MothScript.ReturnToInactivePool();
                if (MothScript.IsGold)
                {
                    // TODO Level.Stats.GoldMoths++; etc
                    Hypersonic();
                    Fog.Echolocate();
                }
                else
                {
                    // TODO Level.Stats.GreenMoths++; etc
                    Fog.Echolocate();
                }
                break;
            case "ExitTrigger":
                CircleCollider2D ClumsyCollider = GetComponent<CircleCollider2D>();
                ClumsyCollider.enabled = false;
                PlayerRigidBody.isKinematic = true;
                StartCoroutine("CaveExitAnimation");
                break;
            case "Spore":
                Fog.Minimise();
                break;
        }
    }

    void StartGame()
    {
        bGameStarted = true;
        PlayerRigidBody.WakeUp();
        PlayerRigidBody.isKinematic = false;
        Level.StartGame();
        Fog.Resume();
        Anim.enabled = true;
    }

    public void PauseGame()
    {
        bGamePaused = true;
        Level.PauseGame();
        SavedVelocity = PlayerRigidBody.velocity;
        PlayerRigidBody.Sleep();
        Fog.Pause();
        Level.Stats.SaveStats();
        Anim.enabled = false;
    }

    void Hypersonic()
    {
        Level.DestroyOnScreenEvils();
    }

    public void ResumeGame()
    {
        // Start the resume sequence (countdown from 3)
        if (bGameResuming) { return; }
        Level.Stats.SaveStats();
        ResumeTimerStart = Time.time;
        bGameResuming = true;
        PauseText.text = ResumeTimer.ToString();
        StartCoroutine("UpdateResumeTimer");
    }

    IEnumerator UpdateResumeTimer()
    {
        while (ResumeTimerStart + ResumeTimer > Time.time)
        {
            float TimeRemaining = ResumeTimerStart + ResumeTimer - Time.time;
            PauseText.text = Mathf.Ceil(TimeRemaining).ToString();
            yield return null;
        }
        ResumeGameplay();
    }

    void ResumeGameplay()
    {
        bGamePaused = false;
        bGameResuming = false;
        PauseText.enabled = false;
        PlayerRigidBody.WakeUp();
        PlayerRigidBody.velocity = SavedVelocity;
        Level.ResumeGame();
        Fog.Resume();
        Anim.enabled = true;
    }

    IEnumerator StartRushAnim()
    {
        bIsRushing = true;
        RushStartTime = Time.time;
        PlayerRigidBody.gravityScale = 0f;
        PlayerRigidBody.velocity = new Vector2(8f, 0f);

        Anim.Play("Rush", 0, 0);

        yield return new WaitForSeconds(0.07f);

        if (bIsAlive)
        {
            PlayerSpeed = RushSpeed;
            Level.UpdateGameSpeed(PlayerSpeed);
            PlayerRigidBody.velocity = Vector2.zero;
        }
    }

    IEnumerator EndRushAnim()
    {
        PlayerSpeed = 1f;
        Level.UpdateGameSpeed(PlayerSpeed);
        bIsRushing = false;
        PlayerRigidBody.gravityScale = GravityScale;
        
        while (PlayerRigidBody.position.x > PlayerStartPos.x && !bAtEnd)
        {
            float NewSpeed = 3 - (2*PlayerRigidBody.position.x / (PlayerRigidBody.position.x + PlayerStartPos.x));
            PlayerRigidBody.velocity = new Vector2(-NewSpeed, PlayerRigidBody.velocity.y);
            yield return null;
        }
        PlayerRigidBody.velocity = new Vector2(0, PlayerRigidBody.velocity.y);
        Anim.Play("Flap", 0, 0);
    }

    IEnumerator CaveExitAnimation()
    {
        Vector2 TargetExitPoint = new Vector2(Toolbox.TileSizeX / 2f, 0f);
        while (transform.position.x < TargetExitPoint.x)
        {
            float XShift = Time.deltaTime * Toolbox.Instance.LevelSpeed;
            float YShift = (TargetExitPoint.y - transform.position.y) / (4 * Toolbox.Instance.LevelSpeed);
            transform.position += new Vector3(XShift, YShift, 0f);
            yield return null;
        }
        Level.LevelWon();
        bGamePaused = true;
    }

    IEnumerator CaveEntranceAnimation()
    {
        yield return null;
    }
}

