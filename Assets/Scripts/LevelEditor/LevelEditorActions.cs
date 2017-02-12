using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Disclaimer: this was an early script which was intended to hold some simple information
/// Then, while still in the early days, I expanded the functionality and kept it all in one
/// bulky class. Have fun!
/// </summary>
public class LevelEditorActions : MonoBehaviour
{
    public LevelContainer Level;

    private GameObject _levelObj;
    public LevelProgressionHandler.Levels LevelId;
    public bool DebugMode;

    private readonly StalactiteEditor _stalEditControl = new StalactiteEditor();

    private Transform _caveParent;
    private Transform _mothParent;
    private Transform _stalParent;
    private Transform _shroomParent;
    private Transform _webParent;
    private Transform _spiderParent;
    private Transform _triggerParent;
    private Transform _npcParent;
    private int _numSections;
    private LevelProgressionHandler.Levels _loadedLevelNum;

    private float _tileSizeX;
    private float _caveZ;
    private float _stalZ;
    private float _shroomZ;
    private float _mothZ;
    private float _spiderZ;
    private float _webZ;
    private float _triggerZ;
    private float _npcZ;

    private void Awake()
    {
        Toolbox.Instance.Debug = DebugMode;
        if (LevelId == 0)
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
        _spiderParent = GameObject.Find("Spiders").transform;
        _triggerParent = GameObject.Find("Triggers").transform;
        _shroomParent = GameObject.Find("Mushrooms").transform;
        _npcParent = GameObject.Find("Npcs").transform;
        _webParent = GameObject.Find("Webs").transform;
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
        _npcZ = Toolbox.Instance.ZLayers["NPC"];

        _stalEditControl.SetZLayers(_triggerZ);

        _mothParent.position = new Vector3(0, 0, _mothZ);
        _spiderParent.position = new Vector3(0, 0, _spiderZ);
        _webParent.position = new Vector3(0, 0, _webZ);
        _triggerParent.position = new Vector3(0, 0, _triggerZ);
        _shroomParent.position = new Vector3(0, 0, _shroomZ);
        _npcParent.position = new Vector3(0, 0, _npcZ);
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
        StoreSpiders();
        StoreWebs();
        StoreTriggers();
        StoreNpcs();

        string levelName = LevelId + ".xml";
        const string pathName = "Assets/Resources/LevelXML";
        Level.Save(Path.Combine(pathName, levelName));
        Debug.Log("Level data saved to " + pathName + "/" + levelName);
    }

    public void TestButton()
    {
        GameData.Instance.Level = LevelId;
        Toolbox.Instance.Debug = DebugMode;
        SceneManager.LoadScene("Levels");
    }

    public void LevelUp()
    {
        LevelId++;
        while (LevelId.ToString().Contains("Boss"))
            LevelId++;
        SetLevelNum();
    }

    public void LevelDown()
    {
        LevelId--;
        while (LevelId.ToString().Contains("Boss"))
            LevelId--;
        SetLevelNum();
    }

    private void SetLevelNum()
    {
        GameObject.Find("LevelNumText").GetComponent<Text>().text = "Level: " + LevelId.ToString();
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
            Level.Caves[i].Npcs = new NPCPool.NpcType[0];
        }
    }

    private void StoreCaveIndexes()
    {
        foreach (Transform cave in _caveParent)
        {
            int index = Mathf.RoundToInt(cave.position.x / _tileSizeX);
            if (cave.name == "CaveEntrance")
            {
                Level.Caves[index].BottomIndex = Toolbox.CaveStartIndex;
                Level.Caves[index].TopIndex = Toolbox.CaveStartIndex;
            }
            else if (cave.name == "CaveExit")
            {
                Level.Caves[index].BottomIndex = Toolbox.CaveEndIndex;
                Level.Caves[index].TopIndex = Toolbox.CaveEndIndex;
            }
            else if (cave.name == "CaveGnomeEnd")
            {
                Level.Caves[index].BottomIndex = Toolbox.CaveGnomeEndIndex;
                Level.Caves[index].TopIndex = Toolbox.CaveGnomeEndIndex;
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
        var stalCounts = GetObjCounts(_stalParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Stals = new StalPool.StalType[stalCounts[i]];
        }

        int[] stalNum = new int[_numSections];
        foreach (Transform stal in _stalParent)
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
            newStal.SpawnTranform = new Spawnable.SpawnType
            {
                Pos = new Vector2(stal.position.x - _tileSizeX * index, stal.position.y),
                Scale = stalObj != null ? stalObj.localScale : Vector3.zero,
                Rotation = stalObj != null ? stalObj.localRotation : Quaternion.identity
            };
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
        var shroomCounts = GetObjCounts(_shroomParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Shrooms = new ShroomPool.ShroomType[shroomCounts[i]];
        }

        int[] shroomNum = new int[_numSections];
        foreach (Transform shroom in _shroomParent)
        {
            int index = Mathf.RoundToInt(shroom.position.x / _tileSizeX);

            ShroomPool.ShroomType newShroom = Level.Caves[index].Shrooms[shroomNum[index]];
            newShroom.SpawnTransform = ProduceSpawnTf(shroom, index);
            newShroom.SpecialEnabled = false;
            Level.Caves[index].Shrooms[shroomNum[index]] = newShroom;
            shroomNum[index]++;
        }
    }

    private void StoreMoths()
    {
        var mothCounts = GetObjCounts(_mothParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Moths = new MothPool.MothType[mothCounts[i]];
        }

        int[] mothNum = new int[_numSections];
        foreach (Transform moth in _mothParent)
        {
            int index = Mathf.RoundToInt(moth.position.x / _tileSizeX);

            MothPool.MothType newMoth = Level.Caves[index].Moths[mothNum[index]];
            newMoth.SpawnTransform = ProduceSpawnTf(moth, index);
            newMoth.Colour = moth.GetComponent<Moth>().Colour;
            newMoth.PathType = moth.GetComponent<Moth>().PathType;
            Level.Caves[index].Moths[mothNum[index]] = newMoth;
            mothNum[index]++;
        }
    }

    private void StoreSpiders()
    {
        var spiderCounts = GetObjCounts(_spiderParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Spiders = new SpiderPool.SpiderType[spiderCounts[i]];
        }

        int[] spiderNum = new int[_numSections];
        foreach (Transform spider in _spiderParent)
        {
            int index = Mathf.RoundToInt(spider.position.x / _tileSizeX);

            SpiderPool.SpiderType newSpider = Level.Caves[index].Spiders[spiderNum[index]];
            newSpider.SpawnTransform = ProduceSpawnTf(spider, index);
            newSpider.SpiderSwings = spider.GetComponent<SpiderClass>().SwingingSpider;
            Level.Caves[index].Spiders[spiderNum[index]] = newSpider;
            spiderNum[index]++;
        }
    }

    private void StoreWebs()
    {
        var webCounts = GetObjCounts(_webParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Webs = new WebPool.WebType[webCounts[i]];
        }

        int[] webNum = new int[_numSections];
        foreach (Transform web in _webParent)
        {
            int index = Mathf.RoundToInt(web.position.x / _tileSizeX);

            WebPool.WebType newWeb = Level.Caves[index].Webs[webNum[index]];
            newWeb.SpawnTransform = ProduceSpawnTf(web, index);
            newWeb.SpecialWeb = web.GetComponent<WebClass>().SpecialWeb;
            Level.Caves[index].Webs[webNum[index]] = newWeb;
            webNum[index]++;
        }
    }

    private void StoreTriggers()
    {
        var triggerCounts = GetObjCounts(_triggerParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Triggers = new TriggerHandler.TriggerType[triggerCounts[i]];
        }

        int[] triggerNum = new int[_numSections];
        foreach (Transform trigger in _triggerParent)
        {
            int index = Mathf.RoundToInt(trigger.position.x / _tileSizeX);

            TriggerHandler.TriggerType newTrigger = Level.Caves[index].Triggers[triggerNum[index]];
            newTrigger.SpawnTransform = ProduceSpawnTf(trigger, index);
            newTrigger.EventId = trigger.GetComponent<TriggerClass>().EventId;
            newTrigger.EventType = trigger.GetComponent<TriggerClass>().EventType;
            newTrigger.PausesGame = trigger.GetComponent<TriggerClass>().PausesGame;
            Level.Caves[index].Triggers[triggerNum[index]] = newTrigger;
            triggerNum[index]++;
        }
    }

    private void StoreNpcs()
    {
        var npcCounts = GetObjCounts(_npcParent);
        for (int i = 0; i < _numSections; i++)
        {
            Level.Caves[i].Npcs = new NPCPool.NpcType[npcCounts[i]];
        }
        int[] npcNum = new int[_numSections];
        foreach (Transform npc in _npcParent)
        {
            int index = Mathf.RoundToInt(npc.position.x / _tileSizeX);
            NPCPool.NpcType newNpc = Level.Caves[index].Npcs[npcNum[index]];
            newNpc.SpawnTransform = ProduceSpawnTf(npc, index);
            newNpc.Type = npc.GetComponent<NPC>().Type;
            Level.Caves[index].Npcs[npcNum[index]] = newNpc;
            npcNum[index]++;
        }
    }

    private Spawnable.SpawnType ProduceSpawnTf(Transform objTf, int index)
    {
        var spawnTf = new Spawnable.SpawnType
        {
            Pos = new Vector2(objTf.position.x - _tileSizeX * index, objTf.position.y),
            Scale = objTf.localScale,
            Rotation = objTf.localRotation
        };
        return spawnTf;
    }

    public void LoadBtn()
    {
        if (LevelId == 0) { return; }
        TextAsset levelTxt = (TextAsset)Resources.Load("LevelXML/" + LevelId);
        Level = LevelContainer.LoadFromText(levelTxt.text);
        ClearLevelObjects();
        SetLevelObjects();
        _loadedLevelNum = LevelId;
    }

    private void ClearLevelObjects()
    {
        Destroy(GameObject.Find("Stalactites"));
        Destroy(GameObject.Find("Mushrooms"));
        Destroy(GameObject.Find("Moths"));
        Destroy(GameObject.Find("Spiders"));
        Destroy(GameObject.Find("Webs"));
        Destroy(GameObject.Find("Triggers"));
        Destroy(GameObject.Find("Npcs"));
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
        _shroomParent = shrooms.transform;
        _webParent = webs.transform;
        _spiderParent = spiders.transform;
        _triggerParent = triggers.transform;
        _npcParent = new GameObject("Npcs").transform;

        _stalParent.SetParent(_levelObj.transform);
        _shroomParent.SetParent(_levelObj.transform);
        _mothParent.SetParent(_levelObj.transform);
        _spiderParent.SetParent(_levelObj.transform);
        _webParent.SetParent(_levelObj.transform);
        _triggerParent.SetParent(_levelObj.transform);
        _npcParent.SetParent(_levelObj.transform);

        //GameObject Clumsy = (GameObject)Instantiate(Resources.Load("ClumsyLevelEditor"), _levelObj.transform);
        //Clumsy.name = "Clumsy";
        //Clumsy.transform.position = new Vector3(Level.Clumsy.Pos.x, Level.Clumsy.Pos.y, _clumsyZ);
        //Clumsy.transform.localRotation = Level.Clumsy.Rotation;
        //Clumsy.transform.localScale = Level.Clumsy.Scale;

        for (int i = 0; i < Level.Caves.Length; i++)
        {
            LevelContainer.CaveType cave = Level.Caves[i];
            GameObject caveTop;
            if (cave.TopIndex == Toolbox.CaveStartIndex)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveEntrance"), caves.transform);
                caveTop.name = "CaveEntrance";
            }
            else if (cave.TopIndex == Toolbox.CaveEndIndex)
            {
                caveTop = (GameObject)Instantiate(Resources.Load("Caves/CaveExit"), caves.transform);
                caveTop.name = "CaveExit";
            }
            else if (cave.TopIndex == Toolbox.CaveGnomeEndIndex)
            {
                caveTop = (GameObject) Instantiate(Resources.Load("Caves/CaveGnomeEnd"), caves.transform);
                caveTop.name = "CaveGnomeEnd";
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
            SetNpcs(cave.Npcs, i);
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

            newStal.transform.position = new Vector3(stal.SpawnTranform.Pos.x + posIndex * _tileSizeX, stal.SpawnTranform.Pos.y, _stalZ);
            if (stalObj != null)
            {
                stalObj.localScale = stal.SpawnTranform.Scale;
                stalObj.localRotation = stal.SpawnTranform.Rotation;
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
            Spawnable.SpawnType spawnTf = shroom.SpawnTransform;
            spawnTf.Pos += new Vector2(posIndex * _tileSizeX, 0f);
            newShroom.GetComponent<Mushroom>().SetTransform(newShroom.transform, spawnTf);
        }
    }

    private void SetMoths(GameObject moths, MothPool.MothType[] mothList, int posIndex)
    {
        foreach (MothPool.MothType moth in mothList)
        {
            GameObject newMoth = (GameObject)Instantiate(Resources.Load("Collectibles/Moth"), moths.transform);
            Spawnable.SpawnType spawnTf = moth.SpawnTransform;
            spawnTf.Pos += new Vector2(posIndex * _tileSizeX, 0f);
            newMoth.GetComponent<Moth>().SetTransform(newMoth.transform, spawnTf);
            newMoth.GetComponent<Moth>().Colour = moth.Colour;
            newMoth.GetComponent<Moth>().PathType = moth.PathType;
        }
    }

    private void SetSpiders(GameObject spiders, SpiderPool.SpiderType[] spiderList, int posIndex)
    {
        if (spiderList == null) { return; }
        foreach (SpiderPool.SpiderType spider in spiderList)
        {
            GameObject newSpider = (GameObject)Instantiate(Resources.Load("Obstacles/Spider"), spiders.transform);
            Spawnable.SpawnType spawnTf = spider.SpawnTransform;
            spawnTf.Pos += new Vector2(posIndex * _tileSizeX, 0f);
            newSpider.GetComponent<SpiderClass>().SetTransform(newSpider.transform, spawnTf);
            newSpider.GetComponent<SpiderClass>().SwingingSpider = spider.SpiderSwings;
        }
    }

    private void SetWebs(GameObject webs, WebPool.WebType[] webList, int posIndex)
    {
        if (webList == null) { return; }
        foreach (WebPool.WebType web in webList)
        {
            GameObject newWeb = (GameObject)Instantiate(Resources.Load("Obstacles/Web"), webs.transform);
            Spawnable.SpawnType spawnTf = web.SpawnTransform;
            spawnTf.Pos += new Vector2(posIndex * _tileSizeX, 0f);
            newWeb.GetComponent<WebClass>().SetTransform(newWeb.transform, spawnTf);
            newWeb.GetComponent<WebClass>().SpecialWeb = web.SpecialWeb;
        }
    }

    private void SetTriggers(GameObject triggers, TriggerHandler.TriggerType[] triggerList, int posIndex)
    {
        if (triggerList == null) { return; }
        foreach (TriggerHandler.TriggerType trigger in triggerList)
        {
            GameObject newTrigger = (GameObject)Instantiate(Resources.Load("Interactables/Trigger"), triggers.transform);
            Spawnable.SpawnType spawnTf = trigger.SpawnTransform;
            spawnTf.Pos += new Vector2(posIndex * _tileSizeX, 0f);
            newTrigger.GetComponent<TriggerClass>().SetTransform(newTrigger.transform, spawnTf);
            newTrigger.GetComponent<TriggerClass>().EventId = trigger.EventId;
            newTrigger.GetComponent<TriggerClass>().EventType = trigger.EventType;
            newTrigger.GetComponent<TriggerClass>().PausesGame = trigger.PausesGame;
        }
    }

    private void SetNpcs(NPCPool.NpcType[] npcList, int posIndex)
    {
        if (npcList == null) return;
        foreach (NPCPool.NpcType npc in npcList)
        {
            GameObject newNpc = (GameObject) Instantiate(Resources.Load("NPCs/Nomee"), _npcParent);
            Spawnable.SpawnType spawnTf = npc.SpawnTransform;
            spawnTf.Pos += new Vector2(posIndex * _tileSizeX, 0f);
            newNpc.GetComponent<NPC>().SetTransform(newNpc.transform, spawnTf);
        }
    }
}
