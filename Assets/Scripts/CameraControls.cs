using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class CameraControls : MonoBehaviour {
	Vector3 mouseDragStart;
	Vector3 positionOnDragStart;
	float startingZ;
	Tilemap tilemap;

	void Start() {
		tilemap = GetComponentInChildren<Tilemap>();
		startingZ = Camera.main.transform.position.z;
		Vector3 center = tilemap.localBounds.center;
		center.z = startingZ;
		Camera.main.transform.position = center;
	}

	void OnMouseOver() {
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) {
			StartPan();
		}

		if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
			Pan();
		}
	}

	void StartPan() {
		mouseDragStart = Input.mousePosition;
		positionOnDragStart = Camera.main.transform.position;
	}

    void Pan() {
		Vector3 worldDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(mouseDragStart);
		Vector3 targetPos = positionOnDragStart - worldDelta;

		Vector3 tilePos = targetPos;
		tilePos.z = tilemap.transform.position.z;

		if (!tilemap.localBounds.Contains(tilePos)) {
			Vector3 closest = tilemap.localBounds.ClosestPoint(targetPos);
			targetPos.x = closest.x;
			targetPos.y = closest.y;
		} 
		targetPos.z = startingZ;
		Camera.main.transform.position = targetPos;
    }
}
