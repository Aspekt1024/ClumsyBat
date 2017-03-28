﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAbility : BossAbility {

    private Rigidbody2D body;

    private bool chargeEnabled;
    private ChargeAction chargeCaller;

	void Start () {
        body = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		
	}

    public void Activate(ChargeAction caller)
    {
        chargeEnabled = true;
        chargeCaller = caller;
        StartCoroutine(Charge());
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "CaveWall" && chargeEnabled)
        {
            chargeEnabled = false;
            chargeCaller.HitWall();
            CameraEventListener.CameraShake(0.4f);
        }
    }

    private IEnumerator Charge()
    {
        const float chargeSpeed = 17f;

        while (chargeEnabled)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                transform.position += Vector3.left * Time.deltaTime * chargeSpeed;
            }
            yield return null;
        }
        StartCoroutine(Knockback());
    }

    private IEnumerator Knockback()
    {
        float animTimer = 0f;
        const float animDuration = 0.3f;
        body.AddForce(Vector2.up * 400);

        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                transform.position += Vector3.right * Time.deltaTime * 4f;
            }
            yield return null;
        }
        chargeCaller.Recovered();
    }
    
}