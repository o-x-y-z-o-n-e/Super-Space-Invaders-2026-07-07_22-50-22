using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class Formation : Wave {

	[SerializeField] private WaveEnemyGroup[] groups;
	[SerializeField] private float pathSpeed;
	[SerializeField] private float oscillateVerticalSpeed;
	[SerializeField] private float oscillateVerticalAmplitude;
	[SerializeField] private float oscillateHorizontalSpeed;
	[SerializeField] private float oscillateHorizontalShift;
    
	private List<EnemyShip> enemies;
	private int nextGroupIndex;

	private float[] groupTimers;

	protected override void Awake() {
		base.Awake();
		enemies = new List<EnemyShip>();
		groupTimers = new float[groups.Length];
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
					foreach(var enemy in groups[nextGroupIndex].Enemies) {
						enemy.gameObject.SetActive(true);
					}
					nextGroupIndex++;
				}
			}
		}

		for(int i = 0; i < nextGroupIndex; i++) {
			float length = splines.Splines[i].GetLength();
			float head = Mathf.Min(groupTimers[i], length);
			float t = Mathf.Clamp01(head / length);
			float slowDownDistance = 3.0F;
			float slowDownBlend = Mathf.Clamp01((head - (length - slowDownDistance)) / slowDownDistance);
			if(t == 1.0F) {
				groupTimers[i] += Time.deltaTime;
				float oscillate = groupTimers[i] - length;
				int j = 0;
				foreach(var enemy in groups[i].Enemies) {
					t = Mathf.Clamp01((length - j * 2.0F) / length);
					splines.Splines[i].Evaluate(t, out float3 position, out float3 tangent, out float3 up);
					position.y += Mathf.Sin((oscillate + j * 0.8F) * oscillateVerticalSpeed) * oscillateVerticalAmplitude;
					position.x += Mathf.Cos(oscillate * oscillateHorizontalSpeed) * oscillateHorizontalShift - oscillateHorizontalShift;
					enemy.transform.position = position;
					enemy.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
					enemy.SetAttackEnabled(true);
					j++;
				}
			} else {
				groupTimers[i] += Time.deltaTime * Mathf.Lerp(pathSpeed, 0.25F, slowDownBlend);
				int j = 0;
				foreach(var enemy in groups[i].Enemies) {
					t = Mathf.Clamp01((head - j * 2.0F) / length);
					splines.Splines[i].Evaluate(t, out float3 position, out float3 tangent, out float3 up);
					position.y = Mathf.Lerp(
						position.y,
						position.y + Mathf.Sin((j * 0.8F) * oscillateVerticalSpeed) * oscillateVerticalAmplitude,
						Mathf.SmoothStep(0.0F, 1.0F, slowDownBlend)
					);
					enemy.transform.position = position;
					enemy.transform.rotation = Quaternion.Slerp(
						Quaternion.LookRotation(Vector3.forward, tangent),
						Quaternion.AngleAxis(180, Vector3.forward),
						Mathf.SmoothStep(0.0F, 1.0F, slowDownBlend)
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