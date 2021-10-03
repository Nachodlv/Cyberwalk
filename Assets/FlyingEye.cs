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
    public float SqrFireDistance = 15.0f;

    [Header("Movement")]
    public float SearchMovementSpeed = 5.0f;
    public float SearchRotationSpeed = 1.0f;
    public float AimRotationSpeed = 1.0f;

    private EyeState mCurrentState = EyeState.Searching;
    private EyeState mPreviusState = EyeState.Searching;

    Vector3 dirToPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        dirToPlayer = GameMode.Singleton.PlayerTransformCached.position - transform.position;
        // Face to player.
        if (mCurrentState == EyeState.Searching || mCurrentState == EyeState.Aiming)
        {
            float rotSpeed = mCurrentState == EyeState.Searching ? SearchRotationSpeed : AimRotationSpeed;
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotSpeed * Time.deltaTime);

            // Move
            if (mCurrentState == EyeState.Searching)
            {
                ProcessSearching();
            }
        }

    }

    void ProcessSearching()
    {
        if (dirToPlayer.sqrMagnitude <= SqrFireDistance)
        {
            SetState(EyeState.Aiming);
        }
        else
        {
            transform.Translate(dirToPlayer.normalized * SearchMovementSpeed * Time.deltaTime);
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
}
