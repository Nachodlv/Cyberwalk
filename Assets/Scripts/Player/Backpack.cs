using System;
using System.Collections.Generic;
using DefaultNamespace.Boxes;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public GameObject FrameHorizontal;
    public GameObject FrameVertical;
    public Transform AttachPoint;
    public List<PickableBox> PickableBoxes { get; set; }

    private int _forceMaxFrames = 1;
    private int _forceFramesRemaining;
    private Vector2 _force;
    private float _extraForce = 1.0f;

    private void Awake()
    {
        PickableBoxes = new List<PickableBox>();
        PlayerStats playerStats = GetComponentInParent<PlayerStats>();
        playerStats.DamageReceived.AddListener(DamageRecevied);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _forceMaxFrames--;
            Debug.Log($"Ticks: {_forceMaxFrames}");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            _forceMaxFrames++;
            Debug.Log($"Ticks: {_forceMaxFrames}");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            _extraForce -= 0.1f;
            Debug.Log($"Extra force: {_extraForce}");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _extraForce += 0.1f;
            Debug.Log($"Extra force: {_extraForce}");
        }
    }


    void FixedUpdate()
    {
        if (_forceFramesRemaining > 0)
        {
            _forceFramesRemaining--;
            foreach (PickableBox box in PickableBoxes)
            {
                Vector2 position = box.Rigidbody2DComponent.position;
                box.Rigidbody2DComponent.MovePosition(position + _force);
            }
        }
    }

    private void DamageRecevied(float damage, MonoBehaviour instigator)
    {
        int damageReceived = (int) damage;
        for (int i = PickableBoxes.Count - 1; i >= 0; i--)
        {
            if (damageReceived > 0 && PickableBoxes[i] is HealthPickableBox)
            {
                PickableBoxes[i].DestroyBox();
                PickableBoxes.RemoveAt(i);
                damageReceived--;
            }
        }
    }

    public void MoveBoxes(float x, float y)
    {
        _forceFramesRemaining = _forceMaxFrames;
        Debug.Log("Moving boxes");
        Debug.Log($"Applying force x: {x}, y: {y}");
        foreach (PickableBox box in PickableBoxes)
        {
            Vector3 position = box.Rigidbody2DComponent.position;
            position.x += x;
            position.y += y;
            Debug.Log($"Position: {position}");
            box.Rigidbody2DComponent.MovePosition(position);
        }
    }
}
