using UnityEngine;
using System.Linq;

public class MultipleInAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        IsActive = false;
        foreach(var conn in connections.Where(conn => conn.Direction == ActionConnection.IODirection.Input))
        {
            conn.CallNext();
        }
    }
}
