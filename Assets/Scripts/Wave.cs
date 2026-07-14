using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

[RequireComponent(typeof(SplineContainer))]
public class Wave : MonoBehaviour {
	
	public WaveType Type => type;
	public string Description => description;

	[SerializeField] private WaveType type;
	[SerializeField] private string description;
	[SerializeField] private List<WaveGroup> groups;

	private SplineContainer splines;

	private void Awake() {
		splines = GetComponent<SplineContainer>();
	}

	public int GetGroupCount() {
		return groups.Count;
	}
	
	public WaveGroup GetGroup(int index) {
		return groups[index];
	}

	private void OnValidate() {
		// TODO: reorder groups, based on timestamp
	}
	
	public void StartGroup(int groupIndex) {
		if(groupIndex >= splines.Splines.Count) return;
		if(groupIndex >= groups.Count) return;
		groups[groupIndex].StartAll(splines.Splines[groupIndex]);
	}

	public void SpawnAll() {
		foreach(var group in groups) {
			group.SpawnAll();
		}
	}

	public void DestroyAll() {
		foreach(var group in groups) {
			group.DestroyAll();
		}
	}
	
}

[System.Serializable]
public class WaveGroup {

	public float Time => time;
	
	public IEnumerable<Enemy> Enemies => instances;
	
	[SerializeField] private float time;
	[SerializeField] private List<Enemy> prefabs;

	private List<Enemy> instances;

	public void SpawnAll() {
		instances = new();
		for(int i = 0; i < prefabs.Count; i++) {
			Enemy instance = GameObject.Instantiate(prefabs[i]);
			instance.name = prefabs[i].name;
			instance.gameObject.SetActive(false);
			instances.Add(instance);
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
		for(int i = 0; i < instances.Count; i++) {
			GameObject.Destroy(instances[i].gameObject);
		}
		instances = null;
	}
	
}

public enum WaveType {
	PathToFormation,
	PathToOffScreen,
}