using UnityEngine;

public class GameWinMenu : GuiView {
    
	public override void OnEscapePressed() {
		Core.Game.ReturnToMainMenu();
	}

	protected override void OnEnable() {
		base.OnEnable();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void OnReturnClicked() {
		Core.Game.ReturnToMainMenu();
	}
	
}
