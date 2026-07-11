using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : GuiView {

	[SerializeField] private Slider healthBar;
	[SerializeField] private TMP_Text scoreText;
	[SerializeField] private TMP_Text livesText;
	[SerializeField] private TMP_Text powerText;
	[SerializeField] private TMP_Text progressNumberLabel;
	[SerializeField] private TMP_Text waveDescriptionLabel;

	private float progressNumberTimer;
	private float waveDescriptionTimer;
	
	private void Awake() {
		
	}
	
	public void SetScore(int points) {
		scoreText.text = points.ToString();
	}

	public void SetLives(int lives) {
		livesText.text = lives.ToString();
	}

	private void Update() {
		if(progressNumberTimer > 0.0F) {
			progressNumberTimer -= Time.deltaTime;
			if(progressNumberTimer <= 0.0F) {
				progressNumberTimer = 0.0F;
				progressNumberLabel.gameObject.SetActive(false);
			}
		}
		if(waveDescriptionTimer > 0.0F) {
			waveDescriptionTimer -= Time.deltaTime;
			if(waveDescriptionTimer <= 0.0F) {
				waveDescriptionTimer = 0.0F;
				waveDescriptionLabel.gameObject.SetActive(false);
			}
		}
	}

	public void ShowProgressNumberTitle(string text, float time) {
		progressNumberLabel.text = text;
		progressNumberLabel.gameObject.SetActive(true);
		progressNumberTimer = time;
	}

	public void ShowWaveDescriptionTitle(string text, float time) {
		waveDescriptionLabel.text = text;
		waveDescriptionLabel.gameObject.SetActive(true);
		waveDescriptionTimer = time;
	}

}
