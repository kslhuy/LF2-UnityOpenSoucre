using Unity.Netcode;

namespace LF2
{
    /// <summary>
    /// Shared state for a Projectile.
    /// </summary>
    public class DUJ_BiscuitNetwork  : NetworkProjectileState
    {
        /// <summary>
        /// This event is raised when the arrow hit an enemy. The argument is the NetworkObjectId of the enemy.
        /// </summary>
        public System.Action<ulong> Dissapear;

        public System.Action<ulong> Appear;
        
        [ClientRpc]
        public void DissapearClientRPC(ulong enemyId)
        {
            HitEnemyEvent?.Invoke(enemyId);
        }

        [ClientRpc]
        public void AppearClientRPC(ulong enemyId)
        {
            HitEnemyEvent?.Invoke(enemyId);
        }
        
    }
}
