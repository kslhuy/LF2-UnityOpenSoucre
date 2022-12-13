using UnityEngine;
using System.Collections.Generic;


namespace LF2.Client{

    /// <summary>
    /// Abstract base class for playing back the visual feedback of Current State.
    /// </summary>

    public abstract class StateFX {

        // // protected StateMachineClient stateMachineFX;

        // public InputPackage Data = new InputPackage();
        // // private Vector3 pivot = new Vector3(0,0.3f,0);

        // // Use for State have serveral different Logic
        // protected StateFX logicStateFX;

        // protected StateType m_stateType;
        // public SkillsDescription m_skillsDescription;
        // public DamageDetails[] subStateDetails;

        // // 
        // public bool ExtendState;
        // public float DurrationExtend;
        

 
        // // Constructor 
        // // protected StateFX(StateMachineClient playerStateMachineFX)
        // // {
        // //     this.stateMachineFX = playerStateMachineFX;
        // // }

        // public float TimeStarted_Animation ;

        // public bool Anticipated { get; protected set; }
        // public float DurationSubState { get; internal set; }

        // public abstract StateType GetId();

        // // Alaways check if player are already play animation first
        // public virtual void Enter(){
        //     Anticipated = false; //once you start for real you are no longer an anticipated action.
        //     TimeStarted_Animation = Time.time;
        //     // NOTE TimeStarted_Animation in Hurt State can be refesh many time to extend duree the cycle of this State 

        // }
        // public virtual void LogicUpdate(){

        // }


        // // Interrupt by somthing
        // public virtual void Exit(){
        //     Anticipated = false;
        // }

        // public virtual void AddCollider(Collider collider){}
        // public virtual void RemoveCollider(Collider collider){}
        // public virtual void OnStayCollider(Collider collider){}



        // /// <summary>
        // /// Called when the visualization receives an animation event.
        // /// </summary>
        // public virtual void OnAnimEvent(int id) { }

        // // Play Animation (shoulde be add base.PlayAnim() in specific (class) that derived from State ) 
        // // See in class AttackStateFX 
        // // Call everyttme when we want change State in  Visual !!!! 

        // public virtual void  PlayAnim( int nbAniamtion = 1 , bool sequen = false ){
        //     Anticipated = true;
        //     TimeStarted_Animation = Time.time;  
        // }
        // public virtual void  PlayPredictState( int nbAniamtion = 1 , bool sequen = false ){
        //     PlayAnim(nbAniamtion , sequen);
        // }


        // public virtual void OnGameplayActivityVisual(ref AttackDataSend attkData){
            
        // }



        // public virtual bool ShouldAnticipate(ref InputPackage requestData){
        //     return false;
        // }

        // /// <summary>
        // /// Utility function that instantiates all the graphics in the Spawns list.
        // /// If parentToOrigin is true, the new graphics are parented to the origin Transform.
        // /// If false, they are positioned/oriented the same way but are not parented.
        // /// </summary>
        // protected List<SpecialFXGraphic> InstantiateSpecialFXGraphics(Transform origin, bool parentToOrigin,bool flip , StateType stateType)
        // {
        //     var returnList = new List<SpecialFXGraphic>();
        //     foreach (var prefab in stateMachineFX.SkillDescription(stateType).SpawnsFX)
        //     {
        //         if (!prefab._Object) { continue; } // skip blank entries in our prefab list
        //         Vector3 pivot = new Vector3(prefab.pivot.x*stateMachineFX.CoreMovement.GetFacingDirection() ,prefab.pivot.y,prefab.pivot.z );
        //         returnList.Add(InstantiateSpecialFXGraphic(prefab._Object, origin,pivot, parentToOrigin ,flip ,stateType));
        //     }
        //     return returnList;
        // }

        // /// <summary>
        // /// Utility function that instantiates one of the graphics from the Spawns list.
        // /// If parentToOrigin is true, the new graphics are parented to the origin Transform.
        // /// If false, they are positioned/oriented the same way but are not parented.
        // /// </summary>
        // protected SpecialFXGraphic InstantiateSpecialFXGraphic(GameObject prefab, Transform origin,Vector3 pivot, bool parentToOrigin,bool flip,StateType stateType)
        // {
        //     if (prefab.GetComponent<SpecialFXGraphic>() == null)
        //     {
        //         throw new System.Exception($"One of the Spawns on action {stateMachineFX.SkillDescription(stateType).StateType} does not have a SpecialFXGraphic component and can't be instantiated!");
        //     }
        //     // var rotation =  flip ?  Quaternion.Euler(new Vector3(0,180,0)) : origin.transform.rotation; 
        //     var graphicsGO = GameObject.Instantiate(prefab, origin.transform.position + pivot, origin.transform.rotation, (parentToOrigin ? origin.transform : null));
        //     return graphicsGO.GetComponent<SpecialFXGraphic>();
        // }
 

        // public virtual void End()
        // {
        //     stateMachineFX.idle();
        // }

    }
}
   