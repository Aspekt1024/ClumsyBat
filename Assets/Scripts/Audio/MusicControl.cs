using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyBat.Music
{
    public enum MusicClip
    {
        BossEntranceIntro = 1000,
        BossEntranceLoop = 1100,
        BossLoop = 2000,
        LevelLoopA = 3000,
        LevelLoopB = 3100,
        LevelLoopC = 3200,
        LevelLoopD = 3300,
    }
    
    public class MusicControl : MonoBehaviour
    {
        [Serializable]
        public struct MusicClipDefinition
        {
            public MusicClip clipName;
            public AudioClip clip;
        }

        [SerializeField] private MusicClipDefinition[] map;
        
        private readonly Dictionary<MusicClip, MusicClipDefinition> musicDict = new Dictionary<MusicClip, MusicClipDefinition>();

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            
            IndexMusic();
            StartLevelMusic();
        }

        private void IndexMusic()
        {
            foreach (var c in map)
            {
                if (musicDict.ContainsKey(c.clipName)) continue;
                musicDict.Add(c.clipName, c);
            }
        }

        public void StartLevelMusic()
        {
            audioSource.clip = musicDict[MusicClip.LevelLoopA].clip;
            audioSource.loop = true;
            audioSource.Play();
        }

        public void StartBossEntranceMusic()
        {
            
        }

        public void StartBossMainMusic()
        {
            
        }
    }
}