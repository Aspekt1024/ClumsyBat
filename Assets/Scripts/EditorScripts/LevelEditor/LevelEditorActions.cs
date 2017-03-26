using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Disclaimer: this was an early script which was intended to hold some simple information
/// Then, while still in the early days, I expanded the functionality and kept it all in one
/// bulky class. Have fun!
/// </summary>
public class LevelEditorActions
{
    public LevelContainer Level;// TODO not sure this is needed
    public LevelEditorObjectHandler objectHandler;

    private readonly StalactiteEditor _stalEditControl = new StalactiteEditor();

    private int _numSections;
    private LevelProgressionHandler.Levels _loadedLevelNum;
    
    private void Start()
    {
        Level = new LevelContainer();
        //SetLevelNum();
        //LoadBtn();
    }

    public void ProcessActions()
    {
        if (objectHandler == null)
        {
            objectHandler = new LevelEditorObjectHandler();
        }
        objectHandler.GUIEvent();
        //SetLevelStats();
    }

    private void SetLevelStats()
    {
        // TODO set text somewhere else
        //GetNumSections();
        int sections = _numSections;
        const float distOffset = 0f;
        float distance = sections * 19.2f - distOffset;
        float timeTaken = distance / 4f;
        GameObject.Find("LevelNumText2").GetComponent<Text>().text = "Level: " + _loadedLevelNum;
        GameObject.Find("NumSectionsText").GetComponent<Text>().text = "Sections: " + sections;
        GameObject.Find("DistanceText").GetComponent<Text>().text = "Distance: " + distance + "m";
        GameObject.Find("TimeText").GetComponent<Text>().text = "Time to complete: " + timeTaken + " sec";
    }

    /*

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
    }*/
}
