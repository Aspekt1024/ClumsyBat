using UnityEngine;

public class ParabolicProjectileNode : BaseNode {

    private ParabolicProjectile parProjectile;
    private Player player;
    
    public override void SetupNode()
    {
        AddInput();
        AddOutput();
    }

    private void SetInterfacePositions()
    {
        SetInput(WindowRect.height / 2);
        SetOutput(WindowRect.height / 2);
    }

    public override void DrawWindow()
    {
        WindowTitle = "Parabolic Projectile";
        WindowRect.width = 150;
        WindowRect.height = 60;


        SetInterfacePositions();
        DrawInterfaces();
    }
    
    public override void GameSetup(BossBehaviour behaviour, GameObject bossReference)
    {
        base.GameSetup(behaviour, bossReference);
        parProjectile = (ParabolicProjectile)bossBehaviour.GetAbility<ParabolicProjectile>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public override void Activate()
    {
        parProjectile.ActivateProjectile(player.transform.position);
        CallNext();
    }
}
