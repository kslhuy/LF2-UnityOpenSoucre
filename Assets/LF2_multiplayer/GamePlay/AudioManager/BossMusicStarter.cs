using UnityEngine;
using UnityEngine.Assertions;

namespace LF2.Client
{
    /// <summary>
    /// Simple class to restart game theme on main menu load
    /// </summary>
    [RequireComponent(typeof(NetworkLifeState)), RequireComponent(typeof(NetworkStaticsPoints))]
    public class BossMusicStarter : MonoBehaviour
    {
        [SerializeField]
        NetworkLifeState m_NetworkLifeState;

        [SerializeField]
        NetworkStaticsPoints m_NetworkHealthState;

        bool m_Won;

        void Start()
        {
            Assert.IsNotNull(m_NetworkLifeState, "NetworkLifeState not set!");
            Assert.IsNotNull(m_NetworkHealthState, "NetworkHealthState not set!");

            m_NetworkLifeState.LifeState.OnValueChanged += OnLifeStateChanged;
            m_NetworkHealthState.HPPoints.OnValueChanged += OnHealthChanged;
        }

        void OnDestroy()
        {
            var netState = GetComponent<NetworkCharacterState>();
            if (netState != null)
            {
                netState.NetworkLifeState.LifeState.OnValueChanged -= OnLifeStateChanged;
                if (netState.Statics != null)
                {
                    netState.Statics.HPPoints.OnValueChanged -= OnHealthChanged;
                }
            }
        }

        private void OnLifeStateChanged(LifeState previousValue, LifeState newValue)
        {
            if (newValue != LifeState.Alive)
            {
                // players won! Start victory theme
                ClientMusicPlayer.Instance.PlayVictoryMusic();
                m_Won = true;
            }
        }

        private void OnHealthChanged(int previousValue, int newValue)
        {
            // don't do anything if battle is over
            if (m_Won) { return; }

            // make sure battle music started anytime boss is hurt
            if (newValue < previousValue)
            {
                ClientMusicPlayer.Instance.PlayBossMusic();
            }
        }
    }
}
