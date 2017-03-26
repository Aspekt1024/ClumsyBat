using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

// TODO kill this script and put it in LevelEditor.
public class LevelEditorActions
{
    public LevelEditorObjectHandler objectHandler;
    
    
    public void ProcessActions()
    {
        if (objectHandler == null)
        {
            objectHandler = new LevelEditorObjectHandler();
        }
        objectHandler.GUIEvent();
    }
    
}
