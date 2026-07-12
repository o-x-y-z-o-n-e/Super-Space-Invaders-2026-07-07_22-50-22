using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : GuiView {

	private const float TITLE_LABEL_FADE = 0.15F;

	[SerializeField] private Slider healthBar;
	[SerializeField] private TMP_Text scoreText;
	[SerializeField] private TMP_Text livesText;
	[SerializeField] private TMP_Text powerText;
	[SerializeField] private TMP_Text progressNumberLabel;
	[SerializeField] private TMP_Text waveDescriptionLabel;

	private float progressNumberTime;
	private float progressNumberTimer;
	private float waveDescriptionTime;
	private float waveDescriptionTimer;
	
	private void Awake() {
		
	}
	
	public override void OnEscapePressed() {
		Core.Game.SetPaused(true);
		Core.Gui.Open<PauseMenu>(true);
	}
	
	public void SetScore(int points) {
		scoreText.text = points.ToString();
	}

	public void SetLives(int lives) {
		livesText.text = lives.ToString();
	}

	private void Update() {
		if(progressNumberLabel.gameObject.activeSelf) {
			progressNumberTimer += Time.deltaTime;
			float alpha = 1.0F;
			if(progressNumberTimer < TITLE_LABEL_FADE) {
				alpha = Mathf.SmoothStep(0.0F, 1.0F, progressNumberTimer / TITLE_LABEL_FADE);
			} else if(progressNumberTimer > progressNumberTime - TITLE_LABEL_FADE) {
				alpha = Mathf.SmoothStep(1.0F, 0.0F, (progressNumberTimer - (progressNumberTime - TITLE_LABEL_FADE)) / TITLE_LABEL_FADE);
			}
			progressNumberLabel.alpha = alpha;
			if(progressNumberTimer >= progressNumberTime) {
				progressNumberTimer = 0.0F;
				progressNumberLabel.gameObject.SetActive(false);
			}
		}
		if(waveDescriptionLabel.gameObject.activeSelf) {
			waveDescriptionTimer += Time.deltaTime;
			float alpha = 1.0F;
			if(waveDescriptionTimer < TITLE_LABEL_FADE) {
				alpha = Mathf.SmoothStep(0.0F, 1.0F, waveDescriptionTimer / TITLE_LABEL_FADE);
			} else if(waveDescriptionTimer > waveDescriptionTime - TITLE_LABEL_FADE) {
				alpha = Mathf.SmoothStep(1.0F, 0.0F, (waveDescriptionTimer - (waveDescriptionTime - TITLE_LABEL_FADE)) / TITLE_LABEL_FADE);
			}
			waveDescriptionLabel.alpha = alpha;
			if(waveDescriptionTimer >= waveDescriptionTime) {
				waveDescriptionTimer = 0.0F;
				waveDescriptionLabel.gameObject.SetActive(false);
			}
		}
	}

	public void ShowProgressNumberTitle(string text, float time) {
		progressNumberLabel.text = text;
		progressNumberLabel.alpha = 0.0F;
		progressNumberLabel.gameObject.SetActive(true);
		progressNumberTime = time;
		progressNumberTimer = 0.0F;
	}

	public void ShowWaveDescriptionTitle(string text, float time) {
		waveDescriptionLabel.text = text;
		waveDescriptionLabel.alpha = 0.0F;
		waveDescriptionLabel.gameObject.SetActive(true);
		waveDescriptionTime = time;
		waveDescriptionTimer = 0.0F;
	}

}
