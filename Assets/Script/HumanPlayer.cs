using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
	KeyCode key;

	private void Start()
	{
		Inputs.instance.PushedOk += SendTime;
	}

	void OnDestroy()
	{
		Inputs.instance.PushedOk -= SendTime;
	}

	public void SetKey(KeyCode newKey)
	{
		key = newKey;
		Inputs.instance.OnPlayerKeyPushed += SendTimeIfCorrectKey;
	}

	public override void SetTarget(int _target)
	{
	}

	void SendTimeIfCorrectKey(KeyCode code)
	{
		if(code == key)
		{
			SendTime();
		}
	}

	protected override void SendTime()
	{
		GameManager.instance.SetPlayerTime(this);
	}
}