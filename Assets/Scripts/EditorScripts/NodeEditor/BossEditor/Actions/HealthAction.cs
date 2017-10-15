using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAction : BaseAction {

    public int Health;

    public enum Ifaces
    {
        Input, Output
    }

    protected override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().SetHealth(Health);
        CallNext((int)Ifaces.Output);
    }

}
