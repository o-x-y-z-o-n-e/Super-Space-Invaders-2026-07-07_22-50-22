using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class Formation : Wave {

	[SerializeField] private WaveEnemyGroup[] groups;
    
	private List<Enemy> enemies;
	private int nextGroupIndex;

	protected override void Awake() {
		base.Awake();
		enemies = new();
	}

	protected override void OnEnable() {
		base.OnEnable();
		foreach(var group in groups) {
			foreach(var enemy in group.SpawnAll()) {
				enemies.Add(enemy);
			}
		}
		nextGroupIndex = 0;
	}

	protected override void OnDisable() {
		base.OnDisable();
		foreach(var group in groups) {
			group.DestroyAll();
		}
	}

	protected override void Update() {
		base.Update();
		if(nextGroupIndex < groups.Length) {
			if(timer > Core.Level.WaveStartWaitTime) {
				if(timer - Core.Level.WaveStartWaitTime >= groups[nextGroupIndex].Time) {
					// groups[nextGroupIndex].StartAll(splines.Splines[nextGroupIndex]);
					foreach(var enemy in groups[nextGroupIndex].Enemies) {
						enemy.gameObject.SetActive(true);
					}
					nextGroupIndex++;
				}
			}
		}

		for(int i = 0; i < nextGroupIndex; i++) {
			float speed = 4.0F;
			float length = splines.Splines[i].GetLength();
			float head = Mathf.Min((timer - groups[i].Time) * speed, length);
			float t = Mathf.Clamp01(head / length);
			float turnDistance = 1.5F;
			float turnBlend = Mathf.Clamp01((head - (length - turnDistance)) / turnDistance);
			if(t == 1.0F) {
				int j = 0;
				foreach(var enemy in groups[i].Enemies) {
					t = Mathf.Clamp01((length - j * 2.0F) / length);
					splines.Splines[i].Evaluate(t, out float3 position, out float3 tangent, out float3 up);
					enemy.transform.position = position;
					enemy.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
					j++;
				}
				// TODO: oscillate in formation
			} else {
				int j = 0;
				foreach(var enemy in groups[i].Enemies) {
					t = Mathf.Clamp01((head - j * 2.0F) / length);
					splines.Splines[i].Evaluate(t, out float3 position, out float3 tangent, out float3 up);
					enemy.transform.position = position;
					enemy.transform.rotation = Quaternion.Slerp(
						Quaternion.LookRotation(Vector3.forward, tangent),
						Quaternion.AngleAxis(180, Vector3.forward),
						turnBlend
					);
					j++;
				}
				// TODO: follow path
			}
		}
	}

	public override bool IsFinished() {
		bool anyAlive = false;
		foreach(var enemy in enemies) {
			if(!enemy.IsDead) {
				anyAlive = true;
				break;
			}
		}
		return !anyAlive;
	}
	

	public int GetGroupCount() {
		return groups.Length;
	}
	
	public WaveEnemyGroup GetGroup(int index) {
		return groups[index];
	}
	
}