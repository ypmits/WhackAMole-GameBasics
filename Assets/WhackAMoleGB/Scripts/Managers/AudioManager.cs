using System.Collections;
using DG.Tweening;
using UnityEngine;


/**
<summary>
Makes playing audio easy

AudioManager can differentiate between music and soundeffects
AudioManager can control the pitch of your audio
AudioManager can read and write to the PlayerPrefs
	- It uses the strings 'music' and 'soundFFX' for saving the on/off state of the music and soundeffects
	- You can change these strings through the static variables: 'PlayerPrefs_soundffx' and 'PlayerPrefs_music'
AudioManager can attach seperate AudioSources for music and effects through 'AttachAudioSources(GameObject)'
AudioManager can 'Pause' and 'Resume' which will pitch the music

Dependency: DOTween (http://dotween.demigiant.com/)
</summary>
*/
public class AudioManager
{
	public static string PlayerPrefs_soundffx = "soundFFX";
	public static string PlayerPrefs_music = "music";
	private static AudioSource _audioSource;
	private static AudioSource _audioSourceMusic;
	private static bool _hasSoundFFX;
	private static bool _hasMusic;
	private static float _savedMusicPitch = 1f;

	public static float savedMusicPitch { get { return _savedMusicPitch; } }

	/**
	<summary>
	Attaches two audioSources to a gameObject of choice, one for the soundEffects and one for the music
	</summary>
	*/
	public static void AttachAudioSources(GameObject go)
	{
		if (_audioSource == null) _audioSource = go.AddComponent<AudioSource>();
		if (_audioSourceMusic == null) _audioSourceMusic = go.AddComponent<AudioSource>();
	}


	public static void Pause()
	{
		_audioSource.Pause();
		PitchMusic(.3f, .35f);
	}


	public static void Resume()
	{
		_audioSource.Play();
		PitchMusic(1f, 1f);
	}


	/**
	<summary>
	plays a clip through the _audioSource
	</summary> 
	*/
	public static void PlaySound(AudioClip clip, float delay = 0f, float volumeScale = 1f)
	{
		if (!_audioSource || !sound || !clip) return;
		if (delay > 0f) DOVirtual.DelayedCall(delay, () => { APlayClip(clip, volumeScale); });
		else APlayClip(clip, volumeScale);
	}

	public static void PlayClipForDuration(AudioClip clip, float duration, float delay = 0f, float volumeScale = 1f)
	{
		if (!_audioSource || !sound || !clip) return;
		else _audioSource = Camera.main.gameObject.GetComponent<AudioSource>();
		if (delay > 0f)
			DOVirtual.DelayedCall(delay, () => { _audioSource.pitch = clip.length / duration; });
		else _audioSource.pitch = clip.length / duration;
		APlayClip(clip, volumeScale);
	}

	private static void APlayClip(AudioClip clip, float volumeScale = 1f)
	{
		_audioSource.PlayOneShot(clip, volumeScale);
	}

	public static void PlaySoundAtPosition(AudioClip sound, Vector3 position)
	{
		if (!sound) return;
		AudioSource.PlayClipAtPoint(sound, position);
	}


	public static void PauseSound()
	{
		if (!sound) return;
		if (_audioSource.isPlaying)
			_audioSource.Pause();
	}

	public static void StopSound()
	{
		if (!sound) return;
		if (_audioSource.isPlaying)
			_audioSource.Stop();
	}

	/**
	<summary>
	Looks up the music-list for a specific id and plays a clip by using 'audioSource.PlayOneShot(clip)'
	usage: AudioManager.playMusic (AudioID.YouDidItPopupMelody, true, 1);
	</summary> 
	*/
	public static void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
	{
		_audioSourceMusic.loop = loop;
		_audioSourceMusic.volume = volume;
		if (_audioSourceMusic.isPlaying)
		{
			_audioSourceMusic.DOFade(0f, .2f).SetEase(Ease.InOutCubic).OnComplete(() =>
			{
				_audioSourceMusic.Stop();
				_audioSourceMusic.clip = clip;
				if (music) _audioSourceMusic.Play();
				_audioSourceMusic.DOPitch(_savedMusicPitch, .25f).SetEase(Ease.InOutCubic);
				_audioSourceMusic.DOFade(volume, .25f).SetEase(Ease.InOutCubic);
			});
		}
		else
		{
			_audioSourceMusic.pitch = 1f;
			_audioSourceMusic.clip = clip;
			if (music) _audioSourceMusic.Play();
		}
	}

	public static void PitchMusic(float toPitch = 0f, float duration = .4f, Ease ease = Ease.InOutCubic)
	{
		if (!music) return;
		_savedMusicPitch = toPitch;
		if (!_audioSourceMusic.isPlaying) return;
		_audioSourceMusic.DOKill();
		_audioSourceMusic.DOPitch(toPitch, duration).SetEase(ease);
	}

	public static void StopMusic(bool fadeOut = true, float fadeOutDuration = .45f)
	{
		_audioSourceMusic.DOKill();
		if(fadeOut) _audioSourceMusic.DOPitch(0f, fadeOutDuration).SetEase(Ease.InOutCubic);
		else _audioSourceMusic.pitch = 0f;
	}

	public static void ResumeMusic()
	{
		if (!_audioSourceMusic.isPlaying)
		{
			_audioSourceMusic.pitch = 1f;
			_audioSourceMusic.Play();
		}
		_audioSourceMusic.DOKill();
		_audioSourceMusic.DOPitch(_savedMusicPitch, .45f).SetEase(Ease.InOutCubic);
	}

	/**
	<summary>
	Checks if PlayerPrefs has an entry for the 'sound', if so read it and spit it back, if not initialize one and set the sound 'off'
	</summary>
	*/
	public static bool sound
	{
		get
		{
			if (PlayerPrefs.HasKey(PlayerPrefs_soundffx)) { _hasSoundFFX = (PlayerPrefs.GetInt(PlayerPrefs_soundffx) == 0) ? false : true; }
			else
			{
				PlayerPrefs.SetInt(PlayerPrefs_soundffx, 0);
				_hasSoundFFX = false;
			}
			return _hasSoundFFX;
		}
		set
		{
			PlayerPrefs.SetInt(PlayerPrefs_soundffx, value ? 1 : 0);
			_hasSoundFFX = value;
		}
	}


	/**
	<summary>
	Checks if PlayerPrefs has an entry for the 'music', if so read it and spit it back, if not initialize one and set the music 'off'
	</summary>
	*/
	public static bool music
	{
		get
		{
			if (PlayerPrefs.HasKey(PlayerPrefs_music)) { _hasMusic = (PlayerPrefs.GetInt(PlayerPrefs_music) == 0) ? false : true; }
			else
			{
				PlayerPrefs.SetInt(PlayerPrefs_music, 0);
				_hasMusic = false;
			}
			return _hasMusic;
		}
		set
		{
			PlayerPrefs.SetInt(PlayerPrefs_music, value ? 1 : 0);
			_hasMusic = value;
		}
	}
}