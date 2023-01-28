using LF2.Utils;
using Unity.Multiplayer.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace LF2.Server
{
    /// <summary>
    /// Server-only component which publishes a message once the LifeState changes.
    /// </summary>
    [RequireComponent(typeof(NetworkLifeState))]
    public class PublishMessageOnLifeChange : NetworkBehaviour
    {
        [SerializeField]
        NetworkLifeState m_NetworkLifeState;
        [SerializeField]
        LifeStateEventChannelSO lifeStateEventChannelSO;

        // [SerializeField]
        // string m_CharacterName;

        [SerializeField]
        CharacterClassContainer m_CharacterClass;

        NetworkNameState m_NameState;

        IPublisher<LifeStateChangedEventMessage> m_Publisher;

        [Inject]
        void InjectDependencies(IPublisher<LifeStateChangedEventMessage> publisher)
        {
            m_Publisher = publisher;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                m_NameState = GetComponent<NetworkNameState>();
                m_NetworkLifeState.LifeState.OnValueChanged += OnLifeStateChanged;

                var gameState = FindObjectOfType<ServerLF2State>();
                if (gameState != null)
                {
                    gameState.Container.Inject(this);
                }
            }
        }

        void OnLifeStateChanged(LifeState previousState, LifeState newState)
        {
            
            if (newState == LifeState.Dead){
                // m_Publisher.Publish(new LifeStateChangedEventMessage(){
                //     // CharacterName = m_NameState != null ? m_NameState.Name.Value : (FixedPlayerName)m_CharacterName,
                //     // CharacterType = m_CharacterClass.CharacterClass.CharacterType,

                //     NewLifeState = newState
                // });
                if (m_CharacterClass.CharacterClass.IsNpc){
                    lifeStateEventChannelSO.RaiseEvent_AI(newState);
                }else{
                    lifeStateEventChannelSO.RaiseEvent_Player(newState , OwnerClientId);
                }
            }
        }
    }
}
