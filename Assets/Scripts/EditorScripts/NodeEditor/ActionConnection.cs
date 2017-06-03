
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ActionConnection {
    
    public enum IODirection
    {
        Input, Output
    }
    
    public int ID;
    public IODirection Direction;
    public int OtherActionID;
    public int OtherConnID;

    [XmlIgnore] public BaseAction Action;
    [XmlIgnore] public ActionConnection ConnectedInterface;

    private bool called;

    public bool IsConnected()
    {
        return ConnectedInterface != null;
    }

    public void CallNext()
    {
        if (IsConnected())
        {
            ConnectedInterface.called = true;
            ConnectedInterface.Action.Activate();
        }
    }

    public bool WasCalled()
    {
        return called;
    }

    public void UseCall()
    {
        called = false;
    }
}
