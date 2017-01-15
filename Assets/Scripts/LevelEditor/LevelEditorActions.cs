using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LevelEditorActions : MonoBehaviour
{

    public LevelContainer Level;

    private GameObject _levelObj;
    public int LevelNum;
    public bool DebugMode;

    private readonly StalactiteEditor _stalEditControl = new StalactiteEditor();

    private Transform _caveParent;
    private Transform _mothParent;
    private Transform _stalParent;
    private int _numSections;
    private int _loadedLevelNum;

    private float _tileSizeX;
    private float _caveZ;
    private float _stalZ;
    private float _shroomZ;
    private float _mothZ;
    private float _spiderZ;
    private float _webZ;
    private float _triggerZ;

    private void Awake()
    {
        Toolbox.Instance.Debug = DebugMode;
        if (LevelNum == 0)
        {
            Debug.Log("No Level Set!!!!");
        }
    }

    private void Start()
    {
        Level = new LevelContainer();
        _caveParent = GameObject.Find("Caves").transform;
        _mothParent = GameObject.Find("Moths").transform;
        _stalParent = GameObject.Find("Stalactites").transform;
        _levelObj = GameObject.Find("Level");
        SetZLayers();
        SetLevelNum();
        LoadBtn();
    }

    private void Update()
    {
        if (_caveParent != null)
        {
            SetLevelStats();
            LineUpCaves();
        }

        if (_mothParent != null)
        {
            AlignMoths();
        }
        
        _stalEditControl.ProcessStalactites(_stalParent);
    }

    private void AlignMoths()
    {
        foreach (Transform moth in _mothParent)
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
        int sections = _numSections;
        const float distOffset = 0f;
        float distance = sections * 19.2f - distOffset;
        float timeTaken = distance / 4f;
        GameObject.Find("LevelNumText2").GetComponent<Text>().text = "Level: " + _loadedLevelNum;
        GameObject.Find("NumSectionsText").GetComponent<Text>().text = "Sections: " + sections;
        GameObject.Find("DistanceText").GetComponent<Text>().text = "Distance: " + distance + "m";
        GameObject.Find("TimeText").GetComponent<Text>().text = "Time to complete: " + timeTaken + " sec";
    }

    private void LineUpCaves()
    {
        foreach (Transform cave in _caveParent)
        {
            int index = Mathf.RoundToInt(cave.position.x / _tileSizeX);
            PolygonCollider2D caveCollider = cave.GetComponent<PolygonCollider2D>();
            if (caveCollider != null)
            {
                caveCollider.enabled = false;
            }

            SpriteRenderer caveRenderer = cave.GetComponent<SpriteRenderer>();
            if (caveRenderer != null)
            {
                if (cave.name != caveRenderer.sprite.name)
                {
                    cave.name = caveRenderer.sprite.name;
                }
            }
            cave.transform.position = new Vector3(index * _tileSizeX, 0f, _caveZ);
        }
    }

    private void SetZLayers()
    {
        _tileSizeX = Toolbox.TileSizeX;
        _caveZ = Toolbox.Instance.ZLayers["Cave"];
        _stalZ = Toolbox.Instance.ZLayers["Stalactite"];
        _shroomZ = Toolbox.Instance.ZLayers["Mushroom"];
        _mothZ = Toolbox.Instance.ZLayers["Moth"];
        _spiderZ = Toolbox.Instance.ZLayers["Spider"];
        _webZ = Toolbox.Instance.ZLayers["Web"];
        _triggerZ = Toolbox.Instance.ZLayers["Trigger"];

        _stalEditControl.SetZLayers(_triggerZ);
    }

    public void SaveBtn()
    {
        if (_caveParent == null) { _caveParent = GameObject.Find("Caves").GetComponent<Transform>(); }
        GetNumSections();
        Level.Caves = new LevelContainer.CaveType[_numSections];
        InitialiseCaveList();
        StoreCaveIndexes();

        StoreStalactites();
        StoreMushrooms();
        StoreMoths();
        StoreClumsy();
        StoreSpiders();
        StoreWebs();
        StoreTriggers();

        string levelName = "Level" + LevelNum + ".xml";
        const string pathName = "Assets/Resources/LevelXML";
        Level.Save(Path.Combine(pathName, levelName));
        Debug.Log("Level data saved to " + pathName + "/" + levelName);
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
        for (int i = 0; i < _numSections; i++)
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
        foreach (Transform cave in _caveParent)
        {
            int index = Mathf.RoundToInt(cave.position.x / _tileSizeX);
            if (cave.name == "CaveEntrance")
            {
                Level.Caves[index].BottomIndex = 1000;
                Level.Caves[index].TopIndex = 1000;
            }
            else if (cave.name == "CaveExit")
            {
                Level.Caves[index].BottomIndex = 1001;
                Level.Caves[index].TopIndex = 1001;
            }
            else if (cave.name.Contains("Top"))
            {
                int caveType = int.Parse(cave.name.Substring(cave.name.Length - 1, 1)) - 1;
                Level.Caves[index].TopIndex = caveType;
                if (cave.name.Contains("Exit"))
                {
                    Level.Caves[index].bTopSecretPath = true;
                }
            }
            else if (cave.name.Contains("Bottom"))
            {
                int caveType = int.Parse(cave.name.Substring(cave.name.Length - 1, 1)) - 1;
                Level.Caves[index].BottomIndex = caveType;
                if (cave.name.Contains("Exit"))
                {
                    Level.Caves[index].bBottomSecretPath = true;
                }
            }
        }
    }

    public void GetNumSections()
    {
        _numSections = 2;
        foreach (Transform cave in _caveParent)
        {
            if (cave.name.Contains("Cave"))
            {
                int index = Mathf.RoundToInt(cave.position.x / _tileSizeX);
                if (index + 1 > _numSections)
                {
                    _numSections = index + 1;
                }
            }
        }
    }

    private int[] GetObjCounts(Transform objParent)
    {
        int[] objCounts = new int[_numSections];
        foreach (Transform obj in objParent)
        {
            int index = Mathf.RoundToInt(obj.position.x / _tileSizeX);
            objCounts[index]++;
        }
        return objCounts;
    }

    private void StoreStalactites()
    {
        Transform stalParent = GameObject.Find("Stalactites").GetComponent<Transform>();
        var stalCounts = GetObjCounts(stalParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Stals = new StalPool.StalType[stalCounts[i]];
        }

        int[] stalNum = new int[_numSections];
        foreach (Transform stal in stalParent)
        {
            Transform stalObj = null;
            Transform stalTrigger = null;
            foreach (Transform stalChild in stal)
            {
                if (stalChild.name == "StalObject") { stalObj = stalChild; }
                else if (stalChild.name == "StalTrigger") { stalTrigger = stalChild; }
            }

            int index = Mathf.RoundToInt(stal.position.x / _tileSizeX);

            StalPool.StalType newStal = Level.Caves[index].Stals[stalNum[index]];
            Stalactite stalScript = stal.GetComponent<Stalactite>();
            newStal.Pos = new Vector2(stal.position.x - _tileSizeX * index, stal.position.y);
            if (stalObj != null)
            {
                newStal.Scale = stalObj.localScale;
                newStal.Rotation = stalObj.localRotation;
            }
            newStal.DropEnabled = stalScript.UnstableStalactite;
            newStal.Flipped = stalScript.Flipped;
            if (stalTrigger != null)
                newStal.TriggerPos = new Vector2(stalTrigger.position.x - _tileSizeX * index, stalTrigger.position.y);
            Level.Caves[index].Stals[stalNum[index]] = newStal;
            stalNum[index]++;
        }
    }

    private void StoreMushrooms()
    {
        Transform shroomParent = GameObject.Find("Mushrooms").GetComponent<Transform>();
        var shroomCounts = GetObjCounts(shroomParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Shrooms = new ShroomPool.ShroomType[shroomCounts[i]];
        }

        int[] shroomNum = new int[_numSections];
        foreach (Transform shroom in shroomParent)
        {
            int index = Mathf.RoundToInt(shroom.position.x / _tileSizeX);

            ShroomPool.ShroomType newShroom = Level.Caves[index].Shrooms[shroomNum[index]];
            newShroom.Pos = new Vector2(shroom.position.x - _tileSizeX * index, shroom.position.y);
            newShroom.Scale = shroom.localScale;
            newShroom.Rotation = shroom.localRotation;
            newShroom.SpecialEnabled = false;
            Level.Caves[index].Shrooms[shroomNum[index]] = newShroom;
            shroomNum[index]++;
        }
    }

    private void StoreMoths()
    {
        Transform mothParent = GameObject.Find("Moths").GetComponent<Transform>();
        var mothCounts = GetObjCounts(mothParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Moths = new MothPool.MothType[mothCounts[i]];
        }

        int[] mothNum = new int[_numSections];
        foreach (Transform moth in mothParent)
        {
            int index = Mathf.RoundToInt(moth.position.x / _tileSizeX);

            MothPool.MothType newMoth = Level.Caves[index].Moths[mothNum[index]];
            newMoth.Pos = new Vector2(moth.position.x - _tileSizeX * index, moth.position.y);
            newMoth.Scale = moth.localScale;
            newMoth.Rotation = moth.localRotation;
            newMoth.Colour = moth.GetComponent<Moth>().Colour;
            Level.Caves[index].Moths[mothNum[index]] = newMoth;
            mothNum[index]++;
        }
    }

    private void StoreSpiders()
    {
        Transform spiderParent = GameObject.Find("Spiders").GetComponent<Transform>();
        var spiderCounts = GetObjCounts(spiderParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Spiders = new SpiderPool.SpiderType[spiderCounts[i]];
        }

        int[] spiderNum = new int[_numSections];
        foreach (Transform spider in spiderParent)
        {
            int index = Mathf.RoundToInt(spider.position.x / _tileSizeX);

            SpiderPool.SpiderType newSpider = Level.Caves[index].Spiders[spiderNum[index]];
            newSpider.Pos = new Vector2(spider.position.x - _tileSizeX * index, spider.position.y);
            newSpider.Scale = spider.localScale;
            newSpider.Rotation = spider.localRotation;
            newSpider.bSwinging = spider.GetComponent<SpiderClass>().SwingingSpider;
            Level.Caves[index].Spiders[spiderNum[index]] = newSpider;
            spiderNum[index]++;
        }
    }

    private void StoreWebs()
    {
        Transform webParent = GameObject.Find("Webs").GetComponent<Transform>();
        var webCounts = GetObjCounts(webParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Webs = new WebPool.WebType[webCounts[i]];
        }

        int[] webNum = new int[_numSections];
        foreach (Transform web in webParent)
        {
            int index = Mathf.RoundToInt(web.position.x / _tileSizeX);

            WebPool.WebType newWeb = Level.Caves[index].Webs[webNum[index]];
            newWeb.Pos = new Vector2(web.position.x - _tileSizeX * index, web.position.y);
            newWeb.Scale = web.localScale;
            newWeb.Rotation = web.localRotation;
            newWeb.bSpecialWeb = web.GetComponent<WebClass>().SpecialWeb;
            Level.Caves[index].Webs[webNum[index]] = newWeb;
            webNum[index]++;
        }
    }

    private void StoreTriggers()
    {
        Transform triggerParent = GameObject.Find("Triggers").GetComponent<Transform>();
        var triggerCounts = GetObjCounts(triggerParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Triggers = new TriggerHandler.TriggerType[triggerCounts[i]];
        }

        int[] triggerNum = new int[_numSections];
        foreach (Transform trigger in triggerParent)
        {
            int index = Mathf.RoundToInt(trigger.position.x / _tileSizeX);

            TriggerHandler.TriggerType newTrigger = Level.Caves[index].Triggers[triggerNum[index]];
            newTrigger.Pos = new Vector2(trigger.position.x - _tileSizeX * index, trigger.position.y);
            newTrigger.Scale = trigger.localScale;
            newTrigger.Rotation = trigger.localRotation;
            newTrigger.EventID = trigger.GetComponent<TriggerClass>().EventID;
            newTrigger.EventType = trigger.GetComponent<TriggerClass>().EventType;
            Level.Caves[index].Triggers[triggerNum[index]] = newTrigger;
            triggerNum[index]++;
        }
    }

    private void StoreClumsy()
    {
        Transform clumsy = GameObject.Find("Clumsy").GetComponent<Transform>();
        LevelContainer.ClumsyType newClumsy = Level.Clumsy;
        newClumsy.Pos = clumsy.position;
        newClumsy.Rotation = clumsy.localRotation;
        newClumsy.Scale = clumsy.localScale;
        Level.Clumsy = newClumsy;
    }

    public void LoadBtn()
    {
        if (LevelNum == 0) { return; }
        TextAsset levelTxt = (TextAsset)Resources.Load("LevelXML/Level" + LevelNum);
        Level = LevelContainer.LoadFromText(levelTxt.text);
        ClearLevelObjects();
        SetLevelObjects();
        _loadedLevelNum = LevelNum;
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

        foreach(Transform cave in _caveParent)
        {
            Destroy(cave.gameObject);
        }
    }

    private void SetLevelObjects()
    {
        if (_levelObj == null) {
            Debug.Log("Lost level");
            _levelObj = new GameObject("Level");
        }
        GameObject stals = new GameObject("Stalactites");
        GameObject shrooms = new GameObject("Mushrooms");
        GameObject moths = new GameObject("Moths");
        GameObject spiders = new GameObject("Spiders");
        GameObject webs = new GameObject("Webs");
        GameObject triggers = new GameObject("Triggers");
        GameObject caves = _caveParent.gameObject;
        _mothParent = moths.transform;
        _stalParent = stals.transform;

        stals.transform.SetParent(_levelObj.transform);
        shrooms.transform.SetParent(_levelObj.transform);
        moths.transform.SetParent(_levelObj.transform);
        spiders.transform.SetParent(_levelObj.transform);
        webs.transform.SetParent(_levelObj.transform);
        triggers.transform.SetParent(_levelObj.transform);

        //GameObject Clumsy = (GameObject)Instantiate(Resources.Load("ClumsyLevelEditor"), _levelObj.transform);
        //Clumsy.name = "Clumsy";
        //Clumsy.transform.position = new Vector3(Level.Clumsy.Pos.x, Level.Clumsy.Pos.y, _clumsyZ);
        //Clumsy.transform.localRotation = Level.Clumsy.Rotation;
        //Clumsy.transform.localScale = Level.Clumsy.Scale;

        for (int i = 0; i < Level.Caves.Length; i++)
        {
            LevelContainer.CaveType cave = Level.Caves[i];
            GameObject caveTop;
            if (cave.TopIndex == 1000)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), caves.transform);
                caveTop.name = "CaveEntrance";
            }
            else if (cave.TopIndex == 1001)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), caves.transform);
                caveTop.name = "CaveExit";
            }
            else
            {
                string caveBottomName = "CaveBottom" + (cave.bBottomSecretPath ? "Exit" : "") + (cave.BottomIndex + 1).ToString();
                string caveTopName = "CaveTop" + (cave.bTopSecretPath ? "Exit" : "") + (cave.TopIndex + 1).ToString();
                var caveBottom = (GameObject)Instantiate(Resources.Load("Caves/" + caveBottomName), caves.transform);
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/" + caveTopName), caves.transform);
                caveBottom.name = caveBottomName;
                caveTop.name = caveTopName;
                caveBottom.transform.position = new Vector3(_tileSizeX * i, 0f, _caveZ);
            }
            caveTop.transform.position = new Vector3(_tileSizeX * i, 0f, _caveZ);

            SetStalactites(stals, cave.Stals, i);
            SetMushrooms(shrooms, cave.Shrooms, i);
            SetMoths(moths, cave.Moths, i);
            SetSpiders(spiders, cave.Spiders, i);
            SetWebs(webs, cave.Webs, i);
            SetTriggers(triggers, cave.Triggers, i);
        }
    }

    private void SetStalactites(GameObject stals, StalPool.StalType[] stalList, int posIndex)
    {
        if (stalList == null) { return; }
        foreach (StalPool.StalType stal in stalList)
        {
            GameObject newStal = (GameObject)Instantiate(Resources.Load("Obstacles/EditorStalactite"), stals.transform);
            newStal.name = "Stalactite";
            Transform stalObj = null;
            Transform stalTrigger = null;
            Stalactite stalScript = newStal.GetComponent<Stalactite>();

            foreach (Transform stalChild in newStal.transform)
            {
                if (stalChild.name == "StalObject") { stalObj = stalChild; }
                else if (stalChild.name == "StalTrigger") { stalTrigger = stalChild; }
            }

            newStal.transform.position = new Vector3(stal.Pos.x + posIndex * _tileSizeX, stal.Pos.y, _stalZ);
            if (stalObj != null)
            {
                stalObj.localScale = stal.Scale;
                stalObj.localRotation = stal.Rotation;
            }
            stalScript.Flipped = stal.Flipped;
            stalScript.UnstableStalactite = stal.DropEnabled;
            if (stalTrigger != null)
                stalTrigger.position = new Vector2(stal.TriggerPos.x + posIndex * _tileSizeX, stal.TriggerPos.y);
        }
    }

    private void SetMushrooms(GameObject shrooms, ShroomPool.ShroomType[] shroomList, int posIndex)
    {
        foreach (ShroomPool.ShroomType shroom in shroomList)
        {
            GameObject newShroom = (GameObject)Instantiate(Resources.Load("Obstacles/Mushroom"), shrooms.transform);
            newShroom.transform.position = new Vector3(shroom.Pos.x + posIndex * _tileSizeX, shroom.Pos.y, _shroomZ);
            newShroom.transform.localScale = shroom.Scale;
            newShroom.transform.localRotation = shroom.Rotation;
        }
    }

    private void SetMoths(GameObject moths, MothPool.MothType[] mothList, int posIndex)
    {
        foreach (MothPool.MothType moth in mothList)
        {
            GameObject newMoth = (GameObject)Instantiate(Resources.Load("Collectibles/Moth"), moths.transform);
            newMoth.transform.position = new Vector3(moth.Pos.x + posIndex * _tileSizeX, moth.Pos.y, _mothZ);
            newMoth.transform.localScale = moth.Scale;
            newMoth.transform.localRotation = moth.Rotation;
            newMoth.GetComponent<Moth>().Colour = moth.Colour;
        }
    }

    private void SetSpiders(GameObject spiders, SpiderPool.SpiderType[] spiderList, int posIndex)
    {
        if (spiderList == null) { return; }
        foreach (SpiderPool.SpiderType spider in spiderList)
        {
            GameObject newSpider = (GameObject)Instantiate(Resources.Load("Obstacles/Spider"), spiders.transform);
            newSpider.transform.position = new Vector3(spider.Pos.x + posIndex * _tileSizeX, spider.Pos.y, _spiderZ);
            newSpider.transform.localScale = spider.Scale;
            newSpider.transform.localRotation = spider.Rotation;
            newSpider.GetComponent<SpiderClass>().SwingingSpider = spider.bSwinging;
        }
    }

    private void SetWebs(GameObject webs, WebPool.WebType[] webList, int posIndex)
    {
        if (webList == null) { return; }
        foreach (WebPool.WebType web in webList)
        {
            GameObject newWeb = (GameObject)Instantiate(Resources.Load("Obstacles/Web"), webs.transform);
            newWeb.transform.position = new Vector3(web.Pos.x + posIndex * _tileSizeX, web.Pos.y, _webZ);
            newWeb.transform.localScale = web.Scale;
            newWeb.transform.localRotation = web.Rotation;
            newWeb.GetComponent<WebClass>().SpecialWeb = web.bSpecialWeb;
        }
    }

    private void SetTriggers(GameObject triggers, TriggerHandler.TriggerType[] triggerList, int posIndex)
    {
        if (triggerList == null) { return; }
        foreach (TriggerHandler.TriggerType trigger in triggerList)
        {
            GameObject newTrigger = (GameObject)Instantiate(Resources.Load("Interactables/Trigger"), triggers.transform);
            newTrigger.transform.position = new Vector3(trigger.Pos.x + posIndex * _tileSizeX, trigger.Pos.y, _triggerZ);
            newTrigger.transform.localScale = trigger.Scale;
            newTrigger.transform.localRotation = trigger.Rotation;
            newTrigger.GetComponent<TriggerClass>().EventID = trigger.EventID;
            newTrigger.GetComponent<TriggerClass>().EventType = trigger.EventType;
        }
    }
}
