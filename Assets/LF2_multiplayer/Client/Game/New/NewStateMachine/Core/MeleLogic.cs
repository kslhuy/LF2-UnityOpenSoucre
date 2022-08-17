using Unity.Netcode;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;


namespace LF2.Client
{
    public class MeleLogic : StateActionLogic
    {


        public override StateType GetId()
        {
            return StateType.NONE;
        }



        // public void SubMeleCollider(ref Collider collider , ref SkillsDescription skillsDescription , ref StateMachineNew stateMachine )
        // {

        //     IHurtBox damageables = collider.GetComponentInParent<IHurtBox>();
 
        //     if (damageables != null && damageables.IsDamageable() && damageables.NetworkObjectId != stateMachine.m_ClientVisual.NetworkObjectId)
        //     {
     

        //         damageables.ReceiveHP(Atk_data);


        //         if (skillsDescription.SpawnsFX.Length > 0)
        //         {
        //             GameObject.Instantiate(skillsDescription.SpawnsFX[0]._Object, damageables.transform.position + stateMachine.CoreMovement.FacingDirection * skillsDescription.SpawnsFX[0].pivot, Quaternion.identity);
        //         }

        //         stateMachine.m_ClientVisual.PlayAudio(skillsDescription.Sounds, damageables.transform.position);
        //         stateMachine.CurrentStateViz.TimeStarted_Animation += 0.1f;
        //         stateMachine.nbHit += 1;
        //     }

        // }



        
    }
    
}

    
