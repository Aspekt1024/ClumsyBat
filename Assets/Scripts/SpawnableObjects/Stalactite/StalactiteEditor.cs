using UnityEngine;
using System.Collections.Generic;

public class StalactiteEditor {

    private Transform stalParent = null;
    private Transform stalTf = null;
    private Stalactite stalScript = null;

    public StalactiteEditor(Transform parentTf)
    {
        stalParent = parentTf;
    }

    public void ProcessStalactites()
    {
        foreach (Transform tf in stalParent)
        {
            stalTf = tf;
            stalScript = stalTf.GetComponent<Stalactite>();

            if (stalScript.DropEnabled)
            {
                ProcessTriggerView();
            }
        }
    }
    

    private void ProcessTriggerView()
    {
        GUI.DrawTexture(new Rect(0, 0, 10, 100), Resources.Load<Texture2D>("Textures/TriggerSquare"));
        
    }
}
