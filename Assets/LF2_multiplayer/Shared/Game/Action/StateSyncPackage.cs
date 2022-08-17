using System;
using Unity.Netcode;
using UnityEngine;
namespace LF2{
    public struct StateSyncPackage : INetworkSerializable {
        public StateType StateTypeEnum;      //the State to play.
        public Vector3 Pos;
        public float RotY;

    
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {


            serializer.SerializeValue(ref StateTypeEnum);
            serializer.SerializeValue(ref Pos);
            serializer.SerializeValue(ref RotY);


        }
    }
}
