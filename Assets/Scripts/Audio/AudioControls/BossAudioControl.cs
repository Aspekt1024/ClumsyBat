using System;
using UnityEngine;

namespace ClumsyBat
{
    public enum BossSounds
    {
        BossStomp = 3000,
        BossCrash = 3010,
        BossMovement = 3020,
        BossDamaged = 3030,
        BossDeath = 3100,
        ThrowProjectile = 3200,
        HitProjectile = 3210,
        BossCrystalActivate = 3500,
        BossCrystalDeactivate = 3501,
        BossCrystalShatter = 3502,
        BossCrystalStartup = 3503,
    }
    
    public class BossAudioControl : AudioControl<BossSounds>
    {
        [Serializable]
        public struct MainClip
        {
            public BossSounds sound;
            public AudioClip clip;
            public float cooldown;
        }
        
        [SerializeField] private MainClip[] map;

        protected override void Init()
        {
            foreach (var clip in map)
            {
                AddClip(new ClumsyClip(clip.sound.ToString(), clip.clip, clip.cooldown));
            }
        }
    }
}