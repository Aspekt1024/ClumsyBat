using System.Collections;
using System.Collections.Generic;
using ClumsyBat;
using UnityEngine;

public class ChargeAbility : BossAbility {

    private Rigidbody2D body;
    private Boss bossScript;

    private bool chargeEnabled;
    private ChargeAction caller;

    private float chargeSpeed;

	void Start () {
        body = GetComponent<Rigidbody2D>();
        bossScript = GetComponent<Boss>();
	}
	
    public void Activate(ChargeAction actionRef)
    {
        chargeEnabled = true;
        caller = actionRef;

        int chargeDir = 1;
        switch (caller.ChargeDirection)
        {
            case ChargeAction.Directions.Left:
                chargeDir = -1;
                break;

            case ChargeAction.Directions.Right:
                chargeDir = 1;
                break;

            case ChargeAction.Directions.Other:
                chargeDir = caller.GetInputXPosition() > transform.position.x ? 1 : -1;
                break;
        }

        chargeSpeed = caller.ChargeSpeed * chargeDir;
        StartCoroutine(Windup());
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("CaveWall") && chargeEnabled)
        {
            chargeEnabled = false;
            caller.HitWall();
            GameStatics.Audio.Boss.PlaySound(BossSounds.BossCrash);

            if (Mathf.Abs(chargeSpeed) > 10f)
            {
                GameStatics.Camera.Shake(0.4f);
            }
        }
    }

    private IEnumerator Windup()
    {
        if (chargeSpeed > 0)
            GetComponent<Boss>().FaceDirection(Boss.Direction.Right);
        else
            GetComponent<Boss>().FaceDirection(Boss.Direction.Left);
        
        body.AddForce(Vector2.up * 500f);

        const float windupTime = 1f;
        yield return StartCoroutine(Wait(windupTime));
        bossScript.Walk();
        StartCoroutine(Charge());
    }

    private IEnumerator Charge()
    {
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
        bossScript.EndWalk();

        float animTimer = 0f;
        const float animDuration = 0.3f;
        body.AddForce(Vector2.up * 400);

        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                transform.position += Vector3.left * Time.deltaTime * 4f * chargeSpeed / 17f;
            }
            yield return null;
        }

        animTimer = 0f;
        const float recoveryTime = 0.5f;
        yield return StartCoroutine(Wait(recoveryTime));

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
