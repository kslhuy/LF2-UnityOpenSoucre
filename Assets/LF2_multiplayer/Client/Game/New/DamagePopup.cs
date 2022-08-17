using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LF2.Client{
    public class DamagePopup : MonoBehaviour
    {
        public static Vector3 pivot = new Vector3 (0,40,0); 
        public static void Create(Vector3 position , int damageAmount)
        {
            Transform damagePopupTransform =  Instantiate(GameDataSourceNew.Instance.pfDamagePopup  , position + pivot, Quaternion.identity);
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(damageAmount);
            
        }


        public float moveYSpeed = 20f;

        private TextMeshPro textMeshPro;
        private float disappearTimer;
        private Color textColor;


        private void Awake() {
            textMeshPro = transform.GetComponent<TextMeshPro>();    
        }
        public void Setup(int damageAmount){
            textMeshPro.SetText(damageAmount.ToString());
            textColor = textMeshPro.color;
            disappearTimer = 1f;
        }


        // Update is called once per frame
        void Update()
        {
            transform.position += new Vector3 (0,moveYSpeed) * Time.deltaTime;
            disappearTimer -= Time.deltaTime; 
            if (disappearTimer < 0){
                // Start disappearing 
                float disappearSpeed  = 3f ; 
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMeshPro.color = textColor;
                if (textColor.a < 0){
                    Destroy(gameObject);
                }
            }
        }
    }

}
