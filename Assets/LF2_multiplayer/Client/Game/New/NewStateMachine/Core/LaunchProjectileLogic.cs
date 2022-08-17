using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
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
        protected void SpwanProjectile(SkillsDescription skillsDescription , Vector3 dirToMove)
        {

            var projectileInfo = GetProjectileInfo(skillsDescription);
            var playerPOS = stateMachineFX.CoreMovement.transform;
            // Debug.Log(projectileInfo.pivot);
            // Debug.Log(playerPOS.eulerAngles);

            var projectile = GameObject.Instantiate(projectileInfo.ProjectilePrefab, playerPOS.position + new Vector3(projectileInfo.pivot.x * stateMachineFX.CoreMovement.FacingDirection ,projectileInfo.pivot.y,projectileInfo.pivot.z) ,playerPOS.rotation);
            // Debug.Log(playerPOS);
            // point the projectile the same way we're facing
            // projectile.transform.right = playerPOS.right;

            //this way, you just need to "place" the arrow by moving it in the prefab, and that will control
            //where it appears next to the stateMachine.

            // projectile.transform.position =  + playerPOS.localToWorldMatrix.MultiplyPoint(projectile.transform.position);
            projectile.GetComponent<ProjectileLogic>().Initialize(stateMachineFX.m_ClientVisual.NetworkObjectId,stateMachineFX.team, dirToMove,in projectileInfo);

            projectile.GetComponent<NetworkObject>().Spawn();
        
        }

    }
}