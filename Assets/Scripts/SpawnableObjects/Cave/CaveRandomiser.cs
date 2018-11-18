using UnityEngine;
using ClumsyBat.Objects;

using static ClumsyBat.Objects.LevelContainer;

public class CaveRandomiser {

    private int NumTopCaveTypes;
    private int NumBottomCaveTypes;
    
    public CaveRandomiser ()
    {
        NumTopCaveTypes = CavePool.NumTopCaveTypes;
        NumBottomCaveTypes = CavePool.NumBottomCaveTypes;
    }

    public int GetRandomTopType()
    {
        return Random.Range(0, NumTopCaveTypes);
    }
    public int GetRandomBottomType()
    {
        return Random.Range(0, NumBottomCaveTypes);
    }

    public CaveType RandomiseObstacleList()
    {
        CaveType cave = new CaveType()
        {
            Stals = new StalPool.StalType[0],
            Shrooms = new ShroomPool.ShroomType[0],
            Moths = new MothPool.MothType[0],
            Spiders = new SpiderPool.SpiderType[0],
            Webs = new WebPool.WebType[0],
            Triggers = null,
            Npcs = null
    };

        const int NumStals = 8;
        int LowerIndex = 0;
        int UpperIndex = 3;

        bool bTop = (Random.Range(0, 2) == 1);

        int InARow = 1;

        int i = 0;
        i = Random.Range(LowerIndex, UpperIndex);
        while ((i < NumStals) && (i < NumStals))
        {
            if (!bTop && (i / 2 - Mathf.Floor(i / 2) < 0.4) && Random.Range(0f, 1f) <= 0.47f)
            {
                cave.Shrooms[0].SpawnTransform.Pos = new Vector2(0, 0);
                cave.Shrooms[0].SpawnTransform.Rotation = new Quaternion();
                cave.Shrooms[0].SpawnTransform.Scale = new Vector2(1f, 1f);
                cave.Shrooms[0].SpecialEnabled = false;
            }
            else
            {
                cave.Stals[0].SpawnTransform.Pos = new Vector2(0, 0);
                cave.Stals[0].SpawnTransform.Rotation = new Quaternion();
                cave.Stals[0].SpawnTransform.Scale = new Vector2(1f, 1f);
                cave.Stals[0].DropEnabled = true;
            }

            int t = Random.Range(0, 2);
            if ((t == 1 && bTop) || t == 0 && !bTop)
            {
                if (InARow == 2)
                {
                    InARow = 1;
                    LowerIndex = i + 3;
                    UpperIndex = i + 5;
                }
                else
                {
                    LowerIndex = i + 1;
                    UpperIndex = i + 2;
                    InARow++;
                }
            }
            else
            {
                bTop = !bTop;
                LowerIndex = i + 3;
                UpperIndex = i + 5;
                InARow = 1;
            }
            i = Random.Range(LowerIndex, UpperIndex);
        }
        
        return cave;
    }
}
