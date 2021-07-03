using UnityEngine;

public class TargetLerp : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] float lerpSpeed = 0.5f;
	#pragma warning restore 0649

	public GameObject target;

	void Update() {
		if (target) {
			transform.position = Vector3.Lerp(transform.position, target.transform.position, lerpSpeed);
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, target.transform.eulerAngles, lerpSpeed);
		}
	}
}
