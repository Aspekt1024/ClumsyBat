using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreCalculator {

    public static int SuggestScore(float distance, int numMoths)
    {
        float timeToComplete = (distance - 12.55f) / 5.5f;
        return (int)(distance * 50 + numMoths * 200 - timeToComplete * 100) - 500;
    }

    public static int GetScore(float distance, int numMoths, float timeTaken)
    {
        return (int)(distance * 50 + numMoths * 200 - timeTaken * 100);
    }
}
