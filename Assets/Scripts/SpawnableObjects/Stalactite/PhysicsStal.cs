using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsStal : MonoBehaviour {



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            foreach (var piece in gameObject.GetComponentsInChildren<Rigidbody2D>())
            {
                piece.isKinematic = false;
            }
        }
    }
}
