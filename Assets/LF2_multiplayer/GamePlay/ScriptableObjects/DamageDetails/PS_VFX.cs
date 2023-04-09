using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LF2.Utils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PS_VFX : MonoBehaviour 
    {
        [SerializeField]
        [Tooltip("If this graphic should automatically Shutdown after a certain time, set it here (in Ticks). -1 means auto-shutdown.")]
        private float m_AutoShutdownTime = -1;

        [SerializeField]
        [Tooltip("Automatically Play after a certain time, set it here (in Ticks). -1 means  Play-on-Awake.")]
        private float m_AutoPlay = -1;

        [SerializeField]
        private bool useFrameCheck;

        // track when Shutdown() is called so we don't try to do it twice
        private bool m_IsShutdown = false;

        // [SerializeField] private Animator animator; 
        private SpriteRenderer spriteRenderer;
        [SerializeField] private  FrameChecker frameChecker;

        private Coroutine coroWaitForSelfDestruct = null;
        private int nbTick;
        private bool _canEnd;
        private bool _canPlay;


        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            frameChecker.InitializeVFX(spriteRenderer);
            frameChecker.initCheck(1);
            // if (m_AutoShutdownTime != -1)
            // {
            //     coroWaitForSelfDestruct = StartCoroutine(CoroWaitForSelfDestruct());
            // }

        }



        private void OnEnable() {
            if (m_AutoPlay == -1){
                frameChecker.initCheck(1);
                _canPlay = true;
            }else{
                StartCoroutine(CoroWaitForPlay());
            }
        }
        private void OnDisable() {
            _canPlay = false;
        }

        private void FixedUpdate() {
            if (_canPlay && nbTick %2 ==0 ){
                frameChecker.CheckFrameVFX(Shutdown);
            }
            nbTick++;

        }

        public void EndFrame(){
            // if (m_AutoShutdownTime == -1 ){
                Shutdown();
            // }else{
            //     _canEnd
            // }
        }


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

        private IEnumerator CoroWaitForPlay()
        {
            yield return new WaitForSeconds(m_AutoPlay);
            _canPlay = true;
        }


        public void InitializeFrameStruct(){
            frameChecker.InitializeFrameStruct();
        }
    }
}


