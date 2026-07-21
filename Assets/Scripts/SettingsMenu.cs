using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : GuiView {

	[SerializeField] private Slider musicVolumeSlider;
	[SerializeField] private Slider sfxVolumeSlider;

	public void OnCloseClicked() {
		Core.Gui.Back();
	}
	
	private void Awake() {
		
	}

	public void OnMusicVolumeChanged() {
		
	}

	public void OnSfxVolumeChanged() {
		
	}

}
