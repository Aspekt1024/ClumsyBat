﻿using UnityEngine;
using System.Collections;

public class TrainingGameHandler : GameHandler
{
    private LoadScreen _loadScreen;
    private GameMenuOverlay _gameMenu;
    private GameUI _gameHud;

    public int TrainingLevelNum = 1;
    private const float ResumeTimerDuration = 3f;
    private float _resumeTimerStart;
    private bool _bInBeam;
    private bool _bPaused;
    private float _chargePercent;
    private const float ChargeSpeed = 10f;  // Percent per second

    private enum TrainingGameState
    {
        Normal, DestroyedEverything
    }
    private TrainingGameState _state;

    private void Start()
    {
        Toolbox.Instance.Level = (int) Toolbox.LevelPrefix.TrainingPrefix + TrainingLevelNum;
        _loadScreen = FindObjectOfType<LoadScreen>();
        _gameHud = FindObjectOfType<GameUI>();
        _gameMenu = FindObjectOfType<GameMenuOverlay>();
        _gameMenu.Hide();
        ThePlayer.Fog.Disable();
        StartCoroutine("LoadSequence");
    }

    private void Update()
    {
        if (_bPaused) return;
        if (_bInBeam)
        {
            _chargePercent += Time.deltaTime * ChargeSpeed;
            if (_chargePercent > 100) { _chargePercent = 100f; }
            _gameHud.SetCustomText(Mathf.FloorToInt(_chargePercent) + "%");
        }

        if (!(_chargePercent >= 99.99f) || _state == TrainingGameState.DestroyedEverything) return;
        StartCoroutine("DestroyScene");
    }

    private IEnumerator LoadSequence()
    {
        yield return new WaitForSeconds(1f);
        StartGame();
    }

    public override void StartGame() // TODO this doesnt need to be implemented. nothing external calls it.
    {
        _gameHud.StartGame();
        _loadScreen.HideLoadScreen();
        PlayerController.EnterGamePlay();
    }

    public override void PauseGame(bool showMenu)
    {
        _bPaused = true;
        ThePlayer.PauseGame(true);
        _gameHud.GamePaused(true);
        if (showMenu)
        {
            _gameMenu.PauseGame();
        }
    }

    public override void ResumeGame(bool immediate = false)
    {
        if (!immediate)
        {
            _gameMenu.RaiseMenu();
            StartCoroutine("UpdateResumeTimer");
        }
        else
        {
            _gameMenu.Hide();
            ResumeGameplay();
        }
    }

    private IEnumerator UpdateResumeTimer()
    {
        float waitTime = _gameMenu.RaiseMenu();
        yield return new WaitForSeconds(waitTime);
        _resumeTimerStart = Time.time;

        while (ThePlayer.IsAlive() && _resumeTimerStart + ResumeTimerDuration - waitTime > Time.time)
        {
            float timeRemaining = _resumeTimerStart + ResumeTimerDuration - Time.time;
            _gameHud.SetResumeTimer(timeRemaining);
            yield return null;
        }
        ResumeGameplay();
    }

    private IEnumerator DestroyScene()
    {
        _state = TrainingGameState.DestroyedEverything;
        ThePlayer.ForceHypersonic();
        Destroy(GameObject.Find("WeirdBeamThing"));// TODO don't do this.
        yield return new WaitForSeconds(0.5f);
        Destroy(GameObject.Find("WeirdOrbThing"));// TODO don't do this.
        yield return new WaitForSeconds(1.2f);
        LevelComplete();
    }

    private void ResumeGameplay()
    {
        _bPaused = false;
        ThePlayer.PauseGame(false);
        _gameHud.HideResumeTimer();
        _gameHud.GamePaused(false);
        PlayerController.ResumeGameplay();
    }

    public override void UpdateGameSpeed(float speed)
    {
    }

    public override void LevelComplete()
    {
        _gameHud.LevelWon();
        StartCoroutine("TrainingComplete");
    }

    private IEnumerator TrainingComplete()
    {
        yield return new WaitForSeconds(2f);
        _gameMenu.WinGame();
    }

    public override void TriggerEntered(Collider2D other)
    {
        switch (other.name)
        {
            case "WeirdBeamThing":
                _bInBeam = true;
                break;
        }
    }

    public override void TriggerExited(Collider2D other)
    {
        switch (other.name)
        {
            case "WeirdBeamThing":
                _bInBeam = false;
                break;
        }
    }
}
