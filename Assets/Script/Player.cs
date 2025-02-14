using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
	public string name;
	public void SetName(string newName)
	{
		name = newName;
	}
	public abstract void SetTarget(int _target);
	protected abstract void SendTime();
}
