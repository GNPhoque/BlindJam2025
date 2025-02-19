using UnityEngine.InputSystem;

public class HumanPlayer : Player
{
	private void Start()
	{
		SetName(GameManager.instance.GetName());
		GameManager.instance.AddHumanPlayer(this);
		AudioManager.instance.PlayRandomSfx(joinSounds);
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
}