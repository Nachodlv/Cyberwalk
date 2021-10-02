using UnityEngine;

namespace DefaultNamespace.Boxes
{
    public class HealthPickableBox : PickableBox
    {
        [SerializeField] private int amountOfHealth = 1;

        protected override void OnRegisterToPlayer()
        {
            base.OnRegisterToPlayer();
            PlayerStats playerStats = CachedPlayer.GetComponent<PlayerStats>();
            playerStats.CurrentHealth += amountOfHealth;
        }

        protected override void OnUnregisterToPlayer()
        {
            base.OnUnregisterToPlayer();
            PlayerStats playerStats = CachedPlayer.GetComponent<PlayerStats>();
            playerStats.CurrentHealth -= amountOfHealth;
        }
    }
}
