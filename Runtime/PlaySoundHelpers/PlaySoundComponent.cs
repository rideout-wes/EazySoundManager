using UnityEngine;

namespace Eazy_Sound_Manager.PlaySoundHelpers
{
	public class PlaySoundComponent : MonoBehaviour, IPlaySoundHelper
	{
		[SerializeField] private PlaySoundScriptable sound;
		[SerializeField] private Transform sourceTransform;

		[SerializeField] private bool muteImmediately;

		[SerializeField] private bool playOnEnable = true;
		[SerializeField] private bool stopOnDisable = true;

		public float MaxVolumeLevel => sound.MaxVolumeLevel;

		public void Play()
		{
			sound.Play(sourceTransform);
		}

		public void Pause()
		{
			sound.Pause();
		}

		public void UnPause()
		{
			sound.UnPause();
		}

		public void FadeIn()
		{
			sound.FadeIn();
		}

		public void FadeOut(float destinationVolume = 0)
		{
			sound.FadeOut();
		}

		public void Mute()
		{
			sound.Mute();
		}

		public void Stop()
		{
			sound.Stop();
		}

		private void OnEnable()
		{
			if (playOnEnable)
				Play_Internal();
		}

		private void Play_Internal()
		{
			sound.Play(sourceTransform);
			if (muteImmediately)
				sound.Mute();
		}

		private void OnDisable()
		{
			if (stopOnDisable)
				sound.Stop();
		}
	}
}