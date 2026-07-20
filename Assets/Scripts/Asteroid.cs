using UnityEngine;

public class Asteroid : MonoBehaviour {

	public float Radius => radius;
	
	[SerializeField] private float radius;
	[SerializeField] private float speedMin;
	[SerializeField] private float speedMax;
	[SerializeField] private float spinMin;
	[SerializeField] private float spinMax;

	private float speed;
	private float spin;

	private void Start() {
		speed = Random.Range(speedMin, speedMax);
		spin = Random.Range(spinMin, spinMax) * (Random.value > 0.5F ? 1 : -1);
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;
		transform.position += Vector3.down * speed * Time.deltaTime;
		transform.rotation *= Quaternion.Euler(Vector3.forward * spin * Time.deltaTime);
	}
	
	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
