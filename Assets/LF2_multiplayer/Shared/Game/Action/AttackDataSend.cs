using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    public struct AttackDataSend : INetworkSerializable
    {
        public AttackType AttackTypeEnum;      //the State to play.
        public Vector3 Direction;          //Here you can set how far the character will "fly" when he's hit. It will be his initial speed in x- and y-direction.

        public ulong[] TargetIds;          //NetworkObjectIds of targets, or null if untargeted.

        public int arest;
        public int vrest;
        public int Facing;                  // Attacker Facing
        public int Amount_injury;        //Here you set how many life points should be taken.  
        public int Fall_p;          //determines how an attacked character will react to this itr by cringing, getting into the "Dance of Pain", or falling.
        public int BDefense_p;  //  determine if a character is able to block an attack by defending or if he will go to the broken-defense-frames.


        [Flags]
        private enum PackFlags
        {
            None = 0,
            HasDirection = 1 ,
            HasArest = 1 << 1,
            HasVrest = 1 << 2,
            HasFacing = 1 << 3,
            HasAmount_injury = 1 << 4,
            HasFall_p = 1 << 5,
            HasBDefense_p = 1 << 6,

            HasTargetIds = 1<<7,
            //currently serialized with a byte. Change Read/Write if you add more than 8 fields.
        }

        /// <summary>
        /// Returns true if the StateRequestDatas are "functionally equivalent" (not including their Queueing or Closing properties).
        /// </summary>
        public bool Compare(ref AttackDataSend rhs)
        {
            bool scalarParamsEqual = (AttackTypeEnum, Direction, arest,vrest,Facing,Amount_injury,Fall_p,BDefense_p) == (rhs.AttackTypeEnum, rhs.Direction, rhs.arest,rhs.vrest,rhs.Facing,rhs.Amount_injury,rhs.Fall_p,rhs.BDefense_p);
            if (!scalarParamsEqual) { return false; }

            return true;
        }


        private PackFlags GetPackFlags()
        {
            PackFlags flags = PackFlags.None;
            if (Direction != Vector3.zero) { flags |= PackFlags.HasDirection; }
            if (arest != 0) { flags |= PackFlags.HasArest; }
            if (vrest != 0) { flags |= PackFlags.HasVrest; }
            if (Facing != 0) { flags |= PackFlags.HasFacing; }

            if (Amount_injury != 0) { flags |= PackFlags.HasAmount_injury; }
            if (Fall_p != 0) { flags |= PackFlags.HasFall_p; }
            
            if (BDefense_p!= 0) { flags |= PackFlags.HasBDefense_p; }
            if (TargetIds != null) { flags |= PackFlags.HasTargetIds; }


            return flags;
        }

       public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            PackFlags flags = PackFlags.None;
            if (!serializer.IsReader)
            {
                flags = GetPackFlags();
            }

            serializer.SerializeValue(ref AttackTypeEnum);
            serializer.SerializeValue(ref flags);



            if ((flags & PackFlags.HasFacing) != 0)
            {
                serializer.SerializeValue(ref Facing);
            }
            
            if ((flags & PackFlags.HasAmount_injury) != 0)
            {
                serializer.SerializeValue(ref Amount_injury);
            }

            if ((flags & PackFlags.HasArest) != 0)
            {
                serializer.SerializeValue(ref arest);
            }
            if ((flags & PackFlags.HasVrest) != 0)
            {
                serializer.SerializeValue(ref Direction);
            }
            if ((flags & PackFlags.HasFall_p) != 0)
            {
                serializer.SerializeValue(ref Fall_p);
            }
            if ((flags & PackFlags.HasBDefense_p) != 0)
            {
                serializer.SerializeValue(ref BDefense_p);
            }
            if ((flags & PackFlags.HasTargetIds) != 0)
            {
                serializer.SerializeValue(ref TargetIds);
            }

        }
    }
}