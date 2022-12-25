using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client{

	public abstract class StateActionLogic 
	{
		// internal StateActionLogicSO _originSO;

		// /// <summary>
		// /// Use this property to access shared data from the <see cref="StateActionSO"/> that corresponds to this <see cref="StateAction"/>
		// /// </summary>
		// protected StateActionLogicSO OriginSO => _originSO;

		public SkillsDescription stateData;
            

		/// <summary>
		/// Called every frame the <see cref="StateMachine"/> is in a <see cref="State"/> with this <see cref="StateAction"/>.
		/// </summary>
		public virtual void LogicUpdate(){}

		/// <summary>
		/// Awake is called when creating a new instance. Use this method to cache the components needed for the action.
		/// Act like a Constructor 
 
		///</summary>
		/// <param name="stateMachine">The <see cref="StateMachine"/> this instance belongs to.</param>
		public virtual void Awake(StateMachineNew stateMachine) { }


		/// <summary>
		/// This enum is used to create flexible <c>StateActions</c> which can execute in any of the 3 available "moments".
		/// The StateAction in this case would have to implement all the relative functions, and use an if statement with this enum as a condition to decide whether to act or not in each moment.
		/// </summary>
		public enum SpecificMoment
		{
			Enter, OnStateExit, OnUpdate,
		}


        protected StateMachineNew stateMachineFX;

        public InputPackage Data = new InputPackage();



 
        public float TimeStarted_Animation ;

        public bool Anticipated { get; protected set; }

        public abstract StateType GetId();

        // Alaways check if player are already play animation first
        public virtual void Enter(){
            Anticipated = false; //once you start for real you are no longer an anticipated action.
            TimeStarted_Animation = Time.time;
            // NOTE TimeStarted_Animation in Hurt State can be refesh many time to extend duree the cycle of this State 

        }


        // Interrupt by somthing
        public virtual void Exit(){
            Anticipated = false;
        }

        public virtual void AddCollider(Collider collider){}
        public virtual void RemoveCollider(Collider collider){}
        public virtual void OnStayCollider(Collider collider){}



        /// <summary>
        /// Called when the visualization receives an animation event.
        /// </summary>
        public virtual void OnAnimEvent(int id) { }

        // Play Animation (shoulde be add base.PlayAnim() in specific (class) that derived from State ) 
        // See in class AttackStateFX 
        // Call everyttme when we want change State in  Visual !!!! 

        public virtual void  PlayAnim( int nbAniamtion = 1 , bool sequen = false ){
            Anticipated = true;
            TimeStarted_Animation = Time.time;  
        }
        public virtual void  PlayPredictState( int nbAniamtion = 1 , bool sequen = false ){
            PlayAnim(nbAniamtion , sequen);
        }


        public virtual void OnGameplayActivityVisual(ref AttackDataSend attkData){
            
        }

        // This method is called to determine whether the state should anticipate transitioning to another
        // state or performing some other action. The requestData parameter contains information about the
        // current state of the state machine and the input that it is receiving. The method returns a bool
        // value indicating whether the state should anticipate the transition or action.

        public virtual bool ShouldAnticipate(ref InputPackage requestData){
            return false;
        }

        public virtual void HurtResponder(Vector3 dirToRespond){
        }

        /// <summary>
        /// Utility function that instantiates all the graphics in the Spawns list.
        /// If parentToOrigin is true, the new graphics are parented to the origin Transform.
        /// If false, they are positioned/oriented the same way but are not parented.
        /// </summary>
        protected List<SpecialFXGraphic> InstantiateSpecialFXGraphics(Transform origin, bool parentToOrigin,bool flip , StateType stateType)
        {
            var returnList = new List<SpecialFXGraphic>();
            foreach (var prefab in stateData.SpawnsFX)
            {
                if (!prefab._Object) { continue; } // skip blank entries in our prefab list
                Vector3 pivot = new Vector3(prefab.pivot.x*stateMachineFX.CoreMovement.GetFacingDirection() ,prefab.pivot.y,prefab.pivot.z );
                returnList.Add(InstantiateSpecialFXGraphic(prefab._Object, origin,pivot, parentToOrigin ,flip ,stateType));
            }
            return returnList;
        }

        /// <summary>
        /// Utility function that instantiates one of the graphics from the Spawns list.
        /// If parentToOrigin is true, the new graphics are parented to the origin Transform.
        /// If false, they are positioned/oriented the same way but are not parented.
        /// </summary>
        protected SpecialFXGraphic InstantiateSpecialFXGraphic(GameObject prefab, Transform origin,Vector3 pivot, bool parentToOrigin,bool flip,StateType stateType)
        {
            if (prefab.GetComponent<SpecialFXGraphic>() == null)
            {
                throw new System.Exception($"One of the Spawns on action {stateData.StateType} does not have a SpecialFXGraphic component and can't be instantiated!");
            }
            // var rotation =  flip ?  Quaternion.Euler(new Vector3(0,180,0)) : origin.transform.rotation; 
            var graphicsGO = GameObject.Instantiate(prefab, origin.transform.position + pivot, origin.transform.rotation, (parentToOrigin ? origin.transform : null));
            return graphicsGO.GetComponent<SpecialFXGraphic>();
        }
 

        public virtual void End()
        {
            stateMachineFX.idle();
        }

    }
}
   

