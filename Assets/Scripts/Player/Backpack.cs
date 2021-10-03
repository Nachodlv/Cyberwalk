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

    private void Awake()
    {
        PickableBoxes = new List<PickableBox>();
        PlayerStats playerStats = GetComponentInParent<PlayerStats>();
        playerStats.DamageReceived.AddListener(DamageRecevied);
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

    public void MoveBoxes(Vector2 movement)
    {
        //Debug.Log("Moving the boxes!");
        foreach (PickableBox box in PickableBoxes)
        {
            Vector2 position = box.Rigidbody2DComponent.position;
            box.Rigidbody2DComponent.MovePosition(position + movement);
        }
    }
}
