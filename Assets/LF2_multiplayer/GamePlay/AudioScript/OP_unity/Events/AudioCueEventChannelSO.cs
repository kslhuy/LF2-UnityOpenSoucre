﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace LF2.Client{

	/// <summary>
	/// Event on which <c>AudioCue</c> components send a message to play SFX and music. <c>AudioManager</c> listens on these events, and actually plays the sound.
	/// </summary>
	[CreateAssetMenu(menuName = "Events/AudioCue Event Channel")]
	public class AudioCueEventChannelSO : EventChannelBaseSO
	{
		public AudioCuePlayAction OnAudioCuePlayRequested;
		public AudioCueStopAction OnAudioCueStopRequested;
		public AudioCueFinishAction OnAudioCueFinishRequested;

		public AudioConfigurationSO _defautAudioConfig;

		public AudioCueKey RaisePlayEvent(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace = default)
		{
			AudioCueKey audioCueKey = AudioCueKey.Invalid;

			if (OnAudioCuePlayRequested != null)
			{
				audioCueKey = OnAudioCuePlayRequested.Invoke(audioCue, audioConfiguration, positionInSpace);
			}
			else
			{
				Debug.LogWarning("An AudioCue play event was requested, but nobody picked it up. " +
					"Check why there is no AudioManager already loaded, " +
					"and make sure it's listening on this AudioCue Event channel.");
			}

			return audioCueKey;
		}

		public AudioCueKey RaisePlayEvent(AudioCueSO audioCue)
		{
			AudioCueKey audioCueKey = AudioCueKey.Invalid;

			if (OnAudioCuePlayRequested != null)
			{
				audioCueKey = OnAudioCuePlayRequested.Invoke(audioCue, _defautAudioConfig, default);
			}
			else
			{
				Debug.LogWarning("An AudioCue play event was requested, but nobody picked it up. " +
					"Check why there is no AudioManager already loaded, " +
					"and make sure it's listening on this AudioCue Event channel.");
			}

			return audioCueKey;
		}

		public bool RaiseStopEvent(AudioCueKey audioCueKey)
		{
			bool requestSucceed = false;

			if (OnAudioCueStopRequested != null)
			{
				requestSucceed = OnAudioCueStopRequested.Invoke(audioCueKey);
			}
			else
			{
				Debug.LogWarning("An AudioCue stop event was requested, but nobody picked it up. " +
					"Check why there is no AudioManager already loaded, " +
					"and make sure it's listening on this AudioCue Event channel.");
			}

			return requestSucceed;
		}

		public bool RaiseFinishEvent(AudioCueKey audioCueKey)
		{
			bool requestSucceed = false;

			if (OnAudioCueStopRequested != null)
			{
				requestSucceed = OnAudioCueFinishRequested.Invoke(audioCueKey);
			}
			else
			{
				Debug.LogWarning("An AudioCue finish event was requested, but nobody picked it up. " +
					"Check why there is no AudioManager already loaded, " +
					"and make sure it's listening on this AudioCue Event channel.");
			}

			return requestSucceed;
		}
	}

	public delegate AudioCueKey AudioCuePlayAction(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace);
	public delegate bool AudioCueStopAction(AudioCueKey emitterKey);
	public delegate bool AudioCueFinishAction(AudioCueKey emitterKey);
}
