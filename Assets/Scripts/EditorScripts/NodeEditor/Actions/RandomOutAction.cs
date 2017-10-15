using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomOutAction : BaseAction
{
    public enum Ifaces
    {
        Input,
        PrimaryOut
    }

    public enum RandomOutTypes
    {
        Uniform, Weighted
    }
    public RandomOutTypes RandomType;
    public List<float> OutputWeights = new List<float>();

    private bool alternateSelected;

    protected override void ActivateBehaviour()
    {
        IsActive = false;

        ActionConnection[] outputs = connections.Where(conn => conn.Direction == ActionConnection.IODirection.Output).ToArray();

        int randOutputIndex = 0;
        if (RandomType == RandomOutTypes.Uniform)
        {
            randOutputIndex = Random.Range(0, outputs.Length);
        }
        else if (RandomType == RandomOutTypes.Weighted)
        {
            float rand = Random.Range(0, OutputWeights.Sum());

            float weightCounter = 0;
            for (int i = 0; i < OutputWeights.Count; i++)
            {
                weightCounter += OutputWeights[i];
                if (rand <= weightCounter)
                {
                    randOutputIndex = i;
                    break;
                }
            }
        }

        outputs[randOutputIndex].CallNext();
    }
}
