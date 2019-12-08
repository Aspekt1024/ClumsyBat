using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyBat
{
    public enum MainSounds
    {
        Shield = 1000,
        BreakCrystal = 1100,
        ClumsyDeath = 2000,
        ClumsyFlap = 2010,
        ClumsyRush = 2100,
        HitCeiling = 2110,
        Hypersonic = 2120,
        Perch = 2200,
        BossStomp = 3000,
        BossCrash = 3010,
        BossMovement = 3020,
        BossDamaged = 3030,
        BossDeath = 3100,
        ThrowProjectile = 3200,
        HitProjectile = 3210,
        StalactiteCrack = 4000,
        StalactiteFall = 4010,
        StalactiteExplode = 4020,
        StalactiteCreak = 4030,
        SpiderDrop = 5000,
        SpiderHit = 5010,
        MushroomSpore = 6000,
        SporeEntered = 6010,
        MothAbsorbed = 7000,
        MothCollected = 7010,
        DoorClosing = 8000,
        DoorSlam = 8010,
        MenuButtonClick = 9010,
        MenuDropdownShow = 9100,
        MenuDropdownHide = 9110,
    }

    [Serializable]
    public struct ClumsyClip
    {
        public MainSounds sound;
        public AudioClip clip;
    }
    
    public class MainAudioControl : MonoBehaviour
    {
        [SerializeField] private ClumsyClip[] map;
        
        private readonly Dictionary<MainSounds, ClumsyClip> clipDict = new Dictionary<MainSounds, ClumsyClip>();

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            IndexAudio();
        }

        public void PlaySound(MainSounds sound)
        {
            if (!GameStatics.Data.Settings.SfxOn) return;
            if (!clipDict.ContainsKey(sound)) return;
            if (clipDict[sound].clip == null) return;
            
            audioSource.PlayOneShot(clipDict[sound].clip);
        }

        private void IndexAudio()
        {
            foreach (var clip in map)
            {
                if (clipDict.ContainsKey(clip.sound)) continue;
                clipDict.Add(clip.sound, clip);
            }
        }
    }
}
