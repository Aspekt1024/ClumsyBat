using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretPath : MonoBehaviour {

    public LevelProgressionHandler.Levels LevelToUnlock;
    public bool RequiresBlueMoth;
    public bool HasBlock;

    private bool isActivated;

    private Player player;

    private void Start ()
    {
        RequiresBlueMoth = false;
        if (RequiresBlueMoth)
        {
            isActivated = false;
            // TODO change something nearby to show blue moth is required 
        }
        else
        {
            isActivated = true;
        }

	}
	
	private void Update ()
    {
		
	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
        {
            player = other.gameObject.GetComponent<Player>();
            if (isActivated)
                StartCoroutine(OpenSecretPath());
        }
    }
    
    private IEnumerator OpenSecretPath()
    {
        float animTimer = 0f;
        const float animDuration = 3f;
        const float distToMove = 4.5f;
        
        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                Vector3 dist = Vector3.down * distToMove * Time.deltaTime / animDuration;
                transform.position += dist;

                // TODO shake & sounds & all that
                
                if (player.IsPerched())
                {
                    // Ensure player is perched on this object before lowering/raising
                    RaycastHit2D hit = Physics2D.Raycast(new Vector3(player.transform.position.x, 0, 0), Vector3.down, 10, 1 << LayerMask.NameToLayer("Caves"));
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        player.transform.position += dist;
                    }
                }
            }
            yield return null;
        }
    }
}
