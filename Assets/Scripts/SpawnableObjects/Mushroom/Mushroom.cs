using UnityEngine;
using System.Collections;

public class Mushroom : Spawnable {
    
    private SpriteRenderer _mushroomRenderer;
    private Animator _mushroomAnimator;
    private GameObject _spore;
    private CircleCollider2D _sporeCollider;
    private Animator _sporeAnimator;
    private Player _player;
    
    private bool _bIsTriggered;
    private float _sporeZLayer;

    private void Awake () {
        _sporeZLayer = Toolbox.Instance.ZLayers["Spore"];
        _mushroomRenderer = GetComponent<SpriteRenderer>();
        _mushroomAnimator = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();    // TODO remove this once we use triggers (needed for tooltips)

        SetupSpore();
    }
	
    private void FixedUpdate()
    {
        if (!IsActive) { return; }
        MoveLeft(Time.deltaTime);
    }

	private void Update ()
    {
        if (!IsActive || _bIsTriggered || !(_player.transform.position.x + 10 > transform.position.x)) return;
        _bIsTriggered = true;
        StartCoroutine("PrepareSpores");
    }

    private void SetupSpore()
    {
        Vector3 sporePos = new Vector3(transform.position.x, transform.position.y, _sporeZLayer);
        _spore = (GameObject)Instantiate(Resources.Load("Obstacles/Spore"), sporePos, new Quaternion(), transform);
        _spore.name = "Spore";
        _sporeAnimator = _spore.GetComponent<Animator>();
        _sporeCollider = _spore.GetComponent<CircleCollider2D>();
        _sporeCollider.offset = new Vector2(0f, 0.4f);
        _spore.SetActive(false);
    }

    private IEnumerator PrepareSpores()
    {
        float animationTimer = 0f;
        const float animationDuration = 1.2f;
        const float releaseSporesTime = 0.4f;
        bool bReleaseSporeAnimationTriggered = false;
        Vector3 originalScale = transform.localScale;

        while (animationTimer < animationDuration)
        {
            if (!bPaused)
            {
                animationTimer += Time.deltaTime;
                if (animationTimer > animationDuration - releaseSporesTime && !bReleaseSporeAnimationTriggered)
                {
                    _mushroomAnimator.Play("ReleaseSpore", 0, 0f);
                    bReleaseSporeAnimationTriggered = true;
                }
            }
            yield return null;
        }

        transform.localScale = originalScale;
        StartCoroutine("ReleaseSpores");
    }

    private IEnumerator ReleaseSpores()
    {
        const float animationDuration = 1f;
        const float sporeRiseTime = 0.29f;
        _sporeAnimator.Play("SporeAnim", 0, 0f);
        _spore.SetActive(true);

        float animationTimer = 0f;
        while (animationTimer < animationDuration)
        {
            if (!bPaused)
            {
                animationTimer += Time.deltaTime;
                if (animationTimer <= sporeRiseTime)
                {
                    float distance = (1f + 2f * (animationTimer / sporeRiseTime));
                    float xPos = transform.position.x + _spore.transform.up.x * distance;
                    float yPos = transform.position.y + _spore.transform.up.y * distance;
                    _spore.transform.position = new Vector3(xPos, yPos, _sporeZLayer);
                }
                else
                {
                    float sporeExpandRatio = (animationTimer - sporeRiseTime) / (animationDuration - sporeRiseTime);
                    _sporeCollider.radius = 1f;  // TODO could make this expand over time using SporeExpandTime
                    _sporeCollider.offset = new Vector2(0f, 0.4f - 0.7f * sporeExpandRatio);
                }
            }
            yield return null;
        }
        _spore.SetActive(false);
    }

    public void DeactivateMushroom()
    {
        SendToInactivePool();
    }

    public void Activate(SpawnType spawnTf)
    {
        base.Activate(transform, spawnTf);
        _bIsTriggered = false;
        _mushroomRenderer.enabled = true;
        _spore.transform.Rotate(Vector3.forward * 16f);  // Offsets the spore trajectory to line up with the shroom graphic
    }

    public override void PauseGame(bool pauseGame)
    {
        base.PauseGame(pauseGame);
        _mushroomAnimator.enabled = !pauseGame;
        _sporeAnimator.enabled = !pauseGame;
    }

    public void DestroyMushroom()
    {
        // TODO animation
        // TODO coroutine
        // TODO whatever
        DeactivateMushroom();
    }
}
