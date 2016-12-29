using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the physics of the Lantern carried by Clumsy
/// </summary>
public class Lantern : MonoBehaviour {

    private HingeJoint2D LanternHinge = null;
    private Rigidbody2D LanternBody = null;
    private PolygonCollider2D LanternCollider = null;
    
	void Start ()
    {
        LanternHinge = GetComponent<HingeJoint2D>();
        LanternBody = GetComponent<Rigidbody2D>();
        LanternCollider = GetComponent<PolygonCollider2D>();
	}

    public void Drop()
    {
        LanternHinge.enabled = false;
        LanternBody.velocity = new Vector2(Random.Range(1f, 5f), 1f);
        LanternBody.AddTorque(Random.Range(100f, 600f));
        LanternCollider.enabled = true;
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
        LanternBody.velocity = Vector3.zero;
        LanternBody.isKinematic = true;
    }

    public void ResumeHinge()
    {
        LanternHinge.enabled = true;
        LanternBody.isKinematic = false;
    }
}
