using UnityEngine;

public sealed class WebPool : SpawnPool<WebClass> {
    
    public WebPool()
    {
        ParentName = "Webs";
        ParentZ = Toolbox.Instance.ZLayers["Web"];
        NumObjectsInPool = 4;
        ResourcePath = "Obstacles/Web";
        ObjTag = "Web";
        SetupPool();
    }
    
    public struct WebType
    {
        public Spawnable.SpawnType SpawnTransform;
        public bool SpecialWeb;
    }

    public void SetupWebsInList(WebType[] webList, float xOffset)
    {
        foreach (WebType web in webList)
        {
            WebClass newWeb = GetNextObject();
            Spawnable.SpawnType spawnTf = web.SpawnTransform;
            spawnTf.Pos += new Vector2(xOffset, 0f);
            newWeb.Activate(spawnTf, web.SpecialWeb);
        }
    }
}
