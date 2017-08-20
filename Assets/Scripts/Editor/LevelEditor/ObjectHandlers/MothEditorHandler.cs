using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MothEditorHandler : BaseObjectHandler {

    public MothEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        resourcePath = "Collectibles/Moth";
        parentObj = GetParentTransform("Moths");
        zLayer = LevelEditorConstants.MothZ;
    }

    public int GetNumMoths()
    {
        return parentObj.childCount;
    }

    protected override void Update()
    {
        foreach (Transform moth in parentObj)
        {
            GlueMothObjects(moth);
            SetMothColour(moth);
        }
    }

    private void GlueMothObjects(Transform moth)
    {
        foreach (Transform mothTf in moth.transform)
        {
            if (mothTf.name != "MothTrigger") continue;
            if (mothTf.position != moth.transform.position)
            {
                Selection.activeObject = moth.gameObject;
                mothTf.position = moth.transform.position;
            }
        }
    }

    private void SetMothColour(Transform moth)
    {
        Moth mothScript = moth.GetComponent<Moth>();
        SpriteRenderer mothRenderer = moth.GetComponentInChildren<SpriteRenderer>();

        if (mothRenderer == null)
        {
            Object.DestroyImmediate(moth.gameObject);
            return;
        }

        switch (mothScript.Colour)
        {
            case Moth.MothColour.Blue:
                mothRenderer.color = new Color(0.4f, 0.4f, 1f);
                break;
            case Moth.MothColour.Green:
                mothRenderer.color = new Color(0f, 1f, 0f);
                break;
            case Moth.MothColour.Gold:
                mothRenderer.color = new Color(1f, 1f, 0f);
                break;
        }
    }

    public override void StoreObjects(ref LevelContainer levelObj)
    {
        level = levelObj;
        var mothCounts = GetObjCounts(parentObj);
        for (int i = 0; i < level.Caves.Length; i++)
        {
            level.Caves[i].Moths = new MothPool.MothType[mothCounts[i]];
        }

        int[] mothNum = new int[level.Caves.Length];
        foreach (Transform moth in parentObj)
        {
            int index = GetObjectCaveIndex(moth);

            MothPool.MothType newMoth = level.Caves[index].Moths[mothNum[index]];
            newMoth.SpawnTransform = ProduceSpawnTf(moth, index);
            newMoth.Colour = moth.GetComponent<Moth>().Colour;
            newMoth.PathType = moth.GetComponent<Moth>().PathType;
            level.Caves[index].Moths[mothNum[index]] = newMoth;
            mothNum[index]++;
        }
    }

    protected override void SetObjects(LevelContainer level)
    {
        for (int i = 0; i < level.Caves.Length; i++)
        {
            foreach (MothPool.MothType moth in level.Caves[i].Moths)
            {
                GameObject newMoth = (GameObject)Object.Instantiate(Resources.Load(resourcePath), parentObj);
                Spawnable.SpawnType spawnTf = moth.SpawnTransform;
                spawnTf.Pos += new Vector2(i * LevelEditorConstants.TileSizeX, 0f);
                newMoth.GetComponent<Moth>().SetTransform(newMoth.transform, spawnTf);
                newMoth.GetComponent<Moth>().Colour = moth.Colour;
                newMoth.GetComponent<Moth>().PathType = moth.PathType;
            }
        }
    }
}
