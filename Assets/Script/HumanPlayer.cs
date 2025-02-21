using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HumanPlayer : Player
{
	private PlayerInput playerInput;

	protected override void Start()
	{
		base.Start();
		GameManager.instance.AddHumanPlayer(this);
		playerInput = GetComponent<PlayerInput>();
	}

	public void OnSubmit(InputValue state)
	{
		SendTime();
		StartCoroutine(Rumble(1f, 1f));
	}

	protected override void SendTime()
	{
		GameManager.instance.SetPlayerTime(this);
	}

	public override void SetTarget(int _target)
	{
	}

	IEnumerator Rumble(float duration, float power)
	{
		Gamepad g = Gamepad.all.FirstOrDefault(x => playerInput.devices.Any(d => d.deviceId == x.deviceId));

		if (g == null)
		{
			yield break;
		}

		g.SetMotorSpeeds(power, power);

		yield return new WaitForSeconds(duration);
		g.SetMotorSpeeds(0, 0);
	}
}