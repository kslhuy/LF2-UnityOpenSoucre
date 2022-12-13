using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// This is a temporary abstraction for different shared network states that all handle movement. This
    /// way a single client-side component can be used to update the local client transform. This can be dispensed with
    /// once Netcode for GameObjects handles client-side movement internally.
    /// </summary>

    public struct NetStatePackage : INetworkSerializable
    {
        public StateType StateTypeEnum;      //the State to play.

        public Vector3 Position;          
        public int RotationY;         

        // public int tickClient; 

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref StateTypeEnum);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref RotationY);

            // serializer.SerializeValue(ref tickClient);
        }
    }
    // public interface INetMovement
    // {
    //     /// <summary>
    //     /// The current transform position _ rotation of this entity.
    //     /// </summary>
    //     public NetworkVariable<NetStatePackage> NetworkPos_RotY { get; }

    //     public void InitNetworkPositionAndRotationY(Vector3 initPosition, float initRotationY);
        

    // }
}
