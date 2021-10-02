using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector2 _lastPosition;
    private float _xDistance;

    private void Awake()
    {
        var transformCached = transform;
        _lastPosition = transformCached.position;
        _xDistance = _lastPosition.x - transformCached.parent.position.x;
    }

    private void Update()
    {
        Vector2 parentPosition = transform.parent.position;

        Vector2 newPosition = _lastPosition;
        Vector2 currentPosition = transform.position;

        if (parentPosition.x + _xDistance >= currentPosition.x)
        {
            newPosition.x = Mathf.Max(newPosition.x, currentPosition.x);
        }
        transform.position = newPosition;
        _lastPosition = newPosition;

        transform.rotation = Quaternion.identity;
    }
}
