using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace LF2.ObjectPool
{
    /// <summary>
    /// Object Pool for networked objects, used for controlling how objects are spawned by Netcode. Netcode by default will allocate new memory when spawning new
    /// objects. With this Networked Pool, we're using custom spawning to reuse objects.
    /// Boss Room uses this for projectiles. In theory it should use this for imps too, but we wanted to show vanilla spawning vs pooled spawning.
    /// Hooks to NetworkManager's prefab handler to intercept object spawning and do custom actions
    /// </summary>
    [CreateAssetMenu(fileName = "ObjectPolling", menuName = "Polling/ObjectPolling")]
    public class ConfigObjectPollingSO : ScriptableObject{

        public GameObject Prefab;
        public int PrewarmCount;
   

    }
}
