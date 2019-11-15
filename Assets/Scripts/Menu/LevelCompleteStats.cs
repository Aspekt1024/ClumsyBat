using ClumsyBat;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteStats : MonoBehaviour {

    public Text MothText;
    public Text TimeText;
    public Text ScoreText;
    public Text BestText;

    public RectTransform MothLabel;
    public RectTransform TimeLabel;
    public RectTransform ScoreLabel;
    public RectTransform BestLabel;

    private RectTransform mothRt;
    private RectTransform timeRt;
    private RectTransform scoreRt;
    private RectTransform bestRt;
    
    public void ShowBossLevelStats()
    {
        TimeText.text = ConvertFloatToTimeString(GameStatics.Data.GameState.TimeTaken);
        UIObjectAnimator.Instance.PopInObject(TimeLabel);
        UIObjectAnimator.Instance.PopInObject(timeRt);
    }

    public void ShowNormalLevelStats()
    {
        MothText.text = GameStatics.Data.GameState.MothsEaten + "/" + GameStatics.LevelManager.NumMoths;
        TimeText.text = ConvertFloatToTimeString(GameStatics.Data.GameState.TimeTaken);

        int score = ScoreCalculator.GetScore(GameStatics.Data.GameState.Distance, GameStatics.Data.GameState.MothsEaten, GameStatics.Data.GameState.TimeTaken);
        UpdateBestLevelScore(score);

        ScoreText.text = score.ToString();
        BestText.text = GameStatics.Data.LevelDataHandler.GetBestScore(GameStatics.LevelManager.Level).ToString();

        UIObjectAnimator.Instance.PopInObject(MothLabel);
        UIObjectAnimator.Instance.PopInObject(mothRt);
        UIObjectAnimator.Instance.PopInObject(TimeLabel);
        UIObjectAnimator.Instance.PopInObject(timeRt);
        UIObjectAnimator.Instance.PopInObject(ScoreLabel);
        UIObjectAnimator.Instance.PopInObject(scoreRt);
        UIObjectAnimator.Instance.PopInObject(BestLabel);
        UIObjectAnimator.Instance.PopInObject(bestRt);
    }

    public void PopOutAllObjects()
    {
        if (MothLabel.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(MothLabel);
        if (mothRt.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(mothRt);
        if (TimeLabel.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(TimeLabel);
        if (timeRt.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(timeRt);
        if (ScoreLabel.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(ScoreLabel);
        if (scoreRt.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(scoreRt);
        if (BestLabel.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(BestLabel);
        if (bestRt.gameObject.activeSelf) UIObjectAnimator.Instance.PopOutObject(bestRt);
    }

    private void Start()
    {
        mothRt = MothText.GetComponent<RectTransform>();
        timeRt = TimeText.GetComponent<RectTransform>();
        scoreRt = ScoreText.GetComponent<RectTransform>();
        bestRt = BestText.GetComponent<RectTransform>();
        HideAll();
    }

    private void HideAll()
    {
        MothLabel.gameObject.SetActive(false);
        MothText.gameObject.SetActive(false);
        TimeLabel.gameObject.SetActive(true);
        TimeText.gameObject.SetActive(true);
        ScoreLabel.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(false);
        BestLabel.gameObject.SetActive(false);
        BestText.gameObject.SetActive(false);
    }

    private string ConvertFloatToTimeString(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        if (minutes > 0)
        {
            seconds -= 60 * minutes;
            return $"{minutes}:{seconds:n2}";
        }
        return $"{seconds:n2}s";
    }

    private void UpdateBestLevelScore(int score)
    {
        if (score > GameStatics.Data.LevelDataHandler.GetBestScore(GameStatics.LevelManager.Level))
        {
            GameStatics.Data.LevelDataHandler.SetBestScore(GameStatics.LevelManager.Level, score);
        }
    }
}
