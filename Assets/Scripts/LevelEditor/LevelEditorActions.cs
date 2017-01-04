using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class LevelEditorActions : MonoBehaviour
{

    public LevelContainer Level;

    GameObject LevelObj = null;
    public int LevelNum;
    public bool DebugMode;

    StalactiteEditor StalEditControl = new StalactiteEditor();

    Transform CaveParent = null;
    Transform MothParent = null;
    Transform StalParent = null;
    private int NumSections = 0;
    private int LoadedLevelNum;

    private float TileSizeX;
    private float CaveZ;
    private float StalZ;
    private float ShroomZ;
    private float MothZ;
    private float ClumsyZ;
    private float SpiderZ;
    private float WebZ;
    private float TriggerZ;

    void Awake()
    {
        Toolbox.Instance.Debug = DebugMode;
        if (LevelNum == 0)
        {
            Debug.Log("No Level Set!!!!");
        }
    }

    void Start()
    {
        Level = new LevelContainer();
        CaveParent = GameObject.Find("Caves").transform;
        MothParent = GameObject.Find("Moths").transform;
        StalParent = GameObject.Find("Stalactites").transform;
        LevelObj = GameObject.Find("Level");
        SetZLayers();
        SetLevelNum();
        LoadBtn();
    }

    void Update()
    {
        if (CaveParent != null)
        {
            SetLevelStats();
            LineUpCaves();
        }

        if (MothParent != null)
        {
            AlignMoths();
        }
        
        StalEditControl.ProcessStalactites(StalParent);
    }

    private void AlignMoths()
    {
        foreach (Transform Moth in MothParent)
        {
            Moth MothScript = Moth.GetComponent<Moth>();
            Transform MothTF = null;
            foreach (Transform TF in Moth.transform)
            {
                if (TF.name == "MothTrigger")
                {
                    MothTF = TF;
                }
            }

            if (MothTF.position != Moth.transform.position)
            {
                MothTF.position = Moth.transform.position;
            }

            SpriteRenderer MothRenderer = Moth.GetComponentInChildren<SpriteRenderer>();
            switch (MothScript.Colour)
            {
                case global::Moth.MothColour.Blue:
                    MothRenderer.color = new Color(0f, 0f, 1f);
                    break;
                case global::Moth.MothColour.Green:
                    MothRenderer.color = new Color(0f, 1f, 0f);
                    break;
                case global::Moth.MothColour.Gold:
                    MothRenderer.color = new Color(1f, 1f, 0f);
                    break;
            }
            
        }
    }

    // TODO couldn't figure this out... try again later
    /*private void PlaceOnInput()
    {
        //Vector2 Pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        //Debug.Log(Input.mousePosition.ToString());

        //Debug.Log(Pos);

        Transform MothParent = GameObject.Find("Moths").transform;
        Transform ShroomParent = GameObject.Find("Mushrooms").transform;
        if (Input.GetKeyUp("m"))
        {
            //GameObject NewMoth = (GameObject)Instantiate(Resources.Load("Collectibles/Moth"), new Vector3(Pos.x, Pos.y, MothZ), new Quaternion(), MothParent);
        }
        else if (Input.GetKeyUp("h"))
        {
            GameObject NewShroom = (GameObject)Instantiate(Resources.Load("Obstacles/Mushroom"), ShroomParent);
        }
        else if (Input.GetKeyUp("s"))
        {

        }

    }*/

    private void SetLevelStats()
    {
        GetNumSections();
        int Sections = NumSections;
        float DistOffset = 0f;
        float Distance = Sections * 19.2f - DistOffset;
        float TimeTaken = Distance / 4f;
        GameObject.Find("LevelNumText2").GetComponent<Text>().text = "Level: " + LoadedLevelNum;
        GameObject.Find("NumSectionsText").GetComponent<Text>().text = "Sections: " + Sections.ToString();
        GameObject.Find("DistanceText").GetComponent<Text>().text = "Distance: " + Distance + "m";
        GameObject.Find("TimeText").GetComponent<Text>().text = "Time to complete: " + TimeTaken + " sec";
    }

    private void LineUpCaves()
    {
        foreach (Transform Cave in CaveParent)
        {
            int index = Mathf.RoundToInt(Cave.position.x / TileSizeX);
            PolygonCollider2D Collider = Cave.GetComponent<PolygonCollider2D>();
            if (Collider != null)
            {
                Collider.enabled = false;
            }

            SpriteRenderer Renderer = Cave.GetComponent<SpriteRenderer>();
            if (Renderer != null)
            {
                if (Cave.name != Renderer.sprite.name)
                {
                    Cave.name = Renderer.sprite.name;
                }
            }

            Cave.transform.position = new Vector3(index * TileSizeX, 0f, CaveZ);
        }

    }

    private void SetZLayers()
    {
        TileSizeX = Toolbox.TileSizeX;
        CaveZ = Toolbox.Instance.ZLayers["Cave"];
        StalZ = Toolbox.Instance.ZLayers["Stalactite"];
        ShroomZ = Toolbox.Instance.ZLayers["Mushroom"];
        MothZ = Toolbox.Instance.ZLayers["Moth"];
        ClumsyZ = Toolbox.Instance.ZLayers["Player"];
        SpiderZ = Toolbox.Instance.ZLayers["Spider"];
        WebZ = Toolbox.Instance.ZLayers["Web"];
        TriggerZ = Toolbox.Instance.ZLayers["Trigger"];

        StalEditControl.SetZLayers(TriggerZ);
    }

    public void SaveBtn()
    {
        if (CaveParent == null) { CaveParent = GameObject.Find("Caves").GetComponent<Transform>(); }
        GetNumSections();
        Level.Caves = new LevelContainer.CaveType[NumSections];
        InitialiseCaveList();
        StoreCaveIndexes();

        StoreStalactites();
        StoreMushrooms();
        StoreMoths();
        StoreClumsy();
        StoreSpiders();
        StoreWebs();
        StoreTriggers();

        string LevelName = "Level" + LevelNum + ".xml";
        string PathName = "Assets/Resources/LevelXML";
        Level.Save(Path.Combine(PathName, LevelName));
        Debug.Log("Level data saved to " + PathName + "/" + LevelName);
    }

    public void TestButton()
    {
        Toolbox.Instance.Level = LevelNum;
        Toolbox.Instance.Debug = DebugMode;
        SceneManager.LoadScene("Levels");
    }

    public void LevelUp()
    {
        LevelNum++;
        SetLevelNum();
    }

    public void LevelDown()
    {
        LevelNum--;
        SetLevelNum();
    }

    private void SetLevelNum()
    {
        GameObject.Find("LevelNumText").GetComponent<Text>().text = "Level: " + LevelNum.ToString();
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
            Level.Caves[i].Spiders = new SpiderPool.SpiderType[0];
            Level.Caves[i].Webs = new WebPool.WebType[0];
            Level.Caves[i].Triggers = new TriggerHandler.TriggerType[0];
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
        NumSections = 2;
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
            Transform StalObj = null;
            Transform StalTrigger = null;
            foreach (Transform StalChild in Stal)
            {
                if (StalChild.name == "StalObject") { StalObj = StalChild; }
                else if (StalChild.name == "StalTrigger") { StalTrigger = StalChild; }
            }

            int index = Mathf.RoundToInt(Stal.position.x / TileSizeX);

            StalPool.StalType NewStal = Level.Caves[index].Stals[StalNum[index]];
            Stalactite StalScript = Stal.GetComponent<Stalactite>();
            NewStal.Pos = new Vector2(Stal.position.x - TileSizeX * index, Stal.position.y);
            NewStal.Scale = StalObj.localScale;
            NewStal.Rotation = StalObj.localRotation;
            NewStal.DropEnabled = StalScript.UnstableStalactite;
            NewStal.Flipped = StalScript.Flipped;
            NewStal.FallPreset = StalScript.FallPreset;
            NewStal.TriggerPos = new Vector2(StalTrigger.position.x - TileSizeX * index, StalTrigger.position.y);
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

    private void StoreSpiders()
    {
        Transform SpiderParent = GameObject.Find("Spiders").GetComponent<Transform>();
        int[] SpiderCounts = new int[NumSections];
        SpiderCounts = GetObjCounts(SpiderParent);
        for (int i = 0; i < NumSections; i++)
        {
            Level.Caves[i].Spiders = new SpiderPool.SpiderType[SpiderCounts[i]];
        }

        int[] SpiderNum = new int[NumSections];
        foreach (Transform Spider in SpiderParent)
        {
            int index = Mathf.RoundToInt(Spider.position.x / TileSizeX);

            SpiderPool.SpiderType NewSpider = Level.Caves[index].Spiders[SpiderNum[index]];
            NewSpider.Pos = new Vector2(Spider.position.x - TileSizeX * index, Spider.position.y);
            NewSpider.Scale = Spider.localScale;
            NewSpider.Rotation = Spider.localRotation;
            NewSpider.bSwinging = Spider.GetComponent<SpiderClass>().SwingingSpider;
            Level.Caves[index].Spiders[SpiderNum[index]] = NewSpider;
            SpiderNum[index]++;
        }
    }

    private void StoreWebs()
    {
        Transform WebParent = GameObject.Find("Webs").GetComponent<Transform>();
        int[] WebCounts = new int[NumSections];
        WebCounts = GetObjCounts(WebParent);
        for (int i = 0; i < NumSections; i++)
        {
            Level.Caves[i].Webs = new WebPool.WebType[WebCounts[i]];
        }

        int[] WebNum = new int[NumSections];
        foreach (Transform Web in WebParent)
        {
            int index = Mathf.RoundToInt(Web.position.x / TileSizeX);

            WebPool.WebType NewWeb = Level.Caves[index].Webs[WebNum[index]];
            NewWeb.Pos = new Vector2(Web.position.x - TileSizeX * index, Web.position.y);
            NewWeb.Scale = Web.localScale;
            NewWeb.Rotation = Web.localRotation;
            NewWeb.bSpecialWeb = Web.GetComponent<WebClass>().SpecialWeb;
            Level.Caves[index].Webs[WebNum[index]] = NewWeb;
            WebNum[index]++;
        }
    }

    private void StoreTriggers()
    {
        Transform TriggerParent = GameObject.Find("Triggers").GetComponent<Transform>();
        int[] TriggerCounts = new int[NumSections];
        TriggerCounts = GetObjCounts(TriggerParent);
        for (int i = 0; i < NumSections; i++)
        {
            Level.Caves[i].Triggers = new TriggerHandler.TriggerType[TriggerCounts[i]];
        }

        int[] TriggerNum = new int[NumSections];
        foreach (Transform Trigger in TriggerParent)
        {
            int index = Mathf.RoundToInt(Trigger.position.x / TileSizeX);

            TriggerHandler.TriggerType NewTrigger = Level.Caves[index].Triggers[TriggerNum[index]];
            NewTrigger.Pos = new Vector2(Trigger.position.x - TileSizeX * index, Trigger.position.y);
            NewTrigger.Scale = Trigger.localScale;
            NewTrigger.Rotation = Trigger.localRotation;
            NewTrigger.EventID = Trigger.GetComponent<TriggerClass>().EventID;
            NewTrigger.EventType = Trigger.GetComponent<TriggerClass>().EventType;
            Level.Caves[index].Triggers[TriggerNum[index]] = NewTrigger;
            TriggerNum[index]++;
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
        LoadedLevelNum = LevelNum;
    }

    private void ClearLevelObjects()
    {
        Destroy(GameObject.Find("Stalactites"));
        Destroy(GameObject.Find("Mushrooms"));
        Destroy(GameObject.Find("Moths"));
        Destroy(GameObject.Find("Spiders"));
        Destroy(GameObject.Find("Webs"));
        Destroy(GameObject.Find("Triggers"));
        Destroy(GameObject.Find("Clumsy"));

        foreach(Transform Cave in CaveParent)
        {
            Destroy(Cave.gameObject);
        }
    }

    private void SetLevelObjects()
    {
        if (LevelObj == null) {
            Debug.Log("Lost level");
            LevelObj = new GameObject("Level");
        }
        GameObject Stals = new GameObject("Stalactites");
        GameObject Shrooms = new GameObject("Mushrooms");
        GameObject Moths = new GameObject("Moths");
        GameObject Spiders = new GameObject("Spiders");
        GameObject Webs = new GameObject("Webs");
        GameObject Triggers = new GameObject("Triggers");
        GameObject Caves = CaveParent.gameObject;
        MothParent = Moths.transform;
        StalParent = Stals.transform;

        Stals.transform.SetParent(LevelObj.transform);
        Shrooms.transform.SetParent(LevelObj.transform);
        Moths.transform.SetParent(LevelObj.transform);
        Spiders.transform.SetParent(LevelObj.transform);
        Webs.transform.SetParent(LevelObj.transform);
        Triggers.transform.SetParent(LevelObj.transform);

        GameObject Clumsy = (GameObject)Instantiate(Resources.Load("ClumsyLevelEditor"), LevelObj.transform);
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

            SetStalactites(Stals, Cave.Stals, i);
            SetMushrooms(Shrooms, Cave.Shrooms, i);
            SetMoths(Moths, Cave.Moths, i);
            SetSpiders(Spiders, Cave.Spiders, i);
            SetWebs(Webs, Cave.Webs, i);
            SetTriggers(Triggers, Cave.Triggers, i);
        }
    }

    private void SetStalactites(GameObject Stals, StalPool.StalType[] StalList, int PosIndex)
    {
        if (StalList == null) { return; }
        foreach (StalPool.StalType Stal in StalList)
        {
            GameObject NewStal = (GameObject)Instantiate(Resources.Load("Obstacles/Stalactite"), Stals.transform);
            Transform StalObj = null;
            Transform StalTrigger = null;
            Stalactite StalScript = NewStal.GetComponent<Stalactite>();

            foreach (Transform StalChild in NewStal.transform)
            {
                if (StalChild.name == "StalObject") { StalObj = StalChild; }
                else if (StalChild.name == "StalTrigger") { StalTrigger = StalChild; }
            }

            NewStal.transform.position = new Vector3(Stal.Pos.x + PosIndex * TileSizeX, Stal.Pos.y, StalZ);
            StalObj.localScale = Stal.Scale;
            StalObj.localRotation = Stal.Rotation;
            StalScript.FallPreset = Stal.FallPreset;
            StalScript.Flipped = Stal.Flipped;
            StalScript.UnstableStalactite = Stal.DropEnabled;
            StalTrigger.position = new Vector2(Stal.TriggerPos.x + PosIndex * TileSizeX, Stal.TriggerPos.y);
        }
    }

    private void SetMushrooms(GameObject Shrooms, ShroomPool.ShroomType[] ShroomList, int PosIndex)
    {
        foreach (ShroomPool.ShroomType Shroom in ShroomList)
        {
            GameObject NewShroom = (GameObject)Instantiate(Resources.Load("Obstacles/Mushroom"), Shrooms.transform);
            NewShroom.transform.position = new Vector3(Shroom.Pos.x + PosIndex * TileSizeX, Shroom.Pos.y, ShroomZ);
            NewShroom.transform.localScale = Shroom.Scale;
            NewShroom.transform.localRotation = Shroom.Rotation;
        }
    }

    private void SetMoths(GameObject Moths, MothPool.MothType[] MothList, int PosIndex)
    {
        foreach (MothPool.MothType Moth in MothList)
        {
            GameObject NewMoth = (GameObject)Instantiate(Resources.Load("Collectibles/Moth"), Moths.transform);
            NewMoth.transform.position = new Vector3(Moth.Pos.x + PosIndex * TileSizeX, Moth.Pos.y, MothZ);
            NewMoth.transform.localScale = Moth.Scale;
            NewMoth.transform.localRotation = Moth.Rotation;
            NewMoth.GetComponent<Moth>().Colour = Moth.Colour;
        }
    }

    private void SetSpiders(GameObject Spiders, SpiderPool.SpiderType[] SpiderList, int PosIndex)
    {
        if (SpiderList == null) { return; }
        foreach (SpiderPool.SpiderType Spider in SpiderList)
        {
            GameObject NewSpider = (GameObject)Instantiate(Resources.Load("Obstacles/Spider"), Spiders.transform);
            NewSpider.transform.position = new Vector3(Spider.Pos.x + PosIndex * TileSizeX, Spider.Pos.y, SpiderZ);
            NewSpider.transform.localScale = Spider.Scale;
            NewSpider.transform.localRotation = Spider.Rotation;
            NewSpider.GetComponent<SpiderClass>().SwingingSpider = Spider.bSwinging;
        }
    }

    private void SetWebs(GameObject Webs, WebPool.WebType[] WebList, int PosIndex)
    {
        if (WebList == null) { return; }
        foreach (WebPool.WebType Web in WebList)
        {
            GameObject NewWeb = (GameObject)Instantiate(Resources.Load("Obstacles/Web"), Webs.transform);
            NewWeb.transform.position = new Vector3(Web.Pos.x + PosIndex * TileSizeX, Web.Pos.y, WebZ);
            NewWeb.transform.localScale = Web.Scale;
            NewWeb.transform.localRotation = Web.Rotation;
            NewWeb.GetComponent<WebClass>().SpecialWeb = Web.bSpecialWeb;
        }
    }

    private void SetTriggers(GameObject Triggers, TriggerHandler.TriggerType[] TriggerList, int PosIndex)
    {
        if (TriggerList == null) { return; }
        foreach (TriggerHandler.TriggerType Trigger in TriggerList)
        {
            GameObject NewTrigger = (GameObject)Instantiate(Resources.Load("Interactables/Trigger"), Triggers.transform);
            NewTrigger.transform.position = new Vector3(Trigger.Pos.x + PosIndex * TileSizeX, Trigger.Pos.y, TriggerZ);
            NewTrigger.transform.localScale = Trigger.Scale;
            NewTrigger.transform.localRotation = Trigger.Rotation;
            NewTrigger.GetComponent<TriggerClass>().EventID = Trigger.EventID;
            NewTrigger.GetComponent<TriggerClass>().EventType = Trigger.EventType;
        }
    }
}
