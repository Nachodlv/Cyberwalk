using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    public float GravityForce = 10;

    [Header("Physics Settings")]
    public bool UseCharacterController = true;

    [Header("RigidBody settings")]
    public float GroundCheckCircleRadius = 1.0f;
    public float GroundCheckDistance = 1.0f;
    public LayerMask GroundLayer;

    [Header("Jump Settings")]
    public float JumpForce = 5.0f;
    public float JumpMaxHeight = 5.0f;
    public float LandingTimeThreshold = 1.0f;

    [Header("Movement Settings")]
    public float WalkingMovementSpeed = 1.0f;

    [Header("Debug")]
    public bool DebugRays;

    // RigidBody variables:
    bool mIsRigidBodyGrounded = false;
    Vector2 mGroundHitPosition = Vector2.zero;
    bool mCachedRigidBodyIsGrounded = false;

    float mCurrentSpeed = 0;

    bool mJumpKeyPressed = false;
    bool mJumpKeyHold = false;
    Vector3 mHorizontalVelocity = Vector3.forward;
    Vector3 mVerticalVelocity;
    Vector3 mLastMovementDirection;

    [HideInInspector]
    public CharacterController CharacterControllerComp;
    public Rigidbody2D RigidBodyComp;
    BoxCollider2D mBoxColliderComp;
    //Transform mRender;

    bool IsOverMaxHeight
    {
        get
        {
            return transform.position.y >= JumpMaxHeight;
        }
    }

    bool IsGoingUp
    {
        get
        {
            return mVerticalVelocity.y > 0;
        }
    }

    bool IsCharacterGrounded
    {
        get
        {
            return UseCharacterController ? CharacterControllerComp.isGrounded : CheckIfRigidBodyIsGrounded();
        }
    }

    void Start()
    {
        if (UseCharacterController)
        {
            CharacterControllerComp = GetComponent<CharacterController>();
        }
        else
        {
            RigidBodyComp = GetComponent<Rigidbody2D>();
            mBoxColliderComp = GetComponent<BoxCollider2D>();
        }
        //mRender = GetComponentInChildren<SpriteRenderer>().transform;
    }

    void Update()
    {
        // Check inputs
        mHorizontalVelocity.x = Input.GetAxisRaw("Horizontal");
        mJumpKeyHold = Input.GetButton("Jump");

        if (!UseCharacterController)
        {
            if (!mJumpKeyPressed)
            {
                mJumpKeyPressed = Input.GetButtonDown("Jump");
            }
            return;
        }
        mJumpKeyPressed = Input.GetButtonDown("Jump");

        if(mHorizontalVelocity != Vector3.zero && mHorizontalVelocity.normalized != mLastMovementDirection)
        {
            mLastMovementDirection = mHorizontalVelocity.normalized;
        }

        // Calculate Speed
        mCurrentSpeed = WalkingMovementSpeed;

        // Calculate Jump and Combo things.
        if(IsCharacterGrounded)
        {
            // If is using CharacterController apply a little of force to keep it on the ground so isGrounded = true.
            mVerticalVelocity.y = -GravityForce * Time.deltaTime;

            if(mJumpKeyPressed)
            {
                Jump();
            }
        }
        else
        {
            if(IsOverMaxHeight || (IsGoingUp && !mJumpKeyHold))
            {
                mVerticalVelocity.y = 0;
            }

            // Gravity
            mVerticalVelocity.y -= GravityForce * Time.deltaTime;
        }
        mHorizontalVelocity *= mCurrentSpeed * Time.deltaTime;

        // Apply the velocities.
        CharacterControllerComp.Move(mVerticalVelocity + mHorizontalVelocity);
    }

    void FixedUpdate()
    {
        if (UseCharacterController)
        {
            return;
        }

        if(mHorizontalVelocity != Vector3.zero && mHorizontalVelocity.normalized != mLastMovementDirection)
        {
            mLastMovementDirection = mHorizontalVelocity.normalized;
        }

        // Calculate Speed
        mCurrentSpeed = WalkingMovementSpeed;

        // Calculate Jump and Combo things.
        if(IsCharacterGrounded)
        {
            // If is using CharacterController apply a little of force to keep it on the ground so isGrounded = true.
            //mVerticalVelocity.y = UseCharacterController ? -GravityForce * Time.deltaTime : 0.0f;
            mVerticalVelocity.y = 0.0f;

            if(mJumpKeyPressed)
            {
                mJumpKeyPressed = false;
                Jump();
            }
        }
        else
        {
            // Keep the jump velocity if spacebar is hold.
            if(IsOverMaxHeight || (IsGoingUp && !mJumpKeyHold))
            {
                mVerticalVelocity.y = 0;
            }

            // Gravity
            mVerticalVelocity.y -= GravityForce * Time.deltaTime;
        }
        mHorizontalVelocity *= mCurrentSpeed * Time.deltaTime;

        Vector3 mFinalPosition = transform.position + (mVerticalVelocity + mHorizontalVelocity);

        if (mCachedRigidBodyIsGrounded && mVerticalVelocity.y == 0.0f)
        {
            mFinalPosition.y = mGroundHitPosition.y + mBoxColliderComp.bounds.extents.y;
        }
        RigidBodyComp.MovePosition(mFinalPosition);
    }

    void Jump()
    {
        mVerticalVelocity.y = JumpForce;
    }

    bool CheckIfRigidBodyIsGrounded()
    {
        if (IsGoingUp)
        {
            return false;
        }

        RaycastHit2D hit2D = Physics2D.CircleCast(mBoxColliderComp.bounds.center, GroundCheckCircleRadius, Vector2.down, mBoxColliderComp.bounds.extents.y + GroundCheckDistance - GroundCheckCircleRadius, GroundLayer);

        if (DebugRays)
        {
            Debug.DrawRay(mBoxColliderComp.bounds.center, Vector2.down * GroundCheckDistance, Color.green);
        }

        bool isColliding = hit2D.collider != null;
        mCachedRigidBodyIsGrounded = isColliding;

        if (isColliding)
        {
            mGroundHitPosition = hit2D.point;
        }

        return isColliding;
    }

    void OnDrawGizmos()
    {
        if (DebugRays)
        {
            if (mBoxColliderComp)
            {
                Gizmos.DrawWireSphere(mBoxColliderComp.bounds.center + Vector3.down * (GroundCheckCircleRadius + GroundCheckDistance), GroundCheckCircleRadius);
            }
        }
    }
}
