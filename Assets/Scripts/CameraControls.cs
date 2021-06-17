using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraControls : MonoBehaviour {
	Vector3 mouseDragStart;
	Vector3 positionOnDragStart;
	Tilemap tilemap;

	void Start() {
		tilemap = GetComponentInChildren<Tilemap>();
	}

	void OnMouseDown() {
		mouseDragStart = Input.mousePosition;
		positionOnDragStart = Camera.main.transform.position;
	}

    void OnMouseDrag() {
		Vector3 worldDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(mouseDragStart);
		Vector3 targetPos = positionOnDragStart - worldDelta;

		Vector3 tilePos = targetPos;
		tilePos.z = tilemap.transform.position.z;

		if (!tilemap.localBounds.Contains(tilePos)) {
			Vector3 closest = tilemap.localBounds.ClosestPoint(targetPos);
			targetPos.x = closest.x;
			targetPos.y = closest.y;
		} 
		Camera.main.transform.position = targetPos;
    }
}
