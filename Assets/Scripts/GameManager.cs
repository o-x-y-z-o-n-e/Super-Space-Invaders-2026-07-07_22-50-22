using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public bool Playing => playing;
	public bool Finished => finished;

	public int PlayerLives {
		set {
			extraLives = Mathf.Max(value, 0);
			Core.Gui.Find<HUD>().SetLives(extraLives);
		} 
		get => extraLives;
	}

	public bool IsPaused => isPaused;
	
	private bool isPaused;
	private bool playing;
	private bool finished;
	private int totalScore;
	private int extraLives;

	public void SetPaused(bool paused) {
		isPaused = paused;
		// Time.timeScale = paused ? 0 : 1;
		Cursor.visible = paused;
		Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
		if(isPaused) {
			Core.Gui.Open<PauseMenu>(true);
		} else {
			Core.Gui.Open<HUD>(true);
		}
	}

	public void End(bool won) {
		finished = true;
		Core.Gui.Open<GameOverMenu>(true);
	}

	private void Reset() {
		playing = false;
		finished = false;
		totalScore = 0;
		extraLives = 1;
	}

	public void ReturnToMainMenu() {
		if(playing) SceneManager.LoadScene("Lobby");
		Reset();
		Core.Gui.Open<MainMenu>(true);
	}

	public void Test() {
		Reset();
		playing = true;
		Core.Level.Begin();
	}

	public void New() {
		Reset();
		SceneManager.LoadScene("Level1");
		playing = true;
		Core.Level.Begin();
	}

	public void Load() {
		Reset();
		string level = "Level1";
		SceneManager.LoadScene(level);
		playing = true;
		Core.Level.Begin();
	}

}
