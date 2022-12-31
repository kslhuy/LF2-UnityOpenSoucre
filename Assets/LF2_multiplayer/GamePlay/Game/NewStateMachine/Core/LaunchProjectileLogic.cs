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

            var playerPOS = stateMachineFX.CoreMovement.transform;


            var projectile = GameObject.Instantiate(projectileInfo.ProjectilePrefab, playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z) , rotaion != default ? Quaternion.Euler(rotaion) : playerPOS.rotation  );
            
            projectile.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team, dirToMove);

            projectile.GetComponent<NetworkObject>().Spawn(true);
        
        }

        protected void SpwanProjectileNormal(SkillsDescription.ProjectileInfo projectileInfo , Vector3 dirToMove , Vector3 rotaion = default){
            var playerPOS = stateMachineFX.CoreMovement.transform;
            var projectile = GameObject.Instantiate(projectileInfo.ProjectilePrefab, playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z) , rotaion != default ? Quaternion.Euler(rotaion) : playerPOS.rotation  );
            
            projectile.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team, dirToMove);

        }
        protected void SpwanFX(LF2.SkillsDescription.SpawnsObject spawner ,int facing = 0)
        {
            var playerPOS = stateMachineFX.CoreMovement.transform;

            var projectile = GameObject.Instantiate(spawner._Object);

            projectile.transform.position = playerPOS.position + new Vector3(spawner.pivot.x*facing ,spawner.pivot.y,spawner.pivot.z) ;
            // Debug.Log(go.transform.position);
            projectile.transform.localRotation =  playerPOS.rotation;
            


        
        }
        protected void SpwanProjectileObjectPooling(SkillsDescription.ProjectileInfo projectileInfo, Vector3 dirToMove , Vector3 rotaion = default)
        {

            var playerPOS = stateMachineFX.CoreMovement.transform;
            // Debug.Log(playerPOS.position);

            NetworkObject no = NetworkObjectPool.Singleton.GetNetworkObject(projectileInfo.ProjectilePrefab,playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z),rotaion != default ? Quaternion.Euler(rotaion) : playerPOS.rotation);
            // point the projectile the same way we're facing

            //this way, you just need to "place" the arrow by moving it in the prefab, and that will control
            //where it appears next to the player.

            // var go = no.gameObject;

            // go.SetActive(true);
            // go.transform.position = playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.GetFacingDirection() ,projectileInfo.pivot.y,projectileInfo.pivot.z) ;
            // // Debug.Log(go.transform.position);
            // go.transform.rotation = rotaion != default ? Quaternion.Euler(rotaion) : playerPOS.rotation;
            
            var projectileLogic = no.GetComponent<ProjectileLogic>();
            projectileLogic.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team, dirToMove , rotaion);
            no.Spawn(true);
        
        }

    }
}