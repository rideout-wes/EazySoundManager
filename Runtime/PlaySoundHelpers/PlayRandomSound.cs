using UnityEngine;

namespace Eazy_Sound_Manager.PlaySoundHelpers
{
	[CreateAssetMenu(fileName = "PlayRandomSound", menuName = "ScriptableObjects/Sounds/PlayRandomSound", order = 100)]
	public class PlayRandomSound : ScriptableObject
	{
		[SerializeField] private PlaySoundScriptable[] sounds;

		public void Play()
		{
			int index = sounds.Length == 1 ? 0 : Random.Range(0, sounds.Length);
			sounds[index].Play();
		}
	}
}