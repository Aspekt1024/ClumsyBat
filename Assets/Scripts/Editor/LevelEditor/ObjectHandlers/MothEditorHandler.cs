using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothEditorHandler : BaseObjectHandler {

    public MothEditorHandler(LevelEditorObjectHandler objHandler) : base(objHandler)
    {
        resourcePath = "Collectibles/Moth";
        parentObj = GetParentTransform("Moths");
        zLayer = LevelEditorConstants.MothZ;
    }

    protected override void Update()
    {
        AlignMoths();
    }

    private void AlignMoths()
    {
        foreach (Transform moth in parentObj)
        {
            Moth mothScript = moth.GetComponent<Moth>();
            foreach (Transform mothTf in moth.transform)
            {
                if (mothTf.name != "MothTrigger") continue;
                if (mothTf.position != moth.transform.position)
                {
                    mothTf.position = moth.transform.position;
                }
            }

            SpriteRenderer mothRenderer = moth.GetComponentInChildren<SpriteRenderer>();
            switch (mothScript.Colour)
            {
                case Moth.MothColour.Blue:
                    mothRenderer.color = new Color(0f, 0f, 1f);
                    break;
                case Moth.MothColour.Green:
                    mothRenderer.color = new Color(0f, 1f, 0f);
                    break;
                case Moth.MothColour.Gold:
                    mothRenderer.color = new Color(1f, 1f, 0f);
                    break;
            }
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
