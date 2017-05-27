using UnityEngine;
using System.Linq;

public class MultipleOutAction : BaseAction {
    
    public override void ActivateBehaviour()
    {
        IsActive = false;
        foreach(var conn in connections.Where(conn => conn.Direction == ActionConnection.IODirection.Output))
        {
            conn.CallNext();
        }
    }
}
