using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StalactiteEditor
{
    private Transform stalParent = null;
    private Transform stalTf = null;
    private Stalactite stalScript = null;
    
    private const float AnimTime = 0.2845f; // calculated in-game
    private const float ClumsySpeed = 5f;

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

    // TODO tidy this up
    private void ProcessTriggerView()
    {
        Color trigCol = new Color(0.9f, 0.3f, 0.3f);
        DrawTrigger(stalTf.position.x - stalScript.TriggerPosX, trigCol);
        Handles.DrawLine(new Vector2(stalTf.position.x, stalTf.position.y - 1f), new Vector3(stalTf.position.x - stalScript.TriggerPosX + 0.5f, 0, 0f));
        
        float Dist = stalScript.TriggerPosX - stalTf.localScale.x * 2 / 5f;
        float TimeToReachDest = (Dist / ClumsySpeed) - AnimTime;
        float DistanceToFall = StalDropComponent.FallDistance * Mathf.Pow((TimeToReachDest / StalDropComponent.FallDuration), 2);   // TODO put this formula somewhere else.. static? should reference the same code the in-game control does
        Vector2 telePos = new Vector3(stalTf.position.x, stalTf.position.y - DistanceToFall);

        trigCol = new Color(0.9f, 0.3f, 0.3f);
        Handles.DrawSolidRectangleWithOutline(new Rect(telePos.x - 0.4f, telePos.y - stalTf.localScale.y * 2.6f, 1f, stalTf.localScale.y * 2.6f), new Color(trigCol.r, trigCol.g, trigCol.b, 0.3f), trigCol);
        Handles.DrawLine(new Vector2(stalTf.position.x, stalTf.position.y - 1f), new Vector3(stalTf.position.x + 0.1f, telePos.y));
    }

    private static void DrawTrigger(float xPos, Color rCol)
    {
        float zPos = 10f;
        Vector3[] verts = new Vector3[]
        {
            new Vector3(xPos, -6f, zPos),
            new Vector3(xPos, 6f, zPos),
            new Vector3(xPos + 0.5f, 6f, zPos),
            new Vector3(xPos + 0.5f, -6f, zPos),
        };
        Handles.DrawSolidRectangleWithOutline(verts, new Color(rCol.r, rCol.g, rCol.b, 0.4f), new Color(rCol.r, rCol.g, rCol.b, 0.9f));
    }
}