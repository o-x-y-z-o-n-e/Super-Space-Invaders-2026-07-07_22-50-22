using UnityEngine;

public class PauseMenu : GuiView {
	
	public override void OnEscapePressed() {
		Core.Game.SetPaused(false);
		Core.Gui.Open<HUD>(true);
	}
    
	public void OnResumeClicked() {
		Core.Game.SetPaused(false);
	}

	public void OnSettingsClicked() {
		Core.Gui.Open<SettingsMenu>();
	}

	public void OnQuitClicked() {
		// TODO
	}
	
}
