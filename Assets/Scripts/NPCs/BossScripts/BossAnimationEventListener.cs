using UnityEngine;

namespace ClumsyBat
{
    public class BossAnimationEventListener : MonoBehaviour
    {
        public void Step()
        {
            GameStatics.Audio.Boss.PlaySound(BossSounds.BossMovement);
        }
    }
}