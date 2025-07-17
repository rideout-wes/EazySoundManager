using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Eazy_Sound_Manager.AudioMixerGroups.UI
{
	[CreateAssetMenu(fileName = "GroupVolumesUIManager", menuName = "ScriptableObjects/Sounds/GroupVolumesUIManager", order = 100)]
	public class GroupVolumesUIManager : ScriptableObject
	{
		[SerializeField] private AudioMixerGroupVolumes audioMixerGroupVolumes;

		public void Initialize(IGroupVolumeUIFactory groupVolumeUIFactory)
		{
			IReadOnlyList<AudioMixerGroup> audioMixerGroups = audioMixerGroupVolumes.AllAudioMixerGroups;

			foreach (AudioMixerGroup audioMixerGroup in audioMixerGroups)
			{
				audioMixerGroupVolumes.TryGetVolume(audioMixerGroup, out float currentVolume);
				audioMixerGroupVolumes.TryGetMuted(audioMixerGroup, out bool muted);

				IGroupVolumeUI groupVolumeUI = groupVolumeUIFactory.Create();
				groupVolumeUI.Initialize(audioMixerGroup, muted, currentVolume);
				groupVolumeUI.MuteChanged += OnMutedChanged;
				groupVolumeUI.VolumeChanged += OnVolumeSliderValueChanged;
			}
		}

		private void OnMutedChanged(AudioMixerGroup audioMixerGroup, bool newMuted)
		{
			audioMixerGroupVolumes.TrySetMuted(audioMixerGroup, newMuted);
		}

		private void OnVolumeSliderValueChanged(AudioMixerGroup audioMixerGroup, float newVolume)
		{
			audioMixerGroupVolumes.TrySetVolume(audioMixerGroup, newVolume);
		}
	}
}