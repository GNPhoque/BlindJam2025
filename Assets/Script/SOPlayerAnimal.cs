using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOPlayerAnimal : ScriptableObject
{
	public AudioClip[] joinSounds;
	public AudioClip[] buttonSounds;
	public AudioClip[] defeatSounds;
	public AudioClip[] victorySounds;

	public string[] names;

	public string GetName()
	{
		return names[Random.Range(0, names.Length)];
	}
}
