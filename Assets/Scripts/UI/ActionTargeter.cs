using UnityEngine;
using UnityEngine.EventSystems;

public class ActionTargeter : MonoBehaviour {
	ActionButton armedAction = null;
	TilemapVisuals tilemapVisuals;
	TileTracker tileTracker;
	Transform errorContainer;

	void Start() {
		tilemapVisuals = GameObject.FindObjectOfType<TilemapVisuals>();
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
		errorContainer = new GameObject().transform;
		errorContainer.parent = this.transform;
		errorContainer.name = "Error Container";
	}

	// when the day starts, instantiate action buttons
	// get them from action sources (blueprint, keep)
	// actionbuttonsource needs to be a class that instantiates an action button

	public void OnDayStart() {

	}

	public void SetArmedAction(ActionButton actionButton) {
		armedAction = actionButton;
	}

	public ActionButton GetArmedAction() {
		return armedAction;
	}

	public bool IsArmed() {
		return GetArmedAction() != null;
	}

	public void ClearArmedAction() {
		armedAction.Disarm();
		armedAction = null;
		tilemapVisuals.OnActionDisarm();
	}

	void Update() {
		if (Input.GetMouseButtonDown(1)) {
			// right click to clear the current armed action
			ClearArmedAction();
		} else if (Input.GetMouseButtonDown(0)) {
			if (!IsArmed()) return;
			if (EventSystem.current.IsPointerOverGameObject()) return;
			
			ApplyArmedAction();
		}
	}

	void ApplyArmedAction() {
		tilemapVisuals.ClearTilePreview();
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPos.z = 0;
		tilemapVisuals.ClearTilePreview();
		Vector3Int boardPosition = tileTracker.WorldToBoard(mouseWorldPos);
		if (armedAction.TryToApplyAction(boardPosition)) {
			ClearArmedAction();
		}
	}

	public Transform GetErrorContainer() {
		return errorContainer;
	}
}
