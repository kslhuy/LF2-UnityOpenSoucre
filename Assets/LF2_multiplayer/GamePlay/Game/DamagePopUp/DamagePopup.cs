using System;

using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace LF2.Client{
    public class DamagePopup : MonoBehaviour
    {
        public float moveYSpeed = 20f;

        [SerializeField] private TextMeshPro textMeshPro;
        private float disappearTimer;
        [SerializeField] private Color textColorHit;
        [SerializeField] private Color textColorHeal;
        private Color textColorNow;


		public Action<DamagePopup> OnFinishedPopUp;
        private bool startMove;

        public void Setup(Vector3 position , int damageAmount , Action<DamagePopup> finshPopup){
            transform.position = position;
            textMeshPro.SetText(damageAmount.ToString());
            if (damageAmount > 0) textMeshPro.color = textColorHeal;
            else textMeshPro.color = textColorHit;
            textColorNow = textMeshPro.color;
            disappearTimer = 1f;
            startMove = true;
            OnFinishedPopUp = finshPopup;
        }


        // Update is called once per frame
        void Update()
        {
            if (startMove){

                transform.position += new Vector3 (0,moveYSpeed) * Time.deltaTime;
                disappearTimer -= Time.deltaTime; 
                if (disappearTimer < 0){
                    // Start disappearing 
                    float disappearSpeed  = 3f ; 
                    textColorNow.a -= disappearSpeed * Time.deltaTime;
                    textMeshPro.color = textColorNow;
                    if (textColorNow.a < 0){
                        // Destroy(gameObject);
                        startMove = false;
                        // OnFinishedPopUp?.Invoke(this);
                        OnFinishedPopUp(this);

                    }
                }
            }
        }
    }

}
