using UnityEngine;

public class ClumsyButton : MonoBehaviour {

    private Transform clumsy;

    public void ClumsyClick()
    {
        if (transform.position.x > Toolbox.TileSizeX) return;
        //clumsyScript.ClumsyTapped();
    }

    private void Start()
    {
        clumsy = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        transform.position = new Vector3(clumsy.position.x, clumsy.position.y, transform.position.z);
    }
}
