using UnityEngine;
using System.Collections;

public class Nomee : MonoBehaviour {

    Animator NomeeAnimator = null;

    void Awake()
    {
        NomeeAnimator = GetComponent<Animator>();
    }

	void Start ()
    {
        NomeeAnimator.enabled = true;
	}
	
	void Update ()
    {
	}

    public void TalkingNormalAnim()
    {
        NomeeAnimator.Play("TalkingStandard", 0, 0f);
    }
    public void TalkingBoredAnim()
    {
        NomeeAnimator.Play("TalkingBored", 0, 0f);
    }
    public void StaticAnim()
    {
        NomeeAnimator.Play("Static", 0, 0f);
    }
}
