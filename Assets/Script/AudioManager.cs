using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

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

	public void PlayRandomSfx(AudioClip[] clips)
	{
		if(clips.Length == 0)
		{
			Debug.LogError("Tried to play a random sound from an empty list of sounds");
			return;
		}

		PlaySfx(clips[Random.Range(0, clips.Length)]);
	}

	public void PlaySfx(AudioClip clip)
	{
		audioSource.PlayOneShot(clip);
	}
}
