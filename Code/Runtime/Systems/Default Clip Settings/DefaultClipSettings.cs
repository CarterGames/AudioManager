using System;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
	[Serializable]
	public class DefaultClipSettings
	{
		[SerializeField, Range(0f, 1f)] private float volume = 1f;
		[SerializeField, Range(-3f, 3f)] private float pitch = 1f;


		public float Volume => volume;
		public float Pitch => pitch;
	}
}