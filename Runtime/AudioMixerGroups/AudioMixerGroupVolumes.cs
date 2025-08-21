using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Eazy_Sound_Manager.AudioMixerGroups
{
	[CreateAssetMenu(fileName = "AudioMixerGroupVolumes", menuName = "ScriptableObjects/Sounds/AudioMixerGroupVolumes", order = 100)]
	public class AudioMixerGroupVolumes : ScriptableObject
	{
		[SerializeField] private AudioMixer audioMixer;

		[NonSerialized]
		private Dictionary<AudioMixerGroup, VolumeData> groupToVolumeData;

		[field: NonSerialized]
		public IReadOnlyList<AudioMixerGroup> AllAudioMixerGroups { get; private set; }

		public IReadOnlyDictionary<AudioMixerGroup, VolumeData> GroupToVolumeData => groupToVolumeData;

		[NonSerialized]
		private bool initialized;

		public bool Initialize(List<VolumeData> initialVolumeData)
		{
			InitializeExistingVolumeData(initialVolumeData);

			return Initialize(audioMixer.GetAllAudioMixerGroups(), initialVolumeData);
		}

		public bool Initialize(AudioMixerGroup[] audioMixerGroups, List<VolumeData> initialVolumeData)
		{
			if (initialized)
				return false;

			AllAudioMixerGroups = audioMixerGroups;

			groupToVolumeData = new Dictionary<AudioMixerGroup, VolumeData>(audioMixerGroups.Length);
			foreach (AudioMixerGroup currentGroup in audioMixerGroups)
			{
				VolumeData newVolumeData = GetVolumeData(currentGroup.name, initialVolumeData);
				groupToVolumeData.Add(currentGroup, newVolumeData);
			}

			initialized = true;
			return true;
		}

		public bool TrySetMuted(AudioMixerGroup group, bool muted)
		{
			if (!TryGetDataForGroup(group, out VolumeData volumeData))
				return false;

			volumeData.Muted = muted;
			return true;
		}

		public bool TryGetMuted(AudioMixerGroup group, out bool muted)
		{
			muted = default;
			if (!TryGetDataForGroup(group, out VolumeData volumeData))
				return false;

			muted = volumeData.Muted;
			return true;
		}

		public bool TrySetVolume(AudioMixerGroup group, float volume)
		{
			if (!TryGetDataForGroup(group, out VolumeData volumeData))
				return false;

			volumeData.Volume = volume;
			return true;
		}

		public bool TryGetVolume(AudioMixerGroup group, out float volume)
		{
			volume = default;
			if (!TryGetDataForGroup(group, out VolumeData volumeData))
				return false;

			volume = volumeData.Volume;
			return true;
		}

		private void InitializeExistingVolumeData(List<VolumeData> initialVolumeData)
		{
			if (initialVolumeData == null)
				return;

			foreach (VolumeData volumeData in initialVolumeData)
				volumeData.AudioMixer = audioMixer;
		}

		private VolumeData GetVolumeData(string currentGroupName, List<VolumeData> existingData)
		{
			if (existingData == null)
				return GenerateNewVolumeData(currentGroupName);

			VolumeData existingGroupData = existingData.FirstOrDefault(x => x.GroupName == currentGroupName);
			if (existingGroupData == null)
				return GenerateNewVolumeData(currentGroupName);

			return existingGroupData;
		}

		private VolumeData GenerateNewVolumeData(string currentGroupName)
		{
			return new VolumeData(audioMixer, currentGroupName, currentGroupName + "_Volume");
		}

		private bool TryGetDataForGroup(AudioMixerGroup group, out VolumeData volumeData)
		{
			if (!groupToVolumeData.TryGetValue(group, out volumeData))
			{
				Debug.LogWarning("This AudioMixerGroupVolumes was not setup with the given AudioMixerGroup.");
				return false;
			}

			return true;
		}
	}
}