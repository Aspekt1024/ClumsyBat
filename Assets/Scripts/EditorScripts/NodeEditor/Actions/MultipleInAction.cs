using UnityEngine;
using System.Linq;

public class MultipleInAction : BaseAction {

    protected override void ActivateBehaviour()
    {
        IsActive = false;
        CallNext(0);
    }
}
