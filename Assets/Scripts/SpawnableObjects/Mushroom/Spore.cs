using System.Collections;
using System.Collections.Generic;
using ClumsyBat;
using ClumsyBat.Players;
using UnityEngine;

public class Spore : MonoBehaviour {

    private ParticleSystem sporeEffect;
    private SpriteRenderer sporeSprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameStatics.Player.Clumsy.fog.Minimise();
        }
    }
}
