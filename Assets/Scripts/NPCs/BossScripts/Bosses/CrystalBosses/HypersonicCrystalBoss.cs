using ClumsyBat;
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

        if (GameStatics.Player.Clumsy.Model.position.x > GameStatics.Camera.CurrentCamera.transform.position.x)
            GameStatics.Player.Clumsy.FaceLeft();
        else
            GameStatics.Player.Clumsy.FaceRight();

        CameraEventListener.CameraShake(duration - 1f);
        while (timer < duration)
        {
            Vector2 pos = Vector2.Lerp(GameStatics.Player.Clumsy.Model.position, GameStatics.Camera.CurrentCamera.transform.position, Time.deltaTime);
            GameStatics.Player.Clumsy.Model.position = new Vector3(pos.x, pos.y, GameStatics.Player.Clumsy.Model.position.z);
            timer += Time.deltaTime;
            yield return null;
        }

        var hypersonic = GameStatics.Data.Abilities.GetHypersonicStats();
        hypersonic.AbilityUnlocked = true;
        hypersonic.AbilityAvailable = true;
        hypersonic.AbilityLevel = 1;
        hypersonic.AbilityEvolution = 1;
        GameStatics.Data.Abilities.SaveHypersonicStats(hypersonic);

        GameStatics.Player.Clumsy.DoAction(ClumsyBat.Players.ClumsyAbilityHandler.StaticActions.ForcedHypersonic);
        timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        string dialogue = "It worked! Any time you collect a gold moth, you will activate Hypersonic!";
        Toolbox.Tooltips.ShowDialogue(new TriggerEvent() { Dialogue = new List<string> { dialogue } });

        GameStatics.LevelManager.GameHandler.LevelComplete();
    }
}
