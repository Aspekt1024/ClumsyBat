using System.Collections.Generic;
using UnityEngine;

public class BossDataContainer : ScriptableObject {

    public string BossName;

    public List<BaseAction> Actions = new List<BaseAction>();
}
