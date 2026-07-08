using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] private float radius;
	[SerializeField] private float speed;
	[SerializeField] private float damage;
	[SerializeField] private LayerMask layerMask;

	private Vector3 velocity;
	private Vector3 lastPosition;
	private ContactFilter2D filter;

	private Camera camera;
	private object owner;
	
	private static List<RaycastHit2D> collisions = new List<RaycastHit2D>();

	private void Awake() {
		filter = new ContactFilter2D();
		filter.useLayerMask = true;
		filter.layerMask = layerMask;
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
	}

	public void SetOwner(object owner) {
		this.owner = owner;
	}

}
