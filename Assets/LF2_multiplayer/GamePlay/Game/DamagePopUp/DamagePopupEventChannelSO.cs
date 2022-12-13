using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace LF2.Client{

	/// <summary>
	/// Event on which <c>DamagePopup</c> components send a message to play SFX and music. <c>AudioManager</c> listens on these events, and actually plays the sound.
	/// </summary>
	[CreateAssetMenu(menuName = "Events/DamagePopup Event Channel")]
	public class DamagePopupEventChannelSO : EventChannelBaseSO
	{
		public DamagePopupAction OnDamagePopUpRequested;


		public void RaisePopUpEvent(Vector3 positionInSpace , int damageAmount)
		{

			if (OnDamagePopUpRequested != null)
			{
				OnDamagePopUpRequested.Invoke(positionInSpace, damageAmount);
			}
			else
			{
				Debug.LogWarning("An DamagePopup play event was requested, but nobody picked it up. " +
					"Check why there is no AudioManager already loaded, " +
					"and make sure it's listening on this DamagePopup Event channel.");
			}

		}


	}

	public delegate void DamagePopupAction( Vector3 positionInSpace , int damageAmount);

}
