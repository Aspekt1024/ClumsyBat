﻿using ClumsyBat.Objects;
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
                caveCollider.enabled = true;
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
            
            if ((cave.name.Contains("Top") || cave.name.Contains("Bottom")) && cave.name.Contains("Exit"))
            {
                SecretPath path = cave.GetComponentInChildren<SecretPath>();
                if (path != null)
                {
                    if (path.HasBlock)
                    {
                        path.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    else
                    {
                        path.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
                    }
                }
            }
        }

    }

    public int GetNumSections()
    {
        if (parentObj == null) return 0;

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
                    SecretPath path = cave.GetComponentInChildren<SecretPath>();
                    if (path != null)
                    {
                        level.Caves[index].bSecretPathRequiresMoth = path.RequiresBlueMoth;
                        level.Caves[index].bSecretPathHasBlock = path.HasBlock;
                    }
                    else
                    {
                        level.Caves[index].bSecretPathHasBlock = false;
                    }
                }
            }
            else if (cave.name.Contains("Bottom"))
            {
                int caveType = int.Parse(cave.name.Substring(cave.name.Length - 1, 1)) - 1;
                level.Caves[index].BottomIndex = caveType;
                if (cave.name.Contains("Exit"))
                {
                    level.Caves[index].bBottomSecretPath = true;
                    SecretPath path = cave.GetComponentInChildren<SecretPath>();
                    if (path != null)
                    {
                        level.Caves[index].bSecretPathRequiresMoth = path.RequiresBlueMoth;
                        level.Caves[index].bSecretPathHasBlock = path.HasBlock;
                    }
                    else
                    {
                        level.Caves[index].bSecretPathHasBlock = false;
                    }
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

                if (cave.bBottomSecretPath || cave.bTopSecretPath)
                {
                    SecretPath path = caveTop.GetComponentInChildren<SecretPath>();
                    if (path == null) path = caveBottom.GetComponentInChildren<SecretPath>();
                    path.RequiresBlueMoth = cave.bSecretPathRequiresMoth;
                    path.HasBlock = cave.bSecretPathHasBlock;
                }
            }
            caveTop.transform.position = new Vector3(LevelEditorConstants.TileSizeX * i, 0f, zLayer);
        }
    }

    public GameObject CreateNewTopCave(float xPos, int index = 1)
    {
        string caveTopName = "CaveTop" + index.ToString();
        GameObject caveTop = (GameObject)Object.Instantiate(Resources.Load("Caves/" + caveTopName), parentObj);
        caveTop.name = caveTopName;
        caveTop.transform.position = new Vector3(xPos, 0f, zLayer);
        return caveTop;
    }

    public GameObject CreateNewBottomCave(float xPos, int index = 1)
    {
        string caveBottomName = "CaveBottom" + index.ToString();
        GameObject caveBottom = (GameObject)Object.Instantiate(Resources.Load("Caves/" + caveBottomName), parentObj);
        caveBottom.name = caveBottomName;
        caveBottom.transform.position = new Vector3(xPos, 0f, zLayer);
        return caveBottom;
    }

    public GameObject[] GetCavesAtIndex(float xPos)
    {
        GameObject[] caves = new GameObject[2];
        int caveNum = 0;
        foreach(Transform cave in parentObj)
        {
            if (Mathf.Abs(cave.position.x - xPos) < 0.01f)
            {
                caves[caveNum] = cave.gameObject;
                caveNum++;
                if (caveNum == 2)
                    break;
            }
        }
        return caves;
    }
}
