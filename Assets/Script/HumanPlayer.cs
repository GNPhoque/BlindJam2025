using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
	private void Start()
	{
		Inputs.instance.PushedOk += SendTime;
	}

	void OnDestroy()
	{
		Inputs.instance.PushedOk -= SendTime;
	}

	public override void SetTarget(int _target)
	{
	}

	protected override void SendTime()
	{
		GameManager.instance.SetPlayerTime(this);
	}
}