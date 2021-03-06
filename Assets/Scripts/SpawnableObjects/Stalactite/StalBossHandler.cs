﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalBossHandler : MonoBehaviour {
    
    public const int NumStals = 12;
    public const int IsInvalid = -100;

    private const float startPosition = 10.22f;
    private const float spacing = 1.178f;   // Calculated from NumStals and min/max position
    
    private BossStalPosition[] topStals;
    private BossStalPosition[] bottomStals;

    #region Lifecyc;e
    private void Start()
    {
        topStals = new BossStalPosition[NumStals];
        bottomStals = new BossStalPosition[NumStals];
        for (int i = 0; i < NumStals; i++)
        {
            topStals[i] = new BossStalPosition();
            bottomStals[i] = new BossStalPosition();
        }
    }

    private void OnEnable()
    {
        StalEvents.OnDestroy += StalactiteDestroyed;
    }

    private void OnDisable()
    {
        StalEvents.OnDestroy -= StalactiteDestroyed;
    }
    #endregion

    public int GetFreeTopStalIndex(int startIndex = 0, int endIndex = NumStals - 1, BossStalPosition.StalTypes type = BossStalPosition.StalTypes.Spike)
    {
        startIndex = Mathf.Clamp(startIndex, 0, NumStals - 1);
        endIndex = Mathf.Clamp(endIndex, 0, NumStals - 1);

        List<int> validIndexes = new List<int>();
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (!topStals[i].IsActive)
                validIndexes.Add(i);
        }
        if (validIndexes.Count == 0)
        {
            return IsInvalid;
        }

        int index = validIndexes[Random.Range(0, validIndexes.Count)];
        topStals[index].IsActive = true;
        return index;
    }

    public float ConvertIndexToPosition(int index)
    {
        return startPosition + index * spacing;
    }

    public void ClearTopStals(int[] stalPositionIndexes)
    {
        for (int i = 0; i < stalPositionIndexes.Length; i++)
        {
            if (i >= stalPositionIndexes.Length) break;
            int index = stalPositionIndexes[i];
            if (index >= topStals.Length) continue;
            topStals[index].IsActive = false;
        }
    }

    private void StalactiteDestroyed(int index, SpawnStalAction.StalSpawnDirection direction)
    {
        if (index < 0) return;

        if (direction == SpawnStalAction.StalSpawnDirection.FromBottom)
        {
            bottomStals[index].IsActive = false;
        }
        else
        {
            topStals[index].IsActive = false;
        }
    }
}

public class BossStalPosition
{
    public enum StalTypes
    {
        Spike, Crystal
    }

    public bool IsActive;
    public StalTypes Type;

    public BossStalPosition()
    {
        IsActive = false;
        Type = StalTypes.Spike;
    }
}
