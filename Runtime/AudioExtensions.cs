using UnityEngine;

namespace Eazy_Sound_Manager
{
	public static class AudioExtensions
	{
		/// <summary>
		///     Sets a <see cref="Audio" /> to behave as either a 2D, or 3D sound, based on whether it has a source GameObject.
		/// </summary>
		public static void SetSpacialBlendForSourceObject(this Audio audio, GameObject sourceObject)
		{
			bool is2D = sourceObject == null;
			audio.SpatialBlend = is2D ? 0 : 1;
		}
	}
}