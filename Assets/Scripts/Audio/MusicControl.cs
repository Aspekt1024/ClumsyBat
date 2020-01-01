using System.Collections;
using Audio.MusicControls;
using UnityEngine;

namespace ClumsyBat.Music
{
    public class MusicControl : MonoBehaviour
    {
        #pragma warning disable 649
        [SerializeField] private AudioClip[] levelLoops;
        [SerializeField] private AudioClip bossEntranceIntro;
        [SerializeField] private AudioClip bossEntranceLoop;
        [SerializeField] private AudioClip bossLoop;
        #pragma warning restore 649
        
        private AudioSource audioSource;
        private LevelMusicControl levelMusic;
        private BossMusicControl bossMusic;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            
            levelMusic = new LevelMusicControl(this, levelLoops);
            bossMusic = new BossMusicControl(this, bossEntranceIntro, bossEntranceLoop, bossLoop);
        }

        private void Update()
        {
            levelMusic.Tick();
            bossMusic.Tick();
        }

        public bool IsPlaying => audioSource.isPlaying;
        
        public void Stop()
        {
            StartCoroutine(StopOverTime());
        }

        public void StartLevelMusic()
        {
            levelMusic.Play();
        }

        public void StartBossEntranceMusic()
        {
            bossMusic.PlayEntranceTheme();
        }

        public void StartBossMainMusic()
        {
            bossMusic.PlayBossTheme();
        }

        public void Play(AudioClip clip)
        {
            StartCoroutine(StopThenPlay(clip));
        }

        public void PlayLoop(AudioClip clip)
        {
            StartCoroutine(StopThenPlay(clip, loop: true));
        }
        
        private IEnumerator StopThenPlay(AudioClip clip, bool loop = false)
        {
            if (audioSource.isPlaying)
            {
                yield return StartCoroutine(StopOverTime());
            }
            PlayClip(clip, loop);
        }

        private void PlayClip(AudioClip clip, bool loop = false)
        {
            audioSource.clip = clip;
            audioSource.volume = 1f;
            audioSource.loop = loop;
            audioSource.Play();
        }
        
        private IEnumerator StopOverTime(float duration = 0.5f)
        {
            levelMusic.Stop();
            bossMusic.Stop();
            
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledTime;
                audioSource.volume = Mathf.Lerp(1f, 0f, timer / duration);
                yield return null;
            }
            
            audioSource.Stop();
            audioSource.volume = 1f;
        }
    }
}