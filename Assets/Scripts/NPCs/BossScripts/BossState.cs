using System.Collections.Generic;
using UnityEngine;

public class BossState : ScriptableObject {

    public string BossName = "<BossName>";
    public string StateName = "State";
    public List<BaseAction> Actions = new List<BaseAction>();

}
