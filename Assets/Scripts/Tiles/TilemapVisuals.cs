using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TilemapVisuals : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameObject legendTemplate;
	[SerializeField] Tile highlightTile;
	[SerializeField] Tilemap highlightTilemapTemplate;
	[SerializeField] Tile homeTile;
	[SerializeField] Tile clickedTile;
	[SerializeField] Canvas doubleScaleCanvas; // this gets overridden with the initialized version of itself...is that ok??
	[SerializeField] TileInfo infoBubbleTemplate;
	[SerializeField] GameObject repairEffect;
	[SerializeField] GameObject decayEffect;
	[SerializeField] Tile[] checkerboardTiles;
	[SerializeField] TileBase targetedIndicatorTile;
	#pragma warning restore 0649

	CommandInput console;
	Tilemap tilemap;
	Vector3Int origin;
	Tilemap highlightTilemap;
	Tilemap targetingTilemap;
	Tilemap iconTilemap;
	Tilemap clickedTilemap;
	Tilemap previewTilemap;
	Tilemap singleIconTilemap;
	Tilemap tectonicsTilemap;
	Vector3 mouseWorldPos;
	Vector3Int gridMousePos;
	TileTracker tracker;
	ActionTargeter actionTargeter;
	Canvas infoBubbleCanvas;

	GameObject currentInfoBubble;
	bool showingTileVisuals;
	Vector3Int currentSelectedGridPosition;
	Vector3Int targetedTile;
	bool started = false;

	void Awake() {
		tilemap = GetComponent<Tilemap>();
		tilemap.CompressBounds();
	}

	void Start() {
		tracker = GameObject.FindObjectOfType<TileTracker>();
		console = GameObject.FindObjectOfType<CommandInput>();
		actionTargeter = GameObject.FindObjectOfType<ActionTargeter>();
		origin = tilemap.cellBounds.min;
		CreateHighlightTilemap();
		CreateTectonicsTilemap();
		CreateTargetingTilemap();
		CreateIconTilemap();
		CreateSelectedTilemap();
		CreatePreviewTilemap();
		CreateSingleIconTilemap();
		CreateCheckerboardTilemap();
		CreateDoubleScaleCanvas();
		CreateInfoBubbleCanvas();

		for (int i=0; i<tilemap.cellBounds.size.x; i++) {
			AddLetterLegend(i);
		}

		for (int i=0; i<tilemap.cellBounds.size.y; i++) {
			AddNumberLegend(i);
		}
		started = true;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			HideInfoBubble();
		}

	}

	void AddLetterLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.right*(idx+1)) + Vector3.down*tilemap.cellSize.y * 1.5f;
		g.GetComponent<Text>().text = TileTracker.letters[idx].ToString().ToUpper();
	}

	void AddNumberLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.up*(idx+1)) + Vector3.down*tilemap.cellSize.y * 1.5f;
		g.GetComponent<Text>().text = (idx+1).ToString();
	}

	void CreateDoubleScaleCanvas() {
		doubleScaleCanvas = Instantiate(doubleScaleCanvas, this.transform);
		doubleScaleCanvas.name = "Double Scale Canvas";
	}

	void CreateInfoBubbleCanvas() {
		infoBubbleCanvas = Instantiate(doubleScaleCanvas, this.transform);
		infoBubbleCanvas.name = "Info Bubble Canvas";
		infoBubbleCanvas.sortingOrder = 90;
	}

	public GameObject GetDoubleScaleCanvas() {
		return doubleScaleCanvas.gameObject;
	}
	
	void CreateHighlightTilemap() {
		highlightTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		highlightTilemap.name = "Highlight";
		highlightTilemap.GetComponent<TilemapRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}
	
	void CreateTargetingTilemap() {
		targetingTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		targetingTilemap.name = "Targeting";
		targetingTilemap.GetComponent<TilemapRenderer>().sortingOrder = highlightTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateIconTilemap() {
		iconTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		iconTilemap.name = "Icons";
		iconTilemap.GetComponent<TilemapRenderer>().sortingOrder = targetingTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateSelectedTilemap() {
		clickedTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		clickedTilemap.name = "Clicked";
		clickedTilemap.GetComponent<TilemapRenderer>().sortingOrder = iconTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreatePreviewTilemap() {
		previewTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		previewTilemap.name = "Preview";
		previewTilemap.GetComponent<TilemapRenderer>().sortingOrder = clickedTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateSingleIconTilemap() {
		singleIconTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		singleIconTilemap.name = "Single Icon";
		singleIconTilemap.GetComponent<TilemapRenderer>().sortingOrder = previewTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateTectonicsTilemap() {
		tectonicsTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		tectonicsTilemap.name = "Tectonics";
		tectonicsTilemap.GetComponent<TilemapRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
		UpdateTectonicsTilemap();
	}

	void UpdateTectonicsTilemap() {
		// iterate through each tile on the edge
		// set the tectonics tile to whatever's on the base
		
		// move through x edge first
		// target pos needs to be down 1
		for (int x=tilemap.cellBounds.min.x; x<tilemap.cellBounds.max.x; x++) {
			Vector3Int tileWorldPos = new Vector3Int(x, tilemap.cellBounds.min.y, 0);
			RuleTile tectonicsTile = tracker.GetTileNoRedirect(tracker.CellToBoard(tileWorldPos)).GetTileType().GetTectonicsTile();
			Vector3Int targetPos = tileWorldPos + Vector3Int.down;
			tectonicsTilemap.SetTile(targetPos, tectonicsTile);
		}

		// then y edge, target pos needs to be left 1
		// flip these tiles left
		for (int y=tilemap.cellBounds.min.y; y<tilemap.cellBounds.max.y; y++) {
			Vector3Int tileWorldPos = new Vector3Int(tilemap.cellBounds.min.x, y, 0);
			RuleTile tectonicsTile = tracker.GetTileNoRedirect(tracker.CellToBoard(tileWorldPos)).GetTileType().GetTectonicsTile();
			Vector3Int targetPos = tileWorldPos + Vector3Int.left;
			tectonicsTilemap.SetTile(targetPos, tectonicsTile);
			Vector3 flipX = new Vector3(-1, 1, 1);
			tectonicsTilemap.SetTransformMatrix(targetPos, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, flipX));
		}

		// and then make the bottom tip
		Vector3Int bottomWorldPos = tilemap.cellBounds.min;
		RuleTile bottomTectonicsTile = tracker.GetTileNoRedirect(tracker.CellToBoard(bottomWorldPos)).GetTileType().GetTectonicsTile();
		Vector3Int bottomTargetPos = bottomWorldPos + Vector3Int.left + Vector3Int.down;
		tectonicsTilemap.SetTile(bottomTargetPos, bottomTectonicsTile);
	}

	void CreateCheckerboardTilemap() {
		Vector3Int padding = new Vector3Int(20, 20, 0);
		Tilemap checkerboard = Instantiate(highlightTilemapTemplate, transform.parent);
		checkerboard.name = "Checkerboard";
		checkerboard.GetComponent<TilemapRenderer>().sortingOrder = -100;
		// then fill it while cycling through the tilemap tiles
		Vector3Int checkerboardOrigin = origin - padding;
		Vector3Int checkerboardEnd = tilemap.cellBounds.max + padding;
		bool incrementAtEndOfRow = (checkerboardEnd.y - checkerboardOrigin.y) % checkerboardTiles.Length == 0;
		int checkerIdx = 0;
		for (int x=checkerboardOrigin.x; x<checkerboardEnd.x; x++) {
			for (int y=checkerboardOrigin.y; y<checkerboardEnd.y; y++) {
				Vector3Int position = new Vector3Int(x, y, 0);
				checkerboard.SetTile(position, checkerboardTiles[checkerIdx]);
				if (++checkerIdx >= checkerboardTiles.Length) checkerIdx = 0;
			}
			// offset at the end of the row to make the checker pattern if necessary
			if ((incrementAtEndOfRow) && ++checkerIdx >= checkerboardTiles.Length) checkerIdx = 0;
		}
	}

	public void HighlightTile(Vector3Int gridPos) {
		if (!tilemap.cellBounds.Contains(gridPos)) {
			return;
		}
		highlightTilemap.SetTile(gridPos, highlightTile);
	}

	public void HighlightTiles(IEnumerable<GameTile> tiles) {
		foreach (GameTile tile in tiles) {
			highlightTilemap.SetTile(tile.gridPosition, highlightTile);
		}
	}

	public void OnActionArm() {
		TargetCursorTile(forceRefresh:true);
	}

	void OnMouseOver() {
		TargetCursorTile();
	}

	void TargetCursorTile(bool forceRefresh=false) {
		if (EventSystem.current.IsPointerOverGameObject()) {
			// don't highlight if paused
			ClearTilePreview();
			highlightTilemap.ClearAllTiles();
			return;
		}

		mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		gridMousePos = highlightTilemap.WorldToCell(mouseWorldPos);
		gridMousePos.z = 0;

		if (!tilemap.cellBounds.Contains(gridMousePos)) {
			OnMouseExit();
		}

		if (!forceRefresh && gridMousePos == targetedTile) {
			return;
		}


		targetedTile = gridMousePos;

		// if an action is armed, run its targeting
		if (actionTargeter.IsArmed()) {
			ActionButton action = actionTargeter.GetArmedAction();
			action.OnTileHover(tilemap.CellToWorld(targetedTile));
			ShowSingleIcon(action.GetActionIcon(), gridMousePos);
		}

		highlightTilemap.ClearAllTiles();
		HighlightTile(gridMousePos);
	}

	void OnMouseExit() {
		if (actionTargeter.IsArmed()) {
			ClearTilePreview();
			highlightTilemap.ClearAllTiles();
			actionTargeter.GetArmedAction().HideActionWarning();
		}
	}

	void OnMouseDown() {
		// this will fire if a card is being held or peeked over the board
		// or if the board is anything but left-clicked
		if (actionTargeter.IsArmed() || EventSystem.current.IsPointerOverGameObject() || !Input.GetMouseButton(0)) {
			return;
		}
		OnTileClick(gridMousePos);
	}

	public void OnActionDisarm() {
		ClearTilePreview();
		highlightTilemap.ClearAllTiles();
	}

	void OnTileClick(Vector3Int gridPos, bool silent=false) {

		GameTile gameTile = tracker.GetTileNoRedirect(tracker.CellToBoard(gridPos));
		if (gameTile == null) {
			HideInfoBubble();
			return;
		}

		// silent if the info bubble is being refreshed on a gameboard change
		// OnTileClick is called twice unfortunately, the code to show the info bubble
		// maybe should be extracted to deal with RefreshInfoBubble down below
		if (currentSelectedGridPosition == gridPos && !silent && !actionTargeter.IsArmed()) {
			HideInfoBubble();
			return;
		}

		if (!actionTargeter.IsArmed()) {
			// refresh the info bubble on an action apply
			// but don't pick the new position
			currentSelectedGridPosition = gridPos;
		}

		if (!silent) gameTile.PlayQuerySound();
		DisplayTileVisuals(gameTile);
		ShowInfoBubble(gameTile);
	}

	public void ShowTilePreview(ScriptableTile tile, bool valid, Vector3 tileWorldPosition) {
		ClearTilePreview();
		previewTilemap.SetTile(previewTilemap.WorldToCell(tileWorldPosition), tile);
	}

	public void HideTilePreview() {
		ClearTilePreview();
	}

	public void ClearTilePreview() {
		// these can be null on immediate startup
		previewTilemap?.ClearAllTiles();
		singleIconTilemap?.ClearAllTiles();
		targetingTilemap?.ClearAllTiles();
	}

	public void ShowTargetedTiles(Clockwork[] clockworks, Vector3 tileWorldPosition) {
		targetingTilemap.ClearAllTiles();
		Vector3Int boardPosition = tracker.WorldToBoard(tileWorldPosition);
		List<GameTile> allTargets = new List<GameTile>();
		// for all those tiles, put the targeted indicator tile on their position
		foreach (Clockwork clockwork in clockworks) {
			allTargets.AddRange(clockwork.GetPossibleTargets(boardPosition, tracker));
		}
		foreach (GameTile tile in allTargets) {
			targetingTilemap.SetTile(tile.gridPosition, targetedIndicatorTile);
		}
	}

	public void ShowTileEffect(GameObject effectUI, Vector3 tileWorldPosition) {
		WorldPointCanvas w = Instantiate(effectUI, doubleScaleCanvas.transform).GetComponent<WorldPointCanvas>();
		w.position = tileWorldPosition;
	}

	public void ShowRepairEffect(GameTile tile) {
		ShowTileEffect(repairEffect, tile.worldPosition);
	}

	public void ShowDecayEffect(GameTile tile) {
		ShowTileEffect(decayEffect, tile.worldPosition);
	}

	void ShowInfoBubble(GameTile tile) {
		if (currentInfoBubble) {
			GameObject.Destroy(currentInfoBubble);
		}

		TileInfo tileInfo = Instantiate(infoBubbleTemplate, infoBubbleCanvas.transform);
		currentInfoBubble = tileInfo.gameObject;
		tileInfo.Initialize(tile);
	}

	public void HideInfoBubble() {
		GameObject.Destroy(currentInfoBubble);
		clickedTilemap.ClearAllTiles();
		highlightTilemap.ClearAllTiles();
		iconTilemap.ClearAllTiles();
		currentSelectedGridPosition = new Vector3Int(999, 999, 999);
	}

	void ShowTileIcon(TileHighlight highlight) {
		if (highlight == null) return;
		foreach (Vector3Int cellPos in highlight.targets) {
			iconTilemap.SetTile(cellPos, highlight.tile);
		}
	}

	public void ShowSingleIcon(Tile iconTile, Vector3Int gridPos) {
		singleIconTilemap.ClearAllTiles();
		singleIconTilemap.SetTile(gridMousePos, iconTile);
	}

	public void DisplayTileVisuals(GameTile tile) {
		clickedTilemap.ClearAllTiles();
		iconTilemap.ClearAllTiles();
		HighlightTile(tile.gridPosition);
		clickedTilemap.SetTile(tile.gridPosition, clickedTile);
		foreach (ITileHighlighter h in tile.GetComponents<ITileHighlighter>()) {
			ShowTileIcon(h.GetHighlight());
		}
		// TODO: for each clockwork tile, add a targeting field for it
	}

	public void OnGameBoardChanged() {
		if (!started) return;
		RefreshInfoBubble();
		UpdateTectonicsTilemap();
	}

	void RefreshInfoBubble() {
		if (currentInfoBubble != null) {
			OnTileClick(currentSelectedGridPosition, silent:true);
		}
	}
}
