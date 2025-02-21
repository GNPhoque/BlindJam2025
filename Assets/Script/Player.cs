using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
	public new string name;
	public SOPlayerAnimal animal;

	protected virtual void Start()
	{
		animal = GameManager.instance.GetAnimal();
		SetName();
		AudioManager.instance.PlayRandomSfx(animal.joinSounds);
	}

	void SetName()
	{
		name = animal.GetName();
	}
	public abstract void SetTarget(int _target);
	protected abstract void SendTime();
}
