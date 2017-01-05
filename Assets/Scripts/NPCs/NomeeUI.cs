using UnityEngine;
using System.Collections;

public class NomeeUI : MonoBehaviour {

    Nomee NomeeControl;

    public void TalkingNormalPressed()
    {
        NomeeControl.TalkingNormalAnim();
    }
    public void TalkingBoredPressed()
    {
        NomeeControl.TalkingBoredAnim();
    }
    public void StaticPressed()
    {
        NomeeControl.StaticAnim();
    }

    void Start ()
    {
        NomeeControl = FindObjectOfType<Nomee>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
