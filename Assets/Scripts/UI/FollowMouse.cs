using UnityEngine;

public class FollowMouse : MonoBehaviour {
	public Vector2 margin;
	RectTransform rect;

	void Start() {
		rect = GetComponent<RectTransform>();
	}

	void OnEnable() {
		if (!rect) Start();
		Update();
	}
	
	void Update() {
		rect.position = (Vector2) Input.mousePosition + margin;
	}
}
