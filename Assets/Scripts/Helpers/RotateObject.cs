using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

    public bool RandomRotation;
    public float RandomMin;
    public float RandomMax;
    public float DegreesPerSecond;

    private bool isEnabled;
    private float rotationPerSec;

    public void DisableRotation()
    {
        isEnabled = false;
    }
    
	private void Start ()
    {
        isEnabled = true;
        if (RandomRotation)
        {
            rotationPerSec = Random.Range(RandomMin, RandomMax);
        }
        else
        {
            rotationPerSec = DegreesPerSecond;
        }
	}

    private void Update()
    {
        if (!isEnabled || Toolbox.Instance.GamePaused) return;
        
        transform.Rotate(Vector3.forward, rotationPerSec * Time.deltaTime);
    }
}
