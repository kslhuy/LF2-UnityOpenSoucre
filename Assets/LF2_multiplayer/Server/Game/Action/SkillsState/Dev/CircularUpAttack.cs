
namespace LF2.Server
{
    public class CircularUpAttack : State
    {
        StateType stateType;
        public CircularUpAttack(StateMachineServer statMachine, ref InputPackage data) : base(statMachine)
        {
            stateType = data.StateTypeEnum;
        }
        

        public override StateType GetId()
        {
            return stateType;
        }
    }
}