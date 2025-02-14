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
	[SerializeField] TextMeshProUGUI finalText;
	[SerializeField] int minimumTime;
	[SerializeField] int maximumTime;
	[SerializeField] float delayBeforeHideTimer;

	[SerializeField] List<Player> players;

	int target = 0;
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

	IEnumerator EndRound(float delay)
	{
		yield return new WaitForSeconds(delay);
		StopTimer();
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
	}

	public void StopTimer()
	{
		int eliminated = 0;
		timer.Stop();
		ShowCurrentTime();
		scoreText.text = $"Difference : ";

		for(int i = 0; i < leaderboard.Count; i++)
		{
			scoreText.text += "\n";
			if(leaderboard.ElementAt(i).Value == -1)
			{
				scoreText.text += $"{leaderboard.ElementAt(i).Key.name} : DEAD, ";
				players.Remove(leaderboard.ElementAt(i).Key);
				eliminated++;
			}
			else
			{
				scoreText.text += $"{leaderboard.ElementAt(i).Key.name} : {Mathf.Abs(leaderboard.ElementAt(i).Value - target)}, ";
			}
		}

		//If every player has pressed the button before the end
		//Then eliminate the furthest from the target
		if(eliminated == 0)
		{
			List<KeyValuePair<Player, float>> orderedLeaderboard = leaderboard.OrderBy(x => x.Value).ToList();
			float furthestTime = orderedLeaderboard.First().Value;
			for (int i = 0; i < orderedLeaderboard.Count; i++)
			{
				if(orderedLeaderboard.ElementAt(i).Value == furthestTime)
				{
					players.Remove(orderedLeaderboard.ElementAt(i).Key);
				}
				else
				{
					break;
				}
			}
		}

		//Todo : je sais plus comment ca marche mais la il vire que la virgule
		scoreText.text = scoreText.text.Remove(scoreText.text.Length - 2, 2);

		if(players.Count == 1)
		{
			//WINNER
			finalText.text = $"{players.First().name} wins the game!";
		}
		else if(players.Count == 0)
		{
			//ALL LOSERS
			finalText.text = $"Everybody has lost the game!";
		}
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
		StartCoroutine(EndRound(target));

		timer.Restart();
	}
	#endregion
}
