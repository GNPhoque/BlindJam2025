using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Player : MonoBehaviour
{
	public new string name;
	public SOPlayerAnimal animal;
	public AudioSource audioSource;

	private float minPitchVariation = .8f;
	private float maxPitchVariation = 1.2f;
	public float pitchVariation;

	protected virtual void Start()
	{
		audioSource = GetComponent<AudioSource>();

		animal = GameManager.instance.GetAnimal();
		SetName();
		pitchVariation = Random.Range(minPitchVariation, maxPitchVariation);
		audioSource.pitch = pitchVariation;
		AudioManager.instance.PlayRandomSfx(animal.joinSounds, audioSource);
	}

	void SetName()
	{
		name = animal.GetName();
	}
	public abstract void SetTarget(int _target);
	protected abstract void SendTime();
	public abstract IEnumerator Rumble(float duration, AnimationCurve curve = null, bool applyOffset = false);
}
