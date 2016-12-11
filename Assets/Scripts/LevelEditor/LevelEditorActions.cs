using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class LevelEditorActions : MonoBehaviour {
    
    public LevelContainer Level;

    GameObject LevelObj = null;
    public int LevelNum;
    
    Transform CaveParent = null;
    private int NumSections = 0;

    private const float TileSizeX = 19.2f;  // TODO have this a toolbox const
    private const float CaveZ = 0f;
    private const float StalZ = 1f;
    private const float ShroomZ = 2f;
    private const float MothZ = 3f;
    private const float ClumsyZ = -1f;

    void Awake()
    {
        if (LevelNum == 0)
        {
            Debug.Log("No Level Set!!!!");
        }
    }

    void Start()
    {
        Level = new LevelContainer();
    }

    public void SaveBtn()
    {
        CaveParent = GameObject.Find("Caves").GetComponent<Transform>();
        GetNumSections();
        Level.Caves = new LevelContainer.CaveType[NumSections];
        InitialiseCaveList();
        StoreCaveIndexes();

        StoreStalactites();
        StoreMushrooms();
        StoreMoths();
        StoreClumsy();
        
        string LevelName = "Level" + LevelNum + ".xml";
        string PathName = "Assets/Resources/LevelXML";
        Level.Save(Path.Combine(PathName, LevelName));
        Debug.Log("Level data saved to " + PathName + "/" + LevelName);
    }

    private void InitialiseCaveList()
    {
        for (int i = 0; i < NumSections; i++)
        {
            Level.Caves[i].TopIndex = 0;
            Level.Caves[i].BottomIndex = 0;
            Level.Caves[i].bTopSecretPath = false;
            Level.Caves[i].bBottomSecretPath = false;
            Level.Caves[i].Shrooms = new ShroomPool.ShroomType[0];
            Level.Caves[i].Stals = new StalPool.StalType[0];
            Level.Caves[i].Moths = new MothPool.MothType[0];
        }
    }

    private void StoreCaveIndexes()
    {
        foreach (Transform Cave in CaveParent)
        {
            int index = Mathf.RoundToInt(Cave.position.x / TileSizeX);
            if (Cave.name == "CaveEntrance")
            {
                Level.Caves[index].BottomIndex = 1000;
                Level.Caves[index].TopIndex = 1000;
            }
            else if (Cave.name == "CaveExit")
            {
                Level.Caves[index].BottomIndex = 1001;
                Level.Caves[index].TopIndex = 1001;
            }
            else if (Cave.name.Contains("Top"))
            {
                int CaveType = int.Parse(Cave.name.Substring(Cave.name.Length - 1, 1)) - 1;
                Level.Caves[index].TopIndex = CaveType;
                if (Cave.name.Contains("Exit"))
                {
                    Level.Caves[index].bTopSecretPath = true;
                }
            }
            else if (Cave.name.Contains("Bottom"))
            {
                int CaveType = int.Parse(Cave.name.Substring(Cave.name.Length - 1, 1)) - 1;
                Level.Caves[index].BottomIndex = CaveType;
                if (Cave.name.Contains("Exit"))
                {
                    Level.Caves[index].bBottomSecretPath = true;
                }
            }
        }
    }

    public void GetNumSections()
    {
        foreach (Transform Cave in CaveParent)
        {
            if (Cave.name.Contains("Cave"))
            {
                int index = Mathf.RoundToInt(Cave.position.x / TileSizeX);
                if (index + 1 > NumSections)
                {
                    NumSections = index + 1;
                }
            }
        }
    }

    private int[] GetObjCounts(Transform ObjParent)
    {
        int[] ObjCounts = new int[NumSections];
        foreach (Transform Obj in ObjParent)
        {
            int index = Mathf.RoundToInt(Obj.position.x / TileSizeX);
            ObjCounts[index]++;
        }
        return ObjCounts;
    }

    private void StoreStalactites()
    {
        Transform StalParent = GameObject.Find("Stalactites").GetComponent<Transform>();
        int[] StalCounts = new int[NumSections];
        StalCounts = GetObjCounts(StalParent);
        for (int i = 0; i < NumSections; i++)
        {
            Level.Caves[i].Stals = new StalPool.StalType[StalCounts[i]];
        }

        int[] StalNum = new int[NumSections];
        foreach (Transform Stal in StalParent)
        {
            int index = Mathf.RoundToInt(Stal.position.x / TileSizeX);

            StalPool.StalType NewStal = Level.Caves[index].Stals[StalNum[index]];
            NewStal.Pos = new Vector2(Stal.position.x - TileSizeX * index, Stal.position.y);
            NewStal.Scale = Stal.localScale;
            NewStal.Rotation = Stal.localRotation;
            NewStal.DropEnabled = Stal.GetComponent<Stalactite>().UnstableStalactite;
            Level.Caves[index].Stals[StalNum[index]] = NewStal;
            StalNum[index]++;
        }
    }

    private void StoreMushrooms()
    {
        Transform ShroomParent = GameObject.Find("Mushrooms").GetComponent<Transform>();
        int[] ShroomCounts = new int[NumSections];
        ShroomCounts = GetObjCounts(ShroomParent);
        for (int i = 0; i < NumSections; i++)
        {
            Level.Caves[i].Shrooms = new ShroomPool.ShroomType[ShroomCounts[i]];
        }
        
        int[] ShroomNum = new int[NumSections];
        foreach (Transform Shroom in ShroomParent)
        {
            int index = Mathf.RoundToInt(Shroom.position.x / TileSizeX);

            ShroomPool.ShroomType NewShroom = Level.Caves[index].Shrooms[ShroomNum[index]];
            NewShroom.Pos = new Vector2(Shroom.position.x - TileSizeX * index, Shroom.position.y);
            NewShroom.Scale = Shroom.localScale;
            NewShroom.Rotation = Shroom.localRotation;
            NewShroom.SpecialEnabled = false;
            Level.Caves[index].Shrooms[ShroomNum[index]] = NewShroom;
            ShroomNum[index]++;
        }
    }

    private void StoreMoths()
    {
        Transform MothParent = GameObject.Find("Moths").GetComponent<Transform>();
        int[] MothCounts = new int[NumSections];
        MothCounts = GetObjCounts(MothParent);
        for (int i = 0; i < NumSections; i++)
        {
            Level.Caves[i].Moths = new MothPool.MothType[MothCounts[i]];
        }

        int[] MothNum = new int[NumSections];
        foreach (Transform Moth in MothParent)
        {
            int index = Mathf.RoundToInt(Moth.position.x / TileSizeX);

            MothPool.MothType NewMoth = Level.Caves[index].Moths[MothNum[index]];
            NewMoth.Pos = new Vector2(Moth.position.x - TileSizeX * index, Moth.position.y);
            NewMoth.Scale = Moth.localScale;
            NewMoth.Rotation = Moth.localRotation;
            NewMoth.Colour = Moth.GetComponent<Moth>().Colour;
            Level.Caves[index].Moths[MothNum[index]] = NewMoth;
            MothNum[index]++;
        }
    }

    private void StoreClumsy()
    {
        Transform Clumsy = GameObject.Find("Clumsy").GetComponent<Transform>();
        LevelContainer.ClumsyType NewClumsy = Level.Clumsy;
        NewClumsy.Pos = Clumsy.position;
        NewClumsy.Rotation = Clumsy.localRotation;
        NewClumsy.Scale = Clumsy.localScale;
        Level.Clumsy = NewClumsy;
    }

    public void LoadBtn()
    {
        if (LevelNum == 0) { return; }
        TextAsset LevelTxt = (TextAsset)Resources.Load("LevelXML/Level" + LevelNum);
        Level = LevelContainer.LoadFromText(LevelTxt.text);
        ClearLevelObjects();
        SetLevelObjects();
    }

    private void ClearLevelObjects()
    {
        Destroy(GameObject.Find("Level"));

        LevelObj = new GameObject();
        LevelObj.name = "Level";
    }

    private void SetLevelObjects()
    {
        GameObject Caves = new GameObject("Caves");
        GameObject Stals = new GameObject("Stalactites");
        GameObject Shrooms = new GameObject("Mushrooms");
        GameObject Moths = new GameObject("Moths");
        
        Caves.transform.SetParent(LevelObj.transform);
        Stals.transform.SetParent(LevelObj.transform);
        Shrooms.transform.SetParent(LevelObj.transform);
        Moths.transform.SetParent(LevelObj.transform);

        GameObject Clumsy = (GameObject) Instantiate(Resources.Load("Clumsy"), LevelObj.transform);
        Clumsy.name = "Clumsy";
        Clumsy.transform.position = new Vector3(Level.Clumsy.Pos.x, Level.Clumsy.Pos.y, ClumsyZ);
        Clumsy.transform.localRotation = Level.Clumsy.Rotation;
        Clumsy.transform.localScale = Level.Clumsy.Scale;

        for (int i = 0; i < Level.Caves.Length; i++)
        {
            LevelContainer.CaveType Cave = Level.Caves[i];
            GameObject CaveBottom = null; ;
            GameObject CaveTop = null;
            if (Cave.TopIndex == 1000)
            {
                CaveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), Caves.transform);
                CaveTop.name = "CaveEntrance";
            }
            else if (Cave.TopIndex == 1001)
            {
                CaveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), Caves.transform);
                CaveTop.name = "CaveExit";
            }
            else
            {
                string CaveBottomName = "CaveBottom" + (Cave.bBottomSecretPath ? "Exit" : "") + (Cave.BottomIndex + 1).ToString();
                string CaveTopName = "CaveTop" + (Cave.bTopSecretPath ? "Exit" : "") + (Cave.TopIndex + 1).ToString();
                CaveBottom = (GameObject)Instantiate(Resources.Load("Caves/" + CaveBottomName), Caves.transform);
                CaveTop = (GameObject)Instantiate(Resources.Load("Caves/" + CaveTopName), Caves.transform);
                CaveBottom.name = CaveBottomName;
                CaveTop.name = CaveTopName;
                CaveBottom.transform.position = new Vector3(TileSizeX * i, 0f, CaveZ);
            }
            CaveTop.transform.position = new Vector3(TileSizeX * i, 0f, CaveZ);

            foreach(StalPool.StalType Stal in Cave.Stals)
            {
                GameObject NewStal = (GameObject)Instantiate(Resources.Load("Obstacles/Stalactite"), Stals.transform);
                NewStal.transform.position = new Vector3(Stal.Pos.x + i*TileSizeX, Stal.Pos.y, StalZ);
                NewStal.transform.localScale = Stal.Scale;
                NewStal.transform.localRotation = Stal.Rotation;
                NewStal.GetComponent<Stalactite>().UnstableStalactite = Stal.DropEnabled;
            }

            foreach (ShroomPool.ShroomType Shroom in Cave.Shrooms)
            {
                GameObject NewShroom = (GameObject)Instantiate(Resources.Load("Obstacles/Mushroom"), Shrooms.transform);
                NewShroom.transform.position = new Vector3(Shroom.Pos.x + i * TileSizeX, Shroom.Pos.y, ShroomZ);
                NewShroom.transform.localScale = Shroom.Scale;
                NewShroom.transform.localRotation = Shroom.Rotation;
            }

            foreach (MothPool.MothType Moth in Cave.Moths)
            {
                GameObject NewMoth = (GameObject)Instantiate(Resources.Load("Collectibles/Moth"), Moths.transform);
                NewMoth.transform.position = new Vector3(Moth.Pos.x + i * TileSizeX, Moth.Pos.y, MothZ);
                NewMoth.transform.localScale = Moth.Scale;
                NewMoth.transform.localRotation = Moth.Rotation;
                NewMoth.GetComponent<Moth>().Colour = Moth.Colour;
            }
        }
    }
}
