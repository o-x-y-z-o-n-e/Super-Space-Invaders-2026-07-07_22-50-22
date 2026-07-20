using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

public class Wave : MonoBehaviour {

	public float Timer => timer;
	public string Description => description;

	[SerializeField] private string description;

	protected float timer;
	protected SplineContainer splines;

	protected virtual void Awake() {
		splines = GetComponent<SplineContainer>();
		gameObject.SetActive(false);
	}

	protected virtual void OnEnable() {
		
	}

	protected virtual void OnDisable() {
		
	}

	protected virtual void Start() {
		timer = 0.0F;
	}

	protected virtual void Update() {
		timer += Time.deltaTime;
	}

	public virtual bool IsFinished() {
		return false;
	}
	
}

[System.Serializable]
public class WaveEnemyGroup {

	public float Time => time;
	
	public IEnumerable<Enemy> Enemies => instances;
	
	[SerializeField] private float time;
	[SerializeField] private List<Enemy> prefabs;

	private List<Enemy> instances;

	public IEnumerable<Enemy> SpawnAll() {
		instances = new();
		for(int i = 0; i < prefabs.Count; i++) {
			Enemy instance = GameObject.Instantiate(prefabs[i]);
			instance.name = prefabs[i].name;
			instance.gameObject.SetActive(false);
			instances.Add(instance);
			yield return instance;
		}
	}

	public void StartAll(Spline spline) {
		float pathLength = spline.GetLength();
		for(int i = 0; i < instances.Count; i++) {
			float distance = Mathf.Max((pathLength - i * 2.0F) / Mathf.Max(pathLength, Mathf.Epsilon), 0.0F);
			spline.Evaluate(distance, out float3 position, out float3 tangent, out float3 upVector);
			Enemy instance = instances[i];
			instance.transform.position = position;
			instance.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
			instance.gameObject.SetActive(true);
		}
	}

	public void DestroyAll() {
		if(instances == null) return;
		for(int i = 0; i < instances.Count; i++) {
			if(instances[i] == null) continue;
			GameObject.Destroy(instances[i].gameObject);
		}
		instances = null;
	}
	
}