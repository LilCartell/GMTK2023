using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioSource MusicsSource;
	public AudioSource SoundEffectsSource;

	public AudioClip TitleScreenMusic;
	public AudioClip IntroductionMusic;
	public AudioClip LevelsMusic;
	public AudioClip EndMusic;

	public AudioClip SwapSound;
	public AudioClip RockFallingSound;
	public AudioClip PushyFallingSound;
	public AudioClip ReachedDoorSound;

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
		if(musicClip != null && musicClip != MusicsSource.clip)
        {
			MusicsSource.clip = musicClip;
			MusicsSource.Play();
		}
	}

	public void PlaySound(AudioClip effectSound)
	{
		if(effectSound)
        {
			SoundEffectsSource.clip = effectSound;
			SoundEffectsSource.Play();
		}
	}
}