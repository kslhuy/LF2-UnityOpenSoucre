using UnityEngine;

namespace LF2.Client{

	[CreateAssetMenu(fileName = "NewSoundEmitterFactory", menuName = "Factory/SoundEmitter Factory")]
	public class SoundEmitterFactorySO : FactorySO<SoundEmitter>
	{
		public SoundEmitter prefab = default;

		public override SoundEmitter Create()
		{
			return Instantiate(prefab);
		}
	}
}
