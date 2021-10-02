using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Shooter shooter;

    [Header("Stats")]
    [SerializeField] private float range;
    [SerializeField] private float totalHealth;

    [Header("Events")]
    [SerializeField] private UnityEvent enemyDestroyed;

    private float _currentHealth;

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            if (_currentHealth <= 0.0f)
            {
                EnemyDestroyed();
            }
        }
    }

    private void Awake()
    {
        CurrentHealth = totalHealth;
    }

    private void Update()
    {
        GameObject playerGameObject = GameMode.Singleton.PlayerCached;
        Vector3 playerPosition = playerGameObject.transform.position;
        shooter.LookAt(playerPosition);

        var position = transform.position;
        float distanceToPlayer = Vector2.Distance(playerPosition, position);
        if (distanceToPlayer > range) return;
        RaycastHit2D hit = Physics2D.Raycast(position, playerPosition, Mathf.Infinity, shooter.BulletPrefab.gameObject.layer);
        if (!hit.collider || hit.collider.gameObject == playerGameObject)
        {
            shooter.Shoot();
        }
    }

    private void EnemyDestroyed()
    {
        enemyDestroyed.Invoke();
        Destroy(gameObject);
    }

    public void ApplyDamage(float damage, MonoBehaviour instigator)
    {
        CurrentHealth -= damage;
    }
}
