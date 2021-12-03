using UnityEngine;
using UnityEngine.EventSystems;

public class ActionTargeter : MonoBehaviour {
	public AudioResource invalidActionSound;

	ActionButton armedAction = null;
	TilemapVisuals tilemapVisuals;
	TileTracker tileTracker;
	Transform errorContainer;
	Vector3 rightClickStart = Vector3.zero;

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
		if (GetArmedAction() != actionButton) {
			ClearArmedAction();
		}
		armedAction = actionButton;
		tilemapVisuals.OnActionArm();
	}

	public ActionButton GetArmedAction() {
		return armedAction;
	}

	public bool IsArmed() {
		return GetArmedAction() != null;
	}

	public void ClearArmedAction() {
		if (armedAction != null) {
			armedAction.Disarm();
		}
		armedAction = null;
		tilemapVisuals.OnActionDisarm();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (!IsArmed()) return;
			if (EventSystem.current.IsPointerOverGameObject()) return;
			
			ApplyArmedAction();
		} else {
			// right click to clear the current armed action
			if (Input.GetMouseButtonDown(1)) {
				rightClickStart = Input.mousePosition;
			}
			
			if (Input.GetMouseButtonUp(1)) {
				if (Vector3.Distance(rightClickStart, Input.mousePosition) < 10f) {
					ClearArmedAction();
				}
			}
		}
	}

	void ApplyArmedAction() {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPos.z = 0;
		Vector3Int boardPosition = tileTracker.WorldToBoard(mouseWorldPos);
		if (armedAction.TryToApplyAction(boardPosition)) {
			tilemapVisuals.ClearTilePreview();
		} else {
			invalidActionSound.PlayFrom(this.gameObject);
		}
	}

	public Transform GetErrorContainer() {
		return errorContainer;
	}
}
