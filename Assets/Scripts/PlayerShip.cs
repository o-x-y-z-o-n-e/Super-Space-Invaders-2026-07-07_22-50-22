using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D), typeof(AudioSource))]
public class PlayerShip : SpaceShip, IDamageable {

	private const float SHIELD_FADE_TIME = 0.5F;
	
	public bool IsDead => health <= 0.0F;
	
	[SerializeField] private float acceleration;
	[Space]
	[SerializeField] private Projectile projectilePrefab;
	[SerializeField] private Transform[] projectileSpawns;
	[Space]
	[SerializeField] private float attackInterval;
	[Space]
	[SerializeField] private SpriteRenderer shieldRenderer;
    
	private SpriteRenderer renderer;
	private CircleCollider2D collider;
	private AudioSource audio;
	private Camera camera;

	private float attackCooldown;
	private int projectileSpawnIndex;

	private float health;

	private bool intro;
	private float introTimer;

	private bool outro;
	private float outroTimer;
	
	private float deathTimer;

	private float shieldTimer;

	private Vector2 extraVelocity;
	private bool lockVerticalMovementInput;

	private static List<Collider2D> lootPickupBuffer = new();

	protected override void Awake() {
		base.Awake();
		renderer = GetComponentInChildren<SpriteRenderer>();
		collider = GetComponent<CircleCollider2D>();
		audio = GetComponent<AudioSource>();
	}

	protected override void Start() {
		base.Start();
		camera = Camera.main;
		exhaustRenderer.gameObject.SetActive(true);
		leftTrailRenderer.gameObject.SetActive(true);
		rightTrailRenderer.gameObject.SetActive(true);
		health = 1.0F;
		intro = true;
		extraVelocity = Vector2.zero;
		transform.position = new Vector3(0, -11, 0);
	}

	protected override void Update() {
		if(Core.SuspendGameLoop) return;
		base.Update();

		Shield();
		Death();
		Intro();
		Outro();
		Movement();
		Shooting();
		Looting();
	}

	public void BeginOutro() {
		outro = true;
	}

	private void Shield() {
		if(shieldTimer > 0.0F) {
			shieldTimer -= Time.deltaTime;
			float alpha = Mathf.Clamp01(shieldTimer / SHIELD_FADE_TIME);
			shieldRenderer.color = new Color(shieldRenderer.color.r, shieldRenderer.color.g, shieldRenderer.color.b, alpha);
			if(shieldTimer <= 0.0F) {
				shieldTimer = 0.0F;
				shieldRenderer.gameObject.SetActive(false);
			}
		}
	}

	public void ActiveShield(float time) {
		shieldTimer = time;
		shieldRenderer.color = new Color(shieldRenderer.color.r, shieldRenderer.color.g, shieldRenderer.color.b, 1);
		shieldRenderer.gameObject.SetActive(true);
	}

	private void Death() {
		if(deathTimer > 0.0F) {
			deathTimer -= Time.deltaTime;
			if(deathTimer <= 0.0F) {
				deathTimer = 0.0F;
				if(Core.Game.PlayerLives > 0) {
					Core.Game.PlayerLives--;
					introTimer = 0.0F;
					health = 1.0F;
					intro = true;
					ActiveShield(4.0F);
				} else {
					Core.Game.End(false);
				}
			}
		}
	}

	private void Intro() {
		if(!intro || outro) return;
		//detectVelocity = true;
		lockVerticalMovementInput = true;
		introTimer += Time.deltaTime;
		float length = 1.0F;
		float deccTime = 0.5F;
		float t = Mathf.Clamp01((introTimer - (length - deccTime)) / deccTime);
		extraVelocity = Vector2.Lerp(new Vector2(0, speed), Vector2.zero, t);
		if(t == 1.0F) {
			intro = false;
			detectVelocity = false;
			extraVelocity = Vector2.zero;
			lockVerticalMovementInput = false;
		}
	}

	private void Outro() {
		if(!outro || intro) return;
		//detectVelocity = true;
		lockVerticalMovementInput = true;
		outroTimer += Time.deltaTime;
		float accTime = 1.0F;
		extraVelocity = Vector2.Lerp(Vector2.zero, new Vector2(0, speed * 1.5F), Mathf.Clamp01(outroTimer / accTime));
		if(transform.position.y > 12) {
			outro = false;
			detectVelocity = false;
			extraVelocity = Vector2.zero;
			gameObject.SetActive(false);
			Core.Gui.Transition(Core.Game.NextLevel);
		}
	}

	private void Movement() {
		if(IsDead) return;

		float x = Input.GetAxisRaw("Horizontal");// + Input.GetAxisRaw("Mouse X") * 20;
		float y = Input.GetAxisRaw("Vertical");// + Input.GetAxisRaw("Mouse Y") * 20;
		x = Mathf.Clamp(x, -1, 1);
		y = Mathf.Clamp(y, -1, 1);

		if(lockVerticalMovementInput) {
			y = 0.0F;
		}
		
		Vector2 size = renderer.sprite.bounds.size;
		
		Vector2 targetVelocity = new Vector2(x, y) * speed;
		
		velocity = Vector2.MoveTowards(velocity, targetVelocity, acceleration * Time.deltaTime);

		Vector2 p = transform.position;
		p += velocity * Time.deltaTime;
		p += extraVelocity * Time.deltaTime;
		transform.position = p;

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
		if(!lockVerticalMovementInput) {
			if(transform.position.y < -cameraSizeY + bodySizeY) {
				transform.position = new Vector3(transform.position.x, -cameraSizeY + bodySizeY, transform.position.z);
				velocity.y = 0.0F;
			}
			if(transform.position.y > cameraSizeY - bodySizeY) {
				transform.position = new Vector3(transform.position.x, cameraSizeY - bodySizeY, transform.position.z);
				velocity.y = 0.0F;
			}
		}
	}

	private void Shooting() {
		if(intro) return;
		if(IsDead) return;
		
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
		if(shieldTimer > 0.0F) return false;
		
		health = Mathf.Max(health - amount, 0.0F);
		
		if(health == 0.0F) {
			Instantiate(explosionPrefab, transform.position, transform.rotation);
			transform.position = new Vector3(0, -11, 0);
			deathTimer = 1.0F;
		}

		return health == 0.0F;
	}

	private void Looting() {
		if(intro) return;
		if(IsDead) return;
		
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
