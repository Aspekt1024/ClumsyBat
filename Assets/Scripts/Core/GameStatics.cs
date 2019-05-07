using UnityEngine;

namespace ClumsyBat
{
    /// <summary>
    /// Provides convenient access to the key modules of Clumsy Bat
    /// </summary>
    public class GameStatics
    {
        private GameManager _gameManager;
        private DataManager _dataManager;

        private UIManager _uiManager = new UIManager();
        private LevelManager _levelManager = new LevelManager();
        private CameraManager _cameraManager = new CameraManager();
        private PlayerManager _playerManager;
        private ObjectManager _objectManager = new ObjectManager();
        private AudioManager _audioManager;

        private static GameStatics _instance;
        
        private static GameStatics Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("GameStatics has not been initialised properly. It should be created from the GameManager. But don't worry! This will probably be fine if you're testing only!");
                    _instance = new GameStatics();
                }
                return _instance;
            }
        }

        private static T GetManager<T>(ref T field) where T : new()
        {
            if (field == null)
            {
                field = new T();
            }
            return field;
        }

        public GameStatics()
        {
            _instance = this;
            _gameManager = Object.FindObjectOfType<GameManager>();
            _playerManager = Object.FindObjectOfType<PlayerManager>();
            _audioManager = Object.FindObjectOfType<AudioManager>();
            
            CreateDataObjects();
        }

        public static bool StaticsInitiated { get { return _instance != null; } }

        public static GameManager GameManager { get { return Instance._gameManager; } }
        public static DataManager Data { get { return Instance._dataManager; } }
        public static UIManager UI { get { return Instance._uiManager; } }
        public static LevelManager LevelManager { get { return Instance._levelManager; } }
        public static CameraManager Camera { get { return GetManager(ref Instance._cameraManager); } }
        public static PlayerManager Player { get { return Instance._playerManager; } }
        public static ObjectManager Objects { get { return Instance._objectManager; } }
        public static AudioManager Audio { get { return Instance._audioManager; } }

        /// <summary>
        /// TODO This is a temporary function!!! Once LevelDataControl and AbilityControl are not monobehaviours, they can be removed
        /// </summary>
        private void CreateDataObjects()
        {
            GameObject dataObject = new GameObject("GameData");
            
            AbilityControl abilityData = dataObject.AddComponent<AbilityControl>();
            _dataManager = new DataManager(abilityData);
        }
    }
}
