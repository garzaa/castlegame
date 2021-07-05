using UnityEngine;
using System;

public class TargetLerp : MonoBehaviour {
	const float lerpSpeed = 0.2f;

	GameObject target;
	public Action lerpComplete;


	void Update() {
		if (target) {
			transform.position = Vector3.Lerp(transform.position, target.transform.position, lerpSpeed);
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, target.transform.eulerAngles, lerpSpeed);

			if (
				lerpComplete != null
				&& Approximately(transform.position, target.transform.position)
				&& Approximately(transform.rotation.eulerAngles, target.transform.eulerAngles)
			) {
				Debug.Log("firing lerp complete action");
				lerpComplete.Invoke();
				lerpComplete = null;
			}
		}
	}

	bool Approximately(Vector3 v1, Vector3 v2) {
		return (Vector3.SqrMagnitude(v1) - Vector3.SqrMagnitude(v2)) < float.Epsilon;
	}

	public void SetTarget(GameObject target, Action callback=null) {
		this.target = target;
		if (callback != null) lerpComplete = callback;
	}
}
