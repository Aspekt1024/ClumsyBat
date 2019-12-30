using System;
using UnityEngine;

namespace ClumsyBat
{
    public enum MainSounds
    {
        BreakCrystal = 1100,
        DoorClosing = 8000,
        DoorSlam = 8010,
        MenuButtonClick = 9010,
        MenuDropdownShow = 9100,
        MenuDropdownHide = 9110,
    }
    
    public class MainAudioControl : AudioControl<MainSounds>
    {
        [Serializable]
        public struct MainClip
        {
            public MainSounds sound;
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
