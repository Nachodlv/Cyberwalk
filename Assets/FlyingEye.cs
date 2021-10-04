using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EyeState
{
    Searching = 0,
    Aiming = 1,
    Firing = 2,
    Cooldown = 3,
    Idle = 4
}

public class FlyingEye : MonoBehaviour, IDamageable
{
    [Header("References")]
    public GameObject EyeLight;
    public Material AimingMaterial;
    public Material LaserMaterial;

    [Header("Stats")]
    public float TotalHealth = 5.0f;

    [Header("Times")]
    public float SearchTime = 0.5f;
    public float LockTime = 0.5f;
    public float LoadingTime = 0.5f;
    public float LaserDuration = 0.5f;

    [Header("Distances")]
    public float OffScreenActivationFactor = 0.33f;
    public float FireDistance = 15.0f;

    [Header("Movement")]
    public float SearchMovementSpeed = 5.0f;
    public float SearchRotationSpeed = 1.0f;
    public float AimRotationSpeed = 1.0f;

    [Header("Laser")]
    public float LaserThickness = 2.0f;
    public float LaserDamage = 1.0f;
    public bool PushPlayer = false;
    public LayerMask DamageCheckLayer;

    [Header("Events")]
    [SerializeField] private UnityEvent enemyDestroyed;

    [Header("Debug")]
    public bool DebugTrace = false;


    private EyeState mCurrentState = EyeState.Searching;
    private EyeState mPreviusState = EyeState.Searching;

    float mAimingElapsedTime = 0.0f;
    float mFiringElapsedTime = 0.0f;
    float mCooldownElapsedTime = 0.0f;

    float _currentHealth;

    bool mIsActive = false;

    Vector3 leftUpperMargin;
    Vector3 rightBottomMargin;
    Vector3 dirToPlayer;
    Camera _cachedCamera;

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            if (_currentHealth <= 0.0f)
            {
                EnemyDestroyed();
            }
        }
    }

    void Awake()
    {
        _cachedCamera = Camera.main;
        leftUpperMargin = _cachedCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) * -OffScreenActivationFactor;
        rightBottomMargin = _cachedCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) * OffScreenActivationFactor;
    }

    void Start()
    {
        CurrentHealth = TotalHealth;
        SetState(EyeState.Idle);
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
                // Check if in range.
                if (dirToPlayer.sqrMagnitude <= FireDistance * FireDistance)
                {
                    SetState(EyeState.Aiming);
                }
                else
                {
                    // Movement.
                    transform.position += transform.right * Time.deltaTime * SearchMovementSpeed;
                }
            }
            else if(IsInState(EyeState.Aiming))
            {
                mAimingElapsedTime += Time.deltaTime;
                if (mAimingElapsedTime >= LockTime)
                {
                    SetState(EyeState.Firing);
                }
            }
        }
        else if (IsInState(EyeState.Firing))
        {
            mFiringElapsedTime += Time.deltaTime;
            if (mFiringElapsedTime >= LaserDuration)
            {
                EyeLight.GetComponent<MeshRenderer>().enabled = false;
                SetState(EyeState.Cooldown);
            }

            LaserTrace();
        }
        else if(IsInState(EyeState.Cooldown))
        {
            mCooldownElapsedTime += Time.deltaTime;
            if (mCooldownElapsedTime >= LaserDuration)
            {
                EyeLight.GetComponent<MeshRenderer>().enabled = true;
                SetState(EyeState.Searching);
            }
        }
        else if(IsInState(EyeState.Idle))
        {
            if (IsInActivationRange())
            {
                mIsActive = true;
                SetState(EyeState.Searching);
            }
        }
    }

    private void EnemyDestroyed()
    {
        enemyDestroyed.Invoke();
        Destroy(gameObject);
    }

    public void ApplyDamage(float damage, MonoBehaviour instigator, HitInformation hitInformation)
    {
        if (mIsActive)
        {
            CurrentHealth -= damage;
        }
    }

    void LaserTrace()
    {
        RaycastHit2D[] hit2D = Physics2D.CircleCastAll(transform.position, LaserThickness, transform.right, FireDistance * 2.0f, DamageCheckLayer);

        foreach (RaycastHit2D hit in hit2D)
        {
            if (hit.collider != null)
            {
                IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector3 PushVelocity = PushPlayer ? transform.right : Vector3.zero;
                    damageable.ApplyDamage(LaserDamage, this, new HitInformation(PushVelocity));
                }
            }
        }
    }

    private bool IsInActivationRange()
    {
        Vector3 position = transform.position;
        Vector3 minScreenBounds = _cachedCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)) + leftUpperMargin;
        Vector3 maxScreenBounds = _cachedCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) + rightBottomMargin;
        return position.x > minScreenBounds.x && position.y > minScreenBounds.y && position.x < maxScreenBounds.x &&
               position.y < maxScreenBounds.y;
    }

    void SetState(EyeState InState)
    {
        if (mCurrentState != InState)
        {
            mPreviusState = mCurrentState;
            mCurrentState = InState;
        }

        switch(mCurrentState)
        {
            case EyeState.Aiming:
            {
                GetComponent<LineRenderer>().enabled = true;
                GetComponent<LineRenderer>().sharedMaterial = AimingMaterial;
                mAimingElapsedTime = 0.0f;
                break;
            }
            case EyeState.Firing:
            {
                GetComponent<LineRenderer>().sharedMaterial = LaserMaterial;
                mFiringElapsedTime = 0.0f;
                break;
            }
            case EyeState.Cooldown:
            {
                GetComponent<LineRenderer>().enabled = false;
                mCooldownElapsedTime = 0.0f;
                break;
            }
            default:
            {
                break;
            }
        }
    }

    bool IsInState(EyeState InState)
    {
        return mCurrentState == InState;
    }

    void OnDrawGizmos()
    {
        if (DebugTrace)
        {
            Vector3 LaserEndPoint = transform.position + (transform.right * FireDistance * 2);
            Vector3 UpperLineOffset = (transform.up * LaserThickness * 0.5f);
            Vector3 BottomLineOffset = (transform.up * LaserThickness * 0.5f) * -1;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(LaserEndPoint, LaserThickness * 0.5f);
            Gizmos.DrawLine(transform.position + UpperLineOffset, LaserEndPoint + UpperLineOffset);
            Gizmos.DrawLine(transform.position + BottomLineOffset, LaserEndPoint + BottomLineOffset);
            Gizmos.DrawWireCube(transform.position + (transform.right * FireDistance), new Vector3(LaserThickness, LaserThickness, LaserThickness));
        }
    }
}
