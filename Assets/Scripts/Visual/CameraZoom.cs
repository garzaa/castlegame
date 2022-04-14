using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.EventSystems;

public class CameraZoom : MonoBehaviour {
	const int maxMultiplier = 3;
	int currentMultiplier = 1;

	PixelPerfectCamera cam;
	int originalPPU;
	static CameraZoom cameraZoom;
	
	void Start() {
		cam = GetComponent<PixelPerfectCamera>();
		originalPPU = cam.assetsPPU;
		cameraZoom = this;
	}

	void Update() {
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		var view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
     	var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;

		if (isOutside) return;

		currentMultiplier += (int) Input.mouseScrollDelta.y;
		if (currentMultiplier > maxMultiplier) currentMultiplier = maxMultiplier;
		if (currentMultiplier < 1) currentMultiplier = 1;
		cam.assetsPPU = originalPPU * currentMultiplier;
	}

	public static int GetZoomLevel() {
		return cameraZoom.currentMultiplier;
	}
}
