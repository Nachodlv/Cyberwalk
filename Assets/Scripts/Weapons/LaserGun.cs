using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public float LockTime = 0.5f;
    public float LoadingTime = 0.5f;
    public float BurningDuration = 0.5f;

    void Start()
    {
        
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.up, Color.red);
    }

}
