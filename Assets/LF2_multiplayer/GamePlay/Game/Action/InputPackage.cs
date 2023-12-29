using System;
using Unity.Netcode;
using UnityEngine;
using LF2.Client;

namespace LF2
{
    /// <summary>
    /// List of all Actions supported in the game.
    /// </summary>


    /// <summary>
    /// List of all Types of Actions. There is a many-to-one mapping of Actions to ActionLogics.
    /// </summary>
    public enum StateLogic
    {
        Movement , 
        Attack,
        Hurt,
        Special,
    }



    /// <summary>
    /// Comprehensive class that contains information needed to play back any action on the server. This is what gets sent client->server when
    /// the Action gets played, and also what gets sent server->client to broadcast the action event. Note that the OUTCOMES of the action effect
    /// don't ride along with this object when it is broadcast to clients; that information is sync'd separately, usually by NetworkVariables.
    /// </summary>
    public struct InputPackage : INetworkSerializable
    {
        public int tick;
        public ulong TargetIds ;
        public InputSerialization.DirectionalInput dir;

        // public ButtonInputType buttonType ;

        public StateType buttonID ;
        // public StateType StateTypeEnum;      //the State to play.


       public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref dir);
            // serializer.SerializeValue(ref buttonType);
            serializer.SerializeValue(ref buttonID);
        }
    }


}
