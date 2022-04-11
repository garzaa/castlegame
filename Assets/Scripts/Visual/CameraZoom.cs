using UnityEngine;
using UnityEngine.U2D;

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
		currentMultiplier += (int) Input.mouseScrollDelta.y;
		if (currentMultiplier > maxMultiplier) currentMultiplier = maxMultiplier;
		if (currentMultiplier < 1) currentMultiplier = 1;
		cam.assetsPPU = originalPPU * currentMultiplier;
	}

	public static int GetZoomLevel() {
		return cameraZoom.currentMultiplier;
	}
}
