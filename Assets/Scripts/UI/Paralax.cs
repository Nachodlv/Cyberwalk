using System;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] private float parallax;

    private SpriteRenderer _spriteRenderer;
    private float _xOffset;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private CharacterMovementController controller;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GameMode.Singleton.PlayerCached.GetComponent<CharacterMovementController>();
    }

    private void Update()
    {
        float speed = controller.mHorizontalVelocity.x;
        if (speed > 0.0f)
        {
            _xOffset = (_xOffset + speed * parallax * Time.deltaTime) % 360;
            _spriteRenderer.material.SetTextureOffset(MainTex, new Vector2(_xOffset, 0));
        }
    }

}
