// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DavidAI_DUJstate : DUJstate
// {

//     // Creat Projectile 
//     SkillsData deep;
//     private int hashID;

//     public DavidAI_DUJstate(Player player, StateMachineServer stateMachine, PlayerData playerData, int hashID , SkillsData deep) : base(player, stateMachine, playerData, hashID)
//     {
//         this.deep = deep;
//         this.hashID = hashID;
//     }



//     public override void Enter()
//     {
//         base.Enter();
//         // stateMachine.AnimationBase.EnableProjectilEvent += CreateProjectile;
//     }



//     public override void Exit()
//     {
//         base.Exit();
//         // stateMachine.AnimationBase.EnableProjectilEvent -= CreateProjectile;

//     }



//     public override void LogicUpdate()
//     {
//         base.LogicUpdate();
//         // if(isFinishedAnimation()){
//         //     stateMachine.ChangeState(stateMachine.IdleState);
//         // }
//     }

//     public override void PhysicsUpdate()
//     {
//         base.PhysicsUpdate();
//     }

//     // public void CreateProjectile(){
//     //     DeepDLA.Create(stateMachine.AttackTransform.position,core.SetMovement.FacingDirection*Vector3.right,deep.CharacterType);
//     // }


// }
