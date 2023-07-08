using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioSource MusicsSource;
	public AudioSource SoundEffectsSource;

	public static SoundManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}

		Instance = this;

		DontDestroyOnLoad(this.gameObject);
	}

	public void PlayMusic(AudioClip musicClip)
	{
		MusicsSource.clip = musicClip;
		MusicsSource.Play();
	}

	public void PlaySound(AudioClip effectSound)
	{
		SoundEffectsSource.clip = effectSound;
		SoundEffectsSource.Play();
	}
}