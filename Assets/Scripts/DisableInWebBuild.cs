using UnityEngine;

public class DisableInWebBuild : MonoBehaviour {
	#if UNITY_WEBGL
	void OnEnable() {
		gameObject.SetActive(false);
	}
	#endif
}
