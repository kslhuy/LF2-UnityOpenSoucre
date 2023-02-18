using System;

using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace LF2.Client{
    public class DamagePopupManager : MonoBehaviour
    {
        [SerializeField] DamagePopupPoolSO _poolDamagePopup;
        [SerializeField] int SizePool;
        [SerializeField] private DamagePopupEventChannelSO _damagePopupEventChannelSO;
        public static Vector3 pivot = new Vector3 (0,40,0);

        private void OnEnable() {
            _damagePopupEventChannelSO.OnDamagePopUpRequested += Create;
        }

        private void OnDisable() {
            _damagePopupEventChannelSO.OnDamagePopUpRequested -= Create;
        }

        public void Create(Vector3 position , int damageAmount)
        {
            if (damageAmount == 0) return;
            DamagePopup damagePopup = _poolDamagePopup.Request();
            // Transform damagePopupTransform =  Instantiate(GameDataSourceNew.Instance.pfDamagePopup  , position + pivot, Quaternion.identity);
            // DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(position + pivot , damageAmount, OnFinishedPopup);
            // damagePopup.OnFinishedPopUp += OnFinishedPopup;
            
        }

        private void OnFinishedPopup(DamagePopup damagePopup)
        {
            _poolDamagePopup.Return(damagePopup);
        }

        private void Awake() {
            _poolDamagePopup.Prewarm(SizePool);
            _poolDamagePopup.SetParent(this.transform);

        }


    }

}
