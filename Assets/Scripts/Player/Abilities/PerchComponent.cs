using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerchComponent : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _body;
    private GameHandler _gameHandler;
    private Animator _anim;

    private const float PerchSwitchTime = 0.5f;
    private float _timeSincePerchSwitch;

    private enum PerchState
    {
        Unperched,
        PerchedTop,
        PerchedBottom
    }
    private PerchState _state;


	private void Start ()
	{
	    _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	    _body = _player.GetComponent<Rigidbody2D>();
	    _gameHandler = FindObjectOfType<GameHandler>();
	    _anim = _player.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	private void Update ()
	{
	    _timeSincePerchSwitch += Time.deltaTime;
	}

    public void Perch(string objName)
    {
        if (!PerchSwitchPossible()) return;
        _body.velocity = Vector2.zero;
        if (objName.Contains("Top"))
        {
            _state = PerchState.PerchedTop;
        }
        else if (objName.Contains("Bottom") || _player.transform.position.y < 0f)
        {
            _state = PerchState.PerchedBottom;
        }
        else
        {
            _state = PerchState.PerchedTop;
        }
        _body.isKinematic = true;
        _gameHandler.UpdateGameSpeed(0f);

        SetPerchGraphics();
    }

    private void SetPerchGraphics()
    {
        if (_state == PerchState.PerchedTop)
        {
            _player.transform.localScale = new Vector3(_player.transform.localScale.x, -_player.transform.localScale.y, 1f);
        }
        _anim.Play("Perch", 0, 0f);
    }

    public void Unperch()
    {
        if (!PerchSwitchPossible()) return;
        _player.GetComponent<Rigidbody2D>().isKinematic = false;

        if (_state == PerchState.PerchedTop)
        {
            _player.transform.localScale = new Vector3(_player.transform.localScale.x, -_player.transform.localScale.y, 1f);
        }
        else
        {
            _player.ActivateJump();
        }

        _gameHandler.UpdateGameSpeed(1);
        _state = PerchState.Unperched;
    }

    private bool PerchSwitchPossible()
    {
        if (_timeSincePerchSwitch < PerchSwitchTime) return false;
        _timeSincePerchSwitch = 0f;
        _player.SwitchPerchState();
        return true;
    }
}
