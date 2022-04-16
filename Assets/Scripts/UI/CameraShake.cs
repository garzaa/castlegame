using UnityEngine;

public class CameraShake : MonoBehaviour {
	
	bool shaking = false;
	float shakeAmount;
	float shakeDuration;

	public void TinyShake() {
		Shake(0.02f, 0.05f);
	}

	public void Shake(float amount, float duration) {
		shakeAmount = amount;
		shakeDuration = duration;
	}

	void Update() {
		if (shakeDuration > 0 || shaking) {
			transform.localPosition = OnUnitCircle()*shakeAmount;
			shakeDuration -= Time.unscaledDeltaTime;
		} else {
			shakeDuration = 0f;
			transform.localPosition = Vector3.zero;
		}
	}

	Vector3 OnUnitCircle() {
		float randomAngle = Random.Range(0f, Mathf.PI * 2f);
		return (Vector3) new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle));
	}
}
