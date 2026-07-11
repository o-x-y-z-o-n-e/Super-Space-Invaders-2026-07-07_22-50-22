using UnityEngine;

public class LootDrop : MonoBehaviour {
	
	public LootDropType Type => type;
	public int Amount => amount;

	[SerializeField] private LootDropType type;
	[SerializeField] private int amount;

}

public enum LootDropType {
	Coin,
	Upgrade,
	
}