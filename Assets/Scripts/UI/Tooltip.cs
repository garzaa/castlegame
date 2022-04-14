using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	#pragma warning disable 0649
	[TextArea]
	[SerializeField] string description;
	#pragma warning restore 0649

	float delay = 0.5f;
	
	Coroutine showTooltipCoroutine;
	GameObject template;

	GameObject tooltip;

	public void Start() {
		tooltip = Instantiate(Resources.Load("RuntimeLoaded/TooltipTemplate") as GameObject, this.transform);
		// avoid interfering with resource text getting
		tooltip.transform.SetAsLastSibling();
		tooltip.name = "TooltipTemplate";
		tooltip.SetActive(false);
		tooltip.GetComponentInChildren<Text>().text = description;
	}

	public void SetDescription(string d) {
		description = d;
	}

	public void OnPointerEnter(PointerEventData d) {
		showTooltipCoroutine = StartCoroutine(ShowTooltip());
	}

	IEnumerator ShowTooltip() {
		yield return new WaitForSeconds(delay);
		tooltip.gameObject.SetActive(true);
	}

	public void OnPointerExit(PointerEventData d) {
		if (showTooltipCoroutine != null) StopCoroutine(showTooltipCoroutine);
		tooltip.gameObject.SetActive(false);
	}
}
