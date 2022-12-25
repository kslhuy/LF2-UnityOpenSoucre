using System;
using UnityEngine;
using Unity.Netcode;

namespace LF2
{
    /// <summary>
    /// Common data and RPCs for the GameLoop LF2 stage.
    /// </summary>
    public class NetLF2State : NetworkBehaviour
    {


        [HideInInspector]
        public NetworkVariable<NetworkGuid> BackGroundGUID  = new NetworkVariable<NetworkGuid>();


        public Action GameEnd;
        
        [ServerRpc]
        public void EndGameServerRPC(){
            GameEnd?.Invoke();
        }

        

    }
}