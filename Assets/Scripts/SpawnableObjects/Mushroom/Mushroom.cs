using UnityEngine;
using System.Collections;
using ClumsyBat.Players;

public class Mushroom : Spawnable {
    
    private SpriteRenderer mushroomRenderer;
    private Animator mushroomAnimator;
    private GameObject spore;
    private CircleCollider2D sporeCollider;
    private Animator sporeAnimator;
    private Player player;
    
    private bool isTriggered;
    private float sporeZLayer;

    private void Awake () {
        sporeZLayer = Toolbox.Instance.ZLayers["Spore"];
        mushroomRenderer = GetComponent<SpriteRenderer>();
        mushroomAnimator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();    // TODO remove this once we use triggers (needed for tooltips)

        SetupSpore();
    }

	private void Update ()
    {
        if (isTriggered || !(player.model.position.x + 10 > transform.position.x)) return;
        isTriggered = true;
        StartCoroutine(PrepareSpores());
    }

    protected override void Init()
    {
        isTriggered = false;
        spore.transform.position = new Vector3(transform.position.x, transform.position.y, sporeZLayer);
        sporeAnimator.Play("Normal", 0, 0f);
        spore.transform.rotation = Quaternion.identity;
    }

    private void SetupSpore()
    {
        Vector3 sporePos = new Vector3(transform.position.x, transform.position.y, sporeZLayer);
        spore = (GameObject)Instantiate(Resources.Load("Obstacles/Spore"), sporePos, Quaternion.identity, transform);
        spore.name = "Spore";
        sporeAnimator = spore.GetComponent<Animator>();
        sporeCollider = spore.GetComponent<CircleCollider2D>();
        sporeCollider.offset = new Vector2(0f, 0.4f);
        spore.SetActive(false);
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
            animationTimer += Time.deltaTime;
            if (animationTimer > animationDuration - releaseSporesTime && !bReleaseSporeAnimationTriggered)
            {
                mushroomAnimator.Play("ReleaseSpore", 0, 0f);
                bReleaseSporeAnimationTriggered = true;
            }
            yield return null;
        }

        transform.localScale = originalScale;
        StartCoroutine(ReleaseSpores());
    }

    private IEnumerator ReleaseSpores()
    {
        const float animationDuration = 1f;
        const float sporeRiseTime = 0.29f;
        spore.SetActive(true);
        sporeAnimator.Play("SporeAnim", 0, 0f);

        float animationTimer = 0f;
        while (animationTimer < animationDuration)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer <= sporeRiseTime)
            {
                float distance = (1f + 2f * (animationTimer / sporeRiseTime));
                float xPos = transform.position.x + spore.transform.up.x * distance;
                float yPos = transform.position.y + spore.transform.up.y * distance;
                spore.transform.position = new Vector3(xPos, yPos, sporeZLayer);
            }
            else
            {
                float sporeExpandRatio = (animationTimer - sporeRiseTime) / (animationDuration - sporeRiseTime);
                sporeCollider.radius = 1f;
                sporeCollider.offset = new Vector2(0f, 0.4f - 0.7f * sporeExpandRatio);
            }
            yield return null;
        }
        spore.SetActive(false);
    }

    public void DeactivateMushroom()
    {
        Deactivate();
    }

    public void Spawn(SpawnType spawnTf)
    {
        base.Spawn(transform, spawnTf);
        isTriggered = false;
        mushroomRenderer.enabled = true;
        spore.transform.localEulerAngles = Vector3.forward * 16f;
    }

    public void DestroyMushroom()
    {
        // TODO animation
        DeactivateMushroom();
    }
}
