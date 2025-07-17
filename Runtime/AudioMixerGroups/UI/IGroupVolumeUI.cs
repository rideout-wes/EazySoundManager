using System;
using UnityEngine.Audio;

namespace Eazy_Sound_Manager.AudioMixerGroups.UI
{
	public interface IGroupVolumeUI
	{
		public event Action<AudioMixerGroup, bool> MuteChanged;
		public event Action<AudioMixerGroup, float> VolumeChanged;

		public void Initialize(AudioMixerGroup audioMixerGroup, bool muted, float currentVolume);
		public void SetMuteDisplay(bool muted);
		public void SetVolumeDisplay(float volume);
	}
}