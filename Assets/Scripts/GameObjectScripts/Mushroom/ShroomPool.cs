using UnityEngine;

public class ShroomPool {

    public ShroomPool()
    {
        SetupMushroomPool();
    }

    private const int NumShroomsInPool = 5;
    private const string ShroomResourcePath = "Obstacles/Mushroom";
    private const float ShroomZLayer = 5f;
    
    public struct ShroomType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public bool SpecialEnabled;
    }

    Mushroom[] Shrooms = null;
    int Index = 0;
    
    private Mushroom GetMushroomFromPool()
    {
        Mushroom Shroom = Shrooms[Index];
        Index++;
        if (Index == Shrooms.Length)
        {
            Index = 0;
        }
        return Shroom;
    }
    
    public void SetupMushroomPool()
    {
        Mushroom[] ShroomList = new Mushroom[NumShroomsInPool];
        for (int i = 0; i < NumShroomsInPool; i++)
        {
            GameObject ShroomObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(ShroomResourcePath));
            Mushroom Mushroom = ShroomObj.GetComponent<Mushroom>();
            ShroomObj.transform.position = Toolbox.Instance.HoldingArea;
            ShroomList[i] = Mushroom;
        }
        Shrooms = ShroomList;
        Index = 0;
    }

    public void SetupMushroomsInList(ShroomType[] ShroomList, float XOffset)
    {
        foreach (ShroomType Shroom in ShroomList)
        {
            Mushroom Mushroom = GetMushroomFromPool();
            Mushroom.transform.position = new Vector3(Shroom.Pos.x + XOffset, Shroom.Pos.y, ShroomZLayer);
            Mushroom.transform.localScale = Shroom.Scale;
            Mushroom.transform.localRotation = Shroom.Rotation;
            Mushroom.ActivateMushroom();
        }
    }

    public void SetVelocity(float Speed)
    {
        foreach (Mushroom Shroom in Shrooms)
        {
            Shroom.SetSpeed(Speed);
        }
    }

    public void CheckAndDestroy()
    {
        foreach (Mushroom Shroom in Shrooms)
        {
            Shroom.DeactivateMushroom();
        }
    }
}
