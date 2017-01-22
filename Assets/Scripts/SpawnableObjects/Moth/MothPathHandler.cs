using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MothPathHandler {

    private enum PathStates
    {
        NorthWest, NorthEast, SouthEast, SouthWest
    }
    private PathStates _state;
    private bool _bLeft;

    private Moth.MothPathTypes _pathType;
    private readonly Moth _hostMoth;

    private const float SectionDuration = 1.1f;
    private float _pathTimer;

    private const float Pi = Mathf.PI;

    // Due to the unevenness of the curve, we can remove the extra displacement by calculating it
    private float _displacementX;
    private float _displacementY;
    private float _compensateX;
    private float _compensateY;

    public MothPathHandler(Moth host)
    {
        _hostMoth = host;
    }

    public void MoveAlongClover(float time)
    {
        _pathTimer += time;
        if (_pathTimer >= SectionDuration)
        {
            _pathTimer -= SectionDuration;
            ChooseNextDirection();
        }
        var pathParameters = GetPathParameters(time);
        _hostMoth.MothSprite.position += new Vector3(pathParameters.x, pathParameters.y, 0f);
        _hostMoth.MothSprite.localRotation = Quaternion.AngleAxis(pathParameters.z, Vector3.back);
    }

    private Vector3 GetPathParameters(float time)
    {
        const float pathSpeed = 0.7f;

        float startAngle = GetStartAngle();
        float angleOffset = 270f * _pathTimer / SectionDuration;
        float rotationZ = startAngle + (_bLeft ? -angleOffset : angleOffset); 
        if (rotationZ > 180) { rotationZ -= 360; }
        if (rotationZ < -180) { rotationZ += 360; }

        float offsetX = 0.065f * pathSpeed * Mathf.Sin(Pi / 180 * rotationZ);
        float offsetY = 0.065f * pathSpeed * Mathf.Cos(Pi / 180 * rotationZ);
        if (float.IsNaN(offsetX)) { offsetX = 0; }
        if (float.IsNaN(offsetY)) { offsetY = 0; }

        // Keep track of the displacement so we can remove unevenness
        _displacementX += offsetX;
        _displacementY += offsetY;
        offsetX -= _compensateX * time / SectionDuration;
        offsetY -= _compensateY * time / SectionDuration;

        return new Vector3(offsetX, offsetY, rotationZ);
    }

    private float GetStartAngle()
    {
        switch (_state)
        {
            case PathStates.NorthWest: return -45;
            case PathStates.NorthEast: return 45;
            case PathStates.SouthEast: return 135;
            case PathStates.SouthWest: return -135;
        }
        return 0;
    }

    /// <summary>
    /// Chooses the next section the moth will travel on the clover,
    /// depending on its current trajectory.
    /// </summary>
    private void ChooseNextDirection()
    {
        _compensateX = _displacementX;
        _compensateY = _displacementY;
        _displacementX = 0;
        _displacementY = 0;

        // Update the state to the currect direction the moth is facing
        switch (_state)
        {
            case PathStates.NorthWest:
                _state = !_bLeft ? PathStates.SouthWest : PathStates.NorthEast;
                break;
            case PathStates.NorthEast:
                _state = !_bLeft ? PathStates.NorthWest : PathStates.SouthEast;
                break;
            case PathStates.SouthEast:
                _state = !_bLeft ? PathStates.NorthEast : PathStates.SouthWest;
                break;
            case PathStates.SouthWest:
                _state = !_bLeft ? PathStates.SouthEast : PathStates.NorthWest;
                break;
        }

        // Pick the next direction for the moth to travel
        switch (_pathType)
        {
            case Moth.MothPathTypes.Infinity:
                _bLeft = !_bLeft;
                break;
            case Moth.MothPathTypes.Clover:
                _bLeft = Random.Range(0f, 1f) > 0.5f;
                break;
            case Moth.MothPathTypes.Figure8:
                _bLeft = !_bLeft;
                break;
            case Moth.MothPathTypes.Spiral:
                _bLeft = true;
                break;
            default:
                _bLeft = Random.Range(0f, 1f) > 0.5f;
                break;
        }
    }

    /// <summary>
    /// Set the path type for a newly activated moth
    /// Note: this initialises the moth path
    /// </summary>
    /// <param name="pathType"></param>
    public void SetPathType(Moth.MothPathTypes pathType)
    {
        _pathType = pathType;
        _state = PathStates.NorthWest;
        _pathTimer = 0f;
        _bLeft = pathType != Moth.MothPathTypes.Figure8;
    }
}
