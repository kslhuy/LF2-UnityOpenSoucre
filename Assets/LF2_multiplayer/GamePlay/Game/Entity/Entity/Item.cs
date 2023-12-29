using Unity.Netcode;
using System.Collections.Generic;
using System.IO;
using LF2.Server;

using UnityEngine;
using System.Collections;
using System;
using LF2.Utils;

namespace LF2.Client
{

    public class Item : NetworkBehaviour, IPickable_Item
    {

        public ItemType itemType;
        public ItemType ItemType()
        {
            return itemType;
        }
    }
}

