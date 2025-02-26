using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	[SerializeField] AudioClip tictac;
	private AudioSource audioSource;

	private void Awake()
	{
		//Singleton
		if (instance == null)
		{
			instance = this;
			audioSource = GetComponent<AudioSource>();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void PlayRandomSfx(AudioClip[] clips, AudioSource source = null)
	{
		if(clips.Length == 0)
		{
			Debug.LogError("Tried to play a random sound from an empty list of sounds");
			return;
		}

		PlaySfx(clips[Random.Range(0, clips.Length)], source);
	}

	public void PlaySfx(AudioClip clip, AudioSource source = null)
	{
		if(source == null)
		{
			audioSource.PlayOneShot(clip);
		}

		else
		{
			source.PlayOneShot(clip);
		}
	}

	public void PlayTicTac()
	{
		audioSource.PlayOneShot(tictac);
	}
}
