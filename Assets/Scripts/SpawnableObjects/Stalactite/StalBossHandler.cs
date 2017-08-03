using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalBossHandler : MonoBehaviour {
    
    public const int NumStals = 10;
    public const float IsInvalid = -100;

    private const float startPosition = 12f;
    private const float spacing = 1.178f;

    private BossStalPosition[] topStals;
    private BossStalPosition[] bottomStals;
    
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

    public float GetFreeTopStalXPos(int startIndex = 0, int endIndex = NumStals - 1, BossStalPosition.StalTypes type = BossStalPosition.StalTypes.Spike)
    {
        if (endIndex >= NumStals) endIndex = NumStals - 1;

        List<int> validIndexes = new List<int>();
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (!topStals[i].IsActive)
                validIndexes.Add(i);
        }
        if (validIndexes.Count == 0)
            return IsInvalid;

        int index = validIndexes[Random.Range(0, validIndexes.Count)];
        topStals[index].IsActive = true;
        return startPosition + index * spacing;
    }

    public void ClearTopStals()
    {
        for (int i = 0; i < NumStals; i++)
        {
            topStals[i].IsActive = false;
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
