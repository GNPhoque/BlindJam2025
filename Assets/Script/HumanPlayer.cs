using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HumanPlayer : Player
{
	private PlayerInput playerInput;
	private List<object> currentRumbles;

	protected override void Start()
	{
		base.Start();
		GameManager.instance.AddHumanPlayer(this);
		playerInput = GetComponent<PlayerInput>();
		currentRumbles = new List<object>();
	}

	public void OnSubmit(InputValue state)
	{
		SendTime();
	}

	protected override void SendTime()
	{
		GameManager.instance.SetPlayerTime(this);
	}

	public override void SetTarget(int _target)
	{
	}

	public override IEnumerator Rumble(float duration, AnimationCurve curve = null)
	{
		currentRumbles.Add(new object());
		Gamepad g = Gamepad.all.FirstOrDefault(x => playerInput.devices.Any(d => d.deviceId == x.deviceId));

		if (g == null)
		{
			yield break;
		}

		float power = 1f;

		//If a curve is sent
		if(curve != null)
		{
			float currentDuration = 0f;
			while (currentDuration < duration)
			{
				yield return null;
				power = curve.Evaluate(currentDuration);
				g.SetMotorSpeeds(power, power);
				currentDuration += Time.deltaTime;
			}
		}
		else
		{
			g.SetMotorSpeeds(power, power);

			yield return new WaitForSeconds(duration);
		}

		if(currentRumbles.Count > 0)
		{
			currentRumbles.RemoveAt(0);
		}
		if (currentRumbles.Count == 0)
		{
			g.SetMotorSpeeds(0, 0);
		}
	}
}