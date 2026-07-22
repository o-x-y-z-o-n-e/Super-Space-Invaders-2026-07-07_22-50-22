using UnityEngine;

public class PlayerWeaponPickup : MonoBehaviour, ILootDrop {
    
	[SerializeField] private int weaponIndex;
	[Space]
	[SerializeField] private AudioClipRef pickupSound;

	public void Interact(PlayerShip ship) {
		if(ship.CurrentWeaponIndex == weaponIndex) {
			ship.CurrentWeapon.SetLevel(ship.CurrentWeapon.Level + 1);
		} else {
			ship.SetWeapon(weaponIndex);
		}
		ship.Audio.PlayOneShot(pickupSound.Clip, pickupSound.Volume);
		Destroy(gameObject);
	}
	
}
