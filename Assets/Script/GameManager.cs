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
	[SerializeField] int minimumPlayersToStartGame;
	[SerializeField] int minimumTime;
	[SerializeField] int maximumTime;
	[SerializeField] float delayBeforeHideTimer;
	[SerializeField] float playerJoinRumbleDuration;
	[SerializeField] float playerLoseRumbleDuration;
	[SerializeField] float playerWinRumbleDuration;
	[SerializeField] AnimationCurve playerWinRumbleCurve;

	[SerializeField] OpponentAI aiPlayerPrefab;
	[SerializeField] HumanPlayer humanPlayerPrefab;
	[SerializeField] List<Player> players;
	[SerializeField] List<Player> currentPlayers;
	[SerializeField] List<SOPlayerAnimal> playerAnimals;

	int target = 0;
	Stopwatch timer;
	Dictionary<Player, float> leaderboard;
	Dictionary<KeyCode, Player> playerKeys;
	List<SOPlayerAnimal> assignedAnimals;

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
	}

	void Start()
	{
		timeText.text = "00.00";
		timer = new Stopwatch();
		leaderboard = new Dictionary<Player, float>();
		assignedAnimals = new List<SOPlayerAnimal>();
	}

	void Update()
	{
		string time = timer.Elapsed.ToString();
		timeText.text = time;
	}

	public SOPlayerAnimal GetAnimal()
	{
		//If all assigned, any random
		if(assignedAnimals.Count >= playerAnimals.Count)
		{
			return playerAnimals[Random.Range(0, playerAnimals.Count)];
		}

		//Else pick random until a new one is picked
		SOPlayerAnimal animal = playerAnimals[Random.Range(0, playerAnimals.Count)];
		while (assignedAnimals.Contains(animal))
		{
			animal = playerAnimals[Random.Range(0, playerAnimals.Count)];
		}

		assignedAnimals.Add(animal);
		return animal;
	}

	public void AddHumanPlayer(HumanPlayer player)
	{
		if (!players.Contains(player))
		{
			players.Add(player);
		}
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
		//Lobby
		if (!timer.IsRunning)
		{
			AudioManager.instance.PlayRandomSfx(player.animal.buttonSounds);
			StartCoroutine(player.Rumble(playerJoinRumbleDuration));
			return;
		}

		//Only players remaining in game can send time
		if (!leaderboard.ContainsKey(player))
		{
			return;
		}

		//Player has already submitted time
		if (leaderboard[player] != -1f)
		{
			return;
		}

		//For AI
		if(time == -1)
		{
			time = GetCurrentTime();
		}

		leaderboard[player] = time;
		AudioManager.instance.PlayRandomSfx(player.animal.buttonSounds);
		StartCoroutine(player.Rumble(playerJoinRumbleDuration));
	}

	public void StopTimer()
	{
		int eliminated = 0;
		timer.Stop();
		ShowCurrentTime();
		scoreText.text = $"Difference : ";

		for(int i = 0; i < leaderboard.Count; i++)
		{
			Player player = leaderboard.ElementAt(i).Key;
			scoreText.text += "\n";
			if(leaderboard.ElementAt(i).Value == -1)
			{
				scoreText.text += $"{player.name} : DEAD, ";
				currentPlayers.Remove(player);
				eliminated++;
				AudioManager.instance.PlayRandomSfx(player.animal.defeatSounds);
				StartCoroutine(player.Rumble(playerLoseRumbleDuration));
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
					AudioManager.instance.PlayRandomSfx(orderedLeaderboard.ElementAt(i).Key.animal.defeatSounds);
					StartCoroutine(orderedLeaderboard.ElementAt(i).Key.Rumble(playerLoseRumbleDuration));
				}
				else
				{
					break;
				}
			}
		}

		scoreText.text = scoreText.text.Remove(scoreText.text.Length - 2, 2);

		if(currentPlayers.Count == 1)
		{
			//WINNER
			finalText.text = $"{currentPlayers.First().name} wins the game!";
			AudioManager.instance.PlayRandomSfx(currentPlayers.First().animal.victorySounds);
			StartCoroutine(currentPlayers.First().Rumble(playerWinRumbleDuration, playerWinRumbleCurve));
		}
		else if(currentPlayers.Count == 0)
		{
			//ALL LOSERS
			finalText.text = $"Everybody has lost the game!";
			foreach (Player player in players)
			{
				AudioManager.instance.PlayRandomSfx(player.animal.defeatSounds);
			}
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
}
