using UnityEngine;
using System.Collections;

public class SpiderPool {
    
    public SpiderPool()
    {
        SetupSpiderPool();
        SpiderZLayer = Toolbox.Instance.ZLayers["Spider"];
    }

    private const int NumSpidersInPool = 5;
    private const string SpiderResourcePath = "Obstacles/Spider";

    public struct SpiderType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public bool bSwinging;
    }


    SpiderClass[] Spiders = null;
    int Index = 0;
    private float SpiderZLayer;

    private SpiderClass GetSpiderFromPool()
    {
        SpiderClass Spider = Spiders[Index];
        Index++;
        if (Index == Spiders.Length)
        {
            Index = 0;
        }
        return Spider;
    }

    public void SetupSpiderPool()
    {
        SpiderClass[] SpiderList = new SpiderClass[NumSpidersInPool];
        for (int i = 0; i < NumSpidersInPool; i++)
        {
            GameObject SpiderObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(SpiderResourcePath));
            SpiderClass Spider = SpiderObj.GetComponent<SpiderClass>();
            SpiderObj.transform.position = Toolbox.Instance.HoldingArea;
            SpiderList[i] = Spider;
        }
        Spiders = SpiderList;
        Index = 0;
    }

    public void SetupSpidersInList(SpiderType[] SpiderList, float XOffset)
    {
        foreach (SpiderType Spider in SpiderList)
        {
            SpiderClass NewSpider = GetSpiderFromPool();
            NewSpider.transform.position = new Vector3(Spider.Pos.x + XOffset, Spider.Pos.y, SpiderZLayer);
            NewSpider.transform.localScale = Spider.Scale;
            NewSpider.transform.localRotation = Spider.Rotation;
            NewSpider.ActivateSpider(Spider.bSwinging);
        }
    }

    public void SetVelocity(float Speed)
    {
        foreach (SpiderClass Spider in Spiders)
        {
            Spider.SetSpeed(Speed);
        }
    }

    public void SetPaused(bool PauseGame)
    {
        foreach (SpiderClass Spider in Spiders)
        {
            Spider.SetPaused(PauseGame);
        }
    }
}
