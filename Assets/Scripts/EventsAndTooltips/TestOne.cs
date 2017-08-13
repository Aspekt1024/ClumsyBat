using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestOne : MonoBehaviour {

    private UnityAction someListener;

    private void Awake()
    {
        someListener = new UnityAction(SomeFunc);
    }


    private void OnEnable()
    {
        EventManager.StartListening("test", someListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening("test", someListener);
    }

    private void SomeFunc()
    {
        Debug.Log("yay!");
    }
}
