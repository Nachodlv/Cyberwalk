using System;
using UnityEngine;

public class ScreenBound : MonoBehaviour
{
    [SerializeField] private Vector2 boundingBox;

    private Camera _cachedCamera;

    private void Awake()
    {
        _cachedCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 position = transform.position;

        Vector3 minScreenBounds = _cachedCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = _cachedCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float x = Mathf.Clamp(position.x, minScreenBounds.x + boundingBox.x, maxScreenBounds.x - boundingBox.x);
        float y = Mathf.Clamp(position.y, minScreenBounds.y + boundingBox.y, maxScreenBounds.y - boundingBox.y);
        transform.position = new Vector3(x, y, position.z);
    }
}
