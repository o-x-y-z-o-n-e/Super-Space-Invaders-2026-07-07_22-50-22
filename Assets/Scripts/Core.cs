using UnityEngine;
using UnityEngine.SceneManagement;

public static class Core {

	public static bool SuspendGameLoop => gameManager.IsPaused && levelManager.IsPlaying;
	
	public static Camera Camera => cameraManager;
	public static GameManager Game => gameManager;
	public static LevelManager Level => levelManager;
	public static GuiManager Gui => guiManager;

	private static ObjectManager objectManager;
	private static Camera cameraManager;
	private static GameManager gameManager;
	private static LevelManager levelManager;
	private static GuiManager guiManager;
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void OnUnityLoaded() {
		objectManager = GameObject.Instantiate(Resources.Load<ObjectManager>("ObjectManager"));
		objectManager.name = "ObjectManager";
		GameObject.DontDestroyOnLoad(objectManager);
		
		cameraManager = GameObject.Instantiate(Resources.Load<Camera>("CameraManager"));
		cameraManager.name = "CameraManager";
		GameObject.DontDestroyOnLoad(cameraManager);
		
		guiManager = GameObject.Instantiate(Resources.Load<GuiManager>("GuiManager"));
		guiManager.name = "GuiManager";
		GameObject.DontDestroyOnLoad(guiManager);
		
		gameManager = GameObject.Instantiate(Resources.Load<GameManager>("GameManager"));
		gameManager.name = "GameManager";
		GameObject.DontDestroyOnLoad(gameManager);

		levelManager = GameObject.Instantiate(Resources.Load<LevelManager>("LevelManager"));
		levelManager.name = "LevelManager";
		GameObject.DontDestroyOnLoad(levelManager);
	}
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void OnFirstSceneLoaded() {
		if(SceneManager.GetActiveScene().name == "Lobby") {
			guiManager.Open<MainMenu>(true);
		} else {
			gameManager.Test();
			levelManager.Test();
		}
	}

}
