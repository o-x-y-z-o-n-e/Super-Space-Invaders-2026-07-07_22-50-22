using UnityEngine;
using UnityEngine.SceneManagement;

public static class Core {

	public static bool SuspendGameLoop => gameManager.IsPaused && levelManager.IsActive;
	
	public static GameManager Game => gameManager;
	public static LevelManager Level => levelManager;
	public static GuiManager Gui => guiManager;

	private static GameManager gameManager;
	private static LevelManager levelManager;
	private static GuiManager guiManager;
	
	private static bool firstSceneLoad;
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void OnUnityLoaded() {
		gameManager = GameObject.Instantiate(Resources.Load<GameManager>("GameManager"));
		gameManager.name = "GameManager";
		GameObject.DontDestroyOnLoad(gameManager);

		levelManager = GameObject.Instantiate(Resources.Load<LevelManager>("LevelManager"));
		levelManager.name = "LevelManager";
		GameObject.DontDestroyOnLoad(levelManager);

		guiManager = GameObject.Instantiate(Resources.Load<GuiManager>("GuiManager"));
		guiManager.name = "GuiManager";
		GameObject.DontDestroyOnLoad(guiManager);

		firstSceneLoad = true;
	}
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void OnSceneLoaded() {
		// currentSceneContext = GameObject.FindAnyObjectByType<SceneContext>();
		if(firstSceneLoad) {
			firstSceneLoad = false;
			if(SceneManager.GetActiveScene().name == "Lobby") {
				guiManager.Open<MainMenu>(true);
			} else {
				gameManager.TestSession();
			}
		}
	}

}
