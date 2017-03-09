using System.Collections.Generic;
using UnityEngine;

public class BossDataContainer : ScriptableObject {

    public string BossName;
    public BossDataContainer RootContainer;

    public List<BaseAction> Actions = new List<BaseAction>();

    public bool IsType<T>() where T : BossDataContainer
    {
        return GetType().Equals(typeof(T));
    }
}
