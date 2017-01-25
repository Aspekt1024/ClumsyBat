using System.Collections;
using UnityEngine;

public class PerchComponent : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _body;
    private GameHandler _gameHandler;
    private Animator _anim;
    private Transform _lantern;
    private Rigidbody2D _lanternBody;

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
        _lantern = _player.Lantern.transform;
        _lanternBody = _lantern.GetComponent<Rigidbody2D>();
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
            _player.UnperchBottom();
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
        if (_state == PerchState.Unperched)
        {
            StartCoroutine("MoveLantern", true);
        }
        else
        {
            StartCoroutine("MoveLantern", false);
        }
        _player.transform.localScale = new Vector3(_player.transform.localScale.x, -_player.transform.localScale.y, 1f);

    }

    private IEnumerator MoveLantern(bool bToPlayer)
    {
        _lanternBody.isKinematic = !bToPlayer;
        _lanternBody.velocity = Vector2.zero;
        _lanternBody.angularVelocity = 0f;

        float startAngle = _lantern.localRotation.z;
        Vector3 startPosition = _lantern.position;
        Vector3 endPosition = bToPlayer ? _player.transform.position : _player.transform.position + Vector3.left * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(endPosition, Vector3.up, 2f, ~(1 << LayerMask.NameToLayer("Clumsy")));
        if (hit.collider != null)
            // 0.2f is a magic number i trial-and-errored to offset from the lanter's center
            endPosition += Vector3.up * (hit.distance - 0.2f);

        const float animDuration = 0.26f;
        float animTimer = 0f;
        while (animTimer < animDuration)
        {
            if (bToPlayer) { endPosition = _player.transform.position - new Vector3(0.3f, 0.5f, 0f); }
            animTimer += Time.deltaTime;
            float ratio = animTimer / animDuration;
            float posX = startPosition.x - (startPosition.x - endPosition.x) * ratio;
            float posY = startPosition.y - (startPosition.y - endPosition.y) * ratio;
            float angle = startAngle - (startAngle + 359.9f) * ratio;
            _lantern.eulerAngles = new Vector3(0f, 0f, angle);
            _lantern.position = new Vector3(posX, posY, _lantern.position.z);
            yield return null;
        }
    }
}
