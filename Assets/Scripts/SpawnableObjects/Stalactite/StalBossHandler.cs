using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalBossHandler : MonoBehaviour {
    
    public const int NumStals = 10;
    public const float IsInvalid = -100;

    private const float startPosition = 12f;
    private const float spacing = 1.178f;

    private bool[] topStals;
    private bool[] bottomStals;
    
    private void Start()
    {
        topStals = new bool[NumStals];
        bottomStals = new bool[NumStals];
    }

    public float GetFreeTopStalXPos(int startIndex = 0, int endIndex = NumStals - 1, BossStalPosition.StalTypes type = BossStalPosition.StalTypes.Spike)
    {
        if (endIndex >= NumStals) endIndex = NumStals - 1;

        List<int> validIndexes = new List<int>();
        for (int i = startIndex; i < endIndex + 1; i++)
        {
            if (!topStals[i])
                validIndexes.Add(i);
        }
        if (validIndexes.Count == 0)
            return IsInvalid;

        int index = Random.Range(0, validIndexes.Count);
        topStals[index] = true;
        return startPosition + index * spacing;
    }

    public void ClearTopStals()
    {
        for (int i = 0; i < NumStals; i++)
        {
            topStals[i] = false;
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
