using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenCrystalBall : MonoBehaviour {

    private Rigidbody2D[] pieces;
    private const int numPieces = 18;

	private void Start () {
        pieces = new Rigidbody2D[numPieces];
        int i = 0;
        foreach (Transform tf in transform)
        {
            pieces[i] = tf.GetComponent<Rigidbody2D>();
            pieces[i].isKinematic = true;
            i++;
        }
        gameObject.SetActive(false);
    }

    public void Shatter()
    {
        for (int i = 0; i < numPieces; i++)
        {
            float xVel = 0f;
            if (transform.position.x > Toolbox.PlayerCam.transform.position.x + 1f)
                xVel = Random.Range(-2f, 10f);
            else if (transform.position.x < Toolbox.PlayerCam.transform.position.x - 1f)
                xVel = Random.Range(-10f, 2f);
            else
                xVel = Random.Range(-4f, 4f);

            pieces[i].isKinematic = false;
            pieces[i].velocity = new Vector3(xVel, Random.Range(-4, 10f), 0f);
        }
    }
}
