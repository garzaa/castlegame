using UnityEngine;
using UnityEngine.U2D;

public class CameraZoom : MonoBehaviour {
	const int maxMultiplier = 3;
	int currentMultiplier = 1;

	PixelPerfectCamera cam;
	int originalPPU;
	
	void Start() {
		cam = GetComponent<PixelPerfectCamera>();
		originalPPU = cam.assetsPPU;
	}

	void Update() {
		currentMultiplier += (int) Input.mouseScrollDelta.y;
		if (currentMultiplier > maxMultiplier) currentMultiplier = maxMultiplier;
		if (currentMultiplier < 1) currentMultiplier = 1;
		cam.assetsPPU = originalPPU * currentMultiplier;
	}
}
