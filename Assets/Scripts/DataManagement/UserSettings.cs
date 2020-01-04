using UnityEngine;

namespace ClumsyBat.DataManagement
{
    public class UserSettings
    {
        public void ToggleMusic()
        {
            MusicOn = !MusicOn;
            if (!GameStatics.GameManager.IsInLevel) return;

            if (MusicOn)
            {
                if (GameStatics.LevelManager.IsBossLevel)
                {
                    // TODO check if crystal level first, then
                    // TODO determine if boss fight has already started
                    GameStatics.Audio.Music.StartBossEntranceMusic();
                }
                else
                {
                    GameStatics.Audio.Music.StartLevelMusic();
                }
            }
            else
            {
                GameStatics.Audio.Music.Stop();
            }
        }
        public void ToggleSFX() { SfxOn = !SfxOn; }

        public void ToggleTooltips() { TooltipsOn = !TooltipsOn; }

        public bool MusicOn { get; private set; }
        public bool SfxOn { get; private set; }
        public bool TooltipsOn { get; private set; }

        public UserSettings()
        {
            MusicOn = true;
            SfxOn = true;
            TooltipsOn = true;
        }

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
