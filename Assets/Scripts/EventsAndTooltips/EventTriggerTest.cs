using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerTest : MonoBehaviour {

	private void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            EventManager.TriggerEvent("test");
        }
    }
}
