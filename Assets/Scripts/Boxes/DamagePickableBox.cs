using UnityEngine;

public class DamagePickableBox : PickableBox
{
    [SerializeField] private float DamageToAdd = 1.0f;
    protected override void OnRegisterToPlayer()
    {
        base.OnRegisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.BulletDamage += DamageToAdd;
    }

    protected override void OnUnregisterToPlayer()
    {
        base.OnUnregisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.BulletDamage -= DamageToAdd;
    }
}
