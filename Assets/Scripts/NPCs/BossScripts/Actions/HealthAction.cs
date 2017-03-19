using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAction : BaseAction {

    public int Health;

    public override void ActivateBehaviour()
    {
        boss.GetComponent<Boss>().SetHealth(Health);
        CallNext();
    }

}
