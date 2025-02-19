using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
	public AudioClip[] joinSounds;
	public AudioClip[] buttonSounds;
	public AudioClip[] defeatSounds;
	public AudioClip[] victorySounds;

	public new string name;
	public void SetName(string newName)
	{
		name = newName;
	}
	public abstract void SetTarget(int _target);
	protected abstract void SendTime();
}
