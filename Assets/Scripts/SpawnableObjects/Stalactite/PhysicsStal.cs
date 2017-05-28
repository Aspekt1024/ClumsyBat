using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsStal : MonoBehaviour {

    private const string BrokenStalPath = "Obstacles/Stalactite/BrokenStal";
    private GameObject StalPrefabBroken;
    private GameObject StalObject;
    private Stalactite stalScript;

    private bool isBroken;
    
    private void Awake()
    {
        StalPrefabBroken = Instantiate(Resources.Load<GameObject>(BrokenStalPath), transform);
        StalPrefabBroken.SetActive(false);
        stalScript = GetComponent<Stalactite>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!stalScript.IsBreakable() || isBroken) return;

        if (other.tag == "Boss")
        {
            Break();
        }
    }

    private void Break()
    {
        isBroken = true;
        
        StalPrefabBroken.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().Sleep();
        StartCoroutine(DissolveBrokenStalactite());
    }

    private IEnumerator DissolveBrokenStalactite()
    {
        float timer = 0;
        const float timeBeforeDestroy = 4f;

        while (timer < timeBeforeDestroy)
        {
            if (!Toolbox.Instance.GamePaused)
                timer += Time.deltaTime;
            yield return null;
        }

        transform.position = Toolbox.Instance.HoldingArea;
        Destroy(StalPrefabBroken);
        StalPrefabBroken = Instantiate(Resources.Load<GameObject>(BrokenStalPath), transform);
    }
}
