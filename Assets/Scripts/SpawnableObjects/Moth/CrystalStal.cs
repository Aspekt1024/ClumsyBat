using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalStal : MonoBehaviour {

    private Rigidbody2D crystalBody;
    private SpriteRenderer crystalRenderer;

    private GameObject StalPrefabBroken;
    private const string BrokenStalPath = "Obstacles/Stalactite/BrokenStal";
    private bool isBroken;

    private Transform moth;
    private Animator mothAnim;
    private Moth.MothColour color;

    private MothPool mothPool;
    
    private void Awake()
    {
        GetCrystalComponents();
        ActivateCrystal(Moth.MothColour.Gold);
    }

    private void Start()
    {
        mothPool = GameObject.FindGameObjectWithTag("Scripts").GetComponent<GameHandler>().GetMothPool();
    }

    private void Update()
    {
        if (Toolbox.Instance.GamePaused) return;
        moth.Rotate(Vector3.back, 64 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !isBroken)
        {
            BreakCrystal();
            SpawnMoth();
        }
    }

    private void GetCrystalComponents()
    {
        crystalBody = GetComponent<Rigidbody2D>();
        crystalRenderer = GetComponent<SpriteRenderer>();

        foreach (Transform tf in transform)
        {
            if (tf.name == "Moth")
            {
                moth = tf;
                mothAnim = moth.GetComponent<Animator>();
            }
        }
    }

    private void ActivateCrystal(Moth.MothColour mothColor)
    {
        moth.gameObject.SetActive(true);
        color = mothColor;

        string mothAnimationName = "";
        switch (mothColor)
        {
            case Moth.MothColour.Blue:
                mothAnimationName = "MothBlueCaptured";
                break;

            case Moth.MothColour.Gold:
                mothAnimationName = "MothGoldCaptured";
                break;

            case Moth.MothColour.Green:
                mothAnimationName = "MothGreenCaptured";
                break;
        }

        mothAnim.Play(mothAnimationName, 0, 0f);

        if (StalPrefabBroken != null) Destroy(StalPrefabBroken);

    }

    private void BreakCrystal()
    {
        StalPrefabBroken = Instantiate(Resources.Load<GameObject>(BrokenStalPath), transform);
        foreach (SpriteRenderer r in StalPrefabBroken.GetComponentsInChildren<SpriteRenderer>())
        {
            r.color = new Color(1f, 1f, 1f, 0.7f);
            r.gameObject.layer = LayerMask.NameToLayer("Rubble");
        }

        crystalBody.isKinematic = false;
        moth.gameObject.SetActive(false);
        crystalRenderer.enabled = false;
        StalPrefabBroken.SetActive(true);
    }

    private void SpawnMoth()
    {
        Vector2 spawnLoc = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        spawnLoc += new Vector2(GameObject.FindGameObjectWithTag("MainCamera").transform.position.x, 0f);
        mothPool.ActivateMothFromEssence(moth.transform.position, spawnLoc, color, 5f);
    }
}
