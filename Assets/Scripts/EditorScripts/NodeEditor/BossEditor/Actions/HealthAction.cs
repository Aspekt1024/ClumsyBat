using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAction : BaseAction {

    public int Health;

    public enum Ifaces
    {
        Input, Output
    }

    public override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().SetHealth(Health);
        CallNext((int)Ifaces.Output);
    }

}
