using UnityEngine;

namespace ClumsyBat.InteractiveLevelObjects
{
    public class CaveExit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform == GameStatics.Player.Clumsy.model)
            {
                GameStatics.LevelManager.EndOfLevelReached();
            }
        }
    }
}
