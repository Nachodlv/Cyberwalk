using System;
using UnityEngine;

public class FlickerSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flickerDuration = 0.1f;
    [SerializeField] private float flickerSpeed = 50.0f;
    [SerializeField] private Color flickerColor = Color.red;

    private Color _originalColor;
    private bool _isFlickering;
    private float _flickerStart;

    private void Awake()
    {
        _originalColor = spriteRenderer.color;
    }

    public void StartFlickering()
    {
        _flickerStart = Time.time;
        _isFlickering = true;
    }

    public void StopFlickering()
    {
        _isFlickering = false;
    }

    private void Update()
    {
        if (!_isFlickering)
        {
            return;
        }

        float now = Time.time;
        float t = 0.0f;
        if (now - _flickerStart < flickerDuration)
        {
            t = (Mathf.Sin(now * flickerSpeed) + 1) / 2;
        }
        else
        {
            StopFlickering();
        }

        Color newColor = Color.Lerp(_originalColor, flickerColor, t);
        spriteRenderer.color = newColor;
    }
}
