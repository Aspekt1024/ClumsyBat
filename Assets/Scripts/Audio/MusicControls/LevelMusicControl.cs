using ClumsyBat.Music;
using UnityEngine;

namespace Audio.MusicControls
{
    public class LevelMusicControl
    {
        private readonly MusicControl musicControl;
        private readonly AudioClip[] clips;

        private int currentClip;

        private enum States
        {
            None, Playing
        }

        private States state;

        public LevelMusicControl(MusicControl musicControl, AudioClip[] clips)
        {
            this.musicControl = musicControl;
            this.clips = clips;
        }

        public void Tick()
        {
            if (state == States.None || musicControl.IsPlaying) return;
            PlayRandom();
        }
        
        public void Play()
        {
            if (state == States.Playing) return;
            state = States.Playing;
            PlayRandom();
        }

        public void Stop()
        {
            state = States.None;
        }
        
        private void PlayRandom()
        {
            currentClip = GetNextClipIndex(currentClip);
            musicControl.Play(clips[currentClip]);   
        }

        private int GetNextClipIndex(int currentIndex)
        {
            var newClipIndex = Random.Range(0, clips.Length - 1);
            if (newClipIndex >= currentIndex)
            {
                newClipIndex++;
            }
            if (newClipIndex == clips.Length)
            {
                newClipIndex = 0;
            }

            return newClipIndex;
        }
    }
}