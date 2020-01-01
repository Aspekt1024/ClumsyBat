using UnityEngine;

namespace ClumsyBat.Music
{
    public class BossMusicControl
    {
        private readonly AudioClip entranceIntro;
        private readonly AudioClip entranceLoop;
        private readonly AudioClip bossLoop;

        private readonly MusicControl musicControl;

        private enum States
        {
            None, PlayingEntranceIntro, PlayingEntranceLoop, PlayingBoss
        }

        private States state;
        
        public BossMusicControl(MusicControl musicControl, AudioClip entranceIntro, AudioClip entranceLoop, AudioClip bossLoop)
        {
            this.musicControl = musicControl;
            
            this.entranceIntro = entranceIntro;
            this.entranceLoop = entranceLoop;
            this.bossLoop = bossLoop;
        }

        public void PlayEntranceTheme()
        {
            if (state == States.PlayingEntranceIntro || state == States.PlayingEntranceLoop) return;
            state = States.PlayingEntranceIntro;
            musicControl.Play(entranceIntro);
        }

        public void PlayBossTheme()
        {
            if (state == States.PlayingBoss) return;
            state = States.PlayingBoss;
            musicControl.PlayLoop(bossLoop);
        }

        public void Tick()
        {
            if (state == States.PlayingEntranceIntro && Time.timeSinceLevelLoad > 1f && !musicControl.IsPlaying)
            {
                state = States.PlayingEntranceLoop;
                musicControl.PlayLoop(entranceLoop);
            }
        }

        public void Stop()
        {
            state = States.None;
        }
    }
}