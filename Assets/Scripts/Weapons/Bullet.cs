using System;
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
    private int _bouncesRemaining;
    private Vector2 _lastFrameVelocity;

    private Rigidbody2D _rigidbody2DCached;

    public Rigidbody2D Rigidbody2D =>
        _rigidbody2DCached ? _rigidbody2DCached : _rigidbody2DCached = GetComponent<Rigidbody2D>();

    public void Initialize(float damage, int bounces, float scale)
    {
        _damage = damage;
        _bouncesRemaining = bounces;
        sprite.color = Color.Lerp(minDamageColor, maxDamageColor, (maxDamage - _damage + minDamage) / maxDamage);
        transform.localScale = new Vector3(scale, scale, scale);
        Invoke(nameof(LifeTimeEnded), lifeTime);
    }

    private void LifeTimeEnded()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        _lastFrameVelocity = Rigidbody2D.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable?.ApplyDamage(_damage, this);
            LifeTimeEnded();
        }
        else if (_bouncesRemaining > 0 && other.contactCount > 0)
        {
            Vector2 normal = other.GetContact(0).normal;
            float velocityMagnitude = _lastFrameVelocity.magnitude;

            Vector2 direction = Vector3.Reflect(_lastFrameVelocity.normalized, normal);

            Rigidbody2D.velocity = direction * velocityMagnitude;
            _bouncesRemaining--;
        }
        else
        {
            LifeTimeEnded();
        }
    }

}
