using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelsContainer : MonoBehaviour {
	Animator animator;
	SaveUtil saveUtil;
	Levels levels;
	ScrollRect scrollRect;

	#pragma warning disable 0649
	[SerializeField] GameObject levelContainer;
	[SerializeField] GameObject levelButtonTemplate;
	#pragma warning restore 0649


	void Start() {
		animator = GetComponent<Animator>();
		saveUtil = GameObject.FindObjectOfType<SaveUtil>();
		levels = GameObject.FindObjectOfType<Levels>();
		levelButtonTemplate.gameObject.SetActive(false);
		scrollRect = GetComponentInChildren<ScrollRect>();
		PopulateLevels();
	}

	public void PopulateLevels() {
		List<SceneReference> scenes = levels.GetLevels();
		for (int i=0; i<scenes.Count; i++) {
			string sceneName = levels.PathToLevelName(scenes[i].ScenePath);
			string levelName = $"{(i+1).ToString("D2")} - {sceneName}";
			GameObject g = Instantiate(levelButtonTemplate, levelContainer.transform);

			Text[] textObjects = g.GetComponentsInChildren<Text>();

			textObjects[0].text = levelName;

			if (saveUtil.HasLevelSaved(sceneName)) {
				LevelProgress progress = saveUtil.GetLevelProgress(sceneName);
				textObjects[1].text = progress.days.ToString();
				textObjects[2].text = progress.actions.ToString();
			} else {
				textObjects[1].transform.parent.gameObject.SetActive(false);
				textObjects[2].transform.parent.gameObject.SetActive(false);
			}


			// cant be i due to some weird closure bullshit
			g.GetComponent<Button>().onClick.AddListener(() => LoadLevel(g.transform.GetSiblingIndex()-1));
			g.SetActive(true);
		}
		scrollRect.normalizedPosition = Vector2.zero;
	}

		public void LoadLevel(int levelNum) {
		levels.LoadLevel(levelNum);
	}

	public void ToggleLevels() {
		animator.SetBool("Shown", !animator.GetBool("Shown"));
	}
}
