using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomeeBoss : Boss
{
    private Vector2 targetPosition;

    protected override void DeathSequence()
    {
        throw new System.NotImplementedException();
    }

    protected override void GetBossComponents()
    {
        Body = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<PolygonCollider2D>();
    }

    protected override void BossUpdate()
    {
        float rotation = transform.eulerAngles.z;
        transform.Rotate(Vector3.forward, -rotation);

        targetPosition = new Vector2(21f, 0f);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, targetPosition.y - transform.position.y);
    }
}
