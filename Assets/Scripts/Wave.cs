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
	
	public void Spawn(int groupIndex) {
		if(groupIndex >= splines.Splines.Count) return;
		if(groupIndex >= groups.Count) return;
		groups[groupIndex].Spawn(splines.Splines[groupIndex]);
	}
	
}

[System.Serializable]
public class WaveGroup {

	public float Time => time;
	
	[SerializeField] private float time;
	[SerializeField] private List<Enemy> prefabs;

	private List<Enemy> instances;

	public void Spawn(Spline spline) {
		instances = new();
		float pathLength = spline.GetLength();
		for(int i = 0; i < prefabs.Count; i++) {
			float distance = Mathf.Max((pathLength - i * 2.0F) / Mathf.Max(pathLength, Mathf.Epsilon), 0.0F);
			spline.Evaluate(distance, out float3 position, out float3 tangent, out float3 upVector);
			Enemy instance = GameObject.Instantiate(prefabs[i]);
			instance.name = prefabs[i].name;
			instance.transform.position = position;
			instances.Add(instance);
		}
	}
	
}

public enum WaveType {
	PathToFormation,
	PathToOffScreen,
}