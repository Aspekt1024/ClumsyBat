using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayGamesScript : MonoBehaviour {
    
	private void Start ()
    {
        AuthenticateGooglePlay();
    }

    private void AuthenticateGooglePlay()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated()) return;

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = false;

        Social.localUser.Authenticate(success => {
            if (!success)
                GooglePlayAuthFail();
        });
    }

    private void GooglePlayAuthFail()
    {
        // TODO connection failure toast
    }

    #region Achievements
    public static void UnlockAchievement(string id)
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
        Social.ReportProgress(id, 100, success => { });
    }

    public static void IncrementAchievement(string id, int stepsToIncrement)
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }

    public static void ShowAchievementsUI()
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
        Social.ShowAchievementsUI();
    }
    #endregion Achievements

    #region Leaderboards
    
    public static void AddHighScore(long score)
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
        Social.ReportScore(score, CBBLGId.leaderboard_high_scores, success => { });
    }

    public static void ShowLeaderboardsUI()
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
        Social.ShowLeaderboardUI();
    }
    #endregion Leaderboards
}
