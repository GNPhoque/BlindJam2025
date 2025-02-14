using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] List<string> playerNames;

	[SerializeField] TextMeshProUGUI timeText;
	[SerializeField] TextMeshProUGUI targetText;
	[SerializeField] TextMeshProUGUI scoreText;
	[SerializeField] TextMeshProUGUI finalText;
	[SerializeField] int minimumPlayersToStartGame;
	[SerializeField] int minimumTime;
	[SerializeField] int maximumTime;
	[SerializeField] float delayBeforeHideTimer;

	[SerializeField] OpponentAI aiPlayerPrefab;
	[SerializeField] HumanPlayer humanPlayerPrefab;
	[SerializeField] List<Player> players;
	[SerializeField] List<Player> currentPlayers;

	int target = 0;
	Stopwatch timer;
	Dictionary<Player, float> leaderboard;
	Dictionary<KeyCode, Player> playerKeys;

	private void Awake()
	{
		//Singleton
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		//Events
		Inputs.instance.OnPlayerKeyPushed += CreateHumanPlayer;
	}

	void Start()
	{
		timeText.text = "00.00";
		timer = new Stopwatch();
		leaderboard = new Dictionary<Player, float>();
		playerKeys = new Dictionary<KeyCode, Player>();
		foreach (var key in Inputs.instance.humanKeys)
		{
			playerKeys.Add(key, null);
		}
	}

	void Update()
	{
		string time = timer.Elapsed.ToString();
		timeText.text = time;
	}

	void OnDestroy()
	{
		Inputs.instance.OnPlayerKeyPushed -= CreateHumanPlayer;
	}

	void ResetLeaderboard()
	{
		leaderboard.Clear();

		foreach (var player in currentPlayers)
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
				currentPlayers.Remove(leaderboard.ElementAt(i).Key);
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
					currentPlayers.Remove(orderedLeaderboard.ElementAt(i).Key);
				}
				else
				{
					break;
				}
			}
		}

		//Todo : je sais plus comment ca marche mais la il vire que la virgule
		scoreText.text = scoreText.text.Remove(scoreText.text.Length - 2, 2);

		if(currentPlayers.Count == 1)
		{
			//WINNER
			finalText.text = $"{currentPlayers.First().name} wins the game!";
		}
		else if(currentPlayers.Count == 0)
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

		//New game
		if(currentPlayers.Count <= 1)
		{
			currentPlayers.Clear();

			//Only add human players first
			foreach (var player in players)
			{
				if(player is OpponentAI) 
				{ 
					continue; 
				}
				currentPlayers.Add(player);
			}

			//If not enough humans add AIs
			if(currentPlayers.Count < minimumPlayersToStartGame)
			{
				//Starting with already instantiated AIs
				foreach (var player in players)
				{
					if(player is HumanPlayer)
					{
						continue;
					}
					currentPlayers.Add(player);
					if(currentPlayers.Count >= minimumPlayersToStartGame)
					{
						break;
					}
				}

				//If still not enough, instantiate more
				while(currentPlayers.Count < minimumPlayersToStartGame)
				{
					OpponentAI ai = Instantiate(aiPlayerPrefab);
					ai.SetName(playerNames.ElementAt(Random.Range(0, playerNames.Count - 1)));
					players.Add(ai);
					currentPlayers.Add(ai);
				}
			}
		}

		ResetLeaderboard();

		target = Random.Range(minimumTime, maximumTime);
		foreach (var player in currentPlayers)
		{
			player.SetTarget(target);
		}

		targetText.text = target.ToString();

		StartCoroutine(HideCurrentTimeAfterDelay(delayBeforeHideTimer));
		StartCoroutine(EndRound(target));

		timer.Restart();
	}
	#endregion

	#region -----ASSIGN PLAYERS-----
	public void CreateHumanPlayer(KeyCode key)
	{
		if (playerKeys.ContainsKey(key) && playerKeys[key] == null)
		{
			HumanPlayer player = Instantiate(humanPlayerPrefab);
			player.SetName(playerNames.ElementAt(Random.Range(0, playerNames.Count - 1)));
			player.SetKey(key);
			players.Add(player);
			playerKeys[key] = player;
		}
	}
	#endregion
}
