using UnityEngine;
using System.Collections.Generic;

public class StalactiteEditor {

    private Transform StalParent = null;
    //private float TriggerZ;

    Transform Stal = null;
    Stalactite StalScript = null;
    Transform StalBody = null;
    SpriteRenderer StalTrigger = null;

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
        }
        else
        {
            StalTrigger.enabled = false;
        }
    }

    public void SetZLayers(float TriggerZLayer)
    {
        //TriggerZ = TriggerZLayer;
    }
}
