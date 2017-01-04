using UnityEngine;
using System.Collections.Generic;

public class StalactiteEditor {
    
    public StalactiteEditor()
    {
        SetupTriggerDict();
    }

    private Transform StalParent = null;
    private float TriggerZ;

    Transform Stal = null;
    Stalactite StalScript = null;
    Transform StalBody = null;
    SpriteRenderer StalTrigger = null;

    private Dictionary<Stalactite.FallType, float> TriggerDict = new Dictionary<Stalactite.FallType, float>();

    public void ProcessStalactites(Transform _StalParent)
    {
        if (_StalParent == null) { return; }
        StalParent = _StalParent;

        foreach (Transform StalChild in StalParent)
        {
            Stal = StalChild;
            GetStalComponents();
            
            LockSpritePosition();
            DelegateSpriteRotationAndScale();
            ProcessTriggerView();
        }
    }

    private void GetStalComponents()
    {
        StalBody = null;
        StalTrigger = null;
        StalScript = Stal.GetComponent<Stalactite>();
        foreach (Transform StalChild in Stal)
        {
            if (StalChild.name == "StalObject")
            {
                StalBody = StalChild;
            }
            else if (StalChild.name == "StalTrigger")
            {
                StalTrigger = StalChild.GetComponent<SpriteRenderer>();
            }
        }
    }

    private void LockSpritePosition()
    {
        if (Stal.position != StalBody.position)
        {
            StalBody.position = Stal.position;
        }
    }

    private void DelegateSpriteRotationAndScale()
    {
        if (Stal.localRotation.eulerAngles != Vector3.zero)
        {
            StalBody.localRotation = Stal.localRotation;
            if (StalScript.Flipped) { StalBody.Rotate(Vector3.forward * 180f); }
            Stal.localRotation = new Quaternion();
        }

        if (Stal.localScale != Vector3.one)
        {
            StalBody.localScale = Stal.localScale;
            Stal.localScale = Vector3.one;
        }
    }

    private void ProcessTriggerView()
    {
        if (StalScript.UnstableStalactite)
        {
            StalTrigger.enabled = true;
            if (StalScript.FallPreset != Stalactite.FallType.Custom)
            {
                float TriggerXOffset = TriggerDict[StalScript.FallPreset];
                StalTrigger.transform.position = new Vector3(Stal.position.x + TriggerXOffset, StalTrigger.transform.position.y, TriggerZ);
            }
            else
            {
                StalTrigger.transform.position = new Vector3(StalTrigger.transform.position.x, StalTrigger.transform.position.y, TriggerZ);
            }
        }
        else
        {
            StalTrigger.enabled = false;
        }
    }

    private void SetupTriggerDict()
    {
        TriggerDict.Add(Stalactite.FallType.Over_Easy, -2.3f);
        TriggerDict.Add(Stalactite.FallType.Over_Hard, -2f);
        TriggerDict.Add(Stalactite.FallType.Under_Easy, 0.8f);
        TriggerDict.Add(Stalactite.FallType.Under_Hard, 0.5f);
        TriggerDict.Add(Stalactite.FallType.Under_Dash, -1f);
        TriggerDict.Add(Stalactite.FallType.PostFall, 2f);
        TriggerDict.Add(Stalactite.FallType.PreFall_Early, -2.8f);
        TriggerDict.Add(Stalactite.FallType.PreFall_VeryEarly, -5.2f);
        TriggerDict.Add(Stalactite.FallType.NoFall, 10f);
    }

    public void SetZLayers(float TriggerZLayer)
    {
        TriggerZ = TriggerZLayer;
    }
}
