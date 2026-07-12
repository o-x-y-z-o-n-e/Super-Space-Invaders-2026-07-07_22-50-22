using UnityEngine;

[System.Serializable]
public struct AudioClipRef {
	public AudioClip Clip;
	[Range(0, 1)] public float Volume;
}
