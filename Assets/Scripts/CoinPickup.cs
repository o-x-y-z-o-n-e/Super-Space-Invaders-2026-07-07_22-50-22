using UnityEngine;

public class CoinPickup : MonoBehaviour, ILootDrop {

	[SerializeField] private int amount;
	[Space]
	[SerializeField] private AudioClipRef pickupSound;


	public void Interact(PlayerShip ship) {
		Core.Level.AddScorePoints(amount);
		ship.Audio.PlayOneShot(pickupSound.Clip, pickupSound.Volume);
		Destroy(gameObject);
	}
	
}