using Unity.Netcode;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;


namespace LF2.Client
{
    public class MeleLogic : StateActionLogic
    {

        private AttackDataSend Atk_data = new AttackDataSend() ;

        public override StateType GetId()
        {
            return StateType.NONE;
        }



        public override void AddCollider(Collider collider){
            IHurtBox damageables = collider.GetComponentInParent<IHurtBox>();
            // IRebound reboundable = collider.GetComponent<IRebound>();
            // if (reboundable != null && reboundable.IsReboundable() ){
            //     reboundable.Rebound();
            // }
   
            if (damageables != null && damageables.IsDamageable(stateMachineFX.team) && damageables.NetworkObjectId != stateMachineFX.m_ClientVisual.NetworkObjectId)
            {
                // Atk_data = new AttackDataSend();
                Atk_data.Amount_injury = stateData.DamageDetails[0].damageAmount;
                Atk_data.Direction = new Vector3 (stateData.DamageDetails[0].Dirxyz.x * stateMachineFX.CoreMovement.GetFacingDirection(),stateData.DamageDetails[0].Dirxyz.y , stateData.DamageDetails[0].Dirxyz.z ) ;
                Atk_data.BDefense_p = stateData.DamageDetails[0].Bdefend;
                Atk_data.Fall_p = stateData.DamageDetails[0].fall;
                Atk_data.Effect = (byte)stateData.DamageDetails[0].Effect;

                damageables.ReceiveHP(Atk_data);
                if (stateData.SpawnsFX.Length > 0)
                {
                    Vector3 positionEffect = new (damageables.transform.position.x , stateMachineFX.m_ClientVisual.GetCenterHitBox().y , damageables.transform.position.z) ;
                    GameObject.Instantiate(stateData.SpawnsFX[0]._Object, positionEffect, Quaternion.identity);
                }

                
                // stateMachineFX.m_ClientVisual.ActiveHitLag(0.3f , 0.1f);
                // CameraShake.Instance.ShakeCamera(stateData.Dx,0.1f);
            }

        }




        
    }
    
}

    
