using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D), typeof(AudioSource))]
public class PlayerShip : SpaceShip, IDamageable {
	
	public bool IsDead => health <= 0.0F;
	
	[Space]
	[SerializeField] private float speed;
	[SerializeField] private float acceleration;
	[Space]
	[SerializeField] private float sideMovementScale;
	[Space]
	[SerializeField] private Projectile projectilePrefab;
	[SerializeField] private Transform[] projectileSpawns;
	[Space]
	[SerializeField] private float attackInterval;
    
	private SpriteRenderer renderer;
	private CircleCollider2D collider;
	private AudioSource audio;
	private Camera camera;
	
	private Vector3 velocity;

	private float attackCooldown;
	private int projectileSpawnIndex;

	private float health;

	private bool intro;
	private float introTimer;

	private static List<Collider2D> lootPickupBuffer = new();

	private void Awake() {
		renderer = GetComponentInChildren<SpriteRenderer>();
		collider = GetComponent<CircleCollider2D>();
		audio = GetComponent<AudioSource>();
	}

	private void Start() {
		camera = Camera.main;
		exhaustRenderer.gameObject.SetActive(true);
		leftTrailRenderer.gameObject.SetActive(true);
		rightTrailRenderer.gameObject.SetActive(true);
		health = 10.0F;
		intro = true;
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;

		if(intro) {
			introTimer += Time.deltaTime;
			float t = Mathf.Clamp01(introTimer / 1.25F);
			t = Mathf.SmoothStep(0, 1, t);
			transform.position = Vector3.Lerp(new Vector3(0, -11, 0), new Vector3(0, -6, 0), t);
			exhaustRenderer.transform.localScale = new Vector3(1.0F, Mathf.Lerp(1.0F, 2.0F, Mathf.Sin(t * Mathf.PI)), 1.0F);
			if(t == 1.0F) {
				intro = false;
			} else {
				return;
			}
		}
		
		Movement();
		Shooting();
		Looting();
	}

	private void Movement() {
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		
		Vector2 size = renderer.sprite.bounds.size;
		
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

		float strafe = Mathf.Abs(velocity.x) / Mathf.Max(speed, Mathf.Epsilon);
		leftTrailRenderer.color = new Color(1.0F, 1.0F, 1.0F, strafe);
		rightTrailRenderer.color = new Color(1.0F, 1.0F, 1.0F, strafe);
		leftTrailRenderer.transform.localScale = new Vector3(1.0F, Mathf.Lerp(0.2F, 1.0F, strafe), 1.0F);
		rightTrailRenderer.transform.localScale = new Vector3(1.0F, Mathf.Lerp(0.2F, 1.0F, strafe), 1.0F);
		renderer.transform.localScale = new Vector3(
			Mathf.Lerp(1.0F, sideMovementScale, strafe),
			1.0F,
			1.0F
		);
		
		float thrust = (velocity.y / Mathf.Max(speed, Mathf.Epsilon) + 1.0F) / 2.0F;
		exhaustRenderer.transform.localScale = new Vector3(1.0F, Mathf.Lerp(0.5F, 2.0F, thrust), 1.0F);
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

	public bool ApplyDamage(float amount) {
		if(IsDead) return false;
		if(amount <= 0.0F) return false;
		
		health = Mathf.Max(health - amount, 0.0F);
		
		if(health == 0.0F) {
			Instantiate(explosionPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}

		return health == 0.0F;
	}

	private void Looting() {
		ContactFilter2D filter = new ContactFilter2D();
		filter.useLayerMask = true;
		filter.SetLayerMask(LayerMask.GetMask("LootDrop"));
		Physics2D.OverlapCircle(transform.position, collider.radius, filter, lootPickupBuffer);
		for(int i = 0; i < lootPickupBuffer.Count; i++) {
			if(lootPickupBuffer[i].TryGetComponent(out LootDrop loot)) {
				PickUp(loot);
			}
		}
	}

	private void PickUp(LootDrop loot) {
		switch (loot.Type) {
			case LootDropType.Coin:
				Core.Level.AddScorePoints(loot.Amount);
				break;
			case LootDropType.Upgrade:
				// TODO
				break;
		}
		if(loot.PickupSound.Clip) {
			audio.PlayOneShot(loot.PickupSound.Clip, loot.PickupSound.Volume);
		}
		Destroy(loot.gameObject);
	}

}
