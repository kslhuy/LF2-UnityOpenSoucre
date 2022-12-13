// //  Component  for every test scence 
//  using System.Collections;
// using System.Collections.Generic;
// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.SceneManagement;
// using Random = UnityEngine.Random;

// namespace LF2.Server
// {
//     /// <summary>
//     /// Server specialization of core BossRoom game logic.
//     /// </summary>
//     public class ServerTestState : NetworkBehaviour
//     {
//         [SerializeField]
//         NetworkManager m_NetworkManager;

        
//         [SerializeField]
//         AvatarRegistry m_AvatarRegistry;


//         [SerializeField]
//         [Tooltip("Make sure this is included in the NetworkManager's list of prefabs!")]
//         private NetworkObject m_PlayerPrefab;

//         [SerializeField]
//         [Tooltip("A collection of locations for spawning players")]
//         private Transform[] m_PlayerSpawnPoints;

//         private List<Transform> m_PlayerSpawnPointsList = null;




//         //these Ids are recorded for event unregistration at destruction time and are not maintained (the objects they point to may be destroyed during
//         //the lifetime of the ServerBossRoomState).
//         private List<ulong> m_HeroIds = new List<ulong>();

//         public bool InitialSpawnDone { get; private set; }

//         public override void OnNetworkSpawn(){
                

//             var clientId = m_NetworkManager.LocalClientId;


//             bool didSpawn = DoInitialSpawnIfPossible();

//             if (!didSpawn && InitialSpawnDone &&
//                 !PlayerServerCharacter.GetPlayerServerCharacters().Find(
//                     player => player.OwnerClientId == clientId))
//             {
//                 //somebody joined after the initial spawn. This is a Late Join scenario. This player may have issues
//                 //(either because multiple people are late-joining at once, or because some dynamic entities are
//                 //getting spawned while joining. But that's not something we can fully address by changes in
//                 //ServerBossRoomState.
//                 SpawnPlayer(clientId, true);
//             }
        
//         }

//         private bool DoInitialSpawnIfPossible()
//         {
//             if (!InitialSpawnDone)
//             {
//                 InitialSpawnDone = true;
//                 foreach (var kvp in NetworkManager.ConnectedClients)
//                 {
//                     SpawnPlayer(kvp.Key, false);
//                 }
//                 return true;
//             }
//             return false;
//         }




//         private void SpawnPlayer(ulong clientID,bool lateJoin)
//         {
//             Transform spawnPoint = null;

//             if (m_PlayerSpawnPointsList == null || m_PlayerSpawnPointsList.Count == 0)
//             {
//                 m_PlayerSpawnPointsList = new List<Transform>(m_PlayerSpawnPoints);
//             }

//             Debug.Assert(m_PlayerSpawnPointsList.Count > 0,
//                 $"PlayerSpawnPoints array should have at least 1 spawn points.");

//             int index = Random.Range(0, m_PlayerSpawnPointsList.Count);
//             spawnPoint = m_PlayerSpawnPointsList[index];
//             m_PlayerSpawnPointsList.RemoveAt(index);

//             var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);


//             var newPlayer = Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);

//             var physicsTransform = newPlayer.GetComponent<ServerCharacter>().physicsWrapper.Transform;

//             if (spawnPoint != null)
//             {
//                 physicsTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
//             }

//             var persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
//             Assert.IsTrue(persistentPlayerExists,
//                 $"Matching persistent PersistentPlayer for client {clientID} not found!");

//             // pass character type from persistent player to avatar
//             var networkAvatarGuidStateExists =
//                 newPlayer.TryGetComponent(out NetworkAvatarGuidState networkAvatarGuidState);

//             Assert.IsTrue(networkAvatarGuidStateExists,
//                 $"NetworkCharacterGuidState not found on player avatar!");

//             // if joining late, assign a random character to the persistent player
//             if (lateJoin)
//             {
//                 persistentPlayer.NetworkAvatarGuidState.AvatarGuid.Value =
//                     m_AvatarRegistry.GetSpecifiqueAvatar(0).Guid.ToNetworkGuid();
//             }


//             networkAvatarGuidState.AvatarGuid.Value =
//                 m_AvatarRegistry.GetSpecifiqueAvatar(0).Guid.ToNetworkGuid();



//             var netState = newPlayer.GetComponent<NetworkCharacterState>();

//             m_HeroIds.Add(netState.NetworkObjectId);

//             // spawn players characters with destroyWithScene = true
//             newPlayer.SpawnWithOwnership(clientID,true);
//         }






//     }
// }
