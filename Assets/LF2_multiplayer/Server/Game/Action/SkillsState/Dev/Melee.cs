using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LF2.Server
{
    public class Melee : State
    {
        
        bool m_ExecutionFired;
        StateType stateType;
        public Melee(StateMachineServer statMachine) : base(statMachine)
        {
        }


        public override void Enter()
        {      
            // // TimeStarted_Server = Time.time;
            // Data.StateTypeEnum = StateType.Attack;
            
            // var foes = DetectFoe();
            
            // if (foes != null){
            //     stateMachine.nbHit = stateMachine.nbHit + 1;
            //     if (stateMachine.nbHit > 3) stateMachine.nbHit = 1;
            //     if (stateMachine.nbHit == 3 ) Data.NbAnimation = 3;
            // }
            // // Debug.Log("server Melee");
            
        }



        public override void End()
        {
            stateMachine.ChangeState(StateType.Idle);
            m_ExecutionFired = false;
        }

        public override void Exit()
        {
            m_ExecutionFired = false;
        }

        // /// <summary>
        // /// Returns the ServerCharacter of the foe we hit, or null if none found.
        // /// </summary>
        // /// <returns></returns>
        // private List<IHurtBox> DetectFoe(ulong foeHint = 0 )
        // {
        //     return GetIdealMeleeFoe(stateMachine.serverplayer.IsNpc,stateMachine.serverplayer.physicsWrapper.DamageCollider, m_MaxDistance);
        // }
            
            
        // /// <summary>
        // /// Utility used by Actions to perform Melee attacks. Performs a melee hit-test
        // /// and then looks through the results to find an alive target, preferring the provided
        // /// enemy.
        // /// </summary>
        // /// <param name="isNPC">true if the attacker is an NPC (and therefore should hit PCs). False for the reverse.</param>
        // /// <param name="ourCollider">The collider of the attacking GameObject.</param>
        // /// <param name="meleeRange">The range in meters to check for foes.</param>
        // /// <param name="preferredTargetNetworkId">The NetworkObjectId of our preferred foe, or 0 if no preference</param>
        // /// <returns>ideal target's IHurtBox, or null if no valid target found</returns>
        // public  List<IHurtBox> GetIdealMeleeFoe(bool isNPC, Collider ourCollider, float meleeRange)
        // {
        //     RaycastHit[] results;
        //     int numResults = StateUtils.DetectMeleeFoe(isNPC, ourCollider, meleeRange, out results);
        //     // Debug.Log($" numResults = {numResults}"); 

        //     // Debug.Log(numResults);
        //     List<IHurtBox> AllTargets = new List<IHurtBox>();

        //     //everything that got hit by the raycast should have an IHurtBox component, so we can retrieve that and see if they're appropriate targets.
        //     //we always prefer the hinted foe. If he's still in range, he should take the damage, because he's who the client visualization
        //     //system will play the hit-react on (in case there's any ambiguity).
        //     for (int i = 0; i < numResults; i++)
        //     {
        //         var damageable = results[i].collider.GetComponent<IHurtBox>();
        //         if (damageable != null && damageable.IsDamageable() && 
        //             damageable.NetworkObjectId != stateMachine.serverplayer.NetworkObjectId)
        //         {
        //             // Debug.Log($" damageable = {damageable}");
        //             AllTargets.Add(damageable);
        //         }
                
        //     }
        //     return AllTargets;
        // }

        // InputPackage FillDataRequest(){
        //     InputPackage m_data = new InputPackage();
        //     if (stateMachine.NbHit < 3){
        //         m_data.StateTypeEnum = StateType.Hurt; 
        //         m_data.NbAnimation = stateMachine.NbHit;
        //     }else {
        //         m_data.StateTypeEnum = StateType.Fall;
        //         m_data.Direction = new Vector3 (stateMachine.ServerCharacterMovement.FacingDirection , 0, 0);
        //     }
            
        //     return m_data;
        // }




        public override StateType GetId()
        {
            return stateType;
        }
    }
}