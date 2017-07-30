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
            int numActive = 0;
            foreach(var crystal in crystals)
            {
                if (crystal.IsActive)
                    numActive++;
            }
            if (numActive == crystals.Count)
            {
                HypersonicEventBossWinSequence();
            }
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

    private void HypersonicEventBossWinSequence()
    {
        foreach(var crystal in crystals)
        {
            crystal.Explode();
            Toolbox.Player.EnableHover();
            StartCoroutine(MovePlayerToCenterAndEndScene());
        }
    }

    private IEnumerator MovePlayerToCenterAndEndScene()
    {
        float timer = 0f;
        const float duration = 7f;

        if (Toolbox.Player.transform.position.x > Toolbox.PlayerCam.transform.position.x)
            Toolbox.Player.FaceLeft();
        else
            Toolbox.Player.FaceRight();

        CameraEventListener.CameraShake(duration - 1f);
        while (timer < duration)
        {
            Vector2 pos = Vector2.Lerp(Toolbox.Player.transform.position, Toolbox.PlayerCam.transform.position, Time.deltaTime / 2);
            Toolbox.Player.transform.position = new Vector3(pos.x, pos.y, Toolbox.Player.transform.position.z);
            timer += Time.deltaTime;
            yield return null;
        }

        var hypersonic = GameData.Instance.Data.AbilityData.GetHypersonicStats();
        hypersonic.AbilityUnlocked = true;
        hypersonic.AbilityAvailable = true;
        hypersonic.AbilityLevel = 1;
        hypersonic.AbilityEvolution = 1;
        GameData.Instance.Data.AbilityData.SaveHypersonicStats(hypersonic);

        Toolbox.Player.ForceHypersonic();
        timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Toolbox.Tooltips.ShowDialogue("It worked! Any time you collect a gold moths, you will activate Hypersonic!", 2f, true);
        while (Toolbox.Tooltips.IsPausedForTooltip)
        {
            yield return null;
        }
        
        timer = 0f;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Toolbox.Player.GetGameHandler().LevelComplete();
    }
}
