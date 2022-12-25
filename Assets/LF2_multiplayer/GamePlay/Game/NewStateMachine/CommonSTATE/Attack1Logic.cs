using UnityEngine;

namespace LF2.Client
{
    public class Attack1Logic : MeleLogic
    {
        // private IdleLogicSO _originSO => (IdleLogicSO)base.OriginSO; // The SO this StateAction spawned from

        public override void Awake(StateMachineNew stateMachineFX)
        {
            this.stateMachineFX = stateMachineFX;

        }

        public override void Enter()
        {
            Debug.Log("attack not onwer");
            if(!Anticipated)
            {
                PlayAnim();
            }
            base.Enter();

        }


        public override StateType GetId()
        {
            return stateData.StateType;
        }



        public override void End(){
            stateMachineFX.idle();        
        }


        public override void PlayAnim( int nbanim = 1 , bool sequence = false)
        {
            // Debug.Log(nbanim);

            base.PlayAnim(nbanim);
            stateMachineFX.m_ClientVisual.NormalAnimator.Play(stateMachineFX.m_ClientVisual.VizAnimation.a_Attack_1);
            stateMachineFX.m_ClientVisual.PlayAudio(stateData.Sounds);

            // CameraShake.Instance.ShakeCamera(0.5f,0.2f);

        }

        public override void PlayPredictState(int nbAniamtion = 1, bool sequen = false)
        {
            // Owner Send to Server   =>>  Server propagate to all others players (except this client , the owner (who send))).
            if (stateMachineFX.m_ClientVisual.Owner){
                // Debug.Log("Attack Send to server");
                stateMachineFX.m_ClientVisual.m_NetState.AddPredictState_and_SyncServerRpc(GetId());
            }
            PlayAnim(nbAniamtion, sequen);
            // base.PlayPredictState(nbAniamtion, sequen);
        }


        public override void AddCollider(Collider collider)
        {
            base.AddCollider(collider);
        }


    }

}
