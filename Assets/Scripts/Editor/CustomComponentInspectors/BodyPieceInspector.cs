using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BodyPiece))]
public class BodyPieceInspector : Editor {

    private BodyPiece thisBody;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        thisBody = (BodyPiece)target;

        if (thisBody.CenterPoints)
            thisBody.PointOnThis = V3ToV2(thisBody.OtherBody.position) - V3ToV2(thisBody.transform.position) + thisBody.PointOnOther;
    }

    public void OnSceneGUI()
    {
        if (thisBody == null || thisBody.OtherBody == null) return;
        ShowPoint();
    }

    private void ShowPoint()
    {
        Handles.color = Color.blue;
        Handles.DrawSolidDisc(thisBody.OtherBody.position + thisBody.PointOnOther, Vector3.back, 0.08f);
        Handles.color = Color.white;
        Handles.DrawSolidDisc(thisBody.OtherBody.position + thisBody.PointOnOther, Vector3.back, 0.065f);
        Handles.color = new Color(0f, 0f, 1f, 0.7f);
        Handles.DrawSolidDisc(V3ToV2(thisBody.transform.position) + thisBody.PointOnThis, Vector3.back, 0.06f);

    }

    private Vector2 V3ToV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
}
