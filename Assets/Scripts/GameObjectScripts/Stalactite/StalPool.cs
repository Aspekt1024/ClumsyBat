using UnityEngine;

public class StalPool {

    public StalPool()
    {
        SetupStalactitePool();
    }

    private const int NumStalsInPool = 15;
    private const string StalResourcePath = "Obstacles/Stalactite";

    public struct StalType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public bool DropEnabled;
    }

    Stalactite[] Stals = null;
    int Index = 0;
    private const float StalZLayer = 4f;

    private Stalactite GetStalactiteFromPool()
    {
        Stalactite Stal = Stals[Index];
        Index++;
        if (Index == Stals.Length)
        {
            Index = 0;
        }
        return Stal;
    }

    public void SetupStalactitePool()
    {
        Stalactite[] StalList = new Stalactite[NumStalsInPool];
        for (int i = 0; i < NumStalsInPool; i++)
        {
            GameObject StalObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(StalResourcePath));
            Stalactite Stalactite = StalObj.GetComponent<Stalactite>();
            StalObj.transform.position = Toolbox.Instance.HoldingArea;
            StalList[i] = Stalactite;
        }
        Stals = StalList;
        Index = 0;
    }

    public void SetupStalactitesInList(StalType[] StalList, float XOffset)
    {
        foreach (StalType Stal in StalList)
        {
            Stalactite NewStal = GetStalactiteFromPool();
            NewStal.transform.position = new Vector3(Stal.Pos.x + XOffset, Stal.Pos.y, StalZLayer);
            NewStal.transform.localScale = Stal.Scale;
            NewStal.transform.localRotation = Stal.Rotation;
            NewStal.ActivateStal(Stal.DropEnabled);
        }
    }

    public void SetVelocity(float Speed)
    {
        foreach (Stalactite Stal in Stals)
        {
            Stal.SetSpeed(Speed);
        }
    }

    public void SetPaused(bool PauseGame)
    {
        foreach (Stalactite Stal in Stals)
        {
            Stal.SetPaused(PauseGame);
        }
    }

    public void CheckAndDestroy()
    {
        foreach (Stalactite Stal in Stals)
        {
            Stal.DestroyStalactiteIfInScreen();
        }
    }
}
