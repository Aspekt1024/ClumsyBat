using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StalEvents {

    public delegate void StalactiteEvent(int index, SpawnStalAction.StalSpawnDirection direction);
    public static StalactiteEvent OnDestroy;

    public static void Destroy(int index, SpawnStalAction.StalSpawnDirection direction)
    {
        if (OnDestroy != null)
        {
            OnDestroy(index, direction);
        }
    }
}
