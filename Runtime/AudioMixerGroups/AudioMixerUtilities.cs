using UnityEngine.Audio;

namespace Eazy_Sound_Manager.AudioMixerGroups
{
	public static class AudioMixerUtilities
	{
		public static AudioMixerGroup[] GetAllAudioMixerGroups(this AudioMixer mixer)
		{
			return mixer.FindMatchingGroups("");
		}
	}
}