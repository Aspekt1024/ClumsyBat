using UnityEngine;

public class MothPool {

    public MothPool()
    {
        SetupMothPool();
    }

    private const int NumMothsInPool = 15;
    private const string MothResourcePath = "Collectibles/Moth";

    public struct MothType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public bool Gold;
    }

    Moth[] Moths = null;
    int Index = 0;
    private const float MothZLayer = 1f;

    private Moth GetMothFromPool()
    {
        Moth Moth = Moths[Index];
        Index++;
        if (Index == Moths.Length)
        {
            Index = 0;
        }
        return Moth;
    }

    public void SetupMothPool()
    {
        Moth[] MothList = new Moth[NumMothsInPool];
        for (int i = 0; i < NumMothsInPool; i++)
        {
            GameObject MothObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(MothResourcePath));
            Moth Moth = MothObj.GetComponent<Moth>();
            MothObj.transform.position = Toolbox.Instance.HoldingArea;
            MothList[i] = Moth;
        }
        Moths = MothList;
        Index = 0;
    }

    public void SetupMothsInList(MothType[] MothList, float XOffset)
    {
        foreach (MothType Moth in MothList)
        {
            Moth NewMoth = GetMothFromPool();
            NewMoth.transform.position = new Vector3(Moth.Pos.x + XOffset, Moth.Pos.y, MothZLayer);
            NewMoth.transform.localScale = Moth.Scale;
            NewMoth.transform.localRotation = Moth.Rotation;
            NewMoth.ActivateMoth(Moth.Gold);
        }
    }

    public void SetVelocity(float Speed)
    {
        foreach (Moth Moth in Moths)
        {
            Moth.SetSpeed(Speed);
        }
    }

    public void SetPaused(bool PauseGame)
    {
        foreach (Moth Moth in Moths)
        {
            Moth.SetPaused(PauseGame);
        }
    }
}
