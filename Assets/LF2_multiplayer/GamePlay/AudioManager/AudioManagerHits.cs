
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

namespace LF2.Client{


	[CreateAssetMenu(menuName = "Audio/AudioManagerHits")]
	public class AudioManagerHits : ScriptableObject
	{
		[Serializable]
		public class AudioByEffectHit {
			public AudioCueSO Sound_Hit;
			public DamageEffect damageEffect; 
		}

		public List<AudioByEffectHit> AudioByEffectHits;
	}
}