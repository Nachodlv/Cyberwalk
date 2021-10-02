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


    void Awake()
    {
        //TODO: Change this for a singleton like getter, anonim namespace?
        CachedPlayer = GameObject.FindGameObjectWithTag("Player");

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
        if (Other.gameObject.tag == this.tag || Other.gameObject.tag == "Backpack")
        {
            SetupJointComponent();
            OnRegisterToPlayer();
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
