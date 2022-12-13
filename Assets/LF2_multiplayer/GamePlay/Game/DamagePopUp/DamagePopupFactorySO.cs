using UnityEngine;

namespace LF2.Client{

	[CreateAssetMenu(fileName = "NewDamagePopupFactory", menuName = "Factory/DamagePopup Factory")]
	public class DamagePopupFactorySO : FactorySO<DamagePopup>
	{
		public DamagePopup prefab = default;

		public override DamagePopup Create()
		{
			return Instantiate(prefab);
		}
	}
}
