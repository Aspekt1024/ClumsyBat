using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClumsyButton : MonoBehaviour {

    private Transform clumsy;
    private ClumsyMainMenu clumsyScript;

    public void ClumsyClick()
    {
        if (transform.position.x > Toolbox.TileSizeX) return;
        clumsyScript.ClumsyTapped();
    }

    private void Start()
    {
        clumsy = GameObject.FindGameObjectWithTag("Player").transform;
        clumsyScript = clumsy.GetComponent<ClumsyMainMenu>();
    }

    private void Update()
    {
        transform.position = new Vector3(clumsy.position.x, clumsy.position.y, transform.position.z);
    }
}
