﻿using UnityEngine;
using System.Collections;

public class Lantern : MonoBehaviour {

    private HingeJoint2D LanternHinge = null;
    
	void Start ()
    {
        LanternHinge = GetComponent<HingeJoint2D>();
	}
	
	void Update ()
    {
        
	}

    public void AddRushForce()
    {
        JointMotor2D LanternMotor = LanternHinge.motor;
        LanternMotor.motorSpeed = 1000;
        LanternHinge.motor = LanternMotor;
        StartCoroutine("EngageMotor");
    }

    private IEnumerator EngageMotor()
    {
        LanternHinge.useMotor = true;
        yield return new WaitForSeconds(0.2f);
        LanternHinge.useMotor = false;
    }

    public void PauseHinge()
    {
        LanternHinge.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void ResumeHinge()
    {
        LanternHinge.enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
