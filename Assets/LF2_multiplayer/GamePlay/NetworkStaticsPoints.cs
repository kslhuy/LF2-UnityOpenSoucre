using System;
using System.Collections;
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

        [Tooltip("Timer in Tick to regenerate MP")]
        public int timerMPrecover = 60;
        // public float TimerSendMPToServer = 2f;
        private float nbTick ;
        private int cacheMPChange = 500 ;
        private int cacheHPChange = 500;




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
            cacheMPChange = newValue ;
        }

        private void FixedUpdate() {
            nbTick ++;
            if (IsServer){
                if (MPPoints.Value >= 500 ) return;
                if ( nbTick % timerMPrecover == 0  ){
                    // timeNow = Time.time;
                    MPPoints.Value += 10; 
                }
            }
            // if (Time.time - timeNowSendtoServer > TimerSendMPToServer){
            //     timeNowSendtoServer = Time.time;
            //     cacheMPChange += 1; 
            // }
        }

        // IEnumerator CORO_SendMPToserver(float time){
        //     yield return new WaitForSeconds(time);
        //     MPPoints.Value = cacheMPChange ;
            
        // }
    }
}
