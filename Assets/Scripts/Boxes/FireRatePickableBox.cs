using UnityEngine;

public class FireRatePickableBox : PickableBox
{
    [SerializeField] private float fireRateIncrease = 0.2f;

    protected override void OnRegisterToPlayer()
    {
        base.OnRegisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.FireRate -= fireRateIncrease;
    }

    protected override void OnUnregisterToPlayer()
    {
        base.OnUnregisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.FireRate += fireRateIncrease;
    }
}
