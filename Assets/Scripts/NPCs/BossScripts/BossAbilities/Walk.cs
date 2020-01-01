using ClumsyBat;
using System.Collections;
using UnityEngine;

using WalkOptions = WalkAction.WalkOptions;

public class Walk : BossAbility {

    private BaseAction callerAction;

    private enum WalkDirection
    {
        Left,
        Right
    }

    private WalkDirection _direction;

    public void Activate(BaseAction caller, float duration, float speed, WalkOptions option)
    {
        callerAction = caller;
        if (option == WalkOptions.Left)
            _direction = WalkDirection.Left;
        else if (option == WalkOptions.Right)
            _direction = WalkDirection.Right;

        StartCoroutine(TakeSteps(duration, speed));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "CaveWall")
        {
            if (transform.position.x < GameStatics.Camera.CurrentCamera.transform.position.x)
                _direction = WalkDirection.Right;
            else
                _direction = WalkDirection.Left;
        }
    }

    private IEnumerator TakeSteps(float duration = 1f, float speed = 2.4f)
    {
        float walkTimer = 0f;
        while (walkTimer < duration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                walkTimer += Time.deltaTime;
                int xSign = _direction == WalkDirection.Left ? -1 : 1;
                transform.position += new Vector3(xSign * Time.deltaTime * speed, 0f, 0f);
            }
            yield return null;
        }
        ((WalkAction)callerAction)?.EndWalk();
    }
}
