using System.Collections;
using UnityEngine;

public class PerchComponent : MonoBehaviour
{
    public bool bJumpOnTouchRelease;

    private Player _player;
    private Rigidbody2D _body;
    private GameHandler _gameHandler;
    private Transform _lantern;
    private Rigidbody2D _lanternBody;
    private GameObject rubble;


    private const float PerchSwitchTime = 0.38f;    // Once unperched from bottom, can't re-perch immediately
    private float _timeSinceUnperch;

    private enum PerchState
    {
        Unperched,
        Transitioning,
        PerchedTop,
        PerchedBottom
    }
    private PerchState _state;


	private void Start ()
	{
	    _player = Toolbox.Player;
	    _body = _player.GetComponent<Rigidbody2D>();
	    _gameHandler = FindObjectOfType<GameHandler>();
        _lantern = _player.Lantern.transform;
        _lanternBody = _lantern.GetComponent<Rigidbody2D>();
        rubble = Resources.Load<GameObject>("Effects/SmallRubbleEffect");
    }

    private void Update()
    {
        _timeSinceUnperch += Time.deltaTime;
    }

    public void Perch(string objName, bool touchHeld)
    {
        if (objName.Contains("Top") && touchHeld)
        {
            _state = PerchState.PerchedTop;
        }
        else if (objName.Contains("Bottom") || _player.transform.position.y < 0f)
        {
            if (!PerchPossible()) return;
            if (touchHeld)
            {
                bJumpOnTouchRelease = true;
            }
            _state = PerchState.PerchedBottom;
        }
        else if (touchHeld)
        {
            _state = PerchState.PerchedTop;
        }
        else
        {
            return;
        }

        _player.SwitchPerchState();
        _body.velocity = Vector2.zero;
        _body.isKinematic = true;
        _gameHandler.UpdateGameSpeed(0f);

        SetPerchGraphics();
    }

    private void SetPerchGraphics()
    {
        if (_state == PerchState.PerchedTop)
        {
            StartCoroutine("MoveLantern", false);
            _player.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Perch);
        }
        else
        {
            _player.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Land);
        }
    }

    public void Unperch()
    {
        if (_state == PerchState.Unperched) return;
        _timeSinceUnperch = 0f;
        bJumpOnTouchRelease = false;
        _player.GetComponent<Rigidbody2D>().isKinematic = false;

        RaycastHit2D hit = new RaycastHit2D();

        if (_state == PerchState.PerchedTop)
        {
            _state = PerchState.Transitioning;
            _player.Anim.PlayAnimation(ClumsyAnimator.ClumsyAnimations.Unperch);
            StartCoroutine(Drop(0.1f));
            StartCoroutine("MoveLantern", true);
            hit = Physics2D.Raycast(_player.transform.position, Vector2.up, 5f, 1 << LayerMask.NameToLayer("Caves"));
        }
        else
        {
            _state = PerchState.Unperched;
            _player.transform.position += Vector3.up * 0.2f;
            _player.SwitchPerchState();
            _player.UnperchBottom();
            _gameHandler.UpdateGameSpeed(1);
            hit = Physics2D.Raycast(_player.transform.position, Vector2.down, 5f, 1 << LayerMask.NameToLayer("Caves"));
        }

        if (hit.collider != null)
        {
            GameObject rCopy = Instantiate(rubble, hit.point, Quaternion.identity, hit.collider.transform);
            Destroy(rCopy, 0.5f);
        }

    }

    private bool PerchPossible()
    {
        return !(_timeSinceUnperch < PerchSwitchTime);
    }

    // Stops Clumsy moving immediately after dismounting
    private IEnumerator Drop(float dropDuration)
    {
        float dropTimer = 0f;
        while (dropTimer < dropDuration)
        {
            if (!Toolbox.Instance.GamePaused)
                dropTimer += Time.deltaTime;

            yield return null;
        }

        if (_state == PerchState.Transitioning)
        {
            _state = PerchState.Unperched;
            _player.SwitchPerchState();
            _gameHandler.UpdateGameSpeed(1);
        }
    }

    private IEnumerator MoveLantern(bool bToPlayer)
    {
        _lanternBody.isKinematic = !bToPlayer;
        _lanternBody.velocity = Vector2.zero;
        _lanternBody.angularVelocity = 0f;

        float startAngle = _lantern.localRotation.z;
        Vector3 startPosition = _lantern.position;
        Vector3 endPosition = bToPlayer ? _player.transform.position : _player.transform.position + Vector3.left * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(endPosition, Vector3.up, 2f, ~(1 << LayerMask.NameToLayer("Player")));
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

    public bool IsPerchedOnTop() { return _state == PerchState.PerchedTop; }
    public bool IsPerchedOnBottom() { return _state == PerchState.PerchedBottom; }
}
