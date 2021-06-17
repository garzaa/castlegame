using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraControls : MonoBehaviour {
	Vector3 mouseDragStart;
	Vector3 positionOnDragStart;
	Tilemap tilemap;

	void Start() {
		tilemap = GameObject.FindObjectOfType<Tilemap>();
	}	

    void Update() {
		if (Input.GetMouseButtonDown(0)) {
			mouseDragStart = Input.mousePosition;
			positionOnDragStart = transform.position;
		}

		if (Input.GetMouseButton(0)) {
			Vector3 worldDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(mouseDragStart);
			Vector3 targetPos = positionOnDragStart - worldDelta;

			Vector3 tilePos = targetPos;
			tilePos.z = tilemap.transform.position.z;

			if (tilemap.localBounds.Contains(tilePos/2f)) {
				Vector3 closest = tilemap.localBounds.ClosestPoint(targetPos);
				targetPos.x = closest.x;
				targetPos.y = closest.y;
			} 
			transform.position = targetPos;
		}
    }
}
