using ClumsyBat.Music;
using UnityEngine;

namespace ClumsyBat
{
    [RequireComponent(typeof(MainAudioControl))]
    public class AudioManager : Singleton<PlayerManager>
    {
        public MainAudioControl Main { get; private set; }
        public ClumsyAudioControl Clumsy { get; private set; }
        public BossAudioControl Boss { get; private set; }
        public MusicControl Music { get; private set; }

        private void Awake()
        {
            Main = gameObject.GetComponent<MainAudioControl>();
            Clumsy = gameObject.GetComponent<ClumsyAudioControl>();
            Boss = gameObject.GetComponent<BossAudioControl>();
            Music = gameObject.GetComponent<MusicControl>();
        }
    }
}
