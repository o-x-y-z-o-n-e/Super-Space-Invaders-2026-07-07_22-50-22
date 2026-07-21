using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : SpaceShip, IDamageable {

	public bool IsDead => health <= 0.0F;

	[Space]
	[SerializeField] private float maxHealth;
	[Space]
	[SerializeField] private Projectile projectilePrefab;
	[SerializeField] private Transform projectileSpawn;
	[SerializeField] private float projectileIntervalMin;
	[SerializeField] private float projectileIntervalMax;
	[Space]
	[SerializeField] private int scoreOnDeath;
	
	private float health;
	private float attackCooldown;
	private bool attackEnabled;
	
	private ContactFilter2D contactFilter;
	private CircleCollider2D circleCollider;
	
	private static List<Collider2D> overlapBuffer = new();

	protected override void Awake() {
		base.Awake();
		circleCollider = GetComponent<CircleCollider2D>();
		detectVelocity = true;
		health = maxHealth;
		contactFilter = new ContactFilter2D();
		contactFilter.useLayerMask = true;
		contactFilter.layerMask = LayerMask.GetMask("Player");
	}

	protected override void Start() {
		base.Start();
		attackCooldown = Mathf.Lerp(projectileIntervalMin, projectileIntervalMax, Random.value);
	}

	protected override void Update() {
		if(Core.SuspendGameLoop) return;
		base.Update();
		
		if(attackEnabled && attackCooldown > 0.0F) {
			attackCooldown -= Time.deltaTime;
			if(attackCooldown <= 0.0F) {
				if(!Core.Game.Finished) {
					attackCooldown = Mathf.Lerp(projectileIntervalMin, projectileIntervalMax, Random.value);
					Projectile p = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);
					p.name = projectilePrefab.name;
					p.SetOwner(this);
				}
			}
		}
		
		Physics2D.OverlapCircle(transform.position, circleCollider.radius, contactFilter, overlapBuffer);
		for(int i = 0; i < overlapBuffer.Count; i++) {
			if(overlapBuffer[i].TryGetComponent(out PlayerShip player)) {
				player.ApplyDamage(100.0F);
				// this.ApplyDamage(100.0F);
				break;
			}
		}
	}

	public bool ApplyDamage(float amount) {
		if(IsDead) return false;
		if(amount <= 0.0F) return false;
		
		health = Mathf.Max(health - amount, 0.0F);
		
		if(health == 0.0F) {
			Instantiate(explosionPrefab, transform.position, transform.rotation);
			gameObject.SetActive(false);
			Core.Level.AddScorePoints(scoreOnDeath);
		}

		return health == 0.0F;
	}

	public void SetAttackEnabled(bool enabled) {
		attackEnabled = enabled;
	}
}
