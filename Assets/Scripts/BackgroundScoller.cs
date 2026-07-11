using UnityEngine;

public class BackgroundScoller : MonoBehaviour {
    
	[SerializeField] private float speed;

	private SpriteRenderer[] renderers;

	private void Awake() {
		renderers = GetComponentsInChildren<SpriteRenderer>();
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;
		for(int i = 0; i < renderers.Length; i++) {
			float y = renderers[i].transform.position.y;
			y -= Time.deltaTime * speed;
			if(y < -15.0F) {
				y += 30.0F;
			}
			renderers[i].transform.position = new Vector3(renderers[i].transform.position.x, y, renderers[i].transform.position.z);
		}
	}

}
