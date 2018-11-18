using UnityEngine;
using UnityEditor;
using ClumsyBat.Objects;

public class SpiderEditorHandler : BaseObjectHandler
{
    public SpiderEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        resourcePath = "Obstacles/Spider";
        parentObj = GetParentTransform("Spiders");
        zLayer = LevelEditorConstants.SpiderZ;
    }
    
    protected override void Update()
    {
        foreach (Transform t in parentObj)
        {
            t.GetComponent<HingeJoint2D>().enabled = false;
            SpiderClass s = t.GetComponent<SpiderClass>();
            if (!s.SwingingSpider) continue;

            Vector2 anchorPos = new Vector2(s.WebAnchorPoint.x + s.transform.position.x, s.WebAnchorPoint.y + s.transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(anchorPos.x, 0f), Vector2.up, 20f, 1 << LayerMask.NameToLayer("Caves"));
            if (hit.collider != null)
            {
                anchorPos = new Vector2(anchorPos.x, hit.point.y);
            }

            const float anchorRadius = 0.3f;
            Rect r = new Rect(anchorPos - Vector2.one * anchorRadius/2, Vector2.one * anchorRadius);

            Handles.DrawSolidRectangleWithOutline(r, Color.white, Color.white);
            Handles.DrawLine(anchorPos, t.position);
            Handles.color = new Color(1f, 0.5f, 0.7f, 0.1f);
            Handles.DrawSolidArc(anchorPos, Vector3.back, t.position, 200, Vector2.Distance(anchorPos, t.position));

            s.WebAnchorPoint = new Vector2(anchorPos.x - s.transform.position.x, anchorPos.y - s.transform.position.y);
        }
    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        level = levelObj;
        var SpiderCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Spiders = new SpiderPool.SpiderType[SpiderCounts[i]];
        }

        int[] SpiderNum = new int[level.Caves.Length];
        foreach (Transform Spider in parentObj)
        {
            int index = GetObjectCaveIndex(Spider);

            SpiderPool.SpiderType newSpider = level.Caves[index].Spiders[SpiderNum[index]];
            newSpider.SpawnTransform = ProduceSpawnTf(Spider, index);
            newSpider.SpiderSwings = Spider.GetComponent<SpiderClass>().SwingingSpider;
            newSpider.AnchorPoint = Spider.GetComponent<SpiderClass>().WebAnchorPoint;
            level.Caves[index].Spiders[SpiderNum[index]] = newSpider;
            SpiderNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            if (level.Caves[i].Spiders == null || level.Caves[i].Spiders.Length == 0) continue;
            foreach (SpiderPool.SpiderType Spider in level.Caves[i].Spiders)
            {
                GameObject newSpider = (GameObject)Object.Instantiate(Resources.Load("Obstacles/Spider"), parentObj);
                Spawnable.SpawnType spawnTf = Spider.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newSpider.GetComponent<SpiderClass>().SetTransform(newSpider.transform, spawnTf);
                newSpider.GetComponent<SpiderClass>().SwingingSpider = Spider.SpiderSwings;
                newSpider.GetComponent<SpiderClass>().WebAnchorPoint = Spider.AnchorPoint;
            }
        }
    }
}
