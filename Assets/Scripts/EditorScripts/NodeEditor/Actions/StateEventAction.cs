using UnityEngine;

public class StateEventAction : BaseAction {
    
    public int StateEventID;

    public enum Ifaces
    {
        Input
    }

    public override void ActivateBehaviour()
    {
        // TODO this is messy. Need a better way of matching StateAction to the current State, then we only need to iterate through connections
        foreach(var action in behaviourSet.ParentMachine.Actions)
        {
            if (!action.IsType<StateAction>()) continue;

            if (((StateAction)action).StateName == ((State)behaviourSet).Name)
            {
                foreach(var conn in action.connections)
                {
                    if (conn.ID == StateEventID)
                    {
                        // TODO if stops state
                        conn.CallNext();
                        IsActive = false;
                        action.IsActive = false;
                    }
                }
            }
        }
    }

}
