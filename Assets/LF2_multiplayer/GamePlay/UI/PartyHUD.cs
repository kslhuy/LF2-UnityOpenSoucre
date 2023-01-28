using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LF2.Client;
using UnityEngine.Assertions;
using LF2.Utils;

namespace LF2.Gameplay.UI
{
    /// <summary>
    /// Provides logic for the Party HUD with information on the player and allies
    /// Party HUD shows hero portrait and class info for all ally characters
    /// Party HUD also shows healthbars for each player allows clicks to select an ally
    /// </summary>
    public class PartyHUD : MonoBehaviour
    {
        [SerializeField]
        ClientPlayerAvatarRuntimeCollection m_PlayerAvatars;

        [SerializeField]
        private Image[] m_HeroPortraits;

        [SerializeField]
        private GameObject[] m_AllyPanel;


        [SerializeField]
        private TextMeshProUGUI[] m_PartyNames;

        // [SerializeField]
        // private Image[] m_PartyClassSymbols;

        [SerializeField]
        private Slider[] m_PartyHealthSliders;

        [SerializeField]
        private Slider[] m_PartyManaSliders;

        // track a list of hero (slot 0) + allies
        private ulong[] m_PartyIds;

        // track Hero's target to show when it is the Hero or an ally
        private ulong m_CurrentTarget;

        NetworkCharacterState m_OwnedCharacterState;

        ClientPlayerAvatar m_OwnedPlayerAvatar;

        private Dictionary<ulong, NetworkCharacterState> m_TrackedAllies = new Dictionary<ulong, NetworkCharacterState>();

        // private Client.ClientInputSender m_ClientSender;

        void Awake()
        {
            // Make sure arrays are initialized
            InitPartyArrays();

            m_PlayerAvatars.ItemAdded += PlayerAvatarAdded;
            m_PlayerAvatars.ItemRemoved += PlayerAvatarRemoved;
        }

        void PlayerAvatarAdded(ClientPlayerAvatar clientPlayerAvatar)
        {
            if (clientPlayerAvatar.IsOwner)
            {
                SetHeroData(clientPlayerAvatar);
            }
            else
            {
                SetAllyData(clientPlayerAvatar);
            }
        }

        void PlayerAvatarRemoved(ClientPlayerAvatar clientPlayerAvatar)
        {
            if (m_OwnedPlayerAvatar == clientPlayerAvatar)
            {
                RemoveHero();
            }
            else if (m_TrackedAllies.ContainsKey(clientPlayerAvatar.NetworkObjectId))
            {
                RemoveAlly(clientPlayerAvatar.NetworkObjectId);
                m_TrackedAllies.Remove(clientPlayerAvatar.NetworkObjectId);
            }
        }

        void SetHeroData(ClientPlayerAvatar clientPlayerAvatar)
        {
            var networkCharacterStateExists =
                clientPlayerAvatar.TryGetComponent(out NetworkCharacterState networkCharacterState);

            Assert.IsTrue(networkCharacterStateExists,
                "NetworkCharacterState component not found on ClientPlayerAvatar");

            m_OwnedPlayerAvatar = clientPlayerAvatar;
            m_OwnedCharacterState = networkCharacterState;

            // Hero is always our slot 0
            m_PartyIds[0] = m_OwnedCharacterState.NetworkObject.NetworkObjectId;

            // set hero portrait
            if (m_OwnedCharacterState.TryGetComponent(out NetworkAvatarGuidState avatarGuidState))
            {
                m_HeroPortraits[0].sprite = avatarGuidState.RegisteredAvatar.Portrait;
                // m_PartyNames[0].text = avatarGuidState.RegisteredAvatar.CharacterClass.CharacterType.ToString();
            }

            SetUIFromSlotData(0, m_OwnedCharacterState);

            m_OwnedCharacterState.Statics.HPPoints.OnValueChanged += SetHeroHP;
            m_OwnedCharacterState.Statics.MPPoints.OnValueChanged += SetHeroMP;


            // plus we track their target
            // m_OwnedCharacterState.TargetId.OnValueChanged += OnHeroSelectionChanged;

            // m_ClientSender = m_OwnedCharacterState.GetComponent<ClientInputSender>();
        }

        void SetHeroHP(int previousValue, int newValue)
        {
            m_PartyHealthSliders[0].value = newValue;
        }
        void SetHeroMP(int previousValue, int newValue)
        {
            m_PartyManaSliders[0].value = newValue;
        }

        /// <summary>
        /// Gets Player Name from the NetworkObjectId of his controlled Character.
        /// </summary>
        string GetPlayerName(Component component)
        {
            var networkName = component.GetComponent<NetworkNameState>();
            return networkName.Name.Value;
        }

        // set the class type for an ally - allies are tracked  by appearance so you must also provide appearance id
        void SetAllyData(ClientPlayerAvatar clientPlayerAvatar)
        {
            var networkCharacterStateExists =
                clientPlayerAvatar.TryGetComponent(out NetworkCharacterState networkCharacterState);

            Assert.IsTrue(networkCharacterStateExists,
                "NetworkCharacterState component not found on ClientPlayerAvatar");

            ulong id = networkCharacterState.NetworkObjectId;
            int slot = FindOrAddAlly(id);
            Debug.Log("SetAllyData slot " + slot );

            // do nothing if not in a slot
            if (slot == -1)
            {
                return;
            }

            SetUIFromSlotData(slot, networkCharacterState);

            // set hero portrait
            if (networkCharacterState.TryGetComponent(out NetworkAvatarGuidState avatarGuidState))
            {
                m_HeroPortraits[slot].sprite = avatarGuidState.RegisteredAvatar.Portrait;
                // m_PartyNames[slot].text = avatarGuidState.RegisteredAvatar.CharacterClass.CharacterType.ToString();

            }

            networkCharacterState.Statics.HPPoints.OnValueChanged += (int previousValue, int newValue) =>
            {
                SetAllyHealth(slot, newValue);
            };
            networkCharacterState.Statics.MPPoints.OnValueChanged += (int previousValue, int newValue) =>
            {
                SetAllyMP(slot, newValue);
            };

            m_TrackedAllies.Add(networkCharacterState.NetworkObjectId, networkCharacterState);
        }

        void SetUIFromSlotData(int slot, NetworkCharacterState netState)
        {
            m_PartyHealthSliders[slot].maxValue = netState.CharacterClass.BaseHP.Value;
            m_PartyHealthSliders[slot].value = netState.HPPoints;

            m_PartyManaSliders[slot].maxValue = netState.CharacterClass.BaseMP.Value;
            m_PartyManaSliders[slot].value = netState.MPPoints;
            m_PartyNames[slot].text = GetPlayerName(netState);

        }

        void SetAllyHealth(int slot, int hp)
        {
            m_PartyHealthSliders[slot].value = hp;
        }

        void SetAllyMP(int slot, int hp)
        {
            m_PartyManaSliders[slot].value = hp;
        }


        // private void OnHeroSelectionChanged(ulong prevTarget, ulong newTarget)
        // {
        //     SetHeroSelectFX(m_CurrentTarget, false);
        //     SetHeroSelectFX(newTarget, true);
        // }

        // Helper to change name appearance for selected or unselected party members
        // also updates m_CurrentTarget
        // private void SetHeroSelectFX(ulong target, bool selected)
        // {
        //     // check id against all party slots
        //     int slot = FindOrAddAlly(target, true);
        //     // Debug.Log(slot);
        //     if (slot >= 0)
        //     {
        //         m_PartyNames[slot].color = selected ? Color.green : Color.white;
        //         if (selected)
        //         {
        //             m_CurrentTarget = target;
        //         }
        //         else
        //         {
        //             m_CurrentTarget = 0;
        //         }
        //     }
        // }

        // public void SelectPartyMember(int slot)
        // {
        //     m_ClientSender.RequestAction(ActionType.GeneralTarget, Client.ClientInputSender.SkillTriggerStyle.UI, m_PartyIds[slot]);
        // }

        // helper to initialize the Allies array - safe to call multiple times
        private void InitPartyArrays()
        {
            if (m_PartyIds == null)
            {
                // clear party ID array
                m_PartyIds = new ulong[m_PartyHealthSliders.Length];

                for (int i = 0; i < m_PartyHealthSliders.Length; i++)
                {
                    // initialize all IDs positions to 0 and HP to 1000 on sliders
                    m_PartyIds[i] = 0;
                    m_PartyHealthSliders[i].maxValue = 1000;
                }
            }
        }

        // Helper to find ally slots, returns -1 if no slot is found for the id
        // If a slot is available one will be added for this id unless dontAdd=true
        private int FindOrAddAlly(ulong id, bool dontAdd = false)
        {
            // make sure allies array is ready
            InitPartyArrays();

            int openslot = -1;
            for (int i = 0; i < m_PartyIds.Length; i++)
            {
                // if this ID is in the list, return the slot index
                if (m_PartyIds[i] == id) { return i; }
                // otherwise, record the first open slot (not slot 0 thats for the Hero)
                if (openslot == -1 && i > 0 && m_PartyIds[i] == 0)
                {
                    openslot = i;
                }
            }

            // if we don't add, we are done nw and didnt fint the ID
            if (dontAdd) { return -1; }

            // Party slot was not found for this ID - add one in the open slot
            if (openslot > 0)
            {
                // activeate the correct ally panel
                m_AllyPanel[openslot - 1].SetActive(true);
                // and save ally ID to party array
                m_PartyIds[openslot] = id;
                return openslot;
            }

            // this should not happen unless there are too many players - we didn't find the ally or a slot
            return -1;
        }

        void RemoveHero()
        {
            if (m_OwnedCharacterState && m_OwnedCharacterState.Statics)
            {
                m_OwnedCharacterState.Statics.HPPoints.OnValueChanged -= SetHeroHP;
                m_OwnedCharacterState.Statics.MPPoints.OnValueChanged -= SetHeroMP;
            }

            m_OwnedCharacterState = null;
        }

        /// <summary>
        /// Remove an ally from the PartyHUD UI.
        /// </summary>
        /// <param name="id"> NetworkObjectID of the ally. </param>
        void RemoveAlly(ulong id)
        {
            for (int i = 0; i < m_PartyIds.Length; i++)
            {
                // if this ID is in the list, return the slot index
                if (m_PartyIds[i] == id)
                {
                    m_AllyPanel[i - 1].SetActive(false);
                    // and save ally ID to party array
                    m_PartyIds[i] = 0;
                    return;
                }
            }
            int slot = FindOrAddAlly(id, true);

            if (m_TrackedAllies.TryGetValue(id, out NetworkCharacterState networkCharacterState))
            {
                networkCharacterState.Statics.HPPoints.OnValueChanged -= (int previousValue, int newValue) =>
                {
                    SetAllyHealth(slot, newValue);
                };
            }
        }

        void OnDestroy()
        {
            m_PlayerAvatars.ItemAdded -= PlayerAvatarAdded;
            m_PlayerAvatars.ItemRemoved -= PlayerAvatarRemoved;

            RemoveHero();
            foreach (var kvp in m_TrackedAllies)
            {
                RemoveAlly(kvp.Key);
            }
        }
    }
}
