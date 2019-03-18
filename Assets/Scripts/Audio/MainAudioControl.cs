using System.Collections.Generic;
using UnityEngine;

namespace ClumsyBat
{
    public enum MainSounds
    {
        Shield,
        BreakCrystal,
        ClumsyRush,
    }

    public class MainAudioControl : MonoBehaviour
    {
        public AudioClip Shield;
        public AudioClip BreakCrystal;
        public AudioClip ClumsyRush;

        private AudioSource audioSource;
        private readonly Dictionary<MainSounds, AudioClip> soundDict = new Dictionary<MainSounds, AudioClip>();

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            PopulateSoundDict();
        }

        public void PlaySound(MainSounds sound)
        {
            if (GameStatics.Data.Settings.SfxOn)
            {
                audioSource.PlayOneShot(soundDict[sound]);
            }
        }

        private void PopulateSoundDict()
        {
            soundDict.Add(MainSounds.Shield, Shield);
            soundDict.Add(MainSounds.BreakCrystal, BreakCrystal);
            soundDict.Add(MainSounds.ClumsyRush, ClumsyRush);
        }
    }
}
