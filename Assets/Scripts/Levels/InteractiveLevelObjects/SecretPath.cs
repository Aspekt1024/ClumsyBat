using ClumsyBat.Players;
using System.Collections;
using ClumsyBat;
using UnityEngine;

public class SecretPath : MonoBehaviour {

    public LevelProgressionHandler.Levels LevelToUnlock;
    public bool RequiresBlueMoth;
    public bool HasBlock;

    private bool isActivated;

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
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && isActivated)
        {
            StartCoroutine(OpenSecretPath());
        }
    }

    private IEnumerator OpenSecretPath()
    {
        float animTimer = 0f;
        const float animDuration = 3f;
        const float distToMove = 4.5f;

        var player = GameStatics.Player.Clumsy;
        
        while (animTimer < animDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                animTimer += Time.deltaTime;
                var dist = Vector3.down * distToMove * Time.deltaTime / animDuration;
                transform.position += dist;

                // TODO shake & sounds & all that
                
                if (player.State.IsPerched)
                {
                    // Ensure player is perched on this object before lowering/raising
                    RaycastHit2D hit = Physics2D.Raycast(new Vector3(player.model.position.x, 0, 0), Vector3.down, 10, 1 << LayerMask.NameToLayer("Caves"));
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        player.model.position += dist;
                    }
                }
            }
            yield return null;
        }
    }
}
