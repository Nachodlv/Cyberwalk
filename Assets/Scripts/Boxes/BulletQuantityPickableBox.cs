using UnityEngine;

public class BulletQuantityPickableBox : PickableBox
{
    [SerializeField] private int quantityOfBullets = 1;
    protected override void OnRegisterToPlayer()
    {
        base.OnRegisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.QuantityOfBullets += quantityOfBullets;
    }

    protected override void OnUnregisterToPlayer()
    {
        base.OnUnregisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.QuantityOfBullets -= quantityOfBullets;
    }
}
