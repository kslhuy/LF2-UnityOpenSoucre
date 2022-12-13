using Cinemachine;
using UnityEngine;

namespace LF2.Client{

    public class CameraShake : MonoBehaviour {

        public static CameraShake Instance{get ; private set;}
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin ;

        private float shakeTimer;

        private void Awake() {
            Instance = this;
        }
        private void Start() {
            
            cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            // Debug.Log(cinemachineBasicMultiChannelPerlin);

        }
        public void ShakeCamera(float intensity , float time){

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            // cinemachineBasicMultiChannelPerlin.m_PivotOffset = ()
            shakeTimer = time;
        }

        private void Update() {
            if (shakeTimer > 0){
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0f){
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
                }
            }            
        }
    }

}
