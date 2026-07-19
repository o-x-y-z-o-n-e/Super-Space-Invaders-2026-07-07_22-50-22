using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour {

	[SerializeField] protected float heatPerShot;
	[SerializeField] protected float heatCooldownSpeed;
	[Space]
	[SerializeField] protected float shootInterval;
	[Space]
	[SerializeField] protected Projectile projectilePrefab;
	[SerializeField] private Transform[] projectileSpawns;

	public float Heat => heat;
	public bool Cooldown => cooldown;
	
	private PlayerShip player;
	private int projectileSpawnIndex;
	private float shootTimer;
	private int level;
	private float heat;
	private bool cooldown;

	protected virtual void Awake() {
		player = GetComponentInParent<PlayerShip>();
		level = 1;
		heat = 0.0F;
	}

	protected virtual void OnEnable() {
		projectileSpawnIndex = 0;
		shootTimer = 0.0F;
	}

	protected virtual void Update() {
		if(player.InIntro || player.InOutro || player.IsDead) return;
		Core.Gui.Find<HUD>().SetWeaponCooldown(heat);
		if(shootTimer > 0.0F) {
			shootTimer -= Time.deltaTime;
			if(shootTimer < 0.0F) shootTimer = 0.0F;
		}
		if(shootTimer > 0.0F) return;
		if(heat > 0.0F) {
			heat -= Time.deltaTime * heatCooldownSpeed;
			if(heat <= 0.0F) {
				heat = 0.0F;
				cooldown = false;
			}
		}
		if(cooldown) return;
		if(Input.GetKey(KeyCode.Space)) {
			Shoot();
		}
	}

	protected virtual void Shoot() {
		if(projectileSpawns.Length == 0) return;
		Transform spawn = projectileSpawns[projectileSpawnIndex];
		Projectile p = Instantiate(projectilePrefab, spawn.position + spawn.up * projectilePrefab.Length, spawn.rotation);
		p.name = projectilePrefab.name;
		p.SetOwner(this);
		projectileSpawnIndex = (projectileSpawnIndex + 1) % projectileSpawns.Length;
		shootTimer = shootInterval;
		heat += heatPerShot;
		if(heat >= 100.0F) {
			heat = 100.0F;
			cooldown = true;
		}
	}

	public void SetLevel(int level) {
		this.level = Mathf.Max(1, level);
	}

}
