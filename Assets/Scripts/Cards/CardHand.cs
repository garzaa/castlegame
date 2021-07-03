using UnityEngine;
using UnityEngine.UI;

public class CardHand : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameObject lerpTargetTemplate;
	#pragma warning restore 0649

	public GameObject AddHandTarget() {
		return Instantiate(lerpTargetTemplate, this.transform);
	}

	public void RemoveHandTarget(GameObject target) {
		Destroy(target);
	}
}
