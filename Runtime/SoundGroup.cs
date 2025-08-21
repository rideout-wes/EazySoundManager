using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace Eazy_Sound_Manager
{
	public class SoundGroup : IEnumerable<KeyValuePair<int, Audio>>
	{
		private AudioMixerGroup audioMixerGroup;
		private GameObject defaultAudioSourceObject;

		private Dictionary<int, Audio> groupAudios;
		private ObjectPool<Audio> audioPool;

		public bool IgnoreDuplicates { get; set; }

		public SoundGroup(AudioMixerGroup audioMixerGroup, GameObject defaultAudioSourceObject)
		{
			this.audioMixerGroup = audioMixerGroup;
			this.defaultAudioSourceObject = defaultAudioSourceObject;

			groupAudios = new Dictionary<int, Audio>();
			audioPool = new ObjectPool<Audio>(CreateFunc_Audio, ActionOnGet_Audio, ActionOnRelease_Audio, ActionOnDestroy_Audio);
		}

		public Audio GetNewAudio(AudioClip clip, bool loop, bool persist, float volume, float fadeInValue, float fadeOutValue, GameObject sourceObject)
		{
			Audio audio = audioPool.Get();
			AudioSource audioSourceToUse = AddNewAudioSource(sourceObject);
			audio.Initialize(audioSourceToUse, clip, loop, persist, volume, fadeInValue, fadeOutValue);
			audio.SetSpacialBlendForSourceObject(sourceObject);
			
			groupAudios.Add(audio.AudioID, audio);

			return audio;
		}

		public bool TryGetAudio(int audioID, out Audio returnValue)
		{
			return groupAudios.TryGetValue(audioID, out returnValue);
		}

		public bool TryGetAudio(AudioClip clip, out Audio returnValue)
		{
			returnValue = null;
			foreach (Audio audio in groupAudios.Values)
			{
				if (audio.Clip == clip)
				{
					returnValue = audio;
					return true;
				}
			}

			return false;
		}

		public bool Remove(int audioID)
		{
			if (!groupAudios.TryGetValue(audioID, out Audio audio))
				return false;

			audioPool.Release(audio);
			groupAudios.Remove(audioID);

			return true;
		}

		public Audio this[int id] => groupAudios[id];

		private AudioSource AddNewAudioSource(GameObject sourceObject)
		{
			GameObject objectForAudioSource = sourceObject != null ? sourceObject : defaultAudioSourceObject;
			AudioSource audioSource = objectForAudioSource.AddComponent<AudioSource>();

			audioSource.outputAudioMixerGroup = audioMixerGroup;

			return audioSource;
		}

		#region AudioPool

		private Audio CreateFunc_Audio()
		{
			return new Audio();
		}

		private void ActionOnGet_Audio(Audio obj)
		{
			obj.GenerateNewAudioID();
		}

		private void ActionOnRelease_Audio(Audio obj)
		{
			obj.Clip = null;
		}

		private void ActionOnDestroy_Audio(Audio obj)
		{
			Object.Destroy(obj.AudioSource);
		}

		#endregion

		public IEnumerator<KeyValuePair<int, Audio>> GetEnumerator()
		{
			return groupAudios.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return groupAudios.GetEnumerator();
		}
	}
}