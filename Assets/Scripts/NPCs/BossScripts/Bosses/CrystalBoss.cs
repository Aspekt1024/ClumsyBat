using System.Collections;
using System.Collections.Generic;
using ClumsyBat;
using UnityEngine;

public class CrystalBoss : Boss {

    public Moth.MothColour MothColour;
    public enum CrystalModes
    {
        Timed, Order
    }
    public CrystalModes Mode;
    public int[] Order = new int[5];

    [HideInInspector]
    public bool IsActivated;
    [HideInInspector]
    public bool EventStarted;
    
    protected List<CrystalBall> crystals = new List<CrystalBall>();

    private List<Vector2> scatterPositions = new List<Vector2>();
    
    private void Start ()
    {
        if (Mode != CrystalModes.Order) return;

        foreach (CrystalBall crystal in crystals)
        {
            crystal.SetOrderedMode();
        }
    }

    private void Update()
    {

    }

    protected override void DeathSequence()
    {

    }

    protected virtual void CrystalBossWinSequence()
    {

    }

    protected override void GetBossComponents()
    {
        foreach (Transform tf in transform)
        {
            if (tf.name == "Body")
            {
                foreach (Transform t in tf)
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


    public void CrystalTriggered(int id)
    {
        if (!IsActivated)
        {
            IsActivated = true;
            StartCoroutine(StartEvent());
        }
        else if (EventStarted)
        {
            if (Mode == CrystalModes.Order)
            {
                TriggerOrderedCrystal(id);
            }
            else
            {
                int numActive = 0;
                foreach (var crystal in crystals)
                {
                    if (crystal.IsActive)
                        numActive++;
                }
                if (numActive == crystals.Count)
                {
                    CrystalBossWinSequence();
                }
            }
        }
    }

    private void TriggerOrderedCrystal(int id)
    {
        int numActive = 0;
        int index = GetCrystalIndexFromID(Order[numActive]);

        while (crystals[index].IsActive)
        {
            numActive++;
            index = GetCrystalIndexFromID(Order[numActive]);
        }

        if (numActive == crystals.Count - 1)
        {
            CrystalBossWinSequence();
            GameStatics.Audio.Boss.PlaySound(BossSounds.BossCrystalActivate);
        }
        else if (crystals[index].ID == id)
        {
            crystals[index].Activate();
        }
        else
        {
            ResetCrystals(numActive);
            crystals[GetCrystalIndexFromID(id)].Pulse();
        }
    }

    private int GetCrystalIndexFromID(int id)
    {
        for (int i = 0; i < crystals.Count; i++)
        {
            if (crystals[i].ID == id)
                return i;
        }
        return -1;
    }

    private void ResetCrystals(int numActive)
    {
        GameStatics.Audio.Boss.PlaySound(BossSounds.BossCrystalDeactivate);
        for (int i = 0; i < crystals.Count; i++)
        {
            crystals[i].Deactivate();
        }
    }

    private IEnumerator StartEvent()
    {
        const float animDuration = 2f;
        float animTimer = 0f;

        GameStatics.Camera.Shake(animDuration);
        GameStatics.Audio.Boss.PlaySound(BossSounds.BossCrystalStartup);
        GameStatics.Audio.Boss.PlaySound(BossSounds.BossCrystalRumble2s);
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
        GameStatics.Audio.Boss.PlaySound(BossSounds.BossCrystalActivate);
    }


}
