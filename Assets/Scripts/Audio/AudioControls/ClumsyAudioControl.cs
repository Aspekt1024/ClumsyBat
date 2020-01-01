using System;
using ClumsyBat;
using UnityEngine;

namespace ClumsyBat
{
    public enum ClumsySounds
    {
        Shield = 1000,
        ClumsyDeath = 2000,
        ClumsyFlap = 2010,
        ClumsyRush = 2100,
        HitCeiling = 2110,
        Hypersonic = 2120,
        Perch = 2200,
        MothAbsorbed = 7000,
        MothCollected = 7010,
    }
    
    public class ClumsyAudioControl : AudioControl<ClumsySounds>
    {
        [Serializable]
        public struct MainClip
        {
            public ClumsySounds sound;
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