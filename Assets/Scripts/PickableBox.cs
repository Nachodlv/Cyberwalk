using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickableBox : MonoBehaviour, IDamageable
{
    [Header("Player interaction")]
    [SerializeField]
    protected float OnPickedUpSmoothTime = 0.05f;
    [SerializeField]
    protected float OnPickedUpMaxSpeed = 1000.0f;

    [SerializeField] private UnityEvent onBoxDestroyedEvent;
    [SerializeField] private float impulseWhenHit = 30.0f;

    [SerializeField] private UnityEvent boxGrabbed;
    [SerializeField] private UnityEvent boxConnected;

    // Components and player refence
    protected GameObject CachedPlayer;
    protected Backpack CachedBackpack;
    protected SpringJoint2D SpringJoint2DComponent;
    public Rigidbody2D Rigidbody2DComponent { get; protected set; }
    protected Vector2 OnPickedUpVelocity = Vector2.zero;

    private List<GameObject> _colliders;
    private bool insideBackpackTrigger;
    private bool _destroyed = false;
    private bool InBackpack { get; set; }

    private BoxCollider2D _boxCollider2D;
    public BoxCollider2D BoxCollider2D => _boxCollider2D ? _boxCollider2D : _boxCollider2D = GetComponent<BoxCollider2D>();

    void Awake()
    {
        _colliders = new List<GameObject>();

        //TODO: Change this for a singleton like getter, anonim namespace?
        CachedPlayer = GameMode.Singleton.PlayerCached;
        CachedBackpack = GameMode.Singleton.BackpackCached;

        SpringJoint2DComponent = GetComponent<SpringJoint2D>();
        // If the box is not in the backpack at the start of the game, we disable the sprint component
        // so we avoid the component being removed.
        if (SpringJoint2DComponent.connectedBody == null)
        {
            SpringJoint2DComponent.enabled = false;
        }

        Rigidbody2DComponent = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D Other)
    {
        if (InBackpack || _destroyed)
        {
            return;
        }

        bool IsAnActiveBox = Other.gameObject.CompareTag(tag) && Other.gameObject.GetComponent<PickableBox>().InBackpack;
        bool IsBackpack = Other.collider.CompareTag("Backpack");

        if (IsAnActiveBox || IsBackpack)
        {
            _colliders.Add(Other.gameObject);
            InBackpack = true;
            //SetupJointComponent();
            OnRegisterToPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isBackpack = other.CompareTag("Backpack");
        if (isBackpack)
        {
            insideBackpackTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool isBackpack = other.CompareTag("Backpack");
        if (isBackpack)
        {
            insideBackpackTrigger = false;
            if (InBackpack && _colliders.Count == 0 && !_destroyed)
            {
                InBackpack = false;
                OnUnregisterToPlayer();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _colliders.Remove(other.gameObject);
        if (_colliders.Count == 0 && InBackpack)
        {
            if (!insideBackpackTrigger && !_destroyed)
            {
                InBackpack = false;
                OnUnregisterToPlayer();
            }
        }
    }

    virtual protected void SetupJointComponent()
    {
        if (!SpringJoint2DComponent)
        {
            SpringJoint2DComponent = gameObject.AddComponent<SpringJoint2D>();
        }

        // Setting the component (por las dudas).
        SpringJoint2DComponent.connectedBody = CachedPlayer.GetComponent<Rigidbody2D>();
        SpringJoint2DComponent.anchor = Vector2.zero;
        SpringJoint2DComponent.connectedAnchor = CachedPlayer.transform.InverseTransformPoint(CachedBackpack.AttachPoint.transform.position);
        SpringJoint2DComponent.autoConfigureDistance = false;
        SpringJoint2DComponent.distance = Vector2.Distance(transform.position, CachedBackpack.FrameHorizontal.transform.position) * 0.5f;
        SpringJoint2DComponent.enabled = true;
    }

    // Handle mouse drag over our collider.
    public void OnMouseDrag()
    {
        // Only allow dragging if we are not in the backpack.
        if (!_destroyed && !InBackpack)
        {
            boxGrabbed.Invoke();
            Vector2 MouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 SmootedNewPosition = Vector2.SmoothDamp(transform.position, MouseScreenPosition, ref OnPickedUpVelocity, OnPickedUpSmoothTime, OnPickedUpMaxSpeed);
            Rigidbody2DComponent.MovePosition(SmootedNewPosition);
        }
    }

    protected virtual void OnRegisterToPlayer()
    {
        if (!InBackpack)
        {
            Debug.LogError($"Not in backpack!");
        }
        boxGrabbed.Invoke();
        Backpack backpack = CachedPlayer.GetComponentInChildren<Backpack>();
        backpack.PickableBoxes.Add(this);
    }

    protected virtual void OnUnregisterToPlayer()
    {
        Backpack backpack = CachedPlayer.GetComponentInChildren<Backpack>();
        backpack.PickableBoxes.Remove(this);
        DestroyBox();
    }

    public void DestroyBox()
    {
        // Destroy the spring
        if (SpringJoint2DComponent)
        {
            SpringJoint2DComponent.enabled = false;
        }
        onBoxDestroyedEvent.Invoke();
        _destroyed = true;
    }

    public void ApplyDamage(float damage, MonoBehaviour instigator, HitInformation hitInformation)
    {
        if (hitInformation.IsAbsoluteDamage)
        {
            Destroy(gameObject);
        }
        else
        {
            Rigidbody2DComponent.AddForce(hitInformation.HitDirection * impulseWhenHit);
            if (!InBackpack)
            {
                Debug.Log($"BoxHit {damage}");
                DestroyBox();
            }
        }
    }
}
