using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	[SerializeField] AudioClip tictac;
	[SerializeField] AudioClip[] pressButtonToJoin;
	[SerializeField] AudioClip[] nextRound;
	[SerializeField] AudioClip[] winner;
	[SerializeField] AudioClip[] nobodyWins;

	[SerializeField] AudioClip[] time10s;
	[SerializeField] AudioClip[] time15s;
	[SerializeField] AudioClip[] time20s;

	private AudioSource audioSource;
	public Action OnClipFinishedPlaying;

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

	public void PlayRandomSfx(AudioClip[] clips, AudioSource source = null, bool triggerEventOnEnd = false)
	{
		if(clips.Length == 0)
		{
			Debug.LogError("Tried to play a random sound from an empty list of sounds");
			return;
		}

		PlaySfx(clips[Random.Range(0, clips.Length)], source, triggerEventOnEnd);
	}

	public void PlaySfx(AudioClip clip, AudioSource source = null, bool triggerEventOnEnd = false)
	{
		if(source == null)
		{
			audioSource.PlayOneShot(clip);
		}

		else
		{
			source.PlayOneShot(clip);
		}

		if (triggerEventOnEnd)
		{
			StartCoroutine(TriggerEventAfterClipFinishedPlaying(clip));
		}
	}

	public void PlayTicTac()
	{
		audioSource.PlayOneShot(tictac);
	}

	public void PlayJoinNextRound()
	{
		PlayRandomSfx(pressButtonToJoin, null, true);
	}

	public void Play10Seconds()
	{
		PlayRandomSfx(time10s, null, true);
	}

	public void Play15Seconds()
	{
		PlayRandomSfx(time15s, null, true);
	}

	public void Play20Seconds()
	{
		PlayRandomSfx(time20s, null, true);
	}

	public void PlayNextRound()
	{
		PlayRandomSfx(nextRound, null, true);
	}

	public void PlayWinner()
	{
		PlayRandomSfx(winner, null, true);
	}

	public void PlayNobodyWins()
	{
		PlayRandomSfx(nobodyWins, null, true);
	}

	IEnumerator TriggerEventAfterClipFinishedPlaying(AudioClip clip)
	{
		yield return new WaitForSeconds(clip.length);
		OnClipFinishedPlaying?.Invoke();
	}
}
