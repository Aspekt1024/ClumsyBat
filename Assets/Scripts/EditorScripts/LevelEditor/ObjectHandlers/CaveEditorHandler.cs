using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEditorHandler : BaseObjectHandler {

    public CaveEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        parentObj = GetParentTransform("Caves");
        zLayer = LevelEditorConstants.CaveZ;
    }

    protected override void Update()
    {
        LineUpCaves();
    }

    private void LineUpCaves()
    {
        foreach (Transform cave in parentObj)
        {
            int index = Mathf.RoundToInt(cave.position.x / LevelEditorConstants.TileSizeX);
            PolygonCollider2D caveCollider = cave.GetComponent<PolygonCollider2D>();
            if (caveCollider != null)
            {
                caveCollider.enabled = false;
            }

            SpriteRenderer caveRenderer = cave.GetComponent<SpriteRenderer>();
            if (caveRenderer != null)
            {
                if (cave.name != caveRenderer.sprite.name)
                {
                    cave.name = caveRenderer.sprite.name;
                }
            }
            cave.transform.position = new Vector3(index * LevelEditorConstants.TileSizeX, 0f, zLayer);
        }
    }
}
