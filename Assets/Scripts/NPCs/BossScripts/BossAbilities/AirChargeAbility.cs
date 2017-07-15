using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirChargeAbility : BossAbility {

    private Rigidbody2D body;
    private Boss bossScript;

    private bool chargeEnabled;
    private AirChargeAction caller;

    private float chargeSpeed;

	void Start () {
        bossScript = GetComponent<Boss>();
        if (bossScript == null)
        {
            bossScript = GetComponentInParent<Boss>();
        }
        body = bossScript.Body;
    }

    public void Activate(AirChargeAction actionRef)
    {
        chargeEnabled = false;
        caller = actionRef;

        int chargeDir = 1;
        switch (caller.ChargeDirection)
        {
            case AirChargeAction.Directions.Left:
                chargeDir = -1;
                break;

            case AirChargeAction.Directions.Right:
                chargeDir = 1;
                break;

            case AirChargeAction.Directions.Other:
                chargeDir = caller.GetInputXPosition() > transform.position.x ? 1 : -1;
                break;
        }

        bossScript.FaceDirection(chargeDir > 0 ? Boss.Direction.Right : Boss.Direction.Left);
        chargeSpeed = caller.ChargeSpeed * chargeDir;
        StartCoroutine(Windup());
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "CaveWall" && chargeEnabled)
        {
            chargeEnabled = false;
            caller.HitWall();

            // TODO play sound based on speed
            if (Mathf.Abs(chargeSpeed) > 10f)
                CameraEventListener.CameraShake(0.4f);  // TODO shake time relative to speed
        }
    }

    private IEnumerator Windup()
    {
        bossScript.Walk();

        float airLoopTimer = 0f;
        const float airLoopDuration = .9f;

        while (airLoopTimer < airLoopDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                airLoopTimer += Time.deltaTime;
                body.rotation = (chargeSpeed < 0 ? -1 : 1) * 360f * airLoopTimer / airLoopDuration;
                body.velocity = (chargeSpeed < 0 ? -1 : 1) * body.transform.right.normalized * 7f;
            }
            yield return null;
        }
        body.rotation = 0f;

        StartCoroutine(Charge());
    }

    private IEnumerator Charge()
    {
        body.position += Vector2.right * Mathf.Clamp(chargeSpeed, -1, 1) * 0.1f;
        chargeEnabled = true;
        while (chargeEnabled)
        {
            if (!Toolbox.Instance.GamePaused)
                body.velocity = Vector3.right * chargeSpeed;
            else
                body.velocity = Vector3.zero;

            yield return null;
        }
        
        StartCoroutine(Knockback());
    }

    private IEnumerator Knockback()
    {
        float knockbackTimer = 0f;
        const float knockbackDuration = 0.5f;
        while (knockbackDuration > knockbackTimer)
        {
            if (Toolbox.Instance.GamePaused)
            {
                body.velocity = Vector2.zero;
            }
            else
            {
                knockbackTimer += Time.deltaTime;
                body.velocity = new Vector2(Mathf.Lerp(-chargeSpeed / 2f, 0, knockbackTimer / knockbackDuration), 0f);
            }
            yield return null;
        }
        caller.Recovered();
    }
    
    private IEnumerator Wait(float waitTime)
    {
        float timer = 0f;
        while (timer < waitTime)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;

            yield return null;
        }
    }
}
