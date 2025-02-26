using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class OpponentAI : Player
{
	[SerializeField] float precision;
	[SerializeField] float stopTime;
	[SerializeField] bool useSerializedPrecision;
	[SerializeField] float precisionTargetCalculationRatio;

	int target;

	IEnumerator StopCounting(float delay)
	{
		yield return new WaitForSeconds(delay);
		SendTime();
	}

	public override void SetTarget(int _target)
	{
		target = _target;

		//TODO : maybe some pondération to avoid perfect time (maybe?)
		if (!useSerializedPrecision)
		{
			precision = target * precisionTargetCalculationRatio;
		}

		stopTime = Random.Range(target - precision, target + precision / 2f);
		print($"AI TARGET : {stopTime} ");
		StartCoroutine(StopCounting(stopTime));
	}

	protected override void SendTime()
	{
		GameManager.instance.SetPlayerTime(this, stopTime);
	}

	public override IEnumerator Rumble(float duration, AnimationCurve curve = null)
	{
		//No controller to rumble for AI players
		yield break;
	}
}
