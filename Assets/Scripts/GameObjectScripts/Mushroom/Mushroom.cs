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

    private const float ShroomZLayer = 5f;

    void Awake () {
        MushroomRenderer = GetComponent<SpriteRenderer>();
        MushroomAnimator = GetComponent<Animator>();
        Player = FindObjectOfType<Player>();

        Spore = (GameObject)Instantiate(Resources.Load("Obstacles/Spore"));
        Spore.transform.position = Toolbox.Instance.HoldingArea;
        Spore.name = "Spore";
        Spore.SetActive(false);
        SporeAnimator = Spore.GetComponent<Animator>();
        SporeCollider = Spore.GetComponent<CircleCollider2D>();
    }
	
    void FixedUpdate()
    {
        if (!bIsActive) { return; }
        transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
    }

	void Update ()
    {
        if (!bIsActive) { return; }
        
        if (Spore.activeSelf)
        {
            Spore.transform.position = new Vector3(Spore.transform.position.x - Speed * Time.deltaTime, Spore.transform.position.y, Spore.transform.position.z);
        }

        if (!bIsTriggered && (Player.transform.position.x + 10 > transform.position.x))
        {
            bIsTriggered = true;
            StartCoroutine("PrepareSpores");
        }
	}

    private IEnumerator PrepareSpores()
    {
        float AnimationTimer = 0f;
        const float AnimationDuration = 1.2f;
        Vector3 OriginalScale = transform.localScale;

        MushroomAnimator.Play("ReleaseSpore", 0, 0);
        while (AnimationTimer < AnimationDuration)
        {
            if (Speed > 0)
            {
                AnimationTimer += Time.deltaTime;
            }
            yield return null;
        }

        transform.localScale = OriginalScale;
        StartCoroutine("ReleaseSpores");
    }

    private IEnumerator ReleaseSpores()
    {
        MushroomAnimator.Play("Normal", 0, 0);
        SporeAnimator.Play("Rise", 0, 0);
        Spore.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - 0.1f);
        Spore.SetActive(true);

        const float AnimationDuration = 0.8f;
        float AnimationTimer = 0f;
        while (AnimationTimer < AnimationDuration)
        {
            if (Speed > 0)
            {
                MoveSporeUpward(AnimationTimer / AnimationDuration, 1f, 3f);
                AnimationTimer += Time.deltaTime;
            }
            yield return null;
        }
        StartCoroutine("DissipateSpore");
    }

    private IEnumerator DissipateSpore()
    {
        SporeAnimator.Play("Grow", 0, 0);
        const float AnimationDuration = 0.3f;
        float AnimationTimer = 0f;
        while (AnimationTimer < AnimationDuration)
        {
            if (Speed > 0)
            {
                MoveSporeUpward(AnimationTimer / AnimationDuration, 3f, 4f);
                AnimationTimer += Time.deltaTime;
                IncreaseSporeCollider(AnimationTimer / AnimationDuration);
            }
            yield return null;
        }
        Spore.SetActive(false);
    }

    private void IncreaseSporeCollider(float AnimationPosition)
    {
        SporeCollider.radius = 0.4f + 0.4f * AnimationPosition;
    }

    private void MoveSporeUpward(float AnimationPosition, float StartY, float EndY)
    {
        float YPos = transform.position.y + StartY + (AnimationPosition * (EndY - StartY));
        Spore.transform.position = new Vector3(Spore.transform.position.x, YPos, Spore.transform.position.z);
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
    }

    public void SetSpeed(float _speed)
    {
        Speed = _speed;
        if (_speed == 0)
        {
            MushroomAnimator.enabled = false;
            SporeAnimator.enabled = false;
        }
        else
        {
            MushroomAnimator.enabled = true;
            SporeAnimator.enabled = true;
        }
    }

}
