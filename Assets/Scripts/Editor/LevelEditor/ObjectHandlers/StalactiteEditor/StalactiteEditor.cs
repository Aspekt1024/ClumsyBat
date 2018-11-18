using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ClumsyBat.Objects;

public class StalactiteEditor
{
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
        const float AnimTime = 0.2845f; // impirical calculation
        const float ClumsySpeed = 6f;
        float Dist = stalScript.TriggerPosX - stalTf.localScale.x * 2 / 5f;
        float TimeToReachDest = (Dist / ClumsySpeed) - AnimTime;
        float DistanceToFall = StalDropComponent.FallDistance * Mathf.Pow((TimeToReachDest / StalDropComponent.FallDuration), 2);   // TODO put this formula somewhere else.. static? should reference the same code the in-game control does
        Vector2 telePos = new Vector3(stalTf.position.x, stalTf.position.y - DistanceToFall);

        Handles.color = Color.white;
        Color teleCol = new Color(0.9f, 0.3f, 0.3f);
        Handles.DrawSolidRectangleWithOutline(new Rect(telePos.x - 0.4f, telePos.y - stalTf.localScale.y * 2.6f, 1f, stalTf.localScale.y * 2.6f), new Color(teleCol.r, teleCol.g, teleCol.b, 0.3f), teleCol);
    }
}