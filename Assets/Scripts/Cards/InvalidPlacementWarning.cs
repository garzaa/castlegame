using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InvalidPlacementWarning : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] Text messageContainer;
	#pragma warning restore 0649

	TargetLerp lerp;
	GameObject target;

	public void SetInfo(string message, Vector3 position) {
		if (!lerp) {
			lerp = GetComponent<TargetLerp>();
		}
		if (!target) {
			target = new GameObject();
		}
		messageContainer.text = message;
		target.transform.position = position;
		lerp.target = target;
	}
}
