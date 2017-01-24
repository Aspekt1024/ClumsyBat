using UnityEngine;

public class PerchComponent : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _body;
    private GameHandler _gameHandler;
    private Animator _anim;

    private const float PerchSwitchTime = 0.15f;    // This is to avoid double taps and double collisions
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
            FlipPlayer();
        }
        _anim.Play("Perch", 0, 0f);
    }

    public void Unperch()
    {
        if (!PerchSwitchPossible()) return;
        _player.GetComponent<Rigidbody2D>().isKinematic = false;

        if (_state == PerchState.PerchedTop)
        {
            _state = PerchState.Unperched;
            FlipPlayer();
        }
        else
        {
            _state = PerchState.Unperched;
            _player.transform.position += Vector3.up * 0.2f;
            _player.ActivateJump();
        }

        _gameHandler.UpdateGameSpeed(1);
    }

    private bool PerchSwitchPossible()
    {
        if (_timeSincePerchSwitch < PerchSwitchTime) return false;
        _timeSincePerchSwitch = 0f;
        _player.SwitchPerchState();
        return true;
    }

    private void FlipPlayer()
    {
        _player.transform.localScale = new Vector3(_player.transform.localScale.x, -_player.transform.localScale.y, 1f);
        Transform lantern = _player.Lantern.transform;
        Rigidbody2D lanternBody = lantern.GetComponent<Rigidbody2D>();
        lanternBody.isKinematic = !lanternBody.isKinematic;
        if (!lanternBody.isKinematic) return;

        lanternBody.velocity = Vector2.zero;
        lanternBody.angularVelocity = 0f;
        lantern.rotation = Quaternion.identity;

        lantern.position = _player.transform.position + Vector3.left * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(lantern.position, Vector3.up, 2f, ~(1 << LayerMask.NameToLayer("Clumsy")));
        if (hit.collider != null)
            lantern.position += Vector3.up * (hit.distance - 0.2f); // 0.2f is a magic number i trial-and-errored
    }
}
