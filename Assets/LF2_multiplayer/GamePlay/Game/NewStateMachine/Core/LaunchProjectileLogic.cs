using Unity.Netcode;
using UnityEngine;
using LF2.ObjectPool;

namespace LF2.Client
{
    public class LaunchProjectileLogic : StateActionLogic
    {


        public override bool ShouldAnticipate(ref InputPackage requestData){

            return false;
        }


        public override StateType GetId(){
            return StateType.NONE;
        }
        

        /// <summary>
        /// Looks through the ProjectileInfo list and finds the appropriate one to instantiate.
        /// For the base class, this is always just the first entry with a valid prefab in it!
        /// </summary>
        /// <exception cref="System.Exception">thrown if no Projectiles are valid</exception>
        protected  SkillsDescription.ProjectileInfo GetProjectileInfo(SkillsDescription skillsDescription )
        {
            foreach (var projectileInfo in skillsDescription.Projectiles)
            {
                if (projectileInfo.ProjectilePrefab && projectileInfo.ProjectilePrefab.GetComponent<NetworkProjectileState>())
                    return projectileInfo;
            }
            throw new System.Exception($"State {skillsDescription.StateType} has no usable Projectiles!");
        }

        /// <summary>
        /// Instantiates and configures the arrow. Repeatedly calling this does nothing.
        /// </summary>
        /// <remarks>
        /// This calls GetProjectilePrefab() to find the prefab it should instantiate.
        /// </remarks>
        protected void SpwanProjectile(SkillsDescription.ProjectileInfo projectileInfo , Vector3 dirToMove , Vector3 rotaion = default)
        {

            // var projectileInfo = GetProjectileInfo(skillsDescription);
            var playerPOS = stateMachineFX.CoreMovement.transform;
            // Debug.Log(projectileInfo.pivot);
            // Debug.Log(playerPOS.eulerAngles);

            var projectile = GameObject.Instantiate(projectileInfo.ProjectilePrefab, playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z) , rotaion != default ? Quaternion.Euler(rotaion) : playerPOS.rotation  );
            // Debug.Log(playerPOS);
            // point the projectile the same way we're facing
            // projectile.transform.right = playerPOS.right;

            //this way, you just need to "place" the arrow by moving it in the prefab, and that will control
            //where it appears next to the stateMachine.

            // projectile.transform.position =  + playerPOS.localToWorldMatrix.MultiplyPoint(projectile.transform.position);
            projectile.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team, dirToMove);

            projectile.GetComponent<NetworkObject>().Spawn();
        
        }
        protected void SpwanFX(LF2.SkillsDescription.SpawnsObject spawner , Vector3 dirToMove , Vector3 rotaion = default)
        {

            var playerPOS = stateMachineFX.CoreMovement.transform;
            // Debug.Log(projectileInfo.pivot);
            // Debug.Log(playerPOS.eulerAngles);

            var projectile = GameObject.Instantiate(spawner._Object, playerPOS.position + new Vector3(spawner.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,spawner.pivot.y,spawner.pivot.z)  , rotaion != default ? Quaternion.Euler(rotaion) : playerPOS.rotation  );
            // Debug.Log(playerPOS);
            // point the projectile the same way we're facing
            // projectile.transform.right = playerPOS.right;

            //this way, you just need to "place" the arrow by moving it in the prefab, and that will control
            //where it appears next to the stateMachine.

            // projectile.transform.position =  + playerPOS.localToWorldMatrix.MultiplyPoint(projectile.transform.position);
            projectile.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team, dirToMove);

            projectile.GetComponent<NetworkObject>().Spawn();
        
        }
        protected void SpwanProjectileObjectPooling(SkillsDescription.ProjectileInfo projectileInfo, Vector3 dirToMove , Vector3 rotaion = default)
        {

            var playerPOS = stateMachineFX.CoreMovement.transform;
            // Debug.Log(playerPOS.position);

            NetworkObject no = NetworkObjectPool.Singleton.GetNetworkObject(projectileInfo.ProjectilePrefab);
            // point the projectile the same way we're facing

            //this way, you just need to "place" the arrow by moving it in the prefab, and that will control
            //where it appears next to the player.

            var go = no.gameObject;

            go.transform.position = playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z) ;
            // Debug.Log(go.transform.position);
            go.transform.rotation = rotaion != default ? Quaternion.Euler(rotaion) : playerPOS.rotation;
            
            var projectileLogic = no.GetComponent<ProjectileLogic>();
            projectileLogic.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team, dirToMove , rotaion);
            go.SetActive(true);
            no.Spawn(true);
        
        }

    }
}