using UnityEngine;
using System.Collections;

public class JumpClearance : MonoBehaviour {

    private int NumTriggers = 0;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            NumTriggers++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            NumTriggers--;
        }
    }

    public bool IsEmpty()
    {
        return (NumTriggers == 0 ? true : false);
    }
}
