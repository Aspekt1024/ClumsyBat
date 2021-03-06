﻿using ClumsyBat;
using ClumsyBat.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypersonicCrystalBoss : CrystalBoss {

    protected override void CrystalBossWinSequence()
    {
        foreach (var crystal in crystals)
        {
            crystal.Explode();
        }

        GameStatics.Player.PossessByAI();
        GameStatics.Player.AIController.Hover();
        StartCoroutine(MovePlayerToCenterAndEndScene());
    }

    private IEnumerator MovePlayerToCenterAndEndScene()
    {
        float timer = 0f;
        const float duration = 7f;

        Player clumsy = GameStatics.Player.Clumsy;

        if (clumsy.model.position.x > GameStatics.Camera.CurrentCamera.transform.position.x)
        {
            clumsy.FaceLeft();
        }
        else
        {
            clumsy.FaceRight();
        }

        GameStatics.Audio.Boss.PlaySound(BossSounds.BossCrystalRumble);
        GameStatics.Camera.Shake(duration - 1f);
        while (timer < duration)
        {
            timer += Time.deltaTime;

            Vector3 pos = Vector2.Lerp(clumsy.model.position, GameStatics.Camera.CurrentCamera.transform.position, Time.deltaTime);
            pos.z = clumsy.model.position.z;
            clumsy.model.position = pos;

            yield return null;
        }

        var hypersonic = GameStatics.Data.Abilities.GetHypersonicStats();
        hypersonic.AbilityUnlocked = true;
        hypersonic.AbilityAvailable = true;
        hypersonic.AbilityLevel = 1;
        hypersonic.AbilityEvolution = 1;
        GameStatics.Data.Abilities.SaveHypersonicStats(hypersonic);

        GameStatics.Player.Clumsy.DoAction(ClumsyAbilityHandler.StaticActions.ForcedHypersonic);
        timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        string dialogue = "It worked! Any time you collect a gold moth, you will activate Hypersonic!";
        Toolbox.Tooltips.ShowDialogue(new TriggerEvent() { Dialogue = new List<string> { dialogue } }, DialogueComplete);
    }

    private void DialogueComplete()
    {
        GameStatics.LevelManager.GameHandler.LevelComplete();
    }
}
