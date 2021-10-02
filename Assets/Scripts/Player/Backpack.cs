using System;
using System.Collections.Generic;
using DefaultNamespace.Boxes;
using UnityEngine;

public class Backpack : MonoBehaviour
{
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
}
