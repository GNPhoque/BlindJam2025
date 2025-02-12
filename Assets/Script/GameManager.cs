using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] TextMeshProUGUI timeText;
	[SerializeField] TextMeshProUGUI targetText;
	[SerializeField] TextMeshProUGUI scoreText;
	[SerializeField] int minimumTime;
	[SerializeField] int maximumTime;
	[SerializeField] float delayBeforeHideTimer;

	[SerializeField] List<Player> players;

	int target = 0;
	int scoredPlayers = 0;
	Stopwatch timer;
	Dictionary<Player, float> leaderboard;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		timeText.text = "00.00";
		timer = new Stopwatch();
		leaderboard = new Dictionary<Player, float>();
	}

	void Update()
	{
		string time = timer.Elapsed.ToString();
		timeText.text = time;
	}

	void ResetLeaderboard()
	{
		scoredPlayers = 0;
		leaderboard.Clear();

		foreach (var player in players)
		{
			leaderboard.Add(player, -1);
		}

	}

	IEnumerator HideCurrentTimeAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		HideCurrentTime();
	}

	public float GetCurrentTime()
	{
		return (int)timer.Elapsed.TotalSeconds + timer.Elapsed.Milliseconds / 1000f;
	}

	public void SetPlayerTime(Player player, float time = -1)
	{
		if (!leaderboard.ContainsKey(player))
		{
			return;
		}

		if(time == -1)
		{
			time = GetCurrentTime();
		}

		leaderboard[player] = time;
		
		if(++scoredPlayers >= leaderboard.Count)
		{
			StopTimer();
		}
	}

	public void StopTimer()
	{
		timer.Stop();
		ShowCurrentTime();
		scoreText.text = $"Difference : ";

		for(int i = 0; i < leaderboard.Count; i++)
		{
			scoreText.text += $"Player {i + 1} : {Mathf.Abs(leaderboard.ElementAt(i).Value - target)}, ";
		}

		//Todo : je sais plus comment ca marche mais la il vire que la virgule
		scoreText.text = scoreText.text.Remove(scoreText.text.Length - 3, 2);
	}

	#region -----UI------
	void HideCurrentTime()
	{
		timeText.gameObject.SetActive(false);
		//TODO : same thing with audio
	}

	void ShowCurrentTime()
	{
		timeText.gameObject.SetActive(true);
	}

	#endregion

	#region -----BUTTONS-----
	public void StartTimer()
	{
		//TODO : add players to player list if not enough present

		ResetLeaderboard();

		target = Random.Range(minimumTime, maximumTime);
		foreach (var player in players)
		{
			player.SetTarget(target);
		}

		targetText.text = target.ToString();

		StartCoroutine(HideCurrentTimeAfterDelay(delayBeforeHideTimer));

		timer.Restart();
	}
	#endregion
}
