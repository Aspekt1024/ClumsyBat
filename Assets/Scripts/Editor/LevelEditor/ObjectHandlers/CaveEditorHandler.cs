using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEditorHandler : BaseObjectHandler {
    
    public CaveEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        parentObj = GetParentTransform("Caves");
        zLayer = LevelEditorConstants.CaveZ;
    }

    public override void StoreObjects(ref LevelContainer levelRef)
    {
        level = levelRef;
        //if (parentObj == null) { parentObj = GameObject.Find("Caves").GetComponent<Transform>(); }
        StoreCaveIndexes();
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

    public int GetNumSections()
    {
        int numSections = 2;
        foreach (Transform cave in parentObj)
        {
            if (cave.name.Contains("Cave"))
            {
                int index = Mathf.RoundToInt(cave.position.x / LevelEditorConstants.TileSizeX);
                if (index + 1 > numSections)
                {
                    numSections = index + 1;
                }
            }
        }
        return numSections;
    }

    private void StoreCaveIndexes()
    {
        foreach (Transform cave in parentObj)
        {
            int index = Mathf.RoundToInt(cave.position.x / LevelEditorConstants.TileSizeX);
            if (cave.name == "CaveEntrance")
            {
                level.Caves[index].BottomIndex = Toolbox.CaveStartIndex;
                level.Caves[index].TopIndex = Toolbox.CaveStartIndex;
            }
            else if (cave.name == "CaveExit")
            {
                level.Caves[index].BottomIndex = Toolbox.CaveEndIndex;
                level.Caves[index].TopIndex = Toolbox.CaveEndIndex;
            }
            else if (cave.name == "CaveGnomeEnd")
            {
                level.Caves[index].BottomIndex = Toolbox.CaveGnomeEndIndex;
                level.Caves[index].TopIndex = Toolbox.CaveGnomeEndIndex;
            }
            else if (cave.name.Contains("Top"))
            {
                int caveType = int.Parse(cave.name.Substring(cave.name.Length - 1, 1)) - 1;
                level.Caves[index].TopIndex = caveType;
                if (cave.name.Contains("Exit"))
                {
                    level.Caves[index].bTopSecretPath = true;
                }
            }
            else if (cave.name.Contains("Bottom"))
            {
                int caveType = int.Parse(cave.name.Substring(cave.name.Length - 1, 1)) - 1;
                level.Caves[index].BottomIndex = caveType;
                if (cave.name.Contains("Exit"))
                {
                    level.Caves[index].bBottomSecretPath = true;
                }
            }
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            LevelContainer.CaveType cave = level.Caves[i];
            GameObject caveTop;
            if (cave.TopIndex == Toolbox.CaveStartIndex)
            {
                caveTop = (GameObject)Object.Instantiate(Resources.Load("Caves/CaveEntrance"), parentObj);
                caveTop.name = "CaveEntrance";
            }
            else if (cave.TopIndex == Toolbox.CaveEndIndex)
            {
                caveTop = (GameObject)Object.Instantiate(Resources.Load("Caves/CaveExit"), parentObj);
                caveTop.name = "CaveExit";
            }
            else if (cave.TopIndex == Toolbox.CaveGnomeEndIndex)
            {
                caveTop = (GameObject)Object.Instantiate(Resources.Load("Caves/CaveGnomeEnd"), parentObj);
                caveTop.name = "CaveGnomeEnd";
            }
            else
            {
                string caveBottomName = "CaveBottom" + (cave.bBottomSecretPath ? "Exit" : "") + (cave.BottomIndex + 1).ToString();
                string caveTopName = "CaveTop" + (cave.bTopSecretPath ? "Exit" : "") + (cave.TopIndex + 1).ToString();
                var caveBottom = (GameObject)Object.Instantiate(Resources.Load("Caves/" + caveBottomName), parentObj);
                caveTop = (GameObject)Object.Instantiate(Resources.Load("Caves/" + caveTopName), parentObj);
                caveBottom.name = caveBottomName;
                caveTop.name = caveTopName;
                caveBottom.transform.position = new Vector3(LevelEditorConstants.TileSizeX * i, 0f, zLayer);
            }
            caveTop.transform.position = new Vector3(LevelEditorConstants.TileSizeX * i, 0f, zLayer);
        }
    }
}
