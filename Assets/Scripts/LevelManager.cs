using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class LevelManager : MonoBehaviour {

	public bool IsPlaying => playing;

	private bool playing;
	private int score;
	private LevelReferences refs;
	private int currentWaveIndex;
	private int nextGroupIndex;
	private float timer;

	public void AddScorePoints(int points) {
		score += points;
		Core.Gui.Find<HUD>().SetScore(score);
	}

	public void StartLevel() {
		refs = FindObjectOfType<LevelReferences>();
		score = 0;
		playing = true;
		timer = 0.0F;
		Core.Gui.Find<HUD>().SetScore(0);
		Core.Gui.Find<HUD>().ShowProgressNumberTitle("Level 1", 2.0F);
		FirstWave();
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
		
		timer += Time.deltaTime;

		Wave currentWave = GetCurrentWave();
		if(nextGroupIndex < currentWave.GetGroupCount()) {
			if(timer >= currentWave.GetGroup(nextGroupIndex).Time) {
				currentWave.Spawn(nextGroupIndex);
				nextGroupIndex++;
			}
		}
	}

	private void FirstWave() {
		currentWaveIndex = -1;
		NextWave();
	}
	
	private void NextWave() {
		currentWaveIndex++;
		nextGroupIndex = 0;
		if(currentWaveIndex >= refs.GetWaves().Length) {
			playing = false;
		} else {
			Core.Gui.Find<HUD>().ShowProgressNumberTitle($"Wave {currentWaveIndex+1}", 3.0F);
			Core.Gui.Find<HUD>().ShowWaveDescriptionTitle(GetCurrentWave().Description, 3.0F);
		}
	}

}