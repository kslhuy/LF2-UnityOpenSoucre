using System;
using Unity.Netcode;
using UnityEngine;
namespace LF2{
    public struct StatePackage : INetworkSerializable {
        public int tick;
        public ulong TargetIds;
        public StateType StateTypeEnum;      //the State to play.
        public Vector3 Position;
        public Vector3 Velocity;
        public sbyte Rotation_Y;

    
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref StateTypeEnum);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Velocity);
            serializer.SerializeValue(ref Rotation_Y);

        }
    }
}
