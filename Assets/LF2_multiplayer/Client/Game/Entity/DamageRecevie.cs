using System;
using UnityEngine;
namespace LF2.Client{

    public class DamageRecevie : MonoBehaviour, IHurtBox
    {
        [SerializeField] ClientCharacterVisualization clientCharacterViz; 

        public ulong NetworkObjectId => clientCharacterViz.NetworkObjectId;

        public StateType GetStateType()
        {
            return clientCharacterViz.MStateMachinePlayerViz.CurrentStateViz.GetId()  ;
        }

        public void ReceiveHP(AttackDataSend attackData)
        {
            clientCharacterViz.ReceiveHP(attackData);
            
        }

        public bool IsDamageable(TeamType teamAttacker)
        {
            if (teamAttacker == TeamType.INDEPENDANT){
                return true;
            }
            return clientCharacterViz.IsDamageable() && teamAttacker != clientCharacterViz.teamType;
        }


    }
}
    