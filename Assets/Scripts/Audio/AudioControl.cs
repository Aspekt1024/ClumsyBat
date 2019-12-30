using System.Collections.Generic;
using UnityEngine;

namespace ClumsyBat
{
    public struct ClumsyClip
    {
        public readonly string Name;
        public readonly AudioClip Clip;
        public readonly float Cooldown;

        public ClumsyClip(string name, AudioClip clip, float cooldown)
        {
            Name = name;
            Clip = clip;
            Cooldown = cooldown;
        }
    }
    
    public abstract class AudioControl<T> : MonoBehaviour
    {   
        private readonly Dictionary<string, ClumsyClip> clipDict = new Dictionary<string, ClumsyClip>();
        
        private readonly Dictionary<T, float> clipLastPlayedDict = new Dictionary<T, float>();

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            Init();
        }

        public void PlaySound(T sound)
        {
            if (!GameStatics.Data.Settings.SfxOn) return;
            if (!clipDict.ContainsKey(sound.ToString())) return;
            if (clipDict[sound.ToString()].Clip == null) return;

            var c = clipDict[sound.ToString()];
            if (c.Cooldown > 0)
            {
                if (clipLastPlayedDict.ContainsKey(sound))
                {
                    if (Time.unscaledTime < clipLastPlayedDict[sound] + c.Cooldown) return;
                    clipLastPlayedDict[sound] = Time.unscaledTime;
                }
                else
                {
                    clipLastPlayedDict.Add(sound, Time.unscaledTime);
                }
            }
            
            audioSource.PlayOneShot(c.Clip);
        }

        protected abstract void Init();

        protected void AddClip(ClumsyClip clip)
        {
            if (clipDict.ContainsKey(clip.Name)) return;
            clipDict.Add(clip.Name, clip);
        }
    }
}