using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class LevelManager : MonoBehaviour {

	public bool IsPlaying => playing;

	[SerializeField] private PlayerShip playerPrefab;
	[SerializeField] private float levelStartWaitTime;
	[SerializeField] private float waveStartWaitTime;
	[SerializeField] private float waveEndWaitTime;

	private bool playing;
	private int score;
	private LevelReferences refs;
	private int currentWaveIndex;
	private int nextGroupIndex;
	private float levelTimer;
	private float waveTimer;
	private float waveFinishTime;

	public void AddScorePoints(int points) {
		score += points;
		Core.Gui.Find<HUD>().SetScore(score);
	}

	public void StartLevel() {
		refs = FindObjectOfType<LevelReferences>();
		score = 0;
		playing = true;
		levelTimer = 0.0F;
		waveTimer = 0.0F;
		waveFinishTime = 0.0F;
		Core.Gui.Find<HUD>().SetScore(0);
		Core.Gui.Find<HUD>().ShowProgressNumberTitle("Level 1", 2.5F);
		SpawnPlayer();
	}

	private Wave GetCurrentWave() {
		if(currentWaveIndex >= 0 && currentWaveIndex < refs.GetWaves().Length) {
			return refs.GetWaves()[currentWaveIndex];
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
		}

		Wave currentWave = GetCurrentWave();
		if(currentWave) {
			waveTimer += Time.deltaTime;
			if(nextGroupIndex < currentWave.GetGroupCount()) {
				if(waveTimer > waveStartWaitTime) {
					if(waveTimer - waveStartWaitTime >= currentWave.GetGroup(nextGroupIndex).Time) {
						currentWave.StartGroup(nextGroupIndex);
						nextGroupIndex++;
					}
				}
			} else if(waveFinishTime == 0.0F) {
				bool allDead = true;
				for(int i = 0; i < currentWave.GetGroupCount(); i++) {
					WaveGroup group = currentWave.GetGroup(i);
					foreach(var enemy in group.Enemies) {
						if(!enemy.IsDead) {
							allDead = false;
							break;
						}
					}
					if(!allDead) break;
				}
				if(allDead) {
					waveFinishTime = waveTimer;
				}
			}

			if(waveFinishTime > 0.0F && waveTimer - waveFinishTime > waveEndWaitTime) {
				if(currentWaveIndex + 1 < refs.GetWaves().Length) {
					NextWave();
				} else {
					// TODO: next level
				}
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
		currentWaveIndex++;
		nextGroupIndex = 0;
		waveTimer = 0.0F;
		waveFinishTime = 0.0F;
		if(currentWaveIndex >= refs.GetWaves().Length) {
			playing = false;
		} else {
			GetCurrentWave().SpawnAll();
			Core.Gui.Find<HUD>().ShowProgressNumberTitle($"Wave {currentWaveIndex+1}", 3.0F);
			Core.Gui.Find<HUD>().ShowWaveDescriptionTitle(GetCurrentWave().Description, 3.0F);
		}
	}

}