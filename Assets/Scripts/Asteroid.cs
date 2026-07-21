using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour, IDamageable {

	private const float IMPACT_TIME = 0.2F;
	private const float IMPACT_SPLIT = 0.25F;
	private const float IMPACT_SCALE = 0.9F;

	public float Radius => circleCollider.radius;
	
	[SerializeField] private float speedMin;
	[SerializeField] private float speedMax;
	[SerializeField] private float spinMin;
	[SerializeField] private float spinMax;
	[Space]
	[SerializeField] private float health;
	[SerializeField] private GameObject explosionPrefab;

	private float speed;
	private float spin;
	
	private ContactFilter2D contactFilter;
	private CircleCollider2D circleCollider;
	
	private static List<Collider2D> overlapBuffer = new();

	private float impactTimer;

	private void Awake() {
		circleCollider = GetComponent<CircleCollider2D>();
		contactFilter = new ContactFilter2D();
		contactFilter.useLayerMask = true;
		contactFilter.layerMask = LayerMask.GetMask("Player");
	}

	private void Start() {
		speed = Random.Range(speedMin, speedMax);
		spin = Random.Range(spinMin, spinMax) * (Random.value > 0.5F ? 1 : -1);
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;
		transform.position += Vector3.down * speed * Time.deltaTime;
		transform.rotation *= Quaternion.Euler(Vector3.forward * spin * Time.deltaTime);
		
		Physics2D.OverlapCircle(transform.position, circleCollider.radius, contactFilter, overlapBuffer);
		for(int i = 0; i < overlapBuffer.Count; i++) {
			if(overlapBuffer[i].TryGetComponent(out PlayerShip player)) {
				player.ApplyDamage(100.0F);
			}
		}

		if(impactTimer > 0.0F) {
			impactTimer = Mathf.Max(0.0F, impactTimer - Time.deltaTime);
			float t = 1.0F - Mathf.Clamp01(impactTimer / IMPACT_TIME);
			float s = 1.0F;
			if(t < IMPACT_SPLIT) {
				t = t / IMPACT_SPLIT;
				t = t * t * t;
				s = Mathf.Lerp(1.0F, IMPACT_SCALE, t);
			} else {
				t = (t - IMPACT_SPLIT) / (1.0F - IMPACT_SPLIT);
				t = 1.0F - (1.0F - t) * (1.0F - t);
				s = Mathf.Lerp(IMPACT_SCALE, 1.0F, t);
			}
			gameObject.transform.localScale = new Vector3(s, s, s);
		}
	}

	public bool ApplyDamage(float amount) {
		impactTimer = IMPACT_TIME;
		health -= amount;
		if(health <= 0.0F) {
			if(explosionPrefab) {
				Instantiate(explosionPrefab, transform.position, transform.rotation);
			}
			Destroy(gameObject);
		}
		return health <= 0.0F;
	}
}
