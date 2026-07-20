using UnityEngine;
using System.Collections.Generic;
using System.Timers;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	
	public float LevelStartWaitTime => levelStartWaitTime;
	public float WaveStartWaitTime => waveStartWaitTime;
	public float WaveEndWaitTime => waveEndWaitTime;
	
	public int Score => score;

	public bool IsPlaying => playing;

	[SerializeField] private PlayerShip playerPrefab;
	[SerializeField] private float levelStartWaitTime;
	[SerializeField] private float waveStartWaitTime;
	[SerializeField] private float waveEndWaitTime;

	private bool playing;
	private int score;
	private Wave[] waves;
	private int currentWaveIndex;
	private float levelTimer;
	private float waveFinishTime;

	public void AddScorePoints(int points) {
		score += points;
		Core.Gui.Find<HUD>().SetScore(score);
	}

	private void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

	public void Test() {
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
		var refs = GameObject.FindAnyObjectByType<LevelReferences>();
		if(!refs) return;
		waves = refs.GetWaves();
		score = 0;
		playing = true;
		levelTimer = 0.0F;
		waveFinishTime = 0.0F;
		Core.Game.SetPaused(false);
		Core.Gui.Find<HUD>().ShowProgressNumberTitle("Level 1", 2.5F);
		SpawnPlayer();
	}

	private void OnSceneUnloaded(Scene scene) {
		if(scene.buildIndex > 0) {
			playing = false;
			score = 0;
			waves = null;
			currentWaveIndex = -1;
			levelTimer = 0.0F;
			waveFinishTime = 0.0F;
		}
	}

	private Wave GetCurrentWave() {
		if(currentWaveIndex >= 0 && currentWaveIndex < waves.Length) {
			return waves[currentWaveIndex];
		} else {
			return null;
		}
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;
		if(!playing) return;
		
		levelTimer += Time.deltaTime;

		if(levelTimer - Time.deltaTime < levelStartWaitTime && levelTimer >= levelStartWaitTime) {
			FirstWave();
			return;
		}

		Wave currentWave = GetCurrentWave();
		if(currentWave) {
			if(waveFinishTime == 0.0F) {
				if(currentWave.IsFinished()) {
					waveFinishTime = currentWave.Timer;
				}
			}
			if(waveFinishTime > 0.0F && currentWave.Timer - waveFinishTime > waveEndWaitTime) {
				NextWave();
			}
		}
	}

	private void SpawnPlayer() {
		PlayerShip player = Instantiate(playerPrefab, new Vector3(0, -11, 0), Quaternion.identity);
		player.name = playerPrefab.name;
	}

	private void FirstWave() {
		currentWaveIndex = -1;
		NextWave();
	}
	
	private void NextWave() {
		if(currentWaveIndex >= 0) {
			waves[currentWaveIndex].gameObject.SetActive(false);
		}
		currentWaveIndex++;
		waveFinishTime = 0.0F;
		if(currentWaveIndex >= waves.Length) {
			playing = false;
			FindAnyObjectByType<PlayerShip>().BeginOutro();
		} else {
			waves[currentWaveIndex].gameObject.SetActive(true);
			Core.Gui.Find<HUD>().ShowProgressNumberTitle($"Wave {currentWaveIndex+1}", 3.0F);
			Core.Gui.Find<HUD>().ShowWaveDescriptionTitle(GetCurrentWave().Description, 3.0F);
		}
	}

}