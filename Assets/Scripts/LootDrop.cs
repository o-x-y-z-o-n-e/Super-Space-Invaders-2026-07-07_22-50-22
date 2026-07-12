using UnityEngine;

public class LootDrop : MonoBehaviour {
	
	public AudioClipRef PickupSound => pickupSound;
	
	public LootDropType Type => type;
	public int Amount => amount;

	[SerializeField] private LootDropType type;
	[SerializeField] private int amount;
	[Space]
	[SerializeField] private AudioClipRef pickupSound;
	
}

public enum LootDropType {
	Coin,
	Upgrade,
	
}