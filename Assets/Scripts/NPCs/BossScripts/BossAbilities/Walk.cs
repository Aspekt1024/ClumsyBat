using System.Collections;
using UnityEngine;

public class Walk : MonoBehaviour {

    private enum WalkDirection
    {
        Left,
        Right
    }

    private WalkDirection _direction;

    public void Activate()
    {
        StartCoroutine("TakeSteps");
    }

    private IEnumerator TakeSteps()
    {
        const float walkDuration = 1f;
        float walkTimer = 0f;
        float speed = 2.4f;
        
        int xSign = _direction == WalkDirection.Left ? -1 : 1;

        while (walkTimer < walkDuration)
        {
            if (!Toolbox.Instance.GamePaused)
            {
                walkTimer += Time.deltaTime;
                transform.position += new Vector3(xSign * Time.deltaTime * speed, 0f, 0f);
            }
            yield return null;
        }
        ReverseDirectionIfAtEnd();
    }

    private void ReverseDirectionIfAtEnd()
    {
        float camPosX = GameObject.FindGameObjectWithTag("MainCamera").transform.position.x;
        if (_direction == WalkDirection.Left && transform.position.x < -3.5f + camPosX)
        {
            _direction = WalkDirection.Right;
        }
        else if (_direction == WalkDirection.Right && transform.position.x > 4f + camPosX)
        {
            _direction = WalkDirection.Left;
        }
    }
}
