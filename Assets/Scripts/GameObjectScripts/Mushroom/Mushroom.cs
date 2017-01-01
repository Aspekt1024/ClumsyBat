using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour {

    private bool bIsActive = false;
    private bool bIsTriggered = false;
    private SpriteRenderer MushroomRenderer = null;
    private Animator MushroomAnimator = null;

    private GameObject Spore;
    private CircleCollider2D SporeCollider;
    private Animator SporeAnimator = null;

    private Player Player;
    private float Speed;

    private bool Paused;

    private const float ShroomZLayer = 5f;

    void Awake () {
        MushroomRenderer = GetComponent<SpriteRenderer>();
        MushroomAnimator = GetComponent<Animator>();
        Player = FindObjectOfType<Player>();    // TODO remove this once we use triggers (needed for tooltips)

        SetupSpore();
    }
	
    void FixedUpdate()
    {
        if (!bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

	void Update ()
    {
        if (!bIsActive) { return; }

        if (!bIsTriggered && (Player.transform.position.x + 10 > transform.position.x))
        {
            bIsTriggered = true;
            StartCoroutine("PrepareSpores");
        }
	}

    private void SetupSpore()
    {
        Vector3 SporePos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
        Spore = (GameObject)Instantiate(Resources.Load("Obstacles/Spore"), SporePos, new Quaternion(), transform);
        Spore.name = "Spore";
        SporeAnimator = Spore.GetComponent<Animator>();
        SporeCollider = Spore.GetComponent<CircleCollider2D>();
        SporeCollider.offset = new Vector2(0f, 0.4f);
        Spore.SetActive(false);
    }

    private IEnumerator PrepareSpores()
    {
        float AnimationTimer = 0f;
        const float AnimationDuration = 1.2f;
        const float ReleaseSporesTime = 0.4f;
        bool bReleaseSporeAnimationTriggered = false;
        Vector3 OriginalScale = transform.localScale;

        while (AnimationTimer < AnimationDuration)
        {
            if (!Paused)
            {
                AnimationTimer += Time.deltaTime;
                if (AnimationTimer > AnimationDuration - ReleaseSporesTime && !bReleaseSporeAnimationTriggered)
                {
                    MushroomAnimator.Play("ReleaseSpore", 0, 0f);
                    bReleaseSporeAnimationTriggered = true;
                }
            }
            yield return null;
        }

        transform.localScale = OriginalScale;
        StartCoroutine("ReleaseSpores");
    }

    private IEnumerator ReleaseSpores()
    {
        const float AnimationDuration = 1f;
        const float SporeRiseTime = 0.29f;
        SporeAnimator.Play("SporeAnim", 0, 0f);
        Spore.SetActive(true);

        float AnimationTimer = 0f;
        while (AnimationTimer < AnimationDuration)
        {
            if (!Paused)
            {
                AnimationTimer += Time.deltaTime;
                if (AnimationTimer <= SporeRiseTime)
                {
                    float Distance = (1f + 2f * (AnimationTimer / SporeRiseTime));
                    float XPos = transform.position.x + Spore.transform.up.x * Distance;
                    float YPos = transform.position.y + Spore.transform.up.y * Distance;
                    Spore.transform.position = new Vector3(XPos, YPos, Spore.transform.position.z);
                }
                else
                {
                    float SporeExpandRatio = (AnimationTimer - SporeRiseTime) / (AnimationDuration - SporeRiseTime);
                    SporeCollider.radius = 1f;  // TODO could make this expand over time using SporeExpandTime
                    SporeCollider.offset = new Vector2(0f, 0.4f - 0.7f * SporeExpandRatio);
                }
            }
            yield return null;
        }
        Spore.SetActive(false);
    }

    public void DeactivateMushroom()
    {
        bIsActive = false;
        MushroomRenderer.enabled = false;
        Spore.SetActive(false);
    }

    public void ActivateMushroom()
    {
        bIsActive = true;
        bIsTriggered = false;
        MushroomRenderer.enabled = true;
        Spore.SetActive(false);
        transform.position = new Vector3(transform.position.x, transform.position.y, ShroomZLayer);
        Spore.transform.Rotate(Vector3.forward * 16f);  // Offset the spore trajectory to line up with the shroom graphic
    }

    public void SetSpeed(float _speed)
    {
        Speed = _speed;
    }

    public void SetPaused(bool PauseGame)
    {
        Paused = PauseGame;
        MushroomAnimator.enabled = !PauseGame;
        SporeAnimator.enabled = !PauseGame;
    }

}
