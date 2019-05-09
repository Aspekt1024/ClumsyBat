using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreCalculator {

    private const int distanceMultiplier = 50;
    private const int mothMultiplier = 500;
    private const int timeMultiplier = 200;
    private const int scoreSuggestionOffset = -400;

    public static int SuggestScore(float distance, int numMoths)
    {
        float timeToComplete = (distance - 12.55f) / 5.5f;
        return (int)(distance * distanceMultiplier + numMoths * mothMultiplier - timeToComplete * timeMultiplier) + scoreSuggestionOffset;
    }

    public static int GetScore(float distance, int numMoths, float timeTaken)
    {
        int score = (int)(distance * distanceMultiplier + numMoths * mothMultiplier - timeTaken * timeMultiplier);
        return score > 0 ? score : 0;
    }
    
}
