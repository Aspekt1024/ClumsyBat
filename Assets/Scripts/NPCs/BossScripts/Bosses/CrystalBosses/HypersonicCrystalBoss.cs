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
        Toolbox.Player.EnableHover();
        StartCoroutine(MovePlayerToCenterAndEndScene());
    }

    private IEnumerator MovePlayerToCenterAndEndScene()
    {
        float timer = 0f;
        const float duration = 7f;

        if (Toolbox.Player.transform.position.x > Toolbox.PlayerCam.transform.position.x)
            Toolbox.Player.FaceLeft();
        else
            Toolbox.Player.FaceRight();

        CameraEventListener.CameraShake(duration - 1f);
        while (timer < duration)
        {
            Vector2 pos = Vector2.Lerp(Toolbox.Player.transform.position, Toolbox.PlayerCam.transform.position, Time.deltaTime);
            Toolbox.Player.transform.position = new Vector3(pos.x, pos.y, Toolbox.Player.transform.position.z);
            timer += Time.deltaTime;
            yield return null;
        }

        var hypersonic = GameData.Instance.Data.AbilityData.GetHypersonicStats();
        hypersonic.AbilityUnlocked = true;
        hypersonic.AbilityAvailable = true;
        hypersonic.AbilityLevel = 1;
        hypersonic.AbilityEvolution = 1;
        GameData.Instance.Data.AbilityData.SaveHypersonicStats(hypersonic);

        Toolbox.Player.ForceHypersonic();
        timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Toolbox.Tooltips.ShowDialogue("It worked! Any time you collect a gold moth, you will activate Hypersonic!", 2f, true);
        while (Toolbox.Tooltips.IsPausedForDialogue)
        {
            yield return null;
        }

        Toolbox.Player.GetGameHandler().LevelComplete();
    }
}
