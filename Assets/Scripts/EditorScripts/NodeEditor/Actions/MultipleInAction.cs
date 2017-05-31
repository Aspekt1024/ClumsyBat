using UnityEngine;
using System.Linq;

public class MultipleInAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        IsActive = false;
        CallNext(0);
    }
}
