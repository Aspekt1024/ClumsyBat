
using UnityEngine;

namespace ClumsyBat.Controllers
{
    /// <summary>
    /// Base class for controllers (e.g. Player or AI)
    /// </summary>
    public abstract class Controller : MonoBehaviour
    {
        public GameObject PossessOnStartup;

        public bool IsJumpHeld { get; set; }

        protected IControllable controlledObject;
        
        protected virtual void Awake()
        {
            if (PossessOnStartup != null)
            {
                var controllable = PossessOnStartup.GetComponent<IControllable>();
                if (controllable != null)
                {
                    Possess(controllable);
                }
            }
        }

        public void Possess(IControllable objectToControl)
        {
            if (objectToControl.Controller != null)
            {
                objectToControl.Controller.controlledObject = null;
            }

            controlledObject = objectToControl;
            controlledObject.Controller = this;
        }
    }
}
