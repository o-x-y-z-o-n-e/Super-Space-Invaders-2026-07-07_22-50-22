using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectManager : MonoBehaviour {
	
	[SerializeField] private List<Object> pooledPrefabs;

	private Dictionary<EntityId, ObjectPool> pools;

	private void Awake() {
		pools = new();
		foreach(var obj in pooledPrefabs) {
			if(!pools.ContainsKey(obj.GetEntityId())) {
				GameObject container = new GameObject($"Pool: [{obj.name}]");
				container.transform.SetParent(transform);
				pools.Add(obj.GetEntityId(), new ObjectPool(obj, container));
			}
		}
	}
	
	public T Spawn<T>(T prefab) where T : Object {
		if(pools.TryGetValue(prefab.GetEntityId(), out var pool) && pool.Count > 0) {
			//
		}
		
		T instance = Instantiate(prefab);
		instance.name = prefab.name;
		return instance;
	}
	
}

public class ObjectPool {
	
	public int Count => queue.Count;
	
	public Object Prefab => prefab;
	public GameObject Container => container;

	private Object prefab;
	private GameObject container;
	private Queue<Object> queue;

	public ObjectPool(Object prefab, GameObject container) {
		this.prefab = prefab;
		this.container = container;
		this.queue = new Queue<Object>();
	}

}