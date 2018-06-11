using UnityEngine;

namespace ClumsyBat.Managers
{
    public abstract class ManagerBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("Attempted to access " + typeof(T).ToString() + " but it does not exist in the scene");
                }
                return _instance;
            }
        }

        private static T _instance;

        protected static bool isBeingDestroyed;

        protected virtual void Awake()
        {
            if (isBeingDestroyed || _instance != null)
            {
                Destroy(this);
                return;
            }

            _instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            isBeingDestroyed = true;
        }
    }
}
