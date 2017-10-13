using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomOutAction : BaseAction
{
    public int NumOutputs;

    public override void ActivateBehaviour()
    {
        IsActive = false;

        ActionConnection[] outputs = connections.Where(conn => conn.Direction == ActionConnection.IODirection.Output).ToArray();
        int randOutputIndex = Random.Range(0, outputs.Length);
        outputs[randOutputIndex].CallNext();
    }
}
