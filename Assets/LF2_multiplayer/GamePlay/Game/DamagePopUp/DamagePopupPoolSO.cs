using UnityEngine;
using System.Linq;
using LF2;
using TMPro;

namespace LF2.Client{
    [CreateAssetMenu(fileName = "NewDamagePopupPool", menuName = "Pool/DamagePopup Pool")]

    public class DamagePopupPoolSO : ComponentPoolSO<DamagePopup>
    {
        [SerializeField]
        private DamagePopupFactorySO _factory;

        public override IFactory<DamagePopup> Factory
        {
            get
            {
                return _factory;
            }
            set
            {
                _factory = value as DamagePopupFactorySO;
            }
        }
    }
}

