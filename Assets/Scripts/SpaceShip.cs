using UnityEngine;

public class SpaceShip : MonoBehaviour {
    
	[SerializeField] protected GameObject explosionPrefab;
	[Space]
	[SerializeField] protected SpriteRenderer bodyRenderer;
	[SerializeField] protected SpriteRenderer exhaustRenderer;
	[SerializeField] protected SpriteRenderer leftTrailRenderer;
	[SerializeField] protected SpriteRenderer rightTrailRenderer;
	[Space]
	[SerializeField] private float sideMovementScale;
	[Space]
	[SerializeField] protected float speed;

	private Vector3 lastPosition;
	protected Vector3 velocity;
	protected bool detectVelocity;

	protected virtual void Awake() {
		
	}
	
	protected virtual void Start() {
		lastPosition = transform.position;
	}
	
	
	protected virtual void Update() {
		
	}

	protected virtual void LateUpdate() {
		if(Core.SuspendGameLoop) return;
		if(detectVelocity) {
			velocity = transform.position - lastPosition;
			velocity /= Mathf.Max(Time.deltaTime, Mathf.Epsilon);
		}
		float strafe = Mathf.Abs(velocity.x) / Mathf.Max(speed, Mathf.Epsilon);
		leftTrailRenderer.color = new Color(1.0F, 1.0F, 1.0F, strafe);
		rightTrailRenderer.color = new Color(1.0F, 1.0F, 1.0F, strafe);
		leftTrailRenderer.transform.localScale = new Vector3(1.0F, Mathf.Lerp(0.2F, 1.0F, strafe), 1.0F);
		rightTrailRenderer.transform.localScale = new Vector3(1.0F, Mathf.Lerp(0.2F, 1.0F, strafe), 1.0F);
		bodyRenderer.transform.localScale = new Vector3(
			Mathf.Lerp(1.0F, sideMovementScale, strafe),
			1.0F,
			1.0F
		);
		
		float thrust = (velocity.y / Mathf.Max(speed, Mathf.Epsilon) + 1.0F) / 2.0F;
		exhaustRenderer.transform.localScale = new Vector3(1.0F, Mathf.Lerp(0.5F, 2.0F, thrust), 1.0F);
		lastPosition = transform.position;
	}
	
}
