using ClumsyBat.UI;
using ClumsyBat.Menu.MainMenu;
using UnityEngine;

namespace ClumsyBat
{
    /// <summary>
    /// Manages all of the UI elements of Clumsy Bat
    /// </summary>
    public class UIManager
    {
        public UIManager()
        {
            GameHud = Object.FindObjectOfType<GameHud>();
            DropdownMenu = Object.FindObjectOfType<DropdownMenu>();
            LoadingScreen = Object.FindObjectOfType<LoadingScreen>();
            NavButtons = Object.FindObjectOfType<NavButtonHandler>();
        }

        public GameHud GameHud { get; }
        public DropdownMenu DropdownMenu { get; }
        public LoadingScreen LoadingScreen { get; }
        public NavButtonHandler NavButtons { get; }
    }
}
