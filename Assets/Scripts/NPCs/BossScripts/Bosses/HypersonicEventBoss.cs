using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypersonicEventBoss : Boss {

    public bool IsActivated;
    public bool EventStarted;

    private List<CrystalBall> crystals = new List<CrystalBall>();
    private List<Vector2> scatterPositions = new List<Vector2>();
    
	private void Start ()
    {
		
	}
	
	private void Update ()
    {
		
	}

    protected override void GetBossComponents()
    {
        foreach (Transform tf in transform)
        {
            if (tf.name == "Body")
            {
                foreach(Transform t in tf)
                {
                    CrystalBall crystal = t.GetComponent<CrystalBall>();
                    crystal.Parent = this;
                    crystals.Add(crystal);
                }
            }
            else if (tf.name == "ScatterPositions")
            {
                foreach (Transform t in tf)
                {
                    // We're going to randomise these positions, so we won't need to match the indexes
                    scatterPositions.Add(t.position);
                }
            }
        }
    }
    
    public void CrystalTriggered(int ID)
    {
        if (!IsActivated)
        {
            IsActivated = true;
            StartCoroutine(StartEvent());
        }
        else
        {
            // TODO count all active
        }
    }
    
    private IEnumerator StartEvent()
    {
        const float animDuration = 2f;
        float animTimer = 0f;

        CameraEventListener.CameraShake(animDuration);
        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                foreach (CrystalBall crystal in crystals)
                {
                    crystal.transform.position = Vector3.Lerp(crystal.transform.position, scatterPositions[crystal.ID - 1], animTimer / (2 * animDuration));
                    crystal.transform.position = Vector3.Lerp(crystal.transform.position, crystal.transform.position + new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0f), Time.deltaTime * 2f);
                }
            }
            yield return null;
        }
        
        animTimer = 0f;
        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                foreach (CrystalBall crystal in crystals)
                {
                    crystal.transform.position = Vector3.Lerp(crystal.transform.position, crystal.EndPosition.position, animTimer / animDuration);
                    crystal.transform.position = Vector3.Lerp(crystal.transform.position, crystal.transform.position + new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0f), Time.deltaTime * 2f);
                }
            }
            yield return null;
        }
        foreach (var crystal in crystals)
        {
            StartCoroutine(crystal.CrystalFloat());
        }
        EventStarted = true;
    }
}
