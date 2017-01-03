using UnityEngine;
using System.Collections;

public class WebPool {
    
    public WebPool()
    {
        SetupWebPool();
        WebZLayer = Toolbox.Instance.ZLayers["Web"];
    }

    private const int NumWebsInPool = 4;
    private const string WebResourcePath = "Obstacles/Web";

    public struct WebType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public bool bSpecialWeb;
    }
    
    WebClass[] Webs = null;
    int Index = 0;
    private float WebZLayer;

    private WebClass GetWebFromPool()
    {
        WebClass Web = Webs[Index];
        Index++;
        if (Index == Webs.Length)
        {
            Index = 0;
        }
        return Web;
    }

    public void SetupWebPool()
    {
        WebClass[] WebList = new WebClass[NumWebsInPool];
        for (int i = 0; i < NumWebsInPool; i++)
        {
            GameObject WebObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(WebResourcePath));
            WebClass Web = WebObj.GetComponent<WebClass>();
            WebObj.transform.position = Toolbox.Instance.HoldingArea;
            WebList[i] = Web;
        }
        Webs = WebList;
        Index = 0;
    }

    public void SetupWebsInList(WebType[] WebList, float XOffset)
    {
        foreach (WebType Web in WebList)
        {
            WebClass NewWeb = GetWebFromPool();
            NewWeb.transform.position = new Vector3(Web.Pos.x + XOffset, Web.Pos.y, WebZLayer);
            NewWeb.transform.localScale = Web.Scale;
            NewWeb.transform.localRotation = Web.Rotation;
            NewWeb.ActivateWeb(Web.bSpecialWeb);
        }
    }

    public void SetVelocity(float Speed)
    {
        foreach (WebClass Web in Webs)
        {
            Web.SetSpeed(Speed);
        }
    }

    public void SetPaused(bool PauseGame)
    {
        foreach (WebClass Web in Webs)
        {
            Web.SetPaused(PauseGame);
        }
    }
}
