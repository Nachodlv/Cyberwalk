using UnityEngine;

public struct HitInformation
{
    public HitInformation(Vector2 hitDirection)
    {
        HitDirection = hitDirection;
        IsAbsoluteDamage = false;
    }

    public Vector2 HitDirection;

    // For this is just for the screen bounds
    public bool IsAbsoluteDamage;
}

public interface IDamageable
{
    abstract void ApplyDamage(float damage, MonoBehaviour instigator, HitInformation hitInformation);
}
