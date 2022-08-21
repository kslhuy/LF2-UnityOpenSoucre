using Unity.Netcode;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;


namespace LF2.Client
{
    public class MeleLogic : StateActionLogic
    {

        private AttackDataSend Atk_data ;

        public override StateType GetId()
        {
            return StateType.NONE;
        }



        public override void AddCollider(Collider collider){
            IHurtBox damageables = collider.GetComponentInParent<IHurtBox>();
            IRebound reboundable = collider.GetComponent<IRebound>();

            if (damageables != null && damageables.IsDamageable(stateMachineFX.team) && damageables.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId)
            {
                Atk_data = new AttackDataSend();
                Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                Atk_data.Direction = stateData.DamageDetails[0].Dirxyz;
                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;
                Atk_data.Facing = stateMachineFX.CoreMovement.FacingDirection;

                damageables.ReceiveHP(Atk_data);

                if (stateData.SpawnsFX[0]._Object )
                {
                    GameObject.Instantiate(stateData.SpawnsFX[0]._Object, damageables.transform.position + stateMachineFX.CoreMovement.FacingDirection *stateData.SpawnsFX[0].pivot, Quaternion.identity);
                }
                
                if (stateData.Sounds)      stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds, damageables.transform.position);
                
                stateMachineFX.m_ClientVisual.ActiveHitLag(0.3f , 0.1f);

            }
            if (reboundable != null && reboundable.IsReboundable() ){
                reboundable.Rebound();
            }
   
        }




        
    }
    
}

    
