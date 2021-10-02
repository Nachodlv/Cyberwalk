using UnityEngine;

public interface IDamageable
{
    abstract void ApplyDamage(float damage, MonoBehaviour instigator);
}
