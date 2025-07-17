namespace Eazy_Sound_Manager.PlaySoundHelpers
{
	public interface IPlaySoundHelper
	{
		float MaxVolumeLevel { get; }

		void Play();
		void Pause();
		void UnPause();
		void Stop();
		void FadeIn();
		void FadeOut(float destinationVolume = 0);
		void Mute();
	}
}