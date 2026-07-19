using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	public float Radius => radius;
	public float Length => length;

	[SerializeField] private float radius;
	[SerializeField] private float length;
	[SerializeField] private float speed;
	[SerializeField] private float damage;
	[SerializeField] private LayerMask layerMask;
	[Space]
	[SerializeField] private float spriteSpin;

	private Vector3 velocity;
	private Vector3 lastPosition;
	private ContactFilter2D filter;
	
	private SpriteRenderer spriteRenderer;

	private Camera camera;
	private object owner;
	
	private static List<RaycastHit2D> collisions = new List<RaycastHit2D>();

	private void Awake() {
		filter = new ContactFilter2D();
		filter.useLayerMask = true;
		filter.layerMask = layerMask;
		
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}
	
	private void Start() {
		velocity = transform.up * speed;
		camera = Camera.main;
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;
		
		transform.position += velocity * Time.deltaTime;
		
		Vector3 delta = transform.position - lastPosition;

		Physics2D.CircleCast(lastPosition, radius, delta.normalized, filter, collisions, delta.magnitude);

		for(int i = 0; i < collisions.Count; i++) {
			if(collisions[i].collider.TryGetComponent(out IDamageable damageable)) {
				damageable.ApplyDamage(damage);
				Destroy(gameObject);
			}
		}
			
		lastPosition = transform.position;
		
		float cameraSizeY = camera.orthographicSize;
		float cameraSizeX = cameraSizeY * camera.aspect;
		
		if(
			transform.position.x < -cameraSizeX - radius ||
			transform.position.x > cameraSizeX + radius ||
			transform.position.y < -cameraSizeY - radius ||
			transform.position.y > cameraSizeY + radius
			) {
			Destroy(gameObject);
		}

		if(spriteSpin != 0.0F) {
			spriteRenderer.transform.Rotate(Vector3.forward, spriteSpin * Time.deltaTime);
		}
	}

	public void SetOwner(object owner) {
		this.owner = owner;
	}

	private void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, radius);
		Gizmos.DrawLine(transform.position, transform.position - transform.up * length);
	}

}
