using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")] [SerializeField] private float lifeTime = 5.0f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color minDamageColor;
    [SerializeField] private Color maxDamageColor;
    [SerializeField] private float maxDamage;
    [SerializeField] private float minDamage;

    private float _damage;
    private Rigidbody2D _rigidbody2DCached;

    public Rigidbody2D Rigidbody2D =>
        _rigidbody2DCached ? _rigidbody2DCached : _rigidbody2DCached = GetComponent<Rigidbody2D>();

    public void Initialize(float damage)
    {
        _damage = damage;
        sprite.color = Color.Lerp(minDamageColor, maxDamageColor, (maxDamage - _damage + minDamage) / maxDamage);
        Invoke(nameof(LifeTimeEnded), lifeTime);
    }

    private void LifeTimeEnded()
    {
        Destroy(gameObject);
    }
}
