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
    [SerializeField] private int baseQuantityOfBullets = 1;
    [SerializeField] private float degreesPerBullet = 20.0f;
    [SerializeField] private int baseBulletBounces = 0;
    [SerializeField] private float baseBulletScale = 1.0f;

    private Transform _bulletsParent;
    private float _lastBulletFiredTime;
    private Vector2 _shootLocation;

    public float BulletSpeed { get; set; }
    public float BulletDamage { get; set; }
    public float FireRate { get; set; }
    public int QuantityOfBullets { get; set; }
    public int BulletBounces { get; set; }
    public float BulletScale { get; set; }
    public Bullet BulletPrefab => bulletPrefab;

    private void Awake()
    {
        BulletSpeed = baseSpeed;
        BulletDamage = baseDamage;
        FireRate = baseFireRate;
        BulletBounces = baseBulletBounces;
        QuantityOfBullets = baseQuantityOfBullets;
        BulletScale = baseBulletScale;
        _bulletsParent = new GameObject("Bullets").transform;
        _bulletsParent.parent = transform.parent.parent;
        _bulletsParent.localPosition = Vector3.zero;
    }

    public void LookAt(Vector2 location)
    {
        _shootLocation = location;
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

        Vector2 shootDirection = (_shootLocation - (Vector2) shootingPoint.position).normalized;

        for (int i = 0; i < QuantityOfBullets; i++)
        {
            bool even = i % 2 == 0;
            int index = (int) Mathf.Ceil((float) i / 2);
            float angle = index * degreesPerBullet * (even ? 1 : -1);
            Vector3 eulerRotation = new Vector3(0, 0, angle);

            Bullet bullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity, _bulletsParent);
            bullet.Initialize(BulletDamage, BulletBounces, BulletScale);
            bullet.Rigidbody2D.AddForce(Quaternion.Euler(eulerRotation) * shootDirection * BulletSpeed);
            _lastBulletFiredTime = now;
        }
    }
}
