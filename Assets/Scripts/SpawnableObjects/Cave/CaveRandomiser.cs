using UnityEngine;
using System.Collections;

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

    public LevelObjectHandler.CaveListType RandomiseObstacleList()
    {
        StalPool.StalType[] StalList = new StalPool.StalType[0];
        ShroomPool.ShroomType[] ShroomList = new ShroomPool.ShroomType[0];
        MothPool.MothType[] MothList = new MothPool.MothType[0];
        SpiderPool.SpiderType[] SpiderList = new SpiderPool.SpiderType[0];
        WebPool.WebType[] WebList = new WebPool.WebType[0];

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
                ShroomList[0].SpawnTransform.Pos = new Vector2(0, 0);
                ShroomList[0].SpawnTransform.Rotation = new Quaternion();
                ShroomList[0].SpawnTransform.Scale = new Vector2(1f, 1f);
                ShroomList[0].SpecialEnabled = false;
            }
            else
            {
                StalList[0].SpawnTranform.Pos = new Vector2(0, 0);
                StalList[0].SpawnTranform.Rotation = new Quaternion();
                StalList[0].SpawnTranform.Scale = new Vector2(1f, 1f);
                StalList[0].DropEnabled = true;
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

        LevelObjectHandler.CaveListType ObjectList;
        ObjectList.StalList = StalList;
        ObjectList.MushroomList = ShroomList;
        ObjectList.MothList = MothList;
        ObjectList.SpiderList = SpiderList;
        ObjectList.WebList = WebList;
        ObjectList.TriggerList = null;
        ObjectList.NpcList = null;

        return ObjectList;
    }
}
