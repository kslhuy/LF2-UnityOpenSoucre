
namespace LF2.Server{

    public class PlayerDUJState : State
    {

        public PlayerDUJState(StateMachineServer player) : base(player)
        {
            m_skillsDescription = stateMachine.SkillDescription(GetId());
            subStateDetails = m_skillsDescription.DamageDetails;
            InputPackage data = new InputPackage();
            data.StateTypeEnum = GetId(); 
        
            logicState = stateMachine.MakeState(stateMachine ,ref data )  ;
        }



        public override void Enter()
        {      
            base.Enter();
            logicState.Enter();

        }


        public override void LogicUpdate()
        {
            logicState.LogicUpdate();
        }




        public override void End()
        {
            stateMachine.ChangeState(StateType.Idle);
        }

        public override StateType GetId()
        {
            return StateType.DUJ1;
        }

      



    }
}
