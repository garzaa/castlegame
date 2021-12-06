using UnityEngine;

public class RandomAnimatorStart : MonoBehaviour {
	void Start() {
		Animator a = GetComponent<Animator>();
		a.Update(Random.Range(0, 2f));
	}
}
