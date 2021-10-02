using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableBox : MonoBehaviour
{
    // Components and player refence
    protected GameObject CachedPlayer;
    protected SpringJoint2D SpringJoint2DComponent;
    protected Rigidbody2D Rigidbody2DComponent;

    // Player interaction
    [SerializeField]
    protected float OnPickedUpSmoothTime = 1.0f;

    [SerializeField]
    protected float OnPickedUpMaxSpeed = 1.0f;
    protected Vector2 OnPickedUpVelocity = Vector2.zero;

    private List<GameObject> _colliders;
    private bool insideBackpack;

    public bool IsInBackpack()
    {
        // If the have joint component and is connected, then is attached to player (is in backpack).
        return SpringJoint2DComponent != null && SpringJoint2DComponent.connectedBody != null;
    }

    void Awake()
    {
        _colliders = new List<GameObject>();

        //TODO: Change this for a singleton like getter, anonim namespace?
        CachedPlayer = GameMode.Singleton.PlayerCached;

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
        if (IsInBackpack())
        {
            return;
        }

        bool IsAnActiveBox = Other.gameObject.CompareTag(tag) && Other.gameObject.GetComponent<PickableBox>().IsInBackpack();
        bool IsBackpack = Other.collider.CompareTag("Backpack");

        if (IsAnActiveBox || IsBackpack)
        {
            _colliders.Add(Other.gameObject);
            SetupJointComponent();
            OnRegisterToPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool isBackpack = other.CompareTag("Backpack");
        if (isBackpack)
        {
            insideBackpack = false;
            if (_colliders.Count == 0)
            {

            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _colliders.Remove(other.gameObject);
        if (_colliders.Count == 0)
        {

        }
    }

    virtual protected void SetupJointComponent()
    {
        // Setting the component (por las dudas).
        SpringJoint2DComponent.connectedBody = CachedPlayer.GetComponent<Rigidbody2D>();
        SpringJoint2DComponent.anchor = Vector2.zero;
        SpringJoint2DComponent.connectedAnchor = Vector2.zero;
        SpringJoint2DComponent.autoConfigureDistance = false;
        SpringJoint2DComponent.distance = Vector2.Distance(transform.position, CachedPlayer.transform.position) / 2;
        SpringJoint2DComponent.enabled = true;
    }

    // Handle mouse drag over our collider.
    public void OnMouseDrag()
    {
        // Only allow dragging if we are not in the backpack.
        if (!SpringJoint2DComponent.enabled)
        {
            Vector2 MouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 SmootedNewPosition = Vector2.SmoothDamp(transform.position, MouseScreenPosition, ref OnPickedUpVelocity, OnPickedUpSmoothTime, OnPickedUpMaxSpeed);
            Rigidbody2DComponent.MovePosition(SmootedNewPosition);
        }
    }

    virtual protected void OnRegisterToPlayer()
    {

    }

    virtual protected void OnUnregisterToPlayer()
    {

    }

}
