using UnityEngine;

public class Enemy : SpaceShip, IDamageable {

	public bool IsDead => health <= 0.0F;

	[Space]
	[SerializeField] private float maxHealth;
	[Space]
	[SerializeField] private Projectile projectilePrefab;
	[SerializeField] private Transform projectileSpawn;
	[SerializeField] private float projectileIntervalMin;
	[SerializeField] private float projectileIntervalMax;
	
	private float health;
	private float attackCooldown;

	private void Start() {
		health = maxHealth;
		attackCooldown = Mathf.Lerp(projectileIntervalMin, projectileIntervalMax, Random.value);
	}

	private void Update() {
		if(Core.SuspendGameLoop) return;
		
		if(attackCooldown > 0.0F) {
			attackCooldown -= Time.deltaTime;
			if(attackCooldown <= 0.0F) {
				attackCooldown = Mathf.Lerp(projectileIntervalMin, projectileIntervalMax, Random.value);
				Projectile p = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);
				p.name = projectilePrefab.name;
				p.SetOwner(this);
			}
		}
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
}
