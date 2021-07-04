using UnityEngine;

public class TargetLerp : MonoBehaviour {
	const float lerpSpeed = 0.2f;

	public GameObject target;

	void Update() {
		if (target) {
			transform.position = Vector3.Lerp(transform.position, target.transform.position, lerpSpeed);
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, target.transform.eulerAngles, lerpSpeed);
		}
	}
}
