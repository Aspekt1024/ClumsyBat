using ClumsyBat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothCrystal : Projectile {

    // Set in the inspector
    public GameObject CrystalObject;
    public GameObject MothEssenceObject;
    public BrokenCrystalBall BrokenCrystalBall;

    private SpriteRenderer crystalRenderer;
    private Animator essenceAnim;

    private Moth.MothColour mothColour;
    private Vector3 targetPos;

    private Coroutine floatRoutine;
    private Coroutine moveRoutine;

    public override void Pause()
    {
        base.Pause();
        essenceAnim.speed = 0f;
    }

    public override void Resume()
    {
        base.Resume();
        essenceAnim.speed = 1f;
    }

    public void SetColour(Moth.MothColour colour)
    {
        mothColour = colour;
        essenceAnim = MothEssenceObject.GetComponent<Animator>();

        string mothAnimationName = "";
        switch (colour)
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
        essenceAnim.Play(mothAnimationName);
    }

    public override void Activate()
    {
        targetPos = GameStatics.Camera.CurrentCamera.transform.position;
        targetPos += new Vector3(Random.Range(-5f, 5f), Random.Range(-2f, 3f), 0f);
        targetPos.z = transform.position.z;
        moveRoutine = StartCoroutine(MoveToPosition());

        crystalRenderer = CrystalObject.GetComponent<SpriteRenderer>();
        MothEssenceObject.SetActive(true);
    }

    protected override void PlayerCollision()
    {
        projectileBody.velocity = Vector2.zero;
        projectileBody.angularVelocity = 0f;
        projectileBody.isKinematic = true;

        crystalRenderer.enabled = false;
        projectileCollider.enabled = false;
        BrokenCrystalBall.ShatterAndDespawn(GameStatics.Player.Clumsy.model.position);
        
        if (floatRoutine != null)
        {
            StopCoroutine(floatRoutine);
        }

        StartCoroutine(MoveEssenceToLantern());
    }

    protected override void HypersonicCollision(Vector3 hypersonicOrigin)
    {
        projectileBody.isKinematic = true;
        projectileBody.velocity = Vector2.zero;
        projectileBody.angularVelocity = 0f;

        crystalRenderer.enabled = false;
        projectileCollider.enabled = false;
        MothEssenceObject.SetActive(false);
        BrokenCrystalBall.ShatterAndDespawn(hypersonicOrigin);

        if (floatRoutine != null)
        {
            StopCoroutine(floatRoutine);
        }
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
    }

    private IEnumerator MoveToPosition()
    {
        float timer = 0f;
        float timeBeforeMovementActivation = 0.4f;
        while (timer < timeBeforeMovementActivation)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        
        Body.isKinematic = true;

        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            Body.angularVelocity = Mathf.Lerp(Body.angularVelocity, 0f, Time.deltaTime * 3);
            Body.velocity = Vector2.Lerp(Body.velocity, Vector2.zero, Time.deltaTime * 3);

            Vector3 distance = targetPos - transform.position;
            float speed = 4f;
            Vector3 distToMove = distance.normalized * Time.deltaTime * speed;
            
            if (distToMove.sqrMagnitude <= distance.sqrMagnitude)
            {
                transform.position += distToMove;
            }

            yield return null;
        }

        Body.angularVelocity = 0;
        Body.velocity = Vector2.zero;

        floatRoutine = StartCoroutine(Float());
    }

    private IEnumerator Float()
    {
        bool isRising = Random.Range(0, 2) == 1;

        const float height = 0.1f;
        float lowPoint = targetPos.y - height / 2f;
        float period = Random.Range(0.9f, 1.5f);
        float timer = 0f;

        while (true)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                if (timer > period)
                {
                    isRising = !isRising;
                    timer = 0f;
                }
                float yPos = lowPoint;
                if (isRising)
                {
                    yPos += Mathf.Lerp(0f, height, timer / period);
                }
                else
                {
                    yPos += Mathf.Lerp(height, 0f, timer / period);
                }
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            }
            yield return null;
        }
    }

    private IEnumerator MoveEssenceToLantern()
    {
        Transform lanternTf = GameStatics.Player.Clumsy.lantern.transform;
        Transform essenceTf = MothEssenceObject.transform;

        float timer = 0f;
        const float animDuration = 0.9f;
        
        Vector3 startPosition = MothEssenceObject.transform.position;

        while (timer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                timer += Time.deltaTime;
                Vector3 endPosition = new Vector3(lanternTf.position.x, lanternTf.position.y, startPosition.z);
                float ratio = Mathf.Pow(timer / animDuration, 4);

                MothEssenceObject.transform.position = startPosition - (startPosition - endPosition) * ratio;
            }
            yield return null;
        }
        
        MothEssenceObject.SetActive(false);
        GameStatics.Player.Clumsy.lantern.ConsumeMoth(mothColour);
    }
}
