using ClumsyBat.Objects;
using UnityEngine;

public class WebEditorHandler : BaseObjectHandler {

    public WebEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        resourcePath = "Obstacles/Web";
        parentObj = GetParentTransform("Webs");
        zLayer = LevelEditorConstants.WebZ;
    }

    protected override void Update()
    {
        
    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        level = levelObj;
        var webCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Webs = new WebPool.WebType[webCounts[i]];
        }

        int[] webNum = new int[level.Caves.Length];
        foreach (Transform web in parentObj)
        {
            int index = GetObjectCaveIndex(web);

            WebPool.WebType newWeb = level.Caves[index].Webs[webNum[index]];
            newWeb.SpawnTransform = ProduceSpawnTf(web, index);
            newWeb.SpecialWeb = web.GetComponent<WebClass>().SpecialWeb;
            level.Caves[index].Webs[webNum[index]] = newWeb;
            webNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Webs == null || level.Caves[i].Webs.Length == 0) continue;
            foreach (WebPool.WebType web in level.Caves[i].Webs)
            {
                GameObject newWeb = (GameObject)Object.Instantiate(Resources.Load("Obstacles/Web"), parentObj);
                Spawnable.SpawnType spawnTf = web.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newWeb.GetComponent<WebClass>().SetTransform(newWeb.transform, spawnTf);
                newWeb.GetComponent<WebClass>().SpecialWeb = web.SpecialWeb;
            }
        }
    }
}
