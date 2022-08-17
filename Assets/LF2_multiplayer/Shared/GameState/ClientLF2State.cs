namespace LF2.Client
{

    /// <summary>
    /// Client specialization of core BossRoom game logic.
    /// </summary>
    public class ClientLF2State : GameStateBehaviour
    {
        public override GameState ActiveState {  get { return GameState.LF2_Net; } }


        public override void OnNetworkSpawn()
        {
            if( !IsClient ) { this.enabled = false; }
        }

    }

}
