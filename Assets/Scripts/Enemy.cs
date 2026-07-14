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
	[Space]
	[SerializeField] private int scoreOnDeath;
	
	private float health;
	private float attackCooldown;

	protected override void Awake() {
		base.Awake();
		detectVelocity = true;
		health = maxHealth;
		attackCooldown = Mathf.Lerp(projectileIntervalMin, projectileIntervalMax, Random.value);
	}

	protected override void Update() {
		if(Core.SuspendGameLoop) return;
		base.Update();
		
		if(attackCooldown > 0.0F) {
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
}
