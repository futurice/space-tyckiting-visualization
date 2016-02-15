using UnityEngine;
using System.Collections;

public class SoundEffectPlayer : MonoBehaviour 
{
	public static SoundEffectPlayer Instance { get; private set; }

	public static bool soundsOn;

	[SerializeField]
	private AudioClip missile;
	[SerializeField]
	private AudioClip explosion;
	[SerializeField]
	private AudioClip explosionLarge;
	[SerializeField]
	private AudioClip radar;

	private SoundEffect missileEffect;
	private SoundEffect explosionEffect;
	private SoundEffect explosionLargeEffect;
	private SoundEffect radarEffect;

	private class SoundEffect
	{
		public AudioClip clip;
		public float minPlayInterval = 0.1f;

		private float lastStartedPlaying;

		public bool CanPlay
		{
			get
			{
				return Time.time - lastStartedPlaying > minPlayInterval;
			}
		}

		public void Play()
		{
			if (CanPlay) 
			{
				lastStartedPlaying = Time.time;
				AudioSource.PlayClipAtPoint (clip, Vector3.zero);
			}
		}
	}

	void Awake()
	{
		Instance = this;

		missileEffect = new SoundEffect (){ clip = missile };
		explosionEffect = new SoundEffect (){ clip = explosion };
		explosionLargeEffect = new SoundEffect (){ clip = explosionLarge };
		radarEffect = new SoundEffect (){ clip = radar };
	}

	public void PlayMissile()
	{
		if (soundsOn) missileEffect.Play ();
	}

	public void PlayExplosion()
	{
		if (soundsOn) explosionEffect.Play ();
	}

	public void PlayExplosionLarge()
	{
		if (soundsOn) explosionLargeEffect.Play ();
	}

	public void PlayRadar()
	{
		if (soundsOn) radarEffect.Play ();
	}
}
