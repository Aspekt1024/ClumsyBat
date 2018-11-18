using ClumsyBat.Players;
using System.Collections;
using UnityEngine;

namespace ClumsyBat.Menu
{
    /// <summary>
    /// Entry point into the start menu
    /// </summary>
    public class StartMenuManager : MonoBehaviour
    {
        private Player player;

        private void Start()
        {
            player = FindObjectOfType<Player>();
            StartMainMenuSequence();
        }

        public void StartMainMenuSequence()
        {
            StartCoroutine(Sequence());
        }

        private IEnumerator Sequence()
        {
            yield return new WaitForSeconds(.5f);

            KeyPointsHandler keyPoints = FindObjectOfType<KeyPointsHandler>();
            player.Model.transform.position = keyPoints.EntryPoint.transform.position;

            PlayerManager.Instance.PossessByAI();
            PlayerManager.Instance.AIController.MoveTo(keyPoints.EntryLandingPoint.transform);
            
            player.Physics.EnableCollisions();
        }
    }
}
