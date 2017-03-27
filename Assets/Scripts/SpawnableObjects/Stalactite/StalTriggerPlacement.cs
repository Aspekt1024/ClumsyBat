using UnityEngine;
using System.Collections;

public class StalTriggerPlacement : MonoBehaviour {

    Stalactite Stal = null;
    Transform Body = null;
    Transform Trigger = null;
    SpriteRenderer TelegraphImage = null;

    private float TriggerZ;
    private const float AnimTime = 0.291667f;
    private const float AnimationTime = AnimTime * (6 / 7);
    private const float ClumsySpeed = 4f;

    void Awake()
    {
        TriggerZ = Toolbox.Instance.ZLayers["Trigger"];
        GetScriptComponents();
    }

    void Start()
    {
        PositionTelegraph();
    }
    
	void Update ()
    {
        TelegraphImage.enabled = Stal.DropEnabled;
        if (!Stal.DropEnabled) { return; }
        
        PositionTrigger();
	}

    private void PositionTrigger()
    {
        float DistanceToFall = Body.position.y - transform.position.y + transform.localScale.y / 2f;
        float TimeToReachDest = StalDropComponent.FallDuration * Mathf.Sqrt(DistanceToFall / StalDropComponent.FallDistance);

        float Dist = (AnimationTime + TimeToReachDest) * ClumsySpeed;

        float XPos = Body.position.x - Body.localScale.x * 1/3f - Dist + Trigger.localScale.x / 2f;
        if (float.IsNaN(XPos)) { XPos = 0f; }
        Trigger.position = new Vector3(XPos, Trigger.position.y, Trigger.position.z);
    }

    private void PositionTelegraph()
    {
        float Dist = Body.position.x - Body.localScale.x * 1/3f - (Trigger.position.x - Trigger.localScale.x / 2f);
        float TimeToReachDest = (Dist / ClumsySpeed) - AnimationTime;
        float DistanceToFall = StalDropComponent.FallDistance * Mathf.Pow((TimeToReachDest / StalDropComponent.FallDuration), 2);
        transform.position = new Vector3(transform.position.x, Body.position.y - DistanceToFall + transform.localScale.y / 2f, TriggerZ);
    }

    private void GetScriptComponents()
    {
        Stal = GetComponent<Stalactite>();
        TelegraphImage = GetComponent<SpriteRenderer>();
        // TODO fix this - remove or replace as required
        Debug.Log("dan, fix this if you're using it");
        foreach (Transform TF in Stal.GetComponent<Transform>())
        {
            if (TF.name == "StalObject")
            {
                Body = TF;
            }
            else if (TF.name == "StalTrigger")
            {
                Trigger = TF;
            }
        }
    }
}
