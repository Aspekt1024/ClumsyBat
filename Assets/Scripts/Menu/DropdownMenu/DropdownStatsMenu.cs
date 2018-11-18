using UnityEngine;

namespace ClumsyBat.UI.DropdownMenuComponents
{
    public class DropdownStatsMenu : MenuScreenBase
    {
        private StatsUI StatsScript = null;
        private CanvasGroup StatsMask = null;

        protected override void Awake()
        {
            base.Awake();
            StatsScript = gameObject.AddComponent<StatsUI>();
        }
        
        public void CreateStats()
        {
            StatsScript.CreateStatText();
        }
    }
}