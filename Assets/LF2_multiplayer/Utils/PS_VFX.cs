using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LF2.Utils
{
    public class PS_VFX : MonoBehaviour , ParticleEffect
    {
        [SerializeField]
        [Tooltip("If this graphic should automatically Shutdown after a certain time, set it here (in seconds). -1 means no auto-shutdown.")]
        private float m_AutoShutdownTime = -1;

        // track when Shutdown() is called so we don't try to do it twice
        private bool m_IsShutdown = false;

        [SerializeField] private Animator animator; 
        // [SerializeField] SpriteRenderer spriteRenderer;
        // [SerializeField] private Sprite[] sprite;

        private Coroutine coroWaitForSelfDestruct = null;

        private void Start()
        {
            if (m_AutoShutdownTime != -1)
            {
                coroWaitForSelfDestruct = StartCoroutine(CoroWaitForSelfDestruct());
            }
        }

        // private void FixedUpdate() {
        //     bool timeExpired = Time.time -  CurrentStateViz.TimeStarted_Animation >= CurrentStateViz.stateData.DurationSeconds;

        // }


        // private IEnumerator CoroRunParticleEffect()
        // {
        //     int i = 0;
        //     while(i < sprite.Length){
        //         spriteRenderer.sprite = sprite[i];
        //         i++;
        //         yield return new WaitForSeconds(0.067f); 

        //     }
        // }

        public void Shutdown()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator CoroWaitForSelfDestruct()
        {
            yield return new WaitForSeconds(m_AutoShutdownTime);
            coroWaitForSelfDestruct = null;
            if (!m_IsShutdown)
            {
                Shutdown();
            }
        }
    }
}


