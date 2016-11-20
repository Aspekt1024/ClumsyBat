using UnityEngine;
using System;

class EventListener
{
    private PlayerController Player;
    public EventListener(PlayerController PlayerClass)
    {
        Player = PlayerClass;
        Player.PlayerDeath += new PlayerDeathHandler(PlayerDied);
    }

    private void PlayerDied(object sender, EventArgs e)
    {
        Debug.Log("Player has died...");
        
    }

    public void Detach()
    {
        Player.PlayerDeath -= new PlayerDeathHandler(PlayerDied);
        Player = null;
    }
}
