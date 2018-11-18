using UnityEngine;

namespace ClumsyBat.Players
{
    public class PlayerSensor : MonoBehaviour
    {
        private Player player;

        private void Start()
        {
            player = GetComponent<Player>();
        }
    }
}
