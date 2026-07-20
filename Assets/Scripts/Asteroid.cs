using System;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	public float Radius => radius;
	
	[SerializeField] private float radius;
	[SerializeField] private float speed;

	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
