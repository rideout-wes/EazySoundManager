using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Eazy_Sound_Manager.AudioMixerGroups
{
	[Serializable]
	public struct SerializableVolumeData
	{
		public string groupName;
		public string volumeParameter;
		public bool muted;
		public float volume;

		public SerializableVolumeData(string groupName, string volumeParameter, bool muted, float volume)
		{
			this.groupName = groupName;
			this.volumeParameter = volumeParameter;
			this.muted = muted;
			this.volume = volume;
		}
	}

	public class VolumeData
	{
		private AudioMixer _audioMixer;
		private bool _muted;
		private float _volume;

		private string volumeParameter;

		public AudioMixer AudioMixer
		{
			get => _audioMixer;
			set
			{
				_audioMixer = value;
				Volume = Volume; // Reapply the volume
				Muted = Muted; // Reapply the muted state
			}
		}

		public bool Muted
		{
			get => _muted;
			set
			{
				_muted = value;
				float newMixerVolume = Muted ? 0 : Volume;
				SetVolumeOnMixer(newMixerVolume);
			}
		}

		public float Volume
		{
			get => _volume;
			set
			{
				_volume = value;
				SetVolumeOnMixer(Volume);
			}
		}

		public string GroupName { get; private set; }

		public VolumeData(AudioMixer audioMixer, string currentGroupName, string volumeParameter, bool muted = false, float volume = 1f)
		{
			this.volumeParameter = volumeParameter;
			_audioMixer = audioMixer;
			GroupName = currentGroupName;
			Volume = volume;
			Muted = muted;
		}

		public VolumeData(string groupName, string volumeParameter, bool muted, float volume)
		{
			GroupName = groupName;
			this.volumeParameter = volumeParameter;
			_muted = muted;
			_volume = volume;
		}

		public VolumeData(SerializableVolumeData serializableVolumeData) :
			this(serializableVolumeData.groupName, serializableVolumeData.volumeParameter, serializableVolumeData.muted, serializableVolumeData.volume) { }

		public SerializableVolumeData GetSerializableData()
		{
			return new SerializableVolumeData(GroupName, volumeParameter, Muted, Volume);
		}

		private bool SetVolumeOnMixer(float volume)
		{
			if (_audioMixer == null)
				throw new InvalidOperationException("AudioMixer must be set before modifying its volume.");

			volume = Mathf.Clamp01(volume);
			volume = Mathf.Max(volume, 0.0001f);
			volume = Mathf.Log10(volume) * 20f;
			return _audioMixer.SetFloat(volumeParameter, volume);
		}
	}
}