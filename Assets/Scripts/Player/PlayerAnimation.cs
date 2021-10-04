using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMovementController controller;
    [SerializeField] private float walkingAnimationSpeed;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int Ground = Animator.StringToHash("ground");

    private Vector2 _previousPosition = Vector2.zero;
    private bool _inAir;
    private float _fixGroundTime = 0.5f;
    private float _jumpTime;

    private void Awake()
    {
        controller.OnCharacterJump.AddListener(CharacterJump);
    }

    private void Update()
    {
        float Now = Time.time;
        if (Now - _jumpTime > _fixGroundTime)
        {
            animator.SetBool(Ground, controller.CachedRigidBodyIsGrounded);
        }
        else
        {
            animator.SetBool(Ground, false);
        }

        // if (!controller.CachedRigidBodyIsGrounded)
        // {
        //     _inAir = true;
        //     animator.SetBool(Ground, false);
        // }
        //
        // if (_inAir && controller.CachedRigidBodyIsGrounded)
        // {
        //     _inAir = false;
        //     animator.SetBool(Ground, true);
        // }

        Vector2 currentPosition = controller.RigidBodyComp.position;
        if (_previousPosition != Vector2.zero)
        {
            controller.ClampVelocity();

            //Vector2 distance = currentPosition - _previousPosition;
            Vector2 distance = controller.mHorizontalVelocity;
            //Debug.Log($"Speed: {Mathf.Abs(distance.x)}");

            animator.SetFloat(XSpeed, distance.x);
            float xSpeed = Mathf.Abs(distance.x);

            if (!_inAir && xSpeed > 0.01f)
            {
                animator.speed = xSpeed * walkingAnimationSpeed;
            }
            else
            {
                animator.speed = 1;
            }
        }
        _previousPosition = currentPosition;
    }

    private void CharacterJump()
    {
        _jumpTime = Time.time;
        animator.SetBool(Ground, false);
    }
}
