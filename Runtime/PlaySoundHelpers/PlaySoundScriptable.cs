using UnityEngine;
using UnityEngine.Audio;

namespace Eazy_Sound_Manager.PlaySoundHelpers
{
	[CreateAssetMenu(fileName = "PlaySound", menuName = "ScriptableObjects/Sounds/PlaySound", order = 100)]
	public class PlaySoundScriptable : ScriptableObject, IPlaySoundHelper
	{
		[SerializeField] private AudioClip audioClip;
		[SerializeField] private AudioMixerGroup audioMixerGroup;
		[SerializeField] [Range(0f, 1f)] private float volume = 1f;
		[SerializeField] private bool loop;
		[SerializeField] private bool persist;
		[SerializeField] private bool restartOnPlay;
		[SerializeField] private bool restartOnFadeIn;

		[SerializeField] private float fadeInDuration;
		[SerializeField] private float fadeOutDuration;

		private int playingAudioId = int.MinValue;
		private Audio playingAudio;

		public float MaxVolumeLevel => volume;

		public void Play()
		{
			Play(null);
		}

		public void Play(GameObject sourceObject)
		{
			if (!EazySoundManager.TryGetAudio(audioMixerGroup, playingAudioId, out playingAudio))
			{
				playingAudioId = EazySoundManager.Play(audioMixerGroup, audioClip, volume, loop, persist, fadeInDuration, fadeOutDuration, sourceObject);
				EazySoundManager.TryGetAudio(audioMixerGroup, playingAudioId, out playingAudio);
				return;
			}

			if (restartOnPlay)
				playingAudio.ResetTime();

			if (playingAudio.Paused)
				playingAudio.UnPause();
			else
				playingAudio.Play();
		}

		public void Pause()
		{
			playingAudio?.Pause();
		}

		public void UnPause()
		{
			playingAudio?.UnPause();
		}

		public void Stop()
		{
			playingAudio?.Stop();
		}

		public void FadeIn()
		{
			if (playingAudio == null)
			{
				Play();
				playingAudio.SetVolume(0);
			}

			if (restartOnFadeIn)
				playingAudio.ResetTime();

			playingAudio.SetVolume(volume, fadeInDuration);
		}

		public void FadeOut(float destinationVolume = 0)
		{
			playingAudio?.SetVolume(destinationVolume, fadeOutDuration);
		}

		public void Mute()
		{
			playingAudio?.SetVolume(0);
		}
	}
}