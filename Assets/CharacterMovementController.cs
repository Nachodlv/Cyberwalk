using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovementController : MonoBehaviour
{
    public float GravityForce = 10;
    public float MaxFallingSpeed = 10.0f;
    public float Deceleration = 10.0f;

    [Header("Physics Settings")]
    public bool UseCharacterController = true;

    [Header("RigidBody settings")]
    public float GroundCheckCircleRadius = 1.0f;
    public float GroundCheckDistance = 1.0f;
    public Vector2 GroundCheckUpperCircleOffset = Vector2.zero;
    public LayerMask GroundLayer;

    [Header("Jump Settings")]
    public float JumpForce = 5.0f;
    public float JumpMaxHeight = 5.0f;
    public float CoyoteTimeThreshold = 0.1f;

    [Header("Movement Settings")]
    public float WalkingMovementSpeed = 1.0f;

    [Header("Debug")]
    public bool DebugRays;

    // RigidBody variables:
    bool mIsRigidBodyGrounded = false;
    Vector2 mGroundHitPosition = Vector2.zero;
    public bool CachedRigidBodyIsGrounded { get; private set; }
    public UnityEvent OnCharacterJump;

    Vector3 mLastTickImpulse = Vector3.zero;
    bool mWasImpulsedThisFrame = false;

    float mCurrentSpeed = 0;
    float mCurrentMaxHeight;
    float mLastTimeInGround = 0.0f;

    bool mJumpKeyPressed = false;
    bool mJumpKeyHold = false;
    bool mCoyoteJumpIsPosible = false;
    bool bCoyoteJumpWasConsumed = false;
    [HideInInspector]
    public Vector3 mHorizontalVelocity = Vector3.forward;
    Vector3 mVerticalVelocity;
    Vector3 mLastMovementDirection;

    public bool CollisionLeft { get; set; }
    public bool CollisionRight { get; set; }

    [HideInInspector]
    public CharacterController CharacterControllerComp;
    public Rigidbody2D RigidBodyComp;
    BoxCollider2D mBoxColliderComp;
    //Transform mRender;

    bool IsOverMaxHeight
    {
        get
        {
            return transform.position.y >= mCurrentMaxHeight;
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

    public float FeetHeight => mBoxColliderComp.bounds.center.y - mBoxColliderComp.bounds.extents.y;

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
            mCoyoteJumpIsPosible = !bCoyoteJumpWasConsumed && Time.time - mLastTimeInGround <= CoyoteTimeThreshold;
            if (!mJumpKeyPressed)// && (IsCharacterGrounded || mCanCoyoteJump))
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

        if (mLastTickImpulse.magnitude > 0.005f)
        {
            //Debug.Log(mLastTickImpulse.magnitude);
            if (mWasImpulsedThisFrame)
            {
                mWasImpulsedThisFrame = false;
                // mHorizontalVelocity.x = mLastTickImpulse.x * Time.deltaTime;
                // mVerticalVelocity.y = mLastTickImpulse.y;
                //Vector3 mNewImpulsePosition = transform.position + (mVerticalVelocity + mHorizontalVelocity);
                Vector3 mNewImpulsePosition = transform.position + (mLastTickImpulse * Time.deltaTime);
                // GameMode.Singleton.BackpackCached.MoveBoxes(mLastTickImpulse * Time.deltaTime);
                RigidBodyComp.MovePosition(mNewImpulsePosition);
                mLastTickImpulse = Vector3.zero;
            }
            else
            {
                // Gravity
                mVerticalVelocity.y -= GravityForce * Time.deltaTime;
                mVerticalVelocity.y = Mathf.Max(mVerticalVelocity.y, -MaxFallingSpeed);
            }
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
            mCurrentMaxHeight = transform.position.y + JumpMaxHeight;

            // If is using CharacterController apply a little of force to keep it on the ground so isGrounded = true.
            mVerticalVelocity.y = 0.0f;

            if(mJumpKeyPressed)
            {
                //mJumpKeyPressed = false;
                Jump();
            }
        }
        else
        {
            if (mCoyoteJumpIsPosible && mJumpKeyPressed)
            {
                //mJumpKeyPressed = false;
                Jump();
            }
            else
            {
                //mJumpKeyPressed = false;
                // Keep the jump velocity if spacebar is hold.
                if(IsOverMaxHeight || (IsGoingUp && !mJumpKeyHold))
                {
                    //mVerticalVelocity.y = 0;
                    mVerticalVelocity.y -= GravityForce * Time.deltaTime * Deceleration;
                }

                // Gravity
                mVerticalVelocity.y -= GravityForce * Time.deltaTime;
                mVerticalVelocity.y = Mathf.Max(mVerticalVelocity.y, -MaxFallingSpeed);
            }
        }
        mJumpKeyPressed = false;
        mHorizontalVelocity *= mCurrentSpeed * Time.deltaTime;

       ClampVelocity();
       CollisionLeft = false;
       CollisionRight = false;

        Vector3 mFinalPosition = transform.position + (mVerticalVelocity + mHorizontalVelocity);

        if (IsCharacterGrounded && Math.Abs(mVerticalVelocity.y) < 0.001f)
        {
            var positionY = transform.position.y;
            mFinalPosition.y = mGroundHitPosition.y + mBoxColliderComp.bounds.extents.y;
            float yDifference = mFinalPosition.y - positionY;
            if (Mathf.Abs(yDifference) > 0.05f)
            {
                GameMode.Singleton.BackpackCached.MoveBoxes(new Vector2(0, yDifference));
            }
        }


        RigidBodyComp.MovePosition(mFinalPosition);
    }

    public void ClampVelocity()
    {
        if (CollisionLeft)
        {
            mHorizontalVelocity.x = Mathf.Max(0.0f, mHorizontalVelocity.x);
        }

        if (CollisionRight)
        {
            mHorizontalVelocity.x = Mathf.Min(0.0f, mHorizontalVelocity.x);
        }
    }

    void Jump()
    {
        mVerticalVelocity.y = JumpForce;
        bCoyoteJumpWasConsumed = true;
        OnCharacterJump.Invoke();
    }

    bool CheckIfRigidBodyIsGrounded()
    {
        if (IsGoingUp)
        {
            return false;
        }

        RaycastHit2D hit2D = Physics2D.CircleCast(mBoxColliderComp.bounds.center + (Vector3) GroundCheckUpperCircleOffset, GroundCheckCircleRadius, Vector2.down, GroundCheckDistance, GroundLayer);
        // RaycastHit2D hit2D = Physics2D.CircleCast(mBoxColliderComp.bounds.center, GroundCheckCircleRadius, Vector2.down, mBoxColliderComp.bounds.extents.y + GroundCheckDistance - GroundCheckCircleRadius, GroundLayer);

        if (DebugRays)
        {
            Debug.DrawRay(mBoxColliderComp.bounds.center, Vector2.down * GroundCheckDistance, Color.green);
        }

        bool isColliding = hit2D.collider != null;
        CachedRigidBodyIsGrounded = isColliding;

        if (isColliding)
        {
            bCoyoteJumpWasConsumed = false;
            mLastTimeInGround = Time.time;
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
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(mBoxColliderComp.bounds.center + (Vector3) GroundCheckUpperCircleOffset, GroundCheckCircleRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(mBoxColliderComp.bounds.center + (Vector3) GroundCheckUpperCircleOffset + Vector3.down * GroundCheckDistance, GroundCheckCircleRadius);
            }
        }
    }

    // With a vector of magnitude around 20 seems to work fine.
    public void ApplyForce(Vector2 Force)
    {
        if (Force.magnitude == 0)
        {
            return;
        }
        mLastTickImpulse = Force;
        mWasImpulsedThisFrame = true;
    }

    // void OnMouseDown()
    // {
    //     float X = 0.75f * Mathf.Sign(Random.Range(-1.0f,1.0f));
    //     float Y = 0.75f;
    //     ApplyForce(new Vector2(X,Y) * 20);
    // }

}
