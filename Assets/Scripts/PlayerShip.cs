using UnityEngine;

public class PlayerShip : MonoBehaviour {
	
	[SerializeField] private float speed;
	[SerializeField] private float acceleration;
	[Space]
	[SerializeField] private float sideMovementScale;
	[Space]
	[SerializeField] private Projectile projectilePrefab;
	[SerializeField] private Transform[] projectileSpawns;
	[Space]
	[SerializeField] private float attackInterval;
    
	private SpriteRenderer spriteRenderer;
	private Camera camera;
	
	private Vector3 velocity;

	private float attackCooldown;
	private int projectileSpawnIndex;

	private float health;

	private void Awake() {
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		camera = Camera.main;
		health = 100.0F;
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;
		
		Movement();
		Shooting();
	}

	private void Movement() {
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		
		Vector2 size = spriteRenderer.sprite.bounds.size;
		
		Vector2 targetVelocity = new Vector2(x, y) * speed;
		
		velocity = Vector2.MoveTowards(velocity, targetVelocity, acceleration * Time.deltaTime);
		
		transform.position += velocity * Time.deltaTime;

		float cameraSizeY = camera.orthographicSize;
		float cameraSizeX = cameraSizeY * camera.aspect;
		float bodySizeX = size.x / 2.0F;
		float bodySizeY = size.y / 2.0F;

		if(transform.position.x < -cameraSizeX + bodySizeX) {
			transform.position = new Vector3(-cameraSizeX + bodySizeX, transform.position.y, transform.position.z);
			velocity.x = 0.0F;
		}
		
		if(transform.position.x > cameraSizeX - bodySizeX) {
			transform.position = new Vector3(cameraSizeX - bodySizeX, transform.position.y, transform.position.z);
			velocity.x = 0.0F;
		}
		
		if(transform.position.y < -cameraSizeY + bodySizeY) {
			transform.position = new Vector3(transform.position.x, -cameraSizeY + bodySizeY, transform.position.z);
			velocity.y = 0.0F;
		}
		
		if(transform.position.y > cameraSizeY - bodySizeY) {
			transform.position = new Vector3(transform.position.x, cameraSizeY - bodySizeY, transform.position.z);
			velocity.y = 0.0F;
		}

		spriteRenderer.transform.localScale = new Vector3(
			Mathf.Lerp(1.0F, sideMovementScale, Mathf.Abs(velocity.x) / Mathf.Max(speed, Mathf.Epsilon)),
			1.0F,
			1.0F
		);
	}

	private void Shooting() {
		if(attackCooldown > 0.0F) {
			attackCooldown -= Time.deltaTime;
			if(attackCooldown < 0.0F) attackCooldown = 0.0F;
		}
		
		if(attackCooldown > 0.0F) return;
		if(!Input.GetKey(KeyCode.Space)) return;
		if(projectileSpawns.Length == 0) return;
		
		Transform spawn = projectileSpawns[projectileSpawnIndex];
		Projectile p = Instantiate(projectilePrefab, spawn.position, spawn.rotation);
		p.name = projectilePrefab.name;
		p.SetOwner(this);
		
		projectileSpawnIndex = (projectileSpawnIndex + 1) % projectileSpawns.Length;
		attackCooldown = attackInterval;
	}
	
}
