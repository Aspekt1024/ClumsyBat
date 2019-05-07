using UnityEngine;
using System.Collections;
using ClumsyBat;

public class Moth : Spawnable {

    #region Level Editor Inspector fields
    public MothColour Colour;
    public MothPathHandler.MothPathTypes PathType;
    #endregion

    [HideInInspector]
    public Transform MothSprite;
    private Animator _mothAnimator;
    private Collider2D _mothCollider;
    private Transform _lantern;
    private MothPathHandler _pathHandler;
    private bool _bConsumption;
    private ParticleSystem shimmerEffect;
    
    public enum MothColour { Green, Gold, Blue }
    
    private void Awake ()
    {
        GetMothComponents();
    }
	
	private void FixedUpdate ()
    {
        _pathHandler.MoveAlongPath(Time.fixedDeltaTime);
    }

    public void ConsumeMoth()
    {
        if (!_bConsumption)
        {
            _bConsumption = true;
            _mothCollider.enabled = false;
            StartCoroutine(ConsumeAnim());
        }
    }

    protected override void Init()
    {
        gameObject.SetActive(true);
        _mothCollider.enabled = true;
        PlayNormalAnimation();
    }

    private IEnumerator ConsumeAnim()
    {
        PlayExplosionAnim();

        float animTimer = 0f;
        const float animDuration = 1f;

        float lantenFollowTime = animDuration / 2f;
        Vector3 startPos = new Vector3();
        bool bStartPosSet = false;

        while (animTimer < animDuration)
        {
            animTimer += Time.deltaTime;
            if (animTimer > animDuration - lantenFollowTime)
            {
                if (!bStartPosSet)
                {
                    startPos = MothSprite.position;
                    bStartPosSet = true;
                }
                float timeRatio = (animTimer - (animDuration - lantenFollowTime)) / lantenFollowTime;
                float xOffset = startPos.x - (startPos.x - _lantern.position.x) * timeRatio;
                float yOffset = startPos.y - (startPos.y - _lantern.position.y) * timeRatio;
                MothSprite.position = new Vector3(xOffset, yOffset, startPos.z);
            }
            yield return null;
        }
        
        GameStatics.Player.Clumsy.lantern.ConsumeMoth(Colour);

        Deactivate();
        _bConsumption = false;
    }

    private void PlayNormalAnimation()
    {
        _mothAnimator.enabled = true;
        Color color = new Color();
        switch (Colour)
        {
            case MothColour.Green:
                _mothAnimator.Play("MothGreenAnimation", 0, 0f);
                color = Toolbox.MothGreenColor;
                break;
            case MothColour.Blue:
                _mothAnimator.Play("MothBlueAnimation", 0, 0f);
                color = Toolbox.MothBlueColor;
                break;
            case MothColour.Gold:
                _mothAnimator.Play("MothGoldAnimation", 0, 0f);
                color = Toolbox.MothGoldColor;
                break;
        }
        ParticleSystem.MainModule particleSettings = shimmerEffect.main;
        particleSettings.startColor = new Color(color.r, color.g, color.b, 0.7f);
    }

    private void PlayExplosionAnim()
    {
        shimmerEffect.Stop();
        switch (Colour)
        {
            case MothColour.Green:
                _mothAnimator.Play("MothGreenExplosion", 0, 0f);
                break;
            case MothColour.Blue:
                _mothAnimator.Play("MothBlueExplosion", 0, 0f);
                break;
            case MothColour.Gold:
                _mothAnimator.Play("MothGoldExplosion", 0, 0f);
                break;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        MothSprite.transform.position = transform.position;
    }

    public void Activate(SpawnType spawnTf, MothColour colour, MothPathHandler.MothPathTypes pathType = MothPathHandler.MothPathTypes.Spiral)
    {
        _lantern = GameStatics.Player.Clumsy.lantern.transform;
        base.Spawn(transform, spawnTf);
        _mothAnimator.enabled = true;
        _mothCollider.enabled = true;
        MothSprite.transform.position = transform.position;
        _pathHandler.SetPathType(pathType);
        Colour = colour;
        PlayNormalAnimation();
    }

    public IEnumerator SpawnFromEssence(Vector2 endLocation, float despawnTimer)
    {
        _mothCollider.enabled = false;

        PlayExplosionAnim();
        yield return StartCoroutine(MoveToLocation(endLocation, 0.9f));

        PlayNormalAnimation();
        yield return new WaitForSeconds(0.2f);
        
        if (despawnTimer <= 0)
        {
            StartCoroutine(DespawnFromEssence());
            yield break;
        }

        _mothCollider.enabled = true;
        yield return new WaitForSeconds(despawnTimer);

    }

    private IEnumerator DespawnFromEssence()
    {
        _mothCollider.enabled = false;
        PlayExplosionAnim();
        yield return new WaitForSeconds(0.9f);

        base.Deactivate();
        MothSprite.transform.position = transform.position;
    }

    public IEnumerator CollectFromCrystal()
    {
        PlayExplosionAnim();
        Vector2 startPos = MothSprite.transform.position;
        Vector2 pathLoc = GameStatics.Player.Clumsy.model.position + (GameStatics.Player.Clumsy.IsFacingRight ? Vector3.right : Vector3.left) * 2f;

        float timer = 0f;
        const float duration = 0.8f;
        while (timer < duration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                MothSprite.transform.position = Vector2.Lerp(startPos, pathLoc, timer / duration);
            }
            yield return null;
        }
        ConsumeMoth();
    }

    private IEnumerator MoveToLocation(Vector2 endPos, float animTime)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(endPos.x, endPos.y, startPosition.z);
        float timer = 0f;
        while (timer < animTime)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;

            float ratio = Mathf.Pow(timer / animTime, 4);
            transform.position = startPosition - (startPosition - endPosition) * ratio;
            yield return null;
        }
        transform.position = endPosition;
    }

    private void GetMothComponents()
    {
        //_mothInteractor = gameObject.AddComponent<MothInteractivity>();
        _pathHandler = new MothPathHandler(this);
        foreach (Transform tf in transform)
        {
            if (tf.name == "MothTrigger")
            {
                MothSprite = tf;
                _mothAnimator = tf.GetComponent<Animator>();
                _mothAnimator.enabled = true;
                _mothCollider = tf.GetComponent<Collider2D>();
                foreach (Transform t in tf)
                {
                    if (t.name == "ShimmerEffect")
                    {
                        shimmerEffect = t.GetComponent<ParticleSystem>();
                    }
                }
            }
        }
    }
    
}
