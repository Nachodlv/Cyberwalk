using System;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] private Transform gunSprite;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float maxRotation = 360.0f;
    [SerializeField] private float minRotation = -360.0f;

    [Header("Bullet")]
    [SerializeField] private Bullet bulletPrefab;

    [Header("Stats")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseFireRate;

    private Transform _bulletsParent;
    private float _lastBulletFiredTime;

    public float BulletSpeed { get; set; }
    public float BulletDamage { get; set; }
    public float FireRate { get; set; }
    public Bullet BulletPrefab => bulletPrefab;

    private void Awake()
    {
        BulletSpeed = baseSpeed;
        BulletDamage = baseDamage;
        FireRate = baseFireRate;
        _bulletsParent = new GameObject("Bullets").transform;
    }

    public void LookAt(Vector2 location)
    {
        Vector2 difference = (Vector3) location - gunSprite.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        rotationZ = Mathf.Clamp(rotationZ, minRotation, maxRotation);
        gunSprite.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
    }

    public void Shoot()
    {
        float now = Time.time;
        if (now - _lastBulletFiredTime < FireRate)
        {
            return;
        }

        Bullet bullet = Instantiate(bulletPrefab, shootingPoint.position, gunSprite.rotation, _bulletsParent);
        bullet.Initialize(BulletDamage);
        bullet.Rigidbody2D.AddForce(shootingPoint.right * BulletSpeed);
        _lastBulletFiredTime = now;
    }

    private void OnDestroy()
    {
        if (_bulletsParent)
        {
            Destroy(_bulletsParent.gameObject);
        }
    }
}
