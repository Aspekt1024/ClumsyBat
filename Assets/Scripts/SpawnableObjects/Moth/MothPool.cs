using UnityEngine;

public class MothPool
{
    private Transform _mothParentObject;

    public MothPool()
    {
        SetupMothPool();
    }

    private const int NumMothsInPool = 8;
    private const string MothResourcePath = "Collectibles/Moth";

    public struct MothType
    {
        public Vector2 Pos;
        public Vector2 Scale;
        public Quaternion Rotation;
        public Moth.MothColour Colour;
    }

    private Moth[] _moths;
    private int _index;
    private const float MothZLayer = 1f;

    private Moth GetMothFromPool()
    {
        Moth moth = _moths[_index];
        _index++;
        if (_index == _moths.Length)
        {
            _index = 0;
        }
        return moth;
    }

    public void SetupMothPool()
    {
        _mothParentObject = new GameObject("Moths").transform;
        Moth[] mothList = new Moth[NumMothsInPool];
        for (int i = 0; i < NumMothsInPool; i++)
        {
            GameObject mothObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(MothResourcePath), _mothParentObject);
            Moth moth = mothObj.GetComponent<Moth>();
            mothObj.name = "Moth" + i;
            mothObj.transform.position = Toolbox.Instance.HoldingArea;
            moth.PauseAnimation();
            mothList[i] = moth;
        }
        _moths = mothList;
        _index = 0;
    }

    public void SetupMothsInList(MothType[] mothList, float xOffset)
    {
        foreach (MothType moth in mothList)
        {
            Moth newMoth = GetMothFromPool();
            newMoth.transform.position = new Vector3(moth.Pos.x + xOffset, moth.Pos.y, MothZLayer);
            newMoth.transform.localScale = moth.Scale;
            newMoth.transform.localRotation = moth.Rotation;
            newMoth.ActivateMoth(moth.Colour);
        }
    }

    public void SetVelocity(float speed)
    {
        foreach (Moth moth in _moths)
        {
            moth.SetSpeed(speed);
        }
    }

    public void SetPaused(bool pauseGame)
    {
        foreach (Moth moth in _moths)
        {
            moth.SetPaused(pauseGame);
        }
    }

    public void ActivateMothInRange(float minY, float maxY, Moth.MothColour colour)
    {
        Moth newMoth = GetMothFromPool();
        float mothPosY = Random.Range(minY, maxY);
        newMoth.transform.position = new Vector3(10f, mothPosY, MothZLayer);
        newMoth.ActivateMoth(colour, Moth.MothPathTypes.Sine);
    }
}
