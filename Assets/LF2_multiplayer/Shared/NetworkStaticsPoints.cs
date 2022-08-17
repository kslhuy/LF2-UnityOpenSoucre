using System;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// MonoBehaviour containing only one NetworkVariableInt which represents this object's health.
    /// </summary>
    public class NetworkStaticsPoints : NetworkBehaviour
    {
        [HideInInspector]
        public NetworkVariable<int> HPPoints = new NetworkVariable<int>();
        [HideInInspector]
        public NetworkVariable<int> MPPoints = new NetworkVariable<int>();


        // public subscribable event to be invoked when HP has been fully depleted
        public event Action HitPointsDepleted;
        public event Action MPPointsDepleted;
        // public subscribable event to be invoked when HP has been replenished
        public event Action HitPointsReplenished;
        public event Action MPPointsReplenished;

        void OnEnable()
        {
            HPPoints.OnValueChanged += HPPointsChanged;
            MPPoints.OnValueChanged += MPPointsChanged;
        }

        void OnDisable()
        {
            HPPoints.OnValueChanged -= HPPointsChanged;
            MPPoints.OnValueChanged -= MPPointsChanged;
        }

        void HPPointsChanged(int previousValue, int newValue)
        {
            if (previousValue > 0 && newValue <= 0)
            {
                // newly reached 0 HP
                HitPointsDepleted?.Invoke();
            }
            else if (previousValue <= 0 && newValue > 0)
            {
                // newly revived
                HitPointsReplenished?.Invoke();
            }
        }

        void MPPointsChanged(int previousValue, int newValue)
        {
            if (previousValue > 0 && newValue <= 0)
            {
                // newly reached 0 HP
                MPPointsDepleted?.Invoke();
            }
            else if (previousValue <= 0 && newValue > 0)
            {
                // newly revived
                MPPointsReplenished?.Invoke();
            }
        }
    }
}
