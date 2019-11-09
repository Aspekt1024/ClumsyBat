using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClumsyBat
{
    [RequireComponent(typeof(MainAudioControl))]
    public class AudioManager : Singleton<PlayerManager>
    {
        public MainAudioControl Main { get; private set; }

        private void Awake()
        {
            Main = gameObject.GetComponent<MainAudioControl>();
        }
    }
}
