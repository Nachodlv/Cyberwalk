using UnityEngine;

public class ScaleBulletPickableBox : PickableBox
{
    [SerializeField] private float bulletScale = 0.3f;

    protected override void OnRegisterToPlayer()
    {
        base.OnRegisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.BulletScale += bulletScale;
    }

    protected override void OnUnregisterToPlayer()
    {
        base.OnUnregisterToPlayer();
        Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
        shooter.BulletScale -= bulletScale;
    }
}
