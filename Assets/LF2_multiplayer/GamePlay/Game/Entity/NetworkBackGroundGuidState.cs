// using System;
// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.Serialization;

// namespace LF2
// {
//     /// <summary>
//     /// NetworkBehaviour component to send/receive GUIDs from server to clients.
//     /// </summary>
//     public class NetworkBackGroundGuidState : NetworkBehaviour
//     {
//         [FormerlySerializedAs("AvatarGuidArray")]
//         [HideInInspector]
//         // Use full , for save GUID and Use in between scene 
//         //  From that GUID , we can extrait data from SessionPlayerData 
//         public NetworkVariable<NetworkGuid> BackGroundGuid = new NetworkVariable<NetworkGuid>();        

//         [SerializeField]
//         BackGroundGameRegistry m_BackGroundRegistry;

//         BackGroundGame m_BackGroundGame;

//         public BackGroundGame RegisteredBackGround
//         {
//             get
//             {
//                 if (m_BackGroundGame == null)
//                 {

//                     RegisterAvatar(BackGroundGuid.Value.ToGuid());
//                 }

//                 return m_BackGroundGame;
//             }
//         }


//         public void RegisterAvatar(Guid guid)
//         {

//             if (guid.Equals(Guid.Empty))
//             {
//                 // not a valid Guid
//                 return;
//             }

//             // based on the Guid received, BackGround is fetched from BackGroundRegistry
//             if (!m_BackGroundRegistry.TryGetBackGround(guid, out BackGroundGame background))
//             {
//                 Debug.LogError("BackGround not found!");
//                 return;
//             }

//             if (m_BackGroundGame != null)
//             {
//                 // already set, this is an idempotent call, we don't want to Instantiate twice
//                 return;
//             }

//             m_BackGroundGame = background;
//         }
//     }
// }
