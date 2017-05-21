
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


    public bool IsConnected()
    {
        return ConnectedInterface != null;
    }

    public void CallNext()
    {
        if (IsConnected())
        {
            Debug.Log(Action + " calls " + ConnectedInterface.Action);
            ConnectedInterface.Action.Activate();
        }
    }
}
