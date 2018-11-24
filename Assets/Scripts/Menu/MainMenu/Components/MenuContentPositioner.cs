using UnityEngine;

namespace ClumsyBat.Menu.MainMenu
{
    public class MenuContentPositioner : MonoBehaviour
    {
        private void FixedUpdate()
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }
}
