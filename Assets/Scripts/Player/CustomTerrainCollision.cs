using System;
using UnityEngine;

public class CustomTerrainCollision : MonoBehaviour
{
    private CharacterMovementController _controller;
    private bool _leftCollision;

    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 myPosition = transform.position;
        _controller = other.gameObject.GetComponent<CharacterMovementController>();
        if (_controller)
        {
            Vector2 position = _controller.RigidBodyComp.position;
            _leftCollision = (position - myPosition).x < 0;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        CharacterMovementController controller = other.gameObject.GetComponent<CharacterMovementController>();
        if (controller)
        {
            _controller = null;
        }
    }

    private void FixedUpdate()
    {
        if (_controller)
        {
            _controller.CollisionLeft = !_leftCollision;
            _controller.CollisionRight = _leftCollision;
        }
    }
}
