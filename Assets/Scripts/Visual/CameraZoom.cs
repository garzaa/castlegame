using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.EventSystems;

public class CameraZoom : MonoBehaviour {
	const int maxMultiplier = 3;
	int currentMultiplier = 1;

	PixelPerfectCamera cam;
	int originalPPU;
	static CameraZoom cameraZoom;

	public AudioResource zoomNoise;
	int multiplierLastFrame;
	
	void Start() {
		cam = GetComponent<PixelPerfectCamera>();
		originalPPU = cam.assetsPPU;
		cameraZoom = this;
		multiplierLastFrame = (int) Input.mouseScrollDelta.y;
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

		if (currentMultiplier != multiplierLastFrame) {
			zoomNoise.PlayFrom(this.gameObject);
		}

		multiplierLastFrame = currentMultiplier;
	}

	public static int GetZoomLevel() {
		return cameraZoom.currentMultiplier;
	}
}
