using ClumsyBat.Objects;
using UnityEngine;

public class SaveLevelHandler {

    private LevelContainer level;
    private LevelEditorObjectHandler objects;

    public void Load(LevelEditorObjectHandler objHandler, LevelProgressionHandler.Levels levelId)
    {
        if (levelId == LevelProgressionHandler.Levels.Unassigned) return;

        objects = objHandler;

        TextAsset levelTxt = (TextAsset)Resources.Load("LevelXML/" + levelId);
        level = LevelContainer.LoadFromText(levelTxt.text);
        SetLevelObjects();
        Object.FindObjectOfType<LevelEditor>().ScoreToBeat = level.ScoreToBeat;
    }

    public void Save(LevelEditorObjectHandler objHandler, LevelProgressionHandler.Levels levelId, int scoreToBeat)
    {
        objects = objHandler;
        level = new LevelContainer();

        level.ScoreToBeat = scoreToBeat;
        InitialiseCaveList();

        foreach (var handler in objects.ObjHandlers)
        {
            handler.StoreObjects(ref level);
        }

        string levelName = levelId + ".xml";
        const string pathName = "Assets/Resources/LevelXML";
        level.Save(string.Format("{0}/{1}", pathName, levelName));
        Debug.Log("Level data saved to " + pathName + "/" + levelName);
    }

    private void InitialiseCaveList()
    {
        CaveEditorHandler caveHandler = GetCaveHandler();
        int numSections = caveHandler.GetNumSections();
        level.Caves = new LevelContainer.CaveType[numSections];
    }

    private CaveEditorHandler GetCaveHandler()
    {
        foreach (var handler in objects.ObjHandlers)
        {
            if (handler.IsType<CaveEditorHandler>())
            {
                return (CaveEditorHandler)handler;
            }
        }
        Debug.Log("Cave Editor Handler was not found in objects list");
        return null;
    }
    
    private void SetLevelObjects()
    {
        foreach (var handler in objects.ObjHandlers)
        {
            handler.LoadObjects(level);
        }
    }
}
