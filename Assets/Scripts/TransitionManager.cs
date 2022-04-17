using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class TransitionManager : MonoBehaviour {
	
	float targetVolume = 1f;
	float originalVolume = 1f;
	const float FADE_TIME = 0.5f;
	float elapsedTime;
	float transitionEndTime;

	#pragma warning disable 0649
	[SerializeField] Animator animator;
	#pragma warning restore 0649

	void Start() {
		AudioListener.volume = 0;
		FadeAudio(1);
	}

	void Update() {
		if (Time.time < transitionEndTime) {
			elapsedTime += Time.deltaTime;
			AudioListener.volume = Mathf.Lerp(originalVolume, targetVolume, elapsedTime/FADE_TIME);
		}
	}

	void FadeAudio(float targetVolume) {
		this.targetVolume = targetVolume;
		originalVolume = AudioListener.volume;
		elapsedTime = 0;
		transitionEndTime = Time.time + FADE_TIME;
	}

	public void LoadSceneImmediately(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}

	public void LoadScene(string sceneName) {
		StartCoroutine(LoadAsync(sceneName));
	}

	IEnumerator LoadAsync(string sceneName) {
		animator.SetTrigger("FadeToBlack");
		FadeAudio(0);
		yield return new WaitForSeconds(FADE_TIME);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;

		while (!asyncLoad.isDone) {
			if (asyncLoad.progress >= 0.9f) {
				asyncLoad.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}
