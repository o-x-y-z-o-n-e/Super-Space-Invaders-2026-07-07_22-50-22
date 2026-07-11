using UnityEngine;

public class LevelReferences : MonoBehaviour {

	[SerializeField] private Wave[] waves;

	public Wave[] GetWaves() {
		return waves;
	}

}
