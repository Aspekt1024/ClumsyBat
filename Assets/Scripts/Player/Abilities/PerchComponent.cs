using ClumsyBat;
using ClumsyBat.Players;
using System.Collections;
using UnityEngine;

public class PerchComponent : MonoBehaviour
{
    public bool bJumpOnTouchRelease;

    private Player player;
    private Rigidbody2D _body;
    private Transform _lantern;
    private Rigidbody2D _lanternBody;
    private GameObject rubble;
    
    private const float PerchSwitchTime = 0.38f;    // Once unperched from bottom, can't re-perch immediately
    private float _timeSinceUnperch;

    private bool canPerch;

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
        player = FindObjectOfType<Player>();
	    _body = player.Model.GetComponent<Rigidbody2D>();
        _lantern = player.Lantern.transform;
        _lanternBody = _lantern.GetComponent<Rigidbody2D>();
        rubble = Resources.Load<GameObject>("Effects/SmallRubbleEffect");
        canPerch = true;
    }

    private void Update()
    {
        _timeSinceUnperch += Time.deltaTime;
    }

    public void Enable()
    {
        canPerch = true;
    }

    public void Disable()
    {
        canPerch = false;
    }

    public bool TryPerch(Collision2D collision, bool touchHeld)
    {
        if (!canPerch) return false;
        if (player.State.IsShielded || player.State.IsPerched || !player.State.IsNormal) return false;

        if (collision.contacts[0].point.y > player.Model.transform.position.y)
        {
            if (!touchHeld) return false;

            _state = PerchState.PerchedTop;
        }
        else
        {
            if (!PerchPossible()) return false;
            if (touchHeld)
            {
                bJumpOnTouchRelease = true;
            }
            _state = PerchState.PerchedBottom;
        }

        player.State.SetState(PlayerState.States.Perched, true);
        _body.velocity = Vector2.zero;
        _body.constraints = RigidbodyConstraints2D.FreezeAll;

        SetPerchGraphics();
        return true;
    }

    private void SetPerchGraphics()
    {
        if (_state == PerchState.PerchedTop)
        {
            StartCoroutine(MoveLantern(false));
            player.Animate(ClumsyAnimator.ClumsyAnimations.Perch);
        }
        else
        {
            GameStatics.Camera.Squeeze();
            player.Animate(ClumsyAnimator.ClumsyAnimations.Land);
        }
    }

    public bool Unperch()
    {
        if (_state == PerchState.Unperched) return false;
        _timeSinceUnperch = 0f;
        bJumpOnTouchRelease = false;
        player.Model.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        RaycastHit2D hit = new RaycastHit2D();
        
        if (_state == PerchState.PerchedTop)
        {
            _state = PerchState.Transitioning;
            player.Animate(ClumsyAnimator.ClumsyAnimations.Unperch);
            StartCoroutine(Drop(0.1f));
            StartCoroutine(MoveLantern(true));
            hit = Physics2D.Raycast(player.Model.transform.position, Vector2.up, 5f, 1 << LayerMask.NameToLayer("Caves") | 1 << LayerMask.NameToLayer("CaveTop"));
        }
        else
        {
            _state = PerchState.Unperched;
            player.Model.transform.position += Vector3.up * 0.2f;
            player.State.SetState(PlayerState.States.Perched, false);
            if (player.IsFacingRight)
            {
                player.DoAction(ClumsyAbilityHandler.DirectionalActions.Jump, MovementDirections.Right);
            }
            else
            {
                player.DoAction(ClumsyAbilityHandler.DirectionalActions.Jump, MovementDirections.Left);
            }
            hit = Physics2D.Raycast(player.Model.transform.position, Vector2.down, 5f, 1 << LayerMask.NameToLayer("Caves"));
        }

        if (hit.collider != null)
        {
            GameObject rCopy = Instantiate(rubble, new Vector3(hit.point.x, hit.point.y, player.Model.transform.position.z), Quaternion.identity, hit.collider.transform);
            Destroy(rCopy, 0.5f);
        }
        return true;
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
            player.State.SetState(PlayerState.States.Perched, false);
        }
    }

    private IEnumerator MoveLantern(bool bToPlayer)
    {
        _lanternBody.isKinematic = !bToPlayer;
        _lanternBody.velocity = Vector2.zero;
        _lanternBody.angularVelocity = 0f;

        float startAngle = _lantern.localRotation.z;
        Vector3 startPosition = _lantern.position;
        Vector3 endPosition = bToPlayer ? player.Model.transform.position : player.Model.transform.position + Vector3.left * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(endPosition, Vector3.up, 2f, ~(1 << LayerMask.NameToLayer("Player")));
        if (hit.collider != null)
        {
            // 0.2f is a magic number I trial-and-errored to offset from the lanter's center
            endPosition += Vector3.up * (hit.distance - 0.2f);
        }

        const float animDuration = 0.26f;
        float animTimer = 0f;
        while (animTimer < animDuration)
        {
            if (bToPlayer) { endPosition = player.Model.transform.position - new Vector3(0.3f, 0.5f, 0f); }
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
