using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickableBox : MonoBehaviour
{
    [Header("Player interaction")]
    [SerializeField]
    protected float OnPickedUpSmoothTime = 1.0f;
    [SerializeField]
    protected float OnPickedUpMaxSpeed = 1.0f;

    [SerializeField] private UnityEvent onBoxDestroyedEvent;

    // Components and player refence
    protected GameObject CachedPlayer;
    protected GameObject CachedBackpack;
    protected SpringJoint2D SpringJoint2DComponent;
    protected Rigidbody2D Rigidbody2DComponent;
    protected Vector2 OnPickedUpVelocity = Vector2.zero;

    private List<GameObject> _colliders;
    private bool insideBackpackTrigger;
    private bool _destroyed = false;
    private bool InBackpack { get; set; }

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
            SetupJointComponent();
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
        SpringJoint2DComponent.connectedAnchor = CachedPlayer.transform.InverseTransformPoint(CachedBackpack.GetComponent<Backpack>().AttachPoint.transform.position);
        SpringJoint2DComponent.autoConfigureDistance = false;
        SpringJoint2DComponent.distance = Vector2.Distance(transform.position, CachedBackpack.GetComponent<Backpack>().FrameHorizontal.transform.position) * 0.5f;
        SpringJoint2DComponent.enabled = true;
    }

    // Handle mouse drag over our collider.
    public void OnMouseDrag()
    {
        // Only allow dragging if we are not in the backpack.
        if (!_destroyed && !InBackpack)
        {
            Vector2 MouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 SmootedNewPosition = Vector2.SmoothDamp(transform.position, MouseScreenPosition, ref OnPickedUpVelocity, OnPickedUpSmoothTime, OnPickedUpMaxSpeed);
            Rigidbody2DComponent.MovePosition(SmootedNewPosition);
        }
    }

    protected virtual void OnRegisterToPlayer()
    {
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

}
