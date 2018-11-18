using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ClumsyBat.UI.DropdownMenuComponents
{
    public class DropdownOptionsMenu : MenuScreenBase {

        private Animator _toggleMusic;
        private Animator _toggleSfx;
        private Animator _toggleTooltips;

        private CanvasGroup _optionsMainPanel;
        private CanvasGroup _optionsYesNoPanel;
        private CanvasGroup _optionsOkPanel;

        private Text _optionText;
        private Text _optionConfirmText;

        private enum YesNo
        {
            ResetAllData,
            ResetTooltips,
        }
        private YesNo _confirmOption;

        private void Start()
        {
            GetMenuObjects();
            InitialiseOptionsView();
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            InitialiseOptionsView();
            SetToggleStates();
        }

        private void GetMenuObjects()
        {
            _toggleMusic = GameObject.Find("ToggleMusic").GetComponent<Animator>();
            _toggleSfx = GameObject.Find("ToggleSFX").GetComponent<Animator>();
            _toggleTooltips = GameObject.Find("ToggleTooltips").GetComponent<Animator>();

            _optionsMainPanel = GameObject.Find("OptionsMainPanel").GetComponent<CanvasGroup>();
            _optionsYesNoPanel = GameObject.Find("OptionsYesNoPanel").GetComponent<CanvasGroup>();
            _optionsOkPanel = GameObject.Find("OptionsOKPanel").GetComponent<CanvasGroup>();

            _optionText = GameObject.Find("OptionText").GetComponent<Text>();
            _optionConfirmText = GameObject.Find("OptionConfirmText").GetComponent<Text>();
        }

        public void ResetTooltipsPressed()
        {
            _confirmOption = YesNo.ResetTooltips;
            _optionText.text = "Are you sure you want to reset tooltips?";
            SetPanelVisible(_optionsMainPanel, false);
            SetPanelVisible(_optionsYesNoPanel, true);
        }

        public void ResetDataPressed()
        {
            _confirmOption = YesNo.ResetAllData;
            _optionText.text = "Are you sure you want to erase your story progress? This is not reversible!";
            SetPanelVisible(_optionsMainPanel, false);
            SetPanelVisible(_optionsYesNoPanel, true);
        }

        public void ClearConfirmPressed()
        {
            switch (_confirmOption)
            {
                case YesNo.ResetAllData:
                    _optionConfirmText.text = "Story has been reset!";
                    GameStatics.Data.ResetStoryData();
                    break;
                case YesNo.ResetTooltips:
                    _optionConfirmText.text = "Tooltips have been reset!";
                    //Stats.CompletionData.ResetTooltips();     // TODO redo these options
                    break;
            }
            SetPanelVisible(_optionsYesNoPanel, false);
            SetPanelVisible(_optionsOkPanel, true);
        }

        public void UnconfirmPressed()
        {
            SetPanelVisible(_optionsYesNoPanel, false);
            SetPanelVisible(_optionsMainPanel, true);
        }

        public void OkPressed()
        {
            switch (_confirmOption)
            {
                case YesNo.ResetAllData:
                    FindObjectOfType<LoadingScreen>().ShowLoadScreen();
                    SceneManager.LoadScene("Play");
                    break;
                case YesNo.ResetTooltips:
                    SetPanelVisible(_optionsOkPanel, false);
                    SetPanelVisible(_optionsMainPanel, true);
                    break;
            }
        }

        public void ToggleMusicPressed()
        {
            GameStatics.Data.Settings.ToggleMusic();
            _toggleMusic.Play(GameStatics.Data.Settings.MusicOn ? "MusicON" : "MusicOFF");
            EventListener.MusicToggle();
        }

        public void ToggleSfxPressed()
        {
            GameStatics.Data.Settings.ToggleSFX();
            _toggleSfx.Play(GameStatics.Data.Settings.SfxOn ? "SFXON" : "SFXOFF");
            EventListener.SfxToggle();
        }

        public void ToggleTooltipsPressed()
        {
            GameStatics.Data.Settings.ToggleTooltips();
            _toggleTooltips.Play(GameStatics.Data.Settings.TooltipsOn ? "TooltipsON" : "TooltipsOFF");
        }

        private void InitialiseOptionsView()
        {
            SetPanelVisible(_optionsMainPanel, true);
            SetPanelVisible(_optionsYesNoPanel, false);
            SetPanelVisible(_optionsOkPanel, false);
        }

        private void SetToggleStates()
        {
            string musicState = (GameStatics.Data.Settings.MusicOn ? "MusicON" : "MusicOFF");
            string sfxState = (GameStatics.Data.Settings.SfxOn ? "SFXON" : "SFXOFF");
            string tooltipsState = (GameStatics.Data.Settings.TooltipsOn ? "TooltipsON" : "TooltipsOFF");

            _toggleMusic.Play(musicState);
            _toggleSfx.Play(sfxState);
            _toggleTooltips.Play(tooltipsState);
        }

        private void SetPanelVisible(CanvasGroup panel, bool visible)
        {
            panel.alpha = (visible ? 1f : 0f);
            panel.blocksRaycasts = visible;
            panel.interactable = visible;
        }
    }
}

