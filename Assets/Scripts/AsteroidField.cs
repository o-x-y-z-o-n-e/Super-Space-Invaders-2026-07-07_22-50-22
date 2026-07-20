using UnityEngine;

public class AsteroidField : Wave {
    
	[SerializeField] private float duration;
	[SerializeField] private float spawnStopBeforeEnd;
	[SerializeField] private float spawnIntervalMin;
	[SerializeField] private float spawnIntervalMax;
	[Space]
	[SerializeField] private Asteroid[] prefabs;

	private float spawnTimer;

	protected override void Start() {
		base.Start();
		spawnTimer = 0.0F;
	}

	protected override void Update() {
		base.Update();

		spawnTimer -= Time.deltaTime;
		if(spawnTimer <= 0.0F && timer < duration - spawnStopBeforeEnd) {
			spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
			
			int prefabIndex = Random.Range(0, prefabs.Length);
			Asteroid prefab = prefabs[prefabIndex];
			if(prefab) {
				float cameraSizeY = Core.Camera.orthographicSize;
				float cameraSizeX = cameraSizeY * Core.Camera.aspect;
				float x = Random.Range(-cameraSizeX, cameraSizeX);
				Asteroid asteroid = Instantiate(prefab);
				asteroid.name = prefab.name;
				asteroid.transform.position = new Vector3(x, cameraSizeY + asteroid.Radius + 1.0F, 0.0F);
			}
		}
	}

	public override bool IsFinished() {
		return timer >= duration;
	}
}
