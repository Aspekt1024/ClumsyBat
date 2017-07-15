using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPiece : MonoBehaviour {

    private Boss bossScript;
    
	private void Start ()
    {
	}
	
	private void Update ()
    {
		
	}

    public void SetBossScript(Boss script)
    {
        bossScript = script;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bossScript.TriggerEnter(other);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        bossScript.CollisionEnter(other);
    }
}
