using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private UnityEvent playerDieEvent;
    [SerializeField] private UnityEvent<int, int> healthUpdated;

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            healthUpdated.Invoke(_currentHealth, value);
            _currentHealth = value;
            if (_currentHealth <= 0)
            {
                PlayerDie();
            }
        }
    }

    public UnityEvent<int, int> HealthUpdated => healthUpdated;
    public UnityEvent<float, MonoBehaviour> DamageReceived;

    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
        healthUpdated.Invoke(0, _currentHealth);
    }

    private void PlayerDie()
    {
        playerDieEvent.Invoke();
        SceneManager.LoadScene(0);
    }

    public void ApplyDamage(float damage, MonoBehaviour instigator)
    {
        CurrentHealth -= (int) damage;
        DamageReceived.Invoke(damage, instigator);
    }
}
