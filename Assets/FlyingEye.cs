using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EyeState
{
    Searching = 0,
    Aiming = 1,
    Loading = 2,
    Firing = 3
}

public class FlyingEye : MonoBehaviour
{
    [Header("Times")]
    public float SearchTime = 0.5f;
    public float LockTime = 0.5f;
    public float LoadingTime = 0.5f;
    public float BurningDuration = 0.5f;

    [Header("Distances")]
    public float FireDistance = 15.0f;

    [Header("Movement")]
    public float SearchMovementSpeed = 5.0f;
    public float SearchRotationSpeed = 1.0f;
    public float AimRotationSpeed = 1.0f;

    private EyeState mCurrentState = EyeState.Searching;
    private EyeState mPreviusState = EyeState.Searching;

    float mAimingElapsedTime = 0.0f;

    Vector3 dirToPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        dirToPlayer = GameMode.Singleton.PlayerTransformCached.position - transform.position;
        // Face to player.
        if (IsInState(EyeState.Searching) || IsInState(EyeState.Aiming))
        {
            float rotSpeed = mCurrentState == EyeState.Searching ? SearchRotationSpeed : AimRotationSpeed;
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotSpeed * Time.deltaTime);

            // Move
            if (IsInState(EyeState.Searching))
            {
                ProcessSearching();
            }
            else if(IsInState(EyeState.Aiming))
            {
                mAimingElapsedTime += Time.deltaTime;

                if (mAimingElapsedTime >= LockTime)
                {
                    SetState(EyeState.Firing);
                }
                Debug.DrawRay(transform.position, transform.right * FireDistance * 2.0f, Color.red);
            }
        }
        else if (IsInState(EyeState.Firing))
        {
            const int rayThickness = 2;
            int first = -rayThickness;
            for (int i = first; i < rayThickness; i++)
            {
                Debug.DrawRay(transform.position + (transform.up * i), transform.right * FireDistance * 2.0f, Color.red);
            }
        }

    }

    void ProcessSearching()
    {
        if (dirToPlayer.sqrMagnitude <= FireDistance * FireDistance)
        {
            SetState(EyeState.Aiming);
        }
        else
        {
            transform.position += transform.right * Time.deltaTime * SearchMovementSpeed;
        }
    }

    void SetState(EyeState InState)
    {
        if (mCurrentState != InState)
        {
            mPreviusState = mCurrentState;
            mCurrentState = InState;
        }
    }

    bool IsInState(EyeState InState)
    {
        return mCurrentState == InState;
    }
}
