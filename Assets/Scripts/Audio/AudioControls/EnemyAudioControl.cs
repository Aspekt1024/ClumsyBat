using System;
using UnityEngine;

namespace ClumsyBat
{
    public enum EnemySounds
    {
        StalactiteCrack = 4000,
        StalactiteFall = 4010,
        StalactiteExplode = 4020,
        StalactiteCreak = 4030,
        StalactiteForm = 4040,
        SpiderDrop = 5000,
        SpiderHit = 5010,
        MushroomSpore = 6000,
        SporeEntered = 6010,
    }
    
    public class EnemyAudioControl : AudioControl<EnemySounds>
    {
        [Serializable]
        public struct MainClip
        {
            public EnemySounds sound;
            public AudioClip clip;
            public float cooldown;
        }
        
#pragma warning disable 649 
        [SerializeField] private MainClip[] map;
#pragma warning restore 649 
        
        protected override void Init()
        {
            foreach (var clip in map)
            {
                AddClip(new ClumsyClip(clip.sound.ToString(), clip.clip, clip.cooldown));
            }
        }
    }
}