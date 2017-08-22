using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class PlayGamesScript : MonoBehaviour {
   
	private void Start ()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
	}
	
    private void SignIn()
    {
        Social.localUser.Authenticate(success => { });
    }

    #region Achievements
    public void UnlockAchievement(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public void IncrementAchievement(string id, int stepsToIncrement)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }

    public void ShowAchievementsUI()
    {
        Social.ShowAchievementsUI();
    }
    #endregion Achievements

    #region Leaderboards
    public void AddScoreToLeaderboard(string leaderboardId, long score)
    {
        Social.ReportScore(score, leaderboardId, success => { });
    }

    public void ShowLeaderboardsUI()
    {
        Social.ShowLeaderboardUI();
    }
    #endregion Leaderboards
}
