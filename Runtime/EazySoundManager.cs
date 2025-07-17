using System;
using System.Collections.Generic;
using Eazy_Sound_Manager.AudioMixerGroups;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Eazy_Sound_Manager
{
	/// <summary>
	///     Static class responsible for playing and managing audio and sounds.
	/// </summary>
	public class EazySoundManager : MonoBehaviour
	{
		[SerializeField] private AudioMixer audioMixer;
		[SerializeField] private AudioMixerGroup defaultAudioMixerGroup;

		private static List<int> keysToRemove = new();

		/// <summary>
		///     The gameobject that the sound manager is attached to
		/// </summary>
		public static GameObject Gameobject
		{
			get { return Instance.gameObject; }
		}

		private static EazySoundManager instance;

		private static AudioMixerGroup defaultGroup;
		private static Dictionary<AudioMixerGroup, SoundGroup> soundGroups;

		private static bool initialized = false;

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			Init();
		}

		private static EazySoundManager Instance => instance;

		/// <summary>
		///     Initialized the sound manager
		/// </summary>
		private void Init()
		{
			if (!initialized)
			{
				InitializeSoundGroups();
				defaultGroup = defaultAudioMixerGroup;

				initialized = true;
				transform.SetParent(null);
				DontDestroyOnLoad(this);
			}
		}

		private void InitializeSoundGroups()
		{
			soundGroups = new Dictionary<AudioMixerGroup, SoundGroup>();
			foreach (AudioMixerGroup soundGroupIdentifier in audioMixer.GetAllAudioMixerGroups())
				soundGroups.Add(soundGroupIdentifier, new SoundGroup(soundGroupIdentifier, Instance.gameObject));
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		/// <summary>
		///     Event triggered when a new scene is loaded
		/// </summary>
		/// <param name="scene">The scene that is loaded</param>
		/// <param name="mode">The scene load mode</param>
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			RemoveNonPersistAudio();
		}

		private void Update()
		{
			foreach (AudioMixerGroup soundGroupType in soundGroups.Keys)
				UpdateAllAudio(soundGroupType);
		}

		private static void UpdateAllAudio(AudioMixerGroup soundGroupType)
		{
			if (!soundGroups.TryGetValue(soundGroupType, out SoundGroup soundGroup))
				return;

			keysToRemove.Clear();
			foreach (KeyValuePair<int, Audio> kvp in soundGroup)
			{
				Audio audio = kvp.Value;
				audio.Update();

				// Remove it if it is no longer active (playing)
				if (!audio.IsPlaying && !audio.Paused)
					keysToRemove.Add(kvp.Key);
			}

			foreach (int keyToRemove in keysToRemove)
				soundGroup.Remove(keyToRemove);
		}

		private static void RemoveNonPersistAudio()
		{
			foreach (AudioMixerGroup soundGroupType in soundGroups.Keys)
				RemoveNonPersistAudio(soundGroupType);
		}

		private static void RemoveNonPersistAudio(AudioMixerGroup soundGroupKey)
		{
			if (!soundGroups.TryGetValue(soundGroupKey, out SoundGroup soundGroup))
				return;

			keysToRemove.Clear();
			foreach (KeyValuePair<int, Audio> kvp in soundGroup)
			{
				Audio audio = kvp.Value;
				if (!audio.Persist && audio.Activated)
					keysToRemove.Add(kvp.Key);
			}

			foreach (int keyToRemove in keysToRemove)
				soundGroup.Remove(keyToRemove);
		}

		private static AudioMixerGroup DefaultGroupIfNull(AudioMixerGroup audioMixerGroup)
		{
			if (audioMixerGroup == null)
				audioMixerGroup = defaultGroup;

			if (audioMixerGroup == null)
				throw new Exception("No audioMixerGroup was provided and no default type found.");

			return audioMixerGroup;
		}

		#region GetAudio Functions

		public static bool TryGetAudio(AudioMixerGroup audioMixerGroup, int audioID, out Audio audio)
		{
			audioMixerGroup = DefaultGroupIfNull(audioMixerGroup);

			if (soundGroups.TryGetValue(audioMixerGroup, out SoundGroup soundGroup))
				return soundGroup.TryGetAudio(audioID, out audio);

			audio = null;
			return false;
		}

		public static bool TryGetAudio(AudioMixerGroup audioMixerGroup, AudioClip audioClip, out Audio audio)
		{
			audioMixerGroup = DefaultGroupIfNull(audioMixerGroup);

			if (soundGroups.TryGetValue(audioMixerGroup, out SoundGroup soundGroup))
				return soundGroup.TryGetAudio(audioClip, out audio);

			audio = null;
			return false;
		}

		#endregion

		#region Prepare Function

		private static int PrepareAudio(AudioMixerGroup audioMixerGroup, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds,
			float fadeOutSeconds, Transform sourceTransform)
		{
			if (clip == null) Debug.LogError("[Eazy Sound Manager] Audio clip is null", clip);

			SoundGroup soundGroup = soundGroups[audioMixerGroup];
			bool ignoreDuplicateAudio = soundGroup.IgnoreDuplicates;

			if (ignoreDuplicateAudio)
			{
				if (TryGetAudio(audioMixerGroup, clip, out Audio duplicateAudio))
					return duplicateAudio.AudioID;
			}

			// Create the audioSource
			Audio audio = soundGroup.GetNewAudio(clip, loop, persist, volume, fadeInSeconds, fadeOutSeconds, sourceTransform);
			return audio.AudioID;
		}

		#endregion

		#region Play Functions

		public static int Play(AudioMixerGroup audioMixerGroup, AudioClip clip, float volume = 1f, bool loop = false, bool persist = false,
			float fadeInSeconds = 0f,
			float fadeOutSeconds = 0f, Transform sourceTransform = null)
		{
			audioMixerGroup = DefaultGroupIfNull(audioMixerGroup);

			int audioID = PrepareAudio(audioMixerGroup, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, sourceTransform);

			if (TryGetAudio(audioMixerGroup, audioID, out Audio audio))
				audio.Play();

			return audioID;
		}

		#endregion

		#region Stop Functions

		public static void StopAll()
		{
			foreach (AudioMixerGroup soundGroupType in soundGroups.Keys)
				StopAll(soundGroupType);
		}

		public static void StopAll(AudioMixerGroup audioMixerGroup)
		{
			StopAll(audioMixerGroup, -1f);
		}

		public static void StopAll(AudioMixerGroup audioMixerGroup, float fadeOutSeconds)
		{
			audioMixerGroup = DefaultGroupIfNull(audioMixerGroup);

			SoundGroup soundGroup = soundGroups[audioMixerGroup];

			foreach (KeyValuePair<int, Audio> kvp in soundGroup)
			{
				Audio audio = kvp.Value;
				if (fadeOutSeconds > 0) audio.FadeOutSeconds = fadeOutSeconds;
				audio.Stop();
			}
		}

		#endregion

		#region Pause Functions

		/// <summary>
		///     Pause all audio playing
		/// </summary>
		public static void PauseAll()
		{
			foreach (AudioMixerGroup soundGroupType in soundGroups.Keys)
				PauseAll(soundGroupType);
		}

		public static void PauseAll(AudioMixerGroup audioMixerGroup)
		{
			audioMixerGroup = DefaultGroupIfNull(audioMixerGroup);

			SoundGroup soundGroup = soundGroups[audioMixerGroup];

			foreach (KeyValuePair<int, Audio> kvp in soundGroup)
			{
				Audio audio = kvp.Value;
				audio.Pause();
			}
		}

		#endregion

		#region Resume Functions

		/// <summary>
		///     Resume all audio playing
		/// </summary>
		public static void ResumeAll()
		{
			foreach (AudioMixerGroup soundGroupType in soundGroups.Keys)
				ResumeAll(soundGroupType);
		}

		private static void ResumeAll(AudioMixerGroup audioMixerGroup)
		{
			audioMixerGroup = DefaultGroupIfNull(audioMixerGroup);

			SoundGroup soundGroup = soundGroups[audioMixerGroup];

			foreach (KeyValuePair<int, Audio> kvp in soundGroup)
			{
				Audio audio = kvp.Value;
				audio.Resume();
			}
		}

		#endregion
	}
}