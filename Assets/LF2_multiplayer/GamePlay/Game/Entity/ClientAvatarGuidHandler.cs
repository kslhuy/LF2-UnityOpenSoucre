using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2.Client
{
    /// <summary>
    /// Client-side component that awaits a state change on an avatar's Guid, and fetches matching Avatar from the
    /// AvatarRegistry, if possible. Once fetched, the Animator of Graphics  is update.
    /// </summary>
    [RequireComponent(typeof(NetworkAvatarGuidState))]
    public class ClientAvatarGuidHandler : NetworkBehaviour
    {


        [SerializeField]
        NetworkAvatarGuidState m_NetworkAvatarGuidState;

        [SerializeField]
        Animator m_GraphicsAnimator;
        public Animator graphicsAnimator => m_GraphicsAnimator;


        public event Action<GameObject> AvatarGraphicsSpawned;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Debug.Log("huy spawn");
            InstantiateAvatar();
        }

        void InstantiateAvatar()
        {
            m_GraphicsAnimator.runtimeAnimatorController = m_NetworkAvatarGuidState.RegisteredAvatar.AnimatorController;
        //    if (m_GraphicsAnimator.transform.childCount > 0)
        //     {
        //         // we may receive a NetworkVariable's OnValueChanged callback more than once as a client
        //         // this makes sure we don't spawn a duplicate graphics GameObject
        //         return;
        //     }
        //     if (m_NetworkAvatarGuidState.RegisteredAvatar) Instantiate(m_NetworkAvatarGuidState.RegisteredAvatar.Graphics, m_GraphicsAnimator.transform);

        //     m_GraphicsAnimator.Rebind();
        //     m_GraphicsAnimator.Update(0f);

            // AvatarGraphicsSpawned?.Invoke(m_GraphicsAnimator.gameObject);

        }
    }
}
