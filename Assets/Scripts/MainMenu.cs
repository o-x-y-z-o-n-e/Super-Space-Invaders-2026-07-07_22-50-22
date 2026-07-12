using UnityEngine;
using TMPro;

public class MainMenu : GuiView {

	[SerializeField] private TMP_Text versionLabel;

	private void Awake() {
		versionLabel.text = $"Version: {Application.version}";
	}
	
	public override void OnEscapePressed() {
		// TODO: confirm quit
	}

	public void OnStartCliked() {
		Core.Gui.Open<StartMenu>();
	}

	public void OnLoadClicked() {
		Core.Gui.Open<LoadMenu>();
	}

	public void OnSettingsClicked() {
		Core.Gui.Open<SettingsMenu>();
	}

	public void OnExitClicked() {
		// TODO: confirm quit
		Application.Quit();
	}
	
}
