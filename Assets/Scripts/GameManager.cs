using UnityEngine;

public class GameManager : MonoBehaviour {

	public bool IsPaused;
	
	private bool isPaused;

	private void Update() {
		if(!Core.Level.IsActive) return;
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

	public void TestSession() {
		SetPaused(false);
		
	}

}
