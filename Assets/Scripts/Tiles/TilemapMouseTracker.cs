using UnityEngine;

public class TilemapMouseTracker : MonoBehaviour {
	TileTracker tracker;

	void Start() {
		tracker = GameObject.FindObjectOfType<TileTracker>();
	}

	void OnMouseOver() {
		tracker.OnMouseOver(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

	void OnMouseDown() {
		tracker.OnMouseDown();
	}
}
