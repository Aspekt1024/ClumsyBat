using UnityEngine;

namespace ClumsyBat.Objects
{
    public sealed class SpiderPool : SpawnPool<SpiderClass>
    {

        public SpiderPool()
        {
            ParentName = "Spiders";
            ParentZ = Toolbox.Instance.ZLayers["Spider"];
            ResourcePath = "Obstacles/Spider";
            ObjTag = "Spider";
        }

        public struct SpiderType
        {
            public Spawnable.SpawnType SpawnTransform;
            public bool SpiderSwings;
            public Vector2 AnchorPoint;
        }

        public void SetupSpidersInList(SpiderType[] spiderList, float xOffset)
        {
            foreach (SpiderType spider in spiderList)
            {
                SpiderClass newSpider = GetObjectFromPool();
                Spawnable.SpawnType spawnTf = spider.SpawnTransform;
                spawnTf.Pos += new Vector2(xOffset, 0f);
                newSpider.Spawn(spawnTf, spider.SpiderSwings, spider.AnchorPoint);
            }
        }

        public override void DisableObjects()
        {
            foreach (var obj in ObjPool)
            {
                obj.ClearWebs();
                obj.Deactivate();
            }
            
            if (ParentObject == null) return;
            var webObject = ParentObject.Find("Webs");
            if (webObject != null)
            {
                var objects = webObject.GetComponentsInChildren<Transform>();
                foreach (var obj in objects)
                {
                    if (obj.name == "Webs") continue;
                    GameObject.Destroy(obj.gameObject);
                }
            }
        }
    }
}
