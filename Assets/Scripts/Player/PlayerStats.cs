using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private UnityEvent playerDieEvent;
    [SerializeField] private UnityEvent<int> healthUpdated;

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            healthUpdated.Invoke(_currentHealth);
            if (_currentHealth <= 0)
            {
                PlayerDie();
            }
        }
    }

    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
        healthUpdated.Invoke(_currentHealth);
    }

    private void PlayerDie()
    {
        playerDieEvent.Invoke();
        SceneManager.LoadScene(0);
    }

    public void ApplyDamage(float damage, MonoBehaviour instigator)
    {
        CurrentHealth -= (int) damage;
    }
}
