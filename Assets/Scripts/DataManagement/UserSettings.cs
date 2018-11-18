using UnityEngine;

namespace ClumsyBat.DataManagement
{
    public class UserSettings
    {
        public void EnableMusic() { MusicOn = true; }
        public void DisableMusic() { MusicOn = false; }
        public void ToggleMusic() { MusicOn = !MusicOn; }

        public void EnableSFX() { SfxOn = true; }
        public void DisableSFX() { SfxOn = false; }
        public void ToggleSFX() { SfxOn = !SfxOn; }

        public void EnableTooltips() { TooltipsOn = true; }
        public void DisableTooltips() { TooltipsOn = false; }
        public void ToggleTooltips() { TooltipsOn = !TooltipsOn; }

        public bool MusicOn { get; private set; }
        public bool SfxOn { get; private set; }
        public bool TooltipsOn { get; private set; }

        public void LoadUserSettings()
        {
            if (PlayerPrefs.GetInt("SettingsStored") != 1)
            {
                MusicOn = true;
                SfxOn = true;
                TooltipsOn = true;
                SaveUserSettings();
            }
            else
            {
                MusicOn = PlayerPrefs.GetInt("MusicON") == 1;
                SfxOn = PlayerPrefs.GetInt("SFXON") == 1;
                TooltipsOn = PlayerPrefs.GetInt("TooltipsON") == 1;
            }
        }

        public void SaveUserSettings()
        {
            PlayerPrefs.SetInt("MusicON", MusicOn ? 1 : 0);
            PlayerPrefs.SetInt("SFXON", SfxOn ? 1 : 0);
            PlayerPrefs.SetInt("TooltipsON", TooltipsOn ? 1 : 0);
            PlayerPrefs.SetInt("SettingsStored", 1);
        }
    }

}
