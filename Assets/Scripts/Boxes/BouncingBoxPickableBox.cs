using UnityEngine;

namespace DefaultNamespace.Boxes
{
    public class BouncingBoxPickableBox : PickableBox
    {
        [SerializeField] private int quantityOfBounces = 1;

        protected override void OnRegisterToPlayer()
        {
            base.OnRegisterToPlayer();
            Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
            shooter.BulletBounces += quantityOfBounces;
        }

        protected override void OnUnregisterToPlayer()
        {
            base.OnUnregisterToPlayer();
            Shooter shooter = CachedPlayer.GetComponentInChildren<Shooter>();
            shooter.BulletBounces -= quantityOfBounces;
        }
    }
}
