using UnityEngine;

public class GameManager : MonoBehaviour {

	public bool IsPaused => isPaused;
	
	private bool isPaused;
	private int score;
	private int lives;

	private void Update() {
		if(!Core.Level.IsPlaying) return;
		if(Input.GetKeyDown(KeyCode.Escape)) {
			SetPaused(!isPaused);
		}
	}

	private void SetPaused(bool paused) {
		isPaused = paused;
		// Time.timeScale = paused ? 0 : 1;
		if(!Application.isEditor) {
			Cursor.visible = paused;
			Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
		}

		if(isPaused) {
			Core.Gui.Open<PauseMenu>(true);
		} else {
			Core.Gui.Open<HUD>(true);
		}
	}

	private void Reset() {
		score = 0;
		lives = 3;
	}

	public void Test() {
		Reset();
		SetPaused(false);
		Core.Gui.Find<HUD>().SetLives(lives);
		Core.Level.StartLevel();
	}

	public void New() {
		Reset();
		SetPaused(false);
		Core.Gui.Find<HUD>().SetLives(lives);
		Core.Level.StartLevel();
	}

	public void Load() {
		Reset();
		SetPaused(false);
		Core.Gui.Find<HUD>().SetLives(lives);
		Core.Level.StartLevel();
	}

}
