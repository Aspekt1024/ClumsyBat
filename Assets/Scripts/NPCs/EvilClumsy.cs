using UnityEngine;
using System.Collections;

public class EvilClumsy : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _body;

    private enum BossStates
    {
        Idle,
        Jumping,
        Dead
    }

    private BossStates _state;

	void Start () {
	    _state = BossStates.Idle;
	    _body = GetComponent<Rigidbody2D>();
	    _player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (_state == BossStates.Jumping)
	    {
	        if (_body.velocity.y < 0)
	        {
	            _state = BossStates.Idle;
	        }
	    }
        if (_state != BossStates.Idle) { return; }
	    if (transform.position.y < _player.transform.position.y)
	    {
	        _body.velocity = Vector2.zero;
	        _body.AddForce(new Vector2(0f, 450f));
            _state = BossStates.Jumping;
	    }
	
	}
}
